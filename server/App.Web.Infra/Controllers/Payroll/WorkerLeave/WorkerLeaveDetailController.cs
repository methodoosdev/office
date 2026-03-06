using App.Core.Domain.Payroll;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Payroll;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Payroll;
using App.Services.Security;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll.WorkerLeave;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.WorkerLeave
{
    public partial class WorkerLeaveDetailController : BaseProtectController
    {
        private readonly IWorkerLeaveDetailService _workerLeaveDetailService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkerLeaveDetailModelFactory _workerLeaveDetailModelFactory;

        public WorkerLeaveDetailController(
            IWorkerLeaveDetailService workerLeaveDetailService,
            ILocalizationService localizationService,
            IWorkerLeaveDetailModelFactory workerLeaveDetailModelFactory)
        {
            _workerLeaveDetailService = workerLeaveDetailService;
            _localizationService = localizationService;
            _workerLeaveDetailModelFactory = workerLeaveDetailModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _workerLeaveDetailModelFactory.PrepareWorkerLeaveDetailSearchModelAsync(new WorkerLeaveDetailSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerLeaveDetailSearchModel searchModel)
        {
            //prepare model
            var model = await _workerLeaveDetailModelFactory.PrepareWorkerLeaveDetailListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _workerLeaveDetailModelFactory.PrepareWorkerLeaveDetailModelAsync(new WorkerLeaveDetailModel(), null);

            //prepare form
            var formModel = await _workerLeaveDetailModelFactory.PrepareWorkerLeaveDetailFormModelAsync(new WorkerLeaveDetailFormModel(), 0);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] WorkerLeaveDetailModel model)
        {
            if (ModelState.IsValid)
            {
                var workerLeaveDetail = model.ToEntity<WorkerLeaveDetail>();
                await _workerLeaveDetailService.InsertWorkerLeaveDetailAsync(workerLeaveDetail);

                return Json(workerLeaveDetail.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var workerLeaveDetail = await _workerLeaveDetailService.GetWorkerLeaveDetailByIdAsync(id);
            if (workerLeaveDetail == null)
                return await AccessDenied();

            //prepare model
            var model = await _workerLeaveDetailModelFactory.PrepareWorkerLeaveDetailModelAsync(null, workerLeaveDetail);

            //prepare form
            var formModel = await _workerLeaveDetailModelFactory.PrepareWorkerLeaveDetailFormModelAsync(new WorkerLeaveDetailFormModel(), model.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] WorkerLeaveDetailModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //try to get entity with the specified id
                    var workerLeaveDetail = await _workerLeaveDetailService.GetWorkerLeaveDetailByIdAsync(model.Id);
                    if (workerLeaveDetail == null)
                        return await AccessDenied();

                    workerLeaveDetail = model.ToEntity(workerLeaveDetail);
                    await _workerLeaveDetailService.UpdateWorkerLeaveDetailAsync(workerLeaveDetail);

                    return Json(workerLeaveDetail.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerLeaveDetails.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var workerLeaveDetail = await _workerLeaveDetailService.GetWorkerLeaveDetailByIdAsync(id);
            if (workerLeaveDetail == null)
                return await AccessDenied();

            try
            {
                await _workerLeaveDetailService.DeleteWorkerLeaveDetailAsync(workerLeaveDetail);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerLeaveDetails.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _workerLeaveDetailService.DeleteWorkerLeaveDetailAsync((await _workerLeaveDetailService.GetWorkerLeaveDetailsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerLeaveDetails.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetWorkers(int traderId)
        {
            var list = await _workerLeaveDetailModelFactory.GetWorkersByHyperPayrollIdAsync(traderId);

            return Json(list);
        }

    }
}