using App.Core;
using App.Models.Accounting;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Common.Pdf;
using App.Services.Localization;
using App.Services.Security;
using App.Services.Traders;
using App.Web.Common.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Accounting
{
    public partial class PeriodicityItemsController : BaseProtectController
    {
        private readonly IPeriodicityItemsModelFactory _periodicityItemsModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ILocalizationService _localizationService;

        public PeriodicityItemsController(
            IPeriodicityItemsModelFactory periodicityItemsModelFactory,
            IPermissionService permissionService,
            ITraderConnectionService traderConnectionService,
            IHtmlToPdfService htmlToPdfService,
            ILocalizationService localizationService)
        {
            _periodicityItemsModelFactory = periodicityItemsModelFactory;
            _permissionService = permissionService;
            _traderConnectionService = traderConnectionService;
            _htmlToPdfService = htmlToPdfService;
            _localizationService = localizationService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _periodicityItemsModelFactory.PreparePeriodicityItemsSearchModelAsync(new PeriodicityItemsSearchModel());

            //prepare model
            var tableModel = await _periodicityItemsModelFactory.PreparePeriodicityItemsTableModelAsync(new PeriodicityItemsTableModel());

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] PeriodicityItemsSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var date = searchModel.Period;

            var model = await _periodicityItemsModelFactory.PreparePeriodicityItemsListAsync(connectionResult, date.Year, date.Month);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf([FromBody] PeriodicityItemsSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var period = searchModel.Period.Month;
            var year = searchModel.Period.Year;
            var list = await _periodicityItemsModelFactory.PreparePeriodicityItemsListAsync(connectionResult, period, year);

            var pdfItem = new HtmlPdfItem
            {
                Title = await _localizationService.GetResourceAsync("App.Models.PeriodicityItemsModel.Title"),
                SubTitle = connectionResult.TraderName
            };

            var template = "~/Views/Pdf/PeriodicityItems.cshtml";

            byte[] bytes = await _htmlToPdfService.PrintListToPdf(list, template, pdfItem, false);
            return File(bytes, MimeTypes.ApplicationPdf);
        }
    }
}