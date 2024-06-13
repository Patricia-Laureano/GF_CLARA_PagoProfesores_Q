//var banco1 = new Bancos('banco');
//var tipotransferencia1 = new TiposTrasnferencia('tipodispersion');

var consultar = false;
var borradofechas = false;

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
        $("#formbtndispersion").hide();
        $("#formbtnborarfechas").hide();
        $("#formbtnconsultarpdf").hide();
    });
});//End function jquery

var formPage = function () {
    "use strict"; return {
        clean: function () {

            formValidation.Clean();                   

            $("#fechai").val('');
            $("#fechaf").val('');
            $("#tipodispersion").find('option').attr("selected", false);
            $("#banco").find('option').attr("selected", false);
            $("#formbtndispersion").hide();
            $("#formbtnborarfechas").hide();
            $("#formbtnconsultarpdf").hide();
            $('#cbxSinCuenta').prop('checked', false);
            $("#formbtnborarfechas").hide();
            consultar = false;
            DataTable.init();
        },
        
        setTablaTemporal: function () {
            var filter = $('#Sedes').val();

            var datos = '&fechai=' + $('#fechai').val()
                + '&fechaf=' + $('#fechaf').val()
                + '&banco=' + $('#banco').val()
                + '&tipodispersion=' + $('#tipodispersion').val()
                + '&sinNoCuenta=' + $('#cbxSinCuenta').is(':checked');

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            /*  $.blockUI({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src="" /><br><h3> Espere un momento..</h3>'
            });*/

            $.ajax({
                type: "GET",
                cache: false,
                // async: false,
                url: "/Dispersion/getTabla_TMP/",
                data: "" + datos + "&sede=" + filter,
                success: function (msg) {
                    $('.loader-min-bar').hide();
                    DataTable.init();            
                }
            });
        },

        forzadoborradoTtemp: function () {        
            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');
            
            $.blockUI({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src="" /><br><h3> Espere un momento..</h3>'
            });

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                // async:false,
                url: "/Dispersion/DeleteTablaTemporal",
                //data: model,
                success: function (data) {
                    $('.loader-min-bar').hide();
                    //nueva funcion
                    formPage.setTablaTemporal();
                    // $('#notification').html(data.msg);
                    // DataTable.init();
                },
                error: function (data) {
                    session_error(data);
                }
            });
        },
        
        Concultar: function (id) {          

            consultar = true;

            if (!formValidation.Validate())
                return; 
       
            $("#formbtnborarfechas").hide();//checar
            
            $("#formbtnconsultarpdf").show();
            $("#formbtndispersion").prop("disabled", true);
            $("#formbtnconsultar").prop("disabled", true);
            $("#formbtnclean").prop("disabled", true);
            $("#Sedes").prop("disabled", true);
            
           // return;
            formPage.forzadoborradoTtemp();//forza el eliminar la tabla temporal de dispesion             
        },

        BorrarD: function (id) {
            if (!formValidation.Validate())
                return;

            var r = confirm("¿Estás seguro de borrar las fechas de dispersión?");
            if (r == true) {

                var model = {
                    fechai: $('#fechai').val(),
                    fechaf: $('#fechaf').val(),
                    IdSede: $('#Sedes').val(),
                    banco: $('#banco').val(),
                    IdTransferencia: $('#tipodispersion').val(),
                }


                loading('loading-bar');
                loading('loading-circle', '#datatable', 'Consultando datos..');


                $("#formbtndispersion").prop("disabled", true);
                $("#formbtnconsultar").prop("disabled", true);

                $("#formbtnclean").prop("disabled", true);
                $("#Sedes").prop("disabled", true);

                $("#formbtnborarfechas").hide();


                  $.blockUI({
               css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
               overlayCSS: { backgroundColor: '#FFF' },
               message: '<img src="" /><br><h3> Espere un momento..</h3>'
           });
                 


                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    cache: false,
                    url: "/Dispersion/Delete",
                    data: model,
                    success: function (data) {
                    $('.loader-min-bar').hide();
                 //   $.unblockUI();

                    borradofechas = true;

                   // consultar = false;
                        $('#notification').html(data.msg);

                        var ntype = $('#NotificationType').val();
                        if (ntype == "SUCCESS") {
                          
                            formPage.forzadoborradoTtemp();//forza el eliminar la tabla temporal de dispesion      
                          
                        }
                       // else if (ntype == "ERROR") {
                      //  }

                     


                       // DataTable.init();
                    },
                    error: function (data) {
                        session_error(data);
                    }
                });
            }
        },

        ConsultarPDF: function (id) {
            if (!formValidation.Validate())
                return;

            var model = {
                fechai: $('#fechai').val(),
                fechaf: $('#fechaf').val(),
                IdTransferencia: $('#tipodispersion').val(),
                IdSede: $('#Sedes').val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Dispersion/MergePDFS",
                data: model,

                success: function (data) {
                    $('#notification').html(data.msg);
                },

                error: function (data) {
                    session_error(data);
                }
            });
        },

        ExportExcelDispersion: function (table) {
            if (!formValidation.Validate())
                return;

            var r = confirm("¿Deseas hacer la dispersión?");
            if (r == true) {
                var datos = 'fechai=' + $('#fechai').val()
                    + '&fechaf=' + $('#fechaf').val()
                    + '&banco=' + $('#banco').val()
                    + '&tipodispersion=' + $('#tipodispersion').val()
                    + '&sedes=' + $('#Sedes').val();


               // window.location.href = '/Dispersion/ExportExcelDispersion?' + datos;               

                

                $("#formbtnclean").prop("disabled", true);

                $("#Sedes").prop("disabled", true);

                $("#formbtndispersion").prop("disabled", true);

                $("#formbtnconsultar").prop("disabled", true);


                $("#formbtnborarfechas").hide();


                loading('loading-bar');
              //  loading('loading-circle', '#datatable', 'Consultando datos..');

                /***********************************************/



                $.blockUI({
                    css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                    overlayCSS: { backgroundColor: '#FFF' },
                    message: '<img src="" /><br><h3> Espere un momento..</h3>'
                });

                
                
                $.ajax({
                    type: "GET",
                    cache: false,
                    // async: false,
                    url: "/Dispersion/ExportExcelDispersion/",
                    data:  datos,
                    success: function (msg) {
                      //  $('.loader-min-bar').hide();
                        $('.loading-circle').hide();
                        $('.loader-min-bar').hide();

                        //alert('Si sirve!!!!!!: ' + msg);
                         if (msg.indexOf("_vacio") >= 0) {

                           //  $("#formbtndispersion").show();
                             $("#formbtndispersion").prop("disabled", false);

                             $("#formbtnconsultar").prop("disabled", false);

                             $("#formbtnclean").prop("disabled", false);
                             $("#Sedes").prop("disabled", false);



                             $.unblockUI();
                           

                         }
                         else {
                          
                               formPage.forzadoborradoTtemp();//forza el eliminar la tabla temporal de dispesion  
                         }
                         



                                   


                        window.location.href = '/Dispersion/getResponseExcel?nombrefile=' + msg;
                        



                    }

                    });            




                /****************************************************/




              
            }
        },
    }
}();

Sedes.setSedes_success = function () {
    /*if (!formValidation.Validate())
        return;*/
    $("#formbtnconsultar").prop("disabled", true);
    $("#formbtndispersion").prop("disabled", true);
    $("#formbtnborarfechas").hide();
    
    consultar = false;//cambio
    DataTable.init();
}

var DataTable = function () {
    var pag = 1;
    var order = "FECHA_R";

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

            if (!formValidation.Validate())
                return;

            var datos = 'fechai = ' + $('#fechai').val()
                + '&fechaf = ' + $('#fechaf').val()
                + '&banco = ' + $('#banco').val()
                + '&tipodispersion = ' + $('#tipodispersion').val()
                + '&sedes = ' + $('#Sedes').val();

            window.location.href = '/Dispersion/ExportExcel?' + datos;
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
            if( consultar ){
                filter = $('#Sedes').val();

                loading('loading-bar');
                loading('loading-circle', '#datatable', 'Consultando datos..');
            }

            var datos = '&fechai=' + $('#fechai').val()
                + '&fechaf=' + $('#fechaf').val()
                + '&banco=' + $('#banco').val()
                + '&tipodispersion=' + $('#tipodispersion').val()
                + '&sinNoCuenta=' + $('#cbxSinCuenta').is(':checked');

            $.ajax({
                type: "GET",
                cache: false,
              // async: false,
                url: "/Dispersion/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter,
                success: function (msg) {
                    $('.loader-min-bar').hide();
                    $.unblockUI();

                    $("#datatable").html(msg);
                    //ajax para personas repetidas

                    if (consultar) {
                        if (!borradofechas)
                            DataTable.Coincidencias();
                        // DataTable.Coincidencias();
                      
                        $("#formbtndispersion").show();
                        $("#formbtndispersion").prop("disabled", false);

                        $("#formbtnconsultar").prop("disabled", false);

                        $("#formbtnconsultar").prop("disabled", false);
                        if (msg.indexOf("No existen registros") >= 0) {
                            $("#formbtnborarfechas").hide();                     
                        } else {
                            $("#formbtnborarfechas").show();
                        }
                    } else {
                        $("#formbtndispersion").hide();
                        $("#formbtnborarfechas").hide();

                        setTimeout(function () {
                            $("#formbtnconsultar").prop("disabled", false);
                        }, 600);
                    }
                    //  $("#formbtndispersion").prop("disabled", false);
                    $("#formbtnclean").prop("disabled", false);
                    $("#Sedes").prop("disabled", false);

                    $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                        altClass: 'odd',
                        footer: true,
                        fixedColumns: 3
                    });
                }
            });
        },

        Coincidencias: function () {
            $.ajax({
                type: "GET",
                cache: false,
                // async: false,
                url: "/Dispersion/getPersonasCoincidencias/",
                //  data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + datos + "&filter=" + filter,
                success: function (msg) {
                    if (msg == "") { }
                    else {
                        $("#modal-Mensaje").modal('show');
                        $("#personas_ci").html(msg);
                    }
                }
            });
        }
    }
}();