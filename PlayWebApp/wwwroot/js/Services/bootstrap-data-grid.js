// @ts-check

//import $ from "../../lib/jquery/dist/jquery";


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

        // @ts-ignore
        this.bootstrap = bootstrap;
        this.appActions = appActions;
        // this.render(); // render manually
    }

    addHeader() {
        this.element.append(this.head.element);
    }

    addBody() {
        this.element.append(this.body.element);
    }


    render() {

        this.element = this
            .jquery('<table class="table table-bordered resizable navTable nowrap"></table>');

        var _this = this;

        _this.prop('id', _this.options.gridId);
        _this.prop('data-datasource', _this.options.dataSource.name);

        var settings = _this.getGridSettings(this.options.gridId) || {};
        _this.css = { 'width': 'inherit' };

        var gridHeaderRow = new BSGridRow({ dataSourceName: _this.options.dataSource.name });
        gridHeaderRow.addClass('draggable').addClass('grid-cols');

        var gridBodyRow = new BSGridRow({ isTemplateRow: true, dataSourceName: _this.options.dataSource.name });
        gridBodyRow.addClass('grid-rows');

        gridBodyRow.props([{ key: 'id', value: _this.options.gridId + "_template_row_" }])
        gridBodyRow.css = { 'display': 'none' };

        var gridColumns = _this.applyColSorting(settings);

        gridColumns.forEach(function (gc) {
            var gridCol = gc;
            var colSettings = settings[gridCol.propName] || {};

            var gridHeaderCell = new BSGridCell(gridCol, true);
            gridHeaderCell.addClass('sorting').addClass('ds-col');
            gridHeaderCell.setText(gridCol.name);
            gridHeaderCell.prop('data-th-propname', gridCol.propName);

            var gridBodyCell = new BSGridCell(null, false);


            //var cellInput = '<input class="form-control" type="' + gridCol.dataType + '" />';
            var cellInputVar = null;

            //debugger;
            if (gridCol.dataType === 'select') {
                cellInputVar = new BSGridSelect();
                gridCol.dataSource
                    .forEach((opt) => cellInputVar.append(new BSGridSelectOption(opt)));
                cellInputVar.addClass('form-select');
            }
            else if (gridCol.dataType === 'checkbox') {
                cellInputVar = new BSGridCheckBox();
                // cellInputVar.addClass('form-control');
            }
            else {
                cellInputVar = new BSGridTextInput();
                cellInputVar.addClass('form-control');
            }

            cellInputVar.props([
                { key: 'data-propname', value: gridCol.propName },
                { key: 'title', value: gridCol.name },
                { key: 'id', value: _this.options.gridId + "_template_row_" + gridCol.propName },
                { key: 'placeholder', value: gridCol.name }
            ]);

            if (gridCol.isKey === true) {
                cellInputVar.props([
                    { key: 'disabled', value: true },
                    { key: 'data-keycolumn', value: 'true' }
                ]);
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

        _this.head.addRow(gridHeaderRow);
        _this.body.addRow(gridBodyRow)

        //
        // add header and body to the grid
        //
        _this.addHeader();
        _this.addBody();


        //
        // add grid to the provided container
        //
        _this.jquery('#' + _this.options.containerId).append(_this.element);

        //
        // add data to the grid
        //
        _this.bindDataSource(_this.options.dataSource.initData);

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
     * 
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
     * 
     * @param {object[]} data 
     * @returns 
     */
    bindDataSource(data) {

        if (!data || data.length <= 0) return;

        data.forEach((v, i) => {
            var row = this.addNewRow(i, v, true);
            this.body.addRow(row); // TODO: Fix
            row.prop('data-rowcategory', 'PRESTINE')
        });

    }

    addNewRow(rowNumber, rowData, existingRecord) {
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

            var cellVal = rowData[cellPropName];

            if (input instanceof BSGridCheckBox
                && (cellVal === 'true' || cellVal === 'True' || cellVal === true)) {
                input.prop('checked', 'checked');
            }
            else if (cellVal !== undefined) {
                input.val = cellVal;
            }

            if (existingRecord === false) {
                input.prop('disabled', false);
            }

            input.element.on('change', (e) => {

                row.prop('data-isdirty', true);

                var rowCat = row.getProp('data-rowcategory');
                if (rowCat !== 'ADDED') {
                    row.prop('data-rowcategory', 'UPDATED');
                }

                // remove any previous errors
                row.removeClass('is-invalid').prop('title', '');

                var tooltip = _this.bootstrap.Tooltip.getInstance(this);
                if (tooltip)
                    tooltip.dispose();

                row.notifyListeners(_this.appDataEvents.ON_GRID_UPDATED,
                    { dataSourceName: _this.options.dataSource.name, eventData: e });

            });

            input.element.on('focus', function (e) {
                _this.body.focusRow(row);
            });

            var isLastInput = i == inputs.length - 1;
            if (isLastInput) {
                //
                // insert a new row if its the last input in the row
                //                    
                input.element.on('keydown', (e) => _this.onInputKeyDown(row, e));
            }

        });

        row.element.on('click', function (e) {
            _this.body.focusRow(row);
        });

        return row;

    };

    /**
     * 
     * @param {BSGridRow} row 
     * @param {*} e 
     * @returns 
     */
    onInputKeyDown(row, e) {
        if (e.which !== 9 || e.shiftKey === true) return;

        var lastRowIndex = this.body.rows[this.body.rows.length - 1].getProp('data-index');
        var parentIndex = row.getProp('data-index');

        // console.log(gridRows, currentRowIndex);
        if (lastRowIndex === parentIndex) {
            this.addNewRowToGrid();
        }
    };

    addNewRowToGrid() {
        //var rowCount = this.jquery('#' + this.options.gridId).find('tbody>tr').length;
        var emptyRow = this.addNewRow(this.body.rows.length - 1, this.createEmptyRowData(), false);
        this.body.addRow(emptyRow);

        var inputs = emptyRow.getInputs();
        inputs[0].focus();

        //
        // rowcategory = ADDED || DELETED || UPDATED
        //
        emptyRow.prop('data-rowcategory', 'ADDED');
        emptyRow.prop('data-isdirty', 'true');

        this.notifyListeners(this.appDataEvents.ON_GRID_UPDATED, { dataSourceName: this.options.dataSource.name, eventData: emptyRow });

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
    onHeaderNext(eventArgs) {

        if (!eventArgs || !eventArgs.eventData) return;

        this.resetSorting();
        this.clearGrid();
        this.bindDataSource(eventArgs.eventData[this.options.dataSource.name]);

    };

    onSaveRecord(eventArgs) {

        //
        // remove rows from the grid that has been deleted
        //

        this.body.find("tr[data-rowcategory='DELETED']").remove();
        this.body.find("tr[data-rowcategory='ADDED_DELETED']").remove();

        this.body.rows.filter((v) => {
            if (v.prop('data-rowcategory') === 'DELETED' || v.prop('data-rowcategory') === 'ADDED_DELETED')
                return v;
        }).forEach((v) => this.body.removeRow(v));

        //
        // when main record is saved, disable the key columns of the grid,
        //        
        this.body.rows.forEach((v) => {

            // mark all rows prestine
            v.prop('data-rowcategory', 'PRESTINE');

            // make id inputs disabled
            v.getInputs().filter((x) => {
                if (x.prop('data-keycolumn') === 'true')
                    return x;
            }).forEach((vx) => vx.prop('disabled', true));
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
        var list = this.body.rows.sort(comparer(this.head.rows[0].cells.indexOf(th), ascX = !ascX));

        list.forEach(tr => this.body.append(tr, false));

        dataEventsService.notifyListeners(this.appDataEvents.ON_GRID_CONFIG_UPDATED,
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
        var thx = this.head.rows[0].cells.find((v, i) => v.element[0] === eventArgs.eventData.target);
        // debugger;
        eventArgs.source.sortTable(thx, eventArgs.asc);
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
        //var grid = eventArgs.source;
        var grid = this;

        // TODO: Fix the insert of an empty row on the focus lost of final input
        grid.body.rows.forEach((row, i) => {

            var inputs = row.getInputs();
            inputs.forEach((inp) => { inp.element.off('keydown') });
            var lastInput = inputs[inputs.length - 1];
            lastInput.element.on('keydown', (e) => { this.onInputKeyDown(row, e) });
        });

    };

    registerCallback(key, eventTypeX, callback, dataSourceNameX) {
        dataEventsService.registerCallback(key, eventTypeX, callback, dataSourceNameX);
    };

    registerCallbacks() {
        // debugger;
        var id = this.options.gridId;
        var ds = this.options.dataSource.name;

        this.registerCallback(id, appDataEvents.GRID_DATA, () => this.body.getDirtyRecords(), ds);
        this.registerCallback(id, appDataEvents.ON_ADD_RECORD, (a) => this.onHeaderNext(a), ds);
        this.registerCallback(id, appDataEvents.ON_FETCH_RECORD, (a) => this.onHeaderNext(a), ds);
        this.registerCallback(id, appDataEvents.ON_SAVE_RECORD, (a) => this.onSaveRecord(a), ds);
        this.registerCallback(id, appDataEvents.ON_SAVE_ERROR, (a) => this.onSaveError(a), ds);
        this.registerCallback(id, appDataEvents.ON_SORTING_REQUESTED, (a) => this.onSortingRequest(a), ds);
        this.registerCallback(id, appDataEvents.ON_COLS_REORDERED, (a) => this.onColsReordered(a), ds);
        this.registerCallback(id, appDataEvents.ON_GRID_CONFIG_UPDATED, (ev) => this.onGridConfigurationChanged(ev), ds);
        this.registerCallback(id, appDataEvents.ON_GRID_DATA_BOUND, (ev) => this.onGridDataBound(ev), ds);
    };
}

class BSGridInput extends BSGridBase {
    constructor(options) {
        super();
        this.options = options;
    }

    get val() {
        return this.element.val();
    }


    /**
     * @param {string} v
     */
    set val(v) {
        this.element.val(v);
    }

    clone() {
        // var sc = super.clone();
        // var c = new BSGridInput(this.shClone(this.options));
        // c.element = sc.element;
        // c.children = sc.children;

        // return c;
        return super.clone();
    }
}

class BSGridTextInput extends BSGridInput {
    constructor(options) {
        super(options);
        this.render();
    }

    render() {
        this.element = this.jquery("<input type='text' /> ")
    }

    clone() {
        var sc = super.clone();
        var c = new BSGridTextInput(this.shClone(this.options));
        c.element = sc.element;
        c.children = sc.children;
        return c;
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

    render() {
        this.element = this.jquery("<input type='checkbox' />");
    }

    clone() {
        var sc = super.clone();
        var c = new BSGridCheckBox(this.shClone(this.options));
        c.element = sc.element;
        c.children = sc.children;
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
        return c;
    }


}

class BSGridCell extends BSGridBase {

    /**
     * @param {BSGridColDefinition} options
     * @param {boolean} isHeader
     */
    constructor(options, isHeader) {
        super();
        this.options = options;
        this.isHeader = isHeader;
        this.render();
    }

    render() {
        this.element =
            this.isHeader
                ? this.jquery("<th class='sorting ds-col'></th>")
                : this.jquery("<td></td>");
    }

    clone() {
        // debugger;
        var sc = super.clone();
        var c = new BSGridCell(this.shClone(this.options), this.isHeader);
        c.children = sc.children;
        c.element = sc.element;
        return c;
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
}

class BSGridHeader extends BSGridRowCollection {
    constructor(options) {
        super(options);
        this.render();
    }

    render() {
        this.element = this.jquery("<thead></thead>");
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
        var rows = this.rows.filter((v, i) => {
            if ((v.getProp('data-isdirty') === 'true')) return v;
        });

        return rows;
    }

    getDirtyRecords() {
        var dirtyRows = this.getDirtyRows();

        if (dirtyRows.length === 0) {
            return [];
        }
        var records = [];
        dirtyRows.forEach((row, i) => {

            var rowInputs = row.getInputs();
            var rowIndex = row.getProp('data-index');
            if (rowIndex) {
                rowIndex = parseInt(rowIndex);
            }
            var record = {};
            var rowCat = row.getProp('data-rowcategory');
            record['rowCategory'] = rowCat;

            rowInputs.forEach((rowInput, i) => {
                var cellPropName = rowInput.getProp('data-propname');
                record[cellPropName] = rowInput.val;
            });
            record["clientRowNumber"] = rowIndex;
            records.push(record);
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
                    if (v instanceof BSGridInput)
                        inputs.push(v);
                });
            }
        });
        return inputs;
    }
}

class BSGridOptions {

    /**
     * 
     * @param {string} gridId 
     * @param {string} containerId
     * @param {BSGridColDefinition[]} colDefinition 
     * @param {BSGridDataSource} dataSource 
     */
    constructor(gridId, containerId, colDefinition, dataSource) {
        this.gridId = gridId;
        this.containerId = containerId;
        this.colDefinition = colDefinition;
        this.dataSource = dataSource;
    }
}

class BSGridColDefinition {

    /**
     * @param {string} name
     * @param {string} dataType
     * @param {string} width
     * @param {string} propName
     * @param {boolean} isKey
     * @param {BSGridSelectListItem[]} dataSource
     */
    constructor(name, dataType, width, propName, isKey, dataSource = undefined) {
        this.name = name;
        this.dataType = dataType;
        this.width = width;
        this.propName = propName;
        this.isKey = isKey;
        this.dataSource = dataSource;

    }
}

class BSGridDataSource {

    /**
     * @param {string} name
     * @param {any[]} initData
     * @param {boolean} isRemote
     * @param {string} url
     */
    constructor(name, initData, isRemote, url) {
        this.name = name;
        this.initData = initData;
        this.isRemote = isRemote;
        this.url = url;
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
     */
    constructor(source, eventData, dsName, asc = true) {
        this.source = source;
        this.eventData = eventData;
        this.dsName = dsName;
        this.asc = asc;
    }
}

BootstrapDataGrid.prototype.configurableGrid = function () {
    console.log('configurableGrid is reached', this);
    var headers = this.head.rows[0].cells;
    var dataSourceName = this.options.dataSource.name;

    //
    // an <ul> element which will be populated below with grid columns check-list.
    // the checks can be used to show/hide a particular grid column
    //
    var colsList = this.jquery('.grid-config-cols');
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

            var headerRow = this.head.rows[0];
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

            dataEventsService.notifyListeners(appDataEvents.ON_GRID_CONFIG_UPDATED,
                { dataSourceName: dataSourceName, eventData: e, source: this, action: appActions.COL_SHOW_HIDE });


        });
    });
}

BootstrapDataGrid.prototype.resizableGrid = function () {
    console.log('resizableGrid is reached', this);

    var dataSourceName = this.options.dataSource.name;
    // console.log(table);
    var cols = this.head.rows[0].cells;
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

    console.log('enableColumnReordering is reached', this);

    var dataSourceName = this.options.dataSource.name;
    var jq = this.jquery;
    var _this = this;
    //var gridId = $table.attr('id');
    //console.log('datasource-name', dataSourceName);
    var addWaitMarker = function () {
        var dw = jq('<div></div>');
        dw.addClass('wait-reorder').hide();
        var ct = jq('<div class="d-flex justify-content-center"></div>');
        var ds = jq('<div></div>').addClass('spinner-border');
        ds.append('<span class="visually-hidden">Wait...</span>');
        ct.append(ds);
        dw.append(ct);
        _this.addClass('caption-top');
        var caption = jq('<caption></caption>').append(dw);
        _this.append(caption);
    };

    var thWrap = jq('<div draggable="true" class="grid-header"></div>');

    var headerRow = _this.head.rows[0];
    var cells = headerRow.cells;

    cells.forEach((cell) => {
        var childs = cell.element.children();
        if (childs.length === 0) {
            var txt =cell.element.text();
            cell.element.text('');
            childs = jq('<div></div>').text(txt);
            cell.element.append(childs);
        }
        jq(childs).wrap(thWrap);
    });

    addWaitMarker();

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
            jq('.wait-reorder').css({ 'cursor': 'progress' }).show();

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
                notifyListeners(appDataEvents.ON_COLS_REORDERED,
                    { dataSourceName: dataSourceName, eventData: e, source: _this });

                notifyListeners(appDataEvents.ON_GRID_CONFIG_UPDATED,
                    { dataSourceName: dataSourceName, eventData: e, source: _this, action: appActions.COL_REORDER });

                jq('.wait-reorder').css({ 'cursor': '' }).hide();
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


    var notifyListeners = function (eventType, payload) {
        dataEventsService.notifyListeners(eventType, payload);
    };

}

BootstrapDataGrid.prototype.onGridConfigurationChanged = function (eventArgs) {
    // console.log('grid configuration updated', eventArgs);

    // debugger;

    var action = eventArgs.action;
    var gridId = this.options.gridId;

    var cols = this.head.rows[0].cells;
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
    console.log(eventArgs);

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