var gridNavigationService = function (gridOptions) {

    var cols = gridOptions.cols;
    var dataSource = gridOptions.dataSource;
    var gridId = gridOptions.gridId;


    var addNewRow = function (rowNumber, rowData, existingRecord) {
        var templateRow = $('#' + gridId + "_template_row_").clone();
        templateRow.attr('id', gridId + "_template_row_" + rowNumber);
        templateRow.css('display', 'table-row');
        var childs = templateRow.find('input');

        $.each(childs, function (cellIndex, cellValue) {
            var oldId = $(cellValue).attr('id');
            $(cellValue).attr('id', oldId + "_" + rowNumber);
            var cellPropName = $(cellValue).attr('data-propname');
            var cellType = $(cellValue).attr('type');
            // debugger;
            var xx = rowData[cellPropName];
            if (cellType === 'checkbox') {
                if (xx === 'true' || xx === 'True' || xx === true) {
                    $(cellValue).attr('checked', 'checked');
                }
            }
            else {
                $(cellValue).val(xx);
            }

            if (existingRecord === false) {
                $(cellValue).attr('disabled', false);
            }

        });


        $(templateRow).attr('data-index', rowNumber);
        $(templateRow).addClass('grid-row');


        var rowInputs = $(templateRow).find('input');
        rowInputs.on('change', function (e) {
            var parent = $(e.target).parents('tr');
            $(parent).attr('data-isdirty', true);
            //
            // rowcategory = ADDED || DELETED || UPDATED
            //
            var rowCat = $(parent).attr('data-rowcategory');
            if (rowCat !== 'ADDED') {
                $(parent).attr('data-rowcategory', 'UPDATED');
            }

            notfiyListeners(appDataEvents.ON_GRID_UPDATED, { dataSourceName: 'mainForm', event: e });

        });

        rowInputs.on('focus', function (e) {
            var parent = $(e.target).parents('tr');
            focusRow(parent);
        });

        //
        // insert a new row if its the last input in the row
        //                    

        var lastCell = rowInputs.last();
        $(lastCell).on('keydown', function (e) {

            if (e.which !== 9 || e.shiftKey === true) return;

            var lastRowIndex = $('#' + gridId).find('tbody>tr:visible').last().attr('data-index');
            var parentIndex = $(e.target).parents('tr').first().attr('data-index');

            // console.log(gridRows, currentRowIndex);
            if (lastRowIndex === parentIndex) {
                addNewRowToGrid();
            }
        });

        $(templateRow).on('click', function (e) {
            focusRow(this);
        });

        return templateRow;

    };

    var focusRow = function (row) {
        $(row).removeClass('table-active').addClass('table-active');
        $(row).siblings().removeClass('table-active');
    };

    var createEmptyRowData = function () {
        return {
            refNbr: "",
            isDefault: false,
            street: "",
            postalCode: "",
            city: "",
            country: ""
        }
    };


    var bind = function () {
        var grid = $('#' + gridId);
        var gridHeader = grid.find('thead');
        var gridBody = $("<tbody></tbody>");
        var gridHeaderRow = $("<tr></tr>");
        var gridBodyRow = $("<tr></tr>");
        gridBodyRow.attr('id', gridId + "_template_row_");
        gridBodyRow.css('display', 'none');

        $.each(cols, function (index, value) {
            
            var gridHeaderCell = $("<th></th>");
            var gridBodyCell = $("<td></td>");
            gridHeaderCell.text(value.name);

            var cellInput = "";
            if (value.dataType === 'text') {
                cellInput = '<input class="form-control" type="text" />';
            }
            if (value.dataType === 'boolean') {
                cellInput = '<input type="checkbox" />';
            }
            if (value.dataType === 'number') {
                cellInput = '<input class="form-control" type="number" />';
            }

            if (cellInput !== "") {
                var cellInputVar = $(cellInput);
                cellInputVar.attr('data-propname', value.propName);
                cellInputVar.attr('title', value.name);
                cellInputVar.attr('id', gridId + "_template_row_" + value.propName);

                if (value.width) {
                    //
                    // pixels does not work well for table cells, TODO: use percentages
                    //
                    gridHeaderCell.css('width', value.width); // value.width should be in %age
                    cellInputVar.css('width', "100%");
                }

                if (value.keyColumn === true) {
                    cellInputVar.attr('disabled', true);
                    cellInputVar.attr('data-keycolumn', 'true');
                }

                cellInputVar.attr('placeholder', value.name);
                gridBodyCell.append(cellInputVar);
            }



            gridHeaderRow.append(gridHeaderCell);
            gridBodyRow.append(gridBodyCell);
        });

        gridHeader.append(gridHeaderRow);
        gridBody.append(gridBodyRow);

        grid.append(gridHeader);
        grid.append(gridBody);

        //
        // add data to the grid
        //
        bindDataSource(gridBody, dataSource.data);
    };


    var bindDataSource = function (gridBody, data) {
        if (!data || data.length <= 0) return;
        $.each(data, function (index, value) {
            var row = addNewRow(index, value, true);
            // console.log(row);
            gridBody.append(row);
            //
            // rowcategory = ADDED || DELETED || UPDATED || PRESTINE
            //
            $(row).attr('data-rowcategory', 'PRESTINE')
        });
    };

    var getDrityRows = function () {
        var dirtyRows = $('#' + gridId).find('tbody>tr').map(function () {
            if ($(this).attr('data-isdirty') === 'true') {
                return this;
            }
        });

        var records = [];
        if (dirtyRows.length === 0) {
            return [];
        }

        $.each(dirtyRows, function (index, row) {

            var rowInputs = $(row).find('input');
            var record = {};
            var rowCat = $(row).attr('data-rowcategory');
            record['rowCategory'] = rowCat;
            $.each(rowInputs, function (cellIndex, cell) {

                var cellPropName = $(cell).attr('data-propname');

                var cellType = $(cell).attr('type');

                if (cellType === 'checkbox') {
                    record[cellPropName] = $(cell).is(":checked");
                }
                else {
                    record[cellPropName] = $(cell).val();
                }
            });
            record["index"] = index;
            records.push(record);
        });

        return records;
    };

    var addNewRowToGrid = function () {
        var rowCount = $('#' + gridId).find('tbody>tr').length;
        var emptyRow = addNewRow(rowCount - 1, createEmptyRowData(), false);
        $('#' + gridId).find('tbody').append(emptyRow);
        $(emptyRow).find('input').first().focus();

        //
        // rowcategory = ADDED || DELETED || UPDATED
        //
        $(emptyRow).attr('data-rowcategory', 'ADDED');
        $(emptyRow).attr('data-isdirty', 'true');

        notfiyListeners(appDataEvents.ON_GRID_UPDATED, { dataSourceName: 'mainForm', event: emptyRow });

    };

    var getSelectedRow = function () {
        return $('.table-active').first();
    };

    var deleteRow = function () {
        var row = getSelectedRow();
        if (row.length === 0) return;
        var sibling = $(row).siblings(':visible').last();
        $(row).removeClass('table-active');
        $(row).attr('data-isdirty', 'true');

        $(row).css('display', 'none');
        //
        // rowcategory = NEW || DELETED || UPDATED || ADDED_DELETED
        //
        var rowCat = $(row).attr('data-rowcategory');
        if (rowCat === 'ADDED') {
            $(row).attr('data-rowcategory', 'ADDED_DELETED');
        }
        else {
            $(row).attr('data-rowcategory', 'DELETED');
        }

        notfiyListeners(appDataEvents.ON_GRID_UPDATED, { dataSourceName: 'mainForm', event: row });

        focusRow(sibling);
    };


    var registerCallback = function (key, eventTypeX, callback, dataSourceNameX) {
        if (!window.gridCallbacks || window.gridCallbacks.length === 0) {
            window.gridCallbacks = [];
        }

        //
        // search if callback exist from before
        //
        var index = window.gridCallbacks.findIndex(({ key, eventType, dataSourceName }) => key === gridId && eventType === eventTypeX && dataSourceName === dataSourceNameX);
        // console.log('index: ', index);
        if (index === -1) {
            window.gridCallbacks.push({ key: key, eventType: eventTypeX, callback: callback, dataSourceName: dataSourceNameX });
        }
    };

    var onHeaderNext = function (data) {
        console.log('Grid-Callback: ON_NEXT_RECORD. Data: ', data);
        clearGrid();
        var gridBody = $('#' + gridId).find('tbody');
        bindDataSource($(gridBody), data);

    };

    var onSaveRecord = function (data) {
        //
        // when main record is saved, disable the key columns of the grid
        //
        var keyColumns = $('#' + gridId).find('tbody>tr:visible').find("input[data-keycolumn='true']");
        $.each(keyColumns, function(index, col){
            $(col).attr('disabled', true);
        });

    };

    var notfiyListeners = function (eventType, payload) {
        try {
            if (!window.headerCallbacks || window.headerCallbacks.length === 0) {
                return;
            }

            //
            // search if callback exist from before
            //                
            var releventEvents = window.headerCallbacks.filter(function (value, index) {
                if (value.eventType === eventType)
                    return value;
            });

            if (releventEvents && releventEvents.length > 0) {
                $.each(releventEvents, function (index, ev) {
                    ev.callback(payload);
                });
            }

        } catch (error) {
            console.error(error);
        }
    };

    var registerCallbacks = function () {
        registerCallback(gridId, appDataEvents.GRID_DATA, getDrityRows, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_NEXT_RECORD, onHeaderNext, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_PREV_RECORD, onHeaderNext, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_LAST_RECORD, onHeaderNext, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_FIRST_RECORD, onHeaderNext, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_ADD_RECORD, onHeaderNext, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_FETCH_RECORD, onHeaderNext, dataSource.dataSourceName);

        registerCallback(gridId, appDataEvents.ON_SAVE_RECORD, onSaveRecord, dataSource.dataSourceName);

    };

    var clearGrid = function () {
        $('#' + gridId).find('.grid-row').remove();
    };

    return {
        bind,
        getDrityRows,
        addNewRowToGrid,
        deleteRow, // hides element from display and add to the list of dirty-rows
        registerCallbacks
    };
};