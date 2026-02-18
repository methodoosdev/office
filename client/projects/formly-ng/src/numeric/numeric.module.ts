import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { FormlyFieldNumeric } from './numeric.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldNumeric],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormlyFormFieldModule,
        InputsModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'numeric',
                    component: FormlyFieldNumeric,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            decimals: 0,
                            format: 'n0',
                            size: 'medium',
                            rounded: 'medium',
                            spinners: true
                        }
                    }
                },
                {
                    name: 'decimals',
                    extends: 'numeric',
                    defaultOptions: {
                        props: {
                            decimals: 2,
                            format: 'n2',
                            size: 'medium',
                            rounded: 'medium',
                            spinners: true
                        }
                    }
                }
            ],
        }),
    ],
})
export class FormlyNumericModule { }
