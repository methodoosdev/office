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
    public partial class TraderRatingController : BaseProtectController
    {
        private readonly ITraderRatingService _traderRatingService;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderRatingModelFactory _traderRatingModelFactory;

        public TraderRatingController(
            ITraderRatingService traderRatingService,
            ILocalizationService localizationService,
            ITraderRatingModelFactory traderRatingModelFactory)
        {
            _traderRatingService = traderRatingService;
            _localizationService = localizationService;
            _traderRatingModelFactory = traderRatingModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderRatingModelFactory.PrepareTraderRatingSearchModelAsync(new TraderRatingSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderRatingSearchModel searchModel)
        {
            //prepare model
            var model = await _traderRatingModelFactory.PrepareTraderRatingListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _traderRatingModelFactory.PrepareTraderRatingModelAsync(new TraderRatingModel(), null);

            //prepare form
            var formModel = await _traderRatingModelFactory.PrepareTraderRatingFormModelAsync(new TraderRatingFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TraderRatingModel model)
        {
            if (ModelState.IsValid)
            {
                var traderRating = model.ToEntity<TraderRating>();
                await _traderRatingService.InsertTraderRatingAsync(traderRating);

                return Json(traderRating.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var traderRating = await _traderRatingService.GetTraderRatingByIdAsync(id);
            if (traderRating == null)
                return await AccessDenied();

            //prepare model
            var model = await _traderRatingModelFactory.PrepareTraderRatingModelAsync(null, traderRating);

            //prepare form
            var formModel = await _traderRatingModelFactory.PrepareTraderRatingFormModelAsync(new TraderRatingFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] TraderRatingModel model)
        {
            //try to get entity with the specified id
            var traderRating = await _traderRatingService.GetTraderRatingByIdAsync(model.Id);
            if (traderRating == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    traderRating = model.ToEntity(traderRating);
                    await _traderRatingService.UpdateTraderRatingAsync(traderRating);

                    return Json(traderRating.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderRating.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var traderRating = await _traderRatingService.GetTraderRatingByIdAsync(id);
            if (traderRating == null)
                return await AccessDenied();

            try
            {
                await _traderRatingService.DeleteTraderRatingAsync(traderRating);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderRating.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _traderRatingService.DeleteTraderRatingAsync((await _traderRatingService.GetTraderRatingByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderRating.Errors.TryToDelete");
            }
        }
    }
}