using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace PagoProfesores.Controllers.Pagos
{
    public class DetallePagosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public DetallePagosController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/DetallePagos/detalle_pagos.js" };
            Scripts.SCRIPTS = scripts;
            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10113,  Element = "Controller" }, //PERMISO ACCESO DetallePagos
                 new Factory.Privileges { Permiso = 10114,  Element = "formbtnconsultar" }, //PERMISO AGREGAR DetallePagos
            };
        }

        // GET: DetallePagos
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Pagos", "Detalle de pagos", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "APELLIDOS", "ASC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Detalle de Pagos Start", LOG.CONSULTA, "Ingresa Detalle de Pagos", sesion);

            ViewBag.sede = view.createLevels(sesion, "sede");     

            return View(Factory.View.Access + "Pagos/DetallePagos/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string periodos = "", string tipospago = "", string escuelas = "", string niveles = "", bool consultar = false, string filter = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "VESTADO_CUENTA_DETALLE";
            String[] columnas = { "IDSIU", "Nombre", "Apellidos","Origen de pago", "Campus"
                    , "Periodo", "Contrato", "Concepto","Monto"
                    ,"IVA", "IVA Ret", "ISR Ret","Fecha Pago"
                    ,"Cta Contable","Tipo de pago"};

            String[] campos = {
                      "IDSIU"
                    , "NOMBRES"
                    , "APELLIDOS"
                    , "CVE_ORIGENPAGO"
                    , "CVE_SEDE"
                    , "PERIODO"
                    , "ID_ESQUEMA"  //CVE_CONTRATO              
                    , "CONCEPTO"
                    , "MONTO"
                    , "MONTO_IVA"
                    , "MONTO_IVARET" 
                    , "MONTO_ISRRET"
                    ,"FECHAPAGO"
                    ,"CUENTACONTABLE"
                    ,"TIPODEPAGO" 
                   };

            string[] campossearch = {
                      "CVE_ORIGENPAGO"
                    , "CVE_SEDE"
                    , "PERIODO"
                    , "ID_ESQUEMA"
                    , "IDSIU"
                    , "NOMBRES"
                    , "APELLIDOS"
                    , "CONCEPTO"
                    , "MONTO"
                    , "MONTO_IVA"
                    , "MONTO_IVARET"
                    , "MONTO_ISRRET"
                    ,"FECHAPAGO", "CUENTACONTABLE", "TIPODEPAGO" };

            table.addColumnFormat("IDSIU", delegate (string value, ResultSet res) {
                return "<div style=\"width:100px;\">" + value + "</div>";
            });

            table.addColumnFormat("NOMBRES", delegate (string value, ResultSet res) {
                return "<div style=\"width:100px;\">" + value + "</div>";
            });

            table.addColumnFormat("APELLIDOS", delegate (string value, ResultSet res) {
                return "<div style=\"width:100px;\">" + value + "</div>";
            });

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "IDSIU";
            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";
            table.enabledButtonControls = false;

            string strperiodos;
            string strtipospago;
            string strescuelas;

            if (periodos.Equals("")||periodos.Equals("null")) { strperiodos = ""; }
            else strperiodos = " AND PERIODO = '" + periodos + "'";

            if (tipospago.Equals("") || tipospago.Equals("null")) { strtipospago = ""; }
            else strtipospago = " AND CVE_TIPODEPAGO = '" + tipospago + "'";

            if (escuelas.Equals("")|| escuelas.Equals("null")) { strescuelas = ""; }
            else strescuelas = " AND CVE_ESCUELA = '" + escuelas + "'";
            
            if (consultar) { table.TABLECONDICIONSQL += "" + strperiodos + "" + strtipospago + "" + strescuelas + ""; }
            
            return table.CreateDataTable(sesion);
        }
        
        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return; }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Nombre", typeof(string));
                tbl.Columns.Add("Apellidos", typeof(string));
                tbl.Columns.Add("Origen de pago", typeof(string));
                tbl.Columns.Add("Campus", typeof(string));            
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Contrato", typeof(string));
                tbl.Columns.Add("Concepto", typeof(string));
                tbl.Columns.Add("Monto", typeof(string));
                tbl.Columns.Add("IVA", typeof(string));
                tbl.Columns.Add("IVA Ret", typeof(string));
                tbl.Columns.Add("ISR Ret", typeof(string));
                tbl.Columns.Add("Fecha Pago", typeof(string));               
                tbl.Columns.Add("Cta Contable", typeof(string));
                tbl.Columns.Add("Tipo de pago", typeof(string));

                List<string> filtros = new List<string>();

                if (Request.Params["sedes"] != "" && Request.Params["sedes"] != null)
                    filtros.Add("CVE_SEDE = '" + Request.Params["sedes"] + "'");                
                
                if (Request.Params["periodos"] != "" && Request.Params["periodos"] != null) 
                    filtros.Add("PERIODO = '" + Request.Params["periodos"] + "'");
                
                if (Request.Params["tipospago"] != "" && Request.Params["tipospago"] != null) 
                    filtros.Add("CVE_TIPODEPAGO = '" + Request.Params["tipospago"] + "'");
                
                if (Request.Params["escuelas"] != "" && Request.Params["escuelas"] != null) 
                    filtros.Add("CVE_ESCUELA = '" + Request.Params["escuelas"] + "'");              

                 string conditions =  string.Join<string>(" AND ", filtros.ToArray());

                string union = "";
                if (conditions.Length != 0)
                      union = " WHERE ";

                ResultSet res = db.getTable("SELECT * FROM VESTADO_CUENTA_DETALLE " + union + " "+ conditions);

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("IDSIU"), res.Get("NOMBRES")
                        ,res.Get("APELLIDOS"), res.Get("CVE_ORIGENPAGO")
                        ,res.Get("CVE_SEDE"), res.Get("PERIODO")
                        ,res.Get("ID_ESQUEMA"), res.Get("CONCEPTO"), res.Get("MONTO")
                        ,res.Get("MONTO_IVA"), res.Get("MONTO_IVARET")
                        ,res.Get("MONTO_ISRRET"), res.Get("FECHAPAGO")
                        ,res.Get("CUENTACONTABLE")
                        , res.Get("TIPODEPAGO"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Detalle de Pagos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:O1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:O1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=DetallePagos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Detalle Pagos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Detalle Pagos" + e.Message, sesion);
            }
        }
    }
}