import { Component, OnInit } from "@angular/core";
import { ListItemModel } from "@progress/kendo-angular-buttons";
import { ToastrService } from "ngx-toastr";

import { TranslationService } from "@core";
import { CustomerPermissionUnitOfWork } from "@officeNg";

@Component({
    selector: "customer-permission-list",
    templateUrl: "./customer-permission-list.html"
})
export class CustomerPermissionListComponent implements OnInit {
    pathUrl = 'office/customer-permission';
    settingsMenuItems: ListItemModel[] = [];

    constructor(
        private translationService: TranslationService,
        public uow: CustomerPermissionUnitOfWork,
        private toastrService: ToastrService) {
    }

    ngOnInit(): void {
        this.settingsMenuItems = [{
            text: this.translationService.translate('common.autoInsert'),
            click: () => {

                this.uow.autoInsertMissingPermission()
                    .then(() => {
                        this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                    })
                    .catch((err: Error) => {
                        throw err;
                    });
            }
        }];
    }
}
