using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Logging;
using App.Services.Localization;
using App.Web.Infra.Queries.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Logging
{
    public partial interface IEmployeeActivityLogModelFactory
    {
        Task<EmployeeActivityLogSearchModel> PrepareEmployeeActivityLogSearchModelAsync(EmployeeActivityLogSearchModel searchModel);
        Task<EmployeeActivityLogSearchFormModel> PrepareEmployeeActivityLogSearchFormModelAsync(EmployeeActivityLogSearchFormModel searchFormModel);
        Task<IList<EmployeeActivityLogModel>> PrepareEmployeeActivityLogModelListAsync(string connection, DateTime from, DateTime to);
        Task<EmployeeActivityLogTableModel> PrepareEmployeeActivityLogTableModelAsync(EmployeeActivityLogTableModel tableModel);
    }
    public partial class EmployeeActivityLogModelFactory : IEmployeeActivityLogModelFactory
    {
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;

        public EmployeeActivityLogModelFactory(
            IAppDataProvider dataProvider,
            ILocalizationService localizationService)
        {
            _dataProvider = dataProvider;
            _localizationService = localizationService;
        }

        public virtual Task<EmployeeActivityLogSearchModel> PrepareEmployeeActivityLogSearchModelAsync(EmployeeActivityLogSearchModel searchModel)
        {
            var periodos = DateTime.UtcNow;
            var date = new DateTime(periodos.Year, periodos.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            searchModel.From = date;
            searchModel.To = date.AddMonths(1).AddDays(-1);

            return Task.FromResult(searchModel);
        }

        public virtual Task<EmployeeActivityLogSearchFormModel> PrepareEmployeeActivityLogSearchFormModelAsync(EmployeeActivityLogSearchFormModel formModel)
        {
            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeActivityLogSearchModel>(nameof(EmployeeActivityLogSearchModel.From), FieldType.Date)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeActivityLogSearchModel>(nameof(EmployeeActivityLogSearchModel.To), FieldType.Date)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-3", "col-12 md:col-3" }, left, right);

            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return Task.FromResult(formModel);
        }

        public virtual async Task<IList<EmployeeActivityLogModel>> PrepareEmployeeActivityLogModelListAsync(string connection, DateTime from, DateTime to)
        {
            var pFrom = new LinqToDB.Data.DataParameter("pFrom", from.Date);
            var pTo = new LinqToDB.Data.DataParameter("pTo", to.Date);

            var results = await _dataProvider.QueryAsync<EmployeeActivityLogModel>(connection, OfficeDBQuery.EmployeeActivityLog, pFrom, pTo);

            return results;
        }

        public virtual async Task<EmployeeActivityLogTableModel> PrepareEmployeeActivityLogTableModelAsync(EmployeeActivityLogTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<EmployeeActivityLogModel>(1, nameof(EmployeeActivityLogModel.UserName)),
                ColumnConfig.Create<EmployeeActivityLogModel>(1, nameof(EmployeeActivityLogModel.NickName)),
                ColumnConfig.Create<EmployeeActivityLogModel>(1, nameof(EmployeeActivityLogModel.EmployeeName), hidden: true),
                ColumnConfig.Create<EmployeeActivityLogModel>(1, nameof(EmployeeActivityLogModel.ActivityLogType), hidden: true),
                ColumnConfig.Create<EmployeeActivityLogModel>(1, nameof(EmployeeActivityLogModel.ActivityCount), ColumnType.Numeric, style: textAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.EmployeeActivityLogModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}

