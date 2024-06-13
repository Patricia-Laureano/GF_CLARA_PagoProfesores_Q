

var obj_anios = new Ciclos('anios');

var obj_anios_A = new Ciclos('anios_A');

$(function () {


    var dateToday = new Date();

    $("#fecha_publicacion").datepicker({
        changeMonth: true,
        changeYear: true,
       // minDate: dateToday,
        dateFormat: 'yy-mm-dd'
    });


    $("#fecha_publicacion_A").datepicker({
        changeMonth: true,
        changeYear: true,
        // minDate: dateToday,
        dateFormat: 'yy-mm-dd'
    });




    $(window).load(function () {


        obj_anios.init("obj_anios");

        obj_anios_A.init("obj_anios_A");        


        $("#formbtnadd").html('Guardar');

        $("#formbtnadd2").html('Guardar');
      //  $("#formbtnsave").hide();
      //  $("#formbtndelete").hide();

        formValidation.Inputs(["anios", "Mes", "fecha_publicacion"]);

        formValidation.notEmpty('Año', 'El campo Año no debe estar vacio');
        formValidation.notEmpty('Mes', 'El campo Mes por hora no debe estar vacio');
        //formValidation.notEmpty('fecha_publicacion', 'El campo la Fecha de publicacion por hora no debe estar vacio');

       
        
    

    });
});//End function jquery



function handlerdataCiclos() {

      
}



/*var Sedes = function () {

    "use strict"; return {

        setSedes: function () {

            formPage.edit();
            formPage.edit_A();

       
        }
    }
}();*/


Sedes.setSedes_success = function () {

    formPage.edit();
    formPage.edit_A();
 

}




$('#anios_A').on('change', function () {
 
   formPage.edit_A();

});


$('#anios_A').on('change', function () {

    formPage.edit_A();    
   
});


/********retencion mensual*****/

$('#anios').on('change', function () {

    formPage.edit();

});



function filter() {
   
    formPage.edit();

}




var formPage = function () {
    var idPConsMens;

    "use strict"; return {

        clean: function () {

            formValidation.Clean();

        //    $("#formbtnadd").prop("disabled", false);

          /*  formValidation.Clean();
            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").prop("disabled", true);
            $("#formbtndelete").hide();
            $("#claveNivel").prop("disabled", false);*/

        },



        edit: function () {

          //  idPConsMens = id;         


            var model = {

                Anio: $("#anios").val(),
                Mes: $("#Mes").val(),           
                sede: $('#Sedes').val(),

            }

           // this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/PublicacionConstancias/Edit",
                data: model,
                success: function (data) {

                   data = jQuery.parseJSON(data);

                    $('html, body').animate({ scrollTop: 0 }, 'fast');            

                    $("#fecha_publicacion").val(data.Strm_Fecha_Publicacion);
                    $("#idPConsMensual").val(data.idPConsMens);

                },error: function () {  },

            });

        },

        save: function () {

            var model = {
                Anio: $("#anios").val(),
                Mes: $("#Mes").val(),
                Fecha_Publicacion: $("#fecha_publicacion").val(),
                sede: $('#Sedes').val(),

            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/PublicacionConstancias/Save/",
                data: model,
                success: function (data) {
                  //  formPage.clean();
                    $('#notification').html(data.msg);
                 
                }

            });

        },

   
        add: function () {

            var model = {

                Anio: $("#anios").val(),
                Mes: $("#Mes").val(),
                Fecha_Publicacion: $("#fecha_publicacion").val(),
                sede: $('#Sedes').val(),
            }
          if (!formValidation.Validate())
                return;

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/PublicacionConstancias/Add/",
                data: model,
                success: function (data) {
                   
                    $('#notification').html(data.msg);
                  

                }

            });
        },

/*******************************************************************Retenciones Anuales ***********************************************************/
        

          validateRetencion_Anual: function () {
               var validado = true;

               if ($("#anios_A").val() == "" || $("#anios_A").val() == null) { $("#anios_A").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
               else { $("#anios_A").removeClass("form-control parsley-error").addClass("form-control"); }

               if ($("#fecha_publicacion_A").val() == "" || $("#fecha_publicacion_A").val() == null) { $("#fecha_publicacion_A").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
               else { $("#fecha_publicacion_A").removeClass("form-control parsley-error").addClass("form-control"); }             

               return validado;
        },



        
        edit_A: function () {

            //  idPConsMens = id;   

         

        var model = {

            Anio: $("#anios_A").val(),                  
            sede: $('#Sedes').val(),

        }

            // this.clean();

        $.ajax({
            type: "POST",
            dataType: 'json',
            cache: false,
            url: "/PublicacionConstancias/Edit_A",
            data: model,
            success: function (data) {

                data = jQuery.parseJSON(data);

                $('html, body').animate({ scrollTop: 0 }, 'fast');            

                $("#fecha_publicacion_A").val(data.Strm_Fecha_Publicacion);
              //  $("#idPConsMensual").val(data.idPConsMens);

            },error: function () {  },

        });

    },

 

   
    add_A: function () {


        //alert('entro');

        var model = {

            Anio: $("#anios_A").val(),         
            Fecha_Publicacion: $("#fecha_publicacion_A").val(),
            sede: $('#Sedes').val(),
        }


        if (!formPage.validateRetencion_Anual())
            return;


        $.ajax({
            type: "POST",
            dataType: 'json',
            cache: false,
            url: "/PublicacionConstancias/Add_A/",
            data: model,
            success: function (data) {
                formPage.clean();
                $('#notification').html(data.msg);
                  

            }

        });
    }






    }


}();






