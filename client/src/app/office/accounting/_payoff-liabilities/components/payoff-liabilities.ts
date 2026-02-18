import { Component, OnInit, ViewChild } from "@angular/core";

import { GridComponent } from "@progress/kendo-angular-grid";
import { stateAnimation } from "@primeNg";
import { TranslationService } from "@core";

@Component({
    selector: "payoff-liabilities",
    templateUrl: "./payoff-liabilities.html",
    animations: [stateAnimation]
})
export class PayoffLiabilitiesComponent implements OnInit {
    @ViewChild(GridComponent, { static: false }) table: GridComponent;
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    exportToExcelButtonVisible: boolean = true;
    exportToPdfButtonVisible: boolean = true;

    title: string;
    calcLabel: string;
    traderLabel: string;
    periodLabel: string;
    autofitColumnsLabel: string;

    currentTrader: any;
    year = new Date();

    constructor(
        private translationService: TranslationService) {

        this.calcLabel = this.translationService.translate('common.calc');
        this.traderLabel = this.translationService.translate('common.trader');
        this.periodLabel = this.translationService.translate('common.period');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    ngOnInit(): void {

    }

    calc() {
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canCalc() {
        return this.currentTrader && this.currentTrader.id > 0;
    }

    get isTrader() {
        return true;
    }

    onSelectionChange(trader: any) {
        this.currentTrader = trader;
        this.gridData = null;
    }

    exportToExcel() {
    }

    exportToPdf() {
    }
}
