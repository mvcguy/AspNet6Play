class ManageBooking {

    constructor() {
        
    }

    static initBookingPage(bookingRefNbr, bookingLines, bookingLinesMetadata) {
        var keyCol = 'BookingVm_RefNbr';
        var srv = new persistenceService({
            url: "https://localhost:7096/api/v1/Bookings/",
            formId: "frmBooking",
            idField: 'BookingVm_RefNbr',
            msgContainer: "serverMessages",
            getIdCallback: function () {
                return $('#' + keyCol).val();
            },
            nextEndpoint: function () {
                return $('#' + keyCol).val() + "/next";
            },
            prevEndpoint: function () {
                return $('#' + keyCol).val() + "/previous";
            },
            onAdd: function (id) {
                $('#' + keyCol).val('');
                $('#BookingVm_Description').val('');

                if (id && id !== '')
                    $('#' + keyCol).val(id);

            },
            lastEndpoint: "last",
            firstEndpoint: "first",
            onGetResponse: function (response) {
                $('#' + keyCol).val(response.refNbr);
                $('#BookingVm_Description').val(response.description);
            },
            deleteEndpoint: function () {
                return $('#' + keyCol).val();
            },
            onDeleteResponse: function (response) {

            },
            onSaveResponse: function (response) {

            },
        });
        srv.registerHandlers();
        srv.registerCallbacks();

        //
        // booking lines grid 
        //
        var cols = [];
        cols.push(new BSGridColDefinition("Line nbr", "number", "80px", "refNbr", true));
        cols.push(new BSGridColDefinition("Stock item", "text", "60px", "stockItemRefNbr", false));
        cols.push(new BSGridColDefinition("Description", "text", "220px", "description", false));
        cols.push(new BSGridColDefinition("Quantity", "number", "80px", "quantity", false));
        cols.push(new BSGridColDefinition("Unit cost", "number", "120px", "unitCost", false));
        cols.push(new BSGridColDefinition("Cost", "number", "120px", "extCost", false));
        cols.push(new BSGridColDefinition("Discount", "number", "120px", "discount", false));


        var dataSource = new BSGridDataSource('lines',
            {
                initData: bookingLines,
                metaData: bookingLinesMetadata
                    ? new PagingMetaData(bookingLinesMetadata.pageIndex, bookingLinesMetadata.pageSize, bookingLinesMetadata.totalRecords)
                    : undefined
            },
            true,
            (page) => {
                var refNbr = srv.getIdCallback();
                if (!refNbr) return undefined;
                return `https://localhost:7096/api/v1/bookings/lines/${refNbr}/${page}`;
            });

        var bs = new BSGridOptions("bookingLines", "bookingLines_Container", cols, dataSource);

        var grid = new BootstrapDataGrid(bs);
        grid.registerCallbacks();
        grid.render();

    }
}