import { Component, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";

import { stateAnimation } from "@primeNg";
import { getFieldProperties } from "@formlyNg";
import { FormlyFieldConfig } from "@ngx-formly/core";
import { GridViewToken, IFormlyFormInputs, MyDataItemUnitOfWork } from '@officeNg';
import { distinctUntilChanged, map, startWith } from "rxjs/operators";

@Component({
    selector: "myData-item-list",
    templateUrl: "./myData-item-list.html",
    animations: [stateAnimation]
})
export class MyDataItemListComponent {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    pathUrl = 'office/myData-item';

    infoInputs: IFormlyFormInputs;
    queryParams: string = "traderId,isIssuer,docTypeId"; // without blanks

    constructor(public uow: MyDataItemUnitOfWork) { }

    modelChangeEvent(value: any) {
        this.table?.clearData();
    }
    
    loadPropertiesEvent(result: any) {
        const { fields, ...customPropertiesRest } = result.infoFormModel.customProperties;
        const properties = getFieldProperties(fields);

        const infoInputs = {
            form: new FormGroup({}),
            options: {},
            fields: fields,
            model: result.infoModel,
            properties: properties,
            customProperties: customPropertiesRest
        };

        //infoInputs.properties['listButton']['expressions'] = {
        //    'props.disabled': (field: FormlyFieldConfig) => {
        //        return !((field.model['traderId'] as any as number) > 0);
        //    }
        //};

        infoInputs.properties['docTypeId']['hooks'] = {
            onInit: field => {
                const data = [ ...field.props.options as any[] ];
                const isIssuerControl = field.form.get('isIssuer');
                field.props.options = isIssuerControl.valueChanges.pipe(
                    startWith(data.filter(x => x > 2)),
                    distinctUntilChanged(),
                    map(value => {
                        const options = data.filter(x => {
                            return value ? x.value > 3 : x.value < 3; // Τύπος 1.Πωλήσεις, 4.Αγορές, 8.Δαπάνες
                        });
                        field.formControl.setValue(value ? 4 : 1);
                        return options;
                    }),
                );
            }
        };

        infoInputs.properties['listButton'].props['click'] = () => {
            this.table?.loadDataSource();
        };

        this.infoInputs = infoInputs;
    }
}
