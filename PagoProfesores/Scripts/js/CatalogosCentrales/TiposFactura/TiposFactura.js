$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        formValidation.Inputs(["Cve_TipoFactura", "TipoFactura", "TipoFacturaDescripcion"]);

        formValidation.notEmpty('Cve_TipoFactura', 'El campo Clave no debe de estar vacio');
        formValidation.notEmpty('TipoFactura', 'El campo Tipo de Factura no debe de estar vacio');
        formValidation.notEmpty('TipoFacturaDescripcion', 'El campo Descripción no debe de estar vacio');
        
    });
});//End function jquery

var formPage = function () {
    var Cve_TipoFactura;

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

            $("#Cve_TipoFactura").prop("disabled", false);
        },

        edit: function (id)
        {
            Cve_TipoFactura = id;
            var model =
                {
                    Cve_TipoFactura: id
                }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/TiposFactura/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#Cve_TipoFactura").val(data.Cve_TipoFactura);
                    $("#TipoFactura").val(data.TipoFactura);
                    $("#TipoFacturaDescripcion").val(data.TipoFacturaDescripcion);

                    $("#Cve_TipoFactura").prop("disabled", true);

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

            var model =
                {
                    Cve_TipoFactura                     : Cve_TipoFactura,
                    TipoFactura                         : $("#TipoFactura").val(),
                    TipoFacturaDescripcion              : $("#TipoFacturaDescripcion").val(),
                }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/TiposFactura/Save/",
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
                $('#modal-delete-tiposfactura').modal("show");
                return;
            }

            $('#modal-delete-tiposfactura').modal("hide");

            var model = { 
                Cve_TipoFactura: Cve_TipoFactura
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/TiposFactura/Delete/",
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
                Cve_TipoFactura                         : $("#Cve_TipoFactura").val(),
                TipoFactura                             : $("#TipoFactura").val(),
                TipoFacturaDescripcion                  : $("#TipoFacturaDescripcion").val(),
                
            }

            
            if (!formValidation.Validate())
                return;

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/TiposFactura/Add/",
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
    var order = "Cve_TipoFactura";
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

            window.location.href = '/TiposFactura/ExportExcel';

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
                url: "/TiposFactura/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();


