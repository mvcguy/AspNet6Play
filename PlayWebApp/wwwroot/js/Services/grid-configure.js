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

    // var colPos = 0;
    // var div = undefined;

    // cols.on('mousedown', function (e) {
    //     var col = $(this);
    //     var hdr = col.find('.grid-header-text');

    //     if (!hdr || hdr.length <= 0) return;
    //     var pos = hdr.position();
    //     var off = hdr.offset();

    //     div = hdr.clone();

    //     // div.css({top: pos.top, left: pos.left, position:'absolute', border:'solid 1px red'});

    //     div.css({ top: off.top, left: off.left, position: 'absolute', border: 'solid 1px red' });

    //     div.attr('draggable', true);
    //     div.addClass('clone')
    //     div.position(pos);
    //     col.append(div);

    //     // console.log(pos, div.position());
    // });

    // cols.on('mouseup', function () {
    //     var col = $(this);
    //     col.find('.clone').remove();
    //     div = undefined;
    // });

    // cols.on('mousemove', function (e) {

    //     if (!div) return;

    //     var x = e.pageX;
    //     var y = e.pageY;
    //     var off = { top: y, left: x };

    //     div.css({ top: off.top, left: off.left, position: 'absolute', border: 'solid 1px red' });

    // });

    var srcParent;
    var srcElement;
    var destElement;
    //jQuery.event.props.push('dataTransfer');
    $('.grid-header').on({
        dragstart: function (e) {
            if ($(e.target).hasClass('grid-header')) {
                srcElement = e.target;
                $(this).css('opacity', '0.5');
            }

        },
        dragleave: function (e) {
            e.preventDefault();
            $(this).removeClass('over');
        },
        dragenter: function (e) {
            e.preventDefault();
            $(this).addClass('over');
            // e.preventDefault();
        },
        dragover: function (e) {
            $(this).addClass('over');
            e.preventDefault();
        },
        dragend: function (e) {
            e.preventDefault();
            $(this).css('opacity', '1');
        },
        drop: function (e) {

            e.preventDefault();
            // e.stopPropagation();

            console.log(e.target.className);
            if ($(e.target).hasClass('grid-header')) {

                /*
                 Swap columns
                1. Remove the element from the source
                2. Add to the destination
                3. Remove the element from destination
                4. Add the element to the source
                */

                // e.target.style.background = "";

                destElement = e.target; // 3 remove from destination

                if(srcElement === destElement) return;

                var srcParent = srcElement.parentNode;
                var destParent = destElement.parentNode;

                srcParent.removeChild(srcElement); // 1 remove from source
                destParent.removeChild(destElement);

                destParent.appendChild(srcElement); // 2 insert in the destination
                srcParent.appendChild(destElement);

                $(this).removeClass('over');
            }
        }
    });


}