using Microsoft.Playwright;
using System.Threading.Tasks;

namespace App.Automation
{
    public abstract class ContextTest : BrowserTest
    {
        public IBrowserContext Context { get; private set; } = null!;

        public ContextTest(bool? headless, float? delay, string downLoadsPath, string browserName) : base(headless, delay, downLoadsPath, browserName)
        {
            ContextSetup().Wait();
        }

        public async Task ContextSetup()
        {
            Context = await NewContextAsync(ContextOptions()).ConfigureAwait(false);
        }

        public virtual BrowserNewContextOptions ContextOptions()
        {
            return new()
            {
                Locale = "en-US",
                ColorScheme = ColorScheme.Light,
                IgnoreHTTPSErrors = true,
                AcceptDownloads = true
            };
        }
    }
}
