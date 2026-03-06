using App.Core;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Models.Extensions;
using App.Models.Payroll;
using App.Services.Localization;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll
{
    public partial interface IWorkerCatalogByTraderModelFactory
    {
        Task<WorkerCatalogSearchModel> PrepareWorkerCatalogSearchModelAsync(WorkerCatalogSearchModel searchModel);
        Task<WorkerCatalogListModel> PrepareWorkerCatalogListModelAsync(WorkerCatalogSearchModel searchModel, string connection, int companyId);
    }
    public partial class WorkerCatalogByTraderModelFactory : IWorkerCatalogByTraderModelFactory
    {
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public WorkerCatalogByTraderModelFactory(
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<WorkerCatalogModel>> GetPagedListAsync(WorkerCatalogSearchModel searchModel, string connection, int companyId)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var query = (await _dataProvider.QueryAsync<WorkerCatalogModel>(connection, PayrollQuery.WorkerCatalog, pCompanyId)).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Surname.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmployeeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Vat.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<WorkerCatalogSearchModel> PrepareWorkerCatalogSearchModelAsync(WorkerCatalogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customer = await _workContext.GetCurrentCustomerAsync();

            //prepare page parameters
            searchModel.SetGridPageSize(); searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.WorkerCatalogModel.Title");
            searchModel.DataKey = "employeeId";

            return searchModel;
        }

        public virtual async Task<WorkerCatalogListModel> PrepareWorkerCatalogListModelAsync(WorkerCatalogSearchModel searchModel, string connection, int companyId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var workerCatalogs = await GetPagedListAsync(searchModel, connection, companyId);

            //prepare grid model
            var model = new WorkerCatalogListModel().PrepareToGrid(searchModel, workerCatalogs);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.UpokCode)),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.Surname)),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.EmployeeName)),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.Vat)),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.HrDate), ColumnType.Date, style : centerAlign),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.FrDate), ColumnType.Date, style : centerAlign),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.FrReason)),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.EmployeeSpcDesc)),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.Orismenou), ColumnType.Date, style : centerAlign),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.OrismenouExpire), ColumnType.Date, style : centerAlign),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.BirthDate), ColumnType.Date, style : centerAlign),
                ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.Amka)),
                //ColumnConfig.Create<WorkerCatalogModel>(5, nameof(WorkerCatalogModel.AmIka), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.Salary), ColumnType.Decimal, hidden: true, style: rightAlign),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.SalaryAgreed), ColumnType.Decimal, hidden: true, style: rightAlign),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.OaedEpidomaDate), ColumnType.Date, hidden: true, style : centerAlign),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.CompanyName), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.EpidotisiErgodPercent), ColumnType.Decimal, hidden: true, style: rightAlign),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.Birthday), hidden: true, style: rightAlign),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.OaedEpidoma), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.CndtDescr), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.SepeOaedDesc), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.SepeOaedCode), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.AmKoinAsf), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.OaedEpidotisi), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.EidikotitaId), hidden: true, style: rightAlign),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.PackageId), hidden: true, style: rightAlign),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.EidikotitaDesc), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.EmployeeType), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.OresErgasias), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.EmployeeSpc), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.EmployeeKind), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.MerikiApasx), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.PliresOrario), hidden: true),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.DayHours), ColumnType.Decimal, hidden: true, style: rightAlign),
                //ColumnConfig.Create<WorkerCatalogModel>(3, nameof(WorkerCatalogModel.EmployeeCode), hidden: true)
            };
            return columns;
        }
    }
}