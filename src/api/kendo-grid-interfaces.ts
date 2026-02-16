import { ButtonThemeColor } from "@progress/kendo-angular-buttons";

export interface CreateOrEditEvent {
    pathUrl: string;
    id: number,
    parentId: number
}

export interface ColumnButtonClickEvent {
    action: string;
    dataItem: any;
}

export interface ColumnSetting {
    field: string;
    fieldType: string;
    title: string;
    titleTooltip: string;
    media: string;
    theme: ButtonThemeColor;
    hidden: boolean;
    format?: string;
    class?: string;
    filterType: "text" | "numeric" | "boolean" | "date";
    style: {
        [key: string]: string;
    };
    headerStyle: {
        [key: string]: string;
    };
    disabledField?: string;
    backgroundField?: string;
    link?: string;
    target?: string;
    width?: number;
    order?: number;
}

export declare type CellButtonDisabledFn = (column: ColumnSetting, dataItem: any) => string | string[] | Set<string> | {
    [key: string]: any;
};
