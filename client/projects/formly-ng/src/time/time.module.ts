import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { FormlyFieldTime } from './time.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldTime],
    imports: [
        CommonModule,
        DateInputsModule,
        ReactiveFormsModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'time',
                    component: FormlyFieldTime,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            nullable: false,
                            format: 'HH:mm',
                        },
                    },
                }
            ],
        }),
    ],
})
export class FormlyTimeModule { }
