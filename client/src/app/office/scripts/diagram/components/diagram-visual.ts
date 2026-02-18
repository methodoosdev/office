// diagram-visual.ts
import { Group, TextBlock, Process, MultiLineTextBlock } from '@progress/kendo-angular-diagrams';
import { appendSubLabelWithLastWordWrap } from './helpers';

export function visualCard2(options: any): Group {
    const g = new Group();
    const d = options.dataItem ?? {};

    // Card background (optional)
    g.append(new Process({
        width: d.boxWidth ?? 360,
        height: d.boxHeight ?? 120,
        cornerRadius: 6,
        stroke: { width: 1, color: d.strokeColor || '#A3CFBB' },
        fill: { color: d.colorScheme || '#E0F8F1' }
    }));

    // Your original title (unchanged)
    g.append(new TextBlock({
        text: String(d.label ?? ''),
        x: 16, y: 18,
        fontSize: 20,
        fontWeight: 600,
        fontFamily: 'Inter, system-ui, sans-serif'
    }));

    // SubTitle with "last-word to next line" logic
    appendSubLabelWithLastWordWrap(
        g,
        d.subLabel,
        16,   // x
        50,   // y
        288,  // maxWidthPx
        22,   // fontSize
        'Inter, system-ui, sans-serif',
        1.25  // lineHeight
    );

    return g;
}

export function visualCardMLTB(options: any): Group {
    const g = new Group();
    const d = options.dataItem ?? {};

    // Config (override per-node via dataItem if you want)
    const width = d.boxWidth ?? 320;
    const padX = d.padX ?? 16;
    const padTop = d.padTop ?? 14;
    const padBot = d.padBot ?? 14;
    const gap = d.gap ?? 8;

    const titleSize = d.titleFontSize ?? 20;
    const subSize = d.subTitleFontSize ?? 14;
    const family = d.fontFamily ?? 'Inter, system-ui, sans-serif';

    // Background card (height set after measuring text)
    const card = new Process({
        width,
        height: 60,
        cornerRadius: 6,
        stroke: { width: 1, color: d.strokeColor || '#A3CFBB' },
        fill: { color: d.colorScheme || '#E0F8F1' }
    });
    g.append(card);

    const textWidth = width - padX * 2;

    // Title — wraps automatically to the given width
    const titleTB = new MultiLineTextBlock({
        text: String(d.label ?? ''),
        x: padX,
        y: padTop,
        width: textWidth,
        fontSize: titleSize,
        fontWeight: 600,
        fontFamily: family,
        lineHeight: 1.25,           // tweak as you like
        align: 'start'               // 'start' | 'center' | 'end'
    });
    g.append(titleTB);

    // Measure title height
    const titleBox: any = (titleTB as any).bbox?.() ?? (titleTB as any).clippedBBox?.();
    let y = padTop + (titleBox?.size?.height ?? titleSize);

    // Subtitle (optional) — also wraps
    if (d.subLabel) {
        y += gap;
        const subTB = new MultiLineTextBlock({
            text: String(d.subLabel),
            x: padX,
            y,
            width: textWidth,
            fontSize: subSize,
            fontFamily: family,
            lineHeight: 1.25,
            align: 'start'
        });
        g.append(subTB);

        const subBox: any = (subTB as any).bbox?.() ?? (subTB as any).clippedBBox?.();
        y += (subBox?.size?.height ?? subSize);
    }

    // Resize card to fit content
    card.redraw({ width, height: y + padBot });

    return g;
}

export function visualCard(options: any): Group {
    const g = new Group();
    const d = options.dataItem ?? {};

    g.append(new Process({
        width: 320,
        height: 92,
        cornerRadius: 6,
        stroke: { width: 1, color: d.strokeColor || '#A3CFBB' },
        fill: { color: d.colorScheme || '#E0F8F1' }
    }));

    g.append(new TextBlock({
        text: d.label ?? '',
        x: 16, y: 18,
        fontSize: 20,
        fontWeight: 600,
        fontFamily: 'Inter, system-ui, sans-serif'
    }));

    if (d.subLabel) {
        g.append(new TextBlock({
            text: String(d.subLabel),
            x: 16, y: 50,
            fontSize: 22,
            fontFamily: 'Inter, system-ui, sans-serif'
        }));
    }

    return g;
}
