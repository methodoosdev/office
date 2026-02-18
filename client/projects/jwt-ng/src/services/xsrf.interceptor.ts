import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpXsrfTokenExtractor } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoadingBarService } from "@ngx-loading-bar/core";
import { Observable } from "rxjs";

import { finalize, tap } from 'rxjs/operators';

@Injectable()
export class XsrfInterceptor implements HttpInterceptor { // Handles absolute URLs

    constructor(private tokenExtractor: HttpXsrfTokenExtractor, private loader: LoadingBarService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        /*
            const lcUrl = request.url.toLowerCase();
            if (request.method === "GET" || request.method === "HEAD" ||
              lcUrl.startsWith("http://") || lcUrl.startsWith("https://")) {
              console.log("Original HttpXsrfInterceptor skips both non-mutating requests and absolute URLs.");
              console.log("Skipped request", { lcUrl: lcUrl, method: request.method });
            }
        */
        if (request.method === "POST") {
            const headerName = "X-XSRF-TOKEN";
            const token = this.tokenExtractor.getToken();
            if (token && !request.headers.has(headerName)) {
                request = request.clone({
                    headers: request.headers.set(headerName, token)
                });
            }
        }

        let started = false;
        const ref = this.loader.useRef('http');

        return next.handle(request).pipe(
            tap(() => {
                if (!started) {
                    ref.start();
                    started = true;
                }
            }),
            finalize(() => started && ref.complete()),
        );
    }
}
