import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig } from "@ngx-formly/core";
import { IFormlyFormInputs } from "./formly-form-inputs";

export interface AfterModelChangeEvent {
    fieldProperties: { [key: string]: FormlyFieldConfig };
    model: any;
    form: FormGroup;
}

export interface LoadModelEventEvent {
    inputs: IFormlyFormInputs;
    resultFromServer: any;
}
