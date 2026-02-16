import { Component, ElementRef, EventEmitter, Input, NgModule, Output, ViewChild, effect, model } from "@angular/core";
import { CommonModule } from "@angular/common";
import {
    FileRestrictions, SelectEvent, UploadComponent, UploadEvent, UploadModule,
    ErrorEvent as KendoErrorEvent
} from "@progress/kendo-angular-upload";
import { ToastrService } from "ngx-toastr";
import { PrimeNGConfig } from "@primeNg";

const BLOCKED_EXTENSIONS = ['.exe', '.bat', '.sh', '.msi', '.cmd', '.com', '.vbs'];

@Component({
    selector: "upload-tool",
    template: `
        <kendo-upload #element
            [ngStyle]="style"
            [autoUpload]="true"
            [multiple]="false"
            [saveUrl]="saveUrl"
            [withCredentials]="true"
            [restrictions]="restrictions"
            (select)="validateFiles($event)"
            (error)="onError($event)"
            (complete)="onComplete()"
            (upload)="onUpload($event)"
            (success)="onSuccess($event)"
            (cancel)="onCancel()">
        </kendo-upload>
  `
})
export class UploadToolComponent {
    @ViewChild(UploadComponent) upload!: UploadComponent;
    @ViewChild('element', { read: ElementRef }) element!: ElementRef;
    @Input() style: { [klass: string]: any } | null | undefined = { 'display': 'none' };
    @Input() saveUrl: string;
    @Input() maxFileSize: number = 5;
    @Input() allowedExtensions: string[] = [".xlsm", ".xlsx"];

    @Output() uploadSuccess: EventEmitter<any> = new EventEmitter<any>();

    restrictions: FileRestrictions;

    dataItemId = model(0);

    _dataItemId = effect(() => {
        const dataItemId = this.dataItemId();

        if (dataItemId > 0) {
            const input: HTMLInputElement | null = this.element.nativeElement.querySelector('input[type=file]');
            input?.click();
        }
        return dataItemId;
    });

    constructor(private config: PrimeNGConfig, private toastr: ToastrService) {
        this.restrictions = {
            maxFileSize: this.maxFileSize * 1024 * 1024,
            allowedExtensions: this.allowedExtensions
        };
    }

    validateFiles(e: SelectEvent): void {
        for (const file of e.files) {
            const ext = file.name.slice(file.name.lastIndexOf('.')).toLowerCase();

            if (BLOCKED_EXTENSIONS.includes(ext)) {
                e.preventDefault();
                this.toastr.error(this.config.getTranslation('message.fileTypeNotAllowed'));
                continue;
            }

            if (file.size > this.maxFileSize * 1024 * 1024) {
                e.preventDefault();
                this.toastr.error(`${this.config.getTranslation('message.maxFileSize')}: ${this.maxFileSize} MB`);
            }

            if (
                this.restrictions.allowedExtensions &&
                !this.restrictions.allowedExtensions.includes(ext)
            ) {
                e.preventDefault();
                this.toastr.error(this.config.getTranslation('message.fileExtensionNotAllowed'));
            }
        }
    }

    onUpload(e: UploadEvent): void {
        if (this.dataItemId() == 0) {
            // Defensive: cancel if we lost context (e.g., user closed details)
            e.preventDefault();
            this.toastr.error(this.config.getTranslation('message.noRowSelected'));
            return;
        }

        // Pass row id to backend alongside the file
        e.data = { id: this.dataItemId() } as Record<string, number>;
    }

    onSuccess(e: any): void {
        this.uploadSuccess.emit({});
    }

    onError(e: KendoErrorEvent): void {
        const response = e.response as any;
        const msg = (response?.error as any)?.message || response?.message || this.config.getTranslation('message.operationNotPermitted');
        this.toastr.error(msg);
    }

    onComplete(): void {
        if (this.upload) {
            this.dataItemId.set(0);
            this.upload.clearFiles();
        }
    }

    onCancel(): void {
        this.dataItemId.set(0);
    }
}

@NgModule({
    imports: [CommonModule, UploadModule],
    exports: [UploadToolComponent],
    declarations: [UploadToolComponent]
})
export class UploadToolModule { }
