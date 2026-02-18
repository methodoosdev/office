import { Component, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { FieldArrayType } from '@ngx-formly/core';

@Component({
    selector: 'formly-field-locale-tabs',
    template: `
        <kendo-tabstrip>
            <kendo-tabstrip-tab *ngFor="let tab of field.fieldGroup; let i = index;"
                 [selected]="i === selected">
                <ng-template kendoTabContent>
                    <formly-field [field]="tab"></formly-field>
                </ng-template>
            </kendo-tabstrip-tab>
        </kendo-tabstrip>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldLocaleTabs extends FieldArrayType implements OnInit {
    ngOnInit(): void {
        const data = this.field;
        const model = this.model;

    }
    selected = 0;
}
