import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LandingComponent } from './landing/landing.component';
import { ShellLayoutComponent } from './layout/shell-layout.component';
//import { LoginComponent } from './login/login.component';
//import { WelcomeComponent } from './welcome/welcome.component';

const routes: Routes = [
    {
        path: '',
        component: ShellLayoutComponent,
        children: [
            {
                path: '',
                children: [
                    { path: '', component: LandingComponent },
                    { path: 'welcome', component: LandingComponent },
                    //{ path: 'login', component: LoginComponent }
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
export class ShellRoutingModule { }
