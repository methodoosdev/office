import { AfterViewInit, NgModule, Directive, OnDestroy } from '@angular/core';

@Directive({
    selector: '[stickyHeader]'
})
export class StickyHeaderComponent implements AfterViewInit, OnDestroy {

    ngAfterViewInit() {
        window.addEventListener("resize", this.scrollFixed);
        window.addEventListener("scroll", this.scrollFixed);
    }

    ngOnDestroy() {
        window.removeEventListener("resize", this.scrollFixed);
        window.removeEventListener("scroll", this.scrollFixed);
    }

    private scrollFixed() {
        const grid = <HTMLElement>document.querySelector(".k-grid");
        const header = <HTMLElement>grid.querySelector(".k-grid-header");

        const offset = window.scrollY;
        const tableOffsetTop = grid.offsetTop;
        const tableOffsetBottom = tableOffsetTop + grid.clientHeight - header.clientHeight;

        if (offset < tableOffsetTop || offset > tableOffsetBottom) {
            header.classList.remove("sticky-header");
        } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom) {
            header.classList.add("sticky-header");
        }
    }
}

@Directive({
    selector: '[stickyToolbar]'
})
export class StickyToolbarComponent implements AfterViewInit, OnDestroy {
    //sidebarElement: HTMLElement;

    ngAfterViewInit() {
        window.addEventListener("resize", this.scrollFixed);
        window.addEventListener("scroll", this.scrollFixed);
        // Add event listener to table
        //this.sidebarElement = document.getElementsByClassName("layout-sidebar")[0] as HTMLElement;
        //this.sidebarElement.addEventListener("transitionend", this.widthFixed);
    }

    ngOnDestroy() {
        window.removeEventListener("resize", this.scrollFixed);
        window.removeEventListener("scroll", this.scrollFixed);
        //this.sidebarElement.removeEventListener("transitionend", this.widthFixed);
    }

    private widthFixed() {
        const sidebar = <HTMLElement>document.getElementsByClassName("layout-sidebar")[0];
        const header = <HTMLElement>document.querySelector(".main-toolbar");

        const style = window.getComputedStyle(sidebar);
        const m41 = new DOMMatrixReadOnly(style.transform).m41;

        Object.assign(header.style, {
            width: `calc(100% - ${m41 < 0 ? 0 : sidebar.offsetWidth}px)`
        });
    }

    private scrollFixed() {
        //const sidebar = <HTMLElement>document.getElementsByClassName("layout-sidebar")[0];
        const header = <HTMLElement>document.querySelector(".main-toolbar");
        const parent = header.parentElement;

        const offset = window.scrollY;
        const tableOffsetTop = parent.offsetTop;
        const tableOffsetBottom = tableOffsetTop + parent.clientHeight - header.clientHeight;

        //const style = window.getComputedStyle(sidebar);
        //const m41 = new DOMMatrixReadOnly(style.transform).m41;
        //const width = `calc(100% - ${m41 < 0 ? 0 : sidebar.offsetWidth}px)`;

        if (offset < tableOffsetTop || offset > tableOffsetBottom) {
            header.classList.remove("sticky-header");

            //Object.assign(header.style, {
            //    position: 'inherit',
            //    top: 'inherit',
            //    width: width,
            //    "z-index": 'inherit',
            //    "background-color": 'inherit'
            //});

        } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom) {
            //const sidebar = <HTMLElement>document.getElementsByClassName("layout-sidebar")[0];

            header.classList.add("sticky-header");
            //header.style.width = `calc(100% - ${sidebar.offsetWidth}px)`;

            //Object.assign(header.style, {
            //    position: 'fixed',
            //    top: '64px',
            //    width: width,
            //    "z-index": '9999',
            //    "background-color": '#e3f2fd'
            //});

        }
    }

    getTranslateXY(element: any) {
        const style = window.getComputedStyle(element)
        const matrix = new DOMMatrixReadOnly(style.transform)
        return {
            translateX: matrix.m41,
            translateY: matrix.m42
        }
    }
}

@NgModule({
    declarations: [StickyHeaderComponent, StickyToolbarComponent],
    exports: [StickyHeaderComponent, StickyToolbarComponent]
})
export class StickyModule { }
