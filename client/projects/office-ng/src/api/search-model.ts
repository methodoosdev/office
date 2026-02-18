import { PagerSettings } from '@progress/kendo-angular-grid';
import { GroupDescriptor, State } from '@progress/kendo-data-query';
import { ColumnSetting } from './kendo-grid-interfaces';

export interface ISearchModel {
    readonly page: number;
    readonly pageSize: number;

    group: GroupDescriptor[]
    //availablePageSizes: string;
    //draw: string;
    start: number;
    length: number;
    quickSearch: string;
    sortField: string;
    sortOrder: string;

    title: string;
    height?: number;
    columns: ColumnSetting[];
    pagerSettings: PagerSettings;
    dataKey: string;
    __entityType: string;
}
