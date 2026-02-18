import { EventEmitter, Injectable, Output } from '@angular/core';
import { MessageService } from '@progress/kendo-angular-l10n';

import { elComponentMessages } from '../i18n/el';
import { enComponentMessages } from '../i18n/en';

const componentMsgs: any = {
    ['en-US']: enComponentMessages,
    ['el-GR']: elComponentMessages
};

@Injectable()
export class CustomMessageService extends MessageService {
    @Output() public localeChange = new EventEmitter();
    private localeId: string;

    public set language(value: string) {
        const locale = componentMsgs[value];
        if (locale) {
            this.localeId = value;
            this.localeChange.emit();
            this.notify();
        }
    }

    public get language(): string {
        return this.localeId;
    }

    private get messages(): any {
        const messages = componentMsgs[this.localeId];
        if (messages) {
            return messages;
        }
    }

    public override get(key: string): string {
        return this.messages[key];
    }
}
