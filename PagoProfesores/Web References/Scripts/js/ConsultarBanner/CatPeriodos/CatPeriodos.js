$(function () {
    $(window).load(function () {
        //formPage.clean();
    });
});

var formPage = function () {
    "use strict"; return {


        Disponible: function (ids) {

            var ids_array = ids.split("-");

            var idciclo = ids_array[0].trim();
            var idperiodo = ids_array[1].trim();
            var disponible = 0;

            if ($("#" + ids).is(":checked"))
                disponible = 1;
            else
                disponible = 0;

            var model = {
                DISPONIBLE: disponible,
                CVE_CICLO: idciclo,
                PERIODO: idperiodo
            }


            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/CatPeriodos/setDisponible/",
                data: model,
                success: function (data) {

                    $('#notification').html(data.msg);
                    var ntype = $('#NotificationType').val();

                    if (ntype == "SUCCESS") {
                      //  $.gritter.add({ title: "Disponible", text: "Se ha realizado la operación con exito" });

                    }


                    DataTable.init();

                },
                error: function (msg) {

                }

            });


        },



        consultar: function () {
            var data = 'Ciclo=' + $('#Ciclo').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Transfiriendo datos de BANNER..');
            loading('loading-circle', '#seccion1', 'Consultando BANNER..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/CatPeriodos/Consultar",
                data: data,
                success: function (msg) {
                    if (session_error(msg) == false) {
                        if (msg == "ok")
                            DataTable.setPage(1);
                    }
                    $('#seccion1').unblock();
                },
                error: function () {
                    session_error(data);
                    $('#seccion1').unblock();
                }
            });
        },

        importar: function () {
            var _ciclo = $('#Ciclo').val();
            

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

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Actualizando datos..');
            loading('loading-circle', '#seccion1', 'Importando Años Escolares desde BANNER..');


            $.ajax({
                type: "POST",
                cache: false,
                url: "/CatPeriodos/Importar",
                data: "periodos=" + arrChecked.join() + "&cveCiclo=" + _ciclo,
                success: function (msg) {

                    $('#seccion1').unblock();
                    DataTable.init();

                    $('#notification').html(msg);
                    //console.log(msg);
                },
                error: function (msg) {

                    $('#seccion1').unblock();
                    $('#notification').html(msg);
                    //console.log(msg);
                }
            });
        },

    }
}();


var DataTable = function () {
    var pag = 1;
    var order = "PERIODO";
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
                this.setPage(1);
            }
        },

        exportExcel: function (table) {
            window.location.href = '/CatPeriodos/ExportExcel';
        },

        setShow: function () {
            this.setPage(1);
        },

        edit: function (id) {
            //formPage.edit(id);
        },

        setPage: function (page) {
            pag = page;
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
            var ciclo = $('#Ciclo').val();

            loading('loading-bar');
            loading('loading-circle', '#datatable', 'Consultando datos..');

            $.ajax({
                type: "GET",
                cache: false,
                url: "/CatPeriodos/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null" + "&ciclo=" + ciclo,
                success: function (msg) {
                    $('.loader-min-bar').hide();
                    if (session_error(msg) == false) {
                        $("#datatable").html(msg);
                    }
                },
                error: function (msg) {
                    $('.loader-min-bar').hide();
                    session_error(msg);
                }
            });
        }

    }

}();