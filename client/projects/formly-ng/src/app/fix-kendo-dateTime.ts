import { FormlyFieldConfig, FormlyFieldProps } from '@ngx-formly/core';
import { IntlService } from '@progress/kendo-angular-intl';

export function fixKendoDateTime(model: any, properties: { [key: string]: FormlyFieldConfig<FormlyFieldProps & { [additionalProperties: string]: any; }>; }) {
    const clone = { ...model };

    Object.entries(properties).forEach(([key, item]) => {
        if (item.type === 'date' || item.type === 'dateTime' || item.type === 'monthDate' || item.type === 'yearDate') {
            const date = new Date(clone[key]);

            if (!isNaN(date.getTime())) {
                clone[key] = date;
            }
        }
    });

    return clone;
}

export function parseDate(json) {
    Object.keys(json).map((key) => {
        const date = new Date(json[key]);
        if (!isNaN(date.getTime())) {
            json[key] = date;
        }
    });

    return json;
}

export function parseDateExact(intl: IntlService, json) {
    Object.keys(json).map(key => {
        const date = intl.parseDate(json[key], 'yyyy-MM-ddTHH:mm:ss');
        if (date) { json[key] = date; }
    });

    return json;
}

//fixKendoDateTime(model: any, fieldProperties: { [key: string]: FormlyFieldConfig<FormlyFieldProps & { [additionalProperties: string]: any; } >; }) {
//    const clone = Object.assign({}, model);

//    Object.entries(fieldProperties).forEach(([key, item]) => {
//        if (item.type === 'date') {
//            const date = Date.parse(clone[key]);

//            if (!isNaN(date)) {
//                const value = new Date(date).toUTCString();
//                clone[key] = new Date(value);
//            }
//        }
//    });

//    return clone;
//}
