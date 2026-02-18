import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TabStripModule } from '@progress/kendo-angular-layout';

import { FormlyFieldTabs } from './tabs.type';
import { FormlyFormFieldModule } from '../form-field/form-field.module';

@NgModule({
    declarations: [FormlyFieldTabs],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        TabStripModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'tabs',
                    component: FormlyFieldTabs,
                    wrappers: ['form-field'],
                },
            ],
        }),
    ],
})
export class FormlyTabsModule { }
