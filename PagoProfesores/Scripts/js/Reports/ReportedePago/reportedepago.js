
var consultar = false;

var add_EC_anio1 = new Ciclos('add_EC_anio');
var add_EC_periodo1 = new Periodos('add_EC_periodo');
var add_EC_esquema1 = new Esquema('add_EC_esquema');

$(function () {
    $("#fechai").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });

    $("#fechaf").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });

   

    $(window).load(function () {

        $('#img_generando').hide();
        initProgressBar();

        formValidation.Inputs(["fechai", "fechaf", "add_EC_esquema"]);

        add_EC_anio1.init("add_EC_anio1");  
     
    });


   


});//End function jquery


function handlerdataCiclos() {
}

function handlerdataPeriodos() {
}


$('#add_EC_anio').on('change', function () {
    $('#add_EC_esquema').html("<option><option>");
    //$('#add_EC_concepto').html("<option><option>");
   // $('#add_EC_fechaPago').html("<option><option>");

    if (this.value == "") {//ciclo(anio) 
        $('#add_EC_periodo').html("<option><option>");
    } else {
        add_EC_periodo1.id_ciclo = this.value;
        add_EC_periodo1.init("add_EC_periodo1");
    }
});

$('#add_EC_periodo').on('change', function () {
   // $('#add_EC_concepto').html("<option><option>");
   // $('#add_EC_fechaPago').html("<option><option>");

    if (this.value == "") {//periodo
        $('#add_EC_esquema').html("<option><option>");
    } else {
        add_EC_esquema1.Sede = $('#Sedes').val();
        add_EC_esquema1.Periodo = this.value;
       // add_EC_esquema1.Nivel = $('#add_EC_nivel').val();
        add_EC_esquema1.init();
    }
});



var formPage = function () {
    var idEstadoCuenta;
    "use strict"; return {

        clean: function () {
            formValidation.Clean();
            consultar = false;

            $("#fechai").val('');
            $("#fechaf").val('');

            $("#add_EC_anio").val('');
            $("#add_EC_periodo").val('');
            $("#add_EC_esquema").val('');


            $('#img_generando').hide();
            initProgressBar();

        
        },

      

        exportExcel: function (id) {

            if (!formValidation.Validate())
                return;


            var data = 'fechai=' + $('#fechai').val()
            + '&fechaf=' + $('#fechaf').val()
            + '&tipodocente=' + $('#tipodocente').val()
            + '&pagosdeposito=' + $('#pagosdeposito').val()
            + '&filtro_=' + $('#filtro_').val()
            + '&sedes=' + $('#Sedes').val()           
           + '&idesquema=' + $("#add_EC_esquema").val();
         
            
            startTimer("ReportedePago", function () {

               // DataTable.setShow();
            });



          
           window.location.href = '/ReportedePago/ExportExcel?' + data;          

           // var url = "/ReportedePago/ExportExcel?" + data + "";

         //   var myWindow = window.open(url, '_blank');
         //   myWindow.opener.document.focus();
           // myWindow.document.write('<p>html to write...</p>');
          
          
        },      
      
    }
}();

Sedes.setSedes_success = function () {
    if (!formValidation.Validate())
        return;

    consultar = true;
   
}

