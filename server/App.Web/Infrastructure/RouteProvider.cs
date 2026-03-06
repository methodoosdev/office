using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using App.Services.Installation;
using App.Web.Framework.Mvc.Routing;
using App.Data;

namespace App.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided basic routes
    /// </summary>
    public partial class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //areas
            endpointRouteBuilder.MapControllerRoute(name: "areaRoute",
                pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");

            //home page
            endpointRouteBuilder.MapControllerRoute(name: "Homepage",
                pattern: "default",
                defaults: new { controller = "Home", action = "Index" });

            //install
            endpointRouteBuilder.MapControllerRoute(name: "Installation",
                pattern: $"{NopInstallationDefaults.InstallPath}",
                defaults: new { controller = "Install", action = "Index" });

            if (DataSettingsManager.IsDatabaseInstalled())
                endpointRouteBuilder.MapFallbackToFile("index.html"); // For Angular routing
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;

        #endregion
    }
}