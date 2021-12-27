
var persistenceService = function (serviceParams) {
    var url = serviceParams.url;
    var formId = serviceParams.formId;
    var msgContainer = serviceParams.msgContainer;
    var getIdCallback = serviceParams.getIdCallback;
    var nextEndpoint = serviceParams.nextEndpoint;
    var prevEndpoint = serviceParams.prevEndpoint;
    var onAdd = serviceParams.onAdd;
    var lastEndpoint = serviceParams.lastEndpoint;
    var firstEndpoint = serviceParams.firstEndpoint;
    var deleteEndpoint = serviceParams.deleteEndpoint;

    var onGetResponse = serviceParams.onGetResponse;
    var onDeleteResponse = serviceParams.onDeleteResponse;
    var onSaveResponse = serviceParams.onSaveResponse;
    var idField = serviceParams.idField;

    var recordExist = true;

    var registerHandlers = function () {

        $("#" + formId + " input,textarea,select").each(function () {
            $(this).on('change keydown', function () {
                //
                // element changed
                //
                //console.log('form element is updated', this);
                enableSaveButton(true);
            });
        });

        $('#' + idField).on('change', function (e) {
            //
            // check if record exists from before
            //
            var recordId = $(this).val();
            if (!recordId || recordId === '') return;

            console.log('getting record by ID');
            var getParams = {
                callback: function (response) {
                    //console.log(response);
                    onGetResponse(response);
                    recordExist = true;
                },
                errorCallback: function (error) {
                    console.error(error);
                    resetOnNotFound(error, recordId);
                    if (error.status === 404) {
                        recordExist = false;
                    }
                },
                url: url + recordId,
                action: appDataEvents.ON_FETCH_RECORD,
            };
            getRequest(getParams);
        });

        $('#btnSave').on('click', function (e) {

            //
            // collect data from all the grids which has pending changes
            //

            if (window.gridCallbacks && window.gridCallbacks.length > 0) {

                $.each(window.gridCallbacks, function (index, value) {
                    if (value.eventType === appDataEvents.GRID_DATA) {
                        var girdData = value.callback();
                        console.log("KEY: ", value.key, "Values: ", girdData);
                    }

                });
            }

            saveRecord();
        });

        $("#btnNext").on('click', function (e) {
            if (!getIdCallback()) {
                moveFirst();
                return;
            }
            moveNext();
        });

        $("#btnPrev").on('click', function (e) {
            if (!getIdCallback()) {
                moveLast();
                return;
            }
            movePrev();
        });

        $('#btnFirst').on('click', function (e) {
            moveFirst();
        });

        $('#btnLast').on('click', function (e) {
            moveLast();
        });

        $("#btnAdd").on('click', function (e) {
            onAdd();
            notfiyListeners(appDataEvents.ON_ADD_RECORD, []);
        });

        $("#btnDelete").on('click', function (e) {
            if (!getIdCallback()) return;
            deleteItem();
        });

        //
        // save record
        //
        var saveRecord = function () {
            console.log('saving changes ...');
            var postParams = {
                callback: function (response) {
                    $('#' + msgContainer).attr('class', 'alert alert-success').text("Record is successfully saved").show(0).delay(8000).hide(0);
                    onSaveResponse(response);
                    enableSaveButton(false);
                },
                errorCallback: function (error) {
                    $('#' + msgContainer).attr('class', 'alert alert-danger').text("An error has occurred while saving the changes. Error: " + error.responseText).show(0).delay(8000).hide(0);
                },
                data: $('#' + formId).serializeObject(),
                url: url,
            };
            // console.log(postParams);

            if (recordExist === true)
                putRequest(postParams)
            else
                postRequest(postParams);
        };

        //
        // move to next record
        //
        var moveNext = function () {
            console.log('moving to next record');
            var getParams = {
                callback: function (response) {
                    //console.log(response);
                    onGetResponse(response);
                },
                errorCallback: function (error) {
                    //console.error(error);
                    resetOnNotFound(error);
                },
                url: url + nextEndpoint(),
                action: appDataEvents.ON_NEXT_RECORD,
            };



            getRequest(getParams);
        };

        //
        // move to prev record
        //
        var movePrev = function () {
            console.log('moving to prev record');
            var getParams = {
                callback: function (response) {
                    //console.log(response);
                    onGetResponse(response);
                },
                errorCallback: function (error) {
                    //console.error(error);
                    resetOnNotFound(error);
                },
                url: url + prevEndpoint(),
                action: appDataEvents.ON_PREV_RECORD,
            };
            getRequest(getParams);
        };

        //
        // move to first
        //
        var moveFirst = function () {
            console.log('moving to First record');
            var getParams = {
                callback: function (response) {
                    //console.log(response);
                    onGetResponse(response);

                },
                errorCallback: function (error) {
                    //console.error(error);
                },
                url: url + firstEndpoint,
                action: appDataEvents.ON_FIRST_RECORD,
            };
            getRequest(getParams);
        };

        //
        // move to last
        //
        var moveLast = function () {
            console.log('moving to last record');
            var getParams = {
                callback: function (response) {
                    //console.log(response);
                    onGetResponse(response);
                },
                errorCallback: function (error) {
                    //console.error(error);
                },
                url: url + lastEndpoint,
                action: appDataEvents.ON_LAST_RECORD,
            };
            getRequest(getParams);
        };

        //
        // delete record
        //
        var deleteItem = function () {
            console.log('deleting record');
            var deleteParams = {
                callback: function (response) {

                    //console.log('Record is deleted. Service response: ', response);
                    $('#' + msgContainer).attr('class', 'alert alert-success')
                        .text("Record is successfully deleted").show(0).delay(8000).hide(0);

                    //console.log(response);
                    onDeleteResponse(response);
                    moveNext();
                },
                errorCallback: function (error) {
                    $('#' + msgContainer).attr('class', 'alert alert-danger').text("An error has occurred while deleting the record. Error: " + error.responseText).show(0).delay(8000).hide(0);

                },
                url: url + deleteEndpoint()
            };
            deleteRequest(deleteParams);
        };


        var resetOnNotFound = function (error, recordId) {
            console.log('error-status', error.status);
            if (error.status === 404) {
                onAdd(recordId);
                //
                // notify listeners
                //
                notfiyListeners(appDataEvents.ON_ADD_RECORD, []);

            }
        };

        var enableSaveButton = function (enable) {
            var saveBtn = $('#btnSave');
            if (enable === true)
                saveBtn.removeAttr('disabled');
            else
                saveBtn.attr('disabled', 'disabled');
        };

    };

    var dumpParams = function () {
        //console.log(serviceParams);
    };

    var postRequest = function (postParams) {
        var ajaxOptions = {
            url: postParams.url,
            method: 'POST',
            data: postParams.data ? JSON.stringify(postParams.data) : {},
            contentType: 'application/json',
            headers: postParams.headers ? postParams.headers : {}
        };

        $.ajax(ajaxOptions).then(function done(response) {
            postParams.callback(response);
        }, function error(error) {
            postParams.errorCallback(error);
        });
    };

    var putRequest = function (postParams) {
        var ajaxOptions = {
            url: postParams.url,
            method: 'PUT',
            data: postParams.data ? JSON.stringify(postParams.data) : {},
            contentType: 'application/json',
            headers: postParams.headers ? postParams.headers : {}
        };

        $.ajax(ajaxOptions).then(function done(response) {
            postParams.callback(response);
        }, function error(error) {
            postParams.errorCallback(error);
        });
    };

    
    var notfiyListeners = function (eventType, payload) {
        try {
            if (!window.gridCallbacks || window.gridCallbacks.length === 0) {
                return;
            }

            //
            // search if callback exist from before
            //                
            var releventEvents = window.gridCallbacks.filter(function (value, index) {
                if (value.eventType === eventType)
                    return value;
            });

            if (releventEvents && releventEvents.length > 0) {
                $.each(releventEvents, function (index, ev) {
                    ev.callback(payload);
                });
            }

        } catch (error) {
            console.error(error);
        }
    };


    var getRequest = function (getParams) {
        var ajaxOptions = {
            url: getParams.url,
            method: 'GET',
            headers: getParams.headers ? getParams.headers : {}
        };
        var _notfiyListeners = notfiyListeners;
        $.ajax(ajaxOptions).then(function done(response) {
            getParams.callback(response);
            //
            // notify listeners
            //            
            if (getParams.action) {
                _notfiyListeners(getParams.action, [{
                    refNbr: 120, // sending some fake data
                    isDefault: true,
                    street: "Navigation service",
                    postalCode: "1111",
                    city: "Navland",
                    country: "NAV"
                }]);
            }

        }, function error(error) {
            getParams.errorCallback(error);
        });

    };

    var deleteRequest = function (deleteParams) {
        var ajaxOptions = {
            url: deleteParams.url,
            method: 'DELETE',
            headers: deleteParams.headers ? deleteParams.headers : {}
        };

        $.ajax(ajaxOptions).then(function done(response) {
            deleteParams.callback(response);
        }, function error(error) {
            deleteParams.errorCallback(error);
        });
    };

    return {
        registerHandlers,
        dumpParams,
        postRequest,
        getRequest
    };

};
