import { Component, OnInit } from "@angular/core";
import { ToastrService } from "ngx-toastr";

import { TranslationService } from "@core";
import { NbgTransactionsUnitOfWork } from "@officeNg";

@Component({
    selector: "nbg-list",
    templateUrl: "./nbg-list.html"
})
export class NbgListComponent implements OnInit {

    constructor(
        private translationService: TranslationService,
        private uow: NbgTransactionsUnitOfWork,
        private toastrService: ToastrService) {
    }

    ngOnInit(): void {
        this.uow.availableBanks({}).then((result) => {
            console.log(result);
        });
    }
}
