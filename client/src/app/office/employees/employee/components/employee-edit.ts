import { Component, OnInit, ViewChild } from "@angular/core";

import { AfterModelChangeEvent, FormEditToken, EmployeeUnitOfWork, TradersByEmployeeUnitOfWork, TraderUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { TranslationService } from "@core";
import { Observable } from "rxjs";

@Component({
    selector: "employee-edit",
    templateUrl: "./employee-edit.html"
})
export class EmployeeEditComponent implements OnInit, CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/employee';

    tradersLabel: string;

    parentId: number;

    constructor(
        public uow: EmployeeUnitOfWork,
        public tradersByEmployeeUow: TradersByEmployeeUnitOfWork,
        public traderUow: TraderUnitOfWork,
        private translationService: TranslationService) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    ngOnInit() {
        this.tradersLabel = this.translationService.translate('menu.traders');
    }

    afterModelChange(e: AfterModelChangeEvent) {
        this.parentId = e.model.id;
    }

}
