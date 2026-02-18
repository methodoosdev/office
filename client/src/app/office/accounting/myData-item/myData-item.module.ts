import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { FormListModule, FormlyEditModule, OfficeSharedModule } from '@officeNg';
import { MyDataItemUnitOfWork } from '@officeNg';
import { PrimeSharedModule } from '@primeNg';

import { MyDataItemRoutingModule } from './myData-item-routing.module';
import { MyDataItemListComponent } from './components/myData-item-list';
import { MyDataItemEditComponent } from './components/myData-item-edit';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        PrimeSharedModule,
        MyDataItemRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        MyDataItemListComponent,
        MyDataItemEditComponent
    ],
    providers: [
        MyDataItemUnitOfWork
    ]
})
export class MyDataItemModule { }
