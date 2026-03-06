using App.Core;
using App.Core.Domain.Directory;
using App.Core.Domain.Localization;
using App.Services.Localization;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace App.Services.Common
{
    public partial interface IPriceFormatter 
    {
        Task<string> FormatPriceAsync(decimal price, bool showCurrency = false);
        Task<string> FormatPriceAsync(decimal price, Currency targetCurrency, bool showCurrency);
        Task<string> FormatDecimalAsync(decimal value, int? languageId = null);
        Task<string> FormatDecimalAsync(decimal value, Language language);
    }
    public partial class PriceFormatter : IPriceFormatter
    {
        private readonly CurrencySettings _currencySettings;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;

        public PriceFormatter(CurrencySettings currencySettings,
            ILanguageService languageService,
            IWorkContext workContext)
        {
            _currencySettings = currencySettings;
            _languageService = languageService;
            _workContext = workContext;
        }

        protected virtual string GetCurrencyString(decimal amount,
            bool showCurrency, Currency targetCurrency)
        {
            if (targetCurrency == null)
                throw new ArgumentNullException(nameof(targetCurrency));

            string result;
            if (!string.IsNullOrEmpty(targetCurrency.CustomFormatting))
                //custom formatting specified by a store owner
                result = amount.ToString(targetCurrency.CustomFormatting);
            else
            {
                if (!string.IsNullOrEmpty(targetCurrency.DisplayLocale))
                    //default behavior
                    result = amount.ToString("C", new CultureInfo(targetCurrency.DisplayLocale));
                else
                {
                    //not possible because "DisplayLocale" should be always specified
                    //but anyway let's just handle this behavior
                    result = $"{amount:N} ({targetCurrency.CurrencyCode})";
                    return result;
                }
            }

            //display currency code?
            if (showCurrency && _currencySettings.DisplayCurrencyLabel)
                result = $"{result} ({targetCurrency.CurrencyCode})";
            return result;
        }

        public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency = false)
        {
            return await FormatPriceAsync(price, await _workContext.GetWorkingCurrencyAsync(), showCurrency);
        }

        public virtual Task<string> FormatPriceAsync(decimal price, Currency targetCurrency, bool showCurrency)
        {
            price = Math.Round(price, 2);

            var currencyString = GetCurrencyString(price, showCurrency, targetCurrency);

            return Task.FromResult(currencyString);
        }

        public virtual async Task<string> FormatDecimalAsync(decimal value, int? languageId = null)
        {
            var language = languageId.HasValue ? await _languageService.GetLanguageByIdAsync(languageId.Value) : await _workContext.GetWorkingLanguageAsync();
            return await FormatDecimalAsync(value, language);
        }

        public virtual Task<string> FormatDecimalAsync(decimal value, Language language)
        {
            var languageCulture = string.IsNullOrEmpty(language.LanguageCulture) ? CultureInfo.CurrentCulture.Name : language.LanguageCulture;
            var currencyString = value.ToString("N2", new CultureInfo(languageCulture));

            return Task.FromResult(currencyString);
        }

    }
}