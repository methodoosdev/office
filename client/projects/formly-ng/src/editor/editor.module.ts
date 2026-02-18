import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { EditorModule } from '@progress/kendo-angular-editor';

import { FormlyFormFieldModule } from '../form-field/public_api';
import { FormlyFieldEditor } from './editor.type';

@NgModule({
    declarations: [FormlyFieldEditor],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        EditorModule,

        FormlyFormFieldModule,
        FormlyModule.forChild({
            types: [
                {
                    name: 'editor',
                    component: FormlyFieldEditor,
                    wrappers: ['editor-wrapper']
                }
            ],
        }),
    ],
})
export class FormlyEditorModule { }
