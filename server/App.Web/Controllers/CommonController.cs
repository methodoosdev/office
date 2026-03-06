using App.Core;
using App.Core.Caching;
using App.Services.Directory;
using App.Services.Localization;
using App.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CommonController : BasePublicController
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILanguageService _languageService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        public CommonController(
            ICurrencyService currencyService,
            ILanguageService languageService,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            _currencyService = currencyService;
            _languageService = languageService;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        //available even when navigation is not allowed
        [_CheckAccessPublicStore(ignore: true)]
        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> SetLanguage(string localeId)
        {
            var language = await _languageService.Table.FirstOrDefaultAsync(x => x.LanguageCulture == localeId);
            if (!language?.Published ?? false)
                language = await _workContext.GetWorkingLanguageAsync();

            await _workContext.SetWorkingLanguageAsync(language);

            return Ok();
        }

        //available even when navigation is not allowed
        [_CheckAccessPublicStore(ignore: true)]
        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> SetCurrency(int customerCurrency)
        {
            var currency = await _currencyService.GetCurrencyByIdAsync(customerCurrency);
            if (currency != null)
                await _workContext.SetWorkingCurrencyAsync(currency);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ClearCache()
        {
            await _staticCacheManager.ClearAsync();

            return Ok();
        }

    }
}