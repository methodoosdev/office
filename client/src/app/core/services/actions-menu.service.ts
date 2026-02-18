import { Injectable } from "@angular/core";

import { ApiConfigService, TranslationService } from "@core";
import { RoleName, AuthService } from "@jwtNg";

@Injectable({
    providedIn: 'root'
})
export class ActionsMenuService {

    constructor(
        private translationService: TranslationService,
        private apiConfigService: ApiConfigService,
        private authService: AuthService) {
    }

    private getActionMenu(menus: any[], label: string = null, id: string = '0') {
        const user = this.authService.getAuthUser();

        let items: any[];
        if (user.systemName === RoleName.Administrators) {
            items = menus;
        } else {
            items = menus.filter(x => {
                const menuPermissions = this.apiConfigService.configuration.menus;
                return  x.separator || menuPermissions.includes(x.menu);
            });
        }

        let actionsMenu: any[];
        if (items.length > 0) {
            actionsMenu = [
                {
                    id: id,
                    text: label || this.translationService.translate('common.actions'),
                    items: items
                }
            ];
        }

        return actionsMenu;
    }

    customerActionMenu(label: string = null): any[] {
        const insertLabel = this.translationService.translate('common.insert');
        const deleteLabel = this.translationService.translate('common.delete');
        const permissionsLabel = this.translationService.translate('menu.permissions');

        const menus = [
            { text: `${insertLabel} - ${permissionsLabel}`, menu: 'Protect:CustomerPermission:ImportMapping', id: 'insertPermissions' },
            { text: `${deleteLabel} - ${permissionsLabel}`, menu: 'Protect:CustomerPermission:RemoveMapping', id: 'deletePermissions' },
        ];

        const menu = this.getActionMenu(menus, label);

        return menu;
    }

    traderListActionMenu(label: string = null): any[] {

        const menus1 = [
            { text: this.translationService.translate('trader.efkaNonSalaried'), menu: 'Protect:FinancialObligation:EfkaNonSalaried', id: 'efkaNonSalaried' },
            { text: this.translationService.translate('trader.checkConnection'), menu: 'Protect:Trader:CheckConnection', id: 'checkConnection' },
            //{ text: this.translationService.translate('menu.createEmailInfoFhmPos'), menu: 'Protect:CheckFhmPos:CreateEmail', id: 'createEmailInfoFhmPos' },
            { separator: true },
            { text: this.translationService.translate('menu.undoTraderDeletion'), menu: 'Protect:Trader:UndoTraderDeletion', id: 'undoTraderDeletion' }
        ];

        const menus2 = [
            { text: this.translationService.translate('trader.fromSrf'), menu: 'Protect:SrfTrader:Import', id: 'fromSrf' },
            { text: this.translationService.translate('trader.fromTaxSystem'), menu: 'Protect:TaxSystemTrader:Import', id: 'fromTaxSystem' },
            { text: this.translationService.translate('menu.importKeaoGredentials'), menu: 'Protect:TaxSystemTrader:ImportKeaoGredentials', id: 'importKeaoGredentials' },
            { text: this.translationService.translate('menu.importMyDataCredentials'), menu: 'Protect:MyDataCredentials:Import', id: 'importMyDataCredentials' },
            { text: this.translationService.translate('menu.importPayrollIds'), menu: 'Protect:Trader:ImportPayrollIds', id: 'importPayrollIds' },
            { separator: true },
            { text: this.translationService.translate('trader.importKadTaxisNet'), menu: 'Protect:TraderKad:Import', id: 'importKadTaxisNet' },
            { text: this.translationService.translate('trader.importKadBranchesTaxisNet'), menu: 'Protect:TraderBranch:Import', id: 'importKadBranchesTaxisNet' },
            { text: this.translationService.translate('trader.importTraderBoardMember'), menu: 'Protect:TraderBoardMember:Import', id: 'importTraderBoardMember' },
        ];

        const menu1 = this.getActionMenu(menus1, label);

        const menu2 = this.getActionMenu(menus2, this.translationService.translate('common.retrieve'), '2');

        return [...menu1, ...menu2];
    }

    traderEditActionMenu(label: string = null): any[] {

        const menus = [
            { text: this.translationService.translate('trader.fromRegistry'), menu: 'Protect:BusinessRegistry:Import', id: 'fromRegistry' },
        ];

        const menu = this.getActionMenu(menus, label);

        return menu;
    }

    emailMessageListActionMenu(label: string = null): any[] {

        const menus = [
            { text: this.translationService.translate('menu.financialObligation'), menu: 'Protect:EmailMessage:CreateFinancialObligations', id: 'financialObligation' },
        ];

        const menu = this.getActionMenu(menus, label);

        return menu;
    }
}
