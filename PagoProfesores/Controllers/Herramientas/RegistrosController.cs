using System;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace PagoProfesores.Controllers.Herramientas
{
    public class RegistrosController : Controller
    {
          private database db;
        private System.Collections.Generic.List<Factory.Privileges> Privileges;
        private SessionDB sesion;


        public RegistrosController()
        {
            db = new database();

            string[] scripts = { "js/registros/registros.min.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10269,  Element = "Controller" }, //PERMISO ACCESO PERMISOS
              //   new Factory.Privileges { Permiso = 30133,  Element = "frm-permisos" }, //PERMISO DETALLE PERMISOS
              //   new Factory.Privileges { Permiso = 10262,  Element = "formbtnadd" }, //PERMISO AGREGAR PERMISOS
             //    new Factory.Privileges { Permiso = 10263,  Element = "formbtnsave" }, //PERMISO EDITAR PERMISOS
               //  new Factory.Privileges { Permiso = 10264,  Element = "formbtndelete" }, //PERMISO ELIMINAR PERMISOS
            };

        }




    //    private database db = new database();
        // GET: Registros
        public ActionResult Start()
        {

            SessionDB sesion = SessionDB.start(Request, Response, false, db);
            try
            {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                
                ViewBag.Main = view.createMenu("Administración", "Registros", sesion);

                ViewBag.DataTable = CreateDataTable(10, 1, null, "FECHA_R", "ASC", sesion);

                // String[] scripts = { "js/registros/registros.min.js" };
                //  Scripts.SCRIPTS = scripts;
                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);


                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);


                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Registros(seguimento)", sesion);

                return View();
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Registros(seguimento)" + e.Message, sesion);

                return View();
            }

            
           
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();


            table.TABLE = "LOG";
            String[] columnas = { "Clave.","Usuario", "Actividad", "Detalle", "Fecha" };
            String[] campos = { "PK1", "USUARIO", "ACTIVIDAD", "DETALLE","FECHA_R" };
            string[] campossearch = { "PK1", "USUARIO", "DETALLE", "ACTIVIDAD", "FECHA_R" };


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

           // table.addBtnActions("Editar", "editarRole");


            return table.CreateDataTable(sesion);

        }





    }






}