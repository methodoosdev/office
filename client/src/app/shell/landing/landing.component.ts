import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { Router } from "@angular/router";

import { fadeInAnimation } from "@primeNg";
import { AuthService, RoleName } from "@jwtNg";
import { TranslationService } from "@core";

@Component({
    selector: "app-landing",
    templateUrl: "./landing.component.html",
    encapsulation: ViewEncapsulation.None,
    animations: [fadeInAnimation],
    host: { '[@fadeInAnimation]': '' }
})
export class LandingComponent implements OnInit {

    companyNameLabel: string;
    companySubNameLabel: string;
    loginLabel: string;
    officeLabel: string;


    get gotoLabel(): string {
        return this.isRegistered ? this.officeLabel : this.loginLabel;
    }

    constructor(
        private router: Router,
        private translationService: TranslationService,
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

    goto() {
        this.router.navigateByUrl(this.isRegistered ? "/office" : "/auth/login");
    }

    changeLanguage(localeId: string) {
        this.authService.changeLanguage(localeId)
            .then(() => {
                window.location.reload();
            });
    }

    scrollToElement($element: any): void {
        console.log($element);
        setTimeout(() => {
            $element.scrollIntoView({ behavior: "smooth", block: "start", inline: "nearest" });
        }, 200);

    }
}
