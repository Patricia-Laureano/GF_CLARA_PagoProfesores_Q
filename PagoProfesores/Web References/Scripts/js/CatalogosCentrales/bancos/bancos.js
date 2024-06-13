$(function () {

    $(window).load(function () {
        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        formValidation.Inputs(["clave", "banco"]);
        formValidation.notEmpty('clave', 'El campo clave no debe de estar vacio');
        formValidation.onlyNumbers('clave');
        formValidation.notEmpty('banco', 'El campo Banco no debe estar vacio');

    });
});//End function jquery


var formPage = function () {

    var idBanco;

    "use strict"; return {

        clean: function ()
        {
            formValidation.Clean();


            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").prop("disabled", true);
            $("#formbtndelete").hide();

            $("#clave").prop("disabled", false);
        },

        edit: function (id) {

            idBanco = id;

            var model = {
                Clave: id
            }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Bancos/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#clave").val(data.Clave);
                    $("#banco").val(data.Banco);
                    $("#Transferencias").val(data.Transferencias);

                    $("#clave").prop("disabled", true);
                    

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
                Clave: idBanco,
                Banco: $("#banco").val(),
                Transferencias: $("#Transferencias").val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Bancos/Save/",
                data: model,
                success: function (data) {
                    
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
                Clave: idBanco
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Bancos/Delete/",
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
                Clave: $("#clave").val(),
                Banco: $("#banco").val(),
                Transferencias: $("#Transferencias").val(),
            }

            if (!formValidation.Validate())
                return;
            
            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Bancos/Add/",
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
    var order = "CVE_BANCO";
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

            window.location.href = '/Bancos/ExportExcel';

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

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Bancos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();


