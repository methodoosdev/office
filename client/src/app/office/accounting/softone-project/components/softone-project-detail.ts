import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { stateAnimation } from "@primeNg";
import { SoftoneProjectDetailUnitOfWork } from "@officeNg";

@Component({
    selector: "softone-project-detail",
    templateUrl: "./softone-project-detail.html",
    styleUrls: ["./softone-project-detail.scss"],
    encapsulation: ViewEncapsulation.None,
    animations: [stateAnimation]
})
export class SoftoneProjectDetailComponent implements OnInit {
    animate: boolean = true;
    parentUrl = 'office/softone-project';
    decimals: string[] = ['income', 'incomeFpa', 'expenses', 'expensesFpa', 'collection', 'payment', 'fpa'];

    title: string;
    parentId: number;
    projectId: number;
    
    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public uow: SoftoneProjectDetailUnitOfWork) {
    }

    ngOnInit(): void {
        this.route.params.forEach((params: any) => {

            const projectId = +params.projectId;
            const traderId = +params.traderId;

            this.uow.getProjectName(traderId, projectId)
                .then((result: any) => {
                    this.title = result.projectName;

                    this.projectId = projectId;
                    this.parentId = traderId;

                }).catch((error: Error) => {
                    this.router.navigate([this.parentUrl]);
                    throw error;
                });
        });
    }

    loadPropertiesEvent(searchModel: any) {
        searchModel['projectId'] = this.projectId;
    }
}
