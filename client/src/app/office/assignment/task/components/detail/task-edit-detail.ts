import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { AssignmentTaskActionUnitOfWork } from '@officeNg';

@Component({
    selector: 'task-edit-detail',
    templateUrl: "./task-edit-detail.html",
    //encapsulation: ViewEncapsulation.None,
    //styleUrls: 
})
export class TaskEditDetailComponent implements OnInit {
    @Input() model: any;

    gridData: any[];
    constructor(
        private uow: AssignmentTaskActionUnitOfWork) {
    }

    ngOnInit(): void {
        this.uow.loadList(this.model.id)
            .then((data: any[]) => {
                this.gridData = data;
            })
            .catch((err: Error) => {
                throw err;
            });
    }
}
