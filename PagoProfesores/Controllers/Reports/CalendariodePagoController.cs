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

namespace PagoProfesores.Controllers.Reports
{
    public class CalendariodePagoController : Controller
    {
        private SessionDB sesion;
        private List<Factory.Privileges> Privileges;
        private database db;

        public CalendariodePagoController()
        {
            db = new database();
            string[] scripts = { "js/Reports/CalendariodePago/CalendariodePago.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10142,  Element = "Controller" }, //PERMISO ACESSO AL REPORTE DE CALENDARIO DE PAGOS
                 new Factory.Privileges { Permiso = 10143,  Element = "formbtnConsultar" }, //PERMISO CONSULTAR CALENDARIO DE PAGOS
            };
        }

        // GET: CalendariodePago
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu(18, 19, sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "CVE_CICLO", "ASC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Calendario de Pago' ", sesion);

            return View(Factory.View.Access + "Reports/CalendariodePago/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filterC = "", string filterP = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "QCalendarioRUA01";
            String[] columnas = { "Periodo", "Descripcion", "Fecha Inicial", "Fecha Final", "Tipo de Periodo", "Ciclo Escolar" };
            String[] campos = { "PERIODO", "DESCRIPCION", "FECHA_INICIAL", "FECHA_FINAL", "TIPOPERIODO", "CVE_CICLO" };
            string[] campossearch = { "PERIODO", "DESCRIPCION", "CVE_CICLO" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;
            table.TABLECONDICIONSQL = "CVE_CICLO = '" + filterC + "'";
            if (!filterP.Equals(""))

                table.TABLECONDICIONSQL = "PERIODO = '" + filterP + "'";

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_CICLO";


            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }

        //Exportar Reporte de Calendario de Pagos
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            var sql = string.Empty;

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Descripcion", typeof(string));
                tbl.Columns.Add("Fecha Inicial", typeof(string));
                tbl.Columns.Add("Fecha Final", typeof(string));
                tbl.Columns.Add("Tipo Peridodo", typeof(string));
                tbl.Columns.Add("Ciclo Escolar", typeof(string));

                sql = "SELECT * FROM QCalendarioRUA01";
                if (Request.Params.Count > 0)
                {
                    if (Request.Params["Periodo"] != null && Request.Params["Periodo"] != "")
                    {
                        sql += " where PERIODO = '" + Request.Params["Periodo"] + "'";
                    }
                }
                ResultSet res = db.getTable(sql);

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("PERIODO"), res.Get("DESCRIPCION"), res.Get("FECHA_INICIAL"), res.Get("FECHA_FINAL"), res.Get("TIPOPERIODO"), res.Get("CVE_CICLO"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Calendario de Pago");

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
                    Response.AddHeader("content-disposition", "attachment;  filename=CalendariodePago.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Calendario de Pago", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Calendario de Pago" + e.Message, sesion);

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