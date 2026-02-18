import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { FilterMatchMode } from './filtermatchmode';
import { OverlayOptions } from './overlayoptions';

@Injectable({ providedIn: 'root' })
export class PrimeNGConfig {
    ripple: boolean = false;

    overlayOptions: OverlayOptions = {};

    zIndex: any = {
        modal: 1100,
        overlay: 1000,
        menu: 1000,
        tooltip: 1100
    };

    filterMatchModeOptions = {
        text: [FilterMatchMode.STARTS_WITH, FilterMatchMode.CONTAINS, FilterMatchMode.NOT_CONTAINS, FilterMatchMode.ENDS_WITH, FilterMatchMode.EQUALS, FilterMatchMode.NOT_EQUALS],
        numeric: [FilterMatchMode.EQUALS, FilterMatchMode.NOT_EQUALS, FilterMatchMode.LESS_THAN, FilterMatchMode.LESS_THAN_OR_EQUAL_TO, FilterMatchMode.GREATER_THAN, FilterMatchMode.GREATER_THAN_OR_EQUAL_TO],
        date: [FilterMatchMode.DATE_IS, FilterMatchMode.DATE_IS_NOT, FilterMatchMode.DATE_BEFORE, FilterMatchMode.DATE_AFTER]
    };

    private translation: { [key: string]: any };

    private translationSource = new Subject<any>();

    translationObserver = this.translationSource.asObservable();

    getTranslation(key: string): any {
        return this.translation[key as keyof typeof this.translation];
    }

    setTranslation(value: { [key: string]: any }) {
        this.translation = { ...this.translation, ...value };
        this.translationSource.next(this.translation);
    }

    private isDefined(value: any): boolean {
        return typeof value !== 'undefined' && value !== null;
    }

    getTranslationWithParams(key: string, ...args: any[]): any {
        if (!key || !key.length) {
            return key;
        }

        const value = this.getTranslation(key);

        if (this.isDefined(args[0]) && args.length) {
            return value.replace(/{(\d+)}/g, (_, index) => args[index] || '')
        }

        return value;
    }
}
