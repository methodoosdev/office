import { Injectable } from "@angular/core";

import { BrowserStorageService } from "@jwtNg";

@Injectable({
    providedIn: 'root'
})
export class UtilsService {

    constructor(private browserStorageService: BrowserStorageService) { }

    isEmptyString(value: string): boolean {
        return !value || 0 === value.length;
    }

    vatValidation(afm: string): boolean {
        if (!afm) {
            // value is null
            return false;
        }

        if (afm.length != 9) {
            // Length of value must be equal to nine
            return false;
        }

        if (!/^\d+$/.test(afm)) {
            // Inserted value not is a number
            return false;
        }

        if (afm === '0'.repeat(9)) {
            // Inserted value not is zero number (000000000)
            return false;
        }

        const sum = afm
            .substring(0, 8)
            .split('')
            .reduce((s, v, i) => s + (parseInt(v) << (8 - i)), 0);

        const calc = sum % 11;
        const d9 = parseInt(afm[8]);
        const valid = calc % 10 === d9;

        return valid;
    }

    getCurrentTabId(): number {
        const tabIdToken = "currentTabId";
        let tabId = this.browserStorageService.getSession(tabIdToken);
        if (tabId) {
            return tabId;
        }
        tabId = Math.random();
        this.browserStorageService.setSession(tabIdToken, tabId);
        return tabId;
    }

    /*
     var flatData = [ { "id": 1, "text": "A", "parentId": 0, "hasItems": "true" },
                      { "id": 2, "text": "B", "parentId": 1, "hasItems": "false" },
                      { "id": 3, "text": "C", "parentId": 1, "hasItems": "false" },
                      { "id": 4, "text": "D", "parentId": 0, "hasItems": "false" }];
     */
    processTable(data: any[], rootLevel = 0, idField = 'id', foreignKey = 'parentId') {
        const hash: any = {};

        for (let i = 0; i < data.length; i++) {
            const item = data[i];
            const id = item[idField];
            const parentId = item[foreignKey];

            hash[id] = hash[id] || [];
            hash[parentId] = hash[parentId] || [];

            item.items = hash[id];
            hash[parentId].push(item);
        }

        return hash[rootLevel];
    }

    //res = stringFormat("Hello {0} {1}", "beautiful", "World!")
    //console.log(res) // Hello beautiful World!
    stringFormat(str: string, ...args: string[]) {
        return str.replace(/{(\d+)}/g, (match, index) => args[index] || '');
    }

    camelizeFirstLetter(value: string) {
        const val = value ?? '';
        return val.length > 0 ? val.charAt(0).toLowerCase() + val.slice(1) : '';
    }

    capitalizeFirstLetter(value: string) {
        const val = value ?? '';
        return val.length > 0 ? val.charAt(0).toUpperCase() + val.slice(1) : '';
    }
}
