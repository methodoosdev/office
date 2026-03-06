using App.Core.Domain.Employees;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Employees;
using App.Services.Employees;
using App.Services.Localization;
using App.Services.Security;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Employees;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Employees
{
    public partial class JobTitleController : BaseProtectController
    {
        private readonly IJobTitleService _jobTitleService;
        private readonly ILocalizationService _localizationService;
        private readonly IJobTitleModelFactory _jobTitleModelFactory;

        public JobTitleController(
            IJobTitleService jobTitleService,
            ILocalizationService localizationService,
            IJobTitleModelFactory jobTitleModelFactory)
        {
            _jobTitleService = jobTitleService;
            _localizationService = localizationService;
            _jobTitleModelFactory = jobTitleModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _jobTitleModelFactory.PrepareJobTitleSearchModelAsync(new JobTitleSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] JobTitleSearchModel searchModel)
        {
            //prepare model
            var model = await _jobTitleModelFactory.PrepareJobTitleListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _jobTitleModelFactory.PrepareJobTitleModelAsync(new JobTitleModel(), null);

            //prepare form
            var formModel = await _jobTitleModelFactory.PrepareJobTitleFormModelAsync(new JobTitleFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] JobTitleModel model)
        {
            if (ModelState.IsValid)
            {
                var jobTitle = model.ToEntity<JobTitle>();
                await _jobTitleService.InsertJobTitleAsync(jobTitle);

                return Json(jobTitle.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var jobTitle = await _jobTitleService.GetJobTitleByIdAsync(id);
            if (jobTitle == null)
                return await AccessDenied();

            //prepare model
            var model = await _jobTitleModelFactory.PrepareJobTitleModelAsync(null, jobTitle);

            //prepare form
            var formModel = await _jobTitleModelFactory.PrepareJobTitleFormModelAsync(new JobTitleFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] JobTitleModel model)
        {
            //try to get entity with the specified id
            var jobTitle = await _jobTitleService.GetJobTitleByIdAsync(model.Id);
            if (jobTitle == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    jobTitle = model.ToEntity(jobTitle);
                    await _jobTitleService.UpdateJobTitleAsync(jobTitle);

                    return Json(jobTitle.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.JobTitles.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var jobTitle = await _jobTitleService.GetJobTitleByIdAsync(id);
            if (jobTitle == null)
                return await AccessDenied();

            try
            {
                await _jobTitleService.DeleteJobTitleAsync(jobTitle);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.JobTitles.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _jobTitleService.DeleteJobTitleAsync((await _jobTitleService.GetJobTitlesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.JobTitles.Errors.TryToDelete");
            }
        }
    }
}