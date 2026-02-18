// src/app/chat/chat.component.ts
import { Component, OnDestroy, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { v4 as uuidv4 } from 'uuid';
import { ChatMessage, ChatRequestDto, ChatResponseDto } from '../services/models';
import { ChatSignalRService } from '../services/chat-signalr.service';
import { Subscription } from 'rxjs';

@Component({
    selector: 'chat-assistant',
    templateUrl: './chat-assistant.html',
    styles: [`
        .container { max-width: 760px; margin: 2rem auto; padding: 1rem; }
        .row { display: flex; gap: .5rem; align-items: center; }
        .small { font-size: .85rem; color: #666; }
        .thread { display: flex; flex-direction: column; gap: .5rem; }
        .msg { display: flex; }
        .msg.user { justify-content: flex-end; }
        .msg.assistant { justify-content: flex-start; }
        .bubble {
          max-width: 80%;
          border-radius: .75rem;
          padding: .5rem .75rem;
          white-space: pre-wrap;
          word-break: break-word;
        }
        .user .bubble { background: #e7f0ff; }
        .assistant .bubble { background: #f5f5f5; }
        textarea { width: 100%; min-height: 100px; }
        button { padding: .5rem .9rem; }
        .streaming { opacity: .9; border: 1px dashed #bbb; }
  `]
})
export class ChatAssistantComponent implements OnInit, OnDestroy {
    chatId = uuidv4();

    // Input areas
    userMessage = 'Ο αστυνομικός είναι μπουζούκι;';
    systemPrompt = `1.Να είσαι ευγενικός βοηθός, απάντα σύντομα στα ελληνικά.
    2.Το μπουζούκι είναι όργανο.
    3.Ο αστυνομικός είναι όργανο.
    `;

    // Conversation state
    history: ChatMessage[] = []; // shows both user and assistant turns
    streamed = '';               // live assistant tokens
    finalAnswer = '';            // final assistant answer returned from REST (optional)

    isBusy = false;

    private subs = new Subscription();

    constructor(@Inject('BASE_URL') private baseUrl: string, private hub: ChatSignalRService, private http: HttpClient) {
    }

    async ngOnInit() {
        await this.hub.connect(this.chatId);
        await this.hub.join(this.chatId);

        // SignalR events
        this.subs.add(this.hub.started.subscribe(() => {
            this.streamed = '';
            this.finalAnswer = '';
            this.isBusy = true;
        }));

        this.subs.add(this.hub.token.subscribe(({ chatId, token }) => {
            if (chatId !== this.chatId) return;
            this.streamed += token;
        }));

        this.subs.add(this.hub.completed.subscribe(({ chatId, message }) => {
            if (chatId !== this.chatId) return;
            this.isBusy = false;
            this.finalAnswer = message;
            // Append assistant final turn to history so it remains after streaming
            this.history.push({ role: 'assistant', content: message });
            this.streamed = ''; // clear streamed now that final is appended
        }));
    }

    ngOnDestroy(): void {
        this.subs.unsubscribe();
        this.hub.leave(this.chatId).catch(() => { });
    }

    private aichat(body: ChatRequestDto) {
        return this.http.post<ChatResponseDto>(`${this.baseUrl}api/chatAssistant/chat`, body, {
            withCredentials: true
        });
    }

    onEnter(e: KeyboardEvent) {
        if (e.shiftKey) return;      // allow Shift+Enter = newline

        if (e.key === 'Enter') {
            e.preventDefault();          // block newline
            this.send();
        }
    }

    send() {
        const text = this.userMessage.trim();
        if (!text) return;

        // 1) Show the user's message immediately in the thread
        this.history.push({ role: 'user', content: text });

        // 2) Trigger backend (which streams via SignalR)
        this.isBusy = true;
        this.aichat({
            chatId: this.chatId,
            userMessage: text,
            // Pass compact context (system + last turns)
            history: [
                { role: 'system', content: this.systemPrompt },
                ...this.history.slice(-8) // keep prompts lean
            ],
            systemPrompt: null
        }).subscribe({
            next: () => { /* nothing: streaming handled via SignalR */ },
            error: (err) => {
                this.isBusy = false;
                this.streamed = '';
                alert('Chat error: ' + (err?.message ?? err));
            }
        });

        // 3) Clear input box (the message still remains visible in the thread)
        this.userMessage = '';
    }

    async newChat() {
        await this.hub.leave(this.chatId).catch(() => { });
        this.chatId = uuidv4();
        await this.hub.join(this.chatId).catch(() => { });

        this.history = [];
        this.streamed = '';
        this.finalAnswer = '';
        this.isBusy = false;
    }
}
