
var formPage_EstadoCuenta = function () {
	"use strict"; return {
		idEstadoCuenta: '',
		fileNameXML: '',
		fileNamePDF: '',
		search: function () {
		},
		consultar: function () {
			DataTable_EstadoCuenta.init();
		},
		subirXML: function (confirmar, idEstadoCuenta) {
			if (!confirmar) {
				$('#modal-xml').modal("show");
				$('#file_xml_name').html('');
				$('#file_pdf_name').html('');
				$('#gif_subir_xml').hide();
			//	$('#xml_result').hide();
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
				ValidarXML: true,
				ValidarMySuite: true,
				Sede: $('#Sedes').val(),
			}
			$.ajax({
				type: "POST",
				cache: false,
				url: "/ECW_EstadoCuenta/processXML",
				data: model,
				success: function (msg) {
					if (msg == "0") {
					    //$('#modal-xml').modal("hide");
					    location.reload();
					}
					else {
						$('#xml_result').html(msg);
						//$('#xml_result').show();
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
				url: "/ECW_EstadoCuenta/solicitarPago",
				data: model,
				success: function (msg) {
					//$('#notification').
					//$('#modal-solicitar').modal("hide");
					location.reload();
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

var DataTable_EstadoCuenta = function () {
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
	};
	var filtro = FILTRO_OPTIONS.PENDIENTES;

	"use strict"; return {
		myName: 'DataTable_EstadoCuenta',
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
        
		exportExcel: function (table) {	   

            var data = 'Periodo=' + $('#Periodo').val()
                + '&IdSiu=' + $('#ECW_IDSIU').val()
                + '&filtro=' + filtro
                + '&fltrSedes=' + $('#Sedes').val()
                + '&fltrEscuela='
                + '&fltrTipoContr='
                + '&fltrCCost='
                + '&fltrPagoI='
                + '&fltrPagoF='
                + '&fltrReciI='
                + '&fltrReciF='
                + '&fltrDispI='
                + '&fltrDispF='
                + '&fltrDepoI='
                + '&fltrDepoF=';

            window.location.href = '/ECW_EstadoCuenta/ExportExcel?' + data;
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
            var fltrIdSiu = $("#ECW_IDSIU").val();
            var Sede = $('#Sedes').val(); Sede = Sede ? Sede : '';
			var Periodo = $('#Periodo').val(); Periodo = Periodo ? Periodo : '';
			var Nivel = $('#Nivel').val(); Nivel = Nivel ? Nivel : '';

			loading('loading-bar');

			$.ajax({
				type: "GET"
				,cache: false
				,url: "/ECW_EstadoCuenta/CreateDataTable"
				//url: "/EstadodeCuenta/CreateDataTable",
			//	data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&filtro=" + filtro + "&Sede=" + Sede + "&Periodo=" + Periodo + "&Nivel=" + Nivel,
				,data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&fltrIdSiu=" + fltrIdSiu + "&filtro=" + filtro + "&Sede=" + Sede + "&Periodo=" + Periodo + "&Nivel=" + Nivel
				, success: function (msg) {
				    $('.loader-min-bar').hide();
					$("#datatable-2").html(msg);
				}
			});
		}
	}
}();
