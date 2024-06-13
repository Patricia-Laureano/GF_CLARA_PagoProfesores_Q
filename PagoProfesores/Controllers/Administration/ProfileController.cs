using System;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using System.Web.Script.Serialization;
using System.Diagnostics;
using PagoProfesores.Models.Administration;

namespace PagoProfesores.Controllers.Administration
{
    public class ProfileController : Controller
    {
        // GET: Profile


        private database db = new database();

        public ActionResult Start()
        {
            SessionDB sesion = SessionDB.start(Request, Response, false, db);
            try
            {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                ViewBag.Main = view.createMenu("", "", sesion);

                //  ViewBag.DataTable = CreateDataTable(10, 1, null, "PERMISO", "ASC", sesion);

                String[] scripts = { "js/Administracion/profile.js" };
                Scripts.SCRIPTS = scripts;
                ViewBag.Scripts = Scripts.addScript();

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Ver perfil", sesion);


                //DATOS USUARIO
                ProfileModel model = new ProfileModel();
                model.idusuario = sesion.pkUser;
                model.usuario = sesion.nickName;
                model.getDatosUser();

               // informacion personal
                ViewBag.NOMBRE = model.nombre;
                ViewBag.APATERNO = model.apellidop;
                ViewBag.AMATERNO = model.apellidom;
                ViewBag.RFC = model.rfc;
                ViewBag.EDAD = model.edad;
                ViewBag.USUARIO = sesion.nickName;
                
                //TELEFONOS	    
               model.getTelefono('C');
               ViewBag.TEL_CASA = model.telefono; 
               model.getTelefono('T');
               ViewBag.TEL_TRABAJO = model.telefono;
               model.getTelefono('M');
               ViewBag.TEL_MOVIL = model.telefono;


                //DIRECCION	    
                model.Obtener_Direccion();
                ViewBag.CP = model.cp;
                ViewBag.ESTADO = model.estado;
                ViewBag.PAIS = model.pais;
                ViewBag.MUNDEL = model.mundel;
                ViewBag.CALLE = model.calle;
                ViewBag.COLONIA =  model.colonia;
                ViewBag.NUM = model.numero;



                return View();
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Ver perfil" + e.Message, sesion);

                return View();
            }
        }





    }
}