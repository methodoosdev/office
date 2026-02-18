// diagram-visual.ts
import { Group, TextBlock, Process } from '@progress/kendo-angular-diagrams';

export function visualCard(options: any): Group {
    const g = new Group();
    const d = options.dataItem ?? {};

    if (d.isBucket) {
        // Invisible spacer so the layout can anchor a row
        g.append(new Process({
            width: 10,
            height: 10,
            stroke: { width: 0, color: 'transparent' },
            fill: { color: 'transparent' }
        }));
        return g;
    }

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
        fontSize: 18,
        fontWeight: 600,
        fontFamily: 'Inter, system-ui, sans-serif'
    }));

    if (d.subLabel) {
        g.append(new TextBlock({
            text: String(d.subLabel),
            x: 16, y: 50,
            fontSize: 14,
            fontFamily: 'Inter, system-ui, sans-serif'
        }));
    }

    return g;
}
