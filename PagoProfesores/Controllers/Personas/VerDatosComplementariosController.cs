using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.Personas;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;
using PagoProfesores.Models.Pagos;
using ConnectUrlToken;
using System.Configuration;


namespace PagoProfesores.Controllers.Personas
{
    public class VerDatosComplementariosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public VerDatosComplementariosController()
        {
            db = new database();
            Scripts.SCRIPTS = new string[] {
                "js/Personas/VerDatosComplementarios.js"
            };

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10079,  Element = "Controller" }, //PERMISO ACCESO DatosPersonas
                 new Factory.Privileges { Permiso = 10080,  Element = "frm-datospersonas" }, //PERMISO DETALLE DatosPersonas
                 
            };
        }

        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content(""); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            //ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Personas", "Importar PA Multicampus", sesion);

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            sesion.vdata["TABLE_PERSONAS"] = "QPowerBI";
            sesion.saveSession();

            ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSUI", "ASC");

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

        

            Log.write(this, "DatosPersonas Start", LOG.CONSULTA, "Ingresa Pantalla DatosPersonas", sesion);

            return View(Factory.View.Access + "Personas/VerDatosComplementarios/Start.cshtml");
        }

        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", string filterC = "", string filterP = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "QPowerBI";
            table.COLUMNAS = new string[] { "IDBANNER", "Docente","RFC", "CURP", "Campus","Periodo","Escuela","Descripción de la escuela","NRC", "Sección","Materia","Curso","Nombre de la Materia","Tipo de curso","Tipo de horario","Método de instrucción", "Estatus","Parte del periodo", "Fecha parte del periodo","Lista cruzada", "Máximo grado académico","Tipo docente", "Profesor titular", "Admin importó","Admin modificó" };
            table.CAMPOS = new string[] { "IDSUI", "DOCENTE", "RFC", "CURP", "CAMPUS", "PERIODO", "ESCUELA", "DESCRIPCION_ESCUELA","NRC", "SECCION", "MATERIA", "CURSO", "NOMBRE_MATERIA", "TIPO_CURSO", "TIPO_HORARIO", "METODO_INSTRUCCION", "ESTATUS", "PARTE_PERIODO", "FECHA_PARTE_PERIODO", "LISTA_CRUZADA", "MAXIMO_GRADO_ACADEMICO", "TIPO_DOCENTE", "PROFESOR_TITULAR", "ADMINIMPORT", "ADMINUPDATE" };
            table.CAMPOSSEARCH = new string[] { "IDSUI", "DOCENTE" };
           

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "IDSUI";
            table.TABLECONDICIONSQL = "(USUARIO_IMPORTA=" + sesion.pkUser + " OR USUARIO_ACTUALIZA=" + sesion.pkUser + ")";

            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }

        [HttpPost]
        public ActionResult BuscaPersona(VerDatosComplementariosModel model)
        {

            
            if (model.BuscaPersona())
            {
                Log.write(this, "Controller: VerDatosComplementarios  - BuscaPersona", LOG.CONSULTA, "SQL:" + model.sql, sesion);
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            else
            {
                model.msg = "No se encuentra el ID ingresado";
                Log.write(this, "Controller: VerDatosComplementarios - BuscaPersona", LOG.ERROR, "SQL:" + model.sql, sesion);
                return Json(new JavaScriptSerializer().Serialize(model));
            }
        }

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
    
                ResultSet res = db.getTable("SELECT * FROM QPowerBI WHERE USUARIO_IMPORTA=" + sesion.pkUser.ToString());
                while (res.Next())
                {
                    // Here we add five DataRows.
                
                    tbl.Rows.Add(res.Get("IDSUI"), res.Get("PERIODO"), res.Get("CAMPUS"), res.Get("DOCENTE"), res.Get("RFC"),res.Get("CURP"), res.Get("MAXIMO_GRADO_ACADEMICO"), res.Get("TIPO_DOCENTE"), res.Get("ESTATUS"), res.Get("NRC"), res.Get("HORAS_PROGRAMADAS"), res.Get("ESCUELA"), res.Get("DESCRIPCION_ESCUELA"), res.Get("MATERIA"), res.Get("NOMBRE_MATERIA"),res.Get("CURSO"), res.Get("TIPO_CURSO"), res.Get("SECCION"), res.Get("TIPO_HORARIO"), res.Get("METODO_INSTRUCCION"),res.Get("LISTA_CRUZADA"), res.Get("PARTE_PERIODO"), res.Get("FECHA_PARTE_PERIODO"), res.Get("TABULADOR_LC"), res.Get("TABULADOR_POS"),res.Get("TABULADOR_DI"), res.Get("PROFESOR_TITULAR"), res.Get("PORCENTAJE_RESPONSABILIDAD"), res.Get("NUMERO_INSCRITOS"), res.Get("ACTIVIDADES_NO_DOCENCIA"),res.Get("FECHA_INICIO"), res.Get("FECHA_FIN"));
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

        public class ServiceDatosProfesores
        {
            public string periodo;
            public string campusVPDI;
            public string idSiu;
        }
    }
}