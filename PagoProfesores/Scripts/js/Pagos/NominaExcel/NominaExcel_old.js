
var ciclos = new Ciclos("ciclos");
var periodos = new Periodos("periodos");
var niveles = new Niveles("niveles");

$(function () {

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
		
		niveles.init("niveles");

	});
});//End function jquery


var formPage = function () {

	"use strict"; return {

		clean: function () {
			//formValidation.Clean();

			$("#Ciclo").val("");
			$("#Periodo").val("");
			$("#Nivel").val("");
			$("#TipoDeContrato").val("");

			$('#file').prop('disabled', false);
			$('#formbtnEnviar').prop('disabled', false);
			$('#formbtnImportar').prop('disabled', true);
			initProgressBar();
		},

		subirExcel: function () {

			//$('#file').prop('disabled', true);
			//$('#formbtnEnviar').prop('disabled', true);
			$('#formbtnImportar').prop('disabled', true);

			startTimer("NominaExcel", function ()
			{
				$.ajax({
					type: 'POST',
					cache: false,
					url: '/NominaExcel/FindFirstError',
					success: function (data) {
						$('#notification').html(data.msg);
					},
					error: function (msg) { }
				});

				$('#file').val('');
				$('#file').prop('disabled', false);
				//$('#formbtnEnviar').prop('disabled', false);
				$('#formbtnImportar').prop('disabled', true);

				DataTable.setShow();
			});
		}
	}
}();


var DataTable = function () {
	var pag = 1;
	var order = "IDSIU";
	var sortoption = {
		ASC: "ASC",
		DESC: "DESC"
	};
	var sort = sortoption.ASC;

	"use strict"; return {

		onkeyup_colfield_check: function (e) {
			var enterKey = 13;
			if (e.which == enterKey) {
				pag = 1;
				this.init();
			}
		},
		/*
		exportExcel: function (table) {

			window.location.href = '/Bancos/ExportExcel';

		},
		//*/
		edit: function (id) {
			formPage.edit(id);
		},

		setPage: function (page) {
			pag = page;
			this.init();
		},

		setShow: function (page) {  //update 
			pag = 1;
			this.init();
		},

		Orderby: function (campo) {
			order = campo;
			var sortcampo = $("#SORT-" + campo).data("sort");
			if (sortcampo == sortoption.ASC) { sort = sortoption.DESC; } else { sort = sortoption.ASC; }
			this.init();
		},

		init: function () {
			var show = $("#data-elements-length").val();
			var search = $("#searchtable").val();
			var orderby = order;
			var sorter = sort;

			$.ajax({
				type: "GET",
				cache: false,
				url: "/NominaExcel/CreateDataTable/",
				data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
				success: function (msg) {
					$("#datatable").html(msg);
				}
			});
		}
	}
}();


