using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Pagos
{
    public class ECW_RetencionesAnualesPeriodoController : Controller
    {
        private SessionDB sesion;
        private database db;

        public ECW_RetencionesAnualesPeriodoController()
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

            table.TABLE = "VESTADO_CUENTA_RETANUAL_PERIODO";
            table.COLUMNAS =
                new string[] { "Sede", "A&ntilde;o", "No Pagos", "Monto", "IVA", "IVA Ret", "ISR Ret", "Tipo Transferecia", "Tipo de pago", "Retenciones" };
            table.CAMPOS =
                new string[] { "CVE_SEDE", "ANIODEPOSITO", "NUMPAGOS", "MONTO", "MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "TIPOTRANSFERENCIA", "TIPODEPAGO", "ID_PERSONA",
                "MESDEPOSITO_INI", "MESDEPOSITO_FIN"};
            table.CAMPOSSEARCH =
                new string[] { "" };
            table.CAMPOSHIDDEN =
                new string[] { "MESDEPOSITO_INI", "MESDEPOSITO_FIN" };

            table.addColumnFormat("ID_PERSONA", delegate (string value, ResultSet res)
            {
                return "<button class='btn btn-sm btn-primary' onclick=\"formPage_RetencionesAnualesPeriodo.verRetencion" +
                    "('CR01','" + CVE_SEDE + "', '" + res.Get("MESDEPOSITO_INI") + "', '" + res.Get("MESDEPOSITO_FIN") + "', '" + res.Get("ANIODEPOSITO") +
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
            condition += " AND ANIODEPOSITO IS NOT NULL ";

            table.TABLECONDICIONSQL = condition;
            table.enabledButtonControls = false;
            
            /*
			List<Retencion> list = Retencion.calcRetencionesAnuales(IDSIU, CVE_SEDE, Periodo, Nivel);
			/* *
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
					<th class='table_cell'>Año</th>
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
					+ "<td class='table_cell'>" + ret.ANIODEPOSITO + "</td>"
					+ "<td class='table_cell'>" + ret.NUMPAGOS + "</td>"
					+ "<td class='table_cell'>" + ret.MONTO + "</td>"
					+ "<td class='table_cell'>" + ret.MONTO_IVA + "</td>"
					+ "<td class='table_cell'>" + ret.MONTO_IVARET + "</td>"
					+ "<td class='table_cell'>" + ret.MONTO_ISRRET + "</td>"
					+ "<td class='table_cell'>" + ret.TIPOTRANSFERENCIA + "</td>"
					+ "<td class='table_cell'>" + ret.TIPODEPAGO + "</td>"
					+ "<td class='table_cell'>" + "<button class='btn btn-sm btn-primary' onclick=\"formPage_RetencionesAnuales.verRetencion" +
						"('CR01','" + CVE_SEDE + "', '" + ret.MESDEPOSITO_INI + "', '" + ret.MESDEPOSITO_FIN + "', '" + ret.ANIODEPOSITO +
						"', '" + ret.MONTO.ToString("F") + "', '" + ret.MONTO_IVA.ToString("F") + "', '" + ret.MONTO_IVARET.ToString("F") + "', '" + ret.MONTO_ISRRET.ToString("F") + "');\"" +
						" type='button'>Ver retenci&oacute;n</button>" + "</td>"
					+ "</tr>";
			}

			HTML += "</table>";
			//return HTML;

			List<string> listHeaders = new List<string>() { "CVE_SEDE", "MES-ANIO", "NUMPAGOS", "MONTO", "MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "TIPOTRANSFERENCIA", "TIPODEPAGO", "ID_ESTADODECUENTA", "MESDEPOSITO_INI", "MESDEPOSITO_FIN", "ANIODEPOSITO" };
			List<List<string>> matriz = new List<List<string>>();
			foreach (Retencion ret in list)
			{
				List<string> row = new List<string>();
				row.Add(ret.CVE_SEDE);
				row.Add(ret.ANIODEPOSITO.ToString());
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
            return table.CreateDataTable(sesion, "DataTable_RetencionesAnualesPeriodo");
        }


        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("A&ntilde;o", typeof(string));
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

                string sql = "SELECT * FROM VESTADO_CUENTA_RETANUAL_PERIODO WHERE ANIODEPOSITO IS NOT NULL " + union + " " + TABLECONDICIONSQL;

                ResultSet res = db.getTable(sql);

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("CVE_SEDE")
                        , res.Get("ANIODEPOSITO")
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
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Retenciones_Anuales");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                    // ws.Column(1).Width = 20;
                    // ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:j1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Retenciones_Anuales_Periodo.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Retenciones Anuales por Periodo", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Retenciones Anuales por Periodo" + e.Message, sesion);
            }
        }
    }
}
