using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.Pagos;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Text;
using System;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using PagoProfesores.Controllers.Helper;

namespace PagoProfesores.Controllers.Pagos
{
    public class NominaController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public NominaController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/Nomina/nomina.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10099,  Element = "Controller" }, //PERMISO ACCESO 
                 new Factory.Privileges { Permiso = 10100,  Element = "formbtnconsultar" }, //PERMISO DETALLE 
                 new Factory.Privileges { Permiso = 10101,  Element = "formbtntabulador" }, //PERMISO 
                 new Factory.Privileges { Permiso = 10102,  Element = "formbtnasignar" }, //PERMISO 
                 new Factory.Privileges { Permiso = 10103,  Element = "formbtnasignarCentroC" }, //PERMISO 
                 new Factory.Privileges { Permiso = 10104,  Element = "formbtnGenerarCentroC" },// 
                 new Factory.Privileges { Permiso = 10105,  Element = "formbtnaCalcularNomina" },
            };
        }
        // GET: Nomina
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Pagos", "Calculo de Pagos PA", sesion);//Calculo de Nómina PA

            ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion);
            ViewBag.COMBO_CAMPUSPA = getCampusPA();

            //ATRIBUTOS**************************************
            ViewBag.COMBO_Atributos = getAtributos();
            //FIN ATRIBUTOS**************************************

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);
           

            Log.write(this, "Nomina Start", LOG.CONSULTA, "Ingresa Pantalla Nomina", sesion);

            return View(Factory.View.Access + "Pagos/Nomina/Start.cshtml");
        }
        
        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null,
                                       string periodo = "", string partePeriodo = "", string escuela = "", string opcionPago = "", string campusVPDI = "", string campusPA = "",string Atributos = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();
            //string CheckIcon = "<i class=\"fa fa-check\"></i>";

            table.TABLE = "QPA";

            string[] columnas = { "IDSIU",  "Nombre","Apellido","Procesado", "ESQUEMA DE PAGO", "CENTRO DE COSTO", "Campus", "Sede", "MONTO A PAGAR",
                                  "ID PA", "PERIODO", "Cve Opcion Pago", "CONTRATO", "No. SEMANAS", "Tipo de docente", "NRC",
                                  "Tabulador", "Horas a pagar", "Escuela", "Materia", "Curso", "Nombre de la materia",
                                  "Fecha inicio", "Fecha fin", "Tipo de curso", "Método de instrucción", "Status",
                                  "Inscritos", "Parte del periodo", "Máximo grado académico", "Horas semanales",
                                  "Horas programadas", "% de responsabilidad", "Indicador de sesión","Atributos"};

            string[] campos = {  "IDSIU", "NOMBRE","APELLIDOS","CONESQUEMA","ESQUEMADEPAGO", "ID_CENTRODECOSTOS", "CENTRODECOSTOS", "CAMPUS_INB", "CVE_SEDE", "MONTOAPAGAR", "ID_PA", "PERIODO",
                                "CVE_OPCIONDEPAGO", "OPCIONDEPAGO", "NOSEMANAS", "CVE_TIPODEDOCENTE", "TIPODEDOCENTE",
                                "NRC", "TABULADOR", "HORASAPAGAR", "CVE_ESCUELA", "ESCUELA", "MATERIA", "CURSO", "NOMBREMATERIA",
                                "FECHAINICIAL", "FECHAFINAL", "TIPODECURSO", "METODODEINSTRUCCION", "STATUS", "INSCRITOS",
                                "PARTEDELPERIODO", "MAXIMOGRADOACADEMICO", "HORASSEMANALES", "HORASPROGRAMADAS", "RESPONSABILIDAD",
                                "INDICADORDESESION", "INDICADOR","ATRIBUTOSPROGRAMAGRADO"};

            string[] campossearch = { "IDSIU", "CAMPUS_INB", "CVE_SEDE", "APELLIDOS", "NOMBRE", "MONTOAPAGAR", "ID_PA", "PERIODO",
                                      "CVE_OPCIONDEPAGO", "OPCIONDEPAGO", "NOSEMANAS", "TIPODEDOCENTE", "NRC", "TABULADOR", "HORASAPAGAR", "ESCUELA",
                                      "MATERIA", "CURSO", "NOMBREMATERIA", "FECHAINICIAL", "FECHAFINAL", "TIPODECURSO",
                                      "METODODEINSTRUCCION", "STATUS", "INSCRITOS", "PARTEDELPERIODO", "MAXIMOGRADOACADEMICO",
                                      "HORASSEMANALES", "HORASPROGRAMADAS", "RESPONSABILIDAD", "INDICADORDESESION", "ESQUEMADEPAGO", "CENTRODECOSTOS","ATRIBUTOSPROGRAMAGRADO"};

            string[] camposhidden = { "CVE_TIPODEDOCENTE", "CVE_ESCUELA", "ID_CENTRODECOSTOS", "INDICADOR" };


            table.addColumnFormat("IDSIU", delegate (string value, ResultSet res) {
                if (res.Get("INDICADOR") == "1")
                {
                    return "<div style='width:120px;'>" + value + "&nbsp;<span style='color:#860000'>nómina</span>&nbsp;&nbsp;&nbsp;<span class='fa fa-check-circle'></span></div>";
                }
                else
                {
                    return "<div style=\"width:120px;\">" + value + "</div>";
                }
            });

            table.addColumnFormat("NOMBRE", delegate (string value, ResultSet res) {
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
            table.field_id = "ID_PA";

            table.enabledCheckbox = true;

            TipoDocenteController td = new TipoDocenteController();
            string TipoDocente = td.getTipoDocente();

            var sql = " CVE_TIPODEDOCENTE in ('" + TipoDocente.Replace(",","','")  + "') AND PERIODO = '" + periodo + "'"; //***Atención.- Se ha forzado a presentar sólo a aquellos registros que tengan como Clave de Tipo de Docente el valor HON.

            if (partePeriodo != "" && partePeriodo != null)
                sql += " AND PARTEDELPERIODO = '" + partePeriodo + "'";

            if (escuela != "" && escuela != null)
                sql += " AND CVE_ESCUELA = '" + escuela + "'";

            if (opcionPago != "" && opcionPago != null)
                sql += " AND CVE_OPCIONDEPAGO = '" + opcionPago + "'";


            if (Atributos != "" && Atributos != null)
                sql += " AND ATRIBUTOSPROGRAMAGRADO LIKE  '%" + Atributos + "%'";

            sql += " AND CVE_SEDE = '" + campusVPDI + "' AND CAMPUS_INB = '" + campusPA + "'";

            table.TABLECONDICIONSQL = sql;

            table.enabledButtonControls = false;
            table.addBtnActions("Editar", "editarNomina");


            table.addColumnFormat("ESQUEMADEPAGO", delegate (string campo, ResultSet res) {
                string result = "<div style=\"width:250px; \">"+ campo + "</div>";
                return result;
            });

            table.addColumnFormat("CENTRODECOSTOS", delegate (string campo, ResultSet res) {
                string result = "<div style=\"width:250px; \">" + campo + "</div>";
                return result;
            });

            table.addHeaderClass("ESQUEMA DE PAGO", "header_mark");
            table.addHeaderClass("CENTRO DE COSTO", "header_mark");
            table.addHeaderClass("MONTO A PAGAR", "header_mark");
            table.addHeaderClass("ID PA", "header_mark");
            table.addHeaderClass("PERIODO", "header_mark");
            table.addHeaderClass("CONTRATO", "header_mark");
            table.addHeaderClass("No. SEMANAS", "header_mark");


            return table.CreateDataTable(sesion);
        }

        // POST: Nomina/Edit/
        [HttpPost]
        public ActionResult Edit(NominaModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

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
        
        [HttpGet]
        public string getEsquemaPago(string Periodo, string idEsquema = "")
        {
            NominaModel model = new NominaModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            string selected = "";

            model.Periodo = Periodo;

            StringBuilder sb = new StringBuilder();
            sb.Append("<option></option>");

            foreach (KeyValuePair<string, string> pair in model.getEsquemaPago())
            {
                selected = (idEsquema == pair.Key) ? "selected" : "";            
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        // POST: /Edit
        [HttpPost]
        public ActionResult getDetalleEsquemaPago(NominaModel model)
        {
            Debug.WriteLine("controller getDetalleEsquemaPago");

            if (model.getDetalleEsquemaPago())
            {
               // Debug.WriteLine("controller edit2");
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }


        [HttpPost]
        public ActionResult insertaNomina(NominaModel model)
        {
            Debug.WriteLine("controller insertaNomina");

            try
            {
                if (model.insertaNominaXCentrodeCosto())
                {
                    Log.write(this, "Calculo Nomina: Insertar nomina", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha generado la nomina.") });
                }
                else
                {
                    Log.write(this, "Calculo Nomina: Insertar nomina", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al generar la nomina") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }

        }

        [HttpPost]
        public ActionResult asginaEsquemaPago(NominaModel model)
        {
            Debug.WriteLine("controller asginaEsquemaPago");

            try
            {
                 if (model.asginaEsquemadePago())
                {
                    Log.write(this, "Calculo Nomina: Asignar Esquema", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha asignado el Esquema a la programación Academica seleccionada con exito" ) });
                }
                else
                {
                    Log.write(this, "Calculo Nomina: Asignar Esquema", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al asiganar el esquema") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        [HttpPost]
        public ActionResult asginaEsquemaPagoTodo(NominaModel model)
        {
            Debug.WriteLine("controller asginaEsquemaPagoTodo");

            try
            {
                if (model.asginaEsquemaPago())
                {
                    Log.write(this, "Calculo Nomina: Asignar Esquema - TODO", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha asignado el Esquema a la programación Academica seleccionada con exito") });
                }
                else
                {
                    Log.write(this, "Calculo Nomina: Asignar Esquema - TODO", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al asiganar el esquema") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        // POST: DatosPersonas/Edit/5
        [HttpPost]
        public ActionResult getCentroCostos(NominaModel model)
        {
            if (model.getCentroCostos())
            {
                return Json(new JavaScriptSerializer().Serialize(model.CentroCostos));
            }
            else
            {
                return Json(new { msg = Notification.Error(" Error al asiganar el esquema") });
            }
        }

        [HttpPost]
        public ActionResult asginaCentrodeCostos(NominaModel model)
        {
            Debug.WriteLine("controller asginaEsquemaPago");

            try
            {
                if (model.asginaCentrodeCostos())
                {
                    Log.write(this, "Calculo Nomina: asginaCentroCostos", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha asignado el Centro de Costos  a la programación Academica seleccionada con exito") });
                }
                else
                {
                    Log.write(this, "Calculo Nomina: asginaCentroCostos", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al asignar el Centro de Costos") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult validaAsignacion(NominaModel model)
        {

            //Dictionary<string, PersonaCC> dictCC = new Dictionary<string, PersonaCC>();
            List<PersonaCC> lPersonasCC = new List<PersonaCC>();
            lPersonasCC = model.validaAsignacion();

            return Json(new { lPersonasCC });
        }

        /*  [HttpPost]
          public ActionResult asginaEsquemaPago(NominaModel model) {
              Debug.WriteLine("controller asginaEsquemaPago");

              if (model.asginaEsquemaPago())
              {
                  if (model.asginaCentroCostos())
                  {
                      if (model.insertaNominaXCentrodeCosto())
                      {
                          if (model.insertaEntregaContratosXEsquemaPago())
                          {
                              return Json(new JavaScriptSerializer().Serialize(model));
                          }
                          return View();
                      }
                      return View();
                  }
                  return View();
              }
              return View();
          }*/

        [HttpPost]
        public ActionResult grabarCentroCostos(NominaModel model)
        {
            Debug.WriteLine("controller grabarCentroCostos");

            if (model.grabaCentroCosto())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        [HttpPost]
        public ActionResult generarCentroCostos(NominaModel model)
        {
            Debug.WriteLine("controller generarCentroCostos");

            try
            {
                if (model.generaCentroCostos())
                {
                    Log.write(this, "Calculo Nomina: generarCentroCostos en automático", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha asignado el 'Centro de Costos' a la Programación Academica según los filtros seleccionados.") });
                }
                else
                {
                    Log.write(this, "Calculo Nomina: generarCentroCostos en automático", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al generar el 'Centro de Costos' en automático.") });
                }
            }
            catch (Exception eGCC)
            {
                return Json(new { msg = Notification.Error(eGCC.Message) });
            }
        }

        [HttpPost]
        public ActionResult cambiaTabulador(NominaModel model)
        {
            Debug.WriteLine("controller Nomina: cambiaTabulador");

            try
            {
                if (model.cambiaTabulador())
                {
                    Log.write(this, "Cálculo Nomina: Cambiar Tabulador por selección", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha cambiado el Tabulador a los registros seleccionados de la programación Academica de manera exitosa.") });
                }
                else
                {
                    Log.write(this, "Cálculo Nomina: Cambiar Tabulador por selección", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al intentar cambiar el Tabulador.") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        [HttpPost]
        public ActionResult cambiaTabuladorTodo(NominaModel model)
        {
            Debug.WriteLine("controller Nomina: cambiaTabuladorTodo");

            try
            {
                if (model.cambiaTabuladorTodo())
                {
                    Log.write(this, "Cálculo Nomina: Cambiar Tabulador a todo de acuerdo a los filtros dados.", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha cambiado el Tabulador a la programación Academica de manera exitosa.") });
                }
                else
                {
                    Log.write(this, "Cálculo Nomina: Cambiar Tabulador de acuerdo a los filtros dados.", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al intentar cambiar el Tabulador.") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        public void ExportExcel()
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {

                System.Data.DataTable tbl = new System.Data.DataTable();
                //   tbl.Columns.Add("Registro", typeof(string));
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Nombre", typeof(string));
                tbl.Columns.Add("Apellido", typeof(string));
                tbl.Columns.Add("Procesado", typeof(string));
                tbl.Columns.Add("ESQUEMA DE PAGO", typeof(string));
                tbl.Columns.Add("CENTRO DE COSTO", typeof(string));
                tbl.Columns.Add("Campus", typeof(string));
                tbl.Columns.Add("MONTO A PAGAR", typeof(string));
                tbl.Columns.Add("ID PA", typeof(string));
                tbl.Columns.Add("PERIODO", typeof(string));
                tbl.Columns.Add("Cve Opcion Pago", typeof(string));
                tbl.Columns.Add("CONTRATO", typeof(string));
                tbl.Columns.Add("No. SEMANAS", typeof(string));
                tbl.Columns.Add("Tipo de docente", typeof(string));
                tbl.Columns.Add("NRC", typeof(string));
                tbl.Columns.Add("Tabulador", typeof(string));
                tbl.Columns.Add("Horas a pagar", typeof(string));
                tbl.Columns.Add("Escuela", typeof(string));
                tbl.Columns.Add("Materia", typeof(string));
                tbl.Columns.Add("Curso", typeof(string));
                tbl.Columns.Add("Nombre de la materia", typeof(string));
                tbl.Columns.Add("Fecha inicio", typeof(string));
                tbl.Columns.Add("Fecha fin", typeof(string));
                tbl.Columns.Add("Tipo de curso", typeof(string));
                tbl.Columns.Add("Método de instrucción", typeof(string));
                tbl.Columns.Add("Status", typeof(string));
                tbl.Columns.Add("Inscritos", typeof(string));
                tbl.Columns.Add("Parte del periodo", typeof(string));
                tbl.Columns.Add("Máximo grado académico", typeof(string));
                tbl.Columns.Add("Horas semanales", typeof(string));
                tbl.Columns.Add("Horas programadas", typeof(string));
                tbl.Columns.Add("% de responsabilidad", typeof(string));
                tbl.Columns.Add("Indicador de sesión", typeof(string));


                string periodo = Request.Params["Periodo"];
                string partePeriodo = Request.Params["PartePeriodo"];
                string opcionPago = Request.Params["TipoPago"];
                string escuela = Request.Params["Escuela"];
                string campusVPDI = Request.Params["Campus"];
                string campusPA = Request.Params["CampusPA"];


                var sql2 = "";

                if (partePeriodo != "" && partePeriodo != null)
                    sql2 += " AND PARTEDELPERIODO = '" + partePeriodo + "'";

                //if (nivel != "" && nivel != null)
                //    sql += " AND CVE_NIVEL = '" + nivel + "'";

                if (escuela != "" && escuela != null)
                    sql2 += " AND CVE_ESCUELA = '" + escuela + "'";

                if (opcionPago != "" && opcionPago != null)
                    sql2 += " AND CVE_OPCIONDEPAGO = '" + opcionPago + "'";

                sql2 += " AND CVE_SEDE = '" + campusVPDI + "' AND CAMPUS_INB = '" + campusPA + "'";


                //ResultSet res = db.getTable("SELECT * FROM QPA WHERE  CVE_TIPODEDOCENTE = 'HON' AND PERIODO = '" + periodo + "' " + sql2);
                /*Atención.- Se ha forzado a presentar sólo a aquellos registros que tengan como Clave de Tipo de Docente el valor HON.*/
                ResultSet res = db.getTable("SELECT * FROM QPA WHERE PERIODO = '" + periodo + "' " + sql2);

                while (res.Next())
                {
                    // registro = model.existExcel(false, res.Get("NRC"), res.Get("PERIODO"), res.Get("IDSIU"), res.Get("CAMPUS_INB")) ? "Sí" : "No";

                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("IDSIU"), res.Get("NOMBRE"), res.Get("APELLIDOS"), res.Get("CONESQUEMA")
                       , res.Get("ESQUEMADEPAGO"), res.Get("CENTRODECOSTOS"), res.Get("CAMPUS_INB")
                       , res.Get("MONTOAPAGAR"), res.Get("ID_PA"), res.Get("PERIODO")
                       , res.Get("CVE_OPCIONDEPAGO"), res.Get("OPCIONDEPAGO"), res.Get("NOSEMANAS")
                       , res.Get("TIPODEDOCENTE"), res.Get("NRC"), res.Get("TABULADOR"), res.Get("HORASAPAGAR")
                       , res.Get("ESCUELA"), res.Get("MATERIA")
                       , res.Get("CURSO"), res.Get("NOMBREMATERIA"), res.Get("FECHAINICIAL")
                       , res.Get("FECHAFINAL"), res.Get("TIPODECURSO"), res.Get("METODODEINSTRUCCION"), res.Get("STATUS")
                       , res.Get("INSCRITOS"), res.Get("PARTEDELPERIODO"), res.Get("MAXIMOGRADOACADEMICO")
                       , res.Get("HORASSEMANALES"), res.Get("HORASPROGRAMADAS"), res.Get("RESPONSABILIDAD"), res.Get("INDICADORDESESION")

                       );
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Listado de Nomina");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    // ws.Cells["A1:Z1"].AutoFitColumns();
                    //   ws.Column(1).Width = 20;
                    //  ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:AG1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Nomina_PA.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Nomina", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Nomina de PA" + e.Message, sesion);
            }
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