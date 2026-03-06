using App.Core.Domain.Logging;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Payroll;
using App.Services.Common;
using App.Services.Logging;
using App.Services.Offices;
using App.Services.Payroll;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Messages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll
{
    public partial class ApdTekaController : BaseProtectController
    {
        private readonly IApdTekaService _apdTekaService;
        private readonly IApdTekaModelFactory _apdTekaModelFactory;
        private readonly IPersistStateService _persistStateService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ISqlConnectionService _connectionService;

        public ApdTekaController(
            IApdTekaService apdTekaService,
            IApdTekaModelFactory apdTekaModelFactory,
            IPersistStateService persistStateService,
            ICustomerActivityService customerActivityService,
            ISqlConnectionService connectionService)
        {
            _apdTekaService = apdTekaService;
            _apdTekaModelFactory = apdTekaModelFactory;
            _persistStateService = persistStateService;
            _customerActivityService = customerActivityService;
            _connectionService = connectionService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _apdTekaModelFactory.PrepareApdTekaSearchModelAsync(new ApdTekaSearchModel());

            var filterModel = (await _persistStateService.GetModelInstance<ApdTekaFilterModel>()).Model;
            var filterFormModel = await _apdTekaModelFactory.PrepareApdTekaFilterFormModelAsync(new ApdTekaFilterFormModel());
            var filterDefaultModel = new ApdTekaFilterModel();

            return Json(new { searchModel, filterModel, filterFormModel, filterDefaultModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ApdTekaSearchModel searchModel)
        {
            //prepare model
            var model = await _apdTekaModelFactory.PrepareApdTekaListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var apdTeka = await _apdTekaService.GetApdTekaByIdAsync(id);
            if (apdTeka == null)
                return await AccessDenied();

            //prepare model
            var model = await _apdTekaModelFactory.PrepareApdTekaModelAsync(null, apdTeka);

            //prepare form
            var formModel = await _apdTekaModelFactory.PrepareApdTekaFormModelAsync(new ApdTekaFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ApdTekaModel model)
        {
            //try to get entity with the specified id
            var apdTeka = await _apdTekaService.GetApdTekaByIdAsync(model.Id);
            if (apdTeka == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    apdTeka = model.ToEntity(apdTeka);
                    await _apdTekaService.UpdateApdTekaAsync(apdTeka);

                    return Json(apdTeka.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ApdTekas.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var apdTeka = await _apdTekaService.GetApdTekaByIdAsync(id);
            if (apdTeka == null)
                return await AccessDenied();

            try
            {
                await _apdTekaService.DeleteApdTekaAsync(apdTeka);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ApdTekas.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _apdTekaService.DeleteApdTekaAsync((await _apdTekaService.GetApdTekasByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ApdTekas.Errors.TryToDelete");
            }
        }

        public virtual async Task<IActionResult> SelectCompanyPeriodDialog()
        {
            //prepare form
            var formModel = await _apdTekaModelFactory.PrepareApdTekaDialogFormModelAsync(new ApdTekaDialogFormModel());

            return Json(formModel);

        }

        [HttpPost]
        public virtual async Task<IActionResult> CreatePeriod(int year, int period)
        {
            await _apdTekaModelFactory.PrepareCreatePeriodAsync(year, period);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> PayrollStatus([FromBody] ICollection<int> selectedIds)
        {
            if (selectedIds == null)
                return await AccessDenied();

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            await _apdTekaModelFactory.PreparePayrollStatusAsync(selectedIds, result.Connection);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApdSubmit([FromBody] ICollection<int> selectedIds)
        {
            if (selectedIds == null)
                return await AccessDenied();

            var custActivity = await _apdTekaModelFactory.PrepareApdSubmitAsync(selectedIds);

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.ApdSubmission, custActivity.ToString());

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> TekaSubmit([FromBody] ICollection<int> selectedIds)
        {
            if (selectedIds == null)
                return await AccessDenied();

            var custActivity = await _apdTekaModelFactory.PrepareTekaSubmitAsync(selectedIds);

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.ApdSubmission, custActivity.ToString());

            return Ok();
        }

    }
}