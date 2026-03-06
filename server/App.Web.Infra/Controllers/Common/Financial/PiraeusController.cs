using App.Core;
using App.Models.Financial;
using App.Services.Common;
using App.Services.Helpers;
using App.Services.Hubs;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Financial
{
    public partial class PiraeusController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly ISqlConnectionService _connectionService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper; 
        private readonly IWorkContext _workContext;

        public PiraeusController(
            ITraderService traderService,
            ILocalizationService localizationService,
            ISqlConnectionService connectionService,
            IPriceFormatter priceFormatter,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _localizationService = localizationService;
            _connectionService = connectionService;
            _priceFormatter = priceFormatter;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        //private async Task<string> GetCode(string userName, string password, string connectionId)
        //{
        //    var page = new PiraeusPage(connectionId);

        //    var code = await page.Execute(userName, password);

        //    return code;
        //}

        private Task<string> GetCode(string userName, string password, string connectionId)
        {

            return Task.FromResult("");
        }

        private PiraeusBankToken GetPiraeusBankToken(string code)
        {
            var client_id = "d39a400c-ade4-4095-8828-f49a55fa5533";
            var client_secret = "P4vC1rP7jK4tP8iC4fM3tM4sN6iA1hA8eK7xG5bK0bV5yN7qI3";
            var redirect_uri = "https://www.piraeusbank.gr/serefprod/authcode.aspx";

            var token_url = $"client_id={client_id}&client_secret={client_secret}&grant_type=authorization_code&code={code}&redirect_uri={redirect_uri}";

            var options = new RestClientOptions("https://openbank.piraeusbank.gr/identityserver/connect/token");
            using var client = new RestClient(options);

            var request = new RestRequest { Method = Method.Post }
                .AddHeader("Accept", "application/json")
                .AddHeader("content-type", "application/x-www-form-urlencoded")
                .AddParameter("application/x-www-form-urlencoded", token_url, ParameterType.RequestBody);
            var response = client.Execute(request);
            var token = JsonConvert.DeserializeObject<PiraeusBankToken>(response.Content);

            return token;
        }

        private IList<PiraeusBankAccount> GetAccounts(PiraeusBankToken bank)
        {
            var client_id = "d39a400c-ade4-4095-8828-f49a55fa5533";

            var options = new RestClientOptions("https://api.rapidlink.piraeusbank.gr/piraeusbank/production/v1.2/assets/accounts/");
            using var client = new RestClient(options);

            var request = new RestRequest { Method = Method.Get }
                .AddHeader("Accept", "application/json")
                .AddHeader("Authorization", $"{bank.token_type} {bank.access_token}")
                .AddHeader("x-ibm-client-id", client_id);
            var response = client.Execute(request);
            var item = JsonConvert.DeserializeObject<PiraeusBankAccounts>(response.Content);

            return item.Accounts;
        }

        private IList<PiraeusBankAccountTransaction> GetTransactions(PiraeusBankToken bank, string accountId)
        {
            var client_id = "d39a400c-ade4-4095-8828-f49a55fa5533";

            var options = new RestClientOptions($"https://api.rapidlink.piraeusbank.gr/piraeusbank/production/v1.2/assets/accounts/{accountId}/transactions/{{\"fromDate\":\"2017-01-01\",\"toDate\":\"2023-06-10\", \"pageSize\":100}}");
            using var client = new RestClient(options);

            var request = new RestRequest { Method = Method.Get }
                .AddHeader("Accept", "application/json")
                .AddHeader("Authorization", $"{bank.token_type} {bank.access_token}")
                .AddHeader("x-ibm-client-id", client_id);
            var response = client.Execute(request);
            var item = JsonConvert.DeserializeObject<PiraeusBankAccountTransactions>(response.Content);

            return item.AccountTransactions;
        }
        private string GetIban(PiraeusBankToken bank)
        {
            var client_id = "d39a400c-ade4-4095-8828-f49a55fa5533";

            using var client = new RestClient("https://api.rapidlink.piraeusbank.gr/piraeusbank/production/v1.2/transactions/transferToIban/execute");

            var request = new RestRequest { Method = Method.Post }
                .AddHeader("Accept", "application/json")
                .AddHeader("content-type", "application/json")
                .AddHeader("Authorization", $"{bank.token_type} {bank.access_token}")
                .AddHeader("x-ibm-client-id", client_id)
                .AddParameter("application/json", "{\"SessionKey\":\"wawulkaz\",\"TransactionalPin\":\"tabab\"}", ParameterType.RequestBody);
            var response = client.Execute(request);
            var item = JsonConvert.DeserializeObject<object>(response.Content);

            return string.Empty;
        }


        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [CheckCustomerPermission(true)]
        [HttpPost]
        public virtual async Task<IActionResult> List(string connectionId)
        {
            var code = await GetCode("MAIN998229024", "SHIPING998229024", connectionId);

            var bank = GetPiraeusBankToken(code);

            var accounts = GetAccounts(bank);

            var transactions = GetTransactions(bank, accounts[0].AccountId);

            return Ok(transactions);
        }

    }
}