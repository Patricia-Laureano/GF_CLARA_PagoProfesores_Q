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
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.ConsultarBanner
{
    public class CatPartePeriodosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public CatPartePeriodosController()
        {
            db = new database();

            string[] scripts = { "js/ConsultarBanner/CatPartePeriodos/CatPartePeriodos.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10190,  Element = "Controller" }, //PERMISO ACCESO REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10191,  Element = "formbtnConsultar" }, //PERMISO EDITAR REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10192,  Element = "formbtnImportar" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
            };
        }

        // GET: CatPeriodos
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            CatPartePeriodosModel model = new CatPartePeriodosModel();
            if ((model.sesion = sesion) == null)
                return Content("");
            model.Clean();

            try
            {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                ViewBag.Main = view.createMenu(22, 45, sesion);

                sesion.vdata["TABLE_PERIODOS"] = "PARTE_PERIODOS_TMP";
                sesion.saveSession();

                ViewBag.BlockingPanel_1 = Main.createBlockingPanel("blocking-panel-1");
                ViewBag.DataTable = CreateDataTable(10, 1, null, "PERIODO", "ASC", sesion);

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Parte Periodos' ", sesion);

                return View(Factory.View.Access + "ConsultarBanner/CatPartePeriodos/Start.cshtml");
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa a pantalla 'Periodos' " + e.Message, sesion);

                return View(Factory.View.Access + "ConsultarBanner/CatPartePeriodos/Start.cshtml");
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

            table.TABLE = sesion.vdata["TABLE_PERIODOS"];
            table.COLUMNAS = new string[] { "Reg", "Parte de Periodo", "Descripción" };
            table.CAMPOS = new string[] { "REGISTRADO", "PERIODO", "DESCRIPCION" };
            table.CAMPOSSEARCH = new string[] { "PERIODO", "DESCRIPCION" };
            table.dictColumnFormat["REGISTRADO"] = delegate (string str, ResultSet res) { return str == "True" ? CheckIcon : ""; };

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "PERIODO";

            if (sesion != null)
                table.TABLECONDICIONSQL = " USUARIO = " + sesion.pkUser;

            table.enabledCheckbox = true;

            return table.CreateDataTable(sesion);
        }

        [HttpPost]
        public string CreaCombo(string Sql = "", string Clave = "", string Valor = "", string Inicial = "")
        {
            string Salida = "";
            CatPeriodosModel model = new CatPeriodosModel();
            Salida = model.ComboSql(Sql, Clave, Valor, Inicial);
            return Salida;
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

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_json = serializer.Serialize(
                new ServicePartePeriodo
                {

                });

            Token token = con.getToken();

            try
            {
                CatPartePeriodosModel[] models = con.connectX<CatPartePeriodosModel[]>(token, "srvPartePeriodo", str_json);
                if (models.Length > 0)
                {
                    models[0].sesion = sesion;
                    models[0].Clean();
                    foreach (CatPartePeriodosModel model in models)
                    {
                        model.sesion = sesion;
                        model.FECHA_R = System.DateTime.Today.ToString();
                        model.FECHA_M = System.DateTime.Today.ToString();
                        model.TMP = false;
                        model.REGISTRADO = model.exist() ? "1" : "0";
                        model.TMP = true;
                        model.addTmp();
                    }
                }
                sesion.vdata["TABLE_PERIODOS"] = "PARTE_PERIODOS_TMP";
                sesion.saveSession();

                return "ok";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error." + ex.Message);
            }
            return "-1";// CreateDataTable(10, 1, null, "IDSIU","ASC",sesion);
        }

        public string Importar(string periodos)
        {
            CatPartePeriodosModel model = new CatPartePeriodosModel();
            if ((model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX)) == null)
                return string.Empty;

            if (model.Importar(periodos))
            {
                Log.write(this, "Importar", LOG.EDICION, "periodos:" + periodos, model.sesion);
                return Notification.Succes("Los datos se han actualizado satisfactoriamente.");
            }
            else
                return Notification.Error("No se ha podido hacer la importaci&ocuate;n");
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            var sql = "SELECT CASE WHEN REGISTRADO = 1 THEN 'Si' ELSE 'No' END REGISTRADO, PERIODO, DESCRIPCION FROM PARTE_PERIODOS_TMP";
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Registrado", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Descripción", typeof(string));

                if (sesion != null)
                    sql += " where USUARIO = " + sesion.pkUser;

                ResultSet res = db.getTable(sql);

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("REGISTRADO"), res.Get("PERIODO"), res.Get("DESCRIPCION"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catálogo de parte periodos banner");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:D1"].AutoFitColumns();
                    //ws.Column(1).Width = 20;
                    //ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:D1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=CatalogoPartePeriodosBanner.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catálogo de periodos banner", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catálogo de periodos banner" + e.Message, sesion);
            }
        }
    }

    public class ServicePartePeriodo
    {
    }
}