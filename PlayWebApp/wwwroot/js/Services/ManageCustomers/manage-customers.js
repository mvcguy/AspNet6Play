//
// Configure customer addresses grid
//

var InitShippingAddressesGrid = function (initData) {
    var colFor = function (name, dataType, width, propName, key, dataSource) {
        return { name, dataType, width, propName, keyColumn: key, dataSource };
    };

    // >> cols
    var cols = [];
    cols.push(colFor("Reference", "text", "80px", "refNbr", true));
    cols.push(colFor("Default", "checkbox", "60px", "isDefault", false));
    cols.push(colFor("Street", "text", "220px", "streetAddress", false));
    cols.push(colFor("Postal code", "number", "80px", "postalCode", false));
    cols.push(colFor("City", "text", "120px", "city", false));
    cols.push(colFor("Country", "select", "120px", "country", false,
        [{ text: 'Pakistan', value: 'PK' }, { text: 'Norway', value: 'NO' }, { text: 'Sweden', value: 'SE' }]));
    // << cols

    // >> datasource

    //var initData = @Json.Serialize(Model.CustomerVm.Addresses);

    var dataSource = {
        isRemote: true,
        url: 'https://localhost:7096/api/v1/Customers/address',
        data: initData,
        dataSourceName: 'addresses'
    };

    // << datasource

    var addressesGrid = new gridNavigationService({
        gridId: "customerAddresses",
        cols,
        dataSource
    });

    addressesGrid.bind();

    $('#btnSaveGrid').on('click', function () {
        var rows = addressesGrid.getDrityRows();
        console.log(rows);
    });

    $('#btnAddRow').on('click', function () {
        addressesGrid.addNewRowToGrid();
    });

    $('#btnDeleteRow').on('click', function () {
        addressesGrid.deleteRow();
    });

    addressesGrid.registerCallbacks();

    const bs = new BSGridOptions("gridId");
    

}



//
// page header
//


$(document).ready(function () {
    var keyCol = 'CustomerVm_RefNbr';

    var srv = new persistenceService({
        url: "https://localhost:7096/api/v1/Customers/",
        formId: "frmCustomer",
        idField: keyCol,
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
            $('#CustomerVm_Name').val('');

            if (id && id !== '')
                $('#' + keyCol).val(id);

        },
        lastEndpoint: "last",
        firstEndpoint: "first",
        onGetResponse: function (response) {
            $('#' + keyCol).val(response.refNbr);
            $('#CustomerVm_Name').val(response.name);
        },
        deleteEndpoint: function () {
            return $('#' + keyCol).val();
        },
        onDeleteResponse: function (response) {

        },
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
});
