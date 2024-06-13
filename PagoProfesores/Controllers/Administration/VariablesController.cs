using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.Administration;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Collections;

namespace PagoProfesores.Controllers.Administration
{
    public class VariablesController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;


        public VariablesController()
        {
            db = new database();

            string[] scripts = { "js/Administracion/Variables/variables.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10248,  Element = "Controller" }, //PERMISO ACCESO BANCOS
             //    new Factory.Privileges { Permiso = 10006,  Element = "frm-variables" }, //PERMISO DETALLE BANCOS
                 new Factory.Privileges { Permiso = 10249,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
                 new Factory.Privileges { Permiso = 10250,  Element = "formbtnsave" }, //PERMISO EDITAR BANCOS
                 new Factory.Privileges { Permiso = 10251,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
            };

        }

        // GET: Impuestos
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Administración", "Variables", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "VARIABLE", "ASC", sesion);

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);


            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);


            Log.write(this, "Impuestos asimilados Start", LOG.CONSULTA, "Ingresa variables asimilados", sesion);

            return View(Factory.View.Access + "Administracion/Variables/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "VARIABLES";
            String[] columnas = { "Variable", "Valor", "Descripción","Usuario", "Fecha Modificación" };
            String[] campos = { "VARIABLE", "VALOR", "DESCRIPCION","USUARIO","FECHA_M"};
            string[] campossearch = { "VARIABLE", "VALOR", "DESCRIPCION" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "VARIABLE";

            table.enabledButtonControls = false;

          //  table.addBtnActions("Editar", "editarImpuestos");

            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            SessionDB sesion = SessionDB.start(Request, Response, false, db);                    
           

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Variable", typeof(string));
                tbl.Columns.Add("Valor", typeof(string));
                tbl.Columns.Add("Descripción", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("Fecha Modificación", typeof(string));               


                ResultSet res = db.getTable("SELECT * FROM VARIABLES");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(
                        res.Get("VARIABLE"),
                        res.Get("VALOR"),
                        res.Get("DESCRIPCION"),                   
                        res.Get("USUARIO"),
                        res.Get("FECHA_M")
                    );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Variables");

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
                    Response.AddHeader("content-disposition", "attachment;  filename=Variables.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Export Excel", LOG.CONSULTA, "Exporta Excel Catalogo Variables", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Variables" + e.Message, sesion);
            }
        }


        // POST: Impuestos/Edit/5
        [HttpPost]
        public ActionResult Edit(VariableModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }


        // POST: Impuestos/Save
        [HttpPost]
        public ActionResult Save(VariableModel model)
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
                    return Json(new { msg = Notification.Succes("Variable guardado con exito: " + model.Variable) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al guardar Rango = " + model.Variable) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }







    }
}