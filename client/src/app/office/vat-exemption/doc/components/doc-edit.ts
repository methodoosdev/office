import { Component, ViewChild } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { saveAs } from "@progress/kendo-file-saver";
import { debounceTime, distinctUntilChanged, Observable } from "rxjs";

import { TranslationService, UtilsService, } from "@core";
import { CanComponentDeactivate } from "@jwtNg";
import { AfterModelChangeEvent, FormEditToken, VatExemptionDocUnitOfWork } from "@officeNg";

@Component({
    selector: "vat-exemption-doc-edit",
    templateUrl: "./doc-edit.html"
})
export class VatExemptionDocEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/vat-exemption-doc';

    constructor(private translationService: TranslationService,
        public uow: VatExemptionDocUnitOfWork,
        private utils: UtilsService,
        private toastrService: ToastrService) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    private modelChanged() {
        const model = this.formEdit.getModel();
        const form = this.formEdit.getForm();

        this.uow.docChanged(model, 'modelChanged')
            .then((model) => {
                form.get('adjustedLimit').setValue(model.adjustedLimit);
                form.get('currentTransactionAlphabet').setValue(model.currentTransactionAlphabet);
                form.get('currentLimit').setValue(model.currentLimit);
                form.get('currentLimitAlphabet').setValue(model.currentLimitAlphabet);
            });
    }

    afterModelChange(e: AfterModelChangeEvent) {
        const form = this.formEdit.getForm();
        //this.formEdit.setParentId(e.model['vatExemptionSerialId']);

        setTimeout(() => {

            e.fieldProperties['supplierVat'].props['onClick'] = () => {
                const supplierVat = form.get('supplierVat').value as string;

                if (this.utils.vatValidation(supplierVat)) {
                    this.uow.getSupplierInfo(supplierVat)
                        .then((result) => {
                            form.get('supplierFullName').setValue(result.lastName);
                            form.get('supplierProfessionalActivity').setValue(result.professionalActivity);
                            form.get('supplierAddress').setValue(result.jobAddress);
                            form.get('supplierStreetNumber').setValue(result.jobStreetNumber);
                            form.get('supplierPostcode').setValue(result.jobPostcode);
                            form.get('supplierCity').setValue(result.jobCity);
                            form.get('supplierDoy').setValue(result.doy);
                        }).catch((err: Error) => {
                            throw err;
                        });
                } else {
                    this.toastrService.info(this.translationService.translate('message.invalidVat'));
                }

            };

            form.get('vatExemptionSerialId').valueChanges.subscribe((vatExemptionSerialId) => {
                this.uow.docChanged(e.model, 'serialNoChanged')
                    .then((model) => {
                        form.get('serialLimit').setValue(model.serialLimit);
                        form.get('serialNo').setValue(model.serialNo);
                        this.formEdit.setValue('serialName', model.serialName);
                        //this.formEdit.setParentId(vatExemptionSerialId);

                        form.get('limitBalance').setValue(model.limitBalance);
                        form.get('adjustedLimit').setValue(model.adjustedLimit);
                        form.get('currentLimit').setValue(model.currentLimit);
                        form.get('currentLimitAlphabet').setValue(model.currentLimitAlphabet);

                        form.get('returnDiscount').setValue(model.returnDiscount);
                        form.get('transferFromSeries').setValue(model.transferFromSeries);
                        form.get('transferToSeries').setValue(model.transferToSeries);
                        form.get('currentTransaction').setValue(model.currentTransaction);
                        form.get('currentTransactionAlphabet').setValue(model.currentTransactionAlphabet);
                    });
            });

            form.get('returnDiscount').valueChanges.pipe(debounceTime(1000), distinctUntilChanged()).subscribe((x) => {
                this.modelChanged();
            });

            form.get('transferFromSeries').valueChanges.pipe(debounceTime(1000), distinctUntilChanged()).subscribe((x) => {
                this.modelChanged();
            });

            form.get('transferToSeries').valueChanges.pipe(debounceTime(1000), distinctUntilChanged()).subscribe((x) => {
                this.modelChanged();
            });

            form.get('currentTransaction').valueChanges.pipe(debounceTime(1000), distinctUntilChanged()).subscribe((x) => {
                this.modelChanged();
            });
        }, 0);
    }

    exportToPdf() {
        const id = this.formEdit.getModel().id as number;

        this.uow.exportToPdf({}, id)
            .then((result) => {
                if (result)
                    saveAs(result, "vatExemption.pdf");
                else
                    this.toastrService.error('Pdf file cannot be created.');
            }).catch((err: Error) => {
                throw err;
            });
    }
}
