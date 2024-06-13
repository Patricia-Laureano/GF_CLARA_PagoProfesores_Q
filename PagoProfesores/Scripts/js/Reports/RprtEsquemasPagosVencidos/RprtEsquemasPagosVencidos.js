var periodos = new Periodos("periodos");
var ciclos = new Ciclos("ciclos");

$(function () {
    $(window).load(function () {
        ciclos.init("ciclos");
        formPage.clean();

        $('#periodos').val("");
        $('#situacion').val("");
    });
});//End function jquery

function handlerdataCiclos() {
    periodos.id_ciclo = $('#ciclos').val();
    periodos.init("periodos");
}

function handlerdataPeriodos() {
}

var formPage = function () {

    "use strict"; return {
        clean: function () {
            $("#ciclos").val("");
            $("#periodos").val("");
            $('#situacion').val("");
            //consultar = false;
        },

        consultar: function () {
            if (!formPage.validateInputs()) {
                alert('Necesita seleccionar un "Ciclo".');
                return;
            }

            DataTable.init();
        },

        ConsultaCiclos: function () {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/RprtEsquemasPagosVencidos/ConsultaCiclos/",
                success: function (msg) {
                    if (session_error(msg) == false) {
                        $('#Ciclo').html(msg);
                        formPage.Ciclo_OnChange();
                    }
                },
                error: function (msg) { session_error(msg); }
            });
        },

        ConsultaPeriodos: function () {
            DataTable.init()
        },

        Ciclo_OnChange: function (id) {
            periodos.id_ciclo = id;
            periodos.init("periodos");
        },

        validateInputs: function () {
            var validado = true;

            if ($("#ciclos").val() == "" || $("#ciclos").val() == null) { $("#ciclos").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#ciclos").removeClass("form-control parsley-error").addClass("form-control"); }
            return validado;
        },
    }
}();

var DataTable = function () {
    var controller = "RprtEsquemasPagosVencidos"
    var pag = 1;
    var order = "ANIO";
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
                pag = 1
                this.init();
            }
        },

        exportExcel: function (table) {
            var filter = 'sede=' + $('#Sedes').val();
            var filterA = 'ciclo=' + $('#ciclos').val();
            var filterP = 'periodo=' + $('#periodos').val();
            var filterS = 'situacion=' + $('#situacion').val();

            window.location.href = '/' + controller + '/ExportExcel?' + filter + '&' + filterA + '&' + filterP + '&' + filterS;
        },

        setShow: function (page) {
            pag = 1
            this.init();
        },

        edit: function (id) {
            // formPage.edit(id);
        },

        setPage: function (page) {
            pag = page;
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
            filter = "";

            var filter = $('#Sedes').val();
            var filterA = $("#ciclos").val();
            var filterP = $("#periodos").val();
            var filterS = $("#situacion").val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos...');
            loading('loading-circle', '#seccion1', 'Consultando datos...');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/RprtEsquemasPagosVencidos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&filter=" + filter + "&filterA=" + filterA + "&filterP=" + filterP + "&filterS=" + filterS,
                success: function (msg) {
                    $('#seccion1').unblock();
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();