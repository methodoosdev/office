import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { lastValueFrom } from 'rxjs';
import { ApiConfigService, TranslationService } from "../../core";
import { AuthService, Credentials } from "../../../../dist/jwt-ng";

@Component({
    selector: "app-login",
    templateUrl: "./login.component.html",
    styleUrls: ["./login.component.scss"],
    encapsulation: ViewEncapsulation.None,
    host: {
        '[class.signin]': 'true'
    }
})
export class Login2Component implements OnInit {

    model: Credentials = { username: "", password: "", rememberMe: false };
    error = "";
    returnUrl: string | null = null;
    loading: boolean;

    companyNameLabel: string;
    loginLabel: string;
    homeLabel: string;
    signInLabel: string;
    rememberMeLabel: string;

    constructor(private translationService: TranslationService, 
        private router: Router,
        private route: ActivatedRoute,
        private authService: AuthService,
        private apiConfigService: ApiConfigService,
        private toastrService: ToastrService) {
    }

    ngOnInit() {
        // reset the login status
        this.authService.logout(false);

        // get the return url from route parameters
        this.returnUrl = this.route.snapshot.queryParams["returnUrl"];

        this.companyNameLabel = this.translationService.translate('common.companyName');
        this.loginLabel = this.translationService.translate('common.login');
        this.homeLabel = this.translationService.translate('common.home');
        this.signInLabel = this.translationService.translate('common.signIn');
        this.rememberMeLabel = this.translationService.translate('common.rememberMe');
    }

    submitForm(form: NgForm) {
        console.log(form);

        this.error = "";
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
