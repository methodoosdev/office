using App.Core.Domain.Offices;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Offices;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Offices;
using App.Services.Security;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Offices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Offices
{
    public partial class PeriodicityItemController : BaseProtectController
    {
        private readonly IPeriodicityItemService _periodicityItemService;
        private readonly ILocalizationService _localizationService;
        private readonly IPeriodicityItemModelFactory _periodicityItemModelFactory;

        public PeriodicityItemController(
            IPeriodicityItemService periodicityItemService,
            ILocalizationService localizationService,
            IPeriodicityItemModelFactory periodicityItemModelFactory)
        {
            _periodicityItemService = periodicityItemService;
            _localizationService = localizationService;
            _periodicityItemModelFactory = periodicityItemModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _periodicityItemModelFactory.PreparePeriodicityItemSearchModelAsync(new PeriodicityItemSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] PeriodicityItemSearchModel searchModel)
        {
            //prepare model
            var model = await _periodicityItemModelFactory.PreparePeriodicityItemListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _periodicityItemModelFactory.PreparePeriodicityItemModelAsync(new PeriodicityItemModel(), null);

            //prepare form
            var formModel = await _periodicityItemModelFactory.PreparePeriodicityItemFormModelAsync(new PeriodicityItemFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] PeriodicityItemModel model)
        {
            if (ModelState.IsValid)
            {
                var periodicityItem = model.ToEntity<PeriodicityItem>();
                await _periodicityItemService.InsertPeriodicityItemAsync(periodicityItem);

                return Json(periodicityItem.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var periodicityItem = await _periodicityItemService.GetPeriodicityItemByIdAsync(id);
            if (periodicityItem == null)
                return await AccessDenied();

            //prepare model
            var model = await _periodicityItemModelFactory.PreparePeriodicityItemModelAsync(null, periodicityItem);

            //prepare form
            var formModel = await _periodicityItemModelFactory.PreparePeriodicityItemFormModelAsync(new PeriodicityItemFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] PeriodicityItemModel model)
        {
            //try to get entity with the specified id
            var periodicityItem = await _periodicityItemService.GetPeriodicityItemByIdAsync(model.Id);
            if (periodicityItem == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    periodicityItem = model.ToEntity(periodicityItem);
                    await _periodicityItemService.UpdatePeriodicityItemAsync(periodicityItem);

                    return Json(periodicityItem.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.PeriodicityItems.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var periodicityItem = await _periodicityItemService.GetPeriodicityItemByIdAsync(id);
            if (periodicityItem == null)
                return await AccessDenied();

            try
            {
                await _periodicityItemService.DeletePeriodicityItemAsync(periodicityItem);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.PeriodicityItems.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _periodicityItemService.DeletePeriodicityItemAsync((await _periodicityItemService.GetPeriodicityItemsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.PeriodicityItems.Errors.TryToDelete");
            }
        }
    }
}