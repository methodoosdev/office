using App.Core;
using App.Core.Caching;
using App.Core.Configuration;
using App.Core.Events;
using App.Core.Infrastructure;
using App.Data;
using App.Data.DataProviders;
using App.Services;
using App.Services.Accounting;
using App.Services.Assignment;
using App.Services.Authentication;
using App.Services.Banking;
using App.Services.Blogs;
using App.Services.Caching;
using App.Services.Common;
using App.Services.Configuration;
using App.Services.Customers;
using App.Services.Directory;
using App.Services.Employees;
using App.Services.Events;
using App.Services.ExportImport;
using App.Services.Financial;
using App.Services.Forums;
using App.Services.Gdpr;
using App.Services.Helpers;
using App.Services.Html;
using App.Services.Installation;
using App.Services.Jwt;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Media;
using App.Services.Media.RoxyFileman;
using App.Services.Messages;
using App.Services.News;
using App.Services.Offices;
using App.Services.Payroll;
using App.Services.Polls;
using App.Services.ScheduleTasks;
using App.Services.Scripts;
using App.Services.Security;
using App.Services.Seo;
using App.Services.SimpleTask;
using App.Services.Stores;
using App.Services.Traders;
using App.Services.VatExemption;
using App.Web.Framework.Menu;
using App.Web.Framework.Mvc.Routing;
using App.Web.Framework.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace App.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents the registering services on application startup
    /// </summary>
    public partial class NopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //file provider
            services.AddScoped<INopFileProvider, NopFileProvider>();

            //web helper
            services.AddScoped<IWebHelper, WebHelper>();

            //user agent helper
            services.AddScoped<IUserAgentHelper, UserAgentHelper>();

            //static cache manager
            var appSettings = Singleton<AppSettings>.Instance;
            var distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();
            if (distributedCacheConfig.Enabled)
            {
                switch (distributedCacheConfig.DistributedCacheType)
                {
                    case DistributedCacheType.Memory:
                        services.AddScoped<ILocker, MemoryDistributedCacheManager>();
                        services.AddScoped<IStaticCacheManager, MemoryDistributedCacheManager>();
                        break;
                    case DistributedCacheType.SqlServer:
                        services.AddScoped<ILocker, MsSqlServerCacheManager>();
                        services.AddScoped<IStaticCacheManager, MsSqlServerCacheManager>();
                        break;
                    case DistributedCacheType.Redis:
                        services.AddScoped<ILocker, RedisCacheManager>();
                        services.AddScoped<IStaticCacheManager, RedisCacheManager>();
                        break;
                }
            }
            else
            {
                services.AddSingleton<ILocker, MemoryCacheManager>();
                services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            }

            //work context
            services.AddScoped<IWorkContext, WebWorkContext>();

            //store context
            services.AddScoped<IStoreContext, WebStoreContext>();

            //services

            services.AddScoped<ITokenizer, Tokenizer>();
            services.AddScoped<IDateTimeHelper, DateTimeHelper>();
            services.AddScoped<INumberHelper, NumberHelper>();
            services.AddScoped<INopHtmlHelper, NopHtmlHelper>();
            services.AddSingleton<IRoutePublisher, RoutePublisher>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddScoped<IBBCodeHelper, BBCodeHelper>();
            services.AddScoped<IHtmlFormatter, HtmlFormatter>();
            services.AddScoped<INopUrlHelper, NopUrlHelper>();
            services.AddScoped<IPriceFormatter, PriceFormatter>();

            services.AddScoped<IAppDataProvider, AppDataProvider>();
            services.AddScoped<IPeriodicF2Service, PeriodicF2Service>();
            services.AddScoped<IMyDataItemService, MyDataItemService>();
            services.AddScoped<IAssignmentPrototypeActionService, AssignmentPrototypeActionService>();
            services.AddScoped<IAssignmentPrototypeAssignmentPrototypeActionMappingService, AssignmentPrototypeAssignmentPrototypeActionMappingService>();
            services.AddScoped<IAssignmentPrototypeService, AssignmentPrototypeService>();
            services.AddScoped<IAssignmentReasonService, AssignmentReasonService>();
            services.AddScoped<IAssignmentTaskActionService, AssignmentTaskActionService>();
            services.AddScoped<IAssignmentTaskService, AssignmentTaskService>();
            services.AddScoped<IAntiForgeryCookieService, AntiForgeryCookieService>();
            //services.AddScoped<ICookieAuthenticationService, CookieAuthenticationService>();
            services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IGenericAttributeService, GenericAttributeService>();
            services.AddScoped<IHtmlToPdfService, HtmlToPdfService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<ISearchTermService, SearchTermService>();
            services.AddScoped<ISqlConnectionService, SqlConnectionService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IFieldConfigService, FieldConfigService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<ICustomerOnlineService, CustomerOnlineService>();
            services.AddScoped<ICustomerPermissionCustomerMappingService, CustomerPermissionCustomerMappingService>();
            services.AddScoped<ICustomerPermissionService, CustomerPermissionService>();
            services.AddScoped<ICustomerRegistrationService, CustomerRegistrationService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerTokenService, CustomerTokenService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IGeoLookupService, GeoLookupService>();
            services.AddScoped<IEducationService, EducationService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();
            services.AddScoped<IJobTitleService, JobTitleService>();
            services.AddScoped<IExcelTemplateService, ExcelTemplateService>();
            services.AddScoped<IExportToExcelService, ExportToExcelService>();
            services.AddScoped<IImportFromExcelService, ImportFromExcelService>();
            services.AddScoped<IFinancialObligationService, FinancialObligationService>();
            services.AddScoped<IForumService, ForumService>();
            services.AddScoped<IGdprService, GdprService>();
            services.AddScoped<IInstallStringResourcesService, InstallStringResourcesService>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddScoped<ITokenFactoryService, TokenFactoryService>();
            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<ITokenValidatorService, TokenValidatorService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ILocaleStringResourceService, LocaleStringResourceService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILocalizedEntityService, LocalizedEntityService>();
            services.AddScoped<IActivityLogTypeService, ActivityLogTypeService>();
            services.AddScoped<ICustomerActivityService, CustomerActivityService>();
            services.AddScoped<ILogger, DefaultLogger>();
            services.AddScoped<IDownloadService, DownloadService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IEmailMessageService, EmailMessageService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmtpBuilder, SmtpBuilder>();
            services.AddScoped<IMessageTemplateService, MessageTemplateService>();
            services.AddScoped<IMessageTokenProvider, MessageTokenProvider>();
            services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
            //services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IQueuedEmailService, QueuedEmailService>();
            services.AddScoped<IWorkflowMessageService, WorkflowMessageService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IAccountingOfficeService, AccountingOfficeService>();
            services.AddScoped<IBookmarkService, BookmarkService>();
            services.AddScoped<IChamberService, ChamberService>();
            services.AddScoped<IPeriodicityItemService, PeriodicityItemService>();
            services.AddScoped<IPersistStateService, PersistStateService>();
            services.AddScoped<ITaxFactorService, TaxFactorService>();
            services.AddScoped<IApdTekaService, ApdTekaService>();
            services.AddScoped<IPersonTermExpiryService, PersonTermExpiryService>();
            services.AddScoped<IBoardTermExpiryService, BoardTermExpiryService>();

            services.AddScoped<IWorkerLeaveDetailService, WorkerLeaveDetailService>();
            services.AddScoped<IWorkerScheduleDateService, WorkerScheduleDateService>();
            services.AddScoped<IWorkerScheduleLogService, WorkerScheduleLogService>();
            services.AddScoped<IWorkerScheduleService, WorkerScheduleService>();
            services.AddScoped<IWorkerScheduleShiftService, WorkerScheduleShiftService>();
            services.AddScoped<IWorkerScheduleWorkerService, WorkerScheduleWorkerService>();

            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
            services.AddScoped<IAclService, AclService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUrlRecordService, UrlRecordService>();
            services.AddScoped<ISimpleTaskCategoryService, SimpleTaskCategoryService>();
            services.AddScoped<ISimpleTaskDepartmentService, SimpleTaskDepartmentService>();
            services.AddScoped<ISimpleTaskManagerService, SimpleTaskManagerService>();
            services.AddScoped<ISimpleTaskNatureService, SimpleTaskNatureService>();
            services.AddScoped<ISimpleTaskSectorService, SimpleTaskSectorService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IAccountingWorkService, AccountingWorkService>();
            services.AddScoped<IBusinessRegistryService, BusinessRegistryService>();
            services.AddScoped<ITraderBranchService, TraderBranchService>();
            services.AddScoped<ITraderConnectionService, TraderConnectionService>();
            services.AddScoped<ITraderEmployeeMappingService, TraderEmployeeMappingService>();
            services.AddScoped<ITraderGroupService, TraderGroupService>();
            services.AddScoped<ITraderKadService, TraderKadService>();
            // [Obsolete] last Brache transition-merged
            //services.AddScoped<ITraderParentMappingService, TraderParentMappingService>();
            services.AddScoped<ITraderService, TraderService>();
            services.AddScoped<IWorkingAreaService, WorkingAreaService>();
            services.AddScoped<IModelFactoryService, ModelFactoryService>();
            services.AddScoped<IVatExemptionApprovalService, VatExemptionApprovalService>();
            services.AddScoped<IVatExemptionDocService, VatExemptionDocService>();
            services.AddScoped<IVatExemptionReportService, VatExemptionReportService>();
            services.AddScoped<IVatExemptionSerialService, VatExemptionSerialService>();
            services.AddScoped<ITraderRelationshipService, TraderRelationshipService>();
            services.AddScoped<ITraderMembershipService, TraderMembershipService>();
            services.AddScoped<ITraderBoardMemberTypeService, TraderBoardMemberTypeService>();

            services.AddScoped<ITraderRatingCategoryService, TraderRatingCategoryService>();
            services.AddScoped<ITraderRatingService, TraderRatingService>();
            services.AddScoped<ITraderRatingTraderMappingService, TraderRatingTraderMappingService>();
            services.AddScoped<ITraderInfoService, TraderInfoService>();

            services.AddScoped<ITraderMonthlyBillingService, TraderMonthlyBillingService>();

            services.AddScoped<IScriptFieldService, ScriptFieldService>();
            services.AddScoped<IScriptItemService, ScriptItemService>();
            services.AddScoped<IScriptService, ScriptService>();
            services.AddScoped<IScriptPivotItemService, ScriptPivotItemService>();
            services.AddScoped<IScriptPivotService, ScriptPivotService>();
            services.AddScoped<IScriptTableItemService, ScriptTableItemService>();
            services.AddScoped<IScriptTableNameService, ScriptTableNameService>();
            services.AddScoped<IScriptTableService, ScriptTableService>();
            services.AddScoped<IScriptGroupService, ScriptGroupService>();
            services.AddScoped<IScriptToolService, ScriptToolService>();
            services.AddScoped<IScriptToolItemService, ScriptToolItemService>();
            services.AddScoped<IScriptCloneByTraderService, ScriptCloneByTraderService>();

            services.AddScoped<IBankingTransactionService, BankingTransactionService>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //register all settings
            var typeFinder = Singleton<ITypeFinder>.Instance;

            var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
            foreach (var setting in settings)
            {
                services.AddScoped(setting, serviceProvider =>
                {
                    var storeId = DataSettingsManager.IsDatabaseInstalled()
                        ? serviceProvider.GetRequiredService<IStoreContext>().GetCurrentStore()?.Id ?? 0
                        : 0;

                    return serviceProvider.GetRequiredService<ISettingService>().LoadSettingAsync(setting, storeId).Result;
                });
            }

            //picture service
            if (appSettings.Get<AzureBlobConfig>().Enabled)
                services.AddScoped<IPictureService, AzurePictureService>();
            else
                services.AddScoped<IPictureService, PictureService>();

            //roxy file manager
            services.AddScoped<IRoxyFilemanService, RoxyFilemanService>();
            services.AddScoped<IRoxyFilemanFileProvider, RoxyFilemanFileProvider>();

            //installation service
            services.AddScoped<IInstallationService, InstallationService>();

            //slug route transformer
            if (DataSettingsManager.IsDatabaseInstalled())
                services.AddScoped<SlugRouteTransformer>();

            //schedule tasks
            services.AddSingleton<ITaskScheduler, TaskScheduler>();
            services.AddTransient<IScheduleTaskRunner, ScheduleTaskRunner>();

            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
                foreach (var findInterface in consumer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;
                }, typeof(IConsumer<>)))
                    services.AddScoped(findInterface, consumer);

            //XML sitemap
            services.AddScoped<IXmlSiteMap, XmlSiteMap>();

            //register the Lazy resolver for .Net IoC
            var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;
            if (!useAutofac)
                services.AddScoped(typeof(Lazy<>), typeof(LazyInstance<>));
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 2000;
    }
}