import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class LanguageUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'language' }), httpClient);
    }

    importResources(): Promise<any> {
        const url = `${this.baseUrl}api/language/importResources`;

        return this.httpClient.post(url, {}).toPromise();
    }
}
