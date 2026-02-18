import { Component, OnInit, ViewChild } from "@angular/core";
import { ListItemModel } from "@progress/kendo-angular-buttons";
import { FormlyFieldConfig } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { TranslationService } from "@core";
import { LogUnitOfWork, PersistStateUnitOfWork, GridViewToken, IFormlyFormInputs } from "@officeNg";

@Component({
    selector: "log-list",
    templateUrl: "./log-list.html"
})
export class LogListComponent implements OnInit {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];

    filterInputs: IFormlyFormInputs;

    pathUrl = 'office/log';

    constructor(
        private translationService: TranslationService,
        public uow: LogUnitOfWork,
        public persistStateUow: PersistStateUnitOfWork,
        private toastrService: ToastrService) {
    }

    ngOnInit(): void {
        this.settingsMenuItems = [
            {
                text: this.translationService.translate('common.clearAll'),
                click: () => {
                    this.clearAll();
                }
            }, {
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

    clearAll() {
        this.uow.clearAll()
            .then(() => {
                this.toastrService.success(this.translationService.translate('message.deletionCompleted'));
            })
            .catch((err: Error) => {
                throw err;
            });
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
