using App.Core.Infrastructure;
using App.Services.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring Authorization middleware on application startup
    /// </summary>
    public partial class AuthorizationStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //set default authorization
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(NopCustomerDefaults.AdministratorsRoleName, policy => policy.RequireRole(NopCustomerDefaults.AdministratorsRoleName));
            //    options.AddPolicy(NopCustomerDefaults.RegisteredRoleName, policy => policy.RequireRole(NopCustomerDefaults.RegisteredRoleName));
            //    options.AddPolicy(NopCustomerDefaults.GuestsRoleName, policy => policy.RequireRole(NopCustomerDefaults.GuestsRoleName));
            //    options.AddPolicy(NopCustomerDefaults.OfficesRoleName, policy => policy.RequireRole(NopCustomerDefaults.OfficesRoleName));
            //    options.AddPolicy(NopCustomerDefaults.EmployeesRoleName, policy => policy.RequireRole(NopCustomerDefaults.EmployeesRoleName));
            //    options.AddPolicy(NopCustomerDefaults.TradersRoleName, policy => policy.RequireRole(NopCustomerDefaults.TradersRoleName));
            //});

            //set default authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy(NopJwtDefaults.CorsPolicyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    // Add additional authorization requirements if needed
                });
            });

            //set default authorization
            //services.AddAuthorization();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //Add the Authorization middleware
            application.UseAuthorization();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 600; // Authorization should be loaded before Endpoint and after authentication
    }
}
