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
using System.IO;
using System.Web;
using System.Web.Mvc;
using static PagoProfesores.Models.Pagos.NominaExcelModel;

namespace PagoProfesores.Controllers.Pagos
{
    public class NominaExcelController : Controller
	{
		private database db;
		private List<Factory.Privileges> Privileges;
		private SessionDB sesion;

		public NominaExcelController()
		{
			db = new database();

			Scripts.SCRIPTS = new string[] { "js/Pagos/NominaExcel/NominaExcel.js" };

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10109,  Element = "Controller" }, //PERMISO ACCESO EstadodeCuentaWeb
                 new Factory.Privileges { Permiso = 10110,  Element = "formbtnAuditar" },//PERMISO DETALLE   
                 new Factory.Privileges { Permiso = 10111,  Element = "formbtnGenerar" },//PERMISO DETALLE   
                 new Factory.Privileges { Permiso = 10112,  Element = "formbtnGenerarEdoCta" },//PERMISO DETALLE   
            };
        }

		// GET: NominaExcel
		public ActionResult Start()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content(""); }

			NominaExcelModel model = new NominaExcelModel();
			model.sesion = sesion;
			model.clean();

			Main view = new Main();
			ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu(7, 10, sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "ID_NOMINA", "ASC", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return View(Factory.View.NotAccess);

			Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla NominaExcel", sesion);

			return View(Factory.View.Access + "Pagos/NominaExcel/Start.cshtml");
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

			DataTable table = new DataTable();

            table.TABLE = "V_NOMINA_TMP";
            table.COLUMNAS = new string[] { "IDSIU","Sede", "Periodo", "PartePeriodo", "C. Costos", "T. Docente", "Cursos", "Horas a pagar", "Opc. Pago", "Tipo Pago", "Tabulador", "Esquema Pago", "Monto a pagar","Cve. Escuela","Campus"  };
			table.CAMPOS = new string[] { "IDSIU", "CVE_SEDE", "PERIODO", "PARTEDELPERIODO", "ID_CENTRODECOSTOS", "TIPODEDOCENTE", "NOCURSOS", "HORASAPAGAR", "OPCIONDEPAGO", "CVE_TIPODEPAGO", "TABULADOR", "ID_ESQUEMA", "MONTOAPAGAR", "ERRORES", "ID_NOMINA", "CVE_ESCUELA","CAMPUS_INB"};
			table.CAMPOSSEARCH = new string[] { "IDSIU", "CVE_SEDE", "PERIODO", "ID_CENTRODECOSTOS" };
			table.CAMPOSHIDDEN = new string[] { "ID_NOMINA", "ERRORES" };
			table.TABLECONDICIONSQL = "USUARIO=" + sesion.pkUser;
            
            table.dictColumnFormat.Add("CVE_SEDE", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.CVE_SEDE] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

			table.dictColumnFormat.Add("PERIODO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.PERIODO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});
            
            table.dictColumnFormat.Add("PARTEDELPERIODO", delegate (string data, ResultSet res)
            {
            	return (res.Get("ERRORES")[(int)NOMINA.PARTEPERIODO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
            });
            
            table.dictColumnFormat.Add("ID_CENTRODECOSTOS", delegate (string data, ResultSet res)
			{
                return (res.Get("ERRORES")[(int)NOMINA.ID_CENTRODECOSTOS] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
            });

            table.dictColumnFormat.Add("IDSIU", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.IDSIU] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

            table.dictColumnFormat.Add("TIPODEDOCENTE", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.TIPODEDOCENTE] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

			table.dictColumnFormat.Add("NOCURSOS", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.NOCURSOS] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

			table.dictColumnFormat.Add("HORASAPAGAR", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.HORASAPAGAR] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

			table.dictColumnFormat.Add("OPCIONDEPAGO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.OPCIONDEPAGO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

			table.dictColumnFormat.Add("CVE_TIPODEPAGO", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.CVE_TIPODEPAGO] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

			table.dictColumnFormat.Add("TABULADOR", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.TABULADOR] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

			table.dictColumnFormat.Add("ID_ESQUEMA", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.ID_ESQUEMA] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

			table.dictColumnFormat.Add("MONTOAPAGAR", delegate (string data, ResultSet res)
			{
				return (res.Get("ERRORES")[(int)NOMINA.MONTOAPAGAR] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
			});

            table.dictColumnFormat.Add("CVE_ESCUELA", delegate (string data, ResultSet res)
            {
                return (res.Get("ERRORES")[(int)NOMINA.CVE_ESCUELA] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
            });

            table.dictColumnFormat.Add("CAMPUS_INB", delegate (string data, ResultSet res)
            {
                return (res.Get("ERRORES")[(int)NOMINA.CAMPUS_INB] == '1') ? data : "<div style=\"color:White; background-color:DarkRed; padding: 0px 2px;\">" + data + "</div>";
            });

            table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "ID_NOMINA";

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
                
				var path = Path.Combine(Server.MapPath("~/Upload/Nomina"), newFileName);
				file.SaveAs(path);

				if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return; }

				System.Data.DataTable table = new System.Data.DataTable();
				List<NominaExcelModel> listModels = new List<NominaExcelModel>();

				// Proceso de carga ...
				ValidarExcel(path, sesion, table, listModels);

				AuditarExcel(originalName, table, listModels);
			}
		}

		public bool ValidarExcel(string fileName, SessionDB sesion, System.Data.DataTable tbl, List<NominaExcelModel> listModels)
		{
			ProgressBarCalc progressbar = new ProgressBarCalc(sesion, "NominaExcel");
			progressbar.prepare();

			NominaExcelModel auxModel = new NominaExcelModel();
			auxModel.sesion = sesion;
			auxModel.clean();
			auxModel.cargaListas();

			tbl.Columns.Add("CVE_SEDE", typeof(string));
            tbl.Columns.Add("PARTEDELPERIODO", typeof(string));

            tbl.Columns.Add("PERIODO", typeof(string));
			tbl.Columns.Add("ID_CENTRODECOSTOS", typeof(string));
			tbl.Columns.Add("IDSIU", typeof(string));
			tbl.Columns.Add("ID_PERSONA", typeof(string));

			tbl.Columns.Add("TIPODEDOCENTE", typeof(string));
			tbl.Columns.Add("NOCURSOS", typeof(string));
			tbl.Columns.Add("HORASAPAGAR", typeof(string));

			tbl.Columns.Add("OPCIONDEPAGO", typeof(string));
			tbl.Columns.Add("CVE_TIPODEPAGO", typeof(string));
			tbl.Columns.Add("TABULADOR", typeof(string));
			tbl.Columns.Add("ID_ESQUEMA", typeof(string));
			tbl.Columns.Add("MONTOAPAGAR", typeof(string));

            tbl.Columns.Add("CVE_ESCUELA", typeof(string));
            tbl.Columns.Add("CAMPUS_INB", typeof(string));
            tbl.Columns.Add("MATERIA", typeof(string));
            tbl.Columns.Add("CURSO", typeof(string));
            tbl.Columns.Add("NOMBREMATERIA", typeof(string));

            int paso = 1;
			NOMINA current = NOMINA.CVE_SEDE;

			// Cargar el excel en los modelos.
			try
			{
				using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(fileName)))
				{
					paso = 1;
					// 1.- Get the first worksheet in the workbook
					ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

					Dictionary<string, int> col = new Dictionary<string, int>();
					Dictionary<NOMINA, object> dataValid = new Dictionary<NOMINA, object>();

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

						NominaExcelModel model = new NominaExcelModel();
						model.sesion = sesion;
						paso = 2;
						// 2.- Se asignan los valores al modelo
						current = NOMINA.CVE_SEDE;
						model.CVE_SEDE = worksheet.Cells[row, col["CVE_SEDE"]].Text;

                        current = NOMINA.PARTEPERIODO;
                        model.PARTEPERIODO = worksheet.Cells[row, col["PARTEDELPERIODO"]].Text;//

                        current = NOMINA.PERIODO;
						model.PERIODO = worksheet.Cells[row, col["PERIODO"]].Text;

						current = NOMINA.ID_CENTRODECOSTOS;
                        model.ID_CENTRODECOSTOS = worksheet.Cells[row, col["ID_CENTRODECOSTOS"]].Text;
                        
                        current = NOMINA.IDSIU;
						model.IDSIU = worksheet.Cells[row, col["IDSIU"]].Text;

						current = NOMINA.ID_PERSONA;
						long.TryParse(worksheet.Cells[row, col["ID_PERSONA"]].Text, out model.ID_PERSONA);

						current = NOMINA.TIPODEDOCENTE;
						model.TIPODEDOCENTE = worksheet.Cells[row, col["TIPODEDOCENTE"]].Text;

						current = NOMINA.NOCURSOS;
						int.TryParse(worksheet.Cells[row, col["NOCURSOS"]].Text, out model.NOCURSOS);

						current = NOMINA.HORASAPAGAR;
						double.TryParse(worksheet.Cells[row, col["HORASAPAGAR"]].Text, out model.HORASAPAGAR);

						current = NOMINA.OPCIONDEPAGO;
						model.OPCIONDEPAGO = worksheet.Cells[row, col["OPCIONDEPAGO"]].Text;

						current = NOMINA.CVE_TIPODEPAGO;
						model.CVE_TIPODEPAGO = worksheet.Cells[row, col["CVE_TIPODEPAGO"]].Text;

						current = NOMINA.TABULADOR;
						model.TABULADOR = worksheet.Cells[row, col["TABULADOR"]].Text;

						current = NOMINA.ID_ESQUEMA;
                        model.ID_ESQUEMA = worksheet.Cells[row, col["ID_ESQUEMA"]].Text;

                        current = NOMINA.MONTOAPAGAR;
						double.TryParse(worksheet.Cells[row, col["MONTOAPAGAR"]].Text, out model.MONTOAPAGAR);

                        current = NOMINA.CVE_ESCUELA;
                        model.CVE_ESCUELA = worksheet.Cells[row, col["CVE_ESCUELA"]].Text;

                        current = NOMINA.CAMPUS_INB;
                        model.CAMPUS_INB = worksheet.Cells[row, col["CAMPUS_INB"]].Text;

                        current = NOMINA.MATERIA;
                        model.MATERIA = worksheet.Cells[row, col["MATERIA"]].Text;

                        current = NOMINA.CURSO;
                        model.CURSO = worksheet.Cells[row, col["CURSO"]].Text;

                        current = NOMINA.NOMBREMATERIA;
                        model.NOMBREMATERIA = worksheet.Cells[row, col["NOMBREMATERIA"]].Text;


                        //carga esquema
                        auxModel.cargarEsquema(model.CVE_SEDE, model.PERIODO,model.ID_ESQUEMA);

                        paso = 3;
						// 3.- Se validan
						model.copiaListasDesde(auxModel);
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

				sesion.vdata.Remove("NominaExcelError");
				sesion.saveSession();

				progressbar.complete();
				return true;
			}
			catch (Exception)
			{
				if (paso == 1)
					sesion.vdata["NominaExcelError"] = "Error en archivo de Excel";
				else if (paso == 2)
					sesion.vdata["NominaExcelError"] = "No se encuentra la columna '" + current + "'";
				else if (paso == 3)
					sesion.vdata["NominaExcelError"] = "Error validando Excel";
				sesion.saveSession();

				progressbar.complete();
				return false;
			}
		}

		public void AuditarExcel(string fileName, System.Data.DataTable table, List<NominaExcelModel> listModels)
		{
			try
			{
				using (ExcelPackage pck = new ExcelPackage())
				{
					//Create the worksheet
					ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Nomina");

					//Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
					ws.Cells["A1"].LoadFromDataTable(table, true);
					ws.Cells["A1:L1"].AutoFitColumns();

					Color errorColor = Color.FromArgb(239, 219, 67);
					Color noSavedColor = Color.FromArgb(220, 10, 0);

					for (int i = 0; i < listModels.Count; i++)
					{
						NominaExcelModel model = listModels[i];
						int row = 2 + i;

						if (model.validCells[NOMINA.CVE_SEDE] == false)
							using (ExcelRange rng = ws.Cells["A" + row + ":A" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;    //Set Pattern for the background to Solid
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
                        if (model.validCells[NOMINA.PARTEPERIODO] == false)
                            using (ExcelRange rng = ws.Cells["B" + row + ":B" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[NOMINA.PERIODO] == false)
							using (ExcelRange rng = ws.Cells["C" + row + ":C" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[NOMINA.ID_CENTRODECOSTOS] == false)
							using (ExcelRange rng = ws.Cells["D" + row + ":D" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[NOMINA.IDSIU] == false)
							using (ExcelRange rng = ws.Cells["E" + row + ":F" + row]) // ID_PERSONA TOO
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;    //Set Pattern for the background to Solid
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);  //Set color to dark blue
							}
						if (model.validCells[NOMINA.TIPODEDOCENTE] == false)
							using (ExcelRange rng = ws.Cells["G" + row + ":G" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[NOMINA.NOCURSOS] == false)
							using (ExcelRange rng = ws.Cells["H" + row + ":H" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[NOMINA.HORASAPAGAR] == false)
							using (ExcelRange rng = ws.Cells["I" + row + ":I" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[NOMINA.OPCIONDEPAGO] == false)
							using (ExcelRange rng = ws.Cells["J" + row + ":J" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[NOMINA.CVE_TIPODEPAGO] == false)
							using (ExcelRange rng = ws.Cells["K" + row + ":K" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}
						if (model.validCells[NOMINA.TABULADOR] == false)
							using (ExcelRange rng = ws.Cells["L" + row + ":L" + row])
							{
								rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
								rng.Style.Fill.BackgroundColor.SetColor(errorColor);
							}

                        if (model.validCells[NOMINA.ID_ESQUEMA] == false)//NUEVO
                            using (ExcelRange rng = ws.Cells["M" + row + ":M" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }


                        if (model.validCells[NOMINA.CVE_ESCUELA] == false)
                            using (ExcelRange rng = ws.Cells["O" + row + ":O" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[NOMINA.CAMPUS_INB] == false)
                            using (ExcelRange rng = ws.Cells["P" + row + ":P" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[NOMINA.MATERIA] == false)
                            using (ExcelRange rng = ws.Cells["Q" + row + ":Q" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[NOMINA.CURSO] == false)
                            using (ExcelRange rng = ws.Cells["R" + row + ":R" + row])
                            {
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rng.Style.Fill.BackgroundColor.SetColor(errorColor);
                            }
                        if (model.validCells[NOMINA.NOMBREMATERIA] == false)
                            using (ExcelRange rng = ws.Cells["S" + row + ":S" + row])
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
			NominaExcelModel model = new NominaExcelModel();
			model.sesion = sesion;

			if (sesion.vdata.ContainsKey("NominaExcelError"))
				return Json(new { msg = Notification.Error(sesion.vdata["NominaExcelError"]) });
			
			int total;
			int maxErrors;
			string idsError;
			model.FindFirstError(out total, out maxErrors, out idsError);

			if(total==0)
				return Json(new { msg = Notification.Warning("No se han encontrado registros auditados") });
			else if (maxErrors == 0)
				return Json(new { msg = Notification.Succes("Archivo auditado correctamente") });
			else
				return Json(new { msg = Notification.Error("" + maxErrors + " error(es) ( IDSIU:  " + idsError + ")") });
		}

		[HttpPost]
		public ActionResult generar()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content(""); }
			NominaExcelModel model = new NominaExcelModel();
			model.sesion = sesion;

			int total, errores;
			if (model.generar(out total, out errores) == false)
                return Json(new { msg = Notification.Error("Error al momento de importar, " + errores + " / " + total + " registros no importados.") });
            else
            {
                if (errores == 0)
                    return Json(new { msg = Notification.Succes("Nomina importada correctamente") });
                else
                    return Json(new { msg = Notification.Error("Error al momento de importar, " + errores + " / " + total + " registros no importados.") });
            }
		}

        [HttpPost]
        public ActionResult generarEdoCta()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content(""); }
            NominaExcelModel model = new NominaExcelModel();
            model.sesion = sesion;

            if (model.generarEdoCta())
                return Json(new { msg = Notification.Succes("'Estado de cuenta' generado correctamente.") });
            else
                return Json(new { msg = Notification.Error("Error al momento de generar el 'estado de cuenta'.") });
        }
    }
}