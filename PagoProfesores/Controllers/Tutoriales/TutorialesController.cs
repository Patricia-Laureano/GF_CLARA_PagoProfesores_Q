using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using System;

namespace PagoProfesores.Controllers.Tutoriales
{
    public class TutorialesController : Controller
    {
        private SessionDB sesion;
        private List<Factory.Privileges> Privileges;
        private database db;
        
        public TutorialesController()
        {
            db = new database();
            string[] scripts = {
                /*
                "plugins/RGraph/libraries/RGraph.common.core.js" ,
                "plugins/RGraph/libraries/RGraph.common.dynamic.js",
                "plugins/RGraph/libraries/RGraph.line.js",
                "plugins/RGraph/libraries/RGraph.bar.js",
                "plugins/RGraph/libraries/RGraph.common.tooltips.js",
                "plugins/RGraph/libraries/RGraph.common.key.js",
                "js/Dashboard/Dashboard_2.js" 
            */ 
            };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10271,  Element = "Controller" },        
            };
        }
        
        // GET: Tutoriales
        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { Response.Redirect("~/"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu(62, 0, sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Tutoriales' ", sesion);

            return View(Factory.View.Access + "Tutoriales/Start.cshtml");
        }
    }
}