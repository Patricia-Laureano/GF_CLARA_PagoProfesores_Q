using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Configuration;

namespace PagoProfesores.Controllers.Sociedades
{
    public class SociedadesController : Controller
    {
        private SessionDB sesion;
        private database db;
        private List<Factory.Privileges> Privileges;

        public SociedadesController()
        {
            db = new database();
            String[] scripts = { "plugins/Angular/jquery.ui.widget.js",
                "plugins/Angular/jquery.iframe-transport.js",
                "plugins/Angular/jquery.fileupload.js",
                "js/CatalogosCentrales/sociedades/sociedades.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                new Factory.Privileges { Permiso = 10235,  Element = "Controller" }, //PERMISO ACCESO BANCOS
                new Factory.Privileges { Permiso = 10236,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
                new Factory.Privileges { Permiso = 10237,  Element = "formbtnsave" }, //PERMISO EDITAR BANCOS
                new Factory.Privileges { Permiso = 10238,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
            };
        }

        // GET: Sociedades    EDITAR
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
                ViewBag.Main = view.createMenu("Catalogos Centrales", "Sociedades", sesion);
                ViewBag.DataTable = CreateDataTable(10, 1, null, "SOCIEDAD", "ASC", sesion);
                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);

                Log.write(this, "Sociedades Start", LOG.CONSULTA, "Ingresa Pantalla Sociedades", sesion);
                return View(Factory.View.Access + "CatalogosCentrales/Sociedades/Start.cshtml");
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Sociedades Start", LOG.ERROR, "Ingresa Pantalla Sociedades" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR

                return View(Factory.View.Access + "CatalogosCentrales/Sociedades/Start.cshtml");
            }
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "V_VSOCIEDAD_MYSUITE";                                                           //EDITAR NOMBRE DE LA TABLA EN LA BASE DE DATOS
            String[] columnas = { "Clave", "Sociedad", "RFC", "CURP", "RFC MYSUITE", "Estado", "Ciudad", "Entidad", "Colonia", "Calle", "Número", "CP", "Correo elctrónico", "Representante legal", "RL RFC", "RL CURP" };
            String[] campos = { "CVE_SOCIEDAD", "SOCIEDAD", "RFC", "CURP", "RFCENTITY", "DIRECCION_ESTADO", "DIRECCION_CIUDAD", "DIRECCION_ENTIDAD", "DIRECCION_COLONIA", "DIRECCION_CALLE", "DIRECCION_NUMERO", "DIRECCION_CP", "EMAIL_SOC", "REPRESENTANTELEGAL", "RL_RFC", "RL_CURP" };
            string[] campossearch = { "CVE_SOCIEDAD", "SOCIEDAD", "RFC", "CURP", "DIRECCION_ESTADO", "DIRECCION_CIUDAD", "DIRECCION_ENTIDAD", "DIRECCION_COLONIA", "DIRECCION_CALLE", "DIRECCION_NUMERO", "EMAIL_SOC", "DIRECCION_CP", "REPRESENTANTELEGAL", "RL_RFC", "RL_CURP" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_SOCIEDAD";                                                     //EDITAR PRIMARYKEY DE LA TABLA
            table.enabledButtonControls = false;
            table.addBtnActions("Editar", "editarSociedad");                                     //EDITAR EL NOMBRE DE LA ACCION
            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Clave", typeof(string));
                tbl.Columns.Add("Sociedad", typeof(string));
                tbl.Columns.Add("RFC", typeof(string));
                tbl.Columns.Add("CURP", typeof(string));
                tbl.Columns.Add("Pais", typeof(string));
                tbl.Columns.Add("Estado", typeof(string));
                tbl.Columns.Add("Ciudad", typeof(string));
                tbl.Columns.Add("Entidad", typeof(string));
                tbl.Columns.Add("Colonia", typeof(string));
                tbl.Columns.Add("Calle", typeof(string));
                tbl.Columns.Add("Numero", typeof(string));
                tbl.Columns.Add("CP", typeof(string));
                tbl.Columns.Add("Correo electrónico", typeof(string));
                tbl.Columns.Add("Representante Legal", typeof(string));
                tbl.Columns.Add("RL RFC", typeof(string));
                tbl.Columns.Add("RL Curp", typeof(string));
                tbl.Columns.Add("RL Pais", typeof(string));
                tbl.Columns.Add("RL Estado", typeof(string));
                tbl.Columns.Add("RL Ciudad", typeof(string));
                tbl.Columns.Add("RL Entidad", typeof(string));
                tbl.Columns.Add("RL Colonia", typeof(string));
                tbl.Columns.Add("RL Calle", typeof(string));
                tbl.Columns.Add("RL Numero", typeof(string));
                tbl.Columns.Add("RL CP", typeof(string));
                tbl.Columns.Add("Firma", typeof(string));
                tbl.Columns.Add("Sello", typeof(string));
                tbl.Columns.Add("Notarial No.", typeof(string));
                tbl.Columns.Add("Notarial Volumen", typeof(string));
                tbl.Columns.Add("Notarial Ciudad", typeof(string));
                tbl.Columns.Add("Notarial Notario No", typeof(string));
                tbl.Columns.Add("Notarial Nombre", typeof(string));

                ResultSet res = db.getTable("select * from Sociedades");

                while (res.Next()) // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("CVE_SOCIEDAD"), res.Get("SOCIEDAD"), res.Get("RFC"), res.Get("CURP"), res.Get("DIRECCION_PAIS"), res.Get("DIRECCION_ESTADO"), res.Get("DIRECCION_CIUDAD"), res.Get("DIRECCION_ENTIDAD"), res.Get("DIRECCION_COLONIA"), res.Get("DIRECCION_CALLE"), res.Get("DIRECCION_NUMERO"), res.Get("DIRECCION_CP"), res.Get("EMAIL_SOC"), res.Get("REPRESENTANTELEGAL"), res.Get("RL_RFC"), res.Get("RL_CURP"), res.Get("RL_DIRECCION_PAIS"), res.Get("RL_DIRECCION_ESTADO"), res.Get("RL_DIRECCION_CIUDAD"), res.Get("RL_DIRECCION_ENTIDAD"), res.Get("RL_DIRECCION_COLONIA"), res.Get("RL_DIRECCION_CALLE"), res.Get("RL_DIRECCION_NUMERO"), res.Get("RL_DIRECCION_CP"), res.Get("FIRMA"), res.Get("SELLO"), res.Get("NOTARIAL_NO"), res.Get("NOTARIAL_VOLUMEN"), res.Get("NOTARIAL_NOTARIO_NO"), res.Get("NOTARIAL_LUGAR"), res.Get("NOTARIAL_LIC"), res.Get("REPRESENTANTE_LEGAL_P"));

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Sociedades");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:AI1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Sociedades.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Sociedades", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Sociedades" + e.Message, sesion);
            }
        }

        // POST: Sociedades/Add  EDITAR NOMBRE DE LA FORMA
        [HttpPost]
        public ActionResult Add(Models.SociedadesModel model)                                    //EDITAR EL NOMBRE DEL MODELO
        {
            if (sesion == null) sesion = SessionDB.start(Request, Response, false, db);
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Add())
                    return Json(new { msg = Notification.Succes("Registro agregado con exito: " + model.Sociedad) });    //EDITAR EL CAMPO DEL MODELO
                else
                    return Json(new { msg = Notification.Error("Erro al agregar: " + model.Sociedad) });     //EDITAR EL CAMPO DEL MODELO        
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }

        // POST: Sociedades/Edit/5   EDITAR EL NOMBRE DE LA FORMA
        [HttpPost]
        public ActionResult Edit(Models.SociedadesModel model)        //EDITAR
        {
            if (model.Edit())
                return Json(new JavaScriptSerializer().Serialize(model));

            return View(Factory.View.Access + "CatalogosCentrales/Sociedades/Start.cshtml");
        }

        // POST: Sociedades/Add    EDITAR
        [HttpPost]
        public ActionResult Save(Models.SociedadesModel model)        //EDITAR
        {
            if (sesion == null) sesion = SessionDB.start(Request, Response, false, db);
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Save())
                    return Json(new { msg = Notification.Succes("Registro guardado con exito: " + model.Sociedad) });   //EDITAR
                else
                    return Json(new { msg = Notification.Error(" Erro al GUARDAR: " + model.Sociedad) });    //EDITAR        
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        // POST: Sociedades/Delete/5 EDITAR
        [HttpPost]
        public ActionResult Delete(Models.SociedadesModel model)      //EDITAR
        {
            if (sesion == null) sesion = SessionDB.start(Request, Response, false, db);
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Delete())
                    return Json(new { msg = Notification.Succes("Registro eliminado con exito: " + model.Sociedad) });  //EDITAR
                else return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.Sociedad) });            //EDITAR
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        public JsonResult ListadoEntidadFederativa()
        {

            Models.SociedadesModel datosSoc = new Models.SociedadesModel();
            var Nombre_sociedad = datosSoc.ListadoEntidadFederativa();
            return Json(Nombre_sociedad, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListadoLlavesMySuite()
        {
            try
            {
                string server = ConfigurationManager.AppSettings["serverMySuite"];
                Models.SociedadesModel datosSoc = new Models.SociedadesModel();
                var Nombre_sociedad = datosSoc.ListadoLlavesMySuite(server);
                return Json(Nombre_sociedad, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
    }
}