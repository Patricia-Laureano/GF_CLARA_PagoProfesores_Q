var consultar = false;

$(function () {
    $("#fechai").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });

    $("#fechaf").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });

    $(document).ready(function () {
        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 1
        });
    });

    $(window).load(function () {
        formPage.clean();
        formValidation.Inputs(["fechai", "fechaf"]);

        $('#file_xlsx').fileupload({
        	url: '/FileUploadHandler.ashx?upload=start',
        	add: function (e, data) {
        		var val = data.files[0].name.toLowerCase();
        		var regex = new RegExp("(.*?)\.(xlsx)$");
        		if (!(regex.test(val))) {
        			$(this).val('');
        			alert('Sólo archivos XLSX');
        		}
        		else data.submit();
        	},
        	progress: function (e, data) { },
        	success: function (response, status) {
        		formPage.fileNameXLSX = response;
        		$('#file_xlsx_name').val('' + this.files[0].name);
        	},
        	error: function (error) { }
        });
    });
});//End function jquery



var formPage = function () {
    "use strict"; return {

    	fileNameXLSX: '',
    	clean: function () {
    		consultar = false;

    		$("#fechai").val('');
    		$("#fechaf").val('');

    		$('#file_xlsx').prop('disabled', false);
    		$('#file_xlsx').val('');
    		$('#file_xlsx_name').val('');
    		this.fileChange();
    		$('#formbtnGenerar').hide();

    		$('#img_generando').hide();
    	},

        fileChange: function () {
        	var file = $('#file_xlsx').val();
        	$('#formbtnAuditar').prop('disabled', false);
        	if (file == null || file == "")
        		$('#formbtnAuditar').hide();
        	else
        		$('#formbtnAuditar').show();
        	$('#formbtnGenerar').hide();
        },

        auditar: function () {

            $('#file_xlsx').prop('disabled', true);
        	$('#formbtnLimpiar').prop('disabled', true);
        	$('#formbtnGenerar').hide();
            $('#img_generando').show();

            var sedes = $('#Sedes').val();

            $.ajax({
        		type: "POST",
        		cache: false,
        		url: "/Deposito/ProcessExcel",
        		data: 'filename=' + formPage.fileNameXLSX + '&sedes=' + sedes,
        		success: function (data) {
        			$('#file_xlsx_name').val(formPage.fileNameXLSX = '');
                    $('#formbtnAuditar').prop('disabled', true);
                    $('#img_generando').hide();
                    $('#notification').html(data.msg);

        			var ntype = $('#NotificationType').val();
        			if (ntype == "SUCCESS") {
        				$('#formbtnGenerar').show();
        			}
        			else if (ntype == "ERROR") {
        			}
        			DataTable.init();
        		},
        		complete: function () {
        			$('#file_xlsx').prop('disabled', false);
        			$('#formbtnLimpiar').prop('disabled', false);
        		},
        	});
        },

        generar: function () {
        	$('#file_xlsx').prop('disabled', true);
        	$('#formbtnLimpiar').prop('disabled', true);
        	$('#formbtnAuditar').prop('disabled', true);
        	$('#formbtnGenerar').prop('disabled', true);
        	$('#img_generando').show();
        	$.ajax({
        		type: 'POST',
        		cache: false,
        		url: '/Deposito/Generar',
        		success: function (data) {
        			$('#notification').html(data.msg);
        			DataTable.init();
        		},
        		complete: function (data) {
        			$('#file_xlsx').prop('disabled', false);
        			$('#formbtnLimpiar').prop('disabled', false);
        			$('#formbtnAuditar').prop('disabled', false);
        			$('#formbtnGenerar').prop('disabled', false);
        			$('#formbtnAuditar').hide();
        			$('#formbtnGenerar').hide();
        			$('#img_generando').hide();
        		},
        	});
        },

        Concultar: function (id) {
            if (!formValidation.Validate())
                return;
            consultar = true;
            DataTable.init();
        }
    }
}();

var DataTable = function () {
    var pag = 1;
    var order = "ID_DEPOSITO";

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
            window.location.href = '/Deposito/ExportExcel';
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
            var filter = $('#Sedes').val();

            var datos = '&fechai=' + $('#fechai').val()
                + '&fechaf=' + $('#fechaf').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
            	type: "GET",
            	cache: false,
            	url: "/Deposito/CreateDataTable/",
            	data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter,
            	success: function (msg) {

                    $('.loader-min-bar').hide();
                    $("#datatable").html(msg);

            		$('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            			altClass: 'odd',
            			footer: true,
            			fixedColumns: 1
            		});
            	}
            });
        }
    }
}();