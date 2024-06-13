var Sedes = function () {
    "use strict"; return {
        setSedes: function () {
            var metodoControlle = "PagosPendientes2";
            var filter = $('#Sedes').val();

            $.blockUI({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src=""/><br><h3> Cambiando de campus...</h3>'
            });

            $.ajax({
                type: "POST",
                cache: false,
                url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
                data: "filter_Sede=" + filter,
                success: function (data) {
                    $.unblockUI();
                    $("#pagoPendientes").html(data.pagosPendientes);
                }, error: function (msg) {
                }
            });
        }
    }
}();