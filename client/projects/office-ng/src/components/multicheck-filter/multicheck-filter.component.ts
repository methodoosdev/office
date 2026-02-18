import { Component, Input, Output, EventEmitter, AfterViewInit, NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { LabelModule } from "@progress/kendo-angular-label";
import { distinct, filterBy, FilterDescriptor } from "@progress/kendo-data-query";
import { FilterService, GridModule } from "@progress/kendo-angular-grid";

/**
 * NOTE: Interface declaration here is for demo compilation purposes only!
 * In the usual case include it as an import from the data query package:
 *
 * import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
 */
interface CompositeFilterDescriptor {
    logic: "or" | "and";
    filters: Array<any>;
}

@Component({
    selector: "multicheck-filter",
    template: `
    <ul>
      <li *ngIf="showFilter">
        <input class="k-textbox k-input k-rounded-md" (input)="onInput($event)" />
      </li>
      <li
        #itemElement
        *ngFor="let item of currentData; let i = index"
        (click)="onSelectionChange(valueAccessor(item), itemElement)" [ngClass]="{ 'k-selected': isItemSelected(item) }" >
        <input type="checkbox" #notification kendoCheckBox [checked]="isItemSelected(item)" />
        <kendo-label class="k-checkbox-label" [for]="notification" [text]="textAccessor(item)"></kendo-label>
      </li>
    </ul>
  `,
    styles: [
        `
      ul {
        list-style-type: none;
        height: 200px;
        overflow-y: scroll;
        padding-left: 0;
        padding-right: 12px;
      }

      ul > li {
        padding: 8px 12px;
        border: 1px solid rgba(0, 0, 0, 0.08);
        border-bottom: none;
      }

      ul > li:last-of-type {
        border-bottom: 1px solid rgba(0, 0, 0, 0.08);
      }

      .k-checkbox-label {
        pointer-events: none;
      }
    `,
    ],
})
export class MultiCheckFilterComponent implements AfterViewInit {
    @Input() public isPrimitive: boolean;
    @Input() public currentFilter: CompositeFilterDescriptor;
    @Input() public data;
    @Input() public textField;
    @Input() public valueField;
    @Input() public filterService: FilterService;
    @Input() public field: string;
    @Output() public valueChange = new EventEmitter<number[]>();

    public currentData: unknown[];
    public showFilter = true;
    private value: unknown[] = [];

    public textAccessor = (dataItem: unknown): string =>
        this.isPrimitive ? dataItem : dataItem[this.textField];
    public valueAccessor = (dataItem: unknown): unknown =>
        this.isPrimitive ? dataItem : dataItem[this.valueField];

    public ngAfterViewInit(): void {
        this.currentData = this.data;
        this.value = this.currentFilter.filters.map(
            (f: FilterDescriptor) => f.value
        );

        this.showFilter =
            typeof this.textAccessor(this.currentData[0]) === "string";
    }

    public isItemSelected(item: unknown): boolean {
        return this.value.some((x) => x === this.valueAccessor(item));
    }

    public onSelectionChange(item: unknown, li: HTMLLIElement): void {
        if (this.value.some((x) => x === item)) {
            this.value = this.value.filter((x) => x !== item);
        } else {
            this.value.push(item);
        }

        this.filterService.filter({
            filters: this.value.map((value) => ({
                field: this.field,
                operator: "eq",
                value,
            })),
            logic: "or",
        });

        this.onFocus(li);
    }

    public onInput(e: Event): void {
        this.currentData = distinct(
            [
                ...this.currentData.filter((dataItem) =>
                    this.value.some((val) => val === this.valueAccessor(dataItem))
                ),
                ...filterBy(this.data, {
                    operator: "contains",
                    field: this.textField,
                    value: (e.target as HTMLInputElement).value,
                }),
            ],
            this.textField
        );
    }

    public onFocus(li: HTMLLIElement): void {
        const ul = li.parentNode as HTMLUListElement;
        const below =
            ul.scrollTop + ul.offsetHeight < li.offsetTop + li.offsetHeight;
        const above = li.offsetTop < ul.scrollTop;

        // Scroll to focused checkbox
        if (above) {
            ul.scrollTop = li.offsetTop;
        }

        if (below) {
            ul.scrollTop += li.offsetHeight;
        }
    }
}

@NgModule({
    imports: [CommonModule, GridModule, InputsModule, LabelModule],
    exports: [MultiCheckFilterComponent],
    declarations: [MultiCheckFilterComponent]
})
export class MultiCheckFilterModule { }
