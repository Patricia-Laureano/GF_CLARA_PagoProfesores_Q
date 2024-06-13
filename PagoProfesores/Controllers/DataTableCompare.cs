using ConnectDB;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PagoProfesores.Controllers
{
    public class DataTableCompare
    {
        public string TABLE;
        public string TABLECONDICIONSQL = null;
        public string TABLECONDICIONSQL2 = null;
        public string USUARIO = null;
        public String[] CAMPOSSEARCH;
        public String[] CAMPOS;
        public String[] CAMPOSHIDDEN = { };
        public String[] COLUMNAS;

        public string ESCUELA = null;
        public string PERIODO = null;
        public string SEDE = null;
        public string CAMPUS_INB = null;
        public string PARTEDELPERIODO = null;
        public string OPCIONDEPAGO = null;

        public int[] SHOWVALUES = { 10, 25, 50, 100, 500, 1000, int.MaxValue };
        public Dictionary<string, string> LISTBTNACTIONS;
        private int numreg;
        public int pg;
        public int show;
        public string search;
        public string orderby;
        public string sort;
        public bool btnsaction;
        public string field_id;
        public bool enabledCheckbox = false;
        public bool enabledButtonDelete = false;
        public bool enabledButtonControls = false;
        public bool enabledDoubleClickEditRow = false;
        public bool enablednumRows = false;


        public delegate string ColumnFormat(string value, ResultSet set);
        public Dictionary<string, ColumnFormat> dictColumnFormat;
        public Dictionary<string, string> dictHeaderClass;
        public Dictionary<string, string> dictColumnClass;

        public DataTableCompare()
        {
            this.LISTBTNACTIONS = new Dictionary<string, string>();
            this.dictColumnFormat = new Dictionary<string, ColumnFormat>();
            this.dictHeaderClass = new Dictionary<string, string>();
            this.dictColumnClass = new Dictionary<string, string>();
        }

        public void addBtnActions(string nameButton, string nameFunction)
        {
            this.LISTBTNACTIONS.Add(nameButton, nameFunction);
        }

        public void addColumnFormat(string columnName, ColumnFormat format)
        {
            dictColumnFormat[columnName] = format;
        }

        public void addHeaderClass(string columnName, string _class)
        {
            if (columnName != null)
            {
                if (_class == null)
                    dictHeaderClass.Remove(columnName);
                else
                    dictHeaderClass[columnName] = _class;
            }
        }

        public void addColumnClass(string columnName, string _class)
        {
            if (columnName != null)
            {
                if (_class == null)
                    dictColumnClass.Remove(columnName);
                else
                    dictColumnClass[columnName] = _class;
            }
        }

        public int contadorX = 0;
        public int contadorY = 0;
        List<string> lista_NRCsCambios = new List<string>();
        List<string> lista_IDSIUsCambios = new List<string>();
        List<string> lista_NRCsCambiosAux = new List<string>();
        List<string> lista_IDSIUsCambiosAux = new List<string>();
        string camposModificados = string.Empty;

        public string CreateDataTable(SessionDB sesion, string DataTable = "DataTable")
        {
            ActualizacionPAModel model;
            try
            {
                this.numreg = this.ContRowsTable();
                int numrow = (this.pg - 1) * this.show;

                double numpag = Math.Round(Convert.ToDouble(this.numreg / this.show));
                int denumpag = Convert.ToInt16(numpag + 1);

                StringBuilder sb = new StringBuilder(10000);
                sb.Append("<div>")
                    .Append("<div id=\"data-table_wrapper\" class=\"dataTables_wrapper no-footer\">")
                    .Append("<div class='dataTables_length' id='data-table_length'><label>Mostrar: <select name='data-table_length' onChange='" + DataTable + ".setShow()' id='" + DataTable + "-data-elements-length' aria-controls=\"data-table\">");

                foreach (int showitem in this.SHOWVALUES)
                {
                    var ietmselected = "";
                    if (this.show == showitem) { ietmselected = "selected"; } else { ietmselected = ""; }

                    sb.Append("<option ").Append(ietmselected).Append(" value=\"").Append(showitem).Append("\"> ").Append((showitem == int.MaxValue ? "Todos" : showitem.ToString())).Append(" </option>");
                }
                
                sb.Append("</select> </label></div>")
                    .Append("<div class=\"dataTables_paginate paging_simple_numbers\" style=\"float:left !important; \" id=\"data-table_paginate\"><a class=\"paginate_button previous\" aria-controls=\"data-table\" data-dt-idx=\"0\" ><b><span id='_NumRegX'>").Append(this.numreg).Append("</span></b> Registros</a><a class=\"paginate_button previous\" aria-controls=\"data-table\" data-dt-idx=\"0\" >Registro <b> ").Append(this.pg).Append("</b> /").Append(denumpag).Append("</a></div>");
                
                sb.Append("<div> ").Append(this.getPaged(DataTable)).Append(" </div>");
                //BOTON ELIMINAR
                if (this.enabledButtonDelete)
                {
                    sb.Append("<div  style=\"float:left !important; margin-left:15px; \" id=\"data-table_paginate\"><a class=\"btn btn-sm btn-danger\"  data-dt-idx=\"0\" >Eliminar</a></div>");
                }
                sb.Append("<div id=\"data-table_filter\" style=\"float:right;\" class=\"dataTables_filter\"><img src=\"/Content/images/icon-excel.png\" onClick=\"").Append(DataTable).Append(".exportExcel('").Append(TABLE).Append("');\" style=\"cursor: pointer; \" /><input type=\"search\" style=\"background-color:#D9D9D9 !important\"  id='").Append(DataTable).Append("-searchtable' placeholder=\"Buscar\" value=\"").Append(this.search).Append("\" onkeyup=\"").Append(DataTable).Append(".onkeyup_colfield_check(event)\" aria-controls=\"data-table\"><a class=\"btn-search2\" href=\"javascript:void(0)\" onclick=\"").Append(DataTable).Append(".init();\"  ><i class=\"fa fa-search\"></i></a></div>");
                
                int i = (pg - 1) * show;
                List<String> campostable = new List<String>();
                String htmlcampo = "<th class='";

                //Agregamos las columnas a las tablas
                //------------- Parche ------------
                List<string> list_CAMPOS = new List<string>();
                foreach (string item in CAMPOS)
                    if (CAMPOSHIDDEN.Contains<string>(item) == false)
                        list_CAMPOS.Add(item);
                //---------------------------------
                int x = 0;
                foreach (String columna in this.COLUMNAS)
                {
                    String campotable = htmlcampo;
                    if (list_CAMPOS[x] == orderby) { campotable += "sorting_" + sort.ToLower(); }
                    else { campotable += "sorting"; }

                    // Formato personalizado de los encabezados de la tabla en base a una clase adicional
                    string _class = "";
                    if (dictHeaderClass.ContainsKey(columna))
                    {
                        string format = dictHeaderClass[columna];
                        if (format != null)
                            _class = format;
                    }

                    campotable += " " + _class;
                    campotable += "' id='" + DataTable + "-SORT-" + list_CAMPOS[x] + "' onClick=\"" + DataTable + ".Orderby('" + list_CAMPOS[x] + "')\"";
                    campotable += " data-sort=\"";

                    if (list_CAMPOS[x] == orderby) { campotable += sort; }
                    else { if (sort == "ASC") { campotable += "DESC"; } else { campotable += "ASC"; } }

                    campotable += "\">";
                    campotable += columna.Replace(" ", "&nbsp;") + "</th>";
                    campostable.Add(campotable);
                    x++;
                }

                if (this.enabledButtonControls)
                    campostable.Add("<th class=\"sorting\">&nbsp;</th>");

                sb.Append("<form name=\"formD\" id=\"formD\"><div class=\"outerbox\"><div class=\"innerbox\"><table class=\"bluetable table\" id='" + DataTable + "-fixed' role=\"grid\" aria-describedby=\"data-table_info\">")
                    .Append("<thead>")
                    .Append("<tr role=\"row\">");

                if (this.numreg > 0)
                {
                    if (this.enabledCheckbox)
                        sb.Append("<th class=\"sorting\" tabindex=\"0\" aria-controls=\"data-table\" rowspan=\"1\" colspan=\"1\" style=\"width: 50px;\" ><input type=\"checkbox\" onclick=\"DataTable_ChangeChecked(this,'").Append(DataTable).Append("')\" /></th>");

                    if (this.enablednumRows)
                        sb.Append("<th class=\"sorting\" tabindex=\"0\" aria-controls=\"data-table\" rowspan=\"1\" colspan=\"1\" style=\"width: 50px;\" aria-label=\"Browser: activate to sort column ascending\">No.</th>");

                    foreach (String columna in campostable)
                        sb.Append(columna);
                }

                sb.Append(" </tr>");
                sb.Append(" </thead>");
                sb.Append(" <tbody>");

                if (this.numreg > 0)
                {
                    TABLE = "PA_TMP";

                    ResultSet resX = this.getRowsTable(sesion.db);
                    while (resX.Next())
                    {
                        numrow++;

                        TABLE = "PA";
                        ResultSet res2X = this.getRowsTable2(resX.Get("IDSIU"), resX.Get("NRC"), resX.Get("MATERIA"), resX.Get("CURSO"), sesion.pkUser.ToString(), TABLE);

                        if (res2X.Count > 0) // Signifca que ya existe el registro en PA, va de color amarillo
                        {
                            /**
                             * Inicio: PA BANNER
                             * **/
                            while (res2X.Next())
                            {
                                foreach (string preCampo in this.CAMPOS)
                                {
                                    if (!this.CAMPOSHIDDEN.Contains(preCampo))
                                    {
                                        switch (preCampo)
                                        {
                                            case "FECHAINICIAL":
                                                if (resX.Get("FECHAINICIAL") != res2X.Get("FECHAINICIAL"))
                                                {
                                                    contadorX++;
                                                    contadorY++;
                                                }
                                                break;
                                            case "FECHAFINAL":
                                                if (resX.Get("FECHAFINAL") != res2X.Get("FECHAFINAL"))
                                                {
                                                    contadorX++;
                                                    contadorY++;
                                                }
                                                break;
                                            case "INSCRITOS":
                                                if (resX.Get("INSCRITOS") != res2X.Get("INSCRITOS"))
                                                {
                                                    contadorX++;
                                                    contadorY++;
                                                }
                                                break;
                                            case "HORASSEMANALES":
                                                if (resX.Get("HORASSEMANALES") != res2X.Get("HORASSEMANALES"))
                                                {
                                                    contadorX++;
                                                    contadorY++;
                                                }
                                                break;
                                            case "HORASPROGRAMADAS":
                                                if (resX.Get("HORASPROGRAMADAS") != res2X.Get("HORASPROGRAMADAS"))
                                                {
                                                    contadorX++;
                                                    contadorY++;
                                                }
                                                break;
                                            case "RESPONSABILIDAD":
                                                if (resX.Get("RESPONSABILIDAD") != res2X.Get("RESPONSABILIDAD"))
                                                {
                                                    contadorX++;
                                                    contadorY++;
                                                }
                                                break;
                                            case "HORASAPAGAR":
                                                if (resX.Get("HORASAPAGAR") != res2X.Get("HORASAPAGAR"))
                                                {
                                                    contadorX++;
                                                    contadorY++;
                                                }
                                                break;
                                            case "OPCIONDEPAGO":
                                                if (resX.Get("OPCIONDEPAGO") != res2X.Get("OPCIONDEPAGO"))
                                                {
                                                    contadorX++;
                                                    contadorY++;
                                                }
                                                break;
                                            case "TABULADOR":
                                                if (resX.Get("TABULADOR") != res2X.Get("TABULADOR"))
                                                {
                                                    contadorX++;
                                                    contadorY++;
                                                }
                                                break;
                                            //case "INDICADORDESESION":
                                            //    if (resX.Get("INDICADORDESESION") != res2X.Get("INDICADORDESESION"))
                                            //    {
                                            //        contadorX++;
                                            //        contadorY++;
                                            //    }
                                            //    break;
                                            default:
                                                ;
                                                break;
                                        }
                                    }
                                }

                                if (contadorY > 0)
                                {
                                    lista_IDSIUsCambiosAux.Add(resX.Get("IDSIU"));
                                    lista_NRCsCambiosAux.Add(resX.Get("NRC"));
                                }
                            }
                            contadorY = 0;
                        }
                    }

                    if (lista_IDSIUsCambiosAux.Count > 0 && lista_NRCsCambiosAux.Count > 0)
                    {
                        lista_IDSIUsCambios = lista_IDSIUsCambiosAux.Distinct().ToList();
                        lista_NRCsCambios = lista_NRCsCambiosAux.Distinct().ToList();
                    }

                    TABLE = "PA_TMP";
                    ResultSet res = this.getRowsTable(sesion.db);
                    while (res.Next())
                    {
                        numrow++;

                        TABLE = "PA";
                        ResultSet res2 = this.getRowsTable2(res.Get("IDSIU"), res.Get("NRC"), res.Get("MATERIA"), res.Get("CURSO"), sesion.pkUser.ToString(), TABLE);

                        if (res2.Count > 0) // Signifca que ya existe el registro en PA, va de color amarillo
                        {
                            /**
                             * Inicio: PA BANNER
                             * **/
                            if (contadorX > 0)
                            {
                                while (res2.Next())
                                {
                                    foreach (string _idSiu in lista_IDSIUsCambios)
                                    {
                                        if (res2.Get("IDSIU") == _idSiu)
                                        {
                                            foreach (string _nrc in lista_NRCsCambios)
                                            {
                                                if (res2.Get("NRC") == _nrc)
                                                {
                                                    camposModificados = "";

                                                    sb.Append("<tr class=\"gradeA odd\" role=\"row\" ondblclick=\"").Append(DataTable).Append(".edit('").Append(res.Get(this.field_id)).Append("')\">");
                                                    if (this.enabledCheckbox)
                                                        sb.Append("<td style='background-color: #F4FFAC;' class=\"sorting_1\"><input type=\"checkbox\" id=\"").Append(res.Get(this.field_id)).Append("\" value=\"").Append(res.Get(this.field_id)).Append("\" /></td>");

                                                    if (this.enablednumRows)
                                                        sb.Append("<td style='background-color: #F4FFAC;' class=\"sorting_1\">").Append(numrow).Append("</td>");

                                                    string css_class = "";
                                                    foreach (string campo in this.CAMPOS)
                                                    {
                                                        if (!this.CAMPOSHIDDEN.Contains(campo))
                                                        {
                                                            var value = res.Get(campo);

                                                            // Formato personalizado.
                                                            if (dictColumnFormat.ContainsKey(campo))
                                                            {
                                                                ColumnFormat format = dictColumnFormat[campo];
                                                                if (format != null)
                                                                    value = format.Invoke(value, res);
                                                            }

                                                            if (dictColumnClass.ContainsKey(campo))
                                                            {
                                                                css_class = " " + dictColumnClass[campo];
                                                            }
                                                            else
                                                                css_class = string.Empty;
                                                            
                                                            if (campo == "IP")
                                                            {
                                                                sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'> Actualización de datos </td>");
                                                            }
                                                            else if (campo == "FECHAINICIAL") //validación del campo FECHAINICIAL =)
                                                            {
                                                                if (res.Get("FECHAINICIAL") == res2.Get("FECHAINICIAL")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "1,";
                                                                    sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "FECHAFINAL")
                                                            {
                                                                if (res.Get("FECHAFINAL") == res2.Get("FECHAFINAL")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "2,";
                                                                    sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "INSCRITOS")
                                                            {
                                                                if (res.Get("INSCRITOS") == res2.Get("INSCRITOS")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "3,";
                                                                    sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "HORASSEMANALES")
                                                            {
                                                                if (res.Get("HORASSEMANALES") == res2.Get("HORASSEMANALES")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "4,";
                                                                    sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "HORASPROGRAMADAS")
                                                            {
                                                                if (res.Get("HORASPROGRAMADAS") == res2.Get("HORASPROGRAMADAS")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "5,";
                                                                    sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "RESPONSABILIDAD")
                                                            {
                                                                if (res.Get("RESPONSABILIDAD") == res2.Get("RESPONSABILIDAD")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "6,";
                                                                    sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "HORASAPAGAR")
                                                            {
                                                                if (res.Get("HORASAPAGAR") == res2.Get("HORASAPAGAR")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "7,";
                                                                    sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "OPCIONDEPAGO")
                                                            {
                                                                if (res.Get("OPCIONDEPAGO") == res2.Get("OPCIONDEPAGO")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "8,";
                                                                    sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "TABULADOR")
                                                            {
                                                                if (res.Get("TABULADOR") == res2.Get("TABULADOR")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "9,";
                                                                    sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            //else if (campo == "INDICADORDESESION")
                                                            //{
                                                            //    if (res.Get("INDICADORDESESION") == res2.Get("INDICADORDESESION")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                            //    {
                                                            //        sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                            //    }
                                                            //    else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                            //    {
                                                            //        camposModificados += "10,";
                                                            //        sb.Append("<td style='background-color: #A1FF9A;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                            //    }
                                                            //}
                                                            else
                                                            {
                                                                sb.Append("<td style='background-color: #FDF692;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                            }
                                                        }
                                                    }

                                                    //ENABLED BOTON DE ACCIONES
                                                    if (this.enabledButtonControls)
                                                    {
                                                        sb.Append("<td style='background-color: #FDF692;' class=\"sorting_1\"><div class=\"btn-group m-r-5 m-b-5\"><a class=\"btn btn-warning dropdown-toggle\" data-toggle=\"dropdown\" href=\"javascript:;\" aria-expanded=\"false\">Acción <span class=\"caret\"></span></a><ul class=\"dropdown-menu\">");
                                                        foreach (KeyValuePair<string, string> pair in LISTBTNACTIONS)
                                                        {
                                                            sb.Append("<li><a href =\"javascript:").Append(pair.Value).Append("(").Append(res.Get(this.field_id)).Append(");\"> ").Append(pair.Key).Append(" </a></li>");
                                                        }
                                                        sb.Append("</ul></div></td>");
                                                    }

                                                    sb.Append("</tr>");
                                                    /**
                                                        * Fin: PA BANNER
                                                        * **/

                                                    model = new ActualizacionPAModel();
                                                    model.IdSiuX = res.Get("IDSIU").ToString();
                                                    model.NrcX = res.Get("NRC").ToString();
                                                    model.MateriaX = res.Get("MATERIA").ToString();
                                                    model.CursoX = res.Get("CURSO").ToString();
                                                    model.EstatusX = "Actualización de datos";
                                                    model.TableX = "PA_TMP";
                                                    model.TipoActX = "3";
                                                    model.CamposMX = camposModificados;
                                                    model.UsuarioX = sesion.pkUser.ToString();
                                                    model.insertaPAExcelTMP();

                                                    /**
                                                    * Inicio: PA PAGOPROFESORES
                                                    **/

                                                    camposModificados = "";

                                                    sb.Append("<tr class=\"gradeA odd\" role=\"row\" ondblclick=\"").Append(DataTable).Append(".edit('").Append(res2.Get(this.field_id)).Append("')\">");

                                                    if (this.enabledCheckbox)
                                                        sb.Append("<td style='background-color: #F4FFAC;' class=\"sorting_1\"><input type=\"checkbox\" id=\"").Append(res2.Get(this.field_id)).Append("\" value=\"").Append(res2.Get(this.field_id)).Append("\" /></td>");

                                                    if (this.enablednumRows)
                                                        sb.Append("<td style='background-color: #F4FFAC;' class=\"sorting_1\">").Append(numrow).Append("</td>");

                                                    string css_class2 = "";
                                                    foreach (string campo in this.CAMPOS)
                                                    {
                                                        if (!this.CAMPOSHIDDEN.Contains(campo))
                                                        {

                                                            var value = res2.Get(campo);

                                                            // Formato personalizado.
                                                            if (dictColumnFormat.ContainsKey(campo))
                                                            {
                                                                ColumnFormat format = dictColumnFormat[campo];
                                                                if (format != null)
                                                                    value = format.Invoke(value, res2);
                                                            }
                                                            if (dictColumnClass.ContainsKey(campo))
                                                            {
                                                                css_class2 = " " + dictColumnClass[campo];
                                                            }
                                                            else
                                                                css_class2 = string.Empty;

                                                            if (campo == "IP")
                                                            {
                                                                sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'></td>");
                                                            }
                                                            else if (campo == "FECHAINICIAL") //validación del campo FECHAINICIAL =)
                                                            {
                                                                if (res.Get("FECHAINICIAL") == res2.Get("FECHAINICIAL")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "1,";
                                                                    sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "FECHAFINAL")
                                                            {
                                                                if (res.Get("FECHAFINAL") == res2.Get("FECHAFINAL")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "2,";
                                                                    sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "INSCRITOS")
                                                            {
                                                                if (res.Get("INSCRITOS") == res2.Get("INSCRITOS")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "3,";
                                                                    sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "HORASSEMANALES")
                                                            {
                                                                if (res.Get("HORASSEMANALES") == res2.Get("HORASSEMANALES")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "4,";
                                                                    sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "HORASPROGRAMADAS")
                                                            {
                                                                if (res.Get("HORASPROGRAMADAS") == res2.Get("HORASPROGRAMADAS")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "5,";
                                                                    sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "RESPONSABILIDAD")
                                                            {
                                                                if (res.Get("RESPONSABILIDAD") == res2.Get("RESPONSABILIDAD")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "6,";
                                                                    sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "HORASAPAGAR")
                                                            {
                                                                if (res.Get("HORASAPAGAR") == res2.Get("HORASAPAGAR")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "7,";
                                                                    sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "OPCIONDEPAGO")
                                                            {
                                                                if (res.Get("OPCIONDEPAGO") == res2.Get("OPCIONDEPAGO")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "8,";
                                                                    sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            else if (campo == "TABULADOR")
                                                            {
                                                                if (res.Get("TABULADOR") == res2.Get("TABULADOR")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                                {
                                                                    sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                                else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                                {
                                                                    camposModificados += "9,";
                                                                    sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                }
                                                            }
                                                            //else if (campo == "INDICADORDESESION")
                                                            //{
                                                            //    if (res.Get("INDICADORDESESION") == res2.Get("INDICADORDESESION")) // si el campo a comparar es igual, entonces se pinta normalmente ;)
                                                            //    {
                                                            //        sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                            //    }
                                                            //    else // si el campo a comparar no es igual, entonces resaltamos la diferecia con un color rojo ¬¬
                                                            //    {
                                                            //        camposModificados += "10,";
                                                            //        sb.Append("<td style='background-color: #FFC1C1;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                            //    }
                                                            //}
                                                            else
                                                            {
                                                                sb.Append("<td style='background-color: #F4FFAC;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                            }
                                                        }
                                                    }

                                                    //ENABLED BOTON DE ACCIONES
                                                    if (this.enabledButtonControls)
                                                    {
                                                        sb.Append("<td style='background-color: #F4FFAC;' class=\"sorting_1\"><div class=\"btn-group m-r-5 m-b-5\"><a class=\"btn btn-warning dropdown-toggle\" data-toggle=\"dropdown\" href=\"javascript:;\" aria-expanded=\"false\">Acción <span class=\"caret\"></span></a><ul class=\"dropdown-menu\">");
                                                        foreach (KeyValuePair<string, string> pair in LISTBTNACTIONS)
                                                        {
                                                            sb.Append("<li><a href =\"javascript:").Append(pair.Value).Append("(").Append(res2.Get(this.field_id)).Append(");\"> ").Append(pair.Key).Append(" </a></li>");
                                                        }
                                                        sb.Append("</ul></div></td>");
                                                    }

                                                    sb.Append("</tr>");
                                                    
                                                    // en este bloque se registrara la información en la tabla temporal PAEXCEL_TMP
                                                    model = new ActualizacionPAModel();
                                                    model.IdSiuX = res2.Get("IDSIU").ToString();
                                                    model.NrcX = res2.Get("NRC").ToString();
                                                    model.MateriaX = res2.Get("MATERIA").ToString();
                                                    model.CursoX = res2.Get("CURSO").ToString();
                                                    model.EstatusX = "";
                                                    model.TableX = "PA";
                                                    model.TipoActX = "3";
                                                    model.CamposMX = camposModificados;
                                                    model.UsuarioX = sesion.pkUser.ToString();
                                                    model.insertaPAExcelTMP();
                                                    // fin de PAEXCEL_TMP

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            /**
                             * Fin: PA BANNER
                             * **/
                        }
                    }
                }
                else
                {
                    sb.Append("<tr class=\"gradeA odd\" role=\"row\"><td colspan=\"").Append((this.COLUMNAS.Length + 2)).Append("\">")
                    .Append("<div class=\"jumbotron m-b-0 text-center\" style=\"width:100%; \">")
                    .Append("<h1>No existen registros</h1>")
                    .Append("<p>No hay regristros con diferencias.</p>")
                    .Append("</div>")
                    .Append("</td>")
                    .Append("</tr><input type='hidden' id='").Append(DataTable).Append("_data' value='empty'>");
                }

                sb.Append(" </tbody>")
                .Append(" </table></div></div></form>")
                .Append("</div>")
                .Append("</div>");
                
                return sb.ToString();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /*
        private int ContRowsTable()
        {
            string first = CAMPOS[0];
            String sql = "SELECT COUNT(" + first + ") AS 'MAX' FROM " + TABLE + " ";

            if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
                sql += " WHERE " + TABLECONDICIONSQL;

            if (search != null && search != "")
            {
                if (CAMPOSSEARCH == null || CAMPOSSEARCH.Length == 0)
                    CAMPOSSEARCH = CAMPOS;

                string last = CAMPOSSEARCH[CAMPOSSEARCH.Length - 1];

                if (TABLECONDICIONSQL == null || TABLECONDICIONSQL == "") { sql += " WHERE ("; }
                else { sql += " AND ("; }

                foreach (string campo in CAMPOSSEARCH)
                {
                    sql += "(" + campo + " LIKE '%" + search + "%')";
                    if (last != campo) { sql += " OR "; }
                }
                sql += ") ";
            }

            database db = new database();

            return db.Count(sql);
        }
        */

        private int ContRowsTable()
        {
            string first = CAMPOS[0];
            String sql = "";
            //sql = "SELECT(select COUNT(1)                                                                 " +
            //      "         from PA_TMP T join PA P on P.CVE_SEDE        = T.CVE_SEDE                     " +
            //      "                                and P.CAMPUS_INB      = T.CAMPUS_INB                   " +
            //      "                                and P.PERIODO         = T.PERIODO                      " +
            //      "                                and P.PARTEDELPERIODO = T.PARTEDELPERIODO              " +
            //      "                                and P.NRC             = T.NRC                          " +
            //      "                                and P.MATERIA         = T.MATERIA                      " +
            //      "                                and P.CURSO           = T.CURSO                        " +
            //      "                                and P.IDSIU           = T.IDSIU                        " +
            //      "                                and (P.FECHAINICIAL      != T.FECHAINICIAL     or      " +
            //      "                                     P.FECHAFINAL        != T.FECHAFINAL       or      " +
            //      "                                     P.INSCRITOS         != T.INSCRITOS        or      " +
            //      "                                     P.HORASSEMANALES    != T.HORASSEMANALES   or      " +
            //      "                                     P.HORASPROGRAMADAS  != T.HORASPROGRAMADAS or      " +
            //      "                                     P.RESPONSABILIDAD   != T.RESPONSABILIDAD  or      " +
            //      "                                     P.HORASAPAGAR       != T.HORASAPAGAR      or      " +
            //      "                                     P.CVE_OPCIONDEPAGO  != T.CVE_OPCIONDEPAGO or      " +
            //      "                                     P.TABULADOR         != T.TABULADOR        or      " +
            //      "                                     P.INDICADORDESESION != T.INDICADORDESESION)       " +
            //      "        where T.USUARIO = " + USUARIO + ")                                             " +
            //      "      +                                                                                " +
            //      "      (select(select count(1)                                                          " +
            //      "                from PA_TMP                                                            " +
            //      "               where USUARIO = " + USUARIO + ")                                        " +
            //      "             -                                                                         " +
            //      "             (select count(1)                                                          " +
            //      "                from PA P join PA_TMP PT on P.IDSIU             = PT.IDSIU             " +
            //      "                                        and P.CAMPUS_INB        = PT.CAMPUS_INB        " +
            //      "                                        and P.CVE_ESCUELA       = PT.CVE_ESCUELA       " +
            //      "                                        and P.CVE_SEDE          = PT.CVE_SEDE          " +
            //      "                                        and P.PERIODO           = PT.PERIODO           " +
            //      "                                        and P.CVE_TIPODEDOCENTE = PT.CVE_TIPODEDOCENTE " +
            //      "                                        and P.PARTEDELPERIODO   = PT.PARTEDELPERIODO   " +
            //      "                                        and P.NRC               = PT.NRC               " +
            //      "                                        and P.MATERIA           = PT.MATERIA           " +
            //      "                                        and P.CURSO             = PT.CURSO             " +
            //      "               where PT.USUARIO = " + USUARIO + ")) AS 'MAX'                           ";

            //if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
            //    sql += " WHERE " + TABLECONDICIONSQL;

            /*sql = "SELECT(select COUNT(1)                                                                 " +
                  "         from PA_TMP T join PA P on P.CVE_SEDE        = T.CVE_SEDE                     " +
                  "                                and P.CAMPUS_INB      = T.CAMPUS_INB                   " +
                  "                                and P.PERIODO         = T.PERIODO                      " +
                  "                                and P.PARTEDELPERIODO = T.PARTEDELPERIODO              " +
                  "                                and P.NRC             = T.NRC                          " +
                  "                                and P.MATERIA         = T.MATERIA                      " +
                  "                                and P.CURSO           = T.CURSO                        " +
                  "                                and P.IDSIU           = T.IDSIU                        " +
                  "                                and (P.FECHAINICIAL      != T.FECHAINICIAL     or      " +
                  "                                     P.FECHAFINAL        != T.FECHAFINAL       or      " +
                  "                                     P.INSCRITOS         != T.INSCRITOS        or      " +
                  "                                     P.HORASSEMANALES    != T.HORASSEMANALES   or      " +
                  "                                     P.HORASPROGRAMADAS  != T.HORASPROGRAMADAS or      " +
                  "                                     P.RESPONSABILIDAD   != T.RESPONSABILIDAD  or      " +
                  "                                     P.HORASAPAGAR       != T.HORASAPAGAR      or      " +
                  "                                     P.CVE_OPCIONDEPAGO  != T.CVE_OPCIONDEPAGO or      " +
                  "                                     P.TABULADOR         != T.TABULADOR        or      " +
                  "                                     P.INDICADORDESESION != T.INDICADORDESESION)       " +
                  "        where T.USUARIO = " + USUARIO +
                  "          and (T.CVE_ESCUELA      = " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " or " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " is null)  " +
                  "          and T.PERIODO           = '" + PERIODO + "' " +
                  "          and T.CVE_SEDE          = '" + SEDE + "' " +
                  "          and (T.CAMPUS_INB       = " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " or " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " is null)  " +
                  "          and (T.PARTEDELPERIODO  = " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " or " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " is null)  " +
                  "          and (T.CVE_OPCIONDEPAGO = " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " or " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " is null)) " +
                  "      +                                                                                " +
                  "      (select COUNT(1)                                                                 " +
                  "         from PA_TMP T join PA P on P.CVE_SEDE           = T.CVE_SEDE                  " +
                  "                                and P.CAMPUS_INB         = T.CAMPUS_INB                " +
                  "                                and P.PERIODO            = T.PERIODO                   " +
                  "                                and P.PARTEDELPERIODO    = T.PARTEDELPERIODO           " +
                  "                                and P.NRC                = T.NRC                       " +
                  "                                and P.MATERIA            = T.MATERIA                   " +
                  "                                and P.CURSO              = T.CURSO                     " +
                  "                                and P.INDICADORDESESION != T.INDICADORDESESION         " +
                  "                                and P.IDSIU             != T.IDSIU                     " +
                  "        where T.USUARIO = " + USUARIO +
                  "          and (T.CVE_ESCUELA      = " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " or " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " is null)  " +
                  "          and T.PERIODO           = '" + PERIODO + "' " +
                  "          and T.CVE_SEDE          = '" + SEDE + "' " +
                  "          and (T.CAMPUS_INB       = " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " or " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " is null)  " +
                  "          and (T.PARTEDELPERIODO  = " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " or " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " is null)  " +
                  "          and (T.CVE_OPCIONDEPAGO = " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " or " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " is null)) " +
                  "      +                                                                                " +
                  "      (select(select count(1)                                                          " +
                  "                from PA                                                                " +
                  "               where (CVE_ESCUELA      = " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " or " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " is null)  " +
                  "                 and PERIODO           = '" + PERIODO + "' " +
                  "                 and CVE_SEDE          = '" + SEDE + "' " +
                  "                 and (CAMPUS_INB       = " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " or " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " is null)  " +
                  "                 and (PARTEDELPERIODO  = " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " or " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " is null)  " +
                  "                 and (CVE_OPCIONDEPAGO = " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " or " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " is null)) " +
                  "             -                                                                         " +
                  "             (select count(1)                                                          " +
                  "                from PA_TMP                                                            " +
                  "               where USUARIO           = " + USUARIO  +
                  "                 and (CVE_ESCUELA      = " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " or " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " is null)  " +
                  "                 and PERIODO           = '" + PERIODO + "' " +
                  "                 and CVE_SEDE          = '" + SEDE + "' " +
                  "                 and (CAMPUS_INB       = " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " or " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " is null)  " +
                  "                 and (PARTEDELPERIODO  = " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " or " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " is null)  " +
                  "                 and (CVE_OPCIONDEPAGO = " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " or " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " is null))) AS 'MAX' ";*/

            sql = "       select COUNT(1) AS MAX                                                          " +
                  "         from PA_TMP T join PA P on P.CVE_SEDE        = T.CVE_SEDE                     " +
                  "                                and P.CAMPUS_INB      = T.CAMPUS_INB                   " +
                  "                                and P.PERIODO         = T.PERIODO                      " +
                  "                                and P.PARTEDELPERIODO = T.PARTEDELPERIODO              " +
                  "                                and P.NRC             = T.NRC                          " +
                  "                                and P.MATERIA         = T.MATERIA                      " +
                  "                                and P.CURSO           = T.CURSO                        " +
                  "                                and P.IDSIU           = T.IDSIU                        " +
                  "                                and (P.FECHAINICIAL      != T.FECHAINICIAL     or      " +
                  "                                     P.FECHAFINAL        != T.FECHAFINAL       or      " +
                  "                                     P.INSCRITOS         != T.INSCRITOS        or      " +
                  "                                     P.HORASSEMANALES    != T.HORASSEMANALES   or      " +
                  "                                     P.HORASPROGRAMADAS  != T.HORASPROGRAMADAS or      " +
                  "                                     P.RESPONSABILIDAD   != T.RESPONSABILIDAD  or      " +
                  "                                     P.HORASAPAGAR       != T.HORASAPAGAR      or      " +
                  "                                     P.CVE_OPCIONDEPAGO  != T.CVE_OPCIONDEPAGO or      " +
                  "                                     P.TABULADOR         != T.TABULADOR        or      " +
                  "                                     P.INDICADORDESESION != T.INDICADORDESESION)       " +
                  "        where T.USUARIO           = " + USUARIO +
                  "          and (T.CVE_ESCUELA      = " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " or " + ((ESCUELA == null || ESCUELA == "") ? "null" : "'" + ESCUELA + "'") + " is null)  " +
                  "          and T.PERIODO           = '" + PERIODO + "' " +
                  "          and T.CVE_SEDE          = '" + SEDE + "' " +
                  "          and (T.CAMPUS_INB       = " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " or " + ((CAMPUS_INB == null || CAMPUS_INB == "") ? "null" : "'" + CAMPUS_INB + "'") + " is null)  " +
                  "          and (T.PARTEDELPERIODO  = " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " or " + ((PARTEDELPERIODO == null || PARTEDELPERIODO == "") ? "null" : "'" + PARTEDELPERIODO + "'") + " is null)  " +
                  "          and (T.CVE_OPCIONDEPAGO = " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " or " + ((OPCIONDEPAGO == null || OPCIONDEPAGO == "") ? "null" : "'" + OPCIONDEPAGO + "'") + " is null) ";

            database db = new database();
            return db.Count(sql);
        }

        private ResultSet getRowsTable(database db)
        {
            string last = CAMPOS[CAMPOS.Length - 1];
            string campos = "";

            foreach (string campo in CAMPOS)
            {
                if (TABLE == "PA")
                {
                    if (campo != "REGISTRADO") {
                        campos += campo;
                        if (last != campo) { campos += " , "; }
                    }
                }
                else {
                    campos += campo;
                    if (last != campo) { campos += " , "; }
                }
            }

            string sql = "SELECT " + campos + " FROM " + TABLE + " ";

            if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
                sql += " WHERE " + TABLECONDICIONSQL;
            
            if (search != "" && search != null)
            {
                if (CAMPOSSEARCH == null) { CAMPOSSEARCH = CAMPOS; }
                else
                {
                    if (CAMPOSSEARCH.Length == 0) { CAMPOSSEARCH = CAMPOS; }
                }

                last = CAMPOSSEARCH[CAMPOSSEARCH.Length - 1];

                if (TABLECONDICIONSQL == "" || TABLECONDICIONSQL == null) { sql += " WHERE ("; } else { sql += " AND ("; }

                foreach (string campo in CAMPOSSEARCH)
                {
                    sql += "(" + campo + " LIKE '%" + search + "%')";
                    if (last != campo) { sql += " OR "; }
                }
                sql += ") ";
            }

            sql += " ORDER BY " + orderby + " " + sort;
            //sql += " OFFSET (" + (pg - 1) * show + ") ROWS"; // -- not sure if you
                                                             // need -1
            //sql += " FETCH NEXT " + show + " ROWS ONLY";

            return db.getTable(sql);
        }

        private ResultSet getRowsTableB(database db)
        {
            string last = CAMPOS[CAMPOS.Length - 1];
            string campos = "";

            foreach (string campo in CAMPOS)
            {
                if (TABLE == "PA")
                {
                    if (campo != "REGISTRADO")
                    {
                        campos += campo;
                        if (last != campo) { campos += " , "; }
                    }
                }
                else
                {
                    campos += campo;
                    if (last != campo) { campos += " , "; }
                }
            }

            string sql = "SELECT " + campos + " FROM " + TABLE + " ";

            if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
                sql += " WHERE " + TABLECONDICIONSQL + " AND " + TABLECONDICIONSQL2;

            if (search != "" && search != null)
            {
                if (CAMPOSSEARCH == null) { CAMPOSSEARCH = CAMPOS; }
                else
                {
                    if (CAMPOSSEARCH.Length == 0) { CAMPOSSEARCH = CAMPOS; }
                }

                last = CAMPOSSEARCH[CAMPOSSEARCH.Length - 1];

                if (TABLECONDICIONSQL == "" || TABLECONDICIONSQL == null) { sql += " WHERE ("; } else { sql += " AND ("; }

                foreach (string campo in CAMPOSSEARCH)
                {
                    sql += "(" + campo + " LIKE '%" + search + "%')";
                    if (last != campo) { sql += " OR "; }
                }
                sql += ") ";
            }

            sql += " ORDER BY " + orderby + " " + sort;
            //sql += " OFFSET (" + (pg - 1) * show + ") ROWS"; // -- not sure if you
            // need -1
            //sql += " FETCH NEXT " + show + " ROWS ONLY";

            return db.getTable(sql);
        }

        private ResultSet getRowsTable2(string idsiu, string nrc, string materia, string curso, string usuario, string table)
        {
            String sql = "SELECT null as REGISTRADO, * FROM PA ";

            if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
                sql += " WHERE ";

            sql += " IDSIU       = '" + idsiu + "'";
            sql += " AND NRC     = '" + nrc + "'";
            sql += " AND MATERIA = '" + materia + "'";
            sql += " AND CURSO   = '" + curso + "'";
            //sql += " AND USUARIO = '" + usuario + "'";
            sql += " ORDER BY NRC ASC ";

            database db = new database();

            return db.getTable(sql);
        }

        private ResultSet getRowsTable2B(string idsiu, string nrc, string materia, string curso, string usuario, string indicadorSesion, string table)
        {
            String sql = "SELECT null as REGISTRADO, * FROM PA ";

            if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
                sql += " WHERE ";

            sql += " IDSIU                 != '" + idsiu           + "'";
            sql += " AND INDICADORDESESION != '" + indicadorSesion + "'";
            sql += " AND NRC                = '" + nrc             + "'";
            sql += " AND MATERIA            = '" + materia         + "'";
            sql += " AND CURSO              = '" + curso           + "'";
            sql += " AND USUARIO            = '" + usuario         + "'";
            sql += " ORDER BY NRC ASC ";

            database db = new database();

            return db.getTable(sql);
        }

        private ResultSet getRowsTable3(string idsiu, string nrc, string materia, string curso, string usuario, string escuela, string table)
        {
            string sql = "SELECT null as REGISTRADO, ID_PA   ,CVE_SEDE      ,CAMPUS_INB      ,PERIODO      ,CVE_ESCUELA      ,ESCUELA      ,NRC      ,MATERIA      ,CURSO      ,NOMBREMATERIA      ,FECHAINICIAL      ,FECHAFINAL      ,TIPODECURSO      ,METODODEINSTRUCCION      ,STATUS      ,INSCRITOS      ,PARTEDELPERIODODESC      ,PARTEDELPERIODO      ,IDSIU      ,APELLIDOS      ,NOMBRE      ,RFC      ,CURP      ,CVE_TIPODEDOCENTE      ,TIPODEDOCENTE      ,MAXIMOGRADOACADEMICO      ,HORASSEMANALES      ,HORASPROGRAMADAS      ,RESPONSABILIDAD      ,HORASAPAGAR      ,LOGINADMINISTRATIVO      ,TABULADOR_MONTO      ,CVE_OPCIONDEPAGO      ,OPCIONDEPAGO      ,TABULADOR      ,INDICADORDESESION      ,ID_PERSONA      ,ID_CENTRODECOSTOS      ,ID_ESQUEMA      ,FECHA_PA      ,FECHA_NOMINA      ,CVE_NIVEL      ,NIVEL     ,MONTOAPAGAR      ,FECHAINICIO      ,FECHAFIN      ,NOSEMANAS      ,NOPAGOS      ,BLOQUEOCONTRATO      ,FECHA_R      ,FECHA_M      ,IP      ,USUARIO            ,ELIMINADO      ,INDICADOR FROM " + table;
            //String sql = "SELECT * FROM PA_TMP ";

            if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
                sql += " WHERE ";

            sql += " IDSIU           = '" + idsiu   + "'";
            sql += " AND NRC         = '" + nrc     + "'";
            sql += " AND MATERIA     = '" + materia + "'";
            sql += " AND CURSO       = '" + curso   + "'";
            sql += " AND USUARIO     = '" + usuario + "'";
            sql += " AND CVE_ESCUELA = '" + escuela + "'";
            //sql += " ORDER BY NRC ASC ";

            database db = new database();

            return db.getTable(sql);
        }

        private string getPaged(string DataTable = "DataTable")
        {
            //double _numpag = Math.Round(this.numreg / (double)this.show);
            //int denumpag = Convert.ToInt16(_numpag + 1);
            int denumpag = (int)Math.Ceiling(this.numreg / (double)this.show);

            String paginado = "<div class=\"dataTables_paginate paging_simple_numbers\" style=\"float:left !important; \" id=\"data-table_paginate\">";

            if (this.pg > 1)
            {
                int pagante = this.pg - 1;
                paginado += "<a class=\"paginate_button previous\" href=\"javascript:void(0)\" onclick=\"" + DataTable + ".setPage("
                        + pagante
                        + ");\" aria-controls=\"data-table\" data-dt-idx=\"0\" tabindex=\"0\" id=\"data-table_previous\"><i class=\"fa fa-arrow-left\" style=\"color:#FF8000\"></i> Anterior</a>";
            }
            else
            {
                paginado += "<a class=\"paginate_button previous disabled\" aria-controls=\"data-table\" data-dt-idx=\"0\" tabindex=\"0\" id=\"data-table_previous\"><i class=\"fa fa-arrow-left\" style=\"color:#FF8000\"></i> Anterior</a>";
            }

            paginado += "<span>";

            // Calcular el inicio del arreglo
            int inipg = 0;
            int r = (this.pg - 1) % 5;
            int sumpag = 0;

            if (r == 0)
            {
                inipg = this.pg - 1;
            }
            else
            {
                inipg = ((this.pg - 1) / 5) * 5;
            }

            for (int j = inipg; j < 5 + inipg; j++)
            {
                //if (j < _numpag + 1)
                if (j < denumpag)
                {
                    sumpag = j + 1;

                    if (sumpag == this.pg)
                    {
                        paginado += "<a class=\"paginate_button current\" href=\"javascript:void(0)\" style=\"border-color:#FF8000 !important\" onclick=\"" + DataTable + ".setPage("
                                + sumpag
                                + ");\" aria-controls=\"data-table\" data-dt-idx=\"2\" tabindex=\"0\">"
                                + sumpag + "</a>";
                    }
                    else
                    {
                        paginado += "<a class=\"paginate_button\" href=\"javascript:void(0)\" style=\"border-color:#FF8000 !important\" onclick=\"" + DataTable + ".setPage("
                                + sumpag
                                + ");\" aria-controls=\"data-table\" data-dt-idx=\"2\" tabindex=\"0\">"
                                + sumpag + "</a>";
                    }
                }
            }

            paginado += "</span>";

            //if (this.pg <= _numpag)
            if (this.pg < denumpag)
            {
                int numeropag = this.pg + 1;

                paginado += "<a class=\"paginate_button next\" href=\"javascript:void(0)\" onclick=\"" + DataTable + ".setPage("
                        + numeropag
                        + ");\" aria-controls=\"data-table\" data-dt-idx=\"2\" tabindex=\"0\" id=\"data-table_next\">Siguiente <i class=\"fa fa-arrow-right\" style=\"color:#FF8000\" ></i></a>";
            }
            else
            {
                paginado += "<a class=\"paginate_button next disabled\" aria-controls=\"data-table\" data-dt-idx=\"2\" tabindex=\"0\" id=\"data-table_next\">Siguiente <i class=\"fa fa-arrow-right\" style=\"color:#FF8000\"></i></a>";
            }

            paginado += "</div>";

            return paginado;
        }


    }//<end class>
}