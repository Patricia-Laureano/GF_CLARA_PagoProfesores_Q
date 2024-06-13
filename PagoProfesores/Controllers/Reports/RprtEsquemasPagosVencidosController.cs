using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.Helper;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Reports
{
    public class RprtEsquemasPagosVencidosController : Controller
    {
        private SessionDB sesion;
        private List<Factory.Privileges> Privileges;
        private database db;

        public RprtEsquemasPagosVencidosController()
        {
            db = new database();
            string[] scripts = { "js/Reports/RprtEsquemasPagosVencidos/RprtEsquemasPagosVencidos.js", "plugins/jquery-table2excel/jquery.table2excel.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10144,  Element = "Controller" }, //PERMISO ACESSO AL REPORTE DE CALENDARIO DE PAGOS
                 new Factory.Privileges { Permiso = 10145,  Element = "formbtnConsultar" }, //PERMISO CONSULTAR CALENDARIO DE PAGOS
            };
        }

        // GET: ReportePagosPendientes
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu(18, 76, sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "ANIO", "DESC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Reporte de esquemas de pago de pagos vencidos' ", sesion);

            return View(Factory.View.Access + "Reports/RprtEsquemasPagosVencidos/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filter = "", string filterA = "", string filterP = "", string filterS = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "V_ESQUEMAPAGOSPENDIENTES";
            String[] columnas = { "Esquema de pago", "Concepto de pago", "Fecha de pago", "Año", "Periodo", "Situación" };
            String[] campos = { "ESQUEMA", "CONCEPTO", "FECHAPAGO", "ANIO", "PERIODO", "ESTADO", "SEDE", "FECHAACTUAL" };
            string[] campossearch = { "ESQUEMA", "CONCEPTO", "FECHAPAGO", "ESTADO" };
            string[] camposhidden = { "SEDE", "FECHAACTUAL" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;
            table.CAMPOSHIDDEN = camposhidden;
            table.TABLECONDICIONSQL = "SEDE = '" + filter + "'";

            if (!filterA.Equals(""))
                table.TABLECONDICIONSQL = table.TABLECONDICIONSQL + " AND ANIO = '" + filterA + "'";

            if (!filterP.Equals(""))
                table.TABLECONDICIONSQL = table.TABLECONDICIONSQL + " AND PERIODO = '" + filterP + "'";

            if (!filterS.Equals(""))
                table.TABLECONDICIONSQL = table.TABLECONDICIONSQL + " AND ESTADO = '" + filterS + "'";

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ANIO";

            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }

        //Exportar Reporte pagos pendientes.
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            var sql = string.Empty;

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Esquema de pago", typeof(string));
                tbl.Columns.Add("Concepto de pago", typeof(string));
                tbl.Columns.Add("Fecha de pago", typeof(string));
                tbl.Columns.Add("Año", typeof(string));
                tbl.Columns.Add("Peridodo", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Fecha actual", typeof(string));
                tbl.Columns.Add("Situación", typeof(string));

                sql = "SELECT * FROM V_ESQUEMAPAGOSPENDIENTES";
                if (Request.Params.Count > 0)
                {
                    if (Request.Params["sede"] != null && Request.Params["sede"] != "")
                    {
                        sql += " where sede = '" + Request.Params["sede"] + "'";
                    }

                    if (Request.Params["ciclo"] != null && Request.Params["ciclo"] != "")
                    {
                        sql += " and ANIO = '" + Request.Params["ciclo"] + "'";
                    }

                    if (Request.Params["periodo"] != null && Request.Params["periodo"] != "")
                    {
                        sql += " and PERIODO = '" + Request.Params["periodo"] + "'";
                    }

                    if (Request.Params["situacion"] != null && Request.Params["situacion"] != "")
                    {
                        sql += " and ESTADO = '" + Request.Params["situacion"] + "'";
                    }
                }
                ResultSet res = db.getTable(sql);

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("ESQUEMA"), res.Get("CONCEPTO"), res.Get("FECHAPAGO"), res.Get("ANIO"), res.Get("PERIODO"), res.Get("SEDE"), res.Get("FECHAACTUAL"), res.Get("ESTADO"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Pagos pendientes");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:H1"].AutoFitColumns();
                    //ws.Column(1).Width = 20;
                    //ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:H1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=RptEsquemaPagosVencidos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Reporte de Esquemas de pago de pagos vencidos", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Reporte de Esquemas de pago de pagos vencidos" + e.Message, sesion);
            }
        }

        [HttpGet]
        public string ConsultaCiclos(CiclosModel model)
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            StringBuilder sb = new StringBuilder();
            string selected = "selected";
            foreach (KeyValuePair<long, string> pair in model.ConsultaCiclos())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Key).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        [HttpGet]
        public string ConsultaPeriodos(PeriodosModel model)
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            string ClaveCiclo = Request.Params["ClaveCiclo"];
            StringBuilder sb = new StringBuilder();
            foreach (string str in model.ConsultaPeriodos(ClaveCiclo))
            {
                sb.Append("<option value=\"").Append(str).Append("\">").Append(str).Append("</option>\n");
            }
            return sb.ToString();
        }
    }
}
