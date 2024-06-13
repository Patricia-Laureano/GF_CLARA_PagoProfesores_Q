using ConnectDB;
using Factory;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using PagoProfesores.Models.CatalogosporSede;

namespace PagoProfesores.Controllers.CatalogosporSede
{
    public class ConceptosPagoController : Controller
    {

        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public ConceptosPagoController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosporSede/ConceptosPago/ConceptosPago.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10179,  Element = "Controller" }, //PERMISO ACCESO BANCOS
               //  new Factory.Privileges { Permiso = 10006,  Element = "frm-bancos" }, //PERMISO DETALLE BANCOS
                 new Factory.Privileges { Permiso = 10180,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
                 new Factory.Privileges { Permiso = 10181,  Element = "formbtnsave" }, //PERMISO EDITAR BANCOS
                 new Factory.Privileges { Permiso = 10182,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
            };
        }
        
        // GET: ConceptosPago
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Centrales", "Conceptos de Pago", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "CONCEPTO", "ASC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Bancos Start", LOG.CONSULTA, "Ingresa pantalla Conceptos de Pago", sesion);

            return View(Factory.View.Access + "CatalogosPorSede/ConceptosPago/Start.cshtml");
        }
        
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filter="")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "CONCEPTOSDEPAGO";
            String[] columnas = { "Concepto de Pago", "Descripción", "Usuario", "Fecha modificación" };
            String[] campos = { "CONCEPTO", "CONCEPTO_DES", "USUARIO", "FECHA_M" };
            string[] campossearch = { "CONCEPTO", "CONCEPTO_DES", "USUARIO", "FECHA_M" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;
           // table.TABLECONDICIONSQL = "CVE_SEDE = '"+ filter + "'";

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CONCEPTO";

            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }


        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Concepto", typeof(string));
                tbl.Columns.Add("Descripción", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("Fecha Modificación", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM CONCEPTOSDEPAGO");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("CONCEPTO"), res.Get("CONCEPTO_DES"), res.Get("CVE_SEDE"), res.Get("USUARIO"), res.Get("FECHA_M"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Conceptos de Pago");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:B1"].AutoFitColumns();
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Conceptos de Pago.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Conceptos de Pago", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Conceptos de Pago" + e.Message, sesion);

            }

        }

        // POST: Bancos/Add
        [HttpPost]
        public ActionResult Add(ConceptosPagoModel model)
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
                    return Json(new { msg = Notification.Succes("Registro agregado con exito: " + model.Descripcion) });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al agregar: " + model.Descripcion) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }

        // POST: Bancos/Edit/5
        [HttpPost]
        public ActionResult Edit(ConceptosPagoModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: Bancos/Save
        [HttpPost]
        public ActionResult Save(ConceptosPagoModel model)
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
                    return Json(new { msg = Notification.Succes("Banco guardado con exito: " + model.Descripcion) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al GUARDAR: " + model.Descripcion) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        // POST: Bancos/Delete/5
        [HttpPost]
        public ActionResult Delete(ConceptosPagoModel model)
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
                    return Json(new { msg = Notification.Succes("Banco ELIMINADO con exito: " + model.Descripcion) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.Descripcion) });
                }

            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }


    }
}
