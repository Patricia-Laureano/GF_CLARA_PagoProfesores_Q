//consulta
var comboAnnyoEscolarC = new Ciclos('c_cicloEC');
var comboPeriodosC     = new Periodos('c_periodoEC');
var comboPartePeriodoC = new PartePeriodo('c_partePeriodoEC');
var comboTiposPagosC   = new TiposdePagos('c_tipoDePagoEC');
var comboEscuelasC     = new Escuelas('c_escuelaEC');
var comboCentroCostosC = new CentrosCostos('c_centroCostosEC');

//modifica
var comboAnnyoEscolarM       = new Ciclos('m_annyoEscolarEC');
var comboPeriodosM           = new Periodos('m_periodoEC');
var comboPartePeriodoM       = new PartePeriodo('m_partePeriodoEC');
var comboEsquemasPagosM      = new Esquema('m_esquemaPagoEC');
var comboTiposTransferenciaM = new TiposTrasnferencia('m_tipoDeTransferenciaEC');
var comboTiposPagosM         = new TiposdePagos("m_tipoDePagoEC");
//var comboCentroCostosM       = new CentrosCostosAll("m_cuentaContableEC");
var comboCentroCostosM       = new CentrosCostos("m_cuentaContableEC");
var comboConceptosPagoM      = new ConceptosdePago("m_conceptoDePagoEC");

var comboEsquemasPagosl = new Esquema('l_esquemaPagoEC');
var comboConceptosPagol = new ConceptosdePago("l_conceptoDePagoEC");
var comboFechasPagol = new FechasdePagoEsquema("l_fechaPagoEC");
var comboFechasPago2 = new FechasdePagoEsquema("m_fechaPagoEC");

//cambios edgar
var add_EC_anio1 = new Ciclos('add_EC_anio');
var add_EC_periodo1 = new Periodos('add_EC_periodo');
var add_EC_partePeriodo1 = new PartePeriodo('add_EC_partePeriodo');
var add_EC_esquema1 = new Esquema('add_EC_esquema');
var add_EC_concepto1 = new ConceptosdePago("add_EC_concepto");
var add_EC_fechaPago1 = new FechasdePagoEsquema("add_EC_fechaPago");

//cambios edgar2
var add_ECP_pensionado1 = new Pensionados('add_ECP_pensionado');
var add_ECP_anio1 = new Ciclos('add_ECP_anio');
var add_ECP_periodo1 = new Periodos('add_ECP_periodo');
var add_ECP_partePeriodo1 = new PartePeriodo('add_ECP_partePeriodo');
var add_ECP_esquema1 = new Esquema('add_ECP_esquema');
var add_ECP_concepto1 = new ConceptosdePago("add_ECP_concepto");
var add_ECP_fechaPago1 = new FechasdePagoEsquema("add_ECP_fechaPago");

//edgar 3
var l_anioEC1 = new Ciclos('l_anioEC');
var l_periodoEC1 = new Periodos('l_periodoEC');
var l_partePeriodoEC1 = new PartePeriodo('l_partePeriodoEC');
var edit_EC_partePeriodo1 = new PartePeriodo('edit_EC_partePeriodo');

//combos de estado de cuenta externo
var addExt_EC_anio1 = new Ciclos('addExt_EC_anio');
var addExt_EC_periodo1 = new Periodos('addExt_EC_periodo');
var addExt_EC_partePeriodo1 = new PartePeriodo('addExt_EC_partePeriodo');
var addExt_EC_esquema1 = new Esquema('addExt_EC_esquema');
var addExt_EC_escuela1 = new Escuelas('addExt_EC_escuela');

var escuelas2 = new Escuelas('EscuelaSearch');

$(function () {
    $("#edit_EC_fechasolicitado").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd',
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        }
    });

    $("#edit_EC_fecharecibo").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd',
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        }
    });

    $("#edit_EC_fechadispersion").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd',
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        }
    });

    $("#edit_EC_fechadeposito").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd',
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        }
    });

    $("#edit_EC_fechadePago").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd',
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        }
    });

    $("#fpago").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd',
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        }
    });

    $("#frecibo").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd',
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        }
    });

    $(document).ready(function () {
        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 3
        });
    });

    $(window).load(function () {
        $("#ConceptodePagos").hide();
        $("#divModificaEC").hide();

        formValidation.Inputs(["c_idSiuEC", "c_nombreCompletoEC"]);

        //Deshabilitar el botón de consulta
        $("#formbtnconsultar").prop("disabled", true);
        $("#formbtnmover").prop("disabled", true);
        $("#formbtneliminar").prop("disabled", true);

        comboAnnyoEscolarC.init("comboAnnyoEscolarC");
        comboPartePeriodoC.init("comboPartePeriodoC");
        comboTiposPagosC.init("comboTiposPagosC");
        comboEscuelasC.init("comboEscuelasC");
        comboAnnyoEscolarM.init("comboAnnyoEscolarM");
        comboPartePeriodoM.init("comboPartePeriodoM");
        comboCentroCostosM.EscuelaCVE = $("#EscuelaSearch").val();
        comboCentroCostosM.CampusVPDI = $("#Sedes").val();
        comboCentroCostosM.init("comboCentroCostosM");
        add_EC_anio1.init("add_EC_anio1");
        add_EC_partePeriodo1.init("add_EC_partePeriodo1");
        add_ECP_anio1.init("add_ECP_anio1");
        add_ECP_partePeriodo1.init("add_ECP_partePeriodo1");
        l_anioEC1.init("l_anioEC1");
        l_partePeriodoEC1.init("l_partePeriodoEC");
        edit_EC_partePeriodo1.init("edit_EC_partePeriodo");
        addExt_EC_anio1.init('addExt_EC_anio1');
        addExt_EC_partePeriodo1.init('addExt_EC_partePeriodo1');
        addExt_EC_escuela1.init('addExt_EC_escuela1');
        escuelas2.init("escuelas2");

        formPage.init();
        formPage.clean();

        $('#file_xml').fileupload({
            url: '/FileUploadHandler.ashx?upload=start',

        	add: function (e, data) {
        		var val = data.files[0].name.toLowerCase();
        		var regex = new RegExp("(.*?)\.(xml)$");
        		if (!(regex.test(val))) {
        			$(this).val('');
        			alert('Sólo archivos XML');
        		}
        		else data.submit();
            },

            progress: function (e, data) { },

        	success: function (response, status) {
        		$('#file_xml_name').val('' + this.files[0].name);
        		$('#file_xml_name').show();
        		$('#file_xml_link').hide();
        		formPage.fileNameXML = response;
				//$("#formbtnSubirXML").prop("disabled", false);
				formPage.botonSubir(true, false);
            },

        	error: function (error) { }
        });

        $('#file_pdf').fileupload({
        	url: '/FileUploadHandler.ashx?upload=start',
        	add: function (e, data) {
        		var val = data.files[0].name.toLowerCase();
        		var regex = new RegExp("(.*?)\.(pdf)$");
        		if (!(regex.test(val))) {
        			$(this).val('');
        			alert('Sólo archivos PDF');
        		}
        		else data.submit();
        	},
        	progress: function (e, data) { },
        	success: function (response, status) {
        		$('#file_pdf_name').val('' + this.files[0].name);
        		$('#file_pdf_name').show();
        		$('#file_pdf_link').hide();
        		formPage.fileNamePDF = response;
        		//$("#formbtnSubirXML").prop("disabled", false);
				formPage.botonSubir(true, false);
        	},
        	error: function (error) { }
        });
    });
});//End function jquery

function handlerdataSedes() {//cambio
    $.ajax({
        type: "GET",
        cache: false,
        url: "/EstadodeCuenta/getCampusPA",
        success: function (msg) {
            $('#addExt_EC_campusPA').html(msg);
        },
        error: function (data) {
            session_error(data);
        }
    });

    $('#addExt_EC_periodo').html("<option><option>");
    $('#addExt_EC_esquema').html("<option><option>");
    addExt_EC_anio1.init('addExt_EC_anio1');
}

function handlerdataCiclos() {
}

function handlerdataPartePeriodo() {
}

function handlerdataNiveles() {
}

function handlerdataPeriodos() {
}

function handlerdataEscuelas() {
}

function handlerdataTiposPagos() {
}

function seleccionarPadre(e) {

    var model = {
        IdEdoCtaD: e.value,
        IdSIU: $("#c_idSiuEC").val(),
        IdEdoCta: e.name,
        CveSede: $("#Sedes").val()
    }

    $.ajax({
        type: "POST",
        dataType: 'json',
        cache: false,
        url: "/EstadodeCuenta/seleccionarPadre/",
        data: model,
        success: function (data) {
            if (data.msg) {
                $("#P_" + e.name).prop("checked", true);
            }
        }
    });
}

$('#l_anioEC').on('change', function () {

    $('#l_esquemaPagoEC').html("<option><option>");
    $('#l_conceptoDePagoEC').html("<option><option>");
    $('#l_fechaPagoEC').html("<option><option>");

    if (this.value == "") {//ciclo(anio)
      $('#l_periodoEC').html("<option><option>");
    } else {
        l_periodoEC1.id_ciclo = this.value;
        l_periodoEC1.init("l_periodoEC1");
    }
});

$('#l_periodoEC').on('change', function () {
    //$('#l_conceptoDePagoEC').html("<option><option>");
    //$('#l_fechaPagoEC').html("<option><option>");

    if (this.value == "") {//periodo
        $('#l_esquemaPagoEC').html("<option><option>");
    } else {
        comboEsquemasPagosl.Sede = $('#Sedes').val();
        comboEsquemasPagosl.Periodo = this.value;
        comboEsquemasPagosl.init();
    }
});

$('#l_esquemaPagoEC').on('change', function () {

    $('#l_fechaPagoEC').html("<option><option>");

    if (this.value == "") {//esquema
        $('#l_conceptoDePagoEC').html("<option><option>");
    } else {
        comboConceptosPagol.EsquemaID = this.value;
        comboConceptosPagol.PersonaID = $("#c_idSiuECHidden").val();
        comboConceptosPagol.init();
    }
});

$('#l_conceptoDePagoEC').on('change', function () {
    comboFechasPagol.EsquemaID = $('#l_esquemaPagoEC').val();
    comboFechasPagol.PersonaID = $("#c_idSiuECHidden").val();
    comboFechasPagol.init();
});

$('#l_fechaPagoEC').on('change', function () {
    $("#l_conceptoDePagoEC option[value='" + this.value + "']").attr("selected", "selected");
});

$('#c_cicloEC').on('change', function () {
    $('#l_esquemaPagoEC').html("<option><option>");
    $('#l_conceptoDePagoEC').html("<option><option>");
    $('#l_fechaPagoEC').html("<option><option>");

    if (this.value == "") {//ciclo(anio)
        $('#l_periodoEC').html("<option><option>");
    } else { }
    comboPeriodosC.id_ciclo = this.value;
    comboPeriodosC.init("comboPeriodosC");

});

$('#c_periodoEC').on('change', function () {

    //$('#l_conceptoDePagoEC').html("<option><option>");
    //$('#l_fechaPagoEC').html("<option><option>");

    //l_periodoEC1.Periodo = this.value;
    //l_periodoEC1.id_ciclo = $('#c_cicloEC').val();
    //l_periodoEC1.init("l_periodoEC1");

    if (this.value == "") {//periodo
        //$('#l_esquemaPagoEC').html("<option><option>");
    } else {
        //comboEsquemasPagosl.Sede = $('#Sedes').val();
        //comboEsquemasPagosl.Periodo = this.value;
        //comboEsquemasPagosl.Nivel = $('#c_nivelEC').val();
        //comboEsquemasPagosl.init();
    }
});

$('#add_EC_anio').on('change', function () {
    $('#add_EC_esquema').html("<option><option>");
    $('#add_EC_concepto').html("<option><option>");
    $('#add_EC_fechaPago').html("<option><option>");

   if (this.value == "") {//ciclo(anio)
        $('#add_EC_periodo').html("<option><option>");
    } else {
        add_EC_periodo1.id_ciclo = this.value;
        add_EC_periodo1.init("add_EC_periodo1");
   }
});

$('#add_EC_periodo').on('change', function () {
    $('#add_EC_concepto').html("<option><option>");
    $('#add_EC_fechaPago').html("<option><option>");

    if (this.value == "") {//periodo
        $('#add_EC_esquema').html("<option><option>");
    } else {
        add_EC_esquema1.Sede = $('#Sedes').val();
        add_EC_esquema1.Periodo = this.value;
        add_EC_esquema1.Nivel = $('#add_EC_nivel').val();
        add_EC_esquema1.init();
    }
});

$('#add_EC_esquema').on('change', function () {
    $('#add_EC_fechaPago').html("<option><option>");

    if (this.value == "") {//esquema
        $('#add_EC_concepto').html("<option><option>");
    } else {
        add_EC_concepto1.EsquemaID = this.value;
        add_EC_concepto1.PersonaID = $("#c_idSiuECHidden").val();
        add_EC_concepto1.init();
    }
});

$('#add_EC_concepto').on('change', function () {

    if (this.value == "nuevoConcepto") {
        $("#ConceptodePagos").show();
        $("#agregarCPModal").prop("disabled", true);
        $("#add_EC_fechaPago_div").hide();
    }
    else {
        $("#ConceptodePagos").hide();
        $("#add_EC_fechaPago_div").show();
        $("#agregarCPModal").prop("disabled", false);
    }

    add_EC_fechaPago1.EsquemaID = $('#add_EC_esquema').val();
    add_EC_fechaPago1.PersonaID = $("#c_idSiuECHidden").val();
    add_EC_fechaPago1.init();
});

$('#add_EC_fechaPago').on('change', function () {
    $("#add_EC_concepto option[value='" + this.value + "']").attr("selected", "selected");
});

function handlerdataFechasdepagoEsquema() {
    $("#add_EC_fechaPago option[value='" + $('#add_EC_concepto').val() + "']").attr("selected", "selected");
    $("#add_ECP_fechaPago option[value='" + $('#add_ECP_concepto').val() + "']").attr("selected", "selected");
    $("#l_fechaPagoEC option[value='" + $('#l_conceptoDePagoEC').val() + "']").attr("selected", "selected");
    if ($("#isConceptoPH").val() != "" && $("#isConceptoPH").val() != null)
        $("#m_fechaPagoEC option[value='" + $('#m_conceptoDePagoEC').val() + "']").attr("selected", "selected");

    $("#isConceptoPH").val("");
}

function handlerdatagetConceptosdePago() {

    $("#add_EC_concepto option:eq(1)").before("<option value=\"nuevoConcepto\">Nuevo(+)</option>\n");
    $("#add_EC_concepto option[value='" + $('#idPagosF').val() + "']").attr("selected", "selected");

    if ($("#isEsquemaPH").val() != "" && $("#isEsquemaPH").val() != null)
            $('#m_conceptoDePagoEC option:selected').removeAttr("selected");

    $("#isEsquemaPH").val("");

}

$('#add_ECP_anio').on('change', function () {
    $('#add_ECP_esquema').html("<option><option>");
    $('#add_ECP_concepto').html("<option><option>");
    $('#add_ECP_fechaPago').html("<option><option>");

    if (this.value == "") {//ciclo(anio) Pension
        $('#add_ECP_periodo').html("");
    } else {
         add_ECP_periodo1.id_ciclo = this.value;
         add_ECP_periodo1.init("add_ECP_periodo1");
    }
});

$('#add_ECP_periodo').on('change', function () {
    $('#add_ECP_concepto').html("<option><option>");
    $('#add_ECP_fechaPago').html("<option><option>");

    if (this.value == "") {//periodo Pension
        $('#add_ECP_esquema').html("<option><option>");
    } else {
        add_ECP_esquema1.Sede = $('#Sedes').val();
        add_ECP_esquema1.Periodo = this.value;
        add_ECP_esquema1.Nivel = $('#add_ECP_nivel').val();
        add_ECP_esquema1.init();
    }

});

$('#add_ECP_esquema').on('change', function () {

    $('#add_ECP_fechaPago').html("<option><option>");

    if (this.value == "") {//esquema Pensionados
        $('#add_ECP_concepto').html("<option><option>");
    } else {
        add_ECP_concepto1.EsquemaID = this.value;
        add_ECP_concepto1.PersonaID = $("#c_idSiuECHidden").val();
        add_ECP_concepto1.init();
    }
});

$('#add_ECP_concepto').on('change', function () {

    add_ECP_fechaPago1.EsquemaID = $('#add_ECP_esquema').val();
    add_ECP_fechaPago1.PersonaID = $("#c_idSiuECHidden").val();
    add_ECP_fechaPago1.init();

});

$('#add_ECP_fechaPago').on('change', function () {
    $("#add_ECP_concepto option[value='" + this.value + "']").attr("selected", "selected");
});

$('#m_annyoEscolarEC').on('change', function () {

    $('#m_esquemaPagoEC').html("<option><option>");
    $('#m_conceptoDePagoEC').html("<option><option>");
    $('#m_fechaPagoEC').html("<option><option>");

    if (this.value == "") {//ciclo(anio)
        $('#m_periodoEC').html("<option><option>");
    } else {
        comboPeriodosM.Periodo = "";
        comboPeriodosM.id_ciclo = this.value;
        comboPeriodosM.init("comboPeriodosM");
    }
});

$('#m_periodoEC').on('change', function () {

    $('#m_conceptoDePagoEC').html("<option><option>");
    $('#m_fechaPagoEC').html("<option><option>");

    if (this.value == "") {//periodo
        $('#m_esquemaPagoEC').html("<option><option>");
    } else {
        comboEsquemasPagosM.IdEsquema = "";
        comboEsquemasPagosM.Sede = $('#Sedes').val();
        comboEsquemasPagosM.Periodo = this.value;
        comboEsquemasPagosM.Nivel = $('#m_nivelEC').val();
        comboEsquemasPagosM.init();
    }

});

$('#m_esquemaPagoEC').on('change', function () {
    $('#m_fechaPagoEC').html("<option><option>");
    $('#isEsquemaPH').val("1");

    if (this.value == "") {
        $('#m_conceptoDePagoEC').html("<option><option>");
    }
     else {
        comboConceptosPagoM.EsquemaID = this.value;
        comboConceptosPagoM.init();
    }
});

$('#m_conceptoDePagoEC').on('change', function () {

    $("#isConceptoPH").val("1");

    comboFechasPago2.EsquemaID = $('#m_esquemaPagoEC').val();
    comboFechasPago2.init();
});

$('#m_fechaPagoEC').on('change', function () {
    $("#m_conceptoDePagoEC option[value='" + this.value + "']").attr("selected", "selected");
});

$('#addExt_EC_anio').on('change', function () {

    $('#addExt_EC_esquema').html("<option><option>");

    if (this.value == "") {
        $('#addExt_EC_periodo').html("");
    } else {
        addExt_EC_periodo1.id_ciclo = this.value;
        addExt_EC_periodo1.init("addExt_EC_periodo1");
    }
});

$('#addExt_EC_periodo').on('change', function () {

    if (this.value == "") {//periodo Pension
        $('#addExt_EC_esquema').html("<option><option>");
    } else {

        addExt_EC_esquema1.Sede = $('#Sedes').val();
        addExt_EC_esquema1.Periodo = this.value;
        addExt_EC_esquema1.init('addExt_EC_esquema1');
    }
});

function _toggleTR(idHijo, _trId, displayed) {
	var counter = parseInt($('#' + idHijo).data('count'));
	counter = counter + (displayed ? 1 : -1);
	$('#' + idHijo).data('count', '' + counter);

	if (console) console.log('counter=' + parseInt($('#' + idHijo).data('count')));

	if (counter == 0) {
		$("#" + idHijo).show();
		$("#icon_" + _trId).removeClass("fa fa-plus-circle").addClass("fa fa-minus-circle");
	}
	else {
		$("#" + idHijo).hide();
		$("#icon_" + _trId).removeClass("fa fa-minus-circle").addClass("fa fa-plus-circle");
	}
}

function _toggleEstadoCuentaDetalle(sender) {
	var trId = sender.id;
	var _trId = trId.substr(trId.indexOf("_") + 1, trId.length);

	//             td  /     tr       /     tbody
	var parent = sender.parentElement.parentElement;
	var arregloTR = [];
	for (var i = 0; i < parent.childNodes.length; i++)
		if (parent.childNodes[i].dataset != null && "1" == parent.childNodes[i].dataset['row'])
			arregloTR.push(parent.childNodes[i]);

	// conmutar desplegar //
	var displayed = $('#' + trId).data('activo') == "1";
	displayed = !displayed; // _toggle
	if (displayed)
		$('#' + trId).data('activo', '1'); // cambiar a visible
	else
		$('#' + trId).data('activo', '0'); // cambiar a oculto

	//var arregloTR = document.getElementsByTagName("tr");
	var arrayChecked = [""];
    var idHijo = '';
    var tr;
    var ck;
    var cont = 1;
    var contX = 0;

    var longitud = 0;
	/*
    for (tr in arregloTR) {
        if (arregloTR[tr] != undefined) {
            idHijo = arregloTR[tr].id;
            if (idHijo != undefined) {
                arrayChecked.push(idHijo);
            }
        }
    }

    longitud = arregloTR.length;
	*/
    for (var tr = 0; tr < arregloTR.length; tr++) {
    	if (arregloTR[tr] != undefined) {
    		idHijo = arregloTR[tr].id;
    		if (idHijo != undefined) {
    			arrayChecked.push(idHijo);
    		}
    	}
    }

    for (var tr = 0; tr < arregloTR.length; tr++){
    /*for (tr in arregloTR) {

        if (contX > longitud) {
            break;
        }*/
    	//if (arregloTR[tr] != undefined)
    	{
            idHijo = arregloTR[tr].id;
            if (idHijo != undefined) {
                if (idHijo.substr(0, 4) == 'Hijo') {

                    for (ck in arrayChecked) {
                        if (idHijo == arrayChecked[ck]) {
                            cont = cont + 1;
                            if (cont > 1) {
                                break;
                            }

                        }
                        //arrayChecked.push(idHijo);
                    }

                    if (cont > 1) {
                    	if (idHijo.slice(idHijo.indexOf("_") + 1, idHijo.lastIndexOf("_")) == _trId) {
                    		_toggleTR(idHijo, _trId, displayed);
                    		/*
                        	if ($("#" + idHijo).is(':hidden')) {
                                $("#" + idHijo).show();
                                $("#icon_" + _trId).removeClass("fa fa-plus-circle").addClass("fa fa-minus-circle");
                            } else {
                                $("#" + idHijo).hide();
                                $("#icon_" + _trId).removeClass("fa fa-minus-circle").addClass("fa fa-plus-circle");
                            }
                        	//*/

                            cont = 1;
                        }
                    }
                }
                else if (idHijo.substr(0, 10) == 'SUBPadreTR') {

                    for (ck in arrayChecked) {
                        if (idHijo == arrayChecked[ck]) {
                            cont = cont + 1;
                            if (cont > 1) {
                                //arrayChecked.push(idHijo);
                                break;
                            }
                            //arrayChecked.push(idHijo);
                        }

                    }

                    if (cont > 1) {
                    	if (idHijo.slice(idHijo.indexOf("_") + 1, idHijo.indexOf("-")) == _trId) {
                    		_toggleTR(idHijo, _trId, displayed);
							/*
                            if ($("#" + idHijo).is(':hidden')) {
                                $("#" + idHijo).show();
                                $("#icon_" + _trId).removeClass("fa fa-plus-circle").addClass("fa fa-minus-circle");
                            } else {
                                $("#" + idHijo).hide();
                                $("#icon_" + _trId).removeClass("fa fa-minus-circle").addClass("fa fa-plus-circle");
                            }
                        	//*/

                            cont = 1;
                        }
                    }
                }
                else if (idHijo.substr(0, 7) == 'SUBHijo') {

                    for (ck in arrayChecked) {
                        if (idHijo == arrayChecked[ck]) {
                            cont = cont + 1;
                            if (cont > 1) {
                                //arrayChecked.push(idHijo);
                                break;
                            }
                            //arrayChecked.push(idHijo);
                        }

                    }

                    if (cont > 1) {
                    	if (idHijo.slice(idHijo.indexOf("_") + 1, idHijo.indexOf("-")) == _trId) {
                    		_toggleTR(idHijo, _trId, displayed);
							/*
                            if ($("#" + idHijo).is(':hidden')) {
                                $("#" + idHijo).show();
                                $("#icon_" + _trId).removeClass("fa fa-plus-circle").addClass("fa fa-minus-circle");
                            } else {
                                $("#" + idHijo).hide();
                                $("#icon_" + _trId).removeClass("fa fa-minus-circle").addClass("fa fa-plus-circle");
                            }
							//*/
                            cont = 1;
                        }
                    }
                }

                else if (idHijo.substr(0, 9) == 'PensionTR') {

                    for (ck in arrayChecked) {
                        if (idHijo == arrayChecked[ck]) {
                            cont = cont + 1;
                            if (cont > 1) {
                                //arrayChecked.push(idHijo);
                                break;
                            }

                        }
                        //arrayChecked.push(idHijo);
                    }

                    if (cont > 1) {
                    	if (idHijo.slice(idHijo.indexOf("_") + 1, idHijo.indexOf("-")) == _trId) {
                    		_toggleTR(idHijo, _trId, displayed);
							/*
                            if ($("#" + idHijo).is(':hidden')) {
                                $("#" + idHijo).show();
                                $("#icon_" + _trId).removeClass("fa fa-plus-circle").addClass("fa fa-minus-circle");
                            } else {
                                $("#" + idHijo).hide();
                                $("#icon_" + _trId).removeClass("fa fa-minus-circle").addClass("fa fa-plus-circle");
                            }
							//*/
                            cont = 1;
                        }
                    }
                }

                else if (idHijo.substr(0, 12) == 'SUBPensionTR') {

                    for (ck in arrayChecked) {
                        if (idHijo == arrayChecked[ck]) {
                            cont = cont + 1;
                            if (cont > 1) {
                                //arrayChecked.push(idHijo);
                                break;
                            }
                            //arrayChecked.push(idHijo);
                        }

                    }

                    if (cont > 1) {
                    	if (idHijo.slice(idHijo.indexOf("_") + 1, idHijo.indexOf("-")) == _trId) {
                    		_toggleTR(idHijo, _trId, displayed);
                    		/*
                            if ($("#" + idHijo).is(':hidden')) {
                                $("#" + idHijo).show();
                                $("#icon_" + _trId).removeClass("fa fa-plus-circle").addClass("fa fa-minus-circle");
                            } else {
                                $("#" + idHijo).hide();
                                $("#icon_" + _trId).removeClass("fa fa-minus-circle").addClass("fa fa-plus-circle");
                            }
							//*/
                            cont = 1;
                        }
                    }
                }

            }
        }
        contX = contX + 1;
        //arrayChecked = [""];
    }
}

function _toggleEstadoCuentaDetalle2(sender) {
	var trId = sender.id;
	var _trId = trId.substr(trId.indexOf("-") + 1, trId.length);

	//             td  /     tr       /     tbody
	var parent = sender.parentElement.parentElement;
	var arregloTR = [];
	for (var i = 0; i < parent.childNodes.length; i++)
		if (parent.childNodes[i].dataset != null && "1" == parent.childNodes[i].dataset['row'])
			arregloTR.push(parent.childNodes[i]);
	// conmutar desplegar //
	var displayed = $('#' + trId).data('activo') == "1";
	displayed = !displayed; // _toggle
	if (displayed)
		$('#' + trId).data('activo', '1'); // cambiar a visible
	else
		$('#' + trId).data('activo', '0'); // cambiar a oculto

	//var arregloTR = document.getElementsByTagName("tr");
    var arrayChecked = [""];
    var idHijo = '';
    var tr;
    var ck;
    var cont = 1;

    //for (tr in arregloTR) {
    for (var tr = 0; tr < arregloTR.length; tr++) {
    	if (arregloTR[tr] != undefined) {
    		idHijo = arregloTR[tr].id;
    		if (idHijo != undefined) {
    			arrayChecked.push(idHijo);
    		}
    	}
    }

    for (var tr = 0; tr < arregloTR.length; tr++) {

    	//if (arregloTR[tr] != undefined)
    	{
            idHijo = arregloTR[tr].id;
            if (idHijo != undefined) {
                if (idHijo.substr(0, 7) == 'SUBHijo') {

                    for (ck in arrayChecked) {
                        if (idHijo == arrayChecked[ck]) {
                            cont = cont + 1;
                            if (cont > 1)
                                break;
                        }
                        arrayChecked.push(idHijo);
                    }

                    if (cont > 1) {
                    	if (idHijo.slice(idHijo.indexOf("-") + 1, idHijo.lastIndexOf("-")) == _trId) {
                    		_toggleTR(idHijo, _trId, displayed);
							/*
                    		if ($("#" + idHijo).is(':hidden')) {
                                $("#" + idHijo).show();
                                $("#icon_" + _trId).removeClass("fa fa-plus-circle").addClass("fa fa-minus-circle");
                            } else {
                                $("#" + idHijo).hide();
                                $("#icon_" + _trId).removeClass("fa fa-minus-circle").addClass("fa fa-plus-circle");
                    		}
							//*/
                        }
                    }
                }

                if (idHijo.substr(0, 12) == 'SUBPensionTR') {

                    for (ck in arrayChecked) {
                        if (idHijo == arrayChecked[ck]) {
                            cont = cont + 1;
                            if (cont > 1)
                                break;
                        }
                        arrayChecked.push(idHijo);
                    }

                    if (cont > 1) {
                    	if (idHijo.slice(idHijo.indexOf("-") + 1, idHijo.lastIndexOf("-")) == _trId) {
                    		_toggleTR(idHijo, _trId, displayed);
							/*
                            if ($("#" + idHijo).is(':hidden')) {
                                $("#" + idHijo).show();
                                $("#icon_" + _trId).removeClass("fa fa-plus-circle").addClass("fa fa-minus-circle");
                            } else {
                                $("#" + idHijo).hide();
                                $("#icon_" + _trId).removeClass("fa fa-minus-circle").addClass("fa fa-plus-circle");
                            }
							//*/
                        }
                    }
                }
            }
        }
    }
}

$('#c_escuelaEC').on('change', function () {
    comboCentroCostosC.CampusVPDI = $('#Sedes').val();
    comboCentroCostosC.EscuelaCVE = this.value;;
    comboCentroCostosC.TipoPagoCVE = $('#c_tipoDePagoEC').val();
    comboCentroCostosC.init();
});

$('#c_idSiuEC').keypress(function (e) {


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
                $("#c_nombreCompletoEC").val(((data.Nombres == 'null' || data.Nombres == null) ? '' : data.Nombres) + ' ' + ((data.Apellidos == 'null' || data.Apellidos == null) ? '' : data.Apellidos));

                $("#c_idSiuECHidden").val(data.IdPersona);
                $("#c_origenEC").val(data.origen);
                $("#c_tipoFacturaEC").val(data.cveFactura);

                $("#hdfCorreo0365").val(data.correoO365);

                $("#formbtnconsultar").prop("disabled", false);

                formPage.consultar();
            },
            error: function (msg) {
                session_error(msg);
                $('#notification').html(msg);
            }
        });
    }
});

$('#m_importeEC').focusout(function (e) {   // Se ha quedado en standBy 24/01/2017 =)
    //alert("Habia una vez una piña colada, pero la sacaron de la fiesta");
    importesCalc.recalcular();
    //importesCalc.consultar();
});

$('#m_importeEC').keypress(function (e) {   // Se ha quedado en standBy 24/01/2017 =)
    var enterKey = 13;
    if (e.which == enterKey) {
        importesCalc.recalcular();
    }
});

$('#EscuelaSearch').on('change', function () {
    comboCentroCostosM.CampusVPDI = $('#Sedes').val();
    comboCentroCostosM.EscuelaCVE = this.value;
    comboCentroCostosM.init();
});

function round(value, decimals) {
    return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
}

var importesCalc = function() {
    "use strict";
    return {
        recalcular: function() {
            var model = {
                Monto: $("#m_importeEC").val(),
                TipoPago: $("#c_tipoDePagoEC option:selected").val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/recalcular/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);

                    var montoZ = $("#m_importeEC").val();
                    var tipoPagZ = $("#c_tipoDePagoEC option:selected").val();

                    if (tipoPagZ == 'HDI' || tipoPagZ == 'HDIC' || tipoPagZ == "HIN") {
                        var montoIva = round((parseFloat(montoZ) * parseFloat(data.iva) / 100),2);
                        var montoIvaRet = round((parseFloat(montoIva) * parseFloat(data.ivaRet) / 100), 2);
                        var montoIsrRet = round((parseFloat(montoZ) * parseFloat(data.isrRet) / 100),2);
                        var bancos = round((parseFloat(montoZ) + parseFloat(montoIva) - parseFloat(montoIvaRet) - parseFloat(montoIsrRet)),2);
                    }

                    if (tipoPagZ == 'FDI' || tipoPagZ == 'FDIC' || tipoPagZ == "FIN") {
                        var montoIva = round((parseFloat(montoZ) * parseFloat(data.iva) / 100), 2);
                        var montoIvaRet = 0;
                        var montoIsrRet = 0;
                        var bancos = round((parseFloat(montoZ) + parseFloat(montoIva) - parseFloat(montoIvaRet) - parseFloat(montoIsrRet)), 2);
                    }

                    if (tipoPagZ == 'ADI' || tipoPagZ == 'ADIC' || tipoPagZ == 'AIN') {
                        var montoIva = 0;
                        var montoIvaRet = 0;
                        var importeMarginal = (parseFloat(montoZ) - parseFloat(data.limInferior)) * parseFloat(data.porcExcedente);
                        var montoIsrRet = round((parseFloat(data.cuotaFija) + parseFloat(importeMarginal)), 2);
                        var bancos = round((parseFloat(montoZ) + parseFloat(montoIva) - parseFloat(montoIvaRet) - parseFloat(montoIsrRet)),2);
                    }

                    if (tipoPagZ == 'RDI' || tipoPagZ == 'RIN') {
                        var montoIva = round((parseFloat(montoZ) * parseFloat(data.iva) / 100), 2);
                        var montoIvaRet = round((parseFloat(montoIva) * parseFloat(data.ivaRet) / 100), 2);
                        var montoIsrRet = round((parseFloat(montoZ) * parseFloat(data.isrRet) / 100), 2);
                        var bancos = round((parseFloat(montoZ) + parseFloat(montoIva) - parseFloat(montoIvaRet) - parseFloat(montoIsrRet)), 2);
                    }

                    $("#m_ivaEC").val(montoIva);
                    $("#m_ivaRetencionEC").val(montoIvaRet);
                    $("#m_isrRetencionEC").val(montoIsrRet);
                    $("#m_totalEC").val(bancos);

                    callback();
                }
            });
        },

        consultar: function() {
            var model = {
                IdEdoCta: $("#m_idEdoCta").val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);

                    $("#m_importeEC").val(data.TotalMonto); // se cambio de monto a montoTotal
                    $("#m_ivaEC").val(data.TotalIva);
                    $("#m_ivaRetencionEC").val(data.TotalIvaRet);
                    $("#m_isrRetencionEC").val(data.TotalIsrRet);
                    $("#m_totalEC").val(data.TotalBancos); // Se cambió de totalBancos a monto

                    DataTable.init();
                }
            });
        }
    }
}();

var formPage = function () {
    var IdMenu;
    var _htmlTable = '';
    var IdEdoCta;

    "use strict"; return {
        fileNameXML: '',
    	fileNamePDF: '',
        clean: function () {

            $("#c_idSiuEC").val("");
            $("#c_nombreCompletoEC").val("");
            $("#c_origenEC").val("");
            $("#c_cicloEC").val("");
            $("#c_periodoEC").val("");
            $("#c_tipoDeContratoEC").val("");
            $("#c_pagoFechaIniEC").val("");
            $("#c_pagoFechaFinEC").val("");
            $("#c_reciboFechaIniEC").val("");
            $("#c_reciboFechaFinEC").val("");
            $("#c_dispersionFechaIniEC").val("");
            $("#c_dispersionFechaFinEC").val("");
            $("#c_depositoFechaIniEC").val("");
            $("#c_depositoFechaFinEC").val("");
            $("#m_annyoEscolarEC").val("");
            $("#m_periodoEC").val("");
            $("#m_esquemaPagoEC").val("");
            $("#m_cuentaContableEC").val("");
            $("#m_tipoDeBloqueoEC").val("");
            $("#m_tipoDeTransferenciaEC").val("");
            $("#m_folioChequeEC").val("");
            $("#m_conceptoDePagoEC").val("");
            $("#m_fechaPagoEC").val("");
            $("#m_fechaReciboEC").val("");
            $("#m_fechaDispersionEC").val("");
            $("#m_fechaDepositoEC").val("");
            $("#m_importeEC").val("");
            $("#m_ivaEC").val("");
            $("#m_ivaRetencionEC").val("");
            $("#m_isrRetencionEC").val("");
            $("#m_totalEC").val("");
            $("#m_polizaEC").val("");
            $("#m_folioEC").val("");
            $("#m_beneficiarioEC").val("");
            $("#m_fechaSolicitadoEC").val("");
            $("#m_uuidEC").val("");
            $("#m_xmlEC").val("");
            $("#m_pdfEC").val("");
            $("#m_idPersona").val("");
            $("#m_fechaPago").val("");
            $("#m_concepto").val("");
			 var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
            for (var i = 0; i < maxBloqueos; i++) {
                $('#TipoBloqueo_' + i).prop('checked', false);
            }
        },

        init: function () {
            var array = [
				'#c_pagoFechaIniEC',
				'#c_pagoFechaFinEC',
				'#c_reciboFechaIniEC',
				'#c_reciboFechaFinEC',
				'#c_dispersionFechaIniEC',
				'#c_dispersionFechaFinEC',
				'#c_depositoFechaIniEC',
				'#c_depositoFechaFinEC',
                '#m_fechaPagoEC',
                '#m_fechaReciboEC',
                '#m_fechaDispersionEC',
                '#m_fechaDepositoEC'
            ];
            for (var i = 0; i < array.length; i++)
                $(array[i]).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: 'yy-mm-dd'
                });

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
                                var value = $("#c_nombreCompletoEC").getSelectedItemData().IDSIU;

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



                                        $("#c_idSiuEC").val(data.IdSIU);
                                        $("#c_origenEC").val(data.origen);
                                        $("#c_tipoFacturaEC").val(data.cveFactura);

                                        $("#hdfCorreo0365").val(data.correoO365);

                                        $("#formbtnconsultar").prop("disabled", false);


                                        $("#c_idSiuECHidden").val(data.IdPersona);

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
                        $("#c_nombreCompletoEC").easyAutocomplete(options);
                    } catch {}
                }
            });
        },

        ConfirmarEliminar: function (confirm) {
            var idctaPensionado = $('#hiddenIdPension').val();
            formPage.EliminarPensionado(confirm, idctaPensionado);
        },

        EliminarPensionado: function (confirm, idctaPensionado) {

            $('#hiddenIdPension').val(idctaPensionado);

            if (!confirm) {
                $('#modal-delete-pensionado').modal("show");
                return;
            }

            $('#modal-delete-pensionado').modal("hide");

            var model = {
                IdCtaPensionado: idctaPensionado
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/EliminarPensionado/",
                data: model,
                success: function (data) {
                    $('#notification').html(data.msg);

                    DataTable.init();
                }
            });
        },

        agregarAbrirPesionados: function () {
            add_ECP_pensionado1.IdPersona = $("#c_idSiuECHidden").val();
            add_ECP_pensionado1.init("add_ECP_pensionado1");

            $("#modal-agregarPensionados").modal('show');

        },

        agregarCPModalPensionado: function () {

            if (!formValidation.Validate()) {
                $("#modal-agregarPensionados").modal('hide');
                alert('¡Necesita seleccionar una persona!');
                return;
            }

            if (!formPage.validateInputsPensionados())
                return;

            var model = {
                IdBeneficiario: $("#add_ECP_pensionado").val(),
                IdPersona: $("#c_idSiuECHidden").val(),
                CveSede: $('#Sedes').val(),
                Periodo: $("#add_ECP_periodo").val(),
                PartePeriodo: $("#add_ECP_partePeriodo").val(),
                IdEsquema: $("#add_ECP_esquema").val(),
                ConceptoPagoID: $("#add_ECP_concepto").val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/AgregarEdoCtaModalPensionado/",
                data: model,
                success: function (data) {
                    $('#notification').html(data.msg);

                    $("#modal-agregarPensionados").modal('hide');
                    DataTable.init();
                }
            });
        },

        validateInputsPensionados: function () {
            var validado = true;

            if ($("#add_ECP_pensionado").val() == "" || $("#add_ECP_pensionado").val() == null) { $("#add_ECP_pensionado").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#add_ECP_pensionado").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#add_ECP_periodo").val() == "" || $("#add_ECP_periodo").val() == null) { $("#add_ECP_periodo").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#add_ECP_periodo").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#add_ECP_esquema").val() == "" || $("#add_ECP_esquema").val() == null) { $("#add_ECP_esquema").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#add_ECP_esquema").removeClass("form-control parsley-error").addClass("form-control"); }

            return validado;
        },

        agregarCPModal: function () {

            if (!formValidation.Validate()) {
                alert('¡Necesita Seleccionar una Persona!');
                return;
            }

            if (!formPage.validateInputs())
                return;

            var model = {
                IdPersona: $("#c_idSiuECHidden").val(),
                CveSede: $('#Sedes').val(),
                Periodo: $("#add_EC_periodo").val(),
                PartePeriodo: $("#add_EC_partePeriodo").val(),
                IdEsquema: $("#add_EC_esquema").val(),
                ConceptoPagoID: $("#add_EC_concepto").val(),
                FPago: $("#add_EC_fechaPago option:selected").text(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/AgregarEdoCtaModal/",
                data: model,
                success: function (data) {
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $('#notification').html(data.msg);

                    $("#modal-agregarEstadosCuenta").modal('hide');
                    DataTable.init();
                },
                error: function (msg) {
                    $('#notification').html(data.msg);
                }
            });
        },

        agregarCPNuevoModal: function () {

            idEsquema = $("#idesquema").val();

            var model = {
			    Clave: $("#add_EC_esquema").val(),
			    conceptoPago: $("#cpago").val(),
			    fechaPago: $("#fpago").val(),
			    fechaRecibo: $("#frecibo").val(),
			    tipoBloqueo: null,
                ESPECIAL: 1,
                personaID: $("#c_idSiuECHidden").val(),
			}

            if (!formPage.validateInputsFechas()) {
                return;
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/AddCalendarPagosECP/",
                data: model,
                success: function (data) {

                    $('#add_EC_fechaPago_div').show();

                    add_EC_concepto1.EsquemaID = $('#add_EC_esquema').val();
                    add_EC_concepto1.PersonaID = $("#c_idSiuECHidden").val();
                    add_EC_concepto1.init();

                    add_EC_fechaPago1.EsquemaID = $('#add_EC_esquema').val();
                    add_EC_fechaPago1.PersonaID = $("#c_idSiuECHidden").val();
                    add_EC_fechaPago1.init();

                    $('#idPagosF').val(data.idPagosF);

                    $("#ConceptodePagos").hide();
                    $("#agregarCPModal").prop("disabled", false);
                    DataTable.init();
                }
            });
        },

        validateInputsFechas: function () {

            var validado = true;

            if ($("#fpago").val() == "" || $("#fpago").val() == null) { $("#fpago").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#fpago").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#add_EC_esquema").val() == "" || $("#add_EC_esquema").val() == null) { $("#add_EC_esquema").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#add_EC_esquema").removeClass("form-control parsley-error").addClass("form-control"); }

            return validado;
        },

        validateInputs: function () {
            var validado = true;
            if ($("#add_EC_periodo").val() == "" || $("#add_EC_periodo").val() == null) { $("#add_EC_periodo").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#add_EC_periodo").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#add_EC_esquema").val() == "" || $("#add_EC_esquema").val() == null) { $("#add_EC_esquema").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#add_EC_esquema").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#add_EC_concepto").val() == "" || $("#add_EC_concepto").val() == null) { $("#add_EC_concepto").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#add_EC_concepto").removeClass("form-control parsley-error").addClass("form-control"); }

            return validado;
        },

        validateInputsECExterno: function () {
            var validado = true;
            if ($("#addExt_EC_periodo").val() == "" || $("#addExt_EC_periodo").val() == null) { $("#addExt_EC_periodo").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#addExt_EC_periodo").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#addExt_EC_partePeriodo").val() == "" || $("#addExt_EC_partePeriodo").val() == null) { $("#addExt_EC_partePeriodo").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#addExt_EC_partePeriodo").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#addExt_EC_esquema").val() == "" || $("#addExt_EC_esquema").val() == null) { $("#addExt_EC_esquema").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#addExt_EC_esquema").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#addExt_EC_escuela").val() == "" || $("#addExt_EC_escuela").val() == null) { $("#addExt_EC_escuela").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#addExt_EC_escuela").removeClass("form-control parsley-error").addClass("form-control"); }

            return validado;
        },

        consultar: function () {
            DataTable.init();
            $("#formbtnmover").prop("disabled", false);
            $("#formbtneliminar").prop("disabled", false);
        },

        consultaECWeb: function () {
            if (!formValidation.Validate())
                return;

            var data =
                'Periodo=' + $('#c_periodoEC').val()
                + '&PartePeriodo=' + $('#c_partePeriodoEC').val()
                + '&IDSIU=' + $('#c_idSiuEC').val()
                + '&NombreCompleto=' + $('#c_nombreCompletoEC').val()
                + '&Campus=' + $('#Sedes').val();

            var url = '/Token/GenerarToken?'  + data;

            window.open(url, "_blank");
        },

        edit: function (id) {

            $("#modal-editarEstadosCuentaDetalle").modal('show');

            var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
            for (var i = 0; i < maxBloqueos; i++) {
                $('#TipoBloqueo_' + i).prop('checked', false);
            }

            var model = {
                IdEdoCtaD: id
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);

                   //ciclos
                    $("#m_annyoEscolarEC").val(data.ciclo);

                    //periodos
                    comboPeriodosM.Periodo = data.Periodo;
                    comboPeriodosM.id_ciclo = data.ciclo;
                    comboPeriodosM.init("comboPeriodosM");

                    //periodos
                    $("#m_partePeriodoEC").val(data.PartePeriodo);

                    //esquemas
                    comboEsquemasPagosM.IdEsquema = data.IdEsquema;
                    comboEsquemasPagosM.Sede = $('#Sedes').val();//**ver
                    comboEsquemasPagosM.Periodo = data.Periodo;
                    comboEsquemasPagosM.Nivel = data.CveNivel;
                    comboEsquemasPagosM.init();

                    $("#m_cuentaContableEC").val(data.idCentroCosto);
                    $("#m_conceptoDePagoEC").val(data.Concepto);
                    $("#m_fechaPagoEC").val(data.FechaPago);

                    comboFechasPago2.FechaPago = data.FechaPago;
                    comboFechasPago2.EsquemaID = data.IdEsquema;
                    comboFechasPago2.init();

                    $("#m_fechaReciboEC").val(data.FechaRecibo);
                    $("#m_fechaDispersionEC").val(data.FechaDispersion);
                    $("#m_fechaDepositoEC").val(data.FechaDeposito);
                    $("#m_importeEC").val(data.Monto); // se cambio de monto a montoTotal
                    $("#m_ivaEC").val(data.TotalIva);
                    $("#m_ivaRetencionEC").val(data.TotalIvaRet);
                    $("#m_isrRetencionEC").val(data.TotalIsrRet);
                    $("#m_totalEC").val(data.TotalBancos); // Se cambió de totalBancos a monto
                    $("#m_folioEC").val(data.Folio);
                    $("#m_beneficiarioEC").val(data.Beneficiario);
                    $("#m_fechaSolicitadoEC").val(data.Solicitado);
                    $("#m_uuidEC").val(data.Uuid);
                    $("#m_xmlEC").val(data.Xml);
                    $("#c_tipoDePagoEC").val(data.tipoPago);
                    //hiddenFields
                    $("#m_idPersona").val(data.IdPersona);
                    $("#m_fechaPago").val(data.FechaPago);
                    $("#m_concepto").val(data.Concepto);
                    $("#m_idEdoCta").val(data.IdEdoCta);
                    $("#m_idEdoCtaD").val(data.IdEdoCtaD);

                    $("#m_bloqueado").prop("checked", (data.Bloqueado == "True" ? true : false));

                    comboConceptosPagoM.CampusVPDI = $('#Sedes').val();
                    comboConceptosPagoM.EsquemaID = data.IdEsquema;
                    comboConceptosPagoM.PersonaID = data.IdPersona;
                    comboConceptosPagoM.Periodo = data.Periodo;
                    comboConceptosPagoM.Nivel = data.CveNivel;
                    comboConceptosPagoM.NumPago = data.NumPago;
                    comboConceptosPagoM.init();
                }
            });

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/EditBloqueos/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);

                    for (var i = 0; i < data.arrBloqueos.length; i++) {
                        var idbloqueo = data.arrBloqueos[i];
                        $("input[data-idbloqueo='" + idbloqueo + "']").prop('checked', true);
                    }
                }
            });
        },

        agregarAbrir: function () {
            $("#add_EC_anio").val("");
            $("#add_EC_periodo").val("");
            $("#add_EC_partePeriodo").val("");
            $("#add_EC_esquema").val("");
            $("#add_EC_concepto").val("");
            $("#add_EC_fechaPago").val("");
            $("#modal-agregarEstadosCuenta").modal('show');
        },

        agregarAbrirEdoCtaExterno: function () {

            $("#titulo_edocuentaExt").html("Agregar Estado de Cuenta a externo");
            $("#modal-agregarEstadosCuentaExterno").modal('show');
        },

        agregarAbrirEsquema: function () {

            $("#titulo_edocuentaExt").html("Agregar Esquema");
            $("#modal-agregarEstadosCuentaExterno").modal('show');
        },

        agregarCPModalEdoCtaExterno: function () {

            if (!formValidation.Validate()) {
                alert('¡Necesita seleccionar una persona!');
                $("#modal-agregarEstadosCuentaExterno").modal('hide');
                return;
            }

            if (!formPage.validateInputsECExterno()) {
                alert('Favor de llenar los campos solicitados');
                return;
            }

            var model = {
                CveSede: $('#Sedes').val(),
                IdPersona: $("#c_idSiuECHidden").val(),
                Periodo: $("#addExt_EC_periodo").val(),
                IdEsquema: $("#addExt_EC_esquema").val(),
                PartePeriodo: $("#addExt_EC_partePeriodo").val(),
                CampusINB: $("#addExt_EC_campusPA").val(),
                Escuela: $("#addExt_EC_escuela").val(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/AgregarEdoCtaModalExterno/",
                data: model,
                success: function (data) {
                    $('html, body').animate({ scrollTop: 0 }, 'fast');
                    $('#notification').html(data.msg);

                    $("#modal-agregarEstadosCuentaExterno").modal('hide');
                    DataTable.init();
                },
                error: function (msg) {
                    $('#notification').html(data.msg);
                }
            });
        },

        savePagoEdoCta: function () {

            var validado = true;

            if ($("#m_tipoDePagoEC").val() == "" || $("#m_tipoDePagoEC").val() == null) { $("#m_tipoDePagoEC").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#m_tipoDePagoEC").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#m_tipoDeTransferenciaEC").val() == "" || $("#m_tipoDeTransferenciaEC").val() == null) { $("#m_tipoDeTransferenciaEC").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#m_tipoDeTransferenciaEC").removeClass("form-control parsley-error").addClass("form-control"); }

            //JABB 16122019
            if ($("#edit_EC_fechadePago_div").length) {
                if ($("#edit_EC_fechadePago").val() == "" || $("#edit_EC_fechadePago").val() == null) { $("#edit_EC_fechadePago").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
                else { $("#edit_EC_fechadePago").removeClass("form-control parsley-error").addClass("form-control"); }
            }

            if (validado == true) {
                var model = {
                    IdEdoCtaD: IdEdoCta,
                    FechaSolicitado: $("#edit_EC_fechasolicitado").val(),
                    FechaRecibo: $("#edit_EC_fecharecibo").val(),
                    FechaDispersion: $("#edit_EC_fechadispersion").val(),
                    FechaDeposito: $("#edit_EC_fechadeposito").val(),
                    FechaPago: $("#edit_EC_fechadePago").val(),
                    tipoPago: $("#m_tipoDePagoEC").val(),
                    idTransferencia: $("#m_tipoDeTransferenciaEC").val(),
                    folioCheque: $("#m_folioChequeEC").val()
                }

                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    cache: false,
                    url: "/EstadodeCuenta/savePagoEdoCta/",
                    data: model,
                    success: function (data) {

                        DataTable.init();
                        $('#notification').html(data.msg);
                        $("#modal-editarPagoEstadoCuenta").modal('hide');

                        $('html, body').animate({ scrollTop: 0 }, 'fast');
                    }
                });
            }
        },

        editPago: function (id) {

            $('#eliminarErrorPDFXML').css("display", "none");

            comboTiposPagosM.init("comboTiposPagosM");
            comboTiposTransferenciaM.init("comboTiposTransferenciaM");
            
            $("#m_folioChequeEC").val("");
            //$("#formbtnEliminarPDF").prop("disabled", true);
            //$("#formbtnEliminarXML").prop("disabled", true);

			$('#btn_subir_xml_span').show();
			$('#btn_subir_xml_img').hide();
            $("#modal-editarPagoEstadoCuenta").modal('show');

        	$('#file_xml_name').val('');
        	$('#file_pdf_name').val('');
			this.fileNameXML = '';
        	this.fileNamePDF = '';
        	loading('loading-bar');

        	IdEdoCta = id;

        	var model = {
        		IdEdoCtaD: id,
        		IdSIU:'',
        		CveSede: $('#Sedes').val()
        	}

        	$.ajax({
        		type: "POST",
        		dataType: 'json',
        		cache: false,
        		url: "/EstadodeCuenta/EditPagoEstadoCuenta/",
        		data: model,
        		success: function (data) {
        			data = jQuery.parseJSON(data);

        			$('.loader-min-bar').hide();

                    //A la carga de los combos se le da una mayor prioridad
                    $("#m_tipoDePagoEC option[value='" + data.tipoPago + "']").attr("selected", "selected");
                    $("#m_tipoDeTransferenciaEC").val(data.idTransferencia);

        			$("#edit_EC_periodos").html(data.Periodo);
        			$("#edit_EC_partePeriodo").html(data.PartePeriodo);
        			$("#edit_EC_esquema").html(data.Esquema);
        			$("#edit_EC_concepto").html(data.Concepto);
        			$("#edit_EC_fechapago").html(data.FechaPago);

        			$("#edit_EC_monto").html(data.TotalMonto);
        			$("#edit_EC_iva").html(data.TotalIva);
        			$("#edit_EC_ivaret").html(data.TotalIvaRet);
        			$("#edit_EC_isrret").html(data.TotalIsrRet);
        			$("#edit_EC_banco").html(data.TotalBancos);

        			$('#edit_EC_statuspago').html('<span class="label label-success">' + data.estado + '</span>');

        			$("#edit_EC_fechasolicitado").val(data.FechaSolicitado);
        			$("#edit_EC_fecharecibo").val(data.FechaRecibo);
        			$("#edit_EC_fechadispersion").val(data.FechaDispersion);
                    $("#edit_EC_fechadeposito").val(data.FechaDeposito);
                    $("#edit_EC_fechadePago").val(data.FechaPago);//JABB 16122019

        			formPage.setLinksXMLyPDF(data);
					//$("#formbtnSubirXML").prop("disabled", true);
                    formPage.botonSubir(false, false);

                    $("#m_folioChequeEC").val(data.folioCheque);
                }
            });
        },

        moverAbrir: function () {
		  //selecciona el ciclo de mover si esta sel en el escritorio el año
            $("#l_anioEC option[value='" + $("#c_cicloEC").val() + "']").attr("selected", "selected");

            //Se limipian los elementos que están incluidos en el modal modal-moverEstadosCuenta
            $("#l_anioEC").val('');
            $("#l_periodoEC").val('');
            $("#l_esquemaPagoEC").val('');
            $("#l_conceptoDePagoEC").val('');
            $("#l_fechaPagoEC").val('');

            $("#modal-moverEstadosCuenta").modal('show');
        },

        mover: function () {
            var arr = DataTable.checkboxs;
            var arrChecked = [];
            var checkedBox;
            var padreH;
            var padreHX;
            var padreTR;

            for (var i = 0; i < arr.length; i++) {
                var checkbox_checked;

                if ($('#P_' + arr[i]).length > 0) {
                    checkbox_checked = $('#P_' + arr[i]).prop('checked');

                    if (miNodoPadreX('P_' + arr[i]) == "PadreTR") {
                        padreTR = $('#P_' + arr[i]).val();
                    }

                } else if ($('#H_' + arr[i]).length > 0) {
                    checkbox_checked =$('#H_' + arr[i]).prop('checked');
                } else {
                    checkbox_checked = $('#H_' + padreTR + '-' + arr[i]).prop('checked');
                }

                if (checkbox_checked == true) {
                    //utilizar los parenNode.id
                    if ($('#P_' + arr[i]).length > 0) {
                        arrChecked.push('P' + $('#P_' + arr[i]).val());
                    } else if ($('#H_' + arr[i]).length > 0) {
                        padreH = 'P_' + miNodoPadre('H_' + arr[i]);
                        if (!$('#' + padreH).prop('checked')) {
                            arrChecked.push('H' + $('#H_' + arr[i]).val());
                        }
                    } else {
                        padreHX = 'P_' + miNodoSubPadre('H_' + padreTR + '-' + arr[i]);
                        if (!$('#' + padreHX).prop('checked')) {
                            arrChecked.push('H' + $('#H_' + padreTR + '-' + arr[i]).val());
                        }
                    }
                }
            }

            if (arrChecked.length == 0) {
                alert('Debes seleccionar una casilla');
                return;
            }

            var model = {
                IDs: arrChecked.join(),
                ConceptoPagoID: (($('#l_conceptoDePagoEC').val() == "" || $('#l_conceptoDePagoEC').val() == null) ? '0' : $('#l_conceptoDePagoEC').val())
            }

            $.ajax({
                type: "POST",
                cache: false,
                url: "/EstadodeCuenta/mover/",
                data: model,
                success: function (msg) {
                    DataTable.init();
                    $('#notification').html(msg);
                },
                error: function (msg) {
                    $('#seccion1').unblock();
                    $('#notification').html(msg);
                }
            });

            $("#modal-moverEstadosCuenta").modal('hide');
            DataTable.init();
        },

        eliminarAbrir: function() {
            $("#modal-deleteEstadodecuenta").modal('show');
        },

        eliminar: function () {
            var arr = DataTable.checkboxs;
            var arrChecked = [];
            var checkedBox;

            for (var i = 0; i < arr.length; i++) {
                var checkbox_checked = ($('#P_' + arr[i]).length > 0 ? $('#P_' + arr[i]).prop('checked') : $('#H_' + arr[i]).prop('checked'));

                if (checkbox_checked == true)
                    arrChecked.push(($('#P_' + arr[i]).length > 0 ? 'P' + $('#P_' + arr[i]).val() : 'H' + $('#H_' + arr[i]).val()));
            }

            if (arrChecked.length == 0) {
                alert('Debes seleccionar una casilla');
                return;
            }

            var model = {
                IDs: arrChecked.join(),
                Motivos: $('#motivo').val(),
            }

            $.ajax({
                type: "POST",
                cache: false,
                url: "/EstadodeCuenta/eliminar/",
                data: model,
                success: function (msg) {
                    DataTable.init();
                    $('#notification').html(msg);
                },
                error: function (msg) {
                    $('#seccion1').unblock();
                    $('#notification').html(msg);
                }
            });

            $("#modal-deleteEstadodecuenta").modal('hide');
            DataTable.init();
        },

        grabar: function () {
            importesCalc.recalcular();

            $('#modal-editarEstadosCuentaDetalle').modal('hide');
            $('#modal-validarcontrato').modal('show');

            var arrBloqueos = new Array();
            var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
            for (var i = 0; i < maxBloqueos; i++) {
                var bloqueo = $('#TipoBloqueo_' + i).val();
                var activo = $('#TipoBloqueo_' + i).prop('checked');
                if (activo) {
                    arrBloqueos.push(bloqueo);
                }
            }

            var model = {
                IdEdoCta: $("#m_idEdoCta").val(),
                IdEdoCtaD: $("#m_idEdoCtaD").val(),
                IdPersona: $("#m_idPersona").val(),
                FechaPago: $("#m_fechaPagoEC option:selected").text(),
                NumPago: $("#m_conceptoDePagoEC option:selected").val(),
                Concepto: $("#m_concepto").val(),
                ConceptoM: $("#m_conceptoDePagoEC option:selected").text(),
                FechaRecibo: $("#m_fechaReciboEC").val(),
                FechaDispersion: $("#m_fechaDispersionEC").val(),
                FechaDeposito: $("#m_fechaDepositoEC").val(),
                Monto: $("#m_importeEC").val(),
                TotalIva: $("#m_ivaEC").val(),
                TotalIvaRet: $("#m_ivaRetencionEC").val(),
                TotalIsrRet: $("#m_isrRetencionEC").val(),
                TotalBancos: $("#m_totalEC").val(),
                Folio: $("#m_folioEC").val(),
                Beneficiario: $("#m_beneficiarioEC").val(),
                folioCheque: $("#folioCheque").val(),
                idCentroCosto: $("#m_cuentaContableEC option:selected").val(),
                Bloqueado:  ($('#m_bloqueado').is(':checked') == true ? "on" : "false"),
                IdEsquema: $("#m_esquemaPagoEC").val(),
                Motivos: $('#motivoE').val(),
                bloqueos: arrBloqueos.join(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/Grabar/",
                data: model,
                success: function (data) {
                    DataTable.init();
                    $('#notification').html(data.msg);
                }
            });

            DataTable.init();
        },

        contrato: function (confirm) {
            if (!confirm) {
                return;
            }

            var model = {
                IdPersona: $("#m_idPersona").val(),
                IdEsquema: $("#m_esquemaPagoEC").val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/quitarFechaContrato/",
                data: model,
                success: function (data) {
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });

            $('#modal-validarcontrato').modal('hide');
        },

        ocultar: function (o) {
            if (o === "c") {
                $("#divConsultaEC").hide();
                $("#divModificaEC").show();
            } else {
                $("#divConsultaEC").show();
                $("#divModificaEC").hide();
            }
        },

        agregar: function () {
            $('#modal-editarEstadosCuentaDetalle').modal('hide');
            $('#modal-validarcontrato').modal('show');

            var arrBloqueos = new Array();
            var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
            for (var i = 0; i < maxBloqueos; i++) {
                var bloqueo = $('#TipoBloqueo_' + i).val();
                var activo = $('#TipoBloqueo_' + i).prop('checked');
                if (activo) {
                    arrBloqueos.push(bloqueo);
                }
            }

            var model = {
                IdEdoCta: $("#m_idEdoCta").val(),
                IdEdoCtaD: $("#m_idEdoCtaD").val(),
                NumPago: $("#m_conceptoDePagoEC option:selected").val(),
                ConceptoM: $("#m_conceptoDePagoEC option:selected").text(),
                idTransferencia: $("#m_tipoDeTransferenciaEC").val(),
                idCentroCosto: $("#m_cuentaContableEC").val(),
                FechaPago: $("#m_fechaPagoEC option:selected").text(),
                FechaRecibo: $("#m_fechaReciboEC").val(),
                FechaDispersion: $("#m_fechaDispersionEC").val(),
                FechaDeposito: $("#m_fechaDepositoEC").val(),
                Monto: $("#m_importeEC").val(),
                TotalIva: $("#m_ivaEC").val(),
                TotalIvaRet: $("#m_ivaRetencionEC").val(),
                TotalIsrRet: $("#m_isrRetencionEC").val(),
                TotalBancos: $("#m_totalEC").val(),
                Folio: $("#m_folioEC").val(),
                Beneficiario: $("#m_beneficiarioEC").val(),
                Bloqueado: ($('#m_bloqueado').is(':checked') == true ? "on" : "false"),
                bloqueos: arrBloqueos.join(),
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/EstadodeCuenta/AgregarEdoCta/",
                data: model,
                success: function (data) {
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        setLinksXMLyPDF: function (data) {
            this.fileNameXML = '';
            this.fileNamePDF = '';
            if (data.file_xml_name == '') {
                $('#file_xml_name').show();
                $('#file_xml_name').val('');
                $('#file_xml_link').hide();
                $("#formbtnEliminarXML").attr('disabled', 'disabled');
            }
            else {
                $('#file_xml_name').hide();
                $('#file_xml_link').html('<a href="/Upload/' + data.file_xml_name + '" target="_blank"><i class="fa fa-download"></i>&nbsp;Mostrar archivo XML</a>');
                $('#file_xml_link').show();
                $("#formbtnEliminarXML").removeAttr('disabled');
            }
            if (data.file_pdf_name == '') {
                $('#file_pdf_name').show();
                $('#file_pdf_name').val('');
                $('#file_pdf_link').hide();
                $("#formbtnEliminarPDF").attr('disabled', 'disabled');
            }
            else {
                $('#file_pdf_name').hide();
                $('#file_pdf_link').html('<a href="/Upload/' + data.file_pdf_name + '" target="_blank"><i class="fa fa-download"></i>&nbsp;Mostrar archivo PDF</a>');
                $('#file_pdf_link').show();
                $("#formbtnEliminarPDF").removeAttr('disabled');
            }
        },

        subirXML: function (idEstadoCuenta) {
			if (this.fileNameXML == '' && this.fileNamePDF == '') {
        		alert('Falta especificar el archivo XML o PDF');
        		return false;
			}

			var model = {
				ID_ESTADODECUENTA: IdEdoCta,
				FileNameXML: this.fileNameXML,
				FileNamePDF: this.fileNamePDF,
				ValidarXML: true,
				ValidarMySuite: false,
				Sede: $('#Sedes').val(),
			}

			formPage.botonSubir(false, true);
			$.ajax({
				type: "POST",
				cache: false,
				url: "/ECW_EstadoCuenta/processXML",
				data: model,
				success: function (msg) {
					if (msg == "0") {
						formPage.fileNameXML = '';
						formPage.fileNamePDF = '';
						var model = {
							IdEdoCtaD: IdEdoCta,
							CveSede: $('#Sedes').val(),
						}
						$.ajax({
							type: "POST",
							dataType: 'json',
							cache: false,
							url: "/EstadodeCuenta/EditPagoEstadoCuenta",
							data: model,
							success: function (data) {
								data = jQuery.parseJSON(data);
								formPage.setLinksXMLyPDF(data);
								$('#xml_result').html('');
							},
							complete: function (data) {
								formPage.botonSubir(true, false);
							}
						});
					}
					else {
					    $('#xml_result').html(msg);

						formPage.botonSubir(true, false);
					}
				},
				error: function (msg) {
					formPage.botonSubir(true, false);
				}
			});
			return true;
        },

		botonSubir: function (activo, subiendo) {
			if (activo)
				$("#formbtnSubirXML").prop("disabled", false);
			else
				$("#formbtnSubirXML").prop("disabled", true);
			if (subiendo) {
				$('#btn_subir_xml_span').hide();
				$('#btn_subir_xml_img').show();
			}
			else {
				$('#btn_subir_xml_span').show();
				$('#btn_subir_xml_img').hide();
			}
		},

        cancelarPDF: function () {
            $("#modal-deletePDF").modal('hide');
            $("#modal-editarPagoEstadoCuenta").modal('show');
        },

        cancelarXML: function () {
            $("#modal-deleteXML").modal('hide');
            $("#modal-editarPagoEstadoCuenta").modal('show');
        },

        eliminarPDFAbrirModal: function () {
            $("#modal-editarPagoEstadoCuenta").modal('hide');
            $("#modal-deletePDF").modal('show');
        },

        eliminarXMLAbrirModal: function () {
            $("#modal-editarPagoEstadoCuenta").modal('hide');
            $("#modal-deleteXML").modal('show');
        },

        eliminarPDF: function (idEstadoCuenta) {

            $("#modal-deletePDF").modal('hide');
            $("#modal-editarPagoEstadoCuenta").modal('show');

            if ($('#file_pdf_link').hasClass('hide')) {
                alert('No hay archivo PDF que eliminar');
                return false;
            }

            $.ajax({
                type: "GET",
                cache: false,
                url: "/EstadodeCuenta/eliminarXMLoPDF?EdoCtaID=" + IdEdoCta + "&file=PDF",
                success: function () {
                    $('#file_pdf_name').show();
                    $('#file_pdf_name').val('');
                    $('#file_pdf_link').hide();
                    $('#edit_EC_fecharecibo').val('');
                    $("#formbtnEliminarPDF").attr('disabled', 'disabled');
                },
                error: function (msg) {
                    $('#eliminarErrorPDFXML').css("display", "block");
                }
            });
            return true;
        },

        eliminarXML: function (idEstadoCuenta) {

            $("#modal-deleteXML").modal('hide');
            $("#modal-editarPagoEstadoCuenta").modal('show');

            if ($('#file_xml_link').hasClass('hide')) {
                alert('No hay archivo XML que eliminar');
                return false;
            }

            $.ajax({
                type: "GET",
                cache: false,
                url: "/EstadodeCuenta/eliminarXMLoPDF?EdoCtaID=" + IdEdoCta + "&file=XML",
				success: function (msg) {
					//if (msg == '=)')
					{
						$('#file_xml_name').show();
						$('#file_xml_name').val('');
						$('#file_xml_link').hide();
						$('#edit_EC_fecharecibo').val('');
						$("#formbtnEliminarXML").attr('disabled', 'disabled');
					}
				},
                error: function (msg) {
                    $('#eliminarErrorPDFXML').css("display", "block");
                }
            });
            return true;
        },

        //getCentroCostos: function () {
        //    var model = {
        //        CveSede: $('#Sedes').val(),
        //        Escuela: $('#EscuelaSearch').val(),
        //    }
        //    $.ajax({
        //        type: "POST",
        //        dataType: 'json',
        //        cache: false,
        //        url: "/EstadodeCuenta/getCentroCostos/",
        //        data: model,
        //        success: function (data) {
        //            console.log(data);
        //            data = jQuery.parseJSON(data);
        //            $("#m_cuentaContableEC").html(data);
        //        },
        //        error: function (data) {
        //            session_error(data);
        //        }
        //    });
        //},
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
                this.setPage(1);
            }
        },

        exportExcel: function (table) {
            if (!formValidation.Validate()) {
                alert('Necesita Seleccionar una Persona!!');
                return;
            }

            var data =
             'idSiuEC=' + $('#c_idSiuEC').val()
           + '&fltrSedes=' + $('#Sedes').val()
           + '&fltrPperio=' + $('#c_partePeriodoEC').val()
           + '&fltrPerio=' + $('#c_periodoEC').val()
           + '&fltrEscuela=' + $('#c_escuelaEC').val()
           + '&fltrTipoContr=' + $('#c_tipoDePagoEC').val()
           + '&fltrCCost=' + $('#c_centroCostosEC').val()
           + '&fltrPagoI=' + $('#c_pagoFechaIniEC').val()
           + '&fltrPagoF=' + $('#c_pagoFechaFinEC').val()
           + '&fltrReciI=' + $('#c_reciboFechaIniEC').val()
           + '&fltrReciF=' + $('#c_reciboFechaFinEC').val()
           + '&fltrDispI=' + $('#c_dispersionFechaIniEC').val()
           + '&fltrDispF=' + $('#c_dispersionFechaFinEC').val()
           + '&fltrDepoI=' + $('#c_depositoFechaIniEC').val()
           + '&fltrDepoF=' + $('#c_depositoFechaFinEC').val();

            window.location.href = '/EstadodeCuenta/ExportExcel?' + data;

        },

        edit: function (id) {
            formPage.edit(id);
        },
        editPago: function(id){
            formPage.editPago(id);
        },

        setPage: function (page) {
            pag = page;
            this.init();
        },

        setShow: function () {
            this.setPage(1);
        },

        Orderby: function (campo) {
            order = campo;
            var sortcampo = $('#' + this.myName + '-SORT-' + campo).data("sort");
            if (sortcampo == sortoption.ASC) { sort = sortoption.DESC; } else { sort = sortoption.ASC; }
            this.init();
        },

        init: function () {

            loading('loading-bar');
            loading('loading-circle', '#datatable_edo', 'Consultando datos..');

            var show = 1000;//$('#' + this.myName + '-data-elements-length').val();
            var search = $('#' + this.myName + '-searchtable').val();
            var orderby = order;
            var sorter = sort;
            var fltrIdSiu = $("#c_idSiuEC").val();

            var data =
        '&fltrSedes=' + $('#Sedes').val()
      + '&fltrPperio=' + $('#c_partePeriodoEC').val()
      + '&fltrPerio=' + $('#c_periodoEC').val()
      + '&fltrEscuela=' + $('#c_escuelaEC').val()
      + '&fltrTipoContr=' + $('#c_tipoDePagoEC').val()
      + '&fltrCCost=' + $('#c_centroCostosEC').val()
      + '&fltrPagoI=' + $('#c_pagoFechaIniEC').val()
      + '&fltrPagoF=' + $('#c_pagoFechaFinEC').val()
      + '&fltrReciI=' + $('#c_reciboFechaIniEC').val()
      + '&fltrReciF=' + $('#c_reciboFechaFinEC').val()
      + '&fltrDispI=' + $('#c_dispersionFechaIniEC').val()
      + '&fltrDispF=' + $('#c_dispersionFechaFinEC').val()
      + '&fltrDepoI=' + $('#c_depositoFechaIniEC').val()
      + '&fltrDepoF=' + $('#c_depositoFechaFinEC').val();


            if (search === undefined) search = "";

            $.ajax({
                type: "GET",
                cache: false,
                url: "/EstadodeCuenta/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&fltrIdSiu=" + fltrIdSiu+""+data,
                success: function (msg) {

                    $('.loader-min-bar').hide();

                    if (session_error(msg) == false) {
                        $("#datatable_edo").html(msg);
                    }
                },
                error: function (msg) {
                    session_error(msg);
                }
            });
        }
    }
}();

function convertDate(inputFormat) {

    if (inputFormat === "" || inputFormat === null || inputFormat === undefined) {
        return "";
    } else {
        function pad(s) { return (s < 10) ? '0' + s : s; }
        var d = new Date(inputFormat);
        return [pad(d.getDate() + 1), pad(d.getMonth() + 1), d.getFullYear()].join('/');
    }
}

function showFiltersEtdoCuenta() {
    if ($('#filterssearch').is(":visible")) {
        $('#iconmorefilters').html('<i class="fa fa-plus-square" aria-hidden="true"></i>');
        $('#filterssearch').hide();
    } else {
        $('#iconmorefilters').html('<i class="fa fa-minus-square" aria-hidden="true"></i>');
        $('#filterssearch').show();
    }
}

function buscarTR(idCheckbox) {
}

function miNodoPadre(id) {
    var x = document.getElementById(id).parentNode.parentNode.id;
    var y =  x.slice(x.indexOf("_") + 1,x.lastIndexOf("_"));
    return y;
}

function miNodoPadreX(id) {
    var x = document.getElementById(id).parentNode.parentNode.id;
    var y = x.slice(0, x.indexOf("_"));
    return y;
}

function miNodoSubPadre(id) {
    var x = document.getElementById(id).parentNode.parentNode.id;
    var y = x.slice(x.indexOf("-") + 1, x.lastIndexOf("-"));
    return y;
}