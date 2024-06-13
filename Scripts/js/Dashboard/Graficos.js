
function Grafica() {
    this.datos = [];
    this.dibujagrafica = function (datoos) {
        var d = [];
        d = datoos;
        alert("D=" + datoos);
        line = new RGraph.Line({
            id: 'cvs_hm',
            /*
            data: [
                [4, 5, 8, 7, 6, 4, 3, 5, 8, 4, 3, 5],
                [7, 1, 6, 9, 4, 6, 5, 2, 6, 4, 8, 3]
            ],
            */
            data: d,
            options: {
                labels: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
                gutterBottom: 35,
                linewidth: 2,
                shadow: true,
                adjustable: true,
                adjustableOnly: [, , true],
                title: 'An adjustable line chart',
                titleVpos: 0.5,
                spline: true,
                tickmarks: 'circle',
                ticksize: 2
            }
        }).draw();
    };

    this.obtienedatos = function () {

        var anio = 2017;
        var data2 = [];
        $.ajax({
            type: "GET",
            cache: false,
            url: "/Dashboard/GraficaHonorariosMontos/",
            data: "anio=" + anio,
            success: function (data) {
                this.datos = data;
                //alert(data);
                // alert(data2);
                alert(this.datos);
                this.dibujagrafica(this.datos);
            }

        });
        // return data2;
    };


}