
var consultar = false;

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
        formValidation.Inputs(["fechai", "fechaf"]);


     
    });
});//End function jquery

var formPage = function () {
    var idEstadoCuenta;
    "use strict"; return {

        clean: function () {
            formValidation.Clean();
            consultar = false;

            $("#fechai").val('');
            $("#fechaf").val('');       

        
        },

      

        exportExcel: function (id) {

            if (!formValidation.Validate())
                return;


            var data = 'fechai=' + $('#fechai').val()
            + '&fechaf=' + $('#fechaf').val()
            + '&tipodocente=' + $('#tipodocente').val()
            + '&pagosdeposito=' + $('#pagosdeposito').val()
            + '&filtro_=' + $('#filtro_').val()
            + '&sedes=' + $('#Sedes').val();
            
          
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

