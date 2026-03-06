using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Automation
{
    public abstract class BrowserTest : PlaywrightTest
    {
        public IBrowser Browser { get; private set; } = null!;
        private readonly List<IBrowserContext> _contexts = new();

        public async Task<IBrowserContext> NewContextAsync(BrowserNewContextOptions options)
        {
            var context = await Browser!.NewContextAsync(options).ConfigureAwait(false);
            _contexts.Add(context);
            return context;
        }

        public BrowserTest(bool? headless, float? delay, string downLoadsPath, string browserName) : base(headless, delay, downLoadsPath, browserName)
        {
            BrowserSetup().Wait();
        }

        public async Task BrowserSetup()
        {
            var service = await GetService<BrowserService>().ConfigureAwait(false);
            Browser = service.Browser;
        }

        // Override DisposeAsync to provide child-specific asynchronous cleanup logic
        public override async ValueTask DisposeAsync()
        {
            await LogoutAsync();

            foreach (var context in _contexts)
                await context.CloseAsync().ConfigureAwait(false);

            _contexts.Clear();

            // Call the base class DisposeAsync for parent class cleanup
            await base.DisposeAsync();
            Browser = null!;

            await EmulateBrowserDialogClose();
        }
    }
}