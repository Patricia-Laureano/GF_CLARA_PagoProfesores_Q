Sedes.setSedes_success = function () {
	GetCatalogos();
}

$(function () {

	$(window).load(function () {
		GetCatalogos();
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