using App.Core;
using App.Core.Domain.VatExemption;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.VatExemption;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.VatExemption;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.VatExemption;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.VatExemption
{
    public partial class VatExemptionSerialController : BaseProtectController
    {
        private readonly IVatExemptionDocService _vatExemptionDocService;
        private readonly IVatExemptionSerialService _vatExemptionSerialService;
        private readonly ILocalizationService _localizationService;
        private readonly IVatExemptionSerialModelFactory _vatExemptionSerialModelFactory;
        private readonly IWorkContext _workContext;

        public VatExemptionSerialController(
            IVatExemptionDocService vatExemptionDocService,
            IVatExemptionSerialService vatExemptionSerialService,
            ILocalizationService localizationService,
            IVatExemptionSerialModelFactory vatExemptionSerialModelFactory,
            IWorkContext workContext)
        {
            _vatExemptionDocService = vatExemptionDocService;
            _vatExemptionSerialService = vatExemptionSerialService;
            _localizationService = localizationService;
            _vatExemptionSerialModelFactory = vatExemptionSerialModelFactory;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _vatExemptionSerialModelFactory.PrepareVatExemptionSerialSearchModelAsync(new VatExemptionSerialSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] VatExemptionSerialSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionSerialModelFactory.PrepareVatExemptionSerialListModelAsync(searchModel, trader);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionSerialModelFactory.PrepareVatExemptionSerialModelAsync(new VatExemptionSerialModel(), null, trader.Id);

            //prepare form
            var formModel = await _vatExemptionSerialModelFactory.PrepareVatExemptionSerialFormModelAsync(new VatExemptionSerialFormModel(), trader.Id, false);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] VatExemptionSerialModel model)
        {
            if (ModelState.IsValid)
            {
                var vatExemptionSerial = model.ToEntity<VatExemptionSerial>();
                await _vatExemptionSerialService.InsertVatExemptionSerialAsync(vatExemptionSerial);

                return Json(vatExemptionSerial.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var vatExemptionSerial = await _vatExemptionSerialService.GetVatExemptionSerialByIdAsync(id);
            if (vatExemptionSerial == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionSerial.TraderId))
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionSerialModelFactory.PrepareVatExemptionSerialModelAsync(null, vatExemptionSerial, trader.Id);

            //prepare form
            var formModel = await _vatExemptionSerialModelFactory.PrepareVatExemptionSerialFormModelAsync(new VatExemptionSerialFormModel(), trader.Id, true);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] VatExemptionSerialModel model)
        {
            //try to get entity with the specified id
            var vatExemptionSerial = await _vatExemptionSerialService.GetVatExemptionSerialByIdAsync(model.Id);
            if (vatExemptionSerial == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    vatExemptionSerial = model.ToEntity(vatExemptionSerial);
                    await _vatExemptionSerialService.UpdateVatExemptionSerialAsync(vatExemptionSerial);

                    return Json(vatExemptionSerial.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionSerials.Errors.TryToEdit");
            }
        }

        private async Task<bool> CanDeleteAsync(int parentId)
        {
            var vatExemptionDoc = await _vatExemptionDocService.Table
                .FirstOrDefaultAsync(x => x.VatExemptionSerialId == parentId);

            return vatExemptionDoc == null;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var vatExemptionSerial = await _vatExemptionSerialService.GetVatExemptionSerialByIdAsync(id);
            if (vatExemptionSerial == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionSerial.TraderId))
                return await AccessDenied();

            if (!await CanDeleteAsync(vatExemptionSerial.Id))
                return await BadRequestMessageAsync("App.Errors.FailedCascadeDelete");

            try
            {
                await _vatExemptionSerialService.DeleteVatExemptionSerialAsync(vatExemptionSerial);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionSerials.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            try
            {
                var vatExemptionSerials = (await _vatExemptionSerialService.GetVatExemptionSerialsByIdsAsync(selectedIds.ToArray())).ToList();

                foreach (var vatExemptionSerial in vatExemptionSerials)
                {
                    if (trader.Id.Equals(vatExemptionSerial.TraderId))
                    {
                        if (!await CanDeleteAsync(vatExemptionSerial.Id))
                            return await BadRequestMessageAsync("App.Errors.FailedCascadeDelete");
                        else
                            await _vatExemptionSerialService.DeleteVatExemptionSerialAsync(vatExemptionSerial);
                    }
                    else
                        return await AccessDenied();
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionSerials.Errors.TryToDelete");
            }
        }
    }
}