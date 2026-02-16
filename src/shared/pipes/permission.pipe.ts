import { Pipe, PipeTransform } from '@angular/core';
import { AuthService } from "@jwtNg";

@Pipe({
    name: 'permission',
    pure: false
})
export class PermissionPipe implements PipeTransform {

    constructor(private authService: AuthService) { }

    transform(requiredRoles: string[]): any {

        return this.authService.isAuthUserInRoles(requiredRoles);;
    }
}
