// helpers.ts
import { Group, TextBlock } from '@progress/kendo-angular-diagrams';

// Canvas measurer for accurate pixel width
const canvas = document.createElement('canvas');
const ctx = canvas.getContext('2d')!;

function measureWidthPx(text: string, fontCss: string): number {
    ctx.font = fontCss;
    return ctx.measureText(text).width;
}

/**
 * Appends subLabel. If its width exceeds maxWidthPx,
 * splits the **last word** into a new TextBlock on the next line.
 * Returns the next Y position after the rendered text.
 */
export function appendSubLabelWithLastWordWrap(
    g: Group,
    subLabel: string | undefined,
    x = 16,
    y = 50,
    maxWidthPx = 288,
    fontSize = 22,
    fontFamily = 'Inter, system-ui, sans-serif',
    lineHeight = 1.25
): number {
    const text = String(subLabel ?? '').replace(/\s+/g, ' ').trim();
    if (!text) return y;

    const fontCss = `${fontSize}px ${fontFamily}`;
    const width = measureWidthPx(text, fontCss);

    // If it fits, draw once and return.
    if (width <= maxWidthPx) {
        g.append(new TextBlock({ text, x, y, fontSize, fontFamily }));
        return y + Math.round(fontSize * lineHeight);
    }

    // Otherwise, split the LAST word to the next line.
    const parts = text.split(' ');
    if (parts.length === 1) {
        // Single long token: no spaces to split on — just draw as-is (or handle hyphenation if desired).
        g.append(new TextBlock({ text, x, y, fontSize, fontFamily }));
        return y + Math.round(fontSize * lineHeight);
    }

    const lastWord = parts.pop()!;
    const firstLine = parts.join(' ').trim();

    // Replace the "split last word" block with this stricter loop
    let lastLineWords: string[] = [];
    while (parts.length > 0 && measureWidthPx(parts.join(' '), fontCss) > maxWidthPx) {
        lastLineWords.unshift(parts.pop()!); // move one more word down
    }
    const firstLineStrict = parts.join(' ').trim();
    const secondLine = (lastLineWords.length ? lastLineWords.join(' ') : lastWord).trim();

    g.append(new TextBlock({ text: firstLineStrict, x, y, fontSize, fontFamily }));
    const y2 = y + Math.round(fontSize * lineHeight);
    g.append(new TextBlock({ text: secondLine, x, y: y2, fontSize, fontFamily }));

    //g.append(new TextBlock({ text: firstLine, x, y, fontSize, fontFamily }));

    //const y2 = y + Math.round(fontSize * lineHeight);
    //g.append(new TextBlock({ text: lastWord, x, y: y2, fontSize, fontFamily }));

    return y2 + Math.round(fontSize * lineHeight);
}
