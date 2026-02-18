import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MedicalExamComponent } from './components/medical-exam';

const routes: Routes = [
    { path: '', component: MedicalExamComponent }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class MedicalExamRoutingModule { }
