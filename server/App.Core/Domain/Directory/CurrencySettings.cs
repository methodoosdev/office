using App.Core.Configuration;

namespace App.Core.Domain.Directory
{
    /// <summary>
    /// Currency settings
    /// </summary>
    public partial class CurrencySettings : ISettings
    {
        /// <summary>
        /// A value indicating whether to display currency labels
        /// </summary>
        public bool DisplayCurrencyLabel { get; set; }

        /// <summary>
        /// Primary store currency identifier
        /// </summary>
        public int PrimaryStoreCurrencyId { get; set; }

        /// <summary>
        ///  Primary exchange rate currency identifier
        /// </summary>
        public int PrimaryExchangeRateCurrencyId { get; set; }
    }
}