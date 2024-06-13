using System;
using System.Collections.Generic;
using PagoProfesores.Models.Helper;
using System.Text;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using System.Globalization;

namespace PagoProfesores.Controllers.Reports
{
    public class ReportePolizaFDController : Controller
    {
        private SessionDB sesion;
        private List<Factory.Privileges> Privileges;
        private database db;

        public ReportePolizaFDController()
        {
            db = new database();
            string[] scripts = { "js/Reports/ReportePolizaFD/ReportePolizaFD.js", "plugins/jquery-table2excel/jquery.table2excel.js" };
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
            ViewBag.Main = view.createMenu("Reportes", "Poliza contable por FD", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Nómina por Año y Mes' ", sesion);

            return View(Factory.View.Access + "Reports/ReportePolizaFD/Start.cshtml");
        }

        public string CreateDateMes(string mes, string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(1000000);
            string fullMonthName;
            string condicionsql = "";
            string sql2 = "SELECT * FROM QNominaMesAnioSumFD ";
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
                sb.Append("<thead><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><th><b>" + fullMonthName.ToUpper() + anio + " </b></th><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>");
                sb.Append("<tr><th>Tipo de Transferencia</th>")
                    .Append("<th>Tipo de Pago</th>")
                    .Append("<th>Número de Cuenta</th>")
                    .Append("<th>Cuenta IVA</th>")
                    .Append("<th>Cuenta Retención IVA</th>")
                    .Append("<th>Cuenta Retención ISR</th>")
                    .Append("<th>Carrera</th>")
                    .Append("<th>Fecha de Depósito</th>")
                    .Append("<th>Folio</th>")
                    .Append("<th>ID empleado</th>")
                    .Append("<th>Empleado</th>")
                    .Append("<th>RFC Persona Física</th>")//nuevo
                    .Append("<th>CURP</th>")
                    .Append("<th>Facturar con</th>")
                    .Append("<th>RFC Persona Moral</th>")
                    .Append("<th>Persona Moral</th>")
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
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
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

                    sql = "SELECT * FROM QReportePolizaFD ";
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
                        sb.Append(res.Get("fecha_de_deposito").Substring(0, 10));
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
                        sb.Append("<td>");
                        sb.Append(res.Get("CURP"));
                        sb.Append("</td>");//NUEVO
                        sb.Append("<td>");
                        sb.Append(res.Get("DATOSFISCALES"));
                        sb.Append("</td>");//NUEVO
                        sb.Append("<td>");
                        sb.Append(res.Get("RZ_RFC"));
                        sb.Append("</td>");//NUEVO
                        sb.Append("<td>");
                        sb.Append(res.Get("PERSONA_MORAL"));
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
            StringBuilder sb = new StringBuilder(100000000);
            RecalculoAsimilados recalculo = new RecalculoAsimilados();
			
			sb.Append("<style> .datagrid table { border-collapse: collapse; text-align: left; width: 100%; } .datagrid { background: #fff; overflow: auto; border: 1px solid #AAAAAA; -webkit-border-radius: 6px; -moz-border-radius: 6px; border-radius: 6px; }.datagrid table td, .datagrid table th { padding: 1px 10px; }.datagrid table thead th {background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #FF8520), color-stop(1, #FF8520) );background:-moz-linear-gradient( center top, #FF8520 5%, #FF8520 100% );filter:progid:DXImageTransform.Microsoft.gradient(startColorstr='#FF8520', endColorstr='#FF8520');background-color:#FF8520; color:#FFFFFF; font-size: 15px; font-weight: bold; border-left: 1px solid #FFFFFF; } .datagrid table thead th:first-child { border: none; }.datagrid table tbody td { color: #000000; border-left: 1px solid #FF8520;font-size: 12px;font-weight: normal; border-top: 1px solid #FF8520; }.datagrid table tbody .alt td { background: #FFB982; color: #00496B; }.datagrid table tbody td:first-child { border-left: none; }.datagrid table tbody tr:last-child td { border-bottom: none; }.datagrid table tfoot td div { border-top: 1px solid #AAAAAA;background: #E1EEF4;} .datagrid table tfoot td { padding: 0; font-size: 12px } .datagrid table tfoot td div{ padding: 2px; }.datagrid table tfoot td ul { margin: 0; padding:0; list-style: none; text-align: right; }.datagrid table tfoot  li { display: inline; }.datagrid table tfoot li a { text-decoration: none; display: inline-block;  padding: 2px 8px; margin: 1px;color: #FFFFFF;border: 1px solid #006699;-webkit-border-radius: 3px; -moz-border-radius: 3px; border-radius: 3px; background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #006699), color-stop(1, #00557F) );background:-moz-linear-gradient( center top, #006699 5%, #00557F 100% );filter:progid:DXImageTransform.Microsoft.gradient(startColorstr='#006699', endColorstr='#00557F');background-color:#006699; }.datagrid table tfoot ul.active, .datagrid table tfoot ul a:hover { text-decoration: none;border-color: #006699; color: #FFFFFF; background: none; background-color:#00557F;} iv.dhtmlx_window_active, div.dhx_modal_cover_dv { position: fixed !important; }</style>");
            sb.Append("<div class=\"datagrid\">");
            if (!filterMes.Equals(""))
            {
                string fullMonthName;
                string condicionsql = "";
                string sql2 = "SELECT * FROM QNominaMesAnioSumFD";
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
                    .Append("<th>Fecha de Depósito</th>")
                    .Append("<th>Folio</th>")
                    .Append("<th>ID empleado</th>")
                    .Append("<th>Empleado</th>")
                    .Append("<th>RFC Persona Física</th>")
                    .Append("<th>CURP</th>")
                    .Append("<th>Facturar con</th>")
                    .Append("<th>RFC Persona Moral</th>")
                    .Append("<th>Persona Moral</th>")
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
					sql = "SELECT * FROM QReportePolizaFD " + condicionsql + " and tipo_transferencia='" + res2.Get("tipo_transferencia") + "' and tipo_pago='" + res2.Get("tipo_pago") + "'";
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
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
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

                    sql = "SELECT * FROM QReportePolizaFD ";
                    /*sql = "SELECT pfd1.[id_estadodecuenta],[tipo_transferencia],[cveTipo_transferencia],[tipo_pago] "
                        + "       ,stuff( "
                        + "              (select ', ' + pfd2.[numero_de_cuenta] "
                        + "                 from QReportePolizaFD pfd2 "
                        + "                where pfd1.id_estadodecuenta = pfd2.id_estadodecuenta "
                        + "			     for XML PATH('') "
                        + "             ) "
                        + "	       ,1,1,'') as numero_de_cuenta "
                        + "      ,stuff( "
                        + "             (select ', ' + pfd2.[cuenta_iva] "
                        + "                from QReportePolizaFD pfd2 "
                        + "               where pfd1.id_estadodecuenta = pfd2.id_estadodecuenta "
                        + "                 for XML PATH('') "
                        + "             ) "
                        + "	   ,1,1,'') as cuenta_iva "
                        + "      ,stuff( "
                        + "             (select ', ' + pfd2.[cuenta_ret_iva] "
                        + "                from QReportePolizaFD pfd2 "
                        + "               where pfd1.id_estadodecuenta = pfd2.id_estadodecuenta "
                        + "                 for XML PATH('') "
                        + "             ) "
                        + "	   ,1,1,'') as cuenta_ret_iva "
                        + "      ,stuff( "
                        + "             (select ', ' + pfd2.[cuenta_ret_isr] "
                        + "                from QReportePolizaFD pfd2 "
                        + "               where pfd1.id_estadodecuenta = pfd2.id_estadodecuenta "
                        + "                 for XML PATH('') "
                        + "             ) "
                        + "	   ,1,1,'') as cuenta_ret_isr "
                        + "      ,stuff( "
                        + "             (select ', ' + pfd2.[carrera] "
                        + "                from QReportePolizaFD pfd2 "
                        + "               where pfd1.id_estadodecuenta = pfd2.id_estadodecuenta "
                        + "                 for XML PATH('') "
                        + "             ) "
                        + "	   ,1,1,'') as carrera "
                        + "      ,[fecha_de_deposito],[FOLIO],[anio],[mes],[id],[empleado],[importe],[importe_iva],[importe_iva_retenido],[importe_isr_retenido] "
                        + "      ,[bancos],[RFC],[CURP],[DATOSFISCALES],[RZ_RFC],[PERSONA_MORAL],[sede] "
                        + "        FROM[dbo].[QReportePolizaFD] pfd1 ";
                    sql = sql + condicionsql + " and tipo_transferencia='" + res2.Get("tipo_transferencia") + "' and tipo_pago='" + res2.Get("tipo_pago") + "'";
                    sql = sql + " group by pfd1.[id_estadodecuenta],[tipo_transferencia],[cveTipo_transferencia],[tipo_pago] "
                        + " ,[fecha_de_deposito],[FOLIO],[anio],[mes],[id],[empleado],[importe],[importe_iva],[importe_iva_retenido],[importe_isr_retenido] "
                        + " ,[bancos],[RFC],[CURP],[DATOSFISCALES],[RZ_RFC],[PERSONA_MORAL],[sede]";*/

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
                        sb.Append(res.Get("fecha_de_deposito").Substring(0, 10));
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
                        sb.Append("<td>");
                        sb.Append(res.Get("CURP"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("DATOSFISCALES"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("RZ_RFC"));
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(res.Get("PERSONA_MORAL"));
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
                if (!CreateDateMes("2", filterAnio, sede).Equals(""))
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

		[HttpGet]
        public string ConsultaAnios(AniosNominaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            StringBuilder sb = new StringBuilder();
            string selected = "selected";
            foreach (string a in model.getAnios())
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
            foreach (KeyValuePair<string, string> me in model.getMeses("2017"))
            {
                sb.Append("<option value=\"").Append(me.Key).Append("\" ").Append(selected).Append(">").Append(me.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }
    }
}