function downloadFromUrl(url: string, fileName: string): void {
    const anchorElement = document.createElement('a');

    anchorElement.href = url;
    anchorElement.download = fileName;
    anchorElement.click();
    anchorElement.remove();
}

export function downloadFromByteArray(byteArray: string, fileName: string, contentType: string = "image/png"): void {

    // Convert base64 string to numbers array.
    const numArray = atob(byteArray).split('').map(c => c.charCodeAt(0));

    // Convert numbers array to Uint8Array object.
    const uint8Array = new Uint8Array(numArray);

    // Wrap it by Blob object.
    const blob = new Blob([uint8Array], { type: contentType });

    // Create "object URL" that is linked to the Blob object.
    const url = URL.createObjectURL(blob);

    // Invoke download helper function that implemented in 
    // the earlier section of this article.
    downloadFromUrl(url, fileName);

    // At last, release unused resources.
    URL.revokeObjectURL(url);
}
