var gridNavigationService = function (gridOptions) {

    var cols = gridOptions.cols;
    var dataSource = gridOptions.dataSource;
    var gridId = gridOptions.gridId;


    var addNewRow = function (rowNumber, rowData, existingRecord) {
        var templateRow = $('#' + gridId + "_template_row_").clone();
        templateRow.attr('id', gridId + "_template_row_" + rowNumber);
        templateRow.css('display', 'table-row');
        var childs = templateRow.find('input, select');

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
            else if (cellType === 'select') {
                //console.log('select', xx);
                $(cellValue).val(xx).change();
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

        var rowInputs = $(templateRow).find('input, select');
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

            // remove any previous errors
            $(this).removeClass('is-invalid').attr('title', '');
            var tooltip = bootstrap.Tooltip.getInstance(this);
            if (tooltip)
                tooltip.dispose();

            notifyListeners(appDataEvents.ON_GRID_UPDATED, { dataSourceName: dataSource.dataSourceName, eventData: e });

        });

        rowInputs.on('focus', function (e) {
            var parent = $(e.target).parents('tr');
            focusRow(parent);
        });

        //
        // insert a new row if its the last input in the row
        //                    

        var lastCell = rowInputs.last();
        $(lastCell).on('keydown', onInputKeyDown);

        $(templateRow).on('click', function (e) {
            focusRow(this);
        });

        return templateRow;

    };

    var onInputKeyDown = function (e) {
        if (e.which !== 9 || e.shiftKey === true) return;

        var lastRowIndex = $('#' + gridId).find('tbody>tr:visible').last().attr('data-index');
        var parentIndex = $(e.target).parents('tr').first().attr('data-index');

        // console.log(gridRows, currentRowIndex);
        if (lastRowIndex === parentIndex) {
            addNewRowToGrid();
        }
    };

    var focusRow = function (row) {
        $(row).removeClass('table-active').addClass('table-active');
        $(row).siblings().removeClass('table-active');
    };

    var createEmptyRowData = function () {
        var record = {};
        $.each(cols, function () {
            record[this.propName] = undefined;
        });
        //debugger;
        return record;
    };

    var bind = function () {
        var grid = $('#' + gridId);

        grid.css('width', 'inherit');

        var gridHeader = grid.find('thead');
        var gridBody = $("<tbody></tbody>");
        var gridHeaderRow = $("<tr class='draggable grid-cols'></tr>");
        var gridBodyRow = $("<tr class='grid-rows'></tr>");
        gridBodyRow.attr('id', gridId + "_template_row_");
        gridBodyRow.css('display', 'none');

        $.each(cols, function (index, colDef) {

            var gridHeaderCell = $("<th class='sorting ds-col'></th>");
            var gridBodyCell = $("<td></td>");
            gridHeaderCell.text(colDef.name);

            var cellInput = '<input class="form-control" type="' + colDef.dataType + '" />';

            if (colDef.dataType === 'checkbox') {
                cellInput = '<input type="checkbox" />';
            }
            if (colDef.dataType === 'select')
                cellInput = '<select class="form-select"></select>';

            var cellInputVar = $(cellInput);
            cellInputVar.attr('data-propname', colDef.propName);
            cellInputVar.attr('title', colDef.name);
            cellInputVar.attr('id', gridId + "_template_row_" + colDef.propName);

            if (colDef.width) {
                //
                // pixels does not work well for table cells, TODO: use percentages
                //
                gridHeaderCell.css('width', colDef.width); // value.width should be in %age
                //cellInputVar.css('width', "100%");
            }

            if (colDef.keyColumn === true) {
                cellInputVar.attr('disabled', true);
                cellInputVar.attr('data-keycolumn', 'true');
            }

            cellInputVar.attr('placeholder', colDef.name);
            gridBodyCell.append(cellInputVar);

            //
            // if its a dropdown, then inflate with data from ds
            //
            if (colDef.dataType === 'select') {
                $.each(colDef.dataSource, function () {
                    var option = $("<option></option>");
                    option.val(this.value);
                    option.text(this.text);
                    cellInputVar.append(option);
                });

            }

            //
            // sorting of the data when the header cell is clicked
            //
            gridHeaderCell.attr('data-th-propname', colDef.propName);
            gridHeaderCell.on('click', function (e) {
                var elem = $(this);
                var asc = true;
                if (elem.hasClass('sorting_asc')) {
                    elem.removeClass('sorting_asc').addClass('sorting_desc');
                    asc = false;
                }
                else {
                    elem.removeClass('sorting_desc').addClass('sorting_asc');
                }

                //
                // supports sorting on only one column.
                //
                elem.siblings('th').removeClass('sorting_asc').removeClass('sorting_desc');

                //
                // notify that we need sorting of the column
                //
                var prop = $(e.target).attr('data-th-propname');
                notifyListeners(appDataEvents.ON_SORTING_REQUESTED,
                    { dataSourceName: dataSource.dataSourceName, eventData: e, propName: prop, asc });

            });

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

        //
        // notify that grid is data-bound
        //

        notifyListeners(appDataEvents.ON_GRID_DATA_BOUND, { dataSourceName: dataSource.dataSourceName, eventData: {}, source: grid });

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


    var getDirtyRows = function () {
        var dirtyRows = $('#' + gridId).find('tbody>tr').map(function () {
            if ($(this).attr('data-isdirty') === 'true') {
                return this;
            }
        });
        return dirtyRows;
    };

    var getDirtyRecords = function () {
        var dirtyRows = getDirtyRows();

        if (dirtyRows.length === 0) {
            return [];
        }
        var records = [];
        $.each(dirtyRows, function (index, row) {

            var rowInputs = $(row).find('input, select');
            var rowIndex = $(row).attr('data-index');
            if (rowIndex) {
                rowIndex = parseInt(rowIndex);
            }
            var record = {};
            var rowCat = $(row).attr('data-rowcategory');
            record['rowCategory'] = rowCat;
            $.each(rowInputs, function (cellIndex, rowInput) {

                var cellPropName = $(rowInput).attr('data-propname');

                var cellType = $(rowInput).attr('type');

                if (cellType === 'checkbox') {
                    record[cellPropName] = $(rowInput).is(":checked");
                }
                else {
                    record[cellPropName] = $(rowInput).val();
                }
            });
            record["clientRowNumber"] = rowIndex;
            records.push(record);
        });

        return records;
    };

    var addNewRowToGrid = function () {
        var rowCount = $('#' + gridId).find('tbody>tr').length;
        var emptyRow = addNewRow(rowCount - 1, createEmptyRowData(), false);
        $('#' + gridId).find('tbody').append(emptyRow);
        $(emptyRow).find('input, select').first().focus();

        //
        // rowcategory = ADDED || DELETED || UPDATED
        //
        $(emptyRow).attr('data-rowcategory', 'ADDED');
        $(emptyRow).attr('data-isdirty', 'true');

        notifyListeners(appDataEvents.ON_GRID_UPDATED, { dataSourceName: dataSource.dataSourceName, eventData: emptyRow });

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

        notifyListeners(appDataEvents.ON_GRID_UPDATED, { dataSourceName: dataSource.dataSourceName, eventData: row });

        focusRow(sibling);
    };


    var registerCallback = function (key, eventTypeX, callback, dataSourceNameX) {
        dataEventsService.registerCallback(key, eventTypeX, callback, dataSourceNameX);
    };

    var onHeaderNext = function (eventArgs) {

        if (!eventArgs || !eventArgs.eventData) return;

        resetSorting();
        clearGrid();
        var gridBody = $('#' + gridId).find('tbody');
        bindDataSource($(gridBody), eventArgs.eventData[dataSource.dataSourceName]);

    };

    var onSaveRecord = function (eventArgs) {

        var grid = $('#' + gridId);

        //
        // remove rows from the grid that has been deleted
        //

        grid.find("tr[data-rowcategory='DELETED']").remove();
        grid.find("tr[data-rowcategory='ADDED_DELETED']").remove();

        //
        // when main record is saved, disable the key columns of the grid,
        //
        var visibleRows = grid.find('tbody>tr:visible');
        visibleRows.find("input[data-keycolumn='true']").attr('disabled', true);

        //
        // mark row as prestine on successful saving
        //
        visibleRows.attr('data-rowcategory', 'PRESTINE');

    };

    var onSaveError = function (eventArgs) {

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
        / }*/

        if (!eventArgs || !eventArgs.eventData || !eventArgs.eventData.responseJSON) return;
        var errors = eventArgs.eventData.responseJSON;
        var dsName = dataSource.dataSourceName;
        var dirtyRows = getDirtyRows();
        var grid = $('#' + gridId);

        for (let i = 0; i < dirtyRows.length; i++) {
            var errorProp = dsName + '[' + i + ']';
            var im = errors[errorProp];
            if (im && im.length > 0) {
                var clientIndex = im[0];
                var serverIndex = i;

                var errorRow = getRowByIndex(grid, parseInt(clientIndex));
                if (!errorRow || errorRow.length === 0) continue;

                $.each(cols, function (x, col) {
                    var propName = col.propName.toPascalCaseJson();
                    var inputError = errors[dsName + '[' + serverIndex + '].' + propName];
                    if (inputError && inputError.length > 0) {
                        var input = errorRow.find("input[data-propname=" + col.propName + "]");
                        if (input && input.length > 0) {
                            input.addClass('is-invalid');
                            //console.log(inputError);
                            var allErrors = '';
                            $.each(inputError, function (ie, er) {
                                allErrors += er + ' ';
                            });
                            input.attr('title', allErrors);
                            var tooltip = new bootstrap.Tooltip(input[0], { customClass: 'tooltip-error' });
                        }
                    }

                });
            }
        }

    }

    var getRowByIndex = function (grid, index) {
        return grid.find("tr[data-index = '" + index + "']");
    };

    var onSortingRequest = function (eventArgs) {
        console.log(eventArgs);

        var ds = eventArgs.dataSourceName;
        var prop = eventArgs.propName;
        var $target = $(eventArgs.eventData.target);
        var table =$target.parents('table');
        
        var isTh = $target.prop('tagName').toLowerCase() === 'th';

        if (!isTh) {
            var th = $target.parents('th');
            if (!th || th.length === 0) return;

            eventArgs.eventData.target = th[0];
        }
        
        $(table).sortTable(eventArgs.eventData.target, eventArgs.asc);
    };

    var resetSorting = function () {
        $('#' + gridId)
            .find('.sorting_desc, .sorting_asc')
            .removeClass('sorting_desc')
            .removeClass('sorting_asc');
    };

    var onColsReordered = function (eventArgs) {

        //
        // modify 'keydown' events on the row inputs
        //
        var grid = eventArgs.source;

        var rows = grid.find('tbody>tr');
        $.each(rows, function () {
            var $row = $(this);
            var inputs = $row.find('input, select');
            inputs.off('keydown');
            var lastInput = inputs.last();
            lastInput.on('keydown', onInputKeyDown);
        });


    };

    var notifyListeners = function (eventType, payload) {

        dataEventsService.notifyListeners(eventType, payload);

    };

    var registerCallbacks = function () {
        registerCallback(gridId, appDataEvents.GRID_DATA, getDirtyRecords, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_ADD_RECORD, onHeaderNext, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_FETCH_RECORD, onHeaderNext, dataSource.dataSourceName);

        registerCallback(gridId, appDataEvents.ON_SAVE_RECORD, onSaveRecord, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_SAVE_ERROR, onSaveError, dataSource.dataSourceName);
        registerCallback(gridId, appDataEvents.ON_SORTING_REQUESTED, onSortingRequest, dataSource.dataSourceName);

        registerCallback(gridId, appDataEvents.ON_COLS_REORDERED, onColsReordered, dataSource.dataSourceName);

    };

    var clearGrid = function () {
        $('#' + gridId).find('.grid-row').remove();
    };

    return {
        bind,
        getDirtyRecords,
        addNewRowToGrid,
        deleteRow, // hides element from display and add to the list of dirty-rows
        registerCallbacks
    };
};