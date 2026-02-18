import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { FormlySelectModule as FormlyCoreSelectModule } from '@ngx-formly/core/select';

import { FormlyFieldAutocomplete } from './autocomplete.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldAutocomplete],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        DropDownsModule,

        FormlyFormFieldModule,
        FormlyCoreSelectModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'autocomplete',
                    component: FormlyFieldAutocomplete,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            options: []
                        }
                    }
                }
            ],
        }),
    ],
})
export class FormlyAutocompleteModule { }
