using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Automation
{
    internal class BrowserService : IWorkerService
    {
        public IBrowser Browser { get; internal set; } = null!;
        public Task ResetAsync() => Task.CompletedTask;
        public Task DisposeAsync() => Browser?.CloseAsync() ?? Task.CompletedTask;

        public async Task BuildAsync(PlaywrightTest playwrightInstance)
        {
            var accessToken = Environment.GetEnvironmentVariable("PLAYWRIGHT_SERVICE_ACCESS_TOKEN");
            var serviceUrl = Environment.GetEnvironmentVariable("PLAYWRIGHT_SERVICE_URL");

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(serviceUrl))
            {
                try
                {
                    Browser = await playwrightInstance!.BrowserType!.LaunchAsync(playwrightInstance.LaunchOptions()).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new AppPlaywrightException(ex.Message);
                }
            }
            else
            {
                var exposeNetwork = Environment.GetEnvironmentVariable("PLAYWRIGHT_SERVICE_EXPOSE_NETWORK") ?? "<loopback>";
                var caps = new Dictionary<string, string>
                {
                    ["os"] = Environment.GetEnvironmentVariable("PLAYWRIGHT_SERVICE_OS") ?? "linux",
                    ["runId"] = Environment.GetEnvironmentVariable("PLAYWRIGHT_SERVICE_RUN_ID") ?? DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                };
                var wsEndpoint = $"{serviceUrl}?cap={JsonSerializer.Serialize(caps)}";
                var connectOptions = new BrowserTypeConnectOptions
                {
                    Timeout = 3 * 60 * 1000,
                    ExposeNetwork = exposeNetwork,
                    Headers = new Dictionary<string, string>
                    {
                        ["x-mpt-access-key"] = accessToken
                    }
                };

                Browser = await playwrightInstance!.BrowserType!.ConnectAsync(wsEndpoint, connectOptions).ConfigureAwait(false);
            }
        }
    }
}