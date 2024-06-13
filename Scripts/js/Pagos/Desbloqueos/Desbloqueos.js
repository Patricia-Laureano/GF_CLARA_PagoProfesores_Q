
var add_ciclo1 = new Ciclos('add_ciclos');
var add_periodo1 = new Periodos('add_periodos');
var add_esquema1 = new Esquema('add_esquema');
var add_concepto1 = new ConceptosdePago("add_concepto");
var add_fechaPago1 = new FechasdePagoEsquema("add_fechaPago");



var consultar = false;

$(function () {


    $(document).ready(function () {

       /* $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumns: 3
        });*/

    });


    $(window).load(function () {

        add_ciclo1.init("add_ciclo1");
  
        $('#formbtnbloquear').hide();
        $('#formbtndesbloquear').hide();      
      


        formValidation.Inputs(["add_ciclos", "add_periodos"]);
        formValidation.notEmpty('add_ciclos', 'El campo Año Escolar no debe de estar vacio');
        formValidation.notEmpty('add_periodos', 'El campo periodo no debe de estar vacio');
     //   formValidation.notEmpty('periodos', 'El campo Año escolar no debe de estar vacio');
       // formValidation.notEmpty('niveles', 'El campo Nivel no debe de estar vacio');
       

    });
});//End function jquery






$('#add_ciclos').on('change', function () {

    $('#add_esquema').html("<option><option>");
    $('#add_concepto').html("<option><option>");
    $('#add_fechaPago').html("<option><option>");

    if (this.value == "") {//ciclo(anio) 
        $('#add_periodos').html("<option><option>");
    } else {
        add_periodo1.id_ciclo = this.value;
        add_periodo1.init("add_periodo1");

    }

});

$('#add_periodos').on('change', function () {

    $('#add_concepto').html("<option><option>");
    $('#add_fechaPago').html("<option><option>");

    if (this.value == "") {//periodo
        $('#add_esquema').html("<option><option>");



    } else {

        add_esquema1.Sede = $('#Sedes').val();
        add_esquema1.Periodo = this.value;
     //   add_EC_esquema1.Nivel = $('#add_EC_nivel').val();
        add_esquema1.init();
    }



});



$('#add_esquema').on('change', function () {

    $('#add_fechaPago').html("<option><option>");

    if (this.value == "") {//esquema
        $('#add_concepto').html("<option><option>");

    } else {
        add_concepto1.EsquemaID = this.value;
        add_concepto1.init();
    }

});

$('#add_concepto').on('change', function () {  

    add_fechaPago1.EsquemaID = $('#add_esquema').val();
    add_fechaPago1.init();

});

$('#add_fechaPago').on('change', function () {
    $("#add_concepto option[value='" + this.value + "']").attr("selected", "selected");

});

function handlerdataFechasdepagoEsquema() {

    $("#add_fechaPago option[value='" + $('#add_concepto').val() + "']").attr("selected", "selected");

}


function handlerdataSedes() { }


function handlerdataCiclos() {
    
  //  add_periodo1.id_ciclo = $("#add_ciclos").val();
   // add_periodo1.init("periodo1");


}


function handlerdataPeriodos() {
}

function handlerdatagetConceptosdePago() {
}




var formPage = function () {
 

    "use strict"; return {

        clean: function () {

            formValidation.Clean();
         //   $("#formbtnadd").prop("disabled", false);
        
            $("#add_ciclos").find('option').attr("selected", false);
            $("#add_periodos").find('option').attr("selected", false);
            $("#add_esquema").find('option').attr("selected", false);
            $("#add_concepto").find('option').attr("selected", false);
            $("#add_fechaPago").find('option').attr("selected", false);        

            $('#formbtnbloquear').hide();
            $('#formbtndesbloquear').hide();


           var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
           for (var i = 0; i < maxBloqueos; i++) {
               $('#TipoBloqueo_' + i).prop('checked', false);
           }

            consultar = false;
            DataTable.init();
        },

      
        bloquear: function () {
            formPage.bloquear_desbloquear(true);
        },

        desbloquear: function () {
            formPage.bloquear_desbloquear(false);
        },


        bloquear_desbloquear: function (bloquear) {


            if (!formValidation.Validate())              
                return;
            

            var arrBloqueos = new Array();
            var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
            for (var i = 0; i < maxBloqueos; i++) {
                var bloqueo = $('#TipoBloqueo_' + i).val();
                var activo = $('#TipoBloqueo_' + i).prop('checked');
                if (activo) {
                    arrBloqueos.push(bloqueo);
                }
            }


            var arr = DataTable.checkboxs;
            var arrChecked = [];

            for (var i = 0; i < arr.length; i++) {
                var checkbox_checked = $('#' + arr[i]).prop('checked');

                if (checkbox_checked == true)
                    arrChecked.push(arr[i]);
            }

            if (arrChecked.length == 0) {
                alert('Debes seleccionar una casilla para la persona');
                return;
            }

            var bloqueos = arrBloqueos.join();

            if (bloqueos.length == 0) {
                alert('Debes seleccionar una casilla para bloquear/desbloquear');
                return;
            }
          

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Actualizando datos..');
            //   loading('loading-circle', '#seccion1', 'Importando PA desde BANNER..');

            var metodo = bloquear == true ? "bloquear" : "desBloquear";
           

            $.ajax({
                type: "POST",
                cache: false,
                url: "/Desbloqueos/" + metodo,
                data: "ids=" + arrChecked.join()+'&bloqueos='+ bloqueos,
                success: function (msg) {
                    $('#seccion1').unblock();
                    DataTable.init();
                    $('#notification').html(msg);

                 
               
                },
                error: function (msg) {
                    $('#seccion1').unblock();
                    $('#notification').html(msg);
                }
            });
        },




        


        Concultar: function (id) {

           if (!formValidation.Validate())
               return;          

           consultar = true;
           $('#formbtnbloquear').show();
           $('#formbtndesbloquear').show();

            DataTable.init();

        }




    }


}();

/*var Sedes = function () {

    "use strict"; return {

        setSedes: function () {
            consultar = true;
            DataTable.init();
        }
    }
}();*/



Sedes.setSedes_success = function () {

    consultar = true;
    DataTable.init();

}


var DataTable = function () {
    var pag = 1;
    var order = "IDSIU";
   // var consultar;

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

              var datos ='periodos=' + $('#periodos').val()
                + '&tipospago=' + $('#tipospago').val()               
                + '&escuelas=' + $('#escuelas').val()
                + '&niveles=' + $('#niveles').val()
               // + '&tipocontrato=' + $('#tipocontrato').val()
                + '&sedes=' + $('#Sedes').val();

              window.location.href = '/DetallePagos/ExportExcel?' + datos;

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

            if (consultar) {

                filter = $('#Sedes').val();
            }
          //  var filter = $('#Sedes').val();           


                var arrBloqueos = new Array();
                var maxBloqueos = parseInt($('#TipoBloqueo_length').val());
                for (var i = 0; i < maxBloqueos; i++) {
                    var bloqueo = $('#TipoBloqueo_' + i).val();
                    var activo = $('#TipoBloqueo_' + i).prop('checked');
                    if (activo) {
                        arrBloqueos.push(bloqueo);
                    }
                }

                var bloqueos_filtros = arrBloqueos.join();

            var datos = '&add_periodos=' + $('#add_periodos').val()
                  + '&add_esquema=' + $('#add_esquema').val()
                  + '&add_concepto=' + $('#add_concepto').val()
                  + '&add_fechaPago=' + $('#add_fechaPago').val()
                  + '&bloqueos=' + bloqueos_filtros
                ;    


        loading('loading-bar');
		loading('loading-circle', '#datatable', 'Consultando datos..');


            $.ajax({
                type: "GET",
                cache: false,
                url: "/Desbloqueos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null&consultar=" + consultar + "&filter=" + filter + datos,
                success: function (msg) {
                    $('.loader-min-bar').hide();                
                    $("#datatable").html(msg);

                    $(document).ready(function () {

                   

                    });



                }

            });
        }

    }


}();

