using App.Core.Domain.Messages;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Messages;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Messages;
using App.Web.Admin.Factories.Messages;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Admin.Controllers.Messages
{
    public partial class QueuedEmailController : BaseProtectController
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IQueuedEmailModelFactory _queuedEmailModelFactory;
        private readonly IQueuedEmailService _queuedEmailService;

        public QueuedEmailController(IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IQueuedEmailModelFactory queuedEmailModelFactory,
            IQueuedEmailService queuedEmailService)
        {
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _queuedEmailModelFactory = queuedEmailModelFactory;
            _queuedEmailService = queuedEmailService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _queuedEmailModelFactory.PrepareQueuedEmailSearchModelAsync(new QueuedEmailSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] QueuedEmailSearchModel searchModel)
        {
            //prepare model
            var model = await _queuedEmailModelFactory.PrepareQueuedEmailListModelAsync(searchModel);

            return Json(model);
        }

        //[HttpPost, ActionName("List")]
        //[FormValueRequired("go-to-email-by-number")]
        //public virtual async Task<IActionResult> GoToEmailByNumber(QueuedEmailSearchModel model)
        //{
        //    //try to get a queued email with the specified id
        //    var queuedEmail = await _queuedEmailService.GetQueuedEmailByIdAsync(model.GoDirectlyToNumber);
        //    if (queuedEmail == null)
        //        return await List();

        //    return RedirectToAction("Edit", "QueuedEmail", new { id = queuedEmail.Id });
        //}

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get a queued email with the specified id
            var email = await _queuedEmailService.GetQueuedEmailByIdAsync(id);
            if (email == null)
                return await AccessDenied();

            //prepare model
            var model = await _queuedEmailModelFactory.PrepareQueuedEmailModelAsync(null, email);

            //prepare form
            var formModel = await _queuedEmailModelFactory.PrepareQueuedEmailFormModelAsync(new QueuedEmailFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] QueuedEmailModel model)
        {
            //try to get a queued email with the specified id
            var email = await _queuedEmailService.GetQueuedEmailByIdAsync(model.Id);
            if (email == null)
                return await AccessDenied();

            if (ModelState.IsValid)
            {
                email = model.ToEntity(email);
                //email.DontSendBeforeDateUtc = model.SendImmediately || !model.DontSendBeforeDate.HasValue ?
                //    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value);
                await _queuedEmailService.UpdateQueuedEmailAsync(email);

                return Json(email.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Requeue([FromBody] QueuedEmailModel queuedEmailModel)
        {
            //try to get a queued email with the specified id
            var queuedEmail = await _queuedEmailService.GetQueuedEmailByIdAsync(queuedEmailModel.Id);
            if (queuedEmail == null)
                return RedirectToAction("List");

            var requeuedEmail = new QueuedEmail
            {
                PriorityId = queuedEmail.PriorityId,
                From = queuedEmail.From,
                FromName = queuedEmail.FromName,
                To = queuedEmail.To,
                ToName = queuedEmail.ToName,
                ReplyTo = queuedEmail.ReplyTo,
                ReplyToName = queuedEmail.ReplyToName,
                CC = queuedEmail.CC,
                Bcc = queuedEmail.Bcc,
                Subject = queuedEmail.Subject,
                Body = queuedEmail.Body,
                AttachmentFilePath = queuedEmail.AttachmentFilePath,
                AttachmentFileName = queuedEmail.AttachmentFileName,
                AttachedDownloadId = queuedEmail.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = queuedEmail.EmailAccountId,
                //DontSendBeforeDateUtc = queuedEmailModel.SendImmediately || !queuedEmailModel.DontSendBeforeDate.HasValue ?
                //    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(queuedEmailModel.DontSendBeforeDate.Value)
            };
            await _queuedEmailService.InsertQueuedEmailAsync(requeuedEmail);

            return RedirectToAction("Edit", new { id = requeuedEmail.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a queued email with the specified id
            var email = await _queuedEmailService.GetQueuedEmailByIdAsync(id);
            if (email == null)
                return await AccessDenied();

            await _queuedEmailService.DeleteQueuedEmailAsync(email);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await _queuedEmailService.DeleteQueuedEmailsAsync(await _queuedEmailService.GetQueuedEmailsByIdsAsync(selectedIds.ToArray()));

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteAll()
        {
            await _queuedEmailService.DeleteAllEmailsAsync();

            return Ok();
        }
    }
}