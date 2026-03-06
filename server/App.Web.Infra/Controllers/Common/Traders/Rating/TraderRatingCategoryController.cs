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
    public partial class TraderRatingCategoryController : BaseProtectController
    {
        private readonly ITraderRatingCategoryService _traderRatingCategoryService;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderRatingCategoryModelFactory _traderRatingCategoryModelFactory;

        public TraderRatingCategoryController(
            ITraderRatingCategoryService traderRatingCategoryService,
            ILocalizationService localizationService,
            ITraderRatingCategoryModelFactory traderRatingCategoryModelFactory)
        {
            _traderRatingCategoryService = traderRatingCategoryService;
            _localizationService = localizationService;
            _traderRatingCategoryModelFactory = traderRatingCategoryModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderRatingCategoryModelFactory.PrepareTraderRatingCategorySearchModelAsync(new TraderRatingCategorySearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderRatingCategorySearchModel searchModel)
        {
            //prepare model
            var model = await _traderRatingCategoryModelFactory.PrepareTraderRatingCategoryListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _traderRatingCategoryModelFactory.PrepareTraderRatingCategoryModelAsync(new TraderRatingCategoryModel(), null);

            //prepare form
            var formModel = await _traderRatingCategoryModelFactory.PrepareTraderRatingCategoryFormModelAsync(new TraderRatingCategoryFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TraderRatingCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var traderRatingCategory = model.ToEntity<TraderRatingCategory>();
                await _traderRatingCategoryService.InsertTraderRatingCategoryAsync(traderRatingCategory);

                return Json(traderRatingCategory.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var traderRatingCategory = await _traderRatingCategoryService.GetTraderRatingCategoryByIdAsync(id);
            if (traderRatingCategory == null)
                return await AccessDenied();

            //prepare model
            var model = await _traderRatingCategoryModelFactory.PrepareTraderRatingCategoryModelAsync(null, traderRatingCategory);

            //prepare form
            var formModel = await _traderRatingCategoryModelFactory.PrepareTraderRatingCategoryFormModelAsync(new TraderRatingCategoryFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] TraderRatingCategoryModel model)
        {
            //try to get entity with the specified id
            var traderRatingCategory = await _traderRatingCategoryService.GetTraderRatingCategoryByIdAsync(model.Id);
            if (traderRatingCategory == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    traderRatingCategory = model.ToEntity(traderRatingCategory);
                    await _traderRatingCategoryService.UpdateTraderRatingCategoryAsync(traderRatingCategory);

                    return Json(traderRatingCategory.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderRatingCategory.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var traderRatingCategory = await _traderRatingCategoryService.GetTraderRatingCategoryByIdAsync(id);
            if (traderRatingCategory == null)
                return await AccessDenied();

            try
            {
                await _traderRatingCategoryService.DeleteTraderRatingCategoryAsync(traderRatingCategory);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderRatingCategory.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _traderRatingCategoryService.DeleteTraderRatingCategoryAsync((await _traderRatingCategoryService.GetTraderRatingCategoriesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderRatingCategory.Errors.TryToDelete");
            }
        }
    }
}