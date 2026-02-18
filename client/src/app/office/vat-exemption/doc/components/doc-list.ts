import { Component, ViewChild } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { saveAs } from "@progress/kendo-file-saver";

import { ColumnButtonClickEvent, GridViewToken, VatExemptionDocUnitOfWork } from "@officeNg";

@Component({
    selector: "vat-exemption-doc-list",
    templateUrl: "./doc-list.html"
})
export class VatExemptionDocListComponent {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    pathUrl = 'office/vat-exemption-doc';

    constructor(
        public uow: VatExemptionDocUnitOfWork,
        private toastrService: ToastrService) {
    }

    columnButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'exportToPdf') {
            this.uow.exportToPdf({}, event.dataItem.id)
                .then((result) => {
                    if (result)
                        saveAs(result, "vatExemption.pdf");
                    else
                        this.toastrService.error('Pdf file cannot be created.');
                }).catch((err: Error) => {
                    throw err;
                });
        }
    }
 }
