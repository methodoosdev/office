using App.Core.Domain.Traders;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Traders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class TraderGroupController : BaseProtectController
    {
        private readonly ITraderGroupService _traderGroupService;
        private readonly ITraderGroupModelFactory _traderGroupModelFactory;

        public TraderGroupController(
            ITraderGroupService traderGroupService,
            ITraderGroupModelFactory traderGroupModelFactory)
        {
            _traderGroupService = traderGroupService;
            _traderGroupModelFactory = traderGroupModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderGroupModelFactory.PrepareTraderGroupSearchModelAsync(new TraderGroupSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderGroupSearchModel searchModel)
        {
            //prepare model
            var model = await _traderGroupModelFactory.PrepareTraderGroupListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _traderGroupModelFactory.PrepareTraderGroupModelAsync(new TraderGroupModel(), null);

            //prepare form
            var formModel = await _traderGroupModelFactory.PrepareTraderGroupFormModelAsync(new TraderGroupFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TraderGroupModel model)
        {
            if (ModelState.IsValid)
            {
                var traderGroup = model.ToEntity<TraderGroup>();
                await _traderGroupService.InsertTraderGroupAsync(traderGroup);

                return Json(traderGroup.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var traderGroup = await _traderGroupService.GetTraderGroupByIdAsync(id);
            if (traderGroup == null)
                return await AccessDenied();

            //prepare model
            var model = await _traderGroupModelFactory.PrepareTraderGroupModelAsync(null, traderGroup);

            //prepare form
            var formModel = await _traderGroupModelFactory.PrepareTraderGroupFormModelAsync(new TraderGroupFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] TraderGroupModel model)
        {
            //try to get entity with the specified id
            var traderGroup = await _traderGroupService.GetTraderGroupByIdAsync(model.Id);
            if (traderGroup == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    traderGroup = model.ToEntity(traderGroup);
                    await _traderGroupService.UpdateTraderGroupAsync(traderGroup);

                    return Json(traderGroup.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch (Exception exc)
            {
                return await BadRequestMessageAsync("App.Models.TraderGroups.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var traderGroup = await _traderGroupService.GetTraderGroupByIdAsync(id);
            if (traderGroup == null)
                return await AccessDenied();

            try
            {
                await _traderGroupService.DeleteTraderGroupAsync(traderGroup);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderGroups.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _traderGroupService.DeleteTraderGroupAsync((await _traderGroupService.GetTraderGroupsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderGroups.Errors.TryToDelete");
            }
        }
    }
}