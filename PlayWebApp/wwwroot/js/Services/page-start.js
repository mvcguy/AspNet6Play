// @ts-check

//
// auto register events to enable grid col configuration and allow for re-ordering of the columns
//


class PageStartEventArgs {
    /**
     * @param {string} dataSourceName
     * @param {BootstrapDataGrid[]} grids
     * @param {object[]} forms
     */
    constructor(dataSourceName, grids = undefined, forms = undefined) {
        this.dataSourceName = dataSourceName;
        this.grids = grids;
        this.forms = forms;
    }
}

class PageStart {

    constructor() {
        this.jq = $;
        this.dataEventsService = dataEventsService;
        this.Cookie = Cookie;
    }

    /**
     * @param {PageStartEventArgs} eventArgs
     */
    onPageReady(eventArgs) {

        console.log('onPageReady', eventArgs);
        //var grids = $(document).find('table');
        //console.log(grids);

        eventArgs.grids.forEach((grid) => {

            var id = grid.getProp('id');
            var dsName = grid.options.dataSource.name;

            //console.log(id, dsName);

            if (!id || !dsName) {
                console.log('no datasource found');
                return;
            }

            this.onGridDataBound(grid);

            this.dataEventsService.registerCallback(id, appDataEvents.ON_GRID_CONFIG_UPDATED,
                (ev) => this.onGridConfigurationChanged(ev), dsName);

        })

    }

    onGridConfigurationChanged(eventArgs) {
        

    };


    /**
     * @param {BootstrapDataGrid} grid
     */
    onGridDataBound(grid) {
        //
        // enables the configuration of columns
        //
        grid.configurableGrid();

        //
        // enables to re-order the columns
        //
        grid.enableColumnReordering();

        //
        // make the grid resixeable
        //
        grid.resizableGrid();
    };
}

(function () {
    dataEventsService.registerCallback("page-start-handler",
        appDataEvents.ON_PAGE_READY, (/** @type {PageStartEventArgs} */ ev) => new PageStart().onPageReady(ev), "page");
})();