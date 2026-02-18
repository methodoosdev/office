import { Inject, Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { DialogRef, DialogService } from "@progress/kendo-angular-dialog";
import { Subject } from "rxjs";
import { AuthTokenType, TokenStoreService } from "@jwtNg";
import { ImageDialogComponent } from "@officeNg";
import { downloadFromByteArray } from "../api/download";

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    hubConnection: signalR.HubConnection;
    imageDialogRef: DialogRef;
    private hubName = "hubs/chat";
    private stream: Subject<any> = new Subject<any>();
    public data$ = this.stream.asObservable();

    private progress: Subject<number> = new Subject<number>();
    public progress$ = this.progress.asObservable();

    get connectionId(): string {
        return this.hubConnection?.connectionId;
    }

    constructor(@Inject('BASE_URL') private baseUrl: string,
        private tokenStoreService: TokenStoreService,
        private dialogService: DialogService) {
    }

    init() {
        const options: signalR.IHttpConnectionOptions = {
            //skipNegotiation: true,
            //transport: signalR.HttpTransportType.WebSockets,
            accessTokenFactory: () => {
                return this.tokenStoreService.getRawAuthToken(AuthTokenType.AccessToken);
            }
        };

        this.hubConnection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(`${this.baseUrl}${this.hubName}`, options)
            .withAutomaticReconnect()
            .build();

        this.hubConnection
            .start()
            .then(() => console.log('Connection started'))
            .catch(err => console.log('Error while starting connection: ' + err));

        this.addDownloadImageListener();
        this.addEmulateBrowserListener();
        this.addRecieveDataListener();
        this.addCalcProgressListener();
    }

    public addDownloadImageListener() {
        this.hubConnection.on('downloadImage', (base64, fileName) => {
            downloadFromByteArray(base64, fileName);
        });
    }

    public addEmulateBrowserListener() {
        this.hubConnection.on('openDialog', () => {
            this.imageDialogRef = this.dialogService.open({
                content: ImageDialogComponent,
                width: 1000, height: 750
            });
        });

        this.hubConnection.on('closeDialog', () => {
            this.imageDialogRef.close();
        });

        this.hubConnection.on('emulateBrowser', (base64) => {
            const dialog = this.imageDialogRef.content.instance as ImageDialogComponent;
            dialog.imageSrc = "data:image/png;base64," + base64;
        });
    }

    public addRecieveDataListener() {
        this.hubConnection.on('recieveData', (data: any) => {
            this.stream.next(data);
        });
    }

    public addCalcProgressListener() {
        this.hubConnection.on('calcProgress', (data) => {
            this.progress.next(data);
        });

    }

    OnDestroy(): void {
        this.hubConnection
            .stop()
            .then(() => console.log(`Connection stoped`))
            .catch(err => console.log('Error while stoped connection: ' + err));
    }
}
