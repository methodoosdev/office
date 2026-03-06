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
    public partial class SpecialtyController : BaseProtectController
    {
        private readonly ISpecialtyService _specialtyService;
        private readonly ILocalizationService _localizationService;
        private readonly ISpecialtyModelFactory _specialtyModelFactory;

        public SpecialtyController(
            ISpecialtyService specialtyService,
            ILocalizationService localizationService,
            ISpecialtyModelFactory specialtyModelFactory)
        {
            _specialtyService = specialtyService;
            _localizationService = localizationService;
            _specialtyModelFactory = specialtyModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _specialtyModelFactory.PrepareSpecialtySearchModelAsync(new SpecialtySearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] SpecialtySearchModel searchModel)
        {
            //prepare model
            var model = await _specialtyModelFactory.PrepareSpecialtyListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _specialtyModelFactory.PrepareSpecialtyModelAsync(new SpecialtyModel(), null);

            //prepare form
            var formModel = await _specialtyModelFactory.PrepareSpecialtyFormModelAsync(new SpecialtyFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] SpecialtyModel model)
        {
            if (ModelState.IsValid)
            {
                var specialty = model.ToEntity<Specialty>();
                await _specialtyService.InsertSpecialtyAsync(specialty);

                return Json(specialty.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var specialty = await _specialtyService.GetSpecialtyByIdAsync(id);
            if (specialty == null)
                return await AccessDenied();

            //prepare model
            var model = await _specialtyModelFactory.PrepareSpecialtyModelAsync(null, specialty);

            //prepare form
            var formModel = await _specialtyModelFactory.PrepareSpecialtyFormModelAsync(new SpecialtyFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] SpecialtyModel model)
        {
            //try to get entity with the specified id
            var specialty = await _specialtyService.GetSpecialtyByIdAsync(model.Id);
            if (specialty == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    specialty = model.ToEntity(specialty);
                    await _specialtyService.UpdateSpecialtyAsync(specialty);

                    return Json(specialty.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Specialtys.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var specialty = await _specialtyService.GetSpecialtyByIdAsync(id);
            if (specialty == null)
                return await AccessDenied();

            try
            {
                await _specialtyService.DeleteSpecialtyAsync(specialty);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Specialtys.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _specialtyService.DeleteSpecialtyAsync((await _specialtyService.GetSpecialtiesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Specialtys.Errors.TryToDelete");
            }
        }
    }
}