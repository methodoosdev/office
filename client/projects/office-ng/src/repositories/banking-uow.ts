import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class BankingTraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'bankingTrader' }), httpClient);
    }

    account(bankBIC: string, clientUserId: string, resourceId: string, dateFrom: Date, dateTo: Date): Promise<any> {
        const url = `${this.baseUrl}api/bankingTrader/account`;

        const params = new HttpParams()
            .set('bankBIC', bankBIC)
            .set('clientUserId', clientUserId)
            .set('resourceId', resourceId)
            .set('dateFrom', dateFrom.toDateString())
            .set('dateTo', dateTo.toDateString());

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

}

@Injectable()
export class UserConnectionBankUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'userConnectionBank' }), httpClient);
    }


    getConfig(parentId: number): Promise<any> {
        const url = `${this.baseUrl}api/userConnectionBank/config`;

        const params = new HttpParams()
            .set('parentId', parentId);

        return this.httpClient.get(url, { params: params }).toPromise();
    }
}

@Injectable()
export class AvailableBankUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'availableBank' }), httpClient);
    }

}

@Injectable()
export class AccountListUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'accountList' }), httpClient);
    }

}

@Injectable()
export class CardListItemUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'cardListItem' }), httpClient);
    }

}
