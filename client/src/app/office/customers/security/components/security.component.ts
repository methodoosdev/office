import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Component, Inject, OnInit, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { catchError, map, throwError } from "rxjs";
import { stateAnimation } from "@primeNg";
import { TranslationService } from "@core";

@Component({
    selector: "security-page",
    templateUrl: "./security.component.html",
    animations: [stateAnimation],
    //host: { '[@animationState]': 'active' }
})
export class SecurityComponent implements OnInit {
    @ViewChild('form', { static: false }) form: NgForm;
    formInitialize: boolean;
    model: any;
    availablePermissions: any[];
    availableCustomerRoles: any[];
    titleLabel: string;
    saveLabel: string;
    registerLabel: string;

    constructor(private httpClient: HttpClient,
        @Inject('BASE_URL') private baseUrl: string,
        private translationService: TranslationService) {
    }

    ngOnInit() {
        this.titleLabel = this.translationService.translate('menu.securityPermissions');
        this.saveLabel = this.translationService.translate('common.save');
        this.registerLabel = this.translationService.translate('common.register');

        this.load();
    }

    navigationBack() {
    }

    iFormCollection() {
        const obj: any = {};
        Object.keys(this.model).forEach((key) => {
            const item = this.model[key];

            Object.keys(item).forEach((roleId) => {

                const formKey = "allow_" + roleId;
                const roleName = this.availableCustomerRoles.find(x => x.id == roleId).name;

                obj[formKey] = obj[formKey] || [];
                if (item[roleId] == true) {
                    obj[formKey].push(key);
                }
            });
        });

        return obj;
    }

    get hasChanges() {
        return this.form?.dirty;
    }

    save() {
        const data = this.iFormCollection();

        this.httpClient
            .post<any>(`${this.baseUrl}api/security/permissions`, data)
            .pipe(
                catchError((error: HttpErrorResponse) => throwError(error))
            )
            .subscribe(() => {
                console.log('Successfully saved.');
                this.load();
                this.form.form.markAsPristine();
            });
    }

    register() {
        this.httpClient
            .post<any>(`${this.baseUrl}api/security/registerPermissions`, {})
            .pipe(
                catchError((error: HttpErrorResponse) => throwError(error))
            )
            .subscribe(() => {
                console.log('Successfully register.');
                this.load();
                this.form.form.markAsPristine();
            });
    }

    load() {
        this.httpClient
            .get<any>(`${this.baseUrl}api/security/permissions`)
            .pipe(
                map(response => response || {}),
                catchError((error: HttpErrorResponse) => throwError(error))
            )
            .subscribe(result => {
                console.log(result);
                if (!this.formInitialize) {
                    this.availablePermissions = result.availablePermissions;
                    this.availableCustomerRoles = result.availableCustomerRoles;
                    this.formInitialize = true;
                }
                this.model = result.allowed;
            });
    }
}
