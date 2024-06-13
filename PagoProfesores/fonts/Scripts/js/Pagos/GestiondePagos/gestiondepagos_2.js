
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


    $(document).ready(function () {

        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 3
        });

    });


    $(window).load(function () {

      //  DataTable.init();

      //  formValidation.Inputs(["fechai", "fechaf"]);
      //  formValidation.notEmpty('fechai', 'El campo fecha inicial no debe de estar vacio');
       // formValidation.notEmpty('fechaf', 'El campo fecha final no debe de estar vacio');
       

    });
});//End function jquery


var Sedes = function () {

    "use strict"; return {

        setSedes: function () {
            DataTable.init();
        }
    }
}();




var formPage = function () {

    var idEstadoCuenta;
    "use strict"; return {

        clean: function () {
         //   formValidation.Clean();
            consultar = false;

            DataTable.init();

            $("#fechai").val('');
            $("#fechaf").val('');
            $("#publicar").val('');


            //   DataTable.init();
        }
        ,


        Concultar: function (id) {
            /*
            if (!formValidation.Validate())
                return;
                */
            consultar = true;
            DataTable.consultar = true;
            DataTable.init();

        },

        btnpublicar: function () {

          
            var r = confirm("¿Estas seguro de publicar?");
            if (r == true) {} 
            else {
                return;               
            }  

            $("#publicar").val(1);
            formPage.Publicar();            
              DataTable.init();

        },

        btndespublicar: function () {


            var r = confirm("¿Estas seguro de despublicar?");
            if (r == true) { }
            else {
                return;
            }

            $("#publicar").val(0);
            formPage.Despublicar();
            DataTable.init();

        },


        Publicar: function () {         


            var model = {         
                Publicar: $("#publicar").val(),
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
                url: "/GestiondePagos/Publicar/",
                data: model,
                success: function (data) {

                    // formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();

                }

            });
        },

        Despublicar: function () {         


            var model = {         
                Publicar: $("#publicar").val(),
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
                url: "/GestiondePagos/Despublicar/",
                data: model,
                success: function (data) {

                    // formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();

                }

            });
        }

    }


}();

var Sedes = function () {

    "use strict"; return {

        setSedes: function () {
            DataTable.init();
        }
    }
}();


var DataTable = function () {
    var pag = 1;
    var order = "IDSIU";
    // var consultar;

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
              + '&fechai=' + $('#fechaf').val()
              + '&sedes=' + $('#Sedes').val();

            window.location.href = '/GestiondePagos/ExportExcel?' + datos;

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
            var filter = $('#Sedes').val();
            var publicado = $("#publicar").val();

            var datos = '&fechai=' + $('#fechai').val()
             + '&fechaf=' + $('#fechaf').val();



            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');



            /*$('#datatable').block({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src="/Content/images/load-search.gif" /><br><h2> Buscando..</h2>'
            });*/


            $.ajax({
                type: "GET",
                cache: false,
                url: "/GestiondePagos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter + "&publicado="+ publicado,
                success: function (msg) {

                    $('.loader-min-bar').hide();

                    //  $('#frm-importar').unblock();
                    $("#datatable").html(msg);

                    $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                        altClass: 'odd',
                        footer: true,
                        fixedColumns: 3
                    });





                }

            });
        }

    }

}

();

