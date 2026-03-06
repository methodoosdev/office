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
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Accounting
{
    public partial class IntertemporalBController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IIntertemporalBFactory _intertemporalFactory;
        private readonly IMonthlyBCategoryBulletinFactory _monthlyFinancialBulletinFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;

        public IntertemporalBController(ITraderService traderService,
            IIntertemporalBFactory intertemporalFactory,
            IMonthlyBCategoryBulletinFactory monthlyFinancialBulletinFactory,
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
                    searchModel.ExpirationInventory = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, year, 12, "2_._1.%");
                    searchModel.ExpirationDepreciate = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, year - 1, 12, "66.%");
                }
            }

            //prepare model
            var remodelingCostsTable = await _monthlyFinancialBulletinFactory.PrepareMonthlyBCategoryBulletinRemodelingCostsQueryModelAsync(new MonthlyBCategoryBulletinRemodelingCostsQueryModel());
            var resultForm = await _monthlyFinancialBulletinFactory.PrepareMonthlyBCategoryBulletinResultFormModelAsync(new MonthlyBCategoryBulletinResultFormModel(), DateTime.UtcNow.Year);

            return Json(new { searchModel, remodelingCostsTable, resultForm });

        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] IntertemporalSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var pivot = await _intertemporalFactory.PrepareIntertemporalQueryListAsync(connectionResult);

            var monthlyFinancialBulletinSearchModel = new MonthlyBCategoryBulletinSearchModel
            {
                TraderId = searchModel.TraderId,
                Period = DateTime.UtcNow,
                ExpirationInventory = searchModel.ExpirationInventory,
                ExpirationDepreciate = searchModel.ExpirationDepreciate
            };

            var model = await _monthlyFinancialBulletinFactory.PrepareMonthlyBCategoryBulletinAsync(connectionResult, monthlyFinancialBulletinSearchModel);

            return Json(new { pivot, model.RemodelingCostsList, model.ResultModel });
        }

        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> TraderChanged(int traderId)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var date = DateTime.UtcNow;
            var expirationInventory = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, date.Year, date.Month, "2_._1.%");
            var expirationDepreciate = await _monthlyFinancialBulletinFactory.PrepareExpirationAsync(connectionResult, date.Year - 1, date.Month, "66.%");

            return Json(new { expirationInventory, expirationDepreciate });
        }

    }
}

