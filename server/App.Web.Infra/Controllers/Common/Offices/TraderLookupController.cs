using App.Core;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Offices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Offices
{
    [CheckCustomerPermission(true)]
    public partial class TraderLookupController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ITraderLookupModelFactory _traderLookupModelFactory;

        public TraderLookupController(
            ITraderService traderService,
            ITraderLookupModelFactory traderLookupModelFactory)
        {
            _traderService = traderService;
            _traderLookupModelFactory = traderLookupModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderLookupModelFactory.PrepareTraderLookupSearchModelAsync(new TraderLookupSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderLookupSearchModel searchModel)
        {
            //prepare model
            var model = await _traderLookupModelFactory.PrepareTraderLookupListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetTraderEmails([FromBody] ICollection<int> selectedIds)
        {
            var traders = await _traderService.GetTradersByIdsAsync(selectedIds.ToArray());

            traders = traders
                .Where(e =>
                    !string.IsNullOrEmpty(e.Email?.Trim() ?? "") ||
                    !string.IsNullOrEmpty(e.Email2?.Trim() ?? "") ||
                    !string.IsNullOrEmpty(e.Email3?.Trim() ?? ""))
                .ToList();

            var emails = traders
                .Select(x => (new string[] { x.Email, x.Email2, x.Email3 }).First(f => !string.IsNullOrEmpty(f)))
                .ToList();

            var recipients = string.Join(',', emails);

            return Json(new { recipients });
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetTraderCurrentEmail([FromBody] int traderId)
        {
            var _trader = await _traderService.GetTraderByIdAsync(traderId);
            var trader = _trader.ToModel<TraderModel>();


            var currentEmail = (new string[] { trader.Email, trader.Email2, trader.Email3 }).First(f => !string.IsNullOrEmpty(f));

            return Json(new { traderName = trader.FullName(), email = currentEmail });
        }
    }
}