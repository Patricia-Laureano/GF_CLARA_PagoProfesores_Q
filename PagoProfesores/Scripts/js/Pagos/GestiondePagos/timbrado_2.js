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
        $("#formbtntimbrarTodo").prop('disabled', true);
        $("#formbtntimbrar").prop('disabled', true);
        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 3
        });
    });
    
    $(window).load(function () {
        formValidation.Inputs(["fechai", "fechaf"]);
    });
});//End function jquery

function base64toPDF(datos) {
    alert("pdf");
    $.ajax({
        type: "GET",
        cache: false,
        url: "/Timbrado/base64toPDF",
        data: datos,
        success: function (msg) {
            alert("bajando pdf...");
        }
    });
};

var formPage = function () {
    "use strict"; return {
        clean: function () {
            formValidation.Clean();
            $("#formbtntimbrarTodo").prop('disabled', true);
            $("#formbtntimbrar").prop('disabled', true);
            $("#fechai").val('');
            $("#fechaf").val('');
            consultar = false;
            DataTable.init();
        },

        Concultar: function (id) {
            consultar = true;

            if (!formValidation.Validate())
                return;

            DataTable.init();

            $("#formbtntimbrarTodo").prop('disabled', false);
            $("#formbtntimbrar").prop('disabled', false);
        },

        base64toPDF: function (datos) {
            alert("pdf");

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Timbrado/base64toPDF",
                data: datos,
                success: function (msg) {
                    alert("bajando pdf...");
                }
            });
        },

        obtieneMsjErrorBitacora: function () {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/Timbrado/ObtieneMsgErrorBitacora",
                data: datos,
                success: function (msg) {
                    alert("bajando pdf...");
                }
            });
        },

        TimbrarTodo: function () {
            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Timbrando...');
            var datos = 'fechai=' + $('#fechai').val() + '&fechaf=' + $('#fechaf').val();

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Timbrado/TimbradoTest",
                data: datos,
                success: function (msg) {
                    formPage.Concultar();
                    $('#notification').html(msg);
                }
            });
        },

        Timbrar: function () {
            //var arr = DataTable.checkboxs;
            var arr = [];

            $('#formD input[type=checkbox]').each(function () {
                arr.push($(this).attr("id"));
            }); 

            var arrChecked = [];

            for (var i = 0; i < arr.length / 2; i++) {
                var checkbox_checked = $('#' + arr[i]).prop('checked');

                if (checkbox_checked == true)
                    arrChecked.push(arr[i]);
            }

            if (arrChecked.length == 0) {
                alert('Debes seleccionar una casilla');
                return;
            }

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Timbrando...');
            var datos = 'fechai=' + $('#fechai').val() + '&fechaf=' + $('#fechaf').val();

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Timbrado/Timbrar",
                data: datos + "&IDs=" + arrChecked.join(),
                success: function (msg) {
                    formPage.Concultar();
                    $('#notification').html(msg);
                }
            });
        }
    }
}();

Sedes.setSedes_success = function () {
    consultar = true;
    DataTable.init();
}

var DataTable = function () {
    var pag = 1;
    var order = "ID_ESTADODECUENTA";

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

            window.location.href = '/Timbrado/ExportExcel?' + datos;
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

            var datos = '&fechai=' + $('#fechai').val() + '&fechaf=' + $('#fechaf').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Timbrado/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + '&fechai=' + $('#fechai').val()
                + '&fechaf=' + $('#fechaf').val() + "&filter=" + filter,
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