
var ciclos = new Ciclos("ciclos");
var periodos = new Periodos("periodos");
var niveles = new Niveles("niveles");

$(function () {
    $(document).ready(function () {

        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 1
        });
    });

	$(window).load(function () {
		formPage.clean();

		ciclos.init('ciclos',
			function (obj, data) {
				$('#ciclos').trigger('change');
			},
			function () {
				periodos.id_ciclo = $('#ciclos').val(); //this.value;
				periodos.init("periodos");
			}
		);
		
		niveles.init("niveles");

	});
});//End function jquery


function handlerdataCiclos() { 
}

function handlerdataNiveles() {   
}

var formPage = function () {

	"use strict"; return {

		clean: function () {
			//formValidation.Clean();

			$("#Ciclo").val("");
			$("#Periodo").val("");
			$("#Nivel").val("");
			$("#TipoDeContrato").val("");

			$('#notification').html('');
			$('#file').prop('disabled', false);
			$('#file').val('');
			this.fileChange();
			//$('#formbtnGenerar').prop('disabled', true);
			$('#formbtnGenerar').hide();
			$('#formbtnGenerarEdoCta').hide();
			$('#btn-sgt').hide();
			$('#img_generando').hide();
			initProgressBar();
		},
		fileChange: function () {
			var file = $('#file').val();
			if (file == null || file == "")
				$('#formbtnAuditar').hide();
			else
				$('#formbtnAuditar').show();
			//('disabled', true);
		},

		subirExcel: function () {

			//$('#file').prop('disabled', true);
			//$('#formbtnGenerar').prop('disabled', true);
		    $('#formbtnGenerar').hide();
		    $('#formbtnGenerarEdoCta').hide();

			startTimer("NominaExcel", function ()
			{
				$.ajax({
					type: 'POST',
					cache: false,
					url: '/NominaExcel/FindFirstError',
					success: function (data) {
						$('#notification').html(data.msg);

						var ntype = $('#NotificationType').val();
						if (ntype == "SUCCESS") {
						    $('#btn-sgt').show();
						    $("#formbtnAuditar").show();
							//$('#formbtnGenerar').show();
						}
						else if (ntype == "ERROR") {
						    //$('#btn-sgt').show();
							//$('#formbtnGenerar').hide();
						}
						//$('#file').val('');
					},
					error: function (msg) { }
				});
				DataTable.setShow();
			});
		},

		generar: function () {
			//$('#formbtnLimpiar').prop('disabled', true);
			//$('#formbtnAuditar').prop('disabled', true);
			//$('#formbtnGenerar').prop('disabled', true);
			//$('#formbtnGenerarEdoCta').prop('disabled', true);
			$('#img_generando').show();
			$.ajax({
				type: 'POST',
				cache: false,
				url: '/NominaExcel/generar',
				complete: function(data){
					//$('#formbtnLimpiar').prop('disabled', false);
					//$('#formbtnAuditar').prop('disabled', false);
					//$('#formbtnGenerar').prop('disabled', false);
					//$('#formbtnGenerarEdoCta').prop('disabled', false);
				    //$('#formbtnGenerarEdoCta').show();
				    $('#img_generando').hide();
				    $("#linksgt").attr("href", "javascript:formPage.BotonSgtGenerarEstadodecuenta()");
				    $('#btn-sgt').show();
				},
				success: function (data) {
					$('#notification').html(data.msg);
					DataTable.init();
				}
			});
		},

		generarEdoCta: function () {
		    $('#formbtnLimpiar').prop('disabled', true);
		    $('#formbtnAuditar').prop('disabled', true);
		    $('#formbtnGenerar').prop('disabled', true);
		    $('#formbtnGenerarEdoCta').prop('disabled', true);
            $('#img_generando').show();

		    $.ajax({
		        type: 'POST',
		        cache: false,
                url: '/NominaExcel/generarEdoCta',
		        complete: function (data) {
		            $('#formbtnLimpiar').prop('disabled', false);
		            $('#formbtnAuditar').prop('disabled', false);
		            $('#formbtnGenerar').prop('disabled', false);
		            //$('#formbtnGenerarEdoCta').prop('disabled', false);
		            $('#img_generando').hide();
		        },
		        success: function (data) {
		            $('#notification').html(data.msg);
		            DataTable.init();
		        }
		    });
		},

		BotonSgtGenerarNomina: function () {
		    $("#titulo-legend").html('<span>Generar la nomina de la reciente información auditada.</span>');

		    $('#btn-ant').show();
		    $('#btn-sgt').hide();
		    $('#formbtnGenerar').show();
		    $("#formbtnAuditar").hide();
		    $('#formbtnLimpiar').hide();
		    $('#uploadFile').hide();
		    $('#ligaUpload').hide();

            $("#tab-excel").removeClass("active").addClass("");
            $("#tab-nomina").addClass("active");

            $("#linkant").attr("href", "javascript:formPage.BotonAntSubirExcel()");
	    //$("#linksgt").attr("href", "javascript:formPage.BotonSgtCalcularNomina()");
		},

		BotonAntSubirExcel: function () {
		    $("#titulo-legend").html('<span>Seleccione un archivo de excel para importar los datos.</span>');

		    $('#btn-ant').hide();
		    //$('#btn-sgt').show();
		    $('#formbtnGenerar').hide();
		    $("#formbtnAuditar").hide();
		    $('#formbtnLimpiar').show();
		    $('#uploadFile').show();
		    $('#ligaUpload').show();
            
		    $("#tab-nomina").removeClass("active").addClass("");
		    $("#tab-estadodecuenta").removeClass("active").addClass("");
		    $("#tab-excel").addClass("active");
		    $("#linksgt").attr("href", "javascript:formPage.BotonSgtGenerarNomina()");
		},

		BotonSgtGenerarEstadodecuenta: function () {
		    $("#titulo-legend").html('<span>Generar el estado de cuenta de la nomina calculada.</span>');

		    $("#linkant").attr("href", "javascript:formPage.BotonAntCalcularNomina()");
		    $('#btn-ant').show();
		    $('#btn-sgt').show();
		    $('#formbtnAuditar').hide();
		    $('#formbtnGenerar').hide();
		    $('#formbtnLimpiar').hide();
		    $('#formbtnGenerarEdoCta').prop('disabled', false);
		    $("#formbtnGenerarEdoCta").show();
		    $('#uploadFile').hide();
		    $('#ligaUpload').hide();

		    $("#tab-nomina").removeClass("active").addClass("");
		    $("#tab-estadodecuenta").addClass("active");
		    $("#linksgt").attr("href", "javascript:formPage.BotonSgtEstadodeCuenta()");
		},

		BotonAntCalcularNomina: function () {
		    $("#titulo-legend").html('<span>Generar la nomina de la reciente información auditada.</span>');

		    $('#btn-ant').show();
		    //$('#btn-sgt').show();
		    $('#formbtnGenerar').show();
		    $('#formbtnGenerarEdoCta').hide();
		    $("#formbtnAuditar").hide();
		    $('#formbtnLimpiar').hide();
		    $('#uploadFile').hide();
		    $('#ligaUpload').hide();

		    $("#tab-estadodecuenta").removeClass("active").addClass("");
		    $("#tab-nomina").addClass("active");

		    $("#linkant").attr("href", "javascript:formPage.BotonAntSubirExcel()");
		    //$("#linksgt").attr("href", "javascript:formPage.BotonSgtGenerarEstadodecuenta()");
		},

		BotonSgtEstadodeCuenta: function () {
		    window.location.href = '/EstadodeCuenta';
		}
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

		onkeyup_colfield_check: function (e) {
			var enterKey = 13;
			if (e.which == enterKey) {
				pag = 1;
				this.init();
			}
		},
		/*
		exportExcel: function (table) {

			window.location.href = '/Bancos/ExportExcel';

		},
		//*/
		edit: function (id) {
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

			$.ajax({
				type: "GET",
				cache: false,
				url: "/NominaExcel/CreateDataTable/",
				data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
				success: function (msg) {
				    $("#datatable").html(msg);				   

				    $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
				            altClass: 'odd',
				            footer: true,
				            fixedColumns: 1
				        });
				}
			});
		}
	}
}();