using App.Core;
using App.Core.Domain.VatExemption;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.VatExemption;
using App.Services.Localization;
using App.Services.VatExemption;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.VatExemption;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.VatExemption
{
    public partial class VatExemptionApprovalController : BaseProtectController
    {
        private readonly IVatExemptionApprovalService _vatExemptionApprovalService;
        private readonly IVatExemptionReportService _vatExemptionReportService;
        private readonly IVatExemptionSerialService _vatExemptionSerialService;
        private readonly ILocalizationService _localizationService;
        private readonly IVatExemptionApprovalModelFactory _vatExemptionApprovalModelFactory;
        private readonly IWorkContext _workContext;

        public VatExemptionApprovalController(
            IVatExemptionApprovalService vatExemptionApprovalService,
            IVatExemptionReportService vatExemptionReportService,
            IVatExemptionSerialService vatExemptionSerialService,
            ILocalizationService localizationService,
            IVatExemptionApprovalModelFactory vatExemptionApprovalModelFactory,
            IWorkContext workContext)
        {
            _vatExemptionApprovalService = vatExemptionApprovalService;
            _vatExemptionReportService = vatExemptionReportService;
            _vatExemptionSerialService = vatExemptionSerialService;
            _localizationService = localizationService;
            _vatExemptionApprovalModelFactory = vatExemptionApprovalModelFactory;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _vatExemptionApprovalModelFactory.PrepareVatExemptionApprovalSearchModelAsync(new VatExemptionApprovalSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] VatExemptionApprovalSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionApprovalModelFactory.PrepareVatExemptionApprovalListModelAsync(searchModel, trader);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionApprovalModelFactory.PrepareVatExemptionApprovalModelAsync(new VatExemptionApprovalModel(), null, trader.Id);

            //prepare form
            var formModel = await _vatExemptionApprovalModelFactory.PrepareVatExemptionApprovalFormModelAsync(new VatExemptionApprovalFormModel(), false);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] VatExemptionApprovalModel model)
        {
            if (ModelState.IsValid)
            {
                var vatExemptionApproval = model.ToEntity<VatExemptionApproval>();
                await _vatExemptionApprovalService.InsertVatExemptionApprovalAsync(vatExemptionApproval);

                return Json(vatExemptionApproval.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var vatExemptionApproval = await _vatExemptionApprovalService.GetVatExemptionApprovalByIdAsync(id);
            if (vatExemptionApproval == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionApproval.TraderId))
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionApprovalModelFactory.PrepareVatExemptionApprovalModelAsync(null, vatExemptionApproval, trader.Id);

            //prepare form
            var formModel = await _vatExemptionApprovalModelFactory.PrepareVatExemptionApprovalFormModelAsync(new VatExemptionApprovalFormModel(), true);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] VatExemptionApprovalModel model)
        {
            //try to get entity with the specified id
            var vatExemptionApproval = await _vatExemptionApprovalService.GetVatExemptionApprovalByIdAsync(model.Id);
            if (vatExemptionApproval == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    vatExemptionApproval = model.ToEntity(vatExemptionApproval);
                    await _vatExemptionApprovalService.UpdateVatExemptionApprovalAsync(vatExemptionApproval);

                    return Json(vatExemptionApproval.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionApprovals.Errors.TryToEdit");
            }
        }

        private async Task<bool> CanDeleteAsync(int parentId)
        {
            var vatExemptionReport = await _vatExemptionReportService.Table
                .FirstOrDefaultAsync(x => x.VatExemptionApprovalId == parentId);

            var vatExemptionSerial = await _vatExemptionSerialService.Table
                .FirstOrDefaultAsync(x => x.VatExemptionApprovalId == parentId);

            return vatExemptionReport == null && vatExemptionSerial == null;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var vatExemptionApproval = await _vatExemptionApprovalService.GetVatExemptionApprovalByIdAsync(id);
            if (vatExemptionApproval == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionApproval.TraderId))
                return await AccessDenied();

            if (!await CanDeleteAsync(vatExemptionApproval.Id))
                return await BadRequestMessageAsync("App.Errors.FailedCascadeDelete");

            try
            {
                await _vatExemptionApprovalService.DeleteVatExemptionApprovalAsync(vatExemptionApproval);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionApprovals.Errors.TryToDelete");
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
                var vatExemptionApprovals = (await _vatExemptionApprovalService.GetVatExemptionApprovalsByIdsAsync(selectedIds.ToArray())).ToList();

                foreach (var vatExemptionApproval in vatExemptionApprovals)
                {
                    if (trader.Id.Equals(vatExemptionApproval.TraderId))
                    {
                        if (!await CanDeleteAsync(vatExemptionApproval.Id))
                            return await BadRequestMessageAsync("App.Errors.FailedCascadeDelete");
                        else
                            await _vatExemptionApprovalService.DeleteVatExemptionApprovalAsync(vatExemptionApproval);
                    }
                    else
                        return await AccessDenied();
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionApprovals.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> SetPrimary(int id)
        {
            //try to get a customer role with the specified id
            var vatExemptionApproval = await _vatExemptionApprovalService.GetVatExemptionApprovalByIdAsync(id);
            if (vatExemptionApproval == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionApproval.TraderId))
                return await AccessDenied();

            // Mark all records as not primary
            var vatExemptionApprovals = await _vatExemptionApprovalService.GetAllVatExemptionApprovalsAsync(trader.Id);

            foreach (var item in vatExemptionApprovals)
                item.Active = false;

            await _vatExemptionApprovalService.UpdateVatExemptionApprovalAsync(vatExemptionApprovals);

            // Mark record to primary
            vatExemptionApproval = await _vatExemptionApprovalService.GetVatExemptionApprovalByIdAsync(id);
            vatExemptionApproval.Active = true;

            await _vatExemptionApprovalService.UpdateVatExemptionApprovalAsync(vatExemptionApproval);

            return Ok();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [CheckCustomerPermission(true)]
        public virtual IActionResult ModelStatus([FromBody] VatExemptionApprovalModel model)
        {
            if (ModelState.IsValid)
                return Ok();

            return BadRequestFromModel();
        }

    }
}