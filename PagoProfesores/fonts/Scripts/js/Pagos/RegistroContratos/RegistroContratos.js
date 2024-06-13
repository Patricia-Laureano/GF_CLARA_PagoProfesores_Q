var ciclos1 = new Ciclos("Ciclo");
var periodos1 = new Periodos("Periodo");
var niveles1 = new Niveles("Nivel");

var tipospago1 = new TiposdePagos("tipoPago");

$(function () {


    $(document).ready(function () {

        /*$('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 2
        });*/

    });



    $(window).load(function () {

        formValidation.Inputs(["fechaEntregaRC", "id_siuRC"]);

       /* $("#fechaEntregaRC").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: 'dd/mm/yy'
        });*/

      
        
        ciclos1.init("ciclos1");
        niveles1.init("niveles1");
        tipospago1.init("tipospago1");
    });
});//End function jquery

function handlerdataSedes() { }

function handlerdataCiclos() {

	periodos1.id_ciclo = $("#ciclos").val();
	periodos1.init("periodos1");

}

$('#Ciclo').on('change', function () {
	periodos1.id_ciclo = this.value;
	periodos1.init("periodos1");
});


function handlerdataNiveles() {
}

function handlerdataPeriodos() {
}


function handlerdataTiposPagos() {

}



var formPage = function () {

	var idSIU;
	var idPersona;
	var IDContrato;

	"use strict"; return {

		clean: function () {
			formValidation.Clean();
		},

		getIdPersona : function(){
			return idPersona;
		},

		consultar: function () {
			DataTable.init();
		},

		edit: function (id) {	  


		    IDContrato = id;

			var model = {
			    IdContratos: id
			}

			this.clean();

			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/RegistroContratos/Edit/",
				data: model,
				success: function (data) {
					data = jQuery.parseJSON(data);
					
					idPersona = data.IdPersona;
					$("#IdContrato").val(data.IdContrato);
					$("#id_siuRC").val(data.IdSIU);

					$("#idEsquema").val(data.idEsquemas);
					$("#periodos").val(data.periodos);
			
					$("#nombreRC").val(data.Nombre);				
				    //	$("#fechaEntregaRC").val(convertDate(data.FechaEntrega));

					var strDate= FechaActual();				
					$("#fechaEntregaRC").val(strDate);				

					$('#modal-edit-fecha').modal("show");
				}
			});
		},

		save: function () {

		    var model = {


		        IdContratos: IDContrato,
				IdPersona: idPersona,
				FechaEntrega: $("#fechaEntregaRC").val(),
				idEsquemas: $("#idEsquema").val(),
			    periodos: $("#periodos").val()
			}
			/*
			if (model.FechaEntrega === "" || model.FechaEntrega === null) {
				$('#notification').html(formValidation.getMessage('El campo "Fecha de entrega" no debe de estar vacio'));
				return;
			}
			*/
			$.ajax({
				type: "POST",
				dataType: 'json',
				cache: false,
				url: "/RegistroContratos/Save/",
				data: model,
				complete: function (data) {
					$('#modal-edit-fecha').modal("hide");
				},
				success: function (data) {

					$('#notification').html(data.msg);
					DataTable.init();
				}
			});
		},


		verContrato: function (cve_contrato, cve_sede, periodo, cve_nivel, id_esquema, IDSIU) {
		    $.ajax({
		        type: "GET",
		        cache: false,
		        url: '/ECW_Contratos/SetContrato',
		        //url: "/EstadodeCuentaWeb/SetContrato",
		        data: "cve_contrato=" + cve_contrato + "&cve_sede=" + cve_sede + "&periodo=" + periodo + "&cve_nivel=" + cve_nivel + "&id_esquema=" + id_esquema+"&IDSIU="+IDSIU ,
		        success: function (msg) {
		            if (msg == "0")
		                window.open('/ECW_Contratos/ConvertPDF');
		        }
		    });
		},


		onkeyup_search_id: function (e) {
		    var enterKey = 13;
		    if (e.which == enterKey) {
		        pag = 1;
		        $('#DataTable-searchtable').val('')
		        DataTable.init();
		    }
		},





		/*
        delete: function (confirm) {

            if (!confirm) {
                $('#modal-delete-registrocontratos').modal("show");
                return;
            }

            $('#modal-delete-registrocontratos').modal("hide");

            var model = {
                IdPersona: $("#id_personaRC").val(),
                Nombre: $("#nombreRC").val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/RegistroContratos/Delete/",
                data: model,
                success: function (data) {

                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        }*/
	}
}();

function convertDate(inputFormat) {

	if (inputFormat === "" || inputFormat === null || inputFormat === undefined) {
		return "";
	} else {
		function pad(s) { return (s < 10) ? '0' + s : s; }
		var d = new Date(inputFormat + 'T00:00:01');
		return [pad(d.getDate()), pad(d.getMonth()+1), d.getFullYear()].join('/');
	}

}

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

        onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
                pag = 1;
                this.init();
            }
        },

        exportExcel: function (table) {
          
            var datos = 'periodos=' + $('#Periodo').val()
           + '&tipospago=' + $('#tipoPago').val()
           + '&niveles=' + $('#Nivel').val()
           + '&sedes=' + $('#Sedes').val();         


            window.location.href = '/RegistroContratos/ExportExcel?' + datos;

        },

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
            var sortcampo = $('#' + this.myName + '-SORT-' + campo).data("sort");
            if (sortcampo == sortoption.ASC) { sort = sortoption.DESC; } else { sort = sortoption.ASC; }
            this.init();
        },

        init: function () {
          

                      
            var show = $('#' + this.myName + '-data-elements-length').val();


            var search;
            if ($('#idsiu').val() != "") {
                search = $('#idsiu').val();
                $('#' + this.myName + '-searchtable').val('')

            }// $('#' + this.myName + '-searchtable').val($('#idsiu').val());                           
            else {
                 search = $('#' + this.myName + '-searchtable').val();
            }


            var orderby = order;
            var sorter = sort;
            var filter = $('#Sedes').val();

            var Ciclo = $('#Ciclo').val();
            var Periodo = $('#Periodo').val();
            var Nivel = $('#Nivel').val();
            var tipoPago = $('#tipoPago').val();
            var IdPersona = formPage.getIdPersona();

            if (Ciclo == null) Ciclo = '';
            if (Periodo == null) Periodo = '';
            if (Nivel == null) Nivel = '';
            if (tipoPago == null) tipoPago = '';
            if (IdPersona == null) IdPersona = '';

             loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');


            $.ajax({
            	type: "GET",
            	cache: false,
            	url: "/RegistroContratos/CreateDataTable/",
            	data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null&filter=" + filter
					+ "&Ciclo=" + Ciclo + "&Periodo=" + Periodo + "&Nivel=" + Nivel + "&tipoPago=" + tipoPago + "&IdPersona=" + IdPersona,
            	success: function (msg) {
            	    $("#datatable").html(msg);

            	    $('.loader-min-bar').hide();

            	    /*if ($('#idsiu').val() != "") {            	      
            	        $('#' + this.myName + '-searchtable').val("")

            	    }*/

            		/*$('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            			altClass: 'odd',
            			footer: true,
            			fixedColumns: 2
            		});*/
            	},  error: function (msg) {
            	    session_error(msg);

            	   
            	    }
            });
        }
    }
}();


function FechaActual() {

    /*var d = new Date();
    var strDate = d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();*/



    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!

    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    var today = dd + '-' + mm + '-' + yyyy;
   

    return today;

}


