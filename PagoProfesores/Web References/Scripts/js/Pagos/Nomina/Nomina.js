
var ciclo1 = new Ciclos('Ciclo');
var periodo1 = new Periodos('Periodo');
var escuelas1 = new Escuelas('Escuela');
var tipospago1 = new TiposdePagos('TipoPago');
var esquemaPago1 = new Esquema('Esquema');
var partePeriodo1 = new PartePeriodo('PartePeriodo');

var escuelas2 = new Escuelas('EscuelaSearch');
var comboCentroCostos1 = new CentrosCostos("centroCostos");
var programas1 = new Programas2('Programa');

var tabulador1 = new Tabulador('Tabulador');

$(function () {
    $(document).ready(function () {
        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 4
        });
    });

    $(window).load(function () {

        $("#frm-genera").hide();
        $("#frm-grabar").hide();

        $("#formbtnasignar").show();//boton modal esquemas
        $("#formbtnasignarCentroC").hide();//boton modal centro costos
        $("#formbtnGenerarCentroC").hide();
        $("#formbtnaCalcularNomina").hide();//boton modal centro costos

        $('#btn-sgt-costos').show();//boton siguiente costos        

        ciclo1.init("ciclo1");
        escuelas1.init("escuelas1");
        tipospago1.init("tipospago1");
        escuelas2.init("escuelas2");
        partePeriodo1.init("partePeriodo1");
        //tabulador1.init("tabulador1");

        setTimeout(function () {
            if (gup('N') != "") {
                $("#tab-esquema").removeClass("active").addClass("");
                formPage.BotonSgtCalcularNomina();
            }
            formPage.Consultar();
        }, 1500);

       /* formValidation.Inputs(["Ciclo", "Periodo", "CampusPA"]);
        formValidation.notEmpty('Ciclo', 'El campo Año escolar no debe de estar vacio');
        formValidation.notEmpty('Periodo', 'El campo periodos no debe de estar vacio');
        formValidation.notEmpty('CampusPA', 'El campo campus no debe de estar vacio');*/
    });

});//End function jquery

function handlerdataSedes() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/Nomina/getCampusPA",
        success: function (msg) {
            $('#CampusPA').html(msg);
        },
        error: function (data) {
            session_error(data);
        }
    });
}

function handlerdataCiclos() {
    $('#Ciclo').val(gup('Ciclo'));
    periodo1.id_ciclo = $("#Ciclo").val();
    periodo1.init("periodo1");
}

function handlerdataPeriodos() {
    $('#Periodo').val(gup('Periodo'));
}

function handlerdataNiveles() {
    $('#Nivel').val(gup('Nivel'));
}

function handlerdataEscuelas() {
    $('#Escuela').val(gup('Escuela'));
    $('#CampusPA').val(gup('CampusPA'));
}

function handlerdataTiposPagos() {
    $('#TipoPago').val(gup('TipoPago'));
}

function handlerdataPartePeriodo() {
    if ((gup('PartePeriodo') != "")) { $('#PartePeriodo').val(gup('PartePeriodo')); }
}

$('#Periodo').on('change', function () {
    $('#Esquema').html("");
    $("#Esquema").val('');
    $("#nosemanas").val('');
    $("#nospagos").val('');
    $("#fechai").val('');
    $("#fechaf").val('');

    if ($('#Periodo').val() == "") {
        return;
    }
});

$('#Esquema').on('change', function () {

    if ($('#Esquema').val() == "") {
        $("#nosemanas").val('');
        $("#nopagos").val('');
        $("#fechai").val('');
        $("#fechaf").val('');
        return;
    }

    var model = {
        IdEsquema: this.value,
        CampusVPDI: $('#Sedes').val()
    }

    $.ajax({
        type: "POST",
        dataType: 'json',
        cache: false,
        url: "/Nomina/getDetalleEsquemaPago/",
        data: model,
        success: function (data) {
            data = jQuery.parseJSON(data);

            $("#fechai").val(data.StrmFechaI);
            $("#fechaf").val(data.StrmFechaF);
            $("#nosemanas").val(data.NoSemanas);
            $("#nopagos").val(data.NoPagos);
        }
    });
});

$('#EscuelaSearch').on('change', function () { 
    comboCentroCostos1.CampusVPDI = $('#Sedes').val();
    comboCentroCostos1.EscuelaCVE = this.value;  
    comboCentroCostos1.init();
    programas1.Escuela = this.value;
    programas1.init();
});

$('#Ciclo').on('change', function () {
    periodo1.id_ciclo = this.value;
    periodo1.init("periodo1");
});

var formPage = function () {
    var id;

    "use strict"; return {
        clean: function () {
            //Consultar
            $('#Ciclo').val('');
            $('#Periodo').val('');
            //$('#Nivel').val('');
            $('#Escuela').val('');
            $('#TipoPago').val('');
            $('#CampusPA').val('');

            //Generar esquema de pago
            $('#Esquema').val('');
            $('#Esquema').html('');
            $('#nosemanas').val('');
            $('#nospagos').val('');
            $('#fechai').val('');
            $('#fechaf').val('');

            //Cambio de tabulador
            $('#Tabulador').val('');
            $('#Tabulador').html('');

            //Grabar centro de costos
            $('#nrc').val('');
            $('#materia').val('');
            $('#profesor').val('');
            $('#centroCostos').val('');
            $('#centroCostos').html('');
            $('#perfilNomina').html('');

            $("#frm-genera").hide();
            $("#frm-grabar").hide();

            $("#formbtnasignar").hide();
            $("#formbtnasignarCentroC").hide();
            $("#formbtnGenerarCentroC").hide();
            $("#formbtnaCalcularNomina").hide();//boton modal centro costos
            DataTable.setPage(1);
        },

        Consultar: function () {

            $("#frm-listadoNomina").show();
            $("#frm-genera").show();

            esquemaPago1.Periodo = $("#Periodo").val();
            esquemaPago1.Sede = $('#Sedes').val();

            esquemaPago1.init("esquemaPago1");
            DataTable.init();
        },

        ModalAsignarEsquema: function () {

            esquemaPago1.Periodo = $("#Periodo").val();
            esquemaPago1.Sede = $('#Sedes').val();
            esquemaPago1.init("esquemaPago1");
          
            $('#myModal').modal('show');
        },

        ModalCambiarTabulador: function () {
            tabulador1.Sede = $('#Sedes').val();
            tabulador1.init("tabulador1");

            $('#myModalTabulador').modal('show');
        },

        //BOTON DE ANRERIOR Y SIGUIENTE WIZARD
        //en Esquema de pagos

        BotonAntPA: function () {

            var datos = 'Periodo=' + $('#Periodo').val()
                + '&PartePeriodo=' + $('#PartePeriodo').val()
                + '&Ciclo='        + $('#Ciclo').val()
                + '&TipoPago='     + $('#TipoPago').val()
                + '&Escuela='      + $('#Escuela').val()
                + '&Campus='       + $('#Sedes').val()
                + '&CampusPA='     + gup('CampusPA');

           window.location.href = '/PA?' + datos;
        },

        BotonSgtAsignarCostos: function () {
            $("#titulo-legend").html('<span>Seleccione la Programación Academica a la que desea asignar el Centro de Costos</span>');

            $("#formbtnasignarCentroC").show();
            $("#formbtnGenerarCentroC").show();
            $("#formbtnasignar").hide();//boton modal esquemas
            $("#formbtntabulador").hide();//boton modal tabulador
            $("#formbtnaCalcularNomina").hide();//boton modal centro costos

            $("#tab-esquema").removeClass("active").addClass("");
            $("#tab-costos").addClass("active");

            $("#linkant").attr("href", "javascript:formPage.BotonAntEsquema()");
            $("#linksgt").attr("href", "javascript:formPage.BotonSgtCalcularNomina()");
        },

        BotonAntEsquema: function () {//cambiar titulo a costos

            $("#titulo-legend").html('<span>Seleccione la Programación Academica a la que desea asignar el Esquema de Pago</span>');

            $("#formbtnasignarCentroC").hide();
            $("#formbtnGenerarCentroC").hide();
            $("#formbtnasignar").show();//boton modal esquemas
            $("#formbtntabulador").show();//boton modal tabulador
            $("#formbtnaCalcularNomina").hide();//boton modal centro costos

            $("#tab-costos").removeClass("active").addClass("");
            $("#tab-esquema").addClass("active");

            $("#linkant").attr("href", "javascript:formPage.BotonAntPA()");
            $("#linksgt").attr("href", "javascript:formPage.BotonSgtAsignarCostos()");
        },       

        BotonSgtCalcularNomina: function () {

            $("#titulo-legend").html('<span>Seleccione las Opciones para Calcular los pagos PA</span>');

            $("#formbtnasignarCentroC").hide();
            $("#formbtnGenerarCentroC").hide();
            $("#formbtnaCalcularNomina").show();//boton modal centro costos

            $("#tab-costos").removeClass("active").addClass("");
            $("#tab-calculo-nomina").addClass("active"); 

            $("#linkant").attr("href", "javascript:formPage.BotonAntNomina()");

            var datos = 'Periodo=' + $('#Periodo').val()
                     + '&PartePeriodo=' + $('#PartePeriodo').val()
                     + '&Ciclo=' + $('#Ciclo').val()
                     + '&TipoPago=' + $('#TipoPago').val()
                     + '&Escuela=' + $('#Escuela').val()
                     + '&Sedes=' + $('#Sedes').val()                  
                     + '&CampusPA=' + $('#CampusPA').val();

            $("#linksgt").attr("href", "/NominaXCDC?" + datos);
        },

        BotonAntNomina: function () {//*checar titulo

            $("#titulo-legend").html('<span>Seleccione la Programación Academica a la que desea asignar el Centro de Costos</span>');
         
            $("#btn-sgt-costos").show();

            $("#formbtnasignarCentroC").show();
            $("#formbtnGenerarCentroC").show();
            $("#formbtnasignar").hide();//boton modal esquemas          
            $("#formbtntabulador").hide();//boton modal tabulador
            $("#formbtnaCalcularNomina").hide();//boton modal centro costos

            $("#tab-calculo-nomina").removeClass("active").addClass("");
            $("#tab-costos").addClass("active"); 
            //$("#linkant").attr("href", "");

            $("#linkant").attr("href", "javascript:formPage.BotonAntEsquema()");
            $("#linksgt").attr("href", "javascript:formPage.BotonSgtCalcularNomina()");
        },

        insertaNomina: function () {

            var model = {
                Periodo: $('#Periodo').val(),
                PartePeriodo: $('#PartePeriodo').val(),
                Escuela: $('#Escuela').val(),
                OpcionPago: $('#TipoPago').val(),
                CampusVPDI: $('#Sedes').val(),
                CampusINB: $('#CampusPA').val(),
                IdEsquema: $('#Esquema').val(),
            }

            loading('loading-bar');
            loading('loading-circle', '#wizard', 'Generando nomina..');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/insertaNomina/",
                data: model,
                success: function (data) {
                    loading('stop', '#wizard', '');
                    $('#notification').html(data.msg);
                    DataTable.init();
                },
                error: function (data) {
                    loading('stop', '#wizard', '');
                    session_error(data);  
                }
            });
        },

        ModalAsignarCentroCostos: function () {

            $('#ccForced').html("");
            $('#formbtnAsignarCostos').show();
            $('#formbtnAsignarCostosSi').hide();
            $('#formbtnAsignarCostosNo').hide();

            comboCentroCostos1.CampusVPDI = $('#Sedes').val();
            comboCentroCostos1.EscuelaCVE = $('#EscuelaSearch').val();
            comboCentroCostos1.init();

            $('#myModalCentroCostos').modal('show');

            programas1.init();
        },

        getCentroCostos: function () {

            var model = {
                CampusVPDI: $('#Sedes').val(),
                Escuela: $('#EscuelaSearch').val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/getCentroCostos/",
                data: model,
                success: function (data) {

                    console.log(data);
                    data = jQuery.parseJSON(data);
                    
                    $("#centroCostos").html(data);
                },
                error: function (data) {
                    session_error(data);
                }
            });
        },

        asginaCentrodeCostos: function () {//boton asignar

            var arr = DataTable.checkboxs;
            var arrChecked = [];
            var existenCC = false;

            $('#ccForced').html("");

            for (var i = 0; i < arr.length; i++) {
                var checkbox_checked = $('#' + arr[i]).prop('checked');

                if (checkbox_checked == true)
                    arrChecked.push(arr[i]);
            }

            if (arrChecked.length == 0) {
                alert('Debes seleccionar una casilla');
                return;
            }
            
            var model = {
                CentroCostosID: $('#centroCostos').val(),
                StrmIdsPA: arrChecked.join(),
                Escuela: $('#EscuelaSearch').val(),
                CampusVPDI: $('#Sedes').val(),
                CentroCostosID: $('#centroCostos').val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/validaAsignacion/",
                data: model,

                success: function (data) {

                    var _html = "";

                    if (data.lPersonasCC.length >= 1) {
                        _html = "<div class='row'><span style='color:red;'>Los siguientes registros no coinciden con el Centro de Costo seleccionado.\n¿Desea continuar con la operación?</span><br /></div>";
                        _html += "<div class='row'>";
                        _html += "<table style='width:100%; border:0px;'>";
                        _html += "<tr><th>ID SIU</th>";
                        _html += "<th>Nombre</th>";
                        _html += "<th>Opción de pago</b></div>";
                        _html += "<th>NRC</th></tr>";

                        for (var i = 0; i <= data.lPersonasCC.length - 1; i++) {
                            _html += "<tr><td>" + data.lPersonasCC[i].ccIDSIU    + "</td>";
                            _html += "<td>"     + data.lPersonasCC[i].ccNombre   + "</td>";
                            _html += "<td>"     + data.lPersonasCC[i].ccTipoPago + "</td>";
                            _html += "<td>"     + data.lPersonasCC[i].ccNRC      + "</td></tr>";
                        }
                        _html += "</table>"

                        $('#formbtnAsignarCostos').hide();
                        $('#formbtnAsignarCostosSi').show();
                        $('#formbtnAsignarCostosNo').show();
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            dataType: 'json',
                            cache: false,
                            url: "/Nomina/asginaCentrodeCostos/",
                            data: model,
                            success: function (data) {

                                $('#notification').html(data.msg);
                                $('#myModalCentroCostos').modal('hide');

                                formPage.Consultar();
                                $("#formbtnasignar").hide();
                                $("#formbtnasignarCentroC").show();
                                $("#formbtnGenerarCentroC").show();
                            },
                            error: function (data) {
                                session_error(data);
                            }
                        });
                    }
                    $('#ccForced').html(_html);
                },
                error: function (data) {
                    session_error(data);
                }
            });
        },

        asginaCentrodeCostosSi: function () {//boton asignar

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

            var model = {
                CentroCostosID: $('#centroCostos').val(),
                StrmIdsPA: arrChecked.join(),
                Escuela: $('#EscuelaSearch').val(),
                CampusVPDI: $('#Sedes').val(),
                CentroCostosID: $('#centroCostos').val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/asginaCentrodeCostos/",
                data: model,
                success: function (data) {

                    $('#notification').html(data.msg);
                    $('#myModalCentroCostos').modal('hide');

                    formPage.Consultar();
                    $("#formbtnasignar").hide();
                    $("#formbtnasignarCentroC").show();
                    $("#formbtnGenerarCentroC").show();
                },
                error: function (data) {
                    session_error(data);
                }
            });
        },

        asginaCentrodeCostosNo: function () {//boton asignar

            var arr = DataTable.checkboxs;
            var arrChecked = [];
            var arrCheckedAux = [];
            var existenCC = false;

            for (var i = 0; i < arr.length; i++) {
                var checkbox_checked = $('#' + arr[i]).prop('checked');

                if (checkbox_checked == true)
                    arrChecked.push(arr[i]);
            }

            if (arrChecked.length == 0) {
                alert('Debes seleccionar una casilla');
                return;
            }

            var model = {
                CentroCostosID: $('#centroCostos').val(),
                StrmIdsPA: arrChecked.join(),
                Escuela: $('#EscuelaSearch').val(),
                CampusVPDI: $('#Sedes').val(),
                CentroCostosID: $('#centroCostos').val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/validaAsignacion/",
                data: model,

                success: function (data) {

                    var _html = "";

                    if (data.lPersonasCC.length >= 1) {

                        for (var i = 0; i <= data.lPersonasCC.length - 1; i++) {
                            arrChecked = jQuery.grep(arrChecked, function (value) { return value != data.lPersonasCC[i].ccIDPA });
                        }

                        var model = {
                            CentroCostosID: $('#centroCostos').val(),
                            StrmIdsPA: arrChecked.join(),
                            Escuela: $('#EscuelaSearch').val(),
                            CampusVPDI: $('#Sedes').val(),
                            CentroCostosID: $('#centroCostos').val(),
                        }

                        if (arrChecked.length > 0) {
                            $.ajax({
                                type: "POST",
                                dataType: 'json',
                                cache: false,
                                url: "/Nomina/asginaCentrodeCostos/",
                                data: model,
                                success: function (data) {

                                    $('#notification').html(data.msg);
                                    $('#myModalCentroCostos').modal('hide');

                                    formPage.Consultar();
                                    $("#formbtnasignar").hide();
                                    $("#formbtnasignarCentroC").show();
                                    $("#formbtnGenerarCentroC").show();
                                },
                                error: function (data) {
                                    session_error(data);
                                }
                            });
                        }
                        else {
                            $('#myModalCentroCostos').modal('hide');

                            formPage.Consultar();
                            $("#formbtnasignar").hide();
                            $("#formbtnasignarCentroC").show();
                            $("#formbtnGenerarCentroC").show();
                        }
                    }
                },
                error: function (data) {
                    session_error(data);
                }
            });
        },

        CerrarModalCostos: function () {          
            $('#myModalCentroCostos').modal('hide');
        },

        generar: function () {//boton asignar

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

            var model = {
                Periodo: $('#Periodo').val(),
                PartePeriodo: $('#PartePeriodo').val(),
                Escuela: $('#Escuela').val(),
                CampusVPDI: $('#Sedes').val(),
                CampusINB: $('#CampusPA').val(),
                IdEsquema: $('#Esquema').val(),
                StrmIdsPA: arrChecked.join(), 
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/asginaEsquemaPago/",
                data: model,
                success: function (data) {

                    $('#notification').html(data.msg);
                    $('#myModal').modal('hide');

                    $('#btn-sgt-costos').show();

                    DataTable.init();
                },
                error: function (data) {
                    session_error(data);
                }
            });
        },

        generarTodo: function () {//boton asignar
            var model = {
                Periodo: $('#Periodo').val(),
                PartePeriodo: $('#PartePeriodo').val(),
                Escuela: $('#Escuela').val(),
                CampusVPDI: $('#Sedes').val(),
                CampusINB: $('#CampusPA').val(),
                IdEsquema: $('#Esquema').val(),
                OpcionPago: $('#TipoPago').val()
            }

	        loading('loading-bar');
            loading('loading-circle', '#myModal', 'Actualizando esquema de pago..');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/asginaEsquemaPagoTodo/",
                data: model,
                success: function (data) {

                    $('#notification').html(data.msg);
                    $('#myModal').modal('hide');
                    loading('stop', '#myModal', '');

                    $('#btn-sgt-costos').show();

                    DataTable.init();
                },
                error: function (data) {
                    session_error(data);
                    loading('stop', '#myModal', '');
                }
            });
        },

        generarTabuldaor: function() {
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

            if ($('#Tabulador').val() == "") {
                alert('Debes seleccionar un tabulador.');
                return;
            }

            var model = { // De aqui habrá que despreciar algunos elementos/parámetros
                Tabulador: $('#Tabulador').val(),
                Periodo: $('#Periodo').val(),
                PartePeriodo: $('#PartePeriodo').val(),
                Escuela: $('#Escuela').val(),
                CampusVPDI: $('#Sedes').val(),
                CampusINB: $('#CampusPA').val(),
                IdEsquema: $('#Esquema').val(),
                TipoPago: $('#TipoPago').val(),
                StrmIdsPA: arrChecked.join(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/cambiaTabulador/",
                data: model,
                success: function (data) {

                    $('#notification').html(data.msg);
                    $('#myModalTabulador').modal('hide');

                    //$('#btn-sgt-costos').show();

                    DataTable.init();
                },
                error: function (data) {
                    session_error(data);
                }
            });
        },

        generarTodoTabulador: function () {

            if ($('#Tabulador').val() == "") {
                alert('Debes seleccionar un tabulador.');
                return;
            }

            var model = {
                Tabulador: $('#Tabulador').val(),
                Periodo: $('#Periodo').val(),
                PartePeriodo: $('#PartePeriodo').val(),
                Escuela: $('#Escuela').val(),
                CampusVPDI: $('#Sedes').val(),
                CampusINB: $('#CampusPA').val(),
                OpcionPago: $('#TipoPago').val()
            }

            loading('loading-bar');
            loading('loading-circle', '#myModalTabulador', 'Actualizando tabulador...');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/cambiaTabuladorTodo/",
                data: model,
                success: function (data) {

                    $('#notification').html(data.msg);
                    $('#myModalTabulador').modal('hide');
                    loading('stop', '#myModalTabulador', '');

                    //$('#btn-sgt-costos').show();

                    DataTable.init();
                },
                error: function (data) {
                    session_error(data);
                    loading('stop', '#myModalTabulador', '');
                }
            });
        },

        GenerarCC: function () {

            var model = {
                Periodo: $('#Periodo').val(),
                PartePeriodo: $('#PartePeriodo').val(),
                Escuela: $('#Escuela').val(),
                CampusVPDI: $('#Sedes').val(),
                CampusINB: $('#CampusPA').val(),
                OpcionPago: $('#TipoPago').val(),
            }

            loading('loading-bar');
            loading('loading-circle', '#wizard', 'Actualizando centro de costo..');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/generarCentroCostos/",

                data: model,
                success: function (data) {
                    loading('stop', '#wizard', '');
                    $('#notification').html(data.msg);
                    DataTable.setPage(1);
                },
                error: function (data) {
                    loading('stop', '#wizard', '');
                    session_error(data);
                }
            });
        },

        grabar: function () {

            var model = {
                IdPA: $('#PAID').val(),
                CentroCostosID: $('#centroCostos').val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/grabarCentroCostos/",
                data: model,
                success: function (data) {
                    DataTable.setPage(1);
                },
                error: function (data) {
                    session_error(data);
                }
            });
        },

        ocultar: function (frm) {

            $('#myModal').modal('hide');
            switch (frm) {
                case "genera":
                    $("#frm-genera").hide();
                    break;
                case "grabar":
                    $("#frm-grabar").hide();
            }
        },

        ocultarTabulador: function () {
            $('#myModalTabulador').modal('hide');
        },
    }
}();

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
        checkboxs: [],
        onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
                pag = 1;
                this.init();
            }
        },

        exportExcel: function (table) {

            var datos = 'Periodo=' + $('#Periodo').val()
              + '&PartePeriodo=' + $('#PartePeriodo').val()
              + '&TipoPago=' + $('#TipoPago').val()
              + '&Escuela=' + $('#Escuela').val()
              + '&Campus=' + $('#Sedes').val()
              + '&CampusPA=' + $('#CampusPA').val();


            window.location.href = '/Nomina/ExportExcel?' + datos;
        },

        edit: function (id) {
            formPage.edit(id);
        },

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
            
            var data = '&Periodo='      + $('#Periodo').val()
                     + '&PartePeriodo=' + $('#PartePeriodo').val()
                     + '&Escuela='      + $('#Escuela').val()
                     + '&OpcionPago='   + $('#TipoPago').val()
                     + '&CampusVPDI='   + $('#Sedes').val()
                     + '&CampusPA='     + $('#CampusPA').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Nomina/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + data,
                success: function (msg) {
                    $('.loader-min-bar').hide();

                    if (session_error(msg) == false) {
                        $("#datatable").html(msg);

                        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                            altClass: 'odd',
                            footer: true,
                            fixedColumns: 4
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