import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { EntityResources, UnitOfWork } from "../api/public-api";

@Injectable()
export class AssignmentPrototypeUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'assignmentPrototype' }), httpClient);
    }
}

@Injectable()
export class AssignmentPrototypeActionUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'assignmentPrototypeAction' }), httpClient);
    }
}

@Injectable()
export class AssignmentReasonUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'assignmentReason' }), httpClient);
    }
}

@Injectable()
export class AssignmentTaskUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'assignmentTask' }), httpClient);
    }
}

@Injectable()
export class AssignmentTaskActionUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') public baseUrl: string,
        public httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'assignmentTaskAction' }), httpClient);
    }

    loadList(parentId: number): Promise<any> {
        const url = `${this.baseUrl}api/assignmentTaskAction/loadList`;

        const params = new HttpParams()
            .set('parentId', parentId);

        return this.httpClient.get(url, { params: params }).toPromise();
    }
}

@Injectable()
export class AssignmentPrototypeActionsByAssignmentPrototypeUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'assignmentPrototypeActionsByAssignmentPrototype' }), httpClient);
    }
}

@Injectable()
export class AssignmentTaskActionByEmployeeUnitOfWork extends UnitOfWork {

    constructor(
        @Inject('BASE_URL') baseUrl: string,
        httpClient: HttpClient) {
        super();
        this.repository = this.createRepository(new EntityResources({ baseUrl: baseUrl, controller: 'assignmentTaskActionByEmployee' }), httpClient);
    }
}
