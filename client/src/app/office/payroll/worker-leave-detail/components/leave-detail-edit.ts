import { Component, ViewChild } from "@angular/core";
import { Observable } from "rxjs";
import { FormlyFieldConfig } from "@ngx-formly/core";

import { AfterModelChangeEvent, FormEditToken, WorkerLeaveDetailUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";

@Component({
    selector: "leave-detail-edit",
    templateUrl: "./leave-detail-edit.html"
})
export class WorkerLeaveDetailEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/worker-leave-detail';
    model: any;

    constructor(public uow: WorkerLeaveDetailUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    afterModelChange(e: AfterModelChangeEvent) {

        const form = this.formEdit.getForm();
        this.model = e.model;

        setTimeout(() => {

            const traderId = e.fieldProperties['traderId'];
            const workerId = e.fieldProperties['workerId'];

            traderId.props['change'] = (field: FormlyFieldConfig, traderId?: any) => {
                if (traderId?.type == "change") return;

                if (traderId) {
                    this.uow.getWorkers(traderId)
                        .then((data: any[]) => {

                            const workerId = e.fieldProperties['workerId'];
                            workerId.props['options'] = data;

                        }).catch((err: Error) => {
                            throw err;
                        });
                }

                e.fieldProperties['workerId'].props['options'] = [];
                form.get('workerId').setValue(0);
                this.model.workerName = null;
            };

            workerId.props['change'] = (field: FormlyFieldConfig, workerId?: any) => {
                if (workerId?.type == "change") return;

                if (workerId) {
                    const options = e.fieldProperties['workerId'].props['options'] as any[];

                    const item = options.filter((x) => x.value == workerId);

                    //form.get('workerName').setValue(item[0].label);
                    this.model.workerName = item[0].label;
                } else {
                    //form.get('workerName').setValue(null);
                    this.model.workerName = null;
                }

            };

            //e.form.get('traderId').valueChanges.subscribe((change: boolean) => {
            //    if (change) {
            //        const properties: string[] = ['splitFromDate', 'splitToDate', 'breakSplitFromDate', 'breakSplitToDate'];

            //    }
            //});

        }, 100);
    }
}
