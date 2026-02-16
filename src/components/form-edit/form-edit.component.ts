import { AfterContentInit, Component, ContentChildren, EventEmitter, Input, OnInit, Output, QueryList, TemplateRef, ViewChild, } from "@angular/core";
import { FormGroup, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";

import { ToastrService } from 'ngx-toastr';
import { ButtonThemeColor } from "@progress/kendo-angular-buttons";
import { Observable } from "rxjs";
import { stateAnimation, PrimeTemplate, PrimeNGConfig } from "@primeNg";
import { CanComponentDeactivate } from "@jwtNg";

import { FormEditToken } from "../../api/form-edit-token";
import { UnitOfWork } from "../../api/unit-of-work";
import { ModelChangeEvent } from "../../api/model-change-event";
import { DynamicDialogService } from "../../services/dynamic-dialog.service";
import { DialogResult } from "../../api/dialog-result";

@Component({
    selector: 'form-edit',
    templateUrl: './form-edit.component.html',
    providers: [{ provide: FormEditToken, useExisting: FormEditComponent }],
    animations: [stateAnimation]
})
export class FormEditComponent extends FormEditToken implements OnInit, CanComponentDeactivate, AfterContentInit {
    @ViewChild('editForm', { static: false }) editForm!: NgForm;
    @Input() animate: boolean = true;
    @Input() uow: UnitOfWork;
    @Input() parentUrl: string;
    @Input() borderForm: boolean = true;
    @Input() horizontal: boolean = true;
    @Input() customButtonVisible: boolean = false;
    @Input() saveButtonVisible: boolean = true;
    @Input() saveAndExitButtonVisible: boolean = true;
    @Input() cancelButtonVisible: boolean = true;
    @Input() customButtonTheme: ButtonThemeColor = "tertiary";
    @Input() customButtonLabel: string;

    model: any;
    originModel: any;
    parentId: number;

    title: string;
    saveLabel: string;
    saveAndExitLabel: string;
    cancelLabel: string;
    aboutLabel: string;

    @Output() onModelChange: EventEmitter<ModelChangeEvent> = new EventEmitter();
    @Output() onCustomButton: EventEmitter<any> = new EventEmitter();

    toolbarTemplate: TemplateRef<any>;
    contentTemplate: TemplateRef<any>;
    @ContentChildren(PrimeTemplate) templates: QueryList<any>;
    customProperties: any;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private dynamicDialogService: DynamicDialogService,
        private toastrService: ToastrService,
        private config: PrimeNGConfig) {
        super();

        this.saveLabel = this.config.getTranslation('common.save');
        this.saveAndExitLabel = this.config.getTranslation('common.saveAndExit');
        this.cancelLabel = this.config.getTranslation('common.cancel');
        this.aboutLabel = this.config.getTranslation('common.about');
    }

    ngAfterContentInit() {
        this.templates.forEach((item) => {
            switch (item.getType()) {
                case 'toolbar':
                    this.toolbarTemplate = item.template;
                    break;

                case 'content':
                    this.contentTemplate = item.template;
                    break;

                default:
                    this.contentTemplate = item.template;
                    break;
            }
        });
    }

    getForm(): FormGroup {
        return this.editForm?.form;
    }

    get isAdded() {
        return (this.model && this.model.id === 0) ? true : false;
    }

    get hasChanges(): boolean {
        if (this.editForm) {
            return this.editForm.dirty;
        }
        else {
            return false;
        }
    }

    resetModel(model: any = {}) {
        //this.model = Object.assign({}, model, this.originModel);
        for (let key in this.originModel)
            (this.model as any)[key] = this.originModel[key];

        this.editForm?.reset(this.originModel)
        this.editForm?.form.markAsPristine();
    }

    updateModelPartial(partial: any): void {
        const model = Object.assign({}, this.model, partial);

        this.resetModel(model);
        Object.keys(this.editForm?.controls).forEach(key => {
            this.editForm?.form.get(key).setErrors(null);
        });
        this.editForm?.form.markAsDirty();
    }

    getModel(): any {
        return this.model;
    }

    setValue(property: string, value: any) {
        this.model[property] = value;
    }

    setParentId(parentId: number) {
        this.parentId = parentId;
    }

    resetControl(property: string) {
        this.editForm?.controls[property].reset();
    }

    markAsPristine(): void {
        this.editForm?.form.markAsPristine();
    }

    onDataRetrieved(data: any): void {
        // Reset back to pristine
        //this.ngForm?.reset();

        this.originModel = data.model;
        this.model = Object.assign({}, data.model);

        this.title = data.formModel.customProperties.title;
        this.customProperties = data.formModel.customProperties;

        this.onModelChange.emit({ data: this.model, form: this.editForm?.form });
    }

    ngOnInit(): void {
        this.route.params.forEach((params: any) => {

            const reportId = +params.id;
            this.parentId = +params.parentId;

            this.uow.getEntity(reportId, this.parentId, this.route.snapshot.queryParams)
                .then(data => this.onDataRetrieved(data))
                .catch((err: Error) => {
                    this.navigationBack();
                    throw err;
                });
        });
    }

    cancel(): void {
        this.resetModel();
    }

    navigationBack() {
        if (this.parentId)
            this.router.navigate([this.parentUrl, this.parentId]);
        else
            this.router.navigate([this.parentUrl]);
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        if (!this.hasChanges) { return true; }

        const warning = this.config.getTranslation('common.warning');
        const canDeactivate = this.config.getTranslation('message.canDeactivate');

        return this.dynamicDialogService.open(warning, canDeactivate)
            .then((result) => {
                if (result === DialogResult.Ok) {
                    return this.save(true, true).then(() => true);
                }
                if (result === DialogResult.No) {
                    return true;
                }

                return false;
            });
    }

    private commit() {
        return this.uow.commit(this.model, this.parentId)
            .catch((err: Error) => {
                throw err;
            });
    }

    saveAndExit() {
        this.commit().then(() => {
            const successfullySaved = this.config.getTranslation('message.successfullySaved');

            this.markAsPristine();
            this.toastrService.success(successfullySaved);
            this.navigationBack();
        });
    }

    save(suppressConfirmation: boolean, deactivating: boolean = false) {

        return this.commit()
            .then((modelId: number) => {
                if (!suppressConfirmation) {
                    const successfullySaved = this.config.getTranslation('message.successfullySaved');
                    this.toastrService.success(successfullySaved);
                }

                this.markAsPristine();

                if (!deactivating) {
                    // Navigate to saved model

                    if (this.parentId) {
                        return this.router.navigateByUrl(`${this.parentUrl}/${modelId}`, { skipLocationChange: true }).then(() =>
                            this.router.navigate([this.parentUrl, modelId, this.parentId]));
                    } else {
                        return this.router.navigateByUrl(this.parentUrl, { skipLocationChange: true }).then(() =>
                            this.router.navigate([this.parentUrl, modelId]));
                    }
                }

                return true;
            });
    }

    get canDelete(): boolean {
        return !this.isAdded;
    }

    delete() {
        return this.uow.delete(this.model.id)
            .then(() => {
                const deletionCompleted = this.config.getTranslation('message.deletionCompleted');
                this.toastrService.success(deletionCompleted);
                this.resetModel();
                this.navigationBack();
            })
            .catch((error: any) => {
                const failedToDelete = this.config.getTranslation('message.failedToDelete');
                this.toastrService.success(failedToDelete);
                console.error(error);
            });
    }

    onCustomButtonClick() {
        this.onCustomButton.emit({});
    }
}
