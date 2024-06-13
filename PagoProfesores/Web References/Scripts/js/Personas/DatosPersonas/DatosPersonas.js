
var direccion1 = new Direccion('direccion_cp', 'direccion_pais', 'direccion_estado', 'direccion_ciudad', 'direccion_entidad', 'direccion_colonia');
var direccion2 = new Direccion('rz_direccion_cp', 'rz_direccion_pais', 'rz_direccion_estado', 'rz_direccion_ciudad', 'rz_direccion_entidad', 'rz_direccion_colonia');

$(function () {
   /* $(document).ready(function () {
      $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 3
        });
    });*/
    
    $(window).load(function () {
        $('#dMontoFijo').hide();
        $('#dPensionadoPorcentaje').show();

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        $("#formbtnsave2").hide();
        $("#formbtndelete2").hide();

        formValidation.Inputs(["idsiu", "apellidos", "nombres", "sexo", "cve_tipodepago"]);
		formValidation.notEmpty('idsiu', 'El campo ID no debe de estar vacio');
		formValidation.notEmpty('apellidos', 'El campo Apellidos no debe de estar vacio');
		formValidation.notEmpty('nombres', 'El campo Nombres no debe de estar vacio');
        formValidation.notEmpty('sexo', 'El campo Sexo no debe de estar vacio');
        formValidation.notEmpty('cve_tipodepago', 'El campo Tipo de Pago no debe de estar vacio');
        
		$("#fechanacimiento").datepicker({
			changeMonth: true,
			changeYear: true,
			dateFormat: 'dd/mm/yy',
			yearRange: "-100:+0",
        });

		$("#FechaCedula").datepicker({
			changeMonth: true,
			changeYear: true,
			dateFormat: 'dd/mm/yy',
			yearRange: "-100:+0",
		});

		direccion1.init("direccion1");
		direccion2.init("direccion2");

		formPage.clean();
		DataTable.init();
	});

});//End function jquery

$('#idsiu').keypress(function (e) {
    if (e.which == 13) {
        var model = {
            Idsiu: this.value,
            cveSede: $("#Sedes").val(),
        }

        $.ajax({
            type: "POST",
            dataType: 'json',
            cache: false,
            url: "/DatosPersonas/getIdPersona/",
            data: model,
            success: function (data) {
                data = jQuery.parseJSON(data);
                if (data.Id_Persona !== "-1") {
                    formPage.edit(data.Id_Persona);
                }
                else {
                    $('#notification').html(data.msg);
                }
            },
            error: function (data) {
                $('#notification').html(data.msg);
            }
        });
    }
});

$("#Sedes").on('change', function () {
    formPage.clean();
});

$('#TipoPension').on('change', function () {

    if (this.value == "M") { $('#dMontoFijo').show(); $('#dPensionadoPorcentaje').hide(); }
    else { $('#dMontoFijo').hide(); $('#dPensionadoPorcentaje').show(); }
     
});

var formPage = function () {
	var Id_Persona = 0;
	var CleanDireccion = true;

	"use strict"; return {

        clean: function () {
            console.log("-->" + this.objectName);
            formValidation.Clean();

			$("#origen").val('E');

			direccion1.init("direccion1");
			direccion2.init("direccion2");
            
			$('#PersActivo').prop('checked', true);//
            $('#recibos').prop('checked', true);//
            $("#formbtnadd").show();
			$("#formbtnadd").prop("disabled", false);
			$("#formbtnsave").hide();
			$("#formbtnsave").prop("disabled", true);
			$("#formbtndelete").prop("disabled", true);
            $("#formbtndelete").hide();
            $("#origen").val('E');
            $('html, body').animate({ scrollTop: 0 }, 'fast');
            $("#Id_persona").prop("disabled", true);
            $("#id_persona").val("");
			$("#idsiu").val("");
			$("#profesor").val("");
			$("#apellidos").val("");
			$("#nombres").val("");
			$("#sexo").val("");
			$("#fechanacimiento").val("");
			$("#nacionalidad").val("");
			$("#telefono").val("");
			$("#correo").val("");
			$("#correo365").val("");
			$("#cve_tipodepago").val("");
			$("#mysuite").val("");
			$("#cve_origen").val("");
			$("#rfc").val("");
			$("#curp").val("");
			$("#direccion_cp").val("");
			$("#cve_banco").val("");
			$("#cuentaclabe").val("");
			$("#nocuenta").val("");
			$("#razonsocial").val("");
			$("#rz_rfc").val("");
			$("#rz_curp").val("");
			$("#rz_direccion_pais").val("");
			$("#rz_direccion_estado").val("");
			$("#rz_direccion_ciudad").val("");
			$("#rz_direccion_entidad").val("");
			$("#rz_direccion_colonia").val("");

			$("#rz_direccion_pais").html("");
			$("#rz_direccion_estado").html("");
			$("#rz_direccion_ciudad").html("");
			$("#rz_direccion_entidad").html("");
			$("#rz_direccion_colonia").html("");

			$("#rz_direccion_calle").val("");
			$("#rz_direccion_numero").val("");
			$("#rz_direccion_cp").val("");

			$("#direccion_calle").val("");
			$("#direccion_numero").val("");
			$("#direccion_cp").val("");
			// Contrato
			$("#TituloProfesional").val("");
			$("#Profesion").val("");
			$("#CedulaProfesional").val("");
			$("#FechaCedula").val("");
			$("#SeguroSocial").val("");

			formPagePensionados.clean();
			$("#beneficiarios").hide();
			
			$("#id_persona").prop("disabled", false);
			$("#idsiu").prop("disabled", false);
			$("#profesor").prop("disabled", false);
			$("#apellidos").prop("disabled", false);
			$("#nombres").prop("disabled", false);
			$("#sexo").prop("disabled", false);
			$("#fechanacimiento").prop("disabled", false);
			$("#nacionalidad").prop("disabled", false);
			$("#telefono").prop("disabled", false);
			$("#correo").prop("disabled", false);
			$("#cve_tipodepago").prop("disabled", false);

			$("#rfc").prop("disabled", false);
			$("#curp").prop("disabled", false);

			$("#direccion_pais").prop("disabled", false);
			$("#direccion_estado").prop("disabled", false);
			$("#direccion_ciudad").prop("disabled", false);
			$("#direccion_entidad").prop("disabled", false);
			$("#direccion_colonia").prop("disabled", false);
			$("#direccion_calle").prop("disabled", false);
			$("#direccion_numero").prop("disabled", false);
            
			$("#formbtnadd").prop("disabled", false);
			$("#formbtnsave").prop("disabled", true);
			$("#formbtndelete").prop("disabled", true);

			$("#Id_persona").prop("disabled", false);
		},

        edit: function (id) {
            Id_Persona = id;
			var model =
				{
                    Id_Persona: id,
                    cveSede: $("#Sedes").val(),
                }

            this.clean();

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/DatosPersonas/Edit/",
				data: model,
				success: function (data) {
				    data = jQuery.parseJSON(data);

					$('html, body').animate({ scrollTop: 0 }, 'fast');

                    if (data.Cve_Origen == "B") {
                        $("#origen").val('B');
						$("#id_persona").prop("disabled", true);
						$("#idsiu").prop("disabled", true);
						$("#profesor").prop("disabled", false);
						$("#apellidos").prop("disabled", true);
						$("#nombres").prop("disabled", true);
						$("#sexo").prop("disabled", true);
						$("#fechanacimiento").prop("disabled", true);
						$("#nacionalidad").prop("disabled", true);
						$("#telefono").prop("disabled", true);
						$("#correo").prop("disabled", true);
						$("#cve_tipodepago").prop("disabled", false);
						$("#rfc").prop("disabled", true);
                        $("#curp").prop("disabled", true);
                        $("#direccion_pais").prop("disabled", true);
						$("#direccion_estado").prop("disabled", true);
						$("#direccion_ciudad").prop("disabled", true);
						$("#direccion_entidad").prop("disabled", true);
						$("#direccion_colonia").prop("disabled", true);
						$("#direccion_calle").prop("disabled", true);
                        $("#direccion_numero").prop("disabled", true);
                    }
                    $("#id_persona").val(data.Id_Persona);
					$("#idsiu").val(data.Idsiu);
					$("#profesor").val(data.Profesor);
					$("#apellidos").val(data.Apellidos);
					$("#nombres").val(data.Nombres);
					$("#sexo").val(data.Sexo);
					$("#fechanacimiento").val(data.Fechanacimiento);
					$("#nacionalidad").val(data.Nacionalidad);
					$("#telefono").val(data.Telefono);
					$("#correo").val(data.Correo);
					$("#correo365").val(data.CorreoOffice);
                    $("#cve_tipodepago").val(data.Cve_Tipodepago);
                    if (data.Mysuite == "True")
						$("#recibos").attr('checked', true);
					else
						$("#recibos").attr('checked', false);
					$("#cve_origen").val(data.Cve_Origen);
					$("#rfc").val(data.Rfc);
                    $("#curp").val(data.Curp);

					$("input[name=rrazon][value=" + data.Datos_Fiscales + "]").attr('checked', 'checked');

					$("#direccion_pais").val(data.Direccion_Pais);
					$("#direccion_estado").val(data.Direccion_Estado);
					$("#direccion_ciudad").val(data.Direccion_Ciudad);
					$("#direccion_entidad").val(data.Direccion_Entidad);
                    $("#direccion_colonia").val(data.Direccion_Colonia);

                    $("#direccion_pais").html("<option>" + data.Direccion_Pais + "</option>");
					$("#direccion_estado").html("<option>" + data.Direccion_Estado + "</option>");
					$("#direccion_ciudad").html("<option>" + data.Direccion_Ciudad + "</option>");
					$("#direccion_entidad").html("<option>" + data.Direccion_Entidad + "</option>");
					$("#direccion_colonia").html("<option>" + data.Direccion_Colonia + "</option>");
                    
					$("#direccion_calle").val(data.Direccion_Calle);
					$("#direccion_numero").val(data.Direccion_Numero);
					$("#direccion_cp").val(data.Direccion_Cp);
					$("#cve_banco").val(data.Cve_Banco);
					$("#cuentaclabe").val(data.Cuentaclabe);
					$("#nocuenta").val(data.Nocuenta);
					$("#razonsocial").val(data.Razonsocial);
					$("#rz_rfc").val(data.Rz_Rfc);
					$("#rz_curp").val(data.Rz_Curp);

					$("#rz_direccion_pais").html("<option>" + data.Rz_Direccion_Pais + "</option>");
					$("#rz_direccion_estado").html("<option>" + data.Rz_Direccion_Estado + "</option>");
					$("#rz_direccion_ciudad").html("<option>" +data.Rz_Direccion_Ciudad + "</option>");
					$("#rz_direccion_entidad").html("<option>" +data.Rz_Direccion_Entidad + "</option>");
					$("#rz_direccion_colonia").html("<option>" +data.Rz_Direccion_Colonia + "</option>");
					$("#rz_direccion_calle").val(data.Rz_Direccion_Calle);
					$("#rz_direccion_numero").val(data.Rz_Direccion_Numero);
					$("#rz_direccion_cp").val(data.Rz_Direccion_Cp);

					$("#TituloProfesional").val(data.TituloProfesional);
					$("#Profesion").val(data.Profesion);
					$("#CedulaProfesional").val(data.CedulaProfesional);
					$("#FechaCedula").val(data.FechaCedula);
					$("#SeguroSocial").val(data.SeguroSocial);

                    $("#TipoPension").val(data.TipoPension);
                    $('#PersActivo').prop('checked', data.PersActivo);//		

                    formPagePensionados.setIdPersona(data.Id_Persona);
					$("#beneficiarios").show();
					DataTablePensionados.init();

					$("#id_persona").prop("disabled", true);

					$("#formbtnadd").hide();
					$("#formbtnsave").show();
					$("#formbtndelete").show();
					$("#formbtnadd").prop("disabled", true);
					$("#formbtnsave").prop("disabled", false);
                    $("#formbtndelete").prop("disabled", false);
                },
            });
		},

		save: function ()
		{
			var Idsiu = $("#idsiu").val();
			var model = this.createModel(Idsiu);
            
			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/DatosPersonas/Save/",
				data: model,
				success: function (data) {
					//formPage.clean();
					$('#notification').html(data.msg);
					DataTable.init();
				}
			});
		},

		delete: function (confirm) {

			if (!confirm) {
				$('#modal-delete-datospersonas').modal("show");
				return;
			}

			$('#modal-delete-datospersonas').modal("hide");

			var model = {
				Id_Persona: Id_Persona
			}

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/DatosPersonas/Delete/",
				data: model,
				success: function (data) {

					formPage.clean();
					$('#notification').html(data.msg);
					DataTable.init();
				}
			});
		},

		createModel: function (Idsiu) {
			return {
				Id_Persona:           Id_Persona,
				Idsiu:                Idsiu,
				Profesor:             ($("#profesor").val() == "") ? '0' : $("#profesor").val(),
				Noi:                  '0',
				Apellidos:            $("#apellidos").val(),
				Nombres:              $("#nombres").val(),
				Sexo:                 $("#sexo").val(),
				Fechanacimiento:      $("#fechanacimiento").val(),
				Nacionalidad:         $("#nacionalidad").val(),
				Telefono:             $("#telefono").val(),
				Correo:               $("#correo").val(),
				CorreoOffice:         $("#correo365").val(),
                Cve_Tipodepago:       $("#cve_tipodepago").val(),
				Cve_Origen:           $("#origen").val(),
				Datos_Fiscales:       $("input[name=rrazon]:checked").val(),
				Rfc:                  $("#rfc").val().toUpperCase(),
				Curp:                 $("#curp").val(),
				Cve_Banco:            $("#cve_banco").val(),
				Cuentaclabe:          $("#cuentaclabe").val(),
				Nocuenta:             $("#nocuenta").val(),
				Mysuite:              ($('#recibos').is(':checked')) ? $("#recibos").val() : 0,
				Direccion_Pais:       $("#direccion_pais").val(),
				Direccion_Estado:     $("#direccion_estado").val(),
				Direccion_Ciudad:     $("#direccion_ciudad").val(),
				Direccion_Entidad:    $("#direccion_entidad").val(),
				Direccion_Colonia:    $("#direccion_colonia").val(),
				Direccion_Calle:      $("#direccion_calle").val(),
				Direccion_Numero:     $("#direccion_numero").val(),
				Direccion_Cp:         $("#direccion_cp").val(),
				Razonsocial:          $("#razonsocial").val(),
				Rz_Rfc:               $("#rz_rfc").val().toUpperCase(),
				Rz_Curp:              $("#rz_curp").val(),
				Rz_Direccion_Pais:    $("#rz_direccion_pais").val(),
				Rz_Direccion_Estado:  $("#rz_direccion_estado").val(),
				Rz_Direccion_Ciudad:  $("#rz_direccion_ciudad").val(),
				Rz_Direccion_Entidad: $("#rz_direccion_entidad").val(),
				Rz_Direccion_Colonia: $("#rz_direccion_colonia").val(),
				Rz_Direccion_Calle:   $("#rz_direccion_calle").val(),
				Rz_Direccion_Numero:  $("#rz_direccion_numero").val(),
				Rz_Direccion_Cp:      $("#rz_direccion_cp").val(),
				TituloProfesional:    $("#TituloProfesional").val(),
				Profesion:            $("#Profesion").val(),
				CedulaProfesional:    $("#CedulaProfesional").val(),
				FechaCedula:          $("#FechaCedula").val(),
				SeguroSocial:         $("#SeguroSocial").val(),
				TipoPension:          $('#TipoPension').val(),
                PersActivo:           $('#PersActivo').is(':checked'),
                cveSede:              $('#Sedes').val(),
			}
		},

        add: function () {
			var Idsiu = "E" + $("#idsiu").val();
            var model = this.createModel(Idsiu);

            if (!formValidation.Validate())
			    return;
                  
			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/DatosPersonas/Add/",
				data: model,
				success: function (data) {
					formPage.clean();
					$('#notification').html(data.msg);

                    var ntype = $('#NotificationType').val();

                    if (ntype == "SUCCESS") {
                        var model =
                            {
                                Idsiu: Idsiu,
                                cveSede: $("#Sedes").val(),
                            }

					    $.ajax({
					        type: "POST",
					        dataType: 'json',
					        cache: false,
					        url: "/DatosPersonas/getIdPersona/",
					        data: model,
					        success: function (data) {

                                data = jQuery.parseJSON(data);
                                formPage.edit(data.Id_Persona);
                                $("#idsiu").prop("disabled", true);
                            },
                        });
                    }

                    DataTable.init();

					if (console)
						console.log(">>" + session_error(data));
				}
			});
		},

		onkeyup_cp: function () {
		    direccion1.CP = $("#" + direccion1.idCP).val();

		    if (direccion1.CP == '') {
		        direccion1.init(direccion1);
		    } else {
		        direccion1.setCP();
		    }
		},

		onkeyup_cp2: function () {
		    direccion2.CP = $("#" + direccion2.idCP).val();

		    if (direccion2.CP == '') {
		        direccion2.init(direccion2);
		    } else {
		        direccion2.setCP();
		    }
		},
	}
}();

var DataTable = function () {
    var pag = 1;
    var order = "ID_PERSONA";
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
            var datos = 'sedes=' + $('#Sedes').val();
            window.location.href = '/DatosPersonas/ExportExcel?' + datos;
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

        	var sede = $("#Sedes").val();
            
        	loading('loading-bar');
        	loading('loading-circle', '#datatable', 'Consultando datos..');

        	$.ajax({
        		type: "GET",
        		cache: false,
        		url: "/DatosPersonas/CreateDataTable/",
        		data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null"+ "&sede=" + sede,
        		success: function (msg) {
        			$('.loader-min-bar').hide();

        			$("#datatable").html(msg);
        		}
        	});
        }
    }
}();

//VALIDA RFC
function ValidaRFC(){
     
    var _valido = true; 
    var _rfc = $("#rfc").val();
    var _rz_rfc = $("#rz_rfc").val();

    if(_rfc!=""){
        _valido = rfcValido(_rfc);
        if(!_valido){  $('#notification').html(formValidation.getMessage("El RFC de la persona ingresado es INCORRECTO, favor de revisarlo.")); }
    }
    else if(_rz_rfc!=""){
        _valido = rfcValido(_rz_rfc);

        if(!_valido){  $('#notification').html(formValidation.getMessage("El RFC de la Razón Social ingresado es INCORRECTO, favor de revisarlo.")); }
        
    }

    return _valido;
}

function validarInputRFC(input) {
    var rfc         = input.value.trim().toUpperCase(),
        valido;
        
    var rfcCorrecto = rfcValido(rfc);   // ⬅️ Acá se comprueba
  
    if (rfcCorrecto) {
        
        $('#rfc').css('border','1px solid #ccd0d4');
    } else {
        $('#rfc').css('border','2px solid #ED1C24');
        
    }
}

function validarInputRFCRZ(input){

    var rfc = input.value.trim().toUpperCase(),
        valido;
        
    var rfcCorrecto = rfcValido(rfc);   // ⬅️ Acá se comprueba
  
    if (rfcCorrecto) {
        
        $('#rz_rfc').css('border','1px solid #ccd0d4');
    } else {
        $('#rz_rfc').css('border','2px solid #ED1C24');
        
    }

}

function rfcValido(rfc) {

    var aceptarGenerico = true;

    const re       = /^([A-ZÑ&]{3,4}) ?(?:- ?)?(\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])) ?(?:- ?)?([A-Z\d]{2})([A\d])$/;
    var   validado = rfc.match(re);

    if (!validado)  //Coincide con el formato general del regex?
        return false;

    //Separar el dígito verificador del resto del RFC
    const digitoVerificador = validado.pop(),
          rfcSinDigito      = validado.slice(1).join(''),
          len               = rfcSinDigito.length,

    //Obtener el digito esperado
          diccionario       = "0123456789ABCDEFGHIJKLMN&OPQRSTUVWXYZ Ñ",
          indice            = len + 1;
    var   suma,
          digitoEsperado;

    if (len == 12) suma = 0
    else suma = 481; //Ajuste para persona moral

    for(var i=0; i<len; i++)
        suma += diccionario.indexOf(rfcSinDigito.charAt(i)) * (indice - i);
    digitoEsperado = 11 - suma % 11;
    if (digitoEsperado == 11) digitoEsperado = 0;
    else if (digitoEsperado == 10) digitoEsperado = "A";

    //El dígito verificador coincide con el esperado?
    // o es un RFC Genérico (ventas a público general)?
    if ((digitoVerificador != digitoEsperado)
     && (!aceptarGenerico || rfcSinDigito + digitoVerificador != "XAXX010101000"))
        return false;
    else if (!aceptarGenerico && rfcSinDigito + digitoVerificador == "XEXX010101000")
        return false;
    return rfcSinDigito + digitoVerificador;
}