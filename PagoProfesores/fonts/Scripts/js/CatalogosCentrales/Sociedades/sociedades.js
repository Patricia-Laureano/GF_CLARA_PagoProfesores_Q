var af = '';
var afi = '';
var as = '';
var asi = '';

var direccion1 = new Direccion('Direccion_CP', 'Direccion_Pais', 'Direccion_Estado', 'Direccion_Ciudad', 'Direccion_Entidad', 'Direccion_Colonia');
var direccion2 = new Direccion('RL_Direccion_CP', 'RL_Direccion_Pais', 'RL_Direccion_Estado', 'RL_Direccion_Ciudad', 'RL_Direccion_Entidad', 'RL_Direccion_Colonia');

$(function () {
    $(window).load(function () {
        

       // $('#MyTabSelector').disableTab(1);

        $("#img_firma").hide();
        $("#img_sello").hide();


        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        formValidation.Inputs(["Cve_Sociedad", "Sociedad", "RFC", "CURP"]);
        formValidation.notEmpty('Cve_Sociedad', 'El campo Clave no debe de estar vacio');
        formValidation.notEmpty('Sociedad', 'El campo Clave no debe de estar vacio');
        formValidation.notEmpty('RFC', 'El campo Clave no debe de estar vacio');
        formValidation.notEmpty('CURP', 'El campo Clave no debe de estar vacio');

        direccion1.init("direccion1");
        direccion2.init("direccion2");
       
    });

    $('#firma_img').fileupload({
        url: 'FileUploadHandler.ashx?upload=start',
        add: function (e, data) {
            var val = data.files[0].name.toLowerCase();
            var regex = new RegExp("(.*?)\.(jpg|jpeg)$");
            if (!(regex.test(val))) {
                $(this).val('');
                alert('Sólo archivos JPG');
            }
            else data.submit();
            $('#progressbar').show();
        },
        progress: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progressbar div').css('width', progress + '%');
        },
        success: function (response, status) {
            $('#progressbar').hide();
            $('#progressbar div').css('width', '100%');
            afi = response;
            $("#img_firma").attr('src', '/Upload/' + response);
            $("#img_firma").show();
        },
        error: function (error) {
            $('#progressbar').hide();
            $('#progressbar div').css('width', '0%');
            console.log('error', error);
        }
    });


    $('#sello_img').fileupload({
        url: 'FileUploadHandler.ashx?upload=start',
        add: function (e, data) {
            var val = data.files[0].name.toLowerCase();
            var regex = new RegExp("(.*?)\.(jpg|jpeg)$");
            if (!(regex.test(val))) {
                $(this).val('');
                alert('Sólo archivos JPG');
            }
            else data.submit();
            $('#progressbar').show();
        },
        progress: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progressbar div').css('width', progress + '%');
        },
        success: function (response, status) {
            $('#progressbar').hide();
            $('#progressbar div').css('width', '100%');
            asi = response;
            $("#img_sello").attr('src', '/Upload/' + response);
            $("#img_sello").show();
            

        },
        error: function (error) {
            $('#progressbar').hide();
            $('#progressbar div').css('width', '0%');
            console.log('error', error);
        }
    });
});

formPage = function () {
    var idSociedad;
    var colonias = [];

    "use strict"; return {

        clean: function () {    //limpia los campos y los deja listos para insertar uno nuevo


            $("#img_firma").hide();
            $("#img_sello").hide();




            var Cve_Sociedad = $("#Cve_Sociedad").val("");           //EDITAR
            var Sociedad = $("#Sociedad").val("");               //EDITAR
            var RFC = $("#RFC").val("");                    //EDITAR
            var CURP = $("#CURP").val("");                   //EDITAR
            var Direccion_Pais = $("#Direccion_Pais").val("");         //EDITAR
            var Direccion_Estado = $("#Direccion_Estado").val("");       //EDITAR
            var Direccion_Ciudad = $("#Direccion_Ciudad").val("");       //EDITAR
            var Direccion_Entidad = $("#Direccion_Entidad").val("");      //EDITAR
            var Direccion_Colonia = $("#Direccion_Colonia").val("");      //EDITAR
            var Direccion_Calle = $("#Direccion_Calle").val("");        //EDITAR
            var Direccion_Numero = $("#Direccion_Numero").val("");       //EDITAR
            var Direccion_CP = $("#Direccion_CP").val("");           //EDITAR

            var RepresentanteLegal = $("#RepresentanteLegal").val("");     //EDITAR
            var RL_RFC = $("#RL_RFC").val("");                 //EDITAR
            var RL_CURP = $("#RL_CURP").val("");                //EDITAR
            var RL_Direccion_Pais = $("#RL_Direccion_Pais").val("");      //EDITAR
            var RL_Direccion_Estado = $("#RL_Direccion_Estado").val("");    //EDITAR
            var RL_Direccion_Ciudad = $("#RL_Direccion_Ciudad").val("");    //EDITAR
            var RL_Direccion_Entidad = $("#RL_Direccion_Entidad").val("");   //EDITAR
            var RL_Direccion_Colonia = $("#RL_Direccion_Colonia").val("");   //EDITAR
            var RL_Direccion_Calle = $("#RL_Direccion_Calle").val("");     //EDITAR
            var RL_Direccion_Numero = $("#RL_Direccion_Numero").val("");    //EDITAR
            var RL_Direccion_CP = $("#RL_Direccion_CP").val("");        //EDITAR

            var Firma = $("#Firma").val("");                  //EDITAR
            var Firma_Img = $("#Firma_Img").val("");              //EDITAR
            var Sello = $("#Sello").val("");                  //EDITAR
            var Sello_Img = $("#Sello_Img").val("");
            
            $("#clave").prop("disabled", false);
            $('#Direccion_Ciudad').empty();
            $('#Direccion_Colonia').empty();
            $('#Direccion_Entidad').empty();
            $('#Direccion_Estado').empty();
            $('#RL_Direccion_Ciudad').empty();
            $('#RL_Direccion_Colonia').empty();
            $('#RL_Direccion_Entidad').empty();
            $('#RL_Direccion_Estado').empty();


            $("#numinstnotarial").val("");
            $("#volumen").val("");
            $("#ciudadnotarial").val("");
            $("#numeronotariopub").val("");
            $("#nombrenotariopublico").val("");

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


            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").prop("disabled", true);
            $("#formbtndelete").hide();
        },

        edit: function (id) {

            idSociedad = id;
            $('#Direccion_Ciudad').empty();
            $('#Direccion_Colonia').empty();
            $('#Direccion_Entidad').empty();
            $('#Direccion_Estado').empty();
            $('#RL_Direccion_Ciudad').empty();
            $('#RL_Direccion_Colonia').empty();
            $('#RL_Direccion_Entidad').empty();
            $('#RL_Direccion_Estado').empty();

            var model = {
                Cve_Sociedad: id
            }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sociedades/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $("#Cve_Sociedad").val(data.Cve_Sociedad);                  //EDITAR
                    $("#Sociedad").val(data.Sociedad);                          //EDITAR
                    $("#RFC").val(data.RFC);                                    //EDITAR
                    $("#CURP").val(data.CURP);                                  //EDITAR
                    $("#Direccion_Pais").val(data.Direccion_Pais);              //EDITAR
                    $("#Direccion_Estado").val(data.Direccion_Estado);          //EDITAR
                    $("#Direccion_Ciudad").val(data.Direccion_Ciudad);          //EDITAR
                    $("#Direccion_Entidad").val(data.Direccion_Entidad);        //EDITAR
                    $("#Direccion_Colonia").val(data.Direccion_Colonia);        //EDITAR
                    $("#Direccion_Calle").val(data.Direccion_Calle);            //EDITAR
                    $("#Direccion_Numero").val(data.Direccion_Numero);          //EDITAR
                    $("#Direccion_CP").val(data.Direccion_CP);                  //EDITAR

                    $("#RepresentanteLegal").val(data.RepresentanteLegal);      //EDITAR
                    $("#RL_RFC").val(data.RL_RFC);                              //EDITAR
                    $("#RL_CURP").val(data.RL_CURP);                            //EDITAR
                    $("#RL_Direccion_Pais").val(data.RL_Direccion_Pais);        //EDITAR
                    $("#RL_Direccion_Estado").val(data.RL_Direccion_Estado);    //EDITAR
                    $("#RL_Direccion_Ciudad").val(data.RL_Direccion_Ciudad);    //EDITAR
                    $("#RL_Direccion_Entidad").val(data.RL_Direccion_Entidad);  //EDITAR
                    $("#RL_Direccion_Colonia").val(data.RL_Direccion_Colonia);  //EDITAR
                    $("#RL_Direccion_Calle").val(data.RL_Direccion_Calle);      //EDITAR
                    $("#RL_Direccion_Numero").val(data.RL_Direccion_Numero);    //EDITAR
                    $("#RL_Direccion_CP").val(data.RL_Direccion_CP);


                    $("#Firma").val(data.Firma);

                    $("#img_firma").attr('src', '/Upload/' + data.Firma_Img);
                    if (data.Firma_Img != "" && data.Firma_Img != null) { $("#img_firma").show(); }
                    else { $("#img_firma").hide(); }

                    $("#Sello").val(data.Sello);
                    afi = data.Firma_Img;
                    asi = data.Sello_Img;
                    $("#img_sello").attr('src', '/Upload/' + data.Sello_Img);

                    if (data.Sello_Img != "" && data.Sello_Img!=null) { $("#img_sello").show(); }
                    else { $("#img_sello").hide(); }





                    $("#clave").prop("disabled", true);
                   
                    $("#formbtnadd").hide();
                    $("#formbtnsave").show();
                    $("#formbtndelete").show();
                    $("#formbtnadd").prop("disabled", true);
                    $("#formbtnsave").prop("disabled", false);
                    $("#formbtndelete").prop("disabled", false);

                    $('#Direccion_Estado').append($('<option>', {
                        value: data.Direccion_Estado,
                        text: data.Direccion_Estado
                    }));
                    $('#Direccion_Ciudad').append($('<option>', {
                        value: data.Direccion_Ciudad,
                        text: data.Direccion_Ciudad
                    }));
                    $('#Direccion_Entidad').append($('<option>', {
                        value: data.Direccion_Entidad,
                        text: data.Direccion_Entidad
                    }));
                    $('#Direccion_Colonia').append($('<option>', {
                        value: data.Direccion_Colonia,
                        text: data.Direccion_Colonia
                    }));

                    $('#RL_Direccion_Estado').append($('<option>', {
                        value: data.RL_Direccion_Estado,
                        text: data.RL_Direccion_Estado
                    }));
                    $('#RL_Direccion_Ciudad').append($('<option>', {
                        value: data.RL_Direccion_Ciudad,
                        text: data.RL_Direccion_Ciudad
                    }));
                    $('#RL_Direccion_Entidad').append($('<option>', {
                        value: data.RL_Direccion_Entidad,
                        text: data.RL_Direccion_Entidad
                    }));
                    $('#RL_Direccion_Colonia').append($('<option>', {
                        value: data.RL_Direccion_Colonia,
                        text: data.RL_Direccion_Colonia
                    }));

                    $("#numinstnotarial").val(data.numinstnotarial),
                    $("#volumen").val(data.volumen),
                    $("#ciudadnotarial").val(data.ciudadnotarial),
                    $("#numeronotariopub").val(data.numeronotariopub),
                    $("#nombrenotariopublico").val(data.nombrenotariopublico)

                  /*  $("#requestor").val(data.requestor),
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
                    $("#uuid").val(data.uuid)*/

                }

            });

        },

        save: function () {
            var model = {
                Cve_Sociedad: idSociedad,                         //EDITAR
                Sociedad: $("#Sociedad").val(),                 //EDITAR
                RFC: $("#RFC").val(),                      //EDITAR
                CURP: $("#CURP").val(),                     //EDITAR
                Direccion_Pais: $("#Direccion_Pais").val(),           //EDITAR
                Direccion_Estado: $("#Direccion_Estado").val(),         //EDITAR
                Direccion_Ciudad: $("#Direccion_Ciudad").val(),         //EDITAR
                Direccion_Entidad: $("#Direccion_Entidad").val(),        //EDITAR
                Direccion_Colonia: $("#Direccion_Colonia").val(),        //EDITAR
                Direccion_Calle: $("#Direccion_Calle").val(),          //EDITAR
                Direccion_Numero: $("#Direccion_Numero").val(),         //EDITAR
                Direccion_CP: $("#Direccion_CP").val(),             //EDITAR

                RepresentanteLegal: $("#RepresentanteLegal").val(),       //EDITAR
                RL_RFC: $("#RL_RFC").val(),                   //EDITAR
                RL_CURP: $("#RL_CURP").val(),                  //EDITAR
                RL_Direccion_Pais: $("#RL_Direccion_Pais").val(),        //EDITAR
                RL_Direccion_Estado: $("#RL_Direccion_Estado").val(),      //EDITAR
                RL_Direccion_Ciudad: $("#RL_Direccion_Ciudad").val(),      //EDITAR
                RL_Direccion_Entidad: $("#RL_Direccion_Entidad").val(),     //EDITAR
                RL_Direccion_Colonia: $("#RL_Direccion_Colonia").val(),     //EDITAR
                RL_Direccion_Calle: $("#RL_Direccion_Calle").val(),       //EDITAR
                RL_Direccion_Numero: $("#RL_Direccion_Numero").val(),      //EDITAR
                RL_Direccion_CP: $("#RL_Direccion_CP").val(),          //EDITAR

                Firma: $("#Firma").val(),
                Firma_img: afi,
                Sello: $("#Sello").val(),
                Sello_img: asi,

                numinstnotarial: $("#numinstnotarial").val(),
                volumen: $("#volumen").val(),
                ciudadnotarial: $("#ciudadnotarial").val(),
                numeronotariopub: $("#numeronotariopub").val(),
                nombrenotariopublico: $("#nombrenotariopublico").val(),


              /*  requestor: $("#requestor").val(),
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
                uuid: $("#uuid").val()*/

            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sociedades/Save/",
                data: model,
                success: function (data) {

                    //formPage.clean();
                    $('html, body').animate({ scrollTop: 0 }, 'fast');
                    $('#notification').html(data.msg);
                    DataTable.init();
                }

            });

        },

        delete: function (confirm) {

            if (!confirm) {
                $('#modal-delete-Sociedad').modal("show");
                return;
            }

            $('#modal-delete-Sociedad').modal("hide");

            var model = {
                Cve_Sociedad: idSociedad
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sociedades/Delete/",
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
                Cve_Sociedad: $("#Cve_Sociedad").val(),             //EDITAR
                Sociedad: $("#Sociedad").val(),                 //EDITAR
                RFC: $("#RFC").val(),                      //EDITAR
                CURP: $("#CURP").val(),                     //EDITAR
                Direccion_Pais: $("#Direccion_Pais").val(),           //EDITAR
                Direccion_Estado: $("#Direccion_Estado").val(),         //EDITAR
                Direccion_Ciudad: $("#Direccion_Ciudad").val(),         //EDITAR
                Direccion_Entidad: $("#Direccion_Entidad").val(),        //EDITAR
                Direccion_Colonia: $("#Direccion_Colonia").val(),        //EDITAR
                Direccion_Calle: $("#Direccion_Calle").val(),          //EDITAR
                Direccion_Numero: $("#Direccion_Numero").val(),         //EDITAR
                Direccion_CP: $("#Direccion_CP").val(),             //EDITAR

                RepresentanteLegal: $("#RepresentanteLegal").val(),       //EDITAR
                RL_RFC: $("#RL_RFC").val(),                   //EDITAR
                RL_CURP: $("#RL_CURP").val(),                  //EDITAR
                RL_Direccion_Pais: $("#RL_Direccion_Pais").val(),        //EDITAR
                RL_Direccion_Estado: $("#RL_Direccion_Estado").val(),      //EDITAR
                RL_Direccion_Ciudad: $("#RL_Direccion_Ciudad").val(),      //EDITAR
                RL_Direccion_Entidad: $("#RL_Direccion_Entidad").val(),     //EDITAR
                RL_Direccion_Colonia: $("#RL_Direccion_Colonia").val(),     //EDITAR
                RL_Direccion_Calle: $("#RL_Direccion_Calle").val(),       //EDITAR
                RL_Direccion_Numero: $("#RL_Direccion_Numero").val(),      //EDITAR
                RL_Direccion_CP: $("#RL_Direccion_CP").val(),          //EDITAR

                Firma: $("#Firma").val(),
                Firma_img: afi,
                Sello: $("#Sello").val(),
                Sello_img: asi,


                numinstnotarial: $("#numinstnotarial").val(),
                volumen: $("#volumen").val(),
                ciudadnotarial: $("#ciudadnotarial").val(),
                numeronotariopub: $("#numeronotariopub").val(),
                nombrenotariopublico: $("#nombrenotariopublico").val(),


               /* requestor: $("#requestor").val(),
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
                uuid: $("#uuid").val()*/

            }


            if (!formValidation.Validate())
                return;


            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sociedades/Add/",
                data: model,
                success: function (data) {

                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();

                }

            });
        }

       /* buscarCP: function (tipo) {

            var model = {
                sepomex_codigo: $(tipo + 'Direccion_CP').val()
            }
            $(tipo + 'Direccion_Ciudad').empty();
            $(tipo + 'Direccion_Colonia').empty();
            $(tipo + 'Direccion_Entidad').empty();
            $(tipo + 'Direccion_Estado').empty();
            $(tipo + 'Direccion_Pais').empty();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sociedades/BuscarCodigos/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $.each(data, function (i, item) {
                        $(tipo + 'Direccion_Ciudad').append($('<option>', {
                            value: item.ciudad,
                            text: item.ciudad
                        }));
                        $(tipo + 'Direccion_Ciudad option').each(function () {
                            $(this).siblings("[value='" + this.value + "']").remove();
                        });
                        $(tipo + 'Direccion_Pais').append($('<option>', {
                            value: item.pais,
                            text: item.pais
                        }));
                        $(tipo + 'Direccion_Pais option').each(function () {
                            $(this).siblings("[value='" + this.value + "']").remove();
                        });
                        $(tipo + 'Direccion_Estado').append($('<option>', {
                            value: item.estado,
                            text: item.estado
                        }));
                        $(tipo + 'Direccion_Estado option').each(function () {
                            $(this).siblings("[value='" + this.value + "']").remove();
                        });
                        $(tipo + 'Direccion_Colonia').append($('<option>', {
                            value: item.colonia,
                            text: item.colonia
                        }));
                        $(tipo + 'Direccion_Colonia option').each(function () {
                            $(this).siblings("[value='" + this.value + "']").remove();
                        });
                        $(tipo + 'Direccion_Entidad').append($('<option>', {
                            value: item.municipio,
                            text: item.municipio
                        }));
                        $(tipo + 'Direccion_Entidad option').each(function () {
                            $(this).siblings("[value='" + this.value + "']").remove();
                        });
                    });
                }

            });
        },

        buscarEstado: function (tipo) {

            var model = {
                codigo: ''
            }
            $(tipo + 'Direccion_Ciudad').empty();
            $(tipo + 'Direccion_Colonia').empty();
            $(tipo + 'Direccion_Entidad').empty();
            $(tipo + 'Direccion_Estado').empty();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sociedades/Estados/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $.each(data, function (i, item) {
                        $(tipo + 'Direccion_Estado').append($('<option>', {
                            value: item,
                            text: item
                        }));
                    });
                }

            });
        },

        buscarMunicipio: function (tipo) {

            var model = {
                sepomex_estado: $(tipo + "Direccion_Estado").val()
            }
            $(tipo + 'Direccion_Ciudad').empty();
            $(tipo + 'Direccion_Colonia').empty();
            $(tipo + 'Direccion_Entidad').empty();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sociedades/Municipios/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $.each(data, function (i, item) {
                        $(tipo + 'Direccion_Entidad').append($('<option>', {
                            value: item,
                            text: item
                        }));
                    });
                }

            });
        },

        buscarCiudad: function (tipo) {

            var model = {
                sepomex_estado: $(tipo + "Direccion_Estado").val(),
                sepomex_municipio: $(tipo + "Direccion_Entidad").val()
            }
            $(tipo + 'Direccion_Ciudad').empty();
            $(tipo + 'Direccion_Colonia').empty();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sociedades/Ciudades/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $.each(data, function (i, item) {
                        $(tipo + 'Direccion_Ciudad').append($('<option>', {
                            value: item,
                            text: item
                        }));
                    });
                }

            });
        },

        buscarColonia: function (tipo) {

            var model = {
                sepomex_estado: $(tipo + "Direccion_Estado").val(),
                sepomex_municipio: $(tipo + "Direccion_Entidad").val(),
                sepomex_ciudad: $(tipo + "Direccion_Ciudad").val()
            }
            $(tipo + 'Direccion_Colonia').empty();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Sociedades/Colonias/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    colonias = data;
                    $.each(data, function (i, item) {
                        $(tipo + 'Direccion_Colonia').append($('<option>', {
                            value: item.colonia,
                            text: item.colonia
                        }));
                    });
                    $(tipo + 'Direccion_CP').val(colonias[0].codigo);
                }

            });
        },

        cambiarCP: function (tipo) {
            index = $(tipo + 'Direccion_Colonia').prop('selectedIndex')
            $(tipo + 'Direccion_CP').val(colonias[index].codigo);
        }
        */
    }


    }();


DataTable = function () {
    var pag = 1;
    var order = "Cve_sociedad";
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

            window.location.href = '/Sociedades/ExportExcel';

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
                url: "/Sociedades/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {

                    //$('div.block').unblock();
                    $("#datatable").html(msg);

                }

            });
        }

    }


    }();
