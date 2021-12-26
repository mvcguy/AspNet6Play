var gridNavigationService = function (gridOptions) {

    var cols = gridOptions.cols;
    var dataSource = gridOptions.dataSource;
    var gridId = gridOptions.gridId;

    var bind = function () {
        console.log("Binding grid...");

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


        console.log('Grid header is prepared');
        console.log(gridHeader);
        $('#' + gridId).append(gridHeader);
        $('#' + gridId).append(gridBody);

        $.each(dataSource.data, function (index, value) {
            var templateRow = $('#' + gridId + "_template_row_").clone();
            templateRow.attr('id', gridId + "_template_row_" + index);
            templateRow.css('display', 'table-row');
            var childs = templateRow.find('input');
            $.each(childs, function (cellIndex, cellValue) {
                var oldId = $(cellValue).attr('id');
                $(cellValue).attr('id', oldId + "_" + index);
                var cellPropName = $(cellValue).attr('data-propname');
                var cellType = $(cellValue).attr('type');
                var xx = value[cellPropName];
                if (cellType === 'checkbox') {
                    if (xx === 'true' || xx === 'True' || xx === true) {
                        $(cellValue).attr('checked', 'checked');
                    }
                }
                else {
                    $(cellValue).val(xx);
                }
            });

            gridBody.append(templateRow);

        });

    };

    return {
        bind,
    };
};