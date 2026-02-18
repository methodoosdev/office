import { FormlyFieldConfig } from '@ngx-formly/core';

export function getFieldProperties(fields: FormlyFieldConfig[]): { [key: string]: FormlyFieldConfig } {
    const fieldProperties: { [key: string]: FormlyFieldConfig } = {};
    const setFieldProperties = (fields: FormlyFieldConfig[]) => {
        fields.forEach((item) => {
            const group = item.fieldGroup;
            if (Array.isArray(group)) {
                setFieldProperties(group);
            } else {
                const key = item.key as string;
                fieldProperties[key] = item;
            }
        });
    };

    setFieldProperties(fields);

    return fieldProperties;
}

export function deepCopy(obj) {
    if (obj === null || typeof obj !== 'object') {
        // Return the value if obj is not an object or array
        return obj;
    }

    // Create an array or object to hold the values
    let copy = Array.isArray(obj) ? [] : {};

    // Recursively copy each item in the object or array
    for (let key in obj) {
        if (obj.hasOwnProperty(key)) {
            copy[key] = deepCopy(obj[key]);
        }
    }

    return copy;
}

