import { HttpClient, HttpHeaders, HttpParams, HttpEvent, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ClientDataService {
    private httpClient = inject(HttpClient);

    private buildHttpParams(urlQuery: string, queryParams?: { [key: string]: any }): HttpParams {
        let params = new HttpParams();

        // Convert URL query string into an object if it exists
        if (urlQuery) {
            const searchParams = new URLSearchParams(urlQuery);
            searchParams.forEach((value, key) => {
                params = params.set(key, value);
            });
        }

        if (queryParams && Object.keys(queryParams).length > 0) {
            Object.keys(queryParams).forEach((key) => {
                if (queryParams[key] !== undefined) {
                    params = params.set(key, queryParams[key]);
                }
            });
        }
        return params;
    }

    private buildHttpHeaders(headers?: { [key: string]: string }): HttpHeaders {
        let httpHeaders = new HttpHeaders();
        if (headers) {
            Object.keys(headers).forEach((key) => {
                if (headers[key] !== undefined) {
                    httpHeaders = httpHeaders.set(key, headers[key]);
                }
            });
        }
        return httpHeaders;
    }

    fetchJsonBodyGet<T>(url: string, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<T> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.get<T>(baseUrl, {
            params: this.buildHttpParams(urlQuery, queryParams),
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchJsonBodyPost<T>(url: string, body: any, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<T> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post<T>(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchJsonEventsGet<T>(url: string, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpEvent<T>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.get<T>(baseUrl, {
            params: this.buildHttpParams(urlQuery, queryParams),
            observe: 'events',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchJsonEventsPost<T>(url: string, body: any, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpEvent<T>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post<T>(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            observe: 'events',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchBlobBodyGet(url: string, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<Blob> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.get(baseUrl, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'blob',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchBlobBodyPost(url: string, body: any, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<Blob> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'blob',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchBlobResponseBodyGet(url: string, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpResponse<Blob>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.get(baseUrl, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'blob',
            observe: 'response',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchBlobResponseBodyPost(url: string, body: any, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpResponse<Blob>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'blob',
            observe: 'response',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchBlobEventsGet(url: string, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpEvent<Blob>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.get(baseUrl, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'blob',
            observe: 'events',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchBlobEventsPost(url: string, body: any, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpEvent<Blob>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'blob',
            observe: 'events',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchTextBodyGet(url: string, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<string> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.get(baseUrl, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'text',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchTextBodyPost(url: string, body: any, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<string> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'text',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchTextEventsGet(url: string, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpEvent<string>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.get(baseUrl, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'text',
            observe: 'events',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchTextEventsPost(url: string, body: any, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpEvent<string>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'text',
            observe: 'events',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchArrayBufferBodyGet(url: string, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<ArrayBuffer> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.get(baseUrl, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'arraybuffer',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchArrayBufferBodyPost(url: string, body: any, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<ArrayBuffer> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'arraybuffer',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchArrayBufferEventsGet(url: string, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpEvent<ArrayBuffer>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.get(baseUrl, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'arraybuffer',
            observe: 'events',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    fetchArrayBufferEventsPost(url: string, body: any, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpEvent<ArrayBuffer>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            responseType: 'arraybuffer',
            observe: 'events',
            headers: this.buildHttpHeaders(headers),
        }));
    }

    uploadFileBodyPost(url: string, body: FormData, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<any> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post<any>(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            headers: this.buildHttpHeaders(headers),
        }));
    }

    uploadFileEventsPost(url: string, body: FormData, queryParams?: { [key: string]: any }, headers?: { [key: string]: string }): Promise<HttpEvent<any>> {
        const [baseUrl, urlQuery] = url.split('?');

        return firstValueFrom(this.httpClient.post<HttpEvent<any>>(baseUrl, body, {
            params: this.buildHttpParams(urlQuery, queryParams),
            observe: 'events',
            headers: this.buildHttpHeaders(headers),
        }));
    }
}
