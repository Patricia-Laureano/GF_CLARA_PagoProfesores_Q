

var periodos = new Periodos("periodos");
var ciclos = new Ciclos("ciclos");

$(function () {

    $(window).load(function () {
        ciclos.init("ciclos");
        formPage.clean();

        $('#periodos').val("");
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
        },

        consultar: function () {
            if (!formPage.validateInputs()) {
                alert('Necesita seleccionar un "Periodo".');
                return;
            }

            DataTable.init();
        },

        ConsultaCiclos: function () {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/CalendariodePago/ConsultaCiclos/",
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

            if ($("#periodos").val() == "" || $("#periodos").val() == null) { $("#periodos").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#periodos").removeClass("form-control parsley-error").addClass("form-control"); }
            return validado;
        },
    }
}();


var DataTable = function () {
    var controller = "CalendariodePago"
    var pag = 1;
    var order = "CVE_CICLO";
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
            var data = 'Periodo=' + $('#periodos').val();

            window.location.href = '/' + controller + '/ExportExcel?' + data;

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
            var filterC = $("#ciclos").val();
            var filterP = $("#periodos").val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos...');
            loading('loading-circle', '#seccion1', 'Consultando datos...');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/CalendariodePago/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&filterC=" + filterC + "&filterP=" + filterP,
                success: function (msg) {
                    $('#seccion1').unblock();
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();


