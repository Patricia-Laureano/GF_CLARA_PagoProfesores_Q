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

namespace PagoProfesores.Controllers.Pagos.PA
{
    public class ActualizacionPAController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        // GET: PA
        public ActualizacionPAController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/ActualizacionPA/ActualizacionPA.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10136,  Element = "Controller" }, //PERMISO ACCESO PA
                 new Factory.Privileges { Permiso = 10137,  Element = "formbtnconsultar" }, //PERMISO AGREGAR PA
                 new Factory.Privileges { Permiso = 10138,  Element = "formbtngenerartodo" }, //PERMISO EDITAR PA
            };
        }

        public ActionResult Start()
        {
            ActualizacionPAModel model = new ActualizacionPAModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            model.sesion = sesion;

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Pagos", "Actualización de la PA", sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            sesion.vdata["TABLE_PA"] = "PA_TMP";
            sesion.saveSession();

            ViewBag.BlockingPanel_1 = Main.createBlockingPanel("blocking-panel-1");
            ViewBag.BlockingPanel_2 = Main.createBlockingPanel("blocking-panel-2", false, "");

            model.CleanPersona();
            model.CleanPA();

            ViewBag.DataTable = CreateDataTable(10, 1, null, "NRC", "ASC", sesion);
            //ViewBag.DataTable = CreateDataTable(10, 1, null, "IdPA", "ASC", sesion);
            ViewBag.COMBO_CAMPUSPA = getCampusPA();
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "PA Start", LOG.CONSULTA, "Ingresa pantalla PA", sesion);

            return View(Factory.View.Access + "Pagos/ActualizacionPA/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show, int pg, string search, string orderby, string sort, SessionDB sesion,
                                      string periodo = "", string partePeriodo = "", string opcionPago = "", string campusVPDI = "", string campusPA = "", string Escuela = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            ActualizacionPAModel model = new ActualizacionPAModel();
            model.cleanPAExcelTMP(sesion);

            DataTableCompare table = new DataTableCompare();
            //string CheckIcon = "<i class=\"fa fa-check\"></i>";

            table.TABLE = sesion.vdata["TABLE_PA"];

            string[] columnas = { "Estatus", "IDSIU", "Docente(NOMBRE)","Docente(Apellido)","Campus", "Escuela", "NRC", "Materia", "Curso"
                    ,"Nombre de Materia","Fecha Inicio","Fecha Fin","Tipo de Curso","Metodo de Instrucción"
                    ,"Status","Inscritos","Parte del periodo","Descripción de parte del periodo"
                    ,"Tipo de Docente","Máximo Grado Académico","Horas Semanales","Horas Programadas"
                    ,"% de Responsabilidad","Horas a Pagar","Opción de Pago","Tabulador","Indicador de sesión","Periodo"};

            string[] campos = { "IP", "IDSIU","NOMBRE","APELLIDOS","CAMPUS_INB", "ESCUELA", "NRC",  "MATERIA", "CURSO"
                   ,"NOMBREMATERIA","FECHAINICIAL", "FECHAFINAL","TIPODECURSO","METODODEINSTRUCCION"
                   ,"STATUS","INSCRITOS","PARTEDELPERIODO","PARTEDELPERIODODESC","TIPODEDOCENTE"
                   ,"MAXIMOGRADOACADEMICO","HORASSEMANALES","HORASPROGRAMADAS","RESPONSABILIDAD","HORASAPAGAR"
                   ,"OPCIONDEPAGO","TABULADOR","INDICADORDESESION","PERIODO","USUARIO"
                   ,"CVE_ESCUELA","CVE_TIPODEDOCENTE","CVE_OPCIONDEPAGO" };

            string[] campossearch = { "CAMPUS_INB", "ESCUELA", "MATERIA", "NRC", "CURSO"
                    ,"NOMBREMATERIA","FECHAINICIAL","FECHAFINAL","TIPODECURSO","METODODEINSTRUCCION"
                    ,"STATUS","INSCRITOS","PARTEDELPERIODO","PARTEDELPERIODODESC","IDSIU","APELLIDOS","NOMBRE", "TIPODEDOCENTE"
                    ,"MAXIMOGRADOACADEMICO","HORASSEMANALES","HORASPROGRAMADAS","RESPONSABILIDAD","HORASAPAGAR"
                    ,"OPCIONDEPAGO","TABULADOR","INDICADORDESESION","PERIODO" };

            string[] camposhidden = { "USUARIO", "CVE_ESCUELA", "CVE_TIPODEDOCENTE", "CVE_OPCIONDEPAGO" };

            //table.dictColumnFormat["REGISTRADO"] = delegate (string str, ResultSet res) { return str == "True" ? CheckIcon : ""; };

            //table.dictColumnFormat.Add("IdPA", delegate (string value, ResultSet res)
            //{
            //    return "<div style=\"width:120px;\">" + value + "</div>";
            //});

            //table.dictColumnFormat.Add("NOMBRE", delegate (string value, ResultSet res)
            //{
            //    return "<div style=\"width:120px;\">" + value + "</div>";
            //});

            //TiposPagosController tp = new TiposPagosController();
            //opcionPago = tp.getTiposPagoV();

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "IDSIU";

            table.CAMPOSHIDDEN = camposhidden;

            table.enabledButtonControls = false;

            table.TABLECONDICIONSQL = "USUARIO =" + sesion.pkUser;

            //if (opcionPago != "" && opcionPago != null)
            //{
            //    switch (opcionPago)
            //    {
            //        case "A":
            //            opcionPago = "ADI";
            //            break;
            //        case "H":
            //            opcionPago = "HDI";
            //            break;
            //        default:
            //            opcionPago = "-X1"; //No existe está opción de pago
            //            break;
            //    }
            //}

            var sql = " PERIODO = '" + periodo + "'";

            if (partePeriodo != "" && partePeriodo != null)
                sql += " AND PARTEDELPERIODO = '" + partePeriodo + "'";

            if (opcionPago != "" && opcionPago != null)
                sql += " AND CVE_TIPODEPAGO = '" + opcionPago + "'";

            if (Escuela != "" && Escuela != null)
                sql += " AND CVE_ESCUELA = '" + Escuela + "'";

            sql += " AND CVE_SEDE = '" + campusVPDI + "'";

            table.TABLECONDICIONSQL2 = sql;
            table.USUARIO = sesion.pkUser.ToString();
            table.ESCUELA = Escuela;
            table.PERIODO = periodo;
            table.SEDE = campusVPDI;
            table.CAMPUS_INB = campusPA;
            table.PARTEDELPERIODO = partePeriodo;
            table.OPCIONDEPAGO = opcionPago;

            return table.CreateDataTable(sesion);
        }

        public string CreateDataTable2(int show, int pg, string search, string orderby, string sort, SessionDB sesion)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();
            string CheckIcon = "<i class=\"fa fa-check\"></i>";

            table.TABLE = "QPA";// sesion.vdata["TABLE_PA"];

            string[] columnas = { "IdPA", "Docente(NOMBRE)","Docente(Apellido)","Campus", "Escuela", "NRC", "Materia", "Curso"
                    ,"Nombre de Materia","Fecha Inicio","Fecha Fin","Tipo de Curso","Metodo de Instrucción"
                    ,"Status","Inscritos","Parte del periodo","Descripción de parte del periodo","IDSIU"
                    ,"Tipo de Docente","Máximo Grado Académico","Horas Semanales","Horas Programadas"
                    ,"% de Responsabilidad","Horas a Pagar","Opción de Pago","Tabulador","Indicador de sesión","Periodo"};

            string[] campos = { "ID_PA","NOMBRE","APELLIDOS","CAMPUS_INB", "ESCUELA", "NRC",  "MATERIA", "CURSO"
                   ,"NOMBREMATERIA","FECHAINICIAL", "FECHAFINAL","TIPODECURSO","METODODEINSTRUCCION"
                   ,"STATUS","INSCRITOS","PARTEDELPERIODO","PARTEDELPERIODODESC","IDSIU", "TIPODEDOCENTE"
                   ,"MAXIMOGRADOACADEMICO","HORASSEMANALES","HORASPROGRAMADAS","RESPONSABILIDAD","HORASAPAGAR"
                   ,"OPCIONDEPAGO","TABULADOR","INDICADORDESESION","PERIODO","USUARIO"
                   ,"CVE_ESCUELA","CVE_TIPODEDOCENTE","CVE_OPCIONDEPAGO", "ACTUALIZADO" };

            string[] campossearch = { "CAMPUS_INB", "ESCUELA", "MATERIA", "NRC", "CURSO"
                    ,"NOMBREMATERIA","FECHAINICIAL","FECHAFINAL","TIPODECURSO","METODODEINSTRUCCION"
                    ,"STATUS","INSCRITOS","PARTEDELPERIODO","PARTEDELPERIODODESC","IDSIU","APELLIDOS","NOMBRE", "TIPODEDOCENTE"
                    ,"MAXIMOGRADOACADEMICO","HORASSEMANALES","HORASPROGRAMADAS","RESPONSABILIDAD","HORASAPAGAR"
                    ,"OPCIONDEPAGO","TABULADOR","INDICADORDESESION","PERIODO" };

            string[] camposhidden = { "USUARIO", "CVE_ESCUELA", "CVE_TIPODEDOCENTE", "CVE_OPCIONDEPAGO", "ACTUALIZADO" };

            //table.dictColumnFormat["REGISTRADO"] = delegate (string str, ResultSet res) { return str == "True" ? CheckIcon : ""; };

            table.dictColumnFormat.Add("IdPA", delegate (string value, ResultSet res)
            {
                return "<div style=\"width:120px;\">" + value + "</div>";
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

            table.TABLECONDICIONSQL = "USUARIO = " + sesion.pkUser + " AND ACTUALIZADO = 1";
            return table.CreateDataTable(sesion);
        }

        public string CreateDataTable3(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null,
                                       string periodo = "", string opcionPago = "", string campusVPDI = "", string campusPA = "", string Escuela = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "QNominaXCDC";

            string[] columnas = { "IDSIU", "Nombre(s)", "Apellidos", "Periodo", "Contrato", "Origen", "Tipo de pago", "Centro de costos", "Importe", "Sede", "Escuela" };

            string[] campos = { "IDSIU","NOMBRES", "APELLIDOS","PERIODO", "CONTRATO",   "ORIGEN", "TIPODEPAGO", "CENTRODECOSOTOS", "MONTOAPAGAR", "CVE_SEDE","CVE_ESCUELA",
                                "ID_NOMINA", "ID_CENTRODECOSTOS", "ID_PERSONA", "CVE_TIPODEPAGO", "INDICADOR", "ACTUALIZADO" };

            string[] campossearch = { "PERIODO", "CONTRATO", "IDSIU", "NOMBRES", "APELLIDOS", "ORIGEN", "TIPODEPAGO", "CENTRODECOSOTOS", "MONTOAPAGAR", "CVE_SEDE", "CVE_ESCUELA" };

            string[] camposhidden = { "ID_NOMINA", "ID_CENTRODECOSTOS", "ID_PERSONA", "CVE_TIPODEPAGO", "INDICADOR", "ACTUALIZADO" };


            table.addColumnFormat("IDSIU", delegate (string value, ResultSet res) {
                if (res.Get("INDICADOR") == "1")
                {
                    return "<div style='width:180px;'>" + value + "&nbsp;<span style='color:#860000'>nomina</span>&nbsp;&nbsp;&nbsp;<span class='fa fa-check-circle'></span></div>";
                }
                else if (res.Get("INDICADOR") == "2")
                {
                    return "<div style='width:180px;'>" + value + "&nbsp;<span style='color:#860000'>estado de cuenta</span>&nbsp;&nbsp;&nbsp;<span class='fa fa-check-circle-o'></span></div>";
                }
                else return "<div style=\"width:180px;\">" + value + "</div>";
            });

            table.addColumnFormat("NOMBRES", delegate (string value, ResultSet res) {
                return "<div style=\"width:100px;\">" + value + "</div>";
            });

            table.addColumnFormat("APELLIDOS", delegate (string value, ResultSet res) {
                return "<div style=\"width:100px;\">" + value + "</div>";
            });

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;
            table.CAMPOSHIDDEN = camposhidden;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "IDSIU";

            var sql = " ACTUALIZADO = 1 AND PERIODO = '" + periodo + "'";

            if (opcionPago != "" && opcionPago != null)
            {
                switch (opcionPago)
                {
                    case "A":
                        opcionPago = "ADI";
                        break;
                    case "H":
                        opcionPago = "HDI";
                        break;
                    default:
                        opcionPago = "-X1"; //No existe está opción de pago
                        break;
                }
            }

            if (opcionPago != "" && opcionPago != null)
                sql += " AND CVE_TIPODEPAGO = '" + opcionPago + "'";

            if (Escuela != "" && Escuela != null)
                sql += " AND CVE_ESCUELA = '" + Escuela + "'";

            sql += " AND CVE_SEDE = '" + campusVPDI + "'";

            table.TABLECONDICIONSQL = sql;

            table.enabledButtonControls = false;
            table.addBtnActions("Editar", "editarNomina");

            return table.CreateDataTable(sesion);
        }

        [HttpGet]
        public string Consultar(string Periodo, string TipoDeContrato, string TipoPago, string Escuela, string Campus, string CampusPA, string TipoDocente, string PartePeriodo, string Opc)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            TipoDocenteController td = new TipoDocenteController();
            TipoDocente = td.getTipoDocente();

            TiposPagosController tp = new TiposPagosController();
            TipoPago = tp.getTiposPagoV();

            string paURL = ConfigurationManager.AppSettings["xURL"];
            string paUser = ConfigurationManager.AppSettings["xUser"];
            string paSecret = ConfigurationManager.AppSettings["xSecret"];
            string paFormat = ConfigurationManager.AppSettings["xFormat"];

            ConnectUrlToken.ConnectUrlToken con = new ConnectUrlToken.ConnectUrlToken(paURL, paUser, paSecret, paFormat);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str_json = serializer.Serialize(
                new srvDatosProgramacionAcademica
                {
                    periodo      = Periodo,
                    escuela      = Escuela,
                    tipoContrato = TipoDeContrato,
                    tipoDocente  = TipoDocente,
                    campusPA     = CampusPA,
                    campusVPDI   = Campus,
                    tipoPago     = TipoPago,
                    partePeriodo = PartePeriodo,
                });

            Token token = con.getToken();
            int maxDatos = 0;
            int agregados = 0;
            try
            {
                ActualizacionPAModel aux = new ActualizacionPAModel();
                aux.sesion = sesion;
                (aux as IPersona).CleanPersona();
                (aux as IPA).CleanPA();

                sesion.vdata["sesion_periodo"] = Periodo;

                ActualizacionPAModel[] models = con.connectX<ActualizacionPAModel[]>(token, "srvDatosProgramacionAcademica", str_json);
                maxDatos = models.Length;

                agregados = ActualizacionPAModel.Consultar(models, sesion);

                sesion.vdata["TABLE_PA"] = "PA_TMP";
                sesion.saveSession();

                if (models.Length > 0)
                {
                    if (models.Length == agregados)
                        return Notification.Succes("Datos consultados: " + agregados + " / " + maxDatos);
                    else
                        return Notification.Warning("Datos consultados: " + agregados + " / " + maxDatos);
                }
                else
                    return Notification.Warning("No se han encontrado datos con los filtros especificados.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error." + ex.Message);
            }
            //return "-1";// CreateDataTable(10, 1, null, "IDSIU","ASC",sesion);
            return Notification.Error("Ocurrio un error al consultar la informacion. Registros consultados: " + agregados + " / " + maxDatos);
        }

        public class srvDatosProgramacionAcademica
        {
            public string periodo;
            public string escuela;
            public string tipoContrato;
            public string tipoDocente;
            public string campusPA;
            public string campusVPDI;
            public string tipoPago;
            public string partePeriodo;
        }

        public string Generar(string ids)
        {
            PAModel model = new PAModel();
            //if ((model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX)) == null)
            //return string.Empty;

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

        [HttpPost]
        public ActionResult GenerarTodo(ActualizacionPAModel model)
        {
            var mensaje = "";
            var mensaje2 = "";
            var exito = true;
            var exito2 = true;
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            try
            {
                if (model.GenerarTodo(Convert.ToString(sesion.pkUser)))
                {
                    Log.write(this, "GenerarTodo", LOG.EDICION, "Se han importado todos los registros", model.sesion);
                    mensaje = "Los datos se ha actualizado satisfactoriamente.";
                    exito = true;
                }
                else
                {
                    mensaje = "No se ha podido hacer la Generaci&oacute;n completa.";
                    exito = false;
                }

                if (model.EliminaPA(Convert.ToString(sesion.pkUser)))
                {
                    Log.write(this, "EliminaPA", LOG.EDICION, "Se han eliminado los registros", model.sesion);
                    mensaje2 = "Se han eliminado los registros de la Programación Académica en la base de datos de Pago a Profesores";
                    exito2 = true;

                    model.ReInsertaNomina();        // acomodarlo de una mejor manera
                    model.ReInsertaEstadoCuenta();  // acomodarlo de una mejor manera
                }
                else
                {
                    mensaje2 = "No se han podido eliminar los registros de la Programación Académica.";
                    exito2 = false;
                }

                if (exito == true && exito2 == true)
                {
                    return Json(new { msg = Notification.Succes(mensaje + "\n" + mensaje2) });
                }
                else if ((exito == true && exito2 == false) || (exito == false && exito2 == true))
                {
                    return Json(new { msg = Notification.Warning(mensaje + "\n" + mensaje2) });
                }
                else// if (exito == false && exito2 == false)
                {
                    return Json(new { msg = Notification.Error(mensaje + "\n" + mensaje2) });
                }
            } catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult actualizaNomina(ActualizacionPAModel model)
        {

            //if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            //model.sesion = sesion;

            try
            {
                if (model.ActualizaNomina())
                {
                    Log.write(this, "ActualizaNomina: Insertar nomina", LOG.EDICION, "SP", sesion);
                    return Json(new { msg = Notification.Succes("Los datos se ha actualizado satisfactoriamente.") });
                }
                else
                {
                    Log.write(this, "ActualizaNomina: Insertar nomina", LOG.EDICION, "SP", sesion);
                    return Json(new { msg = Notification.Error("No se han podido realizar los cálculos.") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult generarEdoCta(ActualizacionPAModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            Debug.WriteLine("controller ActualizacionPA");

            try
            {
                //if (model.insertaEntregaContratosXEsquemaPago())
                //{
                //    Log.write(this, "(Re) Generación de estado de cuenta: Insertar entrega de contratos", LOG.EDICION, "SQL:" + model.sql, sesion);

                if (model.calculaEstadocuentaXRegistroNomina(sesion.pkUser.ToString()))
                {
                    Log.write(this, "(Re) Generación de estado de cuenta: Cálculo estado de cuenta", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha generado la entrega de contratos y se han realizado los cálculos para el estado de cuenta.") });
                }
                else
                {
                    Log.write(this, "(Re) Generación de estado de cuenta: Cálculo estado de cuenta", LOG.EDICION, "SQL:" + model.sql + ", IdPersona: " + model.IdPersona, sesion);
                    return Json(new { msg = Notification.Error("Se ha realizado el registro de entrega de contratos, pero el cálculo de estado de cuenta no se generó. IdPersona: " + model.IdPersona) });
                }
                //}
                //else
                //{
                //    Log.write(this, "(Re) Generación de estado de cuenta: Insertar entrega de contratos", LOG.EDICION, "SQL:" + model.sql, sesion);
                //    return Json(new { msg = Notification.Error("Hubo un error al generar la entrega de contratos") });
                //}
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {

            List<string> lcaso3Tmp = new List<string>();
            List<string> lcaso3TmpColumn = new List<string>();
            List<string> lcaso3 = new List<string>();
            List<string> lcaso3Column = new List<string>();

            string[] baseColumns = { "K", //"FECHAINICIAL"        1
                                     "L", //"FECHAFINAL"          2
                                     "P", //"INSCRITOS"           3
                                     "U", //"HORASSEMANALES"      4
                                     "V", //"HORASPROGRAMADAS"    5
                                     "W", //"RESPONSABILIDAD"     6
                                     "X", //"HORASAPAGAR"         7
                                     "Y", //"OPCIONDEPAGO"        8
                                     "Z", //"TABULADOR"           9
                                     "AA"}; //"INDICADORDESESION" 10

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Estatus", typeof(string));
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Docente (Nombre)", typeof(string));
                tbl.Columns.Add("Docente (Apellido)", typeof(string));
                tbl.Columns.Add("Campus", typeof(string));
                tbl.Columns.Add("Escuela", typeof(string));
                tbl.Columns.Add("NRC", typeof(string));
                tbl.Columns.Add("Materia", typeof(string));
                tbl.Columns.Add("Curso", typeof(string));
                tbl.Columns.Add("Nombre de la materia", typeof(string));
                tbl.Columns.Add("Fecha de inicio", typeof(string));
                tbl.Columns.Add("Fecha de fin", typeof(string));
                tbl.Columns.Add("Tipo de curso", typeof(string));
                tbl.Columns.Add("Método de instrucción", typeof(string));
                tbl.Columns.Add("Status", typeof(string));
                tbl.Columns.Add("Inscritos", typeof(string));
                tbl.Columns.Add("Parte del periodo", typeof(string));
                tbl.Columns.Add("Descripción de parte del periodo", typeof(string));
                tbl.Columns.Add("Tipo de docente", typeof(string));
                tbl.Columns.Add("Máximo grado académico", typeof(string));
                tbl.Columns.Add("Horas semanales", typeof(string));
                tbl.Columns.Add("Horas programadas", typeof(string));
                tbl.Columns.Add("% de responsabilidad", typeof(string));
                tbl.Columns.Add("Horas a pagar", typeof(string));
                tbl.Columns.Add("Opción de pago", typeof(string));
                tbl.Columns.Add("Tabulador", typeof(string));
                tbl.Columns.Add("Indicador de sesión", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM PAEXCEL_TMP WHERE USUARIO = " + sesion.pkUser + " ORDER BY ID_PAEXCEL");

                int row = 2;
                string[] columns;
                while (res.Next())
                {

                    if (res.Get("TIPOACTUALIZACION") == "3" && res.Get("ESTATUS").Length > 0)
                    {
                        getRowsTableExcel(tbl, res);
                        lcaso3Tmp.Add("A" + row + ":AB" + row);

                        if (res.Get("CAMPOSMODIFICADOS").Length > 0)
                        {
                            columns = Utils.splittingX(res.Get("CAMPOSMODIFICADOS"), ',');
                            foreach (var cx in columns)
                            {
                                if (cx != "")
                                {
                                    for (int i = 0; i <= baseColumns.Length - 1; i++)
                                    {
                                        if (Int16.Parse(cx) == (i + 1))
                                        {
                                            lcaso3TmpColumn.Add(baseColumns[i] + row + ":" + baseColumns[i] + row);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (res.Get("TIPOACTUALIZACION") == "3" && res.Get("ESTATUS").Length == 0)
                    {
                        getRowsTableExcel(tbl, res);
                        lcaso3.Add("A" + row + ":AB" + row);

                        if (res.Get("CAMPOSMODIFICADOS").Length > 0)
                        {
                            columns = Utils.splittingX(res.Get("CAMPOSMODIFICADOS"), ',');
                            foreach (var cx in columns)
                            {
                                if (cx != "")
                                {
                                    for (int i = 0; i <= baseColumns.Length - 1; i++)
                                    {
                                        if (Int16.Parse(cx) == (i + 1))
                                        {
                                            lcaso3Column.Add(baseColumns[i] + row + ":" + baseColumns[i] + row);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        getRowsTableExcel(tbl, res);
                    }

                    row++;
                }

                //while (res.Next())
                //{
                //    getRowsTableExcel(tbl, res);
                //}

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Actualización de la PA");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:AB1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:AB1"])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    foreach (var x in lcaso3Tmp)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(253, 246, 146));
                            rngX.Style.Font.Color.SetColor(Color.Black);
                        }
                    }

                    foreach (var x in lcaso3TmpColumn)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(161, 255, 154));
                            rngX.Style.Font.Color.SetColor(Color.Black);
                        }
                    }

                    foreach (var x in lcaso3)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(244, 255, 172));
                            rngX.Style.Font.Color.SetColor(Color.Black);
                        }
                    }

                    foreach (var x in lcaso3Column)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 193, 193));
                            rngX.Style.Font.Color.SetColor(Color.Black);
                        }
                    }

                    //Example how to Format Column 1 as numeric 
                    using (ExcelRange col = ws.Cells[2, 1, 2 + tbl.Rows.Count, 1])
                    {
                        col.Style.Numberformat.Format = "#,##0.00";
                        col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    //Write it back to the client
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=Actualiza_PA.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Actualización de programación Academica", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Actualización de programación Academica" + e.Message, sesion);
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

                // sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        public void getRowsTableExcel(System.Data.DataTable tbl, ResultSet res)
        {
            tbl.Rows.Add(res.Get("ESTATUS"), res.Get("IDSIU"), res.Get("NOMBRE"), res.Get("APELLIDOS"), res.Get("CAMPUS_INB")
                , res.Get("ESCUELA"), res.Get("NRC"), res.Get("MATERIA"), res.Get("CURSO"), res.Get("NOMBREMATERIA")
                , res.Get("FECHAINICIAL"), res.Get("FECHAFINAL"), res.Get("TIPODECURSO"), res.Get("METODODEINSTRUCCION")
                , res.Get("STATUS"), res.Get("INSCRITOS"), res.Get("PARTEDELPERIODO"), res.Get("PARTEDELPERIODODESC")
                , res.Get("TIPODEDOCENTE"), res.Get("MAXIMOGRADOACADEMICO"), res.Get("HORASSEMANALES")
                , res.Get("HORASPROGRAMADAS"), res.Get("RESPONSABILIDAD"), res.Get("HORASAPAGAR")
                , res.Get("OPCIONDEPAGO"), res.Get("TABULADOR"), res.Get("INDICADORDESESION")
                , res.Get("PERIODO"));
        }
    }
}