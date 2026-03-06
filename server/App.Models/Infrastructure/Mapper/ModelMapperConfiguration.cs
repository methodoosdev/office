using App.Core.Configuration;
using App.Core.Domain.Accounting;
using App.Core.Domain.Assignment;
using App.Core.Domain.Common;
using App.Core.Domain.Configuration;
using App.Core.Domain.Customers;
using App.Core.Domain.Directory;
using App.Core.Domain.Employees;
using App.Core.Domain.Financial;
using App.Core.Domain.Localization;
using App.Core.Domain.Logging;
using App.Core.Domain.Messages;
using App.Core.Domain.Offices;
using App.Core.Domain.Payroll;
using App.Core.Domain.ScheduleTasks;
using App.Core.Domain.Scripts;
using App.Core.Domain.Security;
using App.Core.Domain.SimpleTask;
using App.Core.Domain.Traders;
using App.Core.Domain.VatExemption;
using App.Core.Infrastructure.Dtos.Accounting;
using App.Core.Infrastructure.Dtos.Financial;
using App.Core.Infrastructure.Dtos.Trader;
using App.Core.Infrastructure.Mapper;
using App.Framework.Models;
using App.Models.Accounting;
using App.Models.Assignment;
using App.Models.Banking;
using App.Models.Common;
using App.Models.Customers;
using App.Models.Directory;
using App.Models.Employees;
using App.Models.Financial;
using App.Models.Localization;
using App.Models.Logging;
using App.Models.Messages;
using App.Models.Offices;
using App.Models.Payroll;
using App.Models.Scripts;
using App.Models.Settings;
using App.Models.SimpleTask;
using App.Models.Tasks;
using App.Models.Traders;
using App.Models.VatExemption;
using AutoMapper;
using AutoMapper.Internal;

namespace App.Models.Infrastructure.Mapper
{
    public partial class ModelMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ModelMapperConfiguration()
        {
            //Admin
            CreateLocalizationMaps();
            CreateLoggingMaps();
            CreateTasksMaps();

            //Assignment
            CreatePeriodicF2();

            //Payroll
            PayrollMaps();
            CreateWorkerLeaveDetaillMaps();
            CreateWorkerScheduleMaps();

            //Common
            CreateCommonMaps();
            CreateTradersMaps();
            CreateEmployeesMaps();
            CreateVatExemptionMaps();
            CreateOfficesMaps();
            CreateSimpleTaskMaps();
            CreateAssignmentMaps();
            CreateFinancialMaps();
            CreateScripts();

            //create specific maps
            CreateCustomersMaps();
            CreateDirectoryMaps();
            CreateMessagesMaps();
            CreateMyDataItemsMaps();

            //add some generic mapping rules
            this.Internal().ForAllMaps((mapConfiguration, map) =>
            {
                //exclude Form and CustomProperties from mapping BaseNopModel
                if (typeof(BaseNopModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    //map.ForMember(nameof(BaseNopModel.Form), options => options.Ignore());
                    map.ForMember(nameof(BaseNopModel.CustomProperties), options => options.Ignore());
                    map.ForMember(nameof(BaseNopModel.__entityType), options => options.Ignore());
                }

                //exclude ActiveStoreScopeConfiguration from mapping ISettingsModel
                if (typeof(ISettingsModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ISettingsModel.ActiveStoreScopeConfiguration), options => options.Ignore());

                //exclude some properties from mapping configuration and models
                if (typeof(IConfig).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IConfig.Name), options => options.Ignore());

                //exclude Locales from mapping ILocalizedModel
                if (typeof(ILocalizedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ILocalizedModel<ILocalizedModel>.Locales), options => options.Ignore());

                //exclude some properties from mapping ACL supported entities and models
                if (typeof(IAclSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IAclSupported.SubjectToAcl), options => options.Ignore());
                if (typeof(IAclSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IAclSupportedModel.AvailableCustomerRoles), options => options.Ignore());
                    map.ForMember(nameof(IAclSupportedModel.SelectedCustomerRoleIds), options => options.Ignore());
                }
            });
        }

        //Admin

        //protected virtual void CreateConfigMaps()
        //{
        //    CreateMap<CacheConfig, CacheConfigModel>();
        //    CreateMap<CacheConfigModel, CacheConfig>();

        //    CreateMap<HostingConfig, HostingConfigModel>();
        //    CreateMap<HostingConfigModel, HostingConfig>();

        //    CreateMap<DistributedCacheConfig, DistributedCacheConfigModel>()
        //        .ForMember(model => model.DistributedCacheTypeValues, options => options.Ignore());
        //    CreateMap<DistributedCacheConfigModel, DistributedCacheConfig>();

        //    CreateMap<AzureBlobConfig, AzureBlobConfigModel>();
        //    CreateMap<AzureBlobConfigModel, AzureBlobConfig>()
        //        .ForMember(entity => entity.Enabled, options => options.Ignore())
        //        .ForMember(entity => entity.DataProtectionKeysEncryptWithVault, options => options.Ignore());

        //    CreateMap<InstallationConfig, InstallationConfigModel>();
        //    CreateMap<InstallationConfigModel, InstallationConfig>();

        //    CreateMap<CommonConfig, CommonConfigModel>();
        //    CreateMap<CommonConfigModel, CommonConfig>();

        //    CreateMap<DataConfig, DataConfigModel>()
        //        .ForMember(model => model.DataProviderTypeValues, options => options.Ignore());
        //    CreateMap<DataConfigModel, DataConfig>();
        //}
        protected virtual void CreateLocalizationMaps()
        {
            CreateMap<Language, LanguageModel>();
            CreateMap<LanguageModel, Language>();

            CreateMap<LocaleResourceModel, LocaleStringResource>()
                .ForMember(entity => entity.LanguageId, options => options.Ignore());
            CreateMap<LocaleStringResource, LocaleResourceModel>();
        }

        protected virtual void CreateLoggingMaps()
        {
            CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(model => model.ActivityLogTypeName, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CustomerEmail, options => options.Ignore());
            CreateMap<ActivityLogModel, ActivityLog>();

            CreateMap<ActivityLogType, ActivityLogTypeModel>();
            CreateMap<ActivityLogTypeModel, ActivityLogType>();

            CreateMap<Log, LogModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.FullMessage, options => options.Ignore())
                .ForMember(model => model.CustomerEmail, options => options.Ignore())
                .ForMember(model => model.LogLevelName, options => options.Ignore());
            CreateMap<LogModel, Log>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LogLevelId, options => options.Ignore());

            //CreateMap<ActivityLog, UserActivityModel>();
            //CreateMap<UserActivityModel, ActivityLog>();
        }

        protected virtual void CreateTasksMaps()
        {
            CreateMap<ScheduleTask, ScheduleTaskModel>();
            CreateMap<ScheduleTaskModel, ScheduleTask>()
                .ForMember(entity => entity.LastStartUtc, options => options.Ignore())
                .ForMember(entity => entity.LastEndUtc, options => options.Ignore())
                .ForMember(entity => entity.LastSuccessUtc, options => options.Ignore())
                .ForMember(entity => entity.Type, options => options.Ignore())
                .ForMember(entity => entity.LastEnabledUtc, options => options.Ignore());
        }

        //Assignment
        private void CreatePeriodicF2()
        {
            CreateMap<PeriodicF2, PeriodicF2Model>()
                .ForMember(model => model.TaxPeriodName, options => options.Ignore())
                .ForMember(model => model.SubmitModeTypeIdName, options => options.Ignore())
                .ForMember(model => model.F523TypeIdName, options => options.Ignore())
                .ForMember(model => model.F007Name, options => options.Ignore())
                .ForMember(model => model.F002Value, options => options.Ignore())
                .ForMember(model => model.F005aValue, options => options.Ignore())
                .ForMember(model => model.F005bValue, options => options.Ignore())
                .ForMember(model => model.EmployeeName, options => options.Ignore());
            CreateMap<PeriodicF2Model, PeriodicF2>();

            CreateMap<PeriodicF2Result, PeriodicF2Model>()
                .ForMember(model => model.TaxPeriodName, options => options.Ignore())
                .ForMember(model => model.SubmitModeTypeIdName, options => options.Ignore())
                .ForMember(model => model.F523TypeIdName, options => options.Ignore())
                .ForMember(model => model.F007Name, options => options.Ignore())
                .ForMember(model => model.F002Value, options => options.Ignore())
                .ForMember(model => model.F005aValue, options => options.Ignore())
                .ForMember(model => model.F005bValue, options => options.Ignore())
                .ForMember(model => model.EmployeeName, options => options.Ignore())
                .ForMember(model => model.TraderId, options => options.Ignore())
                .ForMember(model => model.SubmitModeTypeId, options => options.Ignore())
                .ForMember(model => model.F502Previous, options => options.Ignore())
                .ForMember(model => model.F503Previous, options => options.Ignore())
                .ForMember(model => model.F511Previous, options => options.Ignore())
                .ForMember(model => model.F387Calc, options => options.Ignore())
                .ForMember(model => model.F337Calc, options => options.Ignore())
                .ForMember(model => model.F5402Calc, options => options.Ignore())
                .ForMember(model => model.F001a, options => options.Ignore())
                .ForMember(model => model.F001b, options => options.Ignore())
                .ForMember(model => model.F003, options => options.Ignore())
                .ForMember(model => model.F004, options => options.Ignore())
                .ForMember(model => model.F005a, options => options.Ignore())
                .ForMember(model => model.F005b, options => options.Ignore())
                .ForMember(model => model.F006, options => options.Ignore())
                .ForMember(model => model.F008, options => options.Ignore())
                .ForMember(model => model.F009, options => options.Ignore())
                .ForMember(model => model.F101, options => options.Ignore())
                .ForMember(model => model.F102, options => options.Ignore())
                .ForMember(model => model.F103, options => options.Ignore())
                .ForMember(model => model.F104, options => options.Ignore());
            CreateMap<PeriodicF2Model, PeriodicF2Result>();

            CreateMap<PeriodicF2Result, PeriodicF2>()
                .ForMember(model => model.TraderId, options => options.Ignore())
                .ForMember(model => model.SubmitModeTypeId, options => options.Ignore())
                .ForMember(model => model.F502Previous, options => options.Ignore())
                .ForMember(model => model.F503Previous, options => options.Ignore())
                .ForMember(model => model.F511Previous, options => options.Ignore())
                .ForMember(model => model.F387Calc, options => options.Ignore())
                .ForMember(model => model.F337Calc, options => options.Ignore())
                .ForMember(model => model.F5402Calc, options => options.Ignore())
                .ForMember(model => model.F001a, options => options.Ignore())
                .ForMember(model => model.F001b, options => options.Ignore())
                .ForMember(model => model.F003, options => options.Ignore())
                .ForMember(model => model.F004, options => options.Ignore())
                .ForMember(model => model.F005a, options => options.Ignore())
                .ForMember(model => model.F005b, options => options.Ignore())
                .ForMember(model => model.F006, options => options.Ignore())
                .ForMember(model => model.F008, options => options.Ignore())
                .ForMember(model => model.F009, options => options.Ignore())
                .ForMember(model => model.F101, options => options.Ignore())
                .ForMember(model => model.F102, options => options.Ignore())
                .ForMember(model => model.F103, options => options.Ignore())
                .ForMember(model => model.F104, options => options.Ignore());
            CreateMap<PeriodicF2, PeriodicF2Result>();
        }

        protected virtual void CreateFinancialMaps()
        {
            CreateMap<FinancialObligation, FinancialObligationModel>()
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.CustomerTypeId, options => options.Ignore())
                .ForMember(model => model.LegalFormTypeId, options => options.Ignore())
                .ForMember(model => model.CategoryBookTypeId, options => options.Ignore())
                .ForMember(model => model.CustomerTypeName, options => options.Ignore())
                .ForMember(model => model.LegalFormTypeName, options => options.Ignore())
                .ForMember(model => model.CategoryBookTypeName, options => options.Ignore())
                .ForMember(model => model.CustomerName, options => options.Ignore())
                .ForMember(model => model.PeriodName, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore());
            CreateMap<FinancialObligationModel, FinancialObligation>();

            CreateMap<FinancialObligation, FinancialObligationDto>();
            CreateMap<FinancialObligationDto, FinancialObligation>()
                .ForMember(entity => entity.CustomerId, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());
        }

        protected virtual void CreateScripts()
        {
            CreateMap<ScriptField, ScriptFieldModel>()
                .ForMember(model => model.ScriptTableName, options => options.Ignore())
                .ForMember(model => model.ScriptFunctionName, options => options.Ignore())
                .ForMember(model => model.ScriptAggregateTypeName, options => options.Ignore())
                .ForMember(model => model.ScriptFieldTypeName, options => options.Ignore())
                .ForMember(model => model.ScriptDetailName, options => options.Ignore())
                .ForMember(model => model.ParentGroupName, options => options.Ignore())
                .ForMember(model => model.ScriptGroupName, options => options.Ignore());
            CreateMap<ScriptFieldModel, ScriptField>();

            CreateMap<ScriptItem, ScriptItemModel>()
                .ForMember(model => model.ScriptName, options => options.Ignore())
                .ForMember(model => model.ParentName, options => options.Ignore())
                .ForMember(model => model.ScriptTypeName, options => options.Ignore())
                .ForMember(model => model.ScriptOperationTypeName, options => options.Ignore())
                .ForMember(model => model.ParentGroupName, options => options.Ignore());
            CreateMap<ScriptItemModel, ScriptItem>();

            CreateMap<Script, ScriptModel>()
                .ForMember(model => model.ScriptGroupName, options => options.Ignore())
                .ForMember(model => model.ScriptAlignTypeName, options => options.Ignore());
            CreateMap<ScriptModel, Script>();

            CreateMap<ScriptPivotItem, ScriptPivotItemModel>()
                .ForMember(model => model.ScriptPivotName, options => options.Ignore())
                .ForMember(model => model.ScriptFieldName, options => options.Ignore())
                .ForMember(model => model.ScriptOperationTypeName, options => options.Ignore())
                .ForMember(model => model.ScriptFieldTypeName, options => options.Ignore())
                .ForMember(model => model.ScriptDetailName, options => options.Ignore())
                .ForMember(model => model.ParentGroupName, options => options.Ignore());
            CreateMap<ScriptPivotItemModel, ScriptPivotItem>();

            CreateMap<ScriptPivot, ScriptPivotModel>()
                .ForMember(model => model.ScriptGroupName, options => options.Ignore());
            CreateMap<ScriptPivotModel, ScriptPivot>();

            CreateMap<ScriptTableItem, ScriptTableItemModel>()
                .ForMember(model => model.ScriptTableName, options => options.Ignore())
                .ForMember(model => model.ScriptBehaviorTypeName, options => options.Ignore());
            CreateMap<ScriptTableItemModel, ScriptTableItem>();

            CreateMap<ScriptTable, ScriptTableModel>()
                .ForMember(model => model.ScriptGroupName, options => options.Ignore());
            CreateMap<ScriptTableModel, ScriptTable>();

            CreateMap<ScriptTableName, ScriptTableNameModel>();
            CreateMap<ScriptTableNameModel, ScriptTableName>();

            CreateMap<ScriptGroup, ScriptGroupModel>();
            CreateMap<ScriptGroupModel, ScriptGroup>();

            CreateMap<ScriptTool, ScriptToolModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore());
            CreateMap<ScriptToolModel, ScriptTool>();

            CreateMap<ScriptToolItem, ScriptToolItemModel>()
                .ForMember(model => model.Order, options => options.Ignore())
                .ForMember(model => model.ScriptName, options => options.Ignore())
                .ForMember(model => model.ScriptGroupName, options => options.Ignore());
            CreateMap<ScriptToolItemModel, ScriptToolItem>();

            CreateMap<Script, ScriptReport>()
                .ForMember(model => model.Value, options => options.Ignore())
                .ForMember(model => model.ScriptGroupAlignTypeId, options => options.Ignore());
            CreateMap<ScriptReport, Script>();

            CreateMap<ScriptGroup, ScriptGroup>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<ScriptTable, ScriptTable>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<ScriptTableItem, ScriptTableItem>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<ScriptField, ScriptField>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<Script, Script>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<ScriptItem, ScriptItem>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<ScriptPivot, ScriptPivot>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<ScriptPivotItem, ScriptPivotItem>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<ScriptTool, ScriptTool>().ForMember(d => d.Id, o => o.Ignore());
            CreateMap<ScriptToolItem, ScriptToolItem>().ForMember(d => d.Id, o => o.Ignore());
        }

        //Payroll
        protected virtual void PayrollMaps()
        {
            CreateMap<PayrollCheckDto, PayrollCheckModel>()
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.ErrorMessage, options => options.Ignore())
                .ForMember(model => model.Items, options => options.Ignore());

            CreateMap<ApdTeka, ApdTekaModel>()
                .ForMember(model => model.CompanyName, options => options.Ignore())
                .ForMember(model => model.EmployeeName, options => options.Ignore())
                .ForMember(model => model.ApdSubmitDateOn, options => options.Ignore())
                .ForMember(model => model.TekaSubmitDateOn, options => options.Ignore())
                .ForMember(model => model.InfoDateOn, options => options.Ignore())
                .ForMember(model => model.PeriodName, options => options.Ignore())
                .ForMember(model => model.ErrorMessage, options => options.Ignore());
            CreateMap<ApdTekaModel, ApdTeka>();
        }

        protected virtual void CreateWorkerLeaveDetaillMaps()
        {
            CreateMap<WorkerLeaveDetail, WorkerLeaveDetailModel>()
                .ForMember(model => model.TraderName, options => options.Ignore());
            CreateMap<WorkerLeaveDetailModel, WorkerLeaveDetail>();
        }

        protected virtual void CreateWorkerScheduleMaps()
        {
            CreateMap<WorkerSchedule, WorkerScheduleModel>()
                .ForMember(model => model.Description, options => options.Ignore())
                .ForMember(model => model.Workers, options => options.Ignore())
                .ForMember(model => model.WorkerCardNames, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.EmployeeName, options => options.Ignore())
                .ForMember(model => model.WorkerScheduleTypeName, options => options.Ignore())
                .ForMember(model => model.WorkerScheduleModeTypeName, options => options.Ignore());
            CreateMap<WorkerScheduleModel, WorkerSchedule>();

            CreateMap<WorkerScheduleDate, WorkerScheduleDateModel>()
                .ForMember(model => model.OvertimeTypeName, options => options.Ignore())
                .ForMember(model => model.DailyNonstop, options => options.Ignore())
                .ForMember(model => model.DailySplit, options => options.Ignore())
                .ForMember(model => model.DailyBreak, options => options.Ignore())
                .ForMember(model => model.DailyTotalHours, options => options.Ignore())
                .ForMember(model => model.WorkerCardName, options => options.Ignore())
                .ForMember(model => model.WorkerName, options => options.Ignore());
            CreateMap<WorkerScheduleDateModel, WorkerScheduleDate>()
                .ForMember(entity => entity.Active, options => options.Ignore());

            CreateMap<WorkerScheduleShift, WorkerScheduleShiftModel>();
            CreateMap<WorkerScheduleShiftModel, WorkerScheduleShift>();

            CreateMap<WorkerScheduleWorker, WorkerScheduleWorkerModel>()
                .ForMember(model => model.TraderName, options => options.Ignore());
            CreateMap<WorkerScheduleWorkerModel, WorkerScheduleWorker>()
                .ForMember(entity => entity.ActiveCard, options => options.Ignore());

            CreateMap<WorkerScheduleLog, WorkerScheduleLogModel>()
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.SubmitDateValue, options => options.Ignore())
                .ForMember(model => model.WorkerScheduleModeTypeId, options => options.Ignore())
                .ForMember(model => model.WorkerScheduleModeTypeName, options => options.Ignore());
            CreateMap<WorkerScheduleLogModel, WorkerScheduleLog>();

        }

        //Common
        protected virtual void CreateCommonMaps()
        {
            CreateMap<Setting, SettingModel>()
                .ForMember(setting => setting.AvailableStores, options => options.Ignore())
                .ForMember(setting => setting.Store, options => options.Ignore());

            CreateMap<PersistState, PersistStateModel>()
                .ForMember(model => model.CustomerEmail, options => options.Ignore());
            CreateMap<PersistStateModel, PersistState>();
        }

        protected virtual void CreateTradersMaps()
        {
            CreateMap<TraderLookupItem, TraderLookupModel>();
            CreateMap<TraderLookupModel, TraderLookupItem>();

            CreateMap<AccountingWork, AccountingWorkModel>();
            CreateMap<AccountingWorkModel, AccountingWork>();

            CreateMap<WorkingArea, WorkingAreaModel>();
            CreateMap<WorkingAreaModel, WorkingArea>();

            CreateMap<TraderGroup, TraderGroupModel>();
            CreateMap<TraderGroupModel, TraderGroup>();

            //CreateMap<Trader, TraderDecryptModel>()
            //    .ForMember(model => model.Id, opt => opt.MapFrom(src => src.Id))
            //    .ForMember(model => model.Vat, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Vat))
            //    .ForMember(model => model.LastName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.LastName))
            //    .ForMember(model => model.FirstName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.FirstName))
            //    .ForMember(model => model.Email, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Email))
            //    .ForMember(model => model.Email2, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Email2))
            //    .ForMember(model => model.Email3, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Email3))
            //    .ForMember(model => model.TaxisUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TaxisUserName))
            //    .ForMember(model => model.TaxisPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TaxisPassword))
            //    .ForMember(model => model.RepresentativeUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.RepresentativeUserName))
            //    .ForMember(model => model.RepresentativePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.RepresentativePassword));

            CreateMap<Trader, TraderModel>()
                .ForMember(model => model.Vat, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Vat))
                .ForMember(model => model.LastName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.LastName))
                .ForMember(model => model.FirstName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.FirstName))

                // Γενικά στοιχεία
                .ForMember(model => model.CodeName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.CodeName))
                .ForMember(model => model.LastName2, opt => opt.ConvertUsing(new DecryptConverter(), src => src.LastName2))
                .ForMember(model => model.FatherLastName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.FatherLastName))
                .ForMember(model => model.FatherFirstName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.FatherFirstName))
                .ForMember(model => model.MotherLastName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.MotherLastName))
                .ForMember(model => model.MotherFirstname, opt => opt.ConvertUsing(new DecryptConverter(), src => src.MotherFirstname))
                .ForMember(model => model.IdentityNumber, opt => opt.ConvertUsing(new DecryptConverter(), src => src.IdentityNumber))
                .ForMember(model => model.Amka, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Amka))
                .ForMember(model => model.Gemh, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Gemh))
                .ForMember(model => model.AmIka, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmIka))
                .ForMember(model => model.AmOaee, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmOaee))
                .ForMember(model => model.AmOga, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmOga))
                .ForMember(model => model.AmEtaa, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmEtaa))

                .ForMember(model => model.AmDiasIka, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmDiasIka))
                .ForMember(model => model.AmDiasEtea, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmDiasEtea))
                .ForMember(model => model.AmEmployer, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmEmployer))
                .ForMember(model => model.AmRetirement, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmRetirement))
                .ForMember(model => model.AmsOga, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmsOga))
                .ForMember(model => model.AmsNat, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AmsNat))
                .ForMember(model => model.Eam, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Eam))
                .ForMember(model => model.TradeName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TradeName))

                // Στοιχεία έδρας
                .ForMember(model => model.RegisterCode, opt => opt.ConvertUsing(new DecryptConverter(), src => src.RegisterCode))
                .ForMember(model => model.JobAddress, opt => opt.ConvertUsing(new DecryptConverter(), src => src.JobAddress))
                .ForMember(model => model.JobStreetNumber, opt => opt.ConvertUsing(new DecryptConverter(), src => src.JobStreetNumber))
                .ForMember(model => model.JobCity, opt => opt.ConvertUsing(new DecryptConverter(), src => src.JobCity))
                .ForMember(model => model.JobMunicipality, opt => opt.ConvertUsing(new DecryptConverter(), src => src.JobMunicipality))
                .ForMember(model => model.JobPlace, opt => opt.ConvertUsing(new DecryptConverter(), src => src.JobPlace))
                .ForMember(model => model.JobPostcode, opt => opt.ConvertUsing(new DecryptConverter(), src => src.JobPostcode))
                .ForMember(model => model.JobPhoneNumber1, opt => opt.ConvertUsing(new DecryptConverter(), src => src.JobPhoneNumber1))
                .ForMember(model => model.JobPhoneNumber2, opt => opt.ConvertUsing(new DecryptConverter(), src => src.JobPhoneNumber2))
                .ForMember(model => model.JobFax, opt => opt.ConvertUsing(new DecryptConverter(), src => src.JobFax))

                // Στοιχεία οικίας
                .ForMember(model => model.HomeAddress, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomeAddress))
                .ForMember(model => model.HomeStreetNumber, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomeStreetNumber))
                .ForMember(model => model.HomeCity, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomeCity))
                .ForMember(model => model.HomeMunicipality, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomeMunicipality))
                .ForMember(model => model.HomePlace, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomePlace))
                .ForMember(model => model.HomePostcode, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomePostcode))
                .ForMember(model => model.HomePhoneNumber1, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomePhoneNumber1))
                .ForMember(model => model.HomePhoneNumber2, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomePhoneNumber2))
                .ForMember(model => model.HomeCellphone, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomeCellphone))
                .ForMember(model => model.HomeFax, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HomeFax))

                .ForMember(model => model.Email, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Email))
                .ForMember(model => model.Email2, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Email2))
                .ForMember(model => model.Email3, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Email3))
                .ForMember(model => model.Iban, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Iban))

                // Στοιχεία νομίμου εκπροσώπου - λογιστή
                .ForMember(model => model.RepresentativeUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.RepresentativeUserName))
                .ForMember(model => model.RepresentativePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.RepresentativePassword))
                .ForMember(model => model.IbanPeriodicF2, opt => opt.ConvertUsing(new DecryptConverter(), src => src.IbanPeriodicF2))

                // Στοιχεία σύνδεσης
                .ForMember(model => model.TaxisUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TaxisUserName))
                .ForMember(model => model.TaxisPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TaxisPassword))
                .ForMember(model => model.TaxisKeyNumber, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TaxisKeyNumber))

                .ForMember(model => model.SpecialTaxisUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.SpecialTaxisUserName))
                .ForMember(model => model.SpecialTaxisPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.SpecialTaxisPassword))
                .ForMember(model => model.EfkaUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.EfkaUserName))
                .ForMember(model => model.EfkaPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.EfkaPassword))

                .ForMember(model => model.IntrastatUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.IntrastatUserName))
                .ForMember(model => model.IntrastatPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.IntrastatPassword))
                .ForMember(model => model.EstateUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.EstateUserName))
                .ForMember(model => model.EstatePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.EstatePassword))
                .ForMember(model => model.KeaoUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.KeaoUserName))
                .ForMember(model => model.KeaoPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.KeaoPassword))
                .ForMember(model => model.OaeeUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OaeeUserName))
                .ForMember(model => model.OaeePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OaeePassword))
                .ForMember(model => model.OaeeKeynumber, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OaeeKeynumber))
                .ForMember(model => model.EmployerIkaUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.EmployerIkaUserName))
                .ForMember(model => model.EmployerIkaPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.EmployerIkaPassword))
                .ForMember(model => model.EmployeeIkaUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.EmployeeIkaUserName))
                .ForMember(model => model.EmployeeIkaPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.EmployeeIkaPassword))
                .ForMember(model => model.IkaKeyNumber, opt => opt.ConvertUsing(new DecryptConverter(), src => src.IkaKeyNumber))
                .ForMember(model => model.OaedUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OaedUserName))
                .ForMember(model => model.OaedPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OaedPassword))
                .ForMember(model => model.OaedKeyNumber, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OaedKeyNumber))
                .ForMember(model => model.GemhUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.GemhUserName))
                .ForMember(model => model.GemhPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.GemhPassword))
                .ForMember(model => model.ErmisUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.ErmisUserName))
                .ForMember(model => model.ErmisPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.ErmisPassword))
                .ForMember(model => model.OpsydUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OpsydUserName))
                .ForMember(model => model.OpsydPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OpsydPassword))
                .ForMember(model => model.SepeUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.SepeUserName))
                .ForMember(model => model.SepePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.SepePassword))

                .ForMember(model => model.OeeUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OeeUserName))
                .ForMember(model => model.OeePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OeePassword))
                .ForMember(model => model.OpekepeUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OpekepeUserName))
                .ForMember(model => model.OpekepePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OpekepePassword))
                .ForMember(model => model.AgrotiUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AgrotiUserName))
                .ForMember(model => model.AgrotiPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AgrotiPassword))
                .ForMember(model => model.TepahUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TepahUserName))
                .ForMember(model => model.TepahPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TepahPassword))
                .ForMember(model => model.NatUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.NatUserName))
                .ForMember(model => model.NatPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.NatPassword))

                .ForMember(model => model.DipetheUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.DipetheUserName))
                .ForMember(model => model.DipethePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.DipethePassword))
                .ForMember(model => model.ModUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.ModUserName))
                .ForMember(model => model.ModPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.ModPassword))

                .ForMember(model => model.SmokePrdUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.SmokePrdUserName))
                .ForMember(model => model.SmokePrdPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.SmokePrdPassword))
                .ForMember(model => model.Article39UserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Article39UserName))
                .ForMember(model => model.Article39Password, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Article39Password))

                .ForMember(model => model.MhdasoUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.MhdasoUserName))
                .ForMember(model => model.MhdasoPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.MhdasoPassword))
                .ForMember(model => model.MydataUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.MydataUserName))
                .ForMember(model => model.MydataPaswword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.MydataPaswword))
                .ForMember(model => model.MydataApi, opt => opt.ConvertUsing(new DecryptConverter(), src => src.MydataApi))

                //migration
                .ForMember(model => model.KeaoIkaUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.KeaoIkaUserName))
                .ForMember(model => model.KeaoIkaPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.KeaoIkaPassword))
                .ForMember(model => model.KeaoOaeeUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.KeaoOaeeUserName))
                .ForMember(model => model.KeaoOaeePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.KeaoOaeePassword))
                .ForMember(model => model.KeaoEfkaUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.KeaoEfkaUserName))
                .ForMember(model => model.KeaoEfkaPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.KeaoEfkaPassword))

                // SoftOne
                .ForMember(model => model.LogistikiDataBaseName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.LogistikiDataBaseName))
                .ForMember(model => model.LogistikiUsername, opt => opt.ConvertUsing(new DecryptConverter(), src => src.LogistikiUsername))
                .ForMember(model => model.LogistikiPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.LogistikiPassword))
                .ForMember(model => model.LogistikiIpAddress, opt => opt.ConvertUsing(new DecryptConverter(), src => src.LogistikiIpAddress))
                .ForMember(model => model.LogistikiPort, opt => opt.ConvertUsing(new DecryptConverter(), src => src.LogistikiPort))

                .ForMember(model => model.WebSite, opt => opt.ConvertUsing(new DecryptConverter(), src => src.WebSite))
                //
                .ForMember(model => model.FullName, options => options.Ignore())
                .ForMember(model => model.EmployeeName, options => options.Ignore())
                .ForMember(model => model.ChamberName, options => options.Ignore())
                .ForMember(model => model.TraderGroupName, options => options.Ignore())
                .ForMember(model => model.WorkingAreaName, options => options.Ignore())
                .ForMember(model => model.ProfessionTypeName, options => options.Ignore())
                .ForMember(model => model.CategoryBookTypeName, options => options.Ignore())
                .ForMember(model => model.LegalFormTypeName, options => options.Ignore())
                .ForMember(model => model.CustomerTypeName, options => options.Ignore())
                .ForMember(model => model.LogistikiProgramTypeName, options => options.Ignore())
                .ForMember(model => model.PayrollProgramTypeName, options => options.Ignore())
                .ForMember(model => model.ParentName, options => options.Ignore())
                .ForMember(model => model.BoardMemberTypeName, options => options.Ignore());

            CreateMap<TraderModel, Trader>()
                .ForMember(entity => entity.Vat, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Vat))
                .ForMember(entity => entity.LastName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LastName))
                .ForMember(entity => entity.FirstName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.FirstName))

                // Γενικά στοιχεία
                .ForMember(entity => entity.CodeName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.CodeName))
                .ForMember(entity => entity.LastName2, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LastName2))
                .ForMember(entity => entity.FatherLastName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.FatherLastName))
                .ForMember(entity => entity.FatherFirstName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.FatherFirstName))
                .ForMember(entity => entity.MotherLastName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MotherLastName))
                .ForMember(entity => entity.MotherFirstname, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MotherFirstname))
                .ForMember(entity => entity.IdentityNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.IdentityNumber))
                .ForMember(entity => entity.Amka, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Amka))
                .ForMember(entity => entity.Gemh, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Gemh))
                .ForMember(entity => entity.AmIka, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmIka))
                .ForMember(entity => entity.AmOaee, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmOaee))
                .ForMember(entity => entity.AmOga, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmOga))
                .ForMember(entity => entity.AmEtaa, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmEtaa))

                .ForMember(entity => entity.AmDiasIka, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmDiasIka))
                .ForMember(entity => entity.AmDiasEtea, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmDiasEtea))
                .ForMember(entity => entity.AmEmployer, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmEmployer))
                .ForMember(entity => entity.AmRetirement, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmRetirement))
                .ForMember(entity => entity.AmsOga, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmsOga))
                .ForMember(entity => entity.AmsNat, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmsNat))
                .ForMember(entity => entity.Eam, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Eam))
                .ForMember(entity => entity.TradeName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TradeName))

                // Στοιχεία έδρας
                .ForMember(entity => entity.RegisterCode, opt => opt.ConvertUsing(new EncryptConverter(), src => src.RegisterCode))
                .ForMember(entity => entity.JobAddress, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobAddress))
                .ForMember(entity => entity.JobStreetNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobStreetNumber))
                .ForMember(entity => entity.JobCity, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobCity))
                .ForMember(entity => entity.JobMunicipality, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobMunicipality))
                .ForMember(entity => entity.JobPlace, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobPlace))
                .ForMember(entity => entity.JobPostcode, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobPostcode))
                .ForMember(entity => entity.JobPhoneNumber1, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobPhoneNumber1))
                .ForMember(entity => entity.JobPhoneNumber2, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobPhoneNumber2))
                .ForMember(entity => entity.JobFax, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobFax))

                // Στοιχεία οικίας
                .ForMember(entity => entity.HomeAddress, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeAddress))
                .ForMember(entity => entity.HomeStreetNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeStreetNumber))
                .ForMember(entity => entity.HomeCity, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeCity))
                .ForMember(entity => entity.HomeMunicipality, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeMunicipality))
                .ForMember(entity => entity.HomePlace, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomePlace))
                .ForMember(entity => entity.HomePostcode, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomePostcode))
                .ForMember(entity => entity.HomePhoneNumber1, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomePhoneNumber1))
                .ForMember(entity => entity.HomePhoneNumber2, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomePhoneNumber2))
                .ForMember(entity => entity.HomeCellphone, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeCellphone))
                .ForMember(entity => entity.HomeFax, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeFax))

                .ForMember(entity => entity.Email, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Email))
                .ForMember(entity => entity.Email2, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Email2))
                .ForMember(entity => entity.Email3, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Email3))
                .ForMember(entity => entity.Iban, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Iban))

                // Στοιχεία νομίμου εκπροσώπου - λογιστή
                .ForMember(entity => entity.RepresentativeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.RepresentativeUserName))
                .ForMember(entity => entity.RepresentativePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.RepresentativePassword))
                .ForMember(entity => entity.IbanPeriodicF2, opt => opt.ConvertUsing(new EncryptConverter(), src => src.IbanPeriodicF2))

                // Στοιχεία σύνδεσης
                .ForMember(entity => entity.TaxisUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisUserName))
                .ForMember(entity => entity.TaxisPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisPassword))
                .ForMember(entity => entity.TaxisKeyNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisKeyNumber))

                .ForMember(entity => entity.SpecialTaxisUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SpecialTaxisUserName))
                .ForMember(entity => entity.SpecialTaxisPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SpecialTaxisPassword))
                .ForMember(entity => entity.EfkaUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EfkaUserName))
                .ForMember(entity => entity.EfkaPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EfkaPassword))

                .ForMember(entity => entity.IntrastatUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.IntrastatUserName))
                .ForMember(entity => entity.IntrastatPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.IntrastatPassword))
                .ForMember(entity => entity.EstateUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EstateUserName))
                .ForMember(entity => entity.EstatePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EstatePassword))
                .ForMember(entity => entity.KeaoUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoUserName))
                .ForMember(entity => entity.KeaoPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoPassword))
                .ForMember(entity => entity.OaeeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaeeUserName))
                .ForMember(entity => entity.OaeePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaeePassword))
                .ForMember(entity => entity.OaeeKeynumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaeeKeynumber))
                .ForMember(entity => entity.EmployerIkaUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployerIkaUserName))
                .ForMember(entity => entity.EmployerIkaPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployerIkaPassword))
                .ForMember(entity => entity.EmployeeIkaUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployeeIkaUserName))
                .ForMember(entity => entity.EmployeeIkaPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployeeIkaPassword))
                .ForMember(entity => entity.IkaKeyNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.IkaKeyNumber))
                .ForMember(entity => entity.OaedUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaedUserName))
                .ForMember(entity => entity.OaedPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaedPassword))
                .ForMember(entity => entity.OaedKeyNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaedKeyNumber))
                .ForMember(entity => entity.GemhUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.GemhUserName))
                .ForMember(entity => entity.GemhPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.GemhPassword))
                .ForMember(entity => entity.ErmisUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ErmisUserName))
                .ForMember(entity => entity.ErmisPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ErmisPassword))
                .ForMember(entity => entity.OpsydUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OpsydUserName))
                .ForMember(entity => entity.OpsydPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OpsydPassword))
                .ForMember(entity => entity.SepeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SepeUserName))
                .ForMember(entity => entity.SepePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SepePassword))

                .ForMember(entity => entity.OeeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OeeUserName))
                .ForMember(entity => entity.OeePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OeePassword))
                .ForMember(entity => entity.OpekepeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OpekepeUserName))
                .ForMember(entity => entity.OpekepePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OpekepePassword))
                .ForMember(entity => entity.AgrotiUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AgrotiUserName))
                .ForMember(entity => entity.AgrotiPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AgrotiPassword))
                .ForMember(entity => entity.TepahUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TepahUserName))
                .ForMember(entity => entity.TepahPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TepahPassword))
                .ForMember(entity => entity.NatUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.NatUserName))
                .ForMember(entity => entity.NatPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.NatPassword))

                .ForMember(entity => entity.DipetheUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.DipetheUserName))
                .ForMember(entity => entity.DipethePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.DipethePassword))
                .ForMember(entity => entity.ModUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ModUserName))
                .ForMember(entity => entity.ModPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ModPassword))

                .ForMember(entity => entity.SmokePrdUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SmokePrdUserName))
                .ForMember(entity => entity.SmokePrdPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SmokePrdPassword))
                .ForMember(entity => entity.Article39UserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Article39UserName))
                .ForMember(entity => entity.Article39Password, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Article39Password))

                .ForMember(entity => entity.MhdasoUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MhdasoUserName))
                .ForMember(entity => entity.MhdasoPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MhdasoPassword))
                .ForMember(entity => entity.MydataUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MydataUserName))
                .ForMember(entity => entity.MydataPaswword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MydataPaswword))
                .ForMember(entity => entity.MydataApi, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MydataApi))

                //migration
                .ForMember(entity => entity.KeaoIkaUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoIkaUserName))
                .ForMember(entity => entity.KeaoIkaPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoIkaPassword))
                .ForMember(entity => entity.KeaoOaeeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoOaeeUserName))
                .ForMember(entity => entity.KeaoOaeePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoOaeePassword))
                .ForMember(entity => entity.KeaoEfkaUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoEfkaUserName))
                .ForMember(entity => entity.KeaoEfkaPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoEfkaPassword))

                // SoftOne
                .ForMember(entity => entity.LogistikiDataBaseName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiDataBaseName))
                .ForMember(entity => entity.LogistikiUsername, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiUsername))
                .ForMember(entity => entity.LogistikiPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiPassword))
                .ForMember(entity => entity.LogistikiIpAddress, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiIpAddress))
                .ForMember(entity => entity.LogistikiPort, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiPort))

                .ForMember(entity => entity.WebSite, opt => opt.ConvertUsing(new EncryptConverter(), src => src.WebSite))

                .ForMember(entity => entity.ActivatedType, options => options.Ignore())
                .ForMember(entity => entity.StatusType, options => options.Ignore())
                .ForMember(entity => entity.FarmerType, options => options.Ignore())
                .ForMember(entity => entity.GenderType, options => options.Ignore())
                .ForMember(entity => entity.CategoryBookType, options => options.Ignore())
                .ForMember(entity => entity.LegalFormType, options => options.Ignore())
                .ForMember(entity => entity.VatSystemType, options => options.Ignore())
                .ForMember(entity => entity.EmploymentLevelType, options => options.Ignore())
                .ForMember(entity => entity.ProfessionType, options => options.Ignore())
                .ForMember(entity => entity.AccountingPlanType, options => options.Ignore())
                .ForMember(entity => entity.PopulationType, options => options.Ignore())
                .ForMember(entity => entity.GroupId, options => options.Ignore())
                .ForMember(entity => entity.LogistikiProgramType, options => options.Ignore())
                .ForMember(entity => entity.PayrollProgramType, options => options.Ignore())
                .ForMember(entity => entity.ParentId, options => options.Ignore())
                .ForMember(entity => entity.CustomerType, options => options.Ignore());

            CreateMap<Trader, TraderFullNameModel>()
                .ForMember(model => model.Vat, opt => opt.ConvertUsing(new DecryptConverter(), src => src.Vat))
                .ForMember(model => model.LastName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.LastName))
                .ForMember(model => model.FirstName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.FirstName));

            CreateMap<SrfTraderModel, Trader>()
                .ForMember(entity => entity.Id, options => options.Ignore())
                .ForMember(entity => entity.Vat, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Vat))
                .ForMember(entity => entity.LastName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LastName))
                .ForMember(entity => entity.FirstName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.FirstName))
                .ForMember(entity => entity.Email, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Email))

                // Στοιχεία σύνδεσης
                .ForMember(entity => entity.TaxisUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisUserName))
                .ForMember(entity => entity.TaxisPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisPassword))

                .ForMember(entity => entity.SpecialTaxisUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SpecialTaxisUserName))
                .ForMember(entity => entity.SpecialTaxisPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SpecialTaxisPassword))
                .ForMember(entity => entity.EfkaUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EfkaUserName))
                .ForMember(entity => entity.EfkaPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EfkaPassword))

                .ForMember(entity => entity.OaeeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaeeUserName))
                .ForMember(entity => entity.OaeePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaeePassword))
                .ForMember(entity => entity.EmployerIkaUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployerIkaUserName))
                .ForMember(entity => entity.EmployerIkaPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployerIkaPassword))
                .ForMember(entity => entity.SepeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SepeUserName))
                .ForMember(entity => entity.SepePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SepePassword))

                // SoftOne
                .ForMember(entity => entity.LogistikiDataBaseName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiDataBaseName))
                .ForMember(entity => entity.LogistikiUsername, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiUsername))
                .ForMember(entity => entity.LogistikiPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiPassword))
                .ForMember(entity => entity.LogistikiIpAddress, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiIpAddress))
                .ForMember(entity => entity.LogistikiPort, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LogistikiPort));

            CreateMap<TaxSystemTraderModel, Trader>()
                .ForMember(model => model.Id, options => options.Ignore())
                .ForMember(entity => entity.Vat, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Vat))
                .ForMember(entity => entity.LastName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LastName))
                .ForMember(entity => entity.FirstName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.FirstName))

                // Γενικά στοιχεία
                .ForMember(entity => entity.CodeName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.CodeName))
                .ForMember(entity => entity.LastName2, opt => opt.ConvertUsing(new EncryptConverter(), src => src.LastName2))
                .ForMember(entity => entity.FatherLastName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.FatherLastName))
                .ForMember(entity => entity.FatherFirstName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.FatherFirstName))
                .ForMember(entity => entity.MotherLastName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MotherLastName))
                .ForMember(entity => entity.MotherFirstname, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MotherFirstname))
                .ForMember(entity => entity.IdentityNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.IdentityNumber))
                .ForMember(entity => entity.Amka, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Amka))
                .ForMember(entity => entity.Gemh, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Gemh))
                .ForMember(entity => entity.AmIka, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmIka))
                .ForMember(entity => entity.AmOaee, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmOaee))
                .ForMember(entity => entity.AmOga, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmOga))
                .ForMember(entity => entity.AmEtaa, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmEtaa))

                .ForMember(entity => entity.AmDiasIka, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmDiasIka))
                .ForMember(entity => entity.AmDiasEtea, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmDiasEtea))
                .ForMember(entity => entity.AmEmployer, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmEmployer))
                .ForMember(entity => entity.AmRetirement, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmRetirement))
                .ForMember(entity => entity.AmsOga, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmsOga))
                .ForMember(entity => entity.AmsNat, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AmsNat))
                .ForMember(entity => entity.Eam, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Eam))
                .ForMember(entity => entity.TradeName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TradeName))

                // Στοιχεία έδρας
                .ForMember(entity => entity.RegisterCode, opt => opt.ConvertUsing(new EncryptConverter(), src => src.RegisterCode))
                .ForMember(entity => entity.JobAddress, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobAddress))
                .ForMember(entity => entity.JobStreetNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobStreetNumber))
                .ForMember(entity => entity.JobCity, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobCity))
                .ForMember(entity => entity.JobMunicipality, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobMunicipality))
                .ForMember(entity => entity.JobPlace, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobPlace))
                .ForMember(entity => entity.JobPostcode, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobPostcode))
                .ForMember(entity => entity.JobPhoneNumber1, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobPhoneNumber1))
                .ForMember(entity => entity.JobPhoneNumber2, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobPhoneNumber2))
                .ForMember(entity => entity.JobFax, opt => opt.ConvertUsing(new EncryptConverter(), src => src.JobFax))

                // Στοιχεία οικίας
                .ForMember(entity => entity.HomeAddress, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeAddress))
                .ForMember(entity => entity.HomeStreetNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeStreetNumber))
                .ForMember(entity => entity.HomeCity, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeCity))
                .ForMember(entity => entity.HomeMunicipality, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeMunicipality))
                .ForMember(entity => entity.HomePlace, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomePlace))
                .ForMember(entity => entity.HomePostcode, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomePostcode))
                .ForMember(entity => entity.HomePhoneNumber1, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomePhoneNumber1))
                .ForMember(entity => entity.HomePhoneNumber2, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomePhoneNumber2))
                .ForMember(entity => entity.HomeCellphone, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeCellphone))
                .ForMember(entity => entity.HomeFax, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HomeFax))

                .ForMember(entity => entity.Email, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Email))
                .ForMember(entity => entity.Iban, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Iban))

                // Στοιχεία σύνδεσης
                .ForMember(entity => entity.TaxisUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisUserName))
                .ForMember(entity => entity.TaxisPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisPassword))
                .ForMember(entity => entity.TaxisKeyNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisKeyNumber))

                .ForMember(entity => entity.IntrastatUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.IntrastatUserName))
                .ForMember(entity => entity.IntrastatPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.IntrastatPassword))
                .ForMember(entity => entity.EstateUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EstateUserName))
                .ForMember(entity => entity.EstatePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EstatePassword))
                .ForMember(entity => entity.KeaoUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoUserName))
                .ForMember(entity => entity.KeaoPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.KeaoPassword))
                .ForMember(entity => entity.OaeeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaeeUserName))
                .ForMember(entity => entity.OaeePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaeePassword))
                .ForMember(entity => entity.OaeeKeynumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaeeKeynumber))
                .ForMember(entity => entity.EmployerIkaUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployerIkaUserName))
                .ForMember(entity => entity.EmployerIkaPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployerIkaPassword))
                .ForMember(entity => entity.EmployeeIkaUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployeeIkaUserName))
                .ForMember(entity => entity.EmployeeIkaPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.EmployeeIkaPassword))
                .ForMember(entity => entity.IkaKeyNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.IkaKeyNumber))
                .ForMember(entity => entity.OaedUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaedUserName))
                .ForMember(entity => entity.OaedPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaedPassword))
                .ForMember(entity => entity.OaedKeyNumber, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OaedKeyNumber))
                .ForMember(entity => entity.GemhUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.GemhUserName))
                .ForMember(entity => entity.GemhPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.GemhPassword))
                .ForMember(entity => entity.ErmisUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ErmisUserName))
                .ForMember(entity => entity.ErmisPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ErmisPassword))
                .ForMember(entity => entity.OpsydUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OpsydUserName))
                .ForMember(entity => entity.OpsydPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OpsydPassword))
                .ForMember(entity => entity.SepeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SepeUserName))
                .ForMember(entity => entity.SepePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SepePassword))

                .ForMember(entity => entity.OeeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OeeUserName))
                .ForMember(entity => entity.OeePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OeePassword))
                .ForMember(entity => entity.OpekepeUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OpekepeUserName))
                .ForMember(entity => entity.OpekepePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OpekepePassword))
                .ForMember(entity => entity.AgrotiUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AgrotiUserName))
                .ForMember(entity => entity.AgrotiPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AgrotiPassword))
                .ForMember(entity => entity.TepahUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TepahUserName))
                .ForMember(entity => entity.TepahPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TepahPassword))
                .ForMember(entity => entity.NatUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.NatUserName))
                .ForMember(entity => entity.NatPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.NatPassword))

                .ForMember(entity => entity.DipetheUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.DipetheUserName))
                .ForMember(entity => entity.DipethePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.DipethePassword))
                .ForMember(entity => entity.ModUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ModUserName))
                .ForMember(entity => entity.ModPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ModPassword))

                .ForMember(entity => entity.SmokePrdUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SmokePrdUserName))
                .ForMember(entity => entity.SmokePrdPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SmokePrdPassword))
                .ForMember(entity => entity.Article39UserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Article39UserName))
                .ForMember(entity => entity.Article39Password, opt => opt.ConvertUsing(new EncryptConverter(), src => src.Article39Password))

                .ForMember(entity => entity.MhdasoUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MhdasoUserName))
                .ForMember(entity => entity.MhdasoPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MhdasoPassword))
                .ForMember(entity => entity.MydataUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MydataUserName))
                .ForMember(entity => entity.MydataPaswword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MydataPaswword))
                .ForMember(entity => entity.MydataApi, opt => opt.ConvertUsing(new EncryptConverter(), src => src.MydataApi));

            CreateMap<TraderKad, TraderKadModel>();
            CreateMap<TraderBranch, TraderBranchModel>();

            CreateMap<TraderMembership, TraderMembershipModel>()
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.StartDateOn, options => options.Ignore())
                .ForMember(model => model.ExpireDateOn, options => options.Ignore())
                .ForMember(model => model.TraderBoardMemberTypeName, options => options.Ignore());
            CreateMap<TraderMembershipModel, TraderMembership>();

            CreateMap<TraderRelationship, TraderRelationshipModel>()
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.StartDateOn, options => options.Ignore())
                .ForMember(model => model.ExpireDateOn, options => options.Ignore())
                .ForMember(model => model.TraderBoardMemberTypeName, options => options.Ignore());
            CreateMap<TraderRelationshipModel, TraderRelationship>();

            CreateMap<TraderKadDto, TraderKad>();
            CreateMap<TraderBranchDto, TraderBranch>();

            CreateMap<TraderRelationshipDto, TraderRelationship>();
            CreateMap<TraderMembershipDto, TraderMembership>();

            CreateMap<TraderRatingCategory, TraderRatingCategoryModel>();
            CreateMap<TraderRatingCategoryModel, TraderRatingCategory>();

            CreateMap<TraderInfo, TraderInfoModel>();
            CreateMap<TraderInfoModel, TraderInfo>();

            CreateMap<TraderRating, TraderRatingModel>()
                .ForMember(model => model.CategoryName, options => options.Ignore())
                .ForMember(model => model.DepartmentName, options => options.Ignore());
            CreateMap<TraderRatingModel, TraderRating>();

            CreateMap<TraderMonthlyBilling, TraderMonthlyBillingModel>();
            CreateMap<TraderMonthlyBillingModel, TraderMonthlyBilling>();

        }

        protected virtual void CreateEmployeesMaps()
        {
            CreateMap<Education, EducationModel>()
                .ForMember(model => model.Locales, options => options.Ignore()); ;
            CreateMap<EducationModel, Education>();

            CreateMap<Department, DepartmentModel>();
            CreateMap<DepartmentModel, Department>();

            CreateMap<Specialty, SpecialtyModel>();
            CreateMap<SpecialtyModel, Specialty>();

            CreateMap<JobTitle, JobTitleModel>();
            CreateMap<JobTitleModel, JobTitle>();

            CreateMap<Employee, EmployeeModel>()
                .ForMember(model => model.FullName, options => options.Ignore())
                .ForMember(model => model.EducationName, options => options.Ignore())
                .ForMember(model => model.DepartmentName, options => options.Ignore())
                .ForMember(model => model.SpecialtyName, options => options.Ignore())
                .ForMember(model => model.JobTitleName, options => options.Ignore())
                .ForMember(model => model.SupervisorName, options => options.Ignore());
            CreateMap<EmployeeModel, Employee>()
                .ForMember(entity => entity.Deleted, options => options.Ignore());
        }

        protected virtual void CreateVatExemptionMaps()
        {
            CreateMap<VatExemptionApproval, VatExemptionApprovalModel>()
                .ForMember(model => model.CreatedDateValue, options => options.Ignore())
                .ForMember(model => model.StartingDateValue, options => options.Ignore())
                .ForMember(model => model.ExpiryDateValue, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.KendoUpload, options => options.Ignore());
            CreateMap<VatExemptionApprovalModel, VatExemptionApproval>();

            CreateMap<VatExemptionReport, VatExemptionReportModel>()
                .ForMember(model => model.ApprovalNumber, options => options.Ignore())
                .ForMember(model => model.ApprovalLimit, options => options.Ignore())
                .ForMember(model => model.ApprovalDoy, options => options.Ignore())
                .ForMember(model => model.CreatedDateValue, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore());
            CreateMap<VatExemptionReportModel, VatExemptionReport>();

            CreateMap<VatExemptionSerial, VatExemptionSerialModel>()
                .ForMember(model => model.ApprovalNumber, options => options.Ignore())
                .ForMember(model => model.ApprovalLimit, options => options.Ignore())
                .ForMember(model => model.ApprovalActive, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore());
            CreateMap<VatExemptionSerialModel, VatExemptionSerial>();

            CreateMap<VatExemptionDoc, VatExemptionDocModel>()
                .ForMember(model => model.CreatedDateValue, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore());
            CreateMap<VatExemptionDocModel, VatExemptionDoc>();
        }

        protected virtual void CreateOfficesMaps()
        {

            CreateMap<AccountingOffice, AccountingOfficeModel>()
                .ForMember(dest => dest.TaxisNetUserName, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TaxisNetUserName))
                .ForMember(dest => dest.TaxisNetPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TaxisNetPassword))
                .ForMember(dest => dest.AadeRegistryUsername, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AadeRegistryUsername))
                .ForMember(dest => dest.AadeRegistryPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.AadeRegistryPassword))
                .ForMember(dest => dest.OfficeUsername, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OfficeUsername))
                .ForMember(dest => dest.OfficePassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.OfficePassword))
                .ForMember(dest => dest.SrfUsername, opt => opt.ConvertUsing(new DecryptConverter(), src => src.SrfUsername))
                .ForMember(dest => dest.SrfPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.SrfPassword))
                .ForMember(dest => dest.TaxSystemUsername, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TaxSystemUsername))
                .ForMember(dest => dest.TaxSystemPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.TaxSystemPassword))
                .ForMember(dest => dest.HyperPayrollUsername, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HyperPayrollUsername))
                .ForMember(dest => dest.HyperPayrollPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HyperPayrollPassword))
                .ForMember(dest => dest.HyperLogUsername, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HyperLogUsername))
                .ForMember(dest => dest.HyperLogPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.HyperLogPassword))
                .ForMember(dest => dest.ProsvasisUsername, opt => opt.ConvertUsing(new DecryptConverter(), src => src.ProsvasisUsername))
                .ForMember(dest => dest.ProsvasisPassword, opt => opt.ConvertUsing(new DecryptConverter(), src => src.ProsvasisPassword));

            CreateMap<AccountingOfficeModel, AccountingOffice>()
                .ForMember(dest => dest.LegalStatusType, options => options.Ignore())
                .ForMember(dest => dest.TaxisNetUserName, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisNetUserName))
                .ForMember(dest => dest.TaxisNetPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxisNetPassword))
                .ForMember(dest => dest.AadeRegistryUsername, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AadeRegistryUsername))
                .ForMember(dest => dest.AadeRegistryPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.AadeRegistryPassword))
                .ForMember(dest => dest.OfficeUsername, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OfficeUsername))
                .ForMember(dest => dest.OfficePassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.OfficePassword))
                .ForMember(dest => dest.SrfUsername, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SrfUsername))
                .ForMember(dest => dest.SrfPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.SrfPassword))
                .ForMember(dest => dest.TaxSystemUsername, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxSystemUsername))
                .ForMember(dest => dest.TaxSystemPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.TaxSystemPassword))
                .ForMember(dest => dest.HyperPayrollUsername, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HyperPayrollUsername))
                .ForMember(dest => dest.HyperPayrollPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HyperPayrollPassword))
                .ForMember(dest => dest.HyperLogUsername, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HyperLogUsername))
                .ForMember(dest => dest.HyperLogPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.HyperLogPassword))
                .ForMember(dest => dest.ProsvasisUsername, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ProsvasisUsername))
                .ForMember(dest => dest.ProsvasisPassword, opt => opt.ConvertUsing(new EncryptConverter(), src => src.ProsvasisPassword));

            CreateMap<PeriodicityItem, PeriodicityItemModel>()
                .ForMember(model => model.PeriodicityItemTypeName, options => options.Ignore());
            CreateMap<PeriodicityItemModel, PeriodicityItem>()
                .ForMember(entity => entity.PeriodicityItemType, options => options.Ignore());

            CreateMap<Chamber, ChamberModel>();
            CreateMap<ChamberModel, Chamber>();

            CreateMap<TaxFactor, TaxFactorModel>();
            CreateMap<TaxFactorModel, TaxFactor>();
        }

        protected virtual void CreateSimpleTaskMaps()
        {
            CreateMap<SimpleTaskCategory, SimpleTaskCategoryModel>();
            CreateMap<SimpleTaskCategoryModel, SimpleTaskCategory>();

            CreateMap<SimpleTaskDepartment, SimpleTaskDepartmentModel>();
            CreateMap<SimpleTaskDepartmentModel, SimpleTaskDepartment>();

            CreateMap<SimpleTaskNature, SimpleTaskNatureModel>();
            CreateMap<SimpleTaskNatureModel, SimpleTaskNature>();

            CreateMap<SimpleTaskSector, SimpleTaskSectorModel>();
            CreateMap<SimpleTaskSectorModel, SimpleTaskSector>();

            CreateMap<SimpleTaskManager, SimpleTaskManagerModel>()
                .ForMember(model => model.AssignorName, options => options.Ignore())
                .ForMember(model => model.SimpleTaskPriorityTypeBackground, options => options.Ignore())
                .ForMember(model => model.SimpleTaskTypeBackground, options => options.Ignore())
                .ForMember(model => model.EmployeeName, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.SimpleTaskPriorityTypeName, options => options.Ignore())
                .ForMember(model => model.SimpleTaskTypeName, options => options.Ignore())
                .ForMember(model => model.SimpleTaskCategoryName, options => options.Ignore())
                .ForMember(model => model.SimpleTaskDepartmentName, options => options.Ignore())
                .ForMember(model => model.SimpleTaskNatureName, options => options.Ignore())
                .ForMember(model => model.SimpleTaskSectorName, options => options.Ignore());
            CreateMap<SimpleTaskManagerModel, SimpleTaskManager>();
        }

        protected virtual void CreateAssignmentMaps()
        {
            CreateMap<AssignmentReason, AssignmentReasonModel>();
            CreateMap<AssignmentReasonModel, AssignmentReason>();

            CreateMap<AssignmentPrototype, AssignmentPrototypeModel>();
            CreateMap<AssignmentPrototypeModel, AssignmentPrototype>();

            CreateMap<AssignmentPrototypeAction, AssignmentPrototypeActionModel>()
                .ForMember(model => model.DepartmentName, options => options.Ignore())
                .ForMember(model => model.AssignmentReasonName, options => options.Ignore());
            CreateMap<AssignmentPrototypeActionModel, AssignmentPrototypeAction>();

            CreateMap<AssignmentTask, AssignmentTaskModel>()
                .ForMember(model => model.AssignmentPrototypeId, options => options.Ignore())
                .ForMember(model => model.AssignmentReasonName, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.AssignmentTaskStatus, options => options.Ignore())
                .ForMember(model => model.AssignorName, options => options.Ignore());
            CreateMap<AssignmentTaskModel, AssignmentTask>();

            CreateMap<AssignmentTaskAction, AssignmentTaskActionModel>()
                .ForMember(model => model.AssignmentPrototypeActionId, options => options.Ignore())
                .ForMember(model => model.AssignmentActionStatusTypeName, options => options.Ignore())
                .ForMember(model => model.AssignmentActionPriorityTypeName, options => options.Ignore())
                .ForMember(model => model.EmployeeName, options => options.Ignore())
                .ForMember(model => model.DepartmentName, options => options.Ignore())
                .ForMember(model => model.LetterName, options => options.Ignore())
                .ForMember(model => model.Color, options => options.Ignore())
                .ForMember(model => model.Background, options => options.Ignore())
                .ForMember(model => model.AssignmentTaskName, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.AssignorName, options => options.Ignore())
                .ForMember(model => model.AssignorId, options => options.Ignore())
                .ForMember(model => model.TraderId, options => options.Ignore());
            CreateMap<AssignmentTaskActionModel, AssignmentTaskAction>();
        }

        // spa
        protected virtual void CreateCustomersMaps()
        {
            CreateMap<CustomerRole, CustomerRoleModel>();
            CreateMap<CustomerRoleModel, CustomerRole>()
                .ForMember(entity => entity.FreeShipping, options => options.Ignore())
                .ForMember(entity => entity.TaxExempt, options => options.Ignore())
                .ForMember(entity => entity.OverrideTaxDisplayType, options => options.Ignore())
                .ForMember(entity => entity.DefaultTaxDisplayTypeId, options => options.Ignore())
                .ForMember(entity => entity.PurchasedWithProductId, options => options.Ignore());

            //CreateMap<CustomerSettings, CustomerSettingsModel>();
            //CreateMap<CustomerSettingsModel, CustomerSettings>()
            //    .ForMember(settings => settings.AvatarMaximumSizeBytes, options => options.Ignore())
            //    .ForMember(settings => settings.DeleteGuestTaskOlderThanMinutes, options => options.Ignore())
            //    .ForMember(settings => settings.DownloadableProductsValidateUser, options => options.Ignore())
            //    .ForMember(settings => settings.HashedPasswordFormat, options => options.Ignore())
            //    .ForMember(settings => settings.OnlineCustomerMinutes, options => options.Ignore())
            //    .ForMember(settings => settings.SuffixDeletedCustomers, options => options.Ignore())
            //    .ForMember(settings => settings.LastActivityMinutes, options => options.Ignore());

            CreateMap<ActivityLog, CustomerActivityLogModel>()
               .ForMember(model => model.CreatedOn, options => options.Ignore())
               .ForMember(model => model.ActivityLogTypeName, options => options.Ignore())
               .ForMember(model => model.CustomerEmail, options => options.Ignore());

            //CreateMap<Customer, CustomerModel>()
            //    .ForMember(model => model.Email, options => options.Ignore())
            //    .ForMember(model => model.FullName, options => options.Ignore())
            //    .ForMember(model => model.Company, options => options.Ignore())
            //    .ForMember(model => model.Phone, options => options.Ignore())
            //    .ForMember(model => model.ZipPostalCode, options => options.Ignore())
            //    .ForMember(model => model.CreatedOn, options => options.Ignore())
            //    .ForMember(model => model.LastActivityDate, options => options.Ignore())
            //    .ForMember(model => model.CustomerRoleNames, options => options.Ignore())
            //    .ForMember(model => model.AvatarUrl, options => options.Ignore())
            //    .ForMember(model => model.Password, options => options.Ignore())
            //    .ForMember(model => model.Gender, options => options.Ignore())
            //    .ForMember(model => model.FirstName, options => options.Ignore())
            //    .ForMember(model => model.LastName, options => options.Ignore())
            //    .ForMember(model => model.DateOfBirth, options => options.Ignore())
            //    .ForMember(model => model.StreetAddress, options => options.Ignore())
            //    .ForMember(model => model.StreetAddress2, options => options.Ignore())
            //    .ForMember(model => model.City, options => options.Ignore())
            //    .ForMember(model => model.County, options => options.Ignore())
            //    .ForMember(model => model.CountryId, options => options.Ignore())
            //    .ForMember(model => model.AvailableCountries, options => options.Ignore())
            //    .ForMember(model => model.RegisteredInStore, options => options.Ignore())
            //    .ForMember(model => model.DisplayRegisteredInStore, options => options.Ignore())
            //    .ForMember(model => model.TimeZoneId, options => options.Ignore())
            //    .ForMember(model => model.AllowCustomersToSetTimeZone, options => options.Ignore())
            //    .ForMember(model => model.AvailableTimeZones, options => options.Ignore())
            //    .ForMember(model => model.VatNumber, options => options.Ignore())
            //    .ForMember(model => model.LastVisitedPage, options => options.Ignore())
            //    .ForMember(model => model.AvailableNewsletterSubscriptionStores, options => options.Ignore())
            //    .ForMember(model => model.SelectedNewsletterSubscriptionStoreIds, options => options.Ignore())
            //    .ForMember(model => model.SendEmail, options => options.Ignore())
            //    .ForMember(model => model.SendPm, options => options.Ignore())
            //    .ForMember(model => model.AllowSendingOfPrivateMessage, options => options.Ignore())
            //    .ForMember(model => model.AllowSendingOfWelcomeMessage, options => options.Ignore())
            //    .ForMember(model => model.AllowReSendingOfActivationMessage, options => options.Ignore())
            //    .ForMember(model => model.CustomerActivityLogSearchModel, options => options.Ignore());

            CreateMap<Customer, CustomerModel>()
                .ForMember(model => model.Email, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.CustomerRoleNames, options => options.Ignore())
                .ForMember(model => model.AvatarUrl, options => options.Ignore())
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.SelectedCustomerRoleIds, options => options.Ignore())
                .ForMember(model => model.CountryId, options => options.Ignore())
                //.ForMember(model => model.AvailableCountries, options => options.Ignore())
                .ForMember(model => model.RegisteredInStore, options => options.Ignore())
                //.ForMember(model => model.TimeZoneId, options => options.Ignore())
                //.ForMember(model => model.AvailableTimeZones, options => options.Ignore())
                //.ForMember(model => model.AvailableNewsletterSubscriptionStores, options => options.Ignore())
                .ForMember(model => model.SelectedNewsletterSubscriptionStoreIds, options => options.Ignore())
                .ForMember(model => model.SendEmail, options => options.Ignore())
                .ForMember(model => model.SendPm, options => options.Ignore())
                .ForMember(model => model.AllowSendingOfPrivateMessage, options => options.Ignore())
                .ForMember(model => model.AllowSendingOfWelcomeMessage, options => options.Ignore())
                .ForMember(model => model.AllowReSendingOfActivationMessage, options => options.Ignore())
                .ForMember(model => model.SystemName, options => options.Ignore())
                .ForMember(model => model.EmployeeName, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore());

            CreateMap<CustomerModel, Customer>()
                .ForMember(entity => entity.CustomerGuid, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LastActivityDateUtc, options => options.Ignore())
                .ForMember(entity => entity.EmailToRevalidate, options => options.Ignore())
                .ForMember(entity => entity.RequireReLogin, options => options.Ignore())
                .ForMember(entity => entity.FailedLoginAttempts, options => options.Ignore())
                .ForMember(entity => entity.CannotLoginUntilDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.IsSystemAccount, options => options.Ignore())
                .ForMember(entity => entity.SystemName, options => options.Ignore())
                .ForMember(entity => entity.CurrencyId, options => options.Ignore())
                .ForMember(entity => entity.LanguageId, options => options.Ignore())
                .ForMember(entity => entity.RegisteredInStoreId, options => options.Ignore())
                .ForMember(entity => entity.Gender, options => options.Ignore())
                .ForMember(entity => entity.DateOfBirth, options => options.Ignore())
                .ForMember(entity => entity.Company, options => options.Ignore())
                .ForMember(entity => entity.StreetAddress, options => options.Ignore())
                .ForMember(entity => entity.StreetAddress2, options => options.Ignore())
                .ForMember(entity => entity.ZipPostalCode, options => options.Ignore())
                .ForMember(entity => entity.City, options => options.Ignore())
                .ForMember(entity => entity.County, options => options.Ignore())
                .ForMember(entity => entity.Phone, options => options.Ignore())
                .ForMember(entity => entity.VatNumber, options => options.Ignore())
                .ForMember(entity => entity.LastLoginDateUtc, options => options.Ignore())
                .ForMember(entity => entity.VendorId, options => options.Ignore())
                .ForMember(entity => entity.AvatarPictureId, options => options.Ignore())
                .ForMember(entity => entity.LastVisitedPage, options => options.Ignore())
                .ForMember(entity => entity.AccountActivationToken, options => options.Ignore())
                .ForMember(entity => entity.EmailRevalidationToken, options => options.Ignore())
                .ForMember(entity => entity.PasswordRecoveryToken, options => options.Ignore())
                .ForMember(entity => entity.PasswordRecoveryTokenDateGenerated, options => options.Ignore())
                .ForMember(entity => entity.LanguageAutomaticallyDetected, options => options.Ignore())
                .ForMember(entity => entity.PageSize, options => options.Ignore())
                .ForMember(entity => entity.PageSizeOptions, options => options.Ignore());

            CreateMap<CustomerPermission, CustomerPermissionModel>()
                .ForMember(model => model.AvailableControllerNames, options => options.Ignore())
                .ForMember(model => model.Used, options => options.Ignore());
            CreateMap<CustomerPermissionModel, CustomerPermission>();

            CreateMap<CustomerOnline, CustomerOnlineModel>()
                .ForMember(model => model.LastLoginDate, options => options.Ignore())
                .ForMember(model => model.CompanyName, options => options.Ignore());
            CreateMap<CustomerOnlineModel, CustomerOnline>();

        }

        protected virtual void CreateDirectoryMaps()
        {
            CreateMap<Bookmark, BookmarkModel>();
            CreateMap<BookmarkModel, Bookmark>();

            CreateMap<Country, CountryModel>()
                .ForMember(model => model.SelectedStoreIds, options => options.Ignore())
                .ForMember(model => model.AvailableStores, options => options.Ignore());
            CreateMap<CountryModel, Country>();

            CreateMap<Currency, CurrencyModel>()
                .ForMember(model => model.SelectedStoreIds, options => options.Ignore())
                .ForMember(model => model.AvailableStores, options => options.Ignore())
                .ForMember(model => model.RoundingTypeId, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.IsPrimaryExchangeRateCurrency, options => options.Ignore())
                .ForMember(model => model.IsPrimaryStoreCurrency, options => options.Ignore());
            CreateMap<CurrencyModel, Currency>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());
        }

        protected virtual void CreateMessagesMaps()
        {
            CreateMap<EmailAccount, EmailAccountModel>()
                .ForMember(model => model.InfoMessage, options => options.Ignore())
                .ForMember(model => model.IsDefaultEmailAccount, options => options.Ignore())
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.SendTestEmailTo, options => options.Ignore());
            CreateMap<EmailAccountModel, EmailAccount>()
                .ForMember(entity => entity.Password, options => options.Ignore());

            CreateMap<EmailMessage, EmailMessageModel>()
                .ForMember(model => model.IsSent, options => options.Ignore())
                .ForMember(model => model.EmailMessageTypeName, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.CustomerTypeId, options => options.Ignore())
                .ForMember(model => model.CustomerTypeName, options => options.Ignore())
                .ForMember(model => model.LegalFormTypeId, options => options.Ignore())
                .ForMember(model => model.LegalFormTypeName, options => options.Ignore())
                .ForMember(model => model.CategoryBookTypeId, options => options.Ignore())
                .ForMember(model => model.CategoryBookTypeName, options => options.Ignore())
                .ForMember(model => model.PeriodName, options => options.Ignore())
                .ForMember(model => model.SenderName, options => options.Ignore())
                .ForMember(model => model.CustomerName, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore());
            CreateMap<EmailMessageModel, EmailMessage>();

            CreateMap<QueuedEmail, QueuedEmailModel>()
                //.ForMember(model => model.SendImmediately, options => options.Ignore())
                //.ForMember(model => model.DontSendBeforeDate, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.EmailAccountName, options => options.Ignore())
                .ForMember(model => model.PriorityName, options => options.Ignore())
                .ForMember(model => model.SentOn, options => options.Ignore());
            CreateMap<QueuedEmailModel, QueuedEmail>();
                //.ForMember(entity => entity.AttachmentFileName, options => options.Ignore())
                //.ForMember(entity => entity.AttachmentFilePath, options => options.Ignore())
                //.ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                //.ForMember(entity => entity.DontSendBeforeDateUtc, options => options.Ignore())
                //.ForMember(entity => entity.EmailAccountId, options => options.Ignore())
                //.ForMember(entity => entity.Priority, options => options.Ignore())
                //.ForMember(entity => entity.PriorityId, options => options.Ignore())
                //.ForMember(entity => entity.SentOnUtc, options => options.Ignore());
        }

        protected virtual void CreateMyDataItemsMaps()
        {
            CreateMap<MyDataItem, MyDataItemModel>()
                .ForMember(model => model.LastDateOn, options => options.Ignore())
                .ForMember(model => model.TraderName, options => options.Ignore())
                .ForMember(model => model.CounterpartName, options => options.Ignore())
                .ForMember(model => model.InvoiceTypeName, options => options.Ignore())
                .ForMember(model => model.PaymentMethodName, options => options.Ignore())
                .ForMember(model => model.VatProvisionName, options => options.Ignore())
                .ForMember(model => model.SeriesName, options => options.Ignore())
                .ForMember(model => model.DocTypeName, options => options.Ignore())
                .ForMember(model => model.IsIssuerName, options => options.Ignore())
                .ForMember(model => model.VatCategoryName, options => options.Ignore())
                .ForMember(model => model.ProductCodeName, options => options.Ignore())
                .ForMember(model => model.TaxCategoryName, options => options.Ignore())
                .ForMember(model => model.VatName, options => options.Ignore())
                .ForMember(model => model.CurrencyName, options => options.Ignore());
            CreateMap<MyDataItemModel, MyDataItem>();
        }

        protected virtual void CreateTraderEncryptedMaps()
        {
            CreateMap<MyDataItem, MyDataItemModel>();
            CreateMap<MyDataItemModel, MyDataItem>();
        }

        public int Order => 100;
    }
}