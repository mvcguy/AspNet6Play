//
// Credit:
// https://stackoverflow.com/questions/58660470/resizing-column-width-on-a-table
//

$.fn.resizableGrid = function () {

    var table = this[0];
    var $table = this;
    var dataSourceName = $table.attr('data-datasource');
    // console.log(table);
    var row = table.getElementsByClassName('draggable')[0],
        cols = row ? row.children : undefined;
    if (!cols) return;

    table.style.overflow = 'hidden';

    var tableHeight = table.offsetHeight;

    for (var i = 0; i < cols.length; i++) {
        var div = createDiv(tableHeight);
        cols[i].appendChild(div);
        cols[i].style.position = 'relative';
        setListeners(div, table);
    }

    function setListeners(div, table) {
        var pageX, curCol, nxtCol, curColWidth, nxtColWidth, tableWidth;

        div.addEventListener('mousedown', function (e) {

            // tableWidth = document.getElementById('tableId').offsetWidth;
            tableWidth = table.offsetWidth;

            curCol = e.target.parentElement;
            nxtCol = curCol.nextElementSibling;
            pageX = e.pageX;

            var padding = paddingDiff(curCol);

            curColWidth = curCol.offsetWidth - padding;
            //console.log('MDown: tableW: ', tableWidth, 'pageX: ', pageX, 'padding: ', padding, 'colW: ', curColWidth);
        });

        div.addEventListener('mouseover', function (e) {
            e.target.style.borderRight = '2px solid #0000ff';
        })

        div.addEventListener('mouseout', function (e) {
            e.target.style.borderRight = '';
        })

        document.addEventListener('mousemove', function (e) {
            if (curCol) {
                var diffX = e.pageX - pageX;

                curCol.style.width = (curColWidth + diffX) + 'px';
                // document.getElementById('tableId').style.width = tableWidth + diffX + "px"
                table.style.width = tableWidth + diffX + "px";

                // console.log('MMove: diffX: ', diffX, 'curColW: ', curCol.style.width, 'TableW: ', table.style.width);

            }
        });

        document.addEventListener('mouseup', function (e) {

            if (curCol) {
                dataEventsService.notifyListeners(appDataEvents.ON_GRID_CONFIG_UPDATED,
                    { dataSourceName: dataSourceName, eventData: { e, curCol }, source: $table, action: appActions.COL_RESIZED });

            }

            curCol = undefined;
            nxtCol = undefined;
            pageX = undefined;
            nxtColWidth = undefined;
            curColWidth = undefined
        });
    }

    function createDiv(height) {
        var div = document.createElement('div');
        div.style.top = 0;
        div.style.right = 0;
        div.style.width = '5px';
        div.style.position = 'absolute';
        div.style.cursor = 'col-resize';
        div.style.userSelect = 'none';
        div.style.height = height + 'px';
        return div;
    }

    function paddingDiff(col) {

        if (getStyleVal(col, 'box-sizing') == 'border-box') {
            return 0;
        }

        var padLeft = getStyleVal(col, 'padding-left');
        var padRight = getStyleVal(col, 'padding-right');
        return (parseInt(padLeft) + parseInt(padRight));

    }

    function getStyleVal(elm, css) {
        return (window.getComputedStyle(elm, null).getPropertyValue(css));
    }
}
