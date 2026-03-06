using App.Core.Caching;
using App.Core.Domain.Directory;

namespace App.Services.Directory
{
    /// <summary>
    /// Represents default values related to directory services
    /// </summary>
    public static partial class NopDirectoryDefaults
    {
        #region Caching defaults

        #region Countries

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Two letter ISO code
        /// </remarks>
        public static CacheKey CountriesByTwoLetterCodeCacheKey => new("Nop.country.bytwoletter.{0}", NopEntityCacheDefaults<Country>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Two letter ISO code
        /// </remarks>
        public static CacheKey CountriesByThreeLetterCodeCacheKey => new("Nop.country.bythreeletter.{0}", NopEntityCacheDefaults<Country>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : show hidden records?
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CountriesAllCacheKey => new("Nop.country.all.{0}-{1}-{2}", NopEntityCacheDefaults<Country>.Prefix);

        #endregion

        #region Currencies

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey CurrenciesAllCacheKey => new("Nop.currency.all.{0}", NopEntityCacheDefaults<Currency>.AllPrefix);

        #endregion

        #endregion
    }
}
