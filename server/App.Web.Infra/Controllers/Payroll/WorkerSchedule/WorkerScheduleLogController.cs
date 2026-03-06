using App.Core.Domain.Payroll;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Payroll;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Payroll;
using App.Services.Security;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll.WorkerSchedules;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.WorkerSchedule
{
    public partial class WorkerScheduleLogController : BaseProtectController
    {
        private readonly IWorkerScheduleLogService _workerScheduleLogService;
        private readonly ILocalizationService _localizationService;
        private readonly IPersistStateService _persistStateService;
        private readonly IWorkerScheduleLogModelFactory _workerScheduleLogModelFactory;

        public WorkerScheduleLogController(
            IWorkerScheduleLogService workerScheduleLogService,
            ILocalizationService localizationService,
            IPersistStateService persistStateService,
            IWorkerScheduleLogModelFactory workerScheduleLogModelFactory)
        {
            _workerScheduleLogService = workerScheduleLogService;
            _localizationService = localizationService;
            _persistStateService = persistStateService;
            _workerScheduleLogModelFactory = workerScheduleLogModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _workerScheduleLogModelFactory.PrepareWorkerScheduleLogSearchModelAsync(new WorkerScheduleLogSearchModel());

            var filterModel = (await _persistStateService.GetModelInstance<WorkerScheduleLogFilterModel>()).Model;
            var filterFormModel = await _workerScheduleLogModelFactory.PrepareWorkerScheduleLogFilterFormModelAsync(new WorkerScheduleLogFilterFormModel());
            var filterDefaultModel = new WorkerScheduleLogFilterModel();

            return Json(new { searchModel, filterModel, filterFormModel, filterDefaultModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerScheduleLogSearchModel searchModel)
        {
            //prepare model
            var model = await _workerScheduleLogModelFactory.PrepareWorkerScheduleLogListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var workerScheduleLog = await _workerScheduleLogService.GetWorkerScheduleLogByIdAsync(id);
            if (workerScheduleLog == null)
                return await AccessDenied();

            //prepare model
            var model = await _workerScheduleLogModelFactory.PrepareWorkerScheduleLogModelAsync(null, workerScheduleLog);

            //prepare form
            var formModel = await _workerScheduleLogModelFactory.PrepareWorkerScheduleLogFormModelAsync(new WorkerScheduleLogFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] WorkerScheduleLogModel model)
        {
            //try to get entity with the specified id
            var workerScheduleLog = await _workerScheduleLogService.GetWorkerScheduleLogByIdAsync(model.Id);
            if (workerScheduleLog == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    workerScheduleLog = model.ToEntity(workerScheduleLog);
                    await _workerScheduleLogService.UpdateWorkerScheduleLogAsync(workerScheduleLog);

                    return Json(workerScheduleLog.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerScheduleLogs.Errors.TryToEdit");
            }
        }
    }
}