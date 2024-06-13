
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




    $('#formbtn_desbloqComplemento').hide();


    DataTable.setShow();
    DataTable.init();
    
    $(window).load(function () {

       // formValidation.Inputs(["fechai", "fechaf"]);
     
    });
});//End function jquery

var formPage = function () {
    var idEstadoCuenta;
    "use strict"; return {

        clean: function () {

          // formValidation.Clean();
            consultar = false;

            $("#fechai").val('');
            $("#fechaf").val('');        
            $("#idECP").val("");

            $('#formbtn_desbloqComplemento').hide();
       
            DataTable.setShow();
          //  DataTable.init();
        },  
      

        Concultar: function (id) {
            
           /* if (!formValidation.Validate())
                return;*/
                
       
            consultar = true;
            DataTable.consultar = true;
            //DataTable.setShow();
            DataTable.init();
        },

        desbloqComplemento: function (id) {


            var model = {
              //  IdEdoCta: $("#idECP").val(),
                search: $("#DataTable-searchtable").val(),
                sede: $('#Sedes').val(),
                fechai: $("#fechai").val(),
                fechaf: $("#fechaf").val()
            }

           /* if (!formValidation.Validate())
                return;*/

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/DesbloqueoComplemento/desbloquearComplemento_all/",
                data: model,
                success: function (data) {

                    $('html, body').animate({ scrollTop: 0 }, 'fast');
                  
                    $('#notification').html(data.msg);
                    DataTable.setShow();
                    //DataTable.init();
                }
            });



        },

       
        

    }
}();

Sedes.setSedes_success = function () {
    if (!formValidation.Validate())
        return;

    consultar = true;
    DataTable.init();
}

var DataTable = function () {
    var pag = 1;
    var order = "IDSIU";

    var sortoption = {
        ASC: "ASC",
        DESC: "DESC"
    };

    var sort = sortoption.ASC;

    "use strict"; return {
        myName: 'DataTable',

        onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
                pag = 1;
                this.init();
            }
        },

        exportExcel: function (table) {
            var datos = 'fechai=' + $('#fechai').val()
              + '&fechaf=' + $('#fechaf').val()
              + '&sedes=' + $('#Sedes').val();

            window.location.href = '/DesbloqueoComplemento/ExportExcel?' + datos;
        },

        edit: function (id) {
            formPage.edit(id);
        },

        setPage: function (page) {
            pag = page;
            this.init();
        },

        setShow: function (page) {
            pag = 1;
            this.init();
        },

        Orderby: function (campo) {
            order = campo;
            var sortcampo = $('#' + this.myName + '-SORT-' + campo).data("sort");
            if (sortcampo == sortoption.ASC) { sort = sortoption.DESC; } else { sort = sortoption.ASC; }
            this.init();
        },

        init: function () {
            var show = $('#' + this.myName + '-data-elements-length').val();
            var search = $('#' + this.myName + '-searchtable').val();
            var orderby = order;
            var sorter = sort;

            var filter = "";
            if (consultar) { filter = $('#Sedes').val(); }
           

            var datos = '&fechai=' + $('#fechai').val()
                + '&fechaf=' + $('#fechaf').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/DesbloqueoComplemento/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter,
                success: function (msg) {

                   // alert(msg);

                 if( msg.indexOf("No existen registros") > -1)
                     $('#formbtn_desbloqComplemento').hide();
                 else
                     $('#formbtn_desbloqComplemento').show();

                    $('.loader-min-bar').hide();
                    $("#datatable").html(msg);

                  
                }
            });
        }
    }
}();