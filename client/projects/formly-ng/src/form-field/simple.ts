import { Component, OnInit } from '@angular/core';
import { ɵdefineHiddenProp as defineHiddenProp, FieldWrapper } from '@ngx-formly/core';

@Component({
    selector: 'formly-wrapper-simple',
    template: `
        <ng-container #fieldComponent></ng-container>
    `
})
export class FormlyWrapperSimple extends FieldWrapper implements OnInit {
    ngOnInit() {
        // Minimal API the Kendo error-handler touches
        defineHiddenProp(this.field, '_formField', {
            control: this.formControl,
            controlElementRefs: [],       // you can push element refs if you want
            disabledElement: () => false, // not used here, just a safe default
            showErrorsInitial: () => this.showError,
            showHintsInitial: () => !this.showError,
        });
    }
}
