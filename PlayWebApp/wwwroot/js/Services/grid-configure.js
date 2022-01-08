
/**
 * Allow for hiding/showing grid columns
 */
$.fn.gridConfigure = function () {

    var headers = this.find('.ds-col');
    var $table = $(this);
    var dataSourceName = $table.attr('data-datasource');

    var colsList = $('.grid-config-cols');
    $.each(headers, function () {

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

        chk.on('click', function (e) {
            var $chk = $(this);
            var prop = $chk.attr('data-config-propname');
            if (!prop) return;

            var col = $table.find('th[data-th-propname=' + prop + ']');
            if (!col || col.length <= 0) return;

            var rows = $table.find('.grid-cols, .grid-rows');

            var index = Array.from(col.parent('tr').children()).indexOf(col[0]);
            if (index < 0) return;

            $.each(rows, function () {
                var $row = $(this);
                var cells = $row.find('td, th')[index];

                if (!cells || cells.length <= 0) return;

                if ($chk.is(':checked') === true) {
                    $(cells).show();
                }
                else {
                    $(cells).hide();
                }
            });

            dataEventsService.notifyListeners(appDataEvents.ON_GRID_CONFIG_UPDATED,
                { dataSourceName: dataSourceName, eventData: e, source: $table, action: appActions.COL_SHOW_HIDE });


        });
    });

}

/**
 * Allow for re-ordering the grid columns
 */
$.fn.enableColumnReordering = function () {

    var $table = $(this);
    var dataSourceName = $table.attr('data-datasource');
    //var gridId = $table.attr('id');
    //console.log('datasource-name', dataSourceName);
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

    $.each($table.find('.grid-cols>th'), function () {
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

            var cols = $table.find('.grid-cols>th');

            // dest
            var destParent = $this.parents('th');
            var toIndex = cols.index(destParent);

            // src
            var srcParent = $(srcElement).parents('th');
            var fromIndex = cols.index(srcParent);

            //console.log(toIndex, fromIndex);

            if (toIndex == fromIndex) return;

            var hRow = $this.parents('tr');

            //
            // apply new order to the headers
            //
            reOrder(hRow, cols, fromIndex, toIndex);

            var rows = $table.find('tbody>tr');
            $('.wait-reorder').css({ 'cursor': 'progress' }).show();

            //
            // apply new order to all the rows in the grid
            //
            setTimeout(() => {
                //console.log('Reordering started, ', new Date());
                for (let index = 0; index < rows.length; index++) {
                    var row = rows[index];
                    var cells = $(row).find('td');
                    if (toIndex == fromIndex) return;
                    reOrder(row, cells, fromIndex, toIndex);
                }

                //console.log('Reordering completed, ', new Date());
                //
                // notify about column re-ordering
                //
                notifyListeners(appDataEvents.ON_COLS_REORDERED,
                    { dataSourceName: dataSourceName, eventData: e, source: $table });

                notifyListeners(appDataEvents.ON_GRID_CONFIG_UPDATED,
                    { dataSourceName: dataSourceName, eventData: e, source: $table, action: appActions.COL_REORDER });

                $('.wait-reorder').css({ 'cursor': '' }).hide();
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


    var notifyListeners = function (eventType, payload) {
        dataEventsService.notifyListeners(eventType, payload);
    };
};


/**
 * Allow for sorting the grid data based on the choosen column
 * Credit: https://stackoverflow.com/a/49041392
 * @param {*} th - the header object th to sort
 * @param {boolean} ascX - sort ascending or vice versa
 */

$.fn.sortTable = function (th, ascX) {

    //  console.log('sorting', ascX);
    const getCellValue = (tr, idx) => {
        var child = $($(tr).children()[idx]);
        // console.log('idx: ', idx,  child);
        var text = child.find('input, select').is(":checked") || child.find('input, select').val() || child.text();
        //console.log(text);
        return text;
    };


    // Returns a function responsible for sorting a specific column index 
    // (idx = columnIndex, asc = ascending order?).
    var comparer = function (idx, asc) {
        //console.log('idx: ', idx, 'asc: ', asc);
        // This is used by the array.sort() function...
        return function (a, b) {
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
    var rows = this.find('tbody>tr:visible');
    const tbody = this.find('tbody');
    var $table = $(this);
    var dataSourceName = $table.attr('data-datasource');
    // console.log(rows);
    Array.from(rows)
        .sort(comparer(Array.from(th.parentNode.children).indexOf(th), ascX = !ascX))
        .forEach(tr => $(tbody).append(tr));

    dataEventsService.notifyListeners(appDataEvents.ON_GRID_CONFIG_UPDATED,
        { dataSourceName: dataSourceName, eventData: { th, asc: ascX }, source: $table, action: appActions.COL_SORTING });

};

//
// auto register events to enable grid col configuration and allow for re-ordering of the columns
//
(function () {
    var registerCallback = function (key, eventTypeX, callback, dataSourceNameX) {
        dataEventsService.registerCallback(key, eventTypeX, callback, dataSourceNameX);
    };

    var onGridDataBound = function (eventArgs) {
        //console.log('grid is data bounded', eventArgs);
        var grid = eventArgs.source;
        var gridId = grid.attr('id');

        try {
            var gridSettings = Cookie.getJSON(gridId);
            console.log('GridSettings Cookie: ', gridSettings ? 'settings found' : 'no settings found!');

            if (gridSettings) {

            }

        } catch (error) {
            console.log(error);
        }

        //
        // enables the configuration of columns
        //
        grid.gridConfigure();

        //
        // enables to re-order the columns
        //
        grid.enableColumnReordering();

        //
        // make the grid resixeable
        //
        grid.resizableGrid();
    };

    var onGridConfigurationChanged = function (eventArgs) {
        //console.log('grid configuration updated', eventArgs);

        var table = eventArgs.source;
        var action = eventArgs.action;
        var gridId = table.attr('id');

        var cols = table.find('.ds-col');
        // console.log(cols);
        var colsObj = {};
        $.each(cols, function (index, val) {
            var col = $(this);

            var sort = 'asc';
            if (col.hasClass('sorting_desc'))
                sort = 'desc';

            var prop = col.attr('data-th-propname');

            var propAttr = {

                width: col.css('width'),
                visible: col.is(':visible'),
                sort: sort,
                position: index,
            };

            colsObj[prop] = propAttr;
        });



        Cookie.delete(gridId);
        setTimeout(() => {
            // console.log('Colsobject: ', colsObj);
            Cookie.setJSON(gridId, colsObj, { days: 30, secure: true, SameSite: 'strict' });
        }, 500);

    };

    $(document).ready(function () {

        //console.log('iffy invoked');
        var grids = $(document).find('table');
        //console.log(grids);

        $.each(grids, function () {

            var $grid = $(this);
            var id = $grid.attr('id');
            var dsName = $grid.attr('data-datasource');

            //console.log(id, dsName);

            if (!id || !dsName) {
                console.log('no datasource found');
                return;
            }

            registerCallback(id, appDataEvents.ON_GRID_DATA_BOUND, onGridDataBound, dsName);
            registerCallback(id, appDataEvents.ON_GRID_CONFIG_UPDATED, onGridConfigurationChanged, dsName);

        })

    });

})();

