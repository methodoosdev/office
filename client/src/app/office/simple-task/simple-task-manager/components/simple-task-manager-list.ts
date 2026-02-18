import { Component, OnInit, ViewChild } from "@angular/core";
import { ListItemModel } from "@progress/kendo-angular-buttons";
import { FormlyFieldConfig } from "@ngx-formly/core";

import { TranslationService } from "@core";
import { GridViewToken, IFormlyFormInputs, SimpleTaskManagerUnitOfWork } from "@officeNg";

@Component({
    selector: "simple-task-manager-list",
    templateUrl: "./simple-task-manager-list.html"
})
export class SimpleTaskManagerListComponent implements OnInit {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];

    filterInputs: IFormlyFormInputs;

    pathUrl = 'office/simple-task-manager';

    constructor(private translationService: TranslationService,
        public uow: SimpleTaskManagerUnitOfWork) {
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
