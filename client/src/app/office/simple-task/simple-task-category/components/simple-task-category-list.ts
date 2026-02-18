import { Component } from "@angular/core";
import { SimpleTaskCategoryUnitOfWork } from "@officeNg";

@Component({
    selector: "simple-task-category-list",
    templateUrl: "./simple-task-category-list.html"
})
export class SimpleTaskCategoryListComponent {
    pathUrl = 'office/simple-task-category';
    constructor(public uow: SimpleTaskCategoryUnitOfWork) { }
 }
