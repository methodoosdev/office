import { Component, Inject, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NgForm } from "@angular/forms";
import { HttpErrorResponse } from "@angular/common/http";

import { ToastrService } from "ngx-toastr";
import { Observable } from "rxjs";

import { ProgressHubService, TranslationService } from "@core";
import { PeriodicF2UnitOfWork, DialogResult, DynamicDialogService } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { stateAnimation } from "@primeNg";

@Component({
    selector: "periodic-f2-edit",
    templateUrl: "./periodic-f2-edit.html",
    styleUrls: ["./periodic-f2-edit.scss"],
    providers: [ProgressHubService],
    animations: [stateAnimation],
    encapsulation: ViewEncapsulation.None
})
export class PeriodicF2EditComponent implements OnInit, OnDestroy, CanComponentDeactivate {
    @ViewChild('editForm', { static: false }) editForm!: NgForm;
    animate: boolean = true;
    parentUrl: string;
    zoom: number = 1;
    canSubmit: boolean = true;
    canCalc: boolean = true;
    zoomEnter = false;

    model: any;

    optionsF523: any[];
    submitLabel: string;
    calcLabel: string;
    saveLabel: string;
    saveAndExitLabel: string;
    cancelLabel: string;
    aboutLabel: string;
    originModel: any;
    title: any;
    parentId: number;
    
    constructor(
        @Inject('BASE_URL') private baseUrl: string,
        private translationService: TranslationService,
        private route: ActivatedRoute,
        private router: Router,
        private dynamicDialogService: DynamicDialogService,
        private toastrService: ToastrService,
        private uow: PeriodicF2UnitOfWork,
        private hubService: ProgressHubService) {

        this.submitLabel = this.translationService.translate('common.submited');
        this.calcLabel = this.translationService.translate('common.calc');
        this.saveLabel = this.translationService.translate('common.save');
        this.saveAndExitLabel = this.translationService.translate('common.saveAndExit');
        this.cancelLabel = this.translationService.translate('common.cancel');
        this.aboutLabel = this.translationService.translate('common.about');

        this.parentUrl = "office/periodic-f2";
        this.optionsF523 = this.translationService.translate('lookup.f523') as any as any[];
    }
    get readonly() {
        if (this.model) {
            return this.model.submitModeTypeId == 1;
        }
        else {
            return false;
        }
    }

    toZoom(plus: boolean) {
        this.zoom = plus ? this.zoom + .05 : this.zoom - .05;
        this.zoomEnter = true;
    }

    onDataRetrieved(data: any): void {
        // Reset back to pristine
        //this.ngForm?.reset();

        this.originModel = data.model;
        this.model = Object.assign({}, data.model);
        //this.model = new PeriodicF2Model(data.model);

        this.title = data.formModel.customProperties.title;
    }

    navigationBack() {
        if (this.parentId)
            this.router.navigate([this.parentUrl, this.parentId]);
        else
            this.router.navigate([this.parentUrl]);
    }

    ngOnDestroy(): void {
        //this.hubService.stop();
    }

    ngOnInit() {
        //this.hubService.start();

        this.route.params.forEach((params: any) => {

            const reportId = +params.id;
            this.parentId = +params.parentId;

            this.uow.getEntity(reportId, this.parentId, {})
                .then(data => this.onDataRetrieved(data))
                .catch((err: Error) => {
                    this.navigationBack();
                    throw err;
                });
        });
    }

    get hasChanges(): boolean {
        if (this.editForm) {
            return this.editForm.dirty;
        }
        else {
            return false;
        }
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        if (!this.hasChanges) { return true; }

        const warning = this.translationService.translate('common.warning');
        const canDeactivate = this.translationService.translate('message.canDeactivate');

        return this.dynamicDialogService.open(warning, canDeactivate)
            .then((result) => {
                if (result === DialogResult.Ok) {
                    return this.save(true, true).then(() => true);
                }
                if (result === DialogResult.No) {
                    this.cancel(true);
                    return true;
                }

                return false;
            });
    }

    get canSave(): boolean {
        return this.hasChanges;
    }

    private commit() {
        return this.uow.commit(this.model, this.parentId)
            .catch((err: Error) => {
                throw err;
            });
    }

    saveAndExit() {
        this.commit().then(() => {
            const successfullySaved = this.translationService.translate('message.successfullySaved');

            this.editForm.form.markAsPristine();
            this.toastrService.success(successfullySaved);
            this.navigationBack();
        });
    }

    save(suppressConfirmation: boolean, deactivating: boolean = false) {

        return this.commit().then((modelId: number) => {
            if (!suppressConfirmation) {
                const successfullySaved = this.translationService.translate('message.successfullySaved');
                this.toastrService.success(successfullySaved);
            }

            if (!deactivating) {
                this.editForm.form.markAsPristine();

                // Navigate to saved model
                if (this.parentId) {
                    return this.router.navigateByUrl("", { skipLocationChange: true }).then(() =>
                        this.router.navigate([this.parentUrl, modelId, this.parentId]));
                } else {
                    return this.router.navigateByUrl("", { skipLocationChange: true }).then(() =>
                        this.router.navigate([this.parentUrl, modelId]));
                }
            }

            return true;
        });
    }

    rollback() {
        this.editForm?.reset();
    }

    cancel(deactivating: boolean = false) {
        this.rollback();

        // If model is detached after rollback, navigate back to parent.
        if (!deactivating) {
            this.router.navigate([this.parentUrl]);
        }
    }

    submit() {
        this.canSubmit = false;
        this.uow.submit(this.model.id, false, this.hubService?.connectionId)
            .then(() => {
                const successfullySubmited = this.translationService.translate('message.successfullySubmited');
                this.toastrService.success(successfullySubmited);
            })
            .catch((e: HttpErrorResponse) => {
                this.toastrService.error(this.translationService.translate('error.downloadImageFailed'));
            })
            .finally(() => {
                this.canSubmit = true;
                window.open(`${this.baseUrl}docs/customer-activity-log`, "_blank");
            });
    }

    calc(): void {
        this.canCalc = false;
        this.uow.calc(this.model.id)
            .then((result: any) => {
                this.model = result.model;
                this.editForm.form.markAsDirty();
            })
            .catch((err: Error) => {
                throw err;
            })
            .finally(() => {
                this.canCalc = true;
            });
    }
}
