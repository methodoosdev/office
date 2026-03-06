using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace App.Services.Banking
{
    public partial interface IBankingTransactionService
    {
        Task<string> AuthenticateAsync();
        Task<BankingResult<AvailableBanks>> GetAvailableBanksAsync(string accessToken);
        Task<BankingResult<User>> GetUserIdAsync(string accessToken, string clientUserId);
        Task<BankingResult<UserConnections>> UserConnectionsAsync(string accessToken, string clientUserId, string connectionId = null);
        Task<BankingResult<UserConnectionsAdd>> UserConnectionsAddAsync(string accessToken, string bankBIC, string clientUserId);
        Task<BankingResult<CardLists>> GetCardListAsync(string accessToken, string bankBIC, string clientUserId);
        Task<BankingResult<CardDetails>> GetCardDetailsAsync(string accessToken, string bankBIC, string clientUserId, string resourceId);
        Task<BankingResult<CardTransactions>> GetCardTransactionsAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo);
        Task<BankingResult<CardTransactionsPaged>> GetCardTransactionsPagedAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo,
            string paginationToken = null, int pageCount = 1);
        Task<BankingResult<AccountBeneficiaries>> GetAccountBeneficiariesAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId);
        Task<BankingResult<AccountDetails>> GetAccountDetailsAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId);
        Task<BankingResult<AccountList>> GetAccountListAsync(string accessToken, string bankBIC, string clientUserId);
        Task<BankingResult<AccountTransactions>> GetAccountTransactionsAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo);
        Task<BankingResult<AccountTransactionsPaged>> GetAccountTransactionsPagedAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo,
            string paginationToken = null, int pageCount = 1);
    }
    public partial class BankingTransactionService : IBankingTransactionService
    {
        private readonly string _tokenUrl = "https://my.nbg.gr/identity/connect/token";
        private readonly string _rootUrl = "https://services.nbg.gr/apis/aggregator/ais/v1";
        private readonly string _clientId = "5E420CF5-EB54-436C-9F6D-6BC555948F80";
        private readonly string _clientSecret = "9EC66863-8577-4D31-A5E5-F6B66661FF1B";
        private readonly string _sandboxId = "sandbox_prod";
        private readonly string _scope = "ais-aggregator-api-v1";
        //private readonly string _clientUserId = "serefidis_user_id";
        private readonly string _customerIp = "62.103.74.38";
        private readonly string _callBackUrl = "https://office.serefidis.gr/successful-connection";
        //private string _accessToken;

        public BankingTransactionService()
        {
        }

        public BankingResult<T> CheckRequest<T>(string content, bool isSuccessful, int statusCode) where T: class
        {
            // parse the response once
            var json = JObject.Parse(content);

            // check for an “exception” object with any content
            if (json["exception"] is JObject ex && ex.HasValues)
            {
                var msg = ex["desc"]?.ToString() ?? "Unknown error";
                return BankingResult<T>.Fail($"Failed: {msg}"); //(false, ($"Failed: {msg}"), default(T));
            }

            // (optional) still honour HTTP failures, if you want to catch non‑200s too
            if (!isSuccessful)
                return BankingResult<T>.Fail($"HTTP {statusCode} error: {json}"); //(false, ($"HTTP {statusCode} error: {json}"), default(T));

            var value = JsonConvert.DeserializeObject<T>(content);

            return BankingResult<T>.Ok(value); //(true, string.Empty, value);
        }

        public (bool Success, string Message) CheckRequest(string content, bool isSuccessful, int statusCode)
        {
            // parse the response once
            var json = JObject.Parse(content);

            // check for an “exception” object with any content
            if (json["exception"] is JObject ex && ex.HasValues)
            {
                var msg = ex["desc"]?.ToString() ?? "Unknown error";
                return (false, $"Failed: {msg}");
            }

            // (optional) still honour HTTP failures, if you want to catch non‑200s too
            if (!isSuccessful)
                return (false, $"HTTP {statusCode} error: {json}");

            return (true, string.Empty);
        }

        private RestRequest CreateRequest(string endpoint, Method method, string accessToken)
        {
            var request = new RestRequest(endpoint) { Method = method };
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            return request;
        }

        private void OpenIncognito(string url)
        {
            string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

            using (Process process = new Process())
            {
                process.StartInfo.FileName = chromePath;
                process.StartInfo.Arguments = $"--incognito \"{url}\"";
                process.StartInfo.UseShellExecute = false;

                process.Start();
            } // process is disposed here
        }

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

            var json = JObject.Parse(response.Content);
            var accessToken = json["access_token"].ToString();

            return accessToken;
        }

        #region Client APIs

        public async Task<BankingResult<AvailableBanks>> GetAvailableBanksAsync(string accessToken)
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
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<AvailableBanks>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        #endregion

        #region User APIs

        public async Task<BankingResult<User>> GetUserIdAsync(string accessToken, string clientUserId)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/user", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<User>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        public async Task<BankingResult<UserConnections>> UserConnectionsAsync(string accessToken, string clientUserId, string connectionId = null)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/user/connections", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
                    ? new object()
                    : new { connectionId }
            };

            request.AddJsonBody(body);
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<UserConnections>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        public async Task<BankingResult<UserConnectionsAdd>> UserConnectionsAddAsync(string accessToken, string bankBIC, string clientUserId)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/user/connections/add", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<UserConnectionsAdd>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        #endregion

        #region Card APIs

        public async Task<BankingResult<CardLists>> GetCardListAsync(string accessToken, string bankBIC, string clientUserId)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/card/list", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<CardLists>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        public async Task<BankingResult<CardDetails>> GetCardDetailsAsync(string accessToken, string bankBIC, string clientUserId, string resourceId)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/card/details", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<CardDetails>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        public async Task<BankingResult<CardTransactions>> GetCardTransactionsAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/card/transactions", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
                    resourceId,
                    dateFrom,
                    dateTo
                }
            };

            request.AddJsonBody(body);
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<CardTransactions>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        public async Task<BankingResult<CardTransactionsPaged>> GetCardTransactionsPagedAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo,
            string paginationToken = null, int pageCount = 1)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/card/transactionsPaged", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
                    resourceId,
                    dateFrom,
                    dateTo,
                    paginationData = new
                    {
                        paginationToken,
                        pageCount
                    }
                }
            };

            request.AddJsonBody(body);
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<CardTransactionsPaged>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        #endregion

        #region Account APIs

        public async Task<BankingResult<AccountBeneficiaries>> GetAccountBeneficiariesAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/account/beneficiaries", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<AccountBeneficiaries>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        public async Task<BankingResult<AccountDetails>> GetAccountDetailsAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/account/details", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<AccountDetails>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        public async Task<BankingResult<AccountList>> GetAccountListAsync(string accessToken, string bankBIC, string clientUserId)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/account/list", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<AccountList>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        public async Task<BankingResult<AccountTransactions>> GetAccountTransactionsAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/account/transactions", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
                    resourceId,
                    dateFrom,
                    dateTo,
                    bookingStatus = "both"
                }
            };

            request.AddJsonBody(body);
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<AccountTransactions>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        public async Task<BankingResult<AccountTransactionsPaged>> GetAccountTransactionsPagedAsync(
            string accessToken, string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo,
            string paginationToken = null, int pageCount = 1)
        {
            var guid = Guid.NewGuid().ToString();
            var client = new RestClient(_rootUrl);
            var request = CreateRequest("/account/transactionsPaged", Method.Post, accessToken);
            request.AddHeader("Client-Id", _clientId);
            request.AddHeader("x-client-user-id", clientUserId);
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
                    resourceId,
                    dateFrom,
                    dateTo,
                    bookingStatus = "both",
                    paginationData = new
                    {
                        paginationToken,
                        pageCount
                    }
                }
            };

            request.AddJsonBody(body);
            var rest = await client.ExecuteAsync(request);

            var response = CheckRequest<AccountTransactionsPaged>(rest.Content, rest.IsSuccessful, (int)rest.StatusCode);

            return response;
        }

        #endregion
    }
}