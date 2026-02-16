import { HttpClient, HttpParams } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';

export interface IEntityResources {
    baseUrl: string;
    controller: string;
    actionPreviewGet?: string;
    actionPreviewPost?: string;
    actionListGet?: string;
    actionListPost?: string;
    actionCreateGet?: string;
    actionCreatePost?: string;
    actionEditGet?: string;
    actionEditPost?: string;
    actionDelete?: string;
    actionDeleteSelected?: string;
    actionSetPrimary?: string;
    actionExportToExcel?: string;
    actionExportToPdf?: string;
    actionImportMapping?: string;
    actionRemoveMapping?: string;
    actionImport?: string;
    actionModelStatus?: string;
    actionUpload?: string;
    actionLookup?: string;
}

export class EntityResources {
    public baseUrl: string;
    public controller: string;
    public actionPreviewGet: string;
    public actionPreviewPost: string;
    public actionListGet: string;
    public actionListPost: string;
    public actionCreateGet: string;
    public actionCreatePost: string;
    public actionEditGet: string;
    public actionEditPost: string;
    public actionDelete: string;
    public actionDeleteSelected: string;
    public actionSetPrimary: string;
    public actionExportToExcel: string;
    public actionExportToPdf: string;
    public actionImportMapping: string;
    public actionRemoveMapping: string;
    public actionImport: string;
    public actionModelStatus: string;
    public actionUpload: string;
    public actionLookup: string;

    constructor(options: IEntityResources) {
        this.baseUrl = options.baseUrl;
        this.controller = options.controller;
        this.actionPreviewGet = options.actionPreviewGet || 'preview';
        this.actionPreviewPost = options.actionPreviewPost || 'preview';
        this.actionListGet = options.actionListGet || 'list';
        this.actionListPost = options.actionListPost || 'list';
        this.actionCreateGet = options.actionCreateGet || 'create';
        this.actionCreatePost = options.actionCreatePost || 'create';
        this.actionEditGet = options.actionEditGet || 'edit';
        this.actionEditPost = options.actionEditPost || 'edit';
        this.actionDelete = options.actionDelete || 'delete';
        this.actionDeleteSelected = options.actionDeleteSelected || 'deleteSelected';
        this.actionSetPrimary = options.actionSetPrimary || 'setPrimary';
        this.actionExportToExcel = options.actionExportToExcel || 'exportToExcel';
        this.actionExportToPdf = options.actionExportToPdf || 'exportToPdf';
        this.actionImportMapping = options.actionImportMapping || 'importMapping';
        this.actionRemoveMapping = options.actionRemoveMapping || 'removeMapping';
        this.actionImport = options.actionImport || 'import';
        this.actionModelStatus = options.actionModelStatus || 'modelStatus';
        this.actionUpload = options.actionUpload || 'upload';
        this.actionLookup = options.actionLookup || 'lookup';
    }
}

export interface EntityBase {
    id: number;
}

export interface IRepository {
    resources: EntityResources;
    getList(parentId?: number): Promise<any>;
    getCreate(parentId?: number, ...queryParams: { [key: string]: any; }[]): Promise<any>;
    getEdit(id: number): Promise<any>;
    list(model: any, parentId?: number, params?:{ [key: string]: any; }): Promise<any>;
    create(model: any, parentId?: number): Promise<any>;
    edit(model: any, parentId?: number): Promise<any>;
    delete(id: number): Promise<any>;
    deleteSelected(selections: number[]): Promise<any>;
    setPrimary(id: number, parentId?: number): Promise<any>;
    exportToExcel(model: any, parentId?: number): Promise<any>;
    exportToPdf(model: any, parentId?: number): Promise<Blob>;
    importMapping(model: any, parentId?: number): Promise<any>;
    removeMapping(model: any, parentId?: number): Promise<any>;
    import(selections: number[], parentId?: number): Promise<any>;
    modelStatus(model: any): Promise<any>;
    upload(model: any, path: string): Promise<any>;
    lookup(): Promise<any>;
}

export class Repository implements IRepository {

    constructor(
        public resources: EntityResources,
        public httpClient: HttpClient,
    ) { }

    getList(parentId: number = null): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionListGet}`;

        let params = new HttpParams();

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.get(url, { params: params }));
    }

    private isDefined(value: any): boolean {
        return typeof value !== 'undefined' && value !== null;
    }

    getCreate(parentId: number = null, queryParams: { [key: string]: any; }): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionCreateGet}`;
        //const argumentList: { [name: string]: any }[] = [...args];

        let params = new HttpParams({ fromObject: queryParams });

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.get(url, { params: params }));
    }

    getEdit(id: number): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionEditGet}`;

        const params = new HttpParams()
            .set('id', id);

        return lastValueFrom(this.httpClient.get(url, { params: params }));
    }
    createHttpParams(...args: { [param: string]: any }[]): HttpParams {
        let httpParams = new HttpParams();

        // Iterate over the argument objects and set them in HttpParams
        args.forEach(arg => {
            Object.keys(arg).forEach(key => {
                httpParams = httpParams.set(key, arg[key]);
            });
        });

        return httpParams;
    }
    list(model: any, parentId: number, params: { [key: string]: any; }): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionListPost}`;

        let httpParams = new HttpParams({ fromObject: params });

        if (parentId)
            httpParams = httpParams.set('parentId', parentId);

        return lastValueFrom(this.httpClient.post(url, model, { params: httpParams }));
    }

    create(model: any, parentId: number = null): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionCreatePost}`;

        let params = new HttpParams();

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.post(url, model, { params: params }));
    }

    edit(model: any, parentId: number = null): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionEditPost}`;

        let params = new HttpParams();

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.post(url, model, { params: params }));
    }

    delete(id: number): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionDelete}`;

        const params = new HttpParams()
            .set('id', id);

        return lastValueFrom(this.httpClient.post(url, {}, { params: params }));
    }

    deleteSelected(selections: number[]): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionDeleteSelected}`;

        return lastValueFrom(this.httpClient.post(url, selections));
    }

    setPrimary(id: number, parentId: number = null): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionSetPrimary}`;

        let params = new HttpParams()
            .set('id', id);

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.post(url, {}, { params: params }));
    }

    exportToExcel(model: any, parentId: number): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionExportToExcel}`;

        let params = new HttpParams();

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.post(url, model, { params: params }));
    }

    exportToPdf(model: any, parentId: number): Promise<Blob> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionExportToPdf}`;

        let params = new HttpParams();

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.post(url, model, { params: params, responseType: 'blob' }));
    }

    importMapping(model: any, parentId: number): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionImportMapping}`;

        let params = new HttpParams();

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.post(url, model, { params: params }));
    }

    removeMapping(model: any, parentId: number): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionRemoveMapping}`;

        let params = new HttpParams();

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.post(url, model, { params: params }));
    }

    import(selections: number[], parentId: number): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionImport}`;

        let params = new HttpParams();

        if (parentId)
            params = params.set('parentId', parentId);

        return lastValueFrom(this.httpClient.post(url, selections, { params: params }));
    }

    modelStatus(model: any): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionModelStatus}`;

        return lastValueFrom(this.httpClient.post(url, model));
    }

    upload(model: any, path: string): Promise<any> {
        const url = `${this.resources.baseUrl}api/upload/${this.resources.actionUpload}`;

        const params = new HttpParams()
            .set('path', path);

        return lastValueFrom(this.httpClient.post(url, model, { params: params }));
    }

    lookup(): Promise<any> {
        const url = `${this.resources.baseUrl}api/${this.resources.controller}/${this.resources.actionLookup}`;

        return lastValueFrom(this.httpClient.get(url));
    }
}
