
$(function () {

	$(window).load(function () {
		formPage.ConsultaIdsMenu();
		formPage.clean();
	});
});//End function jquery


var formPage = function () {
	var IdMenu;

	"use strict"; return {
		clean: function () {
			$("#Id").val("");
			$("#Nombre").val("");
			$("#Descripcion").val("");
			$("#Url").val("");
			$("#Padre").val("0");
			$("#Orden").val("");
			$("#PkPermiso").val("0");
			$("#Icono").val("");
			$("IconoMuestra").html("");

			$("#formbtnadd").prop("disabled", false);
			$("#formbtnsave").prop("disabled", true);
			$("#formbtndelete").prop("disabled", true);

			//$("#formbtnadd").show();
			//$("#formbtnsave").hide();
			//$("#formbtndelete").hide();
			//$("#formbtnsave").prop("visible", false);
		},

		edit: function (id) {

			IdMenu = id;
			var model = this.buildModel();

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Menus/Edit/",
				data: model,
				success: function (data) {
					if (session_error(data) == false) {
						data = jQuery.parseJSON(data);
						$('html, body').animate({ scrollTop: 0 }, 'fast');

						$("#Nombre").val(data.Nombre);
						$("#Descripcion").val(data.Descripcion);
						$("#Url").val(data.Url);
						$("#Padre").val(data.Padre);
						$("#Orden").val(data.Orden);
						$("#PkPermiso").val(data.PkPermiso);

						var Icono = data.Icono;
						var index_1 = Icono.indexOf('class="');
						if (0 < index_1) {
							index_1 += 7;
							var index_2 = Icono.indexOf('"', index_1);
							if (index_1 <= index_2)
								Icono = data.Icono.substring(index_1, index_2);
						}
						$("#Icono").val(Icono);


						formPage.Icono_OnChange();

						$("#formbtnadd").prop("disabled", true);
						$("#formbtnsave").prop("disabled", false);
						$("#formbtndelete").prop("disabled", false);
					}
				},
				error: function (data) {
					session_error(data);
				}
			});
		},

		save: function () {
			var model = this.buildModel();
			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Menus/Save/",
				data: model,
				success: function (data) {
					if (session_error(data) == false) {
						$('#content').prepend(data.msg);
						formPage.clean();
						DataTable.init();
					}
				},
				error: function (data) {
					session_error(data);
				}
			});
		},

		delete: function () {

			var model = this.buildModel();

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Menus/Delete/",
				data: model,
				success: function (data) {
					if (session_error(data) == false) {
						$('#content').prepend(data.msg);
						formPage.clean();
						DataTable.init();
					}
				},
				error: function (data) {
					session_error(data);
				}
			});

		},

		add: function () {
			var model = this.buildModel();

			if (this.foundErrors(model))
				return;

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Menus/Add/",
				data: model,
				success: function (data) {
					if (session_error(data) == false) {
						$('#content').prepend(data.msg);
						formPage.clean();
						DataTable.init();
					}
				},
				error: function (data) {
					session_error(data);
				}
			});
		},

		buildModel: function () {
			return {
				Id: IdMenu,
				Nombre: $('#Nombre').val().trim(),
				Descripcion: $('#Descripcion').val().trim(),
				Url: $('#Url').val().trim(),
				Padre: parseInt($('#Padre').val().trim()),
				Orden: parseInt($('#Orden').val().trim()),
				PkPermiso: parseInt($('#PkPermiso').val().trim()),
				Icono: $('#Icono').val().trim()
			}
		},

		foundErrors: function (model) {
			$('#Nombre').removeClass("parsley-error");
			$('#Descripcion').removeClass("parsley-error");
			$('#Url').removeClass("parsley-error");
			$('#Padre').removeClass("parsley-error");
			//$('#Icono').removeClass("parsley-error");

			if (model.Nombre == "") {
				$('#Nombre').addClass("parsley-error");
				$('#Nombre').focus();
				return true;
			}
			if (model.Descripcion == "") {
				$('#Descripcion').addClass("parsley-error");
				$('#Descripcion').focus();
				return true;
			}
			if (model.Url == "") {
				$('#Url').addClass("parsley-error");
				$('#Url').focus();
				return true;
			}
			if (isNaN(model.Padre)) {
				alert("Hace falta especificar un id de menu padre válido.");
				$('#Padre').focus();
				return true;
			}
			if (isNaN(model.Orden)) {
				alert("Hace falta especificar un orden válido.");
				$('#Orden').focus();
				return true;
			}
			if (isNaN(model.PkPermiso)) {
				alert("Hace falta especificar un id de permiso válido.");
				$('#PkPermiso').focus();
				return true;
			}
			/*
        	if (model == "") {
        		alert("Hace falta especificar .");
        		$('#').focus();
        		return true;
        	}
			*/
			return false;
		},

		ConsultaIdsMenu: function () {
			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Menus/ConsultaIdsMenu/",
				success: function (msg) {
					$('#Padre').html(msg);
				},
				error: function (msg) {
					session_error(msg);
				}
			});
		},

		Icono_OnChange: function () {
			var clase = $('#Icono').val();
			$('#IconoMuestra').html('Icono &nbsp;<i class="' + clase + '" style="color:Green;"></i>');
		}

	}


}();


var DataTable = function () {
    var pag = 1;
    var order = "PADRE";
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
                this.init();
            }
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

        	if (search != "") { pag = 1; }

        	/*$('div.block').block({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src="/Content/images/load-search.gif" /><br><h2> Buscando..</h2>'
            });*/



        	$.ajax({
        		type: "GET",
        		cache: false,
        		url: "/Menus/CreateDataTable/",
        		data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
        		success: function (msg) {
        			if (session_error(msg) == false)
        			{
        				$("#datatable").html(msg);
        			}
        		},
        		error: function (msg) {
        			session_error(msg);
        		}
        	});
        }

    }


}();
