import { Component, ViewChild } from "@angular/core";
import { AfterModelChangeEvent, DialogResult, FormEditToken, FormListDetailComponent, AssignmentTaskActionUnitOfWork, AssignmentTaskUnitOfWork } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { TranslationService } from "@core";
import { lastValueFrom, Observable } from "rxjs";
import { DialogRef, DialogService } from "@progress/kendo-angular-dialog";
import { TaskEditDialogComponent } from "./dialog/task-edit-dialog";
import { ToastrService } from "ngx-toastr";

@Component({
    selector: "assignment-task-edit",
    templateUrl: "./task-edit.html"
})
export class AssignmentTaskEditComponent implements CanComponentDeactivate {
    @ViewChild(FormListDetailComponent) table: FormListDetailComponent | null = null;
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/assignment-task';

    parentId: number;
    assignmentTaskActionsLabel: string;
    actions: any[];

    constructor(
        private translationService: TranslationService,
        public uow: AssignmentTaskUnitOfWork,
        public assignmentTaskActionUow: AssignmentTaskActionUnitOfWork,
        private dialogService: DialogService,
        private toastrService: ToastrService) {

        this.assignmentTaskActionsLabel = this.translationService.translate('menu.assignmentTaskAction');

        const yesLabel = this.translationService.translate('common.yes');
        const noLabel = this.translationService.translate('common.cancel');

        this.actions = [
            { text: yesLabel, themeColor: "primary", dialogResult: DialogResult.Ok },
            { text: noLabel, dialogResult: DialogResult.Cancel }
        ];
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    afterModelChange(e: AfterModelChangeEvent) {
        this.parentId = e.model.id;
    }

    columnButtonClick(event: any) {
        if (event.action === 'modify') {
            this.assignmentTaskActionUow.getEntity(event.dataItem.id, null, {})
                .then((res: any) => {

                    const dialogRef: DialogRef = this.dialogService.open({
                        content: TaskEditDialogComponent,
                        actions: this.actions,
                        width: 950, height: 622, minWidth: 160, actionsLayout: 'end'
                    });

                    const dialog = dialogRef.content.instance as TaskEditDialogComponent;
                    dialog.initialize(res.formModel, res.model);

                    lastValueFrom(dialogRef.result).then((result: any) => {
                        if (result.dialogResult === DialogResult.Ok) {

                            this.assignmentTaskActionUow.edit(res.model)
                                .then(() => {
                                    const successfullySaved = this.translationService.translate('message.successfullySaved');
                                    this.toastrService.success(successfullySaved);
                                    this.table.refresh();
                                });
                        }
                    });
                })
                .catch((err: Error) => {
                    throw err;
                });
        }
    }
}
