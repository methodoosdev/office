import { Component, OnInit } from "@angular/core";
import { ListItemModel } from "@progress/kendo-angular-buttons";
import { ToastrService } from "ngx-toastr";

import { TranslationService } from "@core";
import { LanguageUnitOfWork } from "@officeNg";

@Component({
    selector: "language-list",
    templateUrl: "./language-list.html"
})
export class LanguageListComponent implements OnInit {
    settingsMenuItems: ListItemModel[] = [];
    pathUrl = 'office/language';

    constructor(
        private translationService: TranslationService,
        public uow: LanguageUnitOfWork,
        private toastrService: ToastrService) {
    }

    ngOnInit(): void {
        this.settingsMenuItems = [{
            text: this.translationService.translate('common.importResources'),
            click: () => {

                this.uow.importResources()
                    .then(() => {
                        this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
                    })
                    .catch((err: Error) => {
                        throw err;
                    });
            }
        }];
    }
}
