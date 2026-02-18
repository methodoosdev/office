import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class FinancialObligationUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'financialObligation' }), httpClient);
    }

    retrieve(model: any, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/financialObligation/retrieve`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    createEmailMessages(connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/financialObligation/createEmailMessages`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

    createSelectedEmailMessages(model: any, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/financialObligation/createSelectedEmailMessages`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    efkaNonSalaried(model: any, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/financialObligation/efkaNonSalaried`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

}

@Injectable()
export class BankingTransactionsUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'bankingTransactions' }), httpClient);
    }

    submitTo(model: any): Promise<any> {
        const url = `${this.baseUrl}api/bankingTransactions/submitTo`;

        return this.httpClient.post(url, model, { responseType: 'blob' }).toPromise();
    }

    importTo(traderId: number, year: number, month: number): Promise<any> {
        const url = `${this.baseUrl}api/bankingTransactions/importTo`;

        const params = new HttpParams()
            .set('traderId', traderId)
            .set('year', year)
            .set('month', month);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}

@Injectable()
export class PiraeusTransactionsUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'piraeusTransactions' }), httpClient);
    }

    accounts(model: any): Promise<any> {
        const url = `${this.baseUrl}api/piraeusTransactions/accounts`;

        return this.httpClient.post(url, model, { responseType: 'blob' }).toPromise();
    }

    cards(traderId: number, year: number, month: number): Promise<any> {
        const url = `${this.baseUrl}api/piraeusTransactions/cards`;

        const params = new HttpParams()
            .set('traderId', traderId)
            .set('year', year)
            .set('month', month);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}

@Injectable()
export class NbgTransactionsUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'nbgProduction' }), httpClient);
    }

    availableBanks(model: any): Promise<any> {
        const url = `${this.baseUrl}api/nbgProduction/availableBanks`;

        return this.httpClient.post(url, model).toPromise();
    }

    token(model: any): Promise<any> {
        const url = `${this.baseUrl}api/nbgProduction/token`;

        return this.httpClient.post(url, model).toPromise();
    }
}
