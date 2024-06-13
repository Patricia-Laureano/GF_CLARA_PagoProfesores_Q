using ConnectDB;
using ConnectUrlToken;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.ConsultarBanner;
using Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.ConsultarBanner
{
    public class CatTiposContratoController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public CatTiposContratoController()
        {
            db = new database();

            string[] scripts = { "js/ConsultarBanner/CatTiposContrato/CatTiposContrato.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10196,  Element = "Controller" }, //PERMISO ACCESO REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10197,  Element = "formbtnConsultar" }, //PERMISO EDITAR REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10198,  Element = "formbtnImportar" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
            };
        }

        // GET: CatTiposContrato
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            CatTiposContratoModel model = new CatTiposContratoModel();
            //SessionDB sesion = SessionDB.start(Request, Response, false, model.db);
            if ((model.sesion = sesion) == null)
                return Content("");
            model.Clean();

            try
            {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                ViewBag.Main = view.createMenu(22, 45, sesion);

                sesion.vdata["TABLE_TIPOSCONTRATO"] = "TIPOSCONTRATO_TMP";
                sesion.saveSession();

                ViewBag.BlockingPanel_1 = Main.createBlockingPanel("blocking-panel-1");
                ViewBag.DataTable = CreateDataTable(10, 1, null, "TIPOCONTRATO_INB", "ASC", sesion);

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);
                
                Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Tipos de contrato' ", sesion);

                return View(Factory.View.Access + "ConsultarBanner/CatTiposContrato/Start.cshtml");
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa a pantalla 'Tipos de contrato' " + e.Message, sesion);

                return View(Factory.View.Access + "ConsultarBanner/CatTiposContrato/Start.cshtml");
            }
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show, int pg, string search, string orderby, string sort, SessionDB sesion)
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, new database(), SESSION_BEHAVIOR.AJAX)) == null)
                    return string.Empty;

            DataTable table = new DataTable();
            string CheckIcon = "<i class=\"fa fa-check\"></i>";

            table.TABLE = sesion.vdata["TABLE_TIPOSCONTRATO"];
            table.COLUMNAS = new string[] { "Reg", "Tipo de Contrato", "Descripción" };
            table.CAMPOS = new string[] { "REGISTRADO", "TIPOCONTRATO_INB", "TIPOCONTRATODESCRIPCION" };
            table.CAMPOSSEARCH = new string[] { "TIPOCONTRATO_INB", "TIPOCONTRATODESCRIPCION" };
            table.dictColumnFormat["REGISTRADO"] = delegate (string str, ResultSet res) { return str == "True" ? CheckIcon : ""; };
            //table.dictColumnFormat["IMPORTADO"] = delegate (string str) { return str == "True" ? CheckIcon : ""; };

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "TIPOCONTRATO_INB";

            table.enabledCheckbox = true;

            return table.CreateDataTable(sesion);
        }

        [HttpGet]
        public string Consultar()
        {
            //SessionDB sesion;
            if ((sesion = SessionDB.start(Request, Response, false, new database(), SESSION_BEHAVIOR.AJAX)) == null)
                return string.Empty;

            string paURL = ConfigurationManager.AppSettings["xURL"];
            string paUser = ConfigurationManager.AppSettings["xUser"];
            string paSecret = ConfigurationManager.AppSettings["xSecret"];
            string paFormat = ConfigurationManager.AppSettings["xFormat"];

            ConnectUrlToken.ConnectUrlToken con = new ConnectUrlToken.ConnectUrlToken(paURL, paUser, paSecret, paFormat);
            Token token = con.getToken();

            try
            {
                CatTiposContratoModel[] models = con.connectX<CatTiposContratoModel[]>(token, "srvTipoContrato");
                if (models.Length > 0)
                {
                    models[0].sesion = sesion;
                    models[0].Clean();
                    foreach (CatTiposContratoModel model in models)
                    {
                        model.sesion = sesion;
                        model.TIPOCONTRATO_INB = model.CONTRATO;
                        model.TIPOCONTRATODESCRIPCION = model.DESCRIPCION;
                        model.FECHA_R = System.DateTime.Today.ToString();
                        model.FECHA_M = System.DateTime.Today.ToString();
                        model.TMP = false;
                        model.REGISTRADO = model.exist() ? "1" : "0";
                        model.IMPORTADO = "0";
                        model.TMP = true;
                        model.addTmp();
                    }
                }
                sesion.vdata["TABLE_TIPOSCONTRATO"] = "TIPOSCONTRATO_TMP";
                sesion.saveSession();
                return "ok";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error." + ex.Message);
            }
            return "-1";// CreateDataTable(10, 1, null, "IDSIU","ASC",sesion);
        }

        public string Importar(string tipoContrato)
        {
            CatTiposContratoModel model = new CatTiposContratoModel();
            if ((model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX)) == null)
                return string.Empty;

            if (model.Importar(tipoContrato))
            {
                Log.write(this, "Importar", LOG.EDICION, "tipoContrato:" + tipoContrato, model.sesion);
                return Notification.Succes("Los datos se han actualizado satisfactoriamente.");
            }
            else
                return Notification.Error("No se ha podido hacer la importaci&ocuate;n");
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Registrado", typeof(string));
                tbl.Columns.Add("Tipo de contrato INB", typeof(string));
                tbl.Columns.Add("Descripción de contrato", typeof(string));

                ResultSet res = db.getTable("SELECT CASE WHEN REGISTRADO = 1 THEN 'Si' ELSE 'No' END REGISTRADO, TIPOCONTRATO_INB, TIPOCONTRATODESCRIPCION FROM TIPOSCONTRATO_TMP");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("REGISTRADO"), res.Get("TIPOCONTRATO_INB"), res.Get("TIPOCONTRATODESCRIPCION"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catálogo de tipos de contrato banner");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:C1"].AutoFitColumns();
                    //ws.Column(1).Width = 20;
                    //ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:C1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=CatalogoTiposContratoBanner.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catálogo de tipos de contrato banner", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catálogo de tipos de contrato banner" + e.Message, sesion);

            }

        }

    }
}