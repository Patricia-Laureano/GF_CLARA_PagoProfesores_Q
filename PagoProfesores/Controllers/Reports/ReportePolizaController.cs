using System;
using System.Collections.Generic;
using PagoProfesores.Models.Helper;
using System.Text;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace PagoProfesores.Controllers.Reports
{
    public class ReportePolizaController : Controller
    {
        private SessionDB sesion;
        private List<Factory.Privileges> Privileges;
        private database db;

        public ReportePolizaController()
        {
            db = new database();
            string[] scripts = { "js/Reports/ReportePoliza/ReportePoliza.js", "plugins/jquery-table2excel/jquery.table2excel.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10144,  Element = "Controller" }, //PERMISO ACESSO AL REPORTE DE CALENDARIO DE PAGOS
                 new Factory.Privileges { Permiso = 10145,  Element = "formbtnConsultar" }, //PERMISO CONSULTAR CALENDARIO DE PAGOS
            };
        }

        // GET: CalendariodePago
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Reportes", "Póliza contable", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Nómina por Año y Mes' ", sesion);

            return View(Factory.View.Access + "Reports/ReportePoliza/Start.cshtml");
        }
        
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filterMes = "", string filterAnio = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            
            DataTable table = new DataTable();

            table.TABLE = "QNominaMesAnio";
            String[] columnas = { "tipo_transferencia", "tipo_pago", "fecha_de_pago","id","empleado","importe","importe_iva","importe_iva_retenido","importe_isr_retenido","bancos" };
            String[] campos =   { "tipo_transferencia", "tipo_pago", "fecha_de_pago", "id", "empleado", "importe", "importe_iva", "importe_iva_retenido", "importe_isr_retenido", "bancos" };
            string[] campossearch = { "mes" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;
            
            if (!filterAnio.Equals("") && !filterMes.Equals(""))
                table.TABLECONDICIONSQL = "anio = '" + filterAnio + "' and mes='" + filterMes + "'";

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "tipo_transferencia";
            
            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }

        public string CreateDateMes(string mes,string anio,string sede)
        {
            StringBuilder sb = new StringBuilder(1000000);
            string fullMonthName;
            string condicionsql = "";
            string sql2 = "SELECT * FROM QNominaMesAnioSum ";
            string sql;
            
            condicionsql = " WHERE sede = '" + sede + "'";
            condicionsql += " AND anio = '" + anio + "' and mes='" + mes + "'";

            sql2 = sql2 + condicionsql;
            ResultSet res2 = db.getTable(sql2);
            if (!res2.HasRows)
                sb.Append("");
            else
            {
                fullMonthName = new DateTime(2015, Int32.Parse(mes), 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));

                sb.Append("<table id=\"t1\" border=1>");
                sb.Append("<thead><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><th><b>" + fullMonthName.ToUpper() + anio+ " </b></th><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>");
                sb.Append("<tr><th>Tipo de Transferencia</th>")
                    .Append("<th>Tipo de Pago</th>")
                    .Append("<th>Número de Cuenta</th>")
                    .Append("<th>Cuenta IVA</th>")
                    .Append("<th>Cuenta Retención IVA</th>")
                    .Append("<th>Cuenta Retención ISR</th>")
                    .Append("<th>Carrera</th>")
                    .Append("<th>Fecha de Pago</th>")
                    .Append("<th>Folio</th>")
                    .Append("<th>ID empleado</th>")
                    .Append("<th>Empleado</th>")
                    .Append("<th>RFC</th>")//nuevo
                    .Append("<th>Suma de Importe</th>")
                    .Append("<th>Suma de ImpIVA</th>")
                    .Append("<th>Suma de ImpIVARet</th>")
                    .Append("<th>Suma de ImpISRRet</th>")
                    .Append("<th>Suma Bancos</th>")
                    .Append("</tr> </thead>");

                while (res2.Next())
                {
                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append(res2.Get("tipo_transferencia"));
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(res2.Get("tipo_pago"));
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td></td><td></td>");
                    sb.Append("<td></td><td></td>");
                    sb.Append("<td></td>");//NUEVO
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    decimal s = decimal.Parse(res2.Get("suma_importe"));
                    sb.Append(s.ToString("C"));
                    sb.Append("</td>");
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    s = decimal.Parse(res2.Get("suma_importe_iva"));
                    sb.Append(s.ToString("C"));
                    sb.Append("</td>");
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    s = decimal.Parse(res2.Get("suma_importe_iva_retenido"));
                    sb.Append(s.ToString("C"));
                    sb.Append("</td>");
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    s = decimal.Parse(res2.Get("suma_importe_isr_retenido"));
                    sb.Append(s.ToString("C"));
                    sb.Append("</td>");
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    s = decimal.Parse(res2.Get("suma_bancos"));
                    sb.Append(s.ToString("C"));
                    sb.Append("</td>");
                    sb.Append("<tbody>");

                    sql = "SELECT * FROM QReportePoliza ";
                    sql = sql + condicionsql + " and tipo_transferencia='" + res2.Get("tipo_transferencia") + "' and tipo_pago='" + res2.Get("tipo_pago") + "'";

                    ResultSet res = db.getTable(sql);
                    while (res.Next())
                    {
                        sb.Append("<tr>");
                        sb.Append("<td>");
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("numero_de_cuenta"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("cuenta_iva"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("cuenta_ret_iva"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("cuenta_ret_isr"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("carrera"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("fecha_de_pago").Substring(0, 10));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("FOLIO"));
                        sb.Append("</td>");
                        sb.Append("<td class='text'>");
                        sb.Append("&#8203;");
                        sb.Append(res.Get("id"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("empleado"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("RFC"));
                        sb.Append("</td>");//NUEVO
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s0 = decimal.Parse(res.Get("importe"));
                        sb.Append(s0.ToString("C"));
                        sb.Append("</td>");
                        sb.Append("</td>");
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s1 = decimal.Parse(res.Get("importe_iva"));
                        sb.Append(s1.ToString("C"));
                        sb.Append("</td>");
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s2 = decimal.Parse(res.Get("importe_iva_retenido"));
                        sb.Append(s2.ToString("C"));
                        sb.Append("</td>");
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s3 = decimal.Parse(res.Get("importe_isr_retenido"));
                        sb.Append(s3.ToString("C"));
                        sb.Append("</td>");
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s4 = decimal.Parse(res.Get("bancos"));
                        sb.Append(s4.ToString("C"));
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                }
                sb.Append("</tbody>");
                sb.Append("</table>");
            }
            return sb.ToString();
        }

        public string CreateDataTableHTML(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filterMes = "", string filterAnio = "", string sede = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            StringBuilder sb = new StringBuilder(10000000);
            RecalculoAsimilados recalculo = new RecalculoAsimilados();

            sb.Append("<style> .datagrid table { border-collapse: collapse; text-align: left; width: 100%; } .datagrid { background: #fff; overflow: auto; border: 1px solid #AAAAAA; -webkit-border-radius: 6px; -moz-border-radius: 6px; border-radius: 6px; }.datagrid table td, .datagrid table th { padding: 1px 10px; }.datagrid table thead th {background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #FF8520), color-stop(1, #FF8520) );background:-moz-linear-gradient( center top, #FF8520 5%, #FF8520 100% );filter:progid:DXImageTransform.Microsoft.gradient(startColorstr='#FF8520', endColorstr='#FF8520');background-color:#FF8520; color:#FFFFFF; font-size: 15px; font-weight: bold; border-left: 1px solid #FFFFFF; } .datagrid table thead th:first-child { border: none; }.datagrid table tbody td { color: #000000; border-left: 1px solid #FF8520;font-size: 12px;font-weight: normal; border-top: 1px solid #FF8520; }.datagrid table tbody .alt td { background: #FFB982; color: #00496B; }.datagrid table tbody td:first-child { border-left: none; }.datagrid table tbody tr:last-child td { border-bottom: none; }.datagrid table tfoot td div { border-top: 1px solid #AAAAAA;background: #E1EEF4;} .datagrid table tfoot td { padding: 0; font-size: 12px } .datagrid table tfoot td div{ padding: 2px; }.datagrid table tfoot td ul { margin: 0; padding:0; list-style: none; text-align: right; }.datagrid table tfoot  li { display: inline; }.datagrid table tfoot li a { text-decoration: none; display: inline-block;  padding: 2px 8px; margin: 1px;color: #FFFFFF;border: 1px solid #006699;-webkit-border-radius: 3px; -moz-border-radius: 3px; border-radius: 3px; background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #006699), color-stop(1, #00557F) );background:-moz-linear-gradient( center top, #006699 5%, #00557F 100% );filter:progid:DXImageTransform.Microsoft.gradient(startColorstr='#006699', endColorstr='#00557F');background-color:#006699; }.datagrid table tfoot ul.active, .datagrid table tfoot ul a:hover { text-decoration: none;border-color: #006699; color: #FFFFFF; background: none; background-color:#00557F;} iv.dhtmlx_window_active, div.dhx_modal_cover_dv { position: fixed !important; }</style>");
            sb.Append("<div class=\"datagrid\">");
            if (!filterMes.Equals(""))
            {
                string fullMonthName;
                string condicionsql = "";
                string sql2 = "SELECT * FROM QNominaMesAnioSum";
                string sql;
                
                condicionsql = " WHERE sede = '" + sede + "'";

                if (!filterAnio.Equals("") && !filterMes.Equals(""))
                    condicionsql += " AND anio = '" + filterAnio + "' and mes='" + filterMes + "'";

                sql2 = sql2 + condicionsql;
                ResultSet res2 = db.getTable(sql2);

                fullMonthName = new DateTime(2015, Int32.Parse(filterMes), 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));

                sb.Append("<table id=\"t1\" border=1>");
                sb.Append("<thead><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><th><b>" + fullMonthName.ToUpper() + " " + filterAnio + "</b></th><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>");
                sb.Append("<tr><th>Tipo de Transferencia</th>")
                    .Append("<th>Tipo de Pago</th>")
                    .Append("<th>Número de Cuenta</th>")
                    .Append("<th>Cuenta IVA</th>")
                    .Append("<th>Cuenta Retención IVA</th>")
                    .Append("<th>Cuenta Retención ISR</th>")
                    .Append("<th>Carrera</th>")
                    .Append("<th>Fecha de Pago</th>")
                    .Append("<th>Folio</th>")
                    .Append("<th>ID empleado</th>")
                    .Append("<th>Empleado</th>")
                    .Append("<th>RFC</th>")//nuevo
                    .Append("<th>Suma de Importe</th>")
                    .Append("<th>Suma de ImpIVA</th>")
                    .Append("<th>Suma de ImpIVARet</th>")
                    .Append("<th>Suma de ImpISRRet</th>")
                    .Append("<th>Suma Bancos</th>")                  
                    .Append("</tr> </thead>");

                /* _____________________________ bandera recalculo ___________________________ */
                List<string> listIDSIU = new List<string>();
                while (res2.Next())
                {
                    sql = "SELECT * FROM QReportePoliza " + condicionsql + " and tipo_transferencia='" + res2.Get("tipo_transferencia") + "' and tipo_pago='" + res2.Get("tipo_pago") + "'";
                    ResultSet res = db.getTable(sql);
                    while (res.Next())
                        if (!listIDSIU.Contains(res.Get("id")))
                            listIDSIU.Add(res.Get("id"));
                }
                foreach (string IDSIU in listIDSIU)
                    recalculo.bandera_recalculo(IDSIU, sede);
                /* ________________________________________________________ */

                res2.ReStart();
                while (res2.Next())
                {
                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append(res2.Get("tipo_transferencia"));
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(res2.Get("tipo_pago"));
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("</td>");
                    sb.Append("<td></td><td></td>");
                    sb.Append("<td></td><td></td>");
                    sb.Append("<td></td>");//nuevo
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    double s = double.Parse(res2.Get("suma_importe"));
                    sb.Append(s.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                    sb.Append("</td>");
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    s = double.Parse(res2.Get("suma_importe_iva"));
                    sb.Append(s.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                    sb.Append("</td>");
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    s = double.Parse(res2.Get("suma_importe_iva_retenido"));
                    sb.Append(s.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                    sb.Append("</td>");
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    s = double.Parse(res2.Get("suma_importe_isr_retenido"));
                    sb.Append(s.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                    sb.Append("</td>");
                    sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                    s = double.Parse(res2.Get("suma_bancos"));
                    sb.Append(s.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                    sb.Append("</td>");
                    sb.Append("<tbody>");

                    sql = "SELECT * FROM QReportePoliza ";
                    sql = sql + condicionsql + " and tipo_transferencia='" + res2.Get("tipo_transferencia") + "' and tipo_pago='" + res2.Get("tipo_pago") + "'";

                    ResultSet res = db.getTable(sql);
                    while (res.Next())
                    {
                        sb.Append("<tr>");
                        sb.Append("<td>");
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("numero_de_cuenta"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("cuenta_iva"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("cuenta_ret_iva"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("cuenta_ret_isr"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("carrera"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("fecha_de_pago").Substring(0, 10));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("FOLIO"));
                        sb.Append("</td>");
                        sb.Append("<td class='text'>");
                        sb.Append("&#8203;");
                        sb.Append(res.Get("id"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("empleado"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("RFC"));
                        sb.Append("</td>");
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s0 = decimal.Parse(res.Get("importe"));
                        sb.Append(s0.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                        sb.Append("</td>");
                        sb.Append("</td>");
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s1 = decimal.Parse(res.Get("importe_iva"));
                        sb.Append(s1.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                        sb.Append("</td>");
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s2 = decimal.Parse(res.Get("importe_iva_retenido"));
                        sb.Append(s2.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                        sb.Append("</td>");
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s3 = decimal.Parse(res.Get("importe_isr_retenido"));
                        sb.Append(s3.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                        sb.Append("</td>");
                        sb.Append("<td style='mso-number-format:\\0022$\\0022\\#\\,\\#\\#0\\.00'>");
                        decimal s4 = decimal.Parse(res.Get("bancos"));
                        sb.Append(s4.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                        sb.Append("</td>");
                        sb.Append("</td>");                  
                        sb.Append("</tr>");
                    }
                }
                sb.Append("</tbody>");
                sb.Append("</table>");
            }
            else
            {
                sb.Append("<table id=\"t1\" border=1>");
                sb.Append("<tr>");
                if (!CreateDateMes("1", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("1", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("2", filterAnio,sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("2", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("3", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("3", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("4", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("4", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("5", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("5", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("6", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("6", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("7", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("7", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("8", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("8", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("9", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("9", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("10", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("10", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("11", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("11", filterAnio, sede));
                    sb.Append("</td>");
                }
                if (!CreateDateMes("12", filterAnio, sede).Equals(""))
                {
                    sb.Append("<td valign=\"top\">");
                    sb.Append(CreateDateMes("12", filterAnio, sede));
                    sb.Append("</td>");
                   
                }
                sb.Append("</tr>");
                sb.Append("</table>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }

        //Exportar Reporte de Calendario de Pagos
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Opcion de Pago", typeof(string));
                tbl.Columns.Add("Fecha Pago", typeof(string));
                tbl.Columns.Add("ID", typeof(string));
                tbl.Columns.Add("NOMBRE", typeof(string));
                tbl.Columns.Add("APELLIDOS", typeof(string));

                ResultSet res = db.getTable("select DISTINCT OPCIONDEPAGO,FECHA_PA,ID_PERSONA,NOMBRE,APELLIDOS from QNomina");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("OPCIONDEPAGO"), res.Get("FECHA_PA"), res.Get("ID_PERSONA"), res.Get("NOMBRE"), res.Get("APELLIDOS"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("NominaAnoMes");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:F1"].AutoFitColumns();
                    //ws.Column(1).Width = 20;
                    //ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:F1"])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    //Example how to Format Column 1 as numeric 
                    using (ExcelRange col = ws.Cells[2, 1, 2 + tbl.Rows.Count, 1])
                    {
                        col.Style.Numberformat.Format = "#,##0.00";
                        col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    //Write it back to the client
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=NominaAnoMes.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Nomina Anio Mes", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Reporte Poliza" + e.Message, sesion);
            }
        }
        
        [HttpGet]
        public string ConsultaAnios(AniosNominaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            StringBuilder sb = new StringBuilder();
            string selected = "selected";
            foreach (string a  in model.getAnios())
            {
                sb.Append("<option value=\"").Append(a.ToString()).Append("\" ").Append(selected).Append(">").Append(a.ToString()).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        [HttpGet]
        public string ConsultaMeses(MesesNominaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            StringBuilder sb = new StringBuilder();
            string selected = "selected";
            foreach (KeyValuePair<string,string> me in model.getMeses("2017"))
            {
                sb.Append("<option value=\"").Append(me.Key).Append("\" ").Append(selected).Append(">").Append(me.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }
    }
}