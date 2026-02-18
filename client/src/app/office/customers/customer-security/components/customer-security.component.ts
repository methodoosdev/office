import { Component, OnInit, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { TranslationService } from "@core";
import { CustomerSecurityUnitOfWork } from "@officeNg";

@Component({
    selector: "customer-security-page",
    templateUrl: "./customer-security.component.html",
    providers: [CustomerSecurityUnitOfWork],
    animations: [stateAnimation]
})
export class CustomerSecurityComponent implements OnInit {
    @ViewChild('form', { static: false }) form: NgForm;
    formInitialize: boolean;
    model: any;
    availableCustomerPermissions: any[];
    originCustomerPermissions: any[];
    availableCustomers: any[];
    originCustomers: any[];
    searchUser: string = '';
    searchPermission: string = '';

    titleLabel: string;
    saveLabel: string;
    searchUserLabel: string;
    searchPermissionLabel: string;
    permissionsLabel: string;
    autofitColumnsLabel: string;

    constructor(
        private router: Router,
        private uow: CustomerSecurityUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
        this.translationService = translationService as TranslationService;
    }

    ngOnInit() {
        this.titleLabel = this.translationService.translate('menu.permissions');
        this.saveLabel = this.translationService.translate('common.save');
        this.searchUserLabel = this.translationService.translate('common.searchUser');
        this.searchPermissionLabel = this.translationService.translate('common.searchPermission');
        this.permissionsLabel = this.translationService.translate('common.permissions');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');

        this.load();
    }

    navigationBack() {
        this.router.navigate(["/office"]);
    }

    onSearchUser(value: string) {
        if (value.length == 0)
            this.availableCustomers = this.originCustomers;
        else
            this.availableCustomers = this.originCustomers.filter(x => {
                return (x.name as string).toLowerCase().includes(value.toLowerCase());
            });
    }

    onSearchPermission(value: string) {
        if (value.length == 0)
            this.availableCustomerPermissions = this.originCustomerPermissions;
        else
            this.availableCustomerPermissions = this.originCustomerPermissions.filter(x => {
                return (x.name as string).toLowerCase().includes(value.toLowerCase());
            });
    }

    iFormCollection() {
        const obj: any = {};
        Object.keys(this.model).forEach((key) => {
            const item = this.model[key];

            Object.keys(item).forEach((customerId) => {

                const formKey = "allow_" + customerId;
                //const roleName = this.availableCustomers.find(x => x.id == customerId).name;

                obj[formKey] = obj[formKey] || [];
                if (item[customerId] == true) {
                    obj[formKey].push(key);
                }
            });
        });

        return obj;
    }

    get hasChanges() {
        return this.form?.dirty;
    }

    load() {
        this.uow.getProperties()
            .then((result) => {
                this.originCustomerPermissions = Object.assign(result.availableCustomerPermissions);
                this.originCustomers = Object.assign(result.availableCustomers);

                this.availableCustomerPermissions = result.availableCustomerPermissions;
                this.availableCustomers = result.availableCustomers;

                this.model = result.allowed;
            })
            .catch((err: Error) => {
                throw err;
            });
    }

    save() {
        const data = this.iFormCollection();

        this.uow.insertOrUpdate(data)
            .then(() => {
                this.searchUser = '';
                this.searchPermission = '';
                this.form.form.markAsPristine();
                this.toastrService.success(this.translationService.translate('message.modifyCompleted'));
                this.load();
            })
            .catch((err: Error) => {
                throw err;
            });
    }
}
