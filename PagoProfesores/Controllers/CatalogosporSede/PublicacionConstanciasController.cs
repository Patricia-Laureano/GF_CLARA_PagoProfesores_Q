using ConnectDB;
using Factory;
using PagoProfesores.Models.CatalogosporSede;
using Session;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.CatalogosporSede
{
    public class PublicacionConstanciasController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public PublicacionConstanciasController()
        {
            db = new database();

            string[] scripts = { "js/CatalogosPorSede/PubliConstancias/publiconstancias.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10154,  Element = "Controller" }, //PERMISO ACCESO BANCOS
               //  new Factory.Privileges { Permiso = 10006,  Element = "frm-tabulador" }, //PERMISO DETALLE BANCOS
                 new Factory.Privileges { Permiso = 10155,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
                 new Factory.Privileges { Permiso = 10156,  Element = "formbtnadd2" }, //PERMISO EDITAR BANCOS
                // new Factory.Privileges { Permiso = 10009,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
            };
        }
        
        // GET: 
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Por Sede", "Publicación Constancias", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Publicación Constancias Start", LOG.CONSULTA, "Ingresa pantalla Publicación constancias", sesion);

            return View(Factory.View.Access + "CatalogosPorSede/PubliConstancias/Start.cshtml");
        }
        
        // POST: /Add
        [HttpPost]
        public ActionResult Add(PublicacionConstanciasModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Add())
                {
                    Log.write(this, "Add", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Fecha de publicacion guardado con exito") });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al guardado Fecha de publicacion") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }
        
        // POST: /Edit/5
        [HttpPost]
        public ActionResult Edit(PublicacionConstanciasModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }
        
        // POST: /Add_A
        [HttpPost]
        public ActionResult Add_A(PublicacionConstanciasModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Add_A())
                {
                    Log.write(this, "Add", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Fecha de publicacion guardado con exito") });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al guardado Fecha de publicacion") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }
        
        // POST: /Edit/5
        [HttpPost]
        public ActionResult Edit_A(PublicacionConstanciasModel model)
        {
            if (model.Edit_A())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }
    }
}