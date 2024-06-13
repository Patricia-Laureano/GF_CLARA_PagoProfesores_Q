$(function () {

    $(window).load(function () {


       
    });
});//End function jquery


var formPage = function () {
    var idContrato;

    "use strict"; return {

        PDF: function (idContrato) {

            var model = {
                Clave: idContrato,
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ContratosWebPreview/getHTML/",
                data: model,
                success: function (data) {

                    window.open('/ContratosWeb/ConvertPDF');
                }

            });

        },

        clean: function () {
           
        },

        edit: function (id) {

            idContrato = id;

            this.PDF();


        },

        save: function () {

           

        },

        delete: function (confirm) {

         

        },

        add: function () {

          
        }

    }


}();


var DataTable = function () {
    var pag = 1;
    var order = "Cve_Contrato";
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

            window.location.href = '/ContratosWeb/ExportExcel';

        },

        edit: function (id) {
          //  formPage.edit(id);
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



            /*$('div.block').block({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src="/Content/images/load-search.gif" /><br><h2> Buscando..</h2>'
            });*/



            $.ajax({
                type: "GET",
                cache: false,
                url: "/ContratosWebPreview/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {

                    //$('div.block').unblock();
                    $("#datatable").html(msg);

                }

            });
        }

    }


}();


