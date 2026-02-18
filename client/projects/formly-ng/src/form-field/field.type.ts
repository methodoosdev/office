import { Directive, ViewChildren, QueryList } from '@angular/core';
import { FormlyFieldConfig, FieldType as CoreFieldType } from '@ngx-formly/core';
import { NgControl } from '@angular/forms';
import { FormFieldComponent } from '@progress/kendo-angular-inputs';
import { Subject } from 'rxjs';

@Directive()
export abstract class FieldType<F extends FormlyFieldConfig = FormlyFieldConfig> extends CoreFieldType<F> {
    @ViewChildren(NgControl) private set formControls(formControls: QueryList<NgControl>) {
        this.formField['control'] = formControls.first;
    }
    protected static onFireEvent = new Subject<any>();

    static get fireEvent() {
        return FieldType.onFireEvent.asObservable();
    }

    private get formField(): FormFieldComponent {
        return (this.field as any)?.['_formField'];
    }
}
