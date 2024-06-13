$(function () {

    $(window).load(function () {
               
        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        formValidation.Inputs(["clave", "descripcion","cuenta", "cuentaiva", "cuentaretencioniva", "cuentaretencionisr"]);
        formValidation.notEmpty('clave', 'El campo Clave no debe de estar vacio');
        formValidation.notEmpty('descripcion', 'El campo Descripción no debe estar vacio');
        formValidation.notEmpty('cuenta', 'El campo Cuenta contable no debe de estar vacio');
        formValidation.notEmpty('cuentaiva', 'El campo Cuenta IVA no debe estar vacio');
        formValidation.notEmpty('cuentaretencioniva', 'El campo Cuenta retención IVA no debe de estar vacio');
        formValidation.notEmpty('cuentaretencionisr', 'El campo Cuenta retención ISR no debe estar vacio');

        DataTable.init();

        formPage.getProgramas();
        formPage.getTiposdePago();
    });
});//End function jquery

var formPage = function () {

    var Id;
    var controller = "CentroCostos";

    "use strict"; return {

        clean: function () {
            formValidation.Clean();
            $("#cuenta").val("");
            $("#cuentaiva").val("");
            $("#cuentaretencioniva").val("");
            $("#cuentaretencionisr").val("");
            $("#activa").prop("checked", false);

            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").prop("disabled", true);
            $("#formbtndelete").hide();

            $("#clave").prop("disabled", false);
        },

        edit: function (id) {

            Id = id;

            var model = {
                Id: Id
            }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/" + controller + "/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#id").val(data.Id);
                    $("#clave").val(data.Clave);
                    $("#descripcion").val(data.Descripcion);
                    $("#sedes").val(data.Sede);
                    $("#factura").val(data.TipoFactura);
                    formPage.setTiposdePago(data.TipoPago);
                    $("#cuenta").val(data.Cuenta);
                    $("#cuentaiva").val(data.CuentaIVA);
                    $("#cuentaretencioniva").val(data.CuentaRETIVA);
                    $("#cuentaretencionisr").val(data.CuentaRETISR);
                    $("#escuela").val(data.Escuela);
                    formPage.setProgramas(data.Programa);

                    (data.Activa == 1) ? $("#activa").prop("checked", true) : $("#activa").prop("checked", false);

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
                Id: $("#id").val(),
                Clave: $("#clave").val(),
                Descripcion: $("#descripcion").val(),
                Sede: $('#Sedes').val(),
                TipoPago: $("#tipopago").val(),
                Cuenta: $("#cuenta").val(),
                CuentaIVA: $("#cuentaiva").val(),
                CuentaRETIVA: $("#cuentaretencioniva").val(),
                CuentaRETISR: $("#cuentaretencionisr").val(),
                Escuela: $("#escuela").val(),
                Programa: $("#programa").val(),
                Activa: ($("#activa").attr('checked')) ? 1 : 0,
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/" + controller + "/Save/",
                data: model,
                success: function (data) {
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        delete: function (confirm) {

            if (!confirm) {
                $('#modal-delete-centrocostos').modal("show");
                return;
            }

            $('#modal-delete-centrocostos').modal("hide");

            var model = {
                Id: Id
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/" + controller + "/Delete/",
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
                Descripcion: $("#descripcion").val(),
                Sede: $('#Sedes').val(),
                TipoPago: $("#tipopago").val(),
                Cuenta: $("#cuenta").val(),
                CuentaIVA: $("#cuentaiva").val(),
                CuentaRETIVA: $("#cuentaretencioniva").val(),
                CuentaRETISR: $("#cuentaretencionisr").val(),
                Escuela: $("#escuela").val(),
                Programa: $("#programa").val(),
                Activa: ($("#activa").attr('checked')) ? 1 : 0,
            }

            if (!formValidation.Validate())
                return;

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/" + controller + "/Add/",
                data: model,
                success: function (data) {

                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        getProgramas: function () {

            cveEscuela = $("#escuela").val();

            $.ajax({
                type: "GET",
                cache: false,
                url: "/" + controller + "/getProgramas/",
                data: "cveEscuela="+cveEscuela,
                success: function (data) {
                    $("#programa").html(data);
                }
            });
        },
        setProgramas: function (id) {

            cveEscuela = $("#escuela").val();

            $.ajax({
                type: "GET",
                cache: false,
                url: "/" + controller + "/getProgramas/",
                data: "cveEscuela="+cveEscuela,
                success: function (data) {
                    $("#programa").html(data);
                    $("#programa").val(id);
                }
            });
        },

        getTiposdePago: function () {

            cveFactura = $("#factura").val();

            $.ajax({
                type: "GET",
                cache: false,
                url: "/" + controller + "/getTiposdePago/",
                data: "cveFactura=" + cveFactura,
                success: function (data) {
                    $("#tipopago").html(data);
                }
            });
        },

        setTiposdePago: function (id) {

            cveFactura = $("#factura").val();

            $.ajax({
                type: "GET",
                cache: false,
                url: "/" + controller + "/getTiposdePago/",
                data: "cveFactura=" + cveFactura,
                success: function (data) {
                    $("#tipopago").html(data);
                    $("#tipopago").val(id);
                }
            });
        }
    }
}();

var DataTable = function () {
    var controller = "CentroCostos";
    var pag = 1;
    var order = "CVE_CENTRODECOSTOS";
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
            var datos = 'Sedes=' + $('#Sedes').val();
            window.location.href = '/' + controller + '/ExportExcel?' + datos;
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
                url: "/" + controller + "/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter +  "&filter=" + filter + "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();