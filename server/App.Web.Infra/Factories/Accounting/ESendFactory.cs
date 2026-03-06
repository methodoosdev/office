using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Accounting;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Localization;
using App.Web.Infra.Queries.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface IESendFactory
    {
        //Task<IList<ESendModel>> PrepareESendListAsync(string connection, Trader trader, string docName);
        Task<ESendSearchModel> PrepareESendSearchModelAsync(ESendSearchModel searchModel);
        Task<ESendTableModel> PrepareESendTableModelAsync(ESendTableModel tableModel);
        Task<IList<ESendModel>> PrepareESendModelListAsync(IList<ESendDto> values, int companyId, string connection, int logistikiProgramTypeId);
        Task<IList<ESendModel>> PrepareNotESendModelListAsync(IList<ESendDto> values, int logistikiProgramTypeId);
    }
    public class ESendFactory : IESendFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;

        public ESendFactory(
            IFieldConfigService fieldConfigService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
        }

        //public virtual async Task<IList<ESendModel>> PrepareESendListAsync(string connection, Trader trader, string docName)
        //{
        //    var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", trader.CompanyId);
        //    var pDocName = new LinqToDB.Data.DataParameter("pDocName", docName);

        //    var list = await _dataProvider.QueryAsync<ESendModel>(connection, ESendQuery.Docs, pCompanyId, pDocName);

        //    return list;
        //}

        public virtual async Task<ESendSearchModel> PrepareESendSearchModelAsync(ESendSearchModel searchModel)
        {
            searchModel.Period = DateTime.Now.ToUtcRelative();

            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<ESendSearchModel>(nameof(ESendSearchModel.TraderId)) 
            };

            var right1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ESendSearchModel>(nameof(ESendSearchModel.Period), FieldType.MonthDate)
            };

            var right2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ESendSearchModel>(nameof(ESendSearchModel.NotSoftOne), FieldType.Switch)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3", "col-12 md:col-3" }, left, right1, right2);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<ESendTableModel> PrepareESendTableModelAsync(ESendTableModel tableModel)
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ESendModel>(1, nameof(ESendModel.Date)),
                ColumnConfig.Create<ESendModel>(2, nameof(ESendModel.Doc)),
                ColumnConfig.Create<ESendModel>(3, nameof(ESendModel.Payment)),
                ColumnConfig.Create<ESendModel>(4, nameof(ESendModel.Currency)),
                ColumnConfig.Create<ESendModel>(5, nameof(ESendModel.Value), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ESendModel>(6, nameof(ESendModel.Code)),
                ColumnConfig.Create<ESendModel>(7, nameof(ESendModel.DocId)),
                ColumnConfig.Create<ESendModel>(8, nameof(ESendModel.CustomerCode)),
                ColumnConfig.Create<ESendModel>(9, nameof(ESendModel.Number)),
                ColumnConfig.Create<ESendModel>(10, nameof(ESendModel.Index))
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ESendModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        public virtual async Task<IList<ESendModel>> PrepareESendModelListAsync(IList<ESendDto> values, int companyId, string connection, int logistikiProgramTypeId)
        {
            var error = await _localizationService.GetResourceAsync("App.Common.Error");
            var soSource = logistikiProgramTypeId == (int)LogistikiProgramType.SoftOne ? 1351 : 1361;
            var customerCode = "3000000000";
            var list = new List<ESendModel>();
            var index = 1;

            foreach (var model in values)
            {
                var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
                var pSoSource = new LinqToDB.Data.DataParameter("pSoSource", soSource);
                var docSeries = await _dataProvider.QueryAsync<DocSeries>(connection, ESendQuery.Tameiakes, pCompanyId, pSoSource);
                var docSerie = docSeries.Where(x => x.Tameiaki.Trim() == model.Tameiaki.Trim()).FirstOrDefault();

                var items = GetValues(model, soSource == 1351);
                foreach (var item in items)
                {
                    list.Add(new ESendModel
                    {
                        Date = model.Date.ToString("dd/MM/yyyy"),
                        Doc = docSerie == null ? $"{error}: {model.Tameiaki}" : $"{model.Tameiaki} {model.ZhtaNo}",
                        Payment = "Paid",
                        Currency = "EUR",
                        Value = item.Value,
                        Code = item.Code,
                        DocId = docSerie?.Id ?? $"{error}",
						CustomerCode = customerCode,
                        Number = model.ZhtaNo,
                        Index = index.ToString()
                    });
                }
                index++;
            }

            return list;
        }

        public virtual async Task<IList<ESendModel>> PrepareNotESendModelListAsync(IList<ESendDto> values, int logistikiProgramTypeId)
        {
            var error = await _localizationService.GetResourceAsync("App.Common.Error");
            var soSource = logistikiProgramTypeId == (int)LogistikiProgramType.SoftOne ? 1351 : 1361;
            var customerCode = "3000000000";
            var list = new List<ESendModel>();
            var index = 1;

            foreach (var model in values)
            {
                var items = GetValues(model, soSource == 1351);
                foreach (var item in items)
                {
                    list.Add(new ESendModel
                    {
                        Date = model.Date.ToString("dd/MM/yyyy"),
                        Doc = $"{model.Tameiaki} {model.ZhtaNo}",
                        Payment = "Paid",
                        Currency = "EUR",
                        Value = item.Value,
                        Code = item.Code,
                        CustomerCode = customerCode,
                        Number = model.ZhtaNo,
                        Index = index.ToString()
                    });
                }
                index++;
            }

            return list;
        }

        private List<ValueItem> GetValues(ESendDto model, bool softOne)
        {
            var list = new List<ValueItem>();

            if(!model.A6.Equals(0))
                list.Add(new ValueItem { Code = softOne ? "20.06" : "70.01.11.2000.1006", Value = model.A6 });
            if (!model.B13.Equals(0))
                list.Add(new ValueItem { Code = softOne ? "20.13" : "70.01.11.2000.1013", Value = model.B13 });
            if (!model.C24.Equals(0))
                list.Add(new ValueItem { Code = softOne ? "20.24" : "70.01.11.2000.1024", Value = model.C24 });
            if (!model.D36.Equals(0))
                list.Add(new ValueItem { Code = softOne ? "20.36" : "70.01.11.2000.1036", Value = model.D36 });
            if (!model.E0.Equals(0))
                list.Add(new ValueItem { Code = softOne ? "20.00" : "70.01.11.2000.2700", Value = model.E0 });
            if (model.A6.Equals(0) && model.B13.Equals(0) && model.C24.Equals(0) && model.D36.Equals(0) && model.E0.Equals(0))
                list.Add(new ValueItem { Code = softOne ? "20.24" : "70.01.11.2000.1024", Value = 0 });

            return list;
        }

        private class ValueItem
        {
            public string Code { get; set; }
            public decimal Value { get; set; }
        }

        private class DocSeries
        {
            public string Id { get; set; }
            public string Tameiaki { get; set; }
        }

    }
}