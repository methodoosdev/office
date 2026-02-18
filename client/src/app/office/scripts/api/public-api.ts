import { FieldDataType } from "@progress/kendo-angular-grid";
import { AggregateDescriptor } from "@progress/kendo-data-query";

export interface GridColumn {
    field: string;
    title: string;
    hidden?: boolean;
    width?: number;
    minResizableWidth?: number;
    filter?: FieldDataType;
    format?: string;
    textAlign?: string;
    headerAlign?: string;
}
export interface GridResponse<T> {
    title: string;
    data: T[];
    columns: GridColumn[];
    aggregates: AggregateDescriptor[];
}
