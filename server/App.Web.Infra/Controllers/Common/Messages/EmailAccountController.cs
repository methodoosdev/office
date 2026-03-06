using App.Core;
using App.Core.Domain.Messages;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Messages;
using App.Services.Common;
using App.Services.Configuration;
using App.Services.Hubs;
using App.Services.Localization;
using App.Services.Messages;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Messages
{
    public partial class EmailAccountController : BaseProtectController
    {
        #region Fields

        private readonly IHubContext<ChatHub> _hub;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountModelFactory _emailAccountModelFactory;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEmailSender _emailSender;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public EmailAccountController(
            IHubContext<ChatHub> hub,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountModelFactory emailAccountModelFactory,
            IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            ILocalizationService localizationService,
            ISettingService settingService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            IStoreContext storeContext)
        {
            _hub = hub;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountModelFactory = emailAccountModelFactory;
            _emailAccountService = emailAccountService;
            _emailSender = emailSender;
            _localizationService = localizationService;
            _settingService = settingService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _emailAccountModelFactory.PrepareEmailAccountSearchModelAsync(new EmailAccountSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] EmailAccountSearchModel searchModel)
        {
            //prepare model
            var model = await _emailAccountModelFactory.PrepareEmailAccountListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> MarkAsDefaultEmail(int id)
        {
            var defaultEmailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
            if (defaultEmailAccount == null)
                return await AccessDenied();

            _emailAccountSettings.DefaultEmailAccountId = defaultEmailAccount.Id;
            await _settingService.SaveSettingAsync(_emailAccountSettings);

            return Ok();
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(new EmailAccountModel(), null);

            //prepare form
            var formModel = await _emailAccountModelFactory.PrepareEmailAccountFormModelAsync(new EmailAccountFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IActionResult> Create([FromBody] EmailAccountModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var emailAccount = model.ToEntity<EmailAccount>();

                //set password manually
                emailAccount.Password = model.Password;
                await _emailAccountService.InsertEmailAccountAsync(emailAccount);

                return Json(emailAccount.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get an email account with the specified id
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
            if (emailAccount == null)
                return await AccessDenied();

            //prepare model
            var model = await _emailAccountModelFactory.PrepareEmailAccountModelAsync(null, emailAccount);

            //prepare form
            var formModel = await _emailAccountModelFactory.PrepareEmailAccountFormModelAsync(new EmailAccountFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] EmailAccountModel model, bool continueEditing)
        {
            //try to get an email account with the specified id
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
            if (emailAccount == null)
                return await AccessDenied();

            if (ModelState.IsValid)
            {
                emailAccount = model.ToEntity(emailAccount);
                await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

                return Json(emailAccount.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get an email account with the specified id
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
            if (emailAccount == null)
                return await AccessDenied();

            try
            {
                await _emailAccountService.DeleteEmailAccountAsync(emailAccount);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Errors.FailedToSendEmail");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ChangePassword([FromBody] EmailAccountModel model)
        {
            //try to get an email account with the specified id
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(model.Id);
            if (emailAccount == null)
                return await AccessDenied();

            //do not validate model
            emailAccount.Password = model.Password;
            await _emailAccountService.UpdateEmailAccountAsync(emailAccount);

            return Json(emailAccount.Id);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendTestEmail(int id, string email)
        {
            //try to get an email account with the specified id
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(id);
            if (emailAccount == null)
                return await AccessDenied();

            if (!CommonHelper.IsValidEmail(email))
            {
                return await BadRequestMessageAsync("App.Errors.WrongEmail");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new NopException("Enter test email address");

                var subject = (await _storeContext.GetCurrentStoreAsync()).Name + ". Testing email functionality.";
                var body = "Email works fine.";
                await _emailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, email, null);
            }
            catch
            {
                return await BadRequestMessageAsync("App.Errors.FailedToSendEmail");
            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendInfoMessage(string message, string connectionId)
        {
            // send inner message to all employers
            await _hub.Clients.AllExcept(connectionId).SendAsync("innerMessageSignal", message);

            return Ok();
        }

        #endregion
    }
}