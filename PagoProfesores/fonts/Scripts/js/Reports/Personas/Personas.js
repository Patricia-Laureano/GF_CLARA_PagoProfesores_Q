

var periodos = new Periodos("periodos");
var ciclos = new Ciclos("ciclos");
var niveles = new Niveles("niveles");

$(function () {

    $(window).load(function () {
        ciclos.init("ciclos");
        niveles.init("niveles");
        formPage.clean();
    });
});//End function jquery


var formPage = function () {


    "use strict"; return {
        clean: function () {
            $("#ciclos").val("");
            $("#periodos").val("");
            $("#niveles").val("");
        },

        consultar: function () {

            DataTable.init();


        },

        ConsultaCiclos: function () {
            $.ajax({
                type: "GET",
                cache: false,
                url: "/Personas/ConsultaCiclos/",
                success: function (msg) {
                    if (session_error(msg) == false) {
                        $('#Ciclo').html(msg);
                        formPage.Ciclo_OnChange();

                    }
                },
                error: function (msg) { session_error(msg); }
            });
        },

        ConsultaPeriodos: function () {

            DataTable.init()
        },

        ConsultaNiveles: function () {

            DataTable.init()
        },

        Ciclo_OnChange: function (id) {


            periodos.id_ciclo = id;
            periodos.init("periodos");
            niveles.init("niveles");

        },

    }


}();


var DataTable = function () {
    var controller = "Personas"
    var pag = 1;
    var order = "CVE_CICLO";
    var sortoption = {
        ASC: "ASC",
        DESC: "DESC"
    };
    var sort = sortoption.ASC;

    "use strict"; return {

        onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
                pag = 1
                this.init();
            }
        },
        exportExcel: function (table) {

            window.location.href = '/' + controller + '/ExportExcel';

        },

        setShow: function (page) {
            pag = 1
            this.init();
        },

        edit: function (id) {
            // formPage.edit(id);
        },

        setPage: function (page) {
            pag = page;
            this.init();
        },

        Orderby: function (campo) {
            order = campo;
            var sortcampo = $("#SORT-" + campo).data("sort");
            if (sortcampo == sortoption.ASC) { sort = sortoption.DESC; } else { sort = sortoption.ASC; }
            this.init();
        },

        init: function () {

            var show = $("#data-elements-length").val();
            var search = $("#searchtable").val();
            var orderby = order;
            var sorter = sort;
            var filterC = $("#ciclos").val();
            var filterP = $("#periodos").val();
            var filterN = $("#niveles").val();
            $.ajax({
                type: "GET",
                cache: false,
                url: "/Personas/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&filterC=" + filterC + "&filterP=" + filterP + "&filterN=" + filterN,
                success: function (msg) {

                    $("#datatable").html(msg);
                }

            });
        }

    }

}();


