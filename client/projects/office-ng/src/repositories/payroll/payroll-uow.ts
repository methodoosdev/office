import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../../api/public-api";
import { lastValueFrom } from 'rxjs';

@Injectable()
export class EmployerUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'employer' }), httpClient);
    }
}

@Injectable()
export class ApdSubmissionUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'apdSubmission' }), httpClient);
    }

    apdSubmissions(ids: any[], month: number, year: number, connectionId: string) {
        const url = `${this.baseUrl}api/apdSubmission/import`;

        const params = new HttpParams()
            .set('month', month)
            .set('year', year)
            .set('connectionId', connectionId);

        return this.httpClient.post(url, ids, { params: params }).toPromise();
    }
}

@Injectable()
export class FmySubmissionUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'fmySubmission' }), httpClient);
    }

    fmySubmissions(ids: any[], monthFrom: number, monthTo: number, year: number, connectionId: string) {
        const url = `${this.baseUrl}api/fmySubmission/import`;

        const params = new HttpParams()
            .set('monthFrom', monthFrom)
            .set('monthTo', monthTo)
            .set('year', year)
            .set('connectionId', connectionId);

        return this.httpClient.post(url, ids, { params: params }).toPromise();
    }
}

@Injectable()
export class WorkerCatalogByTraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerCatalogByTrader' }), httpClient);
    }
}

@Injectable()
export class WorkerSickLeaveUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerSickLeave' }), httpClient);
    }
}

@Injectable()
export class WorkerLeaveUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerLeave' }), httpClient);
    }
}

@Injectable()
export class WorkerLeaveDetailUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerLeaveDetail' }), httpClient);
    }

    getWorkers(traderId: string) {
        const url = `${this.baseUrl}api/workerLeaveDetail/getWorkers`;

        const params = new HttpParams()
            .set('traderId', traderId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}

@Injectable()
export class ApdContributionUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'apdContribution' }), httpClient);
    }
}

@Injectable()
export class FmyContributionUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'fmyContribution' }), httpClient);
    }
}

@Injectable()
export class PayrollStatusUnitOfWork extends UnitOfWork {
    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'payrollStatus' }), httpClient);
    }
}

@Injectable()
export class EmployeeSalaryCostUnitOfWork extends UnitOfWork {
    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'employeeSalaryCost' }), httpClient);
    }

    getPackages(model: any) {
        const url = `${this.baseUrl}api/employeeSalaryCost/getPackages`;

        return lastValueFrom(this.httpClient.post(url, model));
    }
}


@Injectable()
export class PayrollCheckUnitOfWork extends UnitOfWork {
    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'payrollCheck' }), httpClient);
    }
}

@Injectable()
export class ApdTekaUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'apdTeka' }), httpClient);
    }

    createPeriod(year: number, period: number): Promise<any> {
        const url = `${this.baseUrl}api/apdTeka/createPeriod`;

        const params = new HttpParams()
            .set('year', year)
            .set('period', period);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

    selectCompanyPeriodDialog(): Promise<any> {
        const url = `${this.baseUrl}api/apdTeka/selectCompanyPeriodDialog`;

        return this.httpClient.get(url).toPromise();
    }

    payrollStatus(selectedIds: number[]): Promise<any> {
        const url = `${this.baseUrl}api/apdTeka/payrollStatus`;

        return this.httpClient.post(url, selectedIds).toPromise();
    }

    apdSubmit(selectedIds: number[]): Promise<any> {
        const url = `${this.baseUrl}api/apdTeka/apdSubmit`;

        return this.httpClient.post(url, selectedIds).toPromise();
    }

    tekaSubmit(selectedIds: number[]): Promise<any> {
        const url = `${this.baseUrl}api/apdTeka/tekaSubmit`;

        return this.httpClient.post(url, selectedIds).toPromise();
    }

}
