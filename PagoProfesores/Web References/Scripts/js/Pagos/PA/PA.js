$(function() {
    $(window).load(function () {
        formPage.ConsultaNiveles();
    });
});


var formPage = function () {
    "use strict"; return {

        Clean: function () {
            $("").val("");
        },

        ConsultaNiveles: function () {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/PA/ConsultaNiveles/",
                success: function (msg) {
                    if (session_error(msg) == false) {
                        $("#nivel").html(msg);
                    }
                },
                error: function (msg) {
                    session_error(msg);
                }
            });
        },
    }
}();