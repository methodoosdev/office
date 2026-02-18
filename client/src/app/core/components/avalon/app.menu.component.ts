import { Component } from '@angular/core';
import { Observable, Subscription, of } from 'rxjs';
import { LayoutService } from './service/app.layout.service';

@Component({
    selector: 'app-menu',
    templateUrl: './app.menu.component.html'
})
export class AppMenuComponent {

    constructor(public layoutService: LayoutService) {
    }
}
