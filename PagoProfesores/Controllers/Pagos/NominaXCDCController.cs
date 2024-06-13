using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Pagos
{
    public class NominaXCDCController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public NominaXCDCController ()
        {
            db = new database();

            string[] scripts = { "js/Pagos/NominaXCDC/NominaXCDC.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10095,  Element = "Controller" }, //PERMISO ACCESO 
                 new Factory.Privileges { Permiso = 10098,  Element = "frm-nomina" }, //PERMISO DETALLE 
                 new Factory.Privileges { Permiso = 10096,  Element = "formbtnconsultar" }, //PERMISO 
                 new Factory.Privileges { Permiso = 10097,  Element = "formbtngenerarEstadoC" }, //PERMISO 
            };
        }

        // GET: NominaXCDC
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu(7, 48, sesion);
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


            Log.write(this, "NominaXCDC Start", LOG.CONSULTA, "Ingresa Pantalla NominaXCDC", sesion);

            return View(Factory.View.Access + "Pagos/NominaXCDC/Start.cshtml");
        }

        [HttpPost]
        public ActionResult generarEdoCta(NominaXCDCModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            Debug.WriteLine("controller NominaXCDC");

            try
            {
                if (model.insertaEntregaContratosXEsquemaPago())
                {
                    Log.write(this, "Calculo Nomina: Insertar entrega de contratos", LOG.EDICION, "SQL:" + model.sql, sesion);

                    //if (model.calculaEstadocuentaXEsquemaPago())
                    if (model.calculaEstadocuentaXRegistroNomina(sesion.nickName))
                    {
                        Log.write(this, "Calculo Nomina: Cálculo estado de cuenta", LOG.EDICION, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Succes("Se ha generado la entrega de contratos y se han realizado los cálculos para el estado de cuenta.") });
                    }
                    else
                    {
                        Log.write(this, "Calculo Nomina: Cálculo estado de cuenta", LOG.EDICION, "SQL:" + model.sql + ", IdPersona: " + model.PersonaID, sesion);
                        return Json(new { msg = Notification.Error("Se ha realizado el registro de entrega de contratos, pero el cálculo de estado de cuenta no se generó. IdPersona: " + model.PersonaID) });
                    }
                }
                else
                {
                    Log.write(this, "Calculo Nomina: Insertar entrega de contratos", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Hubo un error al generar la entrega de contratos") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
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

        public string getAtributos()
        {
            PAModel model = new PAModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "";
            string IdAtributo = "";

            foreach (KeyValuePair<string, string> pair in model.getAtributos())
            {
                selected = (IdAtributo == pair.Key) ? "selected" : "";

                // sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        //[HttpGet]
        //public string getCampusPA()
        //{
        //    NominaModel model = new NominaModel();

        //    SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
        //    if (sesion == null)
        //        return "";

        //    StringBuilder sb = new StringBuilder();
        //    foreach (KeyValuePair<string, string> pair in model.getCampusPA())
        //    {
        //        sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
        //    }
        //    return sb.ToString();
        //}

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null,
                                       string periodo = "", string partePeriodo = "", string opcionPago = "", string campusVPDI = "", string campusPA = "", string Escuela = "",string Atributos ="")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();
            //string CheckIcon = "<i class=\"fa fa-check\"></i>";

            table.TABLE = "QNominaXCDC";

            string[] columnas = { "IDSIU", "Nombre(s)", "Apellidos", "Periodo", "Parte del periodo", "Contrato",  "Origen", "Tipo de pago", "Centro de costos", "Importe", "Sede","Escuela","Atributos"};

            string[] campos = { "IDSIU","NOMBRES", "APELLIDOS","PERIODO", "PARTEDELPERIODO", "CONTRATO",   "ORIGEN", "TIPODEPAGO", "CENTRODECOSOTOS", "MONTOAPAGAR", "CVE_SEDE","CVE_ESCUELA",
                                "ID_NOMINA", "ID_CENTRODECOSTOS", "ID_PERSONA", "CVE_TIPODEPAGO", "INDICADOR","ATRIBUTOSPROGRAMAGRADO" };

            string[] campossearch = { "PERIODO", "PARTEDELPERIODO", "CONTRATO", "IDSIU", "NOMBRES", "APELLIDOS", "ORIGEN", "TIPODEPAGO", "CENTRODECOSOTOS", "MONTOAPAGAR", "CVE_SEDE", "CVE_ESCUELA", "ATRIBUTOSPROGRAMAGRADO" };

            string[] camposhidden = { "ID_NOMINA", "ID_CENTRODECOSTOS", "ID_PERSONA", "CVE_TIPODEPAGO", "INDICADOR" };


            table.addColumnFormat("IDSIU", delegate (string value, ResultSet res) {
                if (res.Get("INDICADOR") == "1")
                {
                    return "<div style='width:180px;'>" + value + "&nbsp;<span style='color:#860000'>nómina</span>&nbsp;&nbsp;&nbsp;<span class='fa fa-check-circle'></span></div>";
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

            var sql = " PERIODO = '" + periodo + "'";

            if (partePeriodo != "" && partePeriodo != null)
                sql += " AND PARTEDELPERIODO = '" + partePeriodo + "'";

            //if (opcionPago != "" && opcionPago != null) ** DUDA =)
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

            if (opcionPago != "" && opcionPago != null)
                sql += " AND CVE_TIPODEPAGO = '" + opcionPago + "'";

            if (Escuela != "" && Escuela != null)
                sql += " AND CVE_ESCUELA = '" + Escuela + "'";

            if (Atributos != "" && Atributos != null)
                sql += " AND ATRIBUTOSPROGRAMAGRADO like '%" + Atributos + "%'";

            sql += " AND CVE_SEDE = '" + campusVPDI + "'";

            table.TABLECONDICIONSQL = sql;

            //table.enabledCheckbox = true;
            table.enabledButtonControls = false;
            table.addBtnActions("Editar", "editarNomina");

            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        public void ExportExcel(string periodo = "", string nivel = "", string opcionPago = "", string campusVPDI = "")
        {
            NominaXCDCModel model = new NominaXCDCModel();

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Periodo", typeof(string));
                //tbl.Columns.Add("Nivel", typeof(string));
                tbl.Columns.Add("Contrato", typeof(string));
                tbl.Columns.Add("ID", typeof(string));
                tbl.Columns.Add("Nombre(s)", typeof(string));
                tbl.Columns.Add("Apellidos", typeof(string));
                tbl.Columns.Add("Origen", typeof(string));
                tbl.Columns.Add("Tipo de pago", typeof(string));
                tbl.Columns.Add("Centro de costos", typeof(string));
                tbl.Columns.Add("Importe", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));

                var sql = " PERIODO = '" + periodo + "'";

                //if (nivel != "" && nivel != null)
                //    sql += " AND CVE_NIVEL = '" + nivel + "'";

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

                sql += " AND CVE_SEDE = '" + campusVPDI + "'";

                ResultSet res = db.getTable("SELECT * FROM QNominaXCDC WHERE " + sql + " ORDER BY IDSIU");
                //ResultSet res = db.getTable("SELECT * FROM QNominaXCDC ORDER BY IDSIU");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("PERIODO"), /*res.Get("CVE_NIVEL"),*/ res.Get("CONTRATO"), res.Get("IDSIU")
                        , res.Get("NOMBRES"), res.Get("APELLIDOS"), res.Get("ORIGEN"), res.Get("TIPODEPAGO")
                        , res.Get("CENTRODECOSOTOS"), res.Get("MONTOAPAGAR"), res.Get("CVE_SEDE"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Nomina por centro de costos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:K1"].AutoFitColumns();
                    //   ws.Column(1).Width = 20;
                    //  ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:K1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=ImportarNominaXCDC.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta nomina por centro de costos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta nomina por centro de costos" + e.Message, sesion);
            }
        }
    }
}