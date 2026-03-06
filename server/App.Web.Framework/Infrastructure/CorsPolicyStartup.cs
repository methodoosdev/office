using App.Core.Infrastructure;
using App.Services.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Web.Framework.Infrastructure
{
    public class CorsPolicyStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(NopJwtDefaults.CorsPolicyName,
                    builder => builder
                        .AllowAnyOrigin()
                        //.AllowCredentials()
                        //.WithOrigins("https://localhost:7191", "http://localhost:5141") // Specify the origin of your client app
                        .AllowAnyHeader()
                        .AllowAnyMethod()); // Allow credentials for SignalR
            });
        }

        public void Configure(IApplicationBuilder application)
        {
            //application.UseHttpsRedirection();

            //Add the Authorization middleware
            application.UseCors(NopJwtDefaults.CorsPolicyName);
        }

        public int Order => 501; // Authorization should be loaded before Endpoint and after authentication
    }
}
