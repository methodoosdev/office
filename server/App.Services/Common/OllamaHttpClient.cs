using App.Core.Configuration;
using App.Models.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace App.Services.Common
{
    public partial class OllamaHttpClient
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;

        public OllamaHttpClient(
            AppSettings appSettings,
            HttpClient client)
        {
            var config = appSettings.Get<OllamaConfig>();

            //configure client
            client.BaseAddress = new Uri(config.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(config.RequestTimeoutSeconds);

            _appSettings = appSettings;
            _httpClient = client;
        }

        public virtual async Task StreamChatAsync(ChatRequestDto input, Func<string, Task> onToken, CancellationToken ct = default)
        {
            var config = _appSettings.Get<OllamaConfig>();
            var msgs = new List<object>();

            if (!string.IsNullOrWhiteSpace(input.SystemPrompt))
                msgs.Add(new { role = "system", content = input.SystemPrompt });

            if (input.History is { Count: > 0 })
                msgs.AddRange(input.History.Select(m => new { role = m.Role, content = m.Content }));

            msgs.Add(new { role = "user", content = input.UserMessage });

            var body = new
            {
                model = config.LLama318b,
                messages = msgs,
                stream = true,
                options = new
                {
                    temperature = 0.3
                }
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };

            using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            // Ollama stream: κάθε γραμμή είναι JSON object.
            while (!reader.EndOfStream)
            {
                ct.ThrowIfCancellationRequested();
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    // /api/chat stream chunk σχήματα:
                    // { "message": { "role":"assistant","content":"..." }, "done": false }
                    // { "done": true, "total_duration": ..., ... }
                    using var doc = JsonDocument.Parse(line);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("message", out var msgEl) &&
                        msgEl.TryGetProperty("content", out var contentEl))
                    {
                        var delta = contentEl.GetString() ?? "";
                        if (!string.IsNullOrEmpty(delta))
                            await onToken(delta);
                    }
                    // /api/generate variant (δεν το χρειαζόμαστε τώρα, αλλά το υποστηρίζουμε):
                    else if (root.TryGetProperty("response", out var respEl))
                    {
                        var delta = respEl.GetString() ?? "";
                        if (!string.IsNullOrEmpty(delta))
                            await onToken(delta);
                    }
                }
                catch
                {
                    // αγνόησε μεμονωμένα κακά chunks, αλλά μπορείς να κάνεις log.
                }
            }
        }
    }
}