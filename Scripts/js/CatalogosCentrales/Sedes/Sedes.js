$(function () {

    $(window).load(function () {
        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        formValidation.Inputs(["Cve_Sede", "Sede", "Cve_Sociedad"]);

        formValidation.notEmpty('Cve_Sede', 'El campo Clave no debe de estar vacio');
        formValidation.notEmpty('Sede', 'El campo Sede no debe de estar vacio');
        //formValidation.notEmpty('Campus_Inb', 'El campo Campus INB no debe de estar vacio');
        //formValidation.notEmpty('TipoContrato_Inb', 'El campo Tipo contrato INB no debe de estar vacio');
        formValidation.notEmpty('Cve_Sociedad', 'El campo Sociedad no debe de estar vacio');

    });
});//End function jquery

var formPage = function () {
    var Cve_Sede;

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

            $("#Cve_Sede").prop("disabled", false);
        },

        edit: function (id)
        {
            Cve_Sede = id;
            var model =
                {
                    Cve_Sede: id
                }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sedes/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#Cve_Sociedad").val(data.Cve_Sociedad);
                    $("#Cve_Sede").val(data.Cve_Sede);
                    $("#Sede").val(data.Sede);
                    $("#Campus_Inb").val(data.Campus_Inb);
                    $("#TipoContrato_Inb").val(data.TipoContrato_Inb);

                    $("#Cve_Sede").prop("disabled", true);

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
                    Cve_Sede                        : Cve_Sede,
                    Cve_Sociedad                    : $("#Cve_Sociedad").val(),
                    Sede                            : $("#Sede").val(),
                    Campus_Inb                      : $("#Campus_Inb").val(),
                    TipoContrato_Inb                : $("#TipoContrato_Inb").val(),
                }

            if (!formValidation.Validate())
                return;

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sedes/Save/",
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
                $('#modal-delete-sede').modal("show");
                return;
            }

            $('#modal-delete-sede').modal("hide");

            var model = { 
                Cve_Sede: Cve_Sede
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sedes/Delete/",
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
                Cve_Sociedad                        : $("#Cve_Sociedad").val(),
                Cve_Sede                            : $("#Cve_Sede").val(),
                Sede                                : $("#Sede").val(),
                Campus_Inb                          : $("#Campus_Inb").val(),
                TipoContrato_Inb                    : $("#TipoContrato_Inb").val(),
            }

            
            if (!formValidation.Validate())
                return;

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sedes/Add/",
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
    var order = "Cve_Sede";
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

            window.location.href = '/Sedes/ExportExcel';

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
                url: "/Sedes/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();