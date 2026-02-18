import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { UntypedFormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ScriptTraderUnitOfWork } from "@officeNg";
import { buildFormlyDyadsPackedFromList } from "./helper";

@Component({
    selector: "script-tool-list",
    templateUrl: "./tool-list.html",
    encapsulation: ViewEncapsulation.None,
    styleUrls: ['./tool-list.scss']
})
export class ScriptToolListComponent implements OnInit {
    pathUrl = 'office/script-tool';
    info: any;
    docTitle: string;
    pdfLabel: string = "Δημιουργία PDF";

    form = new UntypedFormGroup({});
    options: FormlyFormOptions = {};
    fields: FormlyFieldConfig[];
    model: any;

    landscape: boolean = false;
    landOptions = {
        value: "Α4 Κάθετα",
        data: ["Α4 Κάθετα", "Α4 Οριζόντια"]
    }

    landscapeChange(value: any): void {
        this.landscape = value == "Α4 Κάθετα" ? false : true;
    }

    fontSizeText: string = "Μέγεθος γραμματοσειράς";
    fontSize: number = 14;
    fontSizeOptions = {
        value: "Μικρό",
        data: ["Μικρό", "Μεσαίο", "Μεγάλο"]
    }

    fontSizeChange(value: any): void {
        if (value == "Μικρό")
            this.fontSize = 14;
        if (value == "Μεσαίο")
            this.fontSize = 15;
        if (value == "Μεγάλο")
            this.fontSize = 16;
    }

    constructor(
        private route: ActivatedRoute,
        public uow: ScriptTraderUnitOfWork) { }

    ngOnInit(): void {
        const id = +this.route.snapshot.paramMap.get('id');
        const traderId = +this.route.snapshot.paramMap.get('traderId');
        const config = this.route.snapshot.paramMap.get('config');

        this.uow.toolReport(id, traderId, config)
            .then((res: any) => {

                this.info = res.info;
                this.docTitle = res.title;

                const { fields, model } = buildFormlyDyadsPackedFromList(res.data, this.model, {
                    rowClassName: 'k-gradient p-3 mb-3 ',
                    packDirection: 'ltr'
                });

                this.fields = fields;
                this.model = model;
            })
            .catch((err: Error) => {
                Promise.reject(err);
            });
    }
}
