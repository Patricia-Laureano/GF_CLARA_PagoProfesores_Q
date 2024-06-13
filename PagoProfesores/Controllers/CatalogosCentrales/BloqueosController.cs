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
    public class BloqueosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public BloqueosController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosCentrales/Bloqueos/Bloqueos.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10216,  Element = "Controller" }, //PERMISO ACCESO Bloqueos
                // new Factory.Privileges { Permiso = 10006,  Element = "frm-bloqueos" }, //PERMISO DETALLE Bloqueos
                 new Factory.Privileges { Permiso = 10217,  Element = "formbtnadd" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10218,  Element = "formbtnsave" }, //PERMISO EDITAR 
                 new Factory.Privileges { Permiso = 10219,  Element = "formbtndelete" }, //PERMISO ELIMINAR 
            };
        }

        public ActionResult Start()
        {

			if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Centrales", "Bloqueos", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "Cve_Bloqueo", "ASC", sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Bloqueos Start", LOG.CONSULTA, "Ingresa Pantalla Bloqueos", sesion);

            return View(Factory.View.Access + "CatalogosCentrales/Bloqueos/Start.cshtml");
        }

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
			string CheckIcon = "<div style='text-align: center'><i class='fa fa-check'></i></div>";
			
			DataTable table = new DataTable();

			table.TABLE = "BLOQUEOS";

			table.COLUMNAS = new string[] { "Clave", "Bloqueo", "Descripción", "Estado cuenta", "Factura", "Pagos" };
			table.CAMPOS = new string[] { "CVE_BLOQUEO", "BLOQUEO", "BLOQUEODESCRIPCION", "ESTADOCUENTA", "FACTURA", "PAGOS" };
			table.CAMPOSSEARCH = new string[] { "CVE_BLOQUEO", "BLOQUEO" };

			table.addColumnFormat("ESTADOCUENTA", delegate (string value, ResultSet res) {
				return value == "True" ? CheckIcon : "";
			});

			table.addColumnFormat("FACTURA", delegate (string value, ResultSet res) {
				return value == "True" ? CheckIcon : "";
			});

			table.addColumnFormat("PAGOS", delegate (string value, ResultSet res) {
				return value == "True" ? CheckIcon : "";
			});

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "CVE_BLOQUEO";

			table.enabledButtonControls = false;

			table.addBtnActions("Editar", "editarBloqueos");

			return table.CreateDataTable(sesion);
		}

         //#EXPORT EXCEL
        public void ExportExcel()
        {
            SessionDB sesion = SessionDB.start(Request, Response, false, db);

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Clave", typeof(string));
                tbl.Columns.Add("Bloqueo", typeof(string));
                tbl.Columns.Add("Descripción", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("Fecha modificación", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM BLOQUEOS");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(
                        res.Get("CVE_BLOQUEO"),
                        res.Get("BLOQUEO"),
                        res.Get("BLOQUEODESCRIPCION"),
                        res.Get("USUARIO"),
                        res.Get("FECHA_M")
                    );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Bloqueos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:E1"].AutoFitColumns();
                    //ws.Column(1).Width = 20;
                    //ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:E1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Bloqueos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Bloqueos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Bloqueos" + e.Message, sesion);
            }
        }

        // POST: Bloqueos/Add
        [HttpPost]
        public ActionResult Add(BloqueosModel model)
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
                    return Json(new { msg = Notification.Succes("Bloqueo agregado con exito: " + model.Bloqueo) });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al agregar:" + model.Bloqueo) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }

        // POST: Bloqueos/Edit/5
        [HttpPost]
        public ActionResult Edit(BloqueosModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: Bloqueos/Save
        [HttpPost]
        public ActionResult Save(BloqueosModel model)
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
                    return Json(new { msg = Notification.Succes("Bloqueo guardado con exito: " + model.Bloqueo) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al guardar Bloqueo = " + model.Bloqueo) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        // POST: Bloqueos/Delete/5
        [HttpPost]
        public ActionResult Delete(BloqueosModel model)
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
                    return Json(new { msg = Notification.Succes("Bloqueo ELIMINADO con exito: " + model.Bloqueo) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error al Eliminar el Bloqueo = " + model.Bloqueo) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }
    }
}
