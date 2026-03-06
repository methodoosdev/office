using App.Core;
using App.Core.Domain.Logging;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Payroll;
using App.Models.Payroll;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using App.Models.Traders;
using App.Core.Domain.Payroll;
using App.Web.Infra.Queries.Payroll;
using App.Data.DataProviders;

namespace App.Web.Infra.Controllers.Payroll
{
    public partial class ApdSubmissionController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IApdSubmissionModelFactory _apdSubmissionModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ISqlConnectionService _connectionService;
        private readonly IAppDataProvider _dataProvider;
        private readonly PlaywrightHttpClient _httpClient;

        public ApdSubmissionController(
            ITraderService traderService,
            IApdSubmissionModelFactory apdSubmissionModelFactory,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ISqlConnectionService connectionService,
            IAppDataProvider appDataProvider,
            PlaywrightHttpClient httpClient)
        {
            _traderService = traderService;
            _apdSubmissionModelFactory = apdSubmissionModelFactory;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _connectionService = connectionService;
            _dataProvider = appDataProvider;
            _httpClient = httpClient;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _apdSubmissionModelFactory.PrepareApdSubmissionSearchModelAsync(new ApdSubmissionSearchModel());

            //prepare model
            var tableModel = await _apdSubmissionModelFactory.PrepareApdSubmissionTableModelAsync(new ApdSubmissionTableModel());

            return Json(new { tableModel, searchModel });
        }

        private async Task<string> GetAmIkaAsync(int companyId)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return string.Empty;

            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(result.Connection, PayrollQuery.EmployerLookupItem);

            var employer = employers.FirstOrDefault(x => x.CompanyId == companyId);

            return employer?.AmIka.Trim() ?? string.Empty;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Import([FromBody] ICollection<int> selectedIds, int month, int year, string connectionId)
        {
            var apds = new List<ApdSubmissionModel>();
            var traders = await _traderService.GetTradersByIdsAsync(selectedIds.ToArray());

            var custActivity = new CustomerActivityResult();

            var index = 1;

            async Task SendAsync(string url, string traderName)
            {
                var result = await _httpClient.SendAsync(HttpMethod.Post, url);
                if (result.Success)
                {
                    var response = JsonConvert.DeserializeObject<DtoListResponse<ApdSubmissionDto>>(result.Content);
                    if (response.Success)
                    {
                        var total = 0m;
                        var models = new List<ApdSubmissionModel>();
                        foreach (var item in response.List)
                        {
                            var model = await _apdSubmissionModelFactory.ExtractApdSubmissionModel(item.Found, item.PdfText, traderName);
                            total += model.TotalContributions;
                            models.Add(model);
                        }
                        var apd = models.First(x => x.Type == "ΚΑΝΟΝΙΚΗ");
                        apd.TotalContributions = total;

                        apds.Add(apd);
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

                var userName = trader.TaxisUserName?.Trim();
                var password = trader.TaxisPassword?.Trim();

                var amIka = await GetAmIkaAsync(trader.HyperPayrollId);

                if (string.IsNullOrEmpty(amIka))
                {
                    var errorMessage = "Δεν έχει καταχωρημενο ΑΜΕ στη μισθοδοσία.";
                    var apd = new ApdSubmissionModel { Surname = traderName, Error = errorMessage };
                    apds.Add(apd);
                    custActivity.AddError($"<b>ΑΜΕ:</b> {traderName}. {errorMessage}");

                    continue;
                }

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    var errorMessage = await _localizationService.GetResourceAsync("App.Errors.WrongCredentials");
                    var apd = new ApdSubmissionModel { Surname = traderName, Error = errorMessage };
                    apds.Add(apd);
                    custActivity.AddError($"<b>Σύνδεση:</b> {traderName}. {errorMessage}");

                    continue;
                }

                var format = "{0}?index={1}&traderName={2}&month={3}&year={4}&userName={5}&password={6}&amIka={7}&connectionId={8}";
                var url = string.Format(format,
                    "api/apdSubmission/list",
                    index,
                    WebUtility.UrlEncode(traderName),
                    month,
                    year,
                    WebUtility.UrlEncode(userName),
                    WebUtility.UrlEncode(password),
                    WebUtility.UrlEncode(amIka),
                    connectionId == "undefined" ? null : connectionId);

                await SendAsync(url, traderName);

                index++;
            }

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.ApdSubmission, custActivity.ToString());

            return Json(apds);
        }
    }
}