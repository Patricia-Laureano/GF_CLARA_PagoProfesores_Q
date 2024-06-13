﻿
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
            fixedColumns: 3
        });
    });

    $(window).load(function () {
        formValidation.Inputs(["fechai", "fechaf"]);
    });
});//End function jquery

var formPage = function () {
    var idEstadoCuenta;
    "use strict"; return {

        clean: function () {
            formValidation.Clean();
            consultar = false;

            $("#fechai").val('');
            $("#fechaf").val('');

            DataTable.setShow();
            DataTable.init();
        },

        Concultar: function (id) {

            if (!formValidation.Validate())
                return;

            var cve_tipodepago = $("#cve_tipodepago").val();

            consultar = true;
            DataTable.consultar = true;
            DataTable.setShow();
            DataTable.init();
        },

    }
}();

Sedes.setSedes_success = function () {
    if (!formValidation.Validate())
        return;

    consultar = true;
    DataTable.init();
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
            var datos = 'fechai=' + $('#fechai').val()
                + '&fechaf=' + $('#fechaf').val()
                + '&sedes=' + $('#Sedes').val();

            window.location.href = '/SeguimientoDocentesFacturas/ExportExcel?' + datos;
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

            var filter = "";
            if (consultar) { filter = $('#Sedes').val(); }

            var cve_tipodepago = $("#cve_tipodepago").val();

            var datos = '&fechai=' + $('#fechai').val()
                + '&fechaf=' + $('#fechaf').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/SeguimientoDocentesFacturas/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter + "&cve_tipodepago=" + cve_tipodepago,
                success: function (msg) {

                    $('.loader-min-bar').hide();
                    $("#datatable").html(msg);
                    $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                        altClass: 'odd',
                        footer: true,
                        fixedColumns: 3
                    });
                }
            });
        }
    }
}();

function returnFacturas() {
    window.location.href = '/SeguimientoDocentesFacturas/';
}