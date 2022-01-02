$.fn.gridConfigure = function () {

    var headers = this.find('.ds-col');
    //console.log(headers);
    var _this = this;

    var colsList = $('.grid-config-cols');
    $.each(headers, function (index) {

        var $header = $(this);
        var propName = $header.attr('data-th-propname');
        if (!propName) return;

        var colsListItem = $('<li class="list-group-item"></li>');

        var chk = $('<input type="checkbox" value="" class="form-check-input me-1" />');
        var chkId = 'col_config_chk_' + propName;
        chk.attr('id', chkId);
        chk.attr('data-config-propname', propName);
        if ($header.is(':visible')) {
            chk.attr('checked', 'checked');
        }

        var chkLbl = $('<label for="' + chkId + '"></label>');
        chkLbl.text($header.text());

        colsListItem.append(chk);
        colsListItem.append(chkLbl);
        colsList.append(colsListItem);

        chk.on('click', function () {
            // continue
            var $chk = $(this);
            var prop = $chk.attr('data-config-propname');
            if (!prop) return;

            var col = $(_this).find('th[data-th-propname=' + prop + ']');
            if (!col || col.length <= 0) return;

            var rows = $(_this).find('.grid-cols, .grid-rows');

            var index = Array.from(col.parent('tr').children()).indexOf(col[0]);
            if (index < 0) return;

            //var rows = $(_this).parents.find('th[data-th-propname=' + prop + ']').parents('tr');
            $.each(rows, function () {
                var cols = $(this).find('td, th')[index];

                if (!cols || cols.length <= 0) return;

                if ($chk.is(':checked') === true) {
                    $(cols).show();
                }
                else {
                    $(cols).hide();
                }

            });


        });
    });


    var saveButton = $('#btnSaveGridConfig');
    saveButton.on('click', function () {
        var colsList = $('.grid-config-cols');
    });


}