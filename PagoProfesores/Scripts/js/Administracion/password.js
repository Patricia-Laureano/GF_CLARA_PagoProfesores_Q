
$(function () {

    $(window).load(function () {

        formValidation.Inputs(["password-actual", "password-indicator-visible", "password-indicator-default"]);
        formValidation.notEmpty('password-actual', 'El campo Password Actual no debe de estar vacio');      
        formValidation.notEmpty('password-indicator-visible', 'El campo Password Nuevo no debe estar vacio');
        formValidation.notEmpty('password-indicator-default', 'El campo Repetir Password no debe estar vacio');

    });
});//End function jquery


function saveNewPassword(usuario, passwordactual ){

    var password = $("#password-indicator-visible").val();


    var model = {
        passwordActual: passwordactual,
        password: password,
        usuario: usuario,
    }           
  

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Password/EditaPassword/",
                data: model,
                success: function (data) {     

              if (data.msg == "Se ha editado con exito el Password") {  }
            
            $("#form-password").hide();
            $("#wizard").show();

            //Logout            
            setTimeout("window.location.href = 'Login/Close'", 2000);

        }
    });                     


}


var formPage = function () {
   

    "use strict"; return {

        clean: function () {
            formValidation.Clean();
            $('#notification').html("");
            $("#password-actual").val("");
            $("#descripcion").val("");
            $("#password-indicator-visible").val("");
            $("#password-indicator-default").val("");
            $("#password-actual").css("border", "#CCD0D4 solid 2px");
            $("#password-indicator-visible").css("border", "#CCD0D4 solid 2px");
            $("#password-indicator-default").css("border", "#CCD0D4 solid 2px");       

        },     


        ChangePassword: function () {

            if (!formValidation.Validate())
                return;
         
                this.validarPasswordAnterior();             
        },


        validarPasswordAnterior: function () {
          
            var validado = false;

            var passwordactual = $('#password-actual').val();
            var usuario = $("#usuario").val();

           
            var model = {
                passwordActual: passwordactual,
                usuario: usuario,
            }     
               
  
                    $.ajax({
                        type: "POST",
                        dataType: 'json',
                        cache: false,
                        url: "/Password/ValidaPassword/",
                        data: model,
                        success: function (data) {                            

                    if (data.msg.trim() == "Existe") {                    
                      
                        validado = true;

                        if ($("#password-indicator-visible").val() == $("#password-indicator-default").val()) {
                            saveNewPassword(usuario, passwordactual);
                        } else {

                            $("#password-indicator-default").css("border", "red solid 2px");
                            $('html,body').animate({ scrollTop: 0 },10);
                            $('#notification').html(formValidation.getMessage('El Password Nuevo no coincide con el campo Repetir Password'));
                                                    }

                    } else {
                        validado = false;
                        $("#password-actual").css("border", "red solid 2px");
                        $('html,body').animate({ scrollTop: 0 }, 10);                      
                        $('#notification').html(formValidation.getMessage('El usuario y/o Password actual son incorrectos'));

                    }

                }

            });

            return validado;            

        },            


        validateInputs: function () {      
            var validado = true;         
            if ($("#password-actual").val() == "") { $("#password-actual").css("border", "red solid 2px"); validado = false; } else { $("#password-actual").css("border", "#CCD0D4 solid 2px"); } 
            if ($("#password-indicator-visible").val() == "") { $("#password-indicator-visible").css("border", "red solid 2px"); validado = false; } else { $("#password-indicator-visible").css("border", "#CCD0D4 solid 2px"); }
            if ($("#password-indicator-default").val() == "") { $("#password-indicator-default").css("border", "red solid 2px"); validado = false; } else { $("#password-indicator-default").css("border", "#CCD0D4 solid 2px"); }
            return validado;         
        }
    }

}();
