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
using System.Collections;

namespace PagoProfesores.Controllers.Personas
{
    public class ImportarDatosPBIController : Controller
    {
        private SessionDB sesion;
        private database db;
        private List<Factory.Privileges> Privileges;


        public string IDSIUSCP;
        public ArrayList myArrayList;

        public string datosConflicto { get; set; }

        public ImportarDatosPBIController()
        {
            db = new database();
            Scripts.SCRIPTS = new string[] { "js/Personas/ImportarDatosPBI.js", "plugins/autocomplete/js/jquery.easy_autocomplete.js" };

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10087,  Element = "Controller" }, //PERMISO ACCESO DatosPersonas           
                 new Factory.Privileges { Permiso = 10088,  Element = "formbtnConsultar" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10281,  Element = "formbtnImportar" }, //PERMISO EDITAR
            };
        }

        // GET: ImportarDatosSIU
        public ActionResult Start()
        {
            ImportarDatosPBIModel model = new ImportarDatosPBIModel();
            SessionDB sesion = SessionDB.start(Request, Response, false, model.db);
            if ((model.sesion = sesion) == null)
                return Content("");
            model.Clean();
            try
            {
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                //ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
                ViewBag.Main = view.createMenu("Personas", "Importar PA Multicampus", sesion);

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                sesion.vdata["TABLE_PERSONAS"] = "QPowerBI_TMP";
                sesion.saveSession();

                ViewBag.BlockingPanel_1 = Main.createBlockingPanel("blocking-panel-1");
                ViewBag.BlockingPanel_2 = Main.createBlockingPanel("blocking-panel-2", false, "");
                ViewBag.DataTable = CreateDataTable(10, 1, null, "IDBANNER", "ASC", sesion);

                model.Clean();
                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Importar Datos PBI' ", sesion);

                return View();
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa a pantalla 'Importar Datos PBI' " + e.Message, sesion);

                return View();
            }
        }


        [HttpGet]
        public string Consultar(string Periodo, string Sedes, string fechai, string fechaf)
        {
            if (sesion == null) sesion = SessionDB.start(Request, Response, false, db);

            ImportarDatosPBIModel.EliminaDetallesConflicto(sesion.pkUser.ToString());

            string paURL = ConfigurationManager.AppSettings["xURL"];
            string paUser = ConfigurationManager.AppSettings["xUser"];
            string paSecret = ConfigurationManager.AppSettings["xSecret"];
            string paFormat = ConfigurationManager.AppSettings["xFormat"];

            ConnectUrlToken.ConnectUrlToken con = new ConnectUrlToken.ConnectUrlToken(paURL, paUser, paSecret, paFormat);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string str_json = "";
            string servicio = "";
            string idbanner = string.Empty;
            bool logGeneralTmp = false;
            string mensajeError = string.Empty;

            var sedesArray = Sedes.Split(',');
            myArrayList = new ArrayList();
            
            foreach (string i in sedesArray)
            {
                myArrayList.Add(i);
            }

            str_json = serializer.Serialize(
                   new ServiceDatosProfesores
                   {
                       campus = myArrayList,
                       periodo = Periodo,
                       fechai = (fechai),
                       fechaf = (fechaf),
                       limit = "20000"
                   });

            servicio = "srvReporte";

            Token token = con.getToken();

            // Token token = "";
            int maxDatos = 0;
            int agregados = 0;
            int conflicto = 0;

                ImportarDatosPBIModel aux = new ImportarDatosPBIModel();
                aux.sesion = sesion;


                aux.Clean();
            try
            {              

                con.connect(token, servicio, str_json);

                ImportarDatosPBIModel[] models = con.connectX<ImportarDatosPBIModel[]>(token, servicio, str_json);
                maxDatos = models.Length;
               
                 if (models.Length > 0)
                {
                    foreach (ImportarDatosPBIModel model in models)
                    {

                        model.sesion = sesion;
                        model.TMP = false;
                        model.TMP = true;
                        model.FECHA_IMPORTADO = "GETDATE()";
                        model.USUARIO_IMPORTA = sesion.pkUser.ToString();
                        model.IMPORTADO_DESDE = "WS";
                        idbanner = model.ID_BANNER;
                        aux.fechainii = __validDateTime(fechai);
                        aux.fechainff = __validDateTime(fechaf);
                        //if ((DateTime.Parse(aux.fechainii.ToString()) >= DateTime.Parse(model.FECHA_INICIO) && DateTime.Parse(aux.fechainii.ToString()) <= DateTime.Parse(model.FECHA_FIN)) && (DateTime.Parse(aux.fechainff.ToString()) >= DateTime.Parse(model.FECHA_INICIO) && DateTime.Parse(aux.fechainii.ToString()) <= DateTime.Parse(model.FECHA_FIN)))
                        //{

                            if (model.addTmp())
                            {
                                agregados++;
                                if (logGeneralTmp == false)
                                {
                                    Log.write(this, "addTmpPOWER_BI", LOG.REGISTRO, ("OK, INSERT COUNT: ") + agregados, this.sesion);
                                    logGeneralTmp = true;


                                }
                                //}
                                //else
                                //{
                                //    conflicto++;
                                //    model.conConflicto(model.ID_BANNER, model.DOCENTE, model.CAMPUSPA, model.NRC, model.CURSO);

                                //    datosConflicto = sesion.pkUser.ToString();
                                //}
                            }
                        //}

                    }
                   
                }
                sesion.vdata["TABLE_PERSONAS"] = "QPowerBI_TMP";
                sesion.saveSession();
                if (models.Length > 0)
                {
                    //if (models.Length == agregados)
                    //    return Notification.Succes("Datos consultados: " + agregados + " / " + maxDatos);
                    //else
                    //    

                    if (conflicto == 0)
                    {
                        return Notification.Succes("Datos consultados: " + agregados + " / " + agregados);
                    }
                    else
                    {                        
                        //return Notification.WarningDetail("Datos consultados: " + agregados + " / " + maxDatos);
                        return Notification.WarningDetailPowerBIConflicto("Existen docentes con conflicto en algún dato: " + conflicto + " / " + agregados, datosConflicto);
                    }
                }
                else
                    return Notification.Warning("No se han encontrado datos con los filtros especificados.");

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error." + ex.Message);
                mensajeError= ex.Message;
                Log.write(this, "importar tpm", LOG.CONSULTA, "ids: " + idbanner, aux.sesion);
            }
            return Notification.Error(mensajeError + "Ocurrio un error al consultar la informacion. Registros consultados: " + conflicto + " / " + agregados);
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show, int pg, string search, string orderby, string sort, SessionDB sesion)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Factory.DataTable table = new Factory.DataTable();
            string CheckIcon = "<i class=\"fa fa-check\"></i>";

            table.TABLE = sesion.vdata["TABLE_PERSONAS"];
            table.COLUMNAS = new string[] { "IDBANNER", "Docente", "Campus", "Escuela", "Descripción de la escuela", "Periodo", "NRC", "Sección", "Materia", "Curso", "Nombde de la materia", "Tipo de curso", "Tipo de horario", "Método de instrucción", "id", "idAdmin" };
            table.CAMPOS = new string[] { "IDBANNER", "DOCENTE", "CAMPUS", "ESCUELA", "DESCRIPCION_ESCUELA", "PERIODO", "NRC", "SECCION", "MATERIA", "CURSO", "NOMBRE_MATERIA", "TIPO_CURSO", "TIPO_HORARIO", "METODO_INSTRUCCION", "ID_PBI_TMP", "USUARIO_IMPORTA" };
            table.CAMPOSSEARCH = new string[] { "IDBANNER", "DOCENTE", "CAMPUS", "ESCUELA", "PERIODO", "NRC", "SECCION", "MATERIA", "CURSO", };
            table.CAMPOS = new string[] { "IMPORTADO", "IDBANNER", "DOCENTE", "CAMPUS", "ESCUELA", "DESCRIPCION_ESCUELA", "PERIODO", "NRC", "SECCION", "MATERIA", "CURSO", "NOMBRE_MATERIA", "TIPO_CURSO", "TIPO_HORARIO", "METODO_INSTRUCCION", "ID_PBI_TMP", "USUARIO_IMPORTA" };
            table.CAMPOSSEARCH = new string[] { "IDBANNER", "DOCENTE", "CAMPUS" , "ESCUELA", "PERIODO", "NRC", "SECCION", "MATERIA", "CURSO", };
            //table.dictColumnFormat["IMPORTADO"] = delegate (string str, ResultSet res) { return str == "True" ? CheckIcon : ""; };

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "IDBANNER";
            table.TABLECONDICIONSQL = "USUARIO_IMPORTA = '" + sesion.pkUser + "'";

            //table.enabledCheckbox = true;
            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("IDBANNER", typeof(string));
                tbl.Columns.Add("PERIODO", typeof(int));
                tbl.Columns.Add("CAMPUS", typeof(string));
                tbl.Columns.Add("DOCENTE", typeof(string));
                tbl.Columns.Add("RFC", typeof(string));
                tbl.Columns.Add("CURP", typeof(string));
                tbl.Columns.Add("MAXIMO_GRADO_ACADEMICO", typeof(string));
                tbl.Columns.Add("TIPO_DE_DOCENTE", typeof(string));
                tbl.Columns.Add("ESTATUS", typeof(string));
                tbl.Columns.Add("NRC", typeof(string));
                tbl.Columns.Add("HORAS_PROGRAMADAS", typeof(string));
                tbl.Columns.Add("ESCUELA", typeof(string));
                tbl.Columns.Add("DESCRIPCION_ESCUELA", typeof(string));
                tbl.Columns.Add("MATERIA", typeof(string));
                tbl.Columns.Add("NOMBRE_DE_MATERIA", typeof(string));
                tbl.Columns.Add("CURSO", typeof(string));
                tbl.Columns.Add("TIPO_CURSO", typeof(string));
                tbl.Columns.Add("SECCION", typeof(string));
                tbl.Columns.Add("TIPO_HORARIO", typeof(string));
                tbl.Columns.Add("METODO_INSTRUCCION", typeof(string));
                tbl.Columns.Add("LISTA_CRUZADA", typeof(string));
                tbl.Columns.Add("PARTE_DEL_PERIODO", typeof(string));
                tbl.Columns.Add("FECHA_PARTE_PERIODO", typeof(string));
                tbl.Columns.Add("TABULADOR_LC", typeof(string));
                tbl.Columns.Add("TABULADOR_POS", typeof(string));
                tbl.Columns.Add("TABULADOR_DI", typeof(string));
                tbl.Columns.Add("PROFESOR_TITULAR", typeof(string));
                tbl.Columns.Add("PORCENTAJE_RESPONSABILIDAD", typeof(string));
                tbl.Columns.Add("NUMERO_INSCRITOS", typeof(string));
                tbl.Columns.Add("ACTIVIDADES_NO_DOCENCIA", typeof(string));
                tbl.Columns.Add("FECHA_INICIO", typeof(string));
                tbl.Columns.Add("FECHA_FIN", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM QPowerBI_TMP WHERE USUARIO_IMPORTA="+sesion.pkUser.ToString());

                while (res.Next())
                {
                    // Here we add five DataRows.

                    tbl.Rows.Add(res.Get("ID_PBI_TMP"), res.Get("PERIODO"), res.Get("CAMPUS"), res.Get("DOCENTE"), res.Get("RFC"), res.Get("CURP"), res.Get("MAXIMO_GRADO_ACADEMICO"), res.Get("TIPO_DOCENTE"), res.Get("ESTATUS"), res.Get("NRC"), res.Get("HORAS_PROGRAMADAS"), res.Get("ESCUELA"), res.Get("DESCRIPCION_ESCUELA"), res.Get("MATERIA"), res.Get("NOMBRE_MATERIA"), res.Get("CURSO"), res.Get("TIPO_CURSO"), res.Get("SECCION"), res.Get("TIPO_HORARIO"), res.Get("METODO_INSTRUCCION"), res.Get("LISTA_CRUZADA"), res.Get("PARTE_PERIODO"), res.Get("FECHA_PARTE_PERIODO"), res.Get("TABULADOR_LC"), res.Get("TABULADOR_POS"), res.Get("TABULADOR_DI"), res.Get("PROFESOR_TITULAR"), res.Get("PORCENTAJE_RESPONSABILIDAD"), res.Get("NUMERO_INSCRITOS"), res.Get("ACTIVIDADES_NO_DOCENCIA"), res.Get("FECHA_INICIO"), res.Get("FECHA_FIN"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("IMPORTACION PA MULTICAMPUS");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:B1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:E1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Importacion_PA_MultiCampus.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel ImportacionPAMultiCampus", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel ImportacionPAMultiCampus" + e.Message, sesion);
            }
        }

        [HttpGet]
        public string ConsultaCiclos(ImportarDatosPBIModel model)
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
        public string ConsultaPeriodos(ImportarDatosPBIModel model)
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
            ImportarDatosPBIModel model = new ImportarDatosPBIModel();
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



        public class ServiceDatosProfesores
        {
            public ArrayList campus { get; set; }
            public string periodo;
            public string fechai;
            public string fechaf;
            public string limit;
        }

        public JsonResult mostrarSedes()
        {
            ImportarDatosPBIModel datosSedes = new ImportarDatosPBIModel();
            var Nombre_sociedad = datosSedes.mostrarSedes();
            return Json(Nombre_sociedad, JsonRequestBehavior.AllowGet);
        }

        public static string WarningDetailPowerBIConflicto(string message, string idsius)
        {
            string alert = "<div class=\"row\">"
                         + "   <div id =\"alerta\" class=\"alert alert-warning fade in\">"
                         + "      <span data-dismiss=\"alert\" class=\"close\">"
                         + "         <img src=\"Content/images/icon-close.png\" style=\"margin-top:-7px\">"
                         + "      </span>"
                         + "      <img src=\"Content/images/icon-warning.png\" class=\"pull-left\" style=\"margin-top:-7px\">"
                         + "      <p> " + message + "</p>"
                         + "      <a id=\"viewDetails\" href=\"#\" onClick=\"javascript:WarningDetailPowerBIConflicto('" + idsius + "');\">Ver detalles</a>"
                         + "   </div>"
                         + "</div>"
                         + "<input type=\"hidden\" id=\"NotificationType\" value=\"WARNING\">";

            return alert;
        }

        public string getDetallesPOWERBI_Conf(string datoss)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            System.Data.DataTable dt = new System.Data.DataTable();
            ImportarDatosPBIModel model = new ImportarDatosPBIModel();
            string htmlTable = string.Empty;

            try { dt = model.BuscaPOWERBI_Conf(datoss); }
            catch { dt = null; }

            if (dt.Rows.Count > 0 && dt != null)
            {
                htmlTable += "<table style='width: 100%; table-layout: fixed;'>"
                           + "<tr><th style='width: 50px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>IDSIU</th>"
                           + "    <th style='width: 200px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>DOCENTE</th>"
                             + "    <th style='width: 50px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>CAMPUS</th>"
                              + "    <th style='width: 40px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>NRC</th>"
                               + "    <th style='width: 50px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>CURSO</th></tr>";


                foreach (DataRow dr in dt.Rows)
                    htmlTable += "<tr><td style='width: 50px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["IDSIU"] + "</td>"
                               + "    <td style='width: 200px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["DOCENTE"] + "</td>"
                               + "    <td style='width: 50px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["CAMPUS"] + "</td>"
                               +"    <td style='width: 40px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["NRC"] + "</td>"
                               +"    <td style='width: 50px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["CURSO"] + "</td></tr>";
                htmlTable += "</table>";
            }
            else
            {
                htmlTable = "<p style='color:red;'>¡Ups! Disculpa, hay un error al consultar el detalle, favor de avisar al administrador.</p>";

            }

            return htmlTable;
        }

        public string __validDateTime(string str)
        {
            string dt;
            if (str != null)
            {
                try
                {
                    string[] array = str.Split(new char[] { '-', ' ' });
                    dt = array[2]+"/"+array[1]+"/" + array[0];
                }
                catch (Exception ex) { return null;/*dt = new DateTime(2000, 1, 1, 0, 0, 0, 0);*/ }
                return dt;
            }
            else
            {
                return str;
            }

        }

        public string __validDateTimeymd(string str)
        {
            string dt;
            if (str != null)
            {
                try
                {
                    string[] array = str.Split(new char[] { '/', ' ' });
                    dt = array[0] + "/" + array[1] + "/" + array[2];
                }
                catch (Exception ex) { return null;/*dt = new DateTime(2000, 1, 1, 0, 0, 0, 0);*/ }
                return dt;
            }
            else
            {
                return str;
            }

        }

    }
}