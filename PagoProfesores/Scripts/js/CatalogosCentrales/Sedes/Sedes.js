
var direccion1 = new Direccion('Direccion_CP', 'Direccion_Pais', 'Direccion_Estado', 'Direccion_Ciudad', 'Direccion_Entidad', 'Direccion_Colonia');

$(function () {

    $(window).load(function () {
        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        formValidation.Inputs(["Cve_Sede", "Sede", "Cve_Sociedad", "Nombre_Responsable", "Correo_Responsable", "Telefono_Responsable","Sociedad_A_Mostrar"]);
        //formValidation.Inputs(["Cve_Sede", "Sede", "Cve_Sociedad", "Direccion_CP", "Direccion_Pais", "Direccion_Estado", "Direccion_Ciudad", "Direccion_Entidad", "Direccion_Colonia", "Direccion_Calle", "Direccion_Numero", "Nombre_Responsable", "Correo_Responsable", "Telefono_Responsable", "Sociedad_A_Mostrar"]);



        formValidation.notEmpty('Cve_Sede', 'El campo Clave no debe de estar vacio');
        formValidation.notEmpty('Sede', 'El campo Sede no debe de estar vacio');
        //formValidation.notEmpty('Campus_Inb', 'El campo Campus INB no debe de estar vacio');
        //formValidation.notEmpty('TipoContrato_Inb', 'El campo Tipo contrato INB no debe de estar vacio');
        formValidation.notEmpty('Cve_Sociedad', 'El campo Sociedad no debe de estar vacio');

        //formValidation.notEmpty('Direccion_CP', 'El campo "Código Postal" no debe de estar vacio');
        //formValidation.notEmpty('Direccion_Pais', 'El campo "País" no debe de estar vacio');
        //formValidation.notEmpty('Direccion_Estado', 'El campo "Estado" no debe de estar vacio');
        //formValidation.notEmpty('Direccion_Ciudad', 'El campo "Ciudad" no debe de estar vacio');
        //formValidation.notEmpty('Direccion_Entidad', 'El campo "Entidad" no debe de estar vacio');
        //formValidation.notEmpty('Direccion_Colonia', 'El campo "Colonia" no debe de estar vacio');
        //formValidation.notEmpty('Direccion_Calle', 'El campo "Calle" no debe de estar vacio');
        //formValidation.notEmpty('Direccion_Numero', 'El campo "Número" no debe de estar vacio');
        formValidation.notEmpty('Nombre_Responsable', 'El campo "Nombre del Responsable" no debe de estar vacio');
        formValidation.notEmpty('Correo_Responsable', 'El campo "Correo Electrónico" no debe de estar vacio');
        formValidation.notEmpty('Telefono_Responsable', 'El campo "Teléfono Responsable" no debe de estar vacio');

        direccion1.init("direccion1");

        listado(null);
        mostrarSociedades(null);
    });
});//End function jquery

var formPage = function () {
    var Cve_Sede;

    "use strict"; return {

        clean: function () {
            formValidation.Clean();

            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").prop("disabled", true);
            $("#formbtndelete").hide();

            $("#Cve_Sede").prop("disabled", false);

            var Direccion_CP = $("#Direccion_CP").val("");           //EDITAR
            var Direccion_Pais = $("#Direccion_Pais").val("");       //EDITAR
            var Direccion_Estado = $("#Direccion_Estado").val("");   //EDITAR
            var Direccion_Ciudad = $("#Direccion_Ciudad").val("");   //EDITAR
            var Direccion_Entidad = $("#Direccion_Entidad").val(""); //EDITAR
            var Direccion_Colonia = $("#Direccion_Colonia").val(""); //EDITAR
            var Direccion_Calle = $("#Direccion_Calle").val("");     //EDITAR
            var Direccion_Numero = $("#Direccion_Numero").val("");   //EDITAR
            var Correo_Responsable = $("#Correo_Responsable").val("");       //EDITAR
        },

        edit: function (id) {
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

                    $("#Direccion_CP").val(data.Direccion_CP);
                    $("#Direccion_Pais").val(data.Direccion_Pais);
                    $("#Direccion_Estado").val(data.Direccion_Estado);
                    $("#Direccion_Ciudad").val(data.Direccion_Ciudad);
                    $("#Direccion_Entidad").val(data.Direccion_Entidad);
                    $("#Direccion_Colonia").val(data.Direccion_Colonia);
                    $("#Direccion_Calle").val(data.Direccion_Calle);
                    $("#Direccion_Numero").val(data.Direccion_Numero);
                    $("#Nombre_Responsable").val(data.Nombre_Responsable);
                    $("#Correo_Responsable").val(data.Correo_Responsable);
                    $("#Telefono_Responsable").val(data.Telefono_Responsable);
                    $("#Sociedad_A_Mostrar").val(data.Sociedad_A_Mostrar);

                    $("#Cve_Sede").prop("disabled", true);

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
                   

                    listado(data.Cve_Sociedad);
                    mostrarSociedades(data.Sociedad_A_Mostrar);
                
                }
            });
        },

        save: function () {

            var cvSociedad = $("#Cve_Sociedad").val();
            var cvSociedadMostrar = $("#Sociedad_A_Mostrar").val();

            if(cvSociedad == null){
                $(this).addClass('parsley-error');
                $('#notification').html(formValidation.getMessage("Seleccione una Sociedad"));
                return false;
            }
            var Cve_Sociedades = "";
            for (var i = 0; i < cvSociedad.length; i++) {
                if (Cve_Sociedades == "") {
                    Cve_Sociedades = cvSociedad[i];
                } else {
                    Cve_Sociedades += "," + cvSociedad[i];
                }
            }

            var model =
            {
                Cve_Sede: Cve_Sede,
                Cve_Sociedad: Cve_Sociedades, //$("#Cve_Sociedad").val()
                Sede: $("#Sede").val(),
                Campus_Inb: $("#Campus_Inb").val(),
                TipoContrato_Inb: $("#TipoContrato_Inb").val(),
                Direccion_Pais: $("#Direccion_Pais").val(),
                Direccion_Estado: $("#Direccion_Estado").val(),
                Direccion_Ciudad: $("#Direccion_Ciudad").val(),
                Direccion_Entidad: $("#Direccion_Entidad").val(),
                Direccion_Colonia: $("#Direccion_Colonia").val(),
                Direccion_Calle: $("#Direccion_Calle").val(),
                Direccion_Numero: $("#Direccion_Numero").val(),
                Direccion_CP: $("#Direccion_CP").val(),
                Correo_Responsable: $("#Correo_Responsable").val(),
                Nombre_Responsable: $("#Nombre_Responsable").val(),
                Telefono_Responsable: $("#Telefono_Responsable").val(),
                Sociedad_A_Mostrar: $("#Sociedad_A_Mostrar").val(),
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
            var cvSociedad = $("#Cve_Sociedad").val();
            var Cve_Sociedades = "";
            for (var i = 0; i < cvSociedad.length; i++) {
                if (Cve_Sociedades == "") {
                    Cve_Sociedades = cvSociedad[i];
                } else {
                    Cve_Sociedades += "," + cvSociedad[i];
                }
            }

            var model = {
                Cve_Sociedad: Cve_Sociedades,
                Cve_Sede: $("#Cve_Sede").val(),
                Sede: $("#Sede").val(),
                Campus_Inb: $("#Campus_Inb").val(),
                TipoContrato_Inb: $("#TipoContrato_Inb").val(),
                Direccion_Pais: $("#Direccion_Pais").val(),
                Direccion_Estado: $("#Direccion_Estado").val(),
                Direccion_Ciudad: $("#Direccion_Ciudad").val(),
                Direccion_Entidad: $("#Direccion_Entidad").val(),
                Direccion_Colonia: $("#Direccion_Colonia").val(),
                Direccion_Calle: $("#Direccion_Calle").val(),
                Direccion_Numero: $("#Direccion_Numero").val(),
                Direccion_CP: $("#Direccion_CP").val(),
                Correo_Responsable: $("#Correo_Responsable").val(),
                Nombre_Responsable: $("#Nombre_Responsable").val(),
                Telefono_Responsable: $("#Telefono_Responsable").val(),
                Sociedad_A_Mostrar: $("#Sociedad_A_Mostrar").val(),
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
function listado(Cve_SociedadList) {

    var dd = Cve_SociedadList;

    $.ajax({
        type: "POST",
        dataType: 'json',
        cache: false,
        url: "/Sedes/ListadoSociedades/",
        success: function (data) {
            if (data != null) {
                var listado = "";
                var sociedadesList = data;
                var sociedadesList1 = sociedadesList.split('/');
                var sociedadesList2 = "";

                listado = "<select id='Cve_Sociedad' class='form-control' multiple='multiple'>";
                var seleccionado = 0;
                for (var j = 0; j < sociedadesList1.length; j++) {
                    if (dd != null) {

                        var Cve_SociedadList = dd.split(',');

                        for (var z = 0; z < Cve_SociedadList.length; z++) {
                            if (sociedadesList1[j].split('|')[0] == Cve_SociedadList[z]) {
                                listado += "<option selected value='" + sociedadesList1[j].split('|')[0] + "'>" + sociedadesList1[j].split('|')[0] + " | " + sociedadesList1[j].split('|')[1] + " </option>";
                                seleccionado = 1;
                                break;
                            } 
                        }
                        if (seleccionado != 1) {
                            listado += "<option value='" + sociedadesList1[j].split('|')[0] + "'>" + sociedadesList1[j].split('|')[0] + " | " + sociedadesList1[j].split('|')[1] + "</option>";
                        }
                        seleccionado = 0;
                    } else {
                        listado += "<option value='" + sociedadesList1[j].split('|')[0] + "'>" + sociedadesList1[j].split('|')[0] + " | " + sociedadesList1[j].split('|')[1] + "</option>";
                    }
                }
                listado += "</select>"
            }
            document.getElementById("divSociedades").innerHTML = listado;
            }
        });
}

function mostrarSociedades(Cve_SociedadList) {

    var dd = Cve_SociedadList;

    $.ajax({
        type: "POST",
        dataType: 'json',
        cache: false,
        url: "/Sedes/MostrarSociedades/",
        data: { Cve_SedeMostrar: dd },
        success: function (data) {
            if (data != null) {
                var listado = "";
                var sociedadesList = data;
                var sociedadesList1 = sociedadesList.split('/');
                var sociedadesList2 = "";

                listado = "<select id='Sociedad_A_Mostrar' class='form-control'>";
                var seleccionado = 0;
                for (var j = 0; j < sociedadesList1.length; j++) {
                    if (dd != null) {

                        var Cve_SociedadList = dd.split(',');

                        for (var z = 0; z < Cve_SociedadList.length; z++) {
                            if (sociedadesList1[j].split('|')[0] == Cve_SociedadList[z]) {
                                listado += "<option selected value='" + sociedadesList1[j].split('|')[0] + "'>" + sociedadesList1[j].split('|')[0] + " | " + sociedadesList1[j].split('|')[1] + " </option>";
                                seleccionado = 1;
                                break;
                            }
                        }
                        if (seleccionado != 1) {
                            listado += "<option value='" + sociedadesList1[j].split('|')[0] + "'>" + sociedadesList1[j].split('|')[0] + " | " + sociedadesList1[j].split('|')[1] + "</option>";
                        }
                        seleccionado = 0;
                    } else {
                        listado += "<option value='" + sociedadesList1[j].split('|')[0] + "'>" + sociedadesList1[j].split('|')[0] + " | " + sociedadesList1[j].split('|')[1] + "</option>";
                    }
                }
                listado += "</select>"
            }
            document.getElementById("divSociedadesMostrar").innerHTML = listado;
        }
    });
}

