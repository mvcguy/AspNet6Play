var dataEventsService = (function () {
    if (!window.playAppCallbacks || window.playAppCallbacks.length === 0) {
        window.playAppCallbacks = [];
    }
    return {
        callbacks: window.playAppCallbacks
    };
})();

dataEventsService.winPopstate = function () {
    var _this = this;
    window.onpopstate = function (e) {
        if (e.state && e.state.eventData) {
            console.log(e.state);
            _this.notifyListeners(appDataEvents.ON_FETCH_RECORD, { eventData: e.state.eventData, skipPush: true });
        }
    };
};

dataEventsService.winPopstate();

dataEventsService.notifyListeners = function (eventType, eventArgs) {
    if (!eventType) return;
    try {
        $.each(this.callbacks, function () {
            if (this.eventType !== eventType || this.dataSourceName !== eventArgs.dataSourceName) return;
            this.callback(eventArgs);
        });

    } catch (error) {
        console.error(error);
    }
};

dataEventsService.registerCallback = function (keyX, eventTypeX, callback, dataSourceNameX) {
    //
    // search if callback exist from before
    //
    if (!eventTypeX) return;
    var index = this.callbacks
        .findIndex(({ key, eventType, dataSourceName }) => key === keyX
            && eventType === eventTypeX
            && dataSourceName === dataSourceNameX);
    //  console.log('index: ', index);
    //if (index === -1) {
    this.callbacks.push({
        key: keyX,
        eventType: eventTypeX,
        callback: callback,
        dataSourceName: dataSourceNameX
    });
    //}
};

dataEventsService.invokeCallback = function (eventType, payload) {
    var resultArray = [];

    $.each(this.callbacks, function () {
        if (this.eventType === eventType) {
            var result = this.callback(payload);
            var dataSourceName = this.dataSourceName;
            resultArray.push({ data: result, dataSourceName: dataSourceName });
            console.log("invokeCallback: Event:", eventType, " payload: ", payload, " Result: ", result);
        }

    });

    return resultArray;
}