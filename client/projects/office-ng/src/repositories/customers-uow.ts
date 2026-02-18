import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class CustomerUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'customer' }), httpClient);
    }

    changePassword(model: any): Promise<any> {
        const url = `${this.baseUrl}api/customer/changePassword`;

        return this.httpClient.post(url, model).toPromise();
    }

    prepareParentIdDialog(): Promise<any> {
        const url = `${this.baseUrl}api/customer/prepareParentIdDialog`;

        return this.httpClient.get(url, {}).toPromise();
    }
}

@Injectable()
export class CustomerPermissionsByCustomerUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'customerPermissionsByCustomer' }), httpClient);
    }

    insertCustomerPermissions(parentId: number, customerId: number): Promise<any> {
        const url = `${this.baseUrl}api/customerPermissionsByCustomer/insertCustomerPermissions`;

        const params = new HttpParams()
            .set('parentId', parentId)
            .set('customerId', customerId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}

@Injectable()
export class CustomerPermissionUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'customerPermission' }), httpClient);
    }

    autoInsertMissingPermission(): Promise<any> {
        const url = `${this.baseUrl}api/customerPermission/autoInsertMissingPermission`;

        return this.httpClient.post(url, {}).toPromise();
    }
}

@Injectable()
export class CustomerRoleUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'customerRole' }), httpClient);
    }
}

@Injectable()
export class CustomerOnlineUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'customerOnline' }), httpClient);
    }
}

@Injectable()
export class CustomerSecurityUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'customerSecurity' }), httpClient);
    }

    getProperties(): Promise<any> {
        return this.repository.getCreate();
    }

    insertOrUpdate(model: any): Promise<any> {
        return this.create(model);
    }

}

@Injectable()
export class CustomerActivityUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'customerActivity' }), httpClient);
    }

    lastActivity(): Promise<any> {
        const url = `${this.baseUrl}api/customerActivity/lastActivity`;

        return this.httpClient.get(url).toPromise();
    }
}
