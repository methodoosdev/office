import { Component, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { Workbook, WorkbookSheet, WorkbookSheetColumn, WorkbookSheetRow } from "@progress/kendo-angular-excel-export";
import { saveAs } from '@progress/kendo-file-saver';
import { ToastrService } from 'ngx-toastr';

import { ListItemModel } from "@progress/kendo-angular-buttons";
import { FormlyFieldConfig } from "@ngx-formly/core";

import { ColumnButtonClickEvent, GridViewToken, IFormlyFormInputs, PersistStateUnitOfWork, WorkerScheduleByEmployeeUnitOfWork } from "@officeNg";
import { TranslationService } from "@core";

@Component({
    selector: "worker-schedule-list-by-employee",
    templateUrl: "./list-by-employee.html",
    providers: [PersistStateUnitOfWork]
})
export class WorkerScheduleByEmployeeListComponent implements OnInit {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    settingsMenuItems: ListItemModel[] = [];

    filterInputs: IFormlyFormInputs;

    pathUrl = 'office/worker-schedule-by-employee';

    constructor(
        private router: Router,
        public uow: WorkerScheduleByEmployeeUnitOfWork,
        public persistStateUow: PersistStateUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
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

    private changeWorkerScheduleModeType(workerScheduleId: number, action: number) {
        this.uow.setModeType(workerScheduleId, action).then(() => {
            this.table.refresh();
            this.toastrService.success(this.translationService.translate('message.successfullyCompleted'));
        });
    }

    columnButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'waiting') {
            this.changeWorkerScheduleModeType(event.dataItem.id, 1);
        }
        if (event.action === 'submit') {
            this.changeWorkerScheduleModeType(event.dataItem.id, 2);
        }
        if (event.action === 'cancel') {
            this.changeWorkerScheduleModeType(event.dataItem.id, 4);
        }
        if (event.action === 'schedule') {
            this.router.navigate(['office/worker-schedule-submit', event.dataItem.id]);
        }
        if (event.action === 'check') {
            this.router.navigate(['office/worker-schedule-check', event.dataItem.id]);
        }
        if (event.action === 'excel') {
            this.exportToExcel(event.dataItem.id, event.dataItem.traderName);
        }
    }

    exportToExcel(id: number, traderName: string) {

        this.uow.exportToExcel({}, id).then((result) => {
            const workbook = new Workbook({
                sheets: <WorkbookSheet[]>[
                    {
                        // Column settings (width)
                        columns: <WorkbookSheetColumn[]>[
                            { autoWidth: true },
                            { autoWidth: true }
                        ],
                        // Title of the sheet
                        // name: 'Customers',
                        // Rows of the sheet
                        rows: <WorkbookSheetRow[]>result
                    }
                ]
            });
            workbook.toDataURL().then(dataUrl => {
                saveAs(dataUrl, `${traderName}_${id}.xlsx`);
            });
        });


        //const workbook = new Workbook({
        //    sheets: <WorkbookSheet[]>[
        //        {
        //            // Column settings (width)
        //            columns: <WorkbookSheetColumn[]>[
        //                { autoWidth: true },
        //                { autoWidth: true }
        //            ],
        //            // Title of the sheet
        //            name: 'Customers',
        //            // Rows of the sheet
        //            rows: <WorkbookSheetRow[]>[{ cells: null }, { cells: null },
        //                // First row (header)
        //                {
        //                    cells: <WorkbookSheetRowCell[]>[
        //                        // First cell
        //                        { value: 'Company Name' },
        //                        // Second cell
        //                        { value: 'Contact' }
        //                    ]
        //                },
        //                // Second row (data)
        //                {
        //                    cells: <WorkbookSheetRowCell[]>[
        //                        { value: 'Around the Horn' },
        //                        { value: 'Thomas Hardy' }
        //                    ]
        //                },
        //                // Third row (data)
        //                {
        //                    cells: <WorkbookSheetRowCell[]>[
        //                        { value: 'B Beverages' },
        //                        { value: 'Victoria Ashworth' }
        //                    ]
        //                }
        //            ]
        //        }
        //    ]
        //});
        //workbook.toDataURL().then(dataUrl => {
        //    saveAs(dataUrl, 'Test.xlsx');
        //});
    }

}
