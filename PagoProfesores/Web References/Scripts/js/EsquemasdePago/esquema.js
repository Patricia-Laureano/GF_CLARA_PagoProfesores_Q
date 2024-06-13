var obj_anios = new Ciclos('anios');
var obj_periodos = new Periodos('periodos');
var obj_ciclofilter = new Ciclos('CicloFilter');
var obj_periodofilter = new Periodos('PeriodoFilter');

$(function () {
    var dateToday = new Date();

    var dates = $("#fechai, #fechaf").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        dateFormat: 'yy-mm-dd',
        numberOfMonths: 1,
        onSelect: function (selectedDate) {
            var option = this.id == "fechai" ? "minDate" : "maxDate",
                instance = $(this).data("datepicker"),
                date = $.datepicker.parseDate(instance.settings.dateFormat || $.datepicker._defaults.dateFormat, selectedDate, instance.settings);
            dates.not(this).datepicker("option", option, date);
        }
    });

    $("#fpago").datepicker({
    	changeMonth: true,
    	changeYear: true,
    	minDate: dateToday,
    	dateFormat: 'yy-mm-dd'
    });

    $("#frecibo").datepicker({
    	changeMonth: true,
    	changeYear: true,
    	minDate: dateToday,
    	dateFormat: 'yy-mm-dd'
    });

    if ($("#idesquema").val() == "") { $("#formbtnadd2").prop("disabled", true); }

    $(window).load(function () {

        obj_anios.init("obj_anios");
        obj_ciclofilter.init("obj_ciclofilter");

        $("#formbtnsave").hide();
        $("#formbtndelete").hide();
        $("#formbtnUpdateEdoCta").hide();
        $("#formbtnsave2").hide();
        $("#formbtndelete2").hide();

        formValidation.Inputs(["esquema", "anios", "periodos", "tipocontrato", "fechai", "fechaf", "nosemanas", "nospagos"]);
        formValidation.notEmpty('esquema', 'El campo esquema no debe de estar vacio');
        formValidation.notEmpty('anios', 'El campo esquema no debe de estar vacio');
        formValidation.notEmpty('periodos', 'El campo esquema no debe de estar vacio');
        formValidation.notEmpty('tipocontrato', 'El campo esquema no debe de estar vacio');
        formValidation.notEmpty('nosemanas', 'El campo esquema no debe de estar vacio');
        formValidation.notEmpty('nospagos', 'El campo esquema no debe de estar vacio');     

        DataTable.init();
    });
});//End function jquery

function handlerdataSedes() { }

function handlerdataCiclos() {
    obj_periodos.id_ciclo = $("#anios").val();
    obj_periodos.init("obj_periodos");

    obj_periodofilter.id_ciclo = $("#CicloFilter").val();
    obj_periodofilter.init("obj_periodofilter");
}

function handlerdataPeriodos() {}

$('#PeriodoFilter').on('change', function () {
    DataTable.init();
});

$('#CicloFilter').on('change', function () {
    obj_periodofilter.id_ciclo = this.value;
    obj_periodofilter.init("obj_periodofilter");
});

function dataSelectperiodo() {    
    hperiodos = $("#hperiodos").val();
    setTimeout(function () {
    $('#periodos option[value="' + hperiodos + '"]').attr('selected', 'selected');   }, 100);   
}

var formPage = function () {
    var idEsquema;

    "use strict"; return {
    	getIdEsquema: function(){
    		return idEsquema;
    	},
        clean: function () {
            formValidation.Clean();

            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").hide();
            $("#formbtndelete").prop("disabled", true);
            $("#clave").prop("disabled", false);
     
            $("#periodos").find('option').attr("selected", false);
            $("#tipocontrato").find('option').attr("selected", false);
            $("#anios").find('option').attr("selected", false);

            $("#esquema").val('');
            $("#fechai").val('');
            $("#fechaf").val('');
            $("#nosemanas").val('');
            $("#nospagos").val('');

            $("#bloqueop").prop("checked", "");

            $("#cpago").find('option').attr("selected", false);
            $("#tbloqueo").find('option').attr("selected", false);
            $("#fpago").val('');
            $("#frecibo").val('');
            $("#idesquema").val('');
            DataTable2.init();

            $("#formbtnadd2").show();
            $("#formbtnadd2").prop("disabled", true);
            $("#formbtnUpdateEdoCta").hide();
            $("#formbtnUpdateEdoCta").prop("disabled", true);
            $("#formbtnsave2").hide();
            $("#formbtnsave2").prop("disabled", true);
            $("#formbtndelete2").hide();
            $("#formbtndelete2").prop("disabled", true);
        },

        edit: function (id) {

            idEsquema = id;

            var model = {
                Clave: idEsquema
            }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#formbtnadd").hide();
                    $("#formbtnsave").show();
                    $("#formbtndelete").show();
                  
                    $("#formbtnadd").prop("disabled", true);
                    $("#formbtnsave").prop("disabled", false);
                    $("#formbtndelete").prop("disabled", false);

                    $("#idesquema").val(idEsquema);
                  
                    if ($("#idesquema").val() == "") { $("#formbtnadd2").prop("disabled", true); }
                    else { $("#formbtnadd2").prop("disabled", false); }
                    
                    $("#anios").val(data.anios);
                    $("#anios option[value=" + data.anios + "]").attr("selected", true);
                    
                    obj_periodos.Periodo = data.periodos;
                    obj_periodos.id_ciclo = data.anios;
                    obj_periodos.init("obj_periodos");

                    $("#hperiodos").val(data.periodos);
                    $("#tipocontrato").val(data.tipocontrato);
                    $("#esquema").val(data.esquema);
                    $("#fechai").val(data.Strmfechai);
                    $("#fechaf").val(data.Strmfechaf);
                    $("#nosemanas").val(data.nsemanas);
                    $("#nospagos").val(data.npagos);
               
                    if (data.bbloqueo == 1) {               
                        $("#bloqueop").attr("checked",true);
                    }
                    else {                                           
                        $("#bloqueop").attr("checked", false);
                    }

                    DataTable2.init();
                },
                complete: dataSelectperiodo
            });
        },

        save: function () { 
            var bloqueo = 0;
            if ($('#bloqueop').is(':checked')) {
                bloqueo = 1;
            }

            var model = {
                Clave: idEsquema,
                periodos: $("#periodos").val(),
                tipocontrato: $("#tipocontrato").val(),
                esquema: $("#esquema").val(),
                fechai: $("#fechai").val(),
                fechaf: $("#fechaf").val(),
                nsemanas: $("#nosemanas").val(),
                npagos: $("#nospagos").val(),
                bloqueo: bloqueo,
                conceptoPago: $("#cpago").val(),
                fechaPago: $("#fpago").val(),
                fechaRecibo: $("#frecibo").val(),
                tipoBloqueo: $("#tbloqueo").val(),
                sedes: $('#Sedes').val(), 
            }

            if (!formValidation.Validate()) {
                $('.nav-tabs a[href="#default-tab-1"]').tab('show');
                return;
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/Save/",
                data: model,
                success: function (data) {
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        delete: function (confirm) {

           if (!confirm) {
                $('#modal-delete-esquemas').modal("show");
                return;
            }

            $('#modal-delete-esquemas').modal("hide");

            var model = {
                Clave: idEsquema
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/Delete/",
                data: model,
                success: function (data) {
                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        add: function () {
            var bloqueo = 0;
            if ($('#bloqueop').is(':checked')) {
                bloqueo = 1;
            }

            var model = {
                periodos: $("#periodos").val(),
                tipocontrato: $("#tipocontrato").val(),
                esquema: $("#esquema").val(),
                fechai: $("#fechai").val(),
                fechaf: $("#fechaf").val(),
                nsemanas: $("#nosemanas").val(),
                npagos: $("#nospagos").val(),
                bloqueo: bloqueo,
                conceptoPago: $("#cpago").val(),
                fechaPago: $("#fpago").val(),
                fechaRecibo: $("#frecibo").val(),
                tipoBloqueo: $("#tbloqueo").val(),
                sedes: $('#Sedes').val(),
            }

            if (!formValidation.Validate()) {
                $('.nav-tabs a[href="#default-tab-1"]').tab('show');
                return;
            }
            
            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/Add/",
                data: model,
                success: function (data) {
                    $('#notification').html(data.msg);
                    $('#idesquema').val(data.IdEsquema);

                    if ($("#idesquema").val() != "") { $("#formbtnadd2").prop("disabled", false); }
                    else { $("#formbtnadd2").prop("disabled", true); }
                    DataTable.init();
                }
            });
        }
    }
}();

var DataTable = function () {
    var pag = 1;
    var order = "ID_ESQUEMA";
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
            var sedes = $('#Sedes').val();
            window.location.href = '/Esquemas/ExportExcel?sedes=' + sedes;
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

            var PeriodoFilter = $("#PeriodoFilter").val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Esquemas/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&PeriodoFilter=" + PeriodoFilter + "&filter=" + filter,
                success: function (msg) {
                    $('.loader-min-bar').hide();
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();

var formPage2 = function () {
    var idPagosF;

    "use strict"; return {
        clean: function () {

            $("#formbtnadd2").show();
            $("#formbtnadd2").prop("disabled", false);
            $("#formbtnUpdateEdoCta").hide();
            $("#formbtnUpdateEdoCta").prop("disabled", true);
            $("#formbtnsave2").hide();
            $("#formbtnsave2").prop("disabled", true);
            $("#formbtndelete2").hide();
            $("#formbtndelete2").prop("disabled", true);
            
            if ($("#idesquema").val() == "") { $("#formbtnadd2").prop("disabled", true); }
            else { $("#formbtnadd2").prop("disabled", false); }      

            $("#cpago").find('option').attr("selected", false);
            $("#tbloqueo").find('option').attr("selected", false);
            $("#fpago").val('');
            $("#frecibo").val('');

            $("#fpago").removeClass("form-control parsley-error").addClass("form-control");

            var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
            for (var i = 0; i < maxBloqueos; i++) {
            	$('#TipoBloqueo_' + i).prop('checked', false);
            }
        },

        edit: function (id) {

            idPagosF = id;

            var model = {
				Clave: formPage.getIdEsquema(),
                idPagosF: idPagosF
            }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/EditCalendarPagos/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#formbtnadd2").hide();
                    $("#formbtnUpdateEdoCta").show();
                    $("#formbtnsave2").show();
                    $("#formbtndelete2").show();
                    $("#formbtnadd2").prop("disabled", true);
                    $("#formbtnUpdateEdoCta").prop("disabled", false);
                    $("#formbtnsave2").prop("disabled", false);
                    $("#formbtndelete2").prop("disabled", false);

                    $("#cpago").val(data.conceptoPago);
                    $("#tbloqueo").val(data.tipoBloqueo);
                    $("#fpago").val(data.fechaPago);
                    $("#frecibo").val(data.fechaRecibo);

                    for (var i = 0; i < data.arrBloqueos.length; i++) {
                    	var idbloqueo = data.arrBloqueos[i];
                    	$("input[data-idbloqueo='" + idbloqueo + "']").prop('checked', true);
                    }
                }
            });
        },

        createModel: function (id) {
        	var arrBloqueos = new Array();
        	var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
        	for (var i = 0; i < maxBloqueos; i++) {
        		var bloqueo = $('#TipoBloqueo_' + i).val();
        		var activo = $('#TipoBloqueo_' + i).prop('checked');
        		if (activo) {
        			arrBloqueos.push(bloqueo);
        		}
        	}
        		
        	return {
        		Clave: idEsquema,
        		idPagosF: id,
        		conceptoPago: $("#cpago").val(),
        		fechaPago: $("#fpago").val(),
        		fechaRecibo: $("#frecibo").val(),
        		tipoBloqueo: $("#tbloqueo").val(),
        		npagos: $("#nospagos").val(),        	    
        		bloqueos: arrBloqueos.join(),
        	}
        },

        save: function () {
            idEsquema = $("#idesquema").val();
            var model = this.createModel(idPagosF);

            if (!formValidation.Validate()) {
                $('.nav-tabs a[href="#default-tab-1"]').tab('show');
                return;
            }

            if (!formPage2.validateInputs()) {
            	$('html,body').animate({ scrollTop: 0 }, 10);
            	$('#notification').html(formValidation.getMessage('El campo Fecha de Pago no debe de estar vacio'));
            	return;
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/SaveCalendarPagos/",
                data: model,
                success: function (data) {
                    formPage2.clean();
                    $('#notification').html(data.msg);
                    DataTable2.init();
                }
            });
        },

        delete: function (confirm) {
            if (!confirm) {
                 $('#modal-delete-calendario').modal("show");
                 return;
             }
 
             $('#modal-delete-calendario').modal("hide");

            var model = {
                idPagosF: idPagosF
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/DeleteCalendarPagos/",
                data: model,
                success: function (data) {
                    formPage2.clean();
                    $('#notification').html(data.msg);
                    DataTable2.init();
                }
            });
        },

        add: function () {

            idEsquema = $("#idesquema").val(); 
            
            var model = this.createModel(0);
            
            if (!formValidation.Validate()) {
                $('.nav-tabs a[href="#default-tab-1"]').tab('show');
                return;
            }

            if (!formPage2.validateInputs()) {
                $('html,body').animate({ scrollTop: 0 }, 10);
                $('#notification').html(formValidation.getMessage('El campo Fecha de Pago no debe de estar vacio'));
                return;
            }

            //hacer validaciones calendario a mano
            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/AddCalendarPagos/",
                data: model,
                success: function (data) {
                    formPage2.clean();
                    $('#notification').html(data.msg);
                    $('html,body').animate({ scrollTop: 0 }, 10);

                    DataTable2.init();
                }
            });
        },

        updateEdoCta: function () {
            idEsquema = $("#idesquema").val();
            var model = this.createModel(idPagosF);

            if (!formValidation.Validate()) {
                $('.nav-tabs a[href="#default-tab-1"]').tab('show');
                return;
            }

            if (!formPage2.validateInputs()) {
                $('html,body').animate({ scrollTop: 0 }, 10);
                $('#notification').html(formValidation.getMessage('El campo Fecha de Pago no debe de estar vacio'));
                return;
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Esquemas/updateEdoCtaCalendarPagos/",
                data: model,
                success: function (data) {
                    formPage2.clean();
                    $('#notification').html(data.msg);
                    DataTable2.init();
                }
            });
        },

        validateInputs: function () {
            var validado = true;

            if ($("#fpago").val() == "") { $("#fpago").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#fpago").removeClass("form-control parsley-error").addClass("form-control"); }

            if ($("#frecibo").val() == "") { $("#frecibo").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#frecibo").removeClass("form-control parsley-error").addClass("form-control"); }

            return validado;
        }
    }
}();

var DataTable2 = function () {
    var pag = 1;
    var order = "ID_ESQUEMA";
    var sortoption = {
        ASC: "ASC",
        DESC: "DESC"
    };
    var sort = sortoption.ASC;

    "use strict"; return {
        myName: 'DataTable2',

        onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
                pag = 1;
                this.init();
            }
        },

        exportExcel: function (table) {
            var sedes = $('#Sedes').val();
            window.location.href = '/Esquemas/ExportExcelClalendario?sedes=' + sedes;
        },

        edit: function (id) {
            formPage2.edit(id);
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
            show = show ? show : '';
            var search = $('#' + this.myName + '-searchtable').val();
            search = search ? search : '';
            var orderby = order;
            var sorter = sort;

            var idEsquema = $("#idesquema").val();
        
            $.ajax({
                type: "GET",
                cache: false,
                url: "/Esquemas/CreateDataTableCalendarP/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null&idEsquema=" + idEsquema,
                success: function (msg) {
                    $("#datatable2").html(msg);
                }
            });
        }
    }
}();