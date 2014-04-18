(function () {

    // Apwei jQuery Extension

    $.fn.disableFor = function (milliseconds) {
        var s = this;
        s.attr('disabled', 'disabled');
        setTimeout(function () {
            s.removeAttr('disabled');
        }, milliseconds);
        return this;
    }
})();