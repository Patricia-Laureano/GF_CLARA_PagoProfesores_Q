using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.Administration;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Administration
{
    public class UsersController : Controller
	{
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        
        public UsersController()
        {
            db = new database();

            string[] scripts = { "js/Administracion/Usuarios/Users.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10252,  Element = "Controller" }, //PERMISO ACCESO 
                 new Factory.Privileges { Permiso = 10253,  Element = "formbtnadd" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10254,  Element = "formbtnsave" }, //PERMISO EDITAR 
                 new Factory.Privileges { Permiso = 10255,  Element = "formbtndelete" }, //PERMISO ELIMINAR 
            };
        }
        
        // GET: Users
        public ActionResult Start()
		{
			UsersModel model = new UsersModel();
			SessionDB sesion = SessionDB.start(Request, Response, false, model.db);

			try
			{
				Main view = new Main();
				ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
				ViewBag.Main = view.createMenu("Administración", "Usuarios", sesion);

				ViewBag.Jerarquias = view.createLevels(sesion,"jerarquia");
			//	ViewBag.JerarquiasUser = view.createTree("tree1", sesion);
				ViewBag.JerarquiasUser = view.createSedes(sesion, "tree1");
				ViewBag.Roles = view.createRoles(sesion);

				ViewBag.DataTable = CreateDataTable(10, 1, null, "USUARIO", "ASC", sesion);

                //	string[] scripts = { "js/Administracion/Usuarios/Users.js" };
                //	Scripts.SCRIPTS = scripts;
                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);


                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);


                return View(Factory.View.Access + "Administracion/Usuarios/Start.cshtml");
            }
			catch (Exception e)
			{
				ViewBag.Notification = Notification.Error(e.Message);
                return View(Factory.View.Access + "Administracion/Usuarios/Start.cshtml");
            }
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show, int pg, string search, string orderby, string sort, SessionDB sesion)
		{
			if (sesion == null)
				sesion = SessionDB.start(Request, Response, false, new database());
			DataTable table = new DataTable();

			table.TABLE = "USUARIOS";
			string[] columnas = { "Usuario", "Nombre", "Apellido paterno", "Apellido materno" };
			string[] campos = { "PK1", "USUARIO", "NOMBRE", "APATERNO", "AMATERNO"  };
			string[] campossearch = { "USUARIO", "NOMBRE", "APATERNO", "AMATERNO" };

			table.CAMPOS = campos;
			table.COLUMNAS = columnas;
			table.CAMPOSSEARCH = campossearch;
			table.CAMPOSHIDDEN = new string[] { "PK1" };

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "PK1";

			table.enabledButtonControls = false;
            return table.CreateDataTable(sesion);
		}

		// POST: Menus/Add
		[HttpPost]
		public ActionResult Add(UsersModel model)
		{
			model.sesion = SessionDB.start(Request, Response, false, model.db);
			try
			{
				if (model.Add())
				{
					//Log.write(this, "Add", LOG.REGISTRO, "SQL:" + sql, sesion);
					return Json(new { msg = Notification.Succes("Usuario agregado con exito: " + model.Nombre) });
				}
				else
				{
					return Json(new { msg = Notification.Error("Error al agregar '" + model.Nombre + "' (" + model.ErrorMessage + ")") });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Factory.Notification.Error(e.Message) });
			}
		}

		// POST: Menus/Edit/
		[HttpPost]
		public ActionResult Edit(UsersModel model)
		{
			if (model.Edit())
			{
				return Json(new JavaScriptSerializer().Serialize(model));
			}
			return Content("-1");
		}
        
		// POST: Menus/Save
		[HttpPost]
		public ActionResult Save(UsersModel model)
		{
			try
			{
				model.sesion = SessionDB.start(Request, Response, false, model.db);
				if (model.Save())
				{
					return Json(new { msg = Notification.Succes("Usuario guardado con exito: " + model.Nombre) });
				}
				else
				{
					return Json(new { msg = Notification.Error("Error al Guardar menu: " + model.Nombre) });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });
			}
		}

		// POST: Menus/Delete/
		[HttpPost]
		public ActionResult Delete(UsersModel model)
		{
			try
			{
				if (model.Delete())
				{
					return Json(new { msg = Notification.Succes("Usuario eliminado con exito: " + model.Nombre) });
				}
				else
				{
					return Json(new { msg = Notification.Error("Error al eliminar: " + model.Nombre) });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });
			}
		}
	}//<end class>
}