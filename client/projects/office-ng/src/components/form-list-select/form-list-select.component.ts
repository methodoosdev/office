import { Component, EventEmitter, Input, Output } from '@angular/core';

import { GroupableSettings, RowClassArgs } from '@progress/kendo-angular-grid';
import { PrimeNGConfig } from '@primeNg';

import { ColumnButtonClickEvent, FormListBaseComponent } from '../../api/public-api';

@Component({
    selector: 'form-list-select',
    templateUrl: 'form-list-select.component.html'
})
export class FormListSelectComponent extends FormListBaseComponent {
    @Input() showSelectAll: boolean = true;
    @Input() groupable: boolean | GroupableSettings = false;
    @Input() rowCallback = (args: RowClassArgs) => ({
        "k-disabled": false
    });

    searchLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    //events
    @Output() onColumnButtonClick: EventEmitter<ColumnButtonClickEvent> = new EventEmitter();

    constructor(
        private config: PrimeNGConfig) {
        super();

        this.yesLabel = this.config.getTranslation('common.yes');
        this.noLabel = this.config.getTranslation('common.no');
        this.searchLabel = this.config.getTranslation('common.search');
        this.autofitColumnsLabel = this.config.getTranslation('common.autofit');
    }

    onColumnButtonClickEvent(event: Event, action: string, dataItem: any) {
        event.stopPropagation();
        this.onColumnButtonClick.emit({ action, dataItem });
    }
}
