import { Component, ViewChild } from "@angular/core";
import { Observable } from "rxjs";

import { AfterModelChangeEvent, AssignmentPrototypeActionsByAssignmentPrototypeUnitOfWork, AssignmentPrototypeActionUnitOfWork, AssignmentPrototypeUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { TranslationService } from "@core";

@Component({
    selector: "assignment-prototype-edit",
    templateUrl: "./prototype-edit.html"
})
export class AssignmentPrototypeEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/assignment-prototype';

    parentId: number;
    assignmentPrototypeActionsLabel: string;

    constructor(
        private translationService: TranslationService,
        public uow: AssignmentPrototypeUnitOfWork,
        public assignmentPrototypeActionUow: AssignmentPrototypeActionUnitOfWork,
        public assignmentPrototypeActionsByAssignmentPrototypeUow: AssignmentPrototypeActionsByAssignmentPrototypeUnitOfWork) {

        this.assignmentPrototypeActionsLabel = this.translationService.translate('menu.assignmentPrototypeAction');
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    afterModelChange(e: AfterModelChangeEvent) {
        this.parentId = e.model.id;
    }
}
