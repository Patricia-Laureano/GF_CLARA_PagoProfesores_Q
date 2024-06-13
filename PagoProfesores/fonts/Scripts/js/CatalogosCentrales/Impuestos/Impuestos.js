$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();
        formValidation.Inputs(["Rango", "LimiteInferior", "LimiteSuperior", "CuotaFija", "PorcentajeExcedente"]);
        formValidation.notEmpty('Rango', 'El campo Rango no debe de estar vacio');
        formValidation.onlyNumbers('Rango');
        formValidation.notEmpty('LimiteInferior', 'El campo Límite inferior no debe de estar vacio');
        formValidation.onlyNumbers('LimiteInferior');
        formValidation.notEmpty('LimiteSuperior', 'El campo Límite superior no debe de estar vacio');
        formValidation.onlyNumbers('LimiteSuperior');
        formValidation.notEmpty('CuotaFija', 'El campo Cuota fija no debe de estar vacio');
        formValidation.onlyNumbers('CuotaFija');
        formValidation.notEmpty('PorcentajeExcedente', 'El campo Porcentaje exedente no debe de estar vacio');
        formValidation.onlyNumbers('PorcentajeExcedente');

    });
});//End function jquery

var formPage = function () {
    var Rango;

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

            $("#Rango").prop("disabled", false);
        },

        edit: function (id)
        {
            Rango = id;
            var model =
                {
                    Rango: id
                }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Impuestos/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#Rango").val(data.Rango);
                    $("#LimiteInferior").val(data.LimiteInferior);
                    $("#LimiteSuperior").val(data.LimiteSuperior);
                    $("#CuotaFija").val(data.CuotaFija);
                    $("#PorcentajeExcedente").val(data.PorcentajeExcedente);

                    $("#Rango").prop("disabled", true);
                    
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
                    Rango                   : Rango,
                    LimiteInferior          : $("#LimiteInferior").val(),
                    LimiteSuperior          : $("#LimiteSuperior").val(),
                    CuotaFija               : $("#CuotaFija").val(),
                    PorcentajeExcedente     : $("#PorcentajeExcedente").val(),

                }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Impuestos/Save/",
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
                $('#modal-delete-impuestos').modal("show");
                return;
            }

            $('#modal-delete-impuestos').modal("hide");

            var model = { 
                Rango: Rango
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Impuestos/Delete/",
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
                Rango                       : $("#Rango").val(),
                LimiteInferior              : $("#LimiteInferior").val(),
                LimiteSuperior              : $("#LimiteSuperior").val(),
                CuotaFija                   : $("#CuotaFija").val(),
                PorcentajeExcedente         : $("#PorcentajeExcedente").val(),
            }

            
            if (!formValidation.Validate())
                return;

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Impuestos/Add/",
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
    var order = "Rango";
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

            window.location.href = '/Impuestos/ExportExcel';

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

            $.ajax({
                type: "GET",
                cache: false,
                url: "/Impuestos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();


