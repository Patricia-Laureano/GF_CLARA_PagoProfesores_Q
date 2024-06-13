
var ciclo1 = new Ciclos('Ciclo');
var periodo1 = new Periodos('Periodo');
var tipospago1 = new TiposdePagosV('TipoPago');
var escuela1 = new Escuelas('Escuela');
var partePeriodo1 = new PartePeriodo('PartePeriodo');
var tipoDocente1 = new TipoDeDocente('TipoDocente');

$(function () {

    $(document).ready(function () {
        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 5
        });
    });

    $(window).load(function () {

        $("#btn-sgt").show();
        ciclo1.init("ciclo1");
        tipospago1.init("tipospago1");
        escuela1.init("escuela1");
        partePeriodo1.init("partePeriodo1");
        tipoDocente1.init("tipoDocente1");

        formValidation.Inputs(["Ciclo", "Periodo", "CampusPA"]);
        formValidation.notEmpty('Ciclo', 'El campo Año escolar no debe de estar vacio');
        formValidation.notEmpty('Periodo', 'El campo periodos no debe de estar vacio');
        formValidation.notEmpty('CampusPA', 'El campo campus no debe de estar vacio');
    });
});//End function jquery

function handlerdataSedes() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/ActualizaciondePA/getCampusPA",
        success: function (msg) {
            $('#CampusPA').html(msg);
        },
        error: function (data) {
            //  session_error(data);
        }
    });
}

Sedes.setSedes_success = function () {
}

function handlerdataCiclos() {

    if ((gup('Ciclo') != "")) { $('#Ciclo').val(gup('Ciclo')); }

    periodo1.id_ciclo = $("#Ciclo").val();
    periodo1.init("periodo1");
}

function handlerdataPeriodos() {
    if ((gup('Periodo') != "")) { $('#Periodo').val(gup('Periodo')); }
}

function handlerdataEscuelas() {
    if ((gup('Escuela') != "")) { $('#Escuela').val(gup('Escuela')); }
    if ((gup('CampusPA') != "")) { $('#CampusPA').val(gup('CampusPA')); }
}

function handlerdataTiposPagos() {
    if ((gup('TipoPago') != "")) { $('#TipoPago').val(gup('TipoPago')); }
}

function handlerdataPartePeriodo() {
    if ((gup('PartePeriodo') != "")) { $('#PartePeriodo').val(gup('PartePeriodo')); }
}

function handlerdataTipoDocente() {
    if ((gup('TipoDocente') != "")) { $('#TipoDocente').val(gup('TipoDocente')); }
}

$('#Ciclo').on('change', function () {
    periodo1.id_ciclo = this.value;
    periodo1.init("periodo1");
});

function verDetalles() {

    //alert($("#modal-detalles-conflictos").width());

    $.ajax({
        type: "GET",
        cache: false,
        url: "/ActualizaciondePA/getDetallesConflictoPA",
        success: function (msg) {
            $("#divDetallesConflictos").html(msg);
            $("#modal-detalles-conflictos").modal('show');
        },
        error: function (data) {
            session_error(data);
        }
    });
    //alert('Detalles ¬¬ ');
}

jQuery.fn.getCheckboxValues = function () {
    var values = [];
    var i = 0;
    this.each(function () {
        // guarda los valores en un array
        values[i++] = this.id;
    });
    // devuelve un array con los checkboxes seleccionados
    return values;
}

var formPage = function () {

    var id;

    "use strict"; return {
        clean: function () {
            $("#Ciclo").val("");
            $("#Periodo").val("");
            $("#Escuela").val("");
            $("#CampusPA").val("");
            $("#TiposPago").val("");
            $("#TipoDeContrato").val("");
            $("#TipoDocente").val("");
            $("#btn-sgt").hide();
        },

        consultar: function () {
            //$("#btn-sgt").hide();
            if (!formValidation.Validate())
                return;

            var data =
				  'Periodo=' + $('#Periodo').val()
                + '&Escuela=' + $('#Escuela').val()
                + '&TipoDeContrato=' + $('#TipoDeContrato').val()
                + '&TipoDocente=' + $('#TipoDocente').val()
                + '&CampusPA=' + $('#CampusPA').val()
                + '&Campus=' + $('#Sedes').val()
                + '&TipoPago=' + $('#TipoPago').val()
	            + '&PartePeriodo=' + $("#PartePeriodo").val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Transfiriendo datos de BANNER..');
            loading('loading-circle', '#seccion1', 'Consultando datos PA en BANNER..');
            //formPage.disableControls(true);

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ActualizaciondePA/Consultar",
                data: data,
                success: function (msg) {

                    $('#seccion1').unblock();

                    if (session_error(msg) == false) {
                        //if (msg != "ok")
                        $('#notification').html(msg);
                    }
                    DataTable.setPage(1);
                },
                error: function (data) {
                    session_error(data);
                    $('#blocking-panel-1').hide();
                    $('#blocking-panel-2').hide();
                }
            });
        },

        BotonSiguienteEsquema: function () {//

            var datos = 'Periodo=' + $('#Periodo').val()
                + '&PartePeriodo=' + $('#PartePeriodo').val()
                + '&Ciclo=' + $('#Ciclo').val()
                + '&TipoPago=' + $('#TipoPago').val()
                + '&Escuela=' + $('#Escuela').val()
                + '&Campus=' + $('#Sedes').val()
                + '&CampusPA=' + $('#CampusPA').val();

            window.location.href = '/Nomina?' + datos;
            //  $("#linksgt").attr("href", "/Nomina");
        },

        generar: function () {//boton importar
            var arr = DataTable.checkboxs;
            var arrChecked = [];

            for (var i = 0; i < arr.length; i++) {
                var checkbox_checked = $('#' + arr[i]).prop('checked');

                if (checkbox_checked == true)
                    arrChecked.push(arr[i]);
            }

            if (arrChecked.length == 0) {
                alert('Debes seleccionar una casilla');
                return;
            }

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Actualizando datos..');
            loading('loading-circle', '#seccion1', 'Importando PA desde BANNER..');

            $.ajax({
                type: "POST",
                cache: false,
                url: "/ActualizaciondePA/Generar",
                data: "ids=" + arrChecked.join(),
                success: function (msg) {
                    $('#seccion1').unblock();
                    DataTable.init();
                    $('#notification').html(msg);

                    //$("#linksgt").attr("href", "javascript:formPage.BotonSgtCalcularNomina()");
                    //   $("#btn-sgt").html('<li class="next" role="button" aria-disabled="false" style="display:none"><a href="/Nomina">Siguiente →</a></li>');


                    $("#btn-sgt").show();
                },
                error: function (msg) {
                    $('#seccion1').unblock();
                    $('#notification').html(msg);
                }
            });
        },

        generarTodo: function () {//boton importar todo

            var data = $("#Sedes").val();

            loading('loading-bar');
            loading('loading-circle', '#wizard', 'Importando PA desde BANNER..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ActualizaciondePA/GenerarTodo",
                data: "sedesPersns=" + data,
                success: function (msg) {
                    loading('stop', '#wizard', '');
                    $('#seccion1').unblock();
                    DataTable.init();
                    $('#notification').html(msg);

                    $("#btn-sgt").show();
                },
                error: function (msg) {
                    loading('stop', '#wizard', '');
                    $('#seccion1').unblock();
                    $('#notification').html(msg);
                }
            });
        },
    }
}();

var DataTable = function () {
    var pag = 1;
    var order = "NRC";
    var sortoption = {
        ASC: "ASC",
        DESC: "DESC"
    };
    var sort = sortoption.ASC;

    "use strict"; return {
        myName: 'DataTable',
        checkboxs: [],
        onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
                pag = 1;
                this.init();
            }
        },

        exportExcel: function (table) {
            window.location.href = '/ActualizaciondePA/ExportExcel';
        },

        /*	edit: function (id) {
                formPage.edit(id);
            },*/

        setPage: function (page) {
            pag = page;
            this.init();
        },

        setShow: function (page) {  //update 
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

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ActualizaciondePA/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {

                    $('.loader-min-bar').hide();
                    $("#datatable").html(msg);


                    $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                        altClass: 'odd',
                        footer: true,
                        fixedColumns: 5
                    });
                }
            });
        }
    }
}();

function gup(name) {

    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var tmpURL = window.location.href;
    var results = regex.exec(tmpURL);
    if (results == null)
        return "";
    else
        return results[1];
}