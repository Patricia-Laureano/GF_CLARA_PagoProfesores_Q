using ConnectDB;
using Session;
using System;
using System.Collections.Generic;
using Factory;
using System.Web.Mvc;
using PagoProfesores.Models.Personas;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Personas
{
    public class DocsOnlineController : Controller
    {

        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public DocsOnlineController()
        {
            db = new database();

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10029,  Element = "Controller" },
            };
        }

        // GET: DocsOnline
        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content(""); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("DocsOnline", "Documentos Online", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "DocsOnline Start", LOG.CONSULTA, "Ingresa pantalla Documentos", sesion);

            return View(Factory.View.Access + "Personas/DocsOnline/Start.cshtml");
        }
    }
}