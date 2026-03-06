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
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Accounting.Controllers
{
    public partial class VatCalculationController : BaseProtectController
    {
        private readonly IVatCalculationFactory _vatCalculationFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public VatCalculationController(
            IVatCalculationFactory vatCalculationFactory,
            ILocalizationService localizationService,
            IHtmlToPdfService htmlToPdfService,
            ITraderConnectionService traderConnectionService,
            ISoftoneQueryFactory softoneQueryFactory,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _vatCalculationFactory = vatCalculationFactory;
            _localizationService = localizationService;
            _htmlToPdfService = htmlToPdfService;
            _traderConnectionService = traderConnectionService;
            _softoneQueryFactory = softoneQueryFactory;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            var traderId = trader?.Id ?? 0;
            var searchModel = new VatCalculationSearchModel();
            var years = new List<SelectionItemList>();
            var periods = new List<SelectionItemList>();

            if (traderId > 0)
            {
                var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
                if (!connectionResult.Success)
                    return await BadRequestMessageAsync(connectionResult.Error);

                var results = await _softoneQueryFactory.FiscalYearAsync(connectionResult.Connection, connectionResult.CompanyId);
                years = results.Years;

                searchModel.TraderId = traderId;
                searchModel.Year = results.Year;
            }

            //prepare model
            searchModel = await _vatCalculationFactory.PrepareVatCalculationSearchModelAsync(searchModel, years);

            //prepare model
            var tableModel = await _vatCalculationFactory.PrepareVatCalculationTableModelAsync(new VatCalculationTableModel());

            return Json(new { tableModel, searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] VatCalculationSearchModel searchModel)
        {            
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            IList<VatCalculationModel> data = new List<VatCalculationModel>();

            if (connectionResult.CategoryBookTypeId == (int)CategoryBookType.C)
                data = await _vatCalculationFactory.PrepareVatCalculationListAsync(
                    connectionResult.Connection, connectionResult.CompanyId, searchModel.TraderId, searchModel.Year);
            else if (connectionResult.CategoryBookTypeId == (int)CategoryBookType.B)
            {
                var isProsvasis = connectionResult.LogistikiProgramTypeId == (int)LogistikiProgramType.Prosvasis;
                data = await _vatCalculationFactory.PrepareVatBCalculationListAsync(
                    connectionResult.Connection, connectionResult.CompanyId, isProsvasis, searchModel.TraderId, searchModel.Year);
            }
            else
                return await BadRequestMessageAsync("No Company");

            await _customerActivityService.InsertActivityOnceAsync(ActivityLogTypeType.VatCalculation);

            return Json(new { data });
        }

        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> TraderChanged(int traderId)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var results = await _softoneQueryFactory.FiscalYearAsync(connectionResult.Connection, connectionResult.CompanyId);

            return Json(results);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf([FromBody] VatCalculationSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            IList<VatCalculationModel> list = new List<VatCalculationModel>();

            if (connectionResult.CategoryBookTypeId == (int)CategoryBookType.C)
                list = await _vatCalculationFactory.PrepareVatCalculationListAsync(
                    connectionResult.Connection, connectionResult.CompanyId, searchModel.TraderId, searchModel.Year);
            else if (connectionResult.CategoryBookTypeId == (int)CategoryBookType.B)
            {
                var isProsvasis = connectionResult.LogistikiProgramTypeId == (int)LogistikiProgramType.Prosvasis;
                list = await _vatCalculationFactory.PrepareVatBCalculationListAsync(
                    connectionResult.Connection, connectionResult.CompanyId, isProsvasis, searchModel.TraderId, searchModel.Year);
            }
            else
                return await BadRequestMessageAsync("No Company");

            var pdfItem = new HtmlPdfItem
            {
                Title = await _localizationService.GetResourceAsync("App.Models.VatCalculationModel.Title"),
                SubTitle = connectionResult.TraderName
            };

            var template = "~/Views/Pdf/VatCalculation.cshtml";

            byte[] bytes = await _htmlToPdfService.PrintListToPdf(list, template, pdfItem);
            return File(bytes, MimeTypes.ApplicationPdf);
        }
    }
}