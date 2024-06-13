var Sedes = function () {

    "use strict"; return {

        setSedes: function () {
          

            var metodoControlle = "Perfil2_";

            var filter = $('#Sedes').val();

            $.ajax({
                type: "POST",
                cache: false,
                url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
                data: "filter_Sede=" + filter,
                success: function (data) {


                    $("#Profesor").html(data.Profesor);
                    $("#Profesor2").html(data.Profesor);
                    $("#NoCuenta").html(data.NoCuenta);
                    $("#Banco").html(data.Banco);
                    $("#RFC").html(data.RFC);
                    $("#Direccion").html(data.Direccion);
                    $("#Fis_Sede").html(data.Fis_Sede);
                    $("#Fis_RecibiDe").html(data.Fis_RecibiDe);
                    $("#Fis_RFC").html(data.Fis_RFC);
                    $("#Fis_Domicilio").html(data.Fis_Domicilio);
                    $("#Fis_Concepto").html(data.Fis_Concepto);
                    $("#IDSIU").html(data.IDSIU);
                    $("#pensiones").html(data.pensiones);



                }, error: function (msg) {



                }
            });


        }
    }
}();