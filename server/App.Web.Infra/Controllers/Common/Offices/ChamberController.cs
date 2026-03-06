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
    public partial class ChamberController : BaseProtectController
    {
        private readonly IChamberService _chamberService;
        private readonly ILocalizationService _localizationService;
        private readonly IChamberModelFactory _chamberModelFactory;

        public ChamberController(
            IChamberService chamberService,
            ILocalizationService localizationService,
            IChamberModelFactory chamberModelFactory,
            ILogger logger,
            IPermissionService permissionService)
        {
            _chamberService = chamberService;
            _localizationService = localizationService;
            _chamberModelFactory = chamberModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _chamberModelFactory.PrepareChamberSearchModelAsync(new ChamberSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ChamberSearchModel searchModel)
        {
            //prepare model
            var model = await _chamberModelFactory.PrepareChamberListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _chamberModelFactory.PrepareChamberModelAsync(new ChamberModel(), null);

            //prepare form
            var formModel = await _chamberModelFactory.PrepareChamberFormModelAsync(new ChamberFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ChamberModel model)
        {
            if (ModelState.IsValid)
            {
                var chamber = model.ToEntity<Chamber>();
                await _chamberService.InsertChamberAsync(chamber);

                return Json(chamber.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var chamber = await _chamberService.GetChamberByIdAsync(id);
            if (chamber == null)
                return await AccessDenied();

            //prepare model
            var model = await _chamberModelFactory.PrepareChamberModelAsync(null, chamber);

            //prepare form
            var formModel = await _chamberModelFactory.PrepareChamberFormModelAsync(new ChamberFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ChamberModel model)
        {
            //try to get entity with the specified id
            var chamber = await _chamberService.GetChamberByIdAsync(model.Id);
            if (chamber == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    chamber = model.ToEntity(chamber);
                    await _chamberService.UpdateChamberAsync(chamber);

                    return Json(chamber.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Chambers.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var chamber = await _chamberService.GetChamberByIdAsync(id);
            if (chamber == null)
                return await AccessDenied();

            try
            {
                await _chamberService.DeleteChamberAsync(chamber);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Chambers.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _chamberService.DeleteChamberAsync((await _chamberService.GetChambersByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Chambers.Errors.TryToDelete");
            }
        }
    }
}