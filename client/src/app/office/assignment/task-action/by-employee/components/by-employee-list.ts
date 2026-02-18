import { Component, ViewChild } from "@angular/core";
import { AssignmentTaskActionByEmployeeUnitOfWork, GridViewToken, IFormlyFormInputs } from "@officeNg";
import { ListItemModel } from "@progress/kendo-angular-buttons";

import { TranslationService } from "@core";
import { FormlyFieldConfig } from "@ngx-formly/core";
import { RowClassArgs } from "@progress/kendo-angular-grid";

@Component({
    selector: "assignment-task-action-by-employee-list",
    templateUrl: "./by-employee-list.html"
})
export class AssignmentTaskActionByEmployeeListComponent {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];

    filterInputs: IFormlyFormInputs;

    pathUrl = 'office/assignment-task-action-by-employee';

    constructor(
        private translationService: TranslationService,
        public uow: AssignmentTaskActionByEmployeeUnitOfWork) {
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

    rowClass = (args: RowClassArgs) => {
        const inProgress = args.dataItem["assignmentActionStatusTypeId"] == 1;
        const now = new Date();
        const expiryDate = new Date(args.dataItem.expiryDate);
        const difference_In_Time = expiryDate.getTime() - now.getTime();
        const difference_In_Days = difference_In_Time / (1000 * 3600 * 24);

        if (inProgress && difference_In_Days <= 7 && difference_In_Days >= 0) {
            return { violet: true };
        } else if (inProgress && difference_In_Days < 0) {
            return { red: true };
        } else {
            return { 'violet red': false };
        }
    };

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
