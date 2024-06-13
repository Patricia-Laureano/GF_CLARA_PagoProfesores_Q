using System;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace PagoProfesores.Controllers.Administration
{
    public class PasswordController : Controller
    {

        private database db = new database();

        // GET: Password
       

        public ActionResult Start()
        {
            SessionDB sesion = SessionDB.start(Request, Response, false, db);
            try
            {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                ViewBag.Main = view.createMenu("", "", sesion);

                //  ViewBag.DataTable = CreateDataTable(10, 1, null, "PERMISO", "ASC", sesion);

                String[] scripts = { "js/Administracion/password.js" };
                Scripts.SCRIPTS = scripts;
                ViewBag.Scripts = Scripts.addScript();


                ViewBag.NOMBRE_USUARIO = sesion.completeName;
                ViewBag.USUARIO = sesion.nickName;


                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Cambiar Password", sesion);

                return View();
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Cambiar Password" + e.Message, sesion);

                return View();
            }
        }



        // Get: Password/ValidaPassword
        [HttpPost]
        public ActionResult ValidaPassword(Models.PasswordModel model)
        {
            try
            {
                Debug.WriteLine("enrto class: ");

                if (model.ValidaPassword(EncryptX.Encode(model.passwordActual)))
                {
                    return Json(new { msg = "Existe" });
                }
                else { return Json(new { msg = "No Existe" }); }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }


        // Get: Password/ValidaPassword
        [HttpPost]
        public ActionResult EditaPassword(Models.PasswordModel model)
        {
            try
            {
                Debug.WriteLine("enrto class edit: ");

                if ( model.ActualizaPassword( EncryptX.Encode(model.password), EncryptX.Encode(model.passwordActual)) )
                {
                    return Json(new { msg = Notification.Succes("Se ha editado con exito el Password") });
                }
                else { return Json(new { msg = Notification.Succes("Error al Editar Password") }); }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }






    }
}