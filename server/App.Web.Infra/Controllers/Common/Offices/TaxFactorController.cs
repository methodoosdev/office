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
    public partial class TaxFactorController : BaseProtectController
    {
        private readonly ITaxFactorService _taxFactorService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxFactorModelFactory _taxFactorModelFactory;

        public TaxFactorController(
           ITaxFactorService taxFactorService,
           ILocalizationService localizationService,
           ITaxFactorModelFactory taxFactorModelFactory)
        {
            _taxFactorService = taxFactorService;
            _localizationService = localizationService;
            _taxFactorModelFactory = taxFactorModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _taxFactorModelFactory.PrepareTaxFactorSearchModelAsync(new TaxFactorSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TaxFactorSearchModel searchModel)
        {
            //prepare model
            var model = await _taxFactorModelFactory.PrepareTaxFactorListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _taxFactorModelFactory.PrepareTaxFactorModelAsync(new TaxFactorModel(), null);

            //prepare form
            var formModel = await _taxFactorModelFactory.PrepareTaxFactorFormModelAsync(new TaxFactorFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TaxFactorModel model)
        {
            if (ModelState.IsValid)
            {
                var taxFactor = model.ToEntity<TaxFactor>();
                await _taxFactorService.InsertTaxFactorAsync(taxFactor);

                return Json(taxFactor.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var taxFactor = await _taxFactorService.GetTaxFactorByIdAsync(id);
            if (taxFactor == null)
                return await AccessDenied();

            //prepare model
            var model = await _taxFactorModelFactory.PrepareTaxFactorModelAsync(null, taxFactor);

            //prepare form
            var formModel = await _taxFactorModelFactory.PrepareTaxFactorFormModelAsync(new TaxFactorFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] TaxFactorModel model)
        {
            //try to get entity with the specified id
            var taxFactor = await _taxFactorService.GetTaxFactorByIdAsync(model.Id);
            if (taxFactor == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    taxFactor = model.ToEntity(taxFactor);
                    await _taxFactorService.UpdateTaxFactorAsync(taxFactor);

                    return Json(taxFactor.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TaxFactor.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var taxFactor = await _taxFactorService.GetTaxFactorByIdAsync(id);
            if (taxFactor == null)
                return await AccessDenied();

            try
            {
                await _taxFactorService.DeleteTaxFactorAsync(taxFactor);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TaxFactors.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _taxFactorService.DeleteTaxFactorAsync((await _taxFactorService.GetTaxFactorsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.TaxFactors.Errors.TryToDelete");
            }
        }

    }
}
