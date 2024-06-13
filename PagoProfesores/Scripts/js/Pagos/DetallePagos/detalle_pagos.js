
var ciclo1 = new Ciclos('ciclos');
var periodo1 = new Periodos('periodos');
var tipospago1 = new TiposdePagos('tipospago');
var escuela1 = new Escuelas('escuelas');
var niveles1 = new Niveles('niveles');


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

        ciclo1.init("ciclo1");
        tipospago1.init("tipospago1");
        escuela1.init("escuela1");
        niveles1.init("niveles1");
      


        formValidation.Inputs(["ciclos", "periodos"]);
        formValidation.notEmpty('ciclos', 'El campo Año escolar no debe de estar vacio');
        formValidation.notEmpty('periodos', 'El campo Año escolar no debe de estar vacio');
       // formValidation.notEmpty('niveles', 'El campo Nivel no debe de estar vacio');
       

    });
});//End function jquery

function handlerdataSedes() { }


function handlerdataCiclos() {
    
    periodo1.id_ciclo = $("#ciclos").val();
    periodo1.init("periodo1");

}


function handlerdataEscuelas() {
}
 
function handlerdataPeriodos() {
}

function handlerdataNiveles() {
}

function handlerdataTiposPagos() { 

}




$('#ciclos').on('change', function () {
   
    periodo1.id_ciclo = this.value;
    periodo1.init("periodo1");
    
});



var formPage = function () {
 

    "use strict"; return {

        clean: function () {
           // formValidation.Clean();
         //   $("#formbtnadd").prop("disabled", false);
        
            $("#ciclos").find('option').attr("selected", false);
            $("#periodos").find('option').attr("selected", false);          
            $("#tipospago").find('option').attr("selected", false);
            $("#escuelas").find('option').attr("selected", false);
          //  $("#tipocontrato").find('option').attr("selected", false);
            $("#niveles").find('option').attr("selected", false);

            consultar = false;

            DataTable.init();
        
            //    $("#esquema").val('');      



        },        



        Concultar: function (id) {

            if (!formValidation.Validate())
                return;


            consultar = true;

          //  DataTable.consultar = true;
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
            filter = "";

            if (consultar) {

                var filter = $('#Sedes').val();
            }
          //  var filter = $('#Sedes').val();           

           var periodos = $("#periodos").val();
           var tipospago = $("#tipospago").val();
           var escuelas = $("#escuelas").val();
           var niveles = $("#niveles").val();
         //  var tipocontrato = $("#tipocontrato").val();        


         /*  $('#datatable').block({
            css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
            overlayCSS: { backgroundColor: '#FFF' },
            message: '<img src="/Content/images/load-search.gif" /><br><h2> Buscando..</h2>'
        });*/


        loading('loading-bar');
		loading('loading-circle', '#datatable', 'Consultando datos..');


            $.ajax({
                type: "GET",
                cache: false,
                url: "/DetallePagos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null&periodos=" + periodos + "&tipospago=" + tipospago + "&escuelas=" + escuelas + "&niveles=" + niveles + "&consultar=" + consultar + "&filter=" + filter,
                success: function (msg) {
                    $('.loader-min-bar').hide();

                   // $('#frm-importar').unblock();
                    $("#datatable").html(msg);

                    $(document).ready(function () {

                       /* $('#' + DataTable.myName + '-fixed').fixedHeaderTable({
                            altClass: 'odd',
                            footer: true,
                            fixedColumns: 3
                        });*/

                    });



                }

            });
        }

    }


}();

