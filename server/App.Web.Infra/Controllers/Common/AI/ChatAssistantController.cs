using App.Models.AI;
using App.Services.Common;
using App.Services.Hubs;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.AI
{
    public partial class ChatAssistantController : BaseProtectController
    {
        private readonly OllamaHttpClient _ollama;
        private readonly IHubContext<ChatHub> _hub;

        public ChatAssistantController(OllamaHttpClient ollama, IHubContext<ChatHub> hub)
        {
            _ollama = ollama;
            _hub = hub;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponseDto>> Chat([FromBody] ChatRequestDto req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.ChatId))
                return BadRequest("ChatId is required.");

            var sb = new System.Text.StringBuilder();
            long outCount = 0;

            // Στέλνουμε ένα μικρό "started" signal (προαιρετικό)
            await _hub.Clients.Group(req.ChatId).SendAsync("started", new { chatId = req.ChatId }, ct);

            await _ollama.StreamChatAsync(req, async token =>
            {
                sb.Append(token);
                outCount += token.Length; // απλό estimate
                await _hub.Clients.Group(req.ChatId).SendAsync("token", new { chatId = req.ChatId, token }, ct);
            }, ct);

            var final = sb.ToString();
            await _hub.Clients.Group(req.ChatId).SendAsync("completed", new
            {
                chatId = req.ChatId,
                message = final
            }, ct);

            return Ok(new ChatResponseDto(req.ChatId, final, outCount));
        }

    }
}