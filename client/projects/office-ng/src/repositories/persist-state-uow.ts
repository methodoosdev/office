import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class PersistStateUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'persistState' }), httpClient);
    }

    saveState(obj: any, modelType: string): Promise<any> {
        const url = `${this.baseUrl}api/persistState/saveState`;

        const params = new HttpParams()
            .set('modelType', modelType);

        return this.httpClient.post(url, obj, { params: params }).toPromise();
    }

    removeState(modelType: string): Promise<any> {
        const url = `${this.baseUrl}api/persistState/removeState`;

        const params = new HttpParams()
            .set('modelType', modelType);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}
