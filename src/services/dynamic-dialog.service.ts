import { Injectable, TemplateRef } from "@angular/core";
import { DialogCloseResult, DialogRef, DialogService } from "@progress/kendo-angular-dialog";
import { lastValueFrom } from 'rxjs';
import { PrimeNGConfig } from "@primeNg";
import { DialogResult } from "../api/dialog-result";

@Injectable({
    providedIn: 'root'
})
export class DynamicDialogService {
    yesLabel: string;
    noLabel: string;
    cancelLabel: string;
    actions: any[];

    constructor(private config: PrimeNGConfig, private dialogService: DialogService) {

        this.yesLabel = this.config.getTranslation('common.yes');
        this.noLabel = this.config.getTranslation('common.no');
        this.cancelLabel = this.config.getTranslation('common.cancel');

        this.actions = [
            { text: this.yesLabel, themeColor: "primary", dialogResult: DialogResult.Ok },
            { text: this.noLabel, dialogResult: DialogResult.No },
            { text: this.cancelLabel, dialogResult: DialogResult.Cancel }
        ];
    }

    open(title: string, body: string | TemplateRef<any>, cancelButtonVisible = true, danger = false): Promise<DialogResult> {

        const dialog: DialogRef = this.dialogService.open({
            title: title,
            content: body,
            actions: cancelButtonVisible ? this.actions : this.actions.filter(x => !(x.dialogResult == DialogResult.Cancel)),
            width: 450, height: 200, minWidth: 250, actionsLayout: 'end'
        });

        if (danger)
            dialog.dialog.location.nativeElement.classList.add('danger');

        return lastValueFrom(dialog.result).then((result: any) => {
            if (result instanceof DialogCloseResult) {
                return DialogResult.Cancel;
            }
            return result['dialogResult'];
        });
    }
}
