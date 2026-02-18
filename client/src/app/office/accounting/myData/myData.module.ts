import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { FormlyEditModule, OfficeSharedModule } from '@officeNg';
import { MyDataUnitOfWork } from '@officeNg';

import { MyDataRoutingModule } from './myData-routing.module';
import { MyDataComponent } from './components/myData';
import { MyDataDialogComponent } from './components/myData-dialog';
import { MyDataDetailDialogComponent } from './components/myData-detail-dialog';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormlyEditModule,
        MyDataRoutingModule
    ],
    declarations: [
        MyDataComponent,
        MyDataDialogComponent,
        MyDataDetailDialogComponent
    ],
    providers: [
        MyDataUnitOfWork
    ]
})
export class MyDataModule { }
