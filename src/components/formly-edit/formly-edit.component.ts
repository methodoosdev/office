import { AfterContentInit, Component, ContentChildren, EventEmitter, Input, OnInit, Output, QueryList, TemplateRef } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";

import { ToastrService } from 'ngx-toastr';
import { FormlyFieldConfig, FormlyFieldProps, FormlyFormOptions } from "@ngx-formly/core";
import { Observable } from "rxjs";

import { PrimeNGConfig, PrimeTemplate, stateAnimation } from "@primeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { fixKendoDateTime, getFieldProperties, deepCopy } from "@formlyNg";

import { FormEditToken } from "../../api/form-edit-token";
import { UnitOfWork } from "../../api/unit-of-work";
import { AfterModelChangeEvent } from "../../api/after-model-change-event";
import { DynamicDialogService } from "../../services/dynamic-dialog.service";
import { DialogResult } from "../../api/dialog-result";
import { ButtonThemeColor } from "@progress/kendo-angular-buttons";

@Component({
    selector: 'formly-edit',
    templateUrl: './formly-edit.component.html',
    providers: [{ provide: FormEditToken, useExisting: FormlyEditComponent }],
    animations: [stateAnimation]
})
export class FormlyEditComponent extends FormEditToken implements OnInit, CanComponentDeactivate, AfterContentInit {
    @Input() animate: boolean = true;
    @Input() uow: UnitOfWork;
    @Input() parentUrl: string;
    @Input() borderForm: boolean = true;
    @Input() horizontal: boolean = true;
    @Input() exportToPdfButtonVisible: boolean = false;
    @Input() saveButtonVisible: boolean = true;
    @Input() saveAndExitButtonVisible: boolean = true;
    @Input() cancelButtonVisible: boolean = true;
    @Input() customButtonVisible: boolean = false;
    @Input() customButtonTheme: ButtonThemeColor = "primary";
    @Input() customButtonLabel: string;

    form = new FormGroup({});
    options: FormlyFormOptions = {};
    fields: FormlyFieldConfig[];
    originModel: any;
    model: any;
    parentId: number;

    title: string;
    saveLabel: string;
    saveAndExitLabel: string;
    cancelLabel: string;
    aboutLabel: string;
    accessDeniedLabel: string;
    internalServerErrorLabel: string;

    @Output() onAfterModelChange: EventEmitter<AfterModelChangeEvent> = new EventEmitter();
    @Output() onExportToPdf: EventEmitter<any> = new EventEmitter();
    @Output() onCustomButton: EventEmitter<any> = new EventEmitter();
    //@Output() onModelChange: EventEmitter<any> = new EventEmitter();

    toolbarTemplate: TemplateRef<any>;
    contentTemplate: TemplateRef<any>;
    @ContentChildren(PrimeTemplate) templates: QueryList<any>;
    _customProperties: any;

    onCustomButtonClick() {
        this.onCustomButton.emit({});
    }

    constructor(
        protected config: PrimeNGConfig,
        protected route: ActivatedRoute,
        protected router: Router,
        protected dynamicDialogService: DynamicDialogService,
        protected toastrService: ToastrService) {
        super();

        this.saveLabel = this.config.getTranslation('common.save');
        this.saveAndExitLabel = this.config.getTranslation('common.saveAndExit');
        this.cancelLabel = this.config.getTranslation('common.cancel');
        this.aboutLabel = this.config.getTranslation('common.about');
        this.accessDeniedLabel = this.config.getTranslation('error.accessDenied');
        this.internalServerErrorLabel = this.config.getTranslation('error.internalServerError');
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

    get isAdded() {
        return (this.model && this.model.id === 0) ? true : false;
    }

    get hasChanges(): boolean {
        return this.form?.dirty || false;
    }

    //updateModelPartial(partial: any): void {
    //    const model = Object.assign({}, this.model, partial);

    //    this.options.resetModel(model);
    //    Object.keys(this.form.controls).forEach(key => {
    //        this.form.get(key).setErrors(null);
    //    });
    //    this.form.markAsDirty();
    //}

    getModel(): any {
        return this.model;
    }

    getForm(): FormGroup {
        return this.form;
    }

    setValue(property: string, value: any) {
        this.model[property] = value;
    }

    setParentId(parentId: number) {
        this.parentId = parentId;
    }

    //resetControl(property: string) {
    //    this.form.controls[property].reset();
    //}

    markAsPristine(): void {
        this.form.markAsPristine();
    }

    //setFieldProperties(fields: FormlyFieldConfig[]) {
    //    fields.forEach((item) => {
    //        const group = item.fieldGroup;
    //        if (Array.isArray(group)) {
    //            this.setFieldProperties(group);
    //        } else {
    //            const key = item.key as string;
    //        }
    //    });
    //};

    onDataRetrieved(data: any): void {
        const fieldProperties = getFieldProperties(data.formModel.customProperties.fields);
        const pureModel = fixKendoDateTime(data.model, fieldProperties);

        this.originModel = deepCopy(pureModel);
        this.model = pureModel;

        this._customProperties = data.formModel.customProperties;
        this.title = data.formModel.customProperties.title;
        this.fields = data.formModel.customProperties.fields;

        this.onAfterModelChange.emit({ fieldProperties: fieldProperties, model: this.model, form: this.form });
    }

    get customProperties() {
        return this._customProperties;
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
        const clone = deepCopy(this.originModel);

        this.options.resetModel(clone);
        Object.keys(this.form.controls).forEach(key => {
            this.form.get(key).setErrors(null);
        });
        this.form.markAsPristine();
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
            this.toastrService.success(this.config.getTranslation('message.successfullySaved'));

            this.form.markAsPristine();
            this.navigationBack();
        });
    }

    save(suppressConfirmation: boolean, deactivating: boolean = false) {

        return this.commit()
            .then((modelId: number) => {
                if (!suppressConfirmation) {
                    this.toastrService.success(this.config.getTranslation('message.successfullySaved'));
                }

                //this.options.updateInitialValue();
                //this.originModel = Object.assign({}, this.model);
                this.form.markAsPristine();

                if (!deactivating) { // Navigate to saved model
                    return this.router.navigateByUrl("", { skipLocationChange: true }).then(() =>
                        this.router.navigate(
                            this.parentId ? [this.parentUrl, modelId, this.parentId] : [this.parentUrl, modelId]));
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
                this.options.resetModel();
                this.navigationBack();
            })
            .catch((error: any) => {
                const failedToDelete = this.config.getTranslation('message.failedToDelete');
                this.toastrService.success(failedToDelete);
                console.error(error);
            });
    }

    exportToPdf() {
        this.onExportToPdf.emit({});
    }
}
