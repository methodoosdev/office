import { Inject, Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { ToastrService } from "ngx-toastr";
import { AuthService, AuthTokenType, TokenStoreService } from "@jwtNg";

@Injectable({
    providedIn: 'root'
})
export class ConnectionHubService {
    private options: signalR.IHttpConnectionOptions = {
        //skipNegotiation: true,
        //transport: signalR.HttpTransportType.WebSockets,
        accessTokenFactory: () => {
            return this.tokenStoreService.getRawAuthToken(AuthTokenType.AccessToken);
        }
    };
    hubConnection: signalR.HubConnection;
    private hubName = "hubs/connection";
    starting: boolean = false;

    get connectionId(): string {
        return this.hubConnection?.connectionId;
    }

    constructor(@Inject('BASE_URL') private baseUrl: string,
        private tokenStoreService: TokenStoreService,
        private toastrService: ToastrService,
        private authService: AuthService) {
    }

    create() {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(`${this.baseUrl}${this.hubName}`, this.options)
            .withAutomaticReconnect()
            .build();

        this.addWorkerScheduleHubListener();
        this.addInnerMessageHubListener();
    }

    start() {
        this.hubConnection
            .start()
            .then(() => {
                this.starting = true;
                console.log('Connection Hub started')
            })
            .catch(err => console.log('Error while starting connection: ' + err));
    }

    stop(): void {
        this.hubConnection
            .stop()
            .then(() => {
                this.starting = false;
                console.log(`Connection Hub stoped`)
            })
            .catch(err => console.log('Error while stoped connection: ' + err));
    }

    public addWorkerScheduleHubListener = () => {
        this.hubConnection.on('workerScheduleSignal', (message) => {
            this.toastrService.info(message, null, {
                disableTimeOut: true,
                closeButton: true,
                tapToDismiss: false
            });
        });
    }

    public addInnerMessageHubListener = () => {
        this.hubConnection.on('innerMessageSignal', (message) => {
            this.toastrService.info(message, null, {
                disableTimeOut: true,
                closeButton: true,
                tapToDismiss: false
            });
            setTimeout(() => {
                this.authService.logout(true);
                document.location.reload();
            }, 6000);
        });
    }
}
