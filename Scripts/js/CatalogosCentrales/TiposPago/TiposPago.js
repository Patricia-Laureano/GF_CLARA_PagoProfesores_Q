$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        formValidation.Inputs(["Cve_TipoFactura", "Cve_TipodePago", "TipodePago", "TipodePagoDescripcion"]);

        formValidation.notEmpty('Cve_TipoFactura', 'El campo Tipo Factura no debe de estar vacio');
        formValidation.notEmpty('Cve_TipodePago', 'El campo Clave no debe de estar vacio');
        formValidation.notEmpty('TipodePago', 'El campo Tipo de Pago no debe de estar vacio');
        formValidation.notEmpty('TipodePagoDescripcion', 'El campo Descripción no debe de estar vacio');
        
    });
});//End function jquery

var formPage = function () {
    var Cve_TipodePago;

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

            $("#Cve_TipodePago").prop("disabled", false);
        },

        edit: function (id)
        {
            Cve_TipodePago = id;
            var model =
                {
                    Cve_TipodePago: id
                }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/TiposPago/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#Cve_TipoFactura").val(data.Cve_TipoFactura);
                    $("#Cve_TipodePago").val(data.Cve_TipodePago);
                    $("#TipodePago").val(data.TipodePago);
                    $("#TipodePagoDescripcion").val(data.TipodePagoDescripcion);

                    $("#Cve_TipodePago").prop("disabled", true);

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
                    Cve_TipodePago                  : Cve_TipodePago,
                    Cve_TipoFactura                 : $("#Cve_TipoFactura").val(),
                    TipodePago                      : $("#TipodePago").val(),
                    TipodePagoDescripcion           : $("#TipodePagoDescripcion").val(),
                }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/TiposPago/Save/",
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
                $('#modal-delete-tipospago').modal("show");
                return;
            }

            $('#modal-delete-tipospago').modal("hide");

            var model = { 
                Cve_TipodePago: Cve_TipodePago
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/TiposPago/Delete/",
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
                Cve_TipoFactura                     : $("#Cve_TipoFactura").val(),
                Cve_TipodePago                      : $("#Cve_TipodePago").val(),
                TipodePago                          : $("#TipodePago").val(),
                TipodePagoDescripcion               : $("#TipodePagoDescripcion").val(),
                
            }

            
            if (!formValidation.Validate())
                return;

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/TiposPago/Add/",
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
    var order = "CVE_TIPODEPAGO";
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

            window.location.href = '/TiposPago/ExportExcel';

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
                url: "/TiposPago/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();


