using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Authentication
{
    public class WebSocketsMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketsMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var request = httpContext.Request;

            // web sockets cannot pass headers so we must take the access token from query param and
            // add it to the header before authentication middleware runs
            if (request.Path.StartsWithSegments("/hubs/chat", StringComparison.OrdinalIgnoreCase) &&
                request.Query.TryGetValue("access_token", out var accessToken))
            {
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            await _next(httpContext);
        }
        public async Task Invoke22(HttpContext context)
        {
            // Check if the request is for the SignalR hub
            if (context.Request.Path.StartsWithSegments("/your_signalr_hub_path"))
            {
                // Check if the access token is provided in the query parameters
                var queryAccessToken = context.Request.Query["access_token"].FirstOrDefault();

                if (!string.IsNullOrEmpty(queryAccessToken))
                {
                    // Use the provided access token
                    context.Request.Headers["Authorization"] = $"Bearer {queryAccessToken}";
                }
                else if (IsTokenExpired())
                {
                    // Refresh the access token
                    var newAccessToken = RefreshAccessToken();

                    // Update the request headers with the new token
                    context.Request.Headers["Authorization"] = $"Bearer {newAccessToken}";
                }
            }

            await _next(context);
        }

        private bool IsTokenExpired()
        {
            // Implement logic to check if the access token is expired or about to expire
            return false; // Replace with your actual logic
        }

        private string RefreshAccessToken()
        {
            // Implement logic to refresh the access token
            return "your_refreshed_access_token"; // Replace with your actual logic
        }
    }
}