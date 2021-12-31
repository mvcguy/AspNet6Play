var dataEventsService = (function () {
    if (!window.playAppCallbacks || window.playAppCallbacks.length === 0) {
        window.playAppCallbacks = [];
    }

    return {
        callbacks: window.playAppCallbacks
    };
})();

dataEventsService.notifyListeners = function (eventType, eventArgs) {
    try {
        $.each(this.callbacks, function () {
            if (this.eventType !== eventType) return;
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
    var index = this.callbacks
        .findIndex(({ key, eventType, dataSourceName }) => key === keyX
            && eventType === eventTypeX
            && dataSourceName === dataSourceNameX);
    // console.log('index: ', index);
    if (index === -1) {
        this.callbacks.push({
            key: keyX,
            eventType: eventTypeX,
            callback: callback,
            dataSourceName: dataSourceNameX
        });
    }
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