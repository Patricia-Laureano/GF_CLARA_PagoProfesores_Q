//var bancos1 = new Bancos('Banco');


$(function () {

	$(window).load(function () {
		
	    $("#Cuenta").val("");
	    $("#Clabe").val("");
	    $("#PensionadoNombre").val("");
	    $("#PensionadoPorcentaje").val("");
	    $("#Banco").val("");
	    $("#MontoFijo").val("");
	    $("#TipoPago").attr('checked', false);
	    $("#Comentarios").val("");


		//$("#formbtnsave").hide();
	   // formValidation.Inputs(["Cuenta", "Clabe", "PensionadoNombre", /*"Banco", "TipoPension"*/]);
	    //formValidation.notEmpty('Cuenta', 'El campo Cuenta no debe de estar vacio');
	   // formValidation.notEmpty('Clabe', 'El campo Clabe no debe de estar vacio');
	    //formValidation.notEmpty('TipoPension', 'El campo Tipo de Pension no debe de estar vacio');
	    formValidation.notEmpty('PensionadoNombre', 'El campo PensionadoNombre no debe de estar vacio');
	    formValidation.notEmpty('banco', 'El campo Banco no debe estar vacio');

		//formValidation.onlyNumbers('clave');
	  //  bancos1.init("bancos1");
		
	});
});//End function jquery



$('#Cuenta').keypress(function (e) {


    $("#Cuenta").removeClass("form-control parsley-error").addClass("form-control");
    $("#Clabe").removeClass("form-control parsley-error").addClass("form-control");

});


$('#Clabe').keypress(function (e) {


    $("#Clabe").removeClass("form-control parsley-error").addClass("form-control");
    $("#Cuenta").removeClass("form-control parsley-error").addClass("form-control");

});




var formPagePensionados = function () {

	var PK1;
	var IdPersona;
	var IdPensionado;
	var IDSIU = '';

	"use strict"; return {

		clean: function () {
			//formValidation.Clean();

			$("#PensionadoId").val("");	//		
			$('#PensionadoActivo').prop('checked', true);//

			$('#PensionadoId').prop('disabled', false);//

			$('#formbtnAgregarPensionado').hide();//
			$('#formbtnGuardarPensionado').hide();//
            //nuevo		
			$("#formbtnadd2").show();
			$("#formbtnsave2").hide();
			$("#formbtndelete2").hide();

			$("#Cuenta").val("");
			$("#Clabe").val("");
            $("#PensionadoNombre").val("");
            $("#PensionadoApellidoP").val("");
            $("#PensionadoApellidoM").val("");
			$("#PensionadoPorcentaje").val("");
			$("#Banco").val("");
			$("#MontoFijo").val("");
			$("#TipoPago").attr('checked', false);		   
			$("#Comentarios").val("");

			var $miSelect = $('#TipoPension');
			$miSelect.val($miSelect.children('option:first').val());
            
			$("#MontoFijo").removeClass("form-control parsley-error").addClass("form-control");
			$("#PensionadoPorcentaje").removeClass("form-control parsley-error").addClass("form-control");
			$("#Cuenta").removeClass("form-control parsley-error").addClass("form-control");
			$("#Banco").removeClass("form-control parsley-error").addClass("form-control");
			$("#Clabe").removeClass("form-control parsley-error").addClass("form-control");
            $("#PensionadoNombre").removeClass("form-control parsley-error").addClass("form-control");
            $("#PensionadoApellidoP").removeClass("form-control parsley-error").addClass("form-control");
            $("#PensionadoApellidoM").removeClass("form-control parsley-error").addClass("form-control");

		    //$('input:radio[name=TipoPago]:checked').val()
		},

		setIdPersona : function (id){
			IdPersona = id;
		},

		edit: function (id) {

			PK1 = id;

			var model = {
				PK1: PK1
			}

			this.clean();

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Pensionados/Edit/",
				data: model,
				success: function (data) {
                    data = jQuery.parseJSON(data);





					$('html, body').animate({ scrollTop: 0 }, 'fast');

					IdPensionado = data.IdPensionado;

					$('#PensionadoId').val(data.IDSIU);
                    $('#PensionadoNombre').val(data.Nombre);
                    $('#PensionadoApellidoP').val(data.ApellidoP);
                    $('#PensionadoApellidoM').val(data.ApellidoM);
					$('#PensionadoPorcentaje').val(data.Porcentaje);
					$('#PensionadoActivo').prop('checked', data.Activo);//					

					if (data.TipoPension == "M") { $('#dMontoFijo').show(); $('#dPensionadoPorcentaje').hide(); }
					else { $('#dMontoFijo').hide(); $('#dPensionadoPorcentaje').show(); }

					if(data.TipoPago=="T"){$('input:radio[name=TipoPago][value=T]').attr('checked', true);}
                    else{$('input:radio[name=TipoPago][value=C]').attr('checked', true);}					

					$('#Cuenta').val(data.Cuenta);
					$('#Clabe').val(data.Clabe);
					$('#Banco').val(data.Banco);
					$('#MontoFijo').val(data.MontoFijo);
					$('#Comentarios').val(data.Comentarios);					

					$("#TipoPension option[value='" + data.TipoPension + "']").attr("selected", "selected");
                      //nuevo
		
					$("#formbtnadd2").hide();
					$('#formbtnsave2').prop('disabled', false);
			         $("#formbtnsave2").show();

					$('#PensionadoId').prop('disabled', true);//
					$('#formbtnAgregarPensionado').hide();//
					$('#formbtnGuardarPensionado').show();//
				}
			});
		},

		createModel: function (PK1) {
			return {
				PK1: PK1,
				IdPersona: IdPersona,
				IdPensionado: IdPensionado,
                Nombre: $("#PensionadoNombre").val(),
                ApellidoP: $("#PensionadoApellidoP").val(),
                ApellidoM: $("#PensionadoApellidoM").val(),
				Porcentaje: $('#PensionadoPorcentaje').val(),
				TipoPago:$('input:radio[name=TipoPago]:checked').val(),
				Cuenta: $("#Cuenta").val(),
				Clabe: $("#Clabe").val(),
				Banco: $("#Banco").val(),
				MontoFijo: $("#MontoFijo").val(),
				Comentarios: $("#Comentarios").val(),
				TipoPension: $("#TipoPension").val(),
				Activo: $('#PensionadoActivo').is(':checked')
			}
		},

        save: function () {



            if (!formPagePensionados.validateInput3())
                return;


			var model = this.createModel(PK1);

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Pensionados/ConsultaPorcentaje/",
				data: model,
				success: function (data) {

					var suma = parseFloat('' + data.Porcentaje) + parseFloat(model.Porcentaje);

					if (suma > 99.9)
						//$('#modal-delete-alert').show();
						$('#modal-alert').modal("show");
						//alert("La suma de los porcentajes debe ser menor a 100%");
					else {

						$.ajax({
							type: "POST",
							dataType: 'json',
							cache: false,
							url: "/Pensionados/Save/",
							data: model,
							success: function (data) {
								formPagePensionados.clean();
								$('#notification').html(data.msg);
                                  //nuevo
		
					$("#formbtnadd2").show();
					//$('#formbtnsave2').prop('disabled', false);
			         $("#formbtnsave2").hide();
					DataTablePensionados.init();
							}
						});
					}

				}
			});


		},

		add: function () {

		    $("#MontoFijo").removeClass("form-control parsley-error").addClass("form-control");
		    $("#PensionadoPorcentaje").removeClass("form-control parsley-error").addClass("form-control");

		    if (!formPagePensionados.validateInput3())
		        return;

		    var TipoPension = $("#TipoPension").val();
		    if (TipoPension == "M") {
		        //validar que monto fijo no este vacio
		        if (!formPagePensionados.validateInput1())
		            return;		      

		    } else {
		        //vaidar que PensionadoPorcentaje no este vacio
		        if (!formPagePensionados.validateInput2())
		            return;
            }

			var model = this.createModel(0);
			model.PK1 = 0;

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Pensionados/ConsultaPorcentaje/",
				data: model,
				success: function (data) {

					var suma = parseFloat('' + data.Porcentaje) + parseFloat(model.Porcentaje);

					if (suma > 99.9)
						//$('#modal-delete-alert').show();
						$('#modal-alert').modal("show");
						//alert("La suma de los porcentajes debe ser menor a 100%");
					else {
						$.ajax({
							type: "POST",
							dataType: 'json',
							cache: false,
							url: "/Pensionados/Add/",
							data: model,
							success: function (data) {

								formPagePensionados.clean();
								$('#notification').html(data.msg);
								DataTablePensionados.init();
							}
						});
					}
				}
			});

			/*
			if (!formValidation.Validate())
				return;
			//*/
		},

		validateInput1: function () {
		    var validado = true;
		    if ($("#MontoFijo").val() == "") { $("#MontoFijo").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
		    //  else { $("#MontoFijo").removeClass("form-control parsley-error").addClass("form-control"); }
		    return validado;
		},

		validateInput2: function () {
		    var validado = true;
		    if ($("#PensionadoPorcentaje").val() == "") { $("#PensionadoPorcentaje").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
		    //  else { $("#MontoFijo").removeClass("form-control parsley-error").addClass("form-control"); }
		    return validado;
		},

		validateInput3: function () {
		    var validado = true;
		
		    if ($("#TipoPension").val() == "") { $("#TipoPension").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
		    else { $("#TipoPension").removeClass("form-control parsley-error").addClass("form-control"); }



		    if ($("#Cuenta").val() != "") {

		        $("#Cuenta").removeClass("form-control parsley-error").addClass("form-control");		       
		    }
		    else {

		        if ($("#Clabe").val() == "") {//cuenta == ""
		            $("#Cuenta").removeClass("form-control").addClass("form-control parsley-error");
		            $("#Clabe").removeClass("form-control").addClass("form-control parsley-error");
		            validado = false;
		        }		       
		    }


		    if ($("#Clabe").val() != "") {

		        $("#Clabe").removeClass("form-control parsley-error").addClass("form-control");
		    }
		    else {

		        if ($("#Cuenta").val() == "") {//Clabe == ""
		            $("#Clabe").removeClass("form-control").addClass("form-control parsley-error");
		            $("#Cuenta").removeClass("form-control").addClass("form-control parsley-error");
		            validado = false;
		        }
		    }




		  /*  if ($("#Cuenta").val() == "") { $("#Cuenta").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
		    else { $("#Cuenta").removeClass("form-control parsley-error").addClass("form-control"); }

		    if ($("#Clabe").val() == "") { $("#Clabe").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
		    else { $("#Clabe").removeClass("form-control parsley-error").addClass("form-control"); }*/


		    if ($("#PensionadoNombre").val() == "") { $("#PensionadoNombre").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
		    else { $("#PensionadoNombre").removeClass("form-control parsley-error").addClass("form-control"); }
		    if ($("#Banco").val() == "") { $("#Banco").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
		    else { $("#Banco").removeClass("form-control parsley-error").addClass("form-control"); }
		    return validado;
        },

		borrar: function(id){
			PK1 = id;
			this.delete(false);
		},

		delete: function (confirm) {

			if (!confirm) {
				$('#modal-delete-pensionado').modal("show");
				return;
			}

			$('#modal-delete-pensionado').modal("hide");
			var model = {
				PK1: PK1
			}

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Pensionados/Delete/",
				data: model,
				success: function (data) {

					formPagePensionados.clean();
					$('#notification').html(data.msg);
					DataTablePensionados.init();
				}
			});
		},

		PensionadoId_onkeypress: function (e) {
			if (e.which == 13) {
				IDSIU = $('#PensionadoId').val();
				this.BuscaPersona();
			}
		},

		BuscaPersona: function () {
			var model =
				{
					IdPersona: IdPersona,
					IDSIU: IDSIU
				};
			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Pensionados/BuscaPersona/",
				data: model,
				success: function (data) {
					if (data.msg == null) {
						data = jQuery.parseJSON(data);
						$('#PensionadoNombre').val(data.Nombre);
						//IdPensionado = data.IdPensionado;
						if (data.found) {
							PK1 = data.PK1;
							$('#PensionadoPorcentaje').val(data.Porcentaje);
							$('#PensionadoActivo').prop('checked', data.Activo);

							$('#formbtnAgregarPensionado').hide();
							$('#formbtnGuardarPensionado').show();
						}
						else {
							IdPensionado = data.IdPensionado;
							$('#PensionadoPorcentaje').val('');
							$('#PensionadoActivo').prop('checked', true);

							$('#formbtnAgregarPensionado').show();
							$('#formbtnGuardarPensionado').hide();
						}
						$('#notification').html('');
					}
					else { // No se encontro la persona
						formPagePensionados.clean();
						$('#PensionadoId').val(IDSIU);

						$('#notification').html(data.msg);
					}
				}
			});
		}
	}
}();


var DataTablePensionados = function () {
	var pag = 1;
	var order = "BENEFICIARIO";
	var sortoption = {
		ASC: "ASC",
		DESC: "DESC"
	};
	var sort = sortoption.ASC;

	"use strict"; return {
		myName: 'DataTablePensionados',

		onkeyup_colfield_check: function (e) {
			var enterKey = 13;
			if (e.which == enterKey) {
				pag = 1;
				this.init();
			}
		},


	
		exportExcel: function (table) {

		   window.location.href = '/Pensionados/ExportExcel';

		},

		edit: function (id) {
			formPagePensionados.edit(id);
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
			show = show ? show : '';
			var search = $('#' + this.myName + '-searchtable').val();
			search = search ? search : '';
			var orderby = order;
			var sorter = sort;

			$.ajax({
				type: "GET",
				cache: false,
				url: "/Pensionados/CreateDataTable/",
				data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
				success: function (msg) {
					$("#datatablePensionados").html(msg);
				}
			});
		}
	}
}();


