$(function () {

    console.log('hello from index');
    //
    // sample using bootstrap data grid 
    //
    var cols = [];
    var initData = [];

    var totCols = 10, totRows = 60;
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
            var start = page <= 1 ? 0 : (page -1 ) * mdata.pageSize;
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

});   