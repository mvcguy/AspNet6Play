$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};


/**
 * Credit: https://stackoverflow.com/a/49041392
 * @param {*} th - the header object th to sort
 * @param {boolean} ascX - sort ascending or vice versa
 */

$.fn.sortTable = function (th, ascX) {

    //  console.log('sorting', ascX);
    const getCellValue = (tr, idx) => {
        var child = $($(tr).children()[idx]);
        // console.log('idx: ', idx,  child);
        var text = child.find('input, select').is(":checked") || child.find('input, select').val() || child.text();
        //console.log(child.find('input, select').is(":checked"));
        return text;
    };


    // Returns a function responsible for sorting a specific column index 
    // (idx = columnIndex, asc = ascending order?).
    var comparer = function (idx, asc) {
         //console.log('idx: ', idx, 'asc: ', asc);
        // This is used by the array.sort() function...
        return function (a, b) {
             //console.log('a: ', a, 'b: ', b);

            // This is a transient function, that is called straight away. 
            // It allows passing in different order of args, based on 
            // the ascending/descending order.
            return function (v1, v2) {
                //  console.log('v1: ', v1, 'v2: ', v2);
                // sort based on a numeric or localeCompare, based on type...
                return (v1 !== '' && v2 !== '' && !isNaN(v1) && !isNaN(v2))
                    ? v1 - v2
                    : v1.toString().localeCompare(v2);
            }(getCellValue(asc ? a : b, idx), getCellValue(asc ? b : a, idx));
        }
    };

    // do the work...
    // const table = th.closest('table');
    var rows = this.find('tbody>tr:visible');
    const tbody = this.find('tbody');
    // console.log(rows);
    Array.from(rows)
        .sort(comparer(Array.from(th.parentNode.children).indexOf(th), ascX = !ascX))
        .forEach(tr => $(tbody).append(tr));

};