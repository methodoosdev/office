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
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Controllers.Accounting
{
    public partial class _AggregateAnalysisController : BaseProtectController
    {
        private readonly _IAggregateAnalysisFactory _aggregateAnalysisFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICashAvailableFactory _cashAvailableFactory;
        private readonly IVatCalculationFactory _vatCalculationFactory;
        private readonly IDateTimeHelper _dateTimeHelper;

        public _AggregateAnalysisController(
            _IAggregateAnalysisFactory aggregateAnalysisFactory,
            ILocalizationService localizationService,
            IHtmlToPdfService htmlToPdfService,
            ITraderConnectionService traderConnectionService,
            ICustomerActivityService customerActivityService,
            ICashAvailableFactory cashAvailableFactory,
            IVatCalculationFactory vatCalculationFactory,
            IDateTimeHelper dateTimeHelper
            )
        {
            _aggregateAnalysisFactory = aggregateAnalysisFactory;
            _localizationService = localizationService;
            _htmlToPdfService = htmlToPdfService;
            _traderConnectionService = traderConnectionService;
            _customerActivityService = customerActivityService;
            _cashAvailableFactory = cashAvailableFactory;
            _vatCalculationFactory = vatCalculationFactory;
            _dateTimeHelper = dateTimeHelper;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _aggregateAnalysisFactory.PrepareAggregateAnalysisSearchModelAsync(new AggregateAnalysisSearchModel());

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

            var period = searchModel.Period;
            var year = searchModel.Year;
            var modelList = await _aggregateAnalysisFactory.PrepareAggregateAnalysisListAsync(connectionResult, period, year);
            var progressList = await _aggregateAnalysisFactory.PrepareAggregateAnalysisProgressListAsync(connectionResult, period, year);
            var totalList = await CashAvailableAndVatAsync(connectionResult, DateTime.Now);

            await _customerActivityService.InsertActivityOnceAsync(ActivityLogTypeType.AggregateAnalysis);

            return Json(new { modelList, progressList, totalList });
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf([FromBody] AggregateAnalysisSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var period = searchModel.Period;
            var year = searchModel.Year;
            var modelList = await _aggregateAnalysisFactory.PrepareAggregateAnalysisListAsync(connectionResult, period, year);
            var progressList = await _aggregateAnalysisFactory.PrepareAggregateAnalysisProgressListAsync(connectionResult, period, year);
            var totalList = await CashAvailableAndVatAsync(connectionResult, DateTime.Now);

            var pdfItem = new HtmlPdfItem
            {
                Title = await _localizationService.GetResourceAsync("App.Models.AggregateAnalysisModel.Title"),
                SubTitle = connectionResult.TraderName
            };

            var template = "~/Views/Pdf/AggregateAnalysis.cshtml";

            byte[] bytes = await _htmlToPdfService.PrintToPdf((modelList, progressList, totalList), template, pdfItem, false);
            return File(bytes, MimeTypes.ApplicationPdf);
        }

        private async Task<IList<AggregateAnalysisTotalModel>> CashAvailableAndVatAsync(TraderConnectionResult connectionResult, DateTime period)
        {
            var cashAvailables = await CashAvailableAsync(connectionResult, period);
            var cashier = cashAvailables.Where(x => x.Type == "1.Cash").Sum(s => s.Total);
            var sidedAccount = cashAvailables.Where(x => x.Type == "2.Bank").Sum(s => s.Total);

            var vatCalculations = await VatCalculationAsync(connectionResult, period);
            var vat = vatCalculations[period.Month].ToPay;

            var model = new AggregateAnalysisTotalModel();
            model.Cash = cashier;
            model.Bank = sidedAccount;
            model.Vat = vat;

            var list = new List<AggregateAnalysisTotalModel> { model };

            return list;
        }
        private async Task<IList<CashAvailableModel>> CashAvailableAsync(TraderConnectionResult connectionResult, DateTime date)
        {
            throw new NotImplementedException();
            //var list = await _cashAvailableFactory.PrepareCashAvailableListAsync(connectionResult, date.Date);

            //return list;
        }
        private async Task<IList<VatCalculationModel>> VatCalculationAsync(TraderConnectionResult connectionResult, DateTime date)
        {
            IList<VatCalculationModel> list = new List<VatCalculationModel>();

            //if (connectionResult.CategoryBookTypeId == (int)CategoryBookType.C)
            //    list = await _vatCalculationFactory.PrepareVatCalculationListAsync(connectionResult, date.Year);
            //else if (connectionResult.CategoryBookTypeId == (int)CategoryBookType.B)
            //    list = await _vatCalculationFactory.PrepareVatBCalculationListAsync(connectionResult, date.Year);

            return list;
        }


    }
}