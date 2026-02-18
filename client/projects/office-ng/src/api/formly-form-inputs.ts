import { FormArray, FormGroup } from '@angular/forms';
import { FormlyFieldConfig, FormlyFormOptions } from '@ngx-formly/core';

export type IFormlyFormInputs = Partial<{
    form: FormGroup; /* | FormArray;*/
    fields: FormlyFieldConfig[];
    options: FormlyFormOptions;
    origin?: any;
    model: any;
    default?: any;
    properties: { [key: string]: FormlyFieldConfig };
    customProperties?: any;
    //modelChange: (value: any) => void;
}>;
