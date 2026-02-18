type FormlyField = any;

interface ScriptItem {
    id: number;
    group: string;
    scriptGroupId: number;
    order: number;
    printed: boolean;
    hasHeader: boolean;
    header: string | null;
    headerCode: string | null;
    headerLeft: string | null;
    headerRight: string | null;
    scriptAlignTypeId: 0 | 1;       // model [left,right]
    scriptGroupAlignTypeId: 0 | 1;  // column (left/right)
    scriptCode: string;
    scriptName: string;
    value: number;
}

type PackDirection = 'ltr' | 'rtl' | 'both' | 'none';
type Side = 'left' | 'right';

type WrapperSpec =
    | string[]
    | ((ctx: { item: ScriptItem; side: Side }) => string[] | undefined);

type ClassSpec =
    | string
    | ((ctx: { item?: ScriptItem; side?: Side }) => string | undefined);

interface BuildOptions {
    packDirection?: PackDirection; // default 'ltr'

    // Wrappers (same as previous message)
    leftItemWrappers?: WrapperSpec;
    rightItemWrappers?: WrapperSpec;
    leftHeaderWrappers?: WrapperSpec;
    rightHeaderWrappers?: WrapperSpec;
    leftItemFieldArrayWrappers?: WrapperSpec;
    rightItemFieldArrayWrappers?: WrapperSpec;

    // ----- NEW: className controls -----
    // Row container (dyad)
    rowClassName?: ClassSpec;                  // appended to row.className
    rowFieldGroupClassName?: ClassSpec;        // appended to row.fieldGroupClassName ('grid')

    // Column containers
    leftColClassName?: ClassSpec;              // appended to 'col-12 md:col-6'
    rightColClassName?: ClassSpec;
    leftColFieldGroupClassName?: ClassSpec;    // appended to 'grid'
    rightColFieldGroupClassName?: ClassSpec;

    // Item (decimals repeat-section) + header (text repeat-section)
    leftItemClassName?: ClassSpec;             // appended to 'col-12'
    rightItemClassName?: ClassSpec;
    leftHeaderClassName?: ClassSpec;           // appended to 'col-12'
    rightHeaderClassName?: ClassSpec;

    // Child items inside the repeat-section (fieldArray)
    leftItemFieldArrayClassName?: ClassSpec;   // appended if provided
    rightItemFieldArrayClassName?: ClassSpec;
}

export function buildFormlyDyadsPackedFromList(
    list: ScriptItem[],
    initialModel: Record<string, any> = {},
    options: BuildOptions = {}
): { fields: FormlyField[]; model: Record<string, any> } {
    const model = { ...initialModel };

    const packDirection: PackDirection = options.packDirection ?? 'ltr';
    const packsLTR = packDirection === 'ltr' || packDirection === 'both';
    const packsRTL = packDirection === 'rtl' || packDirection === 'both';

    const sortByOrder = (items: ScriptItem[]) =>
        [...items].sort((a, b) => a.order - b.order || a.id - b.id);

    const resolveWrappers = (
        spec: WrapperSpec | undefined,
        item: ScriptItem,
        side: Side
    ) => (spec ? (typeof spec === 'function' ? spec({ item, side }) : spec) : undefined);

    const resolveClass = (
        base: string | undefined,
        spec: ClassSpec | undefined,
        ctx: { item?: ScriptItem; side?: Side }
    ): string | undefined => {
        const add = spec ? (typeof spec === 'function' ? spec(ctx) : spec) : '';
        if (!base && !add) return undefined;
        const parts = `${base ?? ''} ${add ?? ''}`.trim().split(/\s+/).filter(Boolean);
        // dedupe while preserving order
        const seen = new Set<string>();
        const merged = parts.filter(c => (seen.has(c) ? false : (seen.add(c), true)));
        return merged.length ? merged.join(' ') : undefined;
    };

    const makeFieldsForItem = (item: ScriptItem, side: Side): FormlyField[] => {
        const key = `key${item.id}`;
        const hdrKey = `hdr${item.id}`;

        // Model defaults
        const leftVal = item.scriptAlignTypeId === 0 ? (item.value ?? 0) : 0;
        const rightVal = item.scriptAlignTypeId === 1 ? (item.value ?? 0) : 0;
        if (!Array.isArray(model[key])) model[key] = [leftVal, rightVal];

        const out: FormlyField[] = [];

        // Header (optional)
        if (item.hasHeader) {
            if (!Array.isArray(model[hdrKey])) {
                model[hdrKey] = [item.headerLeft ?? '', item.headerRight ?? ''];
            }
            const headerWrappers =
                side === 'left'
                    ? resolveWrappers(options.leftHeaderWrappers, item, side)
                    : resolveWrappers(options.rightHeaderWrappers, item, side);

            const headerClass =
                side === 'left'
                    ? resolveClass('col-12', options.leftHeaderClassName, { item, side })
                    : resolveClass('col-12', options.rightHeaderClassName, { item, side });

            const headerField: FormlyField = {
                key: hdrKey,
                type: 'repeat-section',
                className: headerClass,
                props: {
                    left: item.headerCode ?? '',
                    right: item.header ?? '',
                    header: true
                },
                fieldArray: {
                    wrappers: ['simple'], type: 'text' },
            };
            if (headerWrappers) headerField.wrappers = headerWrappers;
            out.push(headerField);
        }

        // Item (decimals)
        const itemWrappers =
            side === 'left'
                ? resolveWrappers(options.leftItemWrappers, item, side)
                : resolveWrappers(options.rightItemWrappers, item, side);

        const itemClass =
            side === 'left'
                ? resolveClass('col-12', options.leftItemClassName, { item, side })
                : resolveClass('col-12', options.rightItemClassName, { item, side });

        const faClass =
            side === 'left'
                ? resolveClass(undefined, options.leftItemFieldArrayClassName, { item, side })
                : resolveClass(undefined, options.rightItemFieldArrayClassName, { item, side });

        const decimalsField: FormlyField = {
            key,
            type: 'repeat-section',
            className: itemClass,
            props: { left: item.scriptCode, right: item.scriptName },
            fieldArray: {
                type: 'decimals',
                wrappers: ['simple'],
                props: { spinners: false },
            },
        };
        if (itemWrappers) decimalsField.wrappers = itemWrappers;
        if (faClass) decimalsField.fieldArray.className = faClass;

        out.push(decimalsField);
        return out;
    };

    // Buckets per scriptGroupId
    const buckets = new Map<number, { left: ScriptItem[]; right: ScriptItem[] }>();
    for (const it of list) {
        const b = buckets.get(it.scriptGroupId) ?? { left: [], right: [] };
        (it.scriptGroupAlignTypeId === 0 ? b.left : b.right).push(it);
        buckets.set(it.scriptGroupId, b);
    }

    const groupIds = [...buckets.keys()].sort((a, b) => a - b);
    const rows: FormlyField[] = [];

    const renderRow = (L: ScriptItem[], R: ScriptItem[]) => {
        const rowCls = resolveClass(undefined, options.rowClassName, {});
        const rowFG = resolveClass('grid mt-2', options.rowFieldGroupClassName, {});

        const leftColCls = resolveClass('col-12 md:col-6', options.leftColClassName, {});
        const rightColCls = resolveClass('col-12 md:col-6', options.rightColClassName, {});
        const leftFGCls = resolveClass('grid', options.leftColFieldGroupClassName, {});
        const rightFGCls = resolveClass('grid', options.rightColFieldGroupClassName, {});

        return {
            className: rowCls,
            fieldGroupClassName: rowFG,
            fieldGroup: [
                {
                    className: leftColCls,
                    fieldGroupClassName: leftFGCls,
                    fieldGroup: sortByOrder(L).flatMap(it => makeFieldsForItem(it, 'left')),
                },
                {
                    className: rightColCls,
                    fieldGroupClassName: rightFGCls,
                    fieldGroup: sortByOrder(R).flatMap(it => makeFieldsForItem(it, 'right')),
                },
            ],
        };
    };

    for (let i = 0; i < groupIds.length; i++) {
        const gid = groupIds[i];
        const g = buckets.get(gid)!;
        const leftOnly = g.left.length > 0 && g.right.length === 0;
        const rightOnly = g.right.length > 0 && g.left.length === 0;

        if (!(leftOnly || rightOnly)) {
            rows.push(renderRow(g.left, g.right));
            continue;
        }

        const nextId = groupIds[i + 1];
        if (nextId !== undefined) {
            const g2 = buckets.get(nextId)!;
            const g2LeftOnly = g2.left.length > 0 && g2.right.length === 0;
            const g2RightOnly = g2.right.length > 0 && g2.left.length === 0;

            if (packsLTR && leftOnly && g2RightOnly) {
                rows.push(renderRow(g.left, g2.right));
                i++;
                continue;
            }
            if (packsRTL && rightOnly && g2LeftOnly) {
                rows.push(renderRow(g2.left, g.right));
                i++;
                continue;
            }
        }

        rows.push(renderRow(g.left, g.right));
    }

    return { fields: rows, model };
}
// Add spacing between dyads; tighten right column; highlight positive left items
//const res = buildFormlyDyadsPackedFromList(list, model, {
//    rowClassName: 'k-gradient p-3 mb-3',
//    leftColClassName: 'pr-2',
//    rightColClassName: 'pl-2',
//    leftItemClassName: ({ item }) => (item.value > 0 ? 'bg-green-50' : ''),
//    rightHeaderClassName: 'text-xs text-muted',
//});

//// Add compact child rows inside right-side decimals repeaters
//const res2 = buildFormlyDyadsPackedFromList(list, model, {
//    rightItemFieldArrayClassName: 'p-0 gap-1',
//    rowFieldGroupClassName: 'gap-3',
//});
