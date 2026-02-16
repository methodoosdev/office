import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, NgModule, Output, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'more-less',
    templateUrl: './more-less.html',
    styleUrls: ['./more-less.scss'],
    encapsulation: ViewEncapsulation.None,
    host: {
        class: 'p-more-less'
    }
})
export class MoreLessComponent {
    @Input() text: string;
    @Input() wordLimit: number = 60;
    @Input() showMore: boolean = false;

    toggle() {
        this.showMore = !this.showMore;
    }

}

@NgModule({
    imports: [CommonModule],
    exports: [MoreLessComponent],
    declarations: [MoreLessComponent]
})
export class MoreLessModule {}
