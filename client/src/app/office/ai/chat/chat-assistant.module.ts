import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { FormlyEditModule, FormListModule, OfficeSharedModule } from '@officeNg';
import { ChatAssistantComponent } from './components/chat-assistant';
import { ChatAssistantRoutingModule } from './chat-assistant-routing.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        ChatAssistantRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        ChatAssistantComponent
    ],
    providers: [
    ]
})
export class ChatAssistantModule { }
