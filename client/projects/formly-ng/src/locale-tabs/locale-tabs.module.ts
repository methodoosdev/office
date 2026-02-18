import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TabStripModule } from '@progress/kendo-angular-layout';

import { FormlyFieldLocaleTabs } from './locale-tabs.type';
import { FormlyFormFieldModule } from '../form-field/form-field.module';

@NgModule({
    declarations: [FormlyFieldLocaleTabs],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        TabStripModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'locale-tabs',
                    component: FormlyFieldLocaleTabs,
                    wrappers: ['form-field'],
                },
            ],
        }),
    ],
})
export class FormlyLocaleTabsModule { }
