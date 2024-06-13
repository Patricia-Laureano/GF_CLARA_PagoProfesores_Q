$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        tinymce.init({
            selector: 'textarea',
            entity_encoding: "raw",
            menu: {
                file: { title: 'File', items: 'newdocument' },
                edit: { title: 'Edit', items: 'undo redo | cut copy paste pastetext | selectall' },
                insert: { title: 'Insert', items: 'link media | template hr' },
                view: { title: 'View', items: 'visualaid' },
                format: { title: 'Format', items: 'bold italic underline strikethrough superscript subscript | formats | removeformat' },
                table: { title: 'Table', items: 'inserttable tableprops deletetable | cell row column' },
                tools: { title: 'Tools', items: 'spellchecker code' },
                campos: { title: 'Campos', items: 'a b c d e f g h i j k l m o p q r' }
            },
            menubar: 'file edit view format table campos',
            setup: function (ed) {

                ed.addButton('btnPDF', {
                    text: 'PDF',
                    icon: false,
                    onclick: function () {
                        var doc = new jsPDF();

                        doc.fromHTML(tinyMCE.activeEditor.getContent(), 15, 15, {
                            'width': 800
                        });

                        // Output as Data URI
                        doc.output('dataurlnewwindow');
                    }
                });

                ed.addMenuItem('a', {
                    text: 'Mes Inicial',
                    context: 'tools',
                    onclick: function() {
                        ed.insertContent('<b>{MES_INICIAL}</b>');
                    }
                });
                ed.addMenuItem('b', {
                    text: 'Mes Final',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{MES_FINAL}</b>');
                    }
                });
                ed.addMenuItem('c', {
                    text: 'Año',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{ANIO}</b>');
                    }
                });
                ed.addMenuItem('d', {
                    text: 'RFC',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{RFC}</b>');
                    }
                });
                ed.addMenuItem('e', {
                    text: 'CURP',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{CURP}</b>');
                    }
                });
                ed.addMenuItem('f', {
                    text: 'Nombre',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NOMBRE}</b>');
                    }
                });
                ed.addMenuItem('g', {
                    text: 'Importe ISR',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{IMPORTE_ISR}</b>');
                    }
                });
                ed.addMenuItem('h', {
                    text: 'Importe IVA',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{IMPORTE_IVA}</b>');
                    }
                });
                ed.addMenuItem('i', {
                    text: 'ISR Retenido',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{ISR_R}</b>');
                    }
                });
                ed.addMenuItem('j', {
                    text: 'IVA Retenido',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{IVA_R}</b>');
                    }
                });
                ed.addMenuItem('k', {
                    text: 'RFC Sociedad',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{RFC_SOCIEDAD}</b>');
                    }
                });
                ed.addMenuItem('l', {
                    text: 'Nombre Sociedad',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NOMBRE_SOCIEDAD}</b>');
                    }
                });
                ed.addMenuItem('m', {
                    text: 'Representante Legal',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{REPRESENTANTE_LEGAL}</b>');
                    }
                });
                ed.addMenuItem('o', {
                    text: 'RFC RL',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{RFC_RL}</b>');
                    }
                });
                ed.addMenuItem('p', {
                    text: 'CURP RL',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{CURP_RL}</b>');
                    }
                });
                ed.addMenuItem('q', {
                    text: 'Firma Retenedor',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{FRIMA_RET}</b>');
                    }
                });
                ed.addMenuItem('r', {
                    text: 'Sello Retenedor',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{SELLO_RET}</b>');
                    }
                });
            },
            plugins: "preview code",
            toolbar: "preview,undo redo | bold italic , alignleft aligncenter alignright | code",
            plugin_preview_width: 700
        });

        formValidation.Inputs(["clave", "contrato"]);
        formValidation.notEmpty('clave', 'El campo clave no debe de estar vacio');
        formValidation.notEmpty('contrato', 'El campo Banco no debe estar vacio');
    });
});//End function jquery


var formPage = function () {
    var idContrato;

    "use strict"; return {
        PDF: function () {

           var model = {
                Formato: escape(tinyMCE.activeEditor.getContent())
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConstanciaRetencion/getHTML/",
                data: model,
                success: function (data) {
                   
                    window.open('/ConstanciaRetencion/ConvertPDF');
                }

            });

        },
        PrintPDF: function (idContrato) {

            var model = {
                Clave: idContrato,
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConstanciaRetencionPreview/getHTML/",
                data: model,
                success: function (data) {

                    window.open('/ConstanciaRetencion/ConvertPDF');
                }

            });

        },
        clean: function () {
            tinyMCE.activeEditor.setContent('');
            formValidation.Clean();
            $("#formbtnadd").show();
            $("#formbtnadd").prop("disabled", false);
            $("#formbtnsave").hide();
            $("#formbtnsave").prop("disabled", true);
            $("#formbtndelete").prop("disabled", true);
            $("#formbtndelete").hide();
            $("#clave").prop("disabled", false);
        },

        edit: function (id) {

            idContrato = id;

            var model = {
                Clave: id
            }

            this.clean();

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConstanciaRetencion/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');
                    $("#clave").val(data.Clave);
                    $("#contrato").val(data.Contrato);
                    tinyMCE.activeEditor.setContent(unescape(data.Formato));
                    $("#clave").prop("disabled", true);
                    $("#formbtnadd").hide();
                    $("#formbtnsave").show();
                    $("#formbtndelete").show();
                    $("#formbtnadd").prop("disabled", true);
                    $("#formbtnsave").prop("disabled", false);
                    $("#formbtndelete").prop("disabled", false);
                }

            });


        },

        save: function () {

            var model = {
                Clave: idContrato,
                Contrato: $("#contrato").val(),
                Formato: escape(tinyMCE.activeEditor.getContent())
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConstanciaRetencion/Save/",
                data: model,
                success: function (data) {
                 //   formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }

            });

        },

        delete: function (confirm) {

            if (!confirm) {
                $('#modal-delete-banco').modal("show");
                return;
            }

            $('#modal-delete-banco').modal("hide");

            var model = {
                Clave: idContrato
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConstanciaRetencion/Delete/",
                data: model,
                success: function (data) {


                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();
                }

            });

        },

        add: function () {

            var model = {
                Clave: $("#clave").val(),
                Contrato: $("#contrato").val(),
                Formato: escape(tinyMCE.activeEditor.getContent())
            }


            if (!formValidation.Validate())
                return;


            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ConstanciaRetencion/Add/",
                data: model,
                success: function (data) {

                    formPage.clean();
                    $('#notification').html(data.msg);
                    DataTable.init();

                }

            });
        }

    }


}();


var DataTable = function () {
    var pag = 1;
    var order = "CVE_RETENCION";
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

            window.location.href = '/ConstanciaRetencion/ExportExcel';

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



            /*$('div.block').block({
                css: { backgroundColor: 'transparent', color: '#336B86', border: "null" },
                overlayCSS: { backgroundColor: '#FFF' },
                message: '<img src="/Content/images/load-search.gif" /><br><h2> Buscando..</h2>'
            });*/



            $.ajax({
                type: "GET",
                cache: false,
                url: "/ConstanciaRetencion/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {

                    //$('div.block').unblock();
                    $("#datatable").html(msg);

                }

            });
        }

    }


}();


