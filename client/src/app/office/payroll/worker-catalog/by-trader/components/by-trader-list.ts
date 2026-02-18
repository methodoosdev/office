import { Component } from "@angular/core";
import { WorkerCatalogByTraderUnitOfWork } from "@officeNg";

@Component({
    selector: "worker-catalog-by-trader-list",
    templateUrl: "./by-trader-list.html"
})
export class WorkerCatalogByTraderListComponent {
    pathUrl = 'office/worker-catalog-by-trader';
    constructor(public uow: WorkerCatalogByTraderUnitOfWork) { }
 }
