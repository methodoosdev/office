import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { EntityResources, IRepository, Repository } from './repository';

@Injectable()
export class UnitOfWork {
    private _repository: IRepository;

    private static committedSubject = new Subject<number>();

    static get committed() {
        return UnitOfWork.committedSubject.asObservable();
    }

    protected get repository() {
        return this._repository;
    }

    protected set repository(value: IRepository) {
        this._repository = value;
    }

    getEntity(id: number, parentId: number = null, params: { [key: string]: any; }): Promise<any> {
        if (id > 0) {
            return this.repository.getEdit(id);
        } else {
            return this.repository.getCreate(parentId, params);
        }
    }

    edit(model: any, parentId: number = null): Promise<any> {
        return this.repository.edit(model, parentId).then((result: number) => {
            UnitOfWork.committedSubject.next(result);
            return result;
        });
    }

    protected create(model: any, parentId: number = null): Promise<any> {
        return this.repository.create(model, parentId).then((result: number) => {
            UnitOfWork.committedSubject.next(result);
            return result;
        });
    }

    commit(model: any, parentId: number = null): Promise<number> {
        if (model.id > 0) {
            return this.repository.edit(model, parentId).then((result: number) => {
                UnitOfWork.committedSubject.next(result);
                return result;
            });
        } else {
            return this.repository.create(model, parentId).then((result: number) => {
                UnitOfWork.committedSubject.next(result);
                return result;
            });
        }
    }

    loadProperties(parentId: number = null) {
        return this.repository.getList(parentId);
    }

    loadDataSource(model: any, parentId: number = null, params: { [key: string]: any; } = null) {
        return this.repository.list(model, parentId, params);
    }

    delete(id: number) {
        return this.repository.delete(id);
    }

    deleteSelected(ids: number[]) {
        return this.repository.deleteSelected(ids);
    }

    setPrimary(id: number, parentId: number = null) {
        return this.repository.setPrimary(id, parentId);
    }

    exportToExcel(model: any, parentId: number = null) {
        return this.repository.exportToExcel(model, parentId);
    }

    exportToPdf(model: any, parentId: number = null) {
        return this.repository.exportToPdf(model, parentId);
    }

    importMapping(model: any, parentId: number = null) {
        return this.repository.importMapping(model, parentId);
    }

    removeMapping(model: any, parentId: number = null) {
        return this.repository.removeMapping(model, parentId);
    }

    import(ids: number[], parentId: number = null) {
        return this.repository.import(ids, parentId);
    }

    modelStatus(model: any) {
        return this.repository.modelStatus(model);
    }

    upload(model: any, path: string) {
        return this.repository.upload(model, path);
    }

    lookup() {
        return this.repository.lookup();
    }

    protected createRepository(entityResources: EntityResources, httpClient: HttpClient) {
        return new Repository(entityResources, httpClient);
    }
}

