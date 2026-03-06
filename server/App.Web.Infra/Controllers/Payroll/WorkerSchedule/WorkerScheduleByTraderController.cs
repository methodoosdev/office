using App.Core;
using App.Core.Domain.Logging;
using App.Core.Domain.Payroll;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Payroll;
using App.Services.Common;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Payroll;
using App.Services.Security;
using App.Web.Infra.Factories.Payroll.WorkerSchedules;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.WorkerSchedule
{
    public partial class WorkerScheduleByTraderController : WorkerScheduleByController
    {
        private readonly IWorkerScheduleService _workerScheduleService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkerScheduleByTraderModelFactory _workerScheduleByTraderModelFactory;
        private readonly IWorkerScheduleWorkerService _workerScheduleWorkerService;
        private readonly ISqlConnectionService _connectionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;

        public WorkerScheduleByTraderController(
            IWorkerScheduleService workerScheduleService,
            ILocalizationService localizationService,
            IWorkerScheduleByTraderModelFactory workerScheduleByTraderModelFactory,
            IWorkerScheduleWorkerService workerScheduleWorkerService,
            ISqlConnectionService connectionService,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext,
            IWorkerScheduleDateService workerScheduleDateService,
            IDateTimeHelper dateTimeHelper) : base(workerScheduleDateService, dateTimeHelper)
        {
            _workerScheduleService = workerScheduleService;
            _localizationService = localizationService;
            _workerScheduleByTraderModelFactory = workerScheduleByTraderModelFactory;
            _workerScheduleWorkerService = workerScheduleWorkerService;
            _connectionService = connectionService;
            _customerActivityService = customerActivityService;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _workerScheduleByTraderModelFactory.PrepareWorkerScheduleSearchModelAsync(new WorkerScheduleSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerScheduleSearchModel searchModel)
        {
            //prepare model
            var model = await _workerScheduleByTraderModelFactory.PrepareWorkerScheduleListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return BadRequest(result.Error);

            //prepare model
            var model = await _workerScheduleByTraderModelFactory.PrepareWorkerScheduleModelAsync(new WorkerScheduleModel(), null, trader, result.Connection);

            //prepare form
            var formModel = await _workerScheduleByTraderModelFactory.PrepareWorkerScheduleFormModelAsync(new WorkerScheduleFormModel(), false, result.Connection);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] WorkerScheduleModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
                    if (!result.Success)
                        return BadRequest(result.Error);

                    var workerScheduleId = await _workerScheduleByTraderModelFactory.CreateWorkerSchedule(model, result.Connection);

                    // Every create not once
                    await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.WorkerSchedule);

                    return Json(workerScheduleId);
                }
                catch
                {
                    return await BadRequestMessageAsync("Create WorkerSchedule failed.");
                }
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(id);
            if (workerSchedule == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !workerSchedule.TraderId.Equals(trader.Id))
                return await AccessDenied();

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return BadRequest(result.Error);

            //prepare model
            var model = await _workerScheduleByTraderModelFactory.PrepareWorkerScheduleModelAsync(null, workerSchedule, trader, result.Connection);

            //prepare form
            var formModel = await _workerScheduleByTraderModelFactory.PrepareWorkerScheduleFormModelAsync(new WorkerScheduleFormModel(), true, result.Connection);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] WorkerScheduleModel model)
        {
            //try to get entity with the specified id
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(model.Id);
            if (workerSchedule == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    workerSchedule = model.ToEntity(workerSchedule);
                    await _workerScheduleService.UpdateWorkerScheduleAsync(workerSchedule);

                    return Json(workerSchedule.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerSchedules.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(id);
            if (workerSchedule == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null || !workerSchedule.TraderId.Equals(trader.Id))
                return await AccessDenied();

            try
            {
                await UpdateWorkerScheduleDateAsync(workerSchedule.Id, trader.Id);

                var last = await _workerScheduleService.Table.Where(x => x.TraderId == trader.Id).OrderByDescending(o => o.Id).FirstAsync();
                if (workerSchedule.Id < last.Id)
                    return await BadRequestMessageAsync("App.Errors.FailedToDeleteOnlyLast");

                if (workerSchedule.WorkerScheduleModeTypeId == (int)WorkerScheduleModeType.Submited)
                    return await BadRequestMessageAsync("App.Errors.FailedWorkerScheduleDelete");

                await _workerScheduleService.DeleteWorkerScheduleAsync(workerSchedule);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerSchedules.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportToExcel(int parentId)
        {
            //try to get entity with the specified id
            var workerScheduleDates = await _workerScheduleDateService.GetAllWorkerScheduleDatesAsync(parentId);
            var employees = await _workerScheduleWorkerService.GetAllWorkerScheduleWorkersAsync(parentId);

            var workerIds = employees.GroupBy(x => x.WorkerId).Select(x => x.Key).ToList();
            var dateValueIds = workerScheduleDates.GroupBy(x => x.WorkingDate).Select(x => x.Key.ToString("dd/MM/yyyy")).ToList();
            var dateIds = workerScheduleDates.GroupBy(x => x.WorkingDate).Select(x => x.Key).ToList();

            var rows = new List<RowModel>();

            var headerCells = new List<CellModel>();
            headerCells.Add(new CellModel { Value = "Υπάλληλος" });
            foreach (var dateId in dateValueIds)
                headerCells.Add(new CellModel { Value = dateId });
            var headerRow = new RowModel { Cells = headerCells };
            rows.Add(headerRow);

            foreach (var workerId in workerIds)
            {
                var cells = new List<CellModel>();
                var cella = new CellModel();
                var worker = employees.First(x => x.WorkerId == workerId);
                cella.Value = $"{worker.WorkerCardName} {worker.WorkerName}";

                cells.Add(cella);
                foreach (var dateId in dateIds)
                {
                    var item = workerScheduleDates.First(x => x.WorkerId == workerId && x.WorkingDate == dateId);
                    var cell = new CellModel { Value = DailyName(item) };
                    cells.Add(cell);
                }
                var row = new RowModel { Cells = cells };
                rows.Add(row);
            }
            rows.Insert(0, new RowModel());
            rows.Insert(0, new RowModel());

            return Ok(rows);
        }

    }
}