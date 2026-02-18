import { Component, ViewChild } from "@angular/core";
import { FormlyFieldConfig } from "@ngx-formly/core";

import { GridViewToken, IFormlyFormInputs, WorkerScheduleLogUnitOfWork } from "@officeNg";
import { AuthService, RoleName } from "@jwtNg";

@Component({
    selector: "worker-schedule-log-list",
    templateUrl: "./worker-schedule-log-list.html"
})
export class WorkerScheduleLogListComponent {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    pathUrl = 'office/worker-schedule-log';
    filterButtonVisible: boolean;

    filterInputs: IFormlyFormInputs;

    constructor(
        authService: AuthService,
        public uow: WorkerScheduleLogUnitOfWork) {
        this.filterButtonVisible = authService.isAuthUserInRole(RoleName.Employees);
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
