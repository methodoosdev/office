import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyFormFieldModule } from '../form-field/public_api';
import { FormlyWrapperRepeatSection } from './repeat-section.type';
import { InputsModule } from '@progress/kendo-angular-inputs';

@NgModule({
    declarations: [FormlyWrapperRepeatSection],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormlyFormFieldModule,
        InputsModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'repeat-section',
                    component: FormlyWrapperRepeatSection,
                }
            ],
        }),
    ],
})
export class FormlyRepeatSectionModule { }
