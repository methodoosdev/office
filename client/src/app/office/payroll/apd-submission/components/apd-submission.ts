import { Component, EventEmitter, Inject, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { FormGroup } from "@angular/forms";
import { ToastrService } from "ngx-toastr";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";

import { slideInOutAnimation, stateAnimation } from "@primeNg";
import { ApdSubmissionUnitOfWork, GridViewToken } from "@officeNg";
import { ProgressHubService, TranslationService } from "@core";

@Component({
    selector: "apd-submission",
    templateUrl: "./apd-submission.html",
    providers: [ProgressHubService],
    animations: [stateAnimation, slideInOutAnimation]
})
export class ApdSubmissionComponent implements OnInit, OnDestroy {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    autofitColumnsLabel: string;
    checkLabel: string;
    yesLabel: string;
    noLabel: string;

    searchForm: FormGroup<any> = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    checkButtonDisabled: boolean = false;
    handleModelChange: boolean = true;
    private progressChange: EventEmitter<string> = new EventEmitter();

    constructor(
        @Inject('BASE_URL') private baseUrl: string,
        private router: Router,
        private uow: ApdSubmissionUnitOfWork,
        private toastrService: ToastrService,
        private hubService: ProgressHubService,
        private translationService: TranslationService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.checkLabel = this.translationService.translate('common.check');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    onModelChange(value: any) {
        if (this.handleModelChange) {
            this.gridData = undefined;
        }
    }

    ngOnDestroy(): void {
        //this.hubService.stop();
    }

    ngOnInit(): void {
        //this.hubService.start();
        //this.hubService.hubConnection.on('progressLabel', (message: string) => {
        //    this.progressChange.emit(message);
        //});

        this.progressChange.subscribe((value: string) => {
            this.searchForm?.get("progress").setValue(value);
        });

        this.uow.loadProperties()
            .then((data: any) => {
                data.searchModel['period'] = new Date(data.searchModel['period']);

                this.title = data.tableModel.customProperties.title;
                this.columns = data.tableModel.customProperties.columns;

                this.searchFields = data.searchModel.customProperties.fields;
                this.searchModel = data.searchModel;

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }
    
    check() {
        const period = this.searchModel.period as Date;

        this.gridData = undefined;
        this.checkButtonDisabled = true;

        this.handleModelChange = false;

        this.uow.apdSubmissions(this.searchModel.selectedKeys, period.getMonth() + 1, period.getFullYear(), this.hubService?.connectionId)
            .then((results: any) => {
                this.gridData = results;
                this.searchForm.get("progress").setValue("");
            }).catch((error: Error) => {
                throw error;
            }).finally(() => {
                this.checkButtonDisabled = false;
                this.toastrService.success(this.translationService.translate('message.processCompleted'));

                setTimeout(() => {
                    window.open(`${this.baseUrl}docs/customer-activity-log`, "_blank");
                    this.handleModelChange = true;
                }, 1000);
            });
    }

    goBack() {
        this.router.navigateByUrl('/office');
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canSelect() {
        return !this.checkButtonDisabled && this.searchModel && this.searchModel.selectedKeys.length > 0;
    }

}
