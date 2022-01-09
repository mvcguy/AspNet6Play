// @ts-check

import $ from "../../lib/jquery/dist/jquery";


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

        this.jquery = $;
        this.dataEventsService = dataEventsService;
        this.appDataEvents = appDataEvents;
        this.Cookie = Cookie;
    }

    notifyListeners(eventType, payload) {
        this.dataEventsService.notifyListeners(eventType, payload);
    }

    getGridSettings(gridId) {
        try {
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

    get css() {
        return this.element.css();
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

    text(txt) {
        this.element.text(txt);
        return this;
    }

    /**
     * 
     * @param {BSGridBase} elem 
     */
    append(elem, pushToArray = true) {

        if (pushToArray) {
            this.children.push(elem);
        }

        return this.element.append(elem.element);
    }

    focus() {
        this.element.focus();
    }

    isEmptyObj(obj) {
        return Object.keys(obj).length === 0;
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
    }

    clearGrid() {
        this.find('.grid-row').remove();

        // remove all except the template row
        var templateRow = this.body.getTemplateRow();
        this.body.rows = [templateRow];
    };

    applyColSettings(col, settings) {

        if (this.isEmptyObj(settings)) return;

        if (settings.visible === false) {
            col.hide();
        }

        if (settings.width) {
            col.css({ 'position': 'relative', 'width': settings.width });
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

    render() {

        this.element = this.jquery('<table></table>');
        var _this = this;

        _this.prop('id', _this.options.gridId)

        var settings = _this.getGridSettings() || {};
        _this.css({ 'width': 'inherit' });

        var gridHeaderRow = new BSGridRow({ dataSourceName: this.options.dataSource.name });
        gridHeaderRow.addClass('draggable').addClass('grid-cells');

        var gridBodyRow = new BSGridRow({ isTemplateRow: true, dataSourceName: this.options.dataSource.name });
        gridBodyRow.addClass('grid-rows');

        gridBodyRow.props([{ key: 'id', value: _this.options.gridId + "_template_row_" }])
        gridBodyRow.css = { 'display': 'none' };

        var gridColumns = this.applyColSorting(settings);

        gridColumns.forEach(function (gc) {
            var gridCol = gc;
            var colSettings = settings[gridCol.propName] || {};

            var gridHeaderCell = new BSGridCell(gridCol);
            gridHeaderCell.addClass('sorting').addClass('ds-col');
            gridHeaderCell.text(gridCol.name);
            gridHeaderCell.props([{ key: 'data-th-propname', value: gridCol.propName }]);

            var gridBodyCell = new BSGridCell(null);


            //var cellInput = '<input class="form-control" type="' + gridCol.dataType + '" />';
            var cellInputVar = null;

            if (gridCol.dataType === 'select') {
                cellInputVar = new BSGridSelect();
                cellInputVar.addClass('form-select');
                cellInputVar.addSelectOptions(); // pass ds: gridCol.dataSource
            }
            else if (gridCol.dataType === 'checkbox') {
                cellInputVar = new BSGridCheckBox();
                cellInputVar.addClass('form-control');
            }
            else {
                cellInputVar = new BSGridTextInput();
                cellInputVar.addClass('form-select');
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
            gridHeaderCell.addSorting();


            _this.applyColSettings(gridHeaderCell, colSettings);
            _this.applyColSettings(gridBodyCell, colSettings);
            gridHeaderRow.append(gridHeaderCell);
            gridBodyRow.append(gridBodyCell);
        });

        _this.head.addRow(gridHeaderRow);
        _this.body.append(gridBodyRow);

        //
        // add data to the grid
        //
        _this.bindDataSource(_this.options.dataSource.initData);

        //
        // notify that grid is data-bound
        //
        _this.notifyListeners(_this.appDataEvents.ON_GRID_DATA_BOUND,
            { dataSourceName: _this.options.dataSource.name, eventData: {}, source: _this });

        //
        // add grid to the provided container
        //
        _this.jquery('#' + _this.options.containerId).append(_this.element);

    };


    addNewRow(rowNumber, rowData, existingRecord) {
        var row = this.body.getTemplateRow().clone();

        row.prop('id', this.options.gridId + "_template_row_" + rowNumber);
        row.prop('data-index', rowNumber);
        row.addClass('grid-row');
        row.css('display', 'table-row');

        var _this = this;

        var inputs = row.getInputs();

        inputs.forEach(function (v, i) {

            var input = v;

            var oldId = input.getProp('id');
            input.prop('id', oldId + "_" + rowNumber);

            var cellPropName = input.getProp('data-propname');

            // debugger;
            var cellVal = rowData[cellPropName];

            if (input instanceof BSGridCheckBox
                && (cellVal === 'true' || cellVal === 'True' || cellVal === true)) {
                input.prop('checked', 'checked');
            }
            else {
                input.val = cellVal;
            }

            if (existingRecord === false) {
                input.prop('disabled', false);
            }

            input.element.on('chnage', function (e) {

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

                this.notifyListeners(_this.appDataEvents.ON_GRID_UPDATED,
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
        var dsName = eventArgs.dsName;

        var dirtyRows = this.body.getDirtyRows();

        for (let i = 0; i < dirtyRows.length; i++) {
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
                            Array.from(inputError).forEach(function (ie, er) {
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
            return v.element.getProp('data-index') === "'" + index + "'";
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

            var thx = this.head.rows[0].cells.find((v, i) => v.element === th[0]);
        }

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
        var grid = eventArgs.source;

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
        var id = this.options.gridId;
        var ds = this.options.dataSource.name;

        this.registerCallback(id, appDataEvents.GRID_DATA, this.body.getDirtyRecords, ds);
        this.registerCallback(id, appDataEvents.ON_ADD_RECORD, this.onHeaderNext, ds);
        this.registerCallback(id, appDataEvents.ON_FETCH_RECORD, this.onHeaderNext, ds);
        this.registerCallback(id, appDataEvents.ON_SAVE_RECORD, this.onSaveRecord, ds);
        this.registerCallback(id, appDataEvents.ON_SAVE_ERROR, this.onSaveError, ds);
        this.registerCallback(id, appDataEvents.ON_SORTING_REQUESTED, this.onSortingRequest, ds);
        this.registerCallback(id, appDataEvents.ON_COLS_REORDERED, this.onColsReordered, ds);
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
}

class BSGridTextInput extends BSGridInput {
    constructor(options) {
        super(options);
    }
}

class BSGridCheckBox extends BSGridInput {
    constructor(options) {
        super(options);
    }

    get val() {
        return this.element.is(':checked');
    }

    render() {
        this.element = this.jquery("<input type='checkbox' />");
    }

}

class BSGridSelect extends BSGridInput {
    constructor(options) {
        super(options);
    }

    /**
     * @param {string} v
     */
    set val(v) {
        this.element.val(v);
        this.element.change();
    }

    render() {
        this.element = this.jquery("<select></select>");
    }

    addSelectOptions() {
        var sOptions = this.options.selectOptions;
        var _jq = this.jquery;
        sOptions.forEach(function () {
            var opt = this;
            var elem = _jq("<option></option>")
            elem.val(opt.value);
            elem.text(opt.text);

            if (opt.isSelected)
                elem.attr('selected', 'selected');
        });
    }
}

class BSGridCell extends BSGridBase {

    /**
     * @param {BSGridColDefinition} options
     */
    constructor(options) {
        super();
        this.options = options;
        this.render();
    }

    render() {
        this.element = this.jquery("<th class='sorting ds-col'></th>");
    }

    addSorting() {
        //
        // sorting of the data when the header cell is clicked
        //
        var th = this.element;
        var _this = this;
        th.addProp('data-th-propname', _this.options.propName);
        th.on('click', function (e) {

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
            th.siblings('th').removeClass('sorting_asc').removeClass('sorting_desc');

            //
            // notify that we need sorting of the column
            //
            var prop = th.attr('data-th-propname');
            _this.notifyListeners(_this.appDataEvents.ON_SORTING_REQUESTED,
                { dataSourceName: _this.options.dataSource.name, eventData: e, propName: prop, asc });

        });
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
        row.removeClass('row-active').addClass('row-active');
        var siblings = this.rowSiblings(row);
        siblings.forEach((v, i) => v.removeClass('row-active'));
    };



    getTemplateRow() {
        var result = this.rows.filter(function () {
            if (this.isTemplateRow === true) return this;
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
        row.removeClass('row-active');
        row.prop('data-isdirty', 'true');
        row.css({ 'display': 'none' });

        var rowCat = row.getProp('data-rowcategory');
        if (rowCat === 'ADDED') {
            row.prop('data-rowcategory', 'ADDED_DELETED');
        }
        else {
            row.prop('data-rowcategory', 'DELETED');
        }

        this.notifyListeners(this.appDataEvents.ON_GRID_UPDATED, { dataSourceName: row.dataSourceName, eventData: row });

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
        this.isTemplateRow = options.isTemplateRow;
        this.dataSourceName = options.dataSourceName;

        if (options.element) {
            this.element = options.element;
        }
        else {
            this.render();
        }

    }

    /**
    * 
    * @param {BSGridCell} cell 
    */
    addCell(cell) {
        this.cells.push(cell);
    }

    render() {
        this.element = this.jquery("<tr></tr>")
    }

    clone() {
        var clone = this.element.clone();
        return new BSGridRow({ element: clone, dataSourceName: this.dataSourceName });
    }

    focusRow() {
        this.removeClass('row-active').addClass('row-active');
    }

    getInputs() {
        /**
         * @type BSGridInput[]
         */
        var inputs = [];

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
     * @param {BSGridDataSource} dataSource
     */
    constructor(name, dataType, width, propName, isKey, dataSource) {
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
     * @param {URL} url
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
    constructor(text, value, isSelected) {
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
