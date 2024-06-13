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
    public class TiposPagoController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public TiposPagoController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosCentrales/TiposPago/TiposPago.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10231,  Element = "Controller" }, //PERMISO ACCESO 
                // new Factory.Privileges { Permiso = 10006,  Element = "frm-tipospago" }, //PERMISO DETALLE 
                 new Factory.Privileges { Permiso = 10232,  Element = "formbtnadd" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10233,  Element = "formbtnsave" }, //PERMISO EDITAR 
                 new Factory.Privileges { Permiso = 10234,  Element = "formbtndelete" }, //PERMISO ELIMINAR 
            };
        }

        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Centrales", "Tipo de Pagos", sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.DataTable = CreateDataTable(10, 1, null, "CVE_TIPODEPAGO", "ASC");

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            ViewBag.ComboTF = CreaCombo("Select * from TIPOSFACTURA order by TIPOFACTURA", "CVE_TIPOFACTURA", "TIPOFACTURA");

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Tipos de Pago Start", LOG.CONSULTA, "Ingresa pantalla Tipos de Pago", sesion);

            return View(Factory.View.Access + "CatalogosCentrales/TiposPago/Start.cshtml");
        }

        [HttpPost]
        public string CreaCombo(string Sql = "", string Clave = "", string Valor = "")
        {
            string Salida = "";
            TiposPagoModel model = new TiposPagoModel();
            Salida = model.ComboSql(Sql, Clave, Valor);
            return Salida;
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "QTiposdePago";
            String[] columnas = { "Tipo Factura","Clave", "Tipo de Pago","Descripción","Usuario","Fecha modificación" };
            String[] campos = { "TIPOFACTURA", "CVE_TIPODEPAGO", "TIPODEPAGO", "TIPODEPAGODESCRIPCION","USUARIO","FECHA_M" };
            string[] campossearch = { "TIPOFACTURA", "CVE_TIPODEPAGO", "TIPODEPAGO", "TIPODEPAGODESCRIPCION" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_TIPODEPAGO";

            table.enabledButtonControls = false;

            table.addBtnActions("Editar", "editarTiposPago");

            return table.CreateDataTable(sesion);
        }

         //#EXPORT EXCEL
        public void ExportExcel()
        {
            SessionDB sesion = SessionDB.start(Request, Response, false, db);

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Tipo Factura", typeof(string));
                tbl.Columns.Add("Clave", typeof(string));
                tbl.Columns.Add("Tipo de Pago", typeof(string));
                tbl.Columns.Add("Descripción", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("Fecha modificación", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM QTiposdePago");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(
                        res.Get("TIPOFACTURA"),
                        res.Get("CVE_TIPODEPAGO"),
                        res.Get("TIPODEPAGO"),
                        res.Get("TIPODEPAGODESCRIPCION"),
                        res.Get("USUARIO"),
                        res.Get("FECHA_M")

                    );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TiposPago");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:F1"].AutoFitColumns();
                    //ws.Column(1).Width = 20;
                    //ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:F1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Tipos de Pago.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo TiposPago", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo TiposPago" + e.Message, sesion);
            }
        }

        // POST: TiposPago/Add
        [HttpPost]
        public ActionResult Add(TiposPagoModel model)
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
                    return Json(new { msg = Notification.Succes("Tipo de Pago agregado con exito: " + model.TipodePago) });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al agregar:" + model.TipodePago) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }

        // POST: TiposPago/Edit/5
        [HttpPost]
        public ActionResult Edit(TiposPagoModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: TiposPago/Save
        [HttpPost]
        public ActionResult Save(TiposPagoModel model)
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
                    return Json(new { msg = Notification.Succes("Tipo de Pago guardado con exito: " + model.TipodePago) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al guardar Tipo de Pago = " + model.TipodePago) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        // POST: TiposPago/Delete/5
        [HttpPost]
        public ActionResult Delete(TiposPagoModel model)
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
                    return Json(new { msg = Notification.Succes("Tipo de Pago ELIMINADO con exito: " + model.TipodePago) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error al Eliminar el Tipo de Pago = " + model.TipodePago) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }
    }
}
