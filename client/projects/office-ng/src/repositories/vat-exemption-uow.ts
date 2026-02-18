import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class VatExemptionApprovalUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'vatExemptionApproval' }), httpClient);
    }
}

@Injectable()
export class VatExemptionReportUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'vatExemptionReport' }), httpClient);
    }
}

@Injectable()
export class VatExemptionSerialUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'vatExemptionSerial' }), httpClient);
    }
}

@Injectable()
export class VatExemptionDocUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'vatExemptionDoc' }), httpClient);
    }

    docChanged(model: any, action: string): Promise<any> {
        const url = `${this.baseUrl}api/vatExemptionDoc/${action}`;

        return this.httpClient.post(url, model).toPromise();
    }

    getSupplierInfo(vat: string): Promise<any> {
        const url = `${this.baseUrl}api/vatExemptionDoc/getSupplierInfo`;

        const params = new HttpParams()
            .set('afmCalledFor', vat);

        return this.httpClient.get(url, { params: params }).toPromise();
    }
}
