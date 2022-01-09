// @ts-check

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
        this.jquery = $;
        // @ts-ignore
        this.dataEventsService = dataEventsService;
        // @ts-ignore
        this.appDataEvents = appDataEvents;
        // @ts-ignore
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
        this.jquery.each(props, function () {
            var prop = this;
            _this.prop(prop.key, prop.value);
        });
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

    text(txt) {
        this.element.text(txt);
        return this;
    }

    /**
     * 
     * @param {BSGridBase} elem 
     */
    append(elem) {
        this.children.push(elem);
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
     * @param {object} jquery 
     * @param {BSGridOptions} options 
     */
    constructor(jquery, options) {
        super();
        this.jquery = jquery;
        this.options = options;

        // @ts-ignore
        this.Cookie = Cookie;

        this.head = new BSGridHeader();
        this.body = new BSGridBody();

        // @ts-ignore
        this.bootstrap = bootstrap;
    }

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

        var gridHeaderRow = new BSGridRow({});
        gridHeaderRow.addClass('draggable').addClass('grid-cells');

        var gridBodyRow = new BSGridRow({ isTemplateRow: true });
        gridBodyRow.addClass('grid-rows');

        gridBodyRow.props([{ key: 'id', value: _this.options.gridId + "_template_row_" }])
        gridBodyRow.css = { 'display': 'none' };

        var gridColumns = this.applyColSorting(settings);

        _this.jquery.each(gridColumns, function () {
            var gridCol = this;
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

            if (gridCol.keyColumn === true) {
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
                _this.focusRow(row);
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
            _this.focusRow(row);
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
     * @param {BSGridRow} row
     */
    rowSiblings(row) {
        return this.body.rows.filter((v, i) => {
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
        _jq.each(sOptions, function () {
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


    getTemplateRow() {
        var result = this.rows.filter(function () {
            if (this.isTemplateRow === true) return this;
        });

        if (result && result.length > 0) return result[0];
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
        return new BSGridRow({ element: clone });
    }

    focusRow() {
        this.removeClass('table-active').addClass('table-active');
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
