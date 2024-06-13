Sedes.setSedes_success = function () {
	GetCatalogos();
	GetEnvioCorreo()
}

$(function () {

	$(window).load(function () {
		GetCatalogos();
		GetEnvioCorreo()
	});
});//End function jquery


var ListOptions;


var BuildModel = function () {

	//var array_values = [];
	if (ListOptions != null)
		for (var i = 0; i < ListOptions.length; i++) {
			//var value = $('#PK_CATALOGO_' + ListOptions[i]).val();
			//array_values.push(value);
			ListOptions[i].Value = $('#PK_CATALOGO_' + ListOptions[i].PkCatalog).val();
		}

	return {
		ClaveSede: $('#Sedes').val(),
		ListOptions: ListOptions,
		/*
		Values

		MetodoPago: $('#MetodoPago').val(),
		FormaPago: $('#FormaPago').val(),
		ClaveProductoServicio: $('#ClaveProductoServicio').val(),
		ClaveUnidad: $('#ClaveUnidad').val(),
		ValidarPDF: $('#ValidarPDF').val(),
		*/
	};
}

var GetCatalogos = function () {
	var model = BuildModel();
	model.ListOptions = null;
	/*
	$('#MetodoPago').html('');
	$('#FormaPago').html('');
	$('#ClaveProductoServicio').html('');
	$('#ClaveUnidad').html('');
	$('#ValidarPDF').html('');
	*/
	loading('loading-bar');
	loading('loading-circle', '#wizard', 'Generando nomina..');
	$('#BtnGuardar').hide();
	$.ajax({
		type: "GET",
		dataType: 'json',
		cache: false,
		url: "/ValidacionCFDI/GetCatalogos/",
		data: model,
		success: function (json) {

			ListOptions = json.ListOptions;
			for (var i = 0; i < json.SelectHTML.length; i++) {
				$('#PK_CATALOGO_' + ListOptions[i].PkCatalog).html(json.SelectHTML[i]);
			}
			
			/*
			$('#MetodoPago').html(json.MetodoPago);
			$('#FormaPago').html(json.FormaPago);
			$('#ClaveProductoServicio').html(json.ClaveProductoServicio);
			$('#ClaveUnidad').html(json.ClaveUnidad);
			$('#ValidarPDF').html(json.ValidarPDF);
			*/
		},
		complete: function (msg) {
			$('.loader-min-bar').hide();
			$('#BtnGuardar').show();
		},
	});
}

function Guardar() {

	var model = BuildModel();

	$.ajax({
		type: "POST",
		dataType: 'json',
		cache: false,
		url: "/ValidacionCFDI/Save/",
		data: model,
		success: function (data) {
			$('#notification').html(data.msg);
		}
	});
}


var EnvioCorreo;
var BuildModelEnvio = function () {

	EnvioCorreo = $("#pkEnvio").val();
	return {
		ClaveSede: $('#Sedes').val(),
		EnvioCorreo: EnvioCorreo,

	};
}
function GetEnvioCorreo() {

	var ClaveSede = $('#Sedes').val();

	$.ajax({
		type: "GET",
		dataType: 'json',
		cache: false,
		url: "/ValidacionCFDI/GetEnvioCorreo/",
		data: { ClaveSede: ClaveSede },
		success: function (data) {

			var content = "";
			if (data != null) {
				content += "<div class='row'>";
				content += "<div class='col-md-4'>";
				content += "		<div class='form-control cfdi_label'>Envio de correo al publicar contrato</div>";
				content += "	</div>";
				content += "		<div class='col-md-5'>";
				content += "		<div class='form-group'>";
				content += "			<select id='pkEnvio' class='form-control'>";
				if (data == false) {
					content += "				<option value='0' selected >NO</option>";
					content += "				<option value='1'>SI</option>";
				} else {
					content += "				<option value='0'>NO</option>";
					content += "				<option value='1' selected >SI</option>";
				}

				content += "			</select>";
				content += "		</div>";
				content += "	</div>";
				content += "</div>";
				document.getElementById("muestraEnvio").innerHTML = content;

			}
			$('#notification').html(data.msg);
		}
	});
}
function GuardarEnvio() {

	var model = BuildModelEnvio();

	$.ajax({
		type: "POST",
		dataType: 'json',
		cache: false,
		url: "/ValidacionCFDI/SaveEnvio/",
		data: model,
		success: function (data) {
			$('#notification').html(data.msg);
		}
	});
}


