using App.Web.Infra.Controllers.Common.Financial;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;

public class NbgApiClient
{
    private readonly string _sandBoxClientId = "C47B563B-D1FC-4FF4-AF9D-DDF97FE21369";
    private readonly string _sandBoxClientSecret = "728C0721-1FBE-4FD3-B18F-6AAB0523F700";
    private readonly string _productionClientId = "191DED44-9FA5-4320-BD92-821969BFC9E4";
    private readonly string _productionClientSecret = "6A8BA6BF-131B-44B9-B4F0-5090B7837E30";
    private readonly string _tokenUrl = "https://my.nbg.gr/identity/connect/token";
    private readonly string _sandBoxRootUrl = "https://apis.nbg.gr/sandbox/aggregator.ais/oauth2/v1.1";
    private readonly string _productionRootUrl = "https://services.nbg.gr/apis/aggregator/ais/v1";
    private readonly string _rootUrl;
    private readonly string _sandboxId;
    private readonly string _scope;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string _accessToken;
    //private string _correlation_Id;
    private string _clientUserId = "serefidis_user_id";
    private string _customerIp = "62.103.74.38";

    public NbgApiClient(bool sandBox)
    {
        //_correlation_Id = "61f37ba6-a396-4fde-80de-627f5eb8d794";//Guid.NewGuid().ToString();
        _rootUrl = sandBox ? _sandBoxRootUrl : _productionRootUrl;
        _sandboxId = sandBox ? "sandbox_sbx" : "sandbox_prod";
        _scope = sandBox ? "sandbox-ais-aggregator-api-v1" : "ais-aggregator-api-v1";
        _clientId = sandBox ? _sandBoxClientId : _productionClientId;
        _clientSecret = sandBox ? _sandBoxClientSecret : _productionClientSecret;
    }

    public string ClientId => _clientId;

    public async Task AuthenticateAsync()
    {
        var client = new RestClient(_tokenUrl);
        var request = new RestRequest { Method = Method.Post };
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("client_id", _clientId);
        request.AddParameter("client_secret", _clientSecret);
        request.AddParameter("scope", _scope);

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception($"Failed to get access token: {response.StatusCode} {response.Content}");
        }

        var json = JObject.Parse(response.Content);
        _accessToken = json["access_token"].ToString();
    }

    private RestRequest CreateRequest(string endpoint, Method method)
    {
        var request = new RestRequest(endpoint) { Method = method };
        request.AddHeader("Authorization", $"Bearer {_accessToken}");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        return request;
    }

    // Sandbox APIs
    public async Task<string> CreateSandboxAsync(string sandboxId, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/sandbox", Method.Post);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> ExportSandboxAsync(string sandboxId)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest($"/sandbox/{sandboxId}", Method.Get);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> ImportSandboxAsync(string sandboxId, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest($"/sandbox/{sandboxId}", Method.Put);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> DeleteSandboxAsync(string sandboxId)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest($"/sandbox/{sandboxId}", Method.Delete);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> UpdateSandboxApplicationAsync(string sandboxId, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest($"/sandbox-administration/update-application/{sandboxId}", Method.Put);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    // Client API
    public async Task<string> GetAvailableBanksAsync()
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/client/connections", Method.Post);
        request.AddHeader("sandbox_id", _sandboxId);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-fapi-interaction-id", guid);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            var json = JObject.Parse(response.Content);
            throw new Exception($"Failed: {json.ToString()}");
        }

        //dynamic tokenResponse = JsonConvert.DeserializeObject(response.Content);
        //return tokenResponse;

        return response.Content.ToString();
    }

    // User APIs
    public async Task<string> GetUserIdAsync()
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user", Method.Post);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-fapi-interaction-id", guid);
        request.AddHeader("sandbox_id", _sandboxId);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        //dynamic tokenResponse = JsonConvert.DeserializeObject(response.Content);
        //return tokenResponse;

        return response.Content.ToString();
    }

    public async Task<string> CurrentUserBankConnectionsAsync(string sandboxId, string clientUserId, string customerIp, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user/connections", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> UserConnectionsAddAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user/connections/add", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> UserConnectionsDeleteAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user/connections/delete", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> UserConnectionsChangeScaMethodAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user/connections/change-sca-method", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> UserConnectionsUpdateScaStatusAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user/connections/update-sca-status", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> UserConnectionsInitScaAuthorizationAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user/connections/init-sca-authorization", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    // Account APIs
    public async Task<string> GetAccountListAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/list", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> GetAccountDetailsAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/details", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> GetAccountTransactionsAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/transactions", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> GetAccountTransactionsPagedAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/transactionsPaged", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> GetAccountBeneficiariesAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/beneficiaries", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    // Card APIs
    public async Task<string> GetCardListAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/card/list", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> GetCardDetailsAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/card/details", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> GetCardTransactionsAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/card/transactions", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }

    public async Task<string> GetCardTransactionsPagedAsync(string sandboxId, string clientUserId, string customerIp, string bankBIC, object payload)
    {
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/card/transactionsPaged", Method.Post);
        request.AddHeader("x-client-user-id", clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", sandboxId);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }
}
