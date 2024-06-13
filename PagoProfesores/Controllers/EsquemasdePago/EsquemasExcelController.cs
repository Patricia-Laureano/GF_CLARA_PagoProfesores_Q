using ConnectDB;
using Factory;
using OfficeOpenXml;
using PagoProfesores.Models.EsquemasdePago;
using Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;
using static PagoProfesores.Models.EsquemasdePago.EsquemasExcelModel;

namespace PagoProfesores.Controllers.EsquemasdePago
{
    public class EsquemasExcelController : Controller
	{
		private database db;
		private List<Factory.Privileges> Privileges;
		private SessionDB sesion;

		public EsquemasExcelController()
		{
			db = new database();

			Scripts.SCRIPTS = new string[] {
				"plugins/Angular/jquery.fileupload.js",
				"js/EsquemasdePago/EsquemasExcel.js"
			};

			Privileges = new List<Factory.Privileges> {
                new Factory.Privileges { Permiso = 10168,  Element = "Controller" }, //PERMISO ACCESO EstadodeCuentaWeb
                new Factory.Privileges { Permiso = 10169,  Element = "formbtnAuditar" }, //PERMISO DETALLE EstadodeCuentaWeb
                new Factory.Privileges { Permiso = 10170,  Element = "formbtnGenerar" }, //PERMISO DETALLE EstadodeCuentaWeb
            };
		}

		// GET: NominaExcel
		public ActionResult Start()
		{
            EsquemasExcelModel model = new EsquemasExcelModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
            model.clean();

			Main view = new Main();
			ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
			ViewBag.Main = view.createMenu("Esquemas de pago", "Importar Esquemas", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "ID_ESQUEMA", "ASC", sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);
            ViewBag.TIPO_BLOQUEO = getTiposDeBloqueo(new EsquemasModel());

			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return View(Factory.View.NotAccess);

            ViewBag.sede = view.createLevels(sesion, "sedes");

            return View(Factory.View.Access + "EsquemasdePago/EsquemasExcel/Start.cshtml");
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null/*, string filter = ""*/)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

			DataTable table = new DataTable();

			table.TABLE = "V_ESQUEMASDEPAGO_TMP";
			table.COLUMNAS = new string[] {             "Reg",        "Esquema",       "Sede",     "Periodo",   "Ctto",         "F. Inicio",   "F. Fin",   "Semanas",   "Pagos",   "Concepto", "F. Pago",   "F. recibo" };
			table.CAMPOS = new string[] { "ID_ESQUEMA", "REGISTRADO", "ESQUEMADEPAGO", "CVE_SEDE", "PERIODO", "CONTRATO", "FECHAINICIO", "FECHAFIN", "NOSEMANAS", "NOPAGOS", "CONCEPTO", "FECHAPAGO", "FECHARECIBO", "ERRORES", "E_DUPLICADO", "CVE_CONTRATO" };
			table.CAMPOSSEARCH = new string[] { "ESQUEMADEPAGO", "CONCEPTO" };
			table.CAMPOSHIDDEN = new string[] { "ID_ESQUEMA", "ERRORES", "E_DUPLICADO", "CVE_CONTRATO" };

            //table.TABLECONDICIONSQL = "CVE_SEDE='" + filter + "'" + " AND USUARIO =" + "'" + sesion.nickName + "'";
            table.TABLECONDICIONSQL = " USUARIO =" + "'" + sesion.nickName + "'";

            table.dictColumnFormat.Add("REGISTRADO", delegate (string data, ResultSet res)
			{
				if (data == "False")
					return (res.Get("E_DUPLICADO") == "True") ? "<i class='fa fa-exclamation-triangle' style='cursor:default; color:DarkRed;' title='Duplicado'></i>" : "";
				else
					return (data == "True") ? "<i class='fa fa-check' style='color:Green;'></i>" : "";
			});

			table.dictColumnFormat.Add("ESQUEMADEPAGO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.ESQUEMADEPAGO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});
            table.dictColumnFormat.Add("CVE_SEDE", delegate (string data, ResultSet res)
            {
                return (res.Get("ERRORES")[(int)ESQUEMA.CVE_SEDE] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
            });
            table.dictColumnFormat.Add("PERIODO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.PERIODO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});
			table.dictColumnFormat.Add("CONTRATO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.CVE_CONTRATO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});
			table.dictColumnFormat.Add("FECHAINICIO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.FECHAINICIO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});
			table.dictColumnFormat.Add("FECHAFIN", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.FECHAFIN] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});
			table.dictColumnFormat.Add("NOSEMANAS", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.NOSEMANAS] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});
			table.dictColumnFormat.Add("NOPAGOS", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.NOPAGOS] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});
			table.dictColumnFormat.Add("CONCEPTO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.CONCEPTO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data.Replace(" ", "&nbsp;") + "</div>";
			});
			table.dictColumnFormat.Add("FECHAPAGO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.FECHAPAGO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px; width:100%;\">" + data + "&nbsp;</div>";
			});
			table.dictColumnFormat.Add("FECHARECIBO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)ESQUEMA.FECHARECIBO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px; width:100%;\">" + data + "&nbsp;</div>";
			});

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "ID_ESQUEMA";

			table.enabledButtonControls = false;

			return table.CreateDataTable(sesion);
		}

		public bool moveFile(string fileName, out string real_path, out string virtual_path)
		{
			real_path = "";
			virtual_path = "";

			if (fileName == null || fileName == "")
				return false;
			string subFolder = ("Esquemas/");
			
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

		public ActionResult processExcel()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content("-1"); }

			string fileName = Request.Params["filename"];
            string sede = Request.Params["sede"];
            string path = Path.Combine(Server.MapPath("~/Upload/Esquemas"), fileName);

			string real_path;
			string aux;

            moveFile(fileName, out real_path, out aux);
            
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
			{
				return Content("-1");
			}

			List<EsquemasExcelModel> listModels;

			// Proceso de carga ...
			if(ValidarExcel(real_path, sesion, out listModels, sede))
			{
				return Content("0");
			}

			return Json(new { msg = Notification.Warning("") });
		}

		public bool ValidarExcel(string fileName, SessionDB sesion, out List<EsquemasExcelModel> listModels, string sede)
		{
			listModels = new List<EsquemasExcelModel>();

			EsquemasExcelModel auxModel = new EsquemasExcelModel();
			auxModel.sesion = sesion;
			auxModel.clean();
            auxModel.SedeX = sede;
			auxModel.cargaListas();
			
			int paso = 1;
			ESQUEMA current = ESQUEMA.ESQUEMADEPAGO;

			// Cargar el excel en los modelos.
			try
			{
				using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(fileName)))
				{
					paso = 1;
					// 1.- Get the first worksheet in the workbook
					ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

					Dictionary<string, int> col = new Dictionary<string, int>();
                    Dictionary<ESQUEMA, object> dataValid = new Dictionary<ESQUEMA, object>();

					int start = worksheet.Dimension.Start.Column;
					int end = worksheet.Dimension.End.Column;
					int y = worksheet.Dimension.Start.Row;
					for (int x = start; x <= end; x++)
					{
						string head = worksheet.Cells[y, x].Text.ToUpper();
						col.Add(head, x);
					}
					start = 1 + worksheet.Dimension.Start.Row;  // se le suma 1 por las cabeceras
					end = worksheet.Dimension.End.Row;

					DateTime dt_1 = DateTime.Now;
					for (int row = start; row <= end; row++)
					{
						// ------------------- Parche para excluir las lineas vacias -------------------
						bool emptyLine = true;
						for (int i = 1; i <= 14; i++)
							if (string.IsNullOrWhiteSpace(worksheet.Cells[row, i].Text) == false) { emptyLine = false; break; }
						if (emptyLine)
							continue;
						// -----------------------------------------------------------------------------

						EsquemasExcelModel model = new EsquemasExcelModel();
						model.sesion = sesion;
						paso = 2;

						// 2.- Se asignan los valores al modelo
						current = ESQUEMA.ESQUEMADEPAGO;
						model.EsquemaDePago = worksheet.Cells[row, col["ESQUEMADEPAGO"]].Text;

						current = ESQUEMA.CVE_SEDE;
						model.Sede = worksheet.Cells[row, col["CVE_SEDE"]].Text;

						current = ESQUEMA.PERIODO;
						model.Periodo = worksheet.Cells[row, col["PERIODO"]].Text;

						current = ESQUEMA.ESQUEMADEPAGODES;
						model.EsquemaDePagoDes = worksheet.Cells[row, col["ESQUEMADEPAGODES"]].Text;

						current = ESQUEMA.CVE_CONTRATO;
						model.ClaveContrato = worksheet.Cells[row, col["CVE_CONTRATO"]].Text;

						current = ESQUEMA.FECHAINICIO;
						model.FechaInicio = worksheet.Cells[row, col["FECHAINICIO"]].Text;

						current = ESQUEMA.FECHAFIN;
						model.FechaFin = worksheet.Cells[row, col["FECHAFIN"]].Text;

						current = ESQUEMA.NOSEMANAS;
						model.NumSemanas = worksheet.Cells[row, col["NOSEMANAS"]].Text;

						current = ESQUEMA.NOPAGOS;
						model.NumPagos = worksheet.Cells[row, col["NOPAGOS"]].Text;

						current = ESQUEMA.BLOQUEOCONTRATO;
						model.BloqueoContrato = worksheet.Cells[row, col["BLOQUEO"]].Text;

						current = ESQUEMA.CONCEPTO;
						model.Concepto = worksheet.Cells[row, col["CONCEPTO"]].Text;

						current = ESQUEMA.FECHAPAGO;
						model.FechaPago = worksheet.Cells[row, col["FECHAPAGO"]].Text;

						current = ESQUEMA.FECHARECIBO;
						model.FechaRecibo = worksheet.Cells[row, col["FECHARECIBO"]].Text;

						paso = 3;
						// 3.- Se validan
						model.copiaListasDesde(auxModel);
						model.Validate();

						// 4.- Se guarda en la tabla temporal.
						model.Add_TMP();
						listModels.Add(model);

						// 5.- Se agregan los datos al datatable.
						
					}
					DateTime dt_2 = DateTime.Now;
					Debug.WriteLine("span:" + (dt_2 - dt_1));
				} // the using statement calls Dispose() which closes the package.

				sesion.vdata.Remove("EsquemasExcelError");
				sesion.saveSession();

				return true;
			}
			catch (Exception)
			{
				if (paso == 1)
					sesion.vdata["EsquemasExcelError"] = "Error en archivo de Excel";
				else if (paso == 2)
					sesion.vdata["EsquemasExcelError"] = "No se encuentra la columna '" + current + "'";
				else if (paso == 3)
					sesion.vdata["EsquemasExcelError"] = "Error validando Excel";
				sesion.saveSession();

				return false;
			}
		}

		[HttpPost]
		public ActionResult FindFirstError()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content(""); }
			EsquemasExcelModel model = new EsquemasExcelModel();
			model.sesion = sesion;

			if (sesion.vdata.ContainsKey("EsquemasExcelError"))
			{
				return Json(new { msg = Notification.Error(sesion.vdata["EsquemasExcelError"]) });
			}

			int total;
			int maxErrors;
			string idsError;
			model.FindFirstError(out total, out maxErrors, out idsError);

			if (total == 0)
				return Json(new { msg = Notification.Warning("No se han encontrado registros auditados") });
			else if (maxErrors == 0)
				return Json(new { msg = Notification.Succes("Archivo auditado correctamente") });
			else
				return Json(new { msg = Notification.Error("" + maxErrors + " error(es). Esquemas: " + idsError) });
		}

		[HttpPost]
		public ActionResult Generar()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content(""); }
			EsquemasExcelModel model = new EsquemasExcelModel();
			model.sesion = sesion;

			string str_bloqueos = Request.Params["bloqueos"];

			int total, errores;
			model.generar(str_bloqueos, out total, out errores);
			if (errores == 0)
				return Json(new { msg = Notification.Succes("Esquemas importados correctamente") });
			else
				return Json(new { msg = Notification.Error("Error al momento de importar, " + errores + " / " + total + " registros no importados.") });
		}

		string getTiposDeBloqueo(EsquemasModel model)
		{
			string html = string.Empty;
			int contador = 0;
			Dictionary<string, string> dict = model.Obtener_TipoBloqueo();
			foreach (KeyValuePair<string, string> pair in dict)
				html += "<label class='radio-inline'>\n<input id='TipoBloqueo_" + (contador++) + "' data-idbloqueo='" + pair.Key + "' type='checkbox' value='" + pair.Key + "'> " + pair.Value + "\n</label>";
			html += "<input type='hidden' id='TipoBloqueo_length' value='" + contador + "'>";
			return html;
		}
	}
}