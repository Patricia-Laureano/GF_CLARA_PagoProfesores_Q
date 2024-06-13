using ConnectDB;
using Factory;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
//using HiQPdf;
using PagoProfesores.Models.CatalogosporSede;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using SelectPdf;

namespace PagoProfesores.Controllers.Pagos
{
    public class ECW_RetencionesMensualesController : Controller
    {
		private SessionDB sesion;
		private database db;

		public ECW_RetencionesMensualesController()
		{
			db = new database();
		}

		// GET: ECW_RetencionesMensuales
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null,
			string Sede = "", string Periodo = "", string Nivel = "")
		{
			if (sesion == null)
				if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return "-1"; }

			string IDSIU = sesion.vdata["IDSIU"];
			string ID_PERSONA = sesion.vdata["ID_PERSONA"];
			string CVE_SEDE = sesion.vdata["Sede"];
			string tipoFactura = sesion.vdata["CVE_TIPOFACTURA"];

			DataTable table = new DataTable();

			table.TABLE = "VESTADO_CUENTA_RETMENSUAL";
			table.COLUMNAS =
				new string[] { "Sede", "Mes-A&ntilde;o", "No Pagos", "Monto", "IVA", "IVA Ret", "ISR Ret", "Tipo Transferecia", "Tipo de pago", "Retenciones" };
			table.CAMPOS =
				new string[] { "CVE_SEDE", "MESANYODEPOSITO", "NUMPAGOS", "MONTO", "MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "TIPOTRANSFERENCIA", "TIPODEPAGO", "ID_PERSONA",
				"MESDEPOSITO", "ANYODEPOSITO"};
			table.CAMPOSSEARCH =
				new string[] { "" };
			table.CAMPOSHIDDEN =
				new string[] { "MESDEPOSITO", "ANYODEPOSITO" };

			table.addColumnFormat("ID_PERSONA", delegate (string value, ResultSet res)
			{
				return "<button class='btn btn-sm btn-primary' onclick=\"formPage_RetencionesMensuales.verRetencion" +
					"('CR01','" + CVE_SEDE + "', '" + res.Get("MESDEPOSITO") + "', '" + res.Get("MESDEPOSITO") + "', '" + res.Get("ANYODEPOSITO") +
					"', '" + res.Get("MONTO") + "', '" + res.Get("MONTO_IVA") + "', '" + res.Get("MONTO_IVARET") + "', '" + res.Get("MONTO_ISRRET") + "', '" + res.Get("ID_ESQUEMA") + "');\"" +
					" type='button'>Ver retenci&oacute;n</button>" + "</td>";
			});

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "CVE_SEDE";

			string condition = " ID_PERSONA='" + ID_PERSONA + "'";
			if (string.IsNullOrWhiteSpace(Sede) == false) condition += " AND CVE_SEDE='" + Sede + "'";
			if (string.IsNullOrWhiteSpace(Periodo) == false) condition += " AND PERIODO='" + Periodo + "'";
			if (string.IsNullOrWhiteSpace(Nivel) == false) condition += " AND CVE_NIVEL='" + Nivel + "'";
			condition += " AND ANYODEPOSITO IS NOT NULL ";

			table.TABLECONDICIONSQL = condition;

			table.enabledButtonControls = false;

			
			return table.CreateDataTable(sesion, "DataTable_RetencionesMensuales");
			//*/


			/* *
			List<Retencion> list = Retencion.calcRetencionesMensuales(IDSIU, CVE_SEDE, Periodo, Nivel, DateTime.Now.Year);

			string HTML = @"
<style type='text/css'>
	.table_begin {
		border: 1px solid #888;
		border-bottom-width: 0;
		border-right-width: 0;
		width: 100%;
	}
	.table_cell {
		border: 1px solid #888;
		border-top-width: 0;
		border-left-width: 0;
		padding: 2px 5px 2px 5px;
	}
</style>
			";

			HTML += "<br/><br/><table class='table_begin'>";

			HTML += @"
				<tr>
					<th class='table_cell'>Sede</th>
					<th class='table_cell'>Mes-año</th>
					<th class='table_cell'>No pagos</th>
					<th class='table_cell'>Monto</th>
					<th class='table_cell'>IVA</th>
					<th class='table_cell'>IVA Ret.</th>
					<th class='table_cell'>ISR Ret</th>
					<th class='table_cell'>Tipo transferencia</th>
					<th class='table_cell'>Tipo de pago</th>
					<th class='table_cell'></th>
				</tr>";
			foreach (Retencion ret in list)
			{
				HTML += "<tr>"
					+ "<td class='table_cell'>" + ret.CVE_SEDE + "</td>"
					+ "<td class='table_cell'>" + (ret.MESDEPOSITO < 10 ? "0" : "") + ret.MESDEPOSITO + "-" + ret.ANIODEPOSITO + "</td>"
					+ "<td class='table_cell'>" + ret.NUMPAGOS + "</td>"
					+ "<td class='table_cell'>" + ret.MONTO + "</td>"
					+ "<td class='table_cell'>" + ret.MONTO_IVA + "</td>"
					+ "<td class='table_cell'>" + ret.MONTO_IVARET + "</td>"
					+ "<td class='table_cell'>" + ret.MONTO_ISRRET + "</td>"
					+ "<td class='table_cell'>" + ret.TIPOTRANSFERENCIA + "</td>"
					+ "<td class='table_cell'>" + ret.TIPODEPAGO + "</td>"
					+ "<td class='table_cell'>" + "<button class='btn btn-sm btn-primary' onclick=\"formPage_RetencionesMensuales.verRetencion" +
						"('CR01','" + CVE_SEDE + "', '" + ret.MESDEPOSITO_INI + "', '" + ret.MESDEPOSITO_FIN + "', '" + ret.ANIODEPOSITO +
						"', '" + ret.MONTO.ToString("F") + "', '" + ret.MONTO_IVA.ToString("F") + "', '" + ret.MONTO_IVARET.ToString("F") + "', '" + ret.MONTO_ISRRET.ToString("F") + "');\"" +
						" type='button'>Ver retenci&oacute;n</button>" + "</td>"
					+ "</tr>";
			}

			HTML += "</table>";
			//return HTML;
			//*/
			/*
			List<string> listHeaders = new List<string>() { "CVE_SEDE", "MES-ANIO", "NUMPAGOS", "MONTO", "MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "TIPOTRANSFERENCIA", "TIPODEPAGO", "ID_ESTADODECUENTA", "MESDEPOSITO_INI", "MESDEPOSITO_FIN", "ANIODEPOSITO" };
			List<List<string>> matriz = new List<List<string>>();
			foreach (Retencion ret in list)
			{
				List<string> row = new List<string>();
				row.Add(ret.CVE_SEDE);
				row.Add((ret.MESDEPOSITO < 10 ? "0" : "") + ret.MESDEPOSITO + "-" + ret.ANIODEPOSITO);
				row.Add(ret.NUMPAGOS.ToString());
				row.Add(ret.MONTO.ToString("F"));
				row.Add(ret.MONTO_IVA.ToString("F"));
				row.Add(ret.MONTO_IVARET.ToString("F"));
				row.Add(ret.MONTO_ISRRET.ToString("F"));
				row.Add(ret.TIPOTRANSFERENCIA);
				row.Add(ret.TIPODEPAGO);
				row.Add(ret.ID_ESTADODECUENTA.ToString());
				// ---
				row.Add(ret.MESDEPOSITO_INI.ToString());
				row.Add(ret.MESDEPOSITO_FIN.ToString());
				row.Add(ret.ANIODEPOSITO.ToString());
				matriz.Add(row);
			}
			ResultSet RES = new ResultSet();
			RES.makeFromMatriz(listHeaders, matriz);
			//*/
		}


        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {

                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Mes-A&ntilde;o", typeof(string));
                tbl.Columns.Add("No Pagos", typeof(string));              
                tbl.Columns.Add("Monto", typeof(string));
                tbl.Columns.Add("IVA", typeof(string));
                tbl.Columns.Add("IVA Ret", typeof(string));
                tbl.Columns.Add("ISR Ret", typeof(string));
                tbl.Columns.Add("Tipo Transferecia", typeof(string));
                tbl.Columns.Add("Tipo de pago", typeof(string));
                tbl.Columns.Add("Retenciones", typeof(string));
             


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
                if (condition.Count > 0) union = " AND ";

                string sql = "SELECT * FROM VESTADO_CUENTA_RETMENSUAL WHERE ANYODEPOSITO IS NOT NULL " + union + " " + TABLECONDICIONSQL;


                ResultSet res = db.getTable(sql);
                

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add( res.Get("CVE_SEDE")
                        , res.Get("MESANYODEPOSITO")                     
                        , res.Get("NUMPAGOS")
                        , res.Get("MONTO")
                        , res.Get("MONTO_IVA")
                        , res.Get("MONTO_IVARET")
                        , res.Get("MONTO_ISRRET")
                        , res.Get("TIPOTRANSFERENCIA")
                        , res.Get("TIPODEPAGO")
                        , res.Get("ID_PERSONA")
                      );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Retenciones_Mensuales");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                    // ws.Column(1).Width = 20;
                    // ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:J1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Retenciones_mensuales.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Retenciones Mensuales", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Retenciones Mensuales" + e.Message, sesion);
            }
        }
        
        //
        //  PDF
        //
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
            sesion.vdata["ID_ESQUEMA"] = Request.Params["id_esquema"];


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
            model.ID_ESQUEMA = sesion.vdata["ID_ESQUEMA"];

            model.getDatos();
            htmlCode = model.fill(htmlCode);

            string htmlString = htmlCode;
            string baseUrl = "";// collection["TxtBaseUrl"];

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


    }// </>

}