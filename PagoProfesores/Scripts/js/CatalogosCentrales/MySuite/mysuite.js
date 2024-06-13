$(function () {
    $(window).load(function () {
        
       // formPage.edit();
       
    });

});




formPage = function () {
    var idSociedad;
    var colonias = [];

    "use strict"; return {

        clean: function () {    //limpia los campos y los deja listos para insertar uno nuevo
           

          /*  $("#requestor").val("");
            $("#transaction").val("");
            $("#country").val("");
            $("#entity").val("");
            $("#user").val("");
            $("#username").val("");
            $("#data1").val("");
            $("#data2").val("");
            $("#data3").val("");
            $("#mensaje").val("");
            $("#file1").val("");
            $("#file2").val("");
            $("#success").val("");
            $("#archivo").val("");
            $("#uuid").val("");*/


         //   $("#formbtnadd").show();
          //  $("#formbtnadd").prop("disabled", false);
          //  $("#formbtnsave").hide();
          //$("#formbtnsave").prop("disabled", true);
           // $("#formbtndelete").prop("disabled", true);
          //  $("#formbtndelete").hide();
        },

        edit: function (id) {
            var model = {
                campusCode:id
            }
            $("#formbtnadd").prop("disabled", true);
            $("#formbtnsave").prop("disabled", false);
            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/MySuite/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#requestor").val(data.requestor),
                    $("#transaction").val(data.transaction),
                    $("#country").val(data.country),
                    $("#entity").val(data.entity),
                    $("#user").val(data.user),
                    $("#username").val(data.username),
                    $("#campusCode").val(data.campusCode),
                    $("#sucursal").val(data.sucursal)

                }

            });
          

        },

        save: function () {


            var model = {
                requestor: $("#requestor").val(),
                transaction: $("#transaction").val(),
                country: $("#country").val(),
                entity: $("#entity").val(),
                user: $("#user").val(),
                username: $("#username").val(),
                campusCode: $("#campusCode").val(),
                sucursal: $("#sucursal").val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/MySuite/Save/",
                data: model,
                success: function (data) {

                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }

            });

        },

        delete: function (confirm) {

           

        },

        add: function () {

                      

            var model = {
                requestor: $("#requestor").val(),
                transaction: $("#transaction").val(),
                country: $("#country").val(),
                entity: $("#entity").val(),
                user: $("#user").val(),
                username: $("#username").val(),
                campusCode: $("#campusCode").val(),
                sucursal: $("#sucursal").val()
            }



            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/MySuite/Add/",
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

DataTable = function () {
    var pag = 1;
    var order = "CAMP_CODE";
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
            window.location.href = '/MySuite/ExportExcel';
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
                url: "/MySuite/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {
                    $("#datatable").html(msg);
                }
            });
        }
    }
}();
