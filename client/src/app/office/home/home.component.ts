import { Component, OnInit } from "@angular/core";

import { AuthService, RoleName } from "@jwtNg";
import { fadeInAnimation } from "@primeNg";
import { BookmarkUnitOfWork } from "@officeNg";
import { TranslationService } from "@core";

@Component({
    selector: "home-page",
    templateUrl: "./home.component.html",
    styleUrls: ["./home.component.css"],
    animations: [fadeInAnimation],
    host: { '[@fadeInAnimation]': '' }
})
export class OfficeHomeComponent implements OnInit {
    roles1 = [RoleName.Traders, RoleName.Employees];
    roles2 = [RoleName.Administrators];

    companyNameLabel: string;
    companySubNameLabel: string;
    loginLabel: string;
    officeLabel: string;

    title = 'Bookmarks';
    bookmarks: any[] = [];

    constructor(
        private translationService: TranslationService,
        private uow: BookmarkUnitOfWork,
        private authService: AuthService) {
    }

    get isRegistered() {
        return this.authService.isAuthUserInRole(RoleName.Registered);
    }

    ngOnInit() {
        this.companyNameLabel = this.translationService.translate('common.companyName');
        this.companySubNameLabel = this.translationService.translate('common.companySubName');
        this.loginLabel = this.translationService.translate('common.login');
        this.officeLabel = this.translationService.translate('common.office');
        this.title = this.translationService.translate('menu.bookmark');

        this.uow.loadList()
            .then((data: any[]) => {
                this.bookmarks = data;
            })
            .catch((err: Error) => {
                throw err;
            });
    }

}
