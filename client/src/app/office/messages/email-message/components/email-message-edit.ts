import { Component, ViewChild } from "@angular/core";
import { FormlyFieldConfig } from "@ngx-formly/core";
import { DialogService } from "@progress/kendo-angular-dialog";
import { ToastrService } from "ngx-toastr";

import { AfterModelChangeEvent, FormEditToken, EmailMessageUnitOfWork, FormListDialogComponent, TraderLookupUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { TranslationService } from "@core";
import { Observable } from "rxjs";

@Component({
    selector: "email-message-edit",
    templateUrl: "./email-message-edit.html"
})
export class EmailMessageEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/email-message';

    constructor(
        private translationService: TranslationService,
        public uow: EmailMessageUnitOfWork,
        public traderLookupUnitOfWork: TraderLookupUnitOfWork,
        private dialogService: DialogService,
        private toastrService: ToastrService) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    afterModelChange(e: AfterModelChangeEvent) {
        const cc = e.fieldProperties['cc'];
        const bcc = e.fieldProperties['bcc'];
        const traderId = e.fieldProperties['traderId'];

        cc.props['onClick'] = (field: FormlyFieldConfig) => {
            this.importRecipients(<string>field.key, field);
        };

        bcc.props['onClick'] = (field: FormlyFieldConfig) => {
            this.importRecipients(<string>field.key, field);
        };

        traderId.props['selectionChange'] = (field: FormlyFieldConfig, event: any) => {

            if (event?.value > 0) {

                this.traderLookupUnitOfWork.getTraderCurrentEmail(event.value)
                    .then((result: any) => {
                        field.form.get('toAddress').setValue(result.email);
                        field.form.get('toName').setValue(result.traderName);
                        field.form.markAsDirty();
                    }).catch((error: Error) => {
                        throw error;
                    });
            }
        };
    }

    importRecipients(key: string, field: FormlyFieldConfig): void {
        const dialogRef = this.dialogService.open({
            content: FormListDialogComponent,
            width: 1120,
            height: 570
        });

        const component = dialogRef.content.instance as FormListDialogComponent;
        component.title = this.translationService.translate('menu.traders');
        component.uow = this.traderLookupUnitOfWork;

        dialogRef.result.subscribe((result: any[]) => {
            if (Array.isArray(result)) {
                this.traderLookupUnitOfWork.getTraderEmails(result)
                    .then((result: any) => {
                        field.form.get(key).setValue(result?.recipients || '');
                        field.form.markAsDirty();
                    }).catch((error: Error) => {
                        throw error;
                    });
            }
        });
    }
}
