using App.Core;
using App.Core.Domain.Payroll;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Payroll;
using App.Models.Traders;
using App.Services;
using App.Services.Common;
using App.Services.Localization;
using App.Services.Payroll;
using App.Services.Traders;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerLeave
{
    public partial interface IWorkerLeaveDetailModelFactory
    {
        Task<WorkerLeaveDetailSearchModel> PrepareWorkerLeaveDetailSearchModelAsync(WorkerLeaveDetailSearchModel searchModel);
        Task<WorkerLeaveDetailListModel> PrepareWorkerLeaveDetailListModelAsync(WorkerLeaveDetailSearchModel searchModel);
        Task<WorkerLeaveDetailModel> PrepareWorkerLeaveDetailModelAsync(WorkerLeaveDetailModel model, WorkerLeaveDetail workerLeaveDetail);
        Task<WorkerLeaveDetailFormModel> PrepareWorkerLeaveDetailFormModelAsync(WorkerLeaveDetailFormModel formModel, int traderId);
        Task<IList<SelectionItemList>> GetWorkersByHyperPayrollIdAsync(int traderId);
    }
    public partial class WorkerLeaveDetailModelFactory : IWorkerLeaveDetailModelFactory
    {
        private readonly IWorkerLeaveDetailService _workerLeaveDetailService;
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ISqlConnectionService _connectionService;
        private readonly IWorkContext _workContext;

        public WorkerLeaveDetailModelFactory(
            IWorkerLeaveDetailService workerLeaveDetailService,
            ITraderService traderService,
            ILocalizationService localizationService,
            IAppDataProvider dataProvider,
            IModelFactoryService modelFactoryService,
            ISqlConnectionService connectionService,
            IWorkContext workContext)
        {
            _workerLeaveDetailService = workerLeaveDetailService;
            _traderService = traderService;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _modelFactoryService = modelFactoryService;
            _connectionService = connectionService;
            _workContext = workContext;
        }
        private async Task<IPagedList<WorkerLeaveDetailModel>> GetPagedListAsync(WorkerLeaveDetailSearchModel searchModel)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync(false);

            var query = _workerLeaveDetailService.Table.AsEnumerable()
                .Select(x =>
                {
                    var model = x.ToModel<WorkerLeaveDetailModel>();
                    model.TraderName = traders.FirstOrDefault(a => a.Value == x.TraderId)?.Label ?? "";
                    model.WorkerName = model.WorkerName ?? "";

                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.WorkerName.ContainsIgnoreCase(searchModel.QuickSearch)
                    );
            }

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader != null)
                query = query.Where(c => c.TraderId == trader.Id);

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<WorkerLeaveDetailSearchModel> PrepareWorkerLeaveDetailSearchModelAsync(WorkerLeaveDetailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize(); searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.WorkerLeaveDetailModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<WorkerLeaveDetailListModel> PrepareWorkerLeaveDetailListModelAsync(WorkerLeaveDetailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var workerLeaveDetails = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new WorkerLeaveDetailListModel().PrepareToGrid(searchModel, workerLeaveDetails);

            return model;
        }

        public virtual Task<WorkerLeaveDetailModel> PrepareWorkerLeaveDetailModelAsync(WorkerLeaveDetailModel model, WorkerLeaveDetail workerLeaveDetail)
        {
            if (workerLeaveDetail != null)
            {
                //fill in model values from the entity
                model ??= workerLeaveDetail.ToModel<WorkerLeaveDetailModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerLeaveDetailModel>(1, nameof(WorkerLeaveDetailModel.TraderName), ColumnType.RouterLink),
                ColumnConfig.Create<WorkerLeaveDetailModel>(2, nameof(WorkerLeaveDetailModel.WorkerName)),
                ColumnConfig.Create<WorkerLeaveDetailModel>(3, nameof(WorkerLeaveDetailModel.DaysLeft), style: rightAlign),
                ColumnConfig.Create<WorkerLeaveDetailModel>(4, nameof(WorkerLeaveDetailModel.Year), style: centerAlign)
            };

            return columns;
        }

        public virtual async Task<WorkerLeaveDetailFormModel> PrepareWorkerLeaveDetailFormModelAsync(WorkerLeaveDetailFormModel formModel, int traderId)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync(FieldConfigType.Payroll);
            var workers = traderId > 0 ? await GetWorkersByHyperPayrollIdAsync(traderId) : new List<SelectionItemList>();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerLeaveDetailModel>(nameof(WorkerLeaveDetailModel.TraderId), FieldType.GridSelect, options: traders),
                FieldConfig.Create<WorkerLeaveDetailModel>(nameof(WorkerLeaveDetailModel.WorkerId), FieldType.GridSelect, options: workers, disableExpression: "(model.traderId < 1) || (model.traderId == null)"),
                FieldConfig.Create<WorkerLeaveDetailModel>(nameof(WorkerLeaveDetailModel.DaysLeft), FieldType.Numeric),
                FieldConfig.Create<WorkerLeaveDetailModel>(nameof(WorkerLeaveDetailModel.Year), FieldType.Numeric),
                //FieldConfig.Create<WorkerLeaveDetailModel>(nameof(WorkerLeaveDetailModel.WorkerName), FieldType.Text, disableExpression: "true")

            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.WorkerLeaveDetailModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }

        public virtual async Task<IList<SelectionItemList>> GetWorkersByHyperPayrollIdAsync(int traderId)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                throw new Exception(result.Error);

            var trader = await _traderService.GetTraderByIdAsync(traderId);
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", trader.HyperPayrollId);
            var workers = await _dataProvider.QueryAsync<WorkerPoco>(result.Connection, WorkerLeaveQuery.Workers, pCompanyId);
            var list = workers.Select(x => new SelectionItemList { Value = x.WorkerId, Label = x.WorkerName }).ToList();

            return list;
        }
    }
}