
function GraficaObtienedatosHM() {
    //function Grafica() {
    this.getDatosBarras = function (anio, sede) {


        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            timeout: 50000,
            url: "/Dashboard/GraficaHonorariosMontosBarras/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
                //   alert("ajax=" + data2);
                arr = JSON.parse(data2);
                //   alert("json=" + arr);
                dhm.borra();
                dhm.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert("Error:" +msg); }
        });
        // });
        return arr;

    };
    this.getDatos = function (anio,sede) {

        //var anio = year;
        var data2 = [];
        var arr;
        //Pace.ignore(function()
       // {
        $.ajax({
            type: "GET",
            cache: false,
            timeout: 50000,
            url: "/Dashboard/GraficaHonorariosMontos/",
            data: "anio=" + anio+"&sede="+sede,
            success: function (data) {
                //var hm = new GraficaObtienedatosHM();
                var dhm = new dibujagraficaHM();
                data2 = data;
          //      alert("ajax=" + data2);
                arr = JSON.parse(data2);
                //alert("json=" + arr);
              
                dhm.borra();
                dhm.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert("Error:" + msg); }
        });
       // });
        return arr;
    }
};

function GraficaObtienedatosAM() {
    //function Grafica() {
    this.getDatosBarras = function (anio, sede) {


        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            timeout: 50000,
            url: "/Dashboard/GraficaAsimiladosMontosBarras/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
                //   alert("ajax=" + data2);
                arr = JSON.parse(data2);
                //   alert("json=" + arr);
                dam.borra();
                dam.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert("Error:" + msg); }
        });
        // });
        return arr;

    };
    this.getDatos = function (anio, sede) {

        //var anio = year;
        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            timeout: 50000,
            url: "/Dashboard/GraficaAsimiladosMontos/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
               //  alert("ajax=" + data2);
                arr = JSON.parse(data2);
             //   alert("json=" + arr);
                dam.borra();
                dam.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert("Error:" + msg); }
        });
        // });
        return arr;
    }
};



function GraficaObtienedatosAD() {
    //function Grafica() {
    this.getDatosBarras = function (anio, sede) {

        //var anio = year;
        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            timeout: 50000,
            url: "/Dashboard/GraficaAsimiladosDocentesBarras/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
                //  alert("ajax=" + data2);
                arr = JSON.parse(data2);
                //   alert("json=" + arr);
                dad.borra();
                dad.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert("Error:" + msg); }
        });
        // });
        return arr;
    }
    this.getDatos = function (anio, sede) {

        //var anio = year;
        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            timeout: 50000,
            url: "/Dashboard/GraficaAsimiladosDocentes/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
               //  alert("ajax=" + data2);
                arr = JSON.parse(data2);
              //   alert("json=" + arr);
                dad.borra();
                dad.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert("Error:" + msg); }
        });
        // });
        return arr;
    }
};
function GraficaObtienedatosHD() {
    //function Grafica() {
    this.getDatosBarras = function (anio, sede) {


        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            timeout: 50000,
            url: "/Dashboard/GraficaHonorariosDocentesBarras/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
                //   alert("ajax=" + data2);
                arr = JSON.parse(data2);
                //   alert("json=" + arr);
                dhd.borra();
                dhd.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert("Error:" + msg); }
        });
        // });
        return arr;

    };
    this.getDatos = function (anio, sede) {

        //var anio = year;
        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            timeout: 50000,
            url: "/Dashboard/GraficaHonorariosDocentes/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
              //   alert("ajax=" + data2);
                arr = JSON.parse(data2);
              //   alert("json=" + arr);
                dhd.borra();
                dhd.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert("Error:" + msg); }
        });
        // });
        return arr;
    }
};
function dibujagraficaHM() {
    this.borra = function () {
        RGraph.Clear(document.getElementById('cvs_hm'));
    }
    this.dibuja = function (datos) {
        if (datos == null || datos.length==0)
        {
            datos = [
                  [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                   [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                   [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                   [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
            ];
        }
        var a = datos[0];
        var b = datos[1];
        var c = datos[2];
        var d = a.concat(b, c);
     
        line = new RGraph.Line({
            id: 'cvs_hm',
          
            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['#5B9BD5', '#ED7D31', '#A5A5A5', '#FFC000'],
                key: ['Total', 'Recibos/Disper', 'Deposito', 'Pagos Pendientes'],
                keyPosition: 'gutter',
                keyPositionGutterBoxed: false,
                keyPositionX: 100,
                gutterBottom: 35,
                gutterLeft: 50,
                linewidth: 2,
                shadow: true,
               // // spline: true
                tickmarks: 'circle',
                ticksize: 2,
                textsize: 8,
                hmargin: 5,
                tooltips: [].concat.apply([], d).map(String),
                textAccessible: true,
                eventsClick: function (e, shape) {
                    alert('Da clic en la linea');
                }
            }
        }).draw();
    }
};

function dibujagraficaAM() {
    this.borra = function () {
        RGraph.Clear(document.getElementById('cvs_am'));

    }
    this.dibuja = function (datos) {

        if (datos == null || datos.length==0)
            {
            datos = [
                  [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                   [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                   [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                   [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
            ];
        }
        var a = datos[0];
        var b = datos[1];
        var c = datos[2];
        var d = datos[3];
        var e = a.concat(b, c, d);
       
        line = new RGraph.Line({
            id: 'cvs_am',

            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['#5B9BD5', '#ED7D31', '#A5A5A5', '#FFC000'],
                key: ['Total','Recibos/Disper','Deposito', 'Pagos Pendientes' ],
                keyPosition: 'gutter',
               // keyPositionGutterBoxed: false,
                keyPositionX: 100,
                gutterBottom: 35,
                gutterLeft: 50,
                linewidth: 2,
                shadow: true,
                // spline: true
                tickmarks: 'circle',
                ticksize: 2,
                textsize: 8,
                hmargin: 5,
                 tooltips: [].concat.apply([],d).map(String)
                
            }
        }).draw();
    }
};

function dibujagraficaAD() {
    this.borra = function () {
        RGraph.Clear(document.getElementById('cvs_ad'));
    }
    this.dibuja = function (datos) {
        if (datos == null || datos.length==0)
            {
            datos = [
                    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                     [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                     [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                     [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
                     ] ;

        }
        var a = datos[0];
        var b = datos[1];
        var c = datos[2];
        var d = a.concat(b, c);

        line = new RGraph.Line({
            id: 'cvs_ad',
            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['#5B9BD5', '#ED7D31', '#A5A5A5', '#FFC000'],
                key: ['Total', 'Recibos/Disper', 'Deposito', 'Pagos Pendientes'],
                keyPosition: 'gutter',
                keyPositionGutterBoxed: false,
                keyPositionX: 100,
                gutterBottom: 35,
                gutterLeft: 50,
                linewidth: 2,
                shadow: true,
                // spline: true
                tickmarks: 'circle',
                ticksize: 2,
                textsize: 8,
                hmargin: 5,
                tooltips: [].concat.apply([], d).map(String),
                textAccessible: true,
                eventsClick: function (e, shape) {
                    alert('Da clic en la linea');
                }
            }
        }).draw();
    }
};
  
function dibujagraficaHD() {

 this.borra = function () {
        RGraph.Clear(document.getElementById('cvs_hd'));
    }
    this.dibuja = function (datos) {
        if (datos == null || datos.length==0)
            {
            datos = [
                    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                     [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                     [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                     [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
                     ] ;

        }
        var a = datos[0];
        var b = datos[1];
        var c = datos[2];
        var d = a.concat(b, c);

        line = new RGraph.Line({
            id: 'cvs_hd',
            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['#5B9BD5', '#ED7D31', '#A5A5A5', '#FFC000'],
                key: ['Total', 'Recibos/Disper', 'Deposito', 'Pagos Pendientes'],
                keyPosition: 'gutter',
                keyPositionGutterBoxed: false,
                keyPositionX: 100,
                gutterBottom: 35,
                gutterLeft: 50,
                linewidth: 2,
                shadow: true,
                // spline: true
                tickmarks: 'circle',
                ticksize: 2,
                textsize: 8,
                hmargin: 5,
                tooltips: [].concat.apply([], d).map(String),
                textAccessible: true,
                eventsClick: function (e, shape) {
                    alert('Da clic en la linea');
                }
            }
        }).draw();
    }
};
function dibujagraficaAMBarras() {
    this.borra = function () {
        RGraph.Clear(document.getElementById('cvs_am'));

    }
    this.dibuja = function (datos) {

        if (datos == null || datos.length == 0) {
            datos = [
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0]

            ];
        }
        var a = datos[0];
        var b = datos[1];
        var c = datos[2];
        var d = datos[3];
        var e = a.concat(b, c, d);

        bar = new RGraph.Bar({
            id: 'cvs_am',

            data: datos,
            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['#5B9BD5', '#ED7D31', '#A5A5A5', '#FFC000'],
                key: ['Total', 'Recibos/Disper', 'Deposito', 'Pagos Pendientes'],
                keyPosition: 'gutter',
                keyPositionGutterBoxed: false,
                keyPositionX: 100,
                gutterBottom: 35,
                gutterLeft: 50,
                hmargin: 5

            }
        }).draw();
    }
};
function dibujagraficaADBarras() {
    this.borra = function () {
        RGraph.Clear(document.getElementById('cvs_ad'));
    }
    this.dibuja = function (datos) {
        if (datos == null || datos.length == 0) {
            datos = [
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0],
                 [0, 0, 0]

            ];

        }
        var a = datos[0];
        var b = datos[1];
        var c = datos[2];
        var d = a.concat(b, c);

        bar = new RGraph.Bar({
            id: 'cvs_ad',
            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['#5B9BD5', '#ED7D31', '#A5A5A5', '#FFC000'],
                key: ['Total', 'Recibos/Disper', 'Deposito', 'Pagos Pendientes'],
                keyPosition: 'gutter',
                keyPositionGutterBoxed: false,
                keyPositionX: 100,
                gutterBottom: 35,
                gutterLeft: 50,
                hmargin: 5

            }
        }).draw();

    }
};
function dibujagraficaHDBarras() {
    this.borra = function () {
        RGraph.Clear(document.getElementById('cvs_hd'));
    }
    this.dibuja = function (datos) {
        if (datos == null || datos.length == 0)
        {
            datos = [
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0]

            ];
        }
        var a = datos[0];
        var b = datos[1];
        var c = datos[2];
        var d = a.concat(b, c);

        //line = new RGraph.Line({
         bar = new RGraph.Bar({
            id: 'cvs_hd',
            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['#5B9BD5', '#ED7D31', '#A5A5A5', '#FFC000'],
                key: ['Total', 'Recibos/Disper', 'Deposito', 'Pagos Pendientes'],
                keyPosition: 'gutter',
                keyPositionGutterBoxed: false,
                keyPositionX: 100,
                gutterBottom: 35,
                gutterLeft: 50,               
                hmargin: 5
                
            }
         }).draw();

        /*
        
        new RGraph.Bar({
            id: 'cvs',
            data: [
                [12,15,16],[16,13,12],[13,11,12],
                [15,35,41],[19,14,10],[16,15,13]
            ],
            options: {
                colors: ['#7CB5EC','#434348','#90ED7D'],
                textAccessible: true,
                textSize: 14,
                gutterLeft: 50,
                gutterTop: 40,
                labels: ['January','Febuary','March','April','May','June'],
                shadow: false,
                unitsPost: '%',
                backgroundGridVlines: false,
                backgroundGridBorder: false,
                hmargin: 15,
                hmarginGrouped: 5,
                noxaxis: true,
                noyaxis: true,
                title: 'Fruit baskets sold at Mardi Gras 2012',
                key: ['Fructon Road','Lewisham Avenue','Carnival Mar-de-Gras'],
                keyPosition: 'gutter',
                keyTextSize: 12,
                labelsAbove: true,
                ymax: 50
            }
        }).wave();
    
        */
    }
};
function dibujagraficaHMBarras() {
    this.borra = function () {
        RGraph.Clear(document.getElementById('cvs_hd'));
    }
    this.dibuja = function (datos) {
        if (datos == null || datos.length == 0) {
            datos = [
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0],
                  [0, 0, 0]

            ];
        }
        var a = datos[0];
        var b = datos[1];
        var c = datos[2];
        var d = a.concat(b, c);

        //line = new RGraph.Line({
        bar = new RGraph.Bar({
            id: 'cvs_hm',
            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['#5B9BD5', '#ED7D31', '#A5A5A5', '#FFC000'],
                key: ['Total', 'Recibos/Disper', 'Deposito', 'Pagos Pendientes'],
                keyPosition: 'gutter',
                keyPositionGutterBoxed: false,
                keyPositionX: 100,
                gutterBottom: 35,
                gutterLeft: 50,
                hmargin: 5

            }
        }).draw();

        /*
        
        new RGraph.Bar({
            id: 'cvs',
            data: [
                [12,15,16],[16,13,12],[13,11,12],
                [15,35,41],[19,14,10],[16,15,13]
            ],
            options: {
                colors: ['#7CB5EC','#434348','#90ED7D'],
                textAccessible: true,
                textSize: 14,
                gutterLeft: 50,
                gutterTop: 40,
                labels: ['January','Febuary','March','April','May','June'],
                shadow: false,
                unitsPost: '%',
                backgroundGridVlines: false,
                backgroundGridBorder: false,
                hmargin: 15,
                hmarginGrouped: 5,
                noxaxis: true,
                noyaxis: true,
                title: 'Fruit baskets sold at Mardi Gras 2012',
                key: ['Fructon Road','Lewisham Avenue','Carnival Mar-de-Gras'],
                keyPosition: 'gutter',
                keyTextSize: 12,
                labelsAbove: true,
                ymax: 50
            }
        }).wave();
    
        */
    }
};


var am = new GraficaObtienedatosAM();
//var dam = new dibujagraficaAM();
var dam = new dibujagraficaAMBarras();
var ad = new GraficaObtienedatosAD();
//var dad = new dibujagraficaAD();
var dad = new dibujagraficaADBarras();
var hd = new GraficaObtienedatosHD();
//var dhd = new dibujagraficaHD();
var dhd = new dibujagraficaHDBarras();
var hm = new GraficaObtienedatosHM();
//var dhm = new dibujagraficaHM();
var dhm = new dibujagraficaHMBarras();

$(function () {

    $(window).load(function () {
      
        var currentTime = new Date();
        var year = currentTime.getFullYear();
        var sede = $("#Sedes").val();
        animacioncarga();
      //  var w = hm.getDatos(year, sede);
        var w = hm.getDatosBarras(year, sede);
        // var y = am.getDatos(year, sede);
        var y = am.getDatosBarras(year, sede);
        // var z = ad.getDatos(year, sede);
        var z = ad.getDatosBarras(year, sede);
        //var ww = hd.getDatos(year, sede);
        var ww = hd.getDatosBarras(year, sede);



        $("#recarga_am").click(function () {
            var currentTime = new Date();
            var year = currentTime.getFullYear();
           // alert("AM1");
            //var am = new GraficaObtienedatosAM();
            //var dam = new dibujagraficaAM();
            var sede = $("#Sedes").val();
            RGraph.ObjectRegistry.clear(document.getElementById('cvs_am'));
            //var y = am.getDatos(year, sede);
            var y = am.getDatosBarras(year, sede);
        });

        $("#recarga_ad").click(function () {
            var currentTime = new Date();
            var year = currentTime.getFullYear();
           // alert("AD1");
            //var ad = new GraficaObtienedatosAD();
            //var dad = new dibujagraficaAD();
            var sede = $("#Sedes").val();
            RGraph.ObjectRegistry.clear(document.getElementById('cvs_ad'));
            //var w = ad.getDatos(year, sede);
            var w = ad.getDatosBarras(year, sede);
        });

        $("#recarga_hm").click(function () {
            //  alert("HM1");
            var currentTime = new Date();
            var year = currentTime.getFullYear();
            var hm = new GraficaObtienedatosHM();
           

            var sede = $("#Sedes").val();
            RGraph.ObjectRegistry.clear(document.getElementById('cvs_hm'));
            //var z = hm.getDatos(year, sede);
            var z = hm.getDatosBarras(year, sede);
        });


        $("#recarga_hd").click(function () {
            //alert("HD1");
          //  var hd = new GraficaObtienedatosHD();
          //  var dhd = new dibujagraficaHD();
            var currentTime = new Date();
            var year = currentTime.getFullYear();
            var sede = $("#Sedes").val();
            RGraph.ObjectRegistry.clear(document.getElementById('cvs_hd'));
            //var ww = hd.getDatos(year, sede);
            var ww = hd.getDatosBarras(year, sede);
        });


    });
});//End function jquery

function animacioncarga(){

    //$("[data-click=panel-reload]").click(function(e){e.preventDefault();
        var t=$("[data-click=panel-reload]").closest(".panel");
        if(!$(t).hasClass("panel-loading"))
        {var n=$(t).find(".panel-body");
            var r='<div class="panel-loader"><span class="spinner-small"></span></div>';
            $(t).addClass("panel-loading");
            $(n).prepend(r);

            setTimeout(function(){$(t).removeClass("panel-loading");$(t).find(".panel-loader").remove()},2e3)}
  

}

function exporttableXLS() {
    $("#t1").table2excel({
        exclude: ".noExl",
        name: "Excel Document Name",
        filename: "NominaAnoMes",
        fileext: ".xls",
        exclude_img: true,
        exclude_links: true,
        exclude_inputs: true
    });
}

$("#Sedes").change(function ()
{
    // alert("cambio sede");
    animacioncarga();
    var sede = $("#Sedes").val();
    RGraph.ObjectRegistry.clear(document.getElementById('cvs_hd'));
    RGraph.ObjectRegistry.clear(document.getElementById('cvs_hm'));
    RGraph.ObjectRegistry.clear(document.getElementById('cvs_ad'));
    RGraph.ObjectRegistry.clear(document.getElementById('cvs_am'));
    var currentTime = new Date();
    var year = currentTime.getFullYear();
    //var w = hm.getDatos(year, sede);
    var w = hm.getDatosBarras(year, sede);
    var y = am.getDatos(year, sede);
    var z = ad.getDatos(year, sede);
    var ww = hd.getDatosBarras(year, sede);
});


