using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace App.Services.Hubs
{
    public class CalcProgress
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly int _count;
        private int _index = 0;

        public CalcProgress(IHubContext<ChatHub> hub, int count)
        {
            _hub = hub;
            _count = count;
        }

        public async Task CalcAsync(string connectionId, string method = "calcProgress")
        {
            _index++;
            decimal value = _index * 100 / _count;
            var round = Math.Round(value, 0);
            await _hub.Clients.Client(connectionId).SendAsync(method, round);
        }
    }
}