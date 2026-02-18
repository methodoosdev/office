import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderRatingCategoryListComponent } from './components/category-list';
import { TraderRatingCategoryEditComponent } from './components/category-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: TraderRatingCategoryListComponent },
    { path: ':id', component: TraderRatingCategoryEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderRatingCategoryRoutingModule { }
