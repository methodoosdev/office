using App.Core;
using App.Data.DataProviders;
using App.Models.Accounting;
using App.Services.Helpers;
using App.Services.Traders;
using App.Web.Infra.Queries;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface ISoftoneQueryFactory
    {
        Task<(bool Exists, int Schema)> CheckPrevieusYearExistsAsync(string connection, int companyId, int year);
        Task<(CompanyYearUsesModel Model, int CurrentPeriod)> GetActiveTimePeriodAsync(string connection, int companyId, DateTime date);
        Task<Dictionary<int, (DateTime From, DateTime To, string Name)>> GetPeriodsPerYearAsync(string connection, int companyId, int year);
        Task<IList<CompanyPeriodUsesModel>> GetCompanyPeriodUsesAsync(string connection, int companyId, int year = 0);
        Task<IList<CompanyYearUsesModel>> GetCompanyYearUsesAsync(string connection, int companyId, int year = 0);
        Task<IList<AccountingCodePerSchemaPerGradeModel>> GetAccountingCodePerSchemaPerGradeAsync(string connection, string code, int schema = 0, int grande = 0);
        Task<FiscalPeriodPerYearModel> FiscalPeriodPerYearAsync(int traderId);
        Task<FiscalPeriodPerYearModel> FiscalPeriodPerYearAsync(string connection, int companyId);
        Task<FiscalYearModel> FiscalYearAsync(string connection, int companyId);
    }

    public partial class SoftoneQueryFactory : ISoftoneQueryFactory
    {
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public SoftoneQueryFactory(
            ITraderConnectionService traderConnectionService,
            IAppDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _traderConnectionService = traderConnectionService;
            _dataProvider = dataProvider;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        public async Task<(bool Exists, int Schema)> CheckPrevieusYearExistsAsync(string connection, int companyId, int year)
        {
            var companyYears = await GetCompanyYearUsesAsync(connection, companyId, year);
            if (companyYears.Count > 0)
                return (true, companyYears.First().Schema);
            else
                return (false, 0);
        }

        public async Task<(CompanyYearUsesModel Model, int CurrentPeriod)> GetActiveTimePeriodAsync(string connection, int companyId, DateTime date)
        {
            var list = await GetCompanyYearUsesAsync(connection, companyId);

            var item = list.First(x => x.FromDate.Date <= date.Date && x.ToDate.Date >= date.Date);

            var currentPeriod = date.Month;

            if (!(item.FromDate.Month == 1))
                currentPeriod = currentPeriod > 6 ? currentPeriod - 6 : currentPeriod + 6;

            return (item, currentPeriod);
        }

        public async Task<Dictionary<int, (DateTime From, DateTime To, string Name)>> GetPeriodsPerYearAsync(string connection, int companyId, int year)
        {
            var list = await GetCompanyPeriodUsesAsync(connection, companyId, year);
            var culture = CultureInfo.CurrentCulture;

            var dict = list.OrderBy(x => x.Period).ToDictionary(
                item => item.Period,
                item => (item.FromDate.Date, item.ToDate.Date, culture.DateTimeFormat.GetMonthName(item.FromDate.Month))
                );

            return dict;
        }

        public async Task<IList<CompanyPeriodUsesModel>> GetCompanyPeriodUsesAsync(string connection, int companyId, int year = 0)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);

            var list = await _dataProvider.QueryAsync<CompanyPeriodUsesModel>(connection, SoftOneQuery.CompanyPeriodUses, pCompanyId);

            if (year > 0)
                list = list.Where(x => x.Year == year).ToList();

            return list;
        }

        public async Task<IList<CompanyYearUsesModel>> GetCompanyYearUsesAsync(string connection, int companyId, int year = 0)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);

            var list = await _dataProvider.QueryAsync<CompanyYearUsesModel>(connection, SoftOneQuery.CompanyYearUses, pCompanyId);

            if (year > 0)
                list = list.Where(x => x.Year == year).ToList();

            return list;
        }

        public async Task<IList<AccountingCodePerSchemaPerGradeModel>> GetAccountingCodePerSchemaPerGradeAsync(string connection, string code, int schema = 0, int grande = 0)
        {
            var pCode = new LinqToDB.Data.DataParameter("pCode", code);

            var list = await _dataProvider.QueryAsync<AccountingCodePerSchemaPerGradeModel>(connection, SoftOneQuery.AccountingCodesPerSchemaPerGrade, pCode);

            if (schema > 0)
                list = list.Where(x => x.Schema == schema).ToList();

            if (grande > 0)
                list = list.Where(x => x.Grade == grande).ToList();

            return list;
        }

        public async Task<FiscalPeriodPerYearModel> FiscalPeriodPerYearAsync(int traderId)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (!connectionResult.Success)
                throw new Exception(connectionResult.Error);

            return await FiscalPeriodPerYearAsync(connectionResult.Connection, connectionResult.CompanyId);
        }

        public async Task<FiscalPeriodPerYearModel> FiscalPeriodPerYearAsync(string connection, int companyId)
        {
            var fiscalYears = await GetCompanyYearUsesAsync(connection, companyId);
            var years = fiscalYears.Select(x => new SelectionItemList { Value = x.Year, Label = x.Name }).OrderByDescending(o => o.Value).ToList();

            var lastYear = fiscalYears.Max(x => x.Year);

            var dict = await GetPeriodsPerYearAsync(connection, companyId, lastYear);
            var list = dict
                .Select(kvp => new { Key = kvp.Key, Name = kvp.Value.Name })
                .ToList();

            var periods = list.Select(x => new SelectionItemList { Value = x.Key, Label = x.Name }).ToList();
            var period = DateTime.UtcNow.Month;

            dict.TryGetValue(period, out var currentPeriod);

            if (!currentPeriod.From.Month.Equals(period))
                period = period > 6 ? period - 6 : period + 6;

            return new FiscalPeriodPerYearModel { Year = lastYear, Period = period, Years = years, Periods = periods };
        }

        public async Task<FiscalYearModel> FiscalYearAsync(string connection, int companyId)
        {
            var fiscalYears = await GetCompanyYearUsesAsync(connection, companyId);
            var years = fiscalYears.Select(x => new SelectionItemList { Value = x.Year, Label = x.Name }).OrderByDescending(o => o.Value).ToList();

            var lastYear = fiscalYears.Max(x => x.Year);

            return new FiscalYearModel { Year = lastYear, Years = years };
        }

    }
}