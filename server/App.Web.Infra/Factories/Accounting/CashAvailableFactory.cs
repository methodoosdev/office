using App.Core;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Infra.Queries.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Common.Factories
{
    public partial interface ICashAvailableFactory
    {
        Task<CashAvailableSearchModel> PrepareCashAvailableSearchModelAsync(CashAvailableSearchModel searchModel, List<SelectionItemList> years, List<SelectionItemList> periods);
        Task<IList<CashAvailableModel>> PrepareCashAvailableListAsync(string connection, int companyId, CashAvailableSearchModel searchModel);
        Task<CashAvailableTableModel> PrepareCashAvailableTableModelAsync(CashAvailableTableModel tableModel);
    }
    public class CashAvailableFactory : ICashAvailableFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ILocalizationService _localizationService;

        public CashAvailableFactory(
            IFieldConfigService fieldConfigService,
            IAppDataProvider provider,
            ISoftoneQueryFactory softoneQueryFactory,
            ILocalizationService localizationService)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = provider;
            _softoneQueryFactory = softoneQueryFactory;
            _localizationService = localizationService;
        }

        public virtual async Task<IList<CashAvailableModel>> PrepareCashAvailableListAsync(string connection, int companyId, CashAvailableSearchModel searchModel)
        {
            var fiscalYear = (await _softoneQueryFactory.GetCompanyYearUsesAsync(connection, companyId, searchModel.Year)).First();

            var periods = await _softoneQueryFactory.GetPeriodsPerYearAsync(connection, companyId, fiscalYear.Year);

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", fiscalYear.Year);
            var pFromPeriod = new LinqToDB.Data.DataParameter("pFromPeriod", 1);
            var pToPeriod = new LinqToDB.Data.DataParameter("pToPeriod", searchModel.Period);
            var pSchema = new LinqToDB.Data.DataParameter("pSchema", fiscalYear.Schema);

            var list1 = await _dataProvider.QueryAsync<CashAvailableModel>(connection, CashAvailableQuery.All, pCompanyId, pYear, pSchema, pFromPeriod, pToPeriod);

            var previeusYear = await _softoneQueryFactory.CheckPrevieusYearExistsAsync(connection, companyId, fiscalYear.Year - 1);

            var list2 = new List<CashAvailableModel>();

            if (previeusYear.Exists)
            {
                pYear = new LinqToDB.Data.DataParameter("pYear", fiscalYear.Year - 1);
                pFromPeriod = new LinqToDB.Data.DataParameter("pFromPeriod", 0);
                pToPeriod = new LinqToDB.Data.DataParameter("pToPeriod", 12);
                pSchema = new LinqToDB.Data.DataParameter("pSchema", previeusYear.Schema);

                var merge = await _dataProvider.QueryAsync<CashAvailableModel>(connection, CashAvailableQuery.All, pCompanyId, pYear, pSchema, pFromPeriod, pToPeriod);

                list2.AddRange(merge);
            }
            else
            {
                pFromPeriod = new LinqToDB.Data.DataParameter("pFromPeriod", 0);
                pToPeriod = new LinqToDB.Data.DataParameter("pToPeriod", 0);

                var merge = await _dataProvider.QueryAsync<CashAvailableModel>(connection, CashAvailableQuery.All, pCompanyId, pYear, pSchema, pFromPeriod, pToPeriod);

                list2.AddRange(merge);
            }

            var list =
                (from c in list1.Concat(list2)
                 group c by c.Id into grp
                 select new CashAvailableModel
                 {
                     Id = grp.Key,
                     Description = grp.First().Description,
                     Type = grp.First().Type,
                     Total = grp.Sum(c => c.Total)
                 })
                 .ToList();

            return list;
        }

        public virtual async Task<CashAvailableSearchModel> PrepareCashAvailableSearchModelAsync(CashAvailableSearchModel searchModel, List<SelectionItemList> years, List<SelectionItemList> periods)
        {
            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<CashAvailableSearchModel>(nameof(CashAvailableSearchModel.TraderId)) 
            };

            var right1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<CashAvailableSearchModel>(nameof(CashAvailableSearchModel.Year), FieldType.Select, options: years)
            };

            var right2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<CashAvailableSearchModel>(nameof(CashAvailableSearchModel.Period), FieldType.Select, options: periods)
            };

            var fields = (searchModel.TraderId > 0) 
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-3", "col-12 md:col-3" }, right1, right2) 
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3", "col-12 md:col-3" }, left, right1, right2);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<CashAvailableTableModel> PrepareCashAvailableTableModelAsync(CashAvailableTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<CashAvailableModel>(1, nameof(CashAvailableModel.Id), width: 90, hidden: true),
                ColumnConfig.Create<CashAvailableModel>(2, nameof(CashAvailableModel.Description), width: 260),
                ColumnConfig.Create<CashAvailableModel>(3, nameof(CashAvailableModel.Total), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<CashAvailableModel>(4, nameof(CashAvailableModel.Type), hidden: true),
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.CashAvailableModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}