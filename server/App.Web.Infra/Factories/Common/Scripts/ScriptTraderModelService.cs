using App.Core;
using App.Core.Domain.Scripts;
using App.Models.Accounting;
using App.Models.Scripts;
using App.Services.Localization;
using App.Services.Scripts;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Infra.SqlQueries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Scripts
{
    public partial interface IScriptTraderModelService
    {
        Task<decimal> PrepareScriptQueryAsync(ScriptField field, string connection, int companyId);
        Task<decimal> PrepareScriptFunctionAsync(ScriptField field, string connection, int companyId);
        Task<decimal> PrepareScriptFieldAsync(ScriptField field, string connection, int companyId);
        Task<List<(string, decimal)>> PrepareScriptPivotFieldAsync(
            ScriptField field, string connection, int companyId, int year, int period, ScriptPivotShowType showType, bool inventory);
        Task<List<(string, decimal)>> PrepareScriptPivotQueryAsync(
            ScriptField field, string connection, int companyId, int year, int period, ScriptPivotShowType showType, bool inventory);
        Task<Dictionary<int, (int, decimal)>> CreateScriptItemsDictAsync(
            Script script, int traderId, string connection, int companyId, ScriptToolConfigModel config, bool previousFiscalYear = false);
        decimal CreateScriptsDictItem(Dictionary<int, (int, decimal)> dicts);
    }
    public partial class ScriptTraderModelService : IScriptTraderModelService
    {
        private readonly ITraderService _traderService;
        private readonly IScriptFieldService _scriptFieldService;
        private readonly IScriptService _scriptService;
        private readonly IScriptItemService _scriptItemService;
        private readonly IScriptPivotService _scriptPivotService;
        private readonly IScriptPivotItemService _scriptPivotItemService;
        private readonly IScriptTableItemService _scriptTableItemService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public ScriptTraderModelService(ITraderService traderService,
            IScriptFieldService scriptFieldService,
            IScriptService scriptService,
            IScriptItemService scriptItemService,
            IScriptTableItemService scriptTableItemService,
            IScriptPivotService scriptPivotService,
            IScriptPivotItemService scriptPivotItemService,
            ISoftoneQueryFactory softoneQueryFactory,
            ITraderConnectionService traderConnectionService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _scriptFieldService = scriptFieldService;
            _scriptService = scriptService;
            _scriptItemService = scriptItemService;
            _scriptPivotService = scriptPivotService;
            _scriptPivotItemService = scriptPivotItemService;
            _scriptTableItemService = scriptTableItemService;
            _softoneQueryFactory = softoneQueryFactory;
            _traderConnectionService = traderConnectionService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<decimal> PrepareScriptFunctionPaymentsAsync(ScriptField field, string connection, int companyId)
        {
            var fiscalYear = await GetYearAsync(connection, companyId, (ScriptFiscalYearType)field.FiscalYear);

            var builder = new AggregateAnalysisPeriodsSql();

            var value = builder.GetPayments(connection, companyId, fiscalYear.Year, field.PeriodFrom, field.PeriodTo);

            return value;
        }

        private async Task<decimal> PrepareScriptFunctionReceiptsAsync(ScriptField field, string connection, int companyId)
        {
            var fiscalYear = await GetYearAsync(connection, companyId, (ScriptFiscalYearType)field.FiscalYear);

            var builder = new AggregateAnalysisPeriodsSql();

            var value = builder.GetReceipts(connection, companyId, fiscalYear.Year, field.PeriodFrom, field.PeriodTo);

            return value;
        }

        private async Task<decimal> PrepareScriptFunctionOrdersAsync(ScriptField field, string connection, int companyId)
        {
            var fiscalYear = await GetYearAsync(connection, companyId, (ScriptFiscalYearType)field.FiscalYear);

            var builder = new AggregateAnalysisPeriodsSql();

            var value = builder.GetOrders(connection, companyId, fiscalYear.Year, field.PeriodFrom, field.PeriodTo);

            return value;
        }

        public async Task<decimal> PrepareScriptQueryAsync(ScriptField field, string connection, int companyId)
        {
            if (field.ScriptQueryTypeId == (int)ScriptQueryType.Payments)
            {
                return await PrepareScriptFunctionPaymentsAsync(field, connection, companyId);
            }
            if (field.ScriptQueryTypeId == (int)ScriptQueryType.Receipts)
            {
                return await PrepareScriptFunctionReceiptsAsync(field, connection, companyId);
            }
            if (field.ScriptQueryTypeId == (int)ScriptQueryType.Orders)
            {
                return await PrepareScriptFunctionOrdersAsync(field, connection, companyId);
            }

            return 0m;
        }

        public Task<decimal> PrepareScriptFunctionAsync(ScriptField field, string connection, int companyId)
        {
            if (field.ScriptFunctionTypeId == (int)ScriptFunctionType.EmployeesCount)
            {
                return Task.FromResult(10m);
            }
            if (field.ScriptFunctionTypeId == (int)ScriptFunctionType.TaxesFee)
            {
                return Task.FromResult(600m);
            }

            return Task.FromResult(0m);
        }

        private async Task<CompanyYearUsesModel> GetYearAsync(string connection, int companyId, ScriptFiscalYearType type)
        {
            var fiscalYears = await _softoneQueryFactory.GetCompanyYearUsesAsync(connection, companyId);
            var max = fiscalYears.Max(x => x.Year);
            var min = fiscalYears.Min(x => x.Year);

            return type switch
            {
                ScriptFiscalYearType.Current => fiscalYears.First(x => x.Year == max),
                ScriptFiscalYearType.Previews => fiscalYears.Count > 1
                                                  ? fiscalYears.First(x => x.Year == max - 1)
                                                  : fiscalYears.First(x => x.Year == max),
                //ScriptFiscalYearType.Initial => fiscalYears.First(x => x.Year == min),
                _ => null
            };

        }

        public async Task<decimal> PrepareScriptFieldAsync(ScriptField field, string connection, int companyId)
        {
            var scriptTableItems = await _scriptTableItemService.GetAllScriptTableItemsAsync(field.ScriptTableId);

            var fiscalYear = await GetYearAsync(connection, companyId, (ScriptFiscalYearType)field.FiscalYear);

            var builder = new AggregateAnalysisPeriodsSql();

            var inc = scriptTableItems.Where(x => x.ScriptBehaviorTypeId == (int)ScriptBehaviorType.Included)
                .Select(x => x.AccountingCode).ToArray();

            var exc = scriptTableItems.Where(x => x.ScriptBehaviorTypeId == (int)ScriptBehaviorType.Excluded)
                .Select(x => x.AccountingCode).ToArray();

            if (field.BalanceSheet == true)
            {
                var balance = builder.Get(connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1000, 1000, inc, exc);
                return Math.Abs(balance);
            }

            var value = inc.Count() == 0 ? 0m : builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, field.PeriodFrom, field.PeriodTo, inc, exc);

            if (field.Inventory == true)
                value += builder.Get(connection, companyId, fiscalYear.Schema, fiscalYear.Year, 0, 0, inc, exc);

            return value;
        }

        public async Task<List<(string, decimal)>> PrepareScriptPivotFieldAsync(ScriptField field, string connection, int companyId, int year, int period, ScriptPivotShowType showType, bool inventory)
        {
            var list = new List<(string, decimal)>();

            var scriptTableItems = await _scriptTableItemService.GetAllScriptTableItemsAsync(field.ScriptTableId);

            var fiscalYear = (await _softoneQueryFactory.GetCompanyYearUsesAsync(connection, companyId, year)).First();

            var builder = new AggregateAnalysisPeriodsSql();

            var inc = scriptTableItems.Where(x => x.ScriptBehaviorTypeId == (int)ScriptBehaviorType.Included)
                .Select(x => x.AccountingCode).ToArray();

            var exc = scriptTableItems.Where(x => x.ScriptBehaviorTypeId == (int)ScriptBehaviorType.Excluded)
                .Select(x => x.AccountingCode).ToArray();

            for (var i = 1; i <= period; i++)
            {
                var index = showType == ScriptPivotShowType.Period ? i : 1;
                var periodName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                var value = inc.Count() == 0 ? 0m : builder.Get(
                    connection, companyId, fiscalYear.Schema, fiscalYear.Year, index, i, inc, exc);
                list.Add((periodName, value));
            }

            if (field.Inventory && inventory)
            {
                var previewsFiscalYear = (await _softoneQueryFactory.GetCompanyYearUsesAsync(connection, companyId, year - 1)).FirstOrDefault();
                if (previewsFiscalYear == null) // try to get this year inventory
                {
                    var periodName = "Απογραφή";
                    var value = inc.Count() == 0 ? 0m : builder.Get(
                        connection, companyId, fiscalYear.Schema, fiscalYear.Year, 0, 0, inc, exc);

                    if (showType == ScriptPivotShowType.Transfer)
                        list = list.Select(t => (t.Item1, t.Item2 + value)).ToList();
                    list.Add((periodName, value));
                }
                else // try to get previews year all periods 0-12
                {
                    var periodName = "Απογραφή";
                    var value = inc.Count() == 0 ? 0m : builder.Get(
                        connection, companyId, previewsFiscalYear.Schema, previewsFiscalYear.Year, 0, 12, inc, exc);

                    if (showType == ScriptPivotShowType.Transfer)
                        list = list.Select(t => (t.Item1, t.Item2 + value)).ToList();
                    list.Add((periodName, value));
                }
            }

            return list;
        }

        public async Task<List<(string, decimal)>> PrepareScriptPivotQueryAsync(ScriptField field, string connection, int companyId, int year, int period, ScriptPivotShowType showType, bool inventory)
        {
            var list = new List<(string, decimal)>();

            var fiscalYear = (await _softoneQueryFactory.GetCompanyYearUsesAsync(connection, companyId, year)).First();

            var builder = new AggregateAnalysisPeriodsSql();

            for (var i = 1; i <= period; i++)
            {
                var index = showType == ScriptPivotShowType.Period ? i : 1;
                var periodName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                decimal? value = ((ScriptQueryType)field.ScriptQueryTypeId) switch
                {
                    ScriptQueryType.Payments => builder.GetPayments(connection, companyId, fiscalYear.Year, index, i),
                    ScriptQueryType.Receipts => builder.GetReceipts(connection, companyId, fiscalYear.Year, index, i),
                    ScriptQueryType.Orders => builder.GetOrders(connection, companyId, fiscalYear.Year, index, i),
                    _ => null
                };

                list.Add((periodName, value ?? 0));
            }

            if (field.Inventory && inventory)
            {
                var previewsFiscalYear = (await _softoneQueryFactory.GetCompanyYearUsesAsync(connection, companyId, year - 1)).FirstOrDefault();
                if (previewsFiscalYear == null) // try to get this year inventory
                {
                    var periodName = "Απογραφή";
                    decimal? value = ((ScriptQueryType)field.ScriptQueryTypeId) switch
                    {
                        ScriptQueryType.Payments => builder.GetPayments(connection, companyId, fiscalYear.Year, 0, 0),
                        ScriptQueryType.Receipts => builder.GetReceipts(connection, companyId, fiscalYear.Year, 0, 0),
                        ScriptQueryType.Orders => builder.GetOrders(connection, companyId, fiscalYear.Year, 0, 0),
                        _ => null
                    };

                    if (showType == ScriptPivotShowType.Transfer)
                        list = list.Select(t => (t.Item1, t.Item2 + value ?? 0)).ToList();
                    list.Add((periodName, value ?? 0));
                }
                else // try to get previews year all periods 0-12
                {
                    var periodName = "Απογραφή";
                    decimal? value = ((ScriptQueryType)field.ScriptQueryTypeId) switch
                    {
                        ScriptQueryType.Payments => builder.GetPayments(connection, companyId, previewsFiscalYear.Year, 0, 12),
                        ScriptQueryType.Receipts => builder.GetReceipts(connection, companyId, previewsFiscalYear.Year, 0, 12),
                        ScriptQueryType.Orders => builder.GetOrders(connection, companyId, previewsFiscalYear.Year, 0, 12),
                        _ => null
                    };

                    if (showType == ScriptPivotShowType.Transfer)
                        list = list.Select(t => (t.Item1, t.Item2 + value ?? 0)).ToList();
                    list.Add((periodName, value ?? 0));
                }
            }

            return list;
        }

        public async Task<Dictionary<int, (int, decimal)>> CreateScriptItemsDictAsync(
            Script script, int traderId, string connection, int companyId, ScriptToolConfigModel config, bool previousFiscalYear = false)
        {
            var scripts = await _scriptService.GetAllScriptsAsync(traderId);
            var fields = await _scriptFieldService.GetAllScriptFieldsAsync(traderId);
            var scriptItemsDict = new Dictionary<int, (int, decimal)>();

            var scriptItems = await _scriptItemService.GetAllScriptItemsAsync(script.Id);
            foreach (var scriptItem in scriptItems)
            {
                if (scriptItem.ScriptTypeId == (int)ScriptType.Field)
                {
                    var field = fields.First(x => x.Id == scriptItem.ScriptFieldId);

                    if (previousFiscalYear)
                        field.FiscalYear = (int)ScriptFiscalYearType.Previews;

                    if (!field.Locked && field.BalanceSheet == false && config.Active == true)
                    {
                        field.FiscalYear = config.FiscalYear;
                        field.PeriodFrom = config.PeriodFrom;
                        field.PeriodTo = config.PeriodTo;
                        field.Inventory = config.Inventory;
                    }

                    switch ((ScriptFieldType)field.ScriptFieldTypeId)
                    {
                        case ScriptFieldType.Table:
                            var fieldValue = await PrepareScriptFieldAsync(field, connection, companyId);
                            scriptItemsDict.Add(scriptItem.Id, (scriptItem.ScriptOperationTypeId, fieldValue));
                            break;
                        case ScriptFieldType.Query:
                            var queryValue = await PrepareScriptQueryAsync(field, connection, companyId);
                            scriptItemsDict.Add(scriptItem.Id, (scriptItem.ScriptOperationTypeId, queryValue));
                            break;
                        case ScriptFieldType.Function:
                            var functionValue = await PrepareScriptFunctionAsync(field, connection, companyId);
                            scriptItemsDict.Add(scriptItem.Id, (scriptItem.ScriptOperationTypeId, functionValue));
                            break;
                        case ScriptFieldType.Fixed:
                            scriptItemsDict.Add(scriptItem.Id, (scriptItem.ScriptOperationTypeId, field.FixedValue));
                            break;
                    }
                }
                if (scriptItem.ScriptTypeId == (int)ScriptType.Script)
                {
                    var _script = scripts.First(x => x.Id == scriptItem.ParentId);
                    var _scriptItemsDict = await CreateScriptItemsDictAsync(_script, traderId, connection, companyId, config, previousFiscalYear);
                    var _total = CreateScriptsDictItem(_scriptItemsDict);

                    scriptItemsDict.Add(scriptItem.Id, (scriptItem.ScriptOperationTypeId, _total));
                }
            }

            return scriptItemsDict;
        }

        public decimal CreateScriptsDictItem(Dictionary<int, (int, decimal)> dicts)
        {
            if (dicts.Count == 0)
                return 0m;

            var key = dicts.Keys.FirstOrDefault();

            dicts.Remove(key, out (int, decimal) value);
            var total = value.Item2;

            foreach (var dict in dicts)
            {
                switch ((ScriptOperationType)dict.Value.Item1)
                {
                    case ScriptOperationType.Addition:
                        total += dict.Value.Item2;
                        break;
                    case ScriptOperationType.Subtraction:
                        total -= dict.Value.Item2;
                        break;
                    case ScriptOperationType.Multiplication:
                        total *= dict.Value.Item2;
                        break;
                    case ScriptOperationType.Division:
                        total /= dict.Value.Item2;
                        break;
                    case ScriptOperationType.Percent:
                        total = total != 0 ? ((dict.Value.Item2 * 100) / total) - 100 : total;
                        break;
                }
            }

            return total;
        }

    }
}