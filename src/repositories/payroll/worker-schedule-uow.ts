import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../../api/public-api";

@Injectable()
export class WorkerScheduleShiftByTraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerScheduleShiftByTrader' }), httpClient);
    }
}

@Injectable()
export class WorkerScheduleByTraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerScheduleByTrader' }), httpClient);
    }
}

@Injectable()
export class WorkerScheduleByEmployeeUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerScheduleByEmployee' }), httpClient);
    }

    setModeType(id: number, typeId: number): Promise<any> {
        const url = `${this.baseUrl}api/workerScheduleByEmployee/setModeType`;

        const params = new HttpParams()
            .set('id', id)
            .set('typeId', typeId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

}

@Injectable()
export class WorkerScheduleCheckUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerScheduleCheck' }), httpClient);
    }
}

@Injectable()
export class WorkerScheduleLogUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerScheduleLog' }), httpClient);
    }
}

@Injectable()
export class WorkerSchedulePendingUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerSchedulePending' }), httpClient);
    }
}

@Injectable()
export class WorkerScheduleSubmitUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'workerScheduleSubmit' }), httpClient);
    }

    update(model: any): Promise<any> {
        const url = `${this.baseUrl}api/workerScheduleSubmit/update`;

        return this.httpClient.post(url, model).toPromise();
    }

    sendEmail(parentId: number): Promise<any> {
        const url = `${this.baseUrl}api/workerScheduleSubmit/sendEmail`;

        const params = new HttpParams()
            .set('parentId', parentId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

    check(parentId: number): Promise<any> {
        const url = `${this.baseUrl}api/workerScheduleSubmit/check`;

        const params = new HttpParams()
            .set('parentId', parentId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}
