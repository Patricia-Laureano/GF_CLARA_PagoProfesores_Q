using System;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace PagoProfesores.Controllers.Administration
{
	public class RolesController : Controller
	{
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;



        public RolesController()
        {
            db = new database();

            string[] scripts = { "js/Administracion/roles/roles.min.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10256,  Element = "Controller" }, //PERMISO ACCESO ROLES              
                 new Factory.Privileges { Permiso = 10257,  Element = "formbtnadd" }, //PERMISO AGREGAR ROLES
                 new Factory.Privileges { Permiso = 10258,  Element = "formbtnsave" }, //PERMISO EDITAR ROLES
                 new Factory.Privileges { Permiso = 10259,  Element = "formbtndelete" }, //PERMISO ELIMINAR ROLES
                    new Factory.Privileges { Permiso = 10260,  Element = "permiso" }, //PERMISO ELIMINAR ROLES
                     //  new Factory.Privileges { Permiso = 30138,  Element = "frm-roles" }, //PERMISO DETALLE ROLES
            };

        }




        // GET: Roles
        public ActionResult Start()
		{
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();
            try
			{
				Main view = new Main();
				ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
				// ViewBag.Main = view.createMenu(24, 26, sesion);
				ViewBag.Main = view.createMenu("Administración", "Roles y Permisos", sesion);

				ViewBag.DataTable = CreateDataTable(10, 1, null, "ROLE", "ASC", sesion);

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);


                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);


                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Roles", sesion);

				return View();
			}
			catch (Exception e)
			{
				ViewBag.Notification = Notification.Error(e.Message);
				Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Roles" + e.Message, sesion);

				return View();
			}
		}



		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
		{
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

			DataTable table = new DataTable();


			table.TABLE = "ROLES";
			String[] columnas = { "Clave", "Role", "Descripcion" };
			String[] campos = { "PK1", "ROLE", "DESCRIPCION" };
			string[] campossearch = { "PK1", "ROLE" };


			table.CAMPOS = campos;
			table.COLUMNAS = columnas;
			table.CAMPOSSEARCH = campossearch;

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "PK1";



            if (!sesion.permisos.havePermission(Privileges[4].Permiso)) {

                table.enabledButtonControls = false;

            }
            else
            {
                table.enabledButtonControls = true;

                table.addBtnActions("Permiso", "enviarRolPermisos");

            }
              

          


			return table.CreateDataTable(sesion);

		}


        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Role", typeof(string));
                tbl.Columns.Add("Descripción", typeof(string));               

                ResultSet res = db.getTable("SELECT * FROM ROLES ORDER BY PK1");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("ROLE"), res.Get("DESCRIPCION"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Roles");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:B2"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Roles.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Roles", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Roles" + e.Message, sesion);

            }

        }


        // POST: Roles/Add
        [HttpPost]
		public ActionResult Add(Models.RoleModel model)
		{
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });
            try
			{
				if (model.Add())
				{
					return Json(new { msg = Notification.Succes("Agregado Role con exito: " + model.Role) });
				}
				else { return Json(new { msg = Notification.Error(" Erro al agregar: " + model.Role) }); }
			}
			catch (Exception e)
			{
				return Json(new { msg = Factory.Notification.Error(e.Message) });

			}
		}

		// POST: Roles/Edit/5
		[HttpPost]
		public ActionResult Edit(Models.RoleModel model)
		{
			if (model.Edit())
			{
				return Json(new JavaScriptSerializer().Serialize(model));
			}
			return View();
		}


		// POST: Roles/Add
		[HttpPost]
		public ActionResult Save(Models.RoleModel model)
		{

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
			{

				if (model.Save())
				{
					return Json(new { msg = Notification.Succes("Guardado Role con exito: " + model.Role) });
				}
				else { return Json(new { msg = Notification.Error(" Erro al GUARDAR: " + model.Role) }); }
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });

			}
		}


		// POST: Roles/Delete/5
		[HttpPost]
		public ActionResult Delete(Models.RoleModel model)
		{


            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });
            try
			{
				if (model.Delete())
				{
					return Json(new { msg = Notification.Succes("Eliminado Role con exito: " + model.Role) });
				}
				else { return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.Role) }); }

			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });

			}
		}


	}
}
