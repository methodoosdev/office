// diagram-data.builder.ts
import { ShapeOptions, ConnectionOptions } from '@progress/kendo-angular-diagrams';

/* ========= Your domain (no Script table) ========= */
export interface ScriptTool { id: number; title: string; }

export interface ScriptToolItem {
    id: number;
    scriptToolId: number;
    scriptId: number;              // join to ScriptItem.scriptId
    order?: number | null;
    scriptFieldId?: number | null; // optional parent field
}

export interface ScriptItem {
    id: number;
    scriptId: number;              // join to ScriptToolItem.scriptId
    scriptTypeName: string;        // label: "Type: ..."
    parentName: string;            // subLabel
    scriptName: string;
    scriptFieldId?: number | null; // optional: field link at item level
    parentId?: number | null;      // top-level when 0/null
    order?: number | null;
    parentGroupName?: string;      // optional: if you want to group tools by a name
}

export interface ScriptField {
    id: number;
    scriptFieldTypeName: string;   // label: "Type: ..."
    scriptDetailName: string;      // subLabel
    parentScriptFieldId?: number | null; // optional: hierarchical fields
}

/* ========= Input ========= */
export type DiagramInput = {
    rootTool: ScriptTool;
    scriptToolItems: ScriptToolItem[];
    scriptItems: ScriptItem[];
    scriptFields: ScriptField[];
    // Optional: if you want groups, set useGroups=true and make sure scriptItems have parentGroupName
    useGroups?: boolean;
    childrenPerRow?: number; // default 3
};

/* ========= Visual handoff ========= */
type NodeKind = 'tool' | 'group' | 'script' | 'scriptItem' | 'field' | 'bucket';

const palette: Record<NodeKind, { fill: string; stroke: string }> = {
    tool: { fill: '#E0F8F1', stroke: '#A3CFBB' },
    group: { fill: '#F1F3F5', stroke: '#CED4DA' },
    script: { fill: '#E2D9F3', stroke: '#C5B3E6' },
    scriptItem: { fill: '#CFE2FF', stroke: '#9EC5FE' },
    field: { fill: '#FFF3CD', stroke: '#FFE69C' },
    bucket: { fill: '#ffffff', stroke: '#ffffff' } // not visible (visual hides bucket)
};

function makeShape(id: string, label: string, subLabel = '', kind: NodeKind): ShapeOptions {
    const { fill, stroke } = palette[kind];
    return {
        id,
        connectors: [{ name: 'top' }, { name: 'right' }, { name: 'bottom' }, { name: 'left' }],
        dataItem: { label, subLabel, colorScheme: fill, strokeColor: stroke, isBucket: kind === 'bucket' }
    };
}

function connect(from: string, to: string): ConnectionOptions {
    return { from, to, fromConnector: 'bottom', toConnector: 'top', stroke: { color: 'black', width: 2 } };
}

function chunk<T>(arr: T[], size: number): T[][] {
    const rows: T[][] = [];
    for (let i = 0; i < arr.length; i += size) rows.push(arr.slice(i, i + size));
    return rows;
}

function uniqueId(base: string, used: Set<string>): string {
    let id = base;
    let i = 2;
    while (used.has(id)) id = `${base}-${i++}`;
    used.add(id);
    return id;
}

/**
 * Attach children under a parent with bucket rows:
 * - First `perRow` children connect directly parent→child
 * - Remaining are grouped in chunks of `perRow`
 *   Each chunk is placed under an invisible “bucket” node:
 *     parent → bucketRowN → each child in that row
 */
function connectWithBuckets(
    parentId: string,
    childNodeIds: string[],
    perRow: number,
    shapes: ShapeOptions[],
    connections: ConnectionOptions[],
    usedIds: Set<string>
) {
    if (childNodeIds.length <= perRow) {
        childNodeIds.forEach(cid => connections.push(connect(parentId, cid)));
        return;
    }

    // First row: direct children
    const firstRow = childNodeIds.slice(0, perRow);
    firstRow.forEach(cid => connections.push(connect(parentId, cid)));

    // Remaining rows via bucket(s)
    const rest = childNodeIds.slice(perRow);
    const rows = chunk(rest, perRow);

    rows.forEach((row, idx) => {
        const bucketId = uniqueId(`bucket-${parentId}-${idx + 1}`, usedIds);
        shapes.push(makeShape(bucketId, '', '', 'bucket')); // invisible spacer
        connections.push(connect(parentId, bucketId));
        row.forEach(cid => connections.push(connect(bucketId, cid)));
    });
}

/* ========= MAIN BUILDER =========
   Hierarchy:
   Tool → (optional) Groups → Script (per scriptId) → ScriptItem(s) → Field children
*/
export function buildDiagramData(input: DiagramInput): { shapes: ShapeOptions[]; connections: ConnectionOptions[] } {
    const {
        rootTool,
        scriptToolItems,
        scriptItems,
        scriptFields,
        useGroups = false,
        childrenPerRow = 3
    } = input;

    const shapes: ShapeOptions[] = [];
    const connections: ConnectionOptions[] = [];
    const used = new Set<string>();

    // Root
    const rootId = uniqueId(`tool-${rootTool.id}`, used);
    shapes.push(makeShape(rootId, rootTool.title, '', 'tool'));

    // Index items by scriptId
    const itemsByScriptId = new Map<number, ScriptItem[]>();
    for (const it of scriptItems) {
        const arr = itemsByScriptId.get(it.scriptId) ?? [];
        arr.push(it);
        itemsByScriptId.set(it.scriptId, arr);
    }

    // Optional grouping by parentGroupName (from ScriptItem)
    type GroupBucket = { name: string; scriptIds: Set<number> };
    const toolScriptIds = new Set(scriptToolItems.map(ti => ti.scriptId));

    let groups: GroupBucket[] = [{ name: 'All', scriptIds: new Set<number>() }];
    if (useGroups) {
        const map = new Map<string, GroupBucket>();
        for (const [scriptId, items] of itemsByScriptId) {
            if (!toolScriptIds.has(scriptId)) continue;
            const name = items[0]?.parentGroupName ?? 'Ungrouped';
            const b = map.get(name) ?? { name, scriptIds: new Set<number>() };
            b.scriptIds.add(scriptId);
            map.set(name, b);
        }
        groups = Array.from(map.values()).sort((a, b) => a.name.localeCompare(b.name));
    } else {
        // no grouping: single group "All"
        for (const ti of scriptToolItems) groups[0].scriptIds.add(ti.scriptId);
    }

    for (const g of groups) {
        const parentForScripts = (() => {
            if (!useGroups) return rootId;
            const gid = uniqueId(`group-${g.name.toLowerCase().replace(/\W+/g, '-')}`, used);
            shapes.push(makeShape(gid, g.name, '', 'group'));
            connections.push(connect(rootId, gid));
            return gid;
        })();

        const scriptIds = Array.from(g.scriptIds).sort((a, b) => a - b);
        for (const scriptId of scriptIds) {
            // Script node (label from any item; fallback to #id)
            const anyItem = (itemsByScriptId.get(scriptId) ?? [])[0];
            const scriptName = anyItem?.scriptName ?? `Script #${scriptId}`;
            const scriptNodeId = uniqueId(`script-${scriptId}`, used);
            shapes.push(makeShape(scriptNodeId, scriptName, '', 'script'));
            connections.push(connect(parentForScripts, scriptNodeId));

            // Build ScriptItem nodes
            const topItems = (itemsByScriptId.get(scriptId) ?? [])
                .filter(x => (x.parentId ?? 0) === 0)
                .sort((a, b) =>
                    (a.order ?? 0) - (b.order ?? 0) ||
                    (a.scriptTypeName || '').localeCompare(b.scriptTypeName || '') ||
                    (a.parentName || '').localeCompare(b.parentName || '')
                );

            const scriptItemNodeIds: string[] = [];
            for (const si of topItems) {
                const siId = uniqueId(`si-${si.id}`, used);
                shapes.push(makeShape(siId, `Type: ${si.scriptTypeName}`, si.parentName ?? '', 'scriptItem'));
                scriptItemNodeIds.push(siId);

                // Field children (if any) — gather parent + its children by parentScriptFieldId
                const baseFieldId =
                    (si.scriptFieldId && si.scriptFieldId > 0)
                        ? si.scriptFieldId
                        : (scriptToolItems.find(t => t.scriptId === scriptId)?.scriptFieldId ?? 0);

                const fieldNodeIds: string[] = [];
                if (baseFieldId) {
                    // direct field
                    const rootField = scriptFields.find(f => f.id === baseFieldId);
                    if (rootField) {
                        const fId = uniqueId(`sf-${rootField.id}`, used);
                        shapes.push(makeShape(fId, `Type: ${rootField.scriptFieldTypeName}`, rootField.scriptDetailName ?? '', 'field'));
                        fieldNodeIds.push(fId);
                    }
                    // children fields
                    for (const f of scriptFields) {
                        if (f.parentScriptFieldId === baseFieldId) {
                            const fId = uniqueId(`sf-${f.id}`, used);
                            shapes.push(makeShape(fId, `Type: ${f.scriptFieldTypeName}`, f.scriptDetailName ?? '', 'field'));
                            fieldNodeIds.push(fId);
                        }
                    }

                    // connect ScriptItem → Field nodes with bucket rows
                    connectWithBuckets(siId, fieldNodeIds, childrenPerRow, shapes, connections, used);
                }
            }

            // connect Script → ScriptItem nodes with bucket rows (3 per row default)
            connectWithBuckets(scriptNodeId, scriptItemNodeIds, childrenPerRow, shapes, connections, used);
        }
    }

    return { shapes, connections };
}
