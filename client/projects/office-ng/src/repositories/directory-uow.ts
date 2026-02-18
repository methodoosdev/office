import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class BookmarkUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'bookmark' }), httpClient);
    }

    loadList(): Promise<any> {
        const url = `${this.baseUrl}api/bookmark/loadList`;

        return this.httpClient.get(url).toPromise();
    }
}
