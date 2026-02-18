import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderRatingListComponent } from './components/rating-list';
import { TraderRatingEditComponent } from './components/rating-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: TraderRatingListComponent },
    { path: ':id', component: TraderRatingEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderRatingRoutingModule { }
