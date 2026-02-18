import { Component } from '@angular/core';
import { FieldArrayType, FormlyFieldConfig } from '@ngx-formly/core';
import { BaseFormlyFieldProps } from '../form-field/base-formly-field-props';

interface RepeatSectionProps extends BaseFormlyFieldProps {
    left?: string;
    right?: string;
    header?: boolean;
}

@Component({
    selector: 'formly-wrapper-repeat-section',
    template: `
    <div class="repeat-scroll">
        <div class="repeat-section" [ngClass]="{'header': props.header === true}">

          <div class="repeat-label left"> {{ props.left }} </div>
          <div class="repeat-label right"> {{ props.right }} </div>

          <ng-container *ngFor="let f of field.fieldGroup; let i = index">
            <div class="repeat-label input">
              <formly-field [field]="f"></formly-field>
            </div>
          </ng-container>

        </div>
    </div>
  `,
})
export class FormlyWrapperRepeatSection extends FieldArrayType<FormlyFieldConfig<RepeatSectionProps>> { }
