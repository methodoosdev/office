using App.Core.Infrastructure.Mapper;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Logging;
using App.Services.Localization;
using App.Web.Infra.Queries.Common;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Logging
{
    public partial interface ITraderActivityLogModelFactory
    {
        Task<TraderActivityLogSearchModel> PrepareTraderActivityLogSearchModelAsync(TraderActivityLogSearchModel searchModel);
        Task<TraderActivityLogSearchFormModel> PrepareTraderActivityLogSearchFormModelAsync(TraderActivityLogSearchFormModel searchFormModel);
        Task<IList<TraderActivityLogModel>> PrepareTraderActivityLogModelListAsync(string connection, DateTime from, DateTime to);
        Task<TraderActivityLogTableModel> PrepareTraderActivityLogTableModelAsync(TraderActivityLogTableModel tableModel);
    }
    public partial class TraderActivityLogModelFactory : ITraderActivityLogModelFactory
    {
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;

        public TraderActivityLogModelFactory(
            IAppDataProvider dataProvider,
            ILocalizationService localizationService)
        {
            _dataProvider = dataProvider;
            _localizationService = localizationService;
        }

        public virtual Task<TraderActivityLogSearchModel> PrepareTraderActivityLogSearchModelAsync(TraderActivityLogSearchModel searchModel)
        {
            var periodos = DateTime.UtcNow;
            var date = new DateTime(periodos.Year, periodos.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            searchModel.From = date;
            searchModel.To = date.AddMonths(1).AddDays(-1);

            return Task.FromResult(searchModel);
        }

        public virtual Task<TraderActivityLogSearchFormModel> PrepareTraderActivityLogSearchFormModelAsync(TraderActivityLogSearchFormModel formModel)
        {
            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderActivityLogSearchModel>(nameof(TraderActivityLogSearchModel.From), FieldType.Date)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderActivityLogSearchModel>(nameof(TraderActivityLogSearchModel.To), FieldType.Date)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-3", "col-12 md:col-3" }, left, right);

            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return Task.FromResult(formModel);
        }

        public virtual async Task<IList<TraderActivityLogModel>> PrepareTraderActivityLogModelListAsync(string connection, DateTime from, DateTime to)
        {
            var pFrom = new LinqToDB.Data.DataParameter("pFrom", from.Date);
            var pTo = new LinqToDB.Data.DataParameter("pTo", to.Date);

            var list = await _dataProvider.QueryAsync<TraderActivityLogModel>(connection, OfficeDBQuery.TraderActivityLog, pFrom, pTo);

            foreach (var item in list)
            {
                var name = item.TraderName.Split(' ');
                item.TraderName = $"{AesEncryption.Decrypt(name[0])} {AesEncryption.Decrypt(name[1])}";
            }

            return list;
        }

        public virtual async Task<TraderActivityLogTableModel> PrepareTraderActivityLogTableModelAsync(TraderActivityLogTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderActivityLogModel>(1, nameof(TraderActivityLogModel.UserName)),
                ColumnConfig.Create<TraderActivityLogModel>(1, nameof(TraderActivityLogModel.NickName)),
                ColumnConfig.Create<TraderActivityLogModel>(1, nameof(TraderActivityLogModel.TraderName), hidden: true),
                ColumnConfig.Create<TraderActivityLogModel>(1, nameof(TraderActivityLogModel.ActivityLogType), hidden: true),
                ColumnConfig.Create<TraderActivityLogModel>(1, nameof(TraderActivityLogModel.ActivityCount), ColumnType.Numeric, style: textAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderActivityLogModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}

