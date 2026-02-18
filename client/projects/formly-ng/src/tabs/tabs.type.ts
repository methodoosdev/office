import { Component, ChangeDetectionStrategy } from '@angular/core';
import { FieldType, FieldTypeConfig } from '@ngx-formly/core';

@Component({
    selector: 'formly-field-tabs',
    template: `
        <kendo-tabstrip>
            <kendo-tabstrip-tab *ngFor="let tab of field.fieldGroup; let i = index;"
                [title]="tab.props.label" [selected]="i === selected">
                <ng-template kendoTabContent>
                    <formly-field [field]="tab"></formly-field>
                </ng-template>
            </kendo-tabstrip-tab>
        </kendo-tabstrip>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldTabs extends FieldType<FieldTypeConfig> {
    selected = 0;
}
