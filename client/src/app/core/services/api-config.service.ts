import { LOCATION_INITIALIZED } from "@angular/common";
import { HttpClient } from "@angular/common/http";
import { Inject, Injectable, Injector } from "@angular/core";
import { lastValueFrom } from 'rxjs';
import { RoleName } from "@jwtNg";

@Injectable({
    providedIn: 'root'
})
export class ApiConfigService {

    private config: IApiConfig | null = null;

    constructor(@Inject('BASE_URL') private baseUrl: string, private injector: Injector) { }

    loadApiConfig(): Promise<any> {
        const httpClient = this.injector.get<HttpClient>(HttpClient);
        const url = `${this.baseUrl}api/account/getAppInfo`;

        const locationInitialized = this.injector.get(LOCATION_INITIALIZED, Promise.resolve(null));

        return locationInitialized.then(() => {
            return lastValueFrom(httpClient.get<any>(url))
                .then(result => {
                    this.config = {
                        adminRoleName: RoleName.Administrators,
                        localeId: result.language.languageCulture,
                        menus: result.menus,
                        trader: result.trader,
                        employee: result.employee
                    };
                })
                .catch(err => {
                    console.error(`Failed to load CustomerInfo. Make sure ${url} is accessible.`);
                    return Promise.reject(err);
                });
        });
    }

    get configuration(): IApiConfig {
        if (!this.config) {
            throw new Error("Attempted to access configuration property before configuration data was loaded.");
        }
        return this.config;
    }
}

export interface TraderInfo {
    id: number;
    fullName: string;
    bookId: number;
}

export interface EmployeeInfo {
    id: number;
    fullName: string;
    email: string;
}

export interface IApiConfig {
    adminRoleName: string;
    localeId: string;
    menus: string[];
    trader: TraderInfo;
    employee: EmployeeInfo;
}
