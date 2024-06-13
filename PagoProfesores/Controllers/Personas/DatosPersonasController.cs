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
using System.Diagnostics;
using System.Text.RegularExpressions;
using PagoProfesores.Models.Pagos;

namespace PagoProfesores.Controllers.Personas
{
    public class DatosPersonasController : Controller
	{
		private database db;
		private List<Factory.Privileges> Privileges;
		private SessionDB sesion;

        public DatosPersonasController()
		{
			db = new database();
			Scripts.SCRIPTS = new string[] {
				"js/Personas/DatosPersonas/DatosPersonas.js",
				"js/Personas/Pensionados/Pensionados.js"
			};

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10079,  Element = "Controller" }, //PERMISO ACCESO DatosPersonas
                 new Factory.Privileges { Permiso = 10080,  Element = "frm-datospersonas" }, //PERMISO DETALLE DatosPersonas
                 new Factory.Privileges { Permiso = 10081,  Element = "formbtnadd" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10082,  Element = "formbtnsave" }, //PERMISO EDITAR 
                 new Factory.Privileges { Permiso = 10083,  Element = "formbtndelete" }, //PERMISO ELIMINAR 

                 //Pensionados
                 new Factory.Privileges { Permiso = 10084,  Element = "formbtnadd2" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10085,  Element = "formbtnsave2" }, //PERMISO EDITAR 
                 new Factory.Privileges { Permiso = 10086,  Element = "formbtndelete2" }, //PERMISO ELIMINAR 
            };
        }

		public ActionResult Start()
		{
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content(""); }

			Main view = new Main();
			ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
			ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
			ViewBag.Main = view.createMenu("Personas", "Datos personas", sesion);
			ViewBag.DataTable = CreateDataTable(10, 1, null, "ID_PERSONA", "ASC", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return View(Factory.View.NotAccess);

			ViewBag.ComboNAC = CreaCombo("Select * from NACIONALIDADES order by NACIONALIDAD", "NACIONALIDAD", "NACIONALIDAD", "MEXICANA",true);
			ViewBag.ComboTPA = CreaCombo("Select * from TIPOSDEPAGO order by TIPODEPAGO", "CVE_TIPODEPAGO", "TIPODEPAGO", "",true);
			ViewBag.ComboORI = CreaCombo("Select * from ORIGENPERSONA order by ORIGEN", "CVE_ORIGEN", "ORIGEN", "");
			ViewBag.ComboBAN = CreaCombo("Select * from BANCOS order by CVE_BANCO", "CVE_BANCO", "BANCO", "",true);
            ViewBag.ComboBANCO = CreaCombo("Select * from BANCOS order by CVE_BANCO", "CVE_BANCO", "BANCO", "",true);

            Log.write(this, "DatosPersonas Start", LOG.CONSULTA, "Ingresa Pantalla DatosPersonas", sesion);

			return View(Factory.View.Access + "Personas/DatosPersonas/Start.cshtml");
		}

		[HttpPost]
		public string CreaCombo(string Sql = "", string Clave = "", string Valor = "", string Inicial = "",bool opcionone = false)
		{
			string Salida = "";
			DatosPersonasModel model = new DatosPersonasModel();
			Salida = model.ComboSql(Sql, Clave, Valor, Inicial, opcionone);
			return Salida;
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null,string sede = "")
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

			DataTable table = new DataTable();

			table.TABLE = "QPersonas";

			table.COLUMNAS =
				new string[]{  "IDSIU", "Nombres", "Apellidos", "Origen","Acceso&nbsp;Web", "Correo O365","Tipo&nbsp;de&nbsp;pago", "RFC", "CURP", "Banco", "Cuenta&nbsp;CLABE", "Raz&oacute;n&nbsp;social",
				"País", "Estado", "Ciudad", "Municipio&nbsp;/&nbsp;Delegaci&oacute;n", "Colonia", "Calle", "No.", "CP","Instrumento Notarial", "Tomo","Número Notario","Ciudad", "Nombre del Notario"};
            table.CAMPOS =
                new string[]{ "IDSIU", "NOMBRES", "APELLIDOS","Origen",  "CORREO", "CORREO365","TIPODEPAGO", "RFC", "CURP", "CVE_BANCO", "CUENTACLABE", "RAZONSOCIAL",
                "DIRECCION_PAIS", "DIRECCION_ESTADO", "DIRECCION_CIUDAD", "DIRECCION_ENTIDAD", "DIRECCION_COLONIA", "DIRECCION_CALLE", "DIRECCION_NUMERO", "DIRECCION_CP","ID_PERSONA","NOTARIAL_NO","NOTARIAL_VOLUMEN","NOTARIAL_NOTARIO_NO","NOTARIAL_LUGAR","REPRESENTANTE_LEGAL_P" };
                //,"FI_DIRECCION_CP","FI_DIRECCION_PAIS","FI_DIRECCION_ESTADO","FI_DIRECCION_CIUDAD","FI_DIRECCION_ENTIDAD","FI_DIRECCION_COLONIA","FI_DIRECCION_CALLE","FI_DIRECCION_NUMERO" };

            table.CAMPOSSEARCH =
				new string[] { "IDSIU", "NOMBRES", "APELLIDOS","RFC","CURP" };

            table.CAMPOSHIDDEN = new string[] { "ID_PERSONA" };

            table.addColumnClass("IDSIU", "datatable_fixedColumn");
            table.addColumnClass("NOMBRES", "datatable_fixedColumn");
            table.addColumnClass("APELLIDOS", "datatable_fixedColumn");
            
          
            table.dictColumnFormat.Add("RFC", delegate (string value, ResultSet res) {
                if (value != "" && rfcValido(value) == false)
                {
                    return "<div style=\"width:120px; background-color:red;color:;\" > " + value + "</div>";
                } else {
                    return "<div style=\"width:120px; background-color:;color:;\" > " + value + "</div>";
                }
            });

            table.dictColumnFormat.Add("DIRECCION_COLONIA", delegate (string value, ResultSet res) {
                return "<div style=\"width:120px; background-color:;color:;\" > " + value + "</div>";
            });

            table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "ID_PERSONA";
            table.TABLECONDICIONSQL = "CVE_SEDE = '" + sede + "'";

            table.enabledButtonControls = false;

			table.addBtnActions("Editar", "editarDatosPersonas");

			return table.CreateDataTable(sesion);
		}

        //Función para validar un RFC
        // Devuelve el RFC sin espacios ni guiones si es correcto
        // Devuelve false si es inválido
        // (debe estar en mayúsculas, guiones y espacios intermedios opcionales)

        bool rfcValido(string rfc, bool aceptarGenerico = true) {
            
            Regex patronRFC = new Regex(@"^([A-ZÑ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))((-)?([A-Z\d]{3}))?$");

            var validado =  patronRFC.Match(rfc);
            
            if (!validado.Success)  //Coincide con el formato general del regex?
                return false;

            return true;
        }
        
        //#EXPORT EXCEL
        public void ExportExcel()
		{
			SessionDB sesion = SessionDB.start(Request, Response, false, db);

            try
            {
                string sede = Request.Params["sedes"];
                string sql = string.Empty;
                /**
                 * Inicio - Primera pestaña
                 * */
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("ID_PERSONA", typeof(string));
                tbl.Columns.Add("ORIGEN", typeof(string));
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("NOMBRES", typeof(string));
                tbl.Columns.Add("APELLIDOS", typeof(string));
                tbl.Columns.Add("CORREO", typeof(string));
                tbl.Columns.Add("TIPODEPAGO", typeof(string));
                tbl.Columns.Add("RFC", typeof(string));
                tbl.Columns.Add("CURP", typeof(string));
                tbl.Columns.Add("CVE_BANCO", typeof(string));
                tbl.Columns.Add("CUENTACLABE", typeof(string));
                tbl.Columns.Add("RAZONSOCIAL", typeof(string));
                tbl.Columns.Add("DIRECCION_PAIS", typeof(string));
                tbl.Columns.Add("DIRECCION_ESTADO", typeof(string));
                tbl.Columns.Add("DIRECCION_CIUDAD", typeof(string));
                tbl.Columns.Add("DIRECCION_ENTIDAD", typeof(string));
                tbl.Columns.Add("DIRECCION_COLONIA", typeof(string));
                tbl.Columns.Add("DIRECCION_CALLE", typeof(string));
                tbl.Columns.Add("DIRECCION_NUMERO", typeof(string));
                tbl.Columns.Add("DIRECCION_CP", typeof(string));

                sql = "SELECT * FROM QPersonas WHERE CVE_SEDE = '" + sede + "'";

                ResultSet res = db.getTable(sql);

                while (res.Next()) // Here we add five DataRows.
                    tbl.Rows.Add(
                        res.Get("ID_PERSONA"),
                        res.Get("ORIGEN"),
                        res.Get("IDSIU"),
                        res.Get("NOMBRES"),
                        res.Get("APELLIDOS"),
                        res.Get("CORREO"),
                        res.Get("TIPODEPAGO"),
                        res.Get("RFC"),
                        res.Get("CURP"),
                        res.Get("CVE_BANCO"),
                        res.Get("CUENTACLABE"),
                        res.Get("RAZONSOCIAL"),
                        res.Get("DIRECCION_PAIS"),
                        res.Get("DIRECCION_ESTADO"),
                        res.Get("DIRECCION_CIUDAD"),
                        res.Get("DIRECCION_ENTIDAD"),
                        res.Get("DIRECCION_COLONIA"),
                        res.Get("DIRECCION_CALLE"),
                        res.Get("DIRECCION_NUMERO"),
                        res.Get("DIRECCION_CP")
                    );
                /**
                 * Fin - Primera pestaña
                 * */

                /**
                 * Inicio - Segunda pestaña
                 * */
                System.Data.DataTable tblR = new System.Data.DataTable();
                tblR.Columns.Add("ID_PERSONA", typeof(string));
                tblR.Columns.Add("SEDE", typeof(string));
                tblR.Columns.Add("IDSIU", typeof(string));
                tblR.Columns.Add("NOMBRES", typeof(string));
                tblR.Columns.Add("APELLIDOS", typeof(string));
                tblR.Columns.Add("FECHANACIMIENTO", typeof(string));
                tblR.Columns.Add("NACIONALIDAD", typeof(string));
                tblR.Columns.Add("CORREO365", typeof(string));
                tblR.Columns.Add("TELEFONO", typeof(string));
                tblR.Columns.Add("TIPODEPAGO", typeof(string));
                tblR.Columns.Add("RFC", typeof(string));
                tblR.Columns.Add("CURP", typeof(string));
                tblR.Columns.Add("PAIS", typeof(string));
                tblR.Columns.Add("ESTADO", typeof(string));
                tblR.Columns.Add("CIUDAD", typeof(string));
                tblR.Columns.Add("ENTIDAD", typeof(string));
                tblR.Columns.Add("COLONIA", typeof(string));
                tblR.Columns.Add("CALLE", typeof(string));
                tblR.Columns.Add("NUMERO", typeof(string));
                tblR.Columns.Add("CP", typeof(string));
                tblR.Columns.Add("BANCO", typeof(string));
                tblR.Columns.Add("CUENTACLABE", typeof(string));
                tblR.Columns.Add("NOCUENTA", typeof(string));
                tblR.Columns.Add("VERIFICADO", typeof(string));//
                tblR.Columns.Add("TITULOPROFESIONAL", typeof(string));
                tblR.Columns.Add("PROFESION", typeof(string));
                tblR.Columns.Add("CEDULAPROFESIONAL", typeof(string));
                tblR.Columns.Add("FECHACEDULA", typeof(string));
                tblR.Columns.Add("SEGUROSOCIAL", typeof(string));

                sql = "SELECT * FROM QPersonasReporte WHERE SEDE = '" + sede + "'";

                ResultSet resR = db.getTable(sql);



                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();

                while (resR.Next()) { // Here we add five DataRows.

                    model.IDSIU = resR.Get("IDSIU");
                    model.ID_PERSONA = resR.Get("ID_PERSONA");
                    model.NoCuenta = resR.Get("NOCUENTA");

                    if (!string.IsNullOrWhiteSpace(model.NoCuenta))                    
                        model.VerificarCuenta();                   
                    else
                        model.validaCuenta_2(); // limpiar validacuenta** y poner en 0


                    string validado = "NO";
                    if (resR.GetBool("NOCUENTA_VALIDADO")) { validado = "SI"; }



                    tblR.Rows.Add(
                        resR.Get("ID_PERSONA"),
                        resR.Get("SEDE"),
                        resR.Get("IDSIU"),
                        resR.Get("NOMBRES"),
                        resR.Get("APELLIDOS"),
                        resR.Get("FECHANACIMIENTO"),
                        resR.Get("NACIONALIDAD"),
                        resR.Get("CORREO365"),
                        resR.Get("TELEFONO"),
                        resR.Get("TIPODEPAGO"),
                        resR.Get("RFC"),
                        resR.Get("CURP"),
                        resR.Get("PAIS"),
                        resR.Get("ESTADO"),
                        resR.Get("CIUDAD"),
                        resR.Get("ENTIDAD"),
                        resR.Get("COLONIA"),
                        resR.Get("CALLE"),
                        resR.Get("NUMERO"),
                        resR.Get("CP"),
                        resR.Get("BANCO"),
                        resR.Get("CUENTACLABE"),
                        resR.Get("NOCUENTA"),
                        validado,
                        resR.Get("TITULOPROFESIONAL"),
                        resR.Get("PROFESION"),
                        resR.Get("CEDULAPROFESIONAL"),
                        resR.Get("FECHACEDULA"),
                        resR.Get("SEGUROSOCIAL")
                        );


            }
                /**
                 * Fin - Segunda pestaña
                 * */

                using (ExcelPackage pck = new ExcelPackage())
				{
					//Create the worksheet
					ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Personas");
                    ExcelWorksheet wsR = pck.Workbook.Worksheets.Add("Reporte de Personas");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
					ws.Cells["A1:C1"].AutoFitColumns();

                    wsR.Cells["A1"].LoadFromDataTable(tblR, true);
                    wsR.Cells["A1:AB1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:U1"])
					{
						rng.Style.Font.Bold = true;
						rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
						rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
						rng.Style.Font.Color.SetColor(Color.White);
                    }

                    using (ExcelRange rngR = wsR.Cells["A1:AB1"])
                    {
                        rngR.Style.Font.Bold = true;
                        rngR.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                        rngR.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rngR.Style.Font.Color.SetColor(Color.White);
                    }

                    int rowexcel = 2;
                    //FORMAT RFC EXCEL
                    res.ReStart();

                    while (res.Next())
                    {
                        if (!rfcValido(res.Get("RFC")))
                        {
                            ws.Cells["H" + rowexcel].Style.Font.Bold = true;
                            ws.Cells["H" + rowexcel].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["H" + rowexcel].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(153, 0, 0));
                            ws.Cells["H" + rowexcel].Style.Font.Color.SetColor(Color.White);
                        }

                        rowexcel++;
                    }

                    int rowexcelR = 2;
                    //FORMAT RFC EXCEL
                    resR.ReStart();

                    while (resR.Next())
                    {
                        if (!rfcValido(resR.Get("RFC")))
                        {
                            wsR.Cells["K" + rowexcelR].Style.Font.Bold = true;
                            wsR.Cells["K" + rowexcelR].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            wsR.Cells["K" + rowexcelR].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(153, 0, 0));
                            wsR.Cells["K" + rowexcelR].Style.Font.Color.SetColor(Color.White);
                        }

                        rowexcelR++;
                    }

                    //Example how to Format Column 1 as numeric 
                    using (ExcelRange col = ws.Cells[2, 1, 2 + tbl.Rows.Count, 1])
					{
						col.Style.Numberformat.Format = "#,##0.00";
						col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					}

                    using (ExcelRange colR = wsR.Cells[2, 1, 2 + tblR.Rows.Count, 1])
                    {
                        colR.Style.Numberformat.Format = "#,##0.00";
                        colR.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    //Write it back to the client
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.AddHeader("content-disposition", "attachment;  filename=Personas.xlsx");
					Response.BinaryWrite(pck.GetAsByteArray());
				}
				Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo DatosPersonas", sesion);
			}
			catch (Exception e)
			{
				ViewBag.Notification = Notification.Error(e.Message);
				Log.write(this, "Start", LOG.ERROR, "Exporta Excel  DatosPersonas" + e.Message, sesion);
			}
		}

		// POST: DatosPersonas/Add
		[HttpPost]
		public ActionResult Add(DatosPersonasModel model)
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
                        return Json(new { msg = Notification.Succes("Persona agregada con exito: " + model.Idsiu + " - " + model.Nombres + " " + model.Apellidos) });                     

                    } else
                    {
                        Log.write(this, "Add", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Warning("Persona con idsiu: " + model.Idsiu + " Ya existe!" ) });
                    }
                }
				else
				{
					Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al agregar: " + model.Idsiu + " - " + model.Nombres + " " + model.Apellidos) });
                 
                }
			}
			catch (Exception e)
			{
				return Json(new { msg = Factory.Notification.Error(e.Message) });

			}
		}

		// POST: DatosPersonas/Edit/5
		[HttpPost]
		public ActionResult Edit(DatosPersonasModel model)
		{
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            if (model.Edit())
			{
                sesion.vdata["idPersona"] = model.Id_Persona;
                sesion.saveSession();
                return Json(new JavaScriptSerializer().Serialize(model));
			}
			return View();
		}
        
        public ActionResult getIdPersona(DatosPersonasModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            if (model.getIdPersona())
            {
                if (model.Id_Persona != "-1")
                {
                    sesion.vdata["idPersona"] = model.Id_Persona;
                    sesion.saveSession();
                    return Json(new JavaScriptSerializer().Serialize(model));
                }
                else
                {
                    Log.write(this, "Buscar IDSIU en formulario DatosPersonas X", LOG.ERROR, "SQL:" + model.sql, sesion);
                    model.msg = Factory.Notification.Error(" La persona no existe en este contexto: " + model.Idsiu);
                    return Json(new JavaScriptSerializer().Serialize(model));
                }
            }
            else
            {
                Log.write(this, "Buscar IDSIU en formulario DatosPersonas Y", LOG.ERROR, "SQL:" + model.sql, sesion);
                model.msg = Factory.Notification.Error(" La persona no existe en este contexto: " + model.Idsiu);
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            //return View();
        }

        // POST: DatosPersonas/Save
        [HttpPost]
		public ActionResult Save(DatosPersonasModel model)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
			model.sesion = sesion;

			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });
			try
			{
				if (model.Save())
				{
                    if (model.SaveTP())
                    {
                        Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Succes("Persona guardada con exito: " + model.Idsiu + " - " + model.Nombres + " " + model.Apellidos) });
                    } else
                    {
                        Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Warning("Persona guardada con exito: " + model.Idsiu + " - " + model.Nombres + " " + model.Apellidos + ", pero con errores en la PA (tipo de pago).") });
                    }
				}
				else
				{
					Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Error(" Error al guardar Persona: " + model.Idsiu + " - " + model.Nombres + " " + model.Apellidos) });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });

			}
		}

		// POST: DatosPersonas/Delete/5
		[HttpPost]
		public ActionResult Delete(DatosPersonasModel model)
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
					return Json(new { msg = Notification.Succes("Persona ELIMINADA con exito: " + model.Id_Persona) });
				}
				else
				{
					Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
					return Json(new { msg = Notification.Error("Error al Eliminar la Persona = " + model.Id_Persona) });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });

			}
		}

		// POST: DatosPersonas/Edit/5
		[HttpPost]
		public ActionResult CodigoPostalResultados(DatosPersonasModel model)
		{
			Debug.WriteLine("controller CodigoPostalResultados");
			if (model.Obtener_Direccion())
			{
				return Json(new
				{
					Direccion_Estado = model.Direccion_Estado,
					Direccion_Pais = model.Direccion_Pais,
					Direccion_Ciudad = model.Direccion_Ciudad,
					Direccion_Entidad = model.Direccion_Entidad,
					Direccion_Colonia = model.Direccion_Colonia
				});
			}
			return View(Factory.View.Access + "Personas/DatosPersonas/Start.cshtml");
		}
        
		// POST: DatosPersonas/Edit/5
		[HttpPost]
		public ActionResult CodigoPostalRazonSocial(DatosPersonasModel model)
		{
			Debug.WriteLine("controller CodigoPostalRazonSocial");
			if (model.Obtener_Direccion())
			{
				// return Json(new JavaScriptSerializer().Serialize(model));
				return Json(new
				{
					Rz_Direccion_Estado = model.Direccion_Estado,
					Rz_Direccion_Pais = model.Direccion_Pais,
					Rz_Direccion_Ciudad = model.Direccion_Ciudad,
					Rz_Direccion_Entidad = model.Direccion_Entidad,
					Rz_Direccion_Colonia = model.Direccion_Colonia
				});
			}
			return View(Factory.View.Access + "Personas/DatosPersonas/Start.cshtml");
		}
	}//</>
}
