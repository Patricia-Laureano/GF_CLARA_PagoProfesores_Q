using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Controllers.Herramientas;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using static PagoProfesores.Models.Pagos.DepositoModel;

namespace PagoProfesores.Controllers.Pagos
{
    public class DepositoController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public DepositoController()
        {
            db = new database();

			string[] scripts = {
				"plugins/Angular/jquery.fileupload.js",
				"js/Pagos/GestiondePagos/deposito.js"
			};
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10131,  Element = "Controller" }, //PERMISO ACCESO REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10132,  Element = "formbtnAuditar" }, //PERMISO EDITAR REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10133,  Element = "formbtnGenerar" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
            };
        }
        
        // GET: Recibos
        public ActionResult Start()
        {
			if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

			DepositoModel model = new DepositoModel();
			model.sesion = sesion;
			model.clean();

			Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Pagos", "Gestión de Pagos", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "ID_DEPOSITO", "ASC", sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Gestión de Pagos Start", LOG.CONSULTA, "Ingresa Pantalla Deposito", sesion);

            return View(Factory.View.Access + "Pagos/GestiondePagos/Deposito/Start.cshtml");
        }

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string fechai = "", string fechaf = "", string filter = "")
		{
			if (sesion == null)
				if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

			DataTable table = new DataTable();

			table.TABLE = "DEPOSITOS_TMP";
            
			table.COLUMNAS =
				new string[] {
					"Nombre Beneficiario", "Tipo Instrucción", "Código Clave", "Status Pago",
					"Fecha Aplicación / Pago", "Plaza Pago", "Sucusal Pago", "Forma Pago", "Tipo Cuenta", "Banco Receptor",
					"Cuenta Abono", "Importe Pago",
					"Clave Beneficiario", "Referencia Pago",
					"Concepto Pago", "Días Vigencia", "Campo para Ordenar Información"
				};
			table.CAMPOS =
				new string[] { "ID_DEPOSITO",
					"NOMBRE_BENEFICIARIO", "TIPO_INSTRUCCION", "CODIGO_CLAVE", "STATUS_PAGO",
					"FECHA_APLICACION_PAGO", "PLAZA_PAGO", "SUCUSAL_PAGO", "FORMA_PAGO", "TIPO_CUENTA", "BANCO_RECEPTOR",
					"CUENTA_ABONO", "IMPORTE_PAGO",
					"CLAVE_BENEFICIARIO", "REFERENCIA_PAGO",
					"CONCEPTO_PAGO", "DIAS_VIGENCIA", "INFO", "ERRORES"
				};
			table.CAMPOSSEARCH =
				new string[] { "FECHA_APLICACION_PAGO", "CONCEPTO_PAGO" };
			table.CAMPOSHIDDEN =
				new string[] { "ID_DEPOSITO", "ERRORES" };

			table.dictColumnFormat.Add("NOMBRE_BENEFICIARIO", delegate (string data, ResultSet res)
			{
				return data.Replace(" ", "&nbsp;");
			});

			table.dictColumnFormat.Add("FECHA_APLICACION_PAGO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)DEPOSITO.FECHA_APLICACION_PAGO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px; width:100%;\">" + data + "&nbsp;</div>";
			});

            table.dictColumnFormat.Add("CUENTA_ABONO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)DEPOSITO.CUENTA_ABONO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px; width:100%;\">" + data + "&nbsp;</div>";
			});

			table.dictColumnFormat.Add("CLAVE_BENEFICIARIO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)DEPOSITO.CLAVE_BENEFICIARIO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px; width:100%;\">" + data + "&nbsp;</div>";
			});

			table.dictColumnFormat.Add("CONCEPTO_PAGO", delegate (string data, ResultSet res)
			{
				data = data.Replace(" ", "&nbsp;");

                string retorna = data;

                if (res.Get("ERRORES")[(int)DEPOSITO.CONCEPTO_PAGO] == '0')
                    retorna = "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px; width:100%;\">" + data + "&nbsp;</div>";

                if (res.Get("ERRORES")[(int)DEPOSITO.ID_ESTADODECUENTA] == '0')
                    retorna = "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px; width:100%;\">" + data + "&nbsp;</div>";

                return retorna;
			});

            table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "ID_DEPOSITO";

			table.TABLECONDICIONSQL = "(USUARIO=" + sesion.pkUser + ")";

			table.enabledButtonControls = false;

			return table.CreateDataTable(sesion);
		}

		public ActionResult ProcessExcel()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content("-1"); }

			string fileName = Request.Params["filename"];
            string sedes = Request.Params["sedes"];
            string path = Path.Combine(Server.MapPath("~/Upload/Depositos"), fileName);

			string real_path;
			string aux;
			moveFile(fileName, out real_path, out aux);

			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
			{
				return Content("-1");
			}
			List<DepositoModel> listDepositos;
			if (ValidarExcel(real_path, sesion, out listDepositos,sedes))
			{
				int total, regists;
				if (ValidarDatosYGuardar(listDepositos, out total, out regists, sedes))
					return Json(new { msg = Notification.Succes("" + total + " depositos auditados correctamente") });
				else
					return Json(new { msg = Notification.Warning("Depositos auditados: " + regists + " / " + total + "") });
			}
			else
				if (listDepositos.Count > 0)
				return Json(new { msg = Notification.Error("Algunos datos del archivo de Excel no son validos.") });
			else
				return Json(new { msg = Notification.Error("No se han encontrado datos validos en el archivo de Excel.") });
		}

		public bool moveFile(string fileName, out string real_path, out string virtual_path)
		{
			real_path = "";
			virtual_path = "";

			if (fileName == null || fileName == "")
				return false;
			string subFolder = ("Depositos/");

			string path_src = Server.MapPath("~/Upload/");
			string path_tar = Server.MapPath("~/Upload/" + subFolder);
			if (Directory.Exists(path_tar) == false)
				Directory.CreateDirectory(path_tar);
			try
			{
				System.IO.File.Move(path_src + fileName, path_tar + fileName);

				real_path = path_tar + fileName;
				virtual_path = subFolder + fileName;
				return true;
			}
			catch (Exception) { }
			return false;
		}
		
		void print(XmlElement node, int level)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < level; i++)
				sb.Append("  ");
			sb.Append("'");
			sb.Append(node.InnerText);
			sb.Append("'");
			Debug.WriteLine(sb.ToString());

			foreach (XmlElement item in node.ChildNodes)
			{
				print(item, level + 1);
			}
		}

		static string RemoveDiacritics(string text)
		{
			var normalizedString = text.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}

			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

		public bool ValidarExcel(string fileName, SessionDB sesion, out List<DepositoModel> listModels,string sedes)
		{
			listModels = new List<DepositoModel>();

			DepositoModel auxModel = new DepositoModel();
           
            auxModel.sesion = sesion;
			auxModel.cargaListas(sedes);

			// Cargar el excel en los modelos.
			try
			{
				Debug.WriteLine(fileName + " exists: " + (System.IO.File.Exists(fileName) ? "Yes" : "No"));
				using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(fileName)))
				{
					// 1.- Get the first worksheet in the workbook
					ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
					Dictionary<string, int> col = new Dictionary<string, int>();

					int start = worksheet.Dimension.Start.Column;
					int end = worksheet.Dimension.End.Column;
					int y = worksheet.Dimension.Start.Row;
					int y2 = y + 1;
					for (int x = start; x <= end; x++)
					{
						string head = RemoveDiacritics(
							(worksheet.Cells[y, x].Text.Replace("/", "").Trim() + " " +
							worksheet.Cells[y2, x].Text.Replace("/", "").Trim()).Trim()
							).Replace(" ", "_").ToUpper();
						if (col.Keys.Contains(head) == false)
							col.Add(head, x);
					}
					start = 2 + worksheet.Dimension.Start.Row;  // se le suma 2 por las cabeceras
					end = worksheet.Dimension.End.Row;

					int errores = 0;
					
					for (int row = start; row <= end; row++)
						try
						{
							// ------------------- Parche para excluir las lineas vacias -------------------
							bool emptyLine = true;
							for (int i = 1; i <= 15; i++)
								if (string.IsNullOrWhiteSpace(worksheet.Cells[row, i].Text) == false) { emptyLine = false; break; }
							if (emptyLine)
								continue;
							// -----------------------------------------------------------------------------

							DepositoModel model = new DepositoModel();
							model.ReadWorkSheetRow(worksheet, row, col);
							model.sesion = sesion;
							model.CopiaListasDesde(auxModel);
							listModels.Add(model);
						}
						catch (Exception) { errores++; }
				} // the using statement calls Dispose() which closes the package.

				sesion.vdata.Remove("DepositosExcelError");
				sesion.saveSession();

				auxModel.clean();
				return true;
			}
			catch (Exception)
            {
                return false;
            }
		}

		public bool ValidarDatosYGuardar(List<DepositoModel> list, out int total, out int cuantos,string sedes)
		{
			total = list.Count;
			cuantos = 0;
			bool ok = true;
			foreach (DepositoModel model in list)
			{
				bool valid = model.Validate(sedes);
				model.USUARIO = sesion.pkUser.ToString();
				// Si el deposito es valido y se pudo registrar.
				valid = valid & model.Add_TMP();
				if (valid)
					cuantos++;
				ok = ok & valid;
			}
			// Se regresa true si todas las validaciones fueron correctas.
			return ok;
		}

		public void ExportExcel()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return; }

			try
			{

				System.Data.DataTable tbl = new System.Data.DataTable();
				tbl.Columns.Add("Tipo", typeof(string));
				tbl.Columns.Add("Código", typeof(string));
				tbl.Columns.Add("Status", typeof(string));
				tbl.Columns.Add("Fecha", typeof(string));
				tbl.Columns.Add("Plaza", typeof(string));
				tbl.Columns.Add("Sucusal", typeof(string));
				tbl.Columns.Add(" ", typeof(string));
				tbl.Columns.Add("Forma", typeof(string));
				tbl.Columns.Add("Tipo ", typeof(string));
				tbl.Columns.Add("Banco", typeof(string));
				tbl.Columns.Add("Cuenta", typeof(string));
				tbl.Columns.Add("Importe", typeof(string));
				tbl.Columns.Add("Clave", typeof(string));
				tbl.Columns.Add("Nombre", typeof(string));
				tbl.Columns.Add("Referencia", typeof(string));
				tbl.Columns.Add("Concepto", typeof(string));
				tbl.Columns.Add("Días", typeof(string));
				tbl.Columns.Add("Campo para Ordenar", typeof(string));
				tbl.Rows.Add(
					"Instrucción", "Clave", "Pago", "Aplicación/Pago", "Pago", "Pago", " ",
					"Pago", "Cuenta", "Receptor", "Abono", "Pago", "Beneficiario", "Beneficiario", "Pago", "Pago", "Vigencia", "Información");

				List<DepositoModel> listModels = new List<DepositoModel>();
				DepositoModel auxModel = new DepositoModel();
				auxModel.sesion = sesion;
				ResultSet res = auxModel.getRowsTable();

				while (res.Next())
				{
					DepositoModel model = new DepositoModel();
					model.sesion = sesion;
					model.edit(res);
					tbl.Rows.Add(
						model.TIPO_INSTRUCCION
						, model.CODIGO_CLAVE
						, model.STATUS_PAGO
						, model.FECHA_APLICACION_PAGO
						, model.PLAZA_PAGO
						, model.SUCUSAL_PAGO
						, ""
						, model.FORMA_PAGO
						, model.TIPO_CUENTA
						, model.BANCO_RECEPTOR
						, model.CUENTA_ABONO
						, model.IMPORTE_PAGO
						, model.CLAVE_BENEFICIARIO
						, model.NOMBRE_BENEFICIARIO
						, model.REFERENCIA_PAGO
						, model.CONCEPTO_PAGO
						, model.DIAS_VIGENCIA
						, model.INFO
						);

					listModels.Add(model);
				}

				using (ExcelPackage pck = new ExcelPackage())
				{
					//Create the worksheet
					ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Depositos");

					//Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
					ws.Cells["A1"].LoadFromDataTable(tbl, true);
					ws.Cells["A1:R2"].AutoFitColumns();

					//Format the header for column 1-3
					using (ExcelRange rng = ws.Cells["A1:R2"])
					{
						rng.Style.Font.Bold = true;
						rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
						rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
						rng.Style.Font.Color.SetColor(Color.White);
					}
					// PAGO APLICADO

					Dictionary<int, string> cols = new Dictionary<int, string>();
					cols.Add((int)DEPOSITO.FECHA_APLICACION_PAGO, "D");
					cols.Add((int)DEPOSITO.CUENTA_ABONO, "K");
					cols.Add((int)DEPOSITO.CLAVE_BENEFICIARIO, "M");
					cols.Add((int)DEPOSITO.CONCEPTO_PAGO, "P");

					int row = 3;
					foreach (DepositoModel model in listModels)
					{
						// Si el campo ERRORES tiene una validacion unsuccess se agrega a la lista.
						if (model.ERRORES.IndexOf('0') >= 0)
							foreach (KeyValuePair<int, string> pair in cols)
							{
								if (model.ERRORES[pair.Key] == '0')
									using (ExcelRange rng = ws.Cells["" + pair.Value + row + ":" + pair.Value + row])
									{
										//rng.Style.Font.Bold = true;
										rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
										rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(239, 219, 67));  //Set color to dark blue
										//rng.Style.Font.Color.SetColor(Color.White);
									}
							}
						row++;
					}
					/*
					//Example how to Format Column 1 as numeric 
					using (ExcelRange col = ws.Cells[3, 1, 3 + tbl.Rows.Count, 1])
					{
						col.Style.Numberformat.Format = "#,##0.00";
						col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					}
					//*/

					//Write it back to the client
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.AddHeader("content-disposition", "attachment;  filename=Depositos_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
					Response.BinaryWrite(pck.GetAsByteArray());
				}

				Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Estado de Cuenta", sesion);
			}
			catch (Exception e)
			{
				ViewBag.Notification = Notification.Error(e.Message);
				Log.write(this, "Start", LOG.ERROR, "Exporta Excel Estado de Cuenta" + e.Message, sesion);
			}
		}



		[HttpPost]
		public ActionResult Generar(DepositoModel model)
		{
			sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
			if (sesion == null)
				return Content("-1");
			model.sesion = sesion;
			int total, cuantos;
			if (model.Generar(out total, out cuantos))
			{
				return Json(new { msg = Notification.Succes("Depositos guardados correctamente") });
			}
			else
				return Json(new { msg = Notification.Warning("Depositos generados: " + cuantos + " / " + total) });
		}

		[HttpGet]
        public string getBanco(string Banco = "")
        {
            DispersionModel model = new DispersionModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in model.getBanco())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }

    }

}