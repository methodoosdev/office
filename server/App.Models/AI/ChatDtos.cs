using System.Collections.Generic;

namespace App.Models.AI
{
    public record ChatMessage(string Role, string Content);
    // role: "system" | "user" | "assistant"

    public record ChatRequestDto(
        string ChatId,                    // π.χ. GUID από το client
        string UserMessage,               // τελευταίο μήνυμα του χρήστη
        List<ChatMessage> History = null,// προαιρετικό history
        string SystemPrompt = null       // προαιρετικό persona/κανόνες
    );

    public record ChatResponseDto(
        string ChatId,
        string AssistantMessage,          // πλήρης τελική απάντηση (συντίθεται απ’ τα tokens)
        long TokensOut                    // πόσα tokens περίπου έστειλε (προαιρετικό estimate)
    );
}