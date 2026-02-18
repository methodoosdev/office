import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable, } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, lastValueFrom, Observable, throwError } from "rxjs";
import { catchError, finalize, map } from "rxjs/operators";
import { AuthTokenType } from "../api/auth-token-type";
import { AuthUser } from "../api/auth-user";
import { ApiConfig } from "../api/api-config";
import { Credentials } from "../api/credentials";

import { RefreshTokenService } from "./refresh-token.service";
import { TokenStoreService } from "./token-store.service";
import { RoleName } from "../api/role-names";

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    private authStatusSource = new BehaviorSubject<boolean>(false);
    authStatus$ = this.authStatusSource.asObservable();

    constructor(
        private http: HttpClient,
        private router: Router,
        @Inject('BASE_URL') private baseUrl: string,
        private tokenStoreService: TokenStoreService,
        private refreshTokenService: RefreshTokenService
    ) {
        this.updateStatusOnPageRefresh();
        this.refreshTokenService.scheduleRefreshToken(this.isAuthUserLoggedIn(), false);
    }

    login(credentials: Credentials): Observable<boolean> {
        const headers = new HttpHeaders({ "Content-Type": "application/json" });
        const url = `${this.baseUrl}api/${ApiConfig.LoginPath}`;

        return this.http
            .post(url, credentials, { headers: headers })
            .pipe(
                map((response: any) => {
                    this.tokenStoreService.setRememberMe(credentials.rememberMe);
                    if (!response) {
                        console.error("There is no `{'" + ApiConfig.AccessTokenObjectKey +
                            "':'...','" + ApiConfig.RefreshTokenObjectKey + "':'...value...'}` response after login.");
                        this.authStatusSource.next(false);
                        return false;
                    }
                    this.tokenStoreService.storeLoginSession(response);
                    console.log("Logged-in user info", this.getAuthUser());
                    this.refreshTokenService.scheduleRefreshToken(true, true);
                    this.authStatusSource.next(true);
                    return true;
                }),
                catchError((error: HttpErrorResponse) => throwError(() => error))
            );
    }

    getBearerAuthHeader(): HttpHeaders {
        return new HttpHeaders({
            "Content-Type": "application/json",
            "Authorization": `Bearer ${this.tokenStoreService.getRawAuthToken(AuthTokenType.AccessToken)}`
        });
    }

    logout(navigateToHome: boolean): void {
        const headers = new HttpHeaders({ "Content-Type": "application/json" });
        const refreshToken = encodeURIComponent(this.tokenStoreService.getRawAuthToken(AuthTokenType.RefreshToken));
        this.http
            .get(`${this.baseUrl}api/${ApiConfig.LogoutPath}?refreshToken=${refreshToken}`,
                { headers: headers })
            .pipe(
                map(response => response || {}),
                catchError((error: HttpErrorResponse) => throwError(() => error)),
                finalize(() => {
                    this.tokenStoreService.deleteAuthTokens();
                    this.refreshTokenService.unscheduleRefreshToken(true);
                    this.authStatusSource.next(false);
                    if (navigateToHome) {
                        this.router.navigate(["/"]);
                    }
                }))
            .subscribe(result => {
                console.log("logout", result);
            });
    }

    changeLanguage(localeId: string) {
        return lastValueFrom(this.http.get(`${this.baseUrl}api/${ApiConfig.SetLanguagePath}?localeId=${localeId}`))
    }

    clearCache() {
        return lastValueFrom(this.http.post(`${this.baseUrl}api/${ApiConfig.ClearCachePath}`, {}))
    }

    isAuthUserLoggedIn(): boolean {
        return this.tokenStoreService.hasStoredAccessAndRefreshTokens() &&
            !this.tokenStoreService.isAccessTokenTokenExpired();
    }

    getAuthUser(): AuthUser | null {
        if (!this.isAuthUserLoggedIn()) {
            return null;
        }

        const decodedToken = this.tokenStoreService.getDecodedAccessToken();
        const roles = this.tokenStoreService.getDecodedTokenRoles();
        return Object.freeze({
            userId: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
            userName: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
            email: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
            nickName: decodedToken["NickName"],
            systemName: decodedToken["SystemName"],
            serialNumber: decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/serialnumber"],
            roles: roles
        });
    }

    isAuthUserInRoles(requiredRoles: string[]): boolean {
        const user = this.getAuthUser();
        if (!user || !user.roles) {
            return false;
        }

        //if (user.roles.indexOf(RoleName.Administrators.toLowerCase()) >= 0) {
        //    return true; // The `Admin` role has full access to every pages.
        //}

        return requiredRoles.some(requiredRole => {
            if (user.roles) {
                return user.roles.indexOf(requiredRole.toLowerCase()) >= 0;
            } else {
                return false;
            }
        });
    }

    isAuthUserInRole(requiredRole: string): boolean {
        return this.isAuthUserInRoles([requiredRole]);
    }

    private updateStatusOnPageRefresh(): void {
        this.authStatusSource.next(this.isAuthUserLoggedIn());
    }
}
