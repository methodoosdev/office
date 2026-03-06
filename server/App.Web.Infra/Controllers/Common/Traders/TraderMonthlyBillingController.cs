using App.Core.Domain.Traders;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Traders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    [CheckCustomerPermission(true)]
    public partial class TraderMonthlyBillingController : BaseProtectController
    {
        private readonly ITraderMonthlyBillingService _traderMonthlyBillingService;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderMonthlyBillingModelFactory _traderMonthlyBillingModelFactory;

        public TraderMonthlyBillingController(
            ITraderMonthlyBillingService traderMonthlyBillingService,
            ILocalizationService localizationService,
            ITraderMonthlyBillingModelFactory traderMonthlyBillingModelFactory)
        {
            _traderMonthlyBillingService = traderMonthlyBillingService;
            _localizationService = localizationService;
            _traderMonthlyBillingModelFactory = traderMonthlyBillingModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderMonthlyBillingModelFactory.PrepareTraderMonthlyBillingSearchModelAsync(new TraderMonthlyBillingSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderMonthlyBillingSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _traderMonthlyBillingModelFactory.PrepareTraderMonthlyBillingListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _traderMonthlyBillingModelFactory.PrepareTraderMonthlyBillingModelAsync(new TraderMonthlyBillingModel(), null);

            //prepare form
            var formModel = await _traderMonthlyBillingModelFactory.PrepareTraderMonthlyBillingFormModelAsync(new TraderMonthlyBillingFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TraderMonthlyBillingModel model, int parentId)
        {
            if (ModelState.IsValid)
            {
                var traderMonthlyBilling = model.ToEntity<TraderMonthlyBilling>();
                traderMonthlyBilling.TraderId = parentId;
                await _traderMonthlyBillingService.InsertTraderMonthlyBillingAsync(traderMonthlyBilling);

                return Json(traderMonthlyBilling.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var traderMonthlyBilling = await _traderMonthlyBillingService.GetTraderMonthlyBillingByIdAsync(id);
            if (traderMonthlyBilling == null)
                return await AccessDenied();

            //prepare model
            var model = await _traderMonthlyBillingModelFactory.PrepareTraderMonthlyBillingModelAsync(null, traderMonthlyBilling);

            //prepare form
            var formModel = await _traderMonthlyBillingModelFactory.PrepareTraderMonthlyBillingFormModelAsync(new TraderMonthlyBillingFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] TraderMonthlyBillingModel model)
        {
            //try to get entity with the specified id
            var traderMonthlyBilling = await _traderMonthlyBillingService.GetTraderMonthlyBillingByIdAsync(model.Id);
            if (traderMonthlyBilling == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    traderMonthlyBilling = model.ToEntity(traderMonthlyBilling);
                    await _traderMonthlyBillingService.UpdateTraderMonthlyBillingAsync(traderMonthlyBilling);

                    return Json(traderMonthlyBilling.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderMonthlyBillings.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var traderMonthlyBilling = await _traderMonthlyBillingService.GetTraderMonthlyBillingByIdAsync(id);
            if (traderMonthlyBilling == null)
                return await AccessDenied();

            try
            {
                await _traderMonthlyBillingService.DeleteTraderMonthlyBillingAsync(traderMonthlyBilling);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderMonthlyBillings.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _traderMonthlyBillingService.DeleteTraderMonthlyBillingAsync((await _traderMonthlyBillingService.GetTraderMonthlyBillingsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderMonthlyBillings.Errors.TryToDelete");
            }
        }
    }
}