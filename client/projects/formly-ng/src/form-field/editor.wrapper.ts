import { Component, ViewChild, OnInit } from '@angular/core';
import { FormFieldComponent } from '@progress/kendo-angular-inputs';
import { ɵdefineHiddenProp as defineHiddenProp, FieldWrapper, FormlyFieldConfig } from '@ngx-formly/core';
import { BaseFormlyFieldProps } from './base-formly-field-props';

@Component({
    selector: 'formly-wrapper-kendo-editor',
    template: `
    <kendo-formfield class="k-form-field-editor">
      <ng-container #fieldComponent></ng-container>
    </kendo-formfield>
  `,
})
export class FormlyWrapperEditor extends FieldWrapper<FormlyFieldConfig<BaseFormlyFieldProps>> implements OnInit {
    @ViewChild(FormFieldComponent, { static: true }) formfield!: FormFieldComponent;

    ngOnInit() {
        defineHiddenProp(this.field, '_formField', this.formfield);
        defineHiddenProp(this.formfield, 'formControls', undefined);
        this.formfield['showErrorsInitial'] = () => this.showError;
        this.formfield['showHintsInitial'] = () => !this.showError;

        const disabledElement = this.formfield['disabledElement'].bind(this);
        this.formfield['disabledElement'] = () => {
            if (this.formfield.controlElementRefs.length === 0) {
                return false;
            }

            return disabledElement();
        };
    }
}
