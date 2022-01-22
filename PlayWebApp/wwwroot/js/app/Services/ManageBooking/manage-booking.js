class ManageBooking {

    constructor() {
        
    }

    static initBookingPage(bookingLines, bookingLinesMetadata) {
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

    }
}