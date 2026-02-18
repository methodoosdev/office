import { Injectable } from '@angular/core';
import { MessageService } from '@progress/kendo-angular-l10n';
import { CustomMessageService } from './custom-message.service';

import { elCustomMessages } from '../i18n/el';
import { enCustomMessages } from '../i18n/en';

const customMsgs: any = {
    ['en-US']: enCustomMessages,
    ['el-GR']: elCustomMessages
};

@Injectable({
    providedIn: 'root'
})
export class TranslationService {
    messageService: CustomMessageService;

    constructor(messageService: MessageService) {
        this.messageService = messageService as CustomMessageService;
    }

    private isDefined(value: any): boolean {
        return typeof value !== 'undefined' && value !== null;
    }

    // Translate custom messages
    translate(word: string): string {
        const messages = customMsgs[this.messageService.language];
        return messages[word] || word;
    }

    translateWithParams(key: string, ...args: any[]): any {
        if (!key || !key.length) {
            return key;
        }

        const value = this.translate(key);

        if (this.isDefined(args[0]) && args.length) {
            return value.replace(/{(\d+)}/g, (_, index) => args[index] || '')
        }

        return value;
    }
}
