$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        formValidation.Inputs(["concepto", "descripcion"]);
        formValidation.notEmpty('concepto', 'El campo Concepto no debe de estar vacio');
        formValidation.onlyNumbers('clave');
        formValidation.notEmpty('descripcion', 'El campo Descripción no debe estar vacio');

        DataTable.init();

    });

    
});//End function jquery

/*var Sedes = function(){

    "use strict"; return {

        setSedes: function () {
            DataTable.init();
        }
    }
}();*/

var formPage = function () {

    var idConcepto;

    "use strict"; return {

        clean: function () {

            formValidation.Clean();

            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").prop("disabled", true);
            $("#formbtndelete").hide();

            $("#concepto").prop("disabled", false);
        },

        edit: function (id) {

            idConcepto = id;

            var model = {
                Concepto: id
            }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConceptosPago/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#concepto").val(data.Concepto);
                    $("#descripcion").val(data.Descripcion);

                    $("#concepto").prop("disabled", true);
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
                Concepto: idConcepto,
                Descripcion: $("#descripcion").val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConceptosPago/Save/",
                data: model,
                success: function (data) {

                   // formPage.clean();
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
                Concepto: idConcepto
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConceptosPago/Delete/",
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
                Concepto: $("#concepto").val(),
                Descripcion: $("#descripcion").val(),
                sede: $('#Sedes').val(),
            }

            if (!formValidation.Validate())
                return;

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConceptosPago/Add/",
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
    var order = "CONCEPTO";
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

            window.location.href = '/ConceptosPago/ExportExcel';

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
            var filter = $('#Sedes').val();

            $.ajax({
                type: "GET",
                cache: false,
                url: "/ConceptosPago/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&filter=" + filter+ "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();


