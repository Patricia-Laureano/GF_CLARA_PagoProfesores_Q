
var ciclos = new Ciclos("ciclos");
var periodos = new Periodos("periodos");

$(function () {

	$(document).ready(function () {

		$('#' + DataTable.myName + '-fixed').fixedHeaderTable({
			altClass: 'odd',
			footer: true,
			fixedColumns: 2
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

		//niveles.init("niveles");

		$('#file_xlsx').fileupload({
			url: '/FileUploadHandler.ashx?upload=start',
			add: function (e, data) {
				var val = data.files[0].name.toLowerCase();
				var regex = new RegExp("(.*?)\.(xlsx)$");
				if (!(regex.test(val))) {
					$(this).val('');
					alert('Sólo archivos XLSX');
				}
				else data.submit();
			},
			progress: function (e, data) { },
			success: function (response, status) {
				$('#file_xlsx_name').val('' + this.files[0].name);
				$('#file_xlsx_name').show();
				$('#file_xlsx_link').hide();
				formPage.fileNameXLSX = response;
				$("#formbtnAuditar").prop("disabled", false);
			},
			error: function (error) { }
		});
		$('#file_xlsx_name').val('');
	});
});//End function jquery

function handlerdataCiclos() { 
}

function handlerdataNiveles() {   
}

var formPage = function () {

	"use strict"; return {

		fileNameXLSX: '',
		clean: function () {

			$("#Ciclo").val("");
			$("#Periodo").val("");
			//$("#Nivel").val("");
			$("#TipoDeContrato").val("");

			$('#notification').html('');
			$('#file_xlsx').prop('disabled', false);
			$('#file_xlsx').val('');
			this.fileChange();
			//$('#formbtnGenerar').prop('disabled', true);
			$('#formbtnGenerar').hide();
			$('#img_generando').hide();

			$('#file_xlsx_name').val('');

			var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
			for (var i = 0; i < maxBloqueos; i++)
				$('#TipoBloqueo_' + i).prop('checked', false);
        },

		fileChange: function () {
			var file = $('#file_xlsx').val();
			if (file == null || file == "")
				$('#formbtnAuditar').hide();
			else {
				$('#formbtnAuditar').show();
				$('#formbtnAuditar').prop('disabled', false);
			}
			$('#formbtnGenerar').hide();
		},

		subirExcel: function () {

			$('#formbtnGenerar').hide();
			$('#formbtnAuditar').prop('disabled', true);

			$.ajax({
				type: "POST",
				cache: false,
				url: "/EsquemasExcel/processExcel",
				data: 'filename=' + formPage.fileNameXLSX + '&sede=' + $('#Sedes').val(),
                success: function (msg) {

                    if (msg == "0") {
						$('#formbtnGenerar').show();

						$.ajax({
							type: "POST",
							cache: false,
							url: "/EsquemasExcel/FindFirstError",
							success: function (data) {
								$('#notification').html(data.msg);

								var ntype = $('#NotificationType').val();
								if (ntype == "SUCCESS") {
									$('#formbtnGenerar').show();
								}
								else if (ntype == "ERROR") {
									$('#formbtnGenerar').hide();
								}
							}
						});
					}
					DataTable.setShow();
				}
			});
		},

		generar: function () {
			var arrBloqueos = new Array();
			var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
			for (var i = 0; i < maxBloqueos; i++) {
				var bloqueo = $('#TipoBloqueo_' + i).val();
				var activo = $('#TipoBloqueo_' + i).prop('checked');
				if (activo)
					arrBloqueos.push(bloqueo);
			}

			$('#formbtnAuditar').prop('disabled', true);
			$('#formbtnGenerar').prop('disabled', true);

			$.ajax({
				type: 'POST',
				cache: false,
				url: '/EsquemasExcel/Generar',
				data: 'bloqueos=' + arrBloqueos.join(),
				success: function (data) {
					$('#notification').html(data.msg);
					DataTable.init();
				},
				complete: function (data) {
					$('#formbtnAuditar').prop('disabled', false);
					$('#formbtnGenerar').prop('disabled', false);

					$('#formbtnAuditar').hide();
					$('#formbtnGenerar').hide();
				},
			});
		}
	}
}();

var DataTable = function () {
	var pag = 1;
	var order = "ID_ESQUEMA";
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

		edit: function (id) {
		},

		setPage: function (page) {
			pag = page;
			this.init();
		},

		setShow: function () {  //update 
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
				url: "/EsquemasExcel/CreateDataTable/",
				data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",// + "&filter=" + $('#Sedes').val(),
				success: function (msg) {
					$("#datatable").html(msg);

					$('#' + DataTable.myName + '-fixed').fixedHeaderTable({
						altClass: 'odd',
						footer: true,
						fixedColumns: 2
					});
				}
			});
		}
	}
}();