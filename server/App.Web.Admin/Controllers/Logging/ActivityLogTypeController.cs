using App.Core.Domain.Logging;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Logging;
using App.Services.Localization;
using App.Services.Logging;
using App.Web.Admin.Factories.Logging;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Admin.Controllers
{
    public partial class ActivityLogTypeController : BaseProtectController
    {
        private readonly IActivityLogTypeService _activityLogTypeService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogTypeModelFactory _activityLogTypeModelFactory;

        public ActivityLogTypeController(
            IActivityLogTypeService activityLogTypeService,
            ILocalizationService localizationService,
            IActivityLogTypeModelFactory activityLogTypeModelFactory)
        {
            _activityLogTypeService = activityLogTypeService;
            _localizationService = localizationService;
            _activityLogTypeModelFactory = activityLogTypeModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _activityLogTypeModelFactory.PrepareActivityLogTypeSearchModelAsync(new ActivityLogTypeSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ActivityLogTypeSearchModel searchModel)
        {
            //prepare model
            var model = await _activityLogTypeModelFactory.PrepareActivityLogTypeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _activityLogTypeModelFactory.PrepareActivityLogTypeModelAsync(new ActivityLogTypeModel(), null);

            //prepare form
            var formModel = await _activityLogTypeModelFactory.PrepareActivityLogTypeFormModelAsync(new ActivityLogTypeFormModel(), false);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ActivityLogTypeModel model)
        {
            if (ModelState.IsValid)
            {
                var activityLogType = model.ToEntity<ActivityLogType>();
                await _activityLogTypeService.InsertActivityLogTypeAsync(activityLogType);

                return Json(activityLogType.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var activityLogType = await _activityLogTypeService.GetActivityLogTypeByIdAsync(id);
            if (activityLogType == null)
                return await AccessDenied();

            //prepare model
            var model = await _activityLogTypeModelFactory.PrepareActivityLogTypeModelAsync(null, activityLogType);

            //prepare form
            var formModel = await _activityLogTypeModelFactory.PrepareActivityLogTypeFormModelAsync(new ActivityLogTypeFormModel(), true);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ActivityLogTypeModel model)
        {
            //try to get entity with the specified id
            var activityLogType = await _activityLogTypeService.GetActivityLogTypeByIdAsync(model.Id);
            if (activityLogType == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    activityLogType = model.ToEntity(activityLogType);
                    await _activityLogTypeService.UpdateActivityLogTypeAsync(activityLogType);

                    return Json(activityLogType.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ActivityLogTypes.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var activityLogType = await _activityLogTypeService.GetActivityLogTypeByIdAsync(id);
            if (activityLogType == null)
                return await AccessDenied();

            try
            {
                await _activityLogTypeService.DeleteActivityLogTypeAsync(activityLogType);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ActivityLogTypes.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _activityLogTypeService.DeleteActivityLogTypeAsync((await _activityLogTypeService.GetActivityLogTypesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ActivityLogTypes.Errors.TryToDelete");
            }
        }
    }
}