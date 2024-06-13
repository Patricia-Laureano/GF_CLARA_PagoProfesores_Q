
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

        setTimeout(function () { formPage.Consultar(); }, 1000);
    });
});//End function jquery

function handlerdataSedes() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/NominaXCDC/getCampusPA",
        success: function (msg) {
            $('#CampusPA').html(msg);
        },
        error: function (data) {
            session_error(data);
        }
    });
}

/*Sedes.setSedes_success = function () { 

}*/

function handlerdataCiclos() {    
    if((gup('Ciclo')!="")){  $('#Ciclo').val(gup('Ciclo')); }

    periodo1.id_ciclo = $("#Ciclo").val();
    periodo1.init("periodo1");
}

function handlerdataEscuelas() {
    $('#Escuela').val(gup('Escuela'));
    
    if( gup('CampusPA'))
    {
        $('#CampusPA').val(gup('CampusPA'));    
    }
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
            //Consultar
            $('#Ciclo').val('');
            $('#Periodo').val('');
            $('#TipoPago').val('');
            $('#Escuela').val('');
            $('#CampusPA').val('');

            DataTable.setPage(1);
        },

        //BOTON DE ATERIOR Y SIGUIENTE WIZARD
        BotonAntNomina: function () {//

            var datos = 'Periodo=' + $('#Periodo').val()
            + '&PartePeriodo=' + $('#PartePeriodo').val()
            + '&Ciclo=' + $('#Ciclo').val()
            + '&TipoPago=' + $('#TipoPago').val()
            + '&Escuela=' + $('#Escuela').val()
            + '&Campus=' + $('#Sedes').val()        
            + '&CampusPA=' + gup('CampusPA')
            + '&N=1';

            window.location.href = '/Nomina?' + datos;
        },

        BotonSgtEstadoCuenta: function () {
    
            $("#formbtnconsultar").hide()
            $("#formbtngenerarEstadoC").show()

            $("#tab-nominaxcdc").removeClass("active").addClass("");
            $("#tab-estadocuenta").addClass("active");            

            $("#linkant").attr("href", "javascript:formPage.BotonAntNominaXCDC()");
            $("#linksgt").attr("href", "javascript:formPage.BotonSgtImprimirEstadoCuenta()");
        },

        BotonAntNominaXCDC: function () {

            $("#formbtnconsultar").show()
            $("#formbtngenerarEstadoC").hide()

            $("#tab-estadocuenta").removeClass("active").addClass("");
            $("#tab-nominaxcdc").addClass("active");

            $("#linkant").attr("href", "javascript:formPage.BotonAntNomina()");
            $("#linksgt").attr("href", "javascript:formPage.BotonSgtEstadoCuenta()"); 
        },
        
        BotonSgtImprimirEstadoCuenta: function () {
            window.location.href = '/EstadodeCuenta';
        },

        Consultar: function () {
            //$("#frm-listadoNominaXCDC").show();
            DataTable.init();
            $('#btn-sgt').show();
        },

        edit: function (id) {

            var model = {
                IdPA: id
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Nomina/Edit/",
                data: model,
                success: function (data) {

                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#PAID").val(data.IdPA);
                    $("#nrc").val(data.NRC);
                    $("#materia").val(data.NombreMateria);
                    $("#profesor").val(data.Profesor);

                    $("#perfilNomina").html(" Profesor: " + data.IdSiu + " - " + data.Profesor + ", NRC: " + data.NRC + ", Materia: " + data.NombreMateria);

                    $("#frm-grabar").show();
                    comboCentroCostos1.CampusVPDI = data.CampusVPDI;
                    comboCentroCostos1.EscuelaCVE = $('#Escuela').val();
                    comboCentroCostos1.TipoFactura = data.OpcionPago;
                    comboCentroCostos1.CentroCostosID = data.CentroCostosID;
                    comboCentroCostos1.init("comboCentroCostos1");

                }
            });
        },

        ocultar: function (frm) {
        },

        generarEdoCta: function () {

            var model = {
                Periodo: $('#Periodo').val(),
                CampusVPDI: $('#Sedes').val(),
                Campus: $('#CampusPA').val(),
                Escuela: $('#Escuela').val(),
                TipoPago: $('#TipoPago').val(),
                PartePeriodo: $("#PartePeriodo").val(),
            }

            loading('loading-bar');
            loading('loading-circle', '#wizard', 'Generando estado de cuenta..');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/NominaXCDC/generarEdoCta/",
                data: model,
                success: function (data) {
                    loading('stop', '#wizard', '');
                    $('#notification').html(data.msg);
                    DataTable.init();

                },
                error: function (data) {
                    loading('stop', '#wizard', '');
                    session_error(data);
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
                pag = 1;
                this.init();
            }
        },

        exportExcel: function (table) {

            var data = '?Periodo=' + $('#Periodo').val()
         + '&OpcionPago=' + $('#TipoPago').val()
         + '&CampusVPDI=' + $('#Sedes').val();

            window.location.href = '/NominaXCDC/ExportExcel' + data;
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
            
            var data = '&Periodo='      + $('#Periodo').val()
                     + '&PartePeriodo=' + $('#PartePeriodo').val()
                     + '&OpcionPago='   + $('#TipoPago').val()                                         
                     + '&CampusVPDI='   + $('#Sedes').val()
                     + '&CampusPA='     + $('#CampusPA').val()
                     + '&Escuela='      + $('#Escuela').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/NominaXCDC/CreateDataTable/",
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