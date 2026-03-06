using App.Core;
using App.Core.Domain.Logging;
using App.Core.Domain.VatExemption;
using App.Data;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.VatExemption;
using App.Services.Common;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Offices;
using App.Services.VatExemption;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Traders;
using App.Web.Infra.Factories.Common.VatExemption;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.VatExemption
{
    public partial class VatExemptionDocController : BaseProtectController
    {
        private readonly IRepository<VatExemptionApproval> _vatExemptionApprovalRepository;
        private readonly IRepository<VatExemptionSerial> _vatExemptionSerialRepository;
        private readonly IVatExemptionSerialService _vatExemptionSerialService;
        private readonly IVatExemptionDocService _vatExemptionDocService;
        private readonly ILocalizationService _localizationService;
        private readonly IVatExemptionDocModelFactory _vatExemptionDocModelFactory;
        private readonly IBusinessRegistryModelFactory _businessRegistryModelFactory;
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;
        private readonly IHtmlToPdfService _htmlToPdfService;

        public VatExemptionDocController(
            IRepository<VatExemptionApproval> vatExemptionApprovalRepository,
            IRepository<VatExemptionSerial> vatExemptionSerialRepository,
            IVatExemptionSerialService vatExemptionSerialService,
            IVatExemptionDocService vatExemptionDocService,
            ILocalizationService localizationService,
            IVatExemptionDocModelFactory vatExemptionDocModelFactory,
            IBusinessRegistryModelFactory businessRegistryModelFactory,
            IAccountingOfficeService accountingOfficeService,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext,
            IHtmlToPdfService htmlToPdfService)
        {
            _vatExemptionApprovalRepository = vatExemptionApprovalRepository;
            _vatExemptionSerialRepository = vatExemptionSerialRepository;
            _vatExemptionSerialService = vatExemptionSerialService;
            _vatExemptionDocService = vatExemptionDocService;
            _localizationService = localizationService;
            _vatExemptionDocModelFactory = vatExemptionDocModelFactory;
            _businessRegistryModelFactory = businessRegistryModelFactory;
            _accountingOfficeService = accountingOfficeService;
            _customerActivityService = customerActivityService;
            _workContext = workContext;
            _htmlToPdfService = htmlToPdfService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _vatExemptionDocModelFactory.PrepareVatExemptionDocSearchModelAsync(new VatExemptionDocSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] VatExemptionDocSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _vatExemptionDocModelFactory.PrepareVatExemptionDocListModelAsync(searchModel, trader);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            var vatExemptionApproval = await _vatExemptionApprovalRepository.Table
                .Where(w => w.TraderId == trader.Id)
                .FirstOrDefaultAsync(x => x.Active == true);

            if (vatExemptionApproval == null)
                return await BadRequestMessageAsync("App.Models.VatExemptionDocModel.Errors.WrongVatExemptionApproval");

            var vatExemptionSerial = await _vatExemptionSerialRepository.Table
                .FirstOrDefaultAsync(x => x.VatExemptionApprovalId == vatExemptionApproval.Id);
            if (vatExemptionSerial == null)
                return await BadRequestMessageAsync("App.Models.VatExemptionDocModel.Errors.WrongVatExemptionSerial");

            //prepare model
            var model = await _vatExemptionDocModelFactory.PrepareVatExemptionDocModelAsync(new VatExemptionDocModel(), null, trader, vatExemptionSerial);

            //prepare form
            var formModel = await _vatExemptionDocModelFactory.PrepareVatExemptionDocFormModelAsync(new VatExemptionDocFormModel(), false, trader.Id, vatExemptionSerial.VatExemptionApprovalId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] VatExemptionDocModel model)
        {
            if (ModelState.IsValid)
            {
                var vatExemptionDoc = model.ToEntity<VatExemptionDoc>();
                await _vatExemptionDocService.InsertVatExemptionDocAsync(vatExemptionDoc);

                await UpdateVatExemptionSerialAsync(vatExemptionDoc.VatExemptionSerialId, 1);

                // Every create not once
                await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.VatExemptionDoc);

                return Json(vatExemptionDoc.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var vatExemptionDoc = await _vatExemptionDocService.GetVatExemptionDocByIdAsync(id);
            if (vatExemptionDoc == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionDoc.TraderId))
                return await AccessDenied();

            var vatExemptionSerial = await _vatExemptionSerialService.GetVatExemptionSerialByIdAsync(vatExemptionDoc.VatExemptionSerialId);

            //prepare model
            var model = await _vatExemptionDocModelFactory.PrepareVatExemptionDocModelAsync(null, vatExemptionDoc, trader, vatExemptionSerial);

            //prepare form
            var formModel = await _vatExemptionDocModelFactory.PrepareVatExemptionDocFormModelAsync(new VatExemptionDocFormModel(), true, trader.Id, vatExemptionSerial.VatExemptionApprovalId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] VatExemptionDocModel model)
        {
            //try to get entity with the specified id
            var vatExemptionDoc = await _vatExemptionDocService.GetVatExemptionDocByIdAsync(model.Id);
            if (vatExemptionDoc == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    vatExemptionDoc = model.ToEntity(vatExemptionDoc);
                    await _vatExemptionDocService.UpdateVatExemptionDocAsync(vatExemptionDoc);

                    return Json(vatExemptionDoc.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionDocs.Errors.TryToEdit");
            }
        }

        private async Task UpdateVatExemptionSerialAsync(int vatExemptionSerialId, int value)
        {
            var vatExemptionSerial = await _vatExemptionSerialService.GetVatExemptionSerialByIdAsync(vatExemptionSerialId);

            vatExemptionSerial.SerialNo = vatExemptionSerial.SerialNo + value;
            await _vatExemptionSerialService.UpdateVatExemptionSerialAsync(vatExemptionSerial);
        }

        private async Task<bool> CanDeleteAsync(int traderId, int id)
        {
            var vatExemptionDoc = await _vatExemptionDocService.GetLastVatExemptionDocAsync(traderId);

            return vatExemptionDoc.Id == id;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var vatExemptionDoc = await _vatExemptionDocService.GetVatExemptionDocByIdAsync(id);
            if (vatExemptionDoc == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionDoc.TraderId))
                return await AccessDenied();

            if (!await CanDeleteAsync(trader.Id, vatExemptionDoc.Id))
                return await BadRequestMessageAsync("App.Errors.FailedCascadeDelete");

            try
            {
                var vatExemptionSerialId = vatExemptionDoc.VatExemptionSerialId;
                await _vatExemptionDocService.DeleteVatExemptionDocAsync(vatExemptionDoc);

                await UpdateVatExemptionSerialAsync(vatExemptionSerialId, -1);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionDocs.Errors.TryToDelete");
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
                var vatExemptionDocs = (await _vatExemptionDocService.GetVatExemptionDocsByIdsAsync(selectedIds.ToArray())).ToList();

                foreach (var vatExemptionDoc in vatExemptionDocs)
                {
                    if (trader.Id.Equals(vatExemptionDoc.TraderId))
                    {
                        if (!await CanDeleteAsync(trader.Id, vatExemptionDoc.SerialNo))
                            return await BadRequestMessageAsync("App.Errors.FailedCascadeDelete");
                        else
                        {
                            var vatExemptionSerialId = vatExemptionDoc.VatExemptionSerialId;
                            await _vatExemptionDocService.DeleteVatExemptionDocAsync(vatExemptionDoc);
                            await UpdateVatExemptionSerialAsync(vatExemptionSerialId, -1);
                        }
                    }
                    else
                        return await AccessDenied();
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.VatExemptionDocs.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> SerialNoChanged([FromBody] VatExemptionDocModel model)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(model.TraderId))
                return await AccessDenied();

            model = await _vatExemptionDocModelFactory.PrepareVatExemptionDocSerialChangedAsync(model);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ModelChanged([FromBody] VatExemptionDocModel model)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(model.TraderId))
                return await AccessDenied();

            model = await _vatExemptionDocModelFactory.PrepareVatExemptionDocChangedAsync(model);

            return Json(model);
        }

        public virtual async Task<IActionResult> GetSupplierInfo(string afmCalledFor)
        {
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();

            var model = _businessRegistryModelFactory.GetDocumentModel(office.AadeRegistryUsername, office.AadeRegistryPassword, afmCalledFor);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf(int parentId)
        {
            //try to get entity with the specified id
            var vatExemptionDoc = await _vatExemptionDocService.GetVatExemptionDocByIdAsync(parentId);
            if (vatExemptionDoc == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !trader.Id.Equals(vatExemptionDoc.TraderId))
                return await AccessDenied();

            try
            {
                var model = vatExemptionDoc.ToModel<VatExemptionDocModel>();

                var template = "~/Views/Pdf/VatExemptionDoc.cshtml";

                byte[] bytes = await _htmlToPdfService.PrintToPdf(model, template, null);
                return File(bytes, MimeTypes.ApplicationPdf);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var errorMessage = string.Format(await _localizationService.GetResourceAsync("App.Errors.FailedToCreateFile"), "pdf");
                return BadRequest(errorMessage);
            }
        }

    }
}