import { Component, OnDestroy, OnInit } from "@angular/core";

import { AuthService, RoleName } from "@jwtNg";
import { ApiConfigService, LayoutService, TranslationService, UtilsService } from "@core";
import { of } from "rxjs";
import { ToastrService } from "ngx-toastr";

@Component({
    selector: "office-layout",
    templateUrl: "./layout.component.html"
})
export class OfficeLayoutComponent implements OnInit, OnDestroy {
    //@ViewChild(AppMainComponent, { static: true }) public appMain!: AppMainComponent;
    roles = [RoleName.Administrators, RoleName.Offices];
    menuPermissions: string[];

    logoutLabel: string;
    greekMenuLabel: string;
    usMenuLabel: string;
    nickNameLabel: string;
    roleNameLabel: string;
    settingsLabel: string;

    items: any[];
    menus: any[];
    parents: any[] = [];

    constructor(
        private layoutService: LayoutService,
        private authService: AuthService,
        private utilsService: UtilsService,
        private apiConfigService: ApiConfigService,
        private T: TranslationService) {

        this.menus = [
            // root
            { label: 'root', id: 1, parentId: 0 },
            // 1o Level
            { label: T.translate('menu.accounting'), icon: 'pi pi-fw pi-database', id: 100, parentId: 1 },
            { label: T.translate('menu.financially'), icon: 'pi pi-fw pi-box', id: 130, parentId: 1 },
            { label: T.translate('menu.tax'), icon: 'pi pi-fw pi-wallet', id: 200, parentId: 1 },
            { label: T.translate('menu.especially'), icon: 'pi pi-fw pi-tablet', id: 230, parentId: 1 },
            { label: T.translate('menu.payroll'), icon: 'pi pi-fw pi-money-bill', id: 300, parentId: 1 },
            { label: T.translate('menu.administratively'), icon: 'pi pi-fw pi-sitemap', id: 350, parentId: 1 },
            { label: T.translate('menu.catalog'), icon: 'pi pi-fw pi-users', id: 400, parentId: 1 },
            { label: T.translate('menu.settings'), icon: 'pi pi-fw pi-cog', id: 500, parentId: 1 },
            { label: T.translate('menu.security'), icon: 'pi pi-fw pi-volume-up', id: 600, parentId: 1 },
            { label: T.translate('menu.contentManagment'), icon: 'pi pi-fw pi-database', id: 700, parentId: 1 },
            { label: T.translate('menu.configuration'), icon: 'pi pi-fw pi-wrench', id: 800, parentId: 1 },
            { label: T.translate('menu.system'), icon: 'pi pi-fw pi-android', id: 900, parentId: 1 },
            { label: T.translate('menu.vatExemptionDoc'), icon: 'pi pi-fw pi-file', id: 10000, parentId: 1, visible: !authService.isAuthUserInRole(RoleName.Administrators) },
            // 2o Level
            //accounting
            { label: T.translate('menu.periodicityItems'), routerLink: ['/office/periodicity-items'], menu: 'Protect:PeriodicityItems:List', id: 105, parentId: 100 },
            { label: T.translate('menu.monthlyFinancialBulletin'), routerLink: ['/office/monthly-financial-bulletin'], menu: 'Protect:MonthlyFinancialBulletin:List', id: 110, parentId: 100 },
            { label: T.translate('menu.monthlyBCategoryBulletin'), routerLink: ['/office/monthly-bCategory'], menu: 'Protect:MonthlyBCategoryBulletin:List', id: 111, parentId: 100 },
            { label: T.translate('menu.cashAvailable'), routerLink: ['/office/cash-available'], menu: 'Protect:CashAvailable:List', id: 115, parentId: 100 },
            { label: T.translate('menu.vatTransference'), routerLink: ['/office/vat-transference'], menu: 'Protect:VatTransference:List', id: 225, parentId: 100 },
            { label: T.translate('menu.aggregateAnalysis'), routerLink: ['/office/aggregate-analysis'], menu: 'Protect:AggregateAnalysis:List', id: 190, parentId: 100 },
            { label: T.translate('menu.vatCalculation'), routerLink: ['/office/vat-calculation'], menu: 'Protect:VatCalculation:List', id: 195, parentId: 100 },
            { label: T.translate('menu.countingDocument'), routerLink: ['/office/counting-document'], menu: 'Protect:CountingDocument:List', id: 197, parentId: 100 },
            { label: T.translate('menu.intrastat'), id: 1400, parentId: 100 },
            { label: T.translate('menu.electronicTransmisson'), id: 1500, parentId: 100 },
            { label: T.translate('menu.documents'), id: 1200, parentId: 100 },
            { label: T.translate('menu.payroll'), id: 1600, parentId: 100 },
            { label: T.translate('menu.intertemporalC'), routerLink: ['/office/intertemporal-c'], menu: 'Protect:IntertemporalC:List', id: 220, parentId: 100 },
            { label: T.translate('menu.intertemporalB'), routerLink: ['/office/intertemporal-b'], menu: 'Protect:IntertemporalB:List', id: 221, parentId: 100 },


            //tax
            { label: T.translate('menu.financialObligation'), routerLink: ['/office/financial-obligation'], menu: 'Protect:FinancialObligation:List', id: 205, parentId: 200 },

            //financially
            { label: T.translate('menu.numericalIndicators'), id: 1350, parentId: 130 },
            { label: T.translate('menu.bankingTransactions'), routerLink: ['/office/banking-trader'], menu: 'Protect:BankingTrader:List', id: 1360, parentId: 130 },
            { label: T.translate('menu.bankingTransactions'), id: 1300, parentId: 130 },
            { label: T.translate('menu.nbgTransactions'), routerLink: ['/office/nbg-transactions'], menu: 'Protect:NbgTransactions:List', id: 1700, parentId: 1300 },

            //doc

            //tax

            //especially
            { label: T.translate('menu.projects'), id: 235, parentId: 230 },
            { label: T.translate('menu.medicalExam'), routerLink: ['/office/medical-exam'], menu: 'Protect:MedicalExam:List', id: 240, parentId: 230 },
            { label: T.translate('menu.scripts'), id: 250, parentId: 230, visible: authService.isAuthUserInRole(RoleName.Administrators) || authService.isAuthUserInRole(RoleName.Offices) },
            { label: T.translate('menu.chatAssistant'), routerLink: ['/office/chat-assistant'], menu: 'Protect:MedicalExam:List', id: 255, parentId: 230 },

            //payroll
            { label: T.translate('menu.workerCatalog'), routerLink: ['/office/worker-catalog-by-trader'], menu: 'Protect:WorkerCatalogByTrader:List', id: 305, parentId: 300, visible: authService.isAuthUserInRole(RoleName.Traders), },
            { label: T.translate('menu.apdSubmission'), routerLink: ['/office/apd-submission'], menu: 'Protect:ApdSubmission:List', id: 310, parentId: 322 },
            { label: T.translate('menu.fmySubmission'), routerLink: ['/office/fmy-submission'], menu: 'Protect:FmySubmission:List', id: 315, parentId: 322 },
            { label: T.translate('menu.workerSchedules'), id: 320, parentId: 300, visible: !authService.isAuthUserInRole(RoleName.Administrators) },
            { label: T.translate('menu.workerLeaves'), id: 321, parentId: 300 },
            { label: T.translate('menu.submissions'), id: 322, parentId: 300 },
            { label: T.translate('menu.obligations'), id: 323, parentId: 300 },
            { label: T.translate('menu.payrollStatus'), routerLink: ['/office/payroll-status'], menu: 'Protect:PayrollStatus:List', id: 360, parentId: 300, },


            { label: T.translate('menu.apdTeka'), routerLink: ['/office/apd-teka'], menu: 'Protect:ApdTeka:List', id: 340, parentId: 323 },
            { label: T.translate('menu.apdContribution'), routerLink: ['/office/apd-contribution'], menu: 'Protect:ApdContribution:List', id: 345, parentId: 323 },
            { label: T.translate('menu.fmyContribution'), routerLink: ['/office/fmy-contribution'], menu: 'Protect:FmyContribution:List', id: 346, parentId: 323 },

            { label: T.translate('menu.workerSickLeave'), routerLink: ['/office/worker-sick-leave'], menu: 'Protect:WorkerSickLeave:List', id: 325, parentId: 321 },
            { label: T.translate('menu.workerLeave'), routerLink: ['/office/worker-leave'], menu: 'Protect:WorkerLeave:List', id: 330, parentId: 321 },
            { label: T.translate('menu.workerLeaveDetail'), routerLink: ['/office/worker-leave-detail'], menu: 'Protect:WorkerLeaveDetail:List', id: 335, parentId: 321 },

            { label: T.translate('menu.employeeCostCalculation'), routerLink: ['/office/employee-salary-cost'], menu: 'Protect:EmployeeSalaryCost:List', id: 324, parentId: 300 },
            //administratively
            { label: T.translate('menu.emailMessage'), routerLink: ['/office/email-message'], menu: 'Protect:EmailMessage:List', id: 3400, parentId: 350 },
            { label: T.translate('menu.customerActivity'), routerLink: ['/office/customer-activity'], menu: 'Protect:CustomerActivity:List', id: 3405, parentId: 350 },
            { label: T.translate('menu.traderActivityLog'), routerLink: ['/office/trader-activity-log'], menu: 'Protect:TraderActivityLog:List', id: 3410, parentId: 350 },
            { label: T.translate('menu.employeeActivityLog'), routerLink: ['/office/employee-activity-log'], menu: 'Protect:EmployeeActivityLog:List', id: 3415, parentId: 350 },
            { label: T.translate('menu.crm'), id: 3500, parentId: 350 },
            { label: T.translate('menu.assignment'), id: 3600, parentId: 350 },
            { label: T.translate('menu.traderRating'), id: 3700, parentId: 350 },
            { label: T.translate('menu.traderCharge'), routerLink: ['/office/trader-charge'], menu: 'Protect:TraderCharge:List', id: 3710, parentId: 350 },

            //catalog
            { label: T.translate('menu.traders'), routerLink: ['/office/trader'], menu: 'Protect:Trader:List', id: 405, parentId: 400 },
            { label: T.translate('menu.employees'), routerLink: ['/office/employee'], menu: 'Protect:Employee:List', id: 410, parentId: 400 },
            { label: T.translate('menu.users'), routerLink: ['/office/customer'], menu: 'Protect:Customer:List', id: 415, parentId: 400 },
            { label: T.translate('menu.customerOnline'), routerLink: ['/office/customer-online'], menu: 'Protect:CustomerOnline:List', id: 420, parentId: 400 },

            //settings
            { label: T.translate('menu.emailAccounts'), routerLink: ['/office/email-account'], menu: 'Protect:EmailAccount:List', id: 505, parentId: 500 },

            //security
            { label: T.translate('menu.customerRoles'), routerLink: ['/office/customer-role'], menu: 'Protect:CustomerRole:List', id: 605, parentId: 600 },
            //{ label: T.translate('menu.securityPermissions'), routerLink: ['/office/security'], menu: 'Protect:Security:List', id: 610, parentId: 600 },
            //{ label: T.translate('menu.permissions'), routerLink: ['/office/permissions'], menu: 'Protect:CustomerSecurity:List', id: 615, parentId: 600 },
            { label: T.translate('menu.customerPermissions'), routerLink: ['/office/customer-permission'], menu: 'Protect:CustomerPermission:List', id: 620, parentId: 600 },


            //scripts
            { label: T.translate('menu.scriptTableName'), routerLink: ['/office/script-table-name'], menu: 'Protect:ScriptTableName:List', id: 2500, parentId: 250 },
            { label: T.translate('menu.traders'), routerLink: ['/office/script-trader'], menu: 'Protect:ScriptTrader:List', id: 2505, parentId: 250 },

            //configuration
            { label: T.translate('menu.activityLogType'), routerLink: ['/office/activity-log-type'], menu: 'Protect:ActivityLogType:List', id: 804, parentId: 800 },
            { label: T.translate('menu.traderGroup'), routerLink: ['/office/trader-group'], menu: 'Protect:TraderGroup:List', id: 805, parentId: 800 },
            { label: T.translate('menu.departments'), routerLink: ['/office/department'], menu: 'Protect:AssignmentPrototypeAction:List', id: 810, parentId: 800 },
            { label: T.translate('menu.educations'), routerLink: ['/office/education'], menu: 'Protect:Education:List', id: 815, parentId: 800 },
            { label: T.translate('menu.jobTitles'), routerLink: ['/office/jobTitle'], menu: 'Protect:JobTitle:List', id: 820, parentId: 800 },
            { label: T.translate('menu.specialties'), routerLink: ['/office/specialty'], menu: 'Protect:Specialty:List', id: 825, parentId: 800 },
            { label: T.translate('menu.accountingWorks'), routerLink: ['/office/accounting-work'], menu: 'Protect:AccountingWork:List', id: 830, parentId: 800 },
            { label: T.translate('menu.workingAreas'), routerLink: ['/office/working-area'], menu: 'Protect:WorkingArea:List', id: 835, parentId: 800 },
            { label: T.translate('menu.chambers'), routerLink: ['/office/chamber'], menu: 'Protect:Chamber:List', id: 840, parentId: 800 },
            { label: T.translate('menu.periodicityItems'), routerLink: ['/office/periodicity-item'], menu: 'Protect:PeriodicityItem:List', id: 845, parentId: 800 },
            { label: T.translate('menu.bookmark'), routerLink: ['/office/bookmark'], menu: 'Protect:Bookmark:List', id: 850, parentId: 800 },
            { label: T.translate('menu.taxFactor'), routerLink: ['/office/tax-factor'], menu: 'Protect:TaxFactor:List', id: 860, parentId: 800 },

            //system
            { label: T.translate('menu.languages'), routerLink: ['/office/language'], menu: 'Protect:Language:List', id: 905, parentId: 900 },
            { label: T.translate('menu.logs'), routerLink: ['/office/log'], menu: 'Protect:Log:List', id: 910, parentId: 900 },
            { label: T.translate('menu.scheduleTasks'), routerLink: ['/office/schedule-task'], menu: 'Protect:ScheduleTask:List', id: 950, parentId: 900 },
            { label: T.translate('menu.queuedEmail'), routerLink: ['/office/queued-email'], menu: 'Protect:QueuedEmail:List', id: 955, parentId: 900 },

            //system
            { label: T.translate('menu.vatExemptionApproval'), routerLink: ['/office/vat-exemption-approval'], menu: 'Protect:VatExemptionApproval:List', id: 10010, parentId: 10000 },
            { label: T.translate('menu.vatExemptionReport'), routerLink: ['/office/vat-exemption-report'], menu: 'Protect:VatExemptionReport:List', id: 10010, parentId: 10000 },
            { label: T.translate('menu.vatExemptionSerial'), routerLink: ['/office/vat-exemption-serial'], menu: 'Protect:VatExemptionSerial:List', id: 10010, parentId: 10000 },
            { label: T.translate('menu.vatExemptionDoc'), routerLink: ['/office/vat-exemption-doc'], menu: 'Protect:VatExemptionDoc:List', id: 10020, parentId: 10000 },

            // 3o Level
            //workerSchedule
            { label: T.translate('menu.workerScheduleByEmployee'), routerLink: ['/office/worker-schedule-by-employee'], menu: 'Protect:WorkerScheduleByEmployee:List', id: 3110, parentId: 320 },
            { label: T.translate('menu.workerScheduleByTrader'), routerLink: ['/office/worker-schedule-by-trader'], menu: 'Protect:WorkerScheduleByTrader:List', id: 3120, parentId: 320 },
            { label: T.translate('menu.workerScheduleShiftByTrader'), routerLink: ['/office/worker-schedule-shift-by-trader'], menu: 'Protect:WorkerScheduleShiftByTrader:List', id: 3125, parentId: 320 },
            { label: T.translate('menu.workerScheduleLog'), routerLink: ['/office/worker-schedule-log'], menu: 'Protect:WorkerScheduleLog:List', id: 3130, parentId: 320 },
            { label: T.translate('menu.workerSchedulePending'), routerLink: ['/office/worker-schedule-pending'], menu: 'Protect:WorkerSchedulePending:List', id: 3135, parentId: 320 },

            //doc
            { label: T.translate('menu.periodic-f2'), routerLink: ['/office/periodic-f2'], menu: 'Protect:PeriodicF2:List', id: 1205, parentId: 1200 },

            //numericalIndicators
            { label: T.translate('menu.payoffLiabilities'), routerLink: ['/office/payoff-liabilities'], menu: 'Protect:PayoffLiabilities:List', id: 1355, parentId: 1350 },

            //Intrastat
            { label: T.translate('menu.listingF4'), routerLink: ['/office/listingF4'], menu: 'Protect:ListingF4:List', id: 14000, parentId: 1400 },
            { label: T.translate('menu.listingF5'), routerLink: ['/office/listingF5'], menu: 'Protect:ListingF5:List', id: 14010, parentId: 1400 },

            { label: T.translate('menu.eSend'), routerLink: ['/office/eSend'], menu: 'Protect:ESend:List', id: 15000, parentId: 1500 },
            { label: T.translate('menu.myData'), routerLink: ['/office/myData'], menu: 'Protect:MyData:List', id: 15005, parentId: 1500 },
            { label: T.translate('menu.myDataItem'), routerLink: ['/office/myData-item'], menu: 'Protect:MyDataItem:List', id: 15006, parentId: 1500 },

            //crm
            { label: T.translate('menu.simpleTaskManagers'), routerLink: ['/office/simple-task-manager'], menu: 'Protect:SimpleTaskManager:List', id: 3505, parentId: 3500 },
            { label: T.translate('menu.simpleTaskCategories'), routerLink: ['/office/simple-task-category'], menu: 'Protect:SimpleTaskCategory:List', id: 3510, parentId: 3500 },
            { label: T.translate('menu.simpleTaskDepartments'), routerLink: ['/office/simple-task-department'], menu: 'Protect:SimpleTaskAssignmentPrototypeAction:List', id: 3515, parentId: 3500 },
            { label: T.translate('menu.simpleTaskNatures'), routerLink: ['/office/simple-task-nature'], menu: 'Protect:SimpleTaskNature:List', id: 3520, parentId: 3500 },
            { label: T.translate('menu.simpleTaskSectors'), routerLink: ['/office/simple-task-sector'], menu: 'Protect:SimpleTaskSector:List', id: 3525, parentId: 3500 },

            //assignment
            { label: T.translate('menu.assignmentTask'), routerLink: ['/office/assignment-task'], menu: 'Protect:AssignmentTask:List', id: 3605, parentId: 3600 },
            { label: T.translate('menu.assignmentTaskAction'), routerLink: ['/office/assignment-task-action-by-employee'], menu: 'Protect:AssignmentTaskActionByEmployee:List', id: 3610, parentId: 3600, visible: authService.isAuthUserInRole(RoleName.Employees) },
            { label: T.translate('menu.assignmentPrototype'), routerLink: ['/office/assignment-prototype'], menu: 'Protect:AssignmentPrototype:List', id: 3615, parentId: 3600 },
            { label: T.translate('menu.assignmentPrototypeAction'), routerLink: ['/office/assignment-prototype-action'], menu: 'Protect:AssignmentPrototypeAction:List', id: 3620, parentId: 3600 },
            { label: T.translate('menu.assignmentReason'), routerLink: ['/office/assignment-reason'], menu: 'Protect:AssignmentReason:List', id: 3625, parentId: 3600 },

            //projects
            { label: T.translate('menu.viewProjects'), routerLink: ['/office/softone-project'], menu: 'Protect:SoftoneProject:Preview', id: 2305, parentId: 235 },
            { label: T.translate('menu.projectsStatistic'), routerLink: ['/office/softone-project-statistic'], menu: 'Protect:SoftoneProjectStatistic:Preview', id: 2310, parentId: 235 },

            //costCenter
            { label: T.translate('menu.traderRatingBySummaryTable'), routerLink: ['/office/trader-rating-report', 'by-summary-table'], menu: 'Protect:TraderRatingReport:BySummaryTable', id: 3703, parentId: 3700 },
            { label: T.translate('menu.traderRatingByValuationTable'), routerLink: ['/office/trader-rating-report', 'by-valuation-table'], menu: 'Protect:TraderRatingReport:ByValuationTable', id: 3704, parentId: 3700 },
            { label: T.translate('menu.traderRatingByValuationTrader'), routerLink: ['/office/trader-rating-report', 'by-valuation-trader'], menu: 'Protect:TraderRatingReport:ByValuationTrader', id: 3705, parentId: 3700 },
            { label: T.translate('menu.traderRatingCategory'), routerLink: ['/office/trader-rating-category'], menu: 'Protect:TraderRatingCategory:List', id: 3706, parentId: 3700 },
            { label: T.translate('menu.traderRatingGravity'), routerLink: ['/office/trader-rating'], menu: 'Protect:TraderRating:List', id: 3715, parentId: 3700 },
            { label: T.translate('menu.traderRatingByEmployee'), routerLink: ['/office/trader-rating-report', 'by-employee'], menu: 'Protect:TraderRatingReport:ByEmployee', id: 3780, parentId: 3700 },
            { label: T.translate('menu.traderRatingByDepartment'), routerLink: ['/office/trader-rating-report', 'by-department'], menu: 'Protect:TraderRatingReport:ByDepartment', id: 3785, parentId: 3700 },
            { label: T.translate('menu.traderRatingByTrader'), routerLink: ['/office/trader-rating-report', 'by-trader'], menu: 'Protect:TraderRatingReport:ByTrader', id: 3790, parentId: 3700 },

            //payroll
            { label: T.translate('menu.payrollCheck'), routerLink: ['/office/payroll-check'], menu: 'Protect:PayrollCheck:List', id: 2400, parentId: 1600 },
            { label: T.translate('menu.articlesCheck'), routerLink: ['/office/articles-check'], menu: 'Protect:ArticlesCheck:List', id: 2401, parentId: 1600 },

        ];

    }

    private removeEmptyArray(items: any[]) {
        for (let item of items) {
            if (item.items && Array.isArray(item.items)) {
                if (item.items.length > 0) {
                    this.removeEmptyArray(item.items);
                } else {
                    delete item.items;
                }
            }
        }
    }

    private getParent(menu: any) {
        const parent = this.menus.find(f => f.id == menu.parentId);
        const found = this.parents.find(f => f.id == parent.id);
        if (!found) {
            this.parents.push(parent);
            if (parent.parentId > 0)
                this.getParent(parent);
        }
    }

    sort(obj: any): void {
        let items: any[] = obj.items;
        if (items && items.length > 0) {
            items.forEach((item: any) => {
                this.sort(item);
            });
            items = items.sort((a: any, b: any) => (a.id < b.id) ? -1 : 1);
        }
    }

    ngOnDestroy(): void {
    }

    ngOnInit() {
        const user = this.authService.getAuthUser();

        this.nickNameLabel = user.nickName;
        this.roleNameLabel = user.systemName;
        this.menuPermissions = this.apiConfigService.configuration.menus;

        this.settingsLabel = this.T.translate('menu.settings');
        this.greekMenuLabel = this.T.translate('common.greek');
        this.usMenuLabel = this.T.translate('common.english');
        this.logoutLabel = this.T.translate('common.logout');

        if (this.roleNameLabel === RoleName.Administrators) {
            this.items = this.utilsService.processTable(this.menus);
        } else {

            this.items = this.menus.filter(x => {
                return this.menuPermissions.includes(x.menu);
            });
            this.items.forEach(x => this.getParent(x));

            this.items = this.utilsService.processTable([...this.parents, ...this.items]);
        }
        this.removeEmptyArray(this.items);
        this.sort(this.items[0]);

        this.layoutService.mainMenu = of(this.items);

    }

    assignUniqueIds(menu, parentId) {
        const menuItems = menu.items;

        menuItems.forEach((menuItem, index) => {
            const menuItemId = `${parentId || 'menu'}-${index + 1}`;
            menuItem['id'] = menuItemId;
            menuItem['parentId'] = parentId;

            if (menuItem.items && Array.isArray(menuItem.items)) {
                this.assignUniqueIds(menuItem.items, menuItemId);
            }
        });
    }

    changeTheme(event: any, theme: string) {
        event.preventDefault();
        const href = `assets/kendo-theme-default/dist/${theme}.css`;
        const themeEl: any = document.getElementById('theme');
        themeEl.href = href;
    }

    changeLanguage(event: any, localeId: string) {
        event.preventDefault();
        this.authService.changeLanguage(localeId)
            .then(() => {
                window.location.reload();
            });
    }

    logout(event: any) {
        event.preventDefault();
        // reset the login status
        Promise.resolve(null).then(() => {
            this.authService.logout(true);
        }).then(() => {
            this.apiConfigService.loadApiConfig();
        });

    }

    clearCache(event: any) {
        event.preventDefault();
        this.authService.clearCache();
    }
}

