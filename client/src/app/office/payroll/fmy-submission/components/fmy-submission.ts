import { Component, EventEmitter, Inject, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { FormGroup } from "@angular/forms";
import { ToastrService } from "ngx-toastr";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";

import { slideInOutAnimation, stateAnimation } from "@primeNg";
import { GridViewToken, FmySubmissionUnitOfWork } from "@officeNg";
import { ProgressHubService, TranslationService } from "@core";

@Component({
    selector: "fmy-submission",
    templateUrl: "./fmy-submission.html",
    providers: [ProgressHubService],
    animations: [stateAnimation, slideInOutAnimation]
})
export class FmySubmissionComponent implements OnInit, OnDestroy {
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
        private uow: FmySubmissionUnitOfWork,
        private toastrService: ToastrService,
        private chatHubService: ProgressHubService,
        private translationService: TranslationService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
        this.checkLabel = this.translationService.translate('common.check');
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
                data.searchModel['from'] = new Date(data.searchModel['from']);
                data.searchModel['to'] = new Date(data.searchModel['to']);

                this.title = data.tableModel.customProperties.title;
                this.columns = data.tableModel.customProperties.columns;

                this.searchFields = data.searchModel.customProperties.fields;
                this.searchModel = data.searchModel;

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    check() {
        const from = this.searchModel.from as Date;
        const to = this.searchModel.to as Date;

        if (!(from.getFullYear() == to.getFullYear())) {
            this.toastrService.error(this.translationService.translate('message.invalidYear'));
            return;
        }

        if (from.getMonth() > to.getMonth()) {
            this.toastrService.error(this.translationService.translate('message.invalidMonth'));
            return;
        }

        this.gridData = undefined;
        this.checkButtonDisabled = true;

        this.handleModelChange = false;

        this.uow.fmySubmissions(this.searchModel.selectedKeys, from.getMonth() + 1, to.getMonth() + 1, from.getFullYear(), this.chatHubService?.connectionId)
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
