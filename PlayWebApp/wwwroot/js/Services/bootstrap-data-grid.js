// @ts-check

class BootstrapDataGrid {

    /**
     * 
     * @param {object} jquery 
     * @param {BSGridOptions} options 
     */
    constructor(jquery, options) {
        this.jquery = jquery;
        this.options = options;
    }

    get cols() {
        return this.options.colDefinition;
    }

    dumpOptions() {
        console.log(this.options);
    }

}

class BSGridOptions {

    /**
     * 
     * @param {string} gridId 
     * @param {BSGridColDefinition} colDefinition 
     * @param {BSGridDataSource} dataSource 
     */
    constructor(gridId, colDefinition, dataSource) {
        this.gridId = gridId;
        this.colDefinition = colDefinition;
        this.dataSource = dataSource;        
    }
}

class BSGridColDefinition {

    /**
     * @type {string}
     */
    name;
    /**
     * @type {string}
     */
    dataType;

    /**
     * @type {string}
     */
    width;

    /**
     * @type {string}
     */
    propName;

    /**
     * @type {boolean}
     */
    isKey;

    /**
     * @type {BSGridSelectListItem[]}
     */
    dataSource;

}

class BSGridDataSource {

    /**
     * @type {boolean}
     */
    isRemote;

    /**
     * @type {URL}
     */
    url;

    /**
     * @type {object[]}
     */
    initData;

    /**
     * @type {string}
     */
    name;

}

class BSGridSelectListItem {

    /**
     * @type {string}
     */
    text;

    /**
     * @type {string}
     */
    value;

    /**
     * @type {boolean}
     */
    isSelected;
}
