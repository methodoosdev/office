import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface CanComponentDeactivate {
    canDeactivate: () => Observable<boolean> | Promise<boolean> | boolean;
}

@Injectable({
    providedIn: 'root',
})
export class CanDeactivateGuard implements CanDeactivate<CanComponentDeactivate> {

    constructor(private authService: AuthService) { }

    canDeactivate(component: CanComponentDeactivate) {
        if (!this.authService.isAuthUserLoggedIn())
            return true;
        return component.canDeactivate ? component.canDeactivate() : true;
    }
}
