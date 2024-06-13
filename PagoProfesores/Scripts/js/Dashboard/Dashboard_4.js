function handlerdataSedes() { }

function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
};

function getDatosAMBarrasTotal(anio, sede, mes) {
    var data2 = [];
    var arr = 0;

    $.ajax({
        type: "GET",
        async: false,
        timeout: 50000,
        url: "/Dashboard/GraficaAMBarrasTotal/",
        data: "anio=" + anio + "&sede=" + sede + "&mes=" + mes,
        success: function (data) {
            data2 = data;
            arr = JSON.parse(data);
        },
        error: function (msg) { alert("Error:" + JSON.stringify(msg, null, 2)); }
    });
    return arr;
};

function getDatosAMBarrasPagPend(anio, sede, mes) {
    var data2 = [];
    var arr = 0;

    $.ajax({
        type: "GET",
        async: false,
        timeout: 50000,
        url: "/Dashboard/GraficaAMBarrasPagPend/",
        data: "anio=" + anio + "&sede=" + sede + "&mes=" + mes,
        success: function (data) {
            data2 = data;
            arr = JSON.parse(data);
        },
        error: function (msg) { alert("Error:" + JSON.stringify(msg, null, 2)); }
    });
    return arr;
};

function getDatosAMBarrasDepo(anio, sede, mes) {
    var data2 = [];
    var arr = 0;

    $.ajax({
        type: "GET",
        async: false,
        timeout: 50000,
        url: "/Dashboard/GraficaAMBarrasDeposito/",
        data: "anio=" + anio + "&sede=" + sede + "&mes=" + mes,
        success: function (data) {
            data2 = data;
            arr = JSON.parse(data);
        },
        error: function (msg) { alert("Error:" + JSON.stringify(msg, null, 2)); }
    });
    return arr;
};

function getDatosAMBarrasRecibos(anio, sede, mes) {
    var data2 = [];
    var arr = 0;

    $.ajax({
        type: "GET",
        async: false,
        timeout: 50000,
        url: "/Dashboard/GraficaAMBarrasRecibos/",
        data: "anio=" + anio + "&sede=" + sede + "&mes=" + mes,
        success: function (data) {
            data2 = data;
            arr = JSON.parse(data);
        },
        error: function (msg) { alert("Error:" + JSON.stringify(msg, null, 2)); }
    });
    return arr;
};

function getDatosAMBarrasFull(anio,sede){
    var data2 = [];
    var arr = 0;

    $.ajax({
        type: "GET",
        async: false,
        timeout: 50000,
        url: "/Dashboard/GraficaAMBarrasFull/",
        data: "anio=" + anio + "&sede=" + sede ,
        success: function (data) {
            data2 = data;
            arr = JSON.parse(data);
        },
        error: function (msg) { alert("Error:" + JSON.stringify(msg, null, 2)); }
    });
    return arr;
};

function getDatosADBarrasFull(anio, sede) {
    var data2 = [];
    var arr = 0;

    $.ajax({
        type: "GET",
        async: false,
        timeout: 50000,
        url: "/Dashboard/GraficaADBarrasFull/",
        data: "anio=" + anio + "&sede=" + sede,
        success: function (data) {
            data2 = data;
            arr = JSON.parse(data);
        },
        error: function (msg) { alert("Error:" + JSON.stringify(msg, null, 2)); }
    });
    return arr;
};

function getDatosHMBarrasFull(anio, sede) {
    var data2 = [];
    var arr = 0;

    $.ajax({
        type: "GET",
        async: false,
        timeout: 50000,
        url: "/Dashboard/GraficaHMBarrasFull/",
        data: "anio=" + anio + "&sede=" + sede,
        success: function (data) {
            data2 = data;
            arr = JSON.parse(data);
        },
        error: function (msg) { alert("Error:" + JSON.stringify(msg, null, 2)); }
    });
    return arr;
};

function getDatosHDBarrasFull(anio, sede) {
    var data2 = [];
    var arr = 0;

    $.ajax({
        type: "GET",
        async: false,
        timeout: 50000,
        url: "/Dashboard/GraficaHDBarrasFull/",
        data: "anio=" + anio + "&sede=" + sede,
        success: function (data) {
            data2 = data;
            arr = JSON.parse(data);
        },
        error: function (msg) { alert("Error:" + JSON.stringify(msg, null, 2)); }
    });
    return arr;
};

function dibujaAM() {
    var sede = document.getElementById("Sedes").value;
   
    var currentTime = new Date();
    var year = currentTime.getFullYear();
    var dat = getDatosAMBarrasFull(year, sede);
   
    var ctx = document.getElementById("cvs_am").getContext('2d');
    var myChartAM = new Chart(ctx, {
        type: 'bar',
        responsive: true,
        data: {
            labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            datasets: [{
                label: ['Total'],             
                data: dat[0],
                backgroundColor: "#5B9BD5"
            }]
        },
        options: {
            animation: false
        }
    });

    myChartAM.data.datasets.push({
        label: 'Recibos/Disper',
        data: dat[1],
        backgroundColor: "#ED7D31"
    });
    myChartAM.update();

    myChartAM.data.datasets.push({
        label: 'Deposito',
        data: dat[2],
        backgroundColor: "#A5A5A5"
    });
    myChartAM.update();

    myChartAM.data.datasets.push({
        label: 'Pagos Pendientes',
        data: dat[3],
        backgroundColor: "#FFC000"
    });
    myChartAM.update();
};

function dibujaAD() {
    var sede = document.getElementById("Sedes").value;

    var currentTime = new Date();
    var year = currentTime.getFullYear();
    var dat = getDatosADBarrasFull(year, sede);

    var ctx = document.getElementById("cvs_ad").getContext('2d');
    var myChartAD = new Chart(ctx, {
        type: 'bar',
        responsive: true,
        data: {
            labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            datasets: [{
                label: ['Total'],
                data: dat[0],
                backgroundColor: "#5B9BD5"
            }]
        },
        options: {
            animation: false
        }
    });

    myChartAD.data.datasets.push({
        label: 'Recibos/Disper',
        data: dat[1],
        backgroundColor: "#ED7D31"
    });
    myChartAD.update();

    myChartAD.data.datasets.push({
        label: 'Deposito',
        data: dat[2],
        backgroundColor: "#A5A5A5"
    });
    myChartAD.update();

    myChartAD.data.datasets.push({
        label: 'Pagos Pendientes',
        data: dat[3],
        backgroundColor: "#FFC000"
    });
    myChartAD.update();
};

function dibujaHM() {
    var sede = document.getElementById("Sedes").value;

    var currentTime = new Date();
    var year = currentTime.getFullYear();
    var dat = getDatosHMBarrasFull(year, sede);

    var ctx = document.getElementById("cvs_hm").getContext('2d');
    var myChartHM = new Chart(ctx, {
        type: 'bar',
        responsive: true,
        data: {
            labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            datasets: [{
                label: ['Total'],
                data: dat[0],
                backgroundColor: "#5B9BD5"
            }]
        },
        options: {
            animation: false
        }
    });

    myChartHM.data.datasets.push({
        label: 'Recibos/Disper',
        data: dat[1],
        backgroundColor: "#ED7D31"
    });
    myChartHM.update();

    myChartHM.data.datasets.push({
        label: 'Deposito',
        data: dat[2],
        backgroundColor: "#A5A5A5"
    });
    myChartHM.update();

    myChartHM.data.datasets.push({
        label: 'Pagos Pendientes',
        data: dat[3],
        backgroundColor: "#FFC000"
    });
    myChartHM.update();
};

function dibujaHD() {
    var sede = document.getElementById("Sedes").value;

    var currentTime = new Date();
    var year = currentTime.getFullYear();
    var dat = getDatosHDBarrasFull(year, sede);

    var ctx = document.getElementById("cvs_hd").getContext('2d');
    var myChartHD = new Chart(ctx, {
        type: 'bar',
        responsive: true,
        data: {
            labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            datasets: [{
                label: ['Total'],
                data: dat[0],
                backgroundColor: "#5B9BD5"
            }]
        },
        options: {
            animation: false
        }
    });

    myChartHD.data.datasets.push({
        label: 'Recibos/Disper',
        data: dat[1],
        backgroundColor: "#ED7D31"
    });
    myChartHD.update();

    myChartHD.data.datasets.push({
        label: 'Deposito',
        data: dat[2],
        backgroundColor: "#A5A5A5"
    });
    myChartHD.update();

    myChartHD.data.datasets.push({
        label: 'Pagos Pendientes',
        data: dat[3],
        backgroundColor: "#FFC000"
    });
    myChartHD.update();
};

$(function () {
    $(window).load(function () {

        var currentTime = new Date();
        var year = currentTime.getFullYear();
        var sede = $("#Sedes").val();
     
        var am = new dibujaAM();
        var ad = new dibujaAD();
        var hm = new dibujaHM();
        var hd = new dibujaHD();

        $("#recarga_am").click(function () {
            var model = {
                Campus: $('#Sedes').val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Dashboard/GraficaBarrasFull_AM/",
                data: model,
                success: function (data) {
                    var currentTime = new Date();
                    var year = currentTime.getFullYear();
                    var sede = $("#Sedes").val();
                    var am = new dibujaAM();
                }
            });
        });

        $("#recarga_ad").click(function () {
            var model = {
                Campus: $('#Sedes').val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Dashboard/GraficaBarrasFull_AD/",
                data: model,
                success: function (data) {
                    var currentTime = new Date();
                    var year = currentTime.getFullYear();
                    var sede = $("#Sedes").val();
                    var ad = new dibujaAD();
                },

                error: function (data) {
                    session_error(data);
                }
            });
        });

        $("#recarga_hm").click(function () {
            var model = {
                Campus: $('#Sedes').val()
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Dashboard/GraficaBarrasFull_HM/",
                data: model,
                success: function (data) {
                    var currentTime = new Date();
                    var year = currentTime.getFullYear();
                    var sede = $("#Sedes").val();
                    var hm = new dibujaHM();
                }
            });
        });
        
        $("#recarga_hd").click(function () {

            var model = {
                Campus: $('#Sedes').val()
            }
            
            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/Dashboard/GraficaBarrasFull_HD/",
                data: model,
                success: function (data) {
                    var currentTime = new Date();
                    var year = currentTime.getFullYear();
                    var sede = $("#Sedes").val();
                    var hd = new dibujaHD();
                }
            });
        });
    });
});//End function jquery

function animacioncarga() {
    var t = $("[data-click=panel-reload]").closest(".panel");
    if (!$(t).hasClass("panel-loading"))
    {
        var n = $(t).find(".panel-body");
        var r = '<div class="panel-loader"><span class="spinner-small"></span></div>';
        $(t).addClass("panel-loading");
        $(n).prepend(r);

        setTimeout(function () { $(t).removeClass("panel-loading"); $(t).find(".panel-loader").remove() }, 2e3)
    }
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
    animacioncarga();
    var sede = $("#Sedes").val();
    var am = new dibujaAM();
    var ad = new dibujaAD();
    var hm = new dibujaHM();
    var hd = new dibujaHD();
});