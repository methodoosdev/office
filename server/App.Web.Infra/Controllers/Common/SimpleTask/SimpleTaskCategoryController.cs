using App.Core.Domain.SimpleTask;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.SimpleTask;
using App.Services.Localization;
using App.Services.SimpleTask;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.SimpleTask;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.SimpleTask
{
    public partial class SimpleTaskCategoryController : BaseProtectController
    {
        private readonly ISimpleTaskCategoryService _simpleTaskCategoryService;
        private readonly ILocalizationService _localizationService;
        private readonly ISimpleTaskCategoryModelFactory _simpleTaskCategoryModelFactory;

        public SimpleTaskCategoryController(
            ISimpleTaskCategoryService simpleTaskCategoryService,
            ILocalizationService localizationService,
            ISimpleTaskCategoryModelFactory simpleTaskCategoryModelFactory)
        {
            _simpleTaskCategoryService = simpleTaskCategoryService;
            _localizationService = localizationService;
            _simpleTaskCategoryModelFactory = simpleTaskCategoryModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _simpleTaskCategoryModelFactory.PrepareSimpleTaskCategorySearchModelAsync(new SimpleTaskCategorySearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] SimpleTaskCategorySearchModel searchModel)
        {
            //prepare model
            var model = await _simpleTaskCategoryModelFactory.PrepareSimpleTaskCategoryListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _simpleTaskCategoryModelFactory.PrepareSimpleTaskCategoryModelAsync(new SimpleTaskCategoryModel(), null);

            //prepare form
            var formModel = await _simpleTaskCategoryModelFactory.PrepareSimpleTaskCategoryFormModelAsync(new SimpleTaskCategoryFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] SimpleTaskCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var simpleTaskCategory = model.ToEntity<SimpleTaskCategory>();
                await _simpleTaskCategoryService.InsertSimpleTaskCategoryAsync(simpleTaskCategory);

                return Json(simpleTaskCategory.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var simpleTaskCategory = await _simpleTaskCategoryService.GetSimpleTaskCategoryByIdAsync(id);
            if (simpleTaskCategory == null)
                return await AccessDenied();

            //prepare model
            var model = await _simpleTaskCategoryModelFactory.PrepareSimpleTaskCategoryModelAsync(null, simpleTaskCategory);

            //prepare form
            var formModel = await _simpleTaskCategoryModelFactory.PrepareSimpleTaskCategoryFormModelAsync(new SimpleTaskCategoryFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] SimpleTaskCategoryModel model)
        {
            //try to get entity with the specified id
            var simpleTaskCategory = await _simpleTaskCategoryService.GetSimpleTaskCategoryByIdAsync(model.Id);
            if (simpleTaskCategory == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    simpleTaskCategory = model.ToEntity(simpleTaskCategory);
                    await _simpleTaskCategoryService.UpdateSimpleTaskCategoryAsync(simpleTaskCategory);

                    return Json(simpleTaskCategory.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskCategorys.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var simpleTaskCategory = await _simpleTaskCategoryService.GetSimpleTaskCategoryByIdAsync(id);
            if (simpleTaskCategory == null)
                return await AccessDenied();

            try
            {
                await _simpleTaskCategoryService.DeleteSimpleTaskCategoryAsync(simpleTaskCategory);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskCategorys.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _simpleTaskCategoryService.DeleteSimpleTaskCategoryAsync((await _simpleTaskCategoryService.GetSimpleTaskCategoriesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskCategorys.Errors.TryToDelete");
            }
        }
    }
}