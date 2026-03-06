using App.Automation.Hubs;
using App.Automation.Services;
using App.Core.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace App.Automation
{
    public class PageTest : ContextTest
    {
        private IHubContext<ProgressHub> _hub;
        private bool _emulateBrowserEnable;
        private string _connectionId;
        private string _screenshotFilename;

        public IPage Page { get; private set; } = null!;
        public bool LoginIn { get; set; } = false;

        public PageTest(bool? headless = null, float? delay = null, string downLoadsPath = null, string browserName = null, bool emulateBrowserEnable = true, string connectionId = null, string screenshotFilename = null) : base(headless, delay, downLoadsPath, browserName)
        {
            _hub = EngineContext.Current.Resolve<IHubContext<ProgressHub>>();
            _emulateBrowserEnable = emulateBrowserEnable;
            _connectionId = connectionId;
            _screenshotFilename = screenshotFilename;

            PageSetup().Wait();
            _screenshotFilename = screenshotFilename;
        }

        public async Task PageSetup()
        {
            Page = await Context!.NewPageAsync().ConfigureAwait(false);
            await EmulateBrowserDialogOpen();
        }

        protected override async Task<string> GetResourceAsync(string resource)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            return await localizationService.GetResourceAsync(resource);
        }

        protected override async Task SendScreenshotAsync(ILocator locator, string method = "downloadImage")
        {
            locator = await GetLocatorAsync(locator);

            await locator.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var bytes = await locator.ScreenshotAsync();

            var base64 = Convert.ToBase64String(bytes);
            //var image = $"data:image/png;base64,{base64}";

            if (string.IsNullOrEmpty(_connectionId))
                return; //throw new Exception("Error: ConnectionId not exist.");

            _screenshotFilename = string.IsNullOrEmpty(_screenshotFilename) ? $"{this.GetType().Name}.png" : _screenshotFilename;
            await _hub.Clients.Client(_connectionId).SendAsync(method, base64, _screenshotFilename);
        }
        protected override async Task EmulateBrowserAsync()
        {
            if (string.IsNullOrEmpty(_connectionId))
                return; //throw new Exception("Error: ConnectionId not exist.");

            if (_emulateBrowserEnable)
                await SendScreenshotAsync(Page.Locator("//html/body"), "emulateBrowser");
        }
        protected override async Task EmulateBrowserDialogOpen()
        {
            if (string.IsNullOrEmpty(_connectionId))
                return; //throw new Exception("Error: ConnectionId not exist.");

            if (_emulateBrowserEnable)
                await _hub.Clients.Client(_connectionId).SendAsync("openDialog", string.Empty);
        }
        protected override async Task EmulateBrowserDialogClose()
        {
            if (string.IsNullOrEmpty(_connectionId))
                return; //throw new Exception("Error: ConnectionId not exist.");

            if (_emulateBrowserEnable)
                await _hub.Clients.Client(_connectionId).SendAsync("closeDialog", string.Empty);
        }
        public override async Task SendProgressLabelAsync(string label)
        {
            if (string.IsNullOrEmpty(_connectionId))
                return; //throw new Exception("Error: ConnectionId not exist.");

            await _hub.Clients.Client(_connectionId).SendAsync("progressLabel", label);
        }
    }
}
