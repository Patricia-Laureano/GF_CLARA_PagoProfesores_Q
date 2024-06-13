$(function () {

    $(window).load(function () {

        $("#formbtnadd").html('Guardar');
        $("#formbtnsave").hide();
        $("#formbtndelete").hide();

        tinymce.init({
            selector: 'textarea',
            content_css: 'Content/css/print.css',
            valid_children : '+body[style]',
            entity_encoding: "raw",
            menu: {
                file: { title: 'File', items: 'newdocument' },
                edit: { title: 'Edit', items: 'undo redo | cut copy paste pastetext | selectall' },
                insert: { title: 'Insert', items: 'link media | template hr' },
                view: { title: 'View', items: 'visualaid' },
                format: { title: 'Format', items: 'bold italic underline strikethrough superscript subscript | formats | removeformat' },
                table: { title: 'Table', items: 'inserttable tableprops deletetable | cell row column' },
                tools: { title: 'Tools', items: 'spellchecker code' },
                campos: { title: 'Campos', items: '1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38' }
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

                ed.addMenuItem('1', {
                    text: 'SOCIEDAD',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{SOCIEDAD}</b>');
                    }
                });
                ed.addMenuItem('2', {
                    text: 'REPRESENTANTE LEGAL',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{REPRESENTANTE_LEGAL}</b>');
                    }
                });
                ed.addMenuItem('3', {
                    text: 'NOMBRE PROFESOR',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NOMBRE_PROFESOR}</b>');
                    }
                });
                ed.addMenuItem('4', {
                    text: 'NOTARIAL NUMERO',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NOTARIAL_NO}</b>');
                    }
                });
                ed.addMenuItem('5', {
                    text: 'NOTARIAL VOLUMEN',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NOTARIAL_VOLUMEN}</b>');
                    }
                });
                ed.addMenuItem('6', {
                    text: 'NOTARIO NUMERO',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NOTARIAL_NOTARIO_NO}</b>');
                    }
                });
                ed.addMenuItem('7', {
                    text: 'NOTARIAL LUGAR',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NOTARIAL_LUGAR}</b>');
                    }
                });
                ed.addMenuItem('8', {
                    text: 'NOTARIAL LIC',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NOTARIAL_LIC}</b>');
                    }
                });
                ed.addMenuItem('9', {
                    text: 'TABLA MATERIAS',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{TABLA_MATERIAS}</b>');
                    }
                });
                ed.addMenuItem('10', {
                    text: 'DOMICILIO',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{DOMICILIO}</b>');
                    }
                });
                ed.addMenuItem('11', {
                    text: 'NACIONALIDAD',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NACIONALIDAD}</b>');
                    }
                });
                ed.addMenuItem('12', {
                    text: 'FECHA NACIMIENTO',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{FECHA_NAC}</b>');
                    }
                });
                ed.addMenuItem('13', {
                    text: 'CALLE',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{DIRECCION_CALLE}</b>');
                    }
                });
                ed.addMenuItem('14', {
                    text: 'COLONIA',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{COLONIA}</b>');
                    }
                });
                ed.addMenuItem('15', {
                    text: 'DELEGACION',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{DELEGACION}</b>');
                    }
                });
                ed.addMenuItem('16', {
                    text: 'CP',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{CP}</b>');
                    }
                });
                ed.addMenuItem('17', {
                    text: 'CIUDAD',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{CIUDAD}</b>');
                    }
                });
                ed.addMenuItem('18', {
                    text: 'TITULO PROFESIONAL',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{TITULO_PROFESIONAL}</b>');
                    }
                });
                ed.addMenuItem('19', {
                    text: 'PROFESION',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{PROFESION}</b>');
                    }
                });
                ed.addMenuItem('20', {
                    text: 'CEDULA',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{CEDULA}</b>');
                    }
                });
                ed.addMenuItem('21', {
                    text: 'FECHA CEDULA',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{FECHA_CEDULA}</b>');
                    }
                });
                ed.addMenuItem('22', {
                    text: 'NUMERO SS',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NO_SS}</b>');
                    }
                });
                ed.addMenuItem('23', {
                    text: 'INSTITUTO',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{INSTITUTO}</b>');
                    }
                });
                ed.addMenuItem('24', {
                    text: 'RFC',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{RFC}</b>');
                    }
                });
                ed.addMenuItem('25', {
                    text: 'PERSONA MORAL',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{PERSONA_MORAL}</b>');
                    }
                });
                ed.addMenuItem('26', {
                    text: 'CURP',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{CURP}</b>');
                    }
                });
                ed.addMenuItem('27', {
                    text: 'MONTO',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{MONTO}</b>');
                    }
                });
                ed.addMenuItem('28', {
                    text: 'MONTO LETRA',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{MONTO_LETRA}</b>');
                    }
                });
                ed.addMenuItem('29', {
                    text: 'NUMERO PAGOS',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{NO_PAGOS}</b>');
                    }
                });
                ed.addMenuItem('30', {
                    text: 'MOTO TABULADOR',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{MOTO_TABULADOR}</b>');
                    }
                });
                ed.addMenuItem('31', {
                    text: 'VIGENTE PARA',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{VIGENTE_PARA}</b>');
                    }
                });
                ed.addMenuItem('32', {
                    text: 'FIRMA EN',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{FIRMA_EN}</b>');
                    }
                });
                ed.addMenuItem('33', {
                    text: 'FIRMA LUGAR',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{FIRMA_LUGAR}</b>');
                    }
                });
                ed.addMenuItem('34', {
                    text: 'FECHA DIA',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{FECHA_DIA}</b>');
                    }
                });
                ed.addMenuItem('35', {
                    text: 'FECHA MES',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{FECHA_MES}</b>');
                    }
                });
                ed.addMenuItem('36', {
                    text: 'FECHA AÑO',
                    context: 'tools',
                    onclick: function () {
                        ed.insertContent('<b>{FECHA_AÑO}</b>');
                    }
                });


                
            },
            plugins: "preview code,textcolor",
            toolbar: "preview,undo redo | bold italic , alignleft aligncenter alignright | code |forecolor backcolor",
            plugin_preview_width: 700
        });


        //tinymce.activeEditor.dom.setStyle(tinymce.activeEditor.dom.select('p'), 'page-break-inside', 'avoid');
        // Sets the HTML contents of the activeEditor editor
        tinymce.activeEditor.setContent('<style>p{page-break-inside: avoid;}</style>');

        formValidation.Inputs(["clave", "contrato", "descripcion"]);
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
                url: "/ContratosWeb/getHTML/",
                data: model,
                success: function (data) {

                    window.open('/ContratosWeb/ConvertPDF');
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
                url: "/ContratosWebPreview/getHTML/",
                data: model,
                success: function (data) {

                    window.open('/ContratosWeb/ConvertPDF');
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
                url: "/ContratosWeb/Edit/",
                data: model,
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    $('html, body').animate({ scrollTop: 0 }, 'fast');
                    $("#clave").val(data.Clave);
                    $("#contrato").val(data.Contrato);
                    $("#descripcion").val(data.Descripcion);
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
                Descripcion: $("#descripcion").val(),
                Formato: escape(tinyMCE.activeEditor.getContent())
            }

            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ContratosWeb/Save/",
                data: model,
                success: function (data) {

                   // formPage.clean();
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
                url: "/ContratosWeb/Delete/",
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
                Descripcion: $("#descripcion").val(),
                Formato: escape(tinyMCE.activeEditor.getContent())
            }


            if (!formValidation.Validate())
                return;


            $.ajax({
                type: "POST",
                dataType: 'json',
                cache: false,
                url: "/ContratosWeb/Add/",
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
    var order = "Cve_Contrato";
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

            window.location.href = '/ContratosWeb/ExportExcel';

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
                url: "/ContratosWeb/CreateDataTable/",
                data: "pg=" + pag + "&show=" + show + "&search=" + search + "&orderby=" + orderby + "&sort=" + sorter + "&sesion=null",
                success: function (msg) {

                    //$('div.block').unblock();
                    $("#datatable").html(msg);

                }

            });
        }

    }


}();


