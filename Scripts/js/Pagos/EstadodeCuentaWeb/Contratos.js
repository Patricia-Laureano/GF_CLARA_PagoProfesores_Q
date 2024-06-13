var ciclo1 = new Ciclos('ciclos');
var periodo1 = new Periodos('periodos');

$(function () {
    $(window).load(function () {
        ciclo1.init("ciclo1");
    });
});//End function jquery

//cambios Edgar
$('#periodos').on('change', function () {
    ConsultarD();
});

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

function handlerdataCiclos() {
    periodo1.id_ciclo = $("#ciclos").val();
    periodo1.init("periodo1");
}

function handlerdataPeriodos() {
}

function ConsultarD() {
    var metodoControlle = "_Contratos";

    var data = 'filter_Sede=' + $('#Sedes').val()
        + '&periodos=' + $('#periodos').val();

    $.ajax({
        type: "POST",
        cache: false,
        url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
        data: data,
        success: function (data) {
            $("#Contratos").html(data.Contratos);
        }, error: function (msg) {
        }
    });
}

var Sedes = function () {
    "use strict"; return {
        setSedes: function () {
            var metodoControlle = "_Contratos";
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
                    $("#Contratos").html(data.Contratos);
                }, error: function (msg) {
                }
            });
        }
    }
}();