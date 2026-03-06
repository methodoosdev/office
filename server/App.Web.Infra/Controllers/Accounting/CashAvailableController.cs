using App.Core;
using App.Core.Domain.Logging;
using App.Models.Accounting;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Common.Factories;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Controllers.Accounting
{ 
    public partial class CashAvailableController : BaseProtectController
    {
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ICashAvailableFactory _cashAvailableFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;

        public CashAvailableController(
            ITraderConnectionService traderConnectionService,
            ISoftoneQueryFactory softoneQueryFactory,
            ICashAvailableFactory cashAvailableFactory,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext
            )
        {
            _traderConnectionService = traderConnectionService;
            _softoneQueryFactory = softoneQueryFactory;
            _cashAvailableFactory = cashAvailableFactory;
            _customerActivityService = customerActivityService;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            var traderId = trader?.Id ?? 0;
            var searchModel = new CashAvailableSearchModel();
            var years = new List<SelectionItemList>();
            var periods = new List<SelectionItemList>();

            if (traderId > 0)
            {
                var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
                if (!connectionResult.Success)
                    return await BadRequestMessageAsync(connectionResult.Error);

                var results = await _softoneQueryFactory.FiscalPeriodPerYearAsync(connectionResult.Connection, connectionResult.CompanyId);
                years = results.Years;
                periods = results.Periods;

                searchModel.TraderId = traderId;
                searchModel.Year = results.Year;
                searchModel.Period = results.Period;
            }

            //prepare model
            searchModel = await _cashAvailableFactory.PrepareCashAvailableSearchModelAsync(searchModel, years, periods);

            //prepare model
            var tableModel = await _cashAvailableFactory.PrepareCashAvailableTableModelAsync(new CashAvailableTableModel());

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] CashAvailableSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var model = await _cashAvailableFactory.PrepareCashAvailableListAsync(connectionResult.Connection, connectionResult.CompanyId, searchModel);

            await _customerActivityService.InsertActivityOnceAsync(ActivityLogTypeType.CashAvailable);

            return Json(model);
        }

        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> TraderChanged(int traderId)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var results = await _softoneQueryFactory.FiscalPeriodPerYearAsync(connectionResult.Connection, connectionResult.CompanyId);

            return Json(results);
        }

        //[HttpPost]
        //public async Task<IActionResult> ExportToPdf([FromBody] CashAvailableSearchModel searchModel)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTraders))
        //        return await AccessDenied();

        //    var result = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
        //    if (!result.Success)
        //        return await BadRequestMessageAsync(result.Error);

        //    var period = searchModel.Periodos.Month;
        //    var year = searchModel.Periodos.Year;
        //    var list = await _cashAvailableFactory.PrepareCashAvailableListAsync(result.Connection, result.Trader, period, year);

        //    var dict = new Dictionary<string, object>
        //    {
        //        ["title"] = await _localizationService.GetResourceAsync("App.Models.CashAvailableModel.Title"),
        //        ["customerName"] = result.Trader.FullName()
        //    };

        //    var template = "~/Views/Pdf/CashAvailable.cshtml";

        //    byte[] bytes = await _htmlToPdfService.PrintListToPdf(list, template, 0, dict);
        //    return File(bytes, MimeTypes.ApplicationPdf);
        //}
    }
}