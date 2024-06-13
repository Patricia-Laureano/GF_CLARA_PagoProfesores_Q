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
    public class ImpuestosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        
        public ImpuestosController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosCentrales/Impuestos/Impuestos.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10212,  Element = "Controller" }, //PERMISO ACCESO BANCOS
                // new Factory.Privileges { Permiso = 10006,  Element = "frm-impuestos" }, //PERMISO DETALLE BANCOS
                 new Factory.Privileges { Permiso = 10214,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
                 new Factory.Privileges { Permiso = 10214,  Element = "formbtnsave" }, //PERMISO EDITAR BANCOS
                 new Factory.Privileges { Permiso = 10215,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
            };
        }

        // GET: Impuestos
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Centrales", "Impuestos", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "Rango", "ASC", sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);
            
            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);


            Log.write(this, "Impuestos asimilados Start", LOG.CONSULTA, "Ingresa pantalla Impuestos asimilados", sesion);

            return View(Factory.View.Access + "CatalogosCentrales/Impuestos/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "IMPUESTOSASIMILADOS";
            String[] columnas = { "Rango", "Límite inferior $","Límite superior $","Cuota fija $","% Excedente","Usuario","Fecha Modificación","Año"};
            String[] campos = { "RANGO", "LIMITEINFERIOR", "LIMITESUPERIOR", "CUOTAFIJA", "PORCENTAJEEXCEDENTE","USUARIO","FECHA_M","ANIO" };
            string[] campossearch = { "RANGO", "LIMITEINFERIOR", "LIMITESUPERIOR", "CUOTAFIJA", "PORCENTAJEEXCEDENTE","USUARIO","FECHA_M" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "RANGO";

            table.enabledButtonControls = false;

            table.addBtnActions("Editar", "editarImpuestos");

            return table.CreateDataTable(sesion);
        }

         //#EXPORT EXCEL
        public void ExportExcel()
        {
            SessionDB sesion = SessionDB.start(Request, Response, false, db);

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Rango", typeof(string));
                tbl.Columns.Add("Límite inferior", typeof(string));
                tbl.Columns.Add("Límite superior", typeof(string));
                tbl.Columns.Add("Cuota fija", typeof(string));
                tbl.Columns.Add("% Excedente", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("Fecha modificación", typeof(string));
                tbl.Columns.Add("Año", typeof(string));


                ResultSet res = db.getTable("SELECT * FROM IMPUESTOSASIMILADOS");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(
                        res.Get("RANGO"), 
                        res.Get("LIMITEINFERIOR"), 
                        res.Get("LIMITESUPERIOR"), 
                        res.Get("CUOTAFIJA"), 
                        res.Get("PORCENTAJEEXCEDENTE"),
                        res.Get("USUARIO"),
                        res.Get("FECHA_M"),
                        res.Get("ANIO")
                    );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Impuestos asimilados");

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
                    Response.AddHeader("content-disposition", "attachment;  filename=Impuestos_asimilados.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Export Excel", LOG.CONSULTA, "Exporta Excel Catalogo Impuestos asimilados", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Impuestos asimilados" + e.Message, sesion);
            }
        }

        // POST: Impuestos/Add
        [HttpPost]
        public ActionResult Add(ImpuestosModel model)
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
                    return Json(new { msg = Notification.Succes("Registro agregado con exito: " + model.Rango) });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error al agregar:" + model.Rango) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }

        // POST: Impuestos/Edit/5
        [HttpPost]
        public ActionResult Edit(ImpuestosModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: Impuestos/Save
        [HttpPost]
        public ActionResult Save(ImpuestosModel model)
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
                    return Json(new { msg = Notification.Succes("Rango guardado con exito: " + model.Rango) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al guardar Rango = " + model.Rango) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        // POST: Impuestos/Delete/5
        [HttpPost]
        public ActionResult Delete(ImpuestosModel model)
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
                    return Json(new { msg = Notification.Succes("Rango de impuestos ELIMINADO con exito: " + model.Rango) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error al Eliminar el Rango de impuestos = " + model.Rango) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }
    }
}
