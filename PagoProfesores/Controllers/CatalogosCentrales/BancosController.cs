using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.CatalogosporSede;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace PagoProfesores.Controllers.CatalogosCentrales
{
    public class BancosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public BancosController()
        {
            db = new database();
            
            string[] scripts = { "js/CatalogosCentrales/bancos/bancos.js"};
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10174,  Element = "Controller" }, //PERMISO ACCESO BANCOS
                 new Factory.Privileges { Permiso = 10175,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
                 new Factory.Privileges { Permiso = 10176,  Element = "formbtnsave" }, //PERMISO EDITAR BANCOS
                 new Factory.Privileges { Permiso = 10177,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
            };
        }

        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            
            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Centrales", "Bancos", sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.DataTable = CreateDataTable(10, 1, null, "CVE_BANCO", "ASC"); 

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges,sesion);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Bancos Start", LOG.CONSULTA, "Ingresa pantalla Bancos", sesion);

            return View(Factory.View.Access+"CatalogosCentrales/Bancos/Start.cshtml");

        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "BANCOS";
            String[] columnas = { "Clave", "Banco","Transferencias", "Usuario","Fecha modificación" };
            String[] campos = { "CVE_BANCO", "BANCO", "TRANSFERENCIAS", "USUARIO", "FECHA_M" };
            string[] campossearch = { "CVE_BANCO", "BANCO", "TRANSFERENCIAS" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_BANCO";
            
            table.enabledButtonControls = false;
            table.addBtnActions("Editar", "editarBanco");

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
                tbl.Columns.Add("Banco", typeof(string));
                tbl.Columns.Add("Transferencias", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("Fecha Modificación", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM BANCOS");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("CVE_BANCO"), res.Get("BANCO"), res.Get("TRANSFERENCIAS"), res.Get("USUARIO"), res.Get("FECHA_M"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Bancos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:B1"].AutoFitColumns();

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
                    Response.AddHeader("content-disposition", "attachment;  filename=Bancos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Bancos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Bancos" + e.Message, sesion);
            }
        }

        // POST: Bancos/Add
        [HttpPost]
        public ActionResult Add(BancosModel model)
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
                    return Json(new { msg = Notification.Succes("Registro agregado con exito: " + model.Banco) });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al agregar: " + model.Banco) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }

        // POST: Bancos/Edit/5
        [HttpPost]
        public ActionResult Edit(BancosModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: Bancos/Save
        [HttpPost]
        public ActionResult Save(BancosModel model)
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
                    return Json(new { msg = Notification.Succes("Banco guardado con exito: " + model.Banco) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al GUARDAR: " + model.Banco) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        // POST: Bancos/Delete/5
        [HttpPost]
        public ActionResult Delete(BancosModel model)
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
                    return Json(new { msg = Notification.Succes("Banco ELIMINADO con exito: " + model.Banco) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.Banco) });
                }

            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }
    }
}
