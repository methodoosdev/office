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
    public partial class TraderRelationshipController : BaseProtectController
    {
        private readonly ITraderRelationshipService _traderRelationshipService;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderRelationshipModelFactory _traderRelationshipModelFactory;

        public TraderRelationshipController(
            ITraderRelationshipService traderRelationshipService,
            ILocalizationService localizationService,
            ITraderRelationshipModelFactory traderRelationshipModelFactory)
        {
            _traderRelationshipService = traderRelationshipService;
            _localizationService = localizationService;
            _traderRelationshipModelFactory = traderRelationshipModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderRelationshipModelFactory.PrepareTraderRelationshipSearchModelAsync(new TraderRelationshipSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderRelationshipSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _traderRelationshipModelFactory.PrepareTraderRelationshipListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _traderRelationshipModelFactory.PrepareTraderRelationshipModelAsync(new TraderRelationshipModel(), null);

            //prepare form
            var formModel = await _traderRelationshipModelFactory.PrepareTraderRelationshipFormModelAsync(new TraderRelationshipFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TraderRelationshipModel model, int parentId)
        {
            if (ModelState.IsValid)
            {
                var traderRelationship = model.ToEntity<TraderRelationship>();
                traderRelationship.TraderId = parentId;
                await _traderRelationshipService.InsertTraderRelationshipAsync(traderRelationship);

                return Json(traderRelationship.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var traderRelationship = await _traderRelationshipService.GetTraderRelationshipByIdAsync(id);
            if (traderRelationship == null)
                return await AccessDenied();

            //prepare model
            var model = await _traderRelationshipModelFactory.PrepareTraderRelationshipModelAsync(null, traderRelationship);

            //prepare form
            var formModel = await _traderRelationshipModelFactory.PrepareTraderRelationshipFormModelAsync(new TraderRelationshipFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] TraderRelationshipModel model)
        {
            //try to get entity with the specified id
            var traderRelationship = await _traderRelationshipService.GetTraderRelationshipByIdAsync(model.Id);
            if (traderRelationship == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    traderRelationship = model.ToEntity(traderRelationship);
                    await _traderRelationshipService.UpdateTraderRelationshipAsync(traderRelationship);

                    return Json(traderRelationship.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderRelationships.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var traderRelationship = await _traderRelationshipService.GetTraderRelationshipByIdAsync(id);
            if (traderRelationship == null)
                return await AccessDenied();

            try
            {
                await _traderRelationshipService.DeleteTraderRelationshipAsync(traderRelationship);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderRelationships.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _traderRelationshipService.DeleteTraderRelationshipAsync((await _traderRelationshipService.GetTraderRelationshipsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderRelationships.Errors.TryToDelete");
            }
        }
    }
}