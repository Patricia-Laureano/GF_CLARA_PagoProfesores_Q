using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.Pagos;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using ConnectUrlToken;
using PagoProfesores.Controllers.Helper;
using System.Configuration;
using System.Data;

namespace PagoProfesores.Controllers.Pagos.PA
{
    public class PAController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        string IdCampus;

        // GET: PA
        public PAController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/PA/pa.min.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10090,  Element = "Controller" }, //PERMISO ACCESO PA
                 new Factory.Privileges { Permiso = 10094,  Element = "frm-pa" }, //PERMISO DETALLE PA
                 new Factory.Privileges { Permiso = 10091,  Element = "formbtnconsultar" }, //PERMISO 
                 new Factory.Privileges { Permiso = 10092,  Element = "formbtngenerar" }, //PERMISO 
                 new Factory.Privileges { Permiso = 10093,  Element = "formbtngenerartodo" }, //PERMISO importar todo PA
            };
        }

        public ActionResult Start()
        {
            PAModel model = new PAModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            model.sesion = sesion;

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu(7, 8, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;
            //ViewBag.SedesIntercom = "UAX";

            sesion.vdata["TABLE_PA"] = "PA_TMP";
            sesion.saveSession();

            ViewBag.BlockingPanel_1 = Main.createBlockingPanel("blocking-panel-1");
            ViewBag.BlockingPanel_2 = Main.createBlockingPanel("blocking-panel-2", false, "");

            model.CleanPersona();
            model.CleanPA();

            ViewBag.DataTable = CreateDataTable(10, 1, null, "NRC", "ASC", sesion);
            ViewBag.COMBO_CAMPUSPA = getCampusPA();

            //*************TABULADOR**************************************
            ViewBag.TabuladorList = getCampusPATab();
            //*************FIN TABULADOR**************************************
            //ATRIBUTOS**************************************
            ViewBag.COMBO_Atributos = getAtributos();
            //FIN ATRIBUTOS**************************************


            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "PA Start", LOG.CONSULTA, "Ingresa pantalla PA", sesion);

            return View(Factory.View.Access + "Pagos/PA/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show, int pg, string search, string orderby, string sort, SessionDB sesion)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Factory.DataTable table = new Factory.DataTable();
            string CheckIcon = "<i class=\"fa fa-check\"></i>";

            table.TABLE = sesion.vdata["TABLE_PA"];

            string[] columnas = { "Reg", "ID SIU", "Docente(NOMBRE)","Docente(Apellido)","Campus", "Escuela", "NRC", "Materia", "Curso"
                    ,"Nombre de Materia","Fecha Inicio","Fecha Fin","Tipo de Curso","Metodo de Instrucción"
                    ,"Status","Inscritos","Parte del periodo","Descripción de parte del periodo"
                    ,"Tipo de Docente","Máximo Grado Académico","Horas Semanales","Horas Programadas"
                    ,"% de Responsabilidad","Horas a Pagar","Opción de Pago","Tabulador","Indicador de sesión","Periodo","Atributos" };

            string[] campos = { "REGISTRADO", "IDSIU","NOMBRE","APELLIDOS","CAMPUS_INB", "ESCUELA", "NRC",  "MATERIA", "CURSO"
                   ,"NOMBREMATERIA","FECHAINICIAL", "FECHAFINAL","TIPODECURSO","METODODEINSTRUCCION"
                   ,"STATUS","INSCRITOS","PARTEDELPERIODO","PARTEDELPERIODODESC", "TIPODEDOCENTE"
                   ,"MAXIMOGRADOACADEMICO","HORASSEMANALES","HORASPROGRAMADAS","RESPONSABILIDAD","HORASAPAGAR"
                   ,"OPCIONDEPAGO","TABULADOR","INDICADORDESESION","PERIODO","USUARIO"
                   ,"CVE_ESCUELA","CVE_TIPODEDOCENTE","CVE_OPCIONDEPAGO", "INDICADOR",  "ID_PA","ATRIBUTOSPROGRAMAGRADO" };

            string[] campossearch = { "CAMPUS_INB", "ESCUELA", "MATERIA", "NRC", "CURSO"
                    ,"NOMBREMATERIA","FECHAINICIAL","FECHAFINAL","TIPODECURSO","METODODEINSTRUCCION"
                    ,"STATUS","INSCRITOS","PARTEDELPERIODO","PARTEDELPERIODODESC","IDSIU","APELLIDOS","NOMBRE", "TIPODEDOCENTE"
                    ,"MAXIMOGRADOACADEMICO","HORASSEMANALES","HORASPROGRAMADAS","RESPONSABILIDAD","HORASAPAGAR"
                    ,"OPCIONDEPAGO","TABULADOR","INDICADORDESESION","PERIODO","ATRIBUTOSPROGRAMAGRADO" };

            string[] camposhidden = { "USUARIO", "CVE_ESCUELA", "CVE_TIPODEDOCENTE", "CVE_OPCIONDEPAGO", "INDICADOR", "ID_PA" };

            table.dictColumnFormat["REGISTRADO"] = delegate (string str, ResultSet res) { return str == "True" ? CheckIcon : ""; };

            table.dictColumnFormat.Add("IDSIU", delegate (string value, ResultSet res)
            {
                if (res.Get("INDICADOR") == "1")
                {
                    return "<div style='width:200px;'>" + value + "&nbsp;<span style='color:#860000'>nomina</span>&nbsp;&nbsp;&nbsp;<span class='fa fa-check-circle'></span></div>";
                }
                else
                {
                    return "<div style=\"width:120px;\">" + value + "</div>";
                }
            });

            table.dictColumnFormat.Add("NOMBRE", delegate (string value, ResultSet res)
            {
                return "<div style=\"width:120px;\">" + value + "</div>";
            });

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_PA";

            table.CAMPOSHIDDEN = camposhidden;

            table.enabledCheckbox = true;
            table.enabledButtonControls = false;

            //table.TABLECONDICIONSQL = "USUARIO =" + sesion.pkUser;
            table.TABLECONDICIONSQL = "USUARIO =" + sesion.pkUser + " AND OPCIONDEPAGO <> 'Tiempo completo'";
            return table.CreateDataTable(sesion);
        }

        [HttpGet]
        public string Consultar(string Periodo, string TipoDeContrato, string TipoPago, string Escuela, string Campus, string CampusPA, string TipoDocente, string PartePeriodo, string Tabulador, string atributos)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            //TipoDocenteController td = new TipoDocenteController();
            //TipoDocente = td.getTipoDocente();

            //TiposPagosController tp = new TiposPagosController();
            //TipoPago = tp.getTiposPagoV();

            string paURL = ConfigurationManager.AppSettings["xURL"];
            string paUser = ConfigurationManager.AppSettings["xUser"];
            string paSecret = ConfigurationManager.AppSettings["xSecret"];
            string paFormat = ConfigurationManager.AppSettings["xFormat"];

            ConnectUrlToken.ConnectUrlToken con = new ConnectUrlToken.ConnectUrlToken(paURL, paUser, paSecret, paFormat);
            if (TipoPago == "RDI")
            {
                TipoPago = "RSCO";
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_json = serializer.Serialize(
                new srvDatosProgramacionAcademica
                {
                    periodo = Periodo,
                    escuela = Escuela,
                    tipoContrato = TipoDeContrato,
                    //tipoDocente  = TipoDocente,
                    campusPA = CampusPA,
                    campusVPDI = Campus,
                    tipoPago = TipoPago,
                    partePeriodo = PartePeriodo,
                    tipoTabulador = Tabulador,
                });

            Token token = con.getToken();
            int maxDatos = 0;
            int agregados = 0;
            try
            {
                PAModel aux = new PAModel();
                aux.sesion = sesion;
                (aux as IPersona).CleanPersona();
                (aux as IPA).CleanPA();

                sesion.vdata["sesion_periodo"] = Periodo;
                sesion.vdata["atributos"] = atributos;

                PAModel[] models = con.connectX<PAModel[]>(token, "srvDatosProgramacionAcademica", str_json);
                maxDatos = models.Length;

                agregados = PAModel.Consultar(models, sesion);

                sesion.vdata["TABLE_PA"] = "PA_TMP";
                sesion.saveSession();

                if (models.Length > 0)
                {
                    if (models.Length == agregados)
                        return Notification.Succes("Datos consultados: " + agregados + " / " + maxDatos);
                    else
                        return Notification.WarningDetail("Datos consultados: " + agregados + " / " + maxDatos);
                }
                else
                    return Notification.Warning("No se han encontrado datos con los filtros especificados.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error." + ex.Message);
            }
            return Notification.Error("Ocurrio un error al consultar la informacion. Registros consultados: " + agregados + " / " + maxDatos);
        }

        [HttpGet]
        public string getDetallesConflictoPA()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            System.Data.DataTable dt = new System.Data.DataTable();
            PAModel model = new PAModel();
            string htmlTable = string.Empty;

            try { dt = model.ConsultaDetallesConflicto(sesion.pkUser.ToString()); }
            catch (Exception ex) { dt = null; }

            if (dt.Rows.Count > 0 && dt != null)
            {
                htmlTable += "<table style='width: 100%; table-layout: fixed;'>"
                           + "<tr><th style='width: 70px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>IDSIU</th>"
                           + "    <th style='width: 250px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>Nombre</th>"
                           + "    <th style='width: 500px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>Mensaje de error</th>"
                           + "    <th style='width: 4000px; background-color:#3E3D3E; color:#FF8520; overflow: hidden; word-wrap: break-word; vertical-align: top; text-align: left; padding: 8px;'>Query</th></tr>";
                foreach (DataRow dr in dt.Rows)
                {
                    htmlTable += "<tr><td style='width: 70px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["IDSIU"] + "</td>"
                               + "    <td style='width: 250px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["NOMBRE"] + "</td>"
                               + "    <td style='width: 500px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["ERRMSG"] + "</td>"
                               + "    <td style='width: 4000px; overflow: hidden; word-wrap: break-word; vertical-align: top; border-bottom: 1px solid #ddd; text-align: left; padding: 8px;'>" + dr["QUERY"] + "</td></tr>";
                }
                htmlTable += "</table>";
            }
            else
            {
                htmlTable = "<p style='color:red;'>¡Ups! Disculpa, hay un error al consultar el detalle, favor de avisar al administrador.</p>";
            }

            return htmlTable;
        }

        public class srvDatosProgramacionAcademica
        {
            public string periodo;
            public string escuela;
            public string tipoContrato;
            //public string tipoDocente;
            public string campusPA;
            public string campusVPDI;
            public string tipoPago;
            public string partePeriodo;
            public string tipoTabulador;
        }

        public string Generar(string ids)
        {
            PAModel model = new PAModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
            int totalPA;
            int registradosPA;
            int registradosPersonas;

            if (model.Generar(ids, out totalPA, out registradosPA, out registradosPersonas))
            {
                Log.write(this, "Generar", LOG.EDICION, "ids:" + ids, model.sesion);
                return Notification.Succes("Los datos se ha actualizado satisfactoriamente. " + registradosPA + "/" + totalPA + " registros (" + registradosPersonas + " personas)");
            }
            else
                return Notification.Error("No se ha podido hacer la Generaci&oacute;n completa (" + registradosPA + " / " + totalPA + " registros generados)");
        }

        public string GenerarTodo(string sedesPersns)
        {
            PAModel model = new PAModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (model.GenerarTodo(sedesPersns, Convert.ToString(sesion.pkUser)))
            {
                Log.write(this, "GenerarTodo", LOG.EDICION, "Se han importado todos los registros", model.sesion);
                return Notification.Succes("Los datos se ha actualizado satisfactoriamente.");
            }
            else
                return Notification.Error("No se ha podido hacer la Generaci&oacute;n completa.");
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            PAModel model = new PAModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Registro", typeof(string));
                tbl.Columns.Add("Campus", typeof(string));
                tbl.Columns.Add("Escuela", typeof(string));
                tbl.Columns.Add("NRC", typeof(string));
                tbl.Columns.Add("Materia", typeof(string));
                tbl.Columns.Add("Curso", typeof(string));
                tbl.Columns.Add("Nombre de Materia", typeof(string));
                tbl.Columns.Add("Fecha Inicio", typeof(string));
                tbl.Columns.Add("Fecha Fin", typeof(string));
                tbl.Columns.Add("Tipo de Curso", typeof(string));
                tbl.Columns.Add("Metodo de Instrucción", typeof(string));
                tbl.Columns.Add("Status", typeof(string));
                tbl.Columns.Add("Inscritos", typeof(string));
                tbl.Columns.Add("Parte del periodo", typeof(string));
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Docente(Apellido)", typeof(string));
                tbl.Columns.Add("Docente(NOMBRE)", typeof(string));
                tbl.Columns.Add("Tipo de Docente", typeof(string));
                tbl.Columns.Add("Máximo Grado Académico", typeof(string));
                tbl.Columns.Add("Horas Semanales", typeof(string));
                tbl.Columns.Add("Horas Programadas", typeof(string));
                tbl.Columns.Add("% de Responsabilidad", typeof(string));
                tbl.Columns.Add("Horas a Pagar", typeof(string));
                tbl.Columns.Add("Opción de Pago", typeof(string));
                tbl.Columns.Add("Tabulador", typeof(string));
                tbl.Columns.Add("Indicador de sesión", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));

                //ResultSet res = db.getTable("SELECT * FROM PA_TMP WHERE USUARIO = " + sesion.pkUser  + " ORDER BY NRC");
                ResultSet res = db.getTable("SELECT * FROM PA_TMP WHERE USUARIO = " + sesion.pkUser + " AND OPCIONDEPAGO <> 'Tiempo completo' ORDER BY NRC");
                string registro = "";

                while (res.Next())
                {
                    registro = model.existExcel(false, res.Get("NRC"), res.Get("PERIODO"), res.Get("IDSIU"), res.Get("CAMPUS_INB")) ? "Sí" : "No";

                    // Here we add five DataRows.
                    tbl.Rows.Add(registro, res.Get("CAMPUS_INB"), res.Get("ESCUELA"), res.Get("NRC"), res.Get("MATERIA")
                        , res.Get("CURSO"), res.Get("NOMBREMATERIA"), res.Get("FECHAINICIAL"), res.Get("FECHAFINAL")
                        , res.Get("TIPODECURSO"), res.Get("METODODEINSTRUCCION"), res.Get("STATUS")
                        , res.Get("INSCRITOS"), res.Get("PARTEDELPERIODO"), res.Get("IDSIU"), res.Get("APELLIDOS")
                        , res.Get("NOMBRE"), res.Get("TIPODEDOCENTE"), res.Get("MAXIMOGRADOACADEMICO"), res.Get("HORASSEMANALES")
                        , res.Get("HORASPROGRAMADAS"), res.Get("RESPONSABILIDAD"), res.Get("HORASAPAGAR")
                        , res.Get("OPCIONDEPAGO"), res.Get("TABULADOR"), res.Get("INDICADORDESESION")
                        , res.Get("PERIODO"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Propuesta de programación Academica");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:AB1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Propuesta_PA.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Propuesta de programación Academica", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Propuesta de programación Academica" + e.Message, sesion);
            }
        }

        [HttpGet]
        public string getCampusPA()
        {
            PAModel model = new PAModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "";
            string IdCampus = sesion.vdata["Sede"];

            foreach (KeyValuePair<string, string> pair in model.getCampusPA())
            {
                selected = (IdCampus == pair.Key) ? "selected" : "";

                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        public string getCampusPATab()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Clear();
            try
            {
                PAModel model = new PAModel();

                SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
                if (sesion == null)
                    return "";

                StringBuilder sb = new StringBuilder();
                string selected = "";
                IdCampus = sesion.vdata["Sede"];

                foreach (KeyValuePair<string, string> pair in model.getCampusPATab(IdCampus))
                {
                    selected = (IdCampus == pair.Key) ? "selected" : "";

                    sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                    selected = "";
                }

                return sb.ToString();
            }
            catch
            {

            }

            return sbr.ToString();
        }

        public string getAtributos()
        {
            PAModel model = new PAModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sbAt = new StringBuilder();
            string selectedAt = "";
            string IdAtributo = "";

            foreach (KeyValuePair<string, string> pair in model.getAtributos())
            {
                selectedAt = (IdAtributo == pair.Key) ? "selected" : "";

                // sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
                sbAt.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selectedAt).Append(">").Append(pair.Value).Append("</option>\n");
                selectedAt = "";
            }
            return sbAt.ToString();
        }
    }
}