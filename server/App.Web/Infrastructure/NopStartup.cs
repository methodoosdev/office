using App.Core.Infrastructure;
using App.Web.Framework.Factories;
using App.Web.Infrastructure.Installation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Web.Infrastructure
{
    /// <summary>
    /// Represents the registering services on application startup
    /// </summary>
    public partial class NopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //installation localization service
            services.AddScoped<IInstallationLocalizationService, InstallationLocalizationService>();

            //common factories
            services.AddScoped<IAclSupportedModelFactory, AclSupportedModelFactory>();
            services.AddScoped<ILocalizedModelFactory, LocalizedModelFactory>();

            //admin factories

            //factories
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 2002;
    }
}
