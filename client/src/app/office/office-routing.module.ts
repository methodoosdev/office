import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@jwtNg';

import { OfficeLayoutComponent } from './layout/layout.component';
import { OfficeHomeComponent } from './home/home.component';

const routes: Routes = [
    {
        path: '',
        component: OfficeLayoutComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: '',
                canActivateChild: [AuthGuard],
                children: [
                    { path: 'home', component: OfficeHomeComponent },
                    //customers
                    { path: 'customer', loadChildren: () => import('./customers/customer/customer.module').then(m => m.CustomerModule) },
                    { path: 'customer-permission', loadChildren: () => import('./customers/customer-permission/customer-permission.module').then(m => m.CustomerPermissionModule) },
                    { path: 'customer-role', loadChildren: () => import('./customers/customer-role/customer-role.module').then(m => m.CustomerRoleModule) },
                    { path: 'customer-online', loadChildren: () => import('./customers/customer-online/customer-online.module').then(m => m.CustomerOnlineModule) },
                    //directory
                    { path: 'bookmark', loadChildren: () => import('./directory/bookmark/bookmark.module').then(m => m.BookmarkModule) },

                    //employees
                    { path: 'employee', loadChildren: () => import('./employees/employee/employee.module').then(m => m.EmployeeModule) },
                    { path: 'department', loadChildren: () => import('./employees/department/department.module').then(m => m.DepartmentModule) },
                    { path: 'education', loadChildren: () => import('./employees/education/education.module').then(m => m.EducationModule) },
                    { path: 'jobTitle', loadChildren: () => import('./employees/jobTitle/jobTitle.module').then(m => m.JobTitleModule) },
                    { path: 'specialty', loadChildren: () => import('./employees/specialty/specialty.module').then(m => m.SpecialtyModule) },
                    //localization
                    { path: 'language', loadChildren: () => import('./localization/language/language.module').then(m => m.LanguageModule) },
                    { path: 'log', loadChildren: () => import('./logging/log/log.module').then(m => m.LogModule) },
                    { path: 'customer-activity', loadChildren: () => import('./customers/customer-activity/customer-activity.module').then(m => m.CustomerActivityModule) },
                    { path: 'activity-log-type', loadChildren: () => import('./logging/activity-log-type/activity-log-type.module').then(m => m.ActivityLogTypeModule) },
                    { path: 'trader-activity-log', loadChildren: () => import('./logging/trader-activity-log/trader-activity-log.module').then(m => m.TraderActivityLogModule) },
                    { path: 'employee-activity-log', loadChildren: () => import('./logging/employee-activity-log/employee-activity-log.module').then(m => m.EmployeeActivityLogModule) },
                    //messages
                    { path: 'email-account', loadChildren: () => import('./messages/email-account/email-account.module').then(m => m.EmailAccountModule) },
                    { path: 'email-message', loadChildren: () => import('./messages/email-message/email-message.module').then(m => m.EmailMessageModule) },
                    { path: 'queued-email', loadChildren: () => import('./messages/queued-email/queued-email.module').then(m => m.QueuedEmailModule) },
                    //offices
                    { path: 'accounting-office', loadChildren: () => import('./offices/accounting-office/accounting-office.module').then(m => m.AccountingOfficeModule) },
                    { path: 'chamber', loadChildren: () => import('./offices/chambers/chamber.module').then(m => m.ChamberModule) },
                    { path: 'periodicity-item', loadChildren: () => import('./offices/periodicity-item/periodicity-item.module').then(m => m.PeriodicityItemModule) },
                    { path: 'tax-factor', loadChildren: () => import('./offices/tax-factor/tax-factor.module').then(m => m.TaxFactorModule) },
                    //security
                    //{ path: 'security', loadChildren: () => import('./customers/security/security.module').then(m => m.SecurityModule) },
                    { path: 'permissions', loadChildren: () => import('./customers/customer-permission/customer-permission.module').then(m => m.CustomerPermissionModule) },
                    //tasks
                    { path: 'schedule-task', loadChildren: () => import('./tasks/schedule-task/schedule-task.module').then(m => m.ScheduleTaskModule) },
                    //traders
                    { path: 'accounting-work', loadChildren: () => import('./traders/accounting-work/accounting-work.module').then(m => m.AccountingWorkModule) },
                    { path: 'trader', loadChildren: () => import('./traders/trader/trader.module').then(m => m.TraderModule) },
                    { path: 'trader-group', loadChildren: () => import('./traders/trader-group/trader-group.module').then(m => m.TraderGroupModule) },
                    { path: 'trader-membership', loadChildren: () => import('./traders/trader-membership/trader-membership.module').then(m => m.TraderMembershipModule) },
                    { path: 'trader-relationship', loadChildren: () => import('./traders/trader-relationship/trader-relationship.module').then(m => m.TraderRelationshipModule) },
                    { path: 'trader-info', loadChildren: () => import('./traders/trader-info/trader-info.module').then(m => m.TraderInfoModule) },
                    { path: 'trader-monthly-billing', loadChildren: () => import('./traders/trader-monthly-billing/trader-monthly-billing.module').then(m => m.TraderMonthlyBillingModule) },
                    //vat-exemption
                    { path: 'vat-exemption-approval', loadChildren: () => import('./vat-exemption/approval/approval.module').then(m => m.VatExemptionApprovalModule) },
                    { path: 'vat-exemption-report', loadChildren: () => import('./vat-exemption/report/report.module').then(m => m.VatExemptionReportModule) },
                    { path: 'vat-exemption-serial', loadChildren: () => import('./vat-exemption/serial/serial.module').then(m => m.VatExemptionSerialModule) },
                    { path: 'vat-exemption-doc', loadChildren: () => import('./vat-exemption/doc/doc.module').then(m => m.VatExemptionDocModule) },
                    { path: 'working-area', loadChildren: () => import('./traders/working-area/working-area.module').then(m => m.WorkingAreaModule) },
                    //tax
                    { path: 'financial-obligation', loadChildren: () => import('./financial/financial-obligation/financial-obligation.module').then(m => m.FinancialObligationModule) },

                    //finance
                    { path: 'payoff-liabilities', loadChildren: () => import('./accounting/payoff-liabilities/payoff-liabilities.module').then(m => m.PayoffLiabilitiesModule) },
                    //{ path: 'banking-transactions', loadChildren: () => import('./finance/banking/banking-transactions/banking-transactions.module').then(m => m.BankingTransactionsModule) },
                    //{ path: 'piraeus-transactions', loadChildren: () => import('./finance/banking/piraeus/piraeus-transactions.module').then(m => m.PiraeusTransactionsModule) },
                    { path: 'nbg-transactions', loadChildren: () => import('./financial/banking/nbg/nbg.module').then(m => m.NbgModule) },
                    { path: 'banking-trader', loadChildren: () => import('./banking/banking-trader/trader.module').then(m => m.BankingTraderModule) },

                    //reports
                    //crm
                    { path: 'simple-task-category', loadChildren: () => import('./simple-task/simple-task-category/simple-task-category.module').then(m => m.SimpleTaskCategoryModule) },
                    { path: 'simple-task-department', loadChildren: () => import('./simple-task/simple-task-department/simple-task-department.module').then(m => m.SimpleTaskDepartmentModule) },
                    { path: 'simple-task-nature', loadChildren: () => import('./simple-task/simple-task-nature/simple-task-nature.module').then(m => m.SimpleTaskNatureModule) },
                    { path: 'simple-task-sector', loadChildren: () => import('./simple-task/simple-task-sector/simple-task-sector.module').then(m => m.SimpleTaskSectorModule) },
                    { path: 'simple-task-manager', loadChildren: () => import('./simple-task/simple-task-manager/simple-task-manager.module').then(m => m.SimpleTaskManagerModule) },
                    //payroll
                    { path: 'worker-catalog-by-trader', loadChildren: () => import('./payroll/worker-catalog/by-trader/by-trader.module').then(m => m.WorkerCatalogByTraderModule) },
                    { path: 'apd-submission', loadChildren: () => import('./payroll/apd-submission/apd-submission.module').then(m => m.ApdSubmissionModule) },
                    { path: 'fmy-submission', loadChildren: () => import('./payroll/fmy-submission/fmy-submission.module').then(m => m.FmySubmissionModule) },
                    { path: 'worker-schedule-by-employee', loadChildren: () => import('./payroll/worker-schedule/schedule-by-employee/by-employee.module').then(m => m.WorkerScheduleByEmployeeModule) },
                    { path: 'worker-schedule-by-trader', loadChildren: () => import('./payroll/worker-schedule/schedule-by-trader/by-trader.module').then(m => m.WorkerScheduleByTraderModule) },
                    { path: 'worker-schedule-shift-by-trader', loadChildren: () => import('./payroll/worker-schedule/shift-by-trader/worker-schedule-shift.module').then(m => m.WorkerScheduleShiftModule) },
                    { path: 'worker-schedule-submit', loadChildren: () => import('./payroll/worker-schedule/submit/worker-schedule-submit.module').then(m => m.WorkerScheduleSubmitModule) },
                    { path: 'worker-schedule-check', loadChildren: () => import('./payroll/worker-schedule/check/worker-schedule-check.module').then(m => m.WorkerScheduleCheckModule) },
                    { path: 'worker-schedule-log', loadChildren: () => import('./payroll/worker-schedule/log/worker-schedule-log.module').then(m => m.WorkerScheduleLogModule) },
                    { path: 'worker-schedule-pending', loadChildren: () => import('./payroll/worker-schedule/pending/worker-schedule-pending.module').then(m => m.WorkerSchedulePendingModule) },
                    { path: 'worker-sick-leave', loadChildren: () => import('./payroll/worker-sick-leave/worker-sick-leave.module').then(m => m.WorkerSickLeaveModule) },
                    { path: 'worker-leave', loadChildren: () => import('./payroll/worker-leave/worker-leave.module').then(m => m.WorkerLeaveModule) },
                    { path: 'worker-leave-detail', loadChildren: () => import('./payroll/worker-leave-detail/leave-detail.module').then(m => m.WorkerLeaveDetailModule) },
                    { path: 'apd-contribution', loadChildren: () => import('./payroll/apd-contribution/apd-contribution.module').then(m => m.ApdContributionModule) },
                    { path: 'fmy-contribution', loadChildren: () => import('./payroll/fmy-contribution/fmy-contribution.module').then(m => m.FmyContributionModule) },
                    { path: 'payroll-status', loadChildren: () => import('./payroll/payroll-status/payroll-status.module').then(m => m.PayrollStatusModule) },
                    { path: 'employee-salary-cost', loadChildren: () => import('./payroll/employee-salary-cost/employee-salary-cost.module').then(m => m.EmployeeSalaryCostModule) },
                    { path: 'apd-teka', loadChildren: () => import('./payroll/apd-teka/apd-teka.module').then(m => m.ApdTekaModule) },

                    //accounting
                    { path: 'medical-exam', loadChildren: () => import('./accounting/medical-exam/medical-exam.module').then(m => m.MedicalExamModule) },
                    { path: 'listingF4', loadChildren: () => import('./accounting/listingF4/listingF4.module').then(m => m.ListingF4Module) },
                    { path: 'listingF5', loadChildren: () => import('./accounting/listingF5/listingF5.module').then(m => m.ListingF5Module) },
                    { path: 'eSend', loadChildren: () => import('./accounting/eSend/eSend.module').then(m => m.ESendModule) },
                    { path: 'myData', loadChildren: () => import('./accounting/myData/myData.module').then(m => m.MyDataModule) },
                    { path: 'myData-item', loadChildren: () => import('./accounting/myData-item/myData-item.module').then(m => m.MyDataItemModule) },
                    { path: 'periodic-f2', loadChildren: () => import('./accounting/periodic-f2/periodic-f2.module').then(m => m.PeriodicF2Module) },
                    { path: 'cash-available', loadChildren: () => import('./accounting/cash-available/cash-available.module').then(m => m.CashAvailableModule) },
                    { path: 'aggregate-analysis', loadChildren: () => import('./accounting/aggregate-analysis/aggregate-analysis.module').then(m => m.AggregateAnalysisModule) },
                    { path: 'monthly-financial-bulletin', loadChildren: () => import('./accounting/monthly-financial-bulletin/monthly-financial-bulletin.module').then(m => m.MonthlyFinancialBulletinModule) },
                    { path: 'monthly-bCategory', loadChildren: () => import('./accounting/monthly-bCategory/monthly-bCategory.module').then(m => m.MonthlyBCategoryBulletinModule) },
                    { path: 'softone-project', loadChildren: () => import('./accounting/softone-project/softone-project.module').then(m => m.SoftoneProjectModule) },
                    { path: 'vat-calculation', loadChildren: () => import('./accounting/vat-calculation/vat-calculation.module').then(m => m.VatCalculationModule) },
                    { path: 'periodicity-items', loadChildren: () => import('./accounting/periodicity-items/periodicity-items.module').then(m => m.PeriodicityItemsModule) },
                    { path: 'vat-transference', loadChildren: () => import('./accounting/vat-transference/vat-transference.module').then(m => m.VatTransferenceModule) },
                    { path: 'payroll-check', loadChildren: () => import('./accounting/payroll-check/payroll-check.module').then(m => m.PayrollCheckModule) },
                    { path: 'articles-check', loadChildren: () => import('./accounting/articles-check/articles-check.module').then(m => m.ArticlesCheckModule) },
                    { path: 'intertemporal-b', loadChildren: () => import('./accounting/intertemporal-b/intertemporal-b.module').then(m => m.IntertemporalBModule) },
                    { path: 'intertemporal-c', loadChildren: () => import('./accounting/intertemporal-c/intertemporal-c.module').then(m => m.IntertemporalCModule) },
                    { path: 'counting-document', loadChildren: () => import('./accounting/counting-document/counting-document.module').then(m => m.CountingDocumentModule) },

                    //assignment
                    { path: 'assignment-reason', loadChildren: () => import('./assignment/reason/reason.module').then(m => m.AssignmentReasonModule) },
                    { path: 'assignment-prototype', loadChildren: () => import('./assignment/prototype/prototype.module').then(m => m.AssignmentPrototypeModule) },
                    { path: 'assignment-prototype-action', loadChildren: () => import('./assignment/prototype-action/prototype-action.module').then(m => m.AssignmentPrototypeActionModule) },
                    { path: 'assignment-task', loadChildren: () => import('./assignment/task/task.module').then(m => m.AssignmentTaskModule) },
                    { path: 'assignment-task-action-by-employee', loadChildren: () => import('./assignment/task-action/by-employee/by-employee.module').then(m => m.AssignmentTaskActionByEmployeeModule) },

                    //trader-rating
                    { path: 'trader-rating-category', loadChildren: () => import('./traders/trader-rating/category/category.module').then(m => m.TraderRatingCategoryModule) },
                    { path: 'trader-rating', loadChildren: () => import('./traders/trader-rating/rating/rating.module').then(m => m.TraderRatingModule) },
                    { path: 'trader-rating-report', loadChildren: () => import('./traders/trader-rating/report/report.module').then(m => m.TraderRatingReportModule) },

                    { path: 'trader-charge', loadChildren: () => import('./traders/trader-charge/trader-charge.module').then(m => m.TraderChargeModule) },

                    //scripts
                    { path: 'script-table-name', loadChildren: () => import('./scripts/table-name/table-name.module').then(m => m.ScriptTableNameModule) },
                    { path: 'script-trader', loadChildren: () => import('./scripts/script-trader/trader.module').then(m => m.ScriptTraderModule) },
                    { path: 'script-report', loadChildren: () => import('./scripts/report/report.module').then(m => m.ScriptReportModule) },
                    { path: 'script-pivot', loadChildren: () => import('./scripts/pivot/pivot.module').then(m => m.ScriptPivotModule) },
                    { path: 'script-tool', loadChildren: () => import('./scripts/tool/tool.module').then(m => m.ScriptToolModule) },
                    { path: 'script-diagram', loadChildren: () => import('./scripts/diagram/diagram.module').then(m => m.ScriptDiagramModule) },

                    //AI
                    { path: 'chat-assistant', loadChildren: () => import('./ai/chat/chat-assistant.module').then(m => m.ChatAssistantModule) },

                    //home
                    { path: '', component: OfficeHomeComponent }
                ]
            }
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class OfficeRoutingModule { }
