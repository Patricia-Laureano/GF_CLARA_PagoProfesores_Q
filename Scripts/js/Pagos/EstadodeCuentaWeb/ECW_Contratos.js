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

function validaCuenta() {
    if (!$("#valida_check").prop('checked')) {
        alert('Necesitas seleccionar la caja de verificación.');
        return;
    }

    var metodoControlle = "ValidaCuenta";
    var filter = $('#Sedes').val();
    var cuenta = $('#cuenta').val();
    
    $.blockUI({
        css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
        overlayCSS: { backgroundColor: '#FFF' },
        message: '<img src="" /><br><h3> Espere un momento..</h3>'
    });
    
    $.ajax({
        type: "POST",
        cache: false,
        url: "/EstadodeCuentaWeb/" + metodoControlle + "/",
        data: "filter_Sede=" + filter + "&NoCuenta=" + cuenta,
        success: function (data) {
            $.unblockUI();
            $("#valida_check").attr("disabled", true);
            //$("#btn_valida").attr('value', 'Verificado');
            $("#btn_valida").text("Verificado")
            $("#btn_valida").attr("disabled", true);
        }, error: function (msg) {
        }
    });
}

var Sedes = function () {
    "use strict"; return {
        setSedes: function () {
            var metodoControlle = "Home_Actualitation";
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
                    $("#PagosPendientes").html(data.PagosPendientes);
                    $("#PagosDepositados").html(data.PagosDepositados);
                    $("#Contratos").html(data.Contratos);
                    $("#PagosxDepositar").html(data.PagosxDepositar);
                    $("#Bloqueos").html(data.Bloqueos);
                    $("#scheck").html(data.jcasilla_valida);
                    $("#sbotonval").html(data.jboton_validacuenta);
                    $("#NoCuenta").html(data.NoCuenta);
                    $("#CuentaClabe").html(data.CuentaClabe);
                    $("#Banco").html(data.Banco);
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
            var data = 'Periodo=' + $('#Periodo').val()
                + '&Sede=' + $('#Sedes').val();

            window.location.href = '/ECW_Contratos/ExportExcel?' + data;
        },

		edit: function (id) { },

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

function DocsOnline() {

    const formDocsOnline = document.createElement('form');
    formDocsOnline.method = 'POST';
    formDocsOnline.action = 'https://testdocsprofesoresonline.redanahuac.mx/Login/Validate2';
    formDocsOnline.target = "_blank";
    //formDocsOnline.action = 'http://localhost:58402/Login/Validate2';

    const inputUsuario = document.createElement('input');
    inputUsuario.type = 'hidden';
    inputUsuario.name = 'usuario';
    inputUsuario.value = $('#idsiu').val();

    const inputCampus = document.createElement('input');
    inputCampus.type = 'hidden';
    inputCampus.name = 'campus';
    inputCampus.value = $('#Sedes').val();

    const inputO365 = document.createElement('input');
    inputO365.type = 'hidden';
    inputO365.name = 'email';
    inputO365.value = $('#correoO365').val();

    const inputProfesor = document.createElement('input');
    inputProfesor.type = 'hidden';
    inputProfesor.name = 'nombre';
    inputProfesor.value = $('#nombreProfesor').val();

    const inputSedesAll = document.createElement('input');
    inputSedesAll.type = 'hidden';
    inputSedesAll.name = 'sedes';
    inputSedesAll.value = $('#sedesAll').val();

    formDocsOnline.appendChild(inputUsuario);
    formDocsOnline.appendChild(inputCampus);
    formDocsOnline.appendChild(inputO365);
    formDocsOnline.appendChild(inputProfesor);
    formDocsOnline.appendChild(inputSedesAll);

    document.body.appendChild(formDocsOnline);
    formDocsOnline.submit();

    //alert('Documentos ONLILNE');

    //var model = {
    //    usuario: IDSIU,
    //    password: $("#c_tipoDePagoEC option:selected").val(),
    //    campus: cve_sede,
    //    email: ,
    //    nombre: ,
    //}

    //$.ajax({
    //    type: "GET",
    //    url: "https://documentosonline.redanahuac.mx/sistemasanahuac/Login/Validate",
    //    //data: model,
    //    success: function (result) {console.log(result)}, error: function (error) {console.log('x - ' + error)}
    //});
}