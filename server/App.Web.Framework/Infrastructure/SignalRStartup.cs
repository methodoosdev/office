using App.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Framework.Infrastructure
{
    public class SignalRStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.Use(async (context, next) =>
            {
                try
                {
                    // Check if it's a SignalR negotiation request
                    if (context.Request.Path.StartsWithSegments("/yourHub/negotiate"))
                    {
                        // Update the access token as needed
                        // For example, fetch a new token from your token provider

                        // Check if the access token is provided in the query parameters
                        var newAccessToken = context.Request.Query["access_token"].FirstOrDefault();


                        // Modify the request headers with the new access token
                        context.Request.Headers["Authorization"] = "Bearer " + newAccessToken;
                    }

                    await next();
                }
                catch (Exception ex)
                {
                    // Handle negotiation failure
                    // Log the exception or perform any necessary actions
                    // You can also customize the response to the client if needed

                    // Optionally, prevent the connection from being established
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }
            });
        }

        public int Order => 501;
    }
}
