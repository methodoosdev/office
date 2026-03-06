using App.Core;
using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Models;
using App.Models.Payroll;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Payroll;
using App.Services.Traders;
using App.Web.Infra.Queries.Common;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITraderLegalExpireModelFactory
    {
        //Task<TraderLegalExpireSearchModel> PrepareTraderLegalExpireSearchModelAsync(TraderLegalExpireSearchModel searchModel);
        Task<IList<TraderLegalExpireItem>> PrepareTraderLegalExpireListAsync(string connection);
    }
    public partial class TraderLegalExpireModelFactory : ITraderLegalExpireModelFactory
    {
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderService _traderService;
        private readonly IWorkContext _workContext; 

        public TraderLegalExpireModelFactory(
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            ITraderService traderService,
            IWorkContext workContext)
        {
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _traderService = traderService;
            _workContext = workContext;
        }

        //public virtual async Task<TraderLegalExpireSearchModel> PrepareTraderLegalExpireSearchModelAsync(TraderLegalExpireSearchModel searchModel)
        //{
        //    var traders = await _traderService.GetAllTradersAsync();

        //    foreach(var trader in traders)
        //    {
        //        searchModel.TraderId = trader?.Id ?? 0;
        //        searchModel.Vat = trader?.Vat ?? string.Empty;
        //    }

        //    return searchModel;
        //}
        public virtual async Task<IList<TraderLegalExpireItem>> PrepareTraderLegalExpireListAsync(string connection)
        {
            //var list = new List<TraderLegalExpireItem>();

            var legalList = await _dataProvider.QueryAsync<TraderLegalExpireItem>(connection, TraderLegalExpireQuery.TraderLegalExpireItem);

            //foreach (var trader in traders)
            //{
            //    var item = legalList.FirstOrDefault(x => x.Vat == trader.Vat);
            //    if(item != null)
            //    {
            //        list.Add(item);
            //    }
            //}

            return legalList;
        }

        //public virtual async Task<IList<TraderLegalExpireModel>> PrepareTraderLegalExpireListAsync(TraderLegalExpireSearchModel searchModel, Trader trader, string connection)
        //{
        //    var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", trader.HyperPayrollId);
        //    var pYear = new LinqToDB.Data.DataParameter("pYear", searchModel.To.Year);
        //    var pTo = new LinqToDB.Data.DataParameter("pTo", searchModel.To.ToString("yyyyMMdd"));

        //    var list = await _dataProvider.QueryAsync<TraderLegalExpireModel>(connection, TraderLegalExpireQuery.LeaveDays, pCompanyId, pTo, pYear);

        //    foreach (var workerLeaveDetail in workerLeaveDetails)
        //    {
        //        var item = list.FirstOrDefault(x => x.WorkerId == workerLeaveDetail.WorkerId);
        //        if (item != null)
        //        {
        //            item.DaysLeft = workerLeaveDetail.DaysLeft;
        //        }
        //    }

        //    foreach (var worker in list)
        //    {
        //        worker.TotalDaysLeft = worker.Deserved + worker.DaysLeft - worker.DaysTaken;
        //    }

        //    return list;
        //}

    }
    //public partial record TraderLegalExpireSearchModel : BaseNopModel
    //{
    //    public int TraderId { get; set; }
    //    public string Vat { get; set; }
    //}

    public partial record TraderLegalExpireItem 
    {
        public string Vat { get; set; }
        public string TaxisUserName { get; set; }
        public string TaxisPassword { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

    }
}