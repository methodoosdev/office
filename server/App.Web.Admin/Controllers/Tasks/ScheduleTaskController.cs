using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Tasks;
using App.Services.Localization;
using App.Services.Messages;
using App.Services.ScheduleTasks;
using App.Web.Admin.Factories;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc;
using App.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Web.Admin.Controllers
{
    public partial class ScheduleTaskController : BaseProtectController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IScheduleTaskModelFactory _scheduleTaskModelFactory;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IScheduleTaskRunner _taskRunner;

        #endregion

        #region Ctor

        public ScheduleTaskController(
            ILocalizationService localizationService,
            //INotificationService notificationService,
            IScheduleTaskModelFactory scheduleTaskModelFactory,
            IScheduleTaskService scheduleTaskService,
            IScheduleTaskRunner taskRunner)
        {
            _localizationService = localizationService;
            //_notificationService = notificationService;
            _scheduleTaskModelFactory = scheduleTaskModelFactory;
            _scheduleTaskService = scheduleTaskService;
            _taskRunner = taskRunner;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scheduleTaskModelFactory.PrepareScheduleTaskSearchModelAsync(new ScheduleTaskSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScheduleTaskSearchModel searchModel)
        {
            //prepare model
            var model = await _scheduleTaskModelFactory.PrepareScheduleTaskListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(id);
            if (scheduleTask == null)
                return await AccessDenied();

            //prepare model
            var model = await _scheduleTaskModelFactory.PrepareScheduleTaskModelAsync(null, scheduleTask);

            //prepare form
            var formModel = await _scheduleTaskModelFactory.PrepareScheduleTaskFormModelAsync(new ScheduleTaskFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IActionResult> Edit([FromBody] ScheduleTaskModel model)
        {
            //try to get a schedule task with the specified id
            var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(model.Id)
                               ?? throw new ArgumentException("Schedule task cannot be loaded");

            //To prevent inject the XSS payload in Schedule tasks ('Name' field), we must disable editing this field, 
            //but since it is required, we need to get its value before updating the entity.
            if (!string.IsNullOrEmpty(scheduleTask.Name))
            {
                model.Name = scheduleTask.Name;
                ModelState.Remove(nameof(model.Name));
            }

            if (!ModelState.IsValid)
                return BadRequestFromModel();

            scheduleTask = model.ToEntity(scheduleTask);

            await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

            return Json(scheduleTask.Id);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TaskUpdate([FromBody] ScheduleTaskModel model)
        {
            //try to get a schedule task with the specified id
            var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(model.Id)
                               ?? throw new ArgumentException("Schedule task cannot be loaded");

            //To prevent inject the XSS payload in Schedule tasks ('Name' field), we must disable editing this field, 
            //but since it is required, we need to get its value before updating the entity.
            if (!string.IsNullOrEmpty(scheduleTask.Name))
            {
                model.Name = scheduleTask.Name;
                ModelState.Remove(nameof(model.Name));
            }

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            if (!scheduleTask.Enabled && model.Enabled)
                scheduleTask.LastEnabledUtc = DateTime.UtcNow;

            scheduleTask = model.ToEntity(scheduleTask);

            await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> RunNow(int id)
        {
            try
            {
                //try to get a schedule task with the specified id
                var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(id)
                                   ?? throw new ArgumentException("Schedule task cannot be loaded", nameof(id));

                await _taskRunner.ExecuteAsync(scheduleTask, true, true, false);

                //_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.ScheduleTasks.RunNow.Done"));
            }
            catch //(Exception exc)
            {
                //await _notificationService.ErrorNotificationAsync(exc);
            }

            //return RedirectToAction("List");
            return Ok();
        }

        #endregion
    }
}