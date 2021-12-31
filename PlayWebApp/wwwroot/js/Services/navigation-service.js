
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

    var registerCallback = function (key, eventTypeX, callback, dataSourceNameX) {
        dataEventsService.registerCallback(key, eventTypeX, callback, dataSourceNameX);
    };

    var onGridUpdated = function (event) {
        console.log('Grid-eventData: ', event);
        enableSaveButton(true);
    };

    var registerCallbacks = function () {
        registerCallback(formId, appDataEvents.ON_GRID_UPDATED, onGridUpdated, 'mainForm');
        registerCallback(formId, appDataEvents.ON_NAVIGATING, onNavigating, 'mainForm');

        //ON_NAVIGATING
    };

    var onNavigating = function (eventArgs) {
        console.log('ON_NAVIGATING: EventData: ', eventArgs);

    }

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
                    // console.error(error);
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
            var result = $("#" + formId).valid();
            if (result === false) return;

            saveRecord();
        });

        $("#btnNext").on('click', function (e) {

            var ev = { eventData: e };
            notifyListeners(appDataEvents.ON_NAVIGATING, ev);
            if (ev.eventData.cancelAction === true) {
                return;
            }

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
            notifyListeners(appDataEvents.ON_ADD_RECORD, { eventData: e });
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
                    //console.error(error);
                    //$('#' + msgContainer).attr('class', 'alert alert-danger').text("An error has occurred while saving the changes. Error: " + error.responseText).show(0).delay(8000).hide(0);
                },
                data: $('#' + formId).serializeObject(),
                url: url,
            };
            // console.log(postParams);

            if (recordExist === true)
                putRequest(postParams);
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
                notifyListeners(appDataEvents.ON_ADD_RECORD, { eventData: { recordId } });

            }
        };
    };

    var enableSaveButton = function (enable) {
        var saveBtn = $('#btnSave');
        if (enable === true)
            saveBtn.removeAttr('disabled');
        else
            saveBtn.attr('disabled', 'disabled');
    };

    var dumpParams = function () {
        //console.log(serviceParams);
    };

    var postRequest = function (postParams) {

        addGridsDataToRequest(postParams);
        var _notifyListeners = notifyListeners;
        var ajaxOptions = {
            url: postParams.url,
            method: 'POST',
            data: postParams.data ? JSON.stringify(postParams.data) : {},
            contentType: 'application/json',
            headers: postParams.headers ? postParams.headers : {}
        };

        $.ajax(ajaxOptions).then(function done(response) {
            postParams.callback(response);
            _notifyListeners(appDataEvents.ON_SAVE_RECORD, { eventData: response });
        }, function error(error, dt) {
            postParams.errorCallback(error);
            _notifyListeners(appDataEvents.ON_SAVE_ERROR, { eventData: error }, true);
        });
    };

    var putRequest = function (postParams) {

        addGridsDataToRequest(postParams);
        var _notifyListeners = notifyListeners;
        var ajaxOptions = {
            url: postParams.url,
            method: 'PUT',
            data: postParams.data ? JSON.stringify(postParams.data) : {},
            contentType: 'application/json',
            headers: postParams.headers ? postParams.headers : {}
        };

        $.ajax(ajaxOptions).then(function done(response) {
            postParams.callback(response);
            _notifyListeners(appDataEvents.ON_SAVE_RECORD, { eventData: response });
        }, function error(error, dt) {
            postParams.errorCallback(error);
            _notifyListeners(appDataEvents.ON_SAVE_ERROR, { eventData: error }, true);
        });
    };

    var getRequest = function (getParams) {
        var ajaxOptions = {
            url: getParams.url,
            method: 'GET',
            headers: getParams.headers ? getParams.headers : {}
        };
        var _notifyListeners = notifyListeners;
        $.ajax(ajaxOptions).then(function done(response) {

            getParams.callback(response);
            resetFormValidation();

            //
            // notify listeners
            //            
            if (getParams.action) {
                _notifyListeners(getParams.action, { eventData: response });
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
        var _notifyListeners = notifyListeners;
        $.ajax(ajaxOptions).then(function done(response) {
            deleteParams.callback(response);
            _notifyListeners(appDataEvents.ON_DELETE_RECORD, { eventData: response });
        }, function error(error, dt) {
            deleteParams.errorCallback(error);
            _notifyListeners(appDataEvents.ON_SAVE_ERROR, { eventData: error }, true);
        });
    };

    var notifyListeners = function (eventType, eventArgs) {
        dataEventsService.notifyListeners(eventType, eventArgs);
    };

    var resetFormValidation = function () {
        var $form = $('#' + formId);
        $form.valid();
    };

    var addGridsDataToRequest = function (postParams) {
        //
        // collect data from all the grids which has pending changes
        //
        if (!postParams.data) return;

        var grids = dataEventsService.invokeCallback(appDataEvents.GRID_DATA);

        $.each(grids, function (gridIndex, grid) {

            var records = grid.data.filter(function (value, index) {
                if (value.rowCategory === "ADDED") {
                    value["updateType"] = 3;
                }
                else if (value.rowCategory === 'DELETED') {
                    value["updateType"] = 2;
                }
                else if (value.rowCategory === 'UPDATED') {
                    value["updateType"] = 1;
                }


                if (!value.updateType) return undefined;

                return value;
            });

            postParams.data[grid.dataSourceName] = records;
        });
    };

    return {
        registerHandlers,
        registerCallbacks,
        dumpParams,
        postRequest,
        getRequest
    };

};
