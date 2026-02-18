import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatAssistantComponent } from './components/chat-assistant';

const routes: Routes = [
    { path: '', component: ChatAssistantComponent },
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class ChatAssistantRoutingModule { }
