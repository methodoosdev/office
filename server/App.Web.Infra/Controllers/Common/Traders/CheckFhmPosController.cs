using App.Core;
using App.Core.Domain.Logging;
using App.Core.Domain.Messages;
using App.Core.Domain.Traders;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Messages;
using App.Services.Offices;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class CheckFhmPosController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEmailMessageService _emailMessageService;
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly IViewRenderService _viewRenderService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;

        public CheckFhmPosController(
            ITraderService traderService,
            ILocalizationService localizationService,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IEmailMessageService emailMessageService,
            IAccountingOfficeService accountingOfficeService,
            IViewRenderService viewRenderService,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _localizationService = localizationService;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _emailMessageService = emailMessageService;
            _accountingOfficeService = accountingOfficeService;
            _viewRenderService = viewRenderService;
            _customerActivityService = customerActivityService;
            _workContext = workContext;
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateEmail([FromBody] ICollection<int> selectedIds, string connectionId)
        {
            var ca = new CustomerActivityResult();

            var selectedTraders = new List<Trader>();
            var traders = (await _traderService.GetTradersByIdsAsync(selectedIds.ToArray())).ToList();

            foreach (var trader in traders)
            {
                var traderName = trader.ToTraderFullName();

                if (trader.Deleted || !trader.Active)
                    ca.AddError($"{traderName}: Διαγραμμένος ή ανενεργός.");

                else if (!(trader.CategoryBookType == CategoryBookType.B ||
                    trader.CategoryBookType == CategoryBookType.C))
                    ca.AddError($"{traderName}: Μη επιτηδευματίας.");

                else
                    selectedTraders.Add(trader);
            }

            var importedEmailMessages = new List<EmailMessage>();
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();

            foreach (var trader in selectedTraders)
            {
                var traderName = trader.ToTraderFullName();

                //await using (var page = new FhmPosPage(connectionId))
                //{
                //    try
                //    {
                //        if (!await page.LoginSucces(trader.TaxisUserName.Trim(), trader.TaxisPassword.Trim()))
                //            continue;

                //        var email = (new string[] { trader.Email?.Trim(), trader.Email2?.Trim(), trader.Email3?.Trim() }).FirstOrDefault(f => !string.IsNullOrEmpty(f));

                //        if (string.IsNullOrEmpty(email))
                //        {
                //            ca.AddError($"{traderName}: Κενό email.");
                //            continue;
                //        }

                //        var model = await page.Execute();

                //        var defaultEmailAccountId = _emailAccountSettings.DefaultEmailAccountId;
                //        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(defaultEmailAccountId);

                //        var customer = await _workContext.GetCurrentCustomerAsync();

                //        var template = "~/Views/Messages/EmailCashRegisterPos.cshtml";

                //        var html = await _viewRenderService.RenderToStringAsync<(IList<FhmItemModel>, IList<PosItemModel>, IList<ContractsItemModel>)>(template, model);

                //        var date = DateTime.UtcNow;

                //        var emailMessage = new EmailMessage
                //        {
                //            CustomerId = customer.Id,
                //            EmailMessageTypeId = (int)EmailMessageType.Newsletter,
                //            Description = "Επιχειρήσεις και ΑΑΔΕ",
                //            CreatedDate = date,
                //            Subject = "Μητρώο - ΦΗΜ και POS (αρχείο 2)",
                //            Body = html,
                //            FromAddress = emailAccount.Email,
                //            FromName = emailAccount.DisplayName,
                //            ToAddress = email,
                //            ToName = trader.FullName(),
                //            TraderId = trader.Id,
                //            Period = date.Month
                //        };
                //        importedEmailMessages.Add(emailMessage);

                //        ca.AddSuccess($"<b>Δημιουργία email:</b> {traderName}: {email}.");
                //    }
                //    catch (Exception exc)
                //    {
                //        var errorMessage = exc is AppPlaywrightException exception ? exception.Message : await _localizationService.GetResourceAsync("App.Errors.FailedToScraping");

                //        ca.AddError($"{traderName}: {errorMessage}.");
                //    }
                //}
            }

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.CheckFhmPos, ca.ToString());

            await _emailMessageService.InsertEmailMessageAsync(importedEmailMessages);

            return Ok();
        }
    }
}