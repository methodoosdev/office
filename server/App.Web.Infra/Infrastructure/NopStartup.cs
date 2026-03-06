using App.Core.Infrastructure;
using App.Web.Accounting.Factories;
using App.Web.Common.Factories;
using App.Web.Infra.Factories.Accounting;
using App.Web.Infra.Factories.Common.Assignment;
using App.Web.Infra.Factories.Common.Banking;
using App.Web.Infra.Factories.Common.Customers;
using App.Web.Infra.Factories.Common.Employees;
using App.Web.Infra.Factories.Common.Financial;
using App.Web.Infra.Factories.Common.Logging;
using App.Web.Infra.Factories.Common.Messages;
using App.Web.Infra.Factories.Common.Offices;
using App.Web.Infra.Factories.Common.ScriptPivots;
using App.Web.Infra.Factories.Common.Scripts;
using App.Web.Infra.Factories.Common.SimpleTask;
using App.Web.Infra.Factories.Common.Traders;
using App.Web.Infra.Factories.Common.VatExemption;
using App.Web.Infra.Factories.Payroll;
using App.Web.Infra.Factories.Payroll.WorkerLeave;
using App.Web.Infra.Factories.Payroll.WorkerSchedules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Web.Infra.Infrastructure
{
    public partial class NopStartup : INopStartup
    {
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services
            services.AddScoped<IScriptTraderModelService, ScriptTraderModelService>();

            //factories
            services.AddScoped<IAggregateAnalysisFactory, AggregateAnalysisFactory>();
            services.AddScoped<ICashAvailableFactory, CashAvailableFactory>();
            services.AddScoped<IESendFactory, ESendFactory>();
            services.AddScoped<IMedicalExamFactory, MedicalExamFactory>();
            services.AddScoped<IListingF4Factory, ListingF4Factory>();
            services.AddScoped<IListingF5Factory, ListingF5Factory>();
            services.AddScoped<IMonthlyFinancialBulletinFactory, MonthlyFinancialBulletinFactory>();
            services.AddScoped<IMonthlyBCategoryBulletinFactory, MonthlyBCategoryBulletinFactory>();
            services.AddScoped<IPeriodicF2ModelFactory, PeriodicF2ModelFactory>();
            services.AddScoped<IPayoffLiabilitiesFactory, PayoffLiabilitiesFactory>();
            services.AddScoped<IPayrollCheckModelFactory, PayrollCheckModelFactory>();
            services.AddScoped<IArticlesCheckModelFactory, ArticlesCheckModelFactory>();
            services.AddScoped<IPeriodicityItemsModelFactory, PeriodicityItemsModelFactory>();
            services.AddScoped<ISoftoneProjectDetailModelFactory, SoftoneProjectDetailModelFactory>();
            services.AddScoped<ISoftoneProjectModelFactory, SoftoneProjectModelFactory>();
            services.AddScoped<IVatCalculationFactory, VatCalculationFactory>();
            services.AddScoped<IVatTransferenceFactory, VatTransferenceFactory>();
            services.AddScoped<ICountingDocumentFactory, CountingDocumentFactory>();

            services.AddScoped<IAssignmentPrototypeActionModelFactory, AssignmentPrototypeActionModelFactory>();
            services.AddScoped<IAssignmentPrototypeModelFactory, AssignmentPrototypeModelFactory>();
            services.AddScoped<IAssignmentReasonModelFactory, AssignmentReasonModelFactory>();
            services.AddScoped<IAssignmentTaskActionByEmployeeModelFactory, AssignmentTaskActionByEmployeeModelFactory>();
            services.AddScoped<IAssignmentTaskActionModelFactory, AssignmentTaskActionModelFactory>();
            services.AddScoped<IAssignmentTaskModelFactory, AssignmentTaskModelFactory>();

            services.AddScoped<ICustomerActivityModelFactory, CustomerActivityModelFactory>();
            services.AddScoped<ICustomerModelFactory, CustomerModelFactory>();
            services.AddScoped<ICustomerOnlineModelFactory, CustomerOnlineModelFactory>();
            services.AddScoped<ICustomerPermissionModelFactory, CustomerPermissionModelFactory>();
            services.AddScoped<ICustomerRoleModelFactory, CustomerRoleModelFactory>();
            services.AddScoped<ICustomerSecurityModelFactory, CustomerSecurityModelFactory>();
            services.AddScoped<ITraderActivityLogModelFactory, TraderActivityLogModelFactory>();
            services.AddScoped<IEmployeeActivityLogModelFactory, EmployeeActivityLogModelFactory>();

            services.AddScoped<IDepartmentModelFactory, DepartmentModelFactory>();
            services.AddScoped<IEducationModelFactory, EducationModelFactory>();
            services.AddScoped<IEmployeeModelFactory, EmployeeModelFactory>();
            services.AddScoped<IJobTitleModelFactory, JobTitleModelFactory>();
            services.AddScoped<ISpecialtyModelFactory, SpecialtyModelFactory>();

            services.AddScoped<IFinancialObligationModelFactory, FinancialObligationModelFactory>();
            services.AddScoped<IMyDataModelFactory, MyDataModelFactory>();
            services.AddScoped<IMyDataItemModelFactory, MyDataItemModelFactory>();

            services.AddScoped<IEmailAccountModelFactory, EmailAccountModelFactory>();
            services.AddScoped<IEmailMessageModelFactory, EmailMessageModelFactory>();

            services.AddScoped<IAccountingOfficeModelFactory, AccountingOfficeModelFactory>();
            services.AddScoped<IBookmarkModelFactory, BookmarkModelFactory>();
            services.AddScoped<IChamberModelFactory, ChamberModelFactory>();
            services.AddScoped<IPeriodicityItemModelFactory, PeriodicityItemModelFactory>();
            services.AddScoped<IPersistStateModelFactory, PersistStateModelFactory>();
            services.AddScoped<ITaxFactorModelFactory, TaxFactorModelFactory>();
            services.AddScoped<ITraderLookupModelFactory, TraderLookupModelFactory>();

            services.AddScoped<ISimpleTaskCategoryModelFactory, SimpleTaskCategoryModelFactory>();
            services.AddScoped<ISimpleTaskDepartmentModelFactory, SimpleTaskDepartmentModelFactory>();
            services.AddScoped<ISimpleTaskManagerModelFactory, SimpleTaskManagerModelFactory>();
            services.AddScoped<ISimpleTaskNatureModelFactory, SimpleTaskNatureModelFactory>();
            services.AddScoped<ISimpleTaskSectorModelFactory, SimpleTaskSectorModelFactory>();

            services.AddScoped<IAccountingWorkModelFactory, AccountingWorkModelFactory>();
            services.AddScoped<IBusinessRegistryModelFactory, BusinessRegistryModelFactory>();
            services.AddScoped<ISrfTraderModelFactory, SrfTraderModelFactory>();
            services.AddScoped<ITaxSystemTraderModelFactory, TaxSystemTraderModelFactory>();
            services.AddScoped<ITraderBranchModelFactory, TraderBranchModelFactory>();
            services.AddScoped<ITraderGroupModelFactory, TraderGroupModelFactory>();
            services.AddScoped<ITraderKadModelFactory, TraderKadModelFactory>();
            services.AddScoped<ITraderMembershipModelFactory, TraderMembershipModelFactory>();
            services.AddScoped<ITraderRelationshipModelFactory, TraderRelationshipModelFactory>();
            services.AddScoped<ITraderModelFactory, TraderModelFactory>();
            services.AddScoped<IWorkingAreaModelFactory, WorkingAreaModelFactory>();

            services.AddScoped<IVatExemptionApprovalModelFactory, VatExemptionApprovalModelFactory>();
            services.AddScoped<IVatExemptionDocModelFactory, VatExemptionDocModelFactory>();
            services.AddScoped<IVatExemptionReportModelFactory, VatExemptionReportModelFactory>();
            services.AddScoped<IVatExemptionSerialModelFactory, VatExemptionSerialModelFactory>();

            services.AddScoped<IWorkerLeaveDetailModelFactory, WorkerLeaveDetailModelFactory>();
            services.AddScoped<IWorkerLeaveFactory, WorkerLeaveFactory>();
            services.AddScoped<IWorkerSickLeaveFactory, WorkerSickLeaveFactory>();
            services.AddScoped<IEmployeeSalaryCostModelFactory, EmployeeSalaryCostModelFactory>();

            services.AddScoped<IWorkerScheduleByEmployeeModelFactory, WorkerScheduleByEmployeeModelFactory>();
            services.AddScoped<IWorkerScheduleByTraderModelFactory, WorkerScheduleByTraderModelFactory>();
            services.AddScoped<IWorkerScheduleCheckModelFactory, WorkerScheduleCheckModelFactory>();
            services.AddScoped<IWorkerScheduleLogModelFactory, WorkerScheduleLogModelFactory>();
            services.AddScoped<IWorkerSchedulePendingModelFactory, WorkerSchedulePendingModelFactory>();
            services.AddScoped<IWorkerScheduleShiftByTraderModelFactory, WorkerScheduleShiftByTraderModelFactory>();
            services.AddScoped<IWorkerScheduleSubmitModelFactory, WorkerScheduleSubmitModelFactory>();

            services.AddScoped<IApdTekaModelFactory, ApdTekaModelFactory>();
            services.AddScoped<IApdSubmissionModelFactory, ApdSubmissionModelFactory>();
            services.AddScoped<IFmySubmissionModelFactory, FmySubmissionModelFactory>();
            services.AddScoped<IWorkerCatalogByTraderModelFactory, WorkerCatalogByTraderModelFactory>();
            services.AddScoped<IApdContributionModelFactory, ApdContributionModelFactory>();
            services.AddScoped<IFmyContributionModelFactory, FmyContributionModelFactory>();
            services.AddScoped<IPayrollStatusModelFactory, PayrollStatusModelFactory>();

            services.AddScoped<ITraderRatingCategoryModelFactory, TraderRatingCategoryModelFactory>();
            services.AddScoped<ITraderRatingModelFactory, TraderRatingModelFactory>();
            services.AddScoped<ITraderRatingReportModelFactory, TraderRatingReportModelFactory>();
            services.AddScoped<ITraderInfoModelFactory, TraderInfoModelFactory>();

            services.AddScoped<IIntertemporalCFactory, IntertemporalCFactory>();
            services.AddScoped<IIntertemporalBFactory, IntertemporalBFactory>();

            services.AddScoped<ITraderMonthlyBillingModelFactory, TraderMonthlyBillingModelFactory>();
            services.AddScoped<ITraderChargeFactory, TraderChargeFactory>();

            services.AddScoped<ISoftoneQueryFactory, SoftoneQueryFactory>();

            services.AddScoped<IScriptGroupModelFactory, ScriptGroupModelFactory>();
            services.AddScoped<IScriptToolModelFactory, ScriptToolModelFactory>();
            services.AddScoped<IScriptToolItemModelFactory, ScriptToolItemModelFactory>();
            services.AddScoped<IScriptFieldModelFactory, ScriptFieldModelFactory>();
            services.AddScoped<IScriptItemModelFactory, ScriptItemModelFactory>();
            services.AddScoped<IScriptModelFactory, ScriptModelFactory>();
            services.AddScoped<IScriptPivotItemModelFactory, ScriptPivotItemModelFactory>();
            services.AddScoped<IScriptPivotModelFactory, ScriptPivotModelFactory>();
            services.AddScoped<IScriptTableItemModelFactory, ScriptTableItemModelFactory>();
            services.AddScoped<IScriptTableModelFactory, ScriptTableModelFactory>();
            services.AddScoped<IScriptTableNameModelFactory, ScriptTableNameModelFactory>();
            services.AddScoped<IScriptTraderModelFactory, ScriptTraderModelFactory>();

            services.AddScoped<IAccountListModelFactory, AccountListModelFactory>();
            services.AddScoped<ICardListItemModelFactory, CardListItemModelFactory>();
            services.AddScoped<IUserConnectionBankModelFactory, UserConnectionBankModelFactory>();
            services.AddScoped<IAvailableBankModelFactory, AvailableBankModelFactory>();
            services.AddScoped<IBankingTraderModelFactory, BankingTraderModelFactory>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 2900;
    }
}