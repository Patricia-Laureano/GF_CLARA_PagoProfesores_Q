
var ciclo1        = new Ciclos('Ciclo');
var periodo1      = new Periodos('Periodo');
var tipospago1    = new TiposdePagos('TipoPago');
var escuela1      = new Escuelas('Escuela');
var partePeriodo1 = new PartePeriodo('PartePeriodo');

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

        formValidation.Inputs(["Ciclo", "Periodo", "CampusPA"]);
        formValidation.notEmpty("Ciclo", "El campo 'Año Escolar' no debe de estar vacío");
        formValidation.notEmpty("Periodo", "El campo 'Periodo' no debe de estar vacío");
        formValidation.notEmpty("CampusPA", "El campo 'Campus' no debe de estar vacío");
    });
});//End function jquery

function handlerdataSedes() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/ActualizacionPA/getCampusPA",       
        success: function (msg) {
            $('#CampusPA').html(msg);
        },
        error: function (data) {
          //  session_error(data);
        }
    });
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

$('#Ciclo').on('change', function () {
    periodo1.id_ciclo = this.value;
    periodo1.init("periodo1");
});

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

        consultar: function (opc) {
            if (!formValidation.Validate())
                return;

            var data =
                'Periodo=' + $('#Periodo').val()
                + '&TipoDeContrato=' + $('#TipoDeContrato').val()
                + '&TipoPago=' + $('#TipoPago').val()
                + '&Escuela=' + $('#Escuela').val()
                + '&Campus=' + $('#Sedes').val()
                + '&CampusPA=' + $('#CampusPA').val()
                + '&TipoDocente=' + $('#TipoDocente').val()
                + '&PartePeriodo=' + $("#PartePeriodo").val()
                + '&Opc=' + opc;

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Transfiriendo datos de BANNER..');
            loading('loading-circle', '#seccion1', 'Consultando datos PA en BANNER..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ActualizacionPA/Consultar",
                data: data,
                success: function (msg) {

                    $('#seccion1').unblock();

                    if (session_error(msg) == false) {
                        //if (msg != "ok")
                        $('#notification').html(msg);
                    }
                    //DataTable.setPage(1);
                    DataTable.setPageX(1, opc);
                },
                error: function (data) {
                    session_error(data);
                    $('#blocking-panel-1').hide();
                    $('#blocking-panel-2').hide();
                }
            });

            $('#formbtngenerartodo').prop("disabled", false);
        },

        consulConcil: function () {

            //if (!formValidation.Validate())
            //    return;

            //var data =
            //	  'Periodo='         + $('#Periodo').val()
            //    + '&TipoDeContrato=' + $('#TipoDeContrato').val()
            //    + '&TipoPago='       + $('#TipoPago').val()
            //    + '&Escuela='        + $('#Escuela').val()
            //    + '&Campus='         + $('#Campus').val()
            //    + '&CampusPA='       + $('#CampusPA').val()
            //    + '&TipoDocente='    + $('#TipoDocente').val()
            //    + '&PartePeriodo='   + $("#PartePeriodo").val();

            //loading('loading-bar');
            //loading('loading-circle', '#datatable', 'xxx..');
            //loading('loading-circle', '#seccion1', 'zzz..');

            //$.ajax({
            //    type: "GET",
            //    cache: false,
            //    url: "/ActualizacionPA/ConsultarConcil",
            //    data: data,
            //    success: function (msg) {

            //        $('#seccion1').unblock();

            //        if (session_error(msg) == false) {
            //            $('#notification').html(msg);
            //        }
            //        DataTable.setPage(1);
            //    },
            //    error: function (data) {
            //        session_error(data);
            //        $('#blocking-panel-1').hide();
            //        $('#blocking-panel-2').hide();
            //    }
            //});

            DataTable.init2();

            $('#formbtnnomina').prop("disabled", false);
        },

        BotonSiguienteNomina: function () {//

            $("#titulo-legend").html('<span>Generar la nomina de la reciente información consultada.</span>');

            $('#btn-ant').show();
            $('#btn-sgt').hide();
            $('#formbtngenerartodo').hide();
            $('#formbtnconsultarAPA1').hide();
            $('#formbtnedocta').hide();
            $('#formbtnconsuConc').show();
            $('#formbtnnomina').show();

            $("#tab-bannerPa").removeClass("active").addClass("");
            $("#tab-edocta").removeClass("active").addClass("");
            $("#tab-nomina").addClass("active");

            $("#linkant").attr("href", "javascript:formPage.BotonAntPA()");
        },

        BotonAntPA: function () {
            $("#titulo-legend").html('<span>Actualización de la programación académica.</span>');

            $('#btn-ant').hide();
            $('#btn-sgt').hide();
            $('#formbtngenerartodo').show();
            $('#formbtngenerartodo').prop("disabled", true);
            $('#formbtnconsultarAPA1').show();
            $('#formbtnconsuConc').hide();
            $('#formbtnnomina').hide();
            $('#formbtnConsultaNomina').hide();

            $("#tab-bannerPa").addClass("active");
            $("#tab-nomina").removeClass("active").addClass("");

            $("#linksgt").attr("href", "javascript:formPage.BotonSiguienteNomina()");
        },

        BotonSgtGenerarEdoCta: function () {
            $("#titulo-legend").html('<span>Actualización de la programación académica.</span>');

            $('#btn-ant').show();
            $('#btn-sgt').hide();
            $('#formbtnedocta').show();
            $('#formbtnedocta').prop("disabled", false);

            $("#tab-nomina").removeClass("active").addClass("");
            $("#tab-edocta").addClass("active");

            $('#formbtngenerartodo').hide();
            $('#formbtnconsultarAPA1').hide();
            $('#formbtnconsuConc').hide();
            $('#formbtnnomina').hide();
            $('#formbtnConsultaNomina').hide();

            $("#linkant").attr("href", "javascript:formPage.BotonSiguienteNomina()");
            //$("#linksgt").attr("href", "javascript:formPage.BotonSiguienteEdoCta()");
        },

        BotonSgtImprimirEstadoCuenta: function () {
            window.location.href = '/EstadodeCuenta';
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
                url: "/PA/Generar",
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
            var model = {
                PeriodoX: $('#Periodo').val(),
                EscuelaX: $('#Escuela').val(),
                SedeX: $('#Sedes').val(),
                CampusX: $('#CampusPA').val(),
                TipoPago: $('#TipoPago').val(),
                PartePeriodoX: $('#PartePeriodo').val(),
            }

            $.ajax({
                type: "POST",
                cache: false,
                url: "/ActualizacionPA/GenerarTodo",
                data: model,
                success: function (data) {
                    $('#seccion1').unblock();
                    //DataTable.init();
                    //$('#notification').html(data.msg);
                    //$("#btn-sgt").show();
                    formPage.calcularNomina();


                },
                error: function (data) {
                    $('#seccion1').unblock();
                    $('#notification').html(data.msg);
                }
            });
        },

        calcularNomina: function () {
            var model = {
                PeriodoX: $('#Periodo').val(),
                PartePeriodoX: $('#PartePeriodo').val(),
                SedeX: $('#Sedes').val(),
                CampusX: $('#CampusPA').val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ActualizacionPA/actualizaNomina",
                data: model,
                success: function (data) {
                    //$('#seccion1').unblock();
                    //$('#notification').html(data.msg);
                    //DataTable.init3();

                    //$('#formbtnConsultaNomina').show();
                    //$('#formbtnConsultaNomina').prop("disabled", false);

                    //$('#btn-sgt').show();
                    //$("#linksgt").attr("href", "javascript:formPage.BotonSgtGenerarEdoCta()");
                    formPage.generarEdoCta();
                },
                error: function (data) {
                    $('#seccion1').unblock();
                    session_error(data);
                }
            });
        },

        consultarNomina: function () {
            DataTable.init3();
        },

        generarEdoCta: function () {
            var model = {
                PeriodoX: $('#Periodo').val(),
                SedeX: $('#Sedes').val(),
                TipoPago: $('#TipoPago').val(),
            }

            //loading('loading-bar');
            //loading('loading-circle', '#datatable', 'Actualizando datos..');
            //loading('loading-circle', '#seccion1', 'Importando PA desde BANNER..');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ActualizacionPA/generarEdoCta/",
                data: model,
                success: function (data) {
                    //$('#seccion1').unblock();
                    //$('#notification').html(data.msg);
                    DataTable.init3();

                    $("#linksgt").attr("href", "javascript:formPage.BotonSgtImprimirEstadoCuenta()");
                    $('#btn-sgt').show();

                },
                error: function (data) {
                    $('#seccion1').unblock();
                    $('#notification').html(data.msg);
                    session_error(data);
                }
            });
        },

        BotonSiguienteAPA2: function () {
            //$("#titulo-legend").html('<span>Seleccione la Programación Academica a la que desea asignar el Centro de Costos</span>');

            $("#tab-bannerPa1").removeClass("active").addClass("");
            $("#tab-bannerPa2").addClass("active");

            $("#formbtnconsultarAPA1").hide();
            $("#formbtnconsultarAPA2").show();

            $("#linkant").attr("href", "javascript:formPage.BotonAnteriorAPA1()");
            $("#linksgt").attr("href", "javascript:formPage.BotonSiguienteAPA3()");

            $("#btn-ant").show();
        },

        BotonSiguienteAPA3: function () {
            //$("#titulo-legend").html('<span>Seleccione la Programación Academica a la que desea asignar el Centro de Costos</span>');

            $("#tab-bannerPa1").removeClass("active").addClass("");
            $("#tab-bannerPa2").removeClass("active").addClass("");
            $("#tab-bannerPa3").addClass("active");

            $("#formbtnconsultarAPA1").hide();
            $("#formbtnconsultarAPA2").hide();
            $("#formbtnconsultarAPA3").show();

            $("#linkant").attr("href", "javascript:formPage.BotonAnteriorAPA2()");

            $("#btn-sgt").hide();
        },

        BotonAnteriorAPA2: function () {
            //$("#titulo-legend").html('<span>Seleccione la Programación Academica a la que desea asignar el Centro de Costos</span>');

            $("#tab-bannerPa1").removeClass("active").addClass("");
            $("#tab-bannerPa3").removeClass("active").addClass("");
            $("#tab-bannerPa2").addClass("active");

            $("#formbtnconsultarAPA1").hide();
            $("#formbtnconsultarAPA2").show();
            $("#formbtnconsultarAPA3").hide();

            $("#linkant").attr("href", "javascript:formPage.BotonAnteriorAPA1()");
            $("#linksgt").attr("href", "javascript:formPage.BotonSiguienteAPA3()");

            $("#btn-sgt").show();
        },

        BotonAnteriorAPA1: function () {
            //$("#titulo-legend").html('<span>Seleccione la Programación Academica a la que desea asignar el Centro de Costos</span>');

            $("#tab-bannerPa2").removeClass("active").addClass("");
            $("#tab-bannerPa3").removeClass("active").addClass("");
            $("#tab-bannerPa1").addClass("active");

            $("#formbtnconsultarAPA1").show();
            $("#formbtnconsultarAPA2").hide();
            $("#formbtnconsultarAPA3").hide();

            $("#linksgt").attr("href", "javascript:formPage.BotonSiguienteAPA2()");

            $("#btn-ant").hide();
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
            window.location.href = '/ActualizacionPA/ExportExcel';
        },

        /*	edit: function (id) {
                formPage.edit(id);
            },*/

        setPage: function (page) {
            pag = page;
            this.init();
        },

        setPageX: function (page, opcIni) {
            pag = page;
            if (opcIni == 1)
                this.init();
            else if (opcIni == 2)
                this.init2();
            else if (opcIni == 3)
                this.init3();
            else this.init();
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
            var msgX;
            var msgZ;

            var data = '&Periodo=' + $('#Periodo').val()
                     + '&PartePeriodo=' + $('#PartePeriodo').val()
                     + '&OpcionPago=' + $('#TipoPago').val()
                     + '&CampusVPDI=' + $('#Sedes').val()
                     + '&CampusPA=' + $('#CampusPA').val()
                     + '&Escuela=' + $('#Escuela').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ActualizacionPA/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + data,
                success: function (msg) {

                    $('.loader-min-bar').hide();
                    $("#datatable").html(msg);

                    msgX = $('#_NumRegX').text();

                    msgZ = "<div class=\"row\"><div id =\"alerta\" class=\"alert alert-success fade in\"><span data-dismiss=\"alert\" class=\"close\"><img src=\"Content/images/icon-close-green.png\" style=\"margin-top:-7px\"></span><img src=\"Content/images/icon-ok.png\" class=\"pull-left\" style=\"margin-top:-7px\">"
                           + "<p> " + $('#notification').text() + ". Registros a actualizar: " + msgX + "</p></div></div>  <input type=\"hidden\" id=\"NotificationType\" value=\"SUCCESS\">";

                    $('#notification').html(msgZ);

                    $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                        altClass: 'odd',
                        footer: true,
                        fixedColumns: 5
                    });
                }
            });
        },

        init2: function () {
            var show = $('#' + this.myName + '-data-elements-length').val();
            var search = $('#' + this.myName + '-searchtable').val();
            var orderby = order;
            var sorter = sort;

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ActualizacionPA/CreateDataTable2/",
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
        },

        init3: function () {
            var show = $('#' + this.myName + '-data-elements-length').val();
            var search = $('#' + this.myName + '-searchtable').val();
            //var orderby = order;
            var orderby = "IDSIU";
            var sorter = sort;

            var data = '&Periodo=' + $('#Periodo').val()
                     + '&OpcionPago=' + $('#TipoPago').val()
                     + '&CampusVPDI=' + $('#Sedes').val()
                     + '&Escuela=' + $('#Escuela').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ActualizacionPA/CreateDataTable3/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + data,
                success: function (msg) {
                    $('.loader-min-bar').hide();
                    if (session_error(msg) == false) {
                        $("#datatable").html(msg);

                        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                            altClass: 'odd',
                            footer: true,
                            fixedColumns: 3
                        });
                    }
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