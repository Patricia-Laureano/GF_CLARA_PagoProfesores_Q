using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Pagos
{
    public class DesbloqueoComplementoController : Controller
    {

        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public DesbloqueoComplementoController()
        {

            db = new database();

            string[] scripts = { "js/Pagos/GestiondePagos/desbloquearcomp.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {

                new Factory.Privileges { Permiso = 10118,  Element = "Controller" }, //PERMISO ACCESO REGISTRO DE CONTRATOS
                new Factory.Privileges { Permiso = 10119,  Element = "formbtnconsultar" }, //PERMISO EDITAR REGISTRO DE CONTRATOS
              //  new Factory.Privileges { Permiso = 10120,  Element = "formbtn_publicar" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
              //  new Factory.Privileges { Permiso = 10121,  Element = "formbtn_despublicar" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
             //   new Factory.Privileges { Permiso = 10122,  Element = "formbtn_publicar_modal" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
             //   new Factory.Privileges { Permiso = 10123,  Element = "formbtn_despublicar_model" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
            };



        }


        // GET: desbloqueo complemento
        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Pagos", "Gestión de Pagos", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion);

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Registro de contratos Start", LOG.CONSULTA, "Ingresa Pantalla Gestion de Pagos", sesion);

            return View(Factory.View.Access + "Pagos/GestiondePagos/desbloqueocomp/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string fechai = "", string fechaf = "", string filter = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            DataTable table = new DataTable();

            table.TABLE = "VDESBLOQUEO_COMPLEMENTO";

            table.COLUMNAS =
                new string[] { "Campus", "IDSIU","Periodo", "Esquema", "Concepto", "Monto", "IVA", "IVA Ret", "ISR Ret", "Monto Deposito",  "Fecha pago",
                                  "Fecha Recibo", "Fecha Dispersión", "Fecha Depósito", "Fecha Solicitado", "Tipo de pago","Usuario"};
            table.CAMPOS =
                new string[] {"ID_ESTADODECUENTA", "CVE_SEDE", "IDSIU","PERIODO", "ESQUEMA", "CONCEPTO", "MONTO", "MONTO_IVA", "MONTO_IVARET",
                    "MONTO_ISRRET", "BANCOS", "FECHAPAGO", "FECHARECIBO" , "FECHADISPERSION", "FECHADEPOSITO", "FECHA_SOLICITADO","CVE_TIPODEPAGO",
                    "USUARIO", "PADRE", "ID_PERSONA"  };

                       

            table.CAMPOSSEARCH = new string[] {  "PERIODO", "ESQUEMA", "CONCEPTO", "IDSIU" };


            table.CAMPOSHIDDEN = new string[] { "ID_ESTADODECUENTA", "PADRE", "ID_PERSONA"  };


           // string[] camposhidden = { "ID_ESTADODECUENTA", "PADRE" };        

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_ESTADODECUENTA";


            if (filter !="")
            {
                table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";
            }
            else
                table.TABLECONDICIONSQL = "CVE_SEDE = ''";


            List<string> filtros = new List<string>();
         

            if (fechai != "")
            {
                filtros.Add("FECHAPAGO >= '" + fechai + "'");
            }
            if (fechaf != "")
            {
                filtros.Add("FECHAPAGO <= '" + fechaf + "'");
            }

            string union = "";
            if (filter != "" && filtros.Count > 0) { union = " AND "; }

            table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());

            table.enabledButtonControls = false;

           // table.enabledCheckbox = true;

            // table.addBtnActions("Editar", "editarRegistroContratos");

            return table.CreateDataTable(sesion);
        }





        [HttpPost]
        public ActionResult desbloquearComplemento_all(DesbloqueocomplementoModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
            /*
			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });
			//*/
            try
            {
                if (model.desbloquearComplemento_all())
                {
                  //  model.init();
                    Log.write(this, "Desbloqueo Complemento", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se Desbloqueo el Complemento con exito") });
                }
                else
                {
                   // model.init();
                    Log.write(this, "Desbloqueo Complemento", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Desbloquear Complemento(s)") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }






    }
}