using ConnectDB;
using PagoProfesores.Models.Pagos;
using Session;
using System.Web;
using System.Web.Mvc;
//using HiQPdf;
using SelectPdf;
using PagoProfesores.Models.CatalogosporSede;
using System;

namespace PagoProfesores.Controllers.Pagos
{
    public class ECW_RetencionesController : Controller
    {
        private SessionDB sesion;
        private database db;


        public ECW_RetencionesController()
        {
            db = new database();
        }
        
        public string SetRetencion()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return "-1"; }

            sesion.vdata["CVE_RETENCION"] = Request.Params["cve_retencion"];
            sesion.vdata["CVE_SEDE"] = Request.Params["cve_sede"]; 
            sesion.vdata["MES_INICIO"] = Request.Params["mesi"];
            sesion.vdata["MES_FIN"] = Request.Params["mesf"];
            sesion.vdata["ANIO"] = Request.Params["anio"];
            sesion.vdata["MONTO"] = Request.Params["monto"];
            sesion.vdata["MONTO_IVA"] = Request.Params["monto_Iva"];
            sesion.vdata["MONTO_IVARET"] = Request.Params["monto_IvaRet"];
            sesion.vdata["MONTO_ISRRET"] = Request.Params["montoIsrRet"]; 


            sesion.saveSession();
            return "0";
        }

        public void ConvertPDF()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return; }

            // convert HTML to PDF
            byte[] pdfBuffer = null;

            // convert HTML code
            ConstanciaRetencionModel conModel = new ConstanciaRetencionModel();
            conModel.Clave = sesion.vdata["CVE_RETENCION"];
            conModel.Edit();
            string htmlCode = HttpUtility.UrlDecode(conModel.Formato, System.Text.Encoding.Default);

            RetencionesWebPDFModel model = new RetencionesWebPDFModel();
                    

            model.IDSIU = sesion.vdata["IDSIU"];
            model.CVE_SEDE = sesion.vdata["CVE_SEDE"];          
            model.MES_INICIO = sesion.vdata["MES_INICIO"];
            model.MES_FIN = sesion.vdata["MES_FIN"];
            model.ANIO = sesion.vdata["ANIO"];          
            model.MONTO = sesion.vdata["MONTO"];
            model.MONTO_IVA = sesion.vdata["MONTO_IVA"];
            model.MONTO_IVARET = sesion.vdata["MONTO_IVARET"];
            model.MONTO_ISRET = sesion.vdata["MONTO_ISRRET"];


            model.getDatos();
            htmlCode = model.fill(htmlCode);

            //htmlCode = htmlCode.Replace("{SOCIEDAD}", "shalala-shalala");


            string htmlString = htmlCode;
            string baseUrl = "";// collection["TxtBaseUrl"];

            //string thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
            //string baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "ConstanciaRetencion".Length);

            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // license key
            GlobalProperties.LicenseKey = "KgEbChgfGwoZGx0fChsSBBoKGRsEGxgEExMTEw==";

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = Convert.ToInt32(1024);
            converter.Options.WebPageHeight = Convert.ToInt32(0);
            converter.Options.MarginTop = 35;
            converter.Options.MarginBottom = 35;


            PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            pdfBuffer = doc.Save();

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;  filename=ConstanciaRetencion.pdf");
            Response.BinaryWrite(pdfBuffer);
            doc.Close();

        }
        
    }
}