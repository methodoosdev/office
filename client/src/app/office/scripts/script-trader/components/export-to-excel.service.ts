import { Injectable } from "@angular/core";
import * as ExcelJS from 'exceljs';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { saveAs } from '@progress/kendo-file-saver';
import { FileRestrictions, FileInfo } from '@progress/kendo-angular-upload';

type PlaceholderMap = Record<string, string | number | boolean | null>;

@Injectable()
export class ExportToExcelService {

    loading = false;

    // The user mentioned "xls or xlsx"; ExcelJS can't read legacy .xls.
    fileRestrictions: FileRestrictions = { allowedExtensions: ['.xlsx', '.xlsm'] };

    /** Keep the uploaded prototype bytes in memory (not from /assets) */
    private prototypeBytes?: ArrayBuffer;

    constructor(private http: HttpClient) { }

    // === your method name/signature preserved ===
    async fileChange(files: File[] | FileInfo[]) {
        const rawFile =
            (Array.isArray(files) && (files as FileInfo[])[0]?.rawFile) ||
            (Array.isArray(files) && (files as File[])[0]);

        if (!rawFile) return;

        // guard: .xls is not supported by ExcelJS
        const name = (rawFile as File).name?.toLowerCase?.() || '';
        if (name.endsWith('.xls')) {
            console.warn('Legacy .xls is not supported. Please upload .xlsx or .xlsm.');
            return;
        }

        this.loading = true;
        const reader = new FileReader();

        reader.onload = async (e: any) => {
            try {
                const binary = e.target.result as string;          // you asked for binaryString flow
                const mapping = await this.getData(binary);        // steps 1–4
                await this.putValuesAndDownload(mapping);          // 1) put values  2) download
            } catch (err) {
                console.error(err);
            } finally {
                this.loading = false;
            }
        };

        // keep your original API: read as binary string
        reader.readAsBinaryString(rawFile as File);
    }

    // === your method name/signature preserved ===
    async getData(excelBinaryData: string): Promise<PlaceholderMap> {
        // 1) load prototype all cells with styles (ExcelJS preserves them)
        // Convert binary string -> ArrayBuffer for ExcelJS
        const uploadedBytes = this.bstrToArrayBuffer(excelBinaryData);
        const wb = new ExcelJS.Workbook();
        await wb.xlsx.load(uploadedBytes);

        // 2) save loaded prototype in memory for later (not from assets)
        // Keep the exact bytes the user uploaded, so every fill starts from a pristine copy
        this.prototypeBytes = uploadedBytes.slice(0);

        // 3) discover tokens #something and create the array
        const tokens = this.extractPlaceholders(wb);

        // 4) send array to server
        // adjust the URL to your API
        //const apiUrl = '/api/placeholders/resolve';
        //const mapping = await firstValueFrom(
        //    this.http.post<PlaceholderMap>(apiUrl, { placeholders: tokens })
        //);

        const result: PlaceholderMap = {
            "#21": 110.30,
            "#22": 510.40,
            "#29": 8976896510.40,
            "#pre-": 1210.70,
        };
        const mapping = await Promise.resolve(result);

        return mapping;
    }

    // === helpers ===

    /** After getData(), apply values to a FRESH copy of cached prototype and download */
    private async putValuesAndDownload(mapping: PlaceholderMap) {
        if (!this.prototypeBytes) throw new Error('Prototype not loaded in memory.');

        const wb = new ExcelJS.Workbook();
        await wb.xlsx.load(this.prototypeBytes.slice(0)); // fresh copy each time

        this.applyMappingToWorkbook(wb, mapping);

        const buf = await wb.xlsx.writeBuffer();
        const blob = new Blob([buf], {
            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });
        saveAs(blob, 'filled.xlsx'); // use Kendo saver
    }

    /** Find #tokens in strings and rich text across all sheets */
    private extractPlaceholders(wb: ExcelJS.Workbook): string[] {
        const set = new Set<string>();
        const regex = /#[A-Za-z0-9_.-]+/g;

        wb.eachSheet((ws) => {
            ws.eachRow((row) => {
                row.eachCell((cell) => {
                    const v = cell.value;
                    if (v == null) return;

                    if (typeof v === 'string') {
                        v.match(regex)?.forEach(m => set.add(m));
                    } else if (v && typeof v === 'object' && 'richText' in (v as any)) {
                        const parts = (v as any).richText as Array<{ text: string }>;
                        for (const p of parts) p.text.match(regex)?.forEach(m => set.add(m));
                    }
                });
            });
        });

        return Array.from(set).sort();
    }

    /** Replace tokens while preserving all formatting in the template */
    private applyMappingToWorkbook(wb: ExcelJS.Workbook, map: PlaceholderMap): void {
        const keys = Object.keys(map);
        if (!keys.length) return;

        const escaped = keys.map(k => k.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'));
        const big = new RegExp(escaped.join('|'), 'g');

        wb.eachSheet((ws) => {
            ws.eachRow((row) => {
                row.eachCell((cell) => {
                    const v = cell.value;

                    // Plain string cells
                    if (typeof v === 'string') {
                        if (map[v] !== undefined) {
                            const val = map[v];
                            // if cell equals exactly a token, keep native types when possible
                            if (typeof val === 'number' || typeof val === 'boolean' || val === null) {
                                cell.value = val as any;
                            } else {
                                cell.value = String(val);
                            }
                            return;
                        }
                        cell.value = v.replace(big, (m) => this.mapToString(map[m]));
                        return;
                    }

                    // Rich text runs
                    if (v && typeof v === 'object' && 'richText' in (v as any)) {
                        const rt = (v as any).richText as Array<{ text: string }>;
                        rt.forEach(part => {
                            if (map[part.text] !== undefined) {
                                part.text = this.mapToString(map[part.text]);
                            } else {
                                part.text = part.text.replace(big, (m) => this.mapToString(map[m]));
                            }
                        });
                        cell.value = { richText: rt };
                    }
                });
            });
        });
    }

    private mapToString(val: string | number | boolean | null): string {
        return val === null ? '' : String(val);
    }

    private bstrToArrayBuffer(bstr: string): ArrayBuffer {
        const buf = new ArrayBuffer(bstr.length);
        const view = new Uint8Array(buf);
        for (let i = 0; i < bstr.length; i++) view[i] = bstr.charCodeAt(i) & 0xff;
        return buf;
    }
}
