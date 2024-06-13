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
    public class ComunicadoController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public ComunicadoController()
        {
            db = new database();
            Scripts.SCRIPTS = new string[] {
                "js/Personas/Comunicado/Comunicado.js","plugins/autocomplete/js/jquery.easy_autocomplete.js"
            };

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10079,  Element = "Controller" }, //PERMISO ACCESO Comunicado
                 new Factory.Privileges { Permiso = 10080,  Element = "frm-comunicado" }, //PERMISO DETALLE Comunicado
                 new Factory.Privileges { Permiso = 10082,  Element = "formbtnsave" }, //PERMISO GUARDAR
                 new Factory.Privileges { Permiso = 10108,  Element = "formbtnclean" }, //PERMISO ELIMINAR 
            };
        }

        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content(""); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Comunicado", "Comunicado", sesion);
            //ViewBag.DataTable = CreateDataTable(10, 1, null, "ID_PERSONA", "ASC", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Comunicado Start", LOG.CONSULTA, "Ingresa pantalla Comunicado", sesion);

            return View(Factory.View.Access + "Personas/Comunicado/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string PeriodoFilter = "", string filter = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "VCOMUNICADO";

            table.COLUMNAS = new string[] { "Sede", "Periodo", "ID SIU", "Esquema de pago", "Concepto de pago", "Mensaje" };
            table.CAMPOS = new string[] { "CVE_SEDE", "PERIODO", "IDSIU", "ESQUEMADEPAGO", "CONCEPTO", "MENSAJE", "PK1"};
            table.CAMPOSSEARCH = new string[] { "CVE_SEDE", "PERIODO", "IDSIU", "ESQUEMADEPAGO", "CONCEPTO", "MENSAJE" };
            table.CAMPOSHIDDEN = new string[] { "PK1" };

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "PK1";

            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";

            //if (PeriodoFilter.Equals("") || PeriodoFilter.Equals("null")) { }
            //else
            //{
            //    table.TABLECONDICIONSQL += " AND PERIODO = '" + PeriodoFilter + "'";
            //}

            table.enabledButtonControls = false;
            table.addBtnActions("Editar", "editarComunicado");

            return table.CreateDataTable(sesion);
        }
        
        // POST: Comunicado/Guardar
        [HttpPost]
        public ActionResult Guardar(ComunicadoModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.AgregarComunicado())
                {
                    Log.write(this, "Add", LOG.REGISTRO, "SQL: sp_comunicado_inserta", sesion);
                    return Json(new
                    {
                        msg = Notification.Succes("Comunicado agregado con éxito."),
                        IdEsquema = model.PK1
                    });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL: sp_comunicado_inserta", sesion);
                    return Json(new { msg = Notification.Error("Error al agregar.") });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    msg = Factory.Notification.Error(e.Message)
                });
            }
        }

        // POST: /Consultar
        [HttpPost]
        public ActionResult Consultar(ComunicadoModel model)
        {
            if (model.ConsultaComunicado())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }

            return View();
        }

        // POST: /Eliminar
        [HttpPost]
        public ActionResult Editar(ComunicadoModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.ActualizarComunicado())
                {
                    Log.write(this, "Add", LOG.REGISTRO, "SQL: sp_comunicado_actualiza", sesion);
                    return Json(new
                    {
                        msg = Notification.Succes("Comunicado actualizado con éxito."),
                        IdEsquema = model.PK1
                    });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL: sp_comunicado_actualiza", sesion);
                    return Json(new { msg = Notification.Error("Error al actualizar.") });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    msg = Factory.Notification.Error(e.Message)
                });
            }
        }

        [HttpPost]
        public ActionResult Eliminar(ComunicadoModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.EliminarComunicado())
                {
                    //Log.write(this, "Delete", LOG.BORRADO, "SQL:" + model.sql, sesion);
                    //return Json(new { msg = Notification.Succes("Esquema ELIMINADO con exito: " + model.Banco) });
                    return Json(new { msg = Notification.Succes("Comunicado eliminado con éxtio.") });
                }
                else
                {
                    //Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    //return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.Banco) });
                    return Json(new { msg = Notification.Succes("Error el eliminar el comunicado.") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }
    }
}
