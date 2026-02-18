// diagram-data.builder.ts
import { ShapeOptions, ConnectionOptions } from '@progress/kendo-angular-diagrams';

/** ==== Your interfaces (as provided) ==== */
export interface ScriptTool { id: number; title: string; }

export interface ScriptToolItem {
    scriptToolId: number; // FK to ScriptTool.id
    scriptId: number;     // FK to Script.id (we only use scriptId to join to ScriptItem)
}

export interface ScriptItem {
    scriptId: number;                // join to ScriptToolItem.scriptId (one-to-many)
    scriptFieldId?: number | null;   // join to ScriptField.id (optional)
    scriptName: string;
    parentGroupName: string;         // group name (we'll group by this)
    scriptTypeName: string;          // for label => "Type: ..."
    parentName: string;              // for subLabel
}

export interface ScriptField {
    id: number;
    scriptFieldTypeId: number;
    scriptFieldTypeName: string;     // for label => "Type: ..."
    fieldName: string;
    scriptTableId?: number | null;   // not used here, but kept for completeness
    scriptDetailName: string;        // for subLabel
}

/** DTO sent from your server (we’ll ignore 'scripts') */
export type DiagramInput = {
    rootTool: ScriptTool;
    scriptToolItems: ScriptToolItem[];
    scriptItems: ScriptItem[];
    scriptFields: ScriptField[];
    // scripts?: Script[] // intentionally unused
};

/** ==== internal helpers ==== */
type NodeKind = 'tool' | 'group' | 'script' | 'scriptItem' | 'field';
const palette: Record<NodeKind, { fill: string; stroke: string }> = {
    tool: { fill: '#E0F8F1', stroke: '#A3CFBB' },
    group: { fill: '#F1F3F5', stroke: '#CED4DA' },
    script: { fill: '#E2D9F3', stroke: '#C5B3E6' },
    scriptItem: { fill: '#CFE2FF', stroke: '#9EC5FE' },
    field: { fill: '#FFF3CD', stroke: '#FFE69C' }
};

function makeShape(id: string, label: string, subLabel = '', kind: NodeKind): ShapeOptions {
    const { fill, stroke } = palette[kind];
    return {
        id,
        connectors: [{ name: 'top' }, { name: 'right' }, { name: 'bottom' }, { name: 'left' }],
        dataItem: { label, subLabel, colorScheme: fill, strokeColor: stroke }
    };
}

function connect(from: string, to: string): ConnectionOptions {
    return { from, to, fromConnector: 'right', toConnector: 'left', stroke: { color: 'black', width: 2 } };
}

const slug = (s: string) =>
    (s || 'Ungrouped')
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/^-+|-+$/g, '') || 'ungrouped';

function uniqueId(base: string, used: Set<string>): string {
    let id = base;
    let i = 2;
    while (used.has(id)) id = `${base}-${i++}`;
    used.add(id);
    return id;
}

/** ==== MAIN BUILDER ==== */
/**
 * Hierarchy:
 * Tool → Group(by ScriptItem.parentGroupName) → Script node(per ScriptToolItem.scriptId) → ScriptItem → ScriptField (when scriptFieldId > 0)
 */
export function buildDiagramData(input: DiagramInput): { shapes: ShapeOptions[]; connections: ConnectionOptions[] } {
    const { rootTool, scriptToolItems, scriptItems, scriptFields } = input;

    const shapes: ShapeOptions[] = [];
    const connections: ConnectionOptions[] = [];

    // root
    const rootId = `tool-${rootTool.id}`;
    shapes.push(makeShape(rootId, rootTool.title, '', 'tool'));

    // index: scriptId → ScriptItem[]
    const itemsByScriptId = new Map<number, ScriptItem[]>();
    for (const it of scriptItems) {
        const arr = itemsByScriptId.get(it.scriptId) ?? [];
        arr.push(it);
        itemsByScriptId.set(it.scriptId, arr);
    }

    // which scripts does this tool include?
    const toolScriptIds = new Set(scriptToolItems.map(ti => ti.scriptId));

    // derive groups (by parentGroupName) only from items that belong to the tool
    type GroupBucket = { name: string; scriptIds: Set<number> };
    const groupMap = new Map<string, GroupBucket>();
    for (const [scriptId, items] of itemsByScriptId) {
        if (!toolScriptIds.has(scriptId)) continue;
        // choose the group's display name from the first item's parentGroupName
        const grpName = items[0]?.parentGroupName || 'Ungrouped';
        const bucket = groupMap.get(grpName) ?? { name: grpName, scriptIds: new Set<number>() };
        bucket.scriptIds.add(scriptId);
        groupMap.set(grpName, bucket);
    }

    // sort groups alphabetically; inside each group, sort scripts numerically
    const groups = Array.from(groupMap.values()).sort((a, b) => a.name.localeCompare(b.name));
    const usedIds = new Set<string>();

    for (const g of groups) {
        const groupNodeId = uniqueId(`group-${slug(g.name)}`, usedIds);
        shapes.push(makeShape(groupNodeId, g.name, '', 'group'));
        connections.push(connect(rootId, groupNodeId));

        const scriptIds = Array.from(g.scriptIds).sort((a, b) => a - b);
        for (const scriptId of scriptIds) {
            // Script node (we don’t have Script table; take name from any item of that scriptId)
            const anyItem = (itemsByScriptId.get(scriptId) ?? [])[0];
            const scriptName = anyItem?.scriptName || `Script #${scriptId}`;

            const scriptNodeId = uniqueId(`script-${scriptId}`, usedIds);
            shapes.push(makeShape(scriptNodeId, scriptName, '', 'script'));
            connections.push(connect(groupNodeId, scriptNodeId));

            // ScriptItem nodes (for this scriptId)
            const sis = (itemsByScriptId.get(scriptId) ?? [])
                .slice()
                .sort((a, b) =>
                    (a.scriptTypeName || '').localeCompare(b.scriptTypeName || '') ||
                    (a.parentName || '').localeCompare(b.parentName || '')
                );

            let siIndex = 1;
            for (const si of sis) {
                const siNodeId = uniqueId(`si-${scriptId}-${siIndex++}`, usedIds);
                shapes.push(makeShape(siNodeId, `Type: ${si.scriptTypeName}`, si.parentName ?? '', 'scriptItem'));
                connections.push(connect(scriptNodeId, siNodeId));

                // ScriptField child when scriptFieldId > 0
                const fid = si.scriptFieldId ?? 0;
                if (fid > 0) {
                    const f = scriptFields.find(x => x.id === fid);
                    if (f) {
                        const fNodeId = uniqueId(`sf-${f.id}`, usedIds);
                        shapes.push(makeShape(fNodeId, `Type: ${f.scriptFieldTypeName}`, f.scriptDetailName ?? '', 'field'));
                        connections.push(connect(siNodeId, fNodeId));
                    }
                }
            }
        }
    }

    return { shapes, connections };
}
