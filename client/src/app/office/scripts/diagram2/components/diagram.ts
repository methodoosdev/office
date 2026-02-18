import { AfterViewInit, Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScriptTraderUnitOfWork } from "@officeNg";
import { ConnectionDefaults, ConnectionOptions, DiagramLayout, ShapeDefaults, ShapeOptions } from '@progress/kendo-angular-diagrams';

import { buildDiagramData, DiagramInput } from './diagram-data.builder';
import { visualCard } from './diagram-visual';

@Component({
    selector: 'script-diagram',
    templateUrl: "./diagram.html",
    encapsulation: ViewEncapsulation.None,
    styleUrls: ['./diagram.scss']
})
export class ScriptDiagramComponent implements OnInit {
    shapes: ShapeOptions[] = [];
    connections: ConnectionOptions[] = [];
    canvasW = 1200;   // will be recalculated
    canvasH = 800;

    // Vertical expansion (top → bottom)
    layout: DiagramLayout = {
        type: 'tree',
        subtype: 'right',
        horizontalSeparation: 48,
        verticalSeparation: 36
    };

    connectionDefaults: ConnectionDefaults = {
        //fromConnector: 'bottom',
        //toConnector: 'top',
        type: 'cascading',
        startCap: { type: 'None' },
        endCap: { type: 'ArrowEnd' }
    };

    shapeDefaults: ShapeDefaults = {
        visual: visualCard
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
