/* Tree Initialization*/

$.fn.extend({
    treed: function (o) {
      
      var openedClass = 'glyphicon-minus-sign';
      var closedClass = 'glyphicon-plus-sign';
      
      if (typeof o != 'undefined'){
        if (typeof o.openedClass != 'undefined'){
        openedClass = o.openedClass;
        }
        if (typeof o.closedClass != 'undefined'){
        closedClass = o.closedClass;
        }
      };
      
        //initialize each of the top levels
        var tree = $(this);
        tree.addClass("tree");
        tree.find('li').has("ul").each(function () {
            var branch = $(this); //li with children ul
            branch.prepend("<i class='indicator glyphicon " + closedClass + "'></i>");
            branch.addClass('branch');
            branch.on('click', function (e) {
                if (this == e.target) {
                    var icon = $(this).children('i:first');
                    icon.toggleClass(openedClass + " " + closedClass);
                    $(this).children().children().toggle();
                }
            })
            branch.children().children().toggle();
        });
        //fire event from the dynamically added icon
      tree.find('.branch .indicator').each(function(){
        $(this).on('click', function () {
            $(this).closest('li').click();
        });
      });
        //fire event to open branch if the li contains an anchor instead of text
        tree.find('.branch>a').each(function () {
            $(this).on('click', function (e) {
                $(this).closest('li').click();
                e.preventDefault();
            });
        });
        //fire event to open branch if the li contains a button instead of text
        tree.find('.branch>button').each(function () {
            $(this).on('click', function (e) {
                $(this).closest('li').click();
                e.preventDefault();
            });
        });
    }
});

//Initialization of treeviews

$('#tree1').treed();

/* End tree*/


$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

		formPage.clean();
		levels_hide();
	});
});//End function jquery


function levels_show() {
	/*
	$('#labelJerarquia_id').hide();
	$('#comboJerarquia_id').show();
	*/
	$('#comboJerarquia_id').focus();
}
function levels_hide() {
	/*
	$('#labelJerarquia_id').show();
	$('#comboJerarquia_id').hide();
	*/
	//$('#textJerarquia_id').html(" - " + $('#Jerarquia').val());
}

var formPage = function () {
	var IdUser;

	"use strict"; return {
		clean: function () {
			$("#Usuario").val("");
			$("#Password").val("");
			$("#Password_2").val("");
			$("#Nombre").val("");
			$("#APaterno").val("");
			$("#AMaterno").val("");

			$("input:radio[name=disponible][value=1]").attr('checked', 'checked');

			// Sedes
			$('#jerarquia').val('0');
			var checkboxs = $('[data-filter~="checkbox-sedes"]');
			for (var index = 0; index < checkboxs.length ; index++)
				checkboxs[index].checked = false;

			// Roles
			var select_1 = document.getElementById('sel1');
			var select_2 = document.getElementById('sel2');
			while (0 < select_2.length)
			{
				var opc = select_2.options[select_2.length - 1];
				select_2.options[select_2.length - 1] = null;
				select_1.options[select_1.options.length] = opc;
			}
			select_1.selectedIndex = -1;

			$("#formbtnadd").show();
			$("#formbtnadd").prop("disabled", false);
			$("#formbtnsave").hide();
			$("#formbtnsave").prop("disabled", true);
			$("#formbtndelete").prop("disabled", true);
			$("#formbtndelete").hide();

			this.cleanStyles();
		},

		setRoles: function () {
			select_src = document.getElementById('sel1');
			select_tar = document.getElementById('sel2');

			if (select_src.selectedIndex == -1)
				return;
			var opc = select_src.options[select_src.selectedIndex];

			select_src.options[select_src.selectedIndex] = null;
			select_tar.options[select_tar.options.length] = opc;
			select_tar.selectedIndex = select_tar.options.length - 1;
		    //eval(select_2.options[select_2.options.length] = opc);
		},

		quitRoles: function () {
			select_src = document.getElementById('sel2');
			select_tar = document.getElementById('sel1');

			if (select_src.selectedIndex == -1)
				return;
			var opc = select_src.options[select_src.selectedIndex];

			select_src.options[select_src.selectedIndex] = null;
			select_tar.options[select_tar.options.length] = opc;
			select_tar.selectedIndex = select_tar.options.length - 1;
		},

		add: function () {
			var model = this.buildModel();

			if (this.foundErrors(model))
				return;

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Users/Add/",
				data: model,
				success: function (data) {
					$('#notification').html(data.msg);
					formPage.clean();
					DataTable.init();
				}
			});
		},

		edit: function (id) {
			//this.clean();
			$("#formbtnadd").prop("disabled", true);
			IdUser = id;
			var model = this.buildModel();

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Users/Edit/",
				data: model,
				success: function (data) {

					if (data != "-1") {
						data = jQuery.parseJSON(data);
						//formPage.clean();
						$('html, body').animate({ scrollTop: 0 }, 'fast');

						$("#Nombre").val(data.Nombre);
						$("#APaterno").val(data.APaterno);
						$("#AMaterno").val(data.AMaterno);
						$("#Usuario").val(data.Usuario);
						$("#Password").val(data.Password);
						$("#Password_2").val(data.Password);
						//$('input[name=disponible]').val(data.Activo);
						//$("input[name=disponible][value='" + data.Activo + "']").attr('checked', 'checked');
						$("input:radio[name=disponible][value=" + data.Activo + "]").attr('checked', 'checked');

						$('#jerarquia').val(data.SedePrincipal);
						var array = data.IdsSedesAcceso;
						if (array != null)
							for (var index = 0; index < array.length ; index++) {
								var idSede = array[index];
								$('#sede_' + idSede).prop('checked', 'checked');
								//checkbox.checked = true;
							}
						// Roles
						array = data.Roles;
						if (array != null) {
							var select_1 = document.getElementById('sel1');
							var select_2 = document.getElementById('sel2');
							for (var index = 0; index < array.length ; index++) {
								for (var i = 0; i < select_1.options.length; i++)
									if (select_1.options[i].value == array[index]) {
										var opc = select_1.options[i];
										select_1.options[i] = null;
										select_2.options[select_2.options.length] = opc;
										break;
									}
								/*
								$('#sel1').val(array[index]);
								formPage.setRoles();
								*/
							}
						}
						/*
						while (0 < select_2.length) {
							var opc = select_2.options[select_2.length - 1];
							select_2.options[select_2.length - 1] = null;
							select_1.options[select_1.options.length] = opc;
						}
						//*/


						/*
						$("#formbtnadd").hide();
						$("#formbtnsave").show();
						$("#formbtndelete").show();
						*/
						$("#formbtnadd").hide();
						$("#formbtnsave").show();
						$("#formbtndelete").show();
						$("#formbtnadd").prop("disabled", true);
						$("#formbtnsave").prop("disabled", false);
						$("#formbtndelete").prop("disabled", false);
					}
				}
			});
		},

		save: function () {
			var model = this.buildModel();
			if (this.foundErrors(model))
				return;
			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Users/Save/",
				data: model,
				success: function (data) {
					if (data != "-1") {
						$('#notification').html(data.msg);
						
						DataTable.init();
					}
				}
			});
		},

		delete: function () {

			var model = this.buildModel();

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/Users/Delete/",
				data: model,
				success: function (data) {
					$('#content').prepend(data.msg);
					formPage.clean();
					DataTable.init();
				}
			});
		},

		getIdsSedesChecked: function () {
			var idsChecked = [];
			/* Se obtiene el arreglo de checkbox de las sedes. */
			var array = $('[data-filter~="checkbox-sedes"]');
			for (var index = 0; index < array.length ; index++) {
				var checkbox = array[index];
				if (checkbox.checked)
					idsChecked.push(checkbox.dataset.id);
			}
			return idsChecked;
		},

		getRoles: function () {
			var Roles = [];
			//obj = document.getElementById('sel2');
			$("#sel2 option").each(function(event, obj)
			{
				Roles.push($(this).val());
				// Add $(this).val() to your list
			});
			return Roles;
		},

		buildModel: function () {
			return {
				Id: IdUser,
				Usuario: $('#Usuario').val().trim(),
				Password: $('#Password').val().trim(),
				Password_2: $('#Password_2').val().trim(),
				Nombre: $('#Nombre').val().trim(),
				APaterno: $('#APaterno').val().trim(),
				AMaterno: $('#AMaterno').val().trim(),
				Activo: $('input[name=disponible]:checked').val(),
				SedePrincipal: $('#jerarquia').val().trim(),
				IdsSedesAcceso: this.getIdsSedesChecked(),
				Roles: this.getRoles(),
			}
		},

		cleanStyles: function () {
			$('#Nombre').removeClass("parsley-error");
			$('#APaterno').removeClass("parsley-error");
			$('#AMaterno').removeClass("parsley-error");
			$('#Usuario').removeClass("parsley-error");
			$('#Password').removeClass("parsley-error");
			$('#Password_2').removeClass("parsley-error");
		},

		foundErrors: function (model) {
			this.cleanStyles();

			if (model.Nombre == "") {
				$('#Nombre').addClass("parsley-error");
				$('#Nombre').focus();
				return true;
			}
			if (model.APaterno == "") {
				$('#APaterno').addClass("parsley-error");
				$('#APaterno').focus();
				return true;
			}
			if (model.AMaterno == "") {
				$('#AMaterno').addClass("parsley-error");
				$('#AMaterno').focus();
				return true;
			}
			if (model.Usuario == "") {
				$('#Usuario').addClass("parsley-error");
				$('#Usuario').focus();
				return true;
			}
			if (model.Password == "") {
				$('#Password').addClass("parsley-error");
				$('#Password').focus();
				return true;
			}
			if (model.Password_2 != model.Password) {
				$('#Password_2').addClass("parsley-error");
				$('#Password_2').focus();
				return true;
			}
			
			return false;
		},
	}

}();


var DataTable = function () {
	var pag = 1;
	var order = "USUARIO";
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
		setShow: function() {
			this.setPage(1);
		},
		edit: function (id) {
			formPage.clean();
			formPage.edit(id);
		},

		setPage: function (page) {
			pag = page;
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
				url: "/Users/CreateDataTable/",
				data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
				success: function (msg) {

					//$('div.block').unblock();
					$("#datatable").html(msg);

				}

			});
		}

	}


}();


