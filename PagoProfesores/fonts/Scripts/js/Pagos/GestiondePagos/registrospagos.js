

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


    $("#fechair").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });


    $("#fechafr").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });




    $("#fechaid").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });


    $("#fechafd").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });



    $("#fechaidp").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });


    $("#fechafdp").datepicker({
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

       // formValidation.Inputs(["fechai", "fechaf"]);
      //  formValidation.notEmpty('fechai', 'El campo fecha inicial no debe de estar vacio');
       // formValidation.notEmpty('fechaf', 'El campo fecha final no debe de estar vacio');
       

    });
});//End function jquery



var formPage = function () {


    "use strict"; return {

        clean: function () {
           //  formValidation.Clean();
            consultar = false;

           // DataTable.init();

            $("#fechai").val('');
            $("#fechaf").val('');

            $("#fechair").val('');
            $("#fechafr").val('');

            $("#fechaid").val('');
            $("#fechafd").val('');

            $("#fechaidp").val('');
            $("#fechafdp").val('');
        

            DataTable.init();

        },


        Concultar: function (id) {

            /*if (!formValidation.Validate())
                return;*/
            consultar = true;
            //  DataTable.consultar = true;
            DataTable.init();

        }


    }


}();
/*
var Sedes = function () {

    "use strict"; return {

        setSedes: function () {
            DataTable.init();
        }
    }
}();*/

Sedes.setSedes_success = function () {

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
                + '&fechair=' + $('#fechair').val()
                + '&fechafr=' + $('#fechafr').val()
                + '&fechaid=' + $('#fechaid').val()
                + '&fechafd=' + $('#fechafd').val()
                + '&fechaidp=' + $('#fechaidp').val()
                + '&fechafdp=' + $('#fechafdp').val()
                + '&sedes=' + $('#Sedes').val();

            window.location.href = '/RegistrosPagos/ExportExcel?' + datos;
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
                      + '&fechaf=' + $('#fechaf').val()
                      + '&fechair=' + $('#fechair').val()
                      + '&fechafr=' + $('#fechafr').val()
                      + '&fechaid=' + $('#fechaid').val()
                      + '&fechafd=' + $('#fechafd').val()
                      + '&fechaidp=' + $('#fechaidp').val()
                      + '&fechafdp=' + $('#fechafdp').val();

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
                url: "/RegistrosPagos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter,
                success: function (msg) {
                    $('.loader-min-bar').hide();
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