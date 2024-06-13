using System;
using System.Collections.Generic;
using System.Linq;
using ConnectDB;
using Session;
using System.Text;
using System.Globalization;


namespace PagoProfesores.Controllers
{
    //Alex Baca: Copia de la Clase DataTable de Factory.cs
    public class DataTablePlus
    {
        public string TABLE;
        public string TABLED;
        public string TABLECONDICIONSQL = null;
        public string TABLECONDICIONSQLD = null;
        public String[] CAMPOSSEARCH;
        public String[] CAMPOS;
        public String[] CAMPOSHIDDEN = { };
        public String[] COLUMNAS;

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
        public string field_id2;
        public bool enabledCheckbox = false;
        public bool enabledButtonDelete = false;
        public bool enabledButtonControls = false;
        public bool enabledDoubleClickEditRow = false;
        public bool enablednumRows = false;

        public delegate string ColumnFormat(string value, ResultSet set);
        public Dictionary<string, ColumnFormat> dictColumnFormat;
        public Dictionary<string, string> dictHeaderClass;
        public Dictionary<string, string> dictColumnClass;

        public DataTablePlus()
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

        ResultSet res2;
        ResultSet res3;

        public string CreateDataTable(SessionDB sesion, string DataTable = "DataTable", bool editable = true)
        {
            this.numreg = this.ContRowsTable2();
            int numrow = (this.pg - 1) * this.show;

            //double numpag = Math.Round(Convert.ToDouble(this.numreg / this.show)); //AB =)
            double numpag = Math.Round(Convert.ToDouble(this.numreg));
            //int denumpag = Convert.ToInt16(numpag + 1); //AB =)

            StringBuilder sb = new StringBuilder(10000);
            sb.Append("<div>")
                .Append("<div id=\"data-table_wrapper\" class=\"dataTables_wrapper no-footer\">")
                //.Append("<div class='dataTables_length' id='data-table_length'><label>Mostrar: <select name='data-table_length' onChange='" + DataTable + ".setShow()' id='" + DataTable + "-data-elements-length' aria-controls=\"data-table\">"); //AB =)
                //.Append("<div class='dataTables_length' id='data-table_length'>");//AB =)

                //foreach (int showitem in this.SHOWVALUES)  //AB =)
                //{
                //    var ietmselected = "";
                //    if (this.show == showitem) { ietmselected = "selected"; } else { ietmselected = ""; }

                //    sb.Append("<option ").Append(ietmselected).Append(" value=\"").Append(showitem).Append("\"> ").Append((showitem == int.MaxValue ? "Todos" : showitem.ToString())).Append(" </option>");
                //}

                //sb.Append("</select> </label></div>") //AB =)
                //sb.Append("</div>")//AB =)
                //.Append("<div class=\"dataTables_paginate paging_simple_numbers\" style=\"float:left !important; \" id=\"data-table_paginate\"><a class=\"paginate_button previous\" aria-controls=\"data-table\" data-dt-idx=\"0\" ><b>").Append(this.numreg).Append("</b> Registros</a><a class=\"paginate_button previous\" aria-controls=\"data-table\" data-dt-idx=\"0\" >Registro <b> ").Append(this.pg).Append("</b> /").Append(denumpag).Append("</a></div>"); //AB =)
                .Append("<div class=\"row\"><div class=\"dataTables_paginate paging_simple_numbers\" style=\"float:left !important; \" id=\"data-table_paginate\"><a class=\"paginate_button previous\" aria-controls=\"data-table\" data-dt-idx=\"0\" ><b>").Append(this.numreg).Append("</b> Registros</a></div><div class=\"col-md-2\"><b>Pago Retención </b><span style=\"text-decoration:line-through; color:red;\"> Concepto </span></div><div class=\"col-md-2\"><b>Pago Publicado </b><span class=\"fa fa-eye\"></span></div><div class=\"col-md-2\"><b>Contrato Entregado </b><span class=\"fa fa-file-text-o\"></span></div><div class=\"col-md-2\"><b>Pago Depositado </b><span class=\"fa fa-credit-card\"></span></div>");//AB =)
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         //.Append("<div style=\"float:left !important; \" id=\"data-table_paginate\"><a aria-controls=\"data-table\" data-dt-idx=\"0\" ><b>").Append(this.numreg).Append("</b> Registros</a></div>");

            //sb.Append("<div> ").Append(this.getPaged2(DataTable)).Append(" </div>");//AB =)
            //BOTON ELIMINAR
            //if (this.enabledButtonDelete) //AB =)
            //{
            //    sb.Append("<div  style=\"float:left !important; margin-left:15px; \" id=\"data-table_paginate\"><a class=\"btn btn-sm btn-danger\"  data-dt-idx=\"0\" >Eliminar</a></div>");
            //}
            sb.Append("<div id=\"data-table_filter\" style=\"float:right;\" class=\"dataTables_filter\"><img src=\"/Content/images/icon-excel.png\" onClick=\"").Append(DataTable).Append(".exportExcel('").Append(TABLE).Append("');\" style=\"cursor: pointer; \" /><input type=\"search\" style=\"background-color:#D9D9D9 !important\"  id='").Append(DataTable).Append("-searchtable' placeholder=\"Buscar\" value=\"").Append(this.search).Append("\" onkeyup=\"").Append(DataTable).Append(".onkeyup_colfield_check(event)\" aria-controls=\"data-table\"><a class=\"btn-search2\" href=\"javascript:void(0)\" onclick=\"").Append(DataTable).Append(".init();\"  ><i class=\"fa fa-search\"></i></a></div></div>");

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

            sb.Append("<form name=\"formD\" id=\"formD\"><div class=\"outerbox\"><div class=\"innerbox\" ><table class=\"table\" id='" + DataTable + "-fixed' role=\"grid\" aria-describedby=\"data-table_info\">")
                .Append("<thead>")
                .Append("<tr role=\"row\">");

            if (this.numreg > 0)
            {
                if (this.enabledCheckbox) // =)  ***/***
                {
                    sb.Append("<th class=\"sorting\" tabindex=\"0\" aria-controls=\"data-table\" rowspan=\"1\" colspan=\"1\" style=\"width: 50px;\" ><input type=\"checkbox\" onclick=\"DataTable_ChangeChecked(this,'").Append(DataTable).Append("')\" /></th>");
                }
                //if (this.enablednumRows) //AB =)
                //{
                //    sb.Append("<th class=\"sorting\" tabindex=\"0\" aria-controls=\"data-table\" rowspan=\"1\" colspan=\"1\" style=\"width: 50px;\" aria-label=\"Browser: activate to sort column ascending\">No.</th>");
                //}
                foreach (String columna in campostable)
                {
                    sb.Append(columna);
                }
            }
            sb.Append(" </tr>");
            sb.Append(" </thead>");
            sb.Append(" <tbody>");

            List<string> listIds = new List<string>();
          

            if (this.numreg > 0)
            {

                ResultSet res = this.getRowsTable2(sesion.db, 1, "*, 'EC' AS ID_TABLE");

                Dictionary<string, string> list_fpago = cargaListasfechaPago(sesion.db);

                while (res.Next())
                {
                    if (res.Get("PADRE") == "0")
                    {
                        numrow++;
                        sb.Append("<tr data-row='1' data-count='0' id='PadreTR_").Append(res.Get("ID_ESTADODECUENTA")).Append("' class=\"rowcellblue\" role=\"row\" ondblclick=\"").Append(DataTable).Append(".editPago('").Append(res.Get("ID_ESTADODECUENTA")).Append("')\">");
                        if (this.enabledCheckbox)
                        {
                            sb.Append("<td class=\"sorting_1\"><input type=\"checkbox\" id=\"").Append("P_").Append(res.Get(this.field_id)).Append("\" value=\"").Append(res.Get(this.field_id)).Append("\" /></td>");
                        }
                        listIds.Add(res.Get(this.field_id));

                        //if (this.enablednumRows) //AB =)
                        //{
                        //    sb.Append("<td class=\"sorting_1\">").Append(numrow).Append("</td>");
                        //}
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

                                if (campo != "CVE_SEDE")
                                {
                                    if (campo == "CONCEPTO")
                                    {
                                        var value1 = res.Get("BLOQUEOCONTRATO");
                                        var value2 = res.Get("BLOQUEOS");
                                        var value3 = res.Get("FECHADEENTREGA");


                                        string fechapago = mesanio_pago(list_fpago, res.Get("FECHAPAGO"));


                                        if ((value1 == "True") && (value3 == null || value3 == ""))
                                        {
                                            if (list_fpago.Keys.Contains(fechapago))
                                                sb.Append("<td  style='text-decoration: line-through; color: red; background: ").Append(list_fpago[fechapago]).Append(";' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                            else
                                                sb.Append("<td style='text-decoration: line-through; color: red;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");

                                        }
                                        else if (Int32.Parse(value2) > 0)
                                        {
                                            if (list_fpago.Keys.Contains(fechapago)) 
                                                sb.Append("<td  style='text-decoration: line-through; color: red; background:").Append(list_fpago[fechapago]).Append(";' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                            else
                                                sb.Append("<td style='text-decoration: line-through; color: red;' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                        }
                                        else
                                        {//
                                            if(list_fpago.Keys.Contains(fechapago))
                                                sb.Append("<td  style='background: ").Append(list_fpago[fechapago]).Append(";' class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                            else
                                               sb.Append("<td  class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                                                                        
                                        }
                                    }
                                    else if (campo == "MONTO" || campo == "MONTO_IVA" || campo == "MONTO_IVARET" || campo == "MONTO_ISRRET" || campo == "BANCOS")
                                    {
                                        if(campo == "MONTO_ISRRET")
                                            sb.Append("<td class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                        else
                                            sb.Append("<td class='sorting_1").Append(css_class).Append("'>").Append(Double.Parse(value).ToString("C", CultureInfo.CreateSpecificCulture("es-MX"))).Append("</td>");
                                        
                                       
                                    }
                                    else
                                    {
                                        sb.Append("<td class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                                    }
                                }
                                else
                                {
                                    sb.Append("<td id='Padre_").Append(res.Get("ID_ESTADODECUENTA")).Append("' data-activo='1' style='cursor:pointer;' class='sorting_1").Append(css_class).Append("' onclick='javascript: _toggleEstadoCuentaDetalle(this);'><span id='icon_").Append(res.Get("ID_ESTADODECUENTA")).Append("' class='fa fa-minus-circle'>&nbsp;</span>").Append(value).Append("</td>"); // Despliega los acumulados de estado de cuenta
                                }
                            }
                        }

                        //ENABLEB BOTON DE ACCIONES
                        //if (this.enabledButtonControls) //AB =)
                        //{
                        //    sb.Append("<td class=\"sorting_1\"><div class=\"btn-group m-r-5 m-b-5\"><a class=\"btn btn-warning dropdown-toggle\" data-toggle=\"dropdown\" href=\"javascript:;\" aria-expanded=\"false\">Acción <span class=\"caret\"></span></a><ul class=\"dropdown-menu\">");
                        //    foreach (KeyValuePair<string, string> pair in LISTBTNACTIONS)
                        //    {
                        //        sb.Append("<li><a href =\"javascript:").Append(pair.Value).Append("(").Append(res.Get(this.field_id)).Append(");\"> ").Append(pair.Key).Append(" </a></li>");
                        //    }
                        //    sb.Append("</ul></div></td>");
                        //}
                        sb.Append("</tr>");

                        //
                        // ESTADO DE CUENTA DETALLE
                        //

                        TABLECONDICIONSQLD = " ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");
                        field_id2 = "ID_EDOCTADETALLE";
                        res2 = this.getRowsTable2(sesion.db, 2, "*, 'ECD' AS ID_TABLE");
                        while (res2.Next())
                        {
                            sb.Append("<tr data-row='1' data-count='0' id='Hijo_").Append(res2.Get("ID_ESTADODECUENTA")).Append("_").Append(res2.Get("ID_EDOCTADETALLE")).Append("' class=\"gradeA odd\" role=\"row\" ondblclick=\"").Append(DataTable).Append(".edit('").Append(res2.Get(this.field_id2)).Append("')\">");
                            if (this.enabledCheckbox) // =)  ***/***
                            {
                                sb.Append("<td style='background-color: #FFF;'><input type=\"checkbox\"   onclick='seleccionarPadre(this);' name='").Append(res.Get("ID_ESTADODECUENTA")).Append("' id =\"").Append("H_").Append(res2.Get("ID_EDOCTADETALLE")).Append("\" value=\"").Append(res2.Get("ID_EDOCTADETALLE")).Append("\" /></td>");
                            }

                            listIds.Add(res2.Get(this.field_id2));

                            //if (this.enablednumRows) //AB =)
                            //{
                            //    sb.Append("<td class=\"sorting_1\">").Append(numrow).Append("</td>");
                            //}

                            foreach (string campo in this.CAMPOS)
                            {
                                if (!this.CAMPOSHIDDEN.Contains(campo))
                                {
                                    if (campo != "CVE_SEDE")
                                    {
                                        var value = res2.Get(campo);

                                        // Formato personalizado.
                                        if (dictColumnFormat.ContainsKey(campo))
                                        {
                                            ColumnFormat format = dictColumnFormat[campo];
                                            if (format != null)
                                                value = format.Invoke(value, res2);
                                        }

                                        if (campo == "CONCEPTO")
                                        {
                                            var value1 = res2.Get("BLOQUEOCONTRATO");
                                            var value2 = res2.Get("BLOQUEOS");
                                            var value3 = res2.Get("FECHADEENTREGA");

                                            if ((value1 == "True") && (value3 == null || value3 == ""))
                                            {
                                                sb.Append("<td style='background-color: #FFF; text-decoration: line-through; color: red;'>").Append(value).Append("</td>");
                                            }
                                            else if (Int32.Parse(value2) > 0)
                                            {
                                                sb.Append("<td style='background-color: #FFF; text-decoration: line-through; color: red;'>").Append(value).Append("</td>");
                                            }
                                            else
                                            {
                                                sb.Append("<td style='background-color: #FFF;'>").Append(value).Append("</td>");
                                            }
                                        }
                                        else if (campo == "MONTO" || campo == "MONTO_IVA" || campo == "MONTO_IVARET" || campo == "MONTO_ISRRET" || campo == "BANCOS")
                                        {

                                            if (campo == "MONTO_ISRRET")
                                                sb.Append("<td style='background-color: #FFF;'>").Append(value).Append("</td>");
                                            else
                                                sb.Append("<td style='background-color: #FFF;'>").Append(Double.Parse(value).ToString("C", CultureInfo.CreateSpecificCulture("es-MX"))).Append("</td>");
                                            
                                                                                          
                                        }
                                        else
                                        {
                                            sb.Append("<td style='background-color: #FFF;'>").Append(value).Append("</td>");
                                        }
                                    }
                                    else
                                    {
                                        sb.Append("<td style='background-color: #FFF;'></td>"); //Despliega el detalle
                                    }
                                }
                            }
                            sb.Append("</tr>");
                        }
                        // termina el listado de PADRE 0  y sus detalles

                        /*********************************************/
                        /** - COMIENZA LA BÚSQUEDA DE PENSIONADOS - **/
                        /*********************************************/
                        ResultSet resP = this.getRowsTableP(sesion.db, res.Get("ID_ESTADODECUENTA"));
                        while (resP.Next())
                        {

                            sb.Append("<tr data-row='1' data-count='0' id='PensionTR_").Append(res.Get("ID_ESTADODECUENTA")).Append("-").Append(resP.Get("ID_PENSIONADO")).Append("' style='background-color= #F0EDB8;' role=\"row\">");
                            if (this.enabledCheckbox)
                            {
                                if (editable)
                                {
                                    sb.Append("<td id='PensionTD_").Append(resP.Get("ID_CTAPENSIONADO")).Append("' style ='background-color: #FFF; cursor: pointer;'><i id='").Append(resP.Get("ID_CTAPENSIONADO")).
                                    Append("' onclick='javascript:formPage.EliminarPensionado(false,this.id);'").
                                    Append(" class=\"fa fa-trash fa-2\"aria-hidden=\"true\"></i></td>");
                                }
                                else
                                    sb.Append("<td></td>");
                            }

                            listIds.Add(resP.Get("ID_CTAPENSIONADO"));

                            foreach (string campo in this.CAMPOS)
                            {
                                if (!this.CAMPOSHIDDEN.Contains(campo))
                                {
                                    var value = resP.Get(campo);

                                    // Formato personalizado.
                                    if (dictColumnFormat.ContainsKey(campo))
                                    {
                                        ColumnFormat format = dictColumnFormat[campo];
                                        if (format != null)
                                            value = format.Invoke(value, resP);
                                    }
                                    if (dictColumnClass.ContainsKey(campo))
                                    {
                                        css_class = " " + dictColumnClass[campo];
                                    }
                                    else
                                        css_class = string.Empty;

                                    if (campo != "CVE_SEDE")
                                    {
                                        if (campo == "MONTO" || campo == "MONTO_IVA" || campo == "MONTO_IVARET" || campo == "MONTO_ISRRET" || campo == "BANCOS")
                                        {
                                            if (campo == "MONTO_ISRRET")
                                                sb.Append("<td style='background-color: #F0EDB8;'>").Append(value).Append("</td>");
                                            else
                                               sb.Append("<td style='background-color: #F0EDB8;'>").Append(Double.Parse(value).ToString("C", CultureInfo.CreateSpecificCulture("es-MX"))).Append("</td>");

                                        }
                                        else
                                        {
                                            sb.Append("<td style='background-color: #F0EDB8;'>").Append(value).Append("</td>");
                                        }
                                    }
                                    else
                                    {
                                        sb.Append("<td style='background-color: #FFF;'>").Append("</td>");
                                    }
                                }
                            }

                            //ENABLED BOTON DE ACCIONES
                            //if (this.enabledButtonControls) //AB =)
                            //{
                            //    sb.Append("<td style='background-color: #FFFFFF;'><div class=\"btn-group m-r-5 m-b-5\"><a class=\"btn btn-warning dropdown-toggle\" data-toggle=\"dropdown\" href=\"javascript:;\" aria-expanded=\"false\">Acción <span class=\"caret\"></span></a><ul class=\"dropdown-menu\">");
                            //    foreach (KeyValuePair<string, string> pair in LISTBTNACTIONS)
                            //    {
                            //        sb.Append("<li><a href =\"javascript:").Append(pair.Value).Append("(").Append(resP.Get("ID_CTAPENSIONADO")).Append(");\"> ").Append(pair.Key).Append(" </a></li>");
                            //    }
                            //    sb.Append("</ul></div></td>");
                            //}

                            sb.Append("</tr>");
                        }
                        /********************************************/
                        /** - TERMINA LA BÚSQUEDA DE PENSIONADOS - **/
                        /********************************************/

                        /*******************************************/
                        /** - COMIENZA LA BÚSQUEDA DE SUBPADRES - **/
                        /*******************************************/
                        ResultSet resH = this.getRowsTableH(sesion.db, res.Get("ID_ESTADODECUENTA"));
                        while (resH.Next())
                        {
                            sb.Append("<tr data-row='1' data-count='0' id='SUBPadreTR_").Append(res.Get("ID_ESTADODECUENTA")).Append("-").Append(resH.Get("ID_ESTADODECUENTA")).Append("' style='background-color= #F3F3F3;' role=\"row\">");
                            if (this.enabledCheckbox)
                            {
                                sb.Append("<td style='background-color: #FFF;'><input type=\"checkbox\" id=\"").Append("P_").Append(resH.Get(this.field_id)).Append("\" value=\"").Append(resH.Get(this.field_id)).Append("\" /></td>");
                            }

                            listIds.Add(resH.Get(this.field_id));

                            //if (this.enablednumRows) //AB =)
                            //{
                            //    sb.Append("<td style='background-color: #FFFFFF;'>").Append(numrow).Append("</td>");
                            //}
                            foreach (string campo in this.CAMPOS)
                            {
                                if (!this.CAMPOSHIDDEN.Contains(campo))
                                {
                                    var value = resH.Get(campo);

                                    // Formato personalizado.
                                    if (dictColumnFormat.ContainsKey(campo))
                                    {
                                        ColumnFormat format = dictColumnFormat[campo];
                                        if (format != null)
                                            value = format.Invoke(value, resH);
                                    }
                                    if (dictColumnClass.ContainsKey(campo))
                                    {
                                        css_class = " " + dictColumnClass[campo];
                                    }
                                    else
                                        css_class = string.Empty;

                                    if (campo != "CVE_SEDE")
                                    {
                                        if (campo == "CONCEPTO")
                                        {
                                            var value1 = res.Get("BLOQUEOCONTRATO");
                                            var value2 = res.Get("BLOQUEOS");
                                            var value3 = res.Get("FECHADEENTREGA");

                                            if ((value1 == "True") && (value3 == null || value3 == ""))
                                            {
                                                sb.Append("<td style='background-color: #EFEFEF; font-weight:700; text-decoration: line-through; color: red;'>").Append(value).Append("</td>");
                                            }
                                            else if (Int32.Parse(value2) > 0)
                                            {
                                                sb.Append("<td style='background-color: #EFEFEF; font-weight:700; text-decoration: line-through; color: red;'>").Append(value).Append("</td>");
                                            }
                                            else
                                            {
                                                sb.Append("<td style='background-color: #EFEFEF; font-weight:700;'>").Append(value).Append("</td>");
                                            }
                                        }
                                        else if (campo == "MONTO" || campo == "MONTO_IVA" || campo == "MONTO_IVARET" || campo == "MONTO_ISRRET" || campo == "BANCOS")
                                        {
                                                                                        
                                            if(campo == "MONTO_ISRRET")
                                                sb.Append("<td style='background-color: #EFEFEF;'>").Append(value).Append("</td>");                                            
                                            else
                                                sb.Append("<td style='background-color: #EFEFEF;'>").Append(Double.Parse(value).ToString("C", CultureInfo.CreateSpecificCulture("es-MX"))).Append("</td>");



                                        }
                                        else
                                        {
                                            sb.Append("<td style='background-color: #EFEFEF; font-weight:700;'>").Append(value).Append("</td>");
                                        }
                                    }
                                    else
                                    {
                                        sb.Append("<td id='SUBPadre_").Append(res.Get("ID_ESTADODECUENTA")).Append("-").Append(resH.Get("ID_ESTADODECUENTA")).Append("' data-activo='1' style='cursor:pointer; background-color: #EFEFEF; font-weight:700;' ").Append(css_class).Append(" onclick='javascript: _toggleEstadoCuentaDetalle2(this);'><span id='icon_").Append(resH.Get("ID_ESTADODECUENTA")).Append("' class='fa fa-minus-circle'>&nbsp;</span>").Append(value).Append("</td>"); // Despliega los acumulados de estado de cuenta
                                    }
                                }
                            }

                            //ENABLEB BOTON DE ACCIONES
                            //if (this.enabledButtonControls) //AB =)
                            //{
                            //    sb.Append("<td style='background-color: #FFFFFF;'><div class=\"btn-group m-r-5 m-b-5\"><a class=\"btn btn-warning dropdown-toggle\" data-toggle=\"dropdown\" href=\"javascript:;\" aria-expanded=\"false\">Acción <span class=\"caret\"></span></a><ul class=\"dropdown-menu\">");
                            //    foreach (KeyValuePair<string, string> pair in LISTBTNACTIONS)
                            //    {
                            //        sb.Append("<li><a href =\"javascript:").Append(pair.Value).Append("(").Append(resH.Get(this.field_id)).Append(");\"> ").Append(pair.Key).Append(" </a></li>");
                            //    }
                            //    sb.Append("</ul></div></td>");
                            //}

                            sb.Append("</tr>");

                            /*******************************************/
                            /** - TERMINA LA BÚSQUEDA DE SUBPADRES - **/
                            /*******************************************/

                            /*********************************************************/
                            /* Comienza la búsqueda de los SUBHIJOS de cada SUBPADRE */
                            /*********************************************************/
                            TABLECONDICIONSQLD = " ID_ESTADODECUENTA = " + resH.Get("ID_ESTADODECUENTA");
                            field_id2 = "ID_EDOCTADETALLE";
                            res3 = this.getRowsTable2(sesion.db, 2, "*, 'SUBH' AS ID_TABLE");
                            while (res3.Next())
                            {
                                sb.Append("<tr data-row='1' data-count='0' id='SUBHijo_").Append(res.Get("ID_ESTADODECUENTA")).Append("-").Append(res3.Get("ID_ESTADODECUENTA")).Append("-").Append(res3.Get("ID_EDOCTADETALLE")).Append("' class=\"gradeA odd\" role=\"row\" ondblclick=\"").Append(DataTable).Append(".edit('").Append(res3.Get(this.field_id2)).Append("')\">");

                                if (this.enabledCheckbox)
                                {
                                    sb.Append("<td style='background-color: #FFF;'><input type=\"checkbox\" id=\"").Append("H_").Append(res.Get("ID_ESTADODECUENTA")).Append("-").Append(res3.Get("ID_EDOCTADETALLE")).Append("\" value=\"").Append(res3.Get("ID_EDOCTADETALLE")).Append("\" /></td>");
                                }

                                listIds.Add(res3.Get(this.field_id2));

                                //if (this.enablednumRows) //AB =)
                                //{
                                //    sb.Append("<td class=\"sorting_1\">").Append(numrow).Append("</td>");
                                //}
                                foreach (string campo in this.CAMPOS)
                                {
                                    if (!this.CAMPOSHIDDEN.Contains(campo))
                                    {
                                        if (campo != "CVE_SEDE")
                                        {
                                            var value = res3.Get(campo);

                                            // Formato personalizado.
                                            if (dictColumnFormat.ContainsKey(campo))
                                            {
                                                ColumnFormat format = dictColumnFormat[campo];
                                                if (format != null)
                                                    value = format.Invoke(value, res3);
                                            }
                                            if (campo == "CONCEPTO")
                                            {
                                                var value1 = res3.Get("BLOQUEOCONTRATO");
                                                var value2 = res3.Get("BLOQUEOS");
                                                var value3 = res3.Get("FECHADEENTREGA");

                                                if (value1 == "True" && (value3 == null || value3 == ""))
                                                {
                                                    sb.Append("<td style='background-color: #FFF; text-decoration: line-through; color: red;'>").Append(value).Append("</td>");
                                                }
                                                else if (Int32.Parse(value2) > 0)
                                                {
                                                    sb.Append("<td style='background-color: #FFF; text-decoration: line-through; color: red;'>").Append(value).Append("</td>");
                                                }
                                                else
                                                {
                                                    sb.Append("<td style='background-color: #FFF;'>").Append(value).Append("</td>");
                                                }
                                            }
                                            else if (campo == "MONTO" || campo == "MONTO_IVA" || campo == "MONTO_IVARET" || campo == "MONTO_ISRRET" || campo == "BANCOS")
                                            {
                                               
                                                if(campo == "MONTO_ISRRET")
                                                    sb.Append("<td style='background-color: #FFF;'>").Append(value).Append("</td>");
                                                else
                                                sb.Append("<td style='background-color: #FFF;'>").Append(Double.Parse(value).ToString("C", CultureInfo.CreateSpecificCulture("es-MX"))).Append("</td>");
                                                
                                                                                               
                                              
                                            }
                                            else
                                            {
                                                sb.Append("<td style='background-color: #FFF;'>").Append(value).Append("</td>");
                                            }
                                        }
                                        else
                                        {
                                            sb.Append("<td style='background-color: #FFF;'></td>"); //Despliega el detalle
                                        }
                                    }
                                }
                                sb.Append("</tr>");
                            }
                            /********************************************************/
                            /* Termina la búsqueda de los SUBHIJOS de cada SUBPADRE */
                            /********************************************************/

                            /*************************************************/
                            /** - COMIENZA LA BÚSQUEDA DE SUB-PENSIONADOS - **/
                            /*************************************************/
                            ResultSet resP2 = this.getRowsTableSP(sesion.db, res.Get("ID_ESTADODECUENTA"), resH.Get("ID_ESTADODECUENTA"));
                            while (resP2.Next())
                            {

                                sb.Append("<tr data-row='1' data-count='0' id='SUBPensionTR_").Append(res.Get("ID_ESTADODECUENTA")).Append("-").Append(resH.Get("ID_ESTADODECUENTA")).Append("-").Append(resP2.Get("ID_CTAPENSIONADO")).Append("' style='background-color= #F0EDB8;' role=\"row\">");
                                if (this.enabledCheckbox)
                                {
                                    if (editable)
                                    {
                                        sb.Append("<td id='PensionTD_").Append(resP2.Get("ID_CTAPENSIONADO")).Append("' style ='background-color: #FFF; cursor: pointer;'><i id='").Append(resP2.Get("ID_CTAPENSIONADO")).
                                        Append("' onclick='javascript:formPage.EliminarPensionado(false,this.id);'").
                                        Append(" class=\"fa fa-trash fa-2\"aria-hidden=\"true\"></i></td>");
                                    }
                                    else
                                        sb.Append("<td></td>");
                                }


                                listIds.Add(resP2.Get("ID_CTAPENSIONADO"));

                                foreach (string campo in this.CAMPOS)
                                {
                                    if (!this.CAMPOSHIDDEN.Contains(campo))
                                    {

                                        var value = resP2.Get(campo);

                                        // Formato personalizado.
                                        if (dictColumnFormat.ContainsKey(campo))
                                        {
                                            ColumnFormat format = dictColumnFormat[campo];
                                            if (format != null)
                                                value = format.Invoke(value, resP2);
                                        }
                                        if (dictColumnClass.ContainsKey(campo))
                                        {
                                            css_class = " " + dictColumnClass[campo];
                                        }
                                        else
                                            css_class = string.Empty;

                                        if (campo != "CVE_SEDE")
                                        {
                                            if (campo == "MONTO" || campo == "MONTO_IVA" || campo == "MONTO_IVARET" || campo == "MONTO_ISRRET" || campo == "BANCOS")
                                            {

                                               if(campo == "MONTO_ISRRET")
                                                    sb.Append("<td style='background-color: #F0EDB8;'>").Append(value).Append("</td>");
                                               else
                                                sb.Append("<td style='background-color: #F0EDB8;'>").Append(Double.Parse(value).ToString("C", CultureInfo.CreateSpecificCulture("es-MX"))).Append("</td>");
                                                
                                                
                                               
                                            }
                                            else
                                            {
                                                sb.Append("<td style='background-color: #F0EDB8;'>").Append(value).Append("</td>");
                                            }
                                        }
                                        else
                                        {
                                            sb.Append("<td style='background-color: #FFF;'>").Append("</td>");
                                        }
                                    }
                                }

                                //ENABLED BOTON DE ACCIONES
                                //if (this.enabledButtonControls) //AB =)
                                //{
                                //    sb.Append("<td style='background-color: #FFFFFF;'><div class=\"btn-group m-r-5 m-b-5\"><a class=\"btn btn-warning dropdown-toggle\" data-toggle=\"dropdown\" href=\"javascript:;\" aria-expanded=\"false\">Acción <span class=\"caret\"></span></a><ul class=\"dropdown-menu\">");
                                //    foreach (KeyValuePair<string, string> pair in LISTBTNACTIONS)
                                //    {
                                //        sb.Append("<li><a href =\"javascript:").Append(pair.Value).Append("(").Append(resP2.Get("ID_CTAPENSIONADO")).Append(");\"> ").Append(pair.Key).Append(" </a></li>");
                                //    }
                                //    sb.Append("</ul></div></td>");
                                //}

                                sb.Append("</tr>");
                            }
                            /************************************************/
                            /** - TERMINA LA BÚSQUEDA DE SUB-PENSIONADOS - **/
                            /************************************************/
                        }
                    }
                }

                sb.Append("<input type='hidden' id='").Append(DataTable).Append("_data' value='has'>");

                if (this.enabledCheckbox)
                {
                    sb.Append("<script type=\"text/javascript\">").Append(DataTable).Append(".checkboxs=['").Append(string.Join("','", listIds)).Append("']; ").Append("</script>");
                }
            }
            else
            {
                sb.Append("<tr class=\"gradeA odd\" role=\"row\"><td colspan=\"").Append((this.COLUMNAS.Length + 2)).Append("\">")

                .Append("<div class=\"jumbotron m-b-0 text-center\" style=\"width:100%; \">")

                .Append("<h1>No existen registros</h1>")
                .Append("<p>Empiece creando un nuevo registro.</p>")

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



        public string mesanio_pago(Dictionary<string, string> list_fpago, string fechadepago)
        {

            string fechapagoma = "";
            if (list_fpago.Count > 0 && fechadepago != "")//fechapago
            {
                try
                {
                    var culture = new CultureInfo("en-US");
                    string fechabd = fechadepago;
                    // string fechabd = "2018-09-28";
                    DateTime myDateTime = DateTime.ParseExact(fechabd, "yyyy-M-dd", null);
                    // DateTime FECHA_ACTUAL = Convert.ToDateTime(FECHA_ACTUAL_stm, culture);
                    string mes = Convert.ToString(myDateTime.Month);
                    string anio = Convert.ToString(myDateTime.Year);
                    fechapagoma = anio + "-" + mes;
                }
                catch (Exception e) { fechapagoma = ""; }

            }

            return fechapagoma;


        }


        public Dictionary<string, string> cargaListasfechaPago( database db)
        {
            // FECHAPAGO REPETIDAS        

            Dictionary<string, string> list_fechapago;

            string last = CAMPOS[CAMPOS.Length - 1];           

            // String sql = "SELECT " + campos + " FROM " + (opc == 1 ? TABLE : TABLED) + " ";

             String sql = "select FORMAT( CAST(FECHAPAGO AS date) ,'yyyy-M', 'en-US') as FECHAPAGO, count(FECHAPAGO) AS 'No' FROM VESTADO_CUENTA ";

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
           
            sql += " AND CVE_TIPOFACTURA = 'A' ";//*********DESCOMENTAR****************************************************
            sql += " group by FORMAT( CAST(FECHAPAGO AS date) ,'yyyy-M', 'en-US') ";
            sql += " having count(FORMAT( CAST(FECHAPAGO AS date) ,'yyyy-M', 'en-US')) > 1 "; 


            ResultSet res = db.getTable(sql);
        
            list_fechapago = new Dictionary<string, string>();
            string color;
            int i = 1;

            // Random r = new Random();
            // System.Drawing.Color color1 = System.Drawing.Color.FromArgb(0, 0, r.Next(0, 256));//255 128 0 0.32

            while (res.Next())
            {
                switch (i)
                {
                    case 1:
                        color = "#ff800052";
                        break;
                    case 2:
                        color = "#ff8000ab";
                        break;
                    case 3:
                        color = "#ff8000";
                        break;
                    case 4:
                        color = "#ff3d00d9";
                        break;
                    case 5:
                        color = "#ffbe00";
                        break;
                    default:
                        color = "";
                        break;
                } 
                
              
                if (list_fechapago.Keys.Contains(res.Get("FECHAPAGO")) == false && !string.IsNullOrWhiteSpace(res.Get("FECHAPAGO")))
                    list_fechapago.Add(res.Get("FECHAPAGO"), color);

                 i++;

            }



            /* string sql = "select FECHAPAGO, count(FECHAPAGO) AS 'No' " +
                       " from VESTADO_CUENTA WHERE IDSIU = '" + IDSIU + "'" +
                       " AND CVE_SEDE = '" + sede + "'" +
                       " group by FECHAPAGO" +
                       " having count(FECHAPAGO) > 1";*/





            return list_fechapago;          


        }





        private int ContRowsTable2()
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

        private ResultSet getRowsTable2(database db, int opc = 1, string campos = "*")
        {
            string last = CAMPOS[CAMPOS.Length - 1];
            //string campos = "*";

            String sql = "SELECT " + campos + " FROM " + (opc == 1 ? TABLE : TABLED) + " ";

            if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
                sql += " WHERE " + TABLECONDICIONSQL;

            if (TABLECONDICIONSQLD != "" && TABLECONDICIONSQLD != null)
            {
                sql += " AND " + TABLECONDICIONSQLD;
            }


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
            sql += " OFFSET (" + (pg - 1) * show + ") ROWS"; // -- not sure if you
            sql += " FETCH NEXT " + show + " ROWS ONLY";

            return db.getTable(sql);

        }

        private ResultSet getRowsTableH(database db, string IDCta = "")
        {
            String sql = "SELECT *, 'SUBP' AS ID_TABLE FROM VESTADO_CUENTA where PADRE = " + IDCta;
            sql += " ORDER BY FECHAPAGO ";

            return db.getTable(sql);
        }

        private ResultSet getRowsTableP(database db, string IDCta = "")
        {
            String sql = "SELECT *, 'PEN' AS ID_TABLE FROM VESTADO_CUENTA_PENSIONADOS where ID_ESTADODECUENTA = " + IDCta;
            sql += " ORDER BY ID_CTAPENSIONADO ";

            return db.getTable(sql);
        }

        private ResultSet getRowsTableSP(database db, string Padre = "", string IDCta = "")
        {
            String sql = "SELECT *, 'SUBPEN' AS ID_TABLE FROM VESTADO_CUENTA_PENSIONADOS where PADRE = " + Padre + " and ID_ESTADODECUENTA = " + IDCta;
            sql += " ORDER BY ID_CTAPENSIONADO ";

            return db.getTable(sql);
        }

        private string getPaged2(string DataTable = "DataTable")
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



    }//<end class>Alex Baca

}