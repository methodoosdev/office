using App.Core;
using App.Services.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Common
{
    public partial class PlaywrightHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        public PlaywrightHttpClient(HttpClient client, IWorkContext workContext, ILocalizationService localizationService, IWebHostEnvironment env)
        {
            client.BaseAddress = new Uri(env.IsDevelopment() ? "https://localhost:7170/" : "https://playwright-01.serefidis.local/");
            //client.BaseAddress = new Uri(env.IsDevelopment() ? "http://localhost:5232/" : "http://playwright-01.serefidis.local/");

            //this request takes some more time
            client.Timeout = TimeSpan.FromMinutes(1440); // 24 hours

            _httpClient = client;
            _workContext = workContext;
            _localizationService = localizationService;
        }

        private async Task<bool> PingAsync()
        {
            try
            {
                var text =  await _httpClient.GetStringAsync("api/account/list");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<HttpRequestMessage> RequestMessageAsync(HttpMethod httpMethod, string url, object body = null)
        {
            var json = body == null ? "{}" : JsonConvert.SerializeObject(body);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create the request message
            var request = new HttpRequestMessage(httpMethod, url)
            {
                Content = content
            };

            var lang = await _workContext.GetWorkingLanguageAsync();

            // Add a custom header to the request
            request.Headers.Add("X-Custom-Header", lang?.LanguageCulture);

            return request;
        }

        public virtual async Task<HttpClientResult> SendAsync(HttpMethod httpMethod, string url, object body = null)
        {
            var request = await RequestMessageAsync(httpMethod, url, body);

            var result = new HttpClientResult();

            if (!await PingAsync())
            {
                var errorMessage = await _localizationService.GetResourceAsync("App.Playwright.ServerNotRespond");
                result.AddError(errorMessage);
                return result;
            }

            // Send the request
            try
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    result.Content = await response.Content.ReadAsStringAsync();
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex);
                result.AddError("The request was canceled due to timeout.");
            }

            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex);
                result.AddError(ex.Message);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.AddError(ex.Message);
            }

            return result;
        }
    }
}