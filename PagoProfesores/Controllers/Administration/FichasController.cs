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
    public class FichasController : Controller
	{
		// GET: Fichas
		public ActionResult Start()
		{
			MenusModel model = new MenusModel();
			SessionDB sesion = SessionDB.start(Request, Response, false, model.db);
			try
			{
				Main view = new Main();
				ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
				// ViewBag.Main = view.createMenu(24, 26, sesion);
				ViewBag.Main = view.createMenu("Administración", "Fichas", sesion);

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                ViewBag.DataTable = CreateDataTable(10, 1, null, "PADRE", "ASC", sesion);

				String[] scripts = { "js/Administracion/Fichas.js" };
				Scripts.SCRIPTS = scripts;
				ViewBag.Scripts = Scripts.addScript();

				ViewBag.PadreOptionsHTML = ConsultaIdsMenu();
				ViewBag.PkPermisoOptionsHTML = ConsultaIdsPermisos();

				Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla Fichas", sesion);

				return View();
			}
			catch (Exception e)
			{
				ViewBag.Notification = Notification.Error(e.Message);
				Log.write(this, "Start", LOG.ERROR, "Ingresa a pantalla Fichass" + e.Message, sesion);

				return View();
			}
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show, int pg, string search, string orderby, string sort, SessionDB sesion)
		{
			if (sesion == null)
			{
				sesion = SessionDB.start(Request, Response, false, new database());
			}
			DataTable table = new DataTable();

			table.TABLE = "MENU";
			string[] columnas = { "Id", "Titulo", "Descripcion", "Url", "Padre", "Orden", "Permiso" };
			string[] campos = { "PK1", "NOMBRE", "DESCRIPCION", "URL", "PADRE", "ORDEN", "PK_PERMISO" };
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

			table.enabledButtonControls = false;

			table.addBtnActions("Editar", "editarRole");

			return table.CreateDataTable(sesion);
		}

		// POST: Fichas/Add
		[HttpPost]
		public ActionResult Add(MenusModel model)
		{
			model.sesion = SessionDB.start(Request, Response, false, model.db);
			try
			{
				if (model.Add())
				{
					return Json(new { msg = Notification.Succes("Ficha agregada con exito: " + model.Nombre) });
				}
				else { return Json(new { msg = Notification.Error(" Erro al agregar: " + model.Nombre) }); }
			}
			catch (Exception e)
			{
				return Json(new { msg = Factory.Notification.Error(e.Message) });
			}
		}

		// POST: Fichas/Edit/
		[HttpPost]
		public ActionResult Edit(MenusModel model)
		{
			if (model.Edit())
			{
				return Json(new JavaScriptSerializer().Serialize(model));
			}
			return Content("-1");
		}


		// POST: Fichas/Save
		[HttpPost]
		public ActionResult Save(MenusModel model)
		{
			try
			{
				model.sesion = SessionDB.start(Request, Response, false, model.db);
				if (model.Save())
				{
					return Json(new { msg = Notification.Succes("Ficha guardada con exito: " + model.Nombre) });
				}
				else
				{
					return Json(new { msg = Notification.Error(" Erro al Guardar ficha: " + model.Nombre) });
				}
			}
			catch (Exception e)
			{
				return Json(new { msg = Notification.Error(e.Message) });
			}
		}

		// POST: Fichas/Delete/
		[HttpPost]
		public ActionResult Delete(MenusModel model)
		{
			try
			{
				if (model.Delete())
				{
					return Json(new { msg = Notification.Succes("Ficha eliminada con exito: " + model.Nombre) });
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

		// POST: Fichas/ConsultaIdsMenu/
		[HttpPost]
		public string ConsultaIdsMenu()
		{
			MenusModel model = new MenusModel();
			Dictionary<int, string> dict = new Dictionary<int, string>();
			dict.Add(0, "Sin padre");
			model.ConsultaIdsMenu(dict);

			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<int, string> item in dict)
				sb.Append("<option value=\"").Append(item.Key).Append("\">").Append(item.Value).Append("</option>  ");
			return sb.ToString();
		}

		// POST: Fichas/ConsultaIdsPermisos/
		[HttpPost]
		public string ConsultaIdsPermisos()
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

	}//<end class>

}
