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
    public class PersonasController : Controller
    {
        private SessionDB sesion;
        private List<Factory.Privileges> Privileges;
        private database db;

        public PersonasController()
        {
            db = new database();
            string[] scripts = { "js/Reports/Personas/Personas.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10147,  Element = "Controller" }, //PERMISO ACESSO AL REPORTE DE PERSONAS
                 new Factory.Privileges { Permiso = 10148,  Element = "formbtnConsultar" }, //PERMISO CONSULTAR PEPORTE DE PERSONAS
            };

        }

        // GET: CalendariodePago
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Reportes", "Personas con Estado de Cuenta", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "PERIODO", "ASC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Personas con Estado de Cuenta' ", sesion);

            //return View();
            return View(Factory.View.Access + "Reports/Personas/Start.cshtml");


        }




        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filterC = "", string filterP = "", string filterN = "")
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }


            DataTable table = new DataTable();

            table.TABLE = "QPersonasenEstadodeCuenta01";
            String[] columnas = { "ID SIU", "Nombre", "Apellidos","Sexo", "Telefono", "Correo", "tipo de Pago","Tipo de Factura","Periodo","Ciclo","Nivel"};
            String[] campos = { "IDSIU", "NOMBRES", "APELLIDOS", "SEXO", "TELEFONO", "CORREO","TIPODEPAGO","TIPOFACTURA","PERIODO","CVE_CICLO","CVE_NIVEL" };
            string[] campossearch = { "PERIODO", "CVE_CICLO","CVE_NIVEL" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;
            table.TABLECONDICIONSQL = "CVE_CICLO = '" + filterC + "'";
            if (!filterP.Equals(""))

                table.TABLECONDICIONSQL = "PERIODO = '" + filterP + "'";

            if (!filterN.Equals(""))

                table.TABLECONDICIONSQL = "CVE_NIVEL = '" + filterN + "'";

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_CICLO";


            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }

        //Exportar Reporte de Personas con estado de cuenta
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("IDSUI", typeof(string));
                tbl.Columns.Add("Nombres", typeof(string));
                tbl.Columns.Add("Apellidos", typeof(string));
                tbl.Columns.Add("Sexo", typeof(string));
                tbl.Columns.Add("Telefono", typeof(string));
                tbl.Columns.Add("Correo", typeof(string));
                tbl.Columns.Add("Tipo de Pago", typeof(string));
                tbl.Columns.Add("Tipo Factura", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Ciclo Escolar", typeof(string));
                tbl.Columns.Add("Nivel", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM QPersonasenEstadodeCuenta01");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("IDSUI"), res.Get("NOMBRES"), res.Get("APELLIDOS"), res.Get("SEXO"), res.Get("TELEFONO"), res.Get("CORREO"), res.Get("TIPODEPAGO"), res.Get("TIPOFACTURA"), res.Get("PERIODO"), res.Get("CVE_CICLO"), res.Get("CVE_NIVEL"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reporte de Personas con Estado de Cuenta");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:K1"].AutoFitColumns();
                    //ws.Column(1).Width = 20;
                    //ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:K1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=ReportePersonasconEstadoDeCuenta.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Reporte de Personas con Estado de Cuenta", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Reporte de Personas con Estado de Cuenta" + e.Message, sesion);

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

        [HttpGet]
        public string ConsultaNiveles(NivelesModel model)
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            string ClaveCiclo = Request.Params["ClaveNivel"];
            StringBuilder sb = new StringBuilder();
            foreach (string str in model.ConsultaPeriodos(ClaveCiclo))
            {
                sb.Append("<option value=\"").Append(str).Append("\">").Append(str).Append("</option>\n");
            }
            return sb.ToString();
        }

    }

}