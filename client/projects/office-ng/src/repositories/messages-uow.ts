import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class EmailAccountUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'emailAccount' }), httpClient);
    }

    sendTestEmail(id: number, email: string): Promise<any> {
        const url = `${this.baseUrl}api/emailAccount/sendTestEmail`;

        const params = new HttpParams()
            .set('id', id)
            .set('email', email);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

    sendInfoMessage(message: string, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/emailAccount/sendInfoMessage`;

        const params = new HttpParams()
            .set('message', message)
            .set('connectionId', connectionId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}

@Injectable()
export class EmailMessageUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'emailMessage' }), httpClient);
    }

    sendFinancialObligationEmails(connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/emailMessage/sendFinancialObligationEmails`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

    sendSelectedEmails(model: any, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/emailMessage/sendSelectedEmails`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

}

@Injectable()
export class QueuedEmailUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'queuedEmail' }), httpClient);
    }
}
