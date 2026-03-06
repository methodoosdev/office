using App.Core.Domain.Employees;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Employees;
using App.Services.Employees;
using App.Services.Localization;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Employees;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Employees
{
    public partial class EducationController : BaseProtectController
    {
        private readonly IEducationService _educationService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IEducationModelFactory _educationModelFactory;

        public EducationController(
            IEducationService educationService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IEducationModelFactory educationModelFactory)
        {
            _educationService = educationService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _educationModelFactory = educationModelFactory;
        }

        protected virtual async Task UpdateLocalesAsync(Education education, EducationModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(education,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);
            }
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _educationModelFactory.PrepareEducationSearchModelAsync(new EducationSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] EducationSearchModel searchModel)
        {
            //prepare model
            var model = await _educationModelFactory.PrepareEducationListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _educationModelFactory.PrepareEducationModelAsync(new EducationModel(), null);

            //prepare form
            var formModel = await _educationModelFactory.PrepareEducationFormModelAsync(new EducationFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] EducationModel model)
        {
            if (ModelState.IsValid)
            {
                var education = model.ToEntity<Education>();
                await _educationService.InsertEducationAsync(education);

                //locales
                await UpdateLocalesAsync(education, model);

                return Json(education.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var education = await _educationService.GetEducationByIdAsync(id);
            if (education == null)
                return await AccessDenied();

            //prepare model
            var model = await _educationModelFactory.PrepareEducationModelAsync(null, education);

            //prepare form
            var formModel = await _educationModelFactory.PrepareEducationFormModelAsync(new EducationFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] EducationModel model)
        {
            //try to get entity with the specified id
            var education = await _educationService.GetEducationByIdAsync(model.Id);
            if (education == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    education = model.ToEntity(education);
                    await _educationService.UpdateEducationAsync(education);

                    //locales
                    await UpdateLocalesAsync(education, model);

                    return Json(education.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Educations.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var education = await _educationService.GetEducationByIdAsync(id);
            if (education == null)
                return await AccessDenied();

            try
            {
                await _educationService.DeleteEducationAsync(education);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Educations.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _educationService.DeleteEducationAsync((await _educationService.GetEducationsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Educations.Errors.TryToDelete");
            }
        }
    }
}