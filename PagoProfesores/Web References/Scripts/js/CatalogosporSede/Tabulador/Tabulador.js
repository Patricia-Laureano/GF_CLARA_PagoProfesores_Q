$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        formValidation.Inputs([ "tabulador", "montoHora"]);
        //formValidation.onlyNumbers("tabulador");
        formValidation.onlyNumbers("montoHora");
        formValidation.notEmpty('tabulador', 'El campo Tabulador no debe estar vacio');
        formValidation.notEmpty('montoHora', 'El campo Monto por hora no debe estar vacio');
        
        DataTable.init();

    });
});//End function jquery


/*var Sedes = function () {

    "use strict"; return {

        setSedes: function () {
            DataTable.init();
        }
    }
}();*/

var formPage = function () {
    var idTabulador;

    "use strict"; return {

        clean: function () {
            formValidation.Clean();
            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").prop("disabled", true);
            $("#formbtndelete").hide();
            $("#claveNivel").prop("disabled", false);
        },

        edit: function (id) {

            idTabulador = id;
            
            var model = {
                idTabulador: id
            }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Tabulador/Edit",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');
                    $("#claveNivel").val(data.ClaveNivel);
                    $("#tabulador").val(data.Tabulador);
                    $("#montoHora").val(data.MontoHora);
                    //$("#claveSede").val(data.ClaveSede);
                   // $("#claveNivel").prop("disabled", true);
                    $("#formbtnadd").hide();
                    $("#formbtnsave").show();
                    $("#formbtndelete").show();
                    $("#formbtnadd").prop("disabled", true);
                    $("#formbtnsave").prop("disabled", false);
                    $("#formbtndelete").prop("disabled", false);
                }
            });
        },

        save: function () {
            var model = {
                idTabulador : idTabulador,
                ClaveNivel: $("#claveNivel").val(),
                Tabulador: $("#tabulador").val(),
                MontoHora: $("#montoHora").val(),
                ClaveSede: $("#claveSede").val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Tabulador/Save/",
                data: model,
                success: function (data) {
                  //  formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        delete: function (confirm) {

            if (!confirm) {
                $('#modal-delete-banco').modal("show");
                return;
            }

            $('#modal-delete-banco').modal("hide");

            var model = {
                idTabulador: idTabulador
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Tabulador/Delete/",
                data: model,
                success: function (data) {
                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        add: function () {

            var model = {

                ClaveNivel: $("#claveNivel").val(),  
                Tabulador: $("#tabulador").val(),
                MontoHora: $("#montoHora").val(),
                sede: $('#Sedes').val(),
            }
            if (!formValidation.Validate())
                return;

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Tabulador/Add/",
                data: model,
                success: function (data) {
                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        }
    }
}();


var DataTable = function () {
    var pag = 1;
    var order = "CVE_NIVEL";
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
           var sede = $('#Sedes').val();           
           window.location.href = '/Tabulador/ExportExcel?sede=' + sede;
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
                url: "/Tabulador/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter +  "&filter=" + filter + "&sesion=null",
                success: function (msg) {
                    //$('div.block').unblock();
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();





