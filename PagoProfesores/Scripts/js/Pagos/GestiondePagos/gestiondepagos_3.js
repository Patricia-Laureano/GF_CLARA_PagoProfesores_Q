
var consultar = false;

$(function () {
    $("#fechai").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });
    
    $("#fechaf").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd'
    });
    
    $(document).ready(function () {
        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 3
        });
    });
    
    $(window).load(function () {
        formValidation.Inputs(["fechai", "fechaf"]);
        $("#formbtn_publicar").hide();
        $("#formbtn_despublicar").hide();
    });
});//End function jquery

var formPage = function () {
    var idEstadoCuenta;
    "use strict"; return {

        clean: function () {
           formValidation.Clean();
            consultar = false;

            $("#fechai").val('');
            $("#fechaf").val('');
            $("#publicar").val('');
            $("#idECP").val("");
            $("#formbtn_publicar").hide();
            $("#formbtn_despublicar").hide();

            DataTable.setShow();
            DataTable.init();
        },

        edit: function (id) {
            $("#idECP").val(id);

            var model = {
                IdEdoCta: $("#idECP").val(),
                sede: $('#Sedes').val(),
                fechai: $("#fechai").val(),
                fechaf: $("#fechaf").val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/GestiondePagos/DatosPersona/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);

                    $("#id_siuRC").val(data.IDSIU);
                    $("#Nombre").val(data.nombres);
                    $("#concepto").val(data.Concepto);
                    $("#fpago").val(data.Fechadepago);

                    $('#modal-publicar-despublicar').modal("show");
                },
                error: function (msg) {
                }
            });
        },

        Btncerrar: function (id) {
            $('#modal-publicar-despublicar').modal("hide");
            $("#idECP").val("");
        },

        Concultar: function (id) {
            
            if (!formValidation.Validate())
                return;
                
            $("#formbtn_publicar").show();
            $("#formbtn_despublicar").show();
            consultar = true;
            DataTable.consultar = true;
            DataTable.setShow();
            DataTable.init();
        },
        

        Btncerrar_publicar_seleccionar_todos: function (id) {        
            $('#modal-publicar-selec_todos').hide();
            $("#idECP").val("");
        },

        Btncerrar_despublicar_seleccionar_todos: function (id) {
            $('#modal-despublicar-selec_todos').hide();
            $("#idECP").val("");
        },


        btnpublicar_selec_todos: function () {        
            $("#modal-publicar-selec_todos").show();
            $("#publicar").val(1);
        },


        btndespublicar_selec_todos: function () {
            $("#modal-despublicar-selec_todos").show();
            $("#publicar").val(0);
        },



        btnpublicar_seleccionados: function () {


            var arr = DataTable.checkboxs;
            var arrChecked = [];

            for (var i = 0; i < arr.length; i++) {
                var checkbox_checked = $('#' + arr[i]).prop('checked');

                if (checkbox_checked == true)
                    arrChecked.push(arr[i]);
            }

            if (arrChecked.length == 0) {
                alert('Debes seleccionar una casilla');
                return;
            }


            var model = {              
                Publicar: $("#publicar").val(),
                sede: $('#Sedes').val(),
                fechai: $("#fechai").val(),
                fechaf: $("#fechaf").val(),              
                ids: arrChecked.join()
            }

            loading('loading-bar');
           /* loading('loading-circle', '#datatable', 'Publicando..');
            loading('loading-circle', '#seccion1', 'publicando..');*/


            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/GestiondePagos/PublicarDespublicar_Seleccionados/",
                //  data: "ids=" + arrChecked.join(),
                data: model,
                success: function (data) {                      

                    $("#modal-publicar-selec_todos").hide();
                    $("#modal-despublicar-selec_todos").hide();

                    $('.loader-min-bar').hide();
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $('#notification').html(data.msg);
                    DataTable.init();

                }, error: function (msg) {
                    session_error(msg);

                }
            });


        },    



        btnpublicar: function () {

            var r = confirm("¿Estas seguro de publicar?");

            if (r == true) { } 
            else {
                $("#idECP").val("");
                return;               
            }
            
            $("#publicar").val(1);
            formPage.Publicar();
        },

        btndespublicar: function () {
            var r = confirm("¿Estas seguro de despublicar?");
            if (r == true) { }
            else {
                $("#idECP").val("");
                return;
            }

            $("#publicar").val(0);
            formPage.Despublicar();
        },
        
        btnpublicarModal: function () {
            var r = confirm("¿Estas seguro de publicar?");

            if (r == true) { }
            else {
                $("#idECP").val("");
                return;
            }

            $("#publicar").val(1);
            formPage.Publicar();
        },

        btndespublicarModal: function () {
            var r = confirm("¿Estas seguro de despublicar?");
            if (r == true) { }
            else {
                $("#idECP").val("");
                return;
            }

            $("#publicar").val(0);
            formPage.Despublicar();
        },

        Publicar: function () {
            var model = {
                IdEdoCta: $("#idECP").val(),
                Publicar: $("#publicar").val(),
                sede: $('#Sedes').val(),
                fechai: $("#fechai").val(),
                fechaf: $("#fechaf").val()
            }

            if (!formValidation.Validate())
                 return;
            
            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/GestiondePagos/Publicar/",
                data: model,
                success: function (data) {

                    $("#idECP").val("");
                    $('#modal-publicar-despublicar').modal("hide");
                    $("#modal-publicar-selec_todos").hide();               

                    $('#notification').html(data.msg);
                    DataTable.setShow();
                    DataTable.init();
                },
                error: function (msg) {
                }
            });
        },

        Despublicar: function () {
            var model = {
                IdEdoCta: $("#idECP").val(),
                Publicar: $("#publicar").val(),
                sede: $('#Sedes').val(),
                fechai: $("#fechai").val(),
                fechaf: $("#fechaf").val()
            }

            if (!formValidation.Validate())
                 return;
            
            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/GestiondePagos/Despublicar/",
                data: model,
                success: function (data) {
                   
                    $("#idECP").val("");
                    $('#modal-publicar-despublicar').modal("hide");
                    $("#modal-despublicar-selec_todos").hide();

                    $('#notification').html(data.msg);
                    DataTable.setShow();
                    DataTable.init();
                }
            });
        }
    }
}();

Sedes.setSedes_success = function () {
    if (!formValidation.Validate())
        return;

    consultar = true;
    DataTable.init();
}

var DataTable = function () {
    var pag = 1;
    var order = "IDSIU";

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
            var datos = 'fechai=' + $('#fechai').val()
              + '&fechaf=' + $('#fechaf').val()
              + '&sedes=' + $('#Sedes').val();

            window.location.href = '/GestiondePagos/ExportExcel?' + datos;
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

            var filter = "";
            if (consultar) { filter = $('#Sedes').val(); }

            var publicado = $("#publicar").val();

            var datos = '&fechai=' + $('#fechai').val()
                + '&fechaf=' + $('#fechaf').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/GestiondePagos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter + "&publicado="+ publicado,
                success: function (msg) {

                    $('.loader-min-bar').hide();
                    $("#datatable").html(msg);

                    $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                        altClass: 'odd',
                        footer: true,
                        fixedColumns: 3
                    });
                }
            });
        }
    }
}();