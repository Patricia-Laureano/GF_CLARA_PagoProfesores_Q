using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.Personas;
using ConnectUrlToken;
using System.Web.Script.Serialization;
using System.Diagnostics;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Configuration;
using System.Data;

namespace PagoProfesores.Controllers.Personas
{
    public class ImportarDatosSIUController : Controller
    {
        private SessionDB sesion;
        private database db;
        private List<Factory.Privileges> Privileges;

        public int sinCP { get; set; }
        public string IDSIUSCP;

        public ImportarDatosSIUController()
        {
            db = new database();
            Scripts.SCRIPTS = new string[] { "js/Personas/ImportarDatosSIU.js", "plugins/autocomplete/js/jquery.easy_autocomplete.js" };

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10087,  Element = "Controller" }, //PERMISO ACCESO DatosPersonas           
                 new Factory.Privileges { Permiso = 10088,  Element = "formbtnConsultar" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10089,  Element = "formbtnImportar" }, //PERMISO EDITAR
            };
        }

        // GET: ImportarDatosSIU
        public ActionResult Start()
        {
            ImportarDatosSIUModel model = new ImportarDatosSIUModel();
            SessionDB sesion = SessionDB.start(Request, Response, false, model.db);
            if ((model.sesion = sesion) == null)
                return Content("");
            model.Clean();
            try
            {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
                ViewBag.Main = view.createMenu("Personas", "Importar Banner", sesion);

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                sesion.vdata["TABLE_PERSONAS"] = "QPersonasTMP";
                sesion.saveSession();

                ViewBag.BlockingPanel_1 = Main.createBlockingPanel("blocking-panel-1");
                ViewBag.BlockingPanel_2 = Main.createBlockingPanel("blocking-panel-2", false, "");
                ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion);


                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Importar Datos SIU' ", sesion);

                return View();
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa a pantalla 'Importar Datos SIU' " + e.Message, sesion);

                return View();
            }
        }

        [HttpPost]
        public ActionResult BuscaPersona(ImportarDatosSIUModel model)
        {
            if (model.BuscaPersona())
            {
                Log.write(this, "Controller: ImportarDatosSIU  - BuscaPersona", LOG.CONSULTA, "SQL:" + model.sql, sesion);
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            else
            {
                model._msg = "No se encuentra el ID ingresado";
                Log.write(this, "Controller: ImportarDatosSIU - BuscaPersona", LOG.ERROR, "SQL:" + model.sql, sesion);
                return Json(new JavaScriptSerializer().Serialize(model));
            }
        }

        [HttpGet]
        public string Consultar(string Periodo, string Sedes, string IDSIU)
        {
            if (sesion == null) sesion = SessionDB.start(Request, Response, false, db);

            ImportarDatosSIUModel.EliminaDetallesConflicto(sesion.pkUser.ToString());

            string paURL = ConfigurationManager.AppSettings["xURL"];
            string paUser = ConfigurationManager.AppSettings["xUser"];
            string paSecret = ConfigurationManager.AppSettings["xSecret"];
            string paFormat = ConfigurationManager.AppSettings["xFormat"];

            ConnectUrlToken.ConnectUrlToken con = new ConnectUrlToken.ConnectUrlToken(paURL, paUser, paSecret, paFormat);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string str_json = "";
            string servicio = "";

            if (IDSIU == "" || IDSIU == null)
            {
                str_json = serializer.Serialize(
                   new ServiceDatosProfesores
                   {
                       periodo = Periodo,
                       campusVPDI = Sedes
                   });

                servicio = "srvDatosProfesores";
            }
            else
            {
                str_json = serializer.Serialize(

                   new ServiceDatosProfesores
                   {
                       periodo = "x",
                       campusVPDI = Sedes,
                       idSiu = IDSIU
                   });

                servicio = "srvDatosProfesoresDemanda";
            }

            Token token = con.getToken();

            // Token token = "";
            int maxDatos = 0;
            int agregados = 0;
            string idsius = "";
            sinCP = 0;
            string mensError = "";
            try
            {
                ImportarDatosSIUModel aux = new ImportarDatosSIUModel();
                aux.sesion = sesion;
                aux.Clean();

                con.connect(token, servicio, str_json);

                ImportarDatosSIUModel[] models = con.connectX<ImportarDatosSIUModel[]>(token, servicio, str_json);
                maxDatos = models.Length;

                if (models.Length > 0)
                {
                    int eliminaSinCP = 0;
                    foreach (ImportarDatosSIUModel model in models)
                    {

                        model.CVESEDE = Sedes;
                        model.sesion = sesion;
                        model.TMP = false;
                        model.REGISTRADO = model.exist() ? "1" : "0";
                        model.TMP = true;
                        idsius = model.IDSIU;
                        if (model.CVE_TIPODEPAGO == "RSCO")
                        {
                            model.CVE_TIPODEPAGO = "RDI";
                        }

                        if (model.FI_DIRECCION_CP == "" || model.FI_DIRECCION_CP == null)
                        {
                            sinCP++;
                            model.sinCP(model.IDSIU, Periodo, eliminaSinCP, model.NOMBRES + " " + model.APELLIDOS);
                            if (IDSIUSCP == null)
                            {

                                IDSIUSCP = "'/" + model.IDSIU;
                            }
                            else
                            {
                                IDSIUSCP += "|" + model.IDSIU;
                            }

                            eliminaSinCP = 1;
                        }
                        else
                        {
                            if (model.addTmp())
                                agregados++;
                        }

                    }
                    IDSIUSCP += "'";
                }
                sesion.vdata["TABLE_PERSONAS"] = "QPersonasTMP";
                sesion.saveSession();

                //if (models.Length > 0)
                //{
                //    if (models.Length == agregados)
                //        return Notification.Succes("Datos consultados: " + agregados + " / " + maxDatos);
                //    else
                //        return Notification.WarningDetail("Datos consultados: " + agregados + " / " + maxDatos);
                //}
                //else
                //    return Notification.Warning("No se han encontrado datos con los filtros especificados.");
                string datos = Periodo + "|" + sesion.pkUser + "|" + Sedes + "|" + idsius;
                if (models.Length > 0)
                {
                    if (models.Length == agregados && sinCP == 0)
                    {
                        return Notification.Succes("Datos consultados: " + agregados + " / " + maxDatos);

                    }
                    else
                    {
                        return Notification.WarningDetailSinCP("Existen docentes sin Código Postal: " + sinCP + " / " + maxDatos, datos);
                    }
                }
                else
                {
                    return Notification.Warning("No se han encontrado datos con los filtros especificados.");
                }

            }
            catch (Exception ex)
            {
                mensError = ex.Message;
                Debug.WriteLine("Error." + ex.Message);
            }
            return Notification.Error(mensError + "Ocurrio un error al consultar la informacion. Registros consultados: " + agregados + " / " + maxDatos);
        }

        public string importar(string ids, string sede)
        {
            ImportarDatosSIUModel model = new ImportarDatosSIUModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
            model.CVESEDE = sede;

            if (model.Importar(ids))
            {
                Log.write(this, "importar", LOG.EDICION, "ids:" + ids, model.sesion);
                return Notification.Succes("Los datos se han actualizado satisfactoriamente.");
            }
            else
                return Notification.Error("No se ha podido hacer la importación");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show, int pg, string search, string orderby, string sort, SessionDB sesion)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Factory.DataTable table = new Factory.DataTable();
            string CheckIcon = "<i class=\"fa fa-check\"></i>";

            table.TABLE = sesion.vdata["TABLE_PERSONAS"];
            table.COLUMNAS = new string[] { "Reg", "ID", "Nombre", "Apellidos","Tipo de Pago", "RFC", "CURP",
                                            "Pais", "Estado", "Ciudad", "Del/Mun", "Colonia", "Calle",
                                            "CP", "Escuela sede", "Cve. Título", "Título profesional",
                                            "Licenciatura", "Maestría", "Cve. Profesión",
                                            "Profesión", "Cédula profesional", "Fec. Cédula", "NSS"};
            table.CAMPOS = new string[] { "REGISTRADO", "IDSIU", "NOMBRES", "APELLIDOS","TIPODEPAGO", "RFC", "CURP",
                                          "DIRECCION_PAIS", "DIRECCION_ESTADO", "DIRECCION_CIUDAD", "DIRECCION_ENTIDAD",
                                          "DIRECCION_COLONIA", "DIRECCION_CALLE", "DIRECCION_CP", "AREAASIGNACION", "CVE_TITULOPROFESIONAL",
                                          "TITULOPROFESIONAL", "TITULO_LICENCIATURA", "TITULO_MAESTRIA", "CVE_PROFESION", "PROFESION", "CEDULAPROFESIONAL",
                                          "FECHACEDULA", "SEGUROSOCIAL"};
            table.CAMPOSSEARCH = new string[] { "IDSIU", "NOMBRES", "APELLIDOS" };
            table.dictColumnFormat["REGISTRADO"] = delegate (string str, ResultSet res) { return str == "True" ? CheckIcon : ""; };

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "IDSIU";
            table.TABLECONDICIONSQL = "USUARIO = '" + sesion.pkUser + "'";

            table.enabledCheckbox = true;
            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            ImportarDatosSIUModel model = new ImportarDatosSIUModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Registro", typeof(string));
                tbl.Columns.Add("ID", typeof(string));
                tbl.Columns.Add("Nombre", typeof(string));
                tbl.Columns.Add("Apellidos", typeof(string));
                tbl.Columns.Add("RFC", typeof(string));
                tbl.Columns.Add("CURP", typeof(string));
                tbl.Columns.Add("Pais", typeof(string));
                tbl.Columns.Add("Estado", typeof(string));
                tbl.Columns.Add("Ciudad", typeof(string));
                tbl.Columns.Add("Del/Mun", typeof(string));
                tbl.Columns.Add("Colonia", typeof(string));
                tbl.Columns.Add("Calle", typeof(string));
                tbl.Columns.Add("CP", typeof(string));
                tbl.Columns.Add("ESCUELA SEDE", typeof(string));
                tbl.Columns.Add("CLAVE TÍTULO", typeof(string));
                tbl.Columns.Add("TÍTULO PROFESIONAL", typeof(string));
                tbl.Columns.Add("LICENCIATURA", typeof(string));
                tbl.Columns.Add("MAESTRIA", typeof(string));
                tbl.Columns.Add("CVE. PROFESIÓN", typeof(string));
                tbl.Columns.Add("PROFESIÓN", typeof(string));
                tbl.Columns.Add("CÉDULA PROFESIONAL", typeof(string));
                tbl.Columns.Add("FEC. CÉDULA", typeof(string));
                tbl.Columns.Add("NSS", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM QPersonasTMP WHERE USUARIO = " + sesion.pkUser + " ORDER BY IDSIU");
                string registro = "";

                while (res.Next())
                {
                    model.IDSIU = res.Get("IDSIU");
                    model.TMP = false;
                    registro = model.exist() ? "Sí" : "No";

                    // Here we add five DataRows.
                    tbl.Rows.Add(registro, res.Get("IDSIU"), res.Get("NOMBRES"), res.Get("APELLIDOS"), res.Get("RFC")
                        , res.Get("CURP"), res.Get("DIRECCION_PAIS"), res.Get("DIRECCION_ESTADO"), res.Get("DIRECCION_CIUDAD")
                        , res.Get("DIRECCION_ENTIDAD"), res.Get("DIRECCION_COLONIA"), res.Get("DIRECCION_CALLE")
                        , res.Get("DIRECCION_CP"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Importar Datos SIU");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:M1"])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    //Example how to Format Column 1 as numeric 
                    using (ExcelRange col = ws.Cells[2, 1, 2 + tbl.Rows.Count, 1])
                    {
                        col.Style.Numberformat.Format = "#,##0.00";
                        col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    //Write it back to the client
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=ImportarDatos_SIU.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Importar Datos SIU", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Importar Datos SIU" + e.Message, sesion);
            }
        }

        [HttpGet]
        public string ConsultaCiclos(ImportarDatosSIUModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            StringBuilder sb = new StringBuilder();
            string selected = "selected";
            foreach (KeyValuePair<long, string> pair in model.ConsultaCiclos())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Key).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        [HttpGet]
        public string ConsultaPeriodos(ImportarDatosSIUModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            string ClaveCiclo = Request.Params["ClaveCiclo"];
            StringBuilder sb = new StringBuilder();
            foreach (string str in model.ConsultaPeriodos(ClaveCiclo))
            {
                sb.Append("<option value=\"").Append(str).Append("\">").Append(str).Append("</option>\n");
            }
            return sb.ToString();
        }

        [HttpGet]
        public string getDetallesConflictoProfs()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            System.Data.DataTable dt = new System.Data.DataTable();
            ImportarDatosSIUModel model = new ImportarDatosSIUModel();
            string htmlTable = string.Empty;

            try { dt = model.ConsultaDetallesConflicto(sesion.pkUser.ToString()); }
            catch { dt = null; }

            if (dt.Rows.Count > 0 && dt != null)
            {
                htmlTable += "<table style='width: 100%; table-layout: fixed;'>"
                           + "<tr><th style='width: 70px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>IDSIU</th>"
                           + "    <th style='width: 250px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>Nombre</th>"
                           + "    <th style='width: 500px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>Mensaje de error</th>"
                           + "    <th style='width: 4000px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>Query</th></tr>";

                foreach (DataRow dr in dt.Rows)
                    htmlTable += "<tr><td style='width: 70px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["IDSIU"] + "</td>"
                               + "    <td style='width: 250px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["NOMBRE"] + "</td>"
                               + "    <td style='width: 500px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["ERRMSG"] + "</td>"
                               + "    <td style='width: 4000px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["QUERY"] + "</td></tr>";

                htmlTable += "</table>";
            }
            else
            {
                htmlTable = "<p style='color:red;'>¡Ups! Disculpa, hay un error al consultar el detalle, favor de avisar al administrador.</p>";

            }

            return htmlTable;
        }
        public string getDetallesSinCP(string periodo)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            System.Data.DataTable dt = new System.Data.DataTable();
            ImportarDatosSIUModel model = new ImportarDatosSIUModel();
            string htmlTable = string.Empty;

            try { dt = model.BuscaPersonaSCP(periodo); }
            catch { dt = null; }

            if (dt.Rows.Count > 0 && dt != null)
            {
                htmlTable += "<table style='width: 100%; table-layout: fixed;'>"
                           + "<tr><th style='width: 70px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>IDSIU</th>"
                           + "    <th style='width: 250px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>DOCENTE</th></tr>";


                foreach (DataRow dr in dt.Rows)
                    htmlTable += "<tr><td style='width: 70px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["IDSIU"] + "</td>"
                               + "    <td style='width: 250px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["DOCENTE"] + "</td></tr>";

                htmlTable += "</table>";
            }
            else
            {
                htmlTable = "<p style='color:red;'>¡Ups! Disculpa, hay un error al consultar el detalle, favor de avisar al administrador.</p>";

            }

            return htmlTable;
        }
        public class ServiceDatosProfesores
        {
            public string periodo;
            public string campusVPDI;
            public string idSiu;
        }

        public static string WarningDetailSinCP(string message, string idsius)
        {
            string alert = "<div class=\"row\">"
                         + "   <div id =\"alerta\" class=\"alert alert-warning fade in\">"
                         + "      <span data-dismiss=\"alert\" class=\"close\">"
                         + "         <img src=\"Content/images/icon-close.png\" style=\"margin-top:-7px\">"
                         + "      </span>"
                         + "      <img src=\"Content/images/icon-warning.png\" class=\"pull-left\" style=\"margin-top:-7px\">"
                         + "      <p> " + message + "</p>"
                         + "      <a id=\"viewDetails\" href=\"#\" onClick=\"javascript:verDetallesSCP('" + idsius + "');\">Ver detalles</a>"
                         + "   </div>"
                         + "</div>"
                         + "<input type=\"hidden\" id=\"NotificationType\" value=\"WARNING\">";

            return alert;
        }
    }
}