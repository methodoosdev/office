using Microsoft.Playwright;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace App.Automation
{
    public abstract class PlaywrightTest : PageBase, IAsyncDisposable
    {
        private readonly string _browserName;
        private readonly string _downLoadsPath;
        private readonly float _delay;
        private readonly bool _headless;

        private static int _workerCount = 0;
        private static readonly ConcurrentStack<Worker> _allWorkers = new();
        private Worker _currentWorker;
        private static readonly Task<IPlaywright> _playwrightTask = Microsoft.Playwright.Playwright.CreateAsync();

        public IPlaywright Playwright { get; private set; } = null!;
        public IBrowserType BrowserType { get; private set; } = null!;
        public int WorkerIndex { get => _currentWorker!.WorkerIndex; }

        public PlaywrightTest(bool? headless, float? delay, string downLoadsPath, string browserName)
        {
            _headless = headless.HasValue ? headless.Value : true;
            _delay = delay.HasValue ? delay.Value : 200;
            _downLoadsPath = downLoadsPath ?? "C:\\Temp";
            _browserName = browserName ?? Microsoft.Playwright.BrowserType.Firefox;

            Setup().Wait();
        }

        public async Task Setup()
        {
            try
            {
                Playwright = await _playwrightTask.ConfigureAwait(false);
                if (Playwright == null)
                    throw new Exception("Playwright could not be instantiated.");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            BrowserType = Playwright[_browserName];

            // get worker
            if (!_allWorkers.TryPop(out _currentWorker))
            {
                _currentWorker = new();
            }
        }

        public async Task TeardownOk()
        {
            await Task.WhenAll(_currentWorker!.InstantiatedServices.Select(x => x.ResetAsync())).ConfigureAwait(false);
            _allWorkers.Push(_currentWorker);
        }

        public async Task KillBrowser()
        {
            await Task.WhenAll(_currentWorker!.InstantiatedServices.Select(x => x.DisposeAsync())).ConfigureAwait(false);
            _currentWorker.InstantiatedServices.Clear();
        }

        public async Task<T> GetService<T>(Func<T> factory = null) where T : class, IWorkerService, new()
        {
            factory ??= () => new T();
            var serviceType = typeof(T);

            var instance = _currentWorker!.InstantiatedServices.SingleOrDefault(x => serviceType.IsInstanceOfType(x));
            if (instance == null)
            {
                instance = factory();
                await instance.BuildAsync(this).ConfigureAwait(false);
                _currentWorker.InstantiatedServices.Add(instance);
            }

            if (instance is not T)
                throw new Exception("There was a problem instantiating the service.");

            return (T)instance;
        }

        public BrowserTypeLaunchOptions LaunchOptions()
        {
            return new BrowserTypeLaunchOptions
            {
                Headless = _headless,
                SlowMo = _delay,
                DownloadsPath = _downLoadsPath,
                Args = new[] { "--disable-extensions" },
                FirefoxUserPrefs = new Dictionary<string, object>
                {
                    { "browser.download.folderList", 2 },
                    { "browser.download.manager.showWhenStarting", false },
                    { "browser.download.dir", _downLoadsPath },
                    { "browser.helperApps.neverAsk.openFile", "text/csv,application/x-msexcel,application/excel,application/x-excel,application/vnd.ms-excel,image/png,image/jpeg,text/html,text/plain,application/msword,application/xml,application/pdf" },
                    { "browser.helperApps.neverAsk.saveToDisk", "text/csv,application/x-msexcel,application/excel,application/x-excel,application/vnd.ms-excel,image/png,image/jpeg,text/html,text/plain,application/msword,application/xml,application/pdf" },
                    { "browser.helperApps.alwaysAsk.force", false },
                    { "browser.download.manager.alertOnEXEOpen", false },
                    { "browser.download.manager.focusWhenStarting", false },
                    { "browser.download.manager.useWindow", false },
                    { "browser.download.manager.showAlertOnComplete", false },
                    { "browser.download.manager.closeWhenDone", true },
                    { "browser.download.panel.shown", false },
                    { "pdfjs.disabled", true},
                    { "pdfjs.enabledCache.state", false},
                    { "plugin.scan.Acrobat", "99.0" },
                    { "plugin.scan.plid.all", false }

                    //{ "browser.download.useDownloadDir", true },
                    //{ "browser.download.directory_upgrade", true },
                }
            };
        }

        private class Worker
        {
            public int WorkerIndex { get; } = Interlocked.Increment(ref _workerCount);
            public List<IWorkerService> InstantiatedServices { get; } = new();
        }

        public ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);

        public IPageAssertions Expect(IPage page) => Assertions.Expect(page);

        public IAPIResponseAssertions Expect(IAPIResponse response) => Assertions.Expect(response);

        public virtual async ValueTask DisposeAsync()
        {
            await Task.WhenAll(_currentWorker!.InstantiatedServices.Select(x => x.DisposeAsync())).ConfigureAwait(false);
            _currentWorker.InstantiatedServices.Clear();
        }
    }
}