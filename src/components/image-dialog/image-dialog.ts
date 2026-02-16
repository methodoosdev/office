import { CommonModule } from "@angular/common";
import { Component, NgModule } from "@angular/core";
import { DialogContentBase, DialogModule, DialogRef } from "@progress/kendo-angular-dialog";

@Component({
    selector: "image-dialog",
    templateUrl: "./image-dialog.html"

})
export class ImageDialogComponent extends DialogContentBase {
    title = "Scraping";
    imageSrc: string;

    constructor(dialogRef: DialogRef) {
        super(dialogRef);
    }
}

@NgModule({
    imports: [CommonModule, DialogModule],
    exports: [ImageDialogComponent],
    declarations: [ImageDialogComponent]
})
export class ImageDialogModule { }
