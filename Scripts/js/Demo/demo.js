
var periodos1 = new Periodos("periodos");
var ciclos1 = new Ciclos("ciclos");
var niveles = new Niveles("niveles");


$(function () {
    
    $(window).load(function () {

     // periodos1.init("periodos");
        ciclos1.init("ciclos");
        niveles.init("niveles");
     // periodos2.init("periodos2");

    });


    $('#ciclos').on('change', function () {

        periodos1.Periodo 
        periodos1.init("periodos");
        
    });

    //periodos2.id_ciclo = "2015";
    //periodos2.init("periodos2");
    
});//End function jquery