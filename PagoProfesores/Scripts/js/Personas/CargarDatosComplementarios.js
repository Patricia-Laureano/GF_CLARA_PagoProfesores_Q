
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

			//var xhr = new XMLHttpRequest();
			//var formData = new FormData();
			//formData.append("file", file.files[0]);
			//xhr.open("post", "/CargarDatosComplementarios/SendFile", true);
			//xhr.send(formData);

			//xhr.addEventListener("error", function (e) {
			
			//}, false);
			//xhr.addEventListener("load", function (e) {

			//}, false);
			

			startTimer("CargarDatosComplementarios", function ()
			{
				$.ajax({
					type: 'POST',
					cache: false,
					url: '/CargarDatosComplementarios/FindFirstError',
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

		importar: function () {
			
			var option = confirm("¿Los datos serán reemplazados?");
			if (option != true) {
				return;
			}

			loading('loading-bar');
			loading('loading-circle', '#datatable', 'Actualizando datos..');
			loading('loading-circle', '#seccion1', 'Importando Personas desde EXCEL..');

			$.ajax({
				type: "POST",
				cache: false,
				url: "/CargarDatosComplementarios/importar",
				data: {},
				success: function (msg) {
					$('#seccion1').unblock();
					DataTable.init();

					$('#notification').html(msg);
				},
				error: function (msg) {
					$('#seccion1').unblock();
					$('#notification').html(msg);
				}
			});
		},
	}
}();


var DataTable = function () {
	var pag = 1;
	var order = "ID_PBI_TMP";
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
				url: "/CargarDatosComplementarios/CreateDataTable/",
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