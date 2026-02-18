import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { BookmarkListComponent } from './components/bookmark-list';
import { BookmarkEditComponent } from './components/bookmark-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: BookmarkListComponent },
    { path: ':id', component: BookmarkEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class BookmarkRoutingModule { }
