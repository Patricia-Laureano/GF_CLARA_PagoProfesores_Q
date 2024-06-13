using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models;
using System.Web.Script.Serialization;
using System.Text;

namespace PagoProfesores.Controllers.Administration
{
    public class MenusController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        
        public MenusController()
        {
            db = new database();

            string[] scripts = { "js/Administracion/Menus.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10265,  Element = "Controller" }, //PERMISO ACCESO PERMISOS
              //   new Factory.Privileges { Permiso = 30133,  Element = "frm-permisos" }, //PERMISO DETALLE PERMISOS
                 new Factory.Privileges { Permiso = 10266,  Element = "formbtnadd" }, //PERMISO AGREGAR PERMISOS
                 new Factory.Privileges { Permiso = 10267,  Element = "formbtnsave" }, //PERMISO EDITAR PERMISOS
                 new Factory.Privileges { Permiso = 10268,  Element = "formbtndelete" }, //PERMISO ELIMINAR PERMISOS
            };
        }
        
        // GET: Menus
        public ActionResult Start()
		{
			MenusModel model = new MenusModel();
			SessionDB sesion = SessionDB.start(Request, Response, false, model.db);
			try
			{
				Main view = new Main();
				ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
				// ViewBag.Main = view.createMenu(24, 26, sesion);
				ViewBag.Main = view.createMenu("Administración", "Menu", sesion);

				ViewBag.DataTable = CreateDataTable(10, 1, null, "PADRE", "ASC", sesion);
                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);

                ViewBag.PadreOptionsHTML = ConsultaIdsMenu();
				ViewBag.PkPermisoOptionsHTML = ConsultaIdsPermisos();
				ViewBag.OrdenOptionsHTML = buildOptionsHTML();

				Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla Menus. 'hola'", sesion);

				return View();
			}
			catch (Exception e)
			{
				ViewBag.Notification = Notification.Error(e.Message);
				Log.write(this, "Start", LOG.ERROR, "Ingresa a pantalla Menuss" + e.Message, sesion);

				return View();
			}
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show, int pg, string search, string orderby, string sort, SessionDB sesion)
		{
			if (sesion == null)
				if ((sesion = SessionDB.start(Request, Response, false, new database(), SESSION_BEHAVIOR.AJAX)) == null)
					return string.Empty;

			DataTable table = new DataTable();

			table.TABLE = "MENU";
			string[] columnas = { "Id", "Titulo", "Descripcion", "Url", "Padre", "Orden", "Permiso", "Icono" };
			string[] campos = { "PK1", "NOMBRE", "DESCRIPCION", "URL", "PADRE", "ORDEN", "PK_PERMISO", "ICONO" };
			string[] campossearch = { "NOMBRE", "PADRE", "ORDEN" };
            
			table.CAMPOS = campos;
			table.COLUMNAS = columnas;
			table.CAMPOSSEARCH = campossearch;

			table.orderby = orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "PK1";

			table.enabledCheckbox = false;
			table.enabledButtonControls = false;

			table.addBtnActions("Editar", "editarMenu");
			table.addBtnActions("Eliminar", "eliminarMenu");

			return table.CreateDataTable(sesion);
		}

		// POST: Menus/Add
		[HttpPost]
		public ActionResult Add(MenusModel model)
		{
			model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
			if (model.sesion == null) return Content(string.Empty);
			try
			{
				if (model.Add())
				{
					return Json(new { msg = Notification.Succes("Menu agregado con exito: " + model.Nombre) });
				}
				else { return Json(new { msg = Notification.Error("Error al agregar: " + model.Nombre) }); }
			}
			catch (Exception e)
			{
				return Json(new { msg = Factory.Notification.Error(e.Message) });
			}
		}

		// POST: Menus/Edit/
		[HttpPost]
		public ActionResult Edit(MenusModel model)
		{
			model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
			if (model.sesion == null) return Content(string.Empty);
			if (model.Edit())
			{
				model.Dispose();
				return Json(new JavaScriptSerializer().Serialize(model));
			}
			return Content("-1");
		}
        
		// POST: Menus/Save
		[HttpPost]
		public ActionResult Save(MenusModel model)
		{
			try
			{
				model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
				if (model.sesion == null) return Content(string.Empty);
				if (model.Save())
				{
					return Json(new { msg = Notification.Succes("Menu guardado con exito: " + model.Nombre) });
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
		public ActionResult Delete(MenusModel model)
		{
			try
			{
				model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
				if (model.sesion == null) return Content(string.Empty);
				if (model.Delete())
				{
					return Json(new { msg = Notification.Succes("Menu eliminado con exito: " + model.Nombre) });
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

		private string ConsultaIdsMenu()
		{
			MenusModel model = new MenusModel();
			Dictionary<int, string> dict = new Dictionary<int, string>();
			dict.Add(0, "Principal");
			model.ConsultaIdsMenu(dict);

			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<int, string> item in dict)
				sb.Append("<option value=\"").Append(item.Key).Append("\">").Append(item.Value).Append("</option>  ");
			return sb.ToString();
		}

		private string ConsultaIdsPermisos()
		{
			MenusModel model = new MenusModel();
			Dictionary<long, string> dict = new Dictionary<long, string>();
			dict.Add(0, "Sin permiso");
			model.ConsultaIdsPermisos(dict);

			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<long, string> item in dict)
				sb.Append("<option value=\"").Append(item.Key).Append("\">").Append(item.Key).Append(" - ").Append(item.Value).Append("</option>  ");
			return sb.ToString();
		}

		private string buildOptionsHTML()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 1; i <= 20; i++)
				sb.Append("<option value=\"").Append(i).Append("\">").Append(i).Append("</option>  ");
			return sb.ToString();
		}

	}//<end class>
}