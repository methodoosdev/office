import { Component, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";

import { AuthService, CanComponentDeactivate, RoleName } from "@jwtNg";
import { ActionsMenuService, TranslationService, UtilsService } from "@core";
import {
    AfterModelChangeEvent, FormlyEditNewToken, 
    TraderUnitOfWork, EmployeesByTraderUnitOfWork, EmployeeUnitOfWork, TraderKadUnitOfWork,
    TraderBranchUnitOfWork, TraderRelationshipUnitOfWork, TraderMembershipUnitOfWork, TraderInfoUnitOfWork,
    TraderRatingUnitOfWork, TraderRatingByTraderUnitOfWork, TraderMonthlyBillingUnitOfWork, LoadModelEventEvent
} from "@officeNg";
import { Observable } from "rxjs";

@Component({
    selector: "trader-edit",
    templateUrl: "./trader-edit.html"
})
export class TraderEditComponent implements OnInit, CanComponentDeactivate {
    @ViewChild(FormlyEditNewToken, { static: true }) editForm: FormlyEditNewToken | null = null;
    parentUrl = 'office/trader';

    employeesLabel: string;
    boardMemberLabel: string;
    traderKadsLabel: string;
    traderBranchesLabel: string;
    traderRelationshipsLabel: string;
    traderMembershipsLabel: string;
    traderInfoLabel: string;
    traderRatingLabel: string;
    traderMonthlyBillingLabel: string;

    traderRatingVisible: boolean;
    hasPermissions: boolean;

    parentId: number;
    actionsMenu: any[];
    dataForm: LoadModelEventEvent;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public uow: TraderUnitOfWork,
        public employeesByTraderUow: EmployeesByTraderUnitOfWork,
        public employeeUow: EmployeeUnitOfWork,
        public traderKadUow: TraderKadUnitOfWork,
        public traderBranchUow: TraderBranchUnitOfWork,
        public traderRelationshipUow: TraderRelationshipUnitOfWork,
        public traderMembershipUow: TraderMembershipUnitOfWork,
        public traderInfoUow: TraderInfoUnitOfWork,
        public traderRatingUow: TraderRatingUnitOfWork,
        public traderRatingByTraderUow: TraderRatingByTraderUnitOfWork,
        public traderMonthlyBillingUnitOfWork: TraderMonthlyBillingUnitOfWork,
        private utils: UtilsService,
        private toastrService: ToastrService,
        private actionsMenuService: ActionsMenuService,
        private translationService: TranslationService,
        private authService: AuthService    ) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.editForm.canDeactivate();
    }

    loadModelEvent(dataForm: LoadModelEventEvent) {
        this.dataForm = dataForm;
        this.parentId = dataForm.inputs.model.id;
    }


    ngOnInit() {
        this.employeesLabel = this.translationService.translate('menu.employeesManager');
        this.boardMemberLabel = this.translationService.translate('trader.boardMember');
        this.traderKadsLabel = this.translationService.translate('trader.traderKads');
        this.traderRelationshipsLabel = this.translationService.translate('trader.traderRelationships');
        this.traderMembershipsLabel = this.translationService.translate('trader.traderMemberships');
        this.traderBranchesLabel = this.translationService.translate('trader.traderBranches');
        this.traderInfoLabel = this.translationService.translate('trader.traderInfo');
        this.traderRatingLabel = this.translationService.translate('trader.traderRating');
        this.traderMonthlyBillingLabel = this.translationService.translate('trader.traderMonthlyBilling');

        this.hasPermissions = this.authService.isAuthUserInRoles([RoleName.Administrators, RoleName.Offices]);
        this.actionsMenu = this.actionsMenuService.traderEditActionMenu();

        this.route.params.forEach((params: any) => {

            const traderId = +params.id;

            this.employeesByTraderUow.canEmployeeTraderRating(traderId)
                .then(result => {
                    this.traderRatingVisible = result.valid;
                })
                .catch((err: Error) => {
                    throw err;
                });
        });
    }

    onSelectMenu(e: any): void {
        switch (e.item.id) {
            case 'fromRegistry':
                this.importPropertiesFromBusinessRegistry();
                break;

            case 'importBoardMember':
                this.importBoardMember();
                break;
        }
    }

    importPropertiesFromBusinessRegistry(): void {
        const vat = this.dataForm.inputs.model['vat'];
        if (!this.utils.vatValidation(vat)) {
            this.toastrService.error(this.translationService.translate('error.vatValidation'));
            return;
        }

        const url = `${this.uow.baseUrl}api/businessRegistry/import?afmCalledFor=${vat}`;

        this.uow.httpClient.get(url).toPromise()
            .then((result: any) => {
                const form = this.dataForm.inputs.form;

                if (result.vat) {
                    form.get('vat').setValue(result.vat);
                    form.get('lastName').setValue(result.lastName);
                    form.get('doy').setValue(result.doy);
                    form.get('tradeName').setValue(result.tradeName);
                    form.get('jobAddress').setValue(result.jobAddress);
                    form.get('jobStreetNumber').setValue(result.jobStreetNumber);
                    form.get('jobCity').setValue(result.jobCity);
                    form.get('jobPostcode').setValue(result.jobPostcode);
                    form.get('startingDate').setValue(result.startingDate ? new Date(result.startingDate) : null);
                    form.get('expiryDate').setValue(result.expiryDate ? new Date(result.expiryDate) : null);
                    form.get('professionalActivity').setValue(result.professionalActivity);
                }

                form.get('activatedTypeId').setValue(result.activatedTypeId);
                form.get('professionTypeId').setValue(result.professionTypeId);
                form.markAsDirty();

                this.toastrService.success(this.translationService.translate('message.insertionCompleted'));
            }).catch((error: Error) => {
                throw error;
            });
    }

    importBoardMember() {
    }

    modifyTraderRelationships(data: any) {
        this.router.navigate(["office/trader-relationship", data.id, this.parentId]);
    }

    modifyTraderMemberships(data: any) {
        this.router.navigate(["office/trader-membership", data.id, this.parentId]);
    }

    modifyTraderInfo(data: any) {
        this.router.navigate(["office/trader-info", data.id, this.parentId]);
    }

    modifyTraderMonthlyBilling(data: any) {
        this.router.navigate(["office/trader-monthly-billing", data.id, this.parentId]);
    }
}
