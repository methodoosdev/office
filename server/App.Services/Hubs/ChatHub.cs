using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace App.Services.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub: Hub
    {
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var chatId = http?.Request.Query["chatId"].ToString();

            if (!string.IsNullOrWhiteSpace(chatId))
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId!);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception = null)
        {
            var http = Context.GetHttpContext();
            var chatId = http?.Request.Query["chatId"].ToString();

            if (!string.IsNullOrWhiteSpace(chatId))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId!);

            await base.OnDisconnectedAsync(exception);
        }

        // (Optional) Still keep methods if you ever need manual control
        public async Task JoinChat(string chatId) =>
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

        public async Task LeaveChat(string chatId) => 
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }
}
