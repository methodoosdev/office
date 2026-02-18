import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ArticlesCheckListComponent } from './components/articles-check-list';
import { ArticlesCheckViewComponent } from './components/articles-check-view';

const routes: Routes = [
    { path: '', component: ArticlesCheckListComponent },
    { path: ':companyId/:nglId/:year/:period', component: ArticlesCheckViewComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ArticlesCheckRoutingModule { }
