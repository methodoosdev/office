import { Component, ViewChild } from "@angular/core";
import { ColumnButtonClickEvent, GridViewToken, VatExemptionApprovalUnitOfWork } from "@officeNg";
import { TranslationService } from "@core";
import { ToastrService } from "ngx-toastr";

@Component({
    selector: "vat-exemption-approval-list",
    templateUrl: "./approval-list.html"
})
export class VatExemptionApprovalListComponent {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    pathUrl = 'office/vat-exemption-approval';

    constructor(
        public uow: VatExemptionApprovalUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
    }

    columnButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'setPrimary') {
            this.uow.setPrimary(event.dataItem.id)
                .then(() => {
                    this.table.refresh().then(() => {
                        this.toastrService.success(this.translationService.translate('message.successfullyCompleted'));
                    });
                }).catch((err: Error) => {
                    throw err;
                });
        }
    }
}

