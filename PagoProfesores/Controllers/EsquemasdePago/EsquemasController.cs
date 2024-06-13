using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.EsquemasdePago;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Diagnostics;

namespace PagoProfesores.Controllers.EsquemasdePago
{
    public class EsquemasController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        
        public EsquemasController()
        {
            db = new database();

            string[] scripts = { "js/EsquemasdePago/esquema.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10070,  Element = "Controller" }, //PERMISO ACCESO ESQUEMA
                 new Factory.Privileges { Permiso = 10071,  Element = "frm-esquemas" }, //PERMISO DETALLE ESQUEMA
                 new Factory.Privileges { Permiso = 10073,  Element = "formbtnadd" }, //PERMISO AGREGAR ESQUEMA
                 new Factory.Privileges { Permiso = 10074,  Element = "formbtnsave" }, //PERMISO EDITAR ESQUEMA
                 new Factory.Privileges { Permiso = 10075,  Element = "formbtndelete" }, //PERMISO ELIMINAR ESQUEMA

                //CALENDARIO DE PAGOS
                new Factory.Privileges { Permiso = 10076,  Element = "formbtnadd2" }, //PERMISO AGREGAR CALENDARIO
                new Factory.Privileges { Permiso = 10077,  Element = "formbtnsave2" }, //PERMISO EDITAR CALENDARIO
                new Factory.Privileges { Permiso = 10078,  Element = "formbtndelete2" }, //PERMISO ELIMINAR CALENDARIO
            };
        }

		// GET: Esquemas
		public ActionResult Start()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1") ; }

            Main view = new Main();
			ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
			ViewBag.Main = view.createMenu("Esquemas de Pago", "Esquemas", sesion);
			ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
			ViewBag.DataTable = CreateDataTable(10, 1, null, "FECHAINICIO", "ASC");
            ViewBag.DataTableCalendarPago = CreateDataTableCalendarP(10, 1, null, "ID_ESQUEMA", "ASC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return View(Factory.View.NotAccess);
            
			Log.write(this, "Esquemas de pago Start", LOG.CONSULTA, "Ingresa pantalla Esquema", sesion);

            ViewBag.sede = view.createLevels(sesion, "sedes");

			EsquemasModel model = new EsquemasModel();

			// comboTipoContarto     
			model.Obtener_TipoContrato();
			ViewBag.TIPO_CONTRATO = model.tipocontrato;
            
			//Combo CONCEPTO PAGO
			model.Obtener_ConceptoPago();
			ViewBag.CONCEPTO_PAGO = model.conceptoPago;
			//Combo TipoBloqueo
			ViewBag.TIPO_BLOQUEO = getTiposDeBloqueo(model);

			return View(Factory.View.Access + "EsquemasdePago/Start.cshtml");
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string PeriodoFilter = "", string filter = "")
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

			DataTable table = new DataTable();

			table.TABLE = "QEsquemasdePago01";
            
			table.COLUMNAS = new string[] { "ID", "Contrato", "Periodo", "Esquema", "Fecha Inicio", "Fecha Fin", "Semanas", "No. pagos", "Retención por contrato", "Sede" };
			table.CAMPOS = new string[] { "ID_ESQUEMA", "CONTRATO", "PERIODO", "ESQUEMADEPAGO", "FECHAINICIO", "FECHAFIN", "NOSEMANAS", "NOPAGOS", "BLOQUEOCONTRATO", "CVE_SEDE" };
			table.CAMPOSSEARCH = new string[] { "ID_ESQUEMA", "CONTRATO", "PERIODO", "ESQUEMADEPAGO", "FECHAINICIO", "FECHAFIN", "NOSEMANAS", "NOPAGOS", "BLOQUEOCONTRATO", "CVE_SEDE" };

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "ID_ESQUEMA";

			table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";

			if (PeriodoFilter.Equals("") || PeriodoFilter.Equals("null")) { }
			else
			{
				table.TABLECONDICIONSQL += " AND PERIODO = '" + PeriodoFilter + "'";
			}

			table.enabledButtonControls = false;
			table.addBtnActions("Editar", "editarEsquema");

			return table.CreateDataTable(sesion);
		}
        
		//GET CREATE DATATABLE
		public string CreateDataTableCalendarP(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string idEsquema = "")
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

			DataTable table = new DataTable();

			table.TABLE = "QEsquemasdePagoFechas01";

			table.COLUMNAS = new string[]{  "Concepto", "Fecha de pago", "Fecha de recibo", "Retenciones" };
			table.CAMPOS = new string[] { "PK1", "ID_ESQUEMA", "CONCEPTO", "FECHADEPAGO", "FECHA_RECIBO", "BLOQUEO" };
            
            table.CAMPOSSEARCH = new string[]{  "CONCEPTO", "FECHADEPAGO" };
            table.CAMPOSHIDDEN = new string[] { "PK1", "ID_ESQUEMA" };

            table.addColumnFormat("BLOQUEO", delegate (string value, ResultSet res)
			{
				string sql =
					"SELECT B.BLOQUEO FROM ESQUEMASDEPAGOFECHASBLOQUEOS E, BLOQUEOS B" +
					" WHERE E.CVE_BLOQUEO=B.CVE_BLOQUEO AND E.PK_ESQUEMADEPAGO=" + res.Get("ID_ESQUEMA") +
					" AND E.PK_ESQUEMADEPAGOFECHA=" + res.Get("PK1");

				ResultSet set = db.getTable(sql);
				string html = "";
				if (set.HasRows)
				{
                    {
                        html += "<div class='btn-group m-r-5 m-b-5'><a class='btn btn-default dropdown-toggle' data-toggle='dropdown' href='javascript:;' aria-expanded='false'>" +
							set.Count + " retención(s) <span class='caret'></span></a><ul class='dropdown-menu'>";
						while (set.Next())
						{
							html += "<li><a href ='javascript:;'> " + set.Get("BLOQUEO") + " </a></li>";
						}
						html += "</ul></div>";
					}
				}
				else
					html = "";// "<div style='text-align: left;'><i class='fa fa-check'></i></div>";
				return html;
			});

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "PK1";
            table.TABLECONDICIONSQL = "ID_ESQUEMA = " + idEsquema + " AND ((ESPECIAL IS NULL) OR (ESPECIAL = ''))";
            table.enabledButtonControls = false;
            table.addBtnActions("Editar", "editarEsquema");

			return table.CreateDataTable(sesion, "DataTable2");
		}

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("ID", typeof(string));
                tbl.Columns.Add("Contrato", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Esquema", typeof(string));
                tbl.Columns.Add("Fecha Inicio", typeof(string));
                tbl.Columns.Add("Fecha Fin", typeof(string));              
                tbl.Columns.Add("Semanas", typeof(string));
                tbl.Columns.Add("No Pagos", typeof(string));
                tbl.Columns.Add("Boloqueo por Contrato", typeof(string));
                tbl.Columns.Add("sede", typeof(string));

                var sede = Request.Params["sedes"];

                ResultSet res = db.getTable("SELECT * FROM QEsquemasdePago01 WHERE CVE_SEDE = '"+ sede + "'");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("ID_ESQUEMA")
                        , res.Get("CONTRATO")
                        , res.Get("PERIODO")
                        , res.Get("ESQUEMADEPAGO")                     
                        , res.Get("FECHAINICIO")
                        , res.Get("FECHAFIN")
                        , res.Get("NOSEMANAS")
                        , res.Get("NOPAGOS")
                        , res.Get("BLOQUEOCONTRATO")
                        , res.Get("CVE_SEDE"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Esquemas");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 80;

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
                    Response.AddHeader("content-disposition", "attachment;  filename=Esquemas.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Esquemas", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Esquemas" + e.Message, sesion);
            }
        }

        // POST: Esquema/Add
        [HttpPost]
        public ActionResult Add(EsquemasModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });
            
            try
            {
                if (model.Add())
                {
                    if (model.existe)
                    {
                        Log.write(this, "Add", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                        return Json(new
                        {
                            msg = Notification.Succes("Esquema agregado con exito: " + model.esquema),
                            IdEsquema = model.Clave
                        });
                    }
                    else
                    {//ya existe
                        Log.write(this, "Add", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                        return Json(new
                        {
                            msg = Notification.Warning("El registro con Esquema: " + model.esquema + " y Periodo: " + model.periodos + ", Ya existe!"),
                            IdEsquema = model.Clave
                        });
                    }
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    if (model.errorMsg == "2627")
                        return Json(new { msg = Notification.Error("Error al agregar, el nombre del esquema no puede repetirse: " + model.esquema) });
                    else return Json(new { msg = Notification.Error("Error al agregar: " + model.esquema) });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    msg = Factory.Notification.Error(e.Message)  
                });
            }
        }

        // POST: /Edit
        [HttpPost]
        public ActionResult Edit(EsquemasModel model)
        {
            Debug.WriteLine("controller edit");

            if (model.Edit())
            {
                Debug.WriteLine("controller edit2");
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: /Save
        [HttpPost]
        public ActionResult Save(EsquemasModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Save())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Esquema guardado con exito: " + model.esquema) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al GUARDAR: " + model.esquema) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        // POST: Esquema/Delete/
        [HttpPost]
        public ActionResult Delete(EsquemasModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Delete())
                {
                    Log.write(this, "Delete", LOG.BORRADO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Esquema ELIMINADO con exito: " + model.Banco) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.Banco) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }
        
        // POST: 
        [HttpPost]
        public ActionResult ConsultaPeriodos(EsquemasModel model)
        {
            Debug.WriteLine("controller ConsultaPeriodos");
            if (model.Obtener_Periodos())
            {
                return Json(new
                {
                    periodos = model.periodos
                });
            }
           
            return View(Factory.View.Access + "Esquemas/Start.cshtml");
        }

        //#EXPORT EXCEL
        public void ExportExcelClalendario()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("ID Pago", typeof(string));
                tbl.Columns.Add("ID Esquema", typeof(string));
                tbl.Columns.Add("Concepto", typeof(string));
                tbl.Columns.Add("Fecha de Pago", typeof(string));
                tbl.Columns.Add("Tipo de Bloqueo", typeof(string));

                var sede = Request.Params["sedes"];

                ResultSet res = db.getTable("SELECT * FROM QEsquemasdePagoFechas01 WHERE CVE_SEDE = '" + sede + "'");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(
                         res.Get("PK1")
                        ,res.Get("ID_ESQUEMA")                       
                        , res.Get("CONCEPTO")                        
                        , res.Get("FECHADEPAGO")
                        , res.Get("BLOQUEO")
                     );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Calendario de Pagos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:E1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Calendario_de_Pagos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Calendario_de_Pagos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Calendario_de_Pagos" + e.Message, sesion);
            }
        }
        
        // POST: Esquema/AddCalendarPagos
        [HttpPost]
        public ActionResult AddCalendarPagosECP(EsquemasModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.AddCalendarPagosEC())
                {
                    Log.write(this, "AddCalendarPagos", LOG.REGISTRO, "SQL:" + model.sql, sesion);

                    return Json(new
                    {
                        msg = Notification.Succes("Calendario_de_Pagos agregado con exito: " + model.conceptoPago),
                        idPagosF = model.idPagosF
                    });
                }
                else
                {
                    Log.write(this, "AddCalendarPagos", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error al agregar: " + model.conceptoPago) });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    msg = Factory.Notification.Error(e.Message)

                });
            }
        }
        
        // POST: Esquema/AddCalendarPagos
        [HttpPost]
        public ActionResult AddCalendarPagos(EsquemasModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.AddCalendarPagos())
                {
                    Log.write(this, "AddCalendarPagos", LOG.REGISTRO, "SQL:" + model.sql, sesion);

                    if (model.msg.Trim() == "EXCEDIDO")
                    {
                        return Json(new
                        {
                            msg = Notification.Warning("No se puede agregar el pago, ya que el numero Maximo de pagos para este esquema es: "+model.npagos),
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            msg = Notification.Succes("Calendario_de_Pagos agregado con exito: " + model.conceptoPago),
                            idPagosF = model.idPagosF
                        });
                    }
                }
                else
                {
                    Log.write(this, "AddCalendarPagos", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error al agregar: " + model.conceptoPago) });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    msg = Factory.Notification.Error(e.Message)

                });
            }
        }
        
        // POST: /Edit
        [HttpPost]
        public ActionResult EditCalendarPagos(EsquemasModel model)
        {
            Debug.WriteLine("controller CalendarPagos");
            if (model.EditCalendarPagos())
            {
                Debug.WriteLine("controller edit2");
                return Json(new JavaScriptSerializer().Serialize(model));
            }

            return View();
        }

		// POST: /Save
		[HttpPost]
		public ActionResult SaveCalendarPagos(EsquemasModel model)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
			model.sesion = sesion;

			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });

			try
			{
				//if (!model.ExisteEsquema_FechaPago_Edit())
				//{
					if (model.SaveCalendarPagos())
					{
						Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
						return Json(new { msg = Notification.Succes("Esquema guardado con exito: " + model.conceptoPago) });
					}
					else
					{
						Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
						return Json(new { msg = Notification.Error(" Error al GUARDAR: " + model.conceptoPago) });
					}
				//}
				//else
				//{
				//	Log.write(this, "ExisteEsquema_FechaPago_Edit", LOG.CONSULTA, "SQL:" + model.sql, sesion);
				//	return Json(new { msg = Notification.Warning("Ya existe en el esquema la fecha de pago: " + model.fechaPago) });
				//}
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });
			}
		}

        // POST: /Update
        [HttpPost]
        public ActionResult updateEdoCtaCalendarPagos(EsquemasModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                //if (!model.ExisteEsquema_FechaPago_Edit())
                //{
                    if (model.UpdateEdoCtaCalendarPagos())
                    {
                        Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Succes("Se han modificado las fechas de pago con éxito, para el concepto de pago: " + model.conceptoPago) });
                    }
                    else
                    {
                        Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Error(" Error al MODIFICAR: " + model.conceptoPago) });
                    }
                //}
                //else
                //{
                //    Log.write(this, "ExisteEsquema_FechaPago_Edit", LOG.CONSULTA, "SQL:" + model.sql, sesion);
                //    return Json(new { msg = Notification.Warning("Ya existe en el esquema la fecha de pago: " + model.fechaPago) });
                //}
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        // POST: Esquema/Delete/
        [HttpPost]
        public ActionResult DeleteCalendarPagos(EsquemasModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.DeleteCalendarPagos())
                {
                    Log.write(this, "Delete", LOG.BORRADO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Esquema ELIMINADO con exito: " + model.Banco) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.Banco) });
                }

            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }
        
		string getTiposDeBloqueo(EsquemasModel model)
		{
			string html = string.Empty;
			int contador = 0;
			Dictionary<string, string> dict = model.Obtener_TipoBloqueo();
			foreach (KeyValuePair<string, string> pair in dict)
				html += "<div class=\"col-md-2\"> <div class=\"form-group\"><label> " + pair.Value + "</label><br><input id='TipoBloqueo_" + (contador++) + "' data-idbloqueo='" + pair.Key + "' type='checkbox' value='" + pair.Key + "'></div></div>\n";
			html += "<input type='hidden' id='TipoBloqueo_length' value='" + contador + "'>";
			return html;
		}

        public JsonResult buscaSociedades(EsquemasModel model)
        {
            try
            {
                var Nombre_sociedad =model.buscaSociedades();
                return Json(Nombre_sociedad, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
          
   
        }
        }// </>
}