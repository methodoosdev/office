import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class ScheduleTaskUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'scheduleTask' }), httpClient);
    }

    runNow(id: number): Promise<any> {
        const url = `${this.baseUrl}api/scheduleTask/runNow`;

        const params = new HttpParams()
            .set('id', id);

        return this.httpClient.post(url, {}, { params: params }).toPromise();
    }
}
