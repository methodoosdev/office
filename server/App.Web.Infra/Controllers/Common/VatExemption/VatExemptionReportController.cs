using App.Core;
using App.Core.Domain.VatExemption;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.VatExemption;
using App.Services.Localization;
using App.Services.VatExemption;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.VatExemption;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.VatExemption
{
    public partial class VatExemptionReportController : BaseProtectController
    {
        private readonly IVatExemptionReportService _vatExemptionReportService;
        private readonly ILocalizationService _localizationService;
        private readonly IVatExemptionReportModelFactory _vatExemptionReportModelFactory;
        private readonly IWorkContext _workContext;

        public VatExemptionReportController(
            IVatExemptionReportService vatExemptionReportService,
            ILocalizationService localizationService,
            IVatExemptionReportModelFactory vatExemptionReportModelFactory,
            IWorkContext workContext)
        {
            _vatExemptionReportService = vatExemptionReportService;
            _localizationService = localizationService;
            _vatExemptionReportModelFactory = vatExemptionReportModelFactory;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _vatExemptionReportModelFactory.PrepareVatExemptionReportSearchModelAsync(new VatExemptionReportSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] VatExemptionReportSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionReportModelFactory.PrepareVatExemptionReportListModelAsync(searchModel, trader);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionReportModelFactory.PrepareVatExemptionReportModelAsync(new VatExemptionReportModel(), null, trader.Id);

            //prepare form
            var formModel = await _vatExemptionReportModelFactory.PrepareVatExemptionReportFormModelAsync(new VatExemptionReportFormModel(), trader.Id);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] VatExemptionReportModel model)
        {
            if (ModelState.IsValid)
            {
                var vatExemptionReport = model.ToEntity<VatExemptionReport>();
                await _vatExemptionReportService.InsertVatExemptionReportAsync(vatExemptionReport);

                return Json(vatExemptionReport.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var vatExemptionReport = await _vatExemptionReportService.GetVatExemptionReportByIdAsync(id);
            if (vatExemptionReport == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionReport.TraderId))
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionReportModelFactory.PrepareVatExemptionReportModelAsync(null, vatExemptionReport, trader.Id);

            //prepare form
            var formModel = await _vatExemptionReportModelFactory.PrepareVatExemptionReportFormModelAsync(new VatExemptionReportFormModel(), trader.Id);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] VatExemptionReportModel model)
        {
            //try to get entity with the specified id
            var vatExemptionReport = await _vatExemptionReportService.GetVatExemptionReportByIdAsync(model.Id);
            if (vatExemptionReport == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    vatExemptionReport = model.ToEntity(vatExemptionReport);
                    await _vatExemptionReportService.UpdateVatExemptionReportAsync(vatExemptionReport);

                    return Json(vatExemptionReport.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionReports.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var vatExemptionReport = await _vatExemptionReportService.GetVatExemptionReportByIdAsync(id);
            if (vatExemptionReport == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionReport.TraderId))
                return await AccessDenied();

            try
            {
                await _vatExemptionReportService.DeleteVatExemptionReportAsync(vatExemptionReport);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionReports.Errors.TryToDelete");
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
                var vatExemptionReports = (await _vatExemptionReportService.GetVatExemptionReportsByIdsAsync(selectedIds.ToArray())).ToList();

                foreach (var vatExemptionReport in vatExemptionReports)
                {
                    if (trader.Id.Equals(vatExemptionReport.TraderId))
                        await _vatExemptionReportService.DeleteVatExemptionReportAsync(vatExemptionReport);
                    else
                        return await AccessDenied();
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionReports.Errors.TryToDelete");
            }
        }
    }
}