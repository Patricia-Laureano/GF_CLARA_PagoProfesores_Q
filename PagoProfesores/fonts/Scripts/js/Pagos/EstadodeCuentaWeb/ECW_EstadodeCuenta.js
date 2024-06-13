
var formPageEstadoCuenta = function () {
	"use strict"; return {
		idEstadoCuenta: '',
		fileNameXML: '',
		fileNamePDF: '',
		//concepto: '',
		search: function () {
		},
		consultar: function () {
			DataTableEstadoCuenta.init();
		},
		subirXML: function (confirmar, idEstadoCuenta) {
			if (!confirmar) {
				$('#modal-xml').modal("show");
				$('#file_xml_name').html('');
				$('#file_pdf_name').html('');
				$('#gif_subir_xml').hide();
				$('#xml_msg').hide();
				this.idEstadoCuenta = idEstadoCuenta;
				this.fileNameXML = '';
				this.fileNamePDF = '';
				return;
			}
			if (this.fileNameXML == '') {
				alert('Falta especificar el archivo XML');
				return;
			}
			if (this.fileNamePDF == '') {
				alert('Falta especificar el archivo PDF');
				return;
			}
			$('#gif_subir_xml').show();
			var model = {
				ID_ESTADODECUENTA: this.idEstadoCuenta,
				FileNameXML: this.fileNameXML,
				FileNamePDF: this.fileNamePDF,
			}
			$.ajax({
				type: "POST",
				cache: false,
				url: "/EstadodeCuentaWeb/processXML",
				data: model,
				success: function (msg) {
					if (msg == "0") {
						$('#modal-xml').modal("hide");
					}
					else {
						$('#xml_msg').html(msg);
						$('#xml_msg').show();
						$('#gif_subir_xml').hide();
					}

					
				}
			});
		},
		solicitar: function (confirmar, idEstadoCuenta, concepto) {
			if (!confirmar) {
				this.idEstadoCuenta = idEstadoCuenta;
				$('#concepto').html(concepto);
				$('#gif_solicitando').hide();
				$('#modal-solicitar').modal("show");
				return;
			}
			$('#gif_solicitando').show();
			var model = { ID_ESTADODECUENTA : this.idEstadoCuenta};
			$.ajax({
				type: "POST",
				cache: false,
				url: "/EstadodeCuentaWeb/solicitarPago",
				data: model,
				success: function (msg) {
					//$('#notification').
					$('#modal-solicitar').modal("hide");
				}
			});
		},
		verXML: function (obj) {
			var path = $(obj).data('file');
			window.open(path);
		},
		verPDF: function (obj) {
			var path = $(obj).data('file');
			window.open(path);
		},
	}
}();

var DataTableEstadoCuenta = function () {
	var pag = 1;
	var order = "CONCEPTO";
	var sortoption = {
		ASC: "ASC",
		DESC: "DESC"
	};
	var sort = sortoption.ASC;

	var FILTRO_OPTIONS = {
		BLOQUEADOS: 'BLOQUEADOS',
		PENDIENTES: 'PENDIENTES',
		ENTREGADOS: 'ENTREGADOS',
		DEPOSITADOS: 'DEPOSITADOS',

		RETENCIONES_TODAS: 'RETENCIONES_TODAS',
		RETENCIONES_MENSUALES: 'RETENCIONES_MENSUALES',
		RETENCIONES_ANUALES: 'RETENCIONES_ANUALES',
	};
	var filtro = FILTRO_OPTIONS.BLOQUEADOS;
	var filtroFecha = FILTRO_OPTIONS.RETENCIONES_TODAS;

	"use strict"; return {
		myName: 'DataTableEstadoCuenta',
		idEstadoCuenta: '',
		onkeyup_colfield_check: function (e) {
			var enterKey = 13;
			if (e.which == enterKey) {
				pag = 1;
				this.init();
			}
		},
		setFiltro: function (value) {
			switch (value) {
				case 1: filtro = FILTRO_OPTIONS.BLOQUEADOS; break;
				case 2: filtro = FILTRO_OPTIONS.PENDIENTES; break;
				case 3: filtro = FILTRO_OPTIONS.ENTREGADOS; break;
				case 4: filtro = FILTRO_OPTIONS.DEPOSITADOS; break;
			}
		},
		setFiltroFecha: function (value) {
			switch (value) {
				case 1: filtroFecha = FILTRO_OPTIONS.RETENCIONES_TODAS; break;
				case 2: filtroFecha = FILTRO_OPTIONS.RETENCIONES_MENSUALES; break;
				case 3: filtroFecha = FILTRO_OPTIONS.RETENCIONES_ANUALES; break;
			}
		},

		exportExcel: function (table) {
			window.location.href = '/EstadodeCuentaWeb/ExportExcel';
		},

		edit: function (id) {
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

			var show = $('#' + this.myName + '-data-elements-length').val(); show = show ? show : '10';
			var search = $('#' + this.myName + '-searchtable').val(); search = search ? search : '';
			var orderby = order;
			var sorter = sort;

			var Sede = $('#Sedes').val(); Sede = Sede ? Sede : '';
			var Periodo = $('#Periodo').val(); Periodo = Periodo ? Periodo : '';
			var Nivel = $('#Nivel').val(); Nivel = Nivel ? Nivel : '';
			var month = "";
			var year = "";


			loading('loading-bar');
			

			$.ajax({
				type: "GET",
				cache: false,
				url: "/EstadodeCuentaWeb/CreateDataTable_EstadoCuenta",
				data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" +
					"&filtro=" + filtro + "&filtroFecha=" + filtroFecha + "&Sede=" + Sede + "&Periodo=" + Periodo + "&Nivel=" + Nivel,
				success: function (msg) {
				    $('.loader-min-bar').hide();
					$("#datatable-2").html(msg);
				}
			});
		}
	}
}();
