using App.Core.Domain.Payroll;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Payroll;
using App.Services.Common;
using App.Services.Helpers;
using App.Services.Offices;
using App.Services.Payroll;
using App.Web.Infra.Factories.Payroll.WorkerSchedules;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.WorkerSchedule
{
    public partial class WorkerScheduleByEmployeeController : WorkerScheduleByController
    {
        private readonly IWorkerScheduleService _workerScheduleService;
        private readonly IWorkerScheduleWorkerService _workerScheduleWorkerService;
        private readonly IWorkerScheduleByEmployeeModelFactory _workerScheduleByEmployeeModelFactory;
        private readonly ISqlConnectionService _connectionService;
        private readonly IPersistStateService _persistStateService;

        public WorkerScheduleByEmployeeController(
            IWorkerScheduleDateService workerScheduleDateService,
            IWorkerScheduleService workerScheduleService,
            IWorkerScheduleWorkerService workerScheduleWorkerService,
            IWorkerScheduleByEmployeeModelFactory workerScheduleByEmployeeModelFactory,
            IDateTimeHelper dateTimeHelper,
            ISqlConnectionService connectionService,
            IPersistStateService persistStateService) : base(workerScheduleDateService, dateTimeHelper)
        {
            _workerScheduleService = workerScheduleService;
            _workerScheduleWorkerService = workerScheduleWorkerService;
            _workerScheduleByEmployeeModelFactory = workerScheduleByEmployeeModelFactory;
            _connectionService = connectionService;
            _persistStateService = persistStateService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _workerScheduleByEmployeeModelFactory.PrepareWorkerScheduleSearchModelAsync(new WorkerScheduleSearchModel());

            var filterModel = (await _persistStateService.GetModelInstance<WorkerScheduleFilterModel>()).Model;
            var filterFormModel = await _workerScheduleByEmployeeModelFactory.PrepareWorkerScheduleFilterFormModelAsync(new WorkerScheduleFilterFormModel());
            var filterDefaultModel = new WorkerScheduleFilterModel();

            return Json(new { searchModel, filterModel, filterFormModel, filterDefaultModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerScheduleSearchModel searchModel)
        {
            //prepare model
            var model = await _workerScheduleByEmployeeModelFactory.PrepareWorkerScheduleListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(id);
            if (workerSchedule == null)
                return await AccessDenied();

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return BadRequest(result.Error);

            //prepare model
            var model = await _workerScheduleByEmployeeModelFactory.PrepareWorkerScheduleModelAsync(null, workerSchedule, result.Connection, workerSchedule.TraderId);

            //prepare form
            var formModel = await _workerScheduleByEmployeeModelFactory.PrepareWorkerScheduleFormModelAsync(new WorkerScheduleFormModel());

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
                return await BadRequestMessageAsync("App.Models.WorkerSchedule.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(id);
            if (workerSchedule == null)
                return await AccessDenied();

            try
            {
                await UpdateWorkerScheduleDateAsync(workerSchedule.Id, workerSchedule.TraderId);

                var last = await _workerScheduleService.Table.Where(x => x.TraderId == workerSchedule.TraderId).OrderByDescending(o => o.Id).FirstAsync();
                if (workerSchedule.Id < last.Id)
                    return await BadRequestMessageAsync("App.Errors.FailedToDeleteOnlyLast");

                if (workerSchedule.WorkerScheduleModeTypeId == (int)WorkerScheduleModeType.Submited)
                    return await BadRequestMessageAsync("App.Errors.FailedWorkerScheduleDelete");

                await _workerScheduleService.DeleteWorkerScheduleAsync(workerSchedule);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerSchedule.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> SetModeType(int id, int typeId)
        {
            //try to get a customer role with the specified id
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(id);
            if (workerSchedule == null)
                return await AccessDenied();

            workerSchedule.WorkerScheduleModeTypeId = typeId; // Waiting = 1, Submited = 2 Υποβληθηκε, Canceled = 4

            await _workerScheduleService.UpdateWorkerScheduleAsync(workerSchedule);

            return Ok();
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
    public class RowModel
    {
        public List<CellModel> Cells { get; set; }
    }
    public class CellModel
    {
        public string Value { get; set; }
    }
}