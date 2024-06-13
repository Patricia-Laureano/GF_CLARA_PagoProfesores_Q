//var banco1 = new Bancos('banco');
//var tipotransferencia1 = new TiposTrasnferencia('tipodispersion');

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


        //  banco1.init("banco1");
        //  tipotransferencia1.init("tipotransferencia1");

        formValidation.Inputs(["fechai", "fechaf"]);
        //  formValidation.notEmpty('fechai', 'El campo fecha inicial no debe de estar vacio');
        // formValidation.notEmpty('fechaf', 'El campo fecha final no debe de estar vacio');


    });
});//End function jquery



var formPage = function () {


    "use strict"; return {

        clean: function () {

            formValidation.Clean();

            $("#fechai").val('');
            $("#fechaf").val('');
            //$("#tipodispersion").find('option').attr("selected", false);
            //$("#banco").find('option').attr("selected", false);


            consultar = false;
            DataTable.init();

        },


       Concultar: function (id) {

            consultar = true;

            if (!formValidation.Validate())
                return;

            DataTable.init();

        },

       /* ConsultarPDF: function (id) {

            // consultar = true;

            if (!formValidation.Validate())
                return;


            var datos = 'fechai=' + $('#fechai').val()
         + '&fechaf=' + $('#fechaf').val()
         + '&banco=' + $('#banco').val()
         + '&tipodispersion=' + $('#tipodispersion').val()
         + '&sedes=' + $('#Sedes').val();


            $.ajax({
                type: "GET",
                cache: false,
                url: "/Recibos/MergePDFS",
                data: datos,
                success: function (msg) {



                    $('#notification').html(msg);


                }

            });



            //  window.location.href = '/Recibos/MergePDFS?' + datos;

            //  DataTable.init();

        },*/




       ExportExcelTimbrado: function (table) {



            /*if (!formValidation.Validate())
                return;*/


            var r = confirm("Deseas hacer el timbrado!");
            if (r == true) {

                var datos = 'fechai=' + $('#fechai').val()
            + '&fechaf=' + $('#fechaf').val()
          //  + '&banco=' + $('#banco').val()
          //  + '&tipodispersion=' + $('#tipodispersion').val()
           + '&sedes=' + $('#Sedes').val();

                window.location.href = '/Timbrado/ExportExcelTimbrado?' + datos;


            }




        },


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

            window.location.href = '/Recibos/ExportExcel?' + datos;

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
            if (consultar) {
                filter = $('#Sedes').val();
            }


            var datos = '&fechai=' + $('#fechai').val()
            + '&fechaf=' + $('#fechaf').val();
          //  + '&banco=' + $('#banco').val()
           // + '&tipodispersion=' + $('#tipodispersion').val();



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
                url: "/Recibos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter,
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


}();

