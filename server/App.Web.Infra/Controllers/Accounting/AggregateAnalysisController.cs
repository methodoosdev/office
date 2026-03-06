using App.Core;
using App.Core.Domain.Logging;
using App.Core.Domain.Traders;
using App.Models.Accounting;
using App.Services.Common;
using App.Services.Common.Pdf;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Common.Factories;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Controllers.Accounting
{
    public partial class AggregateAnalysisController : BaseProtectController
    {
        private readonly IAggregateAnalysisFactory _aggregateAnalysisFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICashAvailableFactory _cashAvailableFactory;
        private readonly IVatCalculationFactory _vatCalculationFactory;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public AggregateAnalysisController(
            IAggregateAnalysisFactory aggregateAnalysisFactory,
            ILocalizationService localizationService,
            IHtmlToPdfService htmlToPdfService,
            ITraderConnectionService traderConnectionService,
            ICustomerActivityService customerActivityService,
            ICashAvailableFactory cashAvailableFactory,
            IVatCalculationFactory vatCalculationFactory,
            ISoftoneQueryFactory softoneQueryFactory,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext
            )
        {
            _aggregateAnalysisFactory = aggregateAnalysisFactory;
            _localizationService = localizationService;
            _htmlToPdfService = htmlToPdfService;
            _traderConnectionService = traderConnectionService;
            _customerActivityService = customerActivityService;
            _cashAvailableFactory = cashAvailableFactory;
            _vatCalculationFactory = vatCalculationFactory;
            _softoneQueryFactory = softoneQueryFactory;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            var traderId = trader?.Id ?? 0;
            var searchModel = new AggregateAnalysisSearchModel();
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
            searchModel = await _aggregateAnalysisFactory.PrepareAggregateAnalysisSearchModelAsync(searchModel, years, periods);

            //prepare model
            var tableModel = await _aggregateAnalysisFactory.PrepareAggregateAnalysisTableModelAsync(new AggregateAnalysisTableModel());

            //prepare model
            var totalModel = await _aggregateAnalysisFactory.PrepareAggregateAnalysisTotalTableModelAsync(new AggregateAnalysisTotalTableModel());

            return Json(new { searchModel, tableModel, totalModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AggregateAnalysisSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            try
            {
                var modelList = await _aggregateAnalysisFactory.PrepareAggregateAnalysisOfMonthAsync(connectionResult.Connection, connectionResult.CompanyId, searchModel);
                var progressList = await _aggregateAnalysisFactory.PrepareAggregateAnalysisOfPeriodAsync(connectionResult.Connection, connectionResult.CompanyId, searchModel);
                var totalList = await CashAvailableAndVatAsync(
                                    connectionResult,
                                    new CashAvailableSearchModel
                                    {
                                        TraderId = searchModel.TraderId,
                                        Year = searchModel.Year,
                                        Period = searchModel.Period
                                    });
                await _customerActivityService.InsertActivityOnceAsync(ActivityLogTypeType.AggregateAnalysis);

                return Json(new { modelList, progressList, totalList });
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            return BadRequest("Error: Company time period outside limits");
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

        [HttpPost]
        public async Task<IActionResult> ExportToPdf([FromBody] AggregateAnalysisSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var modelList = await _aggregateAnalysisFactory.PrepareAggregateAnalysisOfMonthAsync(connectionResult.Connection, connectionResult.CompanyId, searchModel);
            var progressList = await _aggregateAnalysisFactory.PrepareAggregateAnalysisOfPeriodAsync(connectionResult.Connection, connectionResult.CompanyId, searchModel);
            var totalList = await CashAvailableAndVatAsync(
                    connectionResult,
                    new CashAvailableSearchModel
                    {
                        TraderId = searchModel.TraderId,
                        Year = searchModel.Year,
                        Period = searchModel.Period
                    });

            var pdfItem = new HtmlPdfItem
            {
                Title = await _localizationService.GetResourceAsync("App.Models.AggregateAnalysisModel.Title"),
                SubTitle = connectionResult.TraderName
            };

            var template = "~/Views/Pdf/AggregateAnalysis.cshtml";

            byte[] bytes = await _htmlToPdfService.PrintToPdf((modelList, progressList, totalList), template, pdfItem, false);
            return File(bytes, MimeTypes.ApplicationPdf);
        }

        private async Task<IList<AggregateAnalysisTotalModel>> CashAvailableAndVatAsync(TraderConnectionResult con, CashAvailableSearchModel searchModel)
        {
            var cashAvailables = await _cashAvailableFactory.PrepareCashAvailableListAsync(con.Connection, con.CompanyId, searchModel);
            var cash = cashAvailables.Where(x => x.Type == "1.Cash").Sum(s => s.Total);
            var bank = cashAvailables.Where(x => x.Type == "2.Bank").Sum(s => s.Total);
            var term = cashAvailables.Where(x => x.Type == "3.Term").Sum(s => s.Total);
            var _else = cashAvailables.Where(x => x.Type == "4.Else").Sum(s => s.Total);

            var vatCalculations = await VatCalculationAsync(con, searchModel.TraderId, searchModel.Year);
            var vat = vatCalculations[searchModel.Period].ToPay;

            var model = new AggregateAnalysisTotalModel();
            model.Cash = cash;
            model.Bank = bank;
            model.Term = term;
            model.Else = _else;
            model.Vat = vat;

            var list = new List<AggregateAnalysisTotalModel> { model };

            return list;
        }
        private async Task<IList<VatCalculationModel>> VatCalculationAsync(TraderConnectionResult con, int traderId, int fiscalYear)
        {
            IList<VatCalculationModel> list = new List<VatCalculationModel>();

            if (con.CategoryBookTypeId == (int)CategoryBookType.C)
                list = await _vatCalculationFactory.PrepareVatCalculationListAsync(con.Connection, con.CompanyId, traderId, fiscalYear);
            else if (con.CategoryBookTypeId == (int)CategoryBookType.B)
            {
                var isProsvasis = con.LogistikiProgramTypeId == (int)LogistikiProgramType.Prosvasis;
                list = await _vatCalculationFactory.PrepareVatBCalculationListAsync(con.Connection, con.CompanyId, isProsvasis, traderId, fiscalYear);

            }
            return list;
        }


    }
}