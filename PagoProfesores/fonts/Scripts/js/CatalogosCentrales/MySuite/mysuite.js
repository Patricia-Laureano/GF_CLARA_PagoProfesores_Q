$(function () {
    $(window).load(function () {
        
        formPage.edit();
       
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


            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/MySuite/Edit/",
               // data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#requestor").val(data.requestor),
                    $("#transaction").val(data.transaction),
                    $("#country").val(data.country),
                    $("#entity").val(data.entity),
                    $("#user").val(data.user),
                    $("#username").val(data.username),
                    $("#data1").val(data.data1),
                    $("#data2").val(data.data2),
                    $("#data3").val(data.data3),
                    $("#mensaje").val(data.mensaje),
                    $("#file1").val(data.file1),
                    $("#file2").val(data.file2),
                    $("#success").val(data.success),
                    $("#archivo").val(data.archivo),
                    $("#uuid").val(data.uuid)

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
                data1: $("#data1").val(),
                data2: $("#data2").val(),
                data3: $("#data3").val(),
                mensaje: $("#mensaje").val(),
                file1: $("#file1").val(),
                file2: $("#file2").val(),
                success: $("#success").val(),
                archivo: $("#archivo").val(),
                uuid: $("#uuid").val()
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
                data1: $("#data1").val(),
                data2: $("#data2").val(),
                data3: $("#data3").val(),
                mensaje: $("#mensaje").val(),
                file1: $("#file1").val(),
                file2: $("#file2").val(),
                success: $("#success").val(),
                archivo: $("#archivo").val(),
                uuid: $("#uuid").val()
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
