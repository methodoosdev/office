import { Injectable, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
// custom-preloading-strategy.ts
import { PreloadingStrategy, Route } from '@angular/router';
import { Observable, EMPTY } from 'rxjs';
import { AuthGuard } from '@jwtNg';

@Injectable({ providedIn: 'root' })
export class CustomPreloadingStrategy implements PreloadingStrategy {
    preload(route: Route, load: () => Observable<any>): Observable<any> {
        if (route.data && route.data['preload']) {
            return load();
        } else {
            return EMPTY;
        }
    }
}
const routes: Routes = [
    {
        path: 'office',
        loadChildren: () => import('./office/office.module').then(m => m.OfficeModule),
        canLoad: [AuthGuard]
    },
    { path: 'auth', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule), data: { preload: true } },
    { path: 'docs', loadChildren: () => import('./docs/docs.module').then(m => m.DocsModule) },
    { path: '', loadChildren: () => import('./shell/shell.module').then(m => m.ShellModule), data: { preload: true } },
    { path: '', redirectTo: '/welcome', pathMatch: 'full' },
    { path: '**', redirectTo: '/welcome', pathMatch: 'full' }
];

@NgModule({
    imports: [
        RouterModule.forRoot(
            routes,
            {
                preloadingStrategy: CustomPreloadingStrategy,
                enableTracing: false, // <-- debugging purposes only
                scrollPositionRestoration: 'top'
            }
        )
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
