import { Component, OnInit, ViewChild, ViewEncapsulation } from "@angular/core";
import { DialogService } from "@progress/kendo-angular-dialog";
import { ToastrService } from "ngx-toastr";

import { GridViewToken, CustomerUnitOfWork, CustomerPermissionUnitOfWork, FormListDialogComponent } from "@officeNg";
import { ActionsMenuService, TranslationService } from "@core";

@Component({
    selector: "customer-list",
    templateUrl: "./customer-list.html",
    encapsulation: ViewEncapsulation.None,
    styles: [`
        .k-menu:not(.k-context-menu) {
            background-color: #eceff1;
        }
    `]
})
export class CustomerListComponent implements OnInit {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    pathUrl = 'office/customer';
    actionsMenu: any[];

    constructor(
        private translationService: TranslationService,
        private toastrService: ToastrService,
        private dialogService: DialogService,
        private actionsMenuService: ActionsMenuService,
        private customerPermissionUow: CustomerPermissionUnitOfWork,
        public uow: CustomerUnitOfWork) {
    }

    ngOnInit(): void {
        this.actionsMenu = this.actionsMenuService.customerActionMenu();
    }

    onSelectMenu(e: any): void {
        switch (e.item.id) {
            case 'insertPermissions':
                this.importMapping();
                break;
            case 'deletePermissions':
                this.removeMapping();
                break;
        }
    }

    importMapping() {

        const selectedIds = this.table.GetSelectedKeys();
        if (selectedIds.length == 0) {
            this.toastrService.warning(this.translationService.translate('message.hasSelections'));
            return;
        }

        const dialogRef = this.dialogService.open({
            content: FormListDialogComponent,
            width: 1120,
            height: 570
        });

        const component = dialogRef.content.instance as FormListDialogComponent;
        component.title = this.translationService.translate('menu.customerPermissions');
        component.uow = this.customerPermissionUow;

        dialogRef.result.subscribe((result: any[]) => {
            if (Array.isArray(result)) {
                const data = {
                    customerPermissions: result,
                    customers: selectedIds
                };

                this.customerPermissionUow.importMapping(data)
                    .then(() => {
                        this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                    }).catch((error: Error) => {
                        throw error;
                    });
            }
        });


    }

    removeMapping() {

        const selectedIds = this.table.GetSelectedKeys();
        if (selectedIds.length == 0) {
            this.toastrService.warning(this.translationService.translate('message.hasSelections'));
            return
        }

        const dialogRef = this.dialogService.open({
            content: FormListDialogComponent,
            width: 1120,
            height: 570
        });

        const component = dialogRef.content.instance as FormListDialogComponent;
        component.title = this.translationService.translate('menu.customerPermissions');
        component.uow = this.customerPermissionUow;

        dialogRef.result.subscribe((result: any[]) => {
            if (Array.isArray(result)) {
                const data = {
                    customerPermissions: result,
                    customers: selectedIds
                };

                this.customerPermissionUow.removeMapping(data)
                    .then(() => {
                        this.toastrService.success(this.translationService.translate('message.deletionCompleted'));
                    }).catch((error: Error) => {
                        throw error;
                    });
            }
        });


    }

}
