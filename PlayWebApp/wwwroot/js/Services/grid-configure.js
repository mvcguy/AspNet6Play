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

    var $table = $(this);
    var addWaitMarker = function () {
        var dw = $('<div></div>');
        dw.addClass('wait-reorder').hide();
        var ct = $('<div class="d-flex justify-content-center"></div>');
        var ds = $('<div></div>').addClass('spinner-border');
        ds.append('<span class="visually-hidden">Wait...</span>');
        ct.append(ds);
        dw.append(ct);
        $table.addClass('caption-top');
        $table.append($('<caption></caption>').append(dw));
    };

    var thWrap = $('<div draggable="true" class="grid-header"></div>');
    
    $.each($table.find('.grid-cols>th:visible'), function () {
        var childs = $(this).children();
        if (childs.length === 0) {
            var txt = $(this).text();
            $(this).text('');
            childs = $('<div></div>').text(txt);
            $(this).append(childs);
        }

        $(childs).wrap(thWrap);
    });

    addWaitMarker();

    var srcElement;

    //jQuery.event.props.push('dataTransfer');
    $table.find('.grid-header').on({
        dragstart: function (e) {
            if (!$(e.target).hasClass('grid-header')) {
                srcElement = undefined;
                return;
            };

            srcElement = e.target;
            $(this).css('opacity', '0.5');
        },
        dragleave: function (e) {
            e.preventDefault();
            if (!srcElement) return;

            if (!$(this).hasClass('grid-header')) return;
            $(this).removeClass('over');
        },
        dragenter: function (e) {
            e.preventDefault();
            if (!srcElement) return;

            if (!$(this).hasClass('grid-header')) return;
            $(this).addClass('over');
            // e.preventDefault();
        },
        dragover: function (e) {
            e.preventDefault();
            if (!srcElement) return;

            if (!$(this).hasClass('grid-header')) return;
            $(this).addClass('over');


        },
        dragend: function (e) {
            e.preventDefault();
            if (!srcElement) return;
            $(this).css('opacity', '1');
        },
        drop: function (e) {
            e.preventDefault();
            if (!srcElement) return;
            var $this = $(this);
            $this.removeClass('over');
            var destElement = e.target;
            if (!$this.hasClass('grid-header')) return;
            if (srcElement === destElement) return;

            var cols = $table.find('.grid-cols>th:visible');

            // dest
            var thParent = $this.parents('th');
            var toIndex = cols.index(thParent);

            // src
            var thParent2 = $(srcElement).parents('th');
            var fromIndex = cols.index(thParent2);

            console.log(toIndex, fromIndex);

            if (toIndex == fromIndex) return;

            var hRow = $this.parents('tr');

            reOrder(hRow, cols, fromIndex, toIndex);

            var rows = $table.find('tbody>tr');
            $('.wait-reorder').css({ 'cursor': 'progress' }).show();
            setTimeout(() => {
                console.log('Reordering started, ', new Date());
                for (let index = 0; index < rows.length; index++) {
                    var row = rows[index];
                    var cells = $(row).find('td:visible');
                    if (toIndex == fromIndex) return;
                    reOrder(row, cells, fromIndex, toIndex);
                }
                $('.wait-reorder').css({ 'cursor': '' }).hide();
                console.log('Reordering completed, ', new Date());
            }, 500);

        }
    });

    var reOrder = function (row, cells, fromIndex, toIndex) {

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

        $(row).append(cells);
    };

    var swapRtl = function (cells, fromIndex, toIndex) {
        for (let i = fromIndex; i > toIndex; i--) {
            swap(cells, i, i - 1);
        }
    };

    var swapLtr = function (cells, fromIndex, toIndex) {
        for (let i = fromIndex; i < toIndex; i++) {
            swap(cells, i, i + 1);
        }
    };

    var swap = function (arr, ia, ib) {
        var temp = arr[ia];
        arr[ia] = arr[ib];
        arr[ib] = temp;
    };

    var directions = { rtl: 'RIGHT-TO-LEFT', ltr: 'LEFT-TO-RIGHT' };
}