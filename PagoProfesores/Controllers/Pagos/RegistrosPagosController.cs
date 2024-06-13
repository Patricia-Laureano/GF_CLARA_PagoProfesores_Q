using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Pagos
{
    public class RegistrosPagosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public RegistrosPagosController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/GestiondePagos/registrospagos.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10134,  Element = "Controller" }, //PERMISO ACCESO REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10135,  Element = "formbtnconsultar" }, //PERMISO EDITAR REGISTRO DE CONTRATOS
            };
        }

        // GET: Recibos
        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Pagos", "Gestión de Pagos", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Gestión de Pagos Start", LOG.CONSULTA, "Ingresa Pantalla Registro de pagos", sesion);

            return View(Factory.View.Access + "Pagos/GestiondePagos/RegistrosPagos/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string fechai = "", string fechaf = "", string fechair = "", string fechafr = "", string fechaid = "", string fechafd = "", string fechaidp = "", string fechafdp = "", string filter = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            DataTable table = new DataTable();

            table.TABLE = "QRegistroPagos";

            table.COLUMNAS =
                new string[] { "IDSIU", "Nombre", "Apellidos","Sede", "Origen", "Periodo"
                ,"Concepto","Monto","IVA","IVA Ret","ISR Ret","Fecha Pago","Fecha Recibo", "Fecha Solicitado"
                ,"Tipo Transferecia","Cta Contable","Tipo de pago","UUID","XML" };
            table.CAMPOS =
                new string[] { "IDSIU", "NOMBRES", "APELLIDOS","CVE_SEDE","CVE_ORIGENPAGO","PERIODO",
                   "CONCEPTO","MONTO","MONTO_IVA","MONTO_IVARET","MONTO_ISRRET","FECHAPAGO",
                "FECHARECIBO","FECHA_SOLICITADO","TIPOTRANSFERENCIA","CUENTA_CC","TIPODEPAGO","UUID","XML"};
            table.CAMPOSSEARCH =
                new string[] { "IDSIU", "NOMBRES", "APELLIDOS", "CVE_SEDE", "CVE_ORIGENPAGO", "PERIODO", "CONCEPTO","MONTO", "FECHA_SOLICITADO"};

            table.addColumnClass("IDSIU", "datatable_fixedColumn");
            table.addColumnClass("NOMBRES", "datatable_fixedColumn");
            table.addColumnClass("APELLIDOS", "datatable_fixedColumn");

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "IDSIU";
            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";

            List<string> filtros = new List<string>();

            if (fechai != "")
                filtros.Add("FECHAPAGO >= '" + fechai + "'");                
            
            if (fechaf != "")
                filtros.Add("FECHAPAGO <= '" + fechaf + "'");
            
            if (fechair != "")   
                filtros.Add("FECHARECIBO >= '" + fechair + "'");
            
            if (fechafr != "")  
                filtros.Add("FECHARECIBO <= '" + fechafr + "'");
            
            if (fechaid != "")
                filtros.Add("FECHADISPERSION >= '" + fechaid + "'");
            
            if (fechafd != "")
                filtros.Add("FECHADISPERSION <= '" + fechafd + "'");
            
            if (fechaidp != "")
                filtros.Add("FECHADEPOSITO >= '" + fechaidp + "'");
            
            if (fechafdp != "")
                filtros.Add("FECHADEPOSITO <= '" + fechafdp + "'");             
            
            string union = "";
            if (filter != "" && filtros.Count>0) { union = " AND "; }

            table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());
            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }
        
        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) sesion = SessionDB.start(Request, Response, false, db);
            string sqlExcel = string.Empty;

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Nombre", typeof(string));
                tbl.Columns.Add("Apellidos", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Origen", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Concepto", typeof(string));
                tbl.Columns.Add("Monto", typeof(string));
                tbl.Columns.Add("IVA", typeof(string));
                tbl.Columns.Add("IVA Ret", typeof(string));
                tbl.Columns.Add("ISR Ret", typeof(string));
                tbl.Columns.Add("Fecha Pago", typeof(string));
                tbl.Columns.Add("Fecha Recibo", typeof(string));
                tbl.Columns.Add("Fecha Solicitado", typeof(string));
                tbl.Columns.Add("Tipo Transferecia", typeof(string));
                tbl.Columns.Add("Cta Contable", typeof(string));
                tbl.Columns.Add("Tipo de pago", typeof(string));
                tbl.Columns.Add("UUID", typeof(string));
                tbl.Columns.Add("XML", typeof(string));

                string sede = Request.Params["sedes"];
                string fechai = Request.Params["fechai"];
                string fechaf = Request.Params["fechaf"];
                string fechair = Request.Params["fechair"];
                string fechafr = Request.Params["fechafr"];
                string fechaid = Request.Params["fechaid"];
                string fechafd = Request.Params["fechafd"];
                string fechaidp = Request.Params["fechaidp"];
                string fechafdp = Request.Params["fechafdp"];

                List<string> filtros = new List<string>();        

                if (fechai != "")
                    filtros.Add("FECHAPAGO >= '" + fechai + "'");

                if (fechaf != "")
                    filtros.Add("FECHAPAGO <= '" + fechaf + "'");

                if (fechair != "")
                    filtros.Add("FECHARECIBO >= '" + fechair + "'");

                if (fechafr != "")
                    filtros.Add("FECHARECIBO <= '" + fechafr + "'");

                if (fechaid != "")
                    filtros.Add("FECHADISPERSION >= '" + fechaid + "'");

                if (fechafd != "")
                    filtros.Add("FECHADISPERSION <= '" + fechafd + "'");

                if (fechaidp != "")
                    filtros.Add("FECHADEPOSITO >= '" + fechaidp + "'");

                if (fechafdp != "")
                    filtros.Add("FECHADEPOSITO <= '" + fechafdp + "'");                        


                string conditions = string.Join<string>(" AND ", filtros.ToArray());
                string union = "";

                if (conditions.Length != 0) union = " AND ";

                sqlExcel = "SELECT * FROM QRegistroPagos  WHERE CVE_SEDE = '" + sede + "' " + union + " " + conditions + " order by IDSIU";

                ResultSet res = db.getTable(sqlExcel);
        
                while (res.Next()) // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("IDSIU"), res.Get("NOMBRES"), res.Get("APELLIDOS")
                        , res.Get("CVE_SEDE"), res.Get("CVE_ORIGENPAGO"), res.Get("PERIODO"), res.Get("CONCEPTO"), res.Get("MONTO")
                        , res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("FECHAPAGO"), res.Get("FECHARECIBO"), res.Get("FECHA_SOLICITADO")
                        , res.Get("TIPOTRANSFERENCIA"), res.Get("CUENTA_CC"), res.Get("TIPODEPAGO"), res.Get("UUID"), res.Get("XML"));

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Registros Pagos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:S1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:S1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Registros.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Registros Pagos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Registros Pagos" + e.Message, sesion);
            }
        }
    }
}