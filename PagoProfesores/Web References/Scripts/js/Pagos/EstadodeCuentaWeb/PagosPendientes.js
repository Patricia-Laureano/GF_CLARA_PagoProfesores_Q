var Sedes = function () {

    "use strict"; return {

        setSedes: function () {


            var metodoControlle = "PagosPendientes2";

            var filter = $('#Sedes').val();


            $.ajax({
                type: "POST",
                cache: false,
                url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
                data: "filter_Sede=" + filter,
                success: function (data) {


                    $("#pagoPendientes").html(data.pagosPendientes);


                }, error: function (msg) {



                }
            });


        }
    }
}();