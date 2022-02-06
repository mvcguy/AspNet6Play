$(function () {

    console.log('hello from index');
    //
    // sample using bootstrap data grid 
    //
    var cols = [];
    var initData = [];

    var totCols = 2, totRows = 60;
    for (let i = 0; i < totCols; i++) {
        cols.push(new BSGridColDefinition("COL-" + i, "text", "180px", "col-" + i, false));
    }

    for (let i = 0; i < totRows; i++) {

        var record = {};
        for (let j = 0; j < totCols; j++) {
            record['col-' + j] = 'DATA-' + i + '-' + j;
        }
        initData.push(record);
    }

    var dataSource = new BSGridDataSource('fakeData',
        {
            initData,
            metaData: new PagingMetaData(1, 5, totRows)
        }, false, null,
        (page, data, mdata) => {
            var start = page <= 1 ? 0 : (page - 1) * mdata.pageSize;
            var end = start + mdata.pageSize;
            var maxIndex = data.length - 1;
            if (start > maxIndex || end > maxIndex) return [];
            var pageData = [];
            for (let index = start; index < end; index++) {
                const element = data[index];
                pageData.push(element);
            }
            return pageData;
        });

    var bs = new BSGridOptions("fakeData_table", "dummy-data-container", cols, dataSource);

    var grid = new BootstrapDataGrid(bs);
    grid.registerCallbacks();
    grid.render();

    //
    // infinite scroll sample
    //
    var jq = $;
    var tbody = jq('#infinite_scroll');

    var length = initData.length;
    jq.each(initData, (i, v) => {
        var row = jq('<tr></tr>')
        // console.log(v);
        row.append(`<td>${v['col-0']}</td>`);
        row.append(`<td>${v['col-1']}</td>`);
        if (i === length - 1) {
            row.attr('id', 'scroll_target');
        }
        tbody.append(row);
    });

    let options = {
        root: document.querySelector('#scroll_area'),
        rootMargin: '0px',
        threshold: 0.8,
        trackVisibility: false
    }

    var target = document.querySelector('#scroll_target');

    let observer = new IntersectionObserver((entries, sender) => {

        var entry = entries[0];
        console.log(entry);
        if (entry.isIntersecting === true) {
            console.log('Observer is invoked. Entry: ', entry);
        }
        

    }, options);

    observer.observe(target);

});   