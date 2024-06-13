using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.Pagos;
using PagoProfesores.Models.Reports;
//using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Session;
using System;
using System.Collections.Generic;
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
    public class FacturasController : Controller
    {
        // GET: Facturas
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        static Random rand;

        public FacturasController()
        {
            db = new database();

            string[] scripts = { "js/Reports/Facturas/facturas.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10027,  Element = "Controller" }, //PERMISO ACESSO AL REPORTE DE CALENDARIO DE PAGOS
            };
        }

        public void MergePDFS_Asimilados(DispersionModel bdrecibos)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            bdrecibos.sesion = sesion;


            string fileName = "";
            string directoryName = "";
            string msg_e = "";
            List<string> errores = new List<string>();


            rand = new Random();
            string random = rand.Next().ToString();
            string sede = bdrecibos.IdSede;
            long usuario = sesion.pkUser;


            try
            {
                //PdfDocument outputDocument = new PdfDocument();
                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();

                List<string> list = new List<string>();
                list = bdrecibos.getPDFs();


                int i = 1;
                if (list.Count > 0)
                {
                    try
                    {
                        foreach (string base64BinaryStr in list)
                        {

                            byte[] bytes = Convert.FromBase64String(base64BinaryStr);

                            fileName = "fileasim_" + i + "_" + usuario + "_" + sede + "_" + random + ".pdf";
                            directoryName = "FAsimilados";
                            string path = Request.MapPath("~/Upload/" + directoryName + "") + @"\" + fileName;

                            createPDF(bytes, path, fileName);

                            if (System.IO.File.Exists(path))
                            {
                                try
                                {
                                    PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(path, PdfDocumentOpenMode.Import);
                                    CopyPages(inputDocument, outputDocument);
                                }
                                catch (Exception ex)
                                {
                                    errores.Add("Error Exporta PDF asimilado: " + ex.Message + ", file: " + fileName);  //  Log.write(this, "Start", LOG.ERROR, "Exporta PDF" + " posible archivo corrupto.", sesion);
                                    Log.write(this, "MergePDFS_Asimilados", LOG.ERROR, "Exporta PDF asimilados" + ex.Message, sesion);
                                }
                            }

                            i++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errores.Add("Error Exporta PDF asimilados: " + ex.Message + ", file: " + fileName);
                        Log.write(this, "MergePDFS_Asimilados", LOG.ERROR, "Exporta PDF asimilados" + ex.Message, sesion);
                    }


                    if (outputDocument.PageCount > 0)
                    {
                        string nombrefile = "filesave" + usuario + "_" + sede + "_" + rand.Next().ToString() + ".pdf";

                        string pathsave = Request.MapPath("~/Upload") + @"\" + nombrefile;
                        outputDocument.Save(pathsave);
                        // Process.Start(pathsave);

                        Response.Clear();
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", nombrefile));
                        Response.WriteFile(pathsave);
                        Response.End();

                        ViewBag.Notification = Notification.Warning("se realizo con exito pdf´s");


                    }
                    else
                        ViewBag.Notification = Notification.Warning("no se pudo hacer el marge favor de volver a intentar");



                }
                else
                    ViewBag.Notification = Factory.Notification.Warning("NO EXISTEN REGISTROS CON LA FECHA DE RECIBO");



            }
            catch (Exception e)
            {
                Log.write(this, "MergePDFS_Asimilados", LOG.ERROR, "Exporta PDF asimilados" + e.Message, sesion);
                ViewBag.Notification = Notification.Error(e.Message);
                // return Json(new { msg = Factory.Notification.Error(e.Message) });
            }


        }


        public ActionResult MensajesError(DispersionModel bdrecibos)
        {

            string fileName = "";
            string directoryName = "";
            string msg_e = "";
            List<string> errores = new List<string>();

            try
            {
                //PdfDocument outputDocument = new PdfDocument();
                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();

                //  List<string> list = new List<string>();   
                //  list = bdrecibos.getPDFs_Honorarios();     

                Dictionary<string, FacturasModel> dict = new Dictionary<string, FacturasModel>();

                dict = bdrecibos.getPDFs_Honorarios();

                if (dict.Count > 0)
                {
                    try
                    {
                        string rutaPDF = "";
                        string IDSIU = "";
                        string esquema = "";
                        string concepto = "";


                        //foreach (string rutaPDF in list)
                        foreach (KeyValuePair<string, FacturasModel> Factura in dict)
                        {

                            rutaPDF = Factura.Value.PDF;
                            IDSIU = Factura.Value.IDSIU;
                            esquema = Factura.Value.ESQUEMA;
                            concepto = Factura.Value.CONCEPTO;

                            directoryName = Path.GetDirectoryName(rutaPDF);
                            fileName = Path.GetFileName(rutaPDF);
                            string path = Request.MapPath("~/Upload/" + directoryName + "") + @"\" + fileName;//ver otra val

                            //  errores.Add("Error Exporta PDF: XXX, file: " + fileName + ", IDSIU:" + IDSIU + "/" + esquema + "/" + concepto);  //  Log.write(this, "Start", LOG.ERROR, "Exporta PDF" + " posible archivo corrupto.", sesion);

                            if (System.IO.File.Exists(path))
                            {
                                try
                                {
                                    PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                                    //PdfDocument inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                                    CopyPages(inputDocument, outputDocument);
                                }
                                catch (Exception ex)
                                {
                                    errores.Add("Error Exporta PDF: " + ex.Message + ", file: " + fileName + ", IDSIU:" + IDSIU + "/" + esquema + "/" + concepto);  //  Log.write(this, "Start", LOG.ERROR, "Exporta PDF" + " posible archivo corrupto.", sesion);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    { errores.Add("Error Exporta PDF: " + ex.Message + ", file: " + fileName); }


                    if (outputDocument.PageCount == 0)
                    {
                        return Json(new { msg = Notification.Error("No se encuentra ningún archivo pdf en el servidor con este filtro") });

                    }
                    else
                    {
                        if (errores.Count > 0)
                        {
                            msg_e = string.Join<string>("<br/>\n", errores);
                            return Json(new { msg = Notification.Warning(msg_e) });

                        }

                        return Json(new { msg = "" });

                    }
                }
            }
            catch (Exception e)
            {
                Log.write(this, "MensajesError", LOG.ERROR, "Exporta PDF" + e.Message, sesion);
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
            return Json(new { msg = "" });


        }

        public void createPDF(byte[] bytes, string path, string fileName)
        {



            System.IO.FileStream stream = new System.IO.FileStream(path, FileMode.CreateNew);

            // System.IO.FileStream stream =   new System.IO.FileStream(@"C:\Users\Charly\apps\file.pdf", FileMode.CreateNew);
            System.IO.BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();


        }

        public void MergePDFS(DispersionModel bdrecibos)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            bdrecibos.sesion = sesion;

            rand = new Random();

            try
            {
                string fileName = "";
                string directoryName = "";

                PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();

                List<string> list = new List<string>();

                list = bdrecibos.getPDFs();

                if (list.Count > 0)
                {
                    try
                    {   //H y F
                        foreach (string rutaPDF in list)
                        {
                            directoryName = Path.GetDirectoryName(rutaPDF);
                            fileName = Path.GetFileName(rutaPDF);

                            string path = Request.MapPath("~/Upload/" + directoryName + "") + @"\" + fileName;//ver otra val

                            if (System.IO.File.Exists(path))
                            {

                                try
                                {

                                    // PdfDocument pdfDocument = new PdfDocument(new PdfReader(path);
                                    iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfReader(path));
                                    int n = pdfDocument.GetNumberOfPages();

                                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                                    string currentText = "";

                                    int i = 1;
                                    while (i <= n)
                                    {
                                        currentText += "\n" + PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i), strategy);
                                        if (!currentText.Contains("Sueldos y Salarios"))
                                        {
                                            try
                                            {
                                                PdfSharp.Pdf.PdfDocument inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                                                //PdfDocument inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                                                CopyPages(inputDocument, outputDocument);
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.write(this, "Start", LOG.ERROR, "Exporta PDF" + ex.Message + ", file: " + fileName, sesion);
                                            }
                                        }
                                        i++;
                                    }


                                }
                                catch (Exception ex)
                                {

                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.write(this, "Start", LOG.ERROR, "Exporta PDF: " + ex.Message + ", file: " + fileName, sesion);
                    }

                    string sede = bdrecibos.IdSede;

                    long usuario = sesion.pkUser;
                    string nombrefile = "filesave" + usuario + "_" + sede + "_" + rand.Next().ToString() + ".pdf";

                    string pathsave = Request.MapPath("~/Upload") + @"\" + nombrefile;
                    outputDocument.Save(pathsave);
                    // Process.Start(pathsave);

                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", nombrefile));
                    Response.WriteFile(pathsave);
                    Response.End();

                    ViewBag.Notification = Notification.Warning("se realizo con exito pdf´s");
                }
                else
                    ViewBag.Notification = Factory.Notification.Warning("NO EXISTEN REGISTROS CON LA FECHA DE RECIBO");
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "MergePDFS", LOG.ERROR, "Exporta PDF" + e.Message, sesion);
            }
        }


        //public void CopyPages(PdfDocument from, PdfDocument to)
        public void CopyPages(PdfSharp.Pdf.PdfDocument from, PdfSharp.Pdf.PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
                to.AddPage(from.Pages[i]);
        }

        // GET: Recibos
        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Reportes", "Facturas", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Facturas Start", LOG.CONSULTA, "Ingresa Pantalla Facturas", sesion);

            return View(Factory.View.Access + "Reports/Facturas/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string fechai = "", string fechaf = "", string filter = "", string cve_tipodepago = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            DataTable table = new DataTable();

            table.TABLE = "VESTADO_CUENTA";

            table.COLUMNAS =
                new string[] { "IDSIU", "Nombres", "Apellidos","Sede", "Origen", "Periodo",
                  /*"Nivel",*/"Concepto","Monto","IVA","IVA Ret","ISR Ret","Monto de Deposito","Fecha Pago",
                    "Fecha Recibo"  ,"Tipo Transferecia","Esquema","Banco del Docente","Número de Cuenta",
                    "Cuenta CLABE","Id_estado de cuenta"};//,"Cta Contable" };//,"Tipo de pago"
            table.CAMPOS =
                new string[] { "IDSIU", "NOMBRES", "APELLIDOS","CVE_SEDE","CVE_ORIGENPAGO","PERIODO",
                    /*"CVE_NIVEL" ,*/"CONCEPTO","MONTO","MONTO_IVA","MONTO_IVARET","MONTO_ISRRET","BANCOS","FECHAPAGO",
                "FECHARECIBO","CVE_TIPOTRANSFERENCIA","ESQUEMA","BANCO","NOCUENTA","CUENTACLABE","ID_ESTADODECUENTA"};//"TIPODEPAGO",
            table.CAMPOSSEARCH =
                new string[] { "IDSIU", "NOMBRES", "CVE_SEDE", "CVE_ORIGENPAGO", "PERIODO"//, "CVE_NIVEL"
                ,"CONCEPTO","MONTO"};


            string[] camposhidden = { "ID_ESTADODECUENTA" };


            table.addColumnClass("IDSIU", "datatable_fixedColumn");
            table.addColumnClass("NOMBRES", "datatable_fixedColumn");
            table.addColumnClass("APELLIDOS", "datatable_fixedColumn");
            //  table.addColumnClass("FECHA_PUBLICACION", "datatable_fixedColumn");

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_ESTADODECUENTA";



            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "' AND (FECHARECIBO <> '' AND FECHARECIBO IS NOT NULL) AND PUBLICADO =1 AND (PDF IS NOT NULL AND CAST(PDF AS nvarchar(MAX) ) <> '' )";

            /*   if (publicado == "")
               {
                   table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";
               }
               else if (publicado == "1")
               {
                   table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "' AND ESTA_PUBLICADO='SI'";
               }
               else if (publicado == "0")
               {
                   table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "' AND ESTA_PUBLICADO='NO'";
               }*/



            List<string> filtros = new List<string>();
            //	if (filter != "") filtros.Add("CVE_SEDE = '" + filter + "'");

            if (fechai != "")
            {
                filtros.Add("FECHAPAGO >= '" + fechai + "'");
            }
            if (fechaf != "")
            {
                filtros.Add("FECHAPAGO <= '" + fechaf + "'");
            }


            if (cve_tipodepago != "")
            {
                filtros.Add("CVE_TIPOFACTURA = '" + cve_tipodepago + "'");
            }


            if (cve_tipodepago == "A") filtros.Add(("(FECHAEMISION <> '' AND FECHAEMISION IS NOT NULL)"));


            string union = "";
            if (filter != "" && filtros.Count > 0) { union = " AND "; }

            table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());

            table.enabledButtonControls = false;

            table.enabledCheckbox = true;

            // table.addBtnActions("Editar", "editarRegistroContratos");

            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("¿Esta publicado?", typeof(string));
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

                string sede = Request.Params["sedes"];
                string fechai = Request.Params["fechai"];
                string fechaf = Request.Params["fechaf"];

                List<string> filtros = new List<string>();

                if (fechai != "")
                    filtros.Add("FECHAPAGO >= '" + fechai + "'");

                if (fechaf != "")
                    filtros.Add("FECHAPAGO <= '" + fechaf + "'");

                string conditions = string.Join<string>(" AND ", filtros.ToArray());

                string union = "";
                if (conditions.Length != 0) union = " AND ";

                ResultSet res = db.getTable("SELECT * FROM VESTADO_CUENTA  WHERE CVE_SEDE = '" + sede + "' " + union + " " + conditions);

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("ESTA_PUBLICADO"), res.Get("IDSIU"), res.Get("NOMBRES")
                        , res.Get("APELLIDOS"), res.Get("CVE_SEDE"), res.Get("CVE_ORIGENPAGO"), res.Get("PERIODO"), res.Get("CONCEPTO")
                        , res.Get("MONTO"), res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("FECHAPAGO")
                        , res.Get("FECHARECIBO"), res.Get("CVE_TIPOTRANSFERENCIA"));
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
                    Response.AddHeader("content-disposition", "attachment;  filename=PublicarPagos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Publicar Pagos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Publicar Pagos" + e.Message, sesion);
            }
        }


        public string Load(string v_path)
        {
            //  string path = Server.MapPath("~/Upload/" + v_path);
            StringBuilder sb = new StringBuilder();
            using (FileStream fs = new FileStream(v_path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null)
                            break;
                        sb.Append(line);
                    }
                }
            }
            return sb.ToString();
        }
    }
}