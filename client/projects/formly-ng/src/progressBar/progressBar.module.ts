import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ProgressBarModule } from '@progress/kendo-angular-progressbar';
import { FormlyFieldProgressBar } from './progressBar.type';
import { FormlyFormFieldModule } from '../form-field/public_api';

@NgModule({
    declarations: [FormlyFieldProgressBar],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        ProgressBarModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'progressBar',
                    component: FormlyFieldProgressBar,
                    wrappers: ['form-field'],
                    defaultOptions: {
                        props: {
                            value: 0,
                        }
                    }
                }
            ],
        }),
    ],
})
export class FormlyProgressBarModule { }
