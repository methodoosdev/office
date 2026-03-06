using App.Core;
using App.Core.Domain.Logging;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class MyDataCredentialsController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly PlaywrightHttpClient _httpClient;

        public MyDataCredentialsController(
            ITraderService traderService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            PlaywrightHttpClient httpClient)
        {
            _traderService = traderService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _httpClient = httpClient;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Import([FromBody] ICollection<int> selectedIds, string connectionId)
        {
            var errors = new List<TradeErrorResult>();

            var traders = await _traderService.GetTradersByIdsAsync(selectedIds.ToArray());
            traders = traders.Where((trader)=> !trader.Deleted && trader.Active).ToList();

            var custActivity = new CustomerActivityResult();

            foreach (var _trader in traders)
            {
                var trader = _trader.ToModel<TraderModel>();

                var userName = trader.TaxisUserName?.Trim();
                var password = trader.TaxisPassword?.Trim();
                var traderName = trader.FullName();

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    var errorMessage = await _localizationService.GetResourceAsync("App.Errors.WrongCredentials");
                    custActivity.AddError($"<b>Σύνδεση:</b> {traderName}. {errorMessage}");

                    continue;
                }

                var format = "{0}?traderName={1}&userName={2}&password={3}&connectionId={4}";
                var url = string.Format(format,
                    "api/myDataCredentials/list",
                    WebUtility.UrlEncode(traderName),
                    WebUtility.UrlEncode(userName),
                    WebUtility.UrlEncode(password),
                    connectionId == "undefined" ? null : connectionId);

                var result = await _httpClient.SendAsync(HttpMethod.Post, url);
                if (result.Success)
                {
                    var response = JsonConvert.DeserializeObject<DtoListResponse<MyDataCredentialDto>>(result.Content);
                    if (response.Success)
                    {
                        var myDataCredentials = response.List.FirstOrDefault();
                        trader.MydataPaswword = myDataCredentials?.SubscriptionKey;
                        trader.MydataUserName = myDataCredentials?.UserName;
                        custActivity.AddSuccess(response.Message);
                    }
                    else
                        custActivity.AddError(response.Error);
                }
                else
                    custActivity.AddError(result.Error);
            }

            await _traderService.UpdateTraderAsync(traders);

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.MyDataCredentials, custActivity.ToString());

            return Json(errors);
        }
    }
}