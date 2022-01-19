
class ManageCustomers {
    constructor() {

    }

    static initCustomersPage(initData) {
        var keyCol = 'CustomerVm_RefNbr';
        // @ts-ignore
        var srv = new persistenceService({
            url: "https://localhost:7096/api/v1/Customers/",
            formId: "frmCustomer",
            idField: keyCol,
            msgContainer: "serverMessages",
            responseKeyCol: 'refNbr',
            getIdCallback: function () {
                // @ts-ignore
                return $('#' + keyCol).val();
            },
            nextEndpoint: function () {
                // @ts-ignore
                return $('#' + keyCol).val() + "/next";
            },
            prevEndpoint: function () {
                // @ts-ignore
                return $('#' + keyCol).val() + "/previous";
            },
            onAdd: function (id) {
                // @ts-ignore
                $('#' + keyCol).val('');
                // @ts-ignore
                $('#CustomerVm_Name').val('');

                if (id && id !== '')
                    // @ts-ignore
                    $('#' + keyCol).val(id);

            },
            lastEndpoint: "last",
            firstEndpoint: "first",
            onGetResponse: function (response) {
                // @ts-ignore
                $('#' + keyCol).val(response.refNbr);
                // @ts-ignore
                $('#CustomerVm_Name').val(response.name);
            },
            deleteEndpoint: function () {
                // @ts-ignore
                return $('#' + keyCol).val();
            },
            // @ts-ignore
            onDeleteResponse: function (response) {

            },
            // @ts-ignore
            onSaveResponse: function (response) {

            },

            // important for updating the url and appending the current record query-string
            // useful for bookmarking the record
            urlQuery: function (response) {
                return '?refNbr=' + response.refNbr;
            },
        });
        srv.registerHandlers();
        srv.registerCallbacks();

        //
        // shipping addresses grid 
        //
        var cols = [];
        cols.push(new BSGridColDefinition("Reference", "text", "80px", "refNbr", true));
        cols.push(new BSGridColDefinition("Default", "checkbox", "60px", "isDefault", false));
        cols.push(new BSGridColDefinition("Street", "text", "220px", "streetAddress", false));
        cols.push(new BSGridColDefinition("Postal code", "number", "80px", "postalCode", false));
        cols.push(new BSGridColDefinition("City", "text", "120px", "city", false));
        cols.push(new BSGridColDefinition("Country", "select", "120px", "country", false,
            [new BSGridSelectListItem('Pakistan', 'PK'),
            new BSGridSelectListItem('Norway', 'NO'),
            new BSGridSelectListItem('Sweden', 'SE')]));

        var dataSource = new BSGridDataSource('addresses',
            initData, true, 'https://localhost:7096/api/v1/customers/address/{0}/{1}');

        var bs = new BSGridOptions("customerAddresses", "customerAddresses_Container", cols, dataSource);

        var grid = new BootstrapDataGrid(bs);        
        grid.registerCallbacks();
        grid.render();


    }
}

