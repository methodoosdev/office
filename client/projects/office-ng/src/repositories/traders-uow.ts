import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class WorkingAreaUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workingArea' }), httpClient);
    }
}

@Injectable()
export class TraderGroupUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderGroup' }), httpClient);
    }
}

@Injectable()
export class AccountingWorkUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'accountingWork' }), httpClient);
    }
}

@Injectable()
export class TraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'trader' }), httpClient);
    }

    checkConnection(model: any) {
        const url = `${this.baseUrl}api/trader/checkConnection`;

        return this.httpClient.post(url, model).toPromise();
    }

    undoTraderDeletion(model: any) {
        const url = `${this.baseUrl}api/trader/undoTraderDeletion`;

        return this.httpClient.post(url, model).toPromise();
    }

    importPayrollIds(model: any) {
        const url = `${this.baseUrl}api/trader/importPayrollIds`;

        return this.httpClient.post(url, model).toPromise();
    }
}

@Injectable()
export class TraderKadUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderKad' }), httpClient);
    }

    importTo(selectedIds: number[], connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/traderKad/import`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, selectedIds, { params: params }).toPromise();
    }
}

@Injectable()
export class TraderBranchUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderBranch' }), httpClient);
    }

    importTo(selectedIds: number[], connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/traderBranch/import`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, selectedIds, { params: params }).toPromise();
    }
}

@Injectable()
export class TradersByEmployeeUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'tradersByEmployee' }), httpClient);
    }
}

@Injectable()
export class EmployeesByTraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'employeesByTrader' }), httpClient);
    }

    canEmployeeTraderRating(traderId: number): Promise<any> {
        const url = `${this.baseUrl}api/employeesByTrader/canEmployeeTraderRating`;

        const params = new HttpParams()
            .set('traderId', traderId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}

@Injectable()
export class SrfTraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'srfTrader' }), httpClient);
    }
}

@Injectable()
export class TaxSystemTraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'taxSystemTrader' }), httpClient);
    }

    importKeaoGredentials() {
        const url = `${this.baseUrl}api/taxSystemTrader/importKeaoGredentials`;

        return this.httpClient.post(url, {}).toPromise();
    }
}

@Injectable()
export class CheckFhmPosUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'checkFhmPos' }), httpClient);
    }

    createEmail(selectedIds: number[], connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/checkFhmPos/createEmail`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, selectedIds, { params: params }).toPromise();
    }
}

@Injectable()
export class TraderRelationshipUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderRelationship' }), httpClient);
    }
}

@Injectable()
export class TraderMembershipUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderMembership' }), httpClient);
    }
}

@Injectable()
export class TraderInfoUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderInfo' }), httpClient);
    }
}

@Injectable()
export class TraderBoardMemberUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderBoardMember' }), httpClient);
    }

    importTo(selectedIds: number[], connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/traderBoardMember/import`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, selectedIds, { params: params }).toPromise();
    }
}

@Injectable()
export class MyDataCredentialsUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'myDataCredentials' }), httpClient);
    }

    importTo(selectedIds: number[], connectionId: string): Promise<any> {
        const url = `${this.baseUrl}api/myDataCredentials/import`;

        const params = new HttpParams()
            .set('connectionId', connectionId);

        return this.httpClient.post(url, selectedIds, { params: params }).toPromise();
    }

}

@Injectable()
export class TraderRatingCategoryUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderRatingCategory' }), httpClient);
    }
}

@Injectable()
export class TraderRatingUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderRating' }), httpClient);
    }
}

@Injectable()
export class TraderRatingByTraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderRatingByTrader' }), httpClient);
    }
}

@Injectable()
export class TraderRatingReportUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderRatingReport' }), httpClient);
    }

    byEmployee(): Promise<any> {
        const url = `${this.baseUrl}api/traderRatingReport/byEmployee`;

        return this.httpClient.post(url, {}).toPromise();
    }

    byDepartment(): Promise<any> {
        const url = `${this.baseUrl}api/traderRatingReport/byDepartment`;

        return this.httpClient.post(url, {}).toPromise();
    }

    byTrader(): Promise<any> {
        const url = `${this.baseUrl}api/traderRatingReport/byTrader`;

        return this.httpClient.post(url, {}).toPromise();
    }

    bySummaryTable(): Promise<any> {
        const url = `${this.baseUrl}api/traderRatingReport/bySummaryTable`;

        return this.httpClient.post(url, {}).toPromise();
    }

    byValuationTable(): Promise<any> {
        const url = `${this.baseUrl}api/traderRatingReport/byValuationTable`;

        return this.httpClient.post(url, {}).toPromise();
    }

    byValuationTrader(): Promise<any> {
        const url = `${this.baseUrl}api/traderRatingReport/byValuationTrader`;

        return this.httpClient.post(url, {}).toPromise();
    }
}

@Injectable()
export class TraderMonthlyBillingUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderMonthlyBilling' }), httpClient);
    }
}

@Injectable()
export class TraderChargeUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'traderCharge' }), httpClient);
    }
}


