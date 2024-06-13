using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.CatalogosporSede;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.CatalogosporSede
{
    public class CentroCostosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        
        public CentroCostosController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosporSede/CentroCostos/centrocostos.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10159,  Element = "Controller" }, //PERMISO ACCESO BANCOS
                 new Factory.Privileges { Permiso = 10164,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
                 new Factory.Privileges { Permiso = 10161,  Element = "formbtnsave" }, //PERMISO EDITAR BANCOS
                 new Factory.Privileges { Permiso = 10162,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
            };
        }
        
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos por Sede", "Centros de costos", sesion);
            ViewBag.TipoFactura = ConsultaTipoFacturas();
            ViewBag.Escuelas = ConsultaEscuelas();
            ViewBag.DataTable = CreateDataTable(10, 1, null, "CVE_CENTRODECOSTOS", "ASC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Bancos Start", LOG.CONSULTA, "Ingresa pantalla Bancos", sesion);

            return View(Factory.View.Access + "CatalogosPorSede/CentroCostos/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filter = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "QCentrodeCostos01";
            String[] columnas = { "ID CC", "Clave", "Descripción","Tipo Factura","Tipo de Pago","Cuenta","Cuenta IVA","Cuenta IVA Ret","Cuenta ISR Ret","Sede","Escuela","Programa", "Usuario" ,"Fecha modificación" };
            String[] campos = { "ID_CENTRODECOSTOS", "CVE_CENTRODECOSTOS", "CENTRODECOSTOS","TIPOFACTURA", "TIPODEPAGO", "CUENTA", "CUENTA_IVA", "CUENTA_RETIVA", "CUENTA_RETISR", "SEDE", "ESCUELA", "PROGRAMA", "USUARIO", "FECHA_M", "CVE_SEDE" };
            string[] campossearch = { "CVE_CENTRODECOSTOS", "CENTRODECOSTOS", "USUARIO", "TIPOFACTURA", "TIPODEPAGO", "CUENTA", "CUENTA_IVA", "CUENTA_RETIVA", "CUENTA_RETISR", "SEDE", "ESCUELA", "PROGRAMA", "FECHA_M" };
            string[] camposhiden = { "CVE_SEDE" }; //"ID_CENTRODECOSTOS", 

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;
            table.CAMPOSHIDDEN = camposhiden;
            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";
            
            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_CENTRODECOSTOS";

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
                tbl.Columns.Add("Clave", typeof(string));
                tbl.Columns.Add("Descripcion", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Tipo de Factura", typeof(string));
                tbl.Columns.Add("Tipo Pago", typeof(string));
                tbl.Columns.Add("Cuenta", typeof(string));
                tbl.Columns.Add("Cuenta IVA", typeof(string));
                tbl.Columns.Add("Cuenta IVA Ret", typeof(string));
                tbl.Columns.Add("Cuenta ISR Ret", typeof(string));
                tbl.Columns.Add("Escuela", typeof(string));
                tbl.Columns.Add("Programa", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("Fecha Modificación", typeof(string));

                string sede = Request.Params["Sedes"];

                ResultSet res = db.getTable("SELECT * FROM VCENTRODECOSTOS WHERE CVE_SEDE = '"+ sede +"'");

                while (res.Next())
                    tbl.Rows.Add(res.Get("CVE_CENTRODECOSTOS"), res.Get("CENTRODECOSTOS"), res.Get("CVE_SEDE"), res.Get("TIPOFACTURA"), res.Get("TIPODEPAGO"), res.Get("CUENTA"), res.Get("CUENTA_IVA"), res.Get("CUENTA_RETIVA"), res.Get("CUENTA_RETISR"), res.Get("ESCUELA"), res.Get("PROGRAMA"), res.Get("USUARIO"), res.Get("FECHA_M"));

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Centro de Costos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:B1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:M1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=CentrodeCostos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Centro de Costos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Centro de Costos" + e.Message, sesion);
            }
        }

        [HttpGet]
        public string getProgramas(string cveEscuela)
        {
            CentroCostosModel model = new CentroCostosModel();
            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "selected";
            sb.Append("<option value=\"").Append("\" ").Append(selected).Append(">").Append("</option>\n");
            selected = "";
            foreach (KeyValuePair<string, string> pair in model.ConsultaProgramas(cveEscuela))
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(">").Append(pair.Value).Append("</option>\n");

            return sb.ToString();
        }

        [HttpGet]
        public string getTiposdePago(string cveFactura)
        {
            CentroCostosModel model = new CentroCostosModel();
            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "selected";
            foreach (KeyValuePair<string, string> pair in model.ConsultaTiposdePago(cveFactura))
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }
        
        [HttpGet]
        public string ConsultaTipoFacturas()
        {
            CentroCostosModel model = new CentroCostosModel();
            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "selected";
            foreach (KeyValuePair<string, string> pair in model.ConsultaTipoFacturas())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        [HttpGet]
        public string ConsultaEscuelas()
        {
            CentroCostosModel model = new CentroCostosModel();
            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "selected";
            sb.Append("<option value=\"").Append("\" ").Append(selected).Append(">").Append("</option>\n");
            selected = "";

            foreach (KeyValuePair<string, string> pair in model.ConsultaEscuelas())
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(">").Append(pair.Value).Append("</option>\n");

            return sb.ToString();
        }
        
        // POST: Bancos/Add
        [HttpPost]
        public ActionResult Add(CentroCostosModel model)
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
        public ActionResult Edit(CentroCostosModel model)
        {
            if (model.Edit())
                return Json(new JavaScriptSerializer().Serialize(model));

            return View();
        }
        
        // POST: Bancos/Save
        [HttpPost]
        public ActionResult Save(CentroCostosModel model)
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
        public ActionResult Delete(CentroCostosModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Delete())
                {
                    if (!model.asignado)
                    {
                        Log.write(this, "Delete", LOG.BORRADO, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Succes("Centro de Costos ELIMINADO con exito: " + model.Descripcion) });
                    }
                    else
                    {
                        Log.write(this, "Delete", LOG.BORRADO, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Warning("No se puede eliminar el Centro de Costos por que esta asignado en el Estado de Cuenta ") });
                    }
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
