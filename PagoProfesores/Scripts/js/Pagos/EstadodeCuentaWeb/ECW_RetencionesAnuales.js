﻿var ciclo1 = new Ciclos('Anio');

$(function () {

    $(window).load(function () {
        ciclo1.init("ciclo1");
    });
});//End function jquery

function handlerdataCiclos() {
}

var formPage_RetencionesAnuales = function () {
    "use strict"; return {
        idEstadoCuenta: '',
		search: function () {
		},
		consultar: function () {
			DataTable_RetencionesAnuales.init();
		},
		//verRetencion: function (cve_retencion, cve_sede, mesi, mesf, anio, monto, monto_Iva, monto_IvaRet, montoIsrRet) {
        verRetencion: function (cve_retencion, cve_sede, anio, monto, monto_Iva, monto_IvaRet, montoIsrRet,id_esquema) {
			$.ajax({
				type: "GET",
				cache: false,
				url: '/ECW_RetencionesMensuales/SetRetencion',
				data: "cve_retencion=" + cve_retencion + "&cve_sede=" + cve_sede + "&mesi=1&mesf=12&anio=" + anio + "&monto=" + monto + "&monto_Iva=" + monto_Iva + "&monto_IvaRet=" + monto_IvaRet + "&montoIsrRet=" + montoIsrRet + "&id_esquema=" + id_esquema,
				success: function (msg) {
					if (msg == "0")
						window.open('/ECW_RetencionesMensuales/ConvertPDF');
				}
			});
		},
	}
}();

function filter() {
    var metodoControlle = "_ConstanciasRetencionAnual";
    var data = 'filter_Sede=' + $('#Sedes').val()
        + '&fechaAnio=' + $('#Anio').val();

    $.ajax({
        type: "POST",
        cache: false,
        url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
        data: data,
        success: function (data) {
            $("#ConstanciasRetencionAnual").html(data.ConstanciasRetencionAnual);
        }, error: function (msg) {
        }
    });
}

var Sedes = function () {
    "use strict"; return {
        setSedes: function () {
            var metodoControlle = "_ConstanciasRetencionAnual";
            var filter = $('#Sedes').val();

            $.blockUI({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src=""/><br><h3> Cambiando de campus...</h3>'
            });

            $.ajax({
                type: "POST",
                cache: false,
                url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
                data: "filter_Sede=" + filter,
                success: function (data) {
                    $.unblockUI();
                    $("#ConstanciasRetencionAnual").html(data.ConstanciasRetencionAnual);
                }, error: function (msg) {
                }
            });
        }
    }
}();

var DataTable_RetencionesAnuales = function () {
	var pag = 1;
	var order = "ANIODEPOSITO";
	var sortoption = {
		ASC: "ASC",
		DESC: "DESC"
	};
	var sort = sortoption.DESC

	"use strict"; return {
		myName: 'DataTable_RetencionesAnuales',
		idEstadoCuenta: '',
		onkeyup_colfield_check: function (e) {
			var enterKey = 13;
			if (e.which == enterKey) {
				pag = 1;
				this.init();
			}
		},

		exportExcel: function (table) {
            //var data = 'Periodo=' + $('#Periodo').val()
            //    + '&Sede=' + $('#Sedes').val();
            var data = 'Sede=' + $('#Sedes').val();
            window.location.href = '/ECW_RetencionesAnuales/ExportExcel?' + data;
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
			//var Periodo = $('#Periodo').val(); Periodo = Periodo ? Periodo : '';
			//var Nivel = $('#Nivel').val(); Nivel = Nivel ? Nivel : '';

            loading('loading-bar');
           
			$.ajax({
				type: "GET",
				cache: false,
				url: "/ECW_RetencionesAnuales/CreateDataTable",
				data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" +
					"&Sede=" + Sede,// + "&Periodo=" + Periodo + "&Nivel=" + Nivel,
				success: function (msg) {
				    $('.loader-min-bar').hide();
					$("#datatable-4").html(msg);
				}
			});
		}
	}
}();