
var ciclo1 = new Ciclos('ciclos');
var periodo1 = new Periodos('periodos');



$(function () {   

    $(window).load(function () {
        ciclo1.init("ciclo1");     


    });
});//End function jquery



//cambios Edgar
$('#periodos').on('change', function () {

    ConsultarD();

});



function handlerdataCiclos() {

    periodo1.id_ciclo = $("#ciclos").val();
    periodo1.init("periodo1");

}

function handlerdataPeriodos() {
}


function ConsultarD() {



    var metodoControlle = "_PagosDepositados";

    var data =
     'filter_Sede=' + $('#Sedes').val() 
   + '&periodos=' + $('#periodos').val();

    $.ajax({
        type: "POST",
        cache: false,
        url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
        data: data,
        success: function (data) {

            $("#pagosDepositados").html(data.pagosDepositados);


        }, error: function (msg) {

        }
    });
}

var Sedes = function () {
    "use strict"; return {
        setSedes: function () {
            var metodoControlle = "_PagosDepositados";
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
                    $("#pagosDepositados").html(data.pagosDepositados);
                }, error: function (msg) {
                }
            });
        }
    }
}();