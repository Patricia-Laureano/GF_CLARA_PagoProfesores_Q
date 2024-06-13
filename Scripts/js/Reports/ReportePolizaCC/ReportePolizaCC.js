
function Anios(idElement) {

    this.idElement = idElement;
    this.Anio = "";

    this.init = function (obj) {

        var dir = eval(obj);
        this.Anio = "";
        this.getAnios();

    };


    this.getAnios = function () {

        idElement = this.idElement;
        Anio = this.Anio;

        $.ajax({
            type: "GET",
            cache: false,
            url: "/Anios/getAnios/",
            data: "Anio=" + Anio,
            success: function (data) {
                $('#' + idElement).html(data);

            }
            /*,
            complete: handlerdataAnios
            */
        });

    };
}

function Meses(idElement) {

    this.idElement = idElement;
    this.Mes = "";

    this.init = function (obj) {

        var dir = eval(obj);
        this.Mes = "";
        this.getMeses();

    };


    this.getMeses = function () {

        idElement = this.idElement;
        Mes = this.Mes;
        Anio = this.Anio;
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Meses/getMeses/",
            success: function (data) {
                $('#' + idElement).html(data);

            }
            /*
            ,
            complete: handlerdataMeses
            */
        });

    };
}
var anios = new Anios("anio");
//var meses = new Meses("mes");

$(function () {

    $(window).load(function () {
        anios.init("anios");
        //   meses.init("meses");
        formPage.clean();
    });
});//End function jquery

/*
function handlerdataAnios() {

    anios.id_anio = $('#anio').val();
    anios.init("anios");
}

function handlerdataMeses() {

    meses.id_mes = $('#mes').val();
    meses.init("meses");
}
*/

var formPage = function () {
    "use strict"; return {
        clean: function () {
            $("#anio").val("");
            $("#mes").val("");
        },

        consultar: function () {
            //if (!formPage.validateInputs()) {
            //    alert('Necesita Seleccionar un "Mes".');
            //    return;
            //}

            DataTable.init();
        },

        ConsultaMeses: function () {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/ReportePolizaCC/ConsultaMeses/",
                success: function (msg) {
                    if (session_error(msg) == false) {
                        $('#mes').html(msg);
                    }
                },
                error: function (msg) { session_error(msg); }
            });
        },

        ConsultaAnos: function () {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/ReportePolizaCC/ConsultaAnos/",
                success: function (msg) {
                    if (session_error(msg) == false) {
                        $('#anio').html(msg);
                    }
                },
                error: function (msg) { session_error(msg); }
            });
        },

        Anio_OnChange: function (id) {
            periodos.id_ciclo = id;
            periodos.init("periodos");
        },

        validateInputs: function () {
            var validado = true;

            if ($("#mes").val() == "" || $("#mes").val() == null) { $("#mes").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#mes").removeClass("form-control parsley-error").addClass("form-control"); }
            return validado;
        },
    }
}();


var DataTable = function () {
    var controller = "ReportePolizaCC"
    var pag = 1;
    var order = "fecha_de_pago";
    var sortoption = {
        ASC: "ASC",
        DESC: "DESC"
    };
    var sort = sortoption.ASC;

    "use strict"; return {

        onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
                pag = 1
                this.init();
            }
        },
        exportExcel: function (table) {

            window.location.href = '/' + controller + '/ExportExcel';

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
            var sortcampo = $("#SORT-" + campo).data("sort");
            if (sortcampo == sortoption.ASC) { sort = sortoption.DESC; } else { sort = sortoption.ASC; }
            this.init();
        },

        init: function () {

            var show = $("#data-elements-length").val();
            var search = $("#searchtable").val();
            var orderby = order;
            var sorter = sort;
            var filterMes = $("#mes").val();
            var filterAnio = $("#anio").val();
            var sede = $("#Sedes").val();

            var tipodocente = +$('#tipodocente').val();
            var pagosdeposito = +$('#pagosdeposito').val();


            $("#xls_btn").show();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos...');
            loading('loading-circle', '#seccion1', 'Consultando datos...');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ReportePolizaCC/CreateDataTableHTML/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&filterMes=" + filterMes + "&filterAnio=" + filterAnio + "&sede=" + sede  + '&tipodocente=' + $('#tipodocente').val() + '&pagosdeposito=' + $('#pagosdeposito').val(),
                success: function (msg) {
                    $('#seccion1').unblock();
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();

function exporttableXLS() {
    $("#t1").table2excel({
        exclude: ".noExl",
        name: "Excel Document Name",
        filename: "NominaAnoMes",
        fileext: ".xls",
        exclude_img: true,
        exclude_links: true,
        exclude_inputs: true
    });
}


