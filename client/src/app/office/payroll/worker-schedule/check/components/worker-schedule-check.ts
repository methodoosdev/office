import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { GroupDescriptor, DataResult, process } from '@progress/kendo-data-query';

import { TranslationService } from "@core";
import { WorkerScheduleCheckUnitOfWork } from "@officeNg";
import { slideInOutAnimation, stateAnimation } from '@primeNg';

@Component({
    selector: 'my-app',
    templateUrl: "./worker-schedule-check.html",
    animations: [stateAnimation, slideInOutAnimation]
})
export class WorkerScheduleCheckComponent implements OnInit {
    animate: boolean = true;

    groups: GroupDescriptor[] = [{ field: 'period' }];
    gridData: DataResult;

    parentUrl: string;

    title: string;
    traderName: string;

    parentId: number;

    columns: any;
    detailColumns: any;
    dataSource: any[];

    excelFileName: string;
    pdfFileName: string;
    autofitColumnsLabel: string;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private uow: WorkerScheduleCheckUnitOfWork,
        private translationService: TranslationService) {

        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    public ngOnInit(): void {

        this.route.params.forEach((params: any) => {

            this.parentId = +params.id;

            this.uow.loadProperties(this.parentId)
                .then(result => {
                    const columns = result.model.columns;
                    const detailColumns = result.model.detailColumns;

                    this.title = result.model.title;
                    this.excelFileName = `${result.model.fileName}.xlsx`;
                    this.pdfFileName = `${result.model.fileName}.pdf`;
                    this.traderName = result.model.traderName;

                    this.parentUrl = result.model.isTrader ? 'office/worker-schedule-by-trader' : 'office/worker-schedule-by-employee';

                    this.uow.loadDataSource({}, this.parentId)
                        .then(result => {
                            const data: any[] = result;

                            data.forEach((model) => {

                                Object.keys(columns).forEach(key => {
                                    const property = columns[key];

                                    if (property['fieldType'] === 'time' || property['fieldType'] === 'date') {
                                        const date = Date.parse(model[key]);
                                        if (!isNaN(date)) {
                                            model[key] = new Date(date);
                                        }
                                    }
                                });
                            });


                            this.columns = columns;
                            this.detailColumns = detailColumns;
                            this.dataSource = data;

                            this.loadDataSource();
                        })
                        .catch((err: Error) => {
                            this.navigationBack();
                            throw err;
                        });
                })
                .catch((err: Error) => {
                    this.navigationBack();
                    throw err;
                });
        });
    }

    public groupChange(groups: GroupDescriptor[]): void {
        this.groups = groups;
        this.loadDataSource();
    }

    navigationBack() {
        this.router.navigate([this.parentUrl]);
    }

    private loadDataSource() {
        return Promise.resolve(null).then(() => {
            this.gridData = process(this.dataSource, { group: this.groups });
        });

    }

}

