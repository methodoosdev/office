import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormlyModule } from '@ngx-formly/core';
import { ReactiveFormsModule } from '@angular/forms';
import { FormFieldModule, InputsModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { ExpansionPanelModule } from '@progress/kendo-angular-layout';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';

import { FormlyWrapperFormField } from './form-field.wrapper';
import { FormlyWrapperExpansionPanel } from './expansion-panel.wrapper';
import { FormlyWrapperSimpleSection } from './simple-section';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { FormlyWrapperEditor } from './editor.wrapper';
import { FormlyWrapperSimple } from './simple';

@NgModule({
    declarations: [
        FormlyWrapperFormField, FormlyWrapperExpansionPanel,
        FormlyWrapperSimpleSection, FormlyWrapperSimple, FormlyWrapperEditor
    ],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormFieldModule,

        LabelModule,
        ExpansionPanelModule,
        DropDownsModule,
        InputsModule,
        DateInputsModule,
        ButtonsModule,
        //UploadsModule,


        FormlyModule.forChild({
            wrappers: [
                { name: 'form-field', component: FormlyWrapperFormField },
                { name: 'expansion-panel', component: FormlyWrapperExpansionPanel },
                { name: 'simple-section', component: FormlyWrapperSimpleSection },
                { name: 'simple', component: FormlyWrapperSimple },
                { name: 'editor-wrapper', component: FormlyWrapperEditor },
            ],
        }),
    ],
})
export class FormlyFormFieldModule { }
