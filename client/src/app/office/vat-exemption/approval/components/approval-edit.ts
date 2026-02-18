import { Component, ViewChild } from "@angular/core";
import { Observable } from "rxjs";
import { ToastrService } from "ngx-toastr";

import { TranslationService } from "@core";
import { CanComponentDeactivate } from "@jwtNg";
import { AfterModelChangeEvent, FormEditToken, VatExemptionApprovalUnitOfWork } from "@officeNg";

@Component({
    selector: "vat-exemption-approval-edit",
    templateUrl: "./approval-edit.html"
})
export class VatExemptionApprovalEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/vat-exemption-approval';

    constructor(
        public uow: VatExemptionApprovalUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    afterModelChange(e: AfterModelChangeEvent) {
        //const form = this.formEdit.getForm();
        //const kendoUpload = e.fieldProperties['kendoUpload'];
        //const that = this;

        //kendoUpload.props['onUpload'] = (result: any) => {
        //    if (result.isAdded) {
        //        this.toastrService.error(this.translationService.translate('error.uploadSaveForm'));
        //    } else if ((!result.valid)) {
        //        this.toastrService.error(this.translationService.translate('validationFormError'));
        //    } else {
        //        this.uow.modelStatus(this.formEdit.getModel())
        //            .then(() => {
        //                this.uow.upload(result.data, 'Docs')
        //                    .then((result: any) => {
        //                        this.toastrService.success(this.translationService.translate('message.successfullyCompleted'));
        //                    }).catch((err: Error) => {
        //                        throw err;
        //                    });
        //            }).catch((err: Error) => {
        //                throw err;
        //            });
        //    }



        //    //const approvalNumber: string = form.get('approvalNumber').value;
        //    //if (!approvalNumber || approvalNumber.trim() === '') {
        //    //    this.toastrService.error('ApprovalNumber cannot be empty.');
        //    //    e.preventDefault();
        //    //}
        //};

        //kendoUpload.props['onSuccess'] = (e: SuccessEvent) => {
        //    if (e.operation == 'upload') {
        //        form.get('fileName').setValue(e.files[0].name);
        //        that.formEdit.markAsPristine();
        //    }
        //};

        //kendoUpload.props['onError'] = (e: UploadErrorEvent) => {
        //    if (e.operation == 'remove') {
        //    }
        //};

        ////e.fieldProperties['kendoUpload'].hideExpression = (model: any) => {
        ////    return !(model.repetitionType == 2);
        ////};

    }
}
