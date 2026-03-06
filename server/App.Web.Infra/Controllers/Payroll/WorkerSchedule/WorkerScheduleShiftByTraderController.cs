using App.Core;
using App.Core.Domain.Payroll;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Payroll;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Payroll;
using App.Services.Security;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll.WorkerSchedules;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.WorkerSchedule
{
    public partial class WorkerScheduleShiftByTraderController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IWorkerScheduleShiftService _WorkerScheduleShiftService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkerScheduleShiftByTraderModelFactory _WorkerScheduleShiftByTraderModelFactory;
        private readonly IWorkContext _workContext;

        public WorkerScheduleShiftByTraderController(ITraderService traderService,
            IWorkerScheduleShiftService WorkerScheduleShiftService,
            ILocalizationService localizationService,
            IWorkerScheduleShiftByTraderModelFactory WorkerScheduleShiftByTraderModelFactory,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _WorkerScheduleShiftService = WorkerScheduleShiftService;
            _localizationService = localizationService;
            _WorkerScheduleShiftByTraderModelFactory = WorkerScheduleShiftByTraderModelFactory;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var searchModel = await _WorkerScheduleShiftByTraderModelFactory.PrepareWorkerScheduleShiftSearchModelAsync(new WorkerScheduleShiftSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerScheduleShiftSearchModel searchModel)
        {
            //prepare model
            var model = await _WorkerScheduleShiftByTraderModelFactory.PrepareWorkerScheduleShiftListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _WorkerScheduleShiftByTraderModelFactory.PrepareWorkerScheduleShiftModelAsync(new WorkerScheduleShiftModel(), null, trader.Id);

            //prepare form
            var formModel = await _WorkerScheduleShiftByTraderModelFactory.PrepareWorkerScheduleShiftFormModelAsync(new WorkerScheduleShiftFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] WorkerScheduleShiftModel model)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            if (ModelState.IsValid)
            {
                var errorMessage = await _WorkerScheduleShiftByTraderModelFactory.PrepareWorkerScheduleShiftModelValidationAsync(model, trader.EmployerBreakLimit);
                if (errorMessage == null)
                {
                    var WorkerScheduleShift = model.ToEntity<WorkerScheduleShift>();
                    await _WorkerScheduleShiftService.InsertWorkerScheduleShiftAsync(WorkerScheduleShift);

                    return Json(WorkerScheduleShift.Id);
                }
                return BadRequest(errorMessage);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var WorkerScheduleShift = await _WorkerScheduleShiftService.GetWorkerScheduleShiftByIdAsync(id);
            if (WorkerScheduleShift == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !WorkerScheduleShift.TraderId.Equals(trader.Id))
                return await AccessDenied();

            //prepare model
            var model = await _WorkerScheduleShiftByTraderModelFactory.PrepareWorkerScheduleShiftModelAsync(null, WorkerScheduleShift, trader.Id);

            //prepare form
            var formModel = await _WorkerScheduleShiftByTraderModelFactory.PrepareWorkerScheduleShiftFormModelAsync(new WorkerScheduleShiftFormModel());
            formModel.CustomProperties.Add("breakLimit", trader.EmployerBreakLimit);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] WorkerScheduleShiftModel model)
        {
            //try to get entity with the specified id
            var WorkerScheduleShift = await _WorkerScheduleShiftService.GetWorkerScheduleShiftByIdAsync(model.Id);
            if (WorkerScheduleShift == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !WorkerScheduleShift.TraderId.Equals(trader.Id))
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    var errorMessage = await _WorkerScheduleShiftByTraderModelFactory.PrepareWorkerScheduleShiftModelValidationAsync(model, trader.EmployerBreakLimit);
                    if (errorMessage == null)
                    {
                        WorkerScheduleShift = model.ToEntity(WorkerScheduleShift);
                        await _WorkerScheduleShiftService.UpdateWorkerScheduleShiftAsync(WorkerScheduleShift);

                        return Json(WorkerScheduleShift.Id);
                    }

                    return BadRequest(errorMessage);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerScheduleShift.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var workerScheduleShift = await _WorkerScheduleShiftService.GetWorkerScheduleShiftByIdAsync(id);
            if (workerScheduleShift == null)
                return await AccessDenied();

            try
            {
                await _WorkerScheduleShiftService.DeleteWorkerScheduleShiftAsync(workerScheduleShift);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerScheduleShift.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _WorkerScheduleShiftService.DeleteWorkerScheduleShiftAsync((await _WorkerScheduleShiftService.GetWorkerScheduleShiftsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerScheduleShift.Errors.TryToDelete");
            }
        }
    }
}