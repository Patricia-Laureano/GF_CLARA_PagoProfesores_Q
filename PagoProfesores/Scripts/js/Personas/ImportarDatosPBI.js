var ciclos1 = new Ciclos("Ciclo");
var periodos1 = new Periodos("Periodo");

$(function () {

    $(document).ready(function () {
        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 5
        });
    });

    $(window).load(function () {
        ciclos1.init("ciclos1");
        formPage.clean();
        mostrarSedes();
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

        const changeText = document.querySelector("#fechaf");
        changeText.addEventListener("click", function () {

            if ($("#fechaf").val() != "") {
                document.getElementById("formbtnadd2").style.display = "none";
                document.getElementById("divSociedades").innerHTML = "";
            }

        });

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

            for (let i = 0; i < document.f1.elements.length; i++) {
                if (document.f1.elements[i].type == "checkbox") {
                    if (document.f1.elements[i].checked == true) {
                        if (document.getElementById("chesSeleccionados").value == '0') {
                            document.getElementById("chesSeleccionados").value = document.f1.elements[i].value;
                        } else {
                            document.getElementById("chesSeleccionados").value += ',' + document.f1.elements[i].value;
                        }
                    }
                }
            }
            var SedesSelec = document.getElementById("chesSeleccionados").value;
            var data =
                'Periodo=' + $('#Periodo').val()
                + '&Sedes=' + SedesSelec//$('#Sedes').val()
                + '&IDSIU=' + $('#idSiu').val()
                + '&fechai=' +$('#fechai').val()
                + '&fechaf=' + $('#fechaf').val();
           
            var controller = "Consultar";
            //if ($('#idPersona').val() != "") { controller = "ConsultarPersona"; }

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Transfiriendo datos de BANNER..');
            loading('loading-circle', '#seccion1', 'Consultando BANNER..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ImportarDatosPBI/" + controller,
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
    }
}();

var DataTable = function () {
    var pag = 1;
    var order = "IDBANNER";
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
        setShow: function () {
            this.setPage(1);
        },
        exportExcel: function (table) {

            window.location.href = '/ImportarDatosPBI/ExportExcel';

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
                url: "/ImportarDatosPBI/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $('#seccion1').unblock();
                    $('.loader-min-bar').hide();
                    if (session_error(msg) == false) {
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
        url: "/ImportarDatosPBI/getDetallesConflictoProfs",
        success: function (msg) {
            $("#divDetallesConflictos").html(msg);
            $("#modal-detalles-conflictos").modal('show');
        },
        error: function (data) {
            session_error(data);
        }
    });
}

function verDetallesSCP(idsuis) {

    $.ajax({
        type: "GET",
        cache: false,
        url: "/ImportarDatosSIU/getDetallesSinCP",
        data: "idssuis=" + idsuis,
        success: function (msg) {
            $("#divDetallesConflictos").html(msg);
            $("#modal-detalles-conflictos").modal('show');
        },
        error: function (data) {
            session_error(data);
        }
    });
}

$('#idSiu').keypress(function (e) {

    if (e.which == 13) {
        event.preventDefault();
        var model = {
            IdSIU: this.value,
            CveSede: $('#Sedes').val(),
        }

        $.ajax({
            type: "POST",
            dataType: 'json',
            cache: false,
            url: "/ImportarDatosSIU/BuscaPersona/",
            data: model,
            success: function (data) {

                data = jQuery.parseJSON(data);

                //$("#idSiu").val(data.IdSIU);
                $("#idSiuHidden").val(data.IdPersona);
                $("#hdfCorreo0365").val(data.correoO365);

                $("#formbtnConsultar").prop("disabled", false);

                formPage.consultar();
            },
            error: function (msg) {
                session_error(msg);
                $('#notification').html(msg);
            }
        });
    }
});

function mostrarSedes() {

    $.ajax({
        type: "POST",
        dataType: 'json',
        cache: false,
        url: "/ImportarDatosPBI/mostrarSedes/",
        data: {},
        success: function (data) {
            if (data != null) {
                var listado = "";
                var sedesArray = data;
                var sedes = sedesArray.split('/');
                var linea = 0;

                for (var i = 0; i < sedes.length; i++) {

                    listado += "<input type='checkbox' value='" + sedes[i].split('|')[0] + "' id='sedesCheckbox' style='padding-left:9px;' /><label for='" + sedes[i].split('|')[0] + "' style='padding-right:7px;'>" + sedes[i].split('|')[1] + "</label>";

                    if (linea == 3 || linea == 7 || linea == 11) {
                        listado += "<br /> \r\n";
                    }

                    linea++;
                }
            }
            document.getElementById("sedesCheck").innerHTML = listado;
        }
    });
}

function WarningDetailPowerBIConflicto(datoss) {

    $.ajax({
        type: "GET",
        cache: false,
        url: "/ImportarDatosPBI/getDetallesPOWERBI_Conf",
        data: "datoss=" + datoss,
        success: function (msg) {
            $("#divDetallesConflictos").html(msg);
            $("#modal-detalles-conflictos").modal('show');
        },
        error: function (data) {
            session_error(data);
        }
    });
}