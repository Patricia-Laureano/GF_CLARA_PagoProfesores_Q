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
    public class TiposFacturaController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public TiposFacturaController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosCentrales/TiposFactura/TiposFactura.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10227,  Element = "Controller" }, //PERMISO ACCESO 
                // new Factory.Privileges { Permiso = 10006,  Element = "frm-tiposfactura" }, //PERMISO DETALLE 
                 new Factory.Privileges { Permiso = 10228,  Element = "formbtnadd" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10229,  Element = "formbtnsave" }, //PERMISO EDITAR 
                 new Factory.Privileges { Permiso = 10230,  Element = "formbtndelete" }, //PERMISO ELIMINAR 
            };
        }

        // GET: TiposFactura
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Centrales", "Tipo de Facturas", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "Cve_TipoFactura", "ASC", sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "TiposFactura Start", LOG.CONSULTA, "Ingresa pantalla TiposFactura", sesion);

            return View(Factory.View.Access + "CatalogosCentrales/TiposFactura/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "TiposFactura";
            String[] columnas = { "Clave", "Tipo de Factura","Descripción","Usuario","Fecha modificación" };
            String[] campos = { "CVE_TIPOFACTURA", "TIPOFACTURA", "TIPOFACTURADESCRIPCION","USUARIO","FECHA_M" };
            string[] campossearch = { "CVE_TIPOFACTURA", "TIPOFACTURA", "TIPOFACTURADESCRIPCION","USUARIO","FECHA_M" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_TIPOFACTURA";

            table.enabledButtonControls = false;

            table.addBtnActions("Editar", "editarTiposFactura");

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
                tbl.Columns.Add("Tipo de Factura", typeof(string));
                tbl.Columns.Add("Descripción", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("Fecha modificación", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM TiposFactura");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(
                        res.Get("CVE_TIPOFACTURA"),
                        res.Get("TIPOFACTURA"),
                        res.Get("TIPOFACTURADESCRIPCION"),
                        res.Get("USUARIO"),
                        res.Get("FECHA_M")
                    );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Tipos de Factura");

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
                    Response.AddHeader("content-disposition", "attachment;  filename=Tipos de Factura.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo TiposFactura", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo TiposFactura" + e.Message, sesion);
            }
        }

        // POST: TiposFactura/Add
        [HttpPost]
        public ActionResult Add(TiposFacturaModel model)
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
                    return Json(new { msg = Notification.Succes("Tipo de Factura agregado con exito: " + model.TipoFactura) });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al agregar:" + model.TipoFactura) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }

        // POST: TiposFactura/Edit/5
        [HttpPost]
        public ActionResult Edit(TiposFacturaModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: TiposFactura/Save
        [HttpPost]
        public ActionResult Save(TiposFacturaModel model)
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
                    return Json(new { msg = Notification.Succes("Tipo de Factura guardado con exito: " + model.TipoFactura) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al guardar Tipo de Factura = " + model.TipoFactura) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        // POST: TiposFactura/Delete/5
        [HttpPost]
        public ActionResult Delete(TiposFacturaModel model)
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
                    return Json(new { msg = Notification.Succes("Tipo de Factura ELIMINADO con exito: " + model.TipoFactura) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error al Eliminar el Tipo de Factura = " + model.TipoFactura) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }
    }
}
