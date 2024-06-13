
var ciclos1 = new Ciclos("Ciclo");
var periodos1 = new Periodos("Periodo");
var niveles = new Niveles("niveles");

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
        niveles.init("niveles");

       /* DataTable.init();*/
    });

});//End function jquery



function handlerdataCiclos() {
    periodos1.id_ciclo = $("#Ciclo").val();
    periodos1.init("periodos1");
}

function handlerdataPeriodos() {
}
function handlerdataNiveles() {
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
            $("#idBanner").val("");

        },
        consultar: function () {
            //DataTable.init();
            DataTable.setPage(1);
        }

    
    }
}();

var DataTable = function () {
    var pag = 1;
    var order = "IDSUI";
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

            window.location.href = '/VerDatosComplementarios/ExportExcel';

        },
        edit: function (id) {
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

            $.ajax({
                type: "GET",
                cache: false,
                url: "/VerDatosComplementarios/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);

                    //$('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                    //    altClass: 'odd',
                    //    footer: true,
                    //    fixedColumns: 1
                    //});
                }
            });
        }
    }
}();

;