import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";
import { firstValueFrom } from "rxjs";

@Injectable()
export class ScriptTraderUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptTrader' }), httpClient);
    }

    toolDiagram(id: number, traderId: number): Promise<any> {
        const url = `${this.baseUrl}api/scriptTrader/toolDiagram`;

        const params = new HttpParams()
            .set('id', id)
            .set('traderId', traderId);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

    toolReport(id: number, traderId: number, config: string): Promise<any> {
        const url = `${this.baseUrl}api/scriptTrader/toolReport`;

        const params = new HttpParams()
            .set('id', id)
            .set('traderId', traderId)
            .set('config', config);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }

    print(model: any, traderId: number, categoryBookTypeId: number, config: string): Promise<any> {
        const url = `${this.baseUrl}api/scriptTrader/print`;

        const params = new HttpParams()
            .set('traderId', traderId)
            .set('categoryBookTypeId', categoryBookTypeId)
            .set('config', config);

        return this.httpClient.post(url, model, { params: params }).toPromise();
    }

    pivot(model: any): Promise<any> {
        const url = `${this.baseUrl}api/scriptTrader/pivot`;

        return this.httpClient.post(url, model).toPromise();
    }

    cloneScripts(sourceTraderId: number, targetTraderId: number): Promise<any> {
        const url = `${this.baseUrl}api/scriptTrader/cloneScripts`;

        const params = new HttpParams()
            .set('sourceTraderId', sourceTraderId)
            .set('targetTraderId', targetTraderId);

        return firstValueFrom(this.httpClient.post(url, {}, { params: params }));
    }

    deleteScripts(targetTraderId: number): Promise<any> {
        const url = `${this.baseUrl}api/scriptTrader/deleteScripts`;

        const params = new HttpParams()
            .set('targetTraderId', targetTraderId);

        return firstValueFrom(this.httpClient.post(url, {}, { params: params }));
    }
}

@Injectable()
export class ScriptTableNameUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptTableName' }), httpClient);
    }
}

@Injectable()
export class ScriptTableUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptTable' }), httpClient);
    }
}

@Injectable()
export class ScriptGroupUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptGroup' }), httpClient);
    }
}

@Injectable()
export class ScriptTableItemUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptTableItem' }), httpClient);
    }
}

@Injectable()
export class ScriptFieldUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptField' }), httpClient);
    }
}

@Injectable()
export class ScriptUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'script' }), httpClient);
    }
}

@Injectable()
export class ScriptItemUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptItem' }), httpClient);
    }
}

@Injectable()
export class ScriptPivotUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptPivot' }), httpClient);
    }
}

@Injectable()
export class ScriptPivotItemUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptPivotItem' }), httpClient);
    }
}

@Injectable()
export class ScriptToolUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptTool' }), httpClient);
    }

    getConfig(parentId: number): Promise<any> {
        const url = `${this.baseUrl}api/scriptTool/config`;

        const params = new HttpParams()
            .set('parentId', parentId);

        return firstValueFrom(this.httpClient.get(url, { params: params }));
    }

    downloadExcel(id: number, model: any): Promise<any> {
        const url = `${this.baseUrl}api/scriptTool/downloadExcel`;

        const params = new HttpParams()
            .set('id', id);

        return firstValueFrom(this.httpClient.post(url, model, { params: params, responseType: 'blob' }));
    }

    downloadPrototype(id: number): Promise<any> {
        const url = `${this.baseUrl}api/scriptTool/downloadPrototype`;

        const params = new HttpParams()
            .set('id', id);

        return firstValueFrom(this.httpClient.post(url, {}, { params: params, responseType: 'blob' }));
    }
}

@Injectable()
export class ScriptToolItemUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scriptToolItem' }), httpClient);
    }
}
