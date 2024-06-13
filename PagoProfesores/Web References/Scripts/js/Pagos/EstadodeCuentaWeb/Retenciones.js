function ConsultarD() {



    var metodoControlle = "_Contratos";

    var data =
     'filter_Sede=' + $('#Sedes').val()
   + '&fechaDepoI=' + $('#fechaDepoI').val()
   + '&fechaDepoF=' + $('#fechaDepoF').val();

    $.ajax({
        type: "POST",
        cache: false,
        url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
        data: data,
        success: function (data) {

            $("#Contratos").html(data.Contratos);


        }, error: function (msg) {



        }
    });




}



var Sedes = function () {

    "use strict"; return {

        setSedes: function () {

        
            var metodoControlle = "_Contratos";

            var filter = $('#Sedes').val();

            	$.ajax({
        		type: "POST",
        		cache: false,
        		url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
        		data: "filter_Sede=" + filter,
        		success: function (data) {   


        		    $("#Contratos").html(data.Contratos);

        		
        		}, error: function (msg) {

        		  

        		}
        	});
     

        }
    }
}();