import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScriptTraderUnitOfWork } from "@officeNg";
import { ConnectionDefaults, ConnectionOptions, DiagramLayout, ShapeDefaults, ShapeOptions } from '@progress/kendo-angular-diagrams';

import { DiagramInput, buildDiagramData } from './diagram-data.builder';
import { visualCard2 } from './diagram-visual';

@Component({
    selector: 'script-diagram',
    templateUrl: "./diagram.html",
    encapsulation: ViewEncapsulation.None,
    styleUrls: ['./diagram.scss']
})
export class ScriptDiagramComponent implements OnInit {
    shapes: ShapeOptions[] = [];
    connections: ConnectionOptions[] = [];

    layout: DiagramLayout = {
        type: 'tree',
        subtype: 'right',            // vertical expansion
        horizontalSeparation: 48,
        verticalSeparation: 36
    };

    connectionDefaults: ConnectionDefaults = {
        //fromConnector: 'bottom',
        //toConnector: 'top',
        type: 'cascading'
    };

    shapeDefaults: ShapeDefaults = {
        visual: visualCard2
    };
    constructor(
        private route: ActivatedRoute,
        public uow: ScriptTraderUnitOfWork) {
    }

    ngOnInit(): void {
        const id = +this.route.snapshot.paramMap.get('id');
        const traderId = +this.route.snapshot.paramMap.get('traderId');

        this.uow.toolDiagram(id, traderId)
            .then((data: DiagramInput) => {

                const { shapes, connections } = buildDiagramData(data);
                this.shapes = shapes;
                this.connections = connections;
            })
            .catch((err: Error) => {
                Promise.reject(err);
            });
    }

}
