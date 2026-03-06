using App.Core;
using App.Core.Domain.Financial;
using App.Core.Domain.Logging;
using App.Core.Domain.Messages;
using App.Core.Domain.Traders;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Financial;
using App.Core.Infrastructure.Mapper;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Financial;
using App.Models.Traders;
using App.Services;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Financial;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Messages;
using App.Services.Offices;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Financial;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Financial
{
    public partial class FinancialObligationController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IFinancialObligationService _financialObligationService;
        private readonly ILocalizationService _localizationService;
        private readonly IFinancialObligationModelFactory _financialObligationModelFactory;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEmailMessageService _emailMessageService;
        private readonly IPersistStateService _persistStateService;
        private readonly ISqlConnectionService _connectionService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IViewRenderService _viewRenderService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly PlaywrightHttpClient _httpClient;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;

        public FinancialObligationController(
            ITraderService traderService,
            IFinancialObligationService financialObligationService,
            ILocalizationService localizationService,
            IFinancialObligationModelFactory financialObligationModelFactory,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IEmailMessageService emailMessageService,
            IPersistStateService persistStateService,
            ISqlConnectionService connectionService,
            IPriceFormatter priceFormatter,
            IModelFactoryService modelFactoryService,
            IViewRenderService viewRenderService,
            ICustomerActivityService customerActivityService,
            PlaywrightHttpClient httpClient,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext,
            IWebHelper webHelper)
        {
            _traderService = traderService;
            _financialObligationService = financialObligationService;
            _localizationService = localizationService;
            _financialObligationModelFactory = financialObligationModelFactory;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _emailMessageService = emailMessageService;
            _persistStateService = persistStateService;
            _connectionService = connectionService;
            _priceFormatter = priceFormatter;
            _modelFactoryService = modelFactoryService;
            _viewRenderService = viewRenderService;
            _customerActivityService = customerActivityService;
            _httpClient = httpClient;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
            _webHelper = webHelper;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _financialObligationModelFactory.PrepareFinancialObligationSearchModelAsync(new FinancialObligationSearchModel());

            var requestModel = await _financialObligationModelFactory.PrepareFinancialObligationRequestModelAsync(new FinancialObligationRequestModel());
            var requestFormModel = await _financialObligationModelFactory.PrepareFinancialObligationRequestFormModelAsync(new FinancialObligationRequestFormModel());

            var filterModel = (await _persistStateService.GetModelInstance<FinancialObligationFilterModel>()).Model;
            var filterFormModel = await _financialObligationModelFactory.PrepareFinancialObligationFilterFormModelAsync(new FinancialObligationFilterFormModel());
            var filterDefaultModel = new FinancialObligationFilterModel();

            return Json(new { searchModel, requestModel, requestFormModel, filterModel, filterFormModel, filterDefaultModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] FinancialObligationSearchModel searchModel)
        {
            //prepare model
            var model = await _financialObligationModelFactory.PrepareFinancialObligationListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _financialObligationModelFactory.PrepareFinancialObligationModelAsync(new FinancialObligationModel(), null);

            //prepare form
            var formModel = await _financialObligationModelFactory.PrepareFinancialObligationFormModelAsync(new FinancialObligationFormModel(), false);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] FinancialObligationModel model)
        {
            if (ModelState.IsValid)
            {
                var financialObligation = model.ToEntity<FinancialObligation>();
                await _financialObligationService.InsertFinancialObligationAsync(financialObligation);

                return Json(financialObligation.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var financialObligation = await _financialObligationService.GetFinancialObligationByIdAsync(id);
            if (financialObligation == null)
                return await AccessDenied();

            //prepare model
            var model = await _financialObligationModelFactory.PrepareFinancialObligationModelAsync(null, financialObligation);

            //prepare form
            var formModel = await _financialObligationModelFactory.PrepareFinancialObligationFormModelAsync(new FinancialObligationFormModel(), true);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] FinancialObligationModel model)
        {
            //try to get entity with the specified id
            var financialObligation = await _financialObligationService.GetFinancialObligationByIdAsync(model.Id);
            if (financialObligation == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    financialObligation = model.ToEntity(financialObligation);
                    await _financialObligationService.UpdateFinancialObligationAsync(financialObligation);

                    return Json(financialObligation.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.FinancialObligations.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var financialObligation = await _financialObligationService.GetFinancialObligationByIdAsync(id);
            if (financialObligation == null)
                return await AccessDenied();

            try
            {
                await _financialObligationService.DeleteFinancialObligationAsync(financialObligation);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.FinancialObligations.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _financialObligationService.DeleteFinancialObligationAsync((await _financialObligationService.GetFinancialObligationsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.FinancialObligations.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> EfkaNonSalaried([FromBody] ICollection<int> selectedIds, string connectionId)
        {
            var traders = selectedIds.Count > 0 ?
                (await _traderService.GetTradersByIdsAsync(selectedIds.ToArray()))
                    .Where(e => !e.Deleted && e.Active).ToList() :
                await _traderService.GetAllTradersAsync();

            traders = traders
                .Where(x => x.HasFinancialObligation && 
                    (x.CustomerTypeId == (int)CustomerType.NaturalPerson || x.CustomerTypeId == (int)CustomerType.IndividualCompany))
                .ToList();
            var traderIds = traders.Select(x => x.Id).ToList();

            var model = new FinancialObligationRequestModel { TraderIds = traderIds, ServiceIds = new int[] { 4 }.ToList() };

            return await Retrieve(model, connectionId);
        }
                
        [HttpPost]
        public virtual async Task<IActionResult> Retrieve([FromBody] FinancialObligationRequestModel requestModel, string connectionId)
        {
            //var playwrightUrl = _webHelper.GetPlaywrightUrl(Request);

            var hyperConnection = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);

            var obligations = requestModel.ServiceIds.Sum(x => x);

            var financialObligationItemList = new List<FinancialObligationDto>();

            var custActivity = new CustomerActivityResult();

            var index = 1;

            async Task SendAsync(string url, string title, string traderName)
            {
                var result = await _httpClient.SendAsync(HttpMethod.Post, url);
                if (result.Success)
                {
                    var response = JsonConvert.DeserializeObject<DtoListResponse<FinancialObligationDto>>(result.Content);
                    if (response.Success)
                    {
                        if (response.List.Count == 0)
                            custActivity.AddSuccess($"<b>{title}:</b> Δεν έχει οφειλές - {traderName}");
                        else
                        {
                            financialObligationItemList.AddRange(response.List);
                            custActivity.AddSuccess(response.Message);
                        }
                    }
                    else
                        custActivity.AddError(response.Error);
                }
                else
                    custActivity.AddError(result.Error);
            }

            foreach (var traderId in requestModel.TraderIds)
            {
                var _trader = await _traderService.GetTraderByIdAsync(traderId);
                var trader = _trader.ToModel<TraderModel>();
                if (trader != null)
                {
                    // ΚΕΑΟ - ΙΚΑ Εργοδοτών
                    var traderName = trader.FullName();

                    if (((FinancialObligationType)obligations).HasFlag(FinancialObligationType.KeaoIka)) // ind+leg
                    {
                        var cred1 = (trader.EmployerIkaUserName?.Trim(), trader.EmployerIkaPassword?.Trim());
                        var cred2 = (trader.KeaoIkaUserName?.Trim(), trader.KeaoIkaPassword?.Trim());
                        var sameCredencials = cred1.Item1 == cred2.Item1 && cred1.Item2 == cred2.Item2;

                        var ikaList = new List<(string userName, string password)> { cred1 };
                        if (!sameCredencials)
                            ikaList.Add(cred2);

                        foreach (var item in ikaList)
                        {
                            if (!string.IsNullOrEmpty(item.userName) && !string.IsNullOrEmpty(item.password))
                            {
                                var url = $"api/ikaRetrieve/list?index={index}&traderId={trader.Id}&traderName={WebUtility.UrlEncode(traderName)}&userName={WebUtility.UrlEncode(item.userName)}&password={WebUtility.UrlEncode(item.password)}&connectionId={connectionId}";
                                await SendAsync(url, "ΚΕΑΟ - ΕΦΚΑ Εργοδοτικές εισφόρες", traderName);
                            }
                        }
                    }

                    // ΚΕΑΟ - Οφειλές ΟΑΕΕ
                    if (((FinancialObligationType)obligations).HasFlag(FinancialObligationType.Oaee)) // ind+nat
                    {
                        var userName = trader.KeaoOaeeUserName?.Trim();
                        var password = trader.KeaoOaeePassword?.Trim();

                        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                        {
                            var url = $"api/oaeeRetrieve/list?index={index}&traderId={trader.Id}&traderName={WebUtility.UrlEncode(traderName)}&userName={WebUtility.UrlEncode(userName)}&password={WebUtility.UrlEncode(password)}&connectionId={connectionId}";
                            await SendAsync(url, "ΚΕΑΟ - Οφειλές OAEE", traderName);
                        }
                    }

                    // ΕΦΚΑ Μη μισθωτών
                    if (((FinancialObligationType)obligations).HasFlag(FinancialObligationType.EfkaNonSalaried)) //ind+nat
                    {
                        var userName = trader.TaxisUserName?.Trim();
                        var password = trader.TaxisPassword?.Trim();
                        var vat = trader.Vat?.Trim();
                        var amka = trader.Amka?.Trim();

                        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(vat) && !string.IsNullOrEmpty(amka))
                        {
                            var url = $"api/efkaRetrieve/list?index={index}&traderId={trader.Id}&traderName={WebUtility.UrlEncode(traderName)}&userName={WebUtility.UrlEncode(userName)}&password={WebUtility.UrlEncode(password)}&vat={vat}&amka={amka}&connectionId={connectionId}";
                            await SendAsync(url, "Εισφορές Μη μισθωτών", traderName);
                        }
                    }

                    // Εργοδοτικές εισφορές από ΑΠΔ
                    if (((FinancialObligationType)obligations).HasFlag(FinancialObligationType.Efka)) // ind+leg
                    {
                        if (hyperConnection.Success)
                        {
                            var efkaList = await _financialObligationModelFactory.EfkaTekaResult(_trader, hyperConnection.Connection);
                            financialObligationItemList.AddRange(efkaList);
                            custActivity.AddSuccess("Εργοδοτικές εισφορές από ΑΠΔ");
                        }
                    }

                    // ΚΕΑΟ-ΕΦΚΑ Μη μισθωτών
                    if (((FinancialObligationType)obligations).HasFlag(FinancialObligationType.Keao)) // ind+nat
                    {
                        var userName = trader.KeaoEfkaUserName?.Trim();
                        var password = trader.KeaoEfkaPassword?.Trim();

                        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                        {
                            var url = $"api/keaoRetrieve/list?index={index}&traderId={trader.Id}&traderName={WebUtility.UrlEncode(traderName)}&userName={WebUtility.UrlEncode(userName)}&password={WebUtility.UrlEncode(password)}&connectionId={connectionId}";
                            await SendAsync(url, "ΚΕΑΟ - ΕΦΚΑ Μη μισθωτών", traderName);
                        }
                    }

                    // Οφειλές στην εφορία
                    if (((FinancialObligationType)obligations).HasFlag(FinancialObligationType.Aade))
                    {
                        var userName = trader.TaxisUserName?.Trim();
                        var password = trader.TaxisPassword?.Trim();

                        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                        {
                            var url = $"api/doyRetrieve/list?index={index}&traderId={trader.Id}&traderName={WebUtility.UrlEncode(traderName)}&userName={WebUtility.UrlEncode(userName)}&password={WebUtility.UrlEncode(password)}&connectionId={connectionId}";
                            await SendAsync(url, "ΑΑΔΕ", traderName);
                        }
                    }

                }

                index++;
            }

            if (financialObligationItemList.Count == 0)
            {
                custActivity.AddError("Ο συναλλασσόμενος ή οι συναλ/νοι που επιλέχθηκαν είναι διαγραμμένοι ή ανενεργοί ή δεν ικανοποιούν τα κριτήρια φίλτρου.");
            }

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.FinancialObligation, custActivity.ToString());

            var customer = await _workContext.GetCurrentCustomerAsync();

            var date = DateTime.UtcNow;
            var list = await _financialObligationService.GetAllFinancialObligationsAsync(date.Month, date.Year);
            foreach (var item in financialObligationItemList)
            {
                var exist = list.Any(x =>
                    x.TraderId == item.TraderId && x.PaymentValue == item.PaymentValue &&
                    x.Institution == item.Institution && x.PaymentType == item.PaymentType &&
                    x.PaymentIdentity == item.PaymentIdentity);

                if (!exist)
                {
                    var financialObligation = AutoMapperConfiguration.Mapper.Map<FinancialObligation>(item);
                    financialObligation.CustomerId = customer.Id;
                    financialObligation.CreatedOnUtc = date;

                    await _financialObligationService.InsertFinancialObligationAsync(financialObligation);
                }
            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateEmailMessages(string connectionId)
        {
            var date = DateTime.UtcNow;

            var list = _financialObligationService.Table
                .Where(x =>
                    x.CreatedOnUtc.Month == date.Month && x.CreatedOnUtc.Year == date.Year && !x.IsSent)
                .ToList();

            await CreateEmailMessagesAsync(list, date, connectionId);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateSelectedEmailMessages([FromBody] ICollection<int> selectedIds, string connectionId)
        {
            var date = DateTime.UtcNow;

            var list = await _financialObligationService.GetFinancialObligationsByIdsAsync(selectedIds.ToArray());

            await CreateEmailMessagesAsync(list, date, connectionId);

            return Ok();
        }

        private async Task CreateEmailMessagesAsync(IEnumerable<FinancialObligation> list, DateTime date, string connectionId)
        {
            var dict = list.GroupBy(p => p.TraderId).ToDictionary(p => p.Key, p => p.Select(k => k).ToArray());

            var importedEmailMessages = new List<EmailMessage>();
            var defaultEmailAccountId = _emailAccountSettings.DefaultEmailAccountId;
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(defaultEmailAccountId);

            var months = await _modelFactoryService.GetSelectionItemListAsync(DateLocaleResources.LocaleMonthResourceDict);
            var totalLabel = await _localizationService.GetResourceAsync("App.Common.GrandTotal");
            var customer = await _workContext.GetCurrentCustomerAsync();
            var title = await _localizationService.GetResourceAsync("App.Models.FinancialObligationModel.ListForm.Title");

            var custActivity = new CustomerActivityResult();

            foreach (var kvp in dict)
            {
                var _trader = await _traderService.GetTraderByIdAsync(kvp.Key);
                var trader = _trader.ToModel<TraderModel>();
                var traderName = trader.FullName();

                var emailList = new List<string> { trader.Email, trader.Email2, trader.Email3 }.Where(x => !string.IsNullOrEmpty(x?.Trim())).ToList();
                var ccList = new List<string> { trader.Email2, trader.Email3 }.Where(x => !string.IsNullOrEmpty(x?.Trim())).ToList();
                var cc = string.Join(", ", ccList);

                custActivity.AddSuccess($"<b>Δημιουργία email:</b> {traderName}: {string.Join(", ", emailList)}");

                var financialObligations = kvp.Value.OrderBy(o => o.PaymentDate).ToList();

                foreach (var fo in financialObligations)
                {
                    var paymentValue = await _priceFormatter.FormatPriceAsync(fo.PaymentValue, true);
                    custActivity.AddSuccess($"{fo.Institution} - {fo.PaymentType} - {paymentValue}", true);
                    if (string.IsNullOrEmpty(fo.PaymentIdentity))
                        custActivity.AddInfos($"{traderName}: Κενή ταυτότητα πληρωμής - {fo.PaymentType} - {paymentValue}");
                }

                var total = financialObligations.Sum(x => x.PaymentValue);
                var totalRow = new FinancialObligation { Institution = "Γενικό σύνολο", PaymentValue = total, PaymentDate = date };
                financialObligations.Add(totalRow);

                var monthLabel = months.First(x => x.Value == date.Month).Label;
                var subTitle = $"{title} - {totalLabel}: {await _priceFormatter.FormatPriceAsync(total, true)} - {monthLabel}";

                var template = "~/Views/Messages/EmailFinancialObligation.cshtml";
                var financialObligationItemList = financialObligations.Select(x => AutoMapperConfiguration.Mapper.Map<FinancialObligationDto>(x)).ToList();
                var html = await _viewRenderService
                    .RenderToStringAsync<(IList<FinancialObligationDto>, string, string)>
                        (template, new(financialObligationItemList, subTitle, traderName));

                var emailMessage = new EmailMessage
                {
                    CustomerId = customer.Id,
                    EmailMessageTypeId = (int)EmailMessageType.FinancialObligation,
                    Description = await _localizationService.GetLocalizedEnumAsync(EmailMessageType.FinancialObligation),
                    CreatedDate = date,
                    Subject = title,
                    Body = html,
                    FromAddress = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    ToAddress = trader.Email,
                    ToName = traderName,
                    Cc = string.Join(", ", cc),
                    TraderId = trader.Id,
                    Period = date.Month
                };
                importedEmailMessages.Add(emailMessage);

                foreach (var item in financialObligations)
                    item.IsSent = true;

                // Exlude total rows
                financialObligations = financialObligations.Where(k => k.TraderId > 0).Select(x => x).ToList();

                await _financialObligationService.UpdateFinancialObligationAsync(financialObligations);

                //await progress.CalcAsync(connectionId);
            }

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.FinancialObligation, custActivity.ToString());

            await _emailMessageService.InsertEmailMessageAsync(importedEmailMessages);
        }
    }
}