using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.CatalogosporSede;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.CatalogosporSede
{
    public class ValidacionCFDIController : Controller
    {
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
		private ValidacionCFDIModel model;

		public ValidacionCFDIController()
        {
            Scripts.SCRIPTS = new string[] { "js/CatalogosPorSede/ValidacionCFDI/ValidacionCFDI.js" };

			Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10273,  Element = "Controller" }, //PERMISO ACCESO Validacion CFDI
            };
        }

		// GET: ValidacionCFDI
		public ActionResult Start()
        {
			model = new ValidacionCFDIModel();
			if (sesion == null) { sesion = SessionDB.start(Request, Response, false, model.db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos por Sede", "Validación CFDI", sesion);
			ViewBag.DataTable = "";
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

			ViewBag.Catalogos = model.ConsultaCatalogos();

            Log.write(this, "ValidacionCFDI Start", LOG.CONSULTA, "Ingresa pantalla Validacion CFDI", sesion);

            return View(Factory.View.Access + "CatalogosPorSede/ValidacionCFDI/Start.cshtml");
        }

		[HttpGet]
		public String GetCatalogos(ValidacionCFDIModel model)
		{
			model.ConsultaCatalogosHTML();
			model.Dispose();
			return Json(new JavaScriptSerializer().Serialize(model)).Data.ToString();
		}

		[HttpPost]
        public ActionResult Save(ValidacionCFDIModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Save())
                {
                    Log.write(this, "Save", LOG.EDICION, "ok", sesion);
                    return Json(new { msg = Notification.Succes(" guardado con éxito") });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "error", sesion);
                    return Json(new { msg = Notification.Error(" Error al guardar") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        [HttpGet]
        public JsonResult GetEnvioCorreo(string ClaveSede)
        {

            ValidacionCFDIModel model;
            model = new ValidacionCFDIModel();

            object datos = model.GetEnvioCorreo(ClaveSede);
            model.Dispose();
            return Json(datos, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult SaveEnvio(ValidacionCFDIModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.SaveEnvio())
                {
                    Log.write(this, "Save", LOG.EDICION, "ok", sesion);
                    return Json(new { msg = Notification.Succes(" guardado con éxito") });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "error", sesion);
                    return Json(new { msg = Notification.Error(" Error al guardar") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

    }
}