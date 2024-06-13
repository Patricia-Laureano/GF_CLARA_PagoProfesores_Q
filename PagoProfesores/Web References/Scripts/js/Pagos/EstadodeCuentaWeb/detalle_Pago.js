

var Sedes = function () {

    "use strict"; return {

        setSedes: function () {

            alert('entro');
            return false;


            var metodoControlle = "DetallePago_";

            var filter = $('#Sedes').val();


            $.ajax({
                type: "POST",
                cache: false,
                url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
                data: "filter_Sede=" + filter,
                success: function (data) {

                    $("#estadodeCuenta").html(data.estadodeCuenta);
                 

                }, error: function (msg) {



                }
            });


        }
    }
}();