using App.Web.Infra.Controllers.Common.Financial;
using Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;

public class NbgProduction
{
    private readonly string _tokenUrl = "https://my.nbg.gr/identity/connect/token";
    private readonly string _rootUrl = "https://services.nbg.gr/apis/aggregator/ais/v1";
    private readonly string _scope = "ais-aggregator-api-v1";
    private readonly string _clientId = "5E420CF5-EB54-436C-9F6D-6BC555948F80";
    private readonly string _clientSecret = "9EC66863-8577-4D31-A5E5-F6B66661FF1B";
    private readonly string _sandboxId = "sandbox_prod";
    private readonly string _callBackUrl = "https://developer.nbg.gr/oauth2/redoc-callback";
    //private string _accessToken;
    //private string _correlation_Id;
    private string _clientUserId = "client_user_id";
    private string _customerIp = "62.103.74.38";

    public NbgProduction()
    {
        //_correlation_Id = "61f37ba6-a396-4fde-80de-627f5eb8d794";//Guid.NewGuid().ToString();
    }

    public (bool Status, string Message) CheckRequest(string content, bool isSuccessful, int statusCode)
    {
        // parse the response once
        var json = JObject.Parse(content);

        // check for an “exception” object with any content
        if (json["exception"] is JObject ex && ex.HasValues)
        {
            var msg = ex["desc"]?.ToString() ?? "Unknown error";
            return (false, ($"Failed: {msg}"));
        }

        // (optional) still honour HTTP failures, if you want to catch non‑200s too
        if (!isSuccessful)
            return (false, ($"HTTP {statusCode} error: {json}"));

        return (true, string.Empty);
    }

    public string ClientId => _clientId;

    public async Task<string> AuthenticateAsync()
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

        var item = JsonConvert.DeserializeObject<object>(response.Content);

        var json = JObject.Parse(response.Content);
        var accessToken = json["access_token"].ToString();
        //{"expires_in": 1800}

        return accessToken;
    }

    private RestRequest CreateRequest(string endpoint, Method method, string accessToken)
    {
        var request = new RestRequest(endpoint) { Method = method };
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        return request;
    }

    // Client API
    public async Task<RestResponse> GetAvailableBanksAsync(string accessToken)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/client/connections", Method.Post, accessToken);
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

        return response;
    }

    // User APIs
    public async Task<RestResponse> GetUserIdAsync(string accessToken)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user", Method.Post, accessToken);
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

        return response;
    }

    public async Task<RestResponse> CurrentUserBankConnectionsAsync(string connectionId, string accessToken)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user/connections", Method.Post, accessToken);
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
            },
            payload = new
            {
                connectionId = connectionId
            }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);
                
        return response;
    }

    public async Task<RestResponse> UserConnectionsAddAsync(string accessToken, string bankBIC)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user/connections/add", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-fapi-interaction-id", guid);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("sandbox_id", _sandboxId);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            },
            payload = new
            {
                callBackUrl = _callBackUrl
            }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        return response;
    }

    public async Task<RestResponse> UserConnectionsAsync(string accessToken, string connectionId = null)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/user/connections", Method.Post, accessToken);
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
            },
            payload = string.IsNullOrEmpty(connectionId) 
                ? (object)new object() 
                : new { connectionId }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        return response;
    }

    public async Task<RestResponse> GetAccountBeneficiariesAsync(string accessToken, string bankBIC, string resourceId)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/beneficiaries", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("x-fapi-interaction-id", guid);
        request.AddHeader("sandbox_id", _sandboxId);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            },
            payload = new { resourceId }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        return response;
    }

    public async Task<RestResponse> GetAccountDetailsAsync(string accessToken, string bankBIC, string resourceId)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/details", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("x-fapi-interaction-id", guid);
        request.AddHeader("sandbox_id", _sandboxId);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            },
            payload = new { resourceId }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        return response;
    }

    public async Task<RestResponse> GetAccountListAsync(string accessToken, string bankBIC)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/list", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
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

        return response;
    }

    public async Task<RestResponse> GetAccountTransactionsAsync(string accessToken, string bankBIC, string resourceId, DateTime dateFrom, DateTime dateTo)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/transactions", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("x-fapi-interaction-id", guid);
        request.AddHeader("sandbox_id", _sandboxId);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            },
            payload = new 
            {
                resourceId = resourceId,
                dateFrom = dateFrom,
                dateTo = dateTo,
                bookingStatus = "both"
            }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        return response;
    }

    public async Task<RestResponse> GetAccountTransactionsPagedAsync(
        string accessToken, string bankBIC, string resourceId, DateTime dateFrom, DateTime dateTo,
        string paginationToken = null, int pageCount = 1)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/account/transactionsPaged", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("x-fapi-interaction-id", guid);
        request.AddHeader("sandbox_id", _sandboxId);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            },
            payload = new
            {
                resourceId = resourceId,
                dateFrom = dateFrom,
                dateTo = dateTo,
                bookingStatus = "both",
                paginationData = new
                {
                    paginationToken = paginationToken,
                    pageCount = pageCount
                }
            }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        return response;
    }

    public async Task<RestResponse> GetCardDetailsAsync(string accessToken, string bankBIC, string resourceId)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/card/details", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("x-fapi-interaction-id", guid);
        request.AddHeader("sandbox_id", _sandboxId);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            },
            payload = new { resourceId }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        return response;
    }

    public async Task<RestResponse> GetCardListAsync(string accessToken, string bankBIC)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/card/list", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
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

        return response;
    }

    public async Task<RestResponse> GetCardTransactionsAsync(string accessToken, string bankBIC, string resourceId, DateTime dateFrom, DateTime dateTo)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/card/transactions", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("x-fapi-interaction-id", guid);
        request.AddHeader("sandbox_id", _sandboxId);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            },
            payload = new
            {
                resourceId = resourceId,
                dateFrom = dateFrom,
                dateTo = dateTo
            }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        return response;
    }

    public async Task<RestResponse> GetCardTransactionsPagedAsync(
        string accessToken, string bankBIC, string resourceId, DateTime dateFrom, DateTime dateTo,
        string paginationToken = null, int pageCount = 1)
    {
        var guid = Guid.NewGuid().ToString();
        var client = new RestClient(_rootUrl);
        var request = CreateRequest("/card/transactionsPaged", Method.Post, accessToken);
        request.AddHeader("Client-Id", _clientId);
        request.AddHeader("x-client-user-id", _clientUserId);
        request.AddHeader("x-fapi-customer-ip-address", _customerIp);
        request.AddHeader("x-subject-bank-bic", bankBIC);
        request.AddHeader("x-fapi-interaction-id", guid);
        request.AddHeader("sandbox_id", _sandboxId);

        var body = new
        {
            header = new
            {
                ID = guid,
                application = _clientId
            },
            payload = new
            {
                resourceId = resourceId,
                dateFrom = dateFrom,
                dateTo = dateTo,
                paginationData = new
                {
                    paginationToken = paginationToken,
                    pageCount = pageCount
                }
            }
        };

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);

        return response;
    }

}
