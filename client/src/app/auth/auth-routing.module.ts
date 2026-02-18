import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AccessDeniedComponent } from "./access-denied/access-denied.component";
import { ChangePasswordComponent } from "./change-password/change-password.component";
import { LoginComponent } from './login/login.component';
import { AuthLayoutComponent } from './layout/auth-layout.component';
import { AuthGuard, AuthGuardPermission, RoleName } from '@jwtNg';

const routes: Routes = [
    {
        path: '',
        component: AuthLayoutComponent,
        children: [
            {
                path: '',
                children: [
                    { path: 'login', component: LoginComponent },
                    { path: "accessDenied", component: AccessDeniedComponent },
                    {
                        path: "changePassword", component: ChangePasswordComponent,
                        data: {
                            permission: {
                                permittedRoles: [RoleName.Administrators]
                            } as AuthGuardPermission
                        },
                        canActivate: [AuthGuard]
                    }
                ]
            }
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class AuthRoutingModule { }
