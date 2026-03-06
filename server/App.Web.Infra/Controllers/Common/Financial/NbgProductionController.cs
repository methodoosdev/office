using App.Core;
using App.Services.Banking;
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
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Financial
{
    public partial class NbgProductionController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly ISqlConnectionService _connectionService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;

        public NbgProductionController(
            ITraderService traderService,
            ILocalizationService localizationService,
            ISqlConnectionService connectionService,
            IPriceFormatter priceFormatter,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext,
            IWebHelper webHelper)
        {
            _traderService = traderService;
            _localizationService = localizationService;
            _connectionService = connectionService;
            _priceFormatter = priceFormatter;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
            _webHelper = webHelper;
        }

        private void OpenUrl(string url)
        {
            // If chrome.exe is on your PATH
            //Process.Start("chrome", $"--incognito \"{url}\"");

            // Or, if you need the full path:
            string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            Process.Start(chromePath, $"--incognito \"{url}\"");
        }

        [HttpPost]
        public async Task<IActionResult> AvailableBanks()
        {
            var api = new NbgProduction();

            var access_token = await api.AuthenticateAsync();

            var x2 = await api.GetUserIdAsync(access_token);
            var x2Check = api.CheckRequest(x2.Content, x2.IsSuccessful, (int)x2.StatusCode);
            if (!x2Check.Status) return BadRequest(x2Check.Message);
            var x2Item = JsonConvert.DeserializeObject<User>(x2.Content);

            var response = await api.GetAvailableBanksAsync(access_token);

            var check = api.CheckRequest(response.Content, response.IsSuccessful, (int)response.StatusCode);

            if (!check.Status)
                return BadRequest(check.Message);

            var item = JsonConvert.DeserializeObject<AvailableBanks>(response.Content);

            var nbg = item.Payload.First(x => x.BankBIC == "ETHNGRAA");

            var ucaResp = await api.UserConnectionsAddAsync(access_token, "ETHNGRAA"/*"ERBKGRAA"nbg.BankBIC*/);
            var ucaCheck = api.CheckRequest(ucaResp.Content, ucaResp.IsSuccessful, (int)ucaResp.StatusCode);
            if (!ucaCheck.Status) return BadRequest(ucaCheck.Message);
            var ucaItem = JsonConvert.DeserializeObject<UserConnectionsAdd>(ucaResp.Content);

            OpenUrl(ucaItem.Payload.ConnectionLink);

            var ucResp = await api.UserConnectionsAsync(access_token);
            var ucCheck = api.CheckRequest(ucResp.Content, ucResp.IsSuccessful, (int)ucResp.StatusCode);
            if (!ucCheck.Status) return BadRequest(ucCheck.Message);
            var ucItem = JsonConvert.DeserializeObject<UserConnections>(ucResp.Content);

            var alResp = await api.GetAccountListAsync(access_token, nbg.BankBIC);
            var alCheck = api.CheckRequest(alResp.Content, alResp.IsSuccessful, (int)alResp.StatusCode);
            if (!alCheck.Status) return BadRequest(alCheck.Message);
            var alItem = JsonConvert.DeserializeObject<AccountList>(alResp.Content);

            var resourceId = alItem.Payload.First().ResourceId;
            var adResp = await api.GetAccountDetailsAsync(access_token, nbg.BankBIC, resourceId);
            var adCheck = api.CheckRequest(adResp.Content, adResp.IsSuccessful, (int)adResp.StatusCode);
            if (!adCheck.Status) return BadRequest(adCheck.Message);
            var adItem = JsonConvert.DeserializeObject<AccountDetails>(adResp.Content);

            var now = DateTime.UtcNow;
            var atResp = await api.GetAccountTransactionsAsync(access_token, nbg.BankBIC, resourceId, now.AddYears(-2), now);
            var atCheck = api.CheckRequest(atResp.Content, atResp.IsSuccessful, (int)atResp.StatusCode);
            if (!atCheck.Status) return BadRequest(atCheck.Message);
            var atItem = JsonConvert.DeserializeObject<AccountTransactions>(atResp.Content);

            var atpResp = await api.GetAccountTransactionsPagedAsync(access_token, nbg.BankBIC, resourceId, now.AddYears(-2), now);
            var atpCheck = api.CheckRequest(atpResp.Content, atpResp.IsSuccessful, (int)atpResp.StatusCode);
            if (!atpCheck.Status) return BadRequest(atpCheck.Message);
            var atpItem = JsonConvert.DeserializeObject<AccountTransactionsPaged>(atpResp.Content);

            var abResp = await api.GetAccountBeneficiariesAsync(access_token, nbg.BankBIC, resourceId);
            var abCheck = api.CheckRequest(abResp.Content, abResp.IsSuccessful, (int)abResp.StatusCode);
            if (!abCheck.Status) return BadRequest(abCheck.Message);
            var abItem = JsonConvert.DeserializeObject<AccountBeneficiaries>(abResp.Content);

            var alResp1 = await api.GetCardListAsync(access_token, nbg.BankBIC);
            var alCheck1 = api.CheckRequest(alResp1.Content, alResp1.IsSuccessful, (int)alResp1.StatusCode);
            if (!alCheck1.Status) return BadRequest(alCheck1.Message);
            var alItem1 = JsonConvert.DeserializeObject<CardLists>(alResp1.Content);

            var resourceId1 = alItem1.Cards.First().ResourceId;
            var adResp1 = await api.GetCardDetailsAsync(access_token, nbg.BankBIC, resourceId1);
            var adCheck1 = api.CheckRequest(adResp1.Content, adResp1.IsSuccessful, (int)adResp1.StatusCode);
            if (!adCheck1.Status) return BadRequest(adCheck1.Message);
            var adItem1 = JsonConvert.DeserializeObject<CardDetails>(adResp1.Content);

            var atResp1 = await api.GetCardTransactionsAsync(access_token, nbg.BankBIC, resourceId1, now.AddYears(-2), now);
            var atCheck1 = api.CheckRequest(atResp1.Content, atResp1.IsSuccessful, (int)atResp1.StatusCode);
            if (!atCheck1.Status) return BadRequest(atCheck1.Message);
            var atItem1 = JsonConvert.DeserializeObject<CardTransactions>(atResp1.Content);

            var atpResp1 = await api.GetCardTransactionsPagedAsync(access_token, nbg.BankBIC, resourceId1, now.AddYears(-2), now);
            var atpCheck1 = api.CheckRequest(atpResp1.Content, atpResp1.IsSuccessful, (int)atpResp1.StatusCode);
            if (!atpCheck1.Status) return BadRequest(atpCheck1.Message);
            var atpItem1 = JsonConvert.DeserializeObject<CardTransactionsPaged>(atpResp1.Content);

            Console.WriteLine("Debug");



            return Json(item);

        }
        [HttpPost]
        public async Task<IActionResult> Token()
        {
            var api = new NbgProduction();

            var access_token = await api.AuthenticateAsync();

            return Ok();


            //var api = new NbgProduction();

            //await api.AuthenticateAsync();

            //var x2 = await api.GetUserIdAsync();

            //var userBankConnectionsResp = await api.CurrentUserBankConnectionsAsync("sdfs");

            //var userBankConnectionsCheck = api.CheckRequest(userBankConnectionsResp.Content, userBankConnectionsResp.IsSuccessful, (int)userBankConnectionsResp.StatusCode);

            //if (!userBankConnectionsCheck.Status)
            //    return BadRequest(userBankConnectionsCheck.Message);

            //return Json(new { x1, x2, userBankConnectionsResp.Content });
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [CheckCustomerPermission(true)]
        [HttpPost]
        public IActionResult GetToken()
        {
            var client_id = "C47B563B-D1FC-4FF4-AF9D-DDF97FE21369";
            var redirect_uri = "https://developer.nbg.gr/oauth2/redoc-callback";

            var token_url = $"client_id={client_id}&response_type=code&scope=offline_access&redirect_uri={redirect_uri}";

            var options = new RestClientOptions("https://my.nbg.gr/identity/connect/authorize");
            using var client = new RestClient(options);

            var request = new RestRequest { Method = Method.Get }
                .AddHeader("Accept", "application/json")
                .AddHeader("content-type", "application/x-www-form-urlencoded")
                .AddParameter("application/x-www-form-urlencoded", token_url, ParameterType.RequestBody);
            var response = client.Execute(request);

            //var token = JsonConvert.DeserializeObject<PiraeusBankToken>(response.Content);

            var token_url1 = $"https://my.nbg.gr/identity/connect/authorize?client_id={client_id}&response_type=code&scope=offline_access&redirect_uri={redirect_uri}";

            var client1 = new RestClient(token_url1);
            var request1 = new RestRequest { Method = Method.Get };
            var response1 = client.Execute(request1);


            return Json(new { val1 = response.Content, val2 = response1.Content });
        }
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [CheckCustomerPermission(true)]
        [HttpPost]
        public IActionResult Connect()
        {
            var client_id = "C47B563B-D1FC-4FF4-AF9D-DDF97FE21369";
            var client_secret = "728C0721-1FBE-4FD3-B18F-6AAB0523F700";
            var redirect_uri = "https://developer.nbg.gr/oauth2/redoc-callback";
            var token_url = $"client_id={client_id}&client_secret={client_secret}&grant_type=authorization_code&redirect_uri={redirect_uri}";

            var client = new RestClient("https://my.nbg.gr/identity/connect/token");
            var request = new RestRequest { Method = Method.Post };
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("accept", "application/json");
            request.AddParameter("application/x-www-form-urlencoded", token_url, ParameterType.RequestBody);
            var response = client.Execute(request);

            return Json(new { response.Content });
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [CheckCustomerPermission(true)]
        [HttpPost]
        public IActionResult Demo1()
        {
            var client_id = "C47B563B-D1FC-4FF4-AF9D-DDF97FE21369";
            var client_secret = "728C0721-1FBE-4FD3-B18F-6AAB0523F700";
            // Create a new RestClient with the token endpoint URL
            var client = new RestClient("https://my.nbg.gr/identity/connect/token");

            // Create a new POST request
            var request = new RestRequest { Method = Method.Post };

            // Add necessary headers
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            // Add the body with client credentials
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", client_id);  // Replace with your client_id
            request.AddParameter("client_secret", client_secret);  // Replace with your client_secret
            request.AddParameter("scope", "sandbox-ais-aggregator-api-v1"); // or 'ais-aggregator-api-v1' for production

            // Execute the request and get the response
            RestResponse response = client.Execute(request);

            dynamic tokenResponse = JsonConvert.DeserializeObject(response.Content);

            // Access the access_token
            string accessToken = tokenResponse.access_token;
            string ipAddress = _webHelper.GetCurrentIpAddress();

            var client1 = new RestClient("https://apis.nbg.gr/sandbox/aggregator.ais/oauth2/v1.1/accounts");

            // Create a new GET request
            var request1 = new RestRequest { Method = Method.Get };

            // Add necessary headers
            request1.AddHeader("Authorization", $"Bearer {accessToken}");  // Replace with your actual access token
            request1.AddHeader("client-id", client_id);  // Replace with your actual client_id
            request1.AddHeader("x-fapi-interaction-id", Guid.NewGuid().ToString());  // Generate a unique UUID
            request1.AddHeader("x-fapi-customer-ip-address", ipAddress);  // Replace with your customer's IP
            request1.AddHeader("accept", "application/json");

            // Execute the request and get the response
            RestResponse response1 = client1.Execute(request);




            return Json(new { response.Content });
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [CheckCustomerPermission(true)]
        [HttpPost]
        public IActionResult Demo2()
        {
            var client_id = "C47B563B-D1FC-4FF4-AF9D-DDF97FE21369";
            var client_secret = "728C0721-1FBE-4FD3-B18F-6AAB0523F700";
            // Initialize the RestClient with the accounts endpoint URL
            var client = new RestClient("https://apis.nbg.gr/sandbox/aggregator.ais/oauth2/v1.1/accounts");

            // Create a new GET request
            var request = new RestRequest { Method = Method.Get };

            // Add necessary headers
            request.AddHeader("Authorization", "Bearer your_access_token");  // Replace with your actual access token
            request.AddHeader("client-id", "your_client_id");  // Replace with your actual client_id
            request.AddHeader("x-fapi-interaction-id", Guid.NewGuid().ToString());  // Generate a unique UUID
            request.AddHeader("x-fapi-customer-ip-address", "your_customer_ip");  // Replace with your customer's IP
            request.AddHeader("accept", "application/json");

            // Execute the request and get the response
            RestResponse response = client.Execute(request);

            return Json(new { response.Content });
        }
    }
}