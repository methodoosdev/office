using App.Core;
using App.Core.Domain.Logging;
using App.Core.Infrastructure.Dtos;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Payroll;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Hubs;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll
{
    public partial class FmySubmissionController : BaseProtectController
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly ITraderService _traderService;
        private readonly IFmySubmissionModelFactory _fmySubmissionModelFactory;
        private readonly ISqlConnectionService _connectionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly PlaywrightHttpClient _httpClient;

        public FmySubmissionController(
            IHubContext<ChatHub> hub,
            ITraderService traderService,
            IFmySubmissionModelFactory fmySubmissionModelFactory,
            ISqlConnectionService connectionService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            PlaywrightHttpClient httpClient)
        {
            _hub = hub;
            _traderService = traderService;
            _fmySubmissionModelFactory = fmySubmissionModelFactory;
            _connectionService = connectionService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _httpClient = httpClient;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _fmySubmissionModelFactory.PrepareFmySubmissionSearchModelAsync(new FmySubmissionSearchModel());

            //prepare model
            var tableModel = await _fmySubmissionModelFactory.PrepareFmySubmissionTableModelAsync(new FmySubmissionTableModel());

            return Json(new { tableModel, searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Import([FromBody] ICollection<int> selectedIds, int monthFrom, int monthTo, int year, string connectionId)
        {
            var connection = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!connection.Success)
                return BadRequest(connection.Error);

            var fmys = new List<FmySubmissionModel>();
            var traders = await _traderService.GetTradersByIdsAsync(selectedIds.ToArray());

            var custActivity = new CustomerActivityResult();

            var index = 1;

            async Task SendAsync(string url, string traderName, int hyperPayrollId)
            {
                var result = await _httpClient.SendAsync(HttpMethod.Post, url);
                if (result.Success)
                {
                    var response = JsonConvert.DeserializeObject<DtoListResponse<string>>(result.Content);
                    if (response.Success)
                    {
                        var listFromTaxisNet = _fmySubmissionModelFactory.ExtractFmySubmissionModel(response.List, traderName);
                        var listFromHyperM = await _fmySubmissionModelFactory.GetFmyFromHyperMAsync(connection.Connection, monthFrom, monthTo, year, hyperPayrollId, traderName);

                        foreach (var itemFromHyperM in listFromHyperM)
                        {
                            var itemFromTaxisNet = listFromTaxisNet.Where(x => x.Period == itemFromHyperM.Period).FirstOrDefault();
                            if (itemFromTaxisNet == null)
                                itemFromTaxisNet = new FmySubmissionModel { Check = false, Surname = itemFromHyperM.Surname, Period = itemFromHyperM.Period, SubmissionType = "Δεν υποβλήθηκε" };

                            _fmySubmissionModelFactory.PrepareCompareFmy(itemFromTaxisNet, itemFromHyperM);
                            fmys.Add(itemFromTaxisNet);
                            fmys.Add(itemFromHyperM);
                        }

                        custActivity.AddSuccess(response.Message);
                    }
                    else
                        custActivity.AddError(response.Error);
                }
                else
                    custActivity.AddError(result.Error);
            }

            foreach (var _trader in traders)
            {
                var trader = _trader.ToModel<TraderModel>();
                var traderName = trader.FullName();
                var aadeMySelf = trader.Vat == "038066412" || trader.Vat == "801408066"; // Hack

                var userName = trader.TaxisUserName?.Trim();
                var password = trader.TaxisPassword?.Trim();

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    var errorMessage = await _localizationService.GetResourceAsync("App.Errors.WrongCredentials");
                    custActivity.AddError(errorMessage);

                    continue;
                }

                var format = "{0}?index={1}&traderName={2}&monthFrom={3}&monthTo={4}&year={5}&mySelf={6}&userName={7}&password={8}&connectionId={9}";
                var url = string.Format(format,
                    "api/fmySubmission/list",
                    index,
                    WebUtility.UrlEncode(traderName),
                    monthFrom,
                    monthTo,
                    year,
                    aadeMySelf,
                    WebUtility.UrlEncode(userName),
                    WebUtility.UrlEncode(password),
                    connectionId == "undefined" ? null : connectionId);

                await SendAsync(url, traderName, trader.HyperPayrollId);
            }

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.FmySubmission, custActivity.ToString());

            return Json(fmys);
        }
    }
}