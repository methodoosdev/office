import { HttpErrorResponse } from '@angular/common/http';
import { Component, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { AuthService } from '@jwtNg';
import { fadeInAnimation, PrimeNGConfig } from "@primeNg";
import { ToastrService } from 'ngx-toastr';
import { ApiConfigService, TranslationService } from '@core';
import { lastValueFrom } from 'rxjs';

@Component({
    selector: "app-login",
    templateUrl: './login.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [fadeInAnimation],
    host: { '[@fadeInAnimation]': '' }
})
export class LoginComponent {

    model: any = { email: "", password: "", rememberMe: true };
    returnUrl: string | null = null;
    loading: boolean;

    companyNameLabel: string;
    loginLabel: string;
    homeLabel: string;
    signInLabel: string;
    rememberMeLabel: string;
    userNameLabel: string;
    passwordLabel: string;

    constructor(
        public config: PrimeNGConfig,
        private translationService: TranslationService,
        private router: Router,
        private route: ActivatedRoute,
        private authService: AuthService,
        private apiConfigService: ApiConfigService,
        private toastrService: ToastrService) {
    }
    
    ngOnInit() {
        // reset the login status
        Promise.resolve(null).then(() => {
            this.authService.logout(false);
        }).then(() => {
            this.apiConfigService.loadApiConfig();
        });

        // get the return url from route parameters
        this.returnUrl = this.route.snapshot.queryParams["returnUrl"];

        this.userNameLabel = this.config.getTranslation("userName");
        this.passwordLabel = this.config.getTranslation("password");
        this.companyNameLabel = this.translationService.translate('common.companyName');
        this.loginLabel = this.translationService.translate('common.login');
        this.homeLabel = this.translationService.translate('common.home');
        this.signInLabel = this.translationService.translate('common.signIn');
        this.rememberMeLabel = this.translationService.translate('common.rememberMe');
    }

    submit() {
        this.loading = true;

        lastValueFrom(this.authService.login(this.model))
            .then((isLoggedIn: boolean) => {
                return this.apiConfigService.loadApiConfig().then(() => {
                    return isLoggedIn;
                })
            })
            .then((isLoggedIn: boolean) => {
                if (isLoggedIn) {
                    if (this.returnUrl) {
                        this.router.navigate([this.returnUrl]);
                    } else {
                        this.router.navigate(["/office"]);
                    }
                }
            })
            .catch((e: HttpErrorResponse) => {
                console.error("Login error", e);
                this.toastrService.error(e.error);
            })
            .finally(() => {
                this.loading = false;
            });
    }
}
