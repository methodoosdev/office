import { Component, ViewChild } from "@angular/core";
import { DialogService } from "@progress/kendo-angular-dialog";
import { ToastrService } from "ngx-toastr";

import { FormEditToken, ApdTekaUnitOfWork, FormListDialogComponent, TraderLookupUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { TranslationService } from "@core";
import { Observable } from "rxjs";

@Component({
    selector: "apd-teka-edit",
    templateUrl: "./apd-teka-edit.html"
})
export class ApdTekaEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/apd-teka';

    constructor(
        private translationService: TranslationService,
        public uow: ApdTekaUnitOfWork,
        public traderLookupUnitOfWork: TraderLookupUnitOfWork,
        private dialogService: DialogService,
        private toastrService: ToastrService) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
