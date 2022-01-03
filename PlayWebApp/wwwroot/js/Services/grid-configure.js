$.fn.gridConfigure = function () {

    var headers = this.find('.ds-col');
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
            var $chk = $(this);
            var prop = $chk.attr('data-config-propname');
            if (!prop) return;

            var col = $(_this).find('th[data-th-propname=' + prop + ']');
            if (!col || col.length <= 0) return;

            var rows = $(_this).find('.grid-cols, .grid-rows');

            var index = Array.from(col.parent('tr').children()).indexOf(col[0]);
            if (index < 0) return;

            $.each(rows, function () {
                var $row = $(this);
                var cols = $row.find('td, th')[index];

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

}

$.fn.enableColumnReordering = function () {
    console.log('enabling column re-ordering feature...');
    var $table = $(this);
    var $header = $table.find('thead');
    var cols = $header.find('th');
    cols.css('cursor', 'pointer');
    cols.css('user-select', 'none');

    var colPos = 0;
    var curCol = undefined;

    cols.on('mousedown', function (e) {
        var col = this;
        curCol = 'x';
        console.log(e);

        var rows = $table.find('tr');
        var index = Array.from($(col).parent('tr').children()).indexOf(col);
        // console.log(index);
        var ofs = $(col).offset();
        var pos = $(col).position();
        var x = e.pageX;
        var y = e.pageY;

        colPos = x;

        console.log(ofs, pos, x, y);

        $(col).css('cursor', 'move');

        $.each(rows, function () {
            var $row = $(this);
            var cls = $row.find('td, th')[index];

            if (!cls || cls.length <= 0) return;
            $(cls).css('border', 'solid 1px gray');
            $(cls).css('position', 'relative');


        });
    });

    cols.on('mouseup', function () {
        var col = this;
        curCol = undefined;

        var rows = $table.find('tr');
        var index = Array.from($(col).parent('tr').children()).indexOf(col);
        // console.log(index);
        $(col).css('cursor', 'pointer');
        $.each(rows, function () {
            var $row = $(this);
            var cls = $row.find('td, th')[index];

            if (!cls || cls.length <= 0) return;
            $(cls).css('border', '');
            $(cls).css('position', '');

        });
    });

    cols.on('mousemove', function (e) {
        if (!curCol) return;
        var col = this;
        var curPos = e.pageX;
        var diff = curPos - colPos;
        var pos = $(col).position();
        if (diff < 0) return;

        $(col).css('border', '1px solid red').css('position', 'relative');
        // $(col).css('display', 'block');
        $(col).css('left', pos.left + diff + 'px');

        if (diff < 20) return;


        // console.log(e);
        var rows = $table.find('tr');
        var index = Array.from($(col).parent('tr').children()).indexOf(col);
        // console.log(index);
        $.each(rows, function () {
            var $row = $(this);
            var cls = $row.find('td, th')[index];


            if (!cls || cls.length <= 0) return;
            var $cls = $(cls);

            //$cls.css('display', 'block');

            var col0 = $row.find('td, th')[0];
            var col1 = $row.find('td, th')[1];
            //console.log(col0, col1);

            //swap
            var temp = col0;
            col0 = col1;
            col1 = temp;
            $row.append(col0);
            $row.append(col1);

            $(col0).css('border', '');
            $(col1).css('position', '');


        });
        curCol = undefined;
    });

}