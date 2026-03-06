using App.Core;
using App.Core.Domain.Logging;
using App.Core.Domain.Messages;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Messages;
using App.Services.Customers;
using App.Services.Hubs;
using App.Services.Logging;
using App.Services.Messages;
using App.Services.Offices;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Messages
{
    public partial class EmailMessageController : BaseProtectController
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly IEmailMessageService _emailMessageService;
        private readonly IEmailMessageModelFactory _emailMessageModelFactory;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IPersistStateService _persistStateService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEmailSender _emailSender;
        private readonly IWorkContext _workContext;

        public EmailMessageController(
            IHubContext<ChatHub> hub,
            IEmailMessageService emailMessageService,
            IEmailMessageModelFactory emailMessageModelFactory,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IPersistStateService persistStateService,
            ICustomerActivityService customerActivityService,
            IEmailSender emailSender,
            IWorkContext workContext)
        {
            _hub = hub;
            _emailMessageService = emailMessageService;
            _emailMessageModelFactory = emailMessageModelFactory;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _persistStateService = persistStateService;
            _customerActivityService = customerActivityService;
            _emailSender = emailSender;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _emailMessageModelFactory.PrepareEmailMessageSearchModelAsync(new EmailMessageSearchModel());

            var filterModel = (await _persistStateService.GetModelInstance<EmailMessageFilterModel>()).Model;
            var filterFormModel = await _emailMessageModelFactory.PrepareEmailMessageFilterFormModelAsync(new EmailMessageFilterFormModel());
            var filterDefaultModel = new EmailMessageFilterModel();

            return Json(new { searchModel, filterModel, filterFormModel, filterDefaultModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] EmailMessageSearchModel searchModel)
        {
            //prepare model
            var model = await _emailMessageModelFactory.PrepareEmailMessageListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _emailMessageModelFactory.PrepareEmailMessageModelAsync(new EmailMessageModel(), null);

            //prepare form
            var formModel = await _emailMessageModelFactory.PrepareEmailMessageFormModelAsync(new EmailMessageFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] EmailMessageModel model)
        {
            if (ModelState.IsValid)
            {
                var emailMessage = model.ToEntity<EmailMessage>();
                await _emailMessageService.InsertEmailMessageAsync(emailMessage);

                return Json(emailMessage.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var emailMessage = await _emailMessageService.GetEmailMessageByIdAsync(id);
            if (emailMessage == null)
                return await AccessDenied();

            //prepare model
            var model = await _emailMessageModelFactory.PrepareEmailMessageModelAsync(null, emailMessage);

            //prepare form
            var formModel = await _emailMessageModelFactory.PrepareEmailMessageFormModelAsync(new EmailMessageFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] EmailMessageModel model)
        {
            //try to get entity with the specified id
            var emailMessage = await _emailMessageService.GetEmailMessageByIdAsync(model.Id);
            if (emailMessage == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    emailMessage = model.ToEntity(emailMessage);
                    await _emailMessageService.UpdateEmailMessageAsync(emailMessage);

                    return Json(emailMessage.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.EmailMessages.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var emailMessage = await _emailMessageService.GetEmailMessageByIdAsync(id);
            if (emailMessage == null)
                return await AccessDenied();

            try
            {
                await _emailMessageService.DeleteEmailMessageAsync(emailMessage);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.EmailMessages.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _emailMessageService.DeleteEmailMessageAsync((await _emailMessageService.GetEmailMessagesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.EmailMessages.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendFinancialObligationEmails(string connectionId)
        {
            var date = DateTime.Now.ToUtcRelative();

            var emailMessages = _emailMessageService.Table
                .Where(x =>
                    x.EmailMessageTypeId == (int)EmailMessageType.FinancialObligation &&
                    x.CreatedDate.Year == date.Year &&
                    x.CreatedDate.Month == date.Month && !x.ShippingDate.HasValue)
                .ToList();

            await SendEmailsAsync(emailMessages, date, connectionId);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendSelectedEmails([FromBody] ICollection<int> selectedIds, string connectionId)
        {
            var date = DateTime.Now.ToUtcRelative();

            var emailMessages = await _emailMessageService.GetEmailMessagesByIdsAsync(selectedIds.ToArray());

            await SendEmailsAsync(emailMessages, date, connectionId);

            return Ok();
        }

        private async Task SendEmailsAsync(IList<EmailMessage> emailMessages, DateTime date, string connectionId)
        {
            var defaultEmailAccountId = _emailAccountSettings.DefaultEmailAccountId;
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(defaultEmailAccountId);
            //var progress = new CalcProgress(_hub, emailMessages.Count);
            var customer = await _workContext.GetCurrentCustomerAsync();

            var ca = new CustomerActivityResult();

            foreach (var emailMessage in emailMessages)
            {
                var cc = emailMessage.Cc?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).ToList() ?? new List<string>();

                try
                {
                    await _emailSender.SendEmailAsync(emailAccount, emailMessage.Subject, emailMessage.Body,
                        emailMessage.FromAddress.Trim(), emailMessage.FromName,
                        emailMessage.ToAddress.Trim(), emailMessage.ToName, cc: cc);

                    emailMessage.ShippingDate = date;
                    emailMessage.SenderId = customer.Id;

                    await _emailMessageService.UpdateEmailMessageAsync(emailMessage);

                    ca.AddSuccess($"<b>Αποστολή email:</b> {emailMessage.ToName}: {emailMessage.ToAddress}");
                }
                catch
                {
                    ca.AddError($"{emailMessage.ToName}: {emailMessage.ToAddress}");
                }

                //await progress.CalcAsync(connectionId);
            }

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.SendEmail, ca.ToString());
        }

    }
}