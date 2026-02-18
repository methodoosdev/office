export interface ChatMessage { role: 'system' | 'user' | 'assistant'; content: string; }
export interface ChatRequestDto {
    chatId: string;
    userMessage: string;
    history?: ChatMessage[];
    systemPrompt?: string | null;
}
export interface ChatResponseDto {
    chatId: string;
    assistantMessage: string;
    tokensOut: number;
}
