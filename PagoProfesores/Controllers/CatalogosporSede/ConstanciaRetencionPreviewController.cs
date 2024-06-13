using ConnectDB;
using Factory;
using HiQPdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.Mvc;

namespace PagoProfesores.Models.CatalogosporSede
{
    public class ConstanciaRetencionPreviewController : Controller
    {
        // GET: ContratosWeb
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
      
        public ConstanciaRetencionPreviewController()
        {
            db = new database();

            string[] scripts = { "plugins/editor/tinymce.min.js", "js/CatalogosporSede/ConstanciaRetencionPreview/constanciaRetencion.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10157,  Element = "Controller" }, //PERMISO ACCESO ContratosWebPreview
               /*  new Factory.Privileges { Permiso = 10006,  Element = "frm-ContratosWebPreview" }, //PERMISO DETALLE ContratosWebPreview
                 new Factory.Privileges { Permiso = 10007,  Element = "formbtnadd" }, //PERMISO AGREGAR ContratosWebPreview
                 new Factory.Privileges { Permiso = 10008,  Element = "formbtnsave" }, //PERMISO EDITAR ContratosWebPreview
                 new Factory.Privileges { Permiso = 10009,  Element = "formbtndelete" }, //PERMISO ELIMINAR ContratosWebPreview*/
            };

        }

        // GET: ContratosWeb
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu(31, 47, sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "CVE_RETENCION", "ASC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);
            
            Log.write(this, "ContratosWeb Start", LOG.CONSULTA, "Ingresa pantalla ContratosWeb", sesion);

            return View(Factory.View.Access + "CatalogosPorSede/ConstanciaRetencionPreview/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "FORMATORETENCIONES";
            String[] columnas = { "Clave", "Nombre", "Usuario", "Fecha de Modificación" };
            String[] campos = { "CVE_RETENCION", "RETENCION", "USUARIO", "FECHA_M" };
            string[] campossearch = { "CVE_RETENCION", "RETENCION" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_RETENCION";

            table.enabledButtonControls = false;





            table.addColumnFormat("RETENCION", delegate (string Retencion, ResultSet res) {

                string retencion1 = "<a href=\"javascript:void(0)\" onClick=\"formPage.PDF('" + res.Get("CVE_RETENCION") + "');\"><i class=\"fa fa-print\"></i> " + Retencion + " </a>";
                return retencion1;
            });


            /*  table.addColumnFormat("FORMATO", delegate (string Retencion, ResultSet res) {

                  string contrato1 = "<div style=\"width:25px;\"><button type=\"button\" id=\"formbtnadd\"  onclick=\"formPage.PDF('" + res.Get("CVE_RETENCION") + "')\" class=\"btn btn-sm btn-primary\" >imprimir</button></div>";
                  return contrato1;
              });*/



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
                tbl.Columns.Add("Nombre", typeof(string));
                

                ResultSet res = db.getTable("SELECT * FROM FORMATORETENCIONES");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("CVE_RETENCION"), res.Get("RETENCION"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Constancias Retenciones");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:B1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=ContratosWeb.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Constancias Retenciones", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta  Excel Catalogo Constancias Retenciones" + e.Message, sesion);

            }

        }

 

        
        public void ConvertPDF()
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

   
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
            htmlToPdfConverter.SerialNumber = "pe3M9PXB-w+nMx9fE-19yUlYuV-hZSFl4Wc-k5CFlpSL-lJeLnJyc-nA==";

            // set PDF page size and orientation
            htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
            // set PDF page margins
            //htmlToPdfConverter.Document.Margins = new PdfMargins(0);
            htmlToPdfConverter.Document.Margins.Top = 35;
            htmlToPdfConverter.Document.Margins.Bottom = 35;


            // convert HTML to PDF
            byte[] pdfBuffer = null;

            // convert HTML code
            string htmlCode = HttpUtility.UrlDecode(sesion.vdata["html"], System.Text.Encoding.Default); 

            string thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
            string baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "ConstanciaRetencion".Length);

            // convert HTML code to a PDF memory buffer
            pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlCode, baseUrl);

            // send the PDF file to browser
           // FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            //fileResult.FileDownloadName = "Formato_Constancia_Retencion.pdf";



            //Write it back to the client
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;  filename=ConstanciaRetencion.pdf");
            Response.BinaryWrite(pdfBuffer);

           // return fileResult;


        }

        [HttpPost]
        public ActionResult getHTML(ConstanciaRetencionPreviewModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.Edit();
            sesion.vdata["html"] = model.Formato;
            sesion.saveSession();

            return Json(new { msg = Notification.Succes("Contrato ELIMINADO con exito: ") });

        }

    }
}
