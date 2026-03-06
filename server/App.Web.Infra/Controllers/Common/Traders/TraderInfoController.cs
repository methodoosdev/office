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
    public partial class TraderInfoController : BaseProtectController
    {
        private readonly ITraderInfoService _traderInfoService;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderInfoModelFactory _traderInfoModelFactory;

        public TraderInfoController(
            ITraderInfoService traderInfoService,
            ILocalizationService localizationService,
            ITraderInfoModelFactory traderInfoModelFactory)
        {
            _traderInfoService = traderInfoService;
            _localizationService = localizationService;
            _traderInfoModelFactory = traderInfoModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderInfoModelFactory.PrepareTraderInfoSearchModelAsync(new TraderInfoSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderInfoSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _traderInfoModelFactory.PrepareTraderInfoListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _traderInfoModelFactory.PrepareTraderInfoModelAsync(new TraderInfoModel(), null);

            //prepare form
            var formModel = await _traderInfoModelFactory.PrepareTraderInfoFormModelAsync(new TraderInfoFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TraderInfoModel model, int parentId)
        {
            if (ModelState.IsValid)
            {
                var traderInfo = model.ToEntity<TraderInfo>();
                traderInfo.TraderId = parentId;
                await _traderInfoService.InsertTraderInfoAsync(traderInfo);

                return Json(traderInfo.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var traderInfo = await _traderInfoService.GetTraderInfoByIdAsync(id);
            if (traderInfo == null)
                return await AccessDenied();

            //prepare model
            var model = await _traderInfoModelFactory.PrepareTraderInfoModelAsync(null, traderInfo);

            //prepare form
            var formModel = await _traderInfoModelFactory.PrepareTraderInfoFormModelAsync(new TraderInfoFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] TraderInfoModel model)
        {
            //try to get entity with the specified id
            var traderInfo = await _traderInfoService.GetTraderInfoByIdAsync(model.Id);
            if (traderInfo == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    traderInfo = model.ToEntity(traderInfo);
                    await _traderInfoService.UpdateTraderInfoAsync(traderInfo);

                    return Json(traderInfo.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderInfos.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var traderInfo = await _traderInfoService.GetTraderInfoByIdAsync(id);
            if (traderInfo == null)
                return await AccessDenied();

            try
            {
                await _traderInfoService.DeleteTraderInfoAsync(traderInfo);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderInfos.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _traderInfoService.DeleteTraderInfoAsync((await _traderInfoService.GetTraderInfosByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TraderInfos.Errors.TryToDelete");
            }
        }
    }
}