var ciclo1 = new Ciclos('ciclos');
var periodo1 = new Periodos('periodos');
var consultar = false;

$(function () {

    $(document).ready(function () {
        $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 2
        });
    });

    $(window).load(function () {
        ciclo1.init("ciclo1");
        $("#formbtn_publicar").hide();
        $("#formbtn_despublicar").hide();
    });
});//End function jquery

function handlerdataCiclos() {
    periodo1.id_ciclo = $("#ciclos").val();
    periodo1.init("periodo1");
}

function handlerdataPeriodos() {
}

var formPage = function () {

    "use strict"; return {

        clean: function () {
            formValidation.Clean();
            consultar = false;

            $("#ciclos").val('');
            $("#periodos").val('');
            $("#publicar").val('');
            $("#formbtn_publicar").hide();
            $("#formbtn_despublicar").hide();

            DataTable.setShow();
            DataTable.init();
        },

        edit: function (id) {
           
        },

     

        Concultar: function (id) {
            $("#formbtn_publicar").show();
            $("#formbtn_despublicar").show();
            consultar = true;
            DataTable.consultar = true;
            DataTable.init();
        },

        Btncerrar_publicar_seleccionar_todos: function (id) {
            $('#modal-publicar-selec_todos').hide();
           
        },

        Btncerrar_despublicar_seleccionar_todos: function (id) {
            $('#modal-despublicar-selec_todos').hide();
            
        },

        btnpublicar_selec_todos: function () {
            $("#modal-publicar-selec_todos").show();
            $("#publicar").val(1);
        },

        btndespublicar_selec_todos: function () {
            $("#modal-despublicar-selec_todos").show();
            $("#publicar").val(0) ;
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
                ciclos: $("#ciclos").val(),
                periodos: $("#periodos").val(),
                ids: arrChecked.join()
            }

            loading('loading-bar');

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/PublicarContratos/PublicarDespublicar_Seleccionados/",
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

            var r = confirm("¿Estas seguro de publicar todos los contratos del Campus " +$('#Sedes').val() + "?");

            if (r == true) { loading('loading-bar');}
            else {
                return;
            }

            $("#publicar").val(1);
            formPage.Publicar();
        },

        btndespublicar: function () {
            var r = confirm("¿Estas seguro de publicar todos los contratos del Campus " +$('#Sedes').val() + " ? ");
            if (r == true) { loading('loading-bar'); }
            else {
                return;
            }

            $("#publicar").val(0) ;
            formPage.Despublicar();
        },

        btnpublicarModal: function () {
            var r = confirm("¿Estas seguro de publicar?" + $('#Sedes').val());

            if (r == true) { loading('loading-bar'); }
            else {
                return;
            }

            $("#publicar").val(1);
            formPage.Publicar();
        },

        btndespublicarModal: function () {
            var r = confirm("¿Estas seguro de despublicar?" + $('#Sedes').val());
            if (r == true) { loading('loading-bar');}
            else {
              
                return;
            }

            $("#publicar").val(0) ;
            formPage.Despublicar();
        },

        Publicar: function () {

  
            var model = {
               
                Publicar: $("#publicar").val(),
                sede: $('#Sedes').val(),
                ciclos: $("#ciclos").val(),
                periodos: $("#periodos").val(),
            }


            //if (!formValidation.Validate())
            //    return;
            loading('loading-bar');
            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/PublicarContratos/Publicar/",
                data: model,
                success: function (data) {

                    $("#modal-publicar-selec_todos").hide();
                    $("#modal-despublicar-selec_todos").hide();
                    $('#modal-publicar-despublicar').modal("hide");
                    $("#modal-publicar-selec_todos").hide();

                    $('.loader-min-bar').hide();
                    $('html, body').animate({ scrollTop: 0 }, 'fast');

                    $('#notification').html(data.msg);
                    DataTable.init();
                    
                },
                error: function (msg) {
                }
            });
        },

        Despublicar: function () {
            var model = {
                Publicar: 'NULL',
                sede: $('#Sedes').val(),
                ciclos: $("#ciclos").val(),
                periodos: $("#periodos").val()
            }

          

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/PublicarContratos/Despublicar/",
                data: model,
                success: function (data) {

                    $("#ID_CONTRATO").val("");
                    $('#modal-publicar-despublicar').modal("hide");
                    $("#modal-despublicar-selec_todos").hide();

                    $('#notification').html(data.msg);
                    DataTable.setShow();
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
            var datos = 'ciclos=' + $('#ciclos').val()
                + '&periodos=' + $('#periodos').val()
                + '&sedes=' + $('#Sedes').val();

            window.location.href = '/PublicarContratos/ExportExcel?' + datos;
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

           
            var publicado = "";
            var datos = "";

            if ($('#ciclos').val() != "" && $('#ciclos').val() != null) {
                datos = '&ciclos=' + $('#ciclos').val()
            }
            if ($('#periodos').val() != "" && $('#periodos').val() != null) {
                datos += '&periodos=' + $('#periodos').val()
            }
           

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/PublicarContratos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter + "&publicado=" + publicado,
                success: function (msg) {

                    $('.loader-min-bar').hide();
                    $("#datatable").html(msg);

                    $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                        altClass: 'odd',
                        footer: true,
                        fixedColumns: 2
                    });
                }
            });
        }
    }
}();