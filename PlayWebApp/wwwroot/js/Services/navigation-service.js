
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
        if (!window.headerCallbacks || window.headerCallbacks.length === 0) {
            window.headerCallbacks = [];
        }

        //
        // search if callback exist from before
        //
        var index = window.headerCallbacks.findIndex(({ key, eventType, dataSourceName }) => key === formId && eventType === eventTypeX && dataSourceName === dataSourceNameX);
        // console.log('index: ', index);
        if (index === -1) {
            window.headerCallbacks.push({ key: key, eventType: eventTypeX, callback: callback, dataSourceName: dataSourceNameX });
        }
    };

    var onGridUpdated = function (event) {
        console.log('Grid-Event: ', event);
        enableSaveButton(true);
    };

    var registerCallbacks = function () {
        registerCallback(formId, appDataEvents.ON_GRID_UPDATED, onGridUpdated, 'mainForm');
    };

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
                notfiyListeners(appDataEvents.ON_ADD_RECORD, []);

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
        var _notfiyListeners = notfiyListeners;
        var ajaxOptions = {
            url: postParams.url,
            method: 'POST',
            data: postParams.data ? JSON.stringify(postParams.data) : {},
            contentType: 'application/json',
            headers: postParams.headers ? postParams.headers : {}
        };

        $.ajax(ajaxOptions).then(function done(response) {
            postParams.callback(response);
            _notfiyListeners(appDataEvents.ON_SAVE_RECORD, []);
        }, function error(error, dt) {
            postParams.errorCallback(error);
            _notfiyListeners(appDataEvents.ON_SAVE_ERROR, error.responseJSON, true);
        });
    };

    var addGridsDataToRequest = function (postParams) {
        //
        // collect data from all the grids which has pending changes
        //
        if (!postParams.data) return;

        var grids = [];
        if (window.gridCallbacks && window.gridCallbacks.length > 0) {

            $.each(window.gridCallbacks, function (index, value) {
                if (value.eventType === appDataEvents.GRID_DATA) {
                    var gridData = value.callback();
                    var dataSourceName = value.dataSourceName;
                    grids.push({ gridData, dataSourceName });
                    console.log("grids: ", grids);
                }

            });
        }

        $.each(grids, function (gridIndex, grid) {

            var records = grid.gridData.filter(function (value, index) {
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

    var putRequest = function (postParams) {

        addGridsDataToRequest(postParams);
        var _notfiyListeners = notfiyListeners;
        var ajaxOptions = {
            url: postParams.url,
            method: 'PUT',
            data: postParams.data ? JSON.stringify(postParams.data) : {},
            contentType: 'application/json',
            headers: postParams.headers ? postParams.headers : {}
        };

        $.ajax(ajaxOptions).then(function done(response) {
            postParams.callback(response);
            _notfiyListeners(appDataEvents.ON_SAVE_RECORD, []);
        }, function error(error, dt) {
            postParams.errorCallback(error);
            _notfiyListeners(appDataEvents.ON_SAVE_ERROR, error.responseJSON, true);
        });
    };


    var notfiyListeners = function (eventType, payload, isError) {
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
                    if (isError) {
                        ev.callback(payload);
                    }
                    else {
                        ev.callback(payload[ev.dataSourceName]);
                    }
                    
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
                _notfiyListeners(getParams.action, response);
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
        var _notfiyListeners = notfiyListeners;
        $.ajax(ajaxOptions).then(function done(response) {
            deleteParams.callback(response);
            _notfiyListeners(appDataEvents.ON_DELETE_RECORD, []);
        }, function error(error, dt) {
            deleteParams.errorCallback(error);
            _notfiyListeners(appDataEvents.ON_SAVE_ERROR, error.responseJSON, true);
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
