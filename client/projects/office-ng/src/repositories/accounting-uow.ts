import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";
import { lastValueFrom } from 'rxjs';

@Injectable()
export class ESendUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'esend' }), httpClient);
    }

    loadData(model: any, connectionId: string) {
        const url = `${this.baseUrl}api/eSend/list`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    override exportToExcel(model: any): Promise<any> {
        const url = `${this.baseUrl}api/eSend/exportToExcel`;

        return this.httpClient.post(url, model, { responseType: 'blob' }).toPromise();
    }

}

@Injectable()
export class MedicalExamUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'medicalExam' }), httpClient);
    }

    override exportToExcel(model: any): Promise<any> {
        const url = `${this.baseUrl}api/medicalExam/exportToExcel`;

        return this.httpClient.post(url, model, { responseType: 'blob' }).toPromise();
    }

}

@Injectable()
export class MyDataUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'myData' }), httpClient);
    }

    save(model: any, traderId: number): Promise<any> {
        const url = `${this.baseUrl}api/myData/save`;

        const params = new HttpParams()
            .set('traderId', traderId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    override exportToExcel(model: any): Promise<any> {
        const url = `${this.baseUrl}api/myData/exportToExcel`;

        return this.httpClient.post(url, model, { responseType: 'blob' }).toPromise();
    }

}

@Injectable()
export class MyDataItemUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'myDataItem' }), httpClient);
    }
}

@Injectable()
export class ListingF4UnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'listingF4' }), httpClient);
    }

    loadData(model: any, connectionId: string) {
        const url = `${this.baseUrl}api/listingF4/list`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    submitTo(model: any, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/listingF4/submitTo`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    retrieve(traderId: number, year: number, month: number, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/listingF4/retrieve`;

        const params = new HttpParams()
            .set('traderId', traderId)
            .set('year', year)
            .set('month', month)
            .set('connectionId', connectionId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}

@Injectable()
export class ListingF5UnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'listingF5' }), httpClient);
    }

    loadData(model: any, connectionId: string) {
        const url = `${this.baseUrl}api/listingF5/list`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    submitTo(model: any, connectionId): Promise<any> {
        const url = `${this.baseUrl}api/listingF5/submitTo`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    retrieve(traderId: number, year: number, month: number, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/listingF5/retrieve`;

        const params = new HttpParams()
            .set('traderId', traderId)
            .set('year', year)
            .set('month', month)
            .set('connectionId', connectionId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}
@Injectable()
export class PeriodicF2UnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'periodicF2' }), httpClient);
    }

    retrieve(model: any, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/periodicF2/retrieve`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    generate(model: any, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/periodicF2/generate`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    submit(id: number, representative: boolean, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/periodicF2/submit`;

        const params = new HttpParams()
            .set('id', id)
            .set('representative', representative)
            .set('connectionId', connectionId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

    identityPayment(model: any, connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/periodicF2/identityPayment`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    calc(id: number): Promise<any> {
        const url = `${this.baseUrl}api/periodicF2/calc`;

        const params = new HttpParams()
            .set('id', id);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

}

@Injectable()
export class CashAvailableUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'cashAvailable' }), httpClient);
    }

    traderChanged(traderId: number): Promise<any> {
        const url = `${this.baseUrl}api/cashAvailable/traderChanged`;

        const params = new HttpParams()
            .set('traderId', traderId);

        return this.httpClient.get(url, { params: params }).toPromise();
    }

}

@Injectable()
export class AggregateAnalysisUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'aggregateAnalysis' }), httpClient);
    }

    traderChanged(traderId: number): Promise<any> {
        const url = `${this.baseUrl}api/aggregateAnalysis/traderChanged`;

        const params = new HttpParams()
            .set('traderId', traderId);

        return this.httpClient.get(url, { params: params }).toPromise();
    }
}

@Injectable()
export class MonthlyFinancialBulletinUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'monthlyFinancialBulletin' }), httpClient);
    }

    traderChanged(traderId: number, periodos: Date): Promise<any> {
        const url = `${this.baseUrl}api/monthlyFinancialBulletin/traderChanged`;

        const params = new HttpParams()
            .set('traderId', traderId)
            .set('periodos', periodos.toDateString());

        return this.httpClient.get(url, { params: params }).toPromise();
    }

    monthlyFinancialBulletinToPDF(searchModel: any, level: number) {
        const url = `${this.baseUrl}api/monthlyFinancialBulletin/exportToPdf`;

        const params = new HttpParams()
            .set('level', level);

        return this.httpClient
            .post(url, searchModel, { params: params, responseType: 'blob' }).toPromise();
    }

    resultModelExportToPdf(resultModel: any, traderId: number, periodos: Date) {
        const url = `${this.baseUrl}api/monthlyFinancialBulletin/resultModelExportToPdf`;

        const params = new HttpParams()
            .set('traderId', traderId)
            .set('date', periodos.toDateString());

        return this.httpClient
            .post(url, resultModel, { params: params, responseType: 'blob' }).toPromise();
    }

}

@Injectable()
export class PeriodicityItemsUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'periodicityItems' }), httpClient);
    }
}

@Injectable()
export class VatCalculationUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'vatCalculation' }), httpClient);
    }

    traderChanged(traderId: number): Promise<any> {
        const url = `${this.baseUrl}api/vatCalculation/traderChanged`;

        const params = new HttpParams()
            .set('traderId', traderId);

        return this.httpClient.get(url, { params: params }).toPromise();
    }
}

@Injectable()
export class SoftoneProjectUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'softoneProject' }), httpClient);
    }
}

@Injectable()
export class SoftoneProjectDetailUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'softoneProjectDetail' }), httpClient);
    }

    getProjectName(traderId: number, projectId: number) {
        const url = `${this.baseUrl}api/softoneProjectDetail/getProjectName`;

        const params = new HttpParams()
            .set('traderId', traderId)
            .set('projectId', projectId);

        return this.httpClient
            .post(url, {} , { params: params }).toPromise();
    }
}

@Injectable()
export class PayoffLiabilitiesUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'payoffLiabilities' }), httpClient);
    }
}

@Injectable()
export class VatTransferenceUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'vatTransference' }), httpClient);
    }
}

@Injectable()
export class MonthlyBCategoryBulletinUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'monthlyBCategoryBulletin' }), httpClient);
    }

    traderChanged(traderId: number, period: Date): Promise<any> {
        const url = `${this.baseUrl}api/monthlyBCategoryBulletin/traderChanged`;

        const params = new HttpParams()
            .set('traderId', traderId)
            .set('period', period.toDateString());

        return this.httpClient.get(url, { params: params }).toPromise();
    }
}


@Injectable()
export class IntertemporalCUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'intertemporalC' }), httpClient);
    }

    traderChanged(traderId: number): Promise<any> {
        const url = `${this.baseUrl}api/intertemporalC/traderChanged`;

        const params = new HttpParams()
            .set('traderId', traderId);


        return this.httpClient.get(url, { params: params }).toPromise();
    }
}


@Injectable()
export class IntertemporalBUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'intertemporalB' }), httpClient);
    }

    traderChanged(traderId: number): Promise<any> {
        const url = `${this.baseUrl}api/intertemporalB/traderChanged`;

        const params = new HttpParams()
            .set('traderId', traderId);


        return this.httpClient.get(url, { params: params }).toPromise();
    }
}

@Injectable()
export class CountingDocumentUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'countingDocument' }), httpClient);
    }
}

@Injectable()
export class ArticlesCheckUnitOfWork extends UnitOfWork {
    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'articlesCheck' }), httpClient);
    }

    view(companyId: number, nglId: number, year: number, period: number): Promise<any> {
        const url = `${this.baseUrl}api/articlesCheck/view`;

        const params = new HttpParams()
            .set('companyId', companyId)
            .set('nglId', nglId)
            .set('year', year)
            .set('period', period);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

}
