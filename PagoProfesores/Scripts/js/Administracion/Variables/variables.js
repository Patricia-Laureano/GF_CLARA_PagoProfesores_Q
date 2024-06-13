$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();
        formValidation.Inputs(["variable", "valor"]);
        formValidation.notEmpty('variable', 'El campo variable no debe de estar vacio');
      //  formValidation.onlyNumbers('valor');
        formValidation.notEmpty('valor', 'El campo valor no debe de estar vacio');
       

    });
});//End function jquery

var formPage = function () {
    var Variable;

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


            $("#variable").val('');
            $("#valor").val('');
            $("#descripcion").val('');


          //  $("#variable").prop("disabled", false);
        },

        edit: function (id)
        {
            Variable = id;
            var model =
                {
                    Variable: id
                }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Variables/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#variable").val(data.Variable);
                    $("#valor").val(data.Valor);
                    $("#descripcion").val(data.Descripcion);
                  

                    $("#variable").prop("disabled", true);
                    
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
                    Variable: Variable,
                    Valor          : $("#valor").val(),
                    Descripcion    : $("#descripcion").val(),
                 //   CuotaFija               : $("#CuotaFija").val(),
                 

                }


      

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Variables/Save/",
                data: model,
                success: function (data) {
                    
                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }
            });
        },

        delete: function (confirm) {

            if (!confirm) {
                $('#modal-delete-variables').modal("show");
                return;
            }

            $('#modal-delete-variables').modal("hide");

            var model = { 
                Variable: Variable
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Variables/Delete/",
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
    var order = "VARIABLE";
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

            window.location.href = '/Variables/ExportExcel';

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
                url: "/Variables/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();


