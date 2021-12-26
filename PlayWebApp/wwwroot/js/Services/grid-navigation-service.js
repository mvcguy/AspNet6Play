var gridNavigationService = function (gridOptions) {

    var cols = gridOptions.cols;
    var dataSource = gridOptions.dataSource;
    var gridId = gridOptions.gridId;


    var addNewRow = function (rowNumber, rowData) {
        var templateRow = $('#' + gridId + "_template_row_").clone();
        templateRow.attr('id', gridId + "_template_row_" + rowNumber);
        templateRow.css('display', 'table-row');
        var childs = templateRow.find('input');

        $.each(childs, function (cellIndex, cellValue) {
            var oldId = $(cellValue).attr('id');
            $(cellValue).attr('id', oldId + "_" + rowNumber);
            var cellPropName = $(cellValue).attr('data-propname');
            var cellType = $(cellValue).attr('type');
            var xx = rowData[cellPropName];
            if (cellType === 'checkbox') {
                if (xx === 'true' || xx === 'True' || xx === true) {
                    $(cellValue).attr('checked', 'checked');
                }
            }
            else {
                $(cellValue).val(xx);
            }

        });

        $(templateRow).attr('data-index', rowNumber);

        //
        // insert a new row if its the last input in the row
        //                    

        var rowInputs = $(templateRow).find('input');
        var lastCell = rowInputs.last();
        rowInputs.on('change', function (e) {
            var parent = $(e.target).parents('tr');
            $(parent).attr('data-isdirty', true);
        });


        $(lastCell).on('keydown', function (e) {

            if (e.which !== 9) return;

            var gridBody = $('#' + gridId).find('tbody');
            var gridRows = gridBody.find('tr').length;

            var parent = $(e.target).parents('tr');

            var currentRowIndex = parseInt($(parent).attr('data-index'));

            var isLastRow = (gridRows - 2) === currentRowIndex;
            console.log(gridRows, currentRowIndex);
            if (isLastRow) {
                var emptyRow = addNewRow(rowNumber + 1, createEmptyRowdData());
                gridBody.append(emptyRow);
                $(emptyRow).find('input').first().focus();
            }
        });

        return templateRow;

    };

    var createEmptyRowdData = function () {
        return {
            refNbr: "SELECT",
            isDefault: false,
            street: "",
            postalCode: "0000",
            city: "",
            country: "SELECT"
        }
    };


    var bind = function () {

        var gridHeader = $("<thead></thead>");
        var gridBody = $("<tbody></tbody>");
        var gridHeaderRow = $("<tr></tr>");
        var gridBodyRow = $("<tr></tr>");
        gridBodyRow.attr('id', gridId + "_template_row_");
        gridBodyRow.css('display', 'none');


        $.each(cols, function (index, value) {
            var gridHeaderCell = $("<th></th>");
            var gridBodyCell = $("<td></td>");
            gridHeaderCell.text(value.name);

            if (value.width) {
                gridHeaderCell.css('width', value.width + "px");
            }

            var cellInput = "";
            if (value.dataType === 'text') {
                cellInput = '<input type="text" />';
            }
            if (value.dataType === 'boolean') {
                cellInput = '<input type="checkbox" />';
            }
            if (value.dataType === 'number') {
                cellInput = '<input type="number" />';
            }

            if (cellInput !== "") {
                var cellInputVar = $(cellInput);
                cellInputVar.attr('data-propname', value.propName);
                cellInputVar.attr('title', value.name);
                cellInputVar.attr('id', gridId + "_template_row_" + value.propName);
                gridBodyCell.append(cellInputVar);
            }

            gridHeaderRow.append(gridHeaderCell);
            gridBodyRow.append(gridBodyCell);
        });

        gridHeader.append(gridHeaderRow);
        gridBody.append(gridBodyRow);

        $('#' + gridId).append(gridHeader);
        $('#' + gridId).append(gridBody);

        //
        // add data to the grid
        //

        $.each(dataSource.data, function (index, value) {
            var row = addNewRow(index, value);
            gridBody.append(row);
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
            $.each(rowInputs, function (cellIndex, cell) {

                var cellPropName = $(cell).attr('data-propname');

                var cellType = $(cell).attr('type');

                if (cellType === 'checkbox') {
                    record[cellPropName] = $(cell).is(":checked");
                }
                else {
                    record[cellPropName] = $(cell).val();
                }

                record["index"] = index;
            });
            records.push(record);
        });

        return records;
    };

    var addNewRowToGrid = function () {
        var rowCount = $('#' + gridId).find('tbody>tr').length;
        var emptyRow = addNewRow(rowCount -1, createEmptyRowdData());
        $('#' + gridId).find('tbody').append(emptyRow);
        $(emptyRow).find('input').first().focus();
    };

    return {
        bind,
        getDrityRows,
        addNewRowToGrid,
    };
};