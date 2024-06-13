using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.Personas;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace PagoProfesores.Controllers.Personas
{
    public class PensionadosController : Controller
    {
		private database db;
		private List<Factory.Privileges> Privileges;
		private SessionDB sesion;

		public PensionadosController()
		{
			db = new database();

            Privileges = new List<Factory.Privileges> {
                   new Factory.Privileges { Permiso = 10086,  Element = "formbtndelete" }, //PERMISO ELIMINAR 
            };
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

			DataTable table = new DataTable();

			table.TABLE = "VPENSIONADOS";//VPENSIONADOS

            table.COLUMNAS = new string[]{  "Cuenta","Clabe","Nombre", "Banco","Porcentaje","Monto Fijo", "Tipo de Pago", "Tipo pensión", "" };//IdPersona
            table.CAMPOS = new string[]{ "CUENTA","CLABE", "BENEFICIARION", "BANCO","PORCENTAJE","CUOTA", "TIPODEPAGO","TIPOPENSION", "PK1" };//ID_PERSONA
            table.CAMPOSSEARCH = new string[] {  "CUENTA","CLABE", "BENEFICIARION", "BANCO" };

            table.dictColumnFormat["TIPODEPAGO"] = delegate (string str, ResultSet res) {
                string respuesta = "";
                if (str=="T"){respuesta ="Trasferencia";}
                else {respuesta ="Cheque";}

                return respuesta; 
             };

            table.dictColumnFormat["TIPOPENSION"] = delegate (string str, ResultSet res)
            {
                string respuesta = "";

                if (str.Trim() == "A") { respuesta = "Antes de Impuestos"; }
                else if (str.Trim() == "D") { respuesta = "Después de Impuestos"; }
                else if (str.Trim() == "M") { respuesta = "Monto Fijo"; }
              //  else { respuesta = "Monto Fijo"; }

                return respuesta;
            };


            table.dictColumnFormat["PK1"] = delegate (string str, ResultSet res)
            {
                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                {
                    return "";
                }

                return "<button type=\"button\" class=\"btn btn-sm btn-danger\" onclick=\"formPagePensionados.borrar(" + str + ");\">Quitar</button>";
            };

            string idPersona = sesion.vdata["idPersona"];

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "PK1";
			table.TABLECONDICIONSQL = "ID_PERSONA='" + idPersona + "'";

			//table.enabledButtonControls = true;
			//table.LISTBTNACTIONS.Add("Quitar", "formPagePensionados.borrar");

			return table.CreateDataTable(sesion, "DataTablePensionados");
		}

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {            
                                         

                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Cuenta", typeof(string));
                tbl.Columns.Add("Clabe", typeof(string));
                tbl.Columns.Add("Nombre", typeof(string));             
                tbl.Columns.Add("Banco", typeof(string));
                tbl.Columns.Add("Porcentaje", typeof(string));
                tbl.Columns.Add("Monto Fijo", typeof(string));
                tbl.Columns.Add("Tipo de Pago", typeof(string));
                tbl.Columns.Add("Tipo pensión", typeof(string));


                //  var idPersona = Request.Params["idPersona"];
                string idPersona = sesion.vdata["idPersona"];

                ResultSet res = db.getTable("SELECT * FROM VPENSIONADOS WHERE ID_PERSONA = '" + idPersona + "'");

                string tipopago = "";
                string tipopension = "";

                while (res.Next())
                {
                 
                    tipopago = "";

                    if (res.Get("TIPODEPAGO").Trim() == "T") { tipopago = "Trasferencia"; }
                    else { tipopago = "Cheque"; }


                    tipopension = "";

                    if (res.Get("TIPOPENSION").Trim() == "A") { tipopension = "Antes de Impuestos"; }
                    else if (res.Get("TIPOPENSION").Trim() == "D") { tipopension = "Después de Impuestos"; }
                    else if (res.Get("TIPOPENSION").Trim() == "M") { tipopension = "Monto Fijo"; }                  



                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("CUENTA")
                        , res.Get("CLABE")
                        , res.Get("BENEFICIARIO")                   
                        , res.Get("BANCO")
                        , res.Get("PORCENTAJE")
                        , res.Get("CUOTA")
                        , tipopago
                        , tipopension
                      );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Pensionados");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 20;
                    ws.Column(3).Width = 35;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:H1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Pensionados.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Pensionados", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Pensionados" + e.Message, sesion);
            }
        }
        
        // POST: Pensionados/Add
        [HttpPost]
		public ActionResult Add(PensionadosModel model)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
			model.sesion = sesion;
			/*
			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });
			//*/
			try
			{
				if (model.Add())
				{
					Log.write(this, "Add", LOG.REGISTRO, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Succes("Beneficiario agregado con exito " ) });
				}
				else
				{
					Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Error(" Error al agregar") });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Factory.Notification.Error(e.Message) });
			}
		}

		// POST: Pensionados/Edit/
		[HttpPost]
		public ActionResult Edit(PensionadosModel model)
		{
			if (model.Edit())
			{
				return Json(new JavaScriptSerializer().Serialize(model));
			}
			return View();
		}

		// POST: Pensionados/Save
		[HttpPost]
		public ActionResult Save(PensionadosModel model)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
			model.sesion = sesion;
			/*
			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });
			//*/
			try
			{
				if (model.Save())
				{
					Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Succes("Beneficiario guardado con exito" ) });
				}
				else
				{
					Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Error(" Error al guardar beneficiario  " ) });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });
			}
		}

		// POST: Pensionados/Delete/5
		[HttpPost]
		public ActionResult Delete(PensionadosModel model)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
			model.sesion = sesion;

			try
			{
				if (model.Delete())
				{
					Log.write(this, "Delete", LOG.BORRADO, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Succes("Beneficiario ELIMINADO con exito: " + model.IdPensionado) });
				}
				else
				{
					Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Error("Error al Eliminar la beneficiario = " + model.IdPensionado) });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });
			}
		}

		[HttpPost]
		public ActionResult BuscaPersona(PensionadosModel model)
		{
			if (model.BuscaPersona())
				return Json(new JavaScriptSerializer().Serialize(model));
			else
				return Json(new { msg = Factory.Notification.Warning("No se encontro la persona con el id:" + model.IDSIU) });
		}

		[HttpPost]
		public ActionResult ConsultaPorcentaje(PensionadosModel model)
		{
			double porc = model.ConsultaPorcentaje(model.PK1);
			return Json(new { Porcentaje = porc });
		}
        
        [HttpPost]
        public string CreaCombo(string Sql = "", string Clave = "", string Valor = "", string Inicial = "", bool opcionone = false)
        {
            string Salida = "";
            DatosPersonasModel model = new DatosPersonasModel();
            Salida = model.ComboSql(Sql, Clave, Valor, Inicial, opcionone);
            return Salida;
        }
    }//</>
}