
var c_Ciclo = new Ciclos('Ciclo');
var c_Periodo = new Periodos('Periodo');
var c_Esquema = new Esquema('Esquema');
var c_ConceptoPago = new ConceptosdePago("ConceptoPago");
//var c_FechaPago = new FechasdePagoEsquema("FechaPago");

$(function () {
    $(window).load(function () {
        c_Ciclo.init("Ciclo");

        formValidation.Inputs(["c_idSiu", "c_nombreCompleto", "Ciclo", "Periodo", "Esquema", "ConceptoPago", "FechaPago", "Mensaje"]);
        formValidation.notEmpty('Ciclo', 'El campo "Año escolar" no debe de estar vacio');
        formValidation.notEmpty('Periodo', 'El campo "Periodo" no debe de estar vacio');
        formValidation.notEmpty('Esquema', 'El campo "Esquema" no debe de estar vacio');
        formValidation.notEmpty('ConceptoPago', 'El campo "Concepto de pago" no debe de estar vacio');
        formValidation.notEmpty('FechaPago', 'El campo "Fecha de pago" no debe de estar vacio');
        formValidation.notEmpty('Mensaje', 'El campo "Mensage a profesores" no debe de estar vacio');

        DataTable.init();
        formPage.init();
    });
});

function handlerdataCiclos() {
}

function handlerdataPeriodos() {
}

function handlerdatagetConceptosdePago() {
}

//function handlerdataFechasdepagoEsquema() {
//    $("#FechaPago option[value='" + $('#ConceptoPago').val() + "']").attr("selected", "selected");
//}

$('#Ciclo').on('change', function () {

    $('#Esquema').html("<option><option>");
    $('#ConceptoPago').html("<option><option>");
    //$('#FechaPago').html("<option><option>");

    if (this.value == "") {//ciclo(anio)
        $('#Periodo').html("<option><option>");
    } else {
        c_Periodo.id_ciclo = this.value;
        c_Periodo.Periodo = null;
        c_Periodo.init("Periodo");
    }
});

$('#Periodo').on('change', function () {
    if (this.value == "") {//periodo
        $('#Esquema').html("<option><option>");
    } else {
        c_Esquema.Sede = $('#Sedes').val();
        c_Esquema.Periodo = this.value;
        c_Esquema.IdEsquema = null;
        c_Esquema.init();
    }
});

$('#Esquema').on('change', function () {

    //$('#FechaPago').html("<option><option>");

    if (this.value == "") {//esquema
        $('#ConceptoPago').html("<option><option>");
    } else {
        c_ConceptoPago.EsquemaID = this.value;
        c_ConceptoPago.NumPago = null;
        //c_ConceptoPago.PersonaID = $("#c_idSiuECHidden").val();
        c_ConceptoPago.init();
    }
});

$('#ConceptoPago').on('change', function () {

    $.ajax({
        type: "GET",
        cache: false,
        url: "/ConceptosdePago/getFechaConceptoPago/",
        data: "EsquemaID=" + $('#Esquema').val() + "&ConceptoPagoPk=" + this.value,
        success: function (data) {
            $('#FechaPago').val(data);
        }
    });
});

//$('#FechaPago').on('change', function () {
//    $("#FechaPago option[value='" + this.value + "']").attr("selected", "selected");
//});

$('#c_idSiu').keypress(function (e) {
    if (e.which == 13) {
        var model = {
            IdSIU: this.value,
            CveSede: $('#Sedes').val(),
        }

        $.ajax({
            type: "POST",
            dataType: 'json',
            cache: false,
            url: "/EstadodeCuenta/BuscaPersona/",
            data: model,
            success: function (data) {

                data = jQuery.parseJSON(data);
                $("#c_nombreCompleto").val(((data.Nombres == 'null' || data.Nombres == null) ? '' : data.Nombres) + ' ' + ((data.Apellidos == 'null' || data.Apellidos == null) ? '' : data.Apellidos));

                //$("#c_idSiuECHidden").val(data.IdPersona);
                $("#c_idPersonaHidden").val(data.IdPersona);
                //$("#formbtnconsultar").prop("disabled", false);
                //formPage.consultar();
            },
            error: function (msg) {
                session_error(msg);
                $('#notification').html(msg);
            }
        });
    }
});

var formPage = function () {
    var idComunicado;

    "use strict"; return {
        init: function () {

            var model = {
                CveSede: $('#Sedes').val(),
            }

            $.ajax({
                type: "POST",
                cache: false,
                url: "/Personas2/getPersonas/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);

                    var options = {
                        data: data,
                        getValue: "PERSONA",
                        list: {

                            maxNumberOfElements: 6,
                            match: {
                                enabled: true
                            },

                            onChooseEvent: function () {
                                var value = $("#c_nombreCompleto").getSelectedItemData().IDSIU;

                                var modelX = {
                                    IdSIU: value,
                                    CveSede: $('#Sedes').val()
                                }

                                $.ajax({
                                    type: "POST",
                                    dataType: 'json',
                                    cache: false,
                                    url: "/EstadodeCuenta/BuscaPersona/",
                                    data: modelX,
                                    success: function (data) {
                                        data = jQuery.parseJSON(data);
                                        
                                        $("#c_idSiu").val(data.IdSIU);
                                        $("#c_idSiuHidden").val(data.IdSIU);
                                        //$("#formbtnconsultar").prop("disabled", false);
                                        $("#c_idPersonaHidden").val(data.IdPersona);

                                        formPage.consultar();
                                    },
                                    error: function (msg) {
                                        session_error(msg);
                                        $('#notification').html(msg);
                                    }
                                });
                            }
                        }
                    };

                    try {
                        $("#c_nombreCompleto").easyAutocomplete(options);
                    } catch {}
                }
            });
        },

        clean: function () {
            //formValidation.Clean();

            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").hide();
            $("#formbtndelete").prop("disabled", true);
            //$("#clave").prop("disabled", false);

            $("#c_idSiuHidden").val('');
            $("#c_idPersonaHidden").val('');
            $("#c_idComunicadoHidden").val('');
            $("#c_idSiu").val('');
            $("#c_nombreCompleto").val('');

            //$("#Periodo").find('option').attr("selected", false);
            //$("#Ciclo").find('option').attr("selected", false);

            $("#Ciclo").val('');
            $("#Periodo").val('');
            $("#Esquema").val('');
            $("#ConceptoPago").val('');
            $("#FechaPago").val('');
            $("#Mensaje").val('');
            //this.init();
        },

        add: function () {
            var model = {
                CveSede: $('#Sedes').val(),
                Periodo: $('#Periodo').val(),
                PersonaId: $('#c_idPersonaHidden').val(),
                EsquemaId: $('#Esquema').val(),
                ConceptoPagoPk: $('#ConceptoPago').val(),
                //FechaPago: $('#FechaPago').val(),
                Mensaje: $('#Mensaje').val(),
            }

            //if (!formValidation.Validate()) {
            //    $('.nav-tabs a[href="#default-tab-1"]').tab('show');
            //    return;
            //}

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Comunicado/Guardar/",
                data: model,
                success: function (data) {
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        edit: function (id) {

            idComunicado = id;

            var model = {
                PK1: idComunicado
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Comunicado/Consultar/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#Ciclo").val(data.Ciclo);
                    $("#Ciclo option[value=" + data.Ciclo + "]").attr("selected", true);

                    c_Periodo.Periodo = data.Periodo;
                    c_Periodo.id_ciclo = data.Ciclo;
                    c_Periodo.init("Periodo");

                    c_Esquema.IdEsquema = data.EsquemaId;
                    c_Esquema.Sede = data.CveSede;
                    c_Esquema.Periodo = data.Periodo;
                    c_Esquema.init("Esquema");

                    c_ConceptoPago.NumPago = data.ConceptoPagoPk;
                    c_ConceptoPago.EsquemaID = data.EsquemaId;
                    c_ConceptoPago.init("ConceptoPago");

                    //c_FechaPago.EsquemaID = data.EsquemaId;
                    //c_FechaPago.init("FechaPago");
                    //$("#FechaPago option[value='" + c_ConceptoPago.NumPago + "']").attr("selected", "selected");

                    $("#FechaPago").val(data.FechaPago);

                    $("#formbtnadd").hide();
                    $("#formbtnsave").show();
                    $("#formbtndelete").show();

                    $("#formbtnadd").prop("disabled", true);
                    $("#formbtnsave").prop("disabled", false);
                    $("#formbtndelete").prop("disabled", false);

                    $("#c_idComunicadoHidden").val(idComunicado);

                    $("#Mensaje").val(data.Mensaje);
                    $("#c_nombreCompleto").val(data.Profesor);
                    $("#c_idSiu").val(data.IdSIU);
                    $("#c_idSiuHidden").val(data.IdSIU);
                    $("#c_idPersonaHidden").val(data.PersonaId);
                }
            });
        },

        save: function () {

            var model = {
                PK1: idComunicado,
                CveSede: $('#Sedes').val(),
                Periodo: $("#Periodo").val(),
                PersonaId: $("#c_idPersonaHidden").val(),
                EsquemaId: $("#Esquema").val(),
                ConceptoPagoPk: $("#ConceptoPago").val(),
                //FechaPago: $("#FechaPago").val(),
                Mensaje: $("#Mensaje").val(),
            }

            //if (!formValidation.Validate()) {
            //    $('.nav-tabs a[href="#default-tab-1"]').tab('show');
            //    return;
            //}

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Comunicado/Editar/",
                data: model,
                success: function (data) {
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        delete: function (confirm) {

            if (!confirm) {
                $('#modal-delete-comunicado').modal("show");
                return;
            }

            $('#modal-delete-comunicado').modal("hide");

            var model = {
                PK1: idComunicado
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Comunicado/Eliminar/",
                data: model,
                success: function (data) {
                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },
    }
}();

var DataTable = function () {
    var pag = 1;
    var order = "PK1";
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

        //exportExcel: function (table) {
        //    var sedes = $('#Sedes').val();
        //    window.location.href = '/Esquemas/ExportExcel?sedes=' + sedes;
        //},

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

            formPage.clean();

            loading('loading-bar');
            loading('loading-circle', '#datatable_comunicado', 'Consultando datos..');

            if (search === undefined) search = "";

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Comunicado/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&filter=" + filter,
                success: function (msg) {
                    $('.loader-min-bar').hide();
                    $("#datatable_comunicado").html(msg);
                }
            });
        }
    }
}();