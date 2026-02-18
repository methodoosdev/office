import { Component } from '@angular/core';
import { FieldWrapper, FormlyFieldConfig } from '@ngx-formly/core';
import { BaseFormlyFieldProps } from './base-formly-field-props';

@Component({
    selector: 'formly-wrapper-simple-section',
    template: `
        <legend *ngIf="props?.label" class="k-form-legend">{{props.label}}</legend>
        <ng-container #fieldComponent></ng-container>
    `
})
export class FormlyWrapperSimpleSection extends FieldWrapper<FormlyFieldConfig<BaseFormlyFieldProps>> { }
