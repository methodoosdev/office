using App.Core;
using App.Core.Domain.Logging;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using App.Framework.Components;
using App.Models.Accounting;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Offices;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Accounting.Controllers
{
    public partial class ListingF5Controller : BaseProtectController
    {
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly ITraderService _traderService;
        private readonly IListingF5Factory _listingF5Factory;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly PlaywrightHttpClient _httpClient;

        public ListingF5Controller(
            IAccountingOfficeService accountingOfficeService,
            ITraderService traderService,
            IListingF5Factory listingF5Factory,
            ITraderConnectionService traderConnectionService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            PlaywrightHttpClient httpClient)
        {
            _accountingOfficeService = accountingOfficeService;
            _traderService = traderService;
            _listingF5Factory = listingF5Factory;
            _traderConnectionService = traderConnectionService;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _httpClient = httpClient;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _listingF5Factory.PrepareListingF5SearchModelAsync(new ListingF5SearchModel());

            //prepare model
            var tableModel = await _listingF5Factory.PrepareListingF5TableModelAsync(new ListingF5TableModel());

            var dialogLabels = new Dictionary<string, string>();
            tableModel.CustomProperties.TryGetValue("columns", out object columns);

            foreach (var item in columns as IList<ColumnConfig>)
                dialogLabels.Add(item.Field, item.Title);

            return Json(new { searchModel, tableModel, dialogLabels });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ListingF5SearchModel searchModel, string connectionId)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var date = searchModel.Period;

            var data = await _listingF5Factory.PrepareListingF5ListAsync(connectionResult, date.Year, date.Month);

            return Json(new { searchModel, data });

        }

        [HttpPost]
        public virtual async Task<IActionResult> SubmitTo([FromBody] ListingF5Data data, string connectionId)
        {
            var _trader = await _traderService.GetTraderByIdAsync(data.TraderId);
            if (_trader == null)
                return await AccessDenied();

            var custActivity = new CustomerActivityResult();

            var trader = _trader.ToTraderModel();
            var traderName = trader.FullName();
            var traderVat = trader.Vat.Trim();

            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();
            var userName = office.TaxisNetUserName?.Trim();
            var password = office.TaxisNetPassword?.Trim();

            var format = "{0}?traderName={1}&vat={2}&userName={3}&password={4}&connectionId={5}";
            var url = string.Format(format,
                "api/listingF5/list",
                WebUtility.UrlEncode(traderName),
                traderVat,
                WebUtility.UrlEncode(userName),
                WebUtility.UrlEncode(password),
                connectionId == "undefined" ? null : connectionId);

            var result = await _httpClient.SendAsync(HttpMethod.Post, url, data);
            if (result.Success)
            {
                var response = JsonConvert.DeserializeObject<DtoListResponse<string>>(result.Content);
                if (response.Success)
                    custActivity.AddSuccess(response.Message);
                else
                    custActivity.AddError(response.Error);
            }
            else
                custActivity.AddError(result.Error);

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.ListingF5, custActivity.ToString());

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Retrieve(int traderId, int year, int month, string connectionId)
        {
            var _trader = await _traderService.GetTraderByIdAsync(traderId);
            if (_trader == null)
                return await AccessDenied();

            var retrieves = new List<ListingF5Result>();
            var custActivity = new CustomerActivityResult();

            var trader = _trader.ToTraderModel();
            var traderName = trader.FullName();
            var traderVat = trader.Vat.Trim();

            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();
            var userName = office.TaxisNetUserName?.Trim();
            var password = office.TaxisNetPassword?.Trim();

            var format = "{0}?traderName={1}&vat={2}&year={3}&month={4}&userName={5}&password={6}&connectionId={7}";
            var url = string.Format(format,
                "api/listingF5Retrieve/list",
                WebUtility.UrlEncode(traderName),
                traderVat,
                year,
                month,
                WebUtility.UrlEncode(userName),
                WebUtility.UrlEncode(password),
                connectionId == "undefined" ? null : connectionId);

            var result = await _httpClient.SendAsync(HttpMethod.Post, url);
            if (result.Success)
            {
                var response = JsonConvert.DeserializeObject<DtoListResponse<ListingF5Result>>(result.Content);
                if (response.Success)
                {
                    retrieves.AddRange(response.List);
                    custActivity.AddSuccess(response.Message);
                }
                else
                    custActivity.AddError(response.Error);
            }
            else
                custActivity.AddError(result.Error);

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.ListingF4Retrieve, custActivity.ToString());

            return Json(new { data = retrieves });
        }
    }
}