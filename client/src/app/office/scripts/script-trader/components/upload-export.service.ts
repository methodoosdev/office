import { Inject, Injectable } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { ToastrService } from "ngx-toastr";
import {
    FileRestrictions,
    SelectEvent,
    SuccessEvent,
    ErrorEvent,
    UploadEvent
} from "@progress/kendo-angular-upload";

interface ContractPortfolio {
    id: number;
    portfolioId: number;
    filename: string;
}

interface ContractRow {
    id: number;
    contractPortfolios?: ContractPortfolio[];
}

@Injectable()
export class UploadExportService {
    uploadSaveUrl: string;
    currentRow?: ContractRow; // row associated with the pending upload

    // 2 MB max & explicit whitelist for safety
    private static readonly MAX_FILE_SIZE_MB = 2;
    private static readonly BYTES_PER_MB = 1024 * 1024;
    private static readonly ALLOWED_EXTENSIONS = [
        '.pdf', '.doc', '.docx', '.xls', '.xlsx', '.csv', '.txt', '.png', '.jpg', '.jpeg'
    ];
    private static readonly BLOCKED_EXTENSIONS = [
        '.exe', '.bat', '.sh', '.msi', '.cmd', '.com', '.vbs'
    ];

    restrictions: FileRestrictions = {
        maxFileSize: UploadExportService.MAX_FILE_SIZE_MB * UploadExportService.BYTES_PER_MB,
        allowedExtensions: UploadExportService.ALLOWED_EXTENSIONS
    };

    constructor(
        @Inject(BASE_URL) private baseUrl: string,
        private dataService: ClientDataService,
        private localization: LocalizationService,
        private confirm: ConfirmationDialogService,
        private toastr: ToastrService
    ) {
        this.uploadSaveUrl = `${baseUrl}/contractPortfolio/contractPortfolioAdd`;
    }



    ngOnInit(): void {
        this.deleteFileLabel = this.localization.translate('common.delete');
        this.uploadLabel = this.localization.translate('pages.portfolio.upload');
        this.downloadLabel = this.localization.translate('pages.portfolio.download');

        const url = `${this.baseUrl}/contractPortfolio/getContractPortfolioColumns`;
        this.dataService
            .fetchJsonBodyGet(url)
            .then((result: any[]) => (this.columnsDetail = result))
            .catch((error: Error) => Promise.reject(error));
    }

    showOnlyDetails = (item: ContractRow): boolean => !!item?.contractPortfolios?.length;

    // Safer, scoped way to open the hidden <input type="file"> for this component only
    selectFiles(evt: Event, row: ContractRow): void {
        evt.stopImmediatePropagation();
        this.currentRow = row;
        const host = (this.uploadComponent as any)?.wrapper?.nativeElement as HTMLElement | undefined;
        const input = host?.querySelector('input[type="file"]') as HTMLInputElement | null;
        input?.click();
    }

    validateFiles(e: SelectEvent): void {
        for (const file of e.files) {
            const ext = file.name.slice(file.name.lastIndexOf('.')).toLowerCase();

            if (UploadExportService.BLOCKED_EXTENSIONS.includes(ext)) {
                e.preventDefault();
                this.toastr.error(this.localization.translate('messages.fileTypeNotAllowed'));
                continue;
            }

            if (file.size > this.restrictions.maxFileSize!) {
                e.preventDefault();
                this.toastr.error(this.localization.translate('pages.portfolio.restrictions.maxFileSize'));
            }

            if (
                this.restrictions.allowedExtensions &&
                !this.restrictions.allowedExtensions.includes(ext)
            ) {
                e.preventDefault();
                this.toastr.error(this.localization.translate('messages.fileExtensionNotAllowed'));
            }
        }
    }

    onUpload(e: UploadEvent): void {
        if (!this.currentRow) {
            // Defensive: cancel if we lost context (e.g., user closed details)
            e.preventDefault();
            this.toastr.error(this.localization.translate('messages.noRowSelected'));
            return;
        }

        // Pass row id to backend alongside the file
        e.data = { id: this.currentRow.id } as Record<string, unknown>;
    }

    onSuccess(e: SuccessEvent): void {
        this.loading = true;
        this.grid
            .reloadData()
            .then(() => this.toastr.success(this.localization.translate('messages.uploadSuccessfullyCompleted')))
            .finally(() => (this.loading = false));
    }

    onError(e: ErrorEvent): void {
        const response = e.response as any;
        const msg = (response?.error as any)?.message || response?.message || this.localization.translate('messages.operationNotPermitted');
        this.toastr.error(msg);
    }

    onComplete(): void {
        if (this.uploadComponent) {
            this.currentRow = undefined;
            this.uploadComponent.clearFiles();
        }
    }

    onCancel(): void {
        // Clear the row reference if the user cancels the selection dialog
        this.currentRow = undefined;
    }
}

