
function GraficaObtienedatosHM() {
    //function Grafica() {

    this.getDatos = function (anio,sede) {

        //var anio = 2017;
        var data2 = [];
        var arr;
        //Pace.ignore(function()
       // {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Dashboard/GraficaHonorariosMontos/",
            data: "anio=" + anio+"&sede="+sede,
            success: function (data) {
                data2 = data;
               // alert("ajax=" + data2);
                arr = JSON.parse(data2);
                // alert("json=" + arr);
                dhm.borra();
                dhm.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert(msg); }
        });
       // });
        return arr;
    }
};

function GraficaObtienedatosAM() {
    //function Grafica() {

    this.getDatos = function (anio, sede) {

        //var anio = 2017;
        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Dashboard/GraficaAsimiladosMontos/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
                // alert("ajax=" + data2);
                arr = JSON.parse(data2);
                // alert("json=" + arr);
                dam.borra();
                dam.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert(msg); }
        });
        // });
        return arr;
    }
};



function GraficaObtienedatosAD() {
    //function Grafica() {

    this.getDatos = function (anio, sede) {

        //var anio = 2017;
        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Dashboard/GraficaAsimiladosDocentes/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
                // alert("ajax=" + data2);
                arr = JSON.parse(data2);
                // alert("json=" + arr);
                dad.borra();
                dad.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert(msg); }
        });
        // });
        return arr;
    }
};
function GraficaObtienedatosHD() {
    //function Grafica() {

    this.getDatos = function (anio, sede) {

        //var anio = 2017;
        var data2 = [];
        var arr;
        //Pace.ignore(function()
        // {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Dashboard/GraficaHonorariosDocentes/",
            data: "anio=" + anio + "&sede=" + sede,
            success: function (data) {
                data2 = data;
                // alert("ajax=" + data2);
                arr = JSON.parse(data2);
                // alert("json=" + arr);
                dhd.borra();
                dhd.dibuja(arr);
            }
            ,
            error: function (msg) { session_error(msg); alert(msg); }
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
                colors: ['red', 'green', 'blue'],
                key: ['Deposito', 'Pagos Pendientes', 'Recibos/Disper'],
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
                   [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
            ];
        }
        var a = datos[0];
        var b = datos[1];
        var c = datos[2];     
        var d = a.concat(b, c);
       
        line = new RGraph.Line({
            id: 'cvs_am',

            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['red', 'green', 'blue'],
                key: ['Deposito', 'Pagos Pendientes', 'Recibos/Disper'],
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
                colors: ['red', 'green', 'blue'],
                key: ['Deposito', 'Pagos Pendientes', 'Recibos/Disper'],
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
        if (datos == null || datos.length == 0)
        {
            datos = [
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
            id: 'cvs_hd',

            data: datos,
            options: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                colors: ['red', 'green', 'blue'],
                key: ['Deposito', 'Pagos Pendientes', 'Recibos/Disper'],
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
var hm = new GraficaObtienedatosHM();
var dhm = new dibujagraficaHM();
var am = new GraficaObtienedatosAM();
var dam = new dibujagraficaAM();
var ad = new GraficaObtienedatosAD();
var dad = new dibujagraficaAD();
var hd = new GraficaObtienedatosHD();
var dhd = new dibujagraficaHD();


$(function () {

    $(window).load(function () {
      
        
        var sede = $("#Sedes").val();
        var w = hm.getDatos(2017, sede);
        var y = am.getDatos(2017, sede);
        var z = ad.getDatos(2017, sede);
        var ww = hd.getDatos(2017, sede);
    });
});//End function jquery



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
    var sede = $("#Sedes").val();
    RGraph.ObjectRegistry.clear(document.getElementById('cvs_hd'));
    RGraph.ObjectRegistry.clear(document.getElementById('cvs_hm'));
    RGraph.ObjectRegistry.clear(document.getElementById('cvs_ad'));
    RGraph.ObjectRegistry.clear(document.getElementById('cvs_am'));
    
    var w = hm.getDatos(2017, sede);
    var y = am.getDatos(2017, sede);
    var z = ad.getDatos(2017, sede);
    var ww= hd.getDatos(2017, sede);
});


