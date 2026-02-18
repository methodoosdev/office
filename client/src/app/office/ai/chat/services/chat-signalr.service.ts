// src/app/services/chat-signalr.service.ts
import { Injectable, OnDestroy, Inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject, Observable } from 'rxjs';
import { AuthTokenType, TokenStoreService } from "@jwtNg";

@Injectable({ providedIn: 'root' })
export class ChatSignalRService implements OnDestroy {
    private connection?: signalR.HubConnection;

    private started$ = new Subject<{ chatId: string }>();
    private token$ = new Subject<{ chatId: string; token: string }>();
    private done$ = new Subject<{ chatId: string; message: string }>();
    private errors$ = new Subject<any>();

    options: signalR.IHttpConnectionOptions = {
        //skipNegotiation: true,
        //transport: signalR.HttpTransportType.WebSockets,
        withCredentials: true,
        accessTokenFactory: () => {
            return this.tokenStoreService.getRawAuthToken(AuthTokenType.AccessToken);
        }
    };

    constructor(@Inject('BASE_URL') private baseUrl: string, private tokenStoreService: TokenStoreService) {
    }

    get started(): Observable<{ chatId: string }> { return this.started$.asObservable(); }
    get token(): Observable<{ chatId: string; token: string }> { return this.token$.asObservable(); }
    get completed(): Observable<{ chatId: string; message: string }> { return this.done$.asObservable(); }
    get errors(): Observable<any> { return this.errors$.asObservable(); }

    async connect(chatId: string): Promise<void> {
        // Create a per-chat connection by putting chatId in the URL
        this.connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(`${this.baseUrl}hubs/chat?chatId=${encodeURIComponent(chatId)}`, this.options)
            .withAutomaticReconnect()
            .build();

        this.connection.on('started', payload => this.started$.next(payload));
        this.connection.on('token', payload => this.token$.next(payload));
        this.connection.on('completed', payload => this.done$.next(payload));

        this.connection.onclose(err => this.errors$.next(err));
        await this.connection.start(); // OnConnectedAsync will auto-join the group
    }

    async join(chatId: string): Promise<void> {
        if (!this.connection) throw new Error('SignalR not connected');
        await this.connection.invoke('JoinChat', chatId);
    }

    async leave(chatId: string): Promise<void> {
        if (!this.connection) return;
        await this.connection.invoke('LeaveChat', chatId);
    }

    async disconnect(): Promise<void> {
        if (!this.connection) return;
        await this.connection.stop();
        this.connection = undefined;
    }

    ngOnDestroy(): void {
        this.disconnect().catch(() => { });
    }
}
