using ConnectDB;
using Factory;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
//using HiQPdf;
using SelectPdf;
using PagoProfesores.Models.CatalogosporSede;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace PagoProfesores.Controllers.Pagos
{
    public class ECW_ContratosController : Controller
	{
		private SessionDB sesion;
		private database db;

		public ECW_ContratosController()
		{
			db = new database();
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null
			, string Sede = "", string Periodo = "", string Nivel = "")
		{
			if (sesion == null)
				if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return "-1"; }

			if (search == "undefined")
				search = "";

			string IDSIU = sesion.vdata["IDSIU"];
			string ID_PERSONA = sesion.vdata["ID_PERSONA"];

			DataTable table = new DataTable();

			table.TABLE = "VENTREGA_CONTRATOS";

			table.COLUMNAS =
				new string[] { "Contrato", "Sede", "Periodo", "Nivel", "Esquema", "Tipo contrato", "No. Pagos", "Fec. entrega de contrato", "Monto", "IVA", "IVA Ret", "ISR Ret" };
			table.CAMPOS =
				new string[] { "CVE_CONTRATO", "CVE_SEDE", "PERIODO", "CVE_NIVEL", "ESQUEMA", "TIPODEPAGO", "NUMPAGOS", "FECHADECONTRATO", "MONTO", "MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "ID_ESQUEMA" };
			table.CAMPOSSEARCH =
				new string[] { "CVE_CONTRATO" };
			table.CAMPOSHIDDEN =
				new string[] { "ID_ESQUEMA" };

			table.addColumnFormat("CVE_CONTRATO", delegate (string value, ResultSet res)
			{
				return "<button type='button' class='btn btn-sm btn-primary' onclick=\"formPage_Contratos.verContrato('" + res.Get("CVE_CONTRATO") +
				"','" + res.Get("CVE_SEDE") + "','" + res.Get("PERIODO") + "','" + res.Get("CVE_NIVEL") + "','" + res.Get("ID_ESQUEMA") + "','" + IDSIU + "');\">Ver contrato</button>";
			});
            
			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "CVE_SEDE";

			List<string> condition = new List<string>();
			condition.Add("(ID_PERSONA = " + ID_PERSONA + ")");
			if (string.IsNullOrWhiteSpace(Sede) == false) condition.Add("(CVE_SEDE='" + Sede + "')");
			if (string.IsNullOrWhiteSpace(Periodo) == false) condition.Add("(PERIODO='" + Periodo + "')");
			if (string.IsNullOrWhiteSpace(Nivel) == false) condition.Add("(CVE_NIVEL='" + Nivel + "')");
			table.TABLECONDICIONSQL = string.Join<string>(" AND ", condition);

			table.enabledButtonControls = false;

			return table.CreateDataTable(sesion, "DataTable_Contratos");
		}

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) sesion = SessionDB.start(Request, Response, false, db);

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Contrato", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Esquema", typeof(string));
                tbl.Columns.Add("Tipo contrato", typeof(string));
                tbl.Columns.Add("No. Pagos", typeof(string));
                tbl.Columns.Add("Fec. entrega de contrato", typeof(string));
                tbl.Columns.Add("Monto", typeof(string));
                tbl.Columns.Add("IVA", typeof(string));
                tbl.Columns.Add("IVA Ret", typeof(string));
                tbl.Columns.Add("ISR Ret", typeof(string));
                
                List<string> condition = new List<string>();

                string IDSIU = sesion.vdata["IDSIU"];
                string ID_PERSONA = sesion.vdata["ID_PERSONA"];
                condition.Add("ID_PERSONA = '" + ID_PERSONA + "'");

                if (Request.Params["Periodo"] != "" && Request.Params["Periodo"] != "null")
                    condition.Add("PERIODO = '" + Request.Params["Periodo"] + "'");

                if (Request.Params["Sede"] != "" && Request.Params["Sede"] != "null")
                    condition.Add("CVE_SEDE = '" + Request.Params["Sede"] + "'");                     

                string TABLECONDICIONSQL = string.Join<string>(" AND ", condition);

                string union = "";
                if (condition.Count > 0) union = " WHERE ";

                string sql = "SELECT * FROM VENTREGA_CONTRATOS " + union + " " + TABLECONDICIONSQL;

                ResultSet res = db.getTable(sql);

                while (res.Next())
                    tbl.Rows.Add(res.Get("CVE_CONTRATO")
                        , res.Get("CVE_SEDE")
                        , res.Get("PERIODO")
                        , res.Get("ESQUEMA")
                        , res.Get("TIPODEPAGO")
                        , res.Get("NUMPAGOS")
                        , res.Get("FECHADECONTRATO")
                        , res.Get("MONTO")
                        , res.Get("MONTO_IVA")
                        , res.Get("MONTO_IVARET")
                        , res.Get("MONTO_ISRRET"));

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Contratos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                   // ws.Column(1).Width = 20;
                   // ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:K1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Contratos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Contratos", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Esquemas" + e.Message, sesion);
            }
        }
        
        public string SetContrato()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) return "-1";

			sesion.vdata["IDSIU"] = Request.Params["IDSIU"];
			sesion.vdata["CVE_CONTRATO"] = Request.Params["cve_contrato"];
			sesion.vdata["CVE_SEDE"] = Request.Params["cve_sede"];
			sesion.vdata["PERIODO"] = Request.Params["periodo"];
			sesion.vdata["CVE_NIVEL"] = Request.Params["cve_nivel"];
			sesion.vdata["ID_ESQUEMA"] = Request.Params["id_esquema"];
			sesion.saveSession();
			return "0";
		}
        
        public void ConvertPDF()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) return;
            
            // convert HTML to PDF
            byte[] pdfBuffer = null;

            // convert HTML code
            ContratosWebModel conModel = new ContratosWebModel();
            conModel.Clave = sesion.vdata["CVE_CONTRATO"];
            conModel.Edit();
            string htmlCode = HttpUtility.UrlDecode(conModel.Formato, System.Text.Encoding.Default);

            ContratosWebPDFModel model = new ContratosWebPDFModel();
            model.Sede = sesion.vdata["Sede"];  // from combobox Sedes
            model.IDSIU = sesion.vdata["IDSIU"];
            model.PERIDO = sesion.vdata["PERIODO"];
            model.CVE_SEDE = sesion.vdata["CVE_SEDE"]; // from QEntregaContratos01
            model.ID_ESQUEMA = sesion.vdata["ID_ESQUEMA"];

            model.getDatos();
            htmlCode = model.fill(htmlCode);

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
            Response.AddHeader("content-disposition", "inline; filename=Contrato.pdf");
            Response.BinaryWrite(pdfBuffer);
            doc.Close();
        }
    }
}