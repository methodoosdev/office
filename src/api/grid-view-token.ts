import { IFormlyFormInputs } from "./formly-form-inputs";
import { ISearchModel } from "./search-model";

export abstract class GridViewToken {
    abstract searchModel: ISearchModel;
    abstract canDelete: boolean;
    abstract refresh(): Promise<void>;
    abstract clearData(): void;
    abstract loadDataSource(): Promise<void>;
    abstract GetSelectedKeys(): any[];
    abstract ClearSelectedKeys(): void;
    abstract saveState(): void;
    abstract removeState(): void;
    abstract filterSaveState(inputs: IFormlyFormInputs): void;
    abstract filterRemoveState(inputs: IFormlyFormInputs): void;
    abstract deleteRecord(): void;
    abstract newRecord(): void;
    abstract goBack(): void;
}

export abstract class GridListToken {
}
