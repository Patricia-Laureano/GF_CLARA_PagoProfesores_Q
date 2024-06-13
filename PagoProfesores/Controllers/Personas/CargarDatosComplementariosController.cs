using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Controllers.Herramientas;
using PagoProfesores.Models.Pagos;
using PagoProfesores.Models.Personas;
using Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using static PagoProfesores.Models.Personas.CargarDatosComplementariosModel;

namespace PagoProfesores.Controllers.Personas
{
	public class CargarDatosComplementariosController : Controller
	{
		private database db;
		private List<Factory.Privileges> Privileges;
		private SessionDB sesion;

		public CargarDatosComplementariosController()
		{
			db = new database();

			Scripts.SCRIPTS = new string[] { "js/Personas/CargarDatosComplementarios.js" };

			Privileges = new List<Factory.Privileges> {
				 new Factory.Privileges { Permiso = 10281,  Element = "Controller" }, //PERMISO ACCESO EstadodeCuentaWeb
                 new Factory.Privileges { Permiso = 10281,  Element = "formbtnImportar" }, //PERMISO EDITAR  
            };
		}

		// GET: NominaExcel
		public ActionResult Start()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content(""); }

			CargarDatosComplementariosModel model = new CargarDatosComplementariosModel();
			model.sesion = sesion;
			model.clean();

			Main view = new Main();
			ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
			//ViewBag.Main = view.createMenu(7, 10, sesion);
			ViewBag.Main = view.createMenu("Personas", "Importar PA Multicampus", sesion);
			ViewBag.DataTable = CreateDataTable(10, 1, null, "ID_PBI_TMP", "ASC", sesion);
			ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

			//Intercom
			ViewBag.User = sesion.nickName.ToString();
			ViewBag.Email = sesion.nickName.ToString();
			ViewBag.FechaReg = DateTime.Today;

			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return View(Factory.View.NotAccess);

			Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla CargarDatosComplementarios", sesion);

			return View(Factory.View.Access + "Personas/CargarDatosComplementarios/Start.cshtml");
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

			DataTable table = new DataTable();

			table.TABLE = "QPowerBI_TMP";
			table.COLUMNAS = new string[] {"IdBanner", "Docente", "RFC", "CURP", "Máxima grado de estudio", "Id"};
			table.CAMPOS = new string[] { "IDBANNER", "DOCENTE", "RFC", "CURP", "MAXIMO_GRADO_ACADEMICO", "ID_PBI_TMP"};
			table.CAMPOSSEARCH = new string[] { "IDBANNER", "DOCENTE" };
			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "ID_PBI_TMP";
			table.TABLECONDICIONSQL = "USUARIO_IMPORTA=" + sesion.pkUser;
			table.enabledButtonControls = false;

			return table.CreateDataTable(sesion);
		}

		[HttpPost]
		public void SendFile(HttpPostedFileBase file)
		{
			// Verify that the user selected a file
			if (file != null && file.ContentLength > 0)
			{
				// extract only the filename
				var originalName = Path.GetFileName(file.FileName);
				string ext = Path.GetExtension(file.FileName);
				DateTime now = DateTime.Now;
				string newFileName = "NOM-" + Math.Abs((now.Ticks << 48) | (now.Ticks >> 16)) + ext;

				var path = Path.Combine(Server.MapPath("~/Upload/CargarDatosComplementarios"), newFileName);
				file.SaveAs(path);

				if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return; }

				System.Data.DataTable table = new System.Data.DataTable();
				List<CargarDatosComplementariosModel> listModels = new List<CargarDatosComplementariosModel>();

				// Proceso de carga ...
				ValidarExcel(path, sesion, table, listModels);

				AuditarExcel(originalName, table, listModels);
			}
		}

		public bool ValidarExcel(string fileName, SessionDB sesion, System.Data.DataTable tbl, List<CargarDatosComplementariosModel> listModels)
		{
			ProgressBarCalc progressbar = new ProgressBarCalc(sesion, "CargarDatosComplementarios");
			progressbar.prepare();

			CargarDatosComplementariosModel auxModel = new CargarDatosComplementariosModel();
			auxModel.sesion = sesion;
			auxModel.clean();

			tbl.Columns.Add("IDBANNER", typeof(string));
			tbl.Columns.Add("DOCENTE", typeof(string));
			tbl.Columns.Add("RFC", typeof(string));
			tbl.Columns.Add("CURP", typeof(string));
			tbl.Columns.Add("MAXIMO_GRADO_ACADEMICO", typeof(string));
			tbl.Columns.Add("TIPO_DE_DOCENTE", typeof(string));

			tbl.Columns.Add("NRC", typeof(string));
			tbl.Columns.Add("HORAS_PROGRAMADAS", typeof(string));
			tbl.Columns.Add("TABULADOR", typeof(string));

			tbl.Columns.Add("ESCUELA", typeof(string));
			tbl.Columns.Add("CAMPUS", typeof(string));
			tbl.Columns.Add("MATERIA", typeof(string));
			tbl.Columns.Add("CURSO", typeof(string));
			tbl.Columns.Add("NOMBRE_DE_MATERIA", typeof(string));

			tbl.Columns.Add("PERIODO", typeof(int));
			tbl.Columns.Add("PARTE_DEL_PERIODO", typeof(string));
			tbl.Columns.Add("FECHA_DE_INICIO", typeof(string));
			tbl.Columns.Add("FECHA_FIN", typeof(string));
			tbl.Columns.Add("NUMERO_INSCRITOS", typeof(string));
			tbl.Columns.Add("USUARIO_IMPORTA", typeof(string));
			int paso = 1;
			CARGARDATOS current = CARGARDATOS.IDBANNER;

			// Cargar el excel en los modelos.
			try
			{
				using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(fileName)))
				{
					paso = 1;
					// 1.- Get the first worksheet in the workbook
					ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

					Dictionary<string, int> col = new Dictionary<string, int>();
					Dictionary<CARGARDATOS, object> dataValid = new Dictionary<CARGARDATOS, object>();

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
					progressbar.init(end - start + 2);
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

						CargarDatosComplementariosModel model = new CargarDatosComplementariosModel();
						model.sesion = sesion;
						paso = 2;
						// 2.- Se asignan los valores al modelo
						current = CARGARDATOS.IDBANNER;
						model.IDBANNER = worksheet.Cells[row, col["ID"]].Text;

						current = CARGARDATOS.DOCENTE;
						model.DOCENTE = worksheet.Cells[row, col["DOCENTE"]].Text;

						current = CARGARDATOS.RFC;
						model.RFC = worksheet.Cells[row, col["RFC"]].Text;

						current = CARGARDATOS.CURP;
						model.CURP = worksheet.Cells[row, col["CURP"]].Text;

						current = CARGARDATOS.MAXIMO_GRADO_ACADEMICO;
						model.MAXIMO_GRADO_ACADEMICO = worksheet.Cells[row, col["MAXIMO_GRADO_ACADEMICO"]].Text;

						current = CARGARDATOS.TIPO_DOCENTE;
						model.TIPO_DOCENTE = worksheet.Cells[row, col["TIPO_DE_DOCENTE"]].Text;

						current = CARGARDATOS.NRC;
						model.NRC = worksheet.Cells[row, col["NRC"]].Text;

						current = CARGARDATOS.HORAS_PROGRAMADAS;
						model.HORAS_PROGRAMADAS = worksheet.Cells[row, col["HORAS_PROGRAMADAS"]].Text;

						current = CARGARDATOS.TABULADOR_DI;
						model.TABULADOR_DI = worksheet.Cells[row, col["TABULADOR"]].Text;

						current = CARGARDATOS.ESCUELA;
						model.ESCUELA = worksheet.Cells[row, col["ESCUELA"]].Text;

						current = CARGARDATOS.CAMPUS;
						model.CAMPUS = worksheet.Cells[row, col["CAMPUS"]].Text;

						current = CARGARDATOS.MATERIA;
						model.MATERIA = worksheet.Cells[row, col["MATERIA"]].Text;

						current = CARGARDATOS.CURSO;
						model.CURSO = worksheet.Cells[row, col["CURSO"]].Text;

						current = CARGARDATOS.NOMBRE_MATERIA;
						model.NOMBRE_MATERIA = worksheet.Cells[row, col["NOMBRE_DE_MATERIA"]].Text;

						current = CARGARDATOS.PERIODO;
						model.PERIODO = worksheet.Cells[row, col["PERIODO"]].Text;

						current = CARGARDATOS.PARTE_PERIODO;
						model.PARTE_PERIODO = worksheet.Cells[row, col["PARTE_DEL_PERIODO"]].Text;

						current = CARGARDATOS.FECHA_INICIO;
						model.FECHA_INICIO = model.__validDateTime(worksheet.Cells[row, col["FECHA_DE_INICIO"]].Text);

						current = CARGARDATOS.FECHA_FIN;
						model.FECHA_FIN = model.__validDateTime(worksheet.Cells[row, col["FECHA_FIN"]].Text);

						current = CARGARDATOS.NUMERO_INSCRITOS;
						model.NUMERO_INSCRITOS = worksheet.Cells[row, col["INSCRITOS"]].Text;

						current = CARGARDATOS.USUARIO_IMPORTA;
						model.USUARIO_IMPORTA = sesion.pkUser.ToString();

						paso = 3;

						// 3.- Se validan
						model.validate();

						// 4.- Se guarda en la tabla temporal.
						model.Add_TMP();
						listModels.Add(model);

						// 5.- Se agregan los datos al datatable.
						tbl.Rows.Add(model.getArrayObject(dataValid));

						progressbar.progress();
					}
					DateTime dt_2 = DateTime.Now;
					Debug.WriteLine("span:" + (dt_2 - dt_1));
				} // the using statement calls Dispose() which closes the package.

				sesion.vdata.Remove("CargarDatosComplementariosError");
				sesion.saveSession();

				progressbar.complete();
				return true;
			}
			catch (Exception ex)
			{
				if (paso == 1)
					sesion.vdata["CargarDatosComplementariosError"] = "Error en archivo de Excel";
				else if (paso == 2)
					sesion.vdata["CargarDatosComplementariosError"] = "No se encuentra la columna '" + current + "'";
				else if (paso == 3)
					sesion.vdata["CargarDatosComplementariosError"] = "Error validando Excel";
				sesion.saveSession();

				progressbar.complete();
				return false;
			}
		}

		public void AuditarExcel(string fileName, System.Data.DataTable table, List<CargarDatosComplementariosModel> listModels)
		{
			try
			{
				using (ExcelPackage pck = new ExcelPackage())
				{
					//Create the worksheet
					ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Datos");

					//Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
					ws.Cells["A1"].LoadFromDataTable(table, true);
					ws.Cells["A1:S1"].AutoFitColumns();

					Color errorColor = Color.FromArgb(239, 219, 67);
					Color noSavedColor = Color.FromArgb(220, 10, 0);

					//for (int i = 0; i < listModels.Count; i++)
					//{
					//	CargarDatosComplementariosModel model = listModels[i];
					//	int row = 2 + i;

					//	if (model.validCells[CARGARDATOS.IDBANNER] == false)
					//		using (ExcelRange rng = ws.Cells["A" + row + ":A" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;    //Set Pattern for the background to Solid
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.DOCENTE] == false)
					//		using (ExcelRange rng = ws.Cells["B" + row + ":B" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.RFC] == false)
					//		using (ExcelRange rng = ws.Cells["C" + row + ":C" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.CURP] == false)
					//		using (ExcelRange rng = ws.Cells["D" + row + ":D" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.MAXIMO_GRADO_ACADEMICO] == false)
					//		using (ExcelRange rng = ws.Cells["E" + row + ":E" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.TIPO_DOCENTE] == false)
					//		using (ExcelRange rng = ws.Cells["F" + row + ":F" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.NRC] == false)
					//		using (ExcelRange rng = ws.Cells["G" + row + ":G" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.HORAS_PROGRAMADAS] == false)
					//		using (ExcelRange rng = ws.Cells["H" + row + ":H" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.TABULADOR_DI] == false)
					//		using (ExcelRange rng = ws.Cells["I" + row + ":I" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.ESCUELA] == false)
					//		using (ExcelRange rng = ws.Cells["J" + row + ":J" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.CAMPUS] == false)
					//		using (ExcelRange rng = ws.Cells["K" + row + ":K" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.MATERIA] == false)
					//		using (ExcelRange rng = ws.Cells["L" + row + ":L" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}

					//	if (model.validCells[CARGARDATOS.CURSO] == false)//NUEVO
					//		using (ExcelRange rng = ws.Cells["M" + row + ":M" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}


					//	if (model.validCells[CARGARDATOS.NOMBRE_MATERIA] == false)
					//		using (ExcelRange rng = ws.Cells["N" + row + ":N" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.PERIODO] == false)
					//		using (ExcelRange rng = ws.Cells["O" + row + ":O" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.PARTE_PERIODO] == false)
					//		using (ExcelRange rng = ws.Cells["P" + row + ":P" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.FECHA_INICIO] == false)
					//		using (ExcelRange rng = ws.Cells["Q" + row + ":Q" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.FECHA_FIN] == false)
					//		using (ExcelRange rng = ws.Cells["R" + row + ":R" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//	if (model.validCells[CARGARDATOS.NUMERO_INSCRITOS] == false)
					//		using (ExcelRange rng = ws.Cells["S" + row + ":S" + row])
					//		{
					//			rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
					//			rng.Style.Fill.BackgroundColor.SetColor(errorColor);
					//		}
					//}

					//Write it back to the client
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
					Response.BinaryWrite(pck.GetAsByteArray());
				}
			}
			catch (Exception) { }
		}
		[HttpPost]
		public ActionResult FindFirstError()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content(""); }
			CargarDatosComplementariosModel model = new CargarDatosComplementariosModel();
			model.sesion = sesion;

			if (sesion.vdata.ContainsKey("CargarDatosComplementariosError"))
				return Json(new { msg = Notification.Error(sesion.vdata["CargarDatosComplementariosError"]) });

			int total;
			int maxErrors;
			string idsError;
			model.FindFirstError(out total, out maxErrors, out idsError);

			if (total == 0)
				return Json(new { msg = Notification.Warning("No se han encontrado registros auditados") });
			else if (maxErrors == 0)
				return Json(new { msg = Notification.Succes("Archivo auditado correctamente") });
			else
				return Json(new { msg = Notification.Error("" + maxErrors + " error(es) ( IDSIU:  " + idsError + ")") });
		}

		public string importar(string ids, string sede)
		{
			CargarDatosComplementariosModel model = new CargarDatosComplementariosModel();

			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
			model.sesion = sesion;

			if (model.Importar(ids))
			{
				Log.write(this, "importar", LOG.EDICION, "ids:" + ids, model.sesion);
				return Notification.Succes("Los datos se han actualizado satisfactoriamente.");
			}
			else
				return Notification.Error("No se ha podido hacer la importación");
		}

	}
}