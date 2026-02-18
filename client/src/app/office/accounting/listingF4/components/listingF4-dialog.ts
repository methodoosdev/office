import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { TranslationService } from "@core";

export class ListingF4Model {
    error: string;
    countryCode: string;
    vat: string;
    vatNumber: string;
    goods: number;
    triangleExchange: number;
    services: number;
    products4200: number;
}

@Component({
    selector: "listingF4-dialog",
    templateUrl: "./listingF4-dialog.html",
    styles: [
        `
            input[type='text'] {
                width: 100%;
            }
            .k-inline-checkbox {
                display: inline-flex;
            }
        `
    ]
})
export class ListingF4DialogComponent {
    public active = false;
    public editForm: FormGroup = new FormGroup({
        countryCode: new FormControl(''),
        group: new FormControl(''),
        vat: new FormControl(''),
        goods: new FormControl(0),
        triangleExchange: new FormControl(0),
        services: new FormControl(0),
        products4200: new FormControl(0)
    });

    @Input() public isNew = false;
    @Input() public fields: any;

    @Input() public set model(item: any) {
        this.editForm.reset(item);

        // toggle the Dialog visibility
        this.active = item !== undefined;
    }

    @Output() cancel: EventEmitter<undefined> = new EventEmitter();
    @Output() save: EventEmitter<any> = new EventEmitter();

    insertLabel: string;
    modifyLabel: string;
    cancelLabel: string;
    saveLabel: string;

    constructor(translationService: TranslationService) {
        this.insertLabel = translationService.translate('common.insert');
        this.modifyLabel = translationService.translate('common.modify');
        this.cancelLabel = translationService.translate('common.cancel');
        this.saveLabel = translationService.translate('common.save');
    }

    public onSave(e: PointerEvent): void {
        e.preventDefault();
        this.save.emit(this.editForm.value);
        this.active = false;
    }

    public onCancel(e: PointerEvent): void {
        e.preventDefault();
        this.closeForm();
    }

    public closeForm(): void {
        this.active = false;
        this.cancel.emit();
    }
}
