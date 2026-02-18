import { Component, OnInit, ViewEncapsulation } from "@angular/core";

import { AuthService, RoleName } from "@jwtNg";
import { fadeInAnimation } from "@primeNg";
import { TranslationService } from "@core";
//import { fadeInAnimation, RoleNames, TranslationService } from "@app/api";

@Component({
    selector: "app-welcome",
    templateUrl: "./welcome.component.html",
    encapsulation: ViewEncapsulation.None,
    animations: [fadeInAnimation],
    host: { '[@fadeInAnimation]': '' }
})
export class WelcomeComponent implements OnInit {
    companyNameLabel: string;
    companySubNameLabel: string;
    loginLabel: string;
    officeLabel: string;

    constructor(private translationService: TranslationService,
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
    }

}
