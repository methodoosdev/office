using App.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;

namespace App.Playwright.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }

    }
}
