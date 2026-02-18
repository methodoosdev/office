import { Component, ChangeDetectionStrategy, Type } from '@angular/core';
import { FieldTypeConfig, FormlyFieldConfig } from '@ngx-formly/core';
import { FieldType, BaseFormlyFieldProps } from '../form-field/public_api';

interface EditorProps extends BaseFormlyFieldProps { }

export interface FormlyEditorFieldConfig extends FormlyFieldConfig<EditorProps> {
    type: 'editor' | Type<FormlyFieldEditor>;
}

@Component({
    selector: 'formly-field-kendo-editor',
    template: `
    <kendo-editor
      [formControl]="formControl"
      [formlyAttributes]="field"
      [resizable]="true"
      style="height: 600px;width: 100%">
    </kendo-editor>
  `,
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormlyFieldEditor extends FieldType<FieldTypeConfig<EditorProps>> { }
