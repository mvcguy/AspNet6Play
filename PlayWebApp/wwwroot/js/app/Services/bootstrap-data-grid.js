// @ts-check

// const { SessionStorageService } = require("./session-storage-service");

class BSGridBase {

    /**
     * @type object
     */
    element;

    /**
     * @type BSGridBase[]
     */
    children;

    constructor() {

        // @ts-ignore
        this.jquery = jQuery;
        this.dataEventsService = dataEventsService;
        this.appDataEvents = appDataEvents;
        this.Cookie = Cookie;
        this.children = [];
    }

    notifyListeners(eventType, payload) {
        this.dataEventsService.notifyListeners(eventType, payload);
    }

    getGridSettings(gridId) {
        try {
            // debugger;
            var gridSettings = this.Cookie.getJSON(gridId);
            //console.log('GridSettings Cookie: ', gridSettings ? 'settings found' : 'no settings found!');

            return gridSettings;

        } catch (error) {
            console.log(error);
            return undefined;
        }
    };

    get width() {
        return this.element.css('width');
    }
    set width(width) {
        this.element.css('width', width);
    }

    get visible() {
        return this.element.is(':visible');
    }
    set visible(val) {
        if (val === false)
            this.element.hide()
        else
            this.element.show();
    }

    getCss(t) {
        return this.element.css(t);
    }

    setCss(k, v) {
        this.element.css(k, v);
    }

    set css(css) {
        this.element.css(css);
    }

    /**
     * 
     * @param {object[]} props 
     */
    props(props) {
        var _this = this;
        props.forEach((p) => _this.prop(p.key, p.value))
    }

    prop(key, value) {
        return this.element.attr(key, value);
    }

    /**
     * 
     * @param {string} key 
     * @returns 
     */
    getProp(key) {
        return this.element.attr(key);
    }

    find(selector) {
        return this.element.find(selector);
    }

    addClass(cssClass) {
        this.element.addClass(cssClass);
        return this;
    }

    removeClass(cssClass) {
        this.element.removeClass(cssClass);
        return this;
    }

    hasClass(cssClass) {
        return this.element.hasClass(cssClass);
    }

    setText(txt) {
        this.element.text(txt);
        return this;
    }

    getText() {
        return this.element.text();
    }

    /**
     * 
     * @param {BSGridBase} elem 
     */
    append(elem, pushToArray = true) {

        if (pushToArray) {
            this.children.push(elem);
        }

        this.element.append(elem.element);
        return this;
    }

    focus() {
        this.element.focus();
    }

    isEmptyObj(obj) {
        return Object.keys(obj).length === 0;
    }


    clone() {
        //debugger;
        var c = new BSGridBase();
        // c.element = this.element.clone();
        c.element = this.jquery(this.element[0].cloneNode());

        if (this.children.length > 0) {
            var list = this.children.map((v) => {
                var cc = v.clone();
                c.element.append(cc.element);
                return cc;
            });
            c.children = list;
        }

        return c;
    }

    /**
     * a shallow clone
     * @param {object} obj 
     * @returns 
     */
    shClone(obj) {
        if (!obj) return obj;
        return Object.assign(Object.create(Object.getPrototypeOf(obj)), obj);
    }
}

class BootstrapDataGrid extends BSGridBase {

    /**
     * @type{BSGridActions}
     */
    gridActions;

    /**
     * 
     * @param {BSGridOptions} options 
     */
    constructor(options) {
        super();

        this.options = options;
        // @ts-ignore
        this.Cookie = Cookie;

        this.head = new BSGridHeader();
        this.body = new BSGridBody();

        this.selectors = new BSGridSelectorWindowCollection();

        // @ts-ignore
        this.bootstrap = bootstrap;
        this.appActions = appActions;

        this.paginator = new BSGridPagination(
            new BSGridPaginationOptions(this.options.dataSource.name,
                new PagingMetaData(),
                (page) => this.paginatorCallback(page)));

        // @ts-ignore
        this.sessionCache = new SessionStorageService();


        this.httpClient = new BSGridHttpClient(this.sessionCache, this.options.dataSource.name);

        // this.render(); // render manually

        // this.infiniteScroller = new BSGridInfiniteScroll({ gridElement: this.element });
        this.infiniteScroller = null;


        this.gridActions = null;


    }
    /**
     * @param {number} page
     */
    paginatorCallback(page) {
        // console.log(`Page.Nbr: ${page} is requested`);
        this.fetchGridPage(page);
    }

    addHeader() {
        this.element.append(this.head.element);
    }

    addBody() {
        this.element.append(this.body.element);
    }

    render() {

        this.element = this
            .jquery('<table class="table table-bordered table-hover table-sm resizable navTable nowrap"></table>');

        var _this = this;

        _this.prop('id', _this.options.gridId);
        _this.prop('data-datasource', _this.options.dataSource.name);

        var settings = _this.getGridSettings(this.options.gridId) || {};
        _this.css = { 'width': 'inherit' };


        var gridHeaderRow = new BSGridRow({ dataSourceName: _this.options.dataSource.name, hasGridTitles: true });
        gridHeaderRow.addClass('draggable').addClass('grid-cols');

        var gridBodyRow = new BSGridRow({ isTemplateRow: true, dataSourceName: _this.options.dataSource.name });
        gridBodyRow.addClass('grid-rows');

        gridBodyRow.props([{ key: 'id', value: _this.options.gridId + "_template_row_" }])
        gridBodyRow.css = { 'display': 'none' };

        var gridColumns = _this.applyColSorting(settings);

        //
        // add row markers - this helps to improve the visual appearance of selected row
        //
        var mh = new BSGridCell();
        mh.options.isHeader = true;

        var marker = new BSGridMarker();
        var mb = new BSGridCell();
        mb.append(marker);

        gridHeaderRow.addCell(mh);
        gridBodyRow.addCell(mb);


        gridColumns.forEach(function (gc) {
            var gridCol = gc;
            var colSettings = settings[gridCol.propName] || {};

            gridCol.isHeader = true;
            var gridHeaderCell = new BSGridCell(gridCol);
            gridHeaderCell.addClass('sorting').addClass('ds-col');
            gridHeaderCell.setText(gridCol.name);
            gridHeaderCell.prop('data-th-propname', gridCol.propName);

            var gridBodyCell = new BSGridCell();


            //var cellInput = '<input class="form-control" type="' + gridCol.dataType + '" />';
            var cellInputVar = null;

            //debugger;
            if (gridCol.dataType === 'select') {
                cellInputVar = new BSGridSelect({ dataSourceName: _this.options.dataSource.name });
                gridCol.dataSource
                    .forEach((opt) => cellInputVar.append(new BSGridSelectOption(opt)));
                cellInputVar.addClass('form-select form-select-sm');
            }
            else if (gridCol.dataType === 'checkbox') {
                cellInputVar = new BSGridCheckBox({ dataSourceName: _this.options.dataSource.name });
            }
            else if (gridCol.dataType === 'selector') {
                // TODO: Fix two types of settings!!!
                var sWindow = new BSGridSelectorWindow({
                    propName: gridCol.propName,
                    containerId: _this.options.containerId,
                    urlCb: gridCol.selectorDataCB,
                    gridCols: gridCol.selectorCols
                });
                _this.selectors.add(sWindow);

                cellInputVar = new BSGridSelector({
                    dataSourceName: _this.options.dataSource.name,
                    propName: gridCol.propName,
                    btnId: "btn_" + _this.options.gridId + "_template_row_" + gridCol.propName,
                    cssClass: "form-control form-control-sm",
                    elementId: _this.options.gridId + "_template_row_" + gridCol.propName,
                    inputType: "text",
                    placeHolder: gridCol.name,
                    btnClick: (sender, e) => {
                        sWindow.grid.removeHandler(_this.appDataEvents.ON_ROW_DOUBLE_CLICKED);
                        sWindow.grid.addHandler(_this.appDataEvents.ON_ROW_DOUBLE_CLICKED, (s, ev) => sender.onItemSelected(sWindow, ev));
                        sWindow.show();
                    }
                });

            }
            else {
                cellInputVar = new BSGridTextInput({
                    dataSourceName: _this.options.dataSource.name,
                    inputType: gridCol.dataType
                });
                cellInputVar.addClass('form-control form-control-sm');
            }
            // TODO: Fix two types of settings!!!
            if (gridCol.dataType !== 'selector')
                cellInputVar.props([
                    { key: 'data-propname', value: gridCol.propName },
                    { key: 'title', value: gridCol.name },
                    { key: 'id', value: _this.options.gridId + "_template_row_" + gridCol.propName },
                    { key: 'placeholder', value: gridCol.name }
                ]);

            if (gridCol.isKey === true) {
                cellInputVar.props([
                    { key: 'readonly', value: true },
                    { key: 'data-keycolumn', value: 'true' }
                ]);
            }

            if (_this.options.isReadonly === true) {
                cellInputVar.readonly = true;
                cellInputVar.setCss('cursor', 'pointer');
                cellInputVar.setCss('user-select', 'none');
            }

            gridBodyCell.append(cellInputVar);

            //
            // sorting of the data when the header cell is clicked
            //
            _this.addSorting(gridHeaderCell);


            _this.applyColSettings(gridHeaderCell, colSettings);
            _this.applyColSettings(gridBodyCell, colSettings);
            gridHeaderRow.addCell(gridHeaderCell);
            gridBodyRow.addCell(gridBodyCell);
        });


        //
        // add grid actions toolbar
        //

        //var gridActionsRow = new BSGridRow({ dataSourceName: _this.options.dataSource.name, isActionsRow: true });
        this.gridActions = new BSGridActions();
        this.gridActions.dataSourceName = this.options.dataSource.name;
        this.gridActions.addNewRecordAction((e) => this.addEmptyRow())
            .addDeleteAction((e) => this.body.markDeleted())
            .addGridSettingsAction();
        //var colDef = new BSGridColDefinition();
        //colDef.isHeader = true;
        //colDef.colSpan = gridHeaderRow.cells.length;
        //var actionsCell = new BSGridCell(colDef);
        //actionsCell.removeClass('sorting').removeClass('ds-col').addClass('grid-toolbar');
        //actionsCell.append(gridActions);
        //gridActionsRow.addCell(actionsCell);

        //_this.head.addRow(gridActionsRow);

        _this.head.addRow(gridHeaderRow);
        _this.body.addRow(gridBodyRow)

        //
        // add header and body to the grid
        //
        _this.addHeader();
        _this.addBody();

        //
        // add actions for the grid to the container
        //
        _this.jquery('#' + _this.options.containerId).append(this.gridActions.element);

        //
        // add grid to the provided container
        //
        _this.jquery('#' + _this.options.containerId).append(_this.element);

        //
        // enable infinite scroll
        //
        if (this.options.enableInfiniteScroll === true) {
            this.infiniteScroller = new BSGridInfiniteScroll({ gridElement: this.element, httpClient: _this.httpClient });
            this.infiniteScroller.nextPageCallback = (/** @type {number} */ page) => this.paginatorCallback(page);
            this.infiniteScroller.enable();
        }

        //
        // add data to the grid
        //
        var data = _this.options.dataSource.data.initData;
        var mdata = _this.options.dataSource.data.metaData;
        this.notifyListeners(_this.appDataEvents.ON_FETCH_GRID_RECORD, { dataSourceName: _this.options.dataSource.name, eventData: { items: data, metaData: mdata } });

        //
        // notify that grid is data-bound
        //
        _this.notifyListeners(_this.appDataEvents.ON_GRID_DATA_BOUND,
            { dataSourceName: _this.options.dataSource.name, eventData: {}, source: _this });

    };

    /**
     * @param {BSGridCell} th
     */
    addSorting(th) {
        //
        // sorting of the data when the header cell is clicked
        //
        var _this = this;
        th.element.on('click', function (e) {

            var asc = true;
            if (th.hasClass('sorting_asc')) {
                th.removeClass('sorting_asc').addClass('sorting_desc');
                asc = false;
            }
            else {
                th.removeClass('sorting_desc').addClass('sorting_asc');
            }

            //
            // supports sorting on only one column.
            //
            th.element.siblings('th').removeClass('sorting_asc').removeClass('sorting_desc');

            //
            // notify that we need sorting of the column
            //
            var prop = th.getProp('data-th-propname');

            //
            // TODO: fix
            //
            th.notifyListeners(th.appDataEvents.ON_SORTING_REQUESTED,
                { dataSourceName: _this.options.dataSource.name, eventData: e, propName: prop, asc, source: _this });

        });
    }

    clearGrid() {
        this.find('.grid-row').remove();

        // remove all except the template row
        var templateRow = this.body.getTemplateRow();
        this.body.rows = [templateRow];
    };

    /**
     * Apply visibility and size settings from store cookie.
     * This helps the user not to re-arrange cols based on their needs all the time they open the screen
     * @param {BSGridCell} col
     * @param {object} settings
     */
    applyColSettings(col, settings) {

        if (this.isEmptyObj(settings)) return;

        if (settings.visible === false) {
            col.element.hide();
        }

        if (settings.width) {
            col.css = { 'position': 'relative', 'width': settings.width };
        }
    };

    /**
     * Apply column re-ordering based on the stored cookie
     * This helps the user to not re-order the columns everytime they open the screen.
     * @param {*} settings 
     * @returns {BSGridColDefinition[]}
     */
    applyColSorting(settings) {

        if (!settings || this.isEmptyObj(settings)) return this.options.colDefinition;
        var sortedCols = [];

        this.options.colDefinition.forEach((v, i) => {
            var set = settings[v.propName];
            sortedCols[set.position] = v;
        });

        return sortedCols;
    }

    /**
     * @param {object[]} data
     * @param {PagingMetaData} [metaData]
     * @returns
     */
    bindDataSource(data, metaData) {

        // debugger;
        if (!data || data.length <= 0) return;

        var pagedData = data;
        if (this.options.dataSource.isRemote === false) {
            pagedData = this.options.dataSource.getPageOfflineCB(metaData.pageIndex, this.options.dataSource.data.initData, metaData);
        }

        /**
         * @type {BSGridRow}
         */
        var lastRow = null;
        pagedData.forEach((v, i) => {
            var row = this.addNewRow(i, v, true);
            row.prop('data-rowcategory', 'PRESTINE');
            lastRow = row;
        });

        //
        // update the pagination component
        //
        if (this.options.enableInfiniteScroll == false)
            this.bindPaginator(metaData);
        else {
            this.infiniteScroller.initMetaData = metaData;
            this.infiniteScroller.initData = pagedData;
            if (lastRow) {

                this.infiniteScroller.unobserve();
                this.infiniteScroller.observe(lastRow.element[0]);
            }
        }
    }


    /**
     * @param {PagingMetaData} [paginationModel]
     */
    bindPaginator(paginationModel = new PagingMetaData()) {
        this.paginator.options.pagingMetaData = paginationModel;
        this.paginator.render();
        this.jquery('#' + this.options.containerId).append(this.paginator.element);
    }

    /**
     * @param {string | number} rowNumber
     * @param {{ [x: string]: any; }} rowData
     * @param {boolean} isExistingRecord
     */
    addNewRow(rowNumber, rowData, isExistingRecord) {
        var row = this.body.getTemplateRow().clone();
        row.options.isTemplateRow = false;

        row.prop('id', this.options.gridId + "_template_row_" + rowNumber);
        row.prop('data-index', rowNumber);
        row.addClass('grid-row');
        row.css = { 'display': 'table-row' };

        var _this = this;

        var inputs = row.getInputs();

        // debugger;

        inputs.forEach(function (v, i) {

            var input = v;

            var oldId = input.getProp('id');
            input.prop('id', oldId + "_" + rowNumber);

            var cellPropName = input.getProp('data-propname');
            // console.log('cell-pro', cellPropName);

            var cellVal = rowData[cellPropName];

            if (input.options.inputType === 'date' && cellVal) {
                var date = new Date(cellVal);

                input.val = _this.toDateDisplayFormat(date);
            }
            else if (input instanceof BSGridCheckBox
                && (cellVal === 'true' || cellVal === 'True' || cellVal === true)) {
                input.prop('checked', 'checked');
            }
            else if (cellVal !== undefined) {
                input.val = cellVal;
            }

            // debugger;
            if (isExistingRecord === false) {
                input.disabled = false;
                input.readonly = false;
            }

            input.element.on('change', (e) => {

                row.prop('data-isdirty', true);

                var rowCat = row.getProp('data-rowcategory');
                if (rowCat !== 'ADDED') {
                    row.prop('data-rowcategory', 'UPDATED');
                }

                // remove any previous errors
                input.removeClass('is-invalid').prop('title', '');

                var tooltip = _this.bootstrap.Tooltip.getInstance(e.target);
                if (tooltip)
                    tooltip.dispose();

                var rowData = row.getRowDataExt();

                row.notifyListeners(_this.appDataEvents.ON_GRID_UPDATED,
                    { dataSourceName: _this.options.dataSource.name, eventData: e });

                row.notifyListeners(_this.appDataEvents.ON_FIELD_UPDATED,
                    { dataSourceName: _this.options.dataSource.name, eventData: { row: rowData, field: input } });

                row.notifyListeners(_this.appDataEvents.ON_ROW_UPDATED,
                    { dataSourceName: _this.options.dataSource.name, eventData: rowData });

            });

            input.element.on('focus', function (e) {
                _this.body.focusRow(row);
            });

            if (input instanceof BSGridSelector) {
                // link the callback from option
                //TODO: fix the button ID
                var xId = input.btnElement.attr('id');
                input.btnElement.attr('id', xId + "_" + rowNumber)
            }

        });

        row.element.on('click', function (e) {
            _this.body.focusRow(row);
        });

        this.body.addRow(row);

        var visibleInputs = row.getVisibleInputs();

        if (visibleInputs.length > 0) {
            var lastInput = visibleInputs[visibleInputs.length - 1];

            lastInput.element.on('keydown', (e) => this.onInputKeyDown(row, e));
        }

        return row;

    };

    toDateDisplayFormat(date) {

        var day = date.getDate(),
            month = date.getMonth() + 1,
            year = date.getFullYear();

        month = (month < 10 ? "0" : "") + month;
        day = (day < 10 ? "0" : "") + day;

        return year + "-" + month + "-" + day;
    }

    toTimeDisplayFormat(date) {
        var hour = date.getHours(),
            min = date.getMinutes();

        hour = (hour < 10 ? "0" : "") + hour;
        min = (min < 10 ? "0" : "") + min;

        return hour + ":" + min;
    }

    /**
     * 
     * @param {BSGridRow} row 
     * @param {*} e 
     * @returns 
     */
    onInputKeyDown(row, e) {

        //
        // insert a new row if its the last input in the row
        //   

        if (e.which !== 9 || e.shiftKey === true) return;
        // debugger;
        var visibleRows = this.body.getVisibleRows();
        if (visibleRows.length <= 0) return;
        var lastRowIndex = visibleRows[visibleRows.length - 1].getProp('data-index');
        var parentIndex = row.getProp('data-index');

        // console.log(gridRows, currentRowIndex);
        if (lastRowIndex === parentIndex) {
            var eRow = this.addEmptyRow();
        }
    };

    addEmptyRow() {
        //var rowCount = this.jquery('#' + this.options.gridId).find('tbody>tr').length;
        var emptyRow = this.addNewRow(this.body.rows.length - 1, this.createEmptyRowData(), false);

        var inputs = emptyRow.getVisibleInputs();
        if (inputs.length > 0) {
            inputs[0].focus();
        }

        emptyRow.prop('data-rowcategory', 'ADDED');
        emptyRow.prop('data-isdirty', 'true');

        this.notifyListeners(this.appDataEvents.ON_GRID_UPDATED, { dataSourceName: this.options.dataSource.name, eventData: emptyRow });

        this.infiniteScroller.unobserve();
        this.infiniteScroller.observe(emptyRow.element[0]);

        return emptyRow;
    };

    createEmptyRowData() {
        var record = {};
        this.options.colDefinition.forEach((v, i) => { record[v.propName] = undefined })
        //debugger;
        return record;
    };

    /**
     * 
     * @param {BSGridEventArgs} eventArgs 
     * @returns 
     */
    onHeaderNext(eventArgs, fetchGrid) {

        if (!eventArgs || !eventArgs.eventData) return;

        // console.log(eventArgs);
        this.resetSorting();
        this.clearGrid();
        this.paginator.clear();

        if (fetchGrid === false) return;
        //
        // fetch grid data
        //        
        this.fetchGridPage(1);

        if (this.options.enableInfiniteScroll === true) {
            this.infiniteScroller.currentPage = 1;
        }
    };

    /**
     * @param {number} pageIndex
     */
    fetchGridPage(pageIndex) {

        if (this.options.dataSource.isRemote === true) {
            var url = this.options.dataSource.url(pageIndex);
            if (!url) return;

            var options = new BSGridHttpClientOptions(url, "GET");

            this.httpClient.get(options);
        }
        else {
            var data = this.options.dataSource.data.initData;
            var mdata = this.options.dataSource.data.metaData;
            //var cb = this.options.dataSource.getPageOfflineCB;
            //var pageData = cb(pageIndex, data, mdata);
            this.notifyListeners(this.appDataEvents.ON_FETCH_GRID_RECORD,
                {
                    dataSourceName: this.options.dataSource.name,
                    eventData: {
                        items: data,
                        metaData: new PagingMetaData(pageIndex, mdata.pageSize, mdata.totalRecords)
                    }
                });

        }

    }

    onSaveRecord(eventArgs) {

        //
        // remove rows from the grid that has been deleted
        //

        this.body.find("tr[data-rowcategory='DELETED']").remove();
        this.body.find("tr[data-rowcategory='ADDED_DELETED']").remove();

        this.body.rows
            .filter((v) => v.prop('data-rowcategory') === 'DELETED' || v.prop('data-rowcategory') === 'ADDED_DELETED')
            .forEach((v) => this.body.removeRow(v));

        //
        // when main record is saved, disable the key columns of the grid,
        //        
        this.body.rows.forEach((v) => {

            // mark all rows prestine
            v.prop('data-rowcategory', 'PRESTINE');

            // make id inputs disabled
            v.getInputs().filter((x) => x.isKeyField()).forEach((vx) => { vx.disabled = true; });
        });
    };

    /**
     * 
     * @param {BSGridEventArgs} eventArgs 
     * @returns 
     */
    onSaveError(eventArgs) {

        /*
        // Its assumed that the .net mvc api will convert the model state errors into the following format
        //
        // {
        //     "addresses.[0]": ["1"], // client row index
        //     "addresses.[1]": ["2"],
        //     "addresses.[2]": ["3"],
        //     "addresses[1].City": ["The City: field is required.", "The City: must be at least 3 and at max 128 characters long."],
        //     "addresses[1].Country": ["The Country: field is required.", "The Country: must be at least 2 and at max 128 characters long."],
        //     "addresses[1].PostalCode": ["The Postal code: field is required.", "The Postal code: must be at least 3 and at max 128 characters long."],
        //     "addresses[1].StreetAddress": ["The Street address: field is required.", "The Street address: must be at least 3 and at max 128 characters long."],
        //     "addresses[2].City": ["The City: field is required.", "The City: must be at least 3 and at max 128 characters long."],
        //     "addresses[2].Country": ["The Country: field is required.", "The Country: must be at least 2 and at max 128 characters long."],
        //     "addresses[2].PostalCode": ["The Postal code: field is required.", "The Postal code: must be at least 3 and at max 128 characters long."],
        //     "addresses[2].StreetAddress": ["The Street address: must be at least 3 and at max 128 characters long."]
        / }
        */

        if (!eventArgs || !eventArgs.eventData || !eventArgs.eventData.responseJSON) return;
        var errors = eventArgs.eventData.responseJSON;
        var dsName = this.options.dataSource.name;

        var dirtyRows = this.body.getDirtyRows();

        for (let i = 0; i < dirtyRows.length; i++) {
            //debugger;
            var errorProp = dsName + '[' + i + ']';
            var im = errors[errorProp];
            if (im && im.length > 0) {
                var clientIndex = im[0];
                var serverIndex = i;

                var errorRow = this.getRowByIndex(parseInt(clientIndex));
                if (!errorRow) continue;

                this.options.colDefinition.forEach((col, i) => {

                    // @ts-ignore
                    var propName = col.propName.toPascalCaseJson();
                    var inputError = errors[dsName + '[' + serverIndex + '].' + propName];
                    if (inputError && inputError.length > 0) {
                        var input = errorRow.find("input[data-propname=" + col.propName + "]");

                        if (!input || input.length <= 0) {
                            input = errorRow.find("select[data-propname=" + col.propName + "]");
                            console.log('select found');
                        }

                        if (input && input.length > 0) {
                            input.addClass('is-invalid');
                            //console.log(inputError);
                            var allErrors = '';
                            Array.from(inputError).forEach(function (er) {
                                allErrors += er + ' ';
                            });
                            input.attr('title', allErrors);
                            var tooltip = new this.bootstrap.Tooltip(input[0], { customClass: 'tooltip-error' });
                        }
                    }

                });
            }
        }

    }

    getRowByIndex(index) {
        return this.body.rows.find((v, i) => {
            var prop = v.getProp('data-index');
            return prop === index.toString();
        });
    }

    /**
     * @param {BSGridCell} th
     * @param {boolean} ascX
     */
    sortTable(th, ascX) {

        //  console.log('sorting', ascX);
        const getCellValue = (/** @type {BSGridRow} */ tr, /** @type {number} */ idx) => {
            var child = tr.cells[idx].element;
            // console.log('idx: ', idx,  child);
            var text = child.find('input, select').is(":checked") || child.find('input, select').val() || child.text();
            //console.log(text);
            return text;
        };


        // Returns a function responsible for sorting a specific column index 
        // (idx = columnIndex, asc = ascending order?).
        var comparer = function (/** @type {number} */ idx, /** @type {boolean} */ asc) {
            //console.log('idx: ', idx, 'asc: ', asc);
            // This is used by the array.sort() function...
            return function (/** @type {BSGridRow} */ a, /** @type {BSGridRow} */ b) {
                //console.log('a: ', a, 'b: ', b);

                // This is a transient function, that is called straight away. 
                // It allows passing in different order of args, based on 
                // the ascending/descending order.
                return function (v1, v2) {
                    //  console.log('v1: ', v1, 'v2: ', v2);
                    // sort based on a numeric or localeCompare, based on type...
                    return (v1 !== '' && v2 !== '' && !isNaN(v1) && !isNaN(v2))
                        ? v1 - v2
                        : v1.toString().localeCompare(v2);
                }(getCellValue(asc ? a : b, idx), getCellValue(asc ? b : a, idx));
            }
        };

        // do the work...
        // const table = th.closest('table');

        //debugger;
        var dataSourceName = this.options.dataSource.name;
        // console.log(rows);
        var list = this.body.rows.sort(comparer(this.head.getGridTitlesRow().cells.indexOf(th), ascX = !ascX));

        list.forEach(tr => this.body.append(tr, false));

        this.notifyListeners(appDataEvents.ON_COLS_REORDERED,
            { dataSourceName: dataSourceName, eventData: { th, asc: ascX }, source: this });

        this.notifyListeners(this.appDataEvents.ON_GRID_CONFIG_UPDATED,
            { dataSourceName: dataSourceName, eventData: { th, asc: ascX }, source: this, action: this.appActions.COL_SORTING });

    };

    /**
     * 
     * @param {BSGridEventArgs} eventArgs 
     * @returns 
     */
    onSortingRequest(eventArgs) {
        // console.log(eventArgs);

        var $target = this.jquery(eventArgs.eventData.target);

        var isTh = $target.prop('tagName').toLowerCase() === 'th';

        if (!isTh) {
            var th = $target.parents('th');
            if (!th || th.length === 0) return;

            eventArgs.eventData.target = th[0];


        }
        var thx = this.head.getGridTitlesRow().cells.find((v, i) => v.element[0] === eventArgs.eventData.target);
        // debugger;
        this.sortTable(thx, eventArgs.asc);
    };

    resetSorting() {

        this.head.rows.forEach((v, i) => {
            if (v.hasClass('sorting_desc' || v.hasClass('sorting_asc'))) {
                v.removeClass('sorting_asc').removeClass('sorting_desc');
            }
        });
    };

    /**
     * 
     * @param {BSGridEventArgs} eventArgs 
     */
    onColsReordered(eventArgs) {

        //
        // modify 'keydown' events on the row inputs
        //
        var grid = this;
        // console.log(eventArgs);


        grid.body.rows.forEach((row, i) => {
            var inputs = row.getInputs();
            inputs.forEach((inp) => { inp.element.off('keydown') });
            var visibleInputs = row.getVisibleInputs();
            if (visibleInputs.length <= 0) return;
            var lastInput = visibleInputs[visibleInputs.length - 1];
            lastInput.element.on('keydown', (e) => { this.onInputKeyDown(row, e) });
        });

    };

    registerCallback(key, eventTypeX, callback, dataSourceNameX, verifyDSName = false) {
        dataEventsService.registerCallback(key, eventTypeX, callback, dataSourceNameX, verifyDSName);
    };

    unRegisterCallback(key, eventTypeX, dataSourceNameX) {
        dataEventsService.unRegisterCallback(key, eventTypeX, dataSourceNameX);
    };

    /**
     * @param {string} eventType
     * @param {(source: BootstrapDataGrid, eventObject: object) => any} handler
     */
    addHandler(eventType, handler, verifyDSName = false) {
        var id = this.options.gridId;
        var ds = this.options.dataSource.name;
        this.registerCallback(id, eventType, (/** @type {Object} */ ev) => handler(this, ev), ds, verifyDSName);
    }

    removeHandler(eventType) {
        var id = this.options.gridId;
        var ds = this.options.dataSource.name;
        this.unRegisterCallback(id, eventType, ds);

    }

    onFetchData(eventArgs) {

        // console.log('onFetchData:', eventArgs);

        //
        // populate the grid with the fetched data
        //
        if (this.options.enableInfiniteScroll === false)
            this.clearGrid();
        var md = eventArgs.eventData.metaData;
        if (!md) return;
        this.bindDataSource(eventArgs.eventData.items, new PagingMetaData(md.pageIndex, md.pageSize, md.totalRecords));
    }

    onFetchDataError(eventArgs) {
        // console.error('onFetchDataError: ', eventArgs);
    }

    registerCallbacks(verifyDSName = true) {
        // debugger;
        var id = this.options.gridId;
        var ds = this.options.dataSource.name;

        //
        // subscribe to main view/form events
        //
        this.registerCallback(id, appDataEvents.GRID_DATA, () => this.body.getDirtyRecords(), ds);
        this.registerCallback(id, appDataEvents.ON_ADD_RECORD, (a) => this.onHeaderNext(a, false), ds);
        this.registerCallback(id, appDataEvents.ON_FETCH_RECORD, (a) => this.onHeaderNext(a, true), ds);
        this.registerCallback(id, appDataEvents.ON_SAVE_RECORD, (a) => this.onSaveRecord(a), ds);
        this.registerCallback(id, appDataEvents.ON_SAVE_ERROR, (a) => this.onSaveError(a), ds);

        //
        // subscribe to grid events
        //
        this.registerCallback(id, appDataEvents.ON_SORTING_REQUESTED, (a) => this.onSortingRequest(a), ds, verifyDSName);
        this.registerCallback(id, appDataEvents.ON_COLS_REORDERED, (a) => this.onColsReordered(a), ds, verifyDSName);
        this.registerCallback(id, appDataEvents.ON_GRID_CONFIG_UPDATED, (ev) => this.onGridConfigurationChanged(ev), ds, verifyDSName);
        this.registerCallback(id, appDataEvents.ON_GRID_DATA_BOUND, (ev) => this.onGridDataBound(ev), ds, verifyDSName);
        this.registerCallback(id, appDataEvents.ON_FETCH_GRID_RECORD, (ev) => this.onFetchData(ev), ds, verifyDSName);
        this.registerCallback(id, appDataEvents.ON_FETCH_GRID_RECORD_ERROR, (ev) => this.onFetchDataError(ev), ds, verifyDSName);
    };
}

class BSGridMarker extends BSGridBase {
    constructor() {
        super();
        this.render();
    }

    render() {
        this.element = this.jquery(`<i class="bi bi-caret-right row-marker"></i>`);
    }

    clone() {
        return super.clone();    
    }
}

class BSGridInput extends BSGridBase {
    /**
     * @param {{ inputType: string; dataSourceName: string }} options
     */
    constructor(options) {
        super();
        this.options = options;
    }

    get val() {
        if (this.options.inputType === 'date' && this.element.val())
            return new Date(this.element.val());

        return this.element.val();
    }


    /**
     * @param {object} v
     */
    set val(v) {
        this.element.val(v);
    }

    /**
     * This method should be used with dropdowns where just setting the val of element is not enough
     * this method ensure that 'change' is called after 'val' so that value of the selector is set properly
     * @param {string} v - value
     */
    set valExt(v) {
        this.element.val(v);
        this.element.change();
    }

    get readonly() {
        return this.element.is("readonly");
    }

    set readonly(v) {
        this.element.attr('readonly', v);
    }

    get disabled() {
        return this.element.is("disabled");
    }

    set disabled(v) {
        this.element.attr('disabled', v);
    }

    clone() {
        // var sc = super.clone();
        // var c = new BSGridInput(this.shClone(this.options));
        // c.element = sc.element;
        // c.children = sc.children;

        // return c;
        return super.clone();
    }

    getFieldName() {
        return this.getProp('data-propname');
    }

    addDoubleClickEvent() {
        this.element.on('dblclick', (e) => {
            this.notifyListeners(this.appDataEvents.ON_ROW_DOUBLE_CLICKED,
                { dataSourceName: this.options.dataSourceName, eventData: e, source: this });
        })
    }

    isKeyField() {
        return this.getProp('data-keycolumn') === 'true';
    }

    change() {
        this.element.change();
    }
}

class BSGridTextInput extends BSGridInput {
    constructor(options = { dataSourceName: "", inputType: 'text' }) {
        super(options);
        this.render();
    }

    render() {
        this.element = this.jquery(`<input type='${this.options.inputType}' /> `);
    }

    clone() {
        var sc = super.clone();
        var c = new BSGridTextInput(this.shClone(this.options));
        c.element = sc.element;
        c.children = sc.children;
        c.addDoubleClickEvent();
        return c;
    }
}

class BSGridTextInputExt extends BSGridInput {
    /**
     * @param {{ inputType: string, elementId: string; dataSourceName:string }} options
     */
    constructor(options) {
        super(options);
        this.options = options;
        this.render();
    }



    render() {
        this.element = this.jquery(`#${this.options.elementId}`)
    }
}

class BSGridCheckBox extends BSGridInput {
    constructor(options) {
        super(options);
        this.render();
    }

    get val() {
        return this.element.is(':checked');
    }

    /**
     * @param {string} v
     */
    set val(v) {
        this.element.val(v);
    }


    render() {
        this.element = this.jquery("<input type='checkbox' />");
    }

    clone() {
        var sc = super.clone();
        var c = new BSGridCheckBox(this.shClone(this.options));
        c.element = sc.element;
        c.children = sc.children;
        this.addDoubleClickEvent();
        return c;
    }

}

class BSGridSelectOption extends BSGridBase {

    /**
     * 
     * @param {BSGridSelectListItem} options 
     */
    constructor(options) {
        super();
        this.options = options;
        this.render();
    }

    render() {
        this.element = this.jquery("<option></option>");
        this.element.val(this.options.value);
        this.element.text(this.options.text);

        if (this.options.isSelected)
            this.element.attr('selected', 'selected');
    }

    clone() {
        var clone = super.clone();
        clone.setText(this.element.text());
        return clone;
    }
}

class BSGridSelect extends BSGridInput {
    constructor(options) {
        super(options);
        this.render();
    }

    /**
     * @param {string} v
     */
    set val(v) {
        this.element.val(v);
        this.element.change();
    }

    get val() {
        return this.element.val();
    }

    render() {
        this.element = this.jquery("<select></select>");
    }

    clone() {
        var sc = super.clone();
        var c = new BSGridSelect(this.shClone(this.options));
        c.element = sc.element;
        c.children = sc.children;
        this.addDoubleClickEvent();

        return c;
    }


}

class BSGridSelector extends BSGridInput {

    /**
     * @type {any}
     */
    btnElement;

    /**
     * @type {BSGridTextInput}
     */
    txtElement;

    /**
     * @param {{dataSourceName:string, 
     * propName:string, 
     * inputType: string, 
     * cssClass: string, 
     * placeHolder: string, 
     * btnId: string, 
     * elementId: string,
     * btnClick: (sender:BSGridSelector, e:any)=> void }} options
     */
    constructor(options) {
        super(options);
        this.options = options;
        this.render();
    }

    /**
     * @param {BSGridSelectorWindow} sender
     * @param {any} e
     */
    onItemSelected(sender, e) {

        console.log('row selected', sender.grid.body.getSelectedRow());

        var row = sender.grid.body.getSelectedRow();
        var selectedInput = row.getInputs().find((input) => input.isKeyField());
        if (selectedInput) {
            // console.log('Selected value: ', selectedInput.val);
            // console.log('selector: ', this.txtElement.val);
            this.val = selectedInput.val;
            this.change(); // call change to fire the change event
        }
        sender.selectorModal.hide();
    }

    render() {

        this.txtElement = new BSGridTextInput(this.options);
        this.txtElement
            .addClass(this.options.cssClass)
            .props([{ key: "id", value: this.options.elementId },
            { key: "placeHolder", value: this.options.placeHolder },
            { key: "data-propname", value: this.options.propName }]);

        this.btnElement = this.jquery(`<button class="btn btn-outline-primary" type="button" id="${this.options.btnId}">
                                            <i class="bi bi-search"></i>
                                       </button>`);

        this.btnElement.on('click', (/** @type {MouseEvent} */ e) => {
            this.options.btnClick(this, e);
        });
        this.element = this.jquery('<div class="input-group input-group-sm"></div>');
        this.element = this.element.append(this.txtElement.element).append(this.btnElement);
    }

    getButton() {
        return this.btnElement;
    }

    // override base class methods

    get val() {
        return this.txtElement.val;
    }


    /**
     * @param {string} v
     */
    set val(v) {
        this.txtElement.val = v;
    }

    clone() {
        // debugger;
        var sc = super.clone();
        var c = new BSGridSelector(this.shClone(this.options));
        c.children = sc.children;
        // c.addDoubleClickEvent(); // TODO: why it has to be in the clone method?  

        return c;
    }

    // addDoubleClickEvent() {
    //     this.txtElement.element.on('dblclick', (e) => {
    //         this.notifyListeners(this.appDataEvents.ON_ROW_DOUBLE_CLICKED,
    //             { dataSourceName: this.options.dataSourceName, eventData: e, source: this });
    //     })
    // }

    getFieldName() {
        return this.txtElement.getFieldName();
    }

    change() {
        this.txtElement.change();
    }


}

class BSGridCell extends BSGridBase {

    /**
     * @param {BSGridColDefinition} [options]
     */
    constructor(options) {
        super();
        this.options = options || new BSGridColDefinition();
        this.render();
    }

    render() {
        var rowSpan = this.options ? this.options.rowSpan : undefined;
        var colSpan = this.options ? this.options.colSpan : undefined;

        this.element =
            this.options && this.options.isHeader
                ? this.jquery("<th class='sorting ds-col'></th>")
                : this.jquery("<td></td>");

        if (rowSpan)
            this.element.attr('rowSpan', rowSpan);

        if (colSpan)
            this.element.attr('colSpan', colSpan);
    }

    clone() {
        // debugger;
        var sc = super.clone();
        var c = new BSGridCell(this.shClone(this.options));
        c.children = sc.children;
        c.element = sc.element;
        return c;
    }
}

class BSGridActions extends BSGridBase {


    /**
     * @type {string}
     */
    dataSourceName;
    constructor() {
        super();
        this.render();
    }

    render() {
        this.element = this.jquery('<div class="row actions-container"></div>')
    }

    /**
     * @param {(arg0: object) => any} [callback]
     */
    addDeleteAction(callback) {
        var btn = this.jquery(`<button type="button" class="btn btn-sm btn-outline-danger grid-toolbar-action" 
                                    id="btnDeleteRow_${this.dataSourceName}"><i class="bi bi-trash"></i>
                                </button>`);
        btn.on('click', callback);
        this.element.append(btn);
        return this;
    }

    /**
     * @param {(arg0: object) => any} [callback]
     */
    addNewRecordAction(callback) {
        var btn = this.jquery(`<button type="button" class="btn btn-sm btn-outline-primary grid-toolbar-action" 
                                    id="btnAddRow_${this.dataSourceName}"><i class="bi bi-plus-circle"></i>
                                </button>'`);
        btn.on('click', callback);
        this.element.append(btn);
        return this;
    }

    addGridSettingsAction() {
        var btn = this.jquery(`<button type="button" class="btn btn-sm btn-outline-primary grid-toolbar-action" 
                                    data-bs-toggle="modal" data-bs-target="#staticBackdrop_${this.dataSourceName}" 
                                    id="btnSettings_${this.dataSourceName}"><i class="bi bi-gear"></i>
                                </button>`);
        this.element.append(btn);
        return this;
    }

}

class BSGridRowCollection extends BSGridBase {
    /**
     * @type BSGridRow[]
     */
    rows = [];

    constructor(options) {
        super();
        this.options = options;
    }

    /**
     * 
     * @param {BSGridRow} row 
     */
    addRow(row) {
        this.element.append(row.element);
        this.rows.push(row);
        return this;
    }

    getVisibleRows() {
        return this.rows.filter((row) => row.visible === true);
    }

    // getActionsRow() {
    //     return this.rows.find((row) => row.options.isActionsRow === true);
    // }

    getGridTitlesRow() {
        return this.rows.find((row) => row.options.hasGridTitles === true);
    }
}

class BSGridHeader extends BSGridRowCollection {

    constructor(options) {
        super(options);
        this.render();
    }

    render() {
        this.element = this.jquery('<thead class="table-light"></thead>');
    }

}

class BSGridBody extends BSGridRowCollection {
    constructor(options) {
        super(options);
        this.render();
    }

    render() {
        this.element = this.jquery("<tbody></tbody>");
    }

    /**
    * @param {BSGridRow} row
    */
    rowSiblings(row) {
        return this.rows.filter((v, i) => {
            if (v !== row) return v; // return all except the current row
        })
    }

    /**
     * @param {BSGridRow} row
     */
    focusRow(row) {
        row.removeClass('table-active').addClass('table-active');
        var siblings = this.rowSiblings(row);
        siblings.forEach((v, i) => v.removeClass('table-active'));
    };



    getTemplateRow() {
        var result = this.rows.filter(function (v) {
            if (v.options.isTemplateRow === true) return v;
        });

        if (result && result.length > 0) return result[0];
    }

    getDirtyRows() {
        var rows = this.rows.filter((v, i) => v.isRowDirty());
        return rows;
    }

    getDirtyRecords() {
        var dirtyRows = this.getDirtyRows();

        if (dirtyRows.length === 0) {
            return [];
        }
        var records = [];
        dirtyRows.forEach((row, i) => {
            records.push(row.getRowData());
        })

        return records;
    }

    getSelectedRow() {
        return this.rows.find((v, i) => v.hasClass('table-active'));
    }


    markDeleted() {
        var row = this.getSelectedRow();
        if (!row) return;

        var siblings = this.rowSiblings(row);
        var lastSibling = siblings[siblings.length - 1];
        row.removeClass('table-active');
        row.prop('data-isdirty', 'true');
        row.css = { 'display': 'none' };

        var rowCat = row.getProp('data-rowcategory');
        if (rowCat === 'ADDED') {
            row.prop('data-rowcategory', 'ADDED_DELETED');
        }
        else {
            row.prop('data-rowcategory', 'DELETED');
        }

        this.notifyListeners(this.appDataEvents.ON_GRID_UPDATED, { dataSourceName: row.options.dataSourceName, eventData: row });

        this.focusRow(lastSibling);
    }

    /**
     * Removes the row from rows collection
     * @param {BSGridRow} row 
     */
    removeRow(row) {
        var index = this.rows.indexOf(row);
        if (index > -1)
            this.rows.splice(index);
    }

}

class BSGridRow extends BSGridBase {

    /**
     * @type BSGridCell[]
     */
    cells = [];

    /**
     * @param {{ dataSourceName: string; hasGridTitles?: boolean; isTemplateRow?: boolean;}} options
     */
    constructor(options) {
        super();
        this.options = options;
        //this.isTemplateRow = options.isTemplateRow;
        //this.dataSourceName = options.dataSourceName;

        this.render();
    }

    /**
    * 
    * @param {BSGridCell} cell 
    */
    addCell(cell) {
        this.element.append(cell.element);
        this.cells.push(cell);
    }

    /**
     * @param {BSGridCell[]} cells
     */
    addCells(cells) {
        cells.forEach((cell) => this.addCell(cell));
    }

    render() {
        if (!this.element)
            this.element = this.jquery("<tr></tr>")
    }

    /**
     * 
     * @returns {BSGridRow}
     */
    clone() {
        //var clone = this.element.clone();
        //return new BSGridRow({ element: clone, dataSourceName: this.dataSourceName });
        //let clone = Object.assign(Object.create(Object.getPrototypeOf(this)), this);
        //return clone;
        var parentClone = super.clone();
        //debugger;
        let optClone = this.shClone(this.options);
        optClone.isTemplateRow = false;
        var cloneRow = new BSGridRow(optClone);
        cloneRow.element = parentClone.element;
        cloneRow.children = parentClone.children;
        cloneRow.cells = this.cells.map((v) => {
            var cloneCell = v.clone();
            cloneRow.element.append(cloneCell.element);
            return cloneCell;
        });

        return cloneRow;
    }

    focusRow() {
        this.removeClass('table-active').addClass('table-active');
    }

    getInputs() {
        /**
         * @type BSGridInput[]
         */
        var inputs = [];

        // debugger;
        this.cells.forEach((val, idx) => {
            var children = val.children;
            if (children.length > 0) {
                children.forEach((v, i) => {
                    if (v instanceof BSGridSelector)
                        inputs.push(v.txtElement);
                    else if (v instanceof BSGridInput)
                        inputs.push(v);
                });
            }
        });
        return inputs;
    }

    getVisibleInputs() {
        var inputs = this.getInputs();
        return inputs.filter((input) => input.visible === true);
    }

    getRowDataExt() {
        var rowInputs = this.getInputs();
        var record = {};
        rowInputs.forEach((rowInput, i) => {
            var cellPropName = rowInput.getFieldName();
            record[cellPropName] = rowInput;
        });
        return record;
    }

    getRowData() {
        var rowInputs = this.getInputs();
        var rowIndex = this.getProp('data-index');
        if (rowIndex) {
            rowIndex = parseInt(rowIndex);
        }
        var record = {};
        var rowCat = this.getProp('data-rowcategory');
        record['rowCategory'] = rowCat;

        rowInputs.forEach((rowInput, i) => {
            var cellPropName = rowInput.getFieldName();
            record[cellPropName] = rowInput.val;
        });
        record["clientRowNumber"] = rowIndex;
        // console.log('GetRowData: ', record);
        return record;
    }

    isRowDirty() {
        return this.getProp('data-isdirty') === 'true';
    }

}

class BSGridOptions {

    /**
     * 
     * @param {string} gridId 
     * @param {string} containerId
     * @param {BSGridColDefinition[]} colDefinition 
     * @param {BSGridDataSource} dataSource 
     * @param {boolean} isReadonly
     */
    constructor(gridId, containerId, colDefinition, dataSource, isReadonly = false) {
        this.gridId = gridId;
        this.containerId = containerId;
        this.colDefinition = colDefinition;
        this.dataSource = dataSource;
        this.isReadonly = isReadonly;
        this.enableInfiniteScroll = true;
    }
}

class BSGridColDefinition {

    /**
     * @param {string} [name]
     * @param {string} [dataType]
     * @param {string} [width]
     * @param {string} [propName]
     * @param {boolean} [isKey]
     * @param {BSGridSelectListItem[]} [dataSource]
     * @param {boolean} [isHeader]
     * @param {number} [colSpan]
     * @param {number} [rowSpan]
     * @param {getUrlCallback} [selectorDataCB] - a cb to return the page url
     * @param {BSGridColDefinition[]} [selectorCols] - cols def for selector
     */
    constructor(name, dataType, width, propName, isKey,
        dataSource, isHeader, colSpan, rowSpan, selectorDataCB, selectorCols) {
        this.name = name;
        this.dataType = dataType;
        this.width = width;
        this.propName = propName;
        this.isKey = isKey;
        this.dataSource = dataSource;
        this.isHeader = isHeader;
        this.colSpan = colSpan;
        this.rowSpan = rowSpan;
        this.selectorDataCB = selectorDataCB;
        this.selectorCols = selectorCols;

    }
}



/**
 * Url CB type
 * @callback getUrlCallback
 * @param {number} pageIndex
 * @returns {string} url to access next page
 */

/**
 * A callback type to get the next page in the offline mode
 * @callback getNextPageOffline
 * @param {number} pageIndex
 * @param {object[]} data - dataset
 * @param {PagingMetaData} metadata - dataseta metadata
 * @returns {object[]} returns the data model for the request page
 */


class BSGridDataSource {


    /**
     * @param {string} name
     * @param {{initData: object[];metaData: PagingMetaData;}} initData
     * @param {boolean} isRemote
     * @param {getUrlCallback} url - A cb that will accept a page number and returns the url to the next page
     * @param {getNextPageOffline} getPageOffline - A callback type to get the next page in the offline mode
     */
    constructor(name, initData, isRemote, url = (page) => undefined, getPageOffline = undefined) {
        this.name = name;
        this.data = initData;
        this.isRemote = isRemote;
        this.url = url;
        this.getPageOfflineCB = getPageOffline;
    }

}

class BSGridSelectListItem {

    /**
     * @param {string} text
     * @param {string} value
     * @param {boolean} isSelected
     */
    constructor(text, value, isSelected = false) {
        this.text = text;
        this.value = value;
        this.isSelected = isSelected;
    }

}

class BSGridEventArgs {
    /**
     * @param {BootstrapDataGrid} source
     * @param {object} eventData
     * @param {string} dsName
     * @param {string} idField
     */
    constructor(source, eventData, dsName, asc = true, idField = undefined) {
        this.source = source;
        this.eventData = eventData;
        this.dsName = dsName;
        this.asc = asc;
        this.idField = idField;
    }
}

BootstrapDataGrid.prototype.configurableGrid = function () {
    // console.log('configurableGrid is reached', this);
    var headers = this.head.getGridTitlesRow().cells;
    var dataSourceName = this.options.dataSource.name;

    //
    // A modal for configuring grid columns.
    // The modal ahs an <ul> element which will be populated below with grid columns check-list.
    // the checks can be used to show/hide a particular grid column
    //
    var modelTemplate =
        `<div class="settings-menu grid-config-template">
            <div class="modal fade" id="staticBackdrop_${this.options.dataSource.name}" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1"
            aria-labelledby="staticBackdropLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="staticBackdropLabel_${this.options.dataSource.name}">Configure columns</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <ul class="list-group grid-config-cols">

                            </ul>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" data-bs-dismiss="modal">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
    var modalElem = this.jquery(modelTemplate);
    this.jquery('#' + this.options.containerId).append(modalElem);
    // this.append(modalElem, false);

    var colsList = modalElem.find('.grid-config-cols');
    headers.forEach((header, index) => {

        var propName = header.getProp('data-th-propname');
        if (!propName) return;

        var colsListItem = this.jquery('<li class="list-group-item"></li>');

        var chk = this.jquery('<input type="checkbox" value="" class="form-check-input me-1" />');
        var chkId = 'col_config_chk_' + propName;
        chk.attr('id', chkId);
        chk.attr('data-config-propname', propName);
        if (header.visible === true) {
            chk.attr('checked', 'checked');
        }

        var chkLbl = this.jquery('<label for="' + chkId + '"></label>');
        // debugger;
        chkLbl.text(header.getText());

        colsListItem.append(chk);
        colsListItem.append(chkLbl);
        colsList.append(colsListItem);

        chk.on('click', (e) => {
            var $chk = this.jquery(e.target);
            var prop = $chk.attr('data-config-propname');
            if (!prop) return;

            var headerRow = this.head.getGridTitlesRow();
            // var col = this.find('th[data-th-propname=' + prop + ']');
            var col = headerRow.cells.find((cell) => cell.getProp('data-th-propname') === prop);
            if (!col) return;

            var bodyRows = this.body.rows;

            var rows = [...bodyRows, headerRow];

            //var rows = this.find('.grid-cols, .grid-rows');


            //var index = Array.from(col.parent('tr').children()).indexOf(col[0]);
            var index = headerRow.cells.indexOf(col);
            if (index < 0) return;

            rows.forEach((row) => {

                var cell = row.cells[index];

                if (!cell) return;

                if ($chk.is(':checked') === true) {
                    // $(cell).show();
                    cell.visible = true;
                }
                else {
                    // $(cell).hide();
                    cell.visible = false;
                }
            });

            this.notifyListeners(appDataEvents.ON_COLS_REORDERED,
                { dataSourceName: dataSourceName, eventData: e, source: this });

            this.notifyListeners(appDataEvents.ON_GRID_CONFIG_UPDATED,
                { dataSourceName: dataSourceName, eventData: e, source: this, action: appActions.COL_SHOW_HIDE });

        });
    });
}

BootstrapDataGrid.prototype.resizableGrid = function () {
    // console.log('resizableGrid is reached', this);

    var dataSourceName = this.options.dataSource.name;
    // console.log(table);
    var cols = this.head.getGridTitlesRow().cells;
    this.css = {};

    this.setCss('overflow', 'hidden');

    var tableHeight = this.element[0].offsetHeight;

    for (var i = 0; i < cols.length; i++) {
        var div = createDiv(tableHeight);
        cols[i].element.append(div);
        cols[i].setCss('position', 'relative');
        setListeners(div, cols[i], this);
    }

    /**
     * @param {HTMLDivElement} div
     * @param {BootstrapDataGrid} table
     * @param {BSGridCell} col
     */
    function setListeners(div, col, table) {
        var pageX, /** @type {HTMLTableCellElement} */curCol, curColWidth, nxtColWidth, tableWidth;

        div.addEventListener('mousedown', function (e) {

            tableWidth = table.element[0].offsetWidth;

            curCol = col.element[0];
            pageX = e.pageX;

            var padding = paddingDiff(curCol);

            curColWidth = curCol.offsetWidth - padding;
        });

        div.addEventListener('mouseover', function (e) {
            this.style.borderRight = '2px solid #0000ff';
        })

        div.addEventListener('mouseout', function (e) {
            this.style.borderRight = '';
        })

        document.addEventListener('mousemove', function (e) {
            if (curCol) {
                var diffX = e.pageX - pageX;

                curCol.style.width = (curColWidth + diffX) + 'px';
                table.element[0].style.width = tableWidth + diffX + "px";

            }
        });

        document.addEventListener('mouseup', function (e) {

            if (curCol) {
                table.notifyListeners(appDataEvents.ON_GRID_CONFIG_UPDATED,
                    {
                        dataSourceName: dataSourceName,
                        eventData: { e, curCol },
                        source: table,
                        action: appActions.COL_RESIZED
                    });

            }

            curCol = undefined;
            pageX = undefined;
            nxtColWidth = undefined;
            curColWidth = undefined
        });
    }

    /**
     * @param {string} height
     */
    function createDiv(height) {
        var div = document.createElement('div');
        div.style.top = "0";
        div.style.right = "0";
        div.style.width = '5px';
        div.style.position = 'absolute';
        div.style.cursor = 'col-resize';
        div.style.userSelect = 'none';
        div.style.height = height + 'px';
        return div;
    }

    function paddingDiff(col) {

        if (getStyleVal(col, 'box-sizing') == 'border-box') {
            return 0;
        }

        var padLeft = getStyleVal(col, 'padding-left');
        var padRight = getStyleVal(col, 'padding-right');
        return (parseInt(padLeft) + parseInt(padRight));

    }

    function getStyleVal(elm, css) {
        return (window.getComputedStyle(elm, null).getPropertyValue(css));
    }

}

BootstrapDataGrid.prototype.enableColumnReordering = function () {

    // console.log('enableColumnReordering is reached', this);

    var dataSourceName = this.options.dataSource.name;
    var jq = this.jquery;
    var _this = this;
    //var gridId = $table.attr('id');
    //console.log('datasource-name', dataSourceName);
    var addWaitMarker = function () {
        // var dw = jq('<div></div>');
        // dw.addClass('wait-reorder').hide();
        // var ct = jq('<div class="d-flex justify-content-center"></div>');
        // var ds = jq('<div></div>').addClass('spinner-border');
        // ds.append('<span class="visually-hidden">Wait...</span>');
        // ct.append(ds);
        // dw.append(ct);
        // _this.addClass('caption-top');
        // var caption = jq('<caption></caption>').append(dw);
        // _this.element.append(caption);
    };

    var thWrap = jq('<div draggable="true" class="grid-header"></div>');

    var headerRow = _this.head.getGridTitlesRow();
    var cells = headerRow.cells;

    cells.forEach((cell) => {
        var childs = cell.element.children();
        if (childs.length === 0) {
            var txt = cell.element.text();
            cell.element.text('');
            childs = jq('<div></div>').text(txt);
            cell.element.append(childs);
        }
        jq(childs).wrap(thWrap);
    });

    // addWaitMarker();

    var srcElement;

    //jQuery.event.props.push('dataTransfer');
    _this.find('.grid-header').on({
        dragstart: function (e) {
            if (!jq(this).hasClass('grid-header')) {
                srcElement = undefined;
                return;
            };

            srcElement = e.target;
            jq(this).css('opacity', '0.5');
        },
        dragleave: function (e) {
            e.preventDefault();
            if (!srcElement) return;

            if (!jq(this).hasClass('grid-header')) return;
            jq(this).removeClass('over');
        },
        dragenter: function (e) {
            e.preventDefault();
            if (!srcElement) return;

            if (!jq(this).hasClass('grid-header')) return;
            jq(this).addClass('over');
            // e.preventDefault();
        },
        dragover: function (e) {
            e.preventDefault();
            if (!srcElement) return;

            if (!jq(this).hasClass('grid-header')) return;
            jq(this).addClass('over');


        },
        dragend: function (e) {
            e.preventDefault();
            if (!srcElement) return;
            jq(this).css('opacity', '1');
        },
        drop: function (e) {
            e.preventDefault();
            if (!srcElement) return;
            var $this = jq(this);
            $this.removeClass('over');
            var destElement = e.target;
            if (!$this.hasClass('grid-header')) return;
            if (srcElement === destElement) return;

            //var cols = _this.head.rows[0].cells;

            // dest
            var destParent = $this.parents('th');
            if (!destParent || destParent.length <= 0) return;

            // lookup in cells
            var desParentCell = cells.find((el) => el.element[0] === destParent[0]);
            if (!desParentCell) return;

            var toIndex = cells.indexOf(desParentCell);

            // src
            var srcParent = jq(srcElement).parents('th');
            if (!srcParent || srcParent.length <= 0) return;

            // lookup in cells
            var srcParentCell = cells.find((el) => el.element[0] === srcParent[0]);
            if (!desParentCell) return;

            var fromIndex = cells.indexOf(srcParentCell);

            //console.log(toIndex, fromIndex);

            if (toIndex == fromIndex) return;

            //
            // apply new order to the headers
            //
            reOrder(headerRow, cells, fromIndex, toIndex);

            var rows = _this.body.rows;
            // jq('.wait-reorder').css({ 'cursor': 'progress' }).show();

            //
            // apply new order to all the rows in the grid
            //
            setTimeout(() => {
                //console.log('Reordering started, ', new Date());
                for (let index = 0; index < rows.length; index++) {
                    // debugger;
                    var row = rows[index];
                    var cells = row.cells
                    if (toIndex == fromIndex) return;
                    reOrder(row, cells, fromIndex, toIndex);
                }

                //console.log('Reordering completed, ', new Date());
                //
                // notify about column re-ordering
                //
                _this.notifyListeners(appDataEvents.ON_COLS_REORDERED,
                    { dataSourceName: dataSourceName, eventData: e, source: _this });

                _this.notifyListeners(appDataEvents.ON_GRID_CONFIG_UPDATED,
                    { dataSourceName: dataSourceName, eventData: e, source: _this, action: appActions.COL_REORDER });

                // jq('.wait-reorder').css({ 'cursor': '' }).hide();
            }, 500);

        }
    });

    var reOrder = function (/** @type {BSGridRow} */ row, /** @type {BSGridCell[]} */ cells, /** @type {number} */ fromIndex, /** @type {number} */ toIndex) {

        // debugger;
        if (fromIndex == toIndex) return;

        var dir = directions.ltr;

        if (fromIndex > toIndex) {
            dir = directions.rtl;
        }

        if (dir === directions.rtl) {
            swapRtl(cells, fromIndex, toIndex)
        }
        else {
            swapLtr(cells, fromIndex, toIndex);
        }

        // debugger;
        row.cells = [];
        row.addCells(cells);

        //jq(row).append(cells);
    };

    var swapRtl = function (/** @type {BSGridCell[]} */ cells, /** @type {number} */ fromIndex, /** @type {number} */ toIndex) {
        for (let i = fromIndex; i > toIndex; i--) {
            swap(cells, i, i - 1);
        }
    };

    var swapLtr = function (/** @type {BSGridCell[]} */ cells, /** @type {number} */ fromIndex, /** @type {number} */ toIndex) {
        for (let i = fromIndex; i < toIndex; i++) {
            swap(cells, i, i + 1);
        }
    };

    var swap = function (/** @type {BSGridCell[]} */ arr, /** @type {number} */ ia, /** @type {number} */ ib) {
        var temp = arr[ia];
        arr[ia] = arr[ib];
        arr[ib] = temp;
    };

    var directions = { rtl: 'RIGHT-TO-LEFT', ltr: 'LEFT-TO-RIGHT' };
}

BootstrapDataGrid.prototype.onGridConfigurationChanged = function (eventArgs) {
    // console.log('grid configuration updated', eventArgs);

    // debugger;

    var action = eventArgs.action;
    var gridId = this.options.gridId;

    var cols = this.head.getGridTitlesRow().cells;
    // console.log(cols);
    var colsObj = {};
    cols.forEach((col, index) => {

        var sort = 'asc';
        if (col.hasClass('sorting_desc'))
            sort = 'desc';

        var prop = col.getProp('data-th-propname');

        var colAttr = new BSGridColSettings(col.getCss('width'), col.visible, sort, index);

        colsObj[prop] = colAttr;
    });



    Cookie.delete(gridId);
    setTimeout(() => {
        // console.log('Colsobject: ', colsObj);
        Cookie.setJSON(gridId, colsObj, { days: 30, secure: true, SameSite: 'strict' });
    }, 500);
}

BootstrapDataGrid.prototype.onGridDataBound = function (eventArgs) {
    // console.log(eventArgs);

    // var grid = eventArgs.source;
    //
    // enables the configuration of columns
    //
    this.configurableGrid();

    //
    // enables to re-order the columns
    //
    this.enableColumnReordering();

    //
    // make the grid resixeable
    //
    this.resizableGrid();
};

class BSGridConfigOptions {


    /**
     * @type {BSGridColSettings[]}
     */
    colSettings;
    /**
     * @param {BSGridColSettings[]} colSettings
     */
    constructor(colSettings) {
        this.colSettings = colSettings;
    }

    // TODO: is it needed?
    toJson() {

    }
}

class BSGridColSettings {
    /**
     * @param {string} width
     * @param {boolean} visible
     * @param {string} sort asc|desc
     * @param {number} position
     */
    constructor(width, visible, sort, position) {
        this.width = width;
        this.visible = visible;
        this.sort = sort;
        this.position = position;
    }
}

class BSGridHttpClient extends BSGridBase {

    constructor(sessionStorage, dataSourceName) {
        super();
        //@ts-ignore
        this.jq = jQuery;
        this.appDataEvents = appDataEvents;
        this.sessionStorage = sessionStorage;
        this.dataSourceName = dataSourceName;
    }


    /**
     * @param {BSGridHttpClientOptions} options
     */
    get(options) {
        // debugger;
        var key = JSON.stringify(options);
        var value = this.sessionStorage.getItem(key);
        if (value) {
            this.notifyResponse(value);
            return;
        }

        var _this = this;
        var ajaxOptions = {
            url: options.url,
            method: 'GET',
            headers: options.headers ? options.headers : {}
        };
        this.jq.ajax(ajaxOptions).then(function done(response) {
            // console.log(response);
            _this.sessionStorage.addItem(key, response, new Date(Date.now() + (10 * 60 * 1000)));// expires in 10 minutes
            _this.notifyResponse(response)

        }, function error(error) {
            _this.nofifyError(error);
        });

    };

    notifyResponse(response) {
        this.notifyListeners(this.appDataEvents.ON_FETCH_GRID_RECORD, { dataSourceName: this.dataSourceName, eventData: response });
    }

    nofifyError(error) {
        this.notifyListeners(this.appDataEvents.ON_FETCH_GRID_RECORD_ERROR,
            { dataSourceName: this.dataSourceName, eventData: error, recordId: options.recordId });
    }
}

class BSGridHttpClientOptions {

    /**
     * @param {string} url
     * @param {string} method
     * @param {object[]} headers
     * @param {string} recordId
     */
    constructor(url, method, headers = undefined, recordId = undefined) {
        this.url = url;
        this.method = method;
        this.headers = headers;
        this.recordId = recordId;
        this.cacheResponse = true;
    }
}

class BSGridPaginationOptions {

    /**
     * @param {string} dsName
     * @param {PagingMetaData} pagingMetaData
     */
    constructor(dsName, pagingMetaData, nextPageCallback = (/** @type {Number} */ page) => { }) {
        this.dsName = dsName;
        this.pagingMetaData = pagingMetaData;
        this.nextPageCallback = nextPageCallback;
    }
}

class BSGridPagination extends BSGridBase {

    /**
     * @param {BSGridPaginationOptions} options
     */
    constructor(options) {
        super();
        this.options = options;
        this.listId = `pg_list_${this.options.dsName}`;
        this.containerId = `pg_container_${this.options.dsName}`;
    }

    render() {
        if (this.element)
            this.element.remove();

        this.element =
            this.jquery(
                `<div class="bsgrid-pagination" id="${this.containerId}">
                        <nav aria-label="Page navigation">
                            
                        </nav>
                    </div>`);
        var pageList = this.jquery(`<ul class="pagination justify-content-end" id="${this.listId}"></ul>`);

        for (let index = 1; index <= this.options.pagingMetaData.totalPages && index <= 5; index++) {
            var li = this.jquery('<li class="page-item"></li>');
            var link = this.jquery(`<a class="page-link" href="#" data-p-index="${index}">${index}</a>`);
            li.append(link);
            pageList.append(li);

            link.on('click', (e) => {
                e.preventDefault();
                var index = this.jquery(e.target).attr('data-p-index');

                if (this.options.nextPageCallback)
                    this.options.nextPageCallback(parseInt(index));
            });
        };

        this.element.find('nav').append(pageList);
    }

    clear() {
        this.jquery('#' + this.listId).children('li').remove();
    }
}

class BSGridInfiniteScroll extends BSGridBase {

    /**
     * @type {PagingMetaData} metadata
     */
    initMetaData;

    /**
     * @type {number}
     */
    currentPage;

    /**
     * @type {object[]}
     */
    initData;

    /**
     * @param {{ gridElement: any; httpClient: BSGridHttpClient }} options
     */
    constructor(options) {
        super();
        this.gridElement = options.gridElement;
        this.httpClient = options.httpClient;

        this.s_area = null;
        this.observer = null;
        this.target = null;

        // this.totalPages = null;
        this.currentPage = 1;
        this.initData = null;
        this.initMetaData = null;
        this.nextPageCallback = null;

    }

    /**
     * @param {IntersectionObserverEntry[]} entries
     * @param {IntersectionObserver} sender
     */
    observerCB(entries, sender) {
        var entry = entries[0];
        // console.log(entry);
        if (entry.isIntersecting === true) {
            // console.log('Observer is invoked. Entry: ', entry);
            // console.log('initdata: ', this.initData);
            // console.log('metadata: ', this.initMetaData);

            //
            // fetch next page if we still have more pages to read
            //
            if (this.currentPage < this.initMetaData.totalPages) {
                console.log('Infinite scroll: fetching next page#: ', this.currentPage + 1);
                this.currentPage++;
                this.nextPageCallback(this.currentPage);
            }
        }
    }

    observe(el) {
        this.target = el;
        this.observer.observe(el);
    }

    unobserve() {
        this.observer.unobserve(this.target);
    }

    enable() {
        this.s_area = 'scroll_area_' + this.gridElement.attr('id');
        var scrollArea = this.jquery(`<div class="row" id="${this.s_area}" style="max-height: 200px; overflow-y: auto"></div>`);
        this.gridElement.wrap(scrollArea);

        var root = this.jquery.find(`#${this.s_area}`);
        let options = {
            root: root[0],
            rootMargin: '0px',
            threshold: 0.3,
            trackVisibility: false
        };

        this.observer = new IntersectionObserver((entries, sender) => this.observerCB(entries, sender), options);

        var rows = this.gridElement.find('tr');
        var lastRow = rows[rows.length - 1];
        var target = lastRow;
        // console.log(target, root);
        this.observe(target);
    }
}

class PagingMetaData {
    /**
     * @param {number} pageIndex
     * @param {number} pageSize
     * @param {number} totalRecords
     */
    constructor(pageIndex = 1, pageSize = 10, totalRecords = 10) {
        this.pageIndex = pageIndex;
        this.pageSize = !pageSize || pageSize <= 0 ? 10 : pageSize;
        this.totalRecords = totalRecords;
        this.totalPages = Math.ceil(this.totalRecords / this.pageSize);
    }
}

class BSGridSelectorWindowCollection extends BSGridBase {

    /**@type BSGridSelectorWindow[] */
    items;

    constructor() {
        super();
        this.items = [];
    }

    /**
     * @param {BSGridSelectorWindow} item
     */
    add(item) {
        if (!this.find(item.options.propName))
            this.items.push(item);
    }

    /**
     * @param {string} propName
     * @returns {BSGridSelectorWindow} Item that mataches the propName
     */
    find(propName) {
        return this.items.find((item) => item.options.propName === propName);
    }
}

class BSGridSelectorWindow extends BSGridBase {

    selectorModal;

    /**
     * @param {{ propName: string; containerId: string; urlCb: getUrlCallback; gridCols: BSGridColDefinition[]}} options
     */
    constructor(options) {
        super();
        this.options = options;

        this.parentContainerId = this.options.containerId;
        this.modalId = `${this.parentContainerId}_bs_${this.options.propName}`;
        this.modalTitleId = `${this.parentContainerId}_lbs_${this.options.propName}`;
        this.containerId = `${this.parentContainerId}_cbs_${this.options.propName}`;
        this.gridId = `${this.parentContainerId}_g_${this.options.propName}`;
        this.gridCols = options.gridCols;
        this.render();
        this.grid = this.renderGrid();
        this.onItemSelected = (/** @type {BootstrapDataGrid} */ sender, /** @type {any} */ e) => { console.log(); };
    }


    render() {

        var find = this.jquery('#' + this.parentContainerId).find('#' + this.modalId);
        if (find && find.length === 1) {
            this.element = find;
            // @ts-ignore
            this.selectorModal = bootstrap.Modal.getOrCreateInstance(find);
        }
        else {
            var modelTemplate =
                `<div class="modal" id="${this.modalId}">
                        <div class="modal-dialog modal-dialog-scrollable">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title" id="${this.modalTitleId}">Select a value</h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body">
                                    <div id="${this.containerId}">

                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-primary" data-bs-dismiss="modal">Ok</button>
                                </div>
                            </div>
                        </div>
                </div>`;

            this.element = this.jquery(modelTemplate);
            this.jquery('#' + this.parentContainerId).append(this.element);

            // @ts-ignore
            this.selectorModal = new bootstrap.Modal(this.element);

            this.element[0].addEventListener('shown.bs.modal', (e) => {
                this.grid.clearGrid();
                this.grid.infiniteScroller.currentPage = 1;
                this.grid.fetchGridPage(1)
            });
        }
    }

    show() {
        this.selectorModal.show();
    }

    renderGrid() {
        //
        // grid shown in the selector window
        //
        var dataSource = new BSGridDataSource('bsSelector',
            {
                initData: [],
                metaData: undefined
            },
            true,
            this.options.urlCb
        );

        var bs = new BSGridOptions(this.gridId, this.containerId, this.gridCols, dataSource, true);
        // bs.enableInfiniteScroll = false;

        var grid = new BootstrapDataGrid(bs);
        grid.registerCallbacks();

        // grid.addHandler(grid.appDataEvents.ON_ROW_DOUBLE_CLICKED, this.onItemSelected);

        //
        // following events are linked to parent (primary view/form) and are not needed for selector
        //

        grid.removeHandler(this.appDataEvents.GRID_DATA);
        grid.removeHandler(this.appDataEvents.ON_ADD_RECORD);
        grid.removeHandler(this.appDataEvents.ON_FETCH_RECORD);
        grid.removeHandler(this.appDataEvents.ON_SAVE_RECORD);
        grid.removeHandler(this.appDataEvents.ON_SAVE_ERROR);

        grid.render();

        // hide actions
        grid.gridActions.visible = false;
        return grid;
    }
}