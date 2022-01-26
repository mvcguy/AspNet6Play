class ManageBooking {

    constructor() {

    }

    static initBookingPage(booking) {

        var bookingLines = booking.lines;
        var bookingLinesMetadata = booking.linesMetaData;

        //
        // header fields
        //
        var ref = new BSGridTextInputExt({ elementId: 'BookingVm_RefNbr' });
        var des = new BSGridTextInputExt({ elementId: 'BookingVm_Description' });
        var cus = new BSGridTextInputExt({ elementId: 'BookingVm_CustomerRefNbr' });


        //
        // summary fields
        //
        var txtLinesTotal = new BSGridTextInputExt({ elementId: 'txtLinesTotal' });
        var txtAmount = new BSGridTextInputExt({ elementId: 'txtAmount' });
        var txtTaxable = new BSGridTextInputExt({ elementId: 'txtTaxable' });
        var txtTax = new BSGridTextInputExt({ elementId: 'txtTax' });
        var txtDiscount = new BSGridTextInputExt({ elementId: 'txtDiscount' });
        var txtBalance = new BSGridTextInputExt({ elementId: 'txtBalance' });

        function updateSummary(summary) {
            txtLinesTotal.val = summary.linesTotal;
            txtDiscount.val = summary.discount;
            txtTaxable.val = summary.taxableAmount;
            txtTax.val = summary.taxAmount;
            txtAmount.val = summary.totalAmount;
            txtBalance.val = summary.balance;
        }

        updateSummary(booking);
        var srv = new persistenceService({
            url: "https://localhost:7096/api/v1/Bookings/",
            formId: "frmBooking",
            idField: 'BookingVm_RefNbr',
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
                des.val = '';
                cus.valExt = '';

                if (id && id !== '')
                    ref.val = id;

                updateSummary({});
            },
            lastEndpoint: "last",
            firstEndpoint: "first",
            onGetResponse: function (response) {
                ref.val = response.refNbr;
                des.val = response.description;
                cus.valExt = response.customerRefNbr;

                updateSummary(response);
            },
            deleteEndpoint: function () {
                return ref.val;
            },
            onDeleteResponse: function (response) {

            },
            onSaveResponse: function (response) {
                updateSummary(response);
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



        /**
         * 
         * @param {BootstrapDataGrid} sender 
         */
        var calcSummary = function (sender) {
            var rows = sender.body.rows;
            txtLinesTotal.val = 0.0;
            txtAmount.val = 0.0;
            txtTaxable.val = 0.0;
            txtTax.val = 0.0;
            txtDiscount.val = 0.0;
            txtBalance.val = 0.0;
            var lt = 0.0;
            var templateRow = sender.body.getTemplateRow();
            rows.forEach((r, i) => {

                if (r === templateRow) return;
                var record = r.getRowDataExt();
                if (!record.extCost.val) return;
                lt += parseFloat(record.extCost.val);
            });

            var disc = lt * 0.30;

            var tx = (lt - disc) * 0.025;


            txtLinesTotal.val = lt;
            txtDiscount.val = disc;
            txtTaxable.val = lt - disc;
            txtTax.val = tx;
            txtAmount.val = (lt - disc) + tx;
            txtBalance.val = txtAmount.val;
        };

        grid.addHandler(appDataEvents.ON_GRID_DATA_BOUND, (sender, e) => {
            //calcSummary(sender);
        });

        grid.addHandler(appDataEvents.ON_FIELD_UPDATED, (sender, e) => {

            var fieldName = e.eventData.field.getFieldName();
            var row = e.eventData.row;
            // console.log('on-field-update', fieldName, row);
            if (fieldName === 'quantity' || fieldName === 'unitCost') {
                row.extCost.val = row.quantity.val * row.unitCost.val;

                //calcSummary(sender);
            }
        });

        grid.render();

    }
}