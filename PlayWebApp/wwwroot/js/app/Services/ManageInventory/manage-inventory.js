class ManageInventory {

    constructor() {

    }

    static initPage(model) {

        var lines = model.itemPrices;
        var metaData = model.itemPricesMetaData;

        //
        // header
        //
        var ref = new BSGridTextInputExt({ elementId: 'StockItemVm_RefNbr' });
        var desc = new BSGridTextInputExt({ elementId: 'StockItemVm_ItemDescription' });

        var srv = new persistenceService({
            url: "https://localhost:7096/api/v1/StockItems/",
            formId: "frmStockItems",
            idField: 'StockItemVm_RefNbr',
            msgContainer: "serverMessages",
            getIdCallback: function () {
                return ref.val;
            },
            nextEndpoint: function () {
                return ref.val + "/next";
            },
            prevEndpoint: function () {
                return ref.val + "/previous";
            },
            onAdd: function (id) {
                ref.val = '';
                desc.val = '';
                if (id && id !== '')
                    ref.val = id;

            },
            lastEndpoint: "last",
            firstEndpoint: "first",
            onGetResponse: function (response) {
                ref.val = response.refNbr;
                desc.val = response.itemDescription;
            },
            deleteEndpoint: function () {
                return ref.val;
            },
            onDeleteResponse: function (response) {

            },
            onSaveResponse: function (response) {

            },
        });
        srv.registerHandlers();
        srv.registerCallbacks();

        //
        // item prices
        //

        var cols = [];

        cols.push(new BSGridColDefinition("Line nbr", "number", "80px", "refNbr", true));
        cols.push(new BSGridColDefinition("Unit Cost", "number", "220px", "unitCost", false));
        cols.push(new BSGridColDefinition("Break Qty", "number", "80px", "breakQty", false));
        cols.push(new BSGridColDefinition("UOM", "select", "120px", "unitOfMeasure", false,
            [
                new BSGridSelectListItem('Select', ''),
                new BSGridSelectListItem('KG', 'KG'),
                new BSGridSelectListItem('Liter', 'LT')

            ]));
        cols.push(new BSGridColDefinition("Effective From", "date", "120px", "effectiveFrom", false));
        cols.push(new BSGridColDefinition("Expires on", "date", "120px", "expiresAt", false));

        var dataSource = new BSGridDataSource('itemPrices',
            {
                initData: lines,
                metaData: metaData
                    ? new PagingMetaData(metaData.pageIndex, metaData.pageSize, metaData.totalRecords)
                    : undefined
            },
            true,
            (page) => {
                var refNbr = srv.getIdCallback();
                if (!refNbr) return undefined;
                return `https://localhost:7096/api/v1/stockitems/prices/${refNbr}/${page}`;
            });

        var bs = new BSGridOptions("itemPrices", "itemprices_container", cols, dataSource);

        var grid = new BootstrapDataGrid(bs);
        grid.registerCallbacks();
        grid.render();
    }
}