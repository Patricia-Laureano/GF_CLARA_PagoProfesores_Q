using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using System.Web.Script.Serialization;
using PagoProfesores.Models.CatalogosCentrales;
using System.Configuration;

namespace PagoProfesores.Controllers.CatalogosCentrales
{
    public class MySuiteController : Controller
    {

        private SessionDB sesion;
        private database db;
        private List<Factory.Privileges> Privileges;
        
        public MySuiteController()
        {
            db = new database();

            String[] scripts = {
                "js/CatalogosCentrales/MySuite/mysuite.js" };     //MODIFICAR EL PATH Y NOMBRE DEL ARCHIVO JS
            Scripts.SCRIPTS = scripts;


            Privileges = new List<Factory.Privileges> {
                  new Factory.Privileges { Permiso = 10205,  Element = "Controller" }, //PERMISO ACCESO BANCOS
                //  new Factory.Privileges { Permiso = 10006,  Element = "frm-sociedades" }, //PERMISO DETALLE BANCOS
               //   new Factory.Privileges { Permiso = 10007,  Element = "formbtnadd" }, //PERMISO AGREGAR BANCOS
                  new Factory.Privileges { Permiso = 10206,  Element = "formbtnsave" }, //PERMISO EDITAR BANCOS
               //   new Factory.Privileges { Permiso = 10009,  Element = "formbtndelete" }, //PERMISO ELIMINAR BANCOS
             };
        }

        // GET: Sociedades    EDITAR
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
                ViewBag.Main = view.createMenu("Catalogos Centrales", "MySuite", sesion);
                ViewBag.DataTable = CreateDataTable(10, 1, null, "CAMP_CODE", "ASC", sesion);

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);
                
                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);

                Log.write(this, "Sociedades Start", LOG.CONSULTA, "Ingresa Pantalla MySuite", sesion);


                return View(Factory.View.Access + "CatalogosCentrales/MySuite/Start.cshtml");
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Sociedades Start", LOG.ERROR, "Ingresa Pantalla MySuite" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR

                return View(Factory.View.Access + "CatalogosCentrales/MySuite/Start.cshtml");
            }
        }


        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            string server = ConfigurationManager.AppSettings["serverMySuite"];
            DataTable table = new DataTable();

            table.TABLE = "TIMBRADO_MYSUITE";                                                           //EDITAR NOMBRE DE LA TABLA EN LA BASE DE DATOS
            String[] columnas = { "Clave", "Requestor", "Transacción", "Country", "RFC Entity", "User", "User Name", "Sucursal"};
            String[] campos = { "CAMP_CODE", "REQUESTOR", "XTRANSACTION", "COUNTRY", "RFCENTITY", "USER_R", "USERNAME", "SUCURSAL" };
            string[] campossearch = { "CAMP_CODE", "RFCENTITY" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CAMP_CODE";                                                     //EDITAR PRIMARYKEY DE LA TABLA
            table.TABLECONDICIONSQL = "SERVER = '" + server + "'";
            table.enabledButtonControls = false;
            table.addBtnActions("Editar", "editarMySuite");                                     //EDITAR EL NOMBRE DE LA ACCION
            return table.CreateDataTable(sesion);
        }


        // POST: Sociedades/Edit/5   EDITAR EL NOMBRE DE LA FORMA
        [HttpPost]
        public ActionResult Edit(MySuiteModel model)        //EDITAR
        {

            if (model.Edit())
            {

                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View(Factory.View.Access + "CatalogosCentrales/MySuite/Start.cshtml");
        }



        // POST: Sociedades/Add  EDITAR NOMBRE DE LA FORMA
        [HttpPost]
        public ActionResult Add(MySuiteModel model)                                    //EDITAR EL NOMBRE DEL MODELO
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });


            try
            {
                if (model.Add())
                {
                    return Json(new { msg = Notification.Succes("Registro agregado con exito ") });    //EDITAR EL CAMPO DEL MODELO
                }
                else
                {
                    return Json(new { msg = Notification.Error("Error al agregar " ) });     //EDITAR EL CAMPO DEL MODELO
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }


        // POST: Sociedades/Add    EDITAR
        [HttpPost]
        public ActionResult Save(MySuiteModel model)        //EDITAR
        {


            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Save())
                {
                    return Json(new { msg = Notification.Succes("Registro guardado con exito ") });   //EDITAR
                }
                else
                {
                    return Json(new { msg = Notification.Error(" Error al GUARDAR ") });    //EDITAR
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }




    }
}