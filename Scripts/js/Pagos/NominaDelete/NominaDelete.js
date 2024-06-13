
var ciclo1 = new Ciclos('Ciclo');
var periodo1 = new Periodos('Periodo');
var escuela1 = new Escuelas('Escuela');
var partePeriodo1 = new PartePeriodo('PartePeriodo');

var tipospago1 = new TiposdePagos("TipoPago");

$(function () {
    $(document).ready(function () {
        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 3
        });
    });

    $(window).load(function () {
        $("#formbtngenerarEstadoC").hide();

        ciclo1.init("ciclo1");
        tipospago1.init("tipospago1");
        escuela1.init("escuela1");
        partePeriodo1.init("partePeriodo1");
        
    });
});//End function jquery

function handlerdataSedes() { }

function handlerdataCiclos() {
    
    if((gup('Ciclo')!="")){  $('#Ciclo').val(gup('Ciclo')); }

    periodo1.id_ciclo = $("#Ciclo").val();
    periodo1.init("periodo1");
}

function handlerdataEscuelas() {
    $('#Escuela').val(gup('Escuela'));   
}

$('#Periodo').on('change', function () {
    if ($('#Periodo').val() == "") {
        return;
    }
});

function handlerdataPeriodos() {
    $('#Periodo').val(gup('Periodo'));
}

function handlerdataPartePeriodo() {
    if ((gup('PartePeriodo') != "")) { $('#PartePeriodo').val(gup('PartePeriodo')); }
}

function handlerdataTiposPagos() {
    $('#TipoPago').val(gup('TipoPago'));
}

$('#Ciclo').on('change', function () {
    periodo1.id_ciclo = this.value;
    periodo1.init("periodo1");
});

var formPage = function () {
    var id;

    "use strict"; return {

        clean: function () {         
            $('#_MODAL_DELETE_NOMINA').modal('show');
        },
        
        ocultar: function () {
            $('#_MODAL_DELETE_NOMINA').modal('hide');
        },

        _delete_all: function () {


            if ($("#Periodo").val() == "") {
                alert("debe seleccionar un periodo");
                return;
            }



            var model = {
                Periodo: $("#Periodo").val(),
            //    PartePeriodo: $("#PartePeriodo").val(),
              //  Escuela: $("#Escuela").val(),
              //  TipoPago: $("#TipoPago").val(),
                CampusVPDI: $('#Sedes').val(),
            }

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Eliminando..');
            loading('loading-circle', '#seccion1', 'Eliminando..');

            $('#_MODAL_DELETE_NOMINA').modal('hide');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/NominaDelete/deleteNominaAll/",
                data: model,
                success: function (data) {

                    $('.loader-min-bar').hide();
                    $('html, body').animate({ scrollTop: 0 }, 'fast');                
                    
                    $('#notification').html(data.msg);
                    DataTable.init();

                }, error: function (msg) {
                    session_error(msg);                   
                }
            });
        },



       /* _scriptEliminarBasura: function () {

            
            if (("#Periodo").val() == "") {

                return;
            }


            var model = {
                Periodo: $("#Periodo").val(),
               // PartePeriodo: $("#PartePeriodo").val(),
                //Escuela: $("#Escuela").val(),
               // TipoPago: $("#TipoPago").val(),
                CampusVPDI: $('#Sedes').val(),
            }

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Eliminando..');
            loading('loading-circle', '#seccion1', 'Eliminando..');

            $('#_MODAL_DELETE_NOMINA').modal('hide');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/NominaDelete/ScriptEliminarBasura/",
                data: model,
                success: function (data) {

                    $('.loader-min-bar').hide();
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $('#notification').html(data.msg);
                    DataTable.init();

                }, error: function (msg) {
                    session_error(msg);
                }
            });
        },*/



        
        _delete_seleccionados: function () {
            var arr = DataTable.checkboxs;
            var arrChecked = [];

            for (var i = 0; i < arr.length; i++) {
                var checkbox_checked = $('#' + arr[i]).prop('checked');

                if (checkbox_checked == true)
                    arrChecked.push(arr[i]);
            }

            if (arrChecked.length == 0) {
                alert('Debes seleccionar una casilla');
                return;
            }

            var model = {
                CampusVPDI: $('#Sedes').val(),
                ids: arrChecked.join()
            }

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Eliminando..');
            loading('loading-circle', '#seccion1', 'Eliminando..');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/NominaDelete/deleteNominaSelect/",
                data: model,
                success: function (data) {
                    $('#_MODAL_DELETE_NOMINA').modal('hide');

                    $('.loader-min-bar').hide();
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $('#notification').html(data.msg);
                    DataTable.init();
                },
                error: function (msg) {
                    session_error(msg);
                }
            });
        },

        Consultar: function () {
            DataTable.init();
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
                pag = 1;
                this.init();
            }
        },

        exportExcel: function (table) {
            var data = '?Periodo=' + $('#Periodo').val()
                + '&OpcionPago=' + $('#TipoPago').val()
                + '&CampusVPDI=' + $('#Sedes').val();
            window.location.href = '/NominaDelete/ExportExcel' + data;
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
            var search = $('#' + this.myName + '-searchtable').val();
            var orderby = order;
            var sorter = sort;
            
            var data = '&Periodo='    + $('#Periodo').val()
                     + '&PartePeriodo=' + $('#PartePeriodo').val()
                     + '&OpcionPago=' + $('#TipoPago').val()                                         
                     + '&CampusVPDI=' + $('#Sedes').val()
                     + '&Escuela=' + $('#Escuela').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/NominaDelete/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + data,
                success: function (msg) {
                    $('.loader-min-bar').hide();
                    if (session_error(msg) == false) {
                        $("#datatable").html(msg);

                        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                            altClass: 'odd',
                            footer: true,
                            fixedColumns: 3
                        });
                    }
                }
            });
        }
    }
}();

function gup(name) {

    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var tmpURL = window.location.href;
    var results = regex.exec(tmpURL);
    if (results == null)
        return "";
    else
        return results[1];
}