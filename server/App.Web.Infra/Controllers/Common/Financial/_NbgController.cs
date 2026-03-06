using App.Core;
using App.Services.Common;
using App.Services.Helpers;
using App.Services.Hubs;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using Azure;
using MaxMind.GeoIP2.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Financial
{
    [AllowAnonymous]
    [CheckCustomerPermission(true)]
    public partial class _NbgController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly ISqlConnectionService _connectionService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;

        public _NbgController(
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