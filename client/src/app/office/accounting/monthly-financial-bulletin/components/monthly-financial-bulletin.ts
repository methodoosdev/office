import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { DomSanitizer, SafeStyle } from "@angular/platform-browser";
import { FormGroup } from "@angular/forms";
import { saveAs } from '@progress/kendo-file-saver';
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { TranslationService } from "@core";
import { MonthlyFinancialBulletinUnitOfWork } from "@officeNg";
import { getFieldProperties } from "@formlyNg";

@Component({
    selector: "monthly-financial-bulletin",
    templateUrl: "./monthly-financial-bulletin.html",
    styleUrls: ["./monthly-financial-bulletin.scss"],
    encapsulation: ViewEncapsulation.None,
    animations: [stateAnimation]
})
export class MonthlyFinancialBulletinComponent implements OnInit {
    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    animate: boolean = true;
    allIds: any[];
    expandedIds: any[] = [];
    treeData: any[];
    treeDataOrigin: any[];
    treeColumns: any[];
    treeTableTitle: string;

    title: string;
    resultFormTitle: string;
    calcLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    resultForm = new FormGroup({});
    resultOptions: FormlyFormOptions = {};
    resultFields: FormlyFieldConfig[];
    resultModel: any;

    remodelingCostsData: any[];
    remodelingCostsColumns: any[];
    remodelingCostsTableTitle: string;

    collapseLabel: string;
    expandLabel: string;

    level: number;
    levelData: any[] = [];

    constructor(
        private uow: MonthlyFinancialBulletinUnitOfWork,
        private sanitizer: DomSanitizer,
        private toastrService: ToastrService,
        private translationService: TranslationService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.title = this.translationService.translate('menu.monthlyFinancialBulletin');
        this.calcLabel = this.translationService.translate('common.calc');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
        this.collapseLabel = this.translationService.translate('common.collapse');
        this.expandLabel = this.translationService.translate('common.expand');
    }

    resetForm() {
        this.level = this.levelData.length;
        this.treeData = null;
        this.expandedIds = [];
    }

    modelChangeEvent(value: any) {
        this.level = null;
        this.levelData = [];
        this.treeData = null;
        this.expandedIds = [];
    }

    get canExport() {
        return this.treeData && this.treeData.length > 0;
    }

    get canCalc() {
        return this.searchModel && this.searchModel.traderId > 0;
    }

    ngOnInit(): void {
        this.uow.loadProperties()
            .then((result: any) => {
                result.searchModel['periodos'] = new Date(result.searchModel['periodos']);

                this.searchFields = result.searchModel.customProperties.fields;
                this.searchModel = result.searchModel;

                this.treeColumns = result.tableModel.customProperties.columns;
                this.treeTableTitle = result.tableModel.customProperties.title;

                this.resultFields = [
                    {
                        fieldGroupClassName: 'grid',
                        fieldGroup: [
                            {
                                fieldGroupClassName: 'grid',
                                className: 'col-12 md:col-6',
                                fieldGroup: result.resultForm.customProperties.fields
                            }
                        ],
                    }
                ];
                this.resultFormTitle = result.resultForm.customProperties.title;
                this.remodelingCostsColumns = result.remodelingCostsTable.customProperties.columns;
                this.remodelingCostsTableTitle = result.remodelingCostsTable.customProperties.title;

                const fieldProperties = getFieldProperties(this.searchFields);

                if (fieldProperties['traderId']) {
                    fieldProperties['traderId'].props['selectionChange'] = (field: FormlyFieldConfig, trader: any) => {

                        if (trader) {
                            this.uow.traderChanged(trader.id, this.searchModel.periodos)
                                .then((result: any) => {
                                    field.form.get('expirationInventory').setValue(result.expirationInventory);
                                    field.form.get('expirationDepreciate').setValue(result.expirationDepreciate);
                                    fieldProperties['branch'].props['options'] = result.branches;

                                }).catch((err) => {
                                    this.resetForm();
                                    throw err;
                                });
                        } else {
                            field.form.get('expirationInventory').setValue(0);
                            field.form.get('expirationDepreciate').setValue(0);
                            fieldProperties['branch'].props['options'] = [];
                        }
                    };
                }

                fieldProperties['periodos'].props['valueChange'] = (field: FormlyFieldConfig, date: Date) => {
                    const traderId: number = field.model.traderId;

                    if (traderId > 0) {
                        this.uow.traderChanged(traderId, this.searchModel.periodos)
                            .then((result: any) => {
                                field.form.get('expirationInventory').setValue(result.expirationInventory);
                                field.form.get('expirationDepreciate').setValue(result.expirationDepreciate);
                                fieldProperties['branch'].props['options'] = result.branches;

                            }).catch((err) => {
                                this.resetForm();
                                throw err;
                            });
                    }
                };

            }).catch((err) => {
                this.resetForm();
                throw err;
            });
    }

    levelValueChange(level: number) {

        if (level == this.levelData.length) {
            this.treeData = [...this.treeDataOrigin];
        } else {
            const temp = this.treeDataOrigin.filter(x => x.level <= level);
            this.treeData = [...temp];
        }
    }

    expandCollapse() {
        new Promise((resolve, reject) => {
            setTimeout(() => {
                this.expandedIds = this.expandedIds.length ? [] : this.allIds;
                resolve({});
            }, 0);
        });
    }

    private getLevelData(data: any[], label: string): any[] {
        const list = data.map(x => x.level);
        const unique = list.filter((x, i, a) => {
            return a.indexOf(x) === i;
        });
        const sort = unique.sort((a, b) => b - a);

        return sort.map((x) => {
            return { value: x, label: `${x}-${label}` };
        });
    }

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((result: any) => {
                this.treeData = result.model.treeList as any[];
                this.treeDataOrigin = [...this.treeData]; // clone array
                this.allIds = this.treeDataOrigin.map(x => x.id);

                this.resultModel = result.model.resultModel;
                this.remodelingCostsData = result.model.remodelingCostsList;

                this.levelData = this.getLevelData(this.treeData, this.translationService.translate('common.degree'));
                this.level = this.levelData.length;

            }).catch((err) => {
                this.resetForm();
                throw err;
            });
    }

    exportToPdf() {
        this.uow.monthlyFinancialBulletinToPDF(this.searchModel, this.level)
            .then((result) => {
                if (result)
                    saveAs(result, "financialBulletin.pdf");
                else
                    this.toastrService.error('Pdf file cannot be created.');
            }).catch((err: Error) => {
                throw err;
            });
    }

    resultModelExportToPdf() {
        this.uow.resultModelExportToPdf(this.resultModel, this.searchModel.traderId, this.searchModel.periodos)
            .then((result) => {
                if (result)
                    saveAs(result, "financialBulletinForm.pdf");
                else
                    this.toastrService.error('Pdf file cannot be created.');
            }).catch((err: Error) => {
                throw err;
            });
    }

    public colorCode(value: number): SafeStyle {
        const result = value >= 0 ? '#424242' : '#ff0000';
        return this.sanitizer.bypassSecurityTrustStyle(result);
    }
}
