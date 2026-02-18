import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";

import { ChangePassword } from "./../models/change-password";

@Injectable()
export class ChangePasswordService {

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

    changePassword(model: ChangePassword): Observable<any> {
        const headers = new HttpHeaders({ "Content-Type": "application/json" });
        const url = `${this.baseUrl}api/changePassword`;
        return this.http
            .post(url, model, { headers: headers })
            .pipe(
                map(response => response || {}),
                catchError((error: HttpErrorResponse) => throwError(error))
            );
    }
}
