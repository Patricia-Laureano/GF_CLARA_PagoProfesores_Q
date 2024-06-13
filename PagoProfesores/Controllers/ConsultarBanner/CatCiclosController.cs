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
    public class CatCiclosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public CatCiclosController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/ConsultarBanner/CatCiclos.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10184,  Element = "Controller" }, //PERMISO ACCESO REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10185,  Element = "formbtnConsultar" }, //PERMISO EDITAR REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10186,  Element = "formbtnImportar" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
            };
        }

        // GET: CatCiclos
        public ActionResult Start()
        {
            if(sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            CatCiclosModel model = new CatCiclosModel();
            //SessionDB sesion = SessionDB.start(Request, Response, false, model.db);
            if ((model.sesion = sesion) == null)
                return Content("");
            model.Clean();

            try {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                ViewBag.Main = view.createMenu(22, 45, sesion);

                sesion.vdata["TABLE_CICLOS"] = "CICLOS_TMP";
                sesion.saveSession();

                ViewBag.BlockingPanel_1 = Main.createBlockingPanel("blocking-panel-1");
                ViewBag.BlockingPanel_2 = Main.createBlockingPanel("blocking-panel-2", false, "");
                ViewBag.DataTable = CreateDataTable(10, 1, null, "CVE_CICLO", "DESC", sesion);

                Scripts.SCRIPTS = new string[] { "js/ConsultarBanner/CatCiclos/CatCiclos.js" };

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Ciclos' ", sesion);

                return View(Factory.View.Access + "ConsultarBanner/CatCiclos/Start.cshtml");

            } catch(Exception e) {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa a pantalla 'Ciclos' " + e.Message, sesion);

                return View(Factory.View.Access + "ConsultarBanner/CatCiclos/Start.cshtml");
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

            table.TABLE = sesion.vdata["TABLE_CICLOS"];
            table.COLUMNAS = new string[] { "Reg", "Año Académico", "Ciclo","Disponible"};
            table.CAMPOS = new string[] { "REGISTRADO", "CVE_CICLO", "CICLO","DISPONIBLE"};
            table.CAMPOSSEARCH = new string[] { "CVE_CICLO", "CICLO"};
            table.dictColumnFormat["REGISTRADO"] = delegate (string str, ResultSet res) { return str == "True" ? CheckIcon : ""; };
            //table.dictColumnFormat["IMPORTADO"] = delegate (string str) { return str == "True" ? CheckIcon : ""; };
           

            table.dictColumnFormat["DISPONIBLE"] = delegate (string str, ResultSet res) {

                 string select = "";

                string disponible2 = res.Get("DISPONIBLE");
                if (disponible2 == "True")
                {
                    select = "checked";

                }

       string CheckBox = "<input type = \"checkbox\" id = \"checkdiponible"+ res.Get("CVE_CICLO") + "\"  value=\""+res.Get("CVE_CICLO")+ "\"   onclick=\"formPage.Disponible(this.id,this.value);\"   "+ select + " >";

                string reg = res.Get("REGISTRADO");

               return reg == "True" ? CheckBox : "";

            };

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_CICLO";

            table.enabledCheckbox = true;
            //table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }



        // POST: Pensionados/Add
        [HttpPost]
        public ActionResult setDisponible(CatCiclosModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
            /*
			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });
			//*/
            try
            {
                if (model.setDisponible())
                {
                 //   model.init();
                    Log.write(this, "setDisponible", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("se ha realizado con exito la operación") });
                }
                else
                {
                   // model.init();
                    Log.write(this, "setDisponible", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error en disponible") });
                }
            }

            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
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
                CatCiclosModel[] models = con.connectX<CatCiclosModel[]>(token, "srvCiclos");
                if (models.Length > 0)
                {
                    models[0].sesion = sesion;
                    models[0].Clean();
                    foreach (CatCiclosModel model in models)
                    {
                        model.sesion = sesion;
                        model.CVE_CICLO = model.ANNYO;
                        model.CICLO = model.DESCRIPCION;
                        model.FECHA_R = System.DateTime.Today.ToString();
                        model.FECHA_M = System.DateTime.Today.ToString();
                        model.TMP = false;
                        model.REGISTRADO = model.exist() ? "1" : "0";
                        model.isDisponible();//CONSULTA 1 O 0 DE TABLA (CICLOS) Y LO PONE EN DISPONIBLE DEL MODEL
                        model.IMPORTADO = "0";
                        model.TMP = true;
                        model.addTmp();
                    }
                }
                sesion.vdata["TABLE_CICLOS"] = "CICLOS_TMP";
                sesion.saveSession();
                return "ok";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error." + ex.Message);
            }
            return "-1";// CreateDataTable(10, 1, null, "IDSIU","ASC",sesion);
        }

        public string Importar(string cveCiclos)
        {
            CatCiclosModel model = new CatCiclosModel();
            if ((model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX)) == null)
                return string.Empty;

            if (model.Importar(cveCiclos))
            {
                Log.write(this, "Importar", LOG.EDICION, "cveCiclos:" + cveCiclos, model.sesion);
                return Notification.Succes("Los datos se ha actualizado satisfactoriamente.");
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
                tbl.Columns.Add("Clave del ciclo", typeof(string));
                tbl.Columns.Add("Ciclo", typeof(string));

                ResultSet res = db.getTable("SELECT CASE WHEN REGISTRADO = 1 THEN 'Si' ELSE 'No' END REGISTRADO, CVE_CICLO, CICLO FROM CICLOS_TMP");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("REGISTRADO"), res.Get("CVE_CICLO"), res.Get("CICLO"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catálogo de ciclos banner");

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
                    Response.AddHeader("content-disposition", "attachment;  filename=CatalogoCiclosBanner.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catálogo de ciclos banner", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catálogo de ciclos banner" + e.Message, sesion);

            }

        }
    }
}