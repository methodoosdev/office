import { Component, EventEmitter, Input, Output, forwardRef } from "@angular/core";
import { ToolBarToolComponent } from "@progress/kendo-angular-toolbar";

@Component({
    selector: "landscape-tool",
    template: `
    <ng-template #toolbarTemplate>
      <span>
          <kendo-dropdownlist
            #dropdownlist
            [style.marginLeft.px]="5"
            [style.width.px]="150"
            [data]="options.data"
            [value]="options.value"
            (selectionChange)="onItemClick($event)"
          >
          </kendo-dropdownlist>
      </span>
    </ng-template>
  `,
    providers: [
        {
            provide: ToolBarToolComponent,
            useExisting: forwardRef(() => LandscapeToolComponent),
        },
    ],
})
export class LandscapeToolComponent extends ToolBarToolComponent {

    @Input() options: { value: any; data: string[]; };
    @Output() optionChange: EventEmitter<any> = new EventEmitter();

    public onItemClick(value: any): void {
        this.optionChange.emit(value);
    }
    constructor() {
        super();
    }

}

@Component({
    selector: "fontSize-tool",
    template: `
    <ng-template #toolbarTemplate>
      <span>
        <kendo-label [text]="text">
          <kendo-dropdownlist
            #dropdownlist
            [style.marginLeft.px]="5"
            [style.width.px]="100"
            [data]="options.data"
            [value]="options.value"
            (selectionChange)="onItemClick($event)"
          >
          </kendo-dropdownlist>
        </kendo-label>
      </span>
    </ng-template>
  `,
    providers: [
        {
            provide: ToolBarToolComponent,
            useExisting: forwardRef(() => FontSizeToolComponent),
        },
    ],
})
export class FontSizeToolComponent extends ToolBarToolComponent {

    @Input() text: string;
    @Input() options: { value: any; data: string[]; };
    @Output() optionChange: EventEmitter<any> = new EventEmitter();

    public onItemClick(value: any): void {
        this.optionChange.emit(value);
    }
    constructor() {
        super();
    }

}
