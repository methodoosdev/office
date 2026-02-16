import { AfterContentInit, Component, ContentChildren, EventEmitter, Input, OnInit, Output, QueryList, TemplateRef } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";

import { ToastrService } from 'ngx-toastr';
import { Observable } from "rxjs";

import { PrimeNGConfig, PrimeTemplate, stateAnimation } from "@primeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { fixKendoDateTime, getFieldProperties } from "@formlyNg";

import { FormlyEditNewToken } from "../../api/form-edit-token";
import { UnitOfWork } from "../../api/unit-of-work";
import { LoadModelEventEvent } from "../../api/after-model-change-event";
import { DynamicDialogService } from "../../services/dynamic-dialog.service";
import { DialogResult } from "../../api/dialog-result";
import { IFormlyFormInputs } from "../../api/formly-form-inputs";

@Component({
    selector: 'formly-edit-new',
    templateUrl: './formly-edit-new.component.html',
    providers: [{ provide: FormlyEditNewToken, useExisting: FormlyEditNewComponent }],
    animations: [stateAnimation]
})
export class FormlyEditNewComponent extends FormlyEditNewToken implements CanComponentDeactivate, OnInit, AfterContentInit {
    @Input() animate: boolean = true;
    @Input() uow: UnitOfWork;
    @Input() parentUrl: string;
    @Input() borderForm: boolean = true;
    @Input() horizontal: boolean = true;
    @Input() saveButtonVisible: boolean = true;
    @Input() title: string;
    @Input() inputs: IFormlyFormInputs;
    @Input() parentId: number;
    @Input() initializeBy: Function = () => this.initialize();

    saveLabel: string;
    saveAndExitLabel: string;
    cancelLabel: string;
    aboutLabel: string;
    accessDeniedLabel: string;
    internalServerErrorLabel: string;

    @Output() onModelChange: EventEmitter<any> = new EventEmitter();
    @Output() onLoadModelEvent: EventEmitter<LoadModelEventEvent> = new EventEmitter();
    @Output() onExportToPdf: EventEmitter<any> = new EventEmitter();

    toolbarTemplate: TemplateRef<any>;
    contentTemplate: TemplateRef<any>;
    @ContentChildren(PrimeTemplate) templates: QueryList<any>;

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

    ngOnInit(): void {
        this.initializeBy();
    }

    modelChangeEvent(value: any) {
        this.onModelChange.emit(value);
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

    private get form() {
        return this.inputs?.form;
    }

    private get model() {
        return this.inputs?.model;
    }

    get isAdded() {
        return (this.model && this.model.id === 0) ? true : false;
    }
    
    get hasChanges(): boolean {
        return !!this.form?.dirty || false;
    }
    
    setValue(property: string, value: any) {
        this.model[property] = value;
    }

    markAsPristine(): void {
        this.form.markAsPristine();
    }

    initialize() {
        this.route.params.forEach((params: any) => {

            const reportId = +params.id;
            this.parentId = +params.parentId;

            this.uow.getEntity(reportId, this.parentId, this.route.snapshot.queryParams)
                .then(result => {
                    const { fields, ...customProperties } = result.formModel.customProperties;
                    const properties = getFieldProperties(fields);
                    const pureModel = fixKendoDateTime(result.model, properties);

                    const inputs = {
                        form: new FormGroup({}),
                        options: {},
                        fields: fields,
                        origin: { ...pureModel },
                        model: pureModel,
                        properties: properties,
                        customProperties: customProperties
                    };
                    this.inputs = inputs;

                    this.title = result.formModel.customProperties.title;

                    this.onLoadModelEvent.emit({ inputs: inputs, resultFromServer: result });
                })
                .catch((err: Error) => {
                    this.navigationBack();
                    throw err;
                });
        });
    }

    cancel(): void {
        const clone = { ...this.inputs.origin };

        this.inputs.options.resetModel(clone);
        Object.keys(this.form.controls).forEach(key => {
            this.form.get(key).setErrors(null);
        });
        this.markAsPristine();
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

            this.markAsPristine();
            this.navigationBack();
        });
    }

    save(suppressConfirmation: boolean, deactivating: boolean = false) {

        return this.commit()
            .then((modelId: number) => {
                if (!suppressConfirmation) {
                    this.toastrService.success(this.config.getTranslation('message.successfullySaved'));
                }
                
                this.markAsPristine();

                if (!deactivating) { // Navigate to saved model
                    return this.router.navigateByUrl("", { skipLocationChange: true }).then(() =>
                        this.router.navigate(
                            this.parentId ? [this.parentUrl, modelId, this.parentId] : [this.parentUrl, modelId]));
                }

                return true;
            });
    }
}
