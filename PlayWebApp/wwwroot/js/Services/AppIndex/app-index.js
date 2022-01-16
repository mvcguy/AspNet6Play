$(function () {

    console.log('hello from index');
    //
    // sample using bootstrap data grid 
    //
    var cols = [];
    var initData = [];

    var totCols = 10, totRows = 6;
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
        initData, true, 'https://localhost:7096/api/v1/fake/data');

    var bs = new BSGridOptions("fakeData_table", "dummy-data-container", cols, dataSource);

    var grid = new BootstrapDataGrid(bs);
    grid.registerCallbacks();
    grid.render();

});   