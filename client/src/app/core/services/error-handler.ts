import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, Injector, NgZone } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { TranslationService } from './translation.service';

@Injectable()
export class ErrorHandler {
    constructor(private zone: NgZone, private injector: Injector) { }

    // Handles error and returns a rejected promise.
    error(e: string | any): void {
        this.handle(e);
    }

    private get toastrService(): ToastrService {
        return this.injector.get<ToastrService>(ToastrService);
    }

    private get translationService(): TranslationService {
        return this.injector.get<TranslationService>(TranslationService);
    }

    private handle(error: string | any) {
        // Ignore error if it was already handled once.
        if (error.errorWasHandled) { return; }

        let message: string;

        if (typeof error === 'string') {
            message = error;
        } else {
            error['errorWasHandled'] = true;
            message = this.getErrorMessage(error);
        }

        // Mark error as handled
        console.error(message);

        this.zone.runOutsideAngular(() => {
            const err = this.translationService.translate('error.error');
            this.toastrService.error(message || err);
        });
    }

    getErrorMessage(err: HttpErrorResponse) {
        let errorMessage: string;

        if (err instanceof Error) {
            errorMessage = err.message;
        } else {
            switch (err.status) {
                case 401:
                case 403:
                    errorMessage = this.translationService.translate('error.accessDenied');
                    break;
                case 500:
                    errorMessage = this.translationService.translate('error.internalServerError');
                    break;
                default:
                    const prefix = this.translationService.translate('error.error');
                    if (typeof err.error === 'string')
                        errorMessage = `${prefix} ${err.status}: ${err.error}`;
                    else
                        errorMessage = `${prefix} ${err.status}: ${err.error?.message || err.message}`;
                    break;
            }
        }
        return errorMessage;
    }
}
