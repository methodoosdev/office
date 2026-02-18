import { Component, ViewChild } from "@angular/core";
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { ToastrService } from "ngx-toastr";

import {
    FormEditToken, CustomerUnitOfWork, AfterModelChangeEvent, FormListDetailMappingComponent,
    CustomerPermissionUnitOfWork, CustomerPermissionsByCustomerUnitOfWork, FormlyEditDialogComponent
} from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { TranslationService } from "@core";
import { Observable } from "rxjs";

@Component({
    selector: "customer-edit",
    templateUrl: "./customer-edit.html"
})
export class CustomerEditComponent implements CanComponentDeactivate {
    @ViewChild(FormListDetailMappingComponent) gridView: FormListDetailMappingComponent | null = null;
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/customer';
    customerPermissionsLabel: string;

    parentId: number;
    dialogFormModel: any;

    constructor(
        private dialogService: DialogService,
        private translationService: TranslationService,
        public uow: CustomerUnitOfWork,
        public customerPermissionsByCustomerUow: CustomerPermissionsByCustomerUnitOfWork,
        public customerPermissionUow: CustomerPermissionUnitOfWork,
        private toastrService: ToastrService) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    ngOnInit() {
        this.customerPermissionsLabel = this.translationService.translate('menu.customerPermissions');

        this.uow.prepareParentIdDialog()
            .then((formModel: any) => {
                this.dialogFormModel = formModel;

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    afterModelChange(e: AfterModelChangeEvent) {
        const password = e.fieldProperties['password'];

        this.parentId = e.model.id;

        password.props['onClick'] = () => {

            this.uow.changePassword(e.model)
                .then(() => {
                    e.form.get('password').setValue(null);
                    //this.formEdit.resetControl('password');
                    this.toastrService.success(this.translationService.translate('message.changePasswordSuccess'));
                }).catch((err: Error) => {
                    throw err;
                });
        };
    }

    copyPermissions(customerId: number) {
        const dialogRef: DialogRef = this.dialogService.open({
            content: FormlyEditDialogComponent,
            minHeight: 200, minWidth: 250, width: 350, actionsLayout: 'end'
        });

        const instance = dialogRef.content.instance as FormlyEditDialogComponent;
        const data = {
            model: {
                parentId: 0
            },
            formModel: this.dialogFormModel
        };
        instance.createLabel = true;
        instance.setModel(data);
        instance.title = this.translationService.translate('common.create');

        dialogRef.result
            .subscribe((result: any) => {

                if (result?.text === "Submit") {
                    this.customerPermissionsByCustomerUow.insertCustomerPermissions(instance.model.parentId, customerId)
                        .then(() => {
                            this.gridView.selectedKeys = [];
                            return Promise.resolve(null).then(() => {
                                this.gridView.loadDataSource();
                            })
                        })
                        .catch((err: Error) => {
                            throw err;
                        }).finally(() => {
                            this.toastrService.success(this.translationService.translate('message.processCompleted'));
                        });
                }
            });
    }
}
