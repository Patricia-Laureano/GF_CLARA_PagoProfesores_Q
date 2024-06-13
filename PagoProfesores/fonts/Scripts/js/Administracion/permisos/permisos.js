
$(function () {
  
    $(window).load(function () {


        DataTable.setPage(1);

        formValidation.Inputs(["permiso", "descripcion"]);
        formValidation.notEmpty('permiso', 'El campo permiso Actual no debe de estar vacio');
        formValidation.notEmpty('descripcion', 'El campo descripción no debe estar vacio');

       
    });
});//End function jquery

//valida: solo numeros
$("#clave").live("keypress", function (e) {
    var tecla = document.all ? tecla = e.keyCode : tecla = e.which;
    if (tecla == 13) {
        var search = $("#clave").val();
        formPage.edit(search);
    }
    else
        return ((tecla > 47 && tecla < 58) || tecla == 8 || tecla == 13 || tecla == 110 || tecla == 189 || tecla == 190 || tecla == 0);
});



function gup(name) {

    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var tmpURL = window.location.href;
    var results = regex.exec(tmpURL);
    if (results == null)
        return "";
    else
        return results[1];
}





var formPage = function () {
    var idPermiso;

    "use strict"; return {

        clean: function () {
            // $("#clave").prop("disabled", false);
            // $("#clave").val("");
            formValidation.Clean();
            var permiso = $("#permiso").val("");
            var description = $("#descripcion").val("");
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").prop("disabled", true);

       //     $("#permiso").removeClass("form-control parsley-error").addClass("form-control");
         //   $("#descripcion").removeClass("form-control parsley-error").addClass("form-control");

        },


        permitirPermisos : function (id) {
        
            var generar = false;

            var listapermisos = new Array()
            var cont = 0;

            var permisos = "";
            var rol = gup('idrole');


            if (document.forms.namedItem("form" + id).elements.namedItem("checkboxall" + id).checked == 1) {


                for (i = 0; i < document.forms.namedItem("form" + id).elements.length; i++) {
                    if (document.forms.namedItem("form" + id).elements[i].type == "checkbox") {
                        if (document.forms.namedItem("form" + id).elements[i].checked == 1) {
                            listapermisos[cont] = document.forms.namedItem("form" + id).elements[i].value;
                            cont += 1;
                            // alert(document.form.elements[i].value);
                            generar = true;
                        }
                    }
                }
            }
            else if (document.forms.namedItem("form" + id).elements.namedItem("checkboxall" + id).checked == 0) {



                listapermisos[cont] = document.forms.namedItem("form" + id).elements.namedItem("checkboxall" + id).value;

                for (i = 0; i < document.forms.namedItem("form" + id).elements.length; i++) {

                    if (document.forms.namedItem("form" + id).elements[i].type == "checkbox") {

                        if (document.forms.namedItem("form" + id).elements[i].checked == 1) {
                            cont += 1;

                            listapermisos[cont] = document.forms.namedItem("form" + id).elements[i].value;
                            generar = true;

                        }
                    }
                }

            }


            for (j = 1; j <= listapermisos.length - 1; j++) {
                //alert(document.getElementById('labelNRC'+listacursos[j]));
                if (j == 1) {
                    permisos += listapermisos[j];
                    permisos += "-";
                    //alert(id);
                }
                else {
                    permisos += listapermisos[j];
                    permisos += "-";
                    //alert(idcurso);
                }
            }

            if (generar == true) {


                var model = {
                    idrole: rol,
                    permisos: permisos
                }


                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    cache: false,
                    url: "/Permisos/permitirPermiso/",
                    data: model,
                    success: function (data) {

                        //  $('#content').prepend(data.msg);
                        formPage.clean();
                        DataTable.init();

                        //  BuscarPermisos();

                        $('html,body').animate({ scrollTop: 0 }, 10);
                        $('#notification').html(formValidation.getMessage('Se han permitido los permisos al rol con exito'));
                       // $.gritter.add({ title: "PERMISOS ACTIVADOS", text: "Se han permitido los permisos al rol con exito" });

                    }

                });

            } else {

                $('#titlemodal').html('¡Alerta!');
                $('#bodymodal').html('Debe elegir al menos un permiso a modificar');
                $("#cancelarmodal").hide();
                $("#aceptarmodal").attr("data-dismiss", "modal");
                $('#myModal').modal('show');

            }       
        
        
        },

        
        restringirPermisos: function (id) {
            
            var generar = false;

            var listapermisos = new Array()
            var cont = 0;

            var permisos = "";
            var rol = gup('idrole');

            if (document.forms.namedItem("form" + id).elements.namedItem("checkboxall" + id).checked == 1) {
                for (i = 0; i < document.forms.namedItem("form" + id).elements.length; i++) {
                    if (document.forms.namedItem("form" + id).elements[i].type == "checkbox") {
                        if (document.forms.namedItem("form" + id).elements[i].checked == 1) {
                            listapermisos[cont] = document.forms.namedItem("form" + id).elements[i].value;
                            cont += 1;
                            // alert(document.form.elements[i].value);
                            generar = true;
                        }
                    }
                }
            }
            else if (document.forms.namedItem("form" + id).elements.namedItem("checkboxall" + id).checked == 0) {
                listapermisos[cont] = document.forms.namedItem("form" + id).elements.namedItem("checkboxall" + id).value;
                for (i = 0; i < document.forms.namedItem("form" + id).elements.length; i++) {
                    if (document.forms.namedItem("form" + id).elements[i].type == "checkbox") {
                        if (document.forms.namedItem("form" + id).elements[i].checked == 1) {
                            cont += 1;

                            listapermisos[cont] = document.forms.namedItem("form" + id).elements[i].value;
                            //alert(document.form.elements[i].value);
                            generar = true;

                        }
                    }
                }

            }


            for (j = 1; j <= listapermisos.length - 1; j++) {
                //alert(document.getElementById('labelNRC'+listacursos[j]));
                if (j == 1) {
                    permisos += listapermisos[j];
                    permisos += "-";
                    //alert(id);
                }
                else {
                    permisos += listapermisos[j];
                    permisos += "-";
                    //alert(idcurso);
                }
            }

            if (generar == true) {



                var model = {
                    idrole: rol,
                    permisos: permisos
                }


                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    cache: false,
                    url: "/Permisos/restringirPermiso/",
                    data: model,
                    success: function (data) {

                        //  $('#content').prepend(data.msg);
                        formPage.clean();
                        DataTable.init();

                        //  BuscarPermisos();
                        $('html,body').animate({ scrollTop: 0 }, 10);
                        $('#notification').html(formValidation.getMessage('Se han restringido los permisos al rol con exito'));

                       // $.gritter.add({ title: "PERMISOS RESTRINGIDOS", text: "Se han restringido los permisos al rol con exito" });

                    }

                });

            } else {
                $('#titlemodal').html('¡Alerta!');
                $('#bodymodal').html('Debe elegir al menos un permiso a modificar');
                $("#cancelarmodal").hide();
                $("#aceptarmodal").attr("data-dismiss", "modal");
                $('#myModal').modal('show');
            }

            
         },


        edit: function (id) {

            idPermiso = id;


            $("#permiso").removeClass("form-control parsley-error").addClass("form-control");
            $("#descripcion").removeClass("form-control parsley-error").addClass("form-control");

            var model = {
                idPermiso: id
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Permisos/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');
                    if (data.Permiso != "") { $("#clave").prop("disabled", true); }
                    $("#clave").val(data.idPermiso);
                    $("#permiso").val(data.Permiso);
                    $("#descripcion").val(data.Description);

                    $("#formbtnadd").prop("disabled", true);
                    $("#formbtnsave").prop("disabled", false);
                    $("#formbtndelete").prop("disabled", false);
                }

            });


        },

        save: function () {//edita los campos

            if (!formValidation.Validate())
                return;


                var model = {
                    idPermiso: idPermiso,
                    Permiso: $("#permiso").val(),
                    Description: $("#descripcion").val(),
                }

                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    cache: false,
                    url: "/Permisos/Save/",
                    data: model,
                    success: function (data) {

                     //   alert(data.msg);
                        formPage.clean();
                    
                        $('#notification').html(data.msg);
                     //   $('#content').prepend(data.msg);
                
                        DataTable.init();
                    }

                });

           
        },

        delete: function () {


           
                var confirmar = confirm("Estas seguro de borrar el Permiso?");
                if (confirmar == true) {

                    var model = {
                        idPermiso: idPermiso
                    }

                    $.ajax({
                        type: "POST",
                        dataType: 'json',
                        cache: false,
                        url: "/Permisos/Delete/",
                        data: model,
                        success: function (data) {
                            //  $('#content').prepend(data.msg);
                            formPage.clean();
                            $('#notification').html(data.msg);
                       
                            DataTable.init();
                        }

                    });
                }

            

        },

        add: function () {

            if (!formValidation.Validate())
                return;


                var model = {
                    Permiso: $("#permiso").val(),
                    Description: $("#descripcion").val(),
                }

               
                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    cache: false,
                    url: "/Permisos/Add/",
                    data: model,
                    success: function (data) {

                        formPage.clean();

                          $('#notification').html(data.msg);

                     //   $('#content').prepend(data.msg);
                      
                    
                        DataTable.init();

                    }

                });
            

        },

        validateInputs: function () {

            var validado = true;

            if ($("#permiso").val() == "") { $("#permiso").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#permiso").removeClass("form-control parsley-error").addClass("form-control"); }
            if ($("#descripcion").val() == "") { $("#descripcion").removeClass("form-control").addClass("form-control parsley-error"); validado = false; }
            else { $("#descripcion").removeClass("form-control parsley-error").addClass("form-control"); }

            return validado;
        }
    }

}();


var DataTable = function () {
    var pag = 1;
    var order = "PERMISO";
    var sortoption = {
        ASC: "ASC",
        DESC: "DESC"
    };
    var sort = sortoption.ASC;

    "use strict"; return {
        myName: 'DataTable',
        checkboxs: [],
        onkeyup_colfield_check: function (e) {
            var enterKey = 13;
            if (e.which == enterKey) {
                pag = 1;
                this.init();
            }
        },

        exportExcel: function (table) {

            window.location.href = '/Permisos/ExportExcel';

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
         

            /*$('div.block').block({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src="/Content/images/load-search.gif" /><br><h2> Buscando..</h2>'
            });*/



            $.ajax({
                type: "GET",
                cache: false,
                url: "/Permisos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {

                    //$('div.block').unblock();
                    $("#datatable").html(msg);

                }

            });
        }

    }


}();



/*function BuscarPermisos() {


    //  var show =	$("#data-elements-length").val();
    //   var search =	$("#searchtable").val();

    var idrole = gup('idrole');


    $.ajax({
        type: "GET",
        cache: false,
        url: "Permisos?view=BuscarPermisos&Rol=" + idrole,
        data: "?view=BuscarPermisos&Rol" + idrole,
        success: function (msg) {


            var contenido = msg.split('#%#');


            $('div.block').unblock();
            $("#listresultadosdashboard").html(contenido[0]);
            $("#listresultadossorteos").html(contenido[1]);
            $("#listresultadospoblacion").html(contenido[2]);
            $("#listresultadostalonarios").html(contenido[3]);
            $("#listresultadosventas").html(contenido[4]);
            $("#listresultadospremios").html(contenido[5]);
            $("#listresultadosreportes").html(contenido[6]);
            $("#listresultadosusuarios").html(contenido[7]);
            $("#listresultadosherramientas").html(contenido[8]);



        }

    });


}*/



