using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.CatalogosporSede;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Collections;


namespace PagoProfesores.Controllers.CatalogosCentrales
{
    public class SedesController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public SedesController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosCentrales/Sedes/Sedes.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10239,  Element = "Controller" }, //PERMISO ACCESO 
              //   new Factory.Privileges { Permiso = 10006,  Element = "frm-sedes" }, //PERMISO DETALLE 
                 new Factory.Privileges { Permiso = 10240,  Element = "formbtnadd" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10241,  Element = "formbtnsave" }, //PERMISO EDITAR 
                 new Factory.Privileges { Permiso = 10242,  Element = "formbtndelete" }, //PERMISO ELIMINAR 
            };
        }

        public ActionResult Start()
        {
            SessionDB sesion = SessionDB.start(Request, Response, false, db);
            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Centrales", "Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "Cve_Sede", "ASC", sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);
            //ViewBag.ComboSO = CreaCombo("Select * from SOCIEDADES order by SOCIEDAD", "CVE_SOCIEDAD", "SOCIEDAD");
            ViewBag.ComboCA = CreaCombo("Select * from QCampus order by CAMPUSDESCRIPCION", "CAMPUS_INB", "CAMPUSDESCRIPCION");
            ViewBag.ComboTC = CreaCombo("Select * from QTiposContrato order by TIPOCONTRATODESCRIPCION", "TIPOCONTRATO_INB", "TIPOCONTRATODESCRIPCION");
            ViewBag.ComboPAI = CreaCombo("Select * from PAISES order by PAIS", "PAIS", "PAIS");
         


            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Sedes Start", LOG.CONSULTA, "Ingresa pantalla Sedes", sesion);

            return View(Factory.View.Access + "CatalogosCentrales/Sedes/Start.cshtml");
        }


        [HttpPost]
        public string CreaCombo(string Sql = "", string Clave = "", string Valor = "")
        {
            string Salida = "";
            SedesModel model = new SedesModel();
            Salida = model.ComboSql(Sql, Clave, Valor);
            return Salida;
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "QSedes";
            String[] columnas = { "Clave", "Sede", "Campus INB", "Tipo Contrato INB", "Sociedad","Usuario", "Fecha modificación" };
            String[] campos = { "Cve_Sede", "Sede", "CAMPUSDESCRIPCION", "TIPOCONTRATODESCRIPCION", "NOMBRE_SOCIEDADES", "USUARIO", "FECHA_M" };
            string[] campossearch = { "Sociedad", "CVE_Sede", "Sede", "CAMPUSDESCRIPCION", "TIPOCONTRATODESCRIPCION", "USUARIO", "FECHA_M" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "Cve_Sede";

            table.enabledButtonControls = false;

            table.addBtnActions("Editar", "editarSedes");

            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            SessionDB sesion = SessionDB.start(Request, Response, false, db);

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Sociedad", typeof(string));
                tbl.Columns.Add("Clave", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Campus INB", typeof(string));
                tbl.Columns.Add("Tipo Contrato INB", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("Fecha modificación", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM QSedes");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(
                        res.Get("SOCIEDAD"),
                        res.Get("CVE_SEDE"),
                        res.Get("SEDE"),
                        res.Get("CAMPUSDESCRIPCION"),
                        res.Get("TIPOCONTRATODESCRIPCION"),
                        res.Get("USUARIO"),
                        res.Get("FECHA_M")

                    );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sedes");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:G1"].AutoFitColumns();
                    //ws.Column(1).Width = 20;
                    //ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:G1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Sedes.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Sedes", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Sedes" + e.Message, sesion);
            }
        }

        // POST: Sedes/Add
        [HttpPost]
        public ActionResult Add(SedesModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Add())
                {
                    Log.write(this, "Add", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Sede agregado con exito: " + model.Sede) });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al agregar:" + model.Sede) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }

        // POST: Sedes/Edit/5
        [HttpPost]
        public ActionResult Edit(SedesModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: Sedes/Save
        [HttpPost]
        public ActionResult Save(SedesModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Save())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Sede guardado con exito: " + model.Sede) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al guardar Sede = " + model.Sede) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        // POST: Sedes/Delete/5
        [HttpPost]
        public ActionResult Delete(SedesModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Delete())
                {
                    Log.write(this, "Delete", LOG.BORRADO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Sede ELIMINADA con exito: " + model.Sede) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error al Eliminar la Sede = " + model.Sede) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        public JsonResult ListadoSociedades()
        {

            SedesModel datosSoc = new SedesModel();
            var Nombre_sociedad = datosSoc.ListadoSociedades();
            return Json(Nombre_sociedad, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MostrarSociedades(string Cve_SedeMostrar)
        {
            SedesModel datosSoc = new SedesModel();
            var Nombre_sociedad = datosSoc.MostrarSociedades(Cve_SedeMostrar);
            return Json(Nombre_sociedad, JsonRequestBehavior.AllowGet);
        }
    }
}