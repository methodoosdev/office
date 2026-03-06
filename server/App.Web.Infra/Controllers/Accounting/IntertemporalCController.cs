using App.Core;
using App.Models.Accounting;
using App.Services.Common;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Accounting;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Accounting
{
    public partial class IntertemporalCController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IIntertemporalCFactory _intertemporalFactory;
        private readonly IMonthlyFinancialBulletinFactory _monthlyFinancialBulletinFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;

        public IntertemporalCController(ITraderService traderService,
            IIntertemporalCFactory intertemporalFactory,
            IMonthlyFinancialBulletinFactory monthlyFinancialBulletinFactory,
            ILocalizationService localizationService,
            IHtmlToPdfService htmlToPdfService,
            ITraderConnectionService traderConnectionService,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _intertemporalFactory = intertemporalFactory;
            _monthlyFinancialBulletinFactory = monthlyFinancialBulletinFactory;
            _localizationService = localizationService;
            _htmlToPdfService = htmlToPdfService;
            _traderConnectionService = traderConnectionService;
            _customerActivityService = customerActivityService;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var year = DateTime.UtcNow.Year;
            var searchModel = await _intertemporalFactory.PrepareIntertemporalSearchModelAsync(new IntertemporalSearchModel());

            if (searchModel.TraderId > 0)
            {
                var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
                if (connectionResult.Success)
                {
                    searchModel.ExpirationInventory = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, year, "2_._1.%");
                    searchModel.ExpirationDepreciate = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, year - 1, "66.%");
                }
            }

            //prepare model
            var remodelingCostsTable = await _monthlyFinancialBulletinFactory.PrepareMonthlyFinancialBulletinRemodelingCostsQueryModelAsync(new MonthlyFinancialBulletinRemodelingCostsQueryModel());
            var resultForm = await _monthlyFinancialBulletinFactory.PrepareMonthlyFinancialBulletinResultFormModelAsync(new MonthlyFinancialBulletinResultFormModel(), year);

            return Json(new { searchModel, remodelingCostsTable, resultForm });

        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] IntertemporalSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var pivot = await _intertemporalFactory.PrepareIntertemporalQueryListAsync(connectionResult);

            var monthlyFinancialBulletinSearchModel = new MonthlyFinancialBulletinSearchModel 
            {
                TraderId = searchModel.TraderId,
                Periodos = DateTime.UtcNow,
                ExpirationInventory = searchModel.ExpirationInventory,
                ExpirationDepreciate = searchModel.ExpirationDepreciate
            };

            var model = await _monthlyFinancialBulletinFactory.PrepareMonthlyFinancialBulletinAsync(connectionResult, monthlyFinancialBulletinSearchModel);

            return Json(new { pivot, model.RemodelingCostsList, model.ResultModel });
        }

        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> TraderChanged(int traderId)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var expirationInventory = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, DateTime.UtcNow.Year, "2_._1.%");
            var expirationDepreciate = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, DateTime.UtcNow.Year - 1, "66.%");

            return Json(new { expirationInventory, expirationDepreciate });
        }

    }
}

