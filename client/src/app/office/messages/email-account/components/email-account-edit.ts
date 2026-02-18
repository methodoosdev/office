import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ToastrService } from "ngx-toastr";

import { AfterModelChangeEvent, FormEditToken, EmailAccountUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { ChatHubService, TranslationService } from "@core";
import { Observable } from "rxjs";

@Component({
    selector: "email-account-edit",
    templateUrl: "./email-account-edit.html",
    providers: [ChatHubService]
})
export class EmailAccountEditComponent implements OnInit, OnDestroy, CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/email-account';

    constructor(
        private translationService: TranslationService,
        public uow: EmailAccountUnitOfWork,
        private chatHubService: ChatHubService,
        private toastrService: ToastrService) {
    }

    ngOnDestroy(): void {
        //this.chatHubService.stop();
    }

    ngOnInit(): void {
        //this.chatHubService.start();
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    afterModelChange(e: AfterModelChangeEvent) {
        const sendTestEmailTo = e.fieldProperties['sendTestEmailTo'];
        const infoMessage = e.fieldProperties['infoMessage'];

        sendTestEmailTo.props['onClick'] = () => {

            this.uow.sendTestEmail(e.model.id, e.model.sendTestEmailTo)
                .then(() => {
                    this.toastrService.success(this.translationService.translate('message.testEmailSuccess'));
                }).catch((err: Error) => {
                    throw err;
                });
        };

        infoMessage.props['onClick'] = () => {

            this.uow.sendInfoMessage(e.model.infoMessage, this.chatHubService?.connectionId)
                .then(() => {
                    this.toastrService.success(this.translationService.translate('message.sendInfoMessage'));
                }).catch((err: Error) => {
                    throw err;
                });
        };
    }
}
