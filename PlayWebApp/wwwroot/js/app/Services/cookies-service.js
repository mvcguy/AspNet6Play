
/**
 * simple utility to add/remove cookies based on ES7
 * credit: https://stackoverflow.com/a/48706852
 */
const Cookie = {
    get: name => {
        let c = document.cookie.match(`(?:(?:^|.*; *)${name} *= *([^;]*).*$)|^.*$`)[1]
        if (c) return decodeURIComponent(c)
    },
    set: (name, value, opts = {}) => {
        /*If options contains days then we're configuring max-age*/
        if (opts.days) {
            opts['max-age'] = opts.days * 60 * 60 * 24;

            /*Deleting days from options to pass remaining opts to cookie settings*/
            delete opts.days 
        }

        /*Configuring options to cookie standard by reducing each property*/
        opts = Object.entries(opts).reduce(
            (accumulatedStr, [k, v]) => `${accumulatedStr}; ${k}=${v}`, ''
        )

        /*Finally, creating the key*/
        document.cookie = name + '=' + encodeURIComponent(value) + opts
    },
    delete: (name, opts) => Cookie.set(name, '', {'max-age': -1, ...opts}),
    // path & domain must match cookie being deleted 
    getJSON: (name) => {
        var result = Cookie.get(name);
        if (!result) return '';
        return JSON.parse(result);
    },
    setJSON : (name, value, opts) => Cookie.set(name, JSON.stringify(value), opts)
}