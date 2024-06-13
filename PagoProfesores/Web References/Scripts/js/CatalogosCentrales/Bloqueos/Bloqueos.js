$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();
        formValidation.Inputs(["Cve_Bloqueo", "Bloqueo", "BloqueoDescripcion"]);
        formValidation.notEmpty('Cve_Bloqueo', 'El campo Clave no debe de estar vacio');
        formValidation.notEmpty('Bloqueo', 'El campo Bloqueo no debe de estar vacio');
        formValidation.notEmpty('BloqueoDescripcion', 'El campo Descripción no debe de estar vacio');
        
    });
});//End function jquery

var formPage = function () {
	var Cve_Bloqueo;

	"use strict"; return {

		clean: function () {
			formValidation.Clean();

			$("#Cve_Bloqueo").prop("disabled", false);
			$("#Bloqueo").prop("disabled", false);
			$("#BloqueoDescripcion").prop("disabled", false);
			$('#EstadoCuenta').prop('checked', false);
			$('#Factura').prop('checked', false);
			$('#Pagos').prop('checked', false);

			$("#formbtnadd").show();
			$("#formbtnadd").prop("disabled", false);
			$("#formbtnsave").hide();
			$("#formbtnsave").prop("disabled", true);
			$("#formbtndelete").hide();
			$("#formbtndelete").prop("disabled", true);
		},

		edit: function (id) {
			Cve_Bloqueo = id;
			var model =
                {
                	Cve_Bloqueo: id
                }

			this.clean();

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Bloqueos/Edit/",
				data: model,
				success: function (data) {
					data = jQuery.parseJSON(data);
					$('html, body').animate({ scrollTop: 0 }, 'fast');

					$("#Cve_Bloqueo").val(data.Cve_Bloqueo);
					$("#Bloqueo").val(data.Bloqueo);
					$("#BloqueoDescripcion").val(data.BloqueoDescripcion);
					// checkbox's
					$('#EstadoCuenta').prop('checked', data.EstadoCuenta == 'True' ? true : false);
					$('#Factura').prop('checked', data.Factura == 'True' ? true : false);
					$('#Pagos').prop('checked', data.Pagos == 'True' ? true : false);


					$("#formbtnadd").hide();
					$("#formbtnsave").show();
					$("#formbtndelete").show();
					$("#formbtnadd").prop("disabled", true);
					$("#formbtnsave").prop("disabled", false);
					$("#formbtndelete").prop("disabled", false);


					$("#Cve_Bloqueo").prop("disabled", true);


				}
			});
		},

		save: function () {

			var model = this.createModel();
			/*
                {
                    Cve_Bloqueo                   : Cve_Bloqueo,
                    Bloqueo          : $("#Bloqueo").val(),
                    BloqueoDescripcion          : $("#BloqueoDescripcion").val(),
                    
                }
				*/
			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Bloqueos/Save/",
				data: model,
				success: function (data) {

				//	formPage.clean();
					$('#notification').html(data.msg);
					DataTable.init();
				}
			});
		},

		delete: function (confirm) {

			if (!confirm) {
				$('#modal-delete-bloqueos').modal("show");
				return;
			}

			$('#modal-delete-bloqueos').modal("hide");

			var model = {
				Cve_Bloqueo: Cve_Bloqueo
			}

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Bloqueos/Delete/",
				data: model,
				success: function (data) {

					formPage.clean();
					$('#notification').html(data.msg);
					DataTable.init();
				}
			});
		},

		createModel: function () {
			return {
				Cve_Bloqueo: $("#Cve_Bloqueo").val(),
				Bloqueo: $("#Bloqueo").val(),
				BloqueoDescripcion: $("#BloqueoDescripcion").val(),
				EstadoCuenta: $('#EstadoCuenta').prop('checked'),
				Factura: $('#Factura').prop('checked'),
				Pagos: $('#Pagos').prop('checked'),
				/*
        		CuotaFija: $("#CuotaFija").val(),
        		PorcentajeExcedente: $("#PorcentajeExcedente").val(),
				//*/
			}
		},

		add: function () {
			/*
            var model = {
                Cve_Bloqueo                       : $("#Cve_Bloqueo").val(),
                Bloqueo              : $("#Bloqueo").val(),
                BloqueoDescripcion              : $("#BloqueoDescripcion").val(),
                //CuotaFija                   : $("#CuotaFija").val(),
                PorcentajeExcedente         : $("#PorcentajeExcedente").val(),
            }
			*/
			var model = this.createModel();


			if (!formValidation.Validate())
				return;

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Bloqueos/Add/",
				data: model,
				success: function (data) {

					formPage.clean();
					$('#notification').html(data.msg);
					DataTable.init();

				}
			});
		}
	}
}();

var DataTable = function () {
    var pag = 1;
    var order = "Cve_Bloqueo";
    var sortoption = {
        ASC: "ASC",
        DESC: "DESC"
    };
    var sort = sortoption.ASC;

    "use strict"; return {
        myName: 'DataTable',


        onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
                pag = 1; 
                this.init();
            }
        },

        exportExcel: function (table) {

            window.location.href = '/Bloqueos/ExportExcel';

        },

        edit: function (id) {
          
            formPage.edit(id);
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

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Bloqueos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                   
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();


