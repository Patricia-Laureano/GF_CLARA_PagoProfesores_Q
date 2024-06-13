var ciclos = new Ciclos('Ciclo');
var periodos = new Periodos('Periodo');
var niveles = new Niveles('Nivel');
//var menu = new XMenu();
//var menu_2 = new XMenu();

$(function () {
	$(window).load(function () {

		$('#file_xml_icon').hide();
		$('#file_pdf_icon').hide();
		/*
		menu.init('tab-id-EdoCuenta', 'parent-menu-id');

		//
		// Estado de cuenta
		//
		menu_2.init('tab-id-pend', 'parent-menu-2-id', function (SelectedTab) {
			

			if (menu_2.SelectedTab == 'tab-id-bloq') DataTable_EstadoCuenta.setFiltro(1);
			else if (menu_2.SelectedTab == 'tab-id-pend') DataTable_EstadoCuenta.setFiltro(2);
			else if (menu_2.SelectedTab == 'tab-id-entr') DataTable_EstadoCuenta.setFiltro(3);
			else if (menu_2.SelectedTab == 'tab-id-depo') DataTable_EstadoCuenta.setFiltro(4);
			formPage_EstadoCuenta.consultar();
		});
		//*/
		ciclos.init("ciclos");
		niveles.init("niveles");


		$('#file_xml').fileupload({
			url: '/FileUploadHandler.ashx?upload=start',
			add: function (e, data) {
				var val = data.files[0].name.toLowerCase();
				var regex = new RegExp("(.*?)\.(xml)$");
				if (!(regex.test(val))) {
					$(this).val('');
					alert('Sólo archivos XML');
				}
				else
					data.submit();
			},
			progress: function (e, data) {
				//var progress = parseInt(data.loaded / data.total * 100, 10);
			},
			success: function (response, status) {
				//afi = response;
				//$('#file_xml_icon').show();
				$('#file_xml_name').html('' + this.files[0].name);

				formPage_EstadoCuenta.fileNameXML = response;
			},
			error: function (error) {
				//$('#file_xml_icon').hide();
				$('#file_xml_name').html('');
			}
		});

		$('#file_pdf').fileupload({
			url: '/FileUploadHandler.ashx?upload=start',
			add: function (e, data) {
				var val = data.files[0].name.toLowerCase();
				var regex = new RegExp("(.*?)\.(pdf)$");
				if (!(regex.test(val))) {
					$(this).val('');
					alert('Sólo archivos PDF');
				}
				else
					data.submit();
			},
			progress: function (e, data) {
				//var progress = parseInt(data.loaded / data.total * 100, 10);
			},
			success: function (response, status) {
				//$('#file_xml_icon').show();
				$('#file_pdf_name').html('' + this.files[0].name);

				formPage_EstadoCuenta.fileNamePDF = response;
			},
			error: function (error) {
				$('#file_xml_name').html('');
			}
		});

		Sedes.setSedes_success = function (data) {
			var model = {
				Sede: $('#Sedes').val(),
			}
			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/EstadodeCuentaWeb/CambiarSede",
				data: model,
				success: function (data) {
					data = jQuery.parseJSON(data);

					$('#Fis_Sede').html(data.Fis_Sede);
					$('#Fis_RecibiDe').html(data.Fis_Recibide);
					$('#Fis_RFC').html(data.Fis_RFC);
					$('#Fis_Domicilio').html(data.Fis_Domicilio);
					$('#Fis_Concepto').html(data.Fis_Concepto);
				}
			});

		}

		

	});


	/*formPage_Contratos.consultar();
	formPage_EstadoCuenta.consultar();
	formPage_RetencionesMensuales.consultar();
	formPage_RetencionesAnuales.consultar();*/

});


function handlerdataCiclos() {
	periodos.id_ciclo = $("#Ciclo").val();
	periodos.init("periodos");
}

function handlerdataPeriodos() { }

function handlerdataNiveles() { }

$('#Ciclo').on('change', function (obj) {
	periodos.id_ciclo = this.value;
	periodos.init("periodos");
});




function consultar() {
	/*
	if (menu.SelectedTab == 'tab-id-Contratos')
		formPage_Contratos.consultar();
	else if (menu.SelectedTab == 'tab-id-EdoCuenta')
		formPage_EstadoCuenta.consultar();
	else if (menu.SelectedTab == 'tab-id-RetMensuales')
		formPage_RetencionesMensuales.consultar();
	else if (menu.SelectedTab == 'tab-id-RetAnuales')
		formPage_RetencionesAnuales.consultar();
		*/
}

