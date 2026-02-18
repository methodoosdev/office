import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { FormlyFieldDate } from './date.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldDate],
    imports: [
        CommonModule,
        DateInputsModule,
        ReactiveFormsModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'date',
                    component: FormlyFieldDate,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            nullable: false
                        },
                    },
                },
                {
                    name: 'dateTime',
                    extends: 'date'
                },
                {
                    name: 'monthDate',
                    extends: 'date'
                },
                {
                    name: 'yearDate',
                    extends: 'date'
                }
            ],
        }),
    ],
})
export class FormlyDateModule { }
