import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class TraderLookupUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderLookup' }), httpClient);
    }

    getTraderEmails(model: any): Promise<any> {
        const url = `${this.baseUrl}api/traderLookup/getTraderEmails`;

        return this.httpClient.post(url, model).toPromise();
    }

    getTraderCurrentEmail(traderId: number): Promise<any> {
        const url = `${this.baseUrl}api/traderLookup/getTraderCurrentEmail`;

        const params = new HttpParams()
            .set('traderId', traderId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}
