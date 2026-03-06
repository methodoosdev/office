using App.Core;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Models.Extensions;
using App.Models.Payroll;
using App.Services.Customers;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Payroll;
using App.Services.Traders;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerSchedules
{
    public partial interface IWorkerSchedulePendingModelFactory
    {
        Task<WorkerSchedulePendingSearchModel> PrepareWorkerSchedulePendingSearchModelAsync(WorkerSchedulePendingSearchModel searchModel);
        Task<WorkerSchedulePendingListModel> PrepareWorkerSchedulePendingListModelAsync(WorkerSchedulePendingSearchModel searchModel, string connection);

    }
    public partial class WorkerSchedulePendingModelFactory : IWorkerSchedulePendingModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly IWorkerScheduleService _workerScheduleService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IWorkContext _workContext;

        public WorkerSchedulePendingModelFactory(ITraderService traderService,
            IWorkerScheduleService workerScheduleService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IAppDataProvider dataProvider,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _workerScheduleService = workerScheduleService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _workContext = workContext;
        }

        public async Task<WorkerSchedulePendingSearchModel> PrepareWorkerSchedulePendingSearchModelAsync(WorkerSchedulePendingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.WorkerSchedulePendingModel.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        private async Task<IPagedList<WorkerSchedulePendingModel>> GetPagedListAsync(WorkerSchedulePendingSearchModel searchModel, string connection)
        {
            var currentDate = DateTime.UtcNow;

            var pCurrentToDate = new LinqToDB.Data.DataParameter("pCurrentToDate", currentDate.ToString("yyyy/MM/dd"));

            var list = await _dataProvider
                .QueryAsync<WorkerSchedulePendingModel>(connection, WorkerScheduleQuery.Pending, pCurrentToDate);

            foreach (var item in list)
            {
                item.Submited = item.PeriodToDate.HasValue ? true : false;
                item.TraderName = item.FullNameDecrypt();
            }

            var query = list.AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<WorkerSchedulePendingListModel> PrepareWorkerSchedulePendingListModelAsync(WorkerSchedulePendingSearchModel searchModel, string connection)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var workerSchedulePendings = await GetPagedListAsync(searchModel, connection);

            //prepare grid model
            var model = new WorkerSchedulePendingListModel().PrepareToGrid(searchModel, workerSchedulePendings);

            return model;
        }

        private Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerSchedulePendingModel>(1, nameof(WorkerSchedulePendingModel.TraderName)),
                ColumnConfig.Create<WorkerSchedulePendingModel>(2, nameof(WorkerSchedulePendingModel.PeriodToDate), ColumnType.Date),
                ColumnConfig.Create<WorkerSchedulePendingModel>(3, nameof(WorkerSchedulePendingModel.Submited), ColumnType.Checkbox)
            };

            return Task.FromResult(columns);
        }

    }
}