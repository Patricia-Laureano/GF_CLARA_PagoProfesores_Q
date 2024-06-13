using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.Pagos;

namespace PagoProfesores.Controllers.Pagos
{
    public class DesbloqueosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public DesbloqueosController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/Desbloqueos/desbloqueos.js" };
            Scripts.SCRIPTS = scripts;
            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10021,  Element = "Controller" }, //PERMISO ACCESO DetallePagos
            };
        }

        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Pagos", "Pantalla de Retenciones", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "APELLIDOS", "ASC");

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            EstadodeCuentaModel modelEC = new EstadodeCuentaModel();
            ViewBag.TIPO_BLOQUEO = getTiposDeBloqueo(modelEC);
            ViewBag.FILTRO_TIPODEBLOQUEO = getfiltroDeBloqueo(modelEC);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Desbloqueos de Pagos Start", LOG.CONSULTA, "Ingresa Desbloqueos", sesion);

            ViewBag.sede = view.createLevels(sesion, "sede");

            return View(Factory.View.Access + "Pagos/Desbloqueos/Start.cshtml");
        }

        public string bloquear(string ids, string bloqueos)
        {
            DesbloqueosModel model = new DesbloqueosModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
          
            if (model.bloquear(ids, bloqueos))
            {
                Log.write(this, "bloquear", LOG.EDICION, "ids:" + ids, model.sesion);
                return Notification.Succes("Los bloqueos se ha actualizado satisfactoriamente.");
            }
            else
                return Notification.Error("No se ha podido hacer la actualizaci&oacute;n completa ");
        }

        public string desBloquear(string ids, string bloqueos)
        {
            DesbloqueosModel model = new DesbloqueosModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (model.desBloquear(ids, bloqueos))
            {
                Log.write(this, "desBloquear", LOG.EDICION, "ids:" + ids, model.sesion);
                return Notification.Succes("Los desbloqueos se ha actualizado satisfactoriamente.");
            }
            else
                return Notification.Error("No se ha podido hacer la actualizaci&oacute;n completa ");
        }

        string getfiltroDeBloqueo(EstadodeCuentaModel model)
        {
            string html = string.Empty;
            int contador = 0;
            Dictionary<string, string> dict = model.Obtener_TipoBloqueo();
            foreach (KeyValuePair<string, string> pair in dict)
                html += "<label class='radio-inline'>\n<input id='TipoBloqueo_filtro" + (contador++) + "' data-idbloqueo='" + pair.Key + "' type='checkbox' value='" + pair.Key + "'> " + pair.Value + "\n</label>";
            html += "<input type='hidden' id='TipoBloqueo_length_filtro' value='" + contador + "'>";
            return html;
        }

        string getTiposDeBloqueo(EstadodeCuentaModel model)
        {
            string html = string.Empty;
            int contador = 0;
            Dictionary<string, string> dict = model.Obtener_TipoBloqueo();
            foreach (KeyValuePair<string, string> pair in dict)
                html += "<label class='radio-inline'>\n<input id='TipoBloqueo_" + (contador++) + "' data-idbloqueo='" + pair.Key + "' type='checkbox' value='" + pair.Key + "'> " + pair.Value + "\n</label>";
            html += "<input type='hidden' id='TipoBloqueo_length' value='" + contador + "'>";
            return html;
        }
        
        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", bool consultar = false, string filter = "", string add_periodos = "", string add_esquema = "", string add_concepto = "", string add_fechaPago = "", string bloqueos = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            DataTable table = new DataTable();

            table.TABLE = "VESTADO_CUENTA_BLOQUEOS";

            table.COLUMNAS =
             new string[] { "Campus", "Periodo", "IDSIU", "Nombre", "Apellidos", "Esquema", "Concepto", "Fecha pago" };
            table.CAMPOS =
                new string[] { "CVE_SEDE", "PERIODO",  "IDSIU", "NOMBRES", "APELLIDOS", "ESQUEMA", "CONCEPTO", "FECHAPAGO",
               "ID_ESTADODECUENTA" , "BLOQUEOS", "ID_ESQUEMA", "BLOQUEOCONTRATO","FECHADEENTREGA"};
            table.CAMPOSSEARCH =
                new string[] { "CVE_SEDE", "PERIODO", "ESQUEMA", "CONCEPTO", "FECHAPAGO", "NOMBRES", "APELLIDOS", "IDSIU" };

            table.CAMPOSHIDDEN = new string[] { "ID_ESTADODECUENTA",  "ID_ESQUEMA", "BLOQUEOS", "BLOQUEOCONTRATO", "FECHADEENTREGA" };

            table.addColumnFormat("CONCEPTO", delegate (string value, ResultSet res) {

                string formato = "";

                var value1 = res.Get("BLOQUEOCONTRATO");
                var value2 = res.Get("BLOQUEOS");
                var value3 = res.Get("FECHADEENTREGA");

                if ((value1 == "True") && (value3 == null || value3 == ""))
                    formato = "<div style = \"text-decoration: line-through; color: red;\" class=\"sorting_1\"><div style = \"width:130px;\" >" + res.Get("CONCEPTO") + "</div></div>";
                else if (Int32.Parse(value2) > 0)
                    formato = "<div style = \"text-decoration: line-through; color: red;\" class=\"sorting_1\"><div style = \"width:130px;\" >" + res.Get("CONCEPTO") + "</div></div>";
                else
                    formato = "<div class=\"sorting_1\"><div style = \"width:130px;\" >" + res.Get("CONCEPTO") + "</div></div>";

                return formato;
            });

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_ESTADODECUENTA";
            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";

            List<string> filtros = new List<string>();
            List<string> bloqueos_filtro = new List<string>();
            string bloqueos_list = "";

            if (add_periodos != "" && add_periodos != "null") filtros.Add("PERIODO = '" + add_periodos + "'");
            if (add_esquema != "" && add_esquema != "null") filtros.Add("ID_ESQUEMA = '" + add_esquema + "'");
            if (add_concepto != "" && add_concepto != "null") filtros.Add("PKCONCEPTOPAGO = '" + add_concepto + "'");

            if (bloqueos != "")
            {
                string[] bloqueos_array = bloqueos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string bloqueo in bloqueos_array)
                {
                    if (bloqueo == "ACT") bloqueos_filtro.Add("CVE_BLOQUEO LIKE '%ACT%'");
                    if (bloqueo == "AJU") bloqueos_filtro.Add("CVE_BLOQUEO LIKE '%AJU%'");
                    if (bloqueo == "PMA") bloqueos_filtro.Add("CVE_BLOQUEO LIKE '%PMA%'");
                }
             
                if (bloqueos_filtro.Count > 0)
                    bloqueos_list = " AND (" + string.Join<string>(" AND ", bloqueos_filtro.ToArray()) + ")";
            }

            string union = "";

            if (filtros.Count > 0)
            {
                union = " AND ";
                table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray()) + bloqueos_list;
            }

            table.enabledCheckbox = true;
            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }
    }
}