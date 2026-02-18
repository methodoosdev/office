import { Component, OnInit, ViewChild } from "@angular/core";
import { FormlyFieldConfig } from "@ngx-formly/core";

import { GridViewToken, AssignmentTaskUnitOfWork, AssignmentTaskActionUnitOfWork, IFormlyFormInputs } from "@officeNg";
import { ListItemModel } from "@progress/kendo-angular-buttons";

import { TranslationService } from "@core";

@Component({
    selector: "assignment-task-list",
    templateUrl: "./task-list.html"
})
export class AssignmentTaskListComponent implements OnInit {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];

    filterInputs: IFormlyFormInputs;

    pathUrl = 'office/assignment-task';

    constructor(
        private translationService: TranslationService,
        public uow: AssignmentTaskUnitOfWork,
        public assignmentTaskActionUow: AssignmentTaskActionUnitOfWork) {
    }

    ngOnInit(): void {
        this.settingsMenuItems = [
            {
                text: this.translationService.translate('common.saveState'),
                click: () => {
                    this.table.saveState();
                }
            }, {
                text: this.translationService.translate('common.removeState'),
                click: () => {
                    this.table.removeState();
                }
            }];
    }

    filterInputsChange(filterInputs: IFormlyFormInputs) {

        filterInputs.properties['saveState']['expressions'] = {
            'props.disabled': (field: FormlyFieldConfig) => {
                return !field.form.dirty;
            }
        };

        filterInputs.properties['saveState'].props['click'] = () => {
            this.table.filterSaveState(filterInputs);
        };

        filterInputs.properties['removeState'].props['click'] = () => {
            this.table.filterRemoveState(filterInputs);
        };

        this.filterInputs = filterInputs
    }

 }
