using System.Threading.Tasks;

namespace App.Automation.Services
{
    public partial interface ILocalizationService
    {
        Task<string> GetResourceAsync(string resourceKey, string language = null);
    }

    public class LocalizationService : ILocalizationService
    {
        public virtual Task<string> GetResourceAsync(string resourceKey, string language = null) 
        {
            return Task.FromResult(resourceKey);
        }
    }
}
