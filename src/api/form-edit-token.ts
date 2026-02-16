import { FormGroup } from "@angular/forms";
import { Observable } from "rxjs";

export abstract class FormEditToken {
    abstract model: any;
    //abstract updateModelPartial(partial: any): void;
    //abstract resetControl(property: string): void;
    abstract setValue(property: string, value: any): void;
    abstract setParentId(parentId: number): void;
    abstract getModel(): any;
    abstract customProperties: any;
    abstract getForm(): FormGroup;
    abstract markAsPristine(): void;
    abstract canDeactivate(): Observable<boolean> | Promise<boolean> | boolean;
}

export abstract class FormlyEditNewToken {
    abstract canDeactivate(): Observable<boolean> | Promise<boolean> | boolean;
}
