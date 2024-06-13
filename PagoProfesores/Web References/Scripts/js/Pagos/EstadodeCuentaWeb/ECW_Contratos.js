
var formPage_Contratos = function () {
	"use strict"; return {
		search: function () {
		},
		consultar: function () {
			DataTable_Contratos.init();
		},
		verContrato: function (cve_contrato, cve_sede, periodo, cve_nivel, id_esquema, IDSIU) {
			
		    
		    $.ajax({
				type: "GET",
				cache: false,
				url: '/ECW_Contratos/SetContrato',
				data: "cve_contrato=" + cve_contrato + "&cve_sede=" + cve_sede + "&periodo=" + periodo + "&cve_nivel=" + cve_nivel + "&id_esquema=" + id_esquema + "&IDSIU=" + IDSIU,
				success: function (msg) {
					if (msg == "0")
						window.open('/ECW_Contratos/ConvertPDF');
				}
			});
		},
	}
}();





var Sedes = function () {

    "use strict"; return {

        setSedes: function () {
        

            var metodoControlle = "Home_Actualitation";

            var filter = $('#Sedes').val();


            $.ajax({
                type: "POST",
                cache: false,
                url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
                data: "filter_Sede=" + filter,
                success: function (data) {

                    $("#PagosPendientes").html(data.PagosPendientes);
                    $("#PagosDepositados").html(data.PagosDepositados);
                    $("#Contratos").html(data.Contratos);
                    $("#PagosxDepositar").html(data.PagosxDepositar);
                    $("#Bloqueos").html(data.Bloqueos);


                }, error: function (msg) {



                }
            });


        }
    }
}();


var DataTable_Contratos = function () {
	var pag = 1;
	var order = "CVE_CONTRATO";
	var sortoption = {
		ASC: "ASC",
		DESC: "DESC"
	};
	var sort = sortoption.ASC;

	"use strict"; return {
		myName: 'DataTable_Contratos',

		onkeyup_colfield_check: function (e) {
			var enterKey = 13;
			if (e.which == enterKey) {
				pag = 1;
				this.init();
			}
		},




		exportExcel: function (table) {

		  
	var data =
       'Periodo=' + $('#Periodo').val()
       + '&Sede=' + $('#Sedes').val();


	window.location.href = '/ECW_Contratos/ExportExcel?' + data;


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

			var show = $('#' + this.myName + '-data-elements-length').val();
			var search = $('#' + this.myName + '-searchtable').val();
			var orderby = order;
			var sorter = sort;

			var Sede = $('#Sedes').val(); Sede = Sede ? Sede : '';
			var Periodo = $('#Periodo').val(); Periodo = Periodo ? Periodo : '';
			var Nivel = $('#Nivel').val(); Nivel = Nivel ? Nivel : '';

			loading('loading-bar');

			$.ajax({
				type: "GET",
				cache: false,
				url: "/ECW_Contratos/CreateDataTable/",
				data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" +
					"&Sede=" + Sede + "&Periodo=" + Periodo + "&Nivel=" + Nivel,
				success: function (msg) {
				    $('.loader-min-bar').hide();
					$("#datatable-1").html(msg);
				}
			});
		}
	}
}();
