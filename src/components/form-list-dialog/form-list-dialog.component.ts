import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';

import { PrimeNGConfig } from '@primeNg';
import { ColumnButtonClickEvent, FormListBaseComponent } from '../../api/public-api';
import { GroupableSettings } from '@progress/kendo-angular-grid';

@Component({
    selector: 'form-list-dialog',
    templateUrl: 'form-list-dialog.component.html',
    encapsulation: ViewEncapsulation.None,
    styles: [`
        .k-window-wrap-dialog {
            display: block;
            margin: -16px;
        }
    `],
    host: {
        '[class.k-window-wrap-dialog]': 'true'
    }
})
export class FormListDialogComponent extends FormListBaseComponent {
    @Input() showSelectAll: boolean = true;
    @Input() groupable: boolean | GroupableSettings = false;
    title: string;
    selectLabel: string;
    searchLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    //events
    @Output() onColumnButtonClick: EventEmitter<ColumnButtonClickEvent> = new EventEmitter();

    constructor(
        private dialogRef: DialogRef,
        private config: PrimeNGConfig) {
        super();

        this.yesLabel = this.config.getTranslation('common.yes');
        this.noLabel = this.config.getTranslation('common.no');
        this.selectLabel = this.config.getTranslation('common.select');
        this.searchLabel = this.config.getTranslation('common.search');
        this.autofitColumnsLabel = this.config.getTranslation('common.autofit');
    }

    onColumnButtonClickEvent(event: Event, action: string, dataItem: any) {
        event.stopPropagation();
        this.onColumnButtonClick.emit({ action, dataItem });
    }

    select(): void {
        this.dialogRef.close(this.selectedKeys);
    }

    onCloseEvent() {
        this.dialogRef.close({});
    }
}
