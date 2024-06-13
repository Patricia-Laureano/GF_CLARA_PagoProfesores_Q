var ciclos1 = new Ciclos("Ciclo");
var periodos1 = new Periodos("Periodo");

$(function () {
	$(window).load(function () {
	    ciclos1.init("ciclos1");
	    $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
	        altClass: 'odd',
	        footer: true,
	        fixedColumns: 5
	    });
		formPage.clean();
	});
});//End function jquery

function handlerdataCiclos() {
    periodos1.id_ciclo = $("#Ciclo").val();
    periodos1.init("periodos1");
}

function handlerdataPeriodos() {
}

$('#ciclos').on('change', function () {
    periodos1.id_ciclo = this.value;
    periodos1.init("Periodo");
});

var formPage = function () {
	var IdMenu;

	"use strict"; return {
		clean: function () {
			$("#Ciclo").val("");
			$("#Periodo").val("");
		},
		consultar: function () {

            
			var data =
				'Periodo=' + $('#Periodo').val() 
                + '&Sedes=' + $('#Sedes').val()
                + '&IDSIU=' + $('#idSiu').val();



			var controller = "Consultar";
			//if ($('#idPersona').val() != "") { controller = "ConsultarPersona"; }


			loading('loading-bar');
			loading('loading-circle', '#datatable', 'Transfiriendo datos de BANNER..');
			loading('loading-circle', '#seccion1', 'Consultando BANNER..');            
		

			$.ajax({
				type: "GET",
				cache: false,
				url: "/ImportarDatosSIU/"+controller,
				data: data,
				success: function (msg) {
				    $('#seccion1').unblock();
				    $('.loader-min-bar').hide();

				    if (session_error(msg) == false) {
						$('#notification').html(msg);
					}

					DataTable.setPage(1);
				},
				error: function (data) {
				    session_error(data);
				    $('#seccion1').unblock();
				    $('#blocking-panel-1').hide();
                    $('#blocking-panel-2').hide();
                }
			});
        },

        importar: function () {
			var arr = DataTable.checkboxs;
			var arrChecked = [];
			for (var i = 0; i < arr.length; i++) {
				var checkbox_checked = $('#' + arr[i]).prop('checked');
				if (checkbox_checked == true)
					arrChecked.push(arr[i]);
            }

			if (arrChecked.length==0 ) {
			    alert('Seleccione una casilla');
			    return;			 	    
            }

			loading('loading-bar');
			loading('loading-circle', '#datatable', 'Actualizando datos..');
			loading('loading-circle', '#seccion1', 'Importando Personas desde BANNER..');
			
			$.ajax({
				type: "POST",
				cache: false,
				url: "/ImportarDatosSIU/importar",
                data: "ids=" + arrChecked.join() + "&sede=" + $("#Sedes").val(),
				success: function (msg) {
				    $('#seccion1').unblock();
					DataTable.init();

					$('#notification').html(msg);
				},
				error: function (msg) {
				    $('#seccion1').unblock();
					$('#notification').html(msg);
				}
			});
		},
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
        myName: 'DataTable',
    	checkboxs: [],
    	onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
            	this.setPage(1);
            }
        },
        setShow: function(){
        	this.setPage(1);
        },
        exportExcel: function (table) {

            window.location.href = '/ImportarDatosSIU/ExportExcel';

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

        	loading('loading-bar');
        	loading('loading-circle', '#datatable', 'Consultando datos..');
        	
        	$.ajax({
        		type: "GET",
        		cache: false,
        		url: "/ImportarDatosSIU/CreateDataTable/",
        		data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $('#seccion1').unblock();        		 
        		    $('.loader-min-bar').hide();
        			if (session_error(msg) == false)
        			{
        			    $("#datatable").html(msg);

                         $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
        				    altClass: 'odd',
        				    footer: true,
        				    fixedColumns: 5
                        });
                    }
        		},
        		error: function (msg) {
                $('.loader-min-bar').hide();
        			session_error(msg);
        		}
        	});
        }
    }
}();

function verDetalles() {

    $.ajax({
        type: "GET",
        cache: false,
        url: "/ImportarDatosSIU/getDetallesConflictoProfs",
        success: function (msg) {
            $("#divDetallesConflictos").html(msg);
            $("#modal-detalles-conflictos").modal('show');
        },
        error: function (data) {
            session_error(data);
        }
    });
}