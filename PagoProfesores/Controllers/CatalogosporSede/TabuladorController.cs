using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.CatalogosPorSede;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.CatalogosporSede
{
    public class TabuladorController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public TabuladorController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosPorSede/Tabulador/Tabulador.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10163,  Element = "Controller" }, //PERMISO ACCESO BANCOS
               //  new Factory.Privileges { Permiso = 10006,  Element = "frm-tabulador" }, //PERMISO DETALLE BANCOS
                 new Factory.Privileges { Permiso = 10165,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
                 new Factory.Privileges { Permiso = 10166,  Element = "formbtnsave" }, //PERMISO EDITAR BANCOS
                 new Factory.Privileges { Permiso = 10167,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
            };

        }

        // GET: Tabulador
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Por Sede", "Tabulador", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "CVE_NIVEL", "ASC");

            ViewBag.Niveles = getNiveles();
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Tabulador Start", LOG.CONSULTA, "Ingresa pantalla Tabulador", sesion);

            return View(Factory.View.Access + "CatalogosPorSede/Tabulador/Start.cshtml");
        }
        
        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filter = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();


            table.TABLE = "TABULADOR";
            String[] columnas = { "Nivel", "Tabulador", "Monto Por Hora", "Sede","Usuario","Fecha de Modificación" };
            String[] campos = { "PK1","CVE_NIVEL", "TABULADOR", "Monto", "CVE_SEDE", "USUARIO","FECHA_M" };
            string[] campossearch = { "CVE_NIVEL", "TABULADOR", "Monto", "CVE_SEDE" };
            string[] camposhidden = { "PK1"};


            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSHIDDEN = camposhidden;
            table.CAMPOSSEARCH = campossearch;
            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "PK1";

            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);

        }


        public string getNiveles(string Clave = "")
        {
            TabuladorModel model = new TabuladorModel();
            string niveles = "";
            niveles = model.Niveles(Clave);
        
            return niveles;
        }


        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }


            string sede = Request.Params["sede"];

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Clave Nivel", typeof(string));
                tbl.Columns.Add("Clave Sede", typeof(string));
                tbl.Columns.Add("Tabulador", typeof(string));
                tbl.Columns.Add("Monto por Hora", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM TABULADOR WHERE CVE_SEDE = '"+ sede +"'");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("CVE_NIVEL"), res.Get("CVE_SEDE"), res.Get("TABULADOR"), res.Get("MONTO"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Tabulador");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 40;
                    ws.Column(3).Width = 60;
                    ws.Column(4).Width = 60;

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
                    Response.AddHeader("content-disposition", "attachment;  filename=Tabulador.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Tabulador", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Tabulador" + e.Message, sesion);

            }

        }


        // POST: Tabulador/Add
        [HttpPost]
        public ActionResult Add(TabuladorModel model)
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
                    return Json(new { msg = Notification.Succes("Tabulador agregado con exito") });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al agregar Ta bulador") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }


        // POST: Bancos/Edit/5
        [HttpPost]
        public ActionResult Edit(TabuladorModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }


        // POST: Tabulador/Save
        [HttpPost]
        public ActionResult Save(TabuladorModel model)
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
                    return Json(new { msg = Notification.Succes("Tabulador guardado con exito: " + model.ClaveNivel) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Guardar: " + model.ClaveNivel) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }


        // POST: Tabulador/Delete/5
        [HttpPost]
        public ActionResult Delete(TabuladorModel model)
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
                    return Json(new { msg = Notification.Succes("Tabulador eliminado con exito: " + model.ClaveNivel) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.ClaveNivel) });
                }

            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }


    }
}
