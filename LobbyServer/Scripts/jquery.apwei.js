(function () {

    // Apwei jQuery Extension

    // Enables
    $.fn.enable = function () {
        var s = this;
        s.removeAttr('disabled');
        return s;
    }

    // Disables
    $.fn.disable = function () {
        var s = this;
        s.attr('disabled', 'disabled');
        return s;
    }

    // Disables for a period of time.
    $.fn.disableFor = function (milliseconds) {
        var s = this;
        s.disable();
        setTimeout(function () {
            s.enable();
        }, milliseconds);
        return s;
    }
})();