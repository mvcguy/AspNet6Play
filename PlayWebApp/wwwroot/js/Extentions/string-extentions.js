
/**
 * Converts string to camal casing
 * @returns camelCasing of the string
 */
String.prototype.toCamalCase = function () {
    return this.replace(/(?:^\w|[A-Z]|\b\w)/g, function(word, index) {
        return index === 0 ? word.toLowerCase() : word.toUpperCase();
      }).replace(/\s+/g, '');
};

/**
 * Converts string to pascal casing
 * @returns PascalCasing of the string
 */
String.prototype.toPascalCase = function() {
    return this.replace(/(?:^\w|[A-Z]|\b\w)/g, function(word, index) {
        return index === 0 ? word.toUpperCase() : word.toLowerCase();
      }).replace(/\s+/g, '');
};

String.prototype.toPascalCaseJson = function() {
    return this.replace(/(?:^\w|[A-Z]|\b\w)/g, function(word, index) {
        return index === 0 ? word.toUpperCase() : word;
      }).replace(/\s+/g, '');
};