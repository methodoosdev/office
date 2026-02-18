import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { QueuedEmailRoutingModule } from './queued-email-routing.module';
import { QueuedEmailListComponent } from './components/queued-email-list';
import { QueuedEmailEditComponent } from './components/queued-email-edit';
import { FormlyEditNewModule, FormListModule, FormListDialogModule, QueuedEmailUnitOfWork, OfficeSharedModule, TraderLookupUnitOfWork } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        QueuedEmailRoutingModule,
        FormListModule,
        FormlyEditNewModule,
        FormListDialogModule
    ],
    declarations: [
        QueuedEmailListComponent,
        QueuedEmailEditComponent
    ],
    providers: [
        QueuedEmailUnitOfWork,
        TraderLookupUnitOfWork
    ]
})
export class QueuedEmailModule { }
