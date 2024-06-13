using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.CatalogosporSede;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.CatalogosporSede
{
    public class EscuelasCCController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        
        public EscuelasCCController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosPorSede/EscuelasCC/escuelasCC.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10040,  Element = "Controller" }, //PERMISO ACCESO BANCOS
               //  new Factory.Privileges { Permiso = 10006,  Element = "frm-tabulador" }, //PERMISO DETALLE BANCOS
               //  new Factory.Privileges { Permiso = 10165,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
               //  new Factory.Privileges { Permiso = 10166,  Element = "formbtnsave" }, //PERMISO EDITAR BANCOS
               //  new Factory.Privileges { Permiso = 10167,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
            };
        }

        // GET: EscuelasCC
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Por Sede", "Escuelas CC", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "CVE_ESCUELA", "ASC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            getEscuelasCC(sesion.vdata["Sede"]);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "EscuelasCC Start", LOG.CONSULTA, "Ingresa pantalla EscuelasCC", sesion);

            return View(Factory.View.Access + "CatalogosPorSede/EscuelasCC/Start.cshtml");
        }

        public void getEscuelasCC(string sedes)
        {
            try
            {
                string sql = "";
                string sql2 = "";
                string sql3 = "";
                ResultSet res = null;
                ResultSet res2 = null;

                sql = "SELECT * FROM ESCUELAS";
                res = db.getTable(sql);

                while (res.Next())
                {

                    sql2 = "SELECT * FROM CENTRODECOSTOS_ESCUELAS WHERE CVE_ESCUELA = '" + res.Get("CVE_ESCUELA") + "' AND CVE_SEDE = '" + sedes + "'";
                    res2 = db.getTable(sql2);

                    if (!res2.Next())//existe
                    {
                        sql3 = "INSERT INTO CENTRODECOSTOS_ESCUELAS(";
                        sql3 += "CVE_ESCUELA";
                        sql3 += ",CVE_SEDE";
                        sql3 += ",TIPO";
                        sql3 += ") VALUES(";
                        sql3 += "'" + res.Get("CVE_ESCUELA") + "'";
                        sql3 += ",'" + sedes + "'";
                        sql3 += ",'D'";
                        sql3 += ")";

                        db.executeId(sql3);
                    }
                }
            }
            catch (Exception e) { }
        }
        
        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filter = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            DataTable table = new DataTable();

            table.TABLE = "VCENTRODECOSTOS_ESCUELAS";

            table.COLUMNAS =
                new string[] { "CLAVE", "ESCUELA", "SEDE", "TIPO" };
            table.CAMPOS =
                new string[] { "CVE_ESCUELA", "ESCUELA", "CVE_SEDE","TIPO" };
            table.CAMPOSSEARCH =
                new string[] { "CVE_ESCUELA", "ESCUELA" };

            table.dictColumnFormat.Add("TIPO", delegate (string value, ResultSet res) {

                string directo = "";
                string indirecto = "";               

                if (value == "D") {
                    directo = "selected";
                    indirecto = "";
                }
                else {
                    directo = "";
                    indirecto = "selected";
                }

               string combo = "<div style=\"width:30px; background-color:;color:;\" ><select id=\"tipo"+ res.Get("CVE_ESCUELA") + "\"    onchange=\"Edit('"+ res.Get("CVE_ESCUELA") + "',this)\"><option value = \"D\" " + directo + " >Directo </option><option value = \"I\" " + indirecto + ">Indirecto</option></select></div>";

                return combo;
            });
            
            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_ESCUELA";
            
            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";

            table.enabledButtonControls = false;
            
            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            string sede = Request.Params["sede"];

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Clave", typeof(string));
                tbl.Columns.Add("Escuela", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Tipo", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM VCENTRODECOSTOS_ESCUELAS WHERE CVE_SEDE = '"+ sede + "'");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("CVE_ESCUELA"), res.Get("ESCUELA"), res.Get("CVE_SEDE"), res.Get("TIPO"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Escuelas_CC");

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
                    Response.AddHeader("content-disposition", "attachment;  filename=Escuelas_CC.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Escuelas_CC", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Escuelas_CC" + e.Message, sesion);
            }
        }
        
        // POST: EscuelasCC/Save
        [HttpPost]
        public ActionResult Save(EscuelasCCModel model)
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
                    return Json(new { msg = Notification.Succes(" guardado con exito") });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Guardar") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }
    }
}