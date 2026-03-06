using App.Core;
using App.Core.Domain.Employees;
using App.Core.Domain.Messages;
using App.Core.Domain.Payroll;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Common;
using App.Models.Payroll;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Employees;
using App.Services.Helpers;
using App.Services.Hubs;
using App.Services.Localization;
using App.Services.Messages;
using App.Services.Payroll;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll.WorkerSchedules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.WorkerSchedule
{
    public partial class WorkerScheduleSubmitController : BaseProtectController
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEmployeeService _employeeService;
        private readonly IEmailSender _emailSender;
        private readonly ITraderService _traderService;
        private readonly IWorkerScheduleService _workerScheduleService;
        private readonly IWorkerScheduleDateService _workerScheduleDateService;
        private readonly IWorkerScheduleShiftService _workerScheduleShiftService;
        private readonly IWorkerScheduleSubmitModelFactory _workerScheduleSubmitModelFactory;
        private readonly IWorkerScheduleCheckModelFactory _workerScheduleCheckModelFactory;
        private readonly IWorkerScheduleLogService _workerScheduleLogService;
        private readonly ISqlConnectionService _connectionService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public WorkerScheduleSubmitController(
            IHubContext<ChatHub> hub,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            IEmployeeService employeeService,
            ITraderService traderService,
            IWorkerScheduleService workerScheduleService,
            IWorkerScheduleDateService workerScheduleDateService,
            IWorkerScheduleShiftService workerScheduleShiftService,
            IWorkerScheduleSubmitModelFactory workerScheduleSubmitModelFactory,
            IWorkerScheduleCheckModelFactory workerScheduleCheckModelFactory,
            IWorkerScheduleLogService workerScheduleLogService,
            ISqlConnectionService connectionService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _hub = hub;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _employeeService = employeeService;
            _emailSender = emailSender;
            _traderService = traderService;
            _workerScheduleService = workerScheduleService;
            _workerScheduleDateService = workerScheduleDateService;
            _workerScheduleShiftService = workerScheduleShiftService;
            _workerScheduleSubmitModelFactory = workerScheduleSubmitModelFactory;
            _workerScheduleCheckModelFactory = workerScheduleCheckModelFactory;
            _workerScheduleLogService = workerScheduleLogService;
            _connectionService = connectionService;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(id);
            if (workerSchedule == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            //if true some trader edit other trader record
            if (trader != null && !workerSchedule.TraderId.Equals(trader.Id))
                return await AccessDenied();

            var isTrader = trader != null;
            if (trader == null)
            {
                trader = await _traderService.GetTraderByIdAsync(workerSchedule.TraderId);
            }

            //prepare model
            var model = await _workerScheduleSubmitModelFactory.PrepareWorkerScheduleSubmitModelAsync(new WorkerScheduleSubmitModel());
            model.BreakLimit = trader.EmployerBreakLimit;
            model.TraderName = trader.ToTraderFullName();
            model.CanEdit = !(workerSchedule.WorkerScheduleModeTypeId == (int)WorkerScheduleModeType.Submited);
            model.Shifts = await _workerScheduleShiftService.GetAllWorkerScheduleShiftsAsync(trader.Id);
            model.IsTrader = isTrader;

            //prepare form
            var data = await _workerScheduleSubmitModelFactory.PrepareWorkerScheduleSubmitDataModelAsync(workerSchedule.Id);

            return Json(new { model, data });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] WorkerScheduleDateModel model)
        {
            //try to get entity with the specified id
            var workerScheduleDate = await _workerScheduleDateService.GetWorkerScheduleDateByIdAsync(model.Id);
            if (workerScheduleDate == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    workerScheduleDate = model.ToEntity(workerScheduleDate);
                    await _workerScheduleDateService.UpdateWorkerScheduleDateAsync(workerScheduleDate);

                    return Json(workerScheduleDate.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                //await _logger.ErrorAsync($"Failed to edit Department: `{department.Description}`.", exc);
                return await BadRequestMessageAsync("App.Models.WorkerScheduleDateModel.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Update([FromBody] IList<WorkerScheduleDateModel> models)
        {
            try
            {
                var workerScheduleDates = new List<WorkerScheduleDate>();
                foreach (var model in models)
                {
                    var workerScheduleDate = await _workerScheduleDateService.GetWorkerScheduleDateByIdAsync(model.Id);
                    workerScheduleDate = model.ToEntity(workerScheduleDate);
                    workerScheduleDates.Add(workerScheduleDate);
                }

                await _workerScheduleDateService.UpdateWorkerScheduleDateAsync(workerScheduleDates);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkerScheduleDateModel.Errors.TryToUpdate");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Check(int parentId)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return BadRequest(result.Error);

            var resultList = await _workerScheduleCheckModelFactory.PrepareWorkerScheduleCheckListModelAsync(result.Connection, parentId);
            if (resultList.Any(x => x.MaxFortyHoursPerWeekError))
            {
                var error = await _localizationService.GetResourceAsync("App.Errors.MaxFortyHoursPerWeekError");
                return Json(new { valid = false, error });
            }

            if (resultList.Any(x => x.MaxSixDaysPerWeekError))
            {
                var error = await _localizationService.GetResourceAsync("App.Errors.MaxSixDaysPerWeekError");
                return Json(new { valid = false, error });
            }

            if (resultList.Any(x => x.ContractChangeError))
            {
                var error = await _localizationService.GetResourceAsync("App.Errors.ContractChangeError");
                var workerNames = resultList.Where(x => x.ContractChangeError).Select(k => k.WorkerName).ToList();
                var workers = string.Join(",", workerNames);

                return Json(new { valid = false, error, contractChange = true, workers });
            }

            return Json(new { valid = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendEmail(int parentId)
        {
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(parentId);
            if (workerSchedule == null)
                return await AccessDenied();

            var trader = await _traderService.GetTraderByIdAsync(workerSchedule.TraderId);

            var defaultEmailAccountId = _emailAccountSettings.DefaultEmailAccountId;
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(defaultEmailAccountId);

            if (emailAccount == null)
                return await BadRequestMessageAsync("App.Errors.FailedToSendEmail");

            var employeeList = await _employeeService.GetEmployeesByDepartmentSystemNameAsync(DepartmentDefaults.PayrollName);

            var employees = employeeList.Where(x => x.PayrollInfoEmail && CommonHelper.IsValidEmail(x.EmailContact)).ToList();

            var employeeEmails = employees.Select(x => x.EmailContact).ToList();
            var address = employeeEmails.FirstOrDefault();
            if (address == null)
                return await BadRequestMessageAsync("App.Errors.FailedToSendEmail");

            employeeEmails.Remove(address);
            try
            {
                // send email to payroll team
                var subject = trader.ToTraderFullName() + $": Υποβολή προγράμματος {workerSchedule.Id}.";
                var body = subject;
                await _emailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, address, null, cc: employeeEmails);

                // insert or update workerScheduleLog
                var workerScheduleLog = await _workerScheduleLogService.Table.Where(x => x.WorkerScheduleId == workerSchedule.Id).FirstOrDefaultAsync();
                if (workerScheduleLog == null)
                {
                    var log = new WorkerScheduleLog
                    {
                        WorkerScheduleId = workerSchedule.Id,
                        TraderId = workerSchedule.TraderId,
                        Period = $"{workerSchedule.PeriodFromDate.ToString("dd/MM/yyyy")} - {workerSchedule.PeriodToDate.ToString("dd/MM/yyyy")}",
                        SubmitDate = DateTime.UtcNow
                    };
                    await _workerScheduleLogService.InsertWorkerScheduleLogAsync(log);
                }
                else
                {
                    workerScheduleLog.Notes = null;
                    workerScheduleLog.SubmitDate = DateTime.UtcNow;
                    await _workerScheduleLogService.UpdateWorkerScheduleLogAsync(workerScheduleLog);
                }

                // send inner message to payroll employers
                await _hub.Clients.Group(WorkerScheduleDefaults.PayrollGroupName).SendAsync("workerScheduleSignal", subject);

                return Ok();
            }
            catch { }

            return await BadRequestMessageAsync("App.Errors.FailedToSendEmail");
        }
    }
}