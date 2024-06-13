using System;
using System.Collections.Generic;
using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.Pagos;
using PagoProfesores.Models.Reports;
using PdfSharp.Pdf.IO;
using Session;

using System.Drawing;
using System.IO;
using System.Web.Mvc;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text;
using System.Web;
using System.Xml;


namespace PagoProfesores.Controllers.Reports
{
    public class SeguimientoDocentesFacturasController : Controller
    {
        // GET: SeguimientoDocentesFacturas
        // GET: Facturas
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        static Random rand;


        public SeguimientoDocentesFacturasController()
        {
            db = new database();

            string[] scripts = {
                "js/Reports/SeguimientoDocentesFacturas/SeguimientoDocentesFacturas.js",
                 "js/Pagos/GestiondePagos/download.js"
            };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10027,  Element = "Controller" }, //PERMISO ACESSO AL REPORTE DE CALENDARIO DE PAGOS
            };
        }
        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Reportes", "Seguimiento de docentes con Facturas", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Seguimiento de docentes con Facturas", sesion);

            return View(Factory.View.Access + "Reports/SeguimientoDocentesFacturas/Start.cshtml");
        }
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string fechai = "", string fechaf = "", string filter = "", string cve_tipodepago = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            DataTable table = new DataTable();
            string retorno = string.Empty;

            table.TABLE = "V_SEGUIMIENTODOCENTESFACTURAS";

            table.COLUMNAS =
                new string[] { "IDSIU", "Nombres", "Apellidos","Sede", "Origen", "Periodo",
                  /*"Nivel",*/"Concepto","Monto","IVA","IVA Ret","ISR Ret","Fecha Pago",
                    "Fecha Recibo"  ,"Tipo Transferecia","Estatus","PDF","XML","Id_estado de cuenta","Tipo de Pago"};//,"Tipo de pago"
            table.CAMPOS =
                new string[] { "IDSIU", "NOMBRES", "APELLIDOS","CVE_SEDE","CVE_ORIGENPAGO","PERIODO",
                  "CONCEPTO","MONTO","MONTO_IVA","MONTO_IVARET","MONTO_ISRRET","FECHADEPAGO",
                "FECHARECIBO","CVE_TIPOTRANSFERENCIA","ESTATUS","PDF","XML","ID_ESTADODECUENTA","CVE_TIPODEPAGO"};
            table.CAMPOSSEARCH =
                new string[] { "IDSIU", "NOMBRES", "CVE_SEDE", "CVE_ORIGENPAGO", "PERIODO", "CONCEPTO", "MONTO" };


            string[] camposhidden = { "CVE_TIPODEPAGO" };

            table.dictColumnFormat.Add("PDF", delegate (string value, ResultSet res)
            {
                retorno = "<div style=\"width:130px;\">";
                if (res.Get("CVE_TIPODEPAGO") == "ADI")
                {

                    if (res.Get("ESTATUS") == "TIMBRADO")
                    {
                        //retorno += "<button type='button' onclick='base64toPDF("+ res.Get("PDF")+")';> <i class=\"fa fa-print m-r-5\"></i>PDF</button></div>";
                        retorno += res.Get("PDF") + "</div>";

                        retorno += "</div>";
                        return retorno;
                    }
                    else
                    {
                        retorno += "---</div>";
                        return retorno;
                    }
                }
                else if (res.Get("CVE_TIPODEPAGO") != "ADI" && (res.Get("CVE_TIPODEPAGO") != "" || res.Get("CVE_TIPODEPAGO") != null))
                {
                    if (res.Get("ESTATUS") == "ENTREGADO")
                    {
                        retorno += res.Get("PDF") + "</div>";
                        return retorno;
                    }
                    else
                    {
                        retorno += "---</div>";
                        return retorno;
                    }
                }
                else
                {
                    retorno += "</div>";
                    return retorno;
                }
            });

            table.dictColumnFormat.Add("XML", delegate (string value, ResultSet res)
            {
                retorno = "<div style=\"width:130px;\">";
                if (res.Get("CVE_TIPODEPAGO") == "ADI")
                {

                    if (res.Get("ESTATUS") == "TIMBRADO")
                    {
                        retorno += res.Get("XML") + "</div>";
                        return retorno;
                    }
                    else
                    {
                        retorno += "---</div>";
                        return retorno;
                    }
                }
                else if (res.Get("CVE_TIPODEPAGO") != "ADI" && (res.Get("CVE_TIPODEPAGO") != "" || res.Get("CVE_TIPODEPAGO") != null))
                {
                    if (res.Get("ESTATUS") == "ENTREGADO")
                    {
                        retorno += res.Get("XML") + "</div>";
                        return retorno;
                    }
                    else
                    {
                        retorno += "---</div>";
                        return retorno;
                    }
                }
                else
                {
                    retorno += "---</div>";
                    return retorno;
                }
            });

            table.addColumnClass("IDSIU", "datatable_fixedColumn");
            table.addColumnClass("NOMBRES", "datatable_fixedColumn");
            table.addColumnClass("APELLIDOS", "datatable_fixedColumn");

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_ESTADODECUENTA";

            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "' AND PUBLICADO =1 ";


            List<string> filtros = new List<string>();

            if (fechai != "")
            {
                filtros.Add("FECHADEPAGO >= '" + fechai + "'");
            }
            if (fechaf != "")
            {
                filtros.Add("FECHADEPAGO <= '" + fechaf + "'");
            }


            if (cve_tipodepago != "")
            {
                filtros.Add("CVE_TIPOFACTURA = '" + cve_tipodepago + "'");
            }


            string union = "";
            if (filter != "" && filtros.Count > 0) { union = " AND "; }

            table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());

            table.enabledButtonControls = false;

            //table.enabledCheckbox = true;

            // table.addBtnActions("Editar", "editarRegistroContratos");

            return table.CreateDataTable(sesion);
        }

        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Nombres", typeof(string));
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
                tbl.Columns.Add("Tipo Transferecia", typeof(string));
                tbl.Columns.Add("Estatus", typeof(string));
                string sede = Request.Params["sedes"];
                string fechai = Request.Params["fechai"];
                string fechaf = Request.Params["fechaf"];

                List<string> filtros = new List<string>();

                if (fechai != "")
                    filtros.Add("FECHADEPAGO >= '" + fechai + "'");

                if (fechaf != "")
                    filtros.Add("FECHADEPAGO <= '" + fechaf + "'");

                string conditions = string.Join<string>(" AND ", filtros.ToArray());

                string union = "";
                if (conditions.Length != 0) union = " AND ";

                ResultSet res = db.getTable("SELECT * FROM V_SEGUIMIENTODOCENTESFACTURAS  WHERE CVE_SEDE = '" + sede + "' " + union + " " + conditions);

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("IDSIU"), res.Get("NOMBRES")
                        , res.Get("APELLIDOS"), res.Get("CVE_SEDE"), res.Get("CVE_ORIGENPAGO"), res.Get("PERIODO"), res.Get("CONCEPTO")
                        , res.Get("MONTO"), res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("FECHADEPAGO")
                        , res.Get("FECHARECIBO"), res.Get("CVE_TIPOTRANSFERENCIA"), res.Get("ESTATUS"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Publicar Pagos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:P1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:P1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=SeguimientoDocentesFacturas.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Publicar Pagos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Seguimiento de docentes con Facturas" + e.Message, sesion);
            }
        }
    }


}