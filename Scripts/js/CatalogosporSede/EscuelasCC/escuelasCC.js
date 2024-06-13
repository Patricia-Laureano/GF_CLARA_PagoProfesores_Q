$(function () {

    $(window).load(function () {
  
    

        DataTable.init();

    });
});//End function jquery



function Edit(id,obj) {  

   var idEscuela = id;

  //  alert(idEscuela);

    var model = {
        idEscuela: idEscuela,
        Sede: $("#Sedes").val(),
        tipo: obj.value
    }

   

    $.ajax({
        type: "POST",
        dataType: 'json',
        cache: false,
        url: "/EscuelasCC/Save/",
        data: model,
        success: function (data) {
            //  formPage.clean();
            $('#notification').html(data.msg);
            DataTable.init();
        }
    });


}


/*var Sedes = function () {

    "use strict"; return {

        setSedes: function () {
            DataTable.init();
        }
    }
}();*/



var DataTable = function () {
    var pag = 1;
    var order = "CVE_ESCUELA";
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

        exportExcel: function () {
            var sede = $('#Sedes').val();
                
         //   window.location.href = '/EscuelasCC/ExportExcel';
            window.location.href = '/EscuelasCC/ExportExcel?sede=' + sede;
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

            /*$('div.block').block({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src="/Content/images/load-search.gif" /><br><h2> Buscando..</h2>'
            });*/

            $.ajax({
                type: "GET",
                cache: false,
                url: "/EscuelasCC/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter +  "&filter=" + filter + "&sesion=null",
                success: function (msg) {
                    //$('div.block').unblock();
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();





