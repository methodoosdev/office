using App.Core;
using App.Core.Domain.Logging;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Accounting;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Common.Pdf;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Controllers.Accounting
{
    public partial class MonthlyFinancialBulletinController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IMonthlyFinancialBulletinFactory _monthlyFinancialBulletinFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;

        public MonthlyFinancialBulletinController(ITraderService traderService, 
            IMonthlyFinancialBulletinFactory monthlyFinancialBulletinFactory,
            ILocalizationService localizationService,
            IHtmlToPdfService htmlToPdfService,
            ITraderConnectionService traderConnectionService,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _monthlyFinancialBulletinFactory = monthlyFinancialBulletinFactory;
            _localizationService = localizationService;
            _htmlToPdfService = htmlToPdfService;
            _traderConnectionService = traderConnectionService;
            _customerActivityService = customerActivityService;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            var searchModel = new MonthlyFinancialBulletinSearchModel();
            searchModel.Periodos = DateTime.UtcNow;

            IList<SelectionList> branches = new List<SelectionList>();
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader != null)
            {
                searchModel.TraderId = trader.Id;
                var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(trader.Id);
                if (connectionResult.Success)
                {
                    searchModel.ExpirationInventory = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, searchModel.Periodos.Year, "2_._1.%");
                    searchModel.ExpirationDepreciate = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, searchModel.Periodos.Year - 1, "66.%");
                    branches = await _monthlyFinancialBulletinFactory.PrepareBranchesAsync(connectionResult, searchModel.Periodos.Year, searchModel.Periodos.Month);
                }
            }

            //prepare model
            searchModel = await _monthlyFinancialBulletinFactory.PrepareMonthlyFinancialBulletinSearchModelAsync(searchModel, branches);
            
            //prepare model
            var tableModel = await _monthlyFinancialBulletinFactory.PrepareMonthlyFinancialBulletinTableModelAsync(new MonthlyFinancialBulletinTableModel());

            //prepare model
            var remodelingCostsTable = await _monthlyFinancialBulletinFactory.PrepareMonthlyFinancialBulletinRemodelingCostsQueryModelAsync(new MonthlyFinancialBulletinRemodelingCostsQueryModel());
            var resultForm = await _monthlyFinancialBulletinFactory.PrepareMonthlyFinancialBulletinResultFormModelAsync(new MonthlyFinancialBulletinResultFormModel(), searchModel.Periodos.Year);

            return Json(new { searchModel, tableModel, remodelingCostsTable, resultForm });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] MonthlyFinancialBulletinSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);
            
            var model = await _monthlyFinancialBulletinFactory.PrepareMonthlyFinancialBulletinAsync(connectionResult, searchModel);

            //activity log
            await _customerActivityService.InsertActivityOnceAsync(ActivityLogTypeType.MonthlyFinancialBulletin);

            return Json(new { model });
        }

        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> TraderChanged(int traderId, DateTime periodos)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var expirationInventory = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, periodos.Year, "2_._1.%");
            var expirationDepreciate = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, periodos.Year - 1, "66.%");

            var branches = await _monthlyFinancialBulletinFactory.PrepareBranchesAsync(connectionResult, periodos.Year, periodos.Month);

            return Json(new { expirationInventory, expirationDepreciate, branches });
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf([FromBody] MonthlyFinancialBulletinSearchModel searchModel, int level)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var model = await _monthlyFinancialBulletinFactory.PrepareMonthlyFinancialBulletinAsync(connectionResult, searchModel);

            var firstDayOfYear = new DateTime(searchModel.Periodos.Year, 1, 1);
            var firstDayOfMonth = new DateTime(searchModel.Periodos.Year, searchModel.Periodos.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var period = firstDayOfYear.ToString("dd/MM/yyyy") + " - " + lastDayOfMonth.ToString("dd/MM/yyyy");

            var pdfItem = new HtmlPdfItem
            {
                Title = await _localizationService.GetResourceAsync("App.Models.MonthlyFinancialBulletinModel.Title") + " - " + period,
                SubTitle = connectionResult.TraderName
            };

            var list = model.TreeList.Where(x => x.Level <= level).ToList();

            var template = "~/Views/Pdf/MonthlyFinancialBulletin.cshtml";

            byte[] bytes = await _htmlToPdfService.PrintListToPdf(list, template, pdfItem, false);
            return File(bytes, MimeTypes.ApplicationPdf);
        }

        [HttpPost]
        public async Task<IActionResult> ResultModelExportToPdf([FromBody] MonthlyFinancialBulletinResultFormModel model, int traderId, DateTime date)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(traderId);
            if (trader == null)
                return await AccessDenied();

            var firstDayOfYear = new DateTime(date.Year, 1, 1);
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var period = firstDayOfYear.ToString("dd/MM/yyyy") + " - " + lastDayOfMonth.ToString("dd/MM/yyyy");

            var pdfItem = new HtmlPdfItem
            {
                Title = await _localizationService.GetResourceAsync("App.Models.MonthlyFinancialBulletinResultFormModel.Title") + " - " + period,
                SubTitle = trader.ToTraderFullName()
            };

            var fields = new string[]
            {
                "ExpirationInventory", "NetProfitPeriod", "RemodelingCosts", "PreviousYearDamage",
                "TaxProfitPeriod", "TaxIncome", "HoldingTaxAdvance", "TaxAdvance",
                "PaymentPreviousYear", "TaxesFee", "AmountPayable", "TaxReturn"
            };

            var modelType = model.GetType();
            var list = new List<MonthlyFinancialBulletinPdfForm>();
            foreach (var field in fields) 
            {
                var description = await _localizationService.GetResourceAsync($"App.Models.MonthlyFinancialBulletinResultFormModel.Fields.{field}");
                var value = modelType.GetProperty(field).GetValue(model, null) as decimal?;
                list.Add(new MonthlyFinancialBulletinPdfForm { Description = description, Value = value.Value });
            }

            var template = "~/Views/Pdf/MonthlyFinancialBulletinForm.cshtml";

            byte[] bytes = await _htmlToPdfService.PrintListToPdf(list, template, pdfItem);
            return File(bytes, Core.MimeTypes.ApplicationPdf);
        }

    }
}