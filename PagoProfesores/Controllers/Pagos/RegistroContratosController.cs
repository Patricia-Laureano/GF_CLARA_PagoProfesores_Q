using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Pagos
{
	public class RegistroContratosController : Controller
	{
		private database db;
		private List<Factory.Privileges> Privileges;
		private SessionDB sesion;

		public RegistroContratosController()
		{
			db = new database();

			string[] scripts = { "js/Pagos/RegistroContratos/RegistroContratos.js" };
			Scripts.SCRIPTS = scripts;

			Privileges = new List<Factory.Privileges> {
				 new Factory.Privileges { Permiso = 10115,  Element = "Controller" }, //PERMISO ACCESO REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10116,  Element = "frm-registrocontratos" }, //PERMISO DETALLE REGISTRO DE CONTRATOS
                 new Factory.Privileges { Permiso = 10117,  Element = "formbtnsave" }, //PERMISO EDITAR REGISTRO DE CONTRATOS
            };
		}

		// GET: RegistroContratos
		public ActionResult Start()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

			Main view = new Main();
			ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
			ViewBag.Main = view.createMenu("Pagos", "Registro de contratos", sesion);
			ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
			ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion);

			ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return View(Factory.View.NotAccess);

			Log.write(this, "Registro de contratos Start", LOG.CONSULTA, "Ingresa Pantalla Registro de contratos", sesion);

			return View(Factory.View.Access + "Pagos/RegistroContratos/Start.cshtml");
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string filter = "",
			string Ciclo = "", string Periodo = "", string Nivel = "", string tipoPago = "", string IdPersona="")
		{
			if (sesion == null)
				if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }
            //sesion.vdata["Sede"] = filter;
            sesion.saveSession();


			DataTable table = new DataTable();

			table.TABLE = "VENTREGA_CONTRATOS";

			table.COLUMNAS =
				new string[] { "IDSIU", "Nombre", "Sede", "Esquema", "Fecha de entrega", "Monto total", "Periodo", "Nivel", "Esquema de pago", "Descripción de EP", "Contrato", "Fecha de inicio", "Fecha de fin", "No de semanas", "No de pagos",  "ID Persona" };//"Bloque contrato",
            table.CAMPOS =
				new string[] { "IDSIU", "NOMBRES", "CVE_SEDE", "ID_ESQUEMA", "FECHADECONTRATO", "MONTO", "PERIODO", "CVE_NIVEL", "ESQUEMA", "ESQUEMADEPAGODES", "CVE_CONTRATO", "FECHAINICIO", "FECHAFIN", "NOSEMANAS", "NUMPAGOS", "ID_PERSONA", "ID_CONTRATO" };//, "BLOQUEOCONTRATO"
            table.CAMPOSSEARCH =
				new string[] { "IDSIU", "NOMBRES", "CVE_SEDE", "ID_ESQUEMA", "FECHADECONTRATO", "MONTO", "PERIODO", "CVE_NIVEL", "ESQUEMA", "ESQUEMADEPAGODES", "CVE_CONTRATO", "FECHAINICIO", "FECHAFIN", "NOSEMANAS", "NUMPAGOS" ,"ID_PERSONA" };//, "BLOQUEOCONTRATO",

            string[] camposhidden = { "ID_CONTRATO" };


            table.addColumnClass("IDSIU", "datatable_fixedColumn");
			table.addColumnClass("NOMBRES", "datatable_fixedColumn");

			table.addColumnFormat("IDSIU", delegate (string value, ResultSet res) {
                return "<div style=\"width:100px;\">" + value + "</div>";
            });


            table.addColumnFormat("NOMBRES", delegate (string value, ResultSet res) {

            return "<div style=\"width:100px;\"><a /*style=\"color: #262c31;\"*/ onclick =\"formPage.verContrato('" + res.Get("CVE_CONTRATO") +
                 "','" + res.Get("CVE_SEDE") + "','" + res.Get("PERIODO") + "','" + res.Get("CVE_NIVEL") + "','" + res.Get("ID_ESQUEMA") + "','" + res.Get("IDSIU") + "');\">" + value + "<a/></div>";

            });



			if (IdPersona != "")
				table.addColumnFormat("FECHADECONTRATO", delegate (string value, ResultSet res)
				{
					return res.Get("ID_PERSONA") == IdPersona ? ("<div style=\"font-weight:bold;\">" + value + "</div>") : value;
				});
           
             

            table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "ID_CONTRATO";

            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";

            List<string> filtros = new List<string>();
		//	if (filter != "") filtros.Add("CVE_SEDE = '" + filter + "'");
			//if (Ciclo != "") filtros.Add("CVE_CICLO = '" + Ciclo + "'");
			if (Periodo != "") filtros.Add("PERIODO = '" + Periodo + "'");
			if (Nivel != "") filtros.Add("CVE_NIVEL = '" + Nivel + "'");
			if (tipoPago != "") filtros.Add("CVE_TIPODEPAGO = '" + tipoPago + "'");

            string union = "";
            if (filter != ""&&filtros.Count>0) {  union = " AND "; }

            table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());

            table.enabledButtonControls = false;

			table.addBtnActions("Editar", "editarRegistroContratos");

			return table.CreateDataTable(sesion);
		}

		// POST: RegistroContratos/Edit/5
		[HttpPost]
		public ActionResult Edit(RegistroContratosModel model)
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content("-1"); }
			if (model.Edit())
			{
				return Json(new JavaScriptSerializer().Serialize(model));
			}
			return View();
		}

		// POST: RegistroContratos/Save
		[HttpPost]
		public ActionResult Save(RegistroContratosModel model)
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content("-1"); }
			model.sesion = sesion;

			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });

			try
			{
				if (model.Save())
				{
					Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Succes("Fecha de entrega guardada con exito: " + model.FechaEntrega) });
				}
				else
				{
					Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Error(" Error al GUARDAR: " + model.FechaEntrega) });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });
			}
		}

		// POST: RegistroContratos/Delete/5
		[HttpPost]
		public ActionResult Delete(RegistroContratosModel model)
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content("-1"); }
			model.sesion = sesion;

			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });

			try
			{
				if (model.Delete())
				{
					Log.write(this, "Delete", LOG.BORRADO, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Succes("'Fecha de entrega' borrada con exito para el profesor " + model.Nombre) });
				}
				else
				{
					Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Error(" Error al eliminar la 'fecha de entrega' " + model.FechaEntrega + ", de " + model.Nombre) });
				}

			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });
			}
		}
		//#EXPORT EXCEL
		public void ExportExcel()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return; }

			try
			{
                System.Data.DataTable tbl = new System.Data.DataTable();
				tbl.Columns.Add("IDSIU", typeof(string));
				tbl.Columns.Add("Nombre del profesor", typeof(string));
				tbl.Columns.Add("Sede", typeof(string));
				tbl.Columns.Add("Esquema", typeof(string));
				tbl.Columns.Add("Fecha de entrega", typeof(string));
				tbl.Columns.Add("Monto total", typeof(string));
				tbl.Columns.Add("Periodo", typeof(string));
				//tbl.Columns.Add("Nivel", typeof(string));
				tbl.Columns.Add("Esquema de pago", typeof(string));
				tbl.Columns.Add("Descripción de EP", typeof(string));
				tbl.Columns.Add("Contrato", typeof(string));
				tbl.Columns.Add("Fecha de inicio", typeof(string));
				tbl.Columns.Add("Fecha de fin", typeof(string));
				tbl.Columns.Add("No de semanas", typeof(string));
				tbl.Columns.Add("No de pagos", typeof(string));
			//	tbl.Columns.Add("Bloque contrato", typeof(string));
				tbl.Columns.Add("ID Persona", typeof(string));

            //    string sede = Request.Params["sedes"];

                List<string> filtros = new List<string>();

                if (Request.Params["sedes"] != "" && Request.Params["sedes"] != null)
                    filtros.Add("CVE_SEDE = '" + Request.Params["sedes"] + "'");

               /* if (Request.Params["niveles"] != "" && Request.Params["niveles"] != null)
                    filtros.Add("CVE_NIVEL = '" + Request.Params["niveles"] + "'");*/

                if (Request.Params["periodos"] != "" && Request.Params["periodos"] != null)
                    filtros.Add("PERIODO = '" + Request.Params["periodos"] + "'");

                if (Request.Params["tipospago"] != "" && Request.Params["tipospago"] != null)
                    filtros.Add("CVE_TIPODEPAGO = '" + Request.Params["tipospago"] + "'");


                string conditions = string.Join<string>(" AND ", filtros.ToArray());

                string union = "";
                if (conditions.Length != 0)union = " WHERE ";


                ResultSet res = db.getTable("SELECT * FROM VENTREGA_CONTRATOS " + union + " " + conditions);


                while (res.Next())
				{
					// Here we add five DataRows.
					tbl.Rows.Add(res.Get("IDSIU"), res.Get("NOMBRES"), res.Get("CVE_SEDE"), res.Get("ID_ESQUEMA")
                        , res.Get("FECHADECONTRATO"), res.Get("MONTO"), res.Get("PERIODO")/*, res.Get("CVE_NIVEL")*/
                        , res.Get("ESQUEMA"), res.Get("ESQUEMADEPAGODES"), res.Get("CVE_CONTRATO"), res.Get("FECHAINICIO")
                        , res.Get("FECHAFIN"), res.Get("NOSEMANAS"), res.Get("NUMPAGOS")/*, res.Get("BLOQUEOCONTRATO")*/
                        , res.Get("ID_PERSONA"));
				}

				using (ExcelPackage pck = new ExcelPackage())
				{
					//Create the worksheet
					ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Registro de contratos");

					//Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
					ws.Cells["A1"].LoadFromDataTable(tbl, true);
					ws.Cells["A1:Q1"].AutoFitColumns();
					//ws.Column(1).Width = 20;
					//ws.Column(2).Width = 80;

					//Format the header for column 1-3
					using (ExcelRange rng = ws.Cells["A1:Q1"])
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
					Response.AddHeader("content-disposition", "attachment;  filename=RegistroContrato.xlsx");
					Response.BinaryWrite(pck.GetAsByteArray());
				}

				Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Registro de contratos", sesion);
			}
			catch (Exception e)
			{
				ViewBag.Notification = Notification.Error(e.Message);
				Log.write(this, "Start", LOG.ERROR, "Exporta Excel Registro de contratos" + e.Message, sesion);
			}
		}
	}
}
