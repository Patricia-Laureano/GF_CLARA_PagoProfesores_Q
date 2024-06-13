using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Controllers.Herramientas;
using PagoProfesores.Models.Personas;
using Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using static PagoProfesores.Models.Personas.ImportarDatosExcelModel;

namespace PagoProfesores.Controllers.Personas
{
    public class ImportarDatosExcelController : Controller
    {
        private SessionDB sesion;
        private database db;
        private List<Factory.Privileges> Privileges;

        public int sinCP { get; set; }
        public string IDSIUSCP;

        public ImportarDatosExcelController()
        {
            db = new database();
            Scripts.SCRIPTS = new string[] { "js/Personas/ImportarDatosExcel.js", "plugins/autocomplete/js/jquery.easy_autocomplete.js" };

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10087,  Element = "Controller" }, //PERMISO ACCESO DatosPersonas           
                 new Factory.Privileges { Permiso = 10088,  Element = "formbtnConsultar" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10281,  Element = "formbtnImportar" }, //PERMISO EDITAR
            };
        }

		// GET: NominaExcel
		public ActionResult Start()
		{
			ImportarDatosSIUModel model = new ImportarDatosSIUModel();
			SessionDB sesion = SessionDB.start(Request, Response, false, model.db);
			if ((model.sesion = sesion) == null)
				return Content("");
			model.Clean();
			try
			{
				Main view = new Main();
				ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
				ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
				ViewBag.Main = view.createMenu("Personas", "Importar Datos Excel", sesion);

				ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

				//Intercom
				ViewBag.User = sesion.nickName.ToString();
				ViewBag.Email = sesion.nickName.ToString();
				ViewBag.FechaReg = DateTime.Today;

				sesion.vdata["TABLE_PERSONAS"] = "QPersonasTMP";
				sesion.saveSession();

				ViewBag.BlockingPanel_1 = Main.createBlockingPanel("blocking-panel-1");
				ViewBag.BlockingPanel_2 = Main.createBlockingPanel("blocking-panel-2", false, "");
				ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion);


				if (!sesion.permisos.havePermission(Privileges[0].Permiso))
					return View(Factory.View.NotAccess);

				Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Importar Datos SIU' ", sesion);

				return View();
			}
			catch (Exception e)
			{
				ViewBag.Notification = Notification.Error(e.Message);
				Log.write(this, "Start", LOG.ERROR, "Ingresa a pantalla 'Importar Datos EXcel' " + e.Message, sesion);

				return View();
			}
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show, int pg, string search, string orderby, string sort, SessionDB sesion)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

			Factory.DataTable table = new Factory.DataTable();
			string CheckIcon = "<i class=\"fa fa-check\"></i>";

			table.TABLE = sesion.vdata["TABLE_PERSONAS"];
			table.COLUMNAS = new string[] { "Reg", "ID", "Nombre", "Apellidos", "RFC", "CURP",
											"Pais", "Estado", "Ciudad", "Del/Mun", "Colonia", "Calle",
											"CP", "Escuela sede", "Cve. Título", "Título profesional",
											"Licenciatura", "Maestría", "Cve. Profesión",
											"Profesión", "Cédula profesional", "Fec. Cédula", "NSS"};
			table.CAMPOS = new string[] { "REGISTRADO", "IDSIU", "NOMBRES", "APELLIDOS", "RFC", "CURP",
										  "DIRECCION_PAIS", "DIRECCION_ESTADO", "DIRECCION_CIUDAD", "DIRECCION_ENTIDAD",
										  "DIRECCION_COLONIA", "DIRECCION_CALLE", "DIRECCION_CP", "AREAASIGNACION", "CVE_TITULOPROFESIONAL",
										  "TITULOPROFESIONAL", "TITULO_LICENCIATURA", "TITULO_MAESTRIA", "CVE_PROFESION", "PROFESION", "CEDULAPROFESIONAL",
										  "FECHACEDULA", "SEGUROSOCIAL"};
			table.CAMPOSSEARCH = new string[] { "IDSIU", "NOMBRES", "APELLIDOS" };
			table.dictColumnFormat["REGISTRADO"] = delegate (string str, ResultSet res) { return str == "True" ? CheckIcon : ""; };

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "IDSIU";
			table.TABLECONDICIONSQL = "USUARIO = '" + sesion.pkUser + "'";

			table.enabledCheckbox = true;
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

				var path = Path.Combine(Server.MapPath("~/Upload/ImportarDatosExcel"), newFileName);
				file.SaveAs(path);

				if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return; }

				System.Data.DataTable table = new System.Data.DataTable();
				List<ImportarDatosExcelModel> listModels = new List<ImportarDatosExcelModel>();

				// Proceso de carga ...
				ValidarExcel(path, sesion, table, listModels);

				AuditarExcel(originalName, table, listModels);
			}
		}

		public bool ValidarExcel(string fileName, SessionDB sesion, System.Data.DataTable tbl, List<ImportarDatosExcelModel> listModels)
		{
			ProgressBarCalc progressbar = new ProgressBarCalc(sesion, "ImportarDatosExcel");
			progressbar.prepare();

			ImportarDatosExcelModel auxModel = new ImportarDatosExcelModel();
			auxModel.sesion = sesion;
			auxModel.clean();

			tbl.Columns.Add("IDSIU", typeof(string));
			tbl.Columns.Add("CVESEDE", typeof(string));
			tbl.Columns.Add("NOMBRES", typeof(string));
			tbl.Columns.Add("APELLIDOS", typeof(string));
			tbl.Columns.Add("SEXO", typeof(string));
			tbl.Columns.Add("NACIONALIDAD", typeof(string));
			tbl.Columns.Add("CORREO", typeof(string));
			tbl.Columns.Add("TELEFONO", typeof(string));
			tbl.Columns.Add("RFC", typeof(string));
			tbl.Columns.Add("CURP", typeof(string));
			tbl.Columns.Add("DIRECCION_PAIS", typeof(string));
			tbl.Columns.Add("DIRECCION_ESTADO", typeof(string));
			tbl.Columns.Add("DIRECCION_CIUDAD", typeof(string));
			tbl.Columns.Add("DIRECCION_ENTIDAD", typeof(string));
			tbl.Columns.Add("DIRECCION_COLONIA", typeof(string));
			tbl.Columns.Add("DIRECCION_CALLE", typeof(string));
			tbl.Columns.Add("DIRECCION_CP", typeof(string));
			tbl.Columns.Add("CVE_TITULOPROFESIONAL", typeof(string));
			tbl.Columns.Add("TITULOPROFESIONAL", typeof(string));
			tbl.Columns.Add("CVE_PROFESION", typeof(string));
			tbl.Columns.Add("PROFESION", typeof(string));
			tbl.Columns.Add("CEDULAPROFESIONAL", typeof(string));
			tbl.Columns.Add("SEGUROSOCIAL", typeof(string));
			tbl.Columns.Add("AREAASIGNACION", typeof(string));
			tbl.Columns.Add("CORREO365", typeof(string));
			tbl.Columns.Add("TITULO_LICENCIATURA", typeof(string));
			tbl.Columns.Add("TITULO_MAESTRIA", typeof(string));	
			tbl.Columns.Add("TITULO_DOCTORADO", typeof(string));
			tbl.Columns.Add("FI_DIRECCION_PAIS", typeof(string));
			tbl.Columns.Add("FI_DIRECCION_ESTADO", typeof(string));
			tbl.Columns.Add("FI_DIRECCION_CIUDAD", typeof(string));
			tbl.Columns.Add("FI_DIRECCION_ENTIDAD", typeof(string));
			tbl.Columns.Add("FI_DIRECCION_COLONIA", typeof(string));
			tbl.Columns.Add("FI_DIRECCION_CALLE", typeof(string));
			tbl.Columns.Add("FI_DIRECCION_CP", typeof(string));
			tbl.Columns.Add("FECHACEDULA", typeof(string));
			tbl.Columns.Add("FECHANACIMIENTO", typeof(string));
			tbl.Columns.Add("CVE_TIPOPAGO", typeof(string));
			tbl.Columns.Add("FECHA_R", typeof(string));
			tbl.Columns.Add("USUARIO", typeof(string));
			tbl.Columns.Add("ID_PERSONA", typeof(string));
			tbl.Columns.Add("REGISTRADO", typeof(string));
			tbl.Columns.Add("IMPORTADO", typeof(string));
			tbl.Columns.Add("CVE_ORIGEN", typeof(string));
			
			int paso = 1;
			CARGARDATOSEXCEL current = CARGARDATOSEXCEL.IDSIU;

			// Cargar el excel en los modelos.
			try
			{
				using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(fileName)))
				{
					paso = 1;
					// 1.- Get the first worksheet in the workbook
					ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

					Dictionary<string, int> col = new Dictionary<string, int>();
					Dictionary<CARGARDATOSEXCEL, object> dataValid = new Dictionary<CARGARDATOSEXCEL, object>();

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

						ImportarDatosExcelModel model = new ImportarDatosExcelModel();
						model.sesion = sesion;
						paso = 2;
						// 2.- Se asignan los valores al modelo
						current = CARGARDATOSEXCEL.IDSIU;
						model.IDSIU = worksheet.Cells[row, col["IDSIU"]].Text;
						sesion.vdata["IDSIU"] = worksheet.Cells[row, col["IDSIU"]].Text;

						current = CARGARDATOSEXCEL.CVESEDE;
						model.CVESEDE = worksheet.Cells[row, col["CVESEDE"]].Text;

						sesion.vdata["CVESEDE"]= worksheet.Cells[row, col["CVESEDE"]].Text;

						current = CARGARDATOSEXCEL.NOMBRES;
						model.NOMBRES = worksheet.Cells[row, col["NOMBRES"]].Text;

						current = CARGARDATOSEXCEL.APELLIDOS;
						model.APELLIDOS = worksheet.Cells[row, col["APELLIDOS"]].Text;

						current = CARGARDATOSEXCEL.SEXO;
						model.SEXO = worksheet.Cells[row, col["SEXO"]].Text;

						current = CARGARDATOSEXCEL.NACIONALIDAD;
						model.NACIONALIDAD = worksheet.Cells[row, col["NACIONALIDAD"]].Text;

						current = CARGARDATOSEXCEL.CORREO;
						model.CORREO = worksheet.Cells[row, col["CORREO"]].Text;

						current = CARGARDATOSEXCEL.TELEFONO;
						model.TELEFONO = worksheet.Cells[row, col["TELEFONO"]].Text;

						current = CARGARDATOSEXCEL.RFC;
						model.RFC = worksheet.Cells[row, col["RFC"]].Text;

						current = CARGARDATOSEXCEL.CURP;
						model.CURP = worksheet.Cells[row, col["CURP"]].Text;

						current = CARGARDATOSEXCEL.DIRECCION_PAIS;
						model.DIRECCION_PAIS = worksheet.Cells[row, col["DIRECCION_PAIS"]].Text;

						current = CARGARDATOSEXCEL.DIRECCION_ESTADO;
						model.DIRECCION_ESTADO = worksheet.Cells[row, col["DIRECCION_ESTADO"]].Text;

						current = CARGARDATOSEXCEL.DIRECCION_CIUDAD;
						model.DIRECCION_CIUDAD = worksheet.Cells[row, col["DIRECCION_CIUDAD"]].Text;

						current = CARGARDATOSEXCEL.DIRECCION_ENTIDAD;
						model.DIRECCION_ENTIDAD = worksheet.Cells[row, col["DIRECCION_ENTIDAD"]].Text;

						current = CARGARDATOSEXCEL.DIRECCION_COLONIA;
						model.DIRECCION_COLONIA = worksheet.Cells[row, col["DIRECCION_COLONIA"]].Text;

						current = CARGARDATOSEXCEL.DIRECCION_CALLE;
						model.DIRECCION_CALLE = worksheet.Cells[row, col["DIRECCION_CALLE"]].Text;

						current = CARGARDATOSEXCEL.DIRECCION_CP;
						model.DIRECCION_CP = worksheet.Cells[row, col["DIRECCION_CP"]].Text;

						current = CARGARDATOSEXCEL.CVE_TITULOPROFESIONAL;
						model.CVE_TITULOPROFESIONAL = worksheet.Cells[row, col["CVE_TITULOPROFESIONAL"]].Text;

						current = CARGARDATOSEXCEL.TITULOPROFESIONAL;
						model.TITULOPROFESIONAL = worksheet.Cells[row, col["TITULOPROFESIONAL"]].Text;

						current = CARGARDATOSEXCEL.CVE_PROFESION;
						model.CVE_PROFESION = worksheet.Cells[row, col["CVE_PROFESION"]].Text;

						current = CARGARDATOSEXCEL.PROFESION;
						model.PROFESION = worksheet.Cells[row, col["PROFESION"]].Text;

						current = CARGARDATOSEXCEL.CEDULAPROFESIONAL;
						model.CEDULAPROFESIONAL = worksheet.Cells[row, col["CEDULAPROFESIONAL"]].Text;

						current = CARGARDATOSEXCEL.SEGUROSOCIAL;
						model.SEGUROSOCIAL = worksheet.Cells[row, col["SEGUROSOCIAL"]].Text;

						current = CARGARDATOSEXCEL.AREAASIGNACION;
						model.AREAASIGNACION = worksheet.Cells[row, col["AREAASIGNACION"]].Text;

						current = CARGARDATOSEXCEL.CORREO365;
						model.CORREO365 = worksheet.Cells[row, col["CORREO365"]].Text;

						current = CARGARDATOSEXCEL.TITULO_LICENCIATURA;
						model.TITULO_LICENCIATURA = worksheet.Cells[row, col["TITULO_LICENCIATURA"]].Text;

						current = CARGARDATOSEXCEL.TITULO_MAESTRIA;
						model.TITULO_MAESTRIA = worksheet.Cells[row, col["TITULO_MAESTRIA"]].Text;

						current = CARGARDATOSEXCEL.TITULO_DOCTORADO;
						model.TITULO_DOCTORADO = worksheet.Cells[row, col["TITULO_DOCTORADO"]].Text;

						current = CARGARDATOSEXCEL.FI_DIRECCION_PAIS;
						model.FI_DIRECCION_PAIS = worksheet.Cells[row, col["FI_DIRECCION_PAIS"]].Text;

						current = CARGARDATOSEXCEL.FI_DIRECCION_ESTADO;
						model.FI_DIRECCION_ESTADO = worksheet.Cells[row, col["FI_DIRECCION_ESTADO"]].Text;

						current = CARGARDATOSEXCEL.FI_DIRECCION_CIUDAD;
						model.FI_DIRECCION_CIUDAD = worksheet.Cells[row, col["FI_DIRECCION_CIUDAD"]].Text;

						current = CARGARDATOSEXCEL.FI_DIRECCION_ENTIDAD;
						model.FI_DIRECCION_ENTIDAD = worksheet.Cells[row, col["FI_DIRECCION_ENTIDAD"]].Text;

						current = CARGARDATOSEXCEL.FI_DIRECCION_COLONIA;
						model.FI_DIRECCION_COLONIA = worksheet.Cells[row, col["FI_DIRECCION_COLONIA"]].Text;

						current = CARGARDATOSEXCEL.FI_DIRECCION_CALLE;
						model.FI_DIRECCION_CALLE = worksheet.Cells[row, col["FI_DIRECCION_CALLE"]].Text;

						current = CARGARDATOSEXCEL.FI_DIRECCION_CP;
						model.FI_DIRECCION_CP = worksheet.Cells[row, col["FI_DIRECCION_CP"]].Text;

						current = CARGARDATOSEXCEL.FECHACEDULA;
						model.FECHACEDULA = model.__validDateTime(worksheet.Cells[row, col["FECHACEDULA"]].Text);

						current = CARGARDATOSEXCEL.FECHANACIMIENTO;
						model.FECHANACIMIENTO = model.__validDateTime(worksheet.Cells[row, col["FECHANACIMIENTO"]].Text);

						current = CARGARDATOSEXCEL.CVE_TIPODEPAGO;
						model.CVE_TIPODEPAGO = worksheet.Cells[row, col["CVE_TIPODEPAGO"]].Text;

						paso = 3;

						// 3.- Se validan
						model.validate();
						model.exist_TMP();
						current = CARGARDATOSEXCEL.ID_PERSONA;
						model.ID_PERSONA = model.ID_PERSONA;
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

				sesion.vdata.Remove("ImportarDatosExcelError");
				sesion.saveSession();

				progressbar.complete();
				return true;
			}
			catch (Exception ex)
			{
				if (paso == 1)
					sesion.vdata["ImportarDatosExcelError"] = "Error en archivo de Excel";
				else if (paso == 2)
					sesion.vdata["ImportarDatosExcelError"] = "No se encuentra la columna '" + current + "'";
				else if (paso == 3)
					sesion.vdata["ImportarDatosExcelError"] = "Error validando Excel";
				sesion.saveSession();

				progressbar.complete();
				return false;
			}
		}

		public void AuditarExcel(string fileName, System.Data.DataTable table, List<ImportarDatosExcelModel> listModels)
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

					for (int i = 0; i < listModels.Count; i++)
					{
                        ImportarDatosExcelModel model = listModels[i];
                        int row = 2 + i;

                        if (model.validCells[CARGARDATOSEXCEL.IDSIU] == false)
                            using (ExcelRange rng = ws.Cells["A" + row + ":A" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;    //Set Pattern for the background to Solid
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
						if (model.validCells[CARGARDATOSEXCEL.CVESEDE] == false)
							using (ExcelRange rng = ws.Cells["B" + row + ":B" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;    //Set Pattern for the background to Solid
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.NOMBRES] == false)
                            using (ExcelRange rng = ws.Cells["C" + row + ":C" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.APELLIDOS] == false)
                            using (ExcelRange rng = ws.Cells["D" + row + ":D" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.SEXO] == false)
                            using (ExcelRange rng = ws.Cells["E" + row + ":E" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.NACIONALIDAD] == false)
                            using (ExcelRange rng = ws.Cells["F" + row + ":F" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.CORREO] == false)
                            using (ExcelRange rng = ws.Cells["G" + row + ":G" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.TELEFONO] == false)
                            using (ExcelRange rng = ws.Cells["H" + row + ":H" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.RFC] == false)
                            using (ExcelRange rng = ws.Cells["I" + row + ":I" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.CURP] == false)
                            using (ExcelRange rng = ws.Cells["J" + row + ":J" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.DIRECCION_PAIS] == false)
                            using (ExcelRange rng = ws.Cells["K" + row + ":K" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.DIRECCION_ESTADO] == false)
                            using (ExcelRange rng = ws.Cells["L" + row + ":L" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.DIRECCION_CIUDAD] == false)
                            using (ExcelRange rng = ws.Cells["M" + row + ":M" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }

                        if (model.validCells[CARGARDATOSEXCEL.DIRECCION_ENTIDAD] == false)//NUEVO
                            using (ExcelRange rng = ws.Cells["N" + row + ":N" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }

                        if (model.validCells[CARGARDATOSEXCEL.DIRECCION_COLONIA] == false)
                            using (ExcelRange rng = ws.Cells["O" + row + ":O" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.DIRECCION_CALLE] == false)
                            using (ExcelRange rng = ws.Cells["P" + row + ":P" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.DIRECCION_CP] == false)
                            using (ExcelRange rng = ws.Cells["Q" + row + ":Q" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.CVE_TITULOPROFESIONAL] == false)
                            using (ExcelRange rng = ws.Cells["R" + row + ":R" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.TITULOPROFESIONAL] == false)
                            using (ExcelRange rng = ws.Cells["S" + row + ":S" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[CARGARDATOSEXCEL.CVE_PROFESION] == false)
                            using (ExcelRange rng = ws.Cells["T" + row + ":T" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
						if (model.validCells[CARGARDATOSEXCEL.PROFESION] == false)
							using (ExcelRange rng = ws.Cells["U" + row + ":U" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.CEDULAPROFESIONAL] == false)
							using (ExcelRange rng = ws.Cells["V" + row + ":V" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.SEGUROSOCIAL] == false)
							using (ExcelRange rng = ws.Cells["W" + row + ":W" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.AREAASIGNACION] == false)
							using (ExcelRange rng = ws.Cells["X" + row + ":X" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.CORREO365] == false)
							using (ExcelRange rng = ws.Cells["Y" + row + ":Y" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.TITULO_LICENCIATURA] == false)
							using (ExcelRange rng = ws.Cells["Z" + row + ":Z" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.TITULO_MAESTRIA] == false)
							using (ExcelRange rng = ws.Cells["AA" + row + ":AA" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.TITULO_DOCTORADO] == false)
							using (ExcelRange rng = ws.Cells["AB" + row + ":AB" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.FI_DIRECCION_PAIS] == false)
							using (ExcelRange rng = ws.Cells["AC" + row + ":AC" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.FI_DIRECCION_ESTADO] == false)
							using (ExcelRange rng = ws.Cells["AD" + row + ":AD" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.FI_DIRECCION_CIUDAD] == false)
							using (ExcelRange rng = ws.Cells["AE" + row + ":AE" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.FI_DIRECCION_ENTIDAD] == false)
							using (ExcelRange rng = ws.Cells["AF" + row + ":AF" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.FI_DIRECCION_COLONIA] == false)
							using (ExcelRange rng = ws.Cells["AG" + row + ":AG" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.FI_DIRECCION_CALLE] == false)
							using (ExcelRange rng = ws.Cells["AH" + row + ":AH" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.FI_DIRECCION_CP] == false)
							using (ExcelRange rng = ws.Cells["AI" + row + ":AI" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.FECHACEDULA] == false)
							using (ExcelRange rng = ws.Cells["AJ" + row + ":AJ" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.FECHANACIMIENTO] == false)
							using (ExcelRange rng = ws.Cells["AK" + row + ":AK" +
								"" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[CARGARDATOSEXCEL.CVE_TIPODEPAGO] == false)
							using (ExcelRange rng = ws.Cells["AL" + row + ":AL" +
								"" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}

					}

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
			ImportarDatosExcelModel model = new ImportarDatosExcelModel();
			model.sesion = sesion;

			if (sesion.vdata.ContainsKey("ImportarDatosExcelError"))
				return Json(new { msg = Notification.Error(sesion.vdata["ImportarDatosExcelError"]) });

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
			ImportarDatosExcelModel model = new ImportarDatosExcelModel();

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