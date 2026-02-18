import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SimpleTaskCategoryListComponent } from './components/simple-task-category-list';
import { SimpleTaskCategoryEditComponent } from './components/simple-task-category-edit';
import { CanDeactivateGuard } from '@jwtNg';

const routes: Routes = [
    { path: '', component: SimpleTaskCategoryListComponent },
    { path: ':id', component: SimpleTaskCategoryEditComponent, canDeactivate: [CanDeactivateGuard] }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SimpleTaskCategoryRoutingModule { }
