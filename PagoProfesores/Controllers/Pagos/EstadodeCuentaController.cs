using System;
using System.Web.Mvc;
using PagoProfesores.Models.Pagos;
using Session;
using Factory;
using ConnectDB;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Script.Serialization;
using PagoProfesores.Models.EsquemasdePago;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Web;

namespace PagoProfesores.Controllers.Pagos
{
    public class EstadodeCuentaController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public string TABLECONDICIONSQLD = null;

        public EstadodeCuentaController()
        {
            db = new database();

            string[] scripts = {
				"plugins/Angular/jquery.fileupload.js",
                "js/Pagos/EstadodeCuenta/EstadodeCuenta.js","plugins/autocomplete/js/jquery.easy_autocomplete.js" ,
                "js/Pagos/GestiondePagos/download.js"};
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {

                new Factory.Privileges { Permiso = 10171,  Element = "Controller" }, //PERMISO ACCESO ESTADO CUENTA divConsultaEC
                new Factory.Privileges { Permiso = 10058,  Element = "formbtnconsultaWeb" }, //PERMISO ESTADO CUENTA WEB
                new Factory.Privileges { Permiso = 10059,  Element = "formbtnconsultar" }, //PERMISO BOTOB CONSULTAR
                new Factory.Privileges { Permiso = 10060,  Element = "formbtnagregar" }, //PERMISO OPCION A. CONCEPTO 
                new Factory.Privileges { Permiso = 10061,  Element = "formbtnagregarPensionados" }, //PERMISO OPCION A. PENSIONADO 
                new Factory.Privileges { Permiso = 10062,  Element = "formbtnmover" }, //PERMISO BOTON MOVER
                new Factory.Privileges { Permiso = 10063,  Element = "formbtneliminar" }, //PERMISO BOTON ELIMINAR

                new Factory.Privileges { Permiso = 10064,  Element = "formbtnclonar" }, //PERMISO BOTON CLONAR
                new Factory.Privileges { Permiso = 10065,  Element = "formbtngrabar" }, //PERMISO BOTON GUARDAR
                new Factory.Privileges { Permiso = 10066,  Element = "btnEditarEdoCta" }, //PERMISO BOTON EDITAR EDOCUENTA(GUARDAR en MODAL)
                new Factory.Privileges { Permiso = 10067,  Element = "formbtnagregarEdoCtaExt" }, //PERMISO BOTON MOVER EDOCUENTA(GUARDAR MOVER)
                new Factory.Privileges { Permiso = 10068,  Element = "agregarCPModal" }, //PERMISO BOTON AGREGAR CONCEPTO MODAL
                new Factory.Privileges { Permiso = 10172,  Element = "agregarCPNuevoModal" },//PERMISO BOTON AGREGAR nuevo concepto en el MODAL concepto//
                new Factory.Privileges { Permiso = 10069,  Element = "agregarPModal" }, //PERMISO BOTON AGREGAR PENSION MODAL//
                new Factory.Privileges { Permiso = 10270,  Element = "agregar_group" }, //PERMISO BOTON AGREGAR//

                new Factory.Privileges { Permiso = 10275, Element = "edit_EC_fechadePago"}, //PERMISO PARA MODIFICAR FECHA DE PAGO
                new Factory.Privileges { Permiso = 10275, Element = "edit_EC_fechadePago_div"}, //PERMISO PARA MODIFICAR FECHA DE PAGO
            };
        }

        public ActionResult Start()
        {
            try
            {
                if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                ViewBag.Main = view.createMenu("Pagos", "Estado de cuenta", sesion);
                ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
                ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion, "-1");
                ViewBag.COMBO_CAMPUSPA =  getCampusPA();
                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                EsquemasModel model = new EsquemasModel();
                //Combo CONCEPTO PAGO
                model.Obtener_ConceptoPago();
                ViewBag.CONCEPTO_PAGO = model.conceptoPago;
				EstadodeCuentaModel modelEC = new EstadodeCuentaModel();
                ViewBag.TIPO_BLOQUEO = getTiposDeBloqueo(modelEC);

                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla Estado de cuenta.", sesion);

                return View(Factory.View.Access + "Pagos/EstadodeCuenta/Start.cshtml");
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa a pantalla Estado de cuenta" + e.Message, sesion);

                return View();
            }
        }

        //public void bandera_recalculo(string idsiu, string sede)
        //{
        //    string sql2 = "";
        //    ResultSet res = null;
        //    ResultSet res2 = null;
        //    try
        //    {
        //        string sql = "SELECT * " +
        //              "  FROM VESTADO_CUENTA " +
        //              " WHERE IDSIU = '" + idsiu + "'" +
        //              " and CVE_SEDE = '" + sede + "'" +
        //              " order by FECHAPAGO, PERIODO ";

        //        res = db.getTable(sql);
        //        while (res.Next())
        //        {
        //            //if (string.IsNullOrWhiteSpace(res.Get("FECHARECIBO")) && res.Get("CVE_TIPOFACTURA") == "A")
        //            if (string.IsNullOrWhiteSpace(res.Get("FECHADEPOSITO")) && res.Get("CVE_TIPOFACTURA") == "A")
        //            {
        //                sql2 = "SELECT (ISNULL(SUM(MONTO), 0) ) as montor " +
        //                       "  FROM VESTADO_CUENTA " +
        //                       " WHERE  (FECHADEPOSITO <> '' AND FECHADEPOSITO IS NOT NULL ) " +
        //                       "   AND PADRE                                                  = 0 " +
        //                       "   AND CVE_SEDE                                               = '" + sede + "'" +
        //                       "   AND (FORMAT( CAST(FECHAPAGO AS date) ,'yyyy-MM', 'en-US')) = (FORMAT( CAST( '" + res.Get("FECHAPAGO") + "'  AS date) ,'yyyy-MM', 'en-US'))" +
        //                       "   AND ID_PERSONA                                             = " + res.Get("ID_PERSONA") +
        //                       "   AND CVE_TIPOFACTURA                                        = 'A' " +
        //                       "   AND ID_ESTADODECUENTA                                     <> " + res.Get("ID_ESTADODECUENTA") +
        //                       "   AND FECHAPAGO                                             <= '" + res.Get("FECHAPAGO") + "'";//****CHECAR

        //                res2 = db.getTable(sql2);
        //                if (res2.Next())
        //                    if (res2.GetDecimal("montor") > 0)
        //                    {
        //                        sql = "UPDATE ESTADODECUENTA" +
        //                               " SET RECALCULO = 1" +
        //                               " WHERE  ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");

        //                        db.execute(sql);
        //                    }
        //                    else
        //                    {
        //                        sql = "UPDATE ESTADODECUENTA" +
        //                              " SET RECALCULO = 0" +
        //                              " WHERE  ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");

        //                        db.execute(sql);
        //                    }
        //            }/*else if (!string.IsNullOrWhiteSpace(res.Get("FECHARECIBO")) && res.Get("FECHARECIBO") == "True" && res.Get("CVE_TIPOFACTURA") == "A")
        //            {
        //            }*/
        //        }
        //    }
        //    catch (Exception e) { }
        //}


        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 10, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null,
                                          string fltrIdSiu = "",string fltrSedes = "", string fltrPperio = "", string fltrPerio = "", string fltrEscuela = ""
            , string fltrTipoContr = "", string fltrCCost = "", string fltrPagoI = "", string fltrPagoF = "", string fltrReciI = ""
            , string fltrReciF = "", string fltrDispI = "", string fltrDispF = "", string fltrDepoI = "", string fltrDepoF = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, new database(), SESSION_BEHAVIOR.AJAX)) == null)
                    return string.Empty;

            //Marca con una bandera los conceptos con recalculo del ISR Ret
            RecalculoAsimilados recalculo = new RecalculoAsimilados();
            recalculo.bandera_recalculo(fltrIdSiu, fltrSedes);
            //bandera_recalculo(fltrIdSiu, fltrSedes);
            
            DataTablePlus table = new DataTablePlus();
            string retorno = string.Empty;
            
            table.TABLE = "VESTADO_CUENTA";
            table.TABLED = "VESTADO_CUENTA_DETALLE";
            string[] columnas = { "Campus", "Periodo", "Parte del periodo", "Esquema", "Concepto","Origen Pago", "Monto", "IVA", "IVA Ret", "ISR Ret", "Monto Deposito", "Fecha pago",
                                  "Fecha Recibo", "Fecha Dispersión", "Fecha Depósito", "Fecha Solicitado", "Tipo de pago", "Centro de costos", "Usuario", "F. Modificación"};																																				 

            string[] campos = { "ID_ESTADODECUENTA", "ID_EDOCTADETALLE", "CVE_SEDE", "PERIODO", "PARTEDELPERIODODESC", "ESQUEMA", "CONCEPTO","ORIGEN_PAGO", "MONTO",
                                "MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "BANCOS", "FECHAPAGO", "FECHARECIBO", "FECHADISPERSION", "FECHADEPOSITO", "FECHA_SOLICITADO",
                                "CVE_TIPODEPAGO", "CENTRODECOSTOS", "USUARIO", "FECHA_M", "PADRE", "ID_ESQUEMA", "BLOQUEOS", "CVE_ESCUELA", "PUBLICADO", "VISTA", "PARTEDELPERIODO","RECALCULO" };
																										  
            string[] camposearch = { "ID_ESTADODECUENTA", "CVE_SEDE", "PERIODO", "PARTEDELPERIODODESC", "ESQUEMA", "CONCEPTO", "MONTO", "CVE_TIPODEPAGO",
                                     "MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "BANCOS", "FECHAPAGO", "CENTRODECOSTOS", "USUARIO", "FECHA_M"};

            string[] camposhidden = { "ID_ESTADODECUENTA", "ID_EDOCTADETALLE", "PADRE", "ID_ESQUEMA", "BLOQUEOS", "CVE_ESCUELA", "PUBLICADO", "VISTA", "PARTEDELPERIODO", "RECALCULO" };

            table.dictColumnFormat.Add("CONCEPTO", delegate (string value, ResultSet res)
            {
                retorno = "<div style=\"width:130px;\">" + value + "&nbsp;&nbsp;";
                if (res.Get("VISTA") == "A")
                {
                    if (res.Get("PUBLICADO") == "True")
                        retorno += "&nbsp;<span class='fa fa-eye'></span>";

                    if (res.Get("FECHADEENTREGA") != null && res.Get("FECHADEENTREGA") != string.Empty)
                        retorno += "&nbsp;<span class='fa fa-file-text-o'></span>";

                    if (res.Get("FECHADEPOSITO") != null && res.Get("FECHADEPOSITO") != string.Empty)
                        retorno += "&nbsp;<span class='fa fa-credit-card'></span>";

                    retorno += "</div>";
                    return retorno;
                }
                else
                {
                    retorno += "</div>";
                    return retorno;
                }
            });
            
            table.dictColumnFormat.Add("MONTO_ISRRET", delegate (string value, ResultSet res)
            {
                 if (!string.IsNullOrWhiteSpace(res.Get("RECALCULO")))
                 {
                     //bool recalculo = res.GetBool("RECALCULO");
                     if (res.Get("RECALCULO") == "True")
                        return "<span style=\"font-size: 14px; font-style: italic; text-decoration: underline;color: #ff4021;\">" + Double.Parse(value).ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-MX")) + "</span>";
                    else
                         return Double.Parse(value).ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-MX"));
                 }                   
                 else
                  return Double.Parse(value).ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-MX"));
                //  return "<div style=\"font-size: 14px;\">" + value + "</div>";
            });

            table.dictColumnFormat.Add("CENTRODECOSTOS", delegate (string value, ResultSet res)
            {
                return "<div style=\"width:400px;\">" + value + "</div>";
            });

            table.COLUMNAS = columnas;
            table.CAMPOS = campos;
            table.CAMPOSSEARCH = camposearch;
            table.CAMPOSHIDDEN = camposhidden;

            table.orderby = "FECHAPAGO";// orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_ESTADODECUENTA";
            table.TABLECONDICIONSQL = "IDSIU = '" + fltrIdSiu + "'";

            List<string> filtros = new List<string>();           

            if (fltrSedes != "" && fltrSedes != "null") filtros.Add("CVE_SEDE = '" + fltrSedes + "'");

            if (fltrPerio != "" && fltrPerio != "null") filtros.Add("PERIODO = '" + fltrPerio + "'");

            if (fltrPperio != "" && fltrPperio != "null") filtros.Add("PARTEDELPERIODO = '" + fltrPperio + "'");

            if (fltrEscuela != "" && fltrEscuela != "null") filtros.Add("CVE_ESCUELA = '" + fltrEscuela + "'");

            if (fltrTipoContr != "" && fltrTipoContr != "null") filtros.Add("CVE_TIPODEPAGO = '" + fltrTipoContr + "'");

            if (fltrCCost != "" && fltrCCost != "null") filtros.Add("ID_CENTRODECOSTOS = '" + fltrCCost + "'");  

            if (fltrPagoI != "" && fltrPagoI != "null") filtros.Add("FECHAPAGO >= '" + fltrPagoI + "'");

            if (fltrPagoF != "" && fltrPagoF != "null") filtros.Add("FECHAPAGO <= '" + fltrPagoF + "'");

            if (fltrReciI != "" && fltrReciI != "null") filtros.Add("FECHARECIBO >= '" + fltrReciI + "'");

            if (fltrReciF != "" && fltrReciF != "null") filtros.Add("FECHARECIBO <= '" + fltrReciF + "'");

            if (fltrDispI != "" && fltrDispI != "null") filtros.Add("FECHADISPERSION >= '" + fltrDispI + "'");

            if (fltrDispF != "" && fltrDispF != "null") filtros.Add("FECHADISPERSION <= '" + fltrDispF + "'");

            if (fltrDepoI != "" && fltrDepoI != "null") filtros.Add("FECHADEPOSITO >= '" + fltrDepoI + "'");

            if (fltrDepoF != "" && fltrDepoF != "null") filtros.Add("FECHADEPOSITO <= '" + fltrDepoF + "'");

            string union = "";
            if (filtros.Count > 0) {
                union = " AND ";
                table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());
            }   
            table.enabledCheckbox = true;
            table.enabledButtonControls = false;

            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        ResultSet res2;
        ResultSet res3;

        public void ExportExcel()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return; }

            List<string> lPadre = new List<string>();
            List<string> lPadreBloq = new List<string>();
            List<string> lHijoBloq = new List<string>();
            List<string> lPensionados = new List<string>();
            List<string> lSubPadre = new List<string>();
            List<string> lSubPadreBloq = new List<string>();
            List<string> lSubHijoBloq = new List<string>();
            List<string> lSubPensionados = new List<string>();

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Campus", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Parte del periodo", typeof(string));
                tbl.Columns.Add("Esquema", typeof(string));
                tbl.Columns.Add("Concepto", typeof(string));
                tbl.Columns.Add("Monto", typeof(string));
                tbl.Columns.Add("IVA", typeof(string));
                tbl.Columns.Add("IVA Ret", typeof(string));
                tbl.Columns.Add("ISR Ret", typeof(string));
                tbl.Columns.Add("Banco", typeof(string));
                tbl.Columns.Add("Fecha de pago", typeof(string));
                tbl.Columns.Add("F. Recibo", typeof(string));
                tbl.Columns.Add("F. Dispersión", typeof(string));
                tbl.Columns.Add("F. Depósito", typeof(string));
                tbl.Columns.Add("F. Solicitado", typeof(string));
                tbl.Columns.Add("Tipo de pago", typeof(string));
                tbl.Columns.Add("Centro de costos", typeof(string));
                tbl.Columns.Add("Usuario", typeof(string));
                tbl.Columns.Add("F.Modificación", typeof(string));


                List<string> filtros = new List<string>();
                
                ResultSet res = getRowsTable(Request, filtros, 1);

                int row = 2;
                while (res.Next())
                {
                    if (res.Get("PADRE") == "0")
                    {
                        getRowsTableExcel(tbl, res);
                        lPadre.Add("A" + row + ":T" + row);
                        
                        var bloqueoContrato = res.GetBool("BLOQUEOCONTRATO");//BLOQUEOCONTRATO es 1(bloqueado)
                        if ((bloqueoContrato && string.IsNullOrWhiteSpace(res.Get("FECHADEENTREGA"))) || res.GetInt("BLOQUEOS") > 0)
                            lPadreBloq.Add("E" + row + ":E" + row);                      

                        row++;

                        TABLECONDICIONSQLD = " ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");
                        res2 = getRowsTable(Request, filtros, 2);
                        
                        //Detalle Estado de Cuenta
                        while (res2.Next())
                        {
                            getRowsTableExcelECDetalle(tbl, res2);
                            
                             bloqueoContrato = res2.GetBool("BLOQUEOCONTRATO");//BLOQUEOCONTRATO es 1(bloqueado)
                            if ((bloqueoContrato && string.IsNullOrWhiteSpace(res2.Get("FECHADEENTREGA"))) || res2.GetInt("BLOQUEOS") > 0)
                                lHijoBloq.Add("E" + row + ":E" + row);
                            
                            row++;
                        }
                        // termina el listado de PADRE 0  y sus detalles

                        /** -  LA BÚSQUEDA DE PENSIONADOS - **/
                        ResultSet resP = this.getRowsTableP(sesion.db, res.Get("ID_ESTADODECUENTA"));
                        while (resP.Next())
                        {
                            getRowsTableExcelECDetalle(tbl, resP);
                            lPensionados.Add("B" + row + ":T" + row);
                            row++;
                        }

                        /** - LA BÚSQUEDA DE SUBPADRES - **/
                        ResultSet resH = this.getRowsTableH(sesion.db, res.Get("ID_ESTADODECUENTA"));
                        while (resH.Next())
                        {
                            getRowsTableExcel(tbl, resH);
                            lSubPadre.Add("A" + row + ":T" + row);
                            
                            bloqueoContrato = resH.GetBool("BLOQUEOCONTRATO");//BLOQUEOCONTRATO es 1(bloqueado)
                            if ((bloqueoContrato && string.IsNullOrWhiteSpace(resH.Get("FECHADEENTREGA"))) || resH.GetInt("BLOQUEOS") > 0)
                                lSubPadreBloq.Add("E" + row + ":E" + row);
                            
                            row++;

                            /* Comienza la búsqueda de los SUBHIJOS de cada SUBPADRE */
                            /*********************************************************/
                            TABLECONDICIONSQLD = " ID_ESTADODECUENTA = " + resH.Get("ID_ESTADODECUENTA");
                            res3 = this.getRowsTable(Request, filtros, 2);
                            while (res3.Next())
                            {
                                getRowsTableExcelECDetalle(tbl, res3);
                                
                                bloqueoContrato = res3.GetBool("BLOQUEOCONTRATO");//BLOQUEOCONTRATO es 1(bloqueado)
                                if ((bloqueoContrato && string.IsNullOrWhiteSpace(res3.Get("FECHADEENTREGA"))) || res3.GetInt("BLOQUEOS") > 0)
                                   lSubHijoBloq.Add("E" + row + ":E" + row);
                                
                                row++;
                            }

                            /** - COMIENZA LA BÚSQUEDA DE SUB-PENSIONADOS - **/
                            ResultSet resP2 = this.getRowsTableSP(sesion.db, res.Get("ID_ESTADODECUENTA"), resH.Get("ID_ESTADODECUENTA"));
                            while (resP2.Next())
                            {
                                getRowsTableExcelECDetalle(tbl, resP2);
                                lSubPensionados.Add("B" + row + ":T" + row);
                                row++;
                            }
                        }
                    }
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Estado de Cuenta");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:T1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:T1"])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    foreach(var x in lPadre)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(201, 223, 240));
                            rngX.Style.Font.Color.SetColor(Color.Black);
                        }
                    }

                    foreach (var x in lPadreBloq)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Font.Strike = true;
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Font.Color.SetColor(Color.Red);
                        }
                    }

                    foreach (var x in lPensionados)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 237, 184)); 
                            rngX.Style.Font.Color.SetColor(Color.Black);
                        }
                    }

                    foreach (var x in lHijoBloq)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Font.Strike = true;
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                            rngX.Style.Font.Color.SetColor(Color.Red);
                        }
                    }

                    foreach (var x in lSubPadre)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(226, 231, 235));
                            rngX.Style.Font.Color.SetColor(Color.Black);
                        }
                    }

                    foreach (var x in lSubPadreBloq)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Font.Strike = true;
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Font.Color.SetColor(Color.Red);
                        }
                    }

                    foreach (var x in lSubHijoBloq)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Font.Strike = true;
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                            rngX.Style.Font.Color.SetColor(Color.Red);
                        }
                    }

                    foreach (var x in lSubPensionados)
                    {
                        using (ExcelRange rngX = ws.Cells[x])
                        {
                            rngX.Style.Fill.PatternType = ExcelFillStyle.Solid;                      
                            rngX.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 237, 184)); 
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
                    Response.AddHeader("content-disposition", "attachment;  filename=EstadodeCuenta.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Estado de Cuenta", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Estado de Cuenta" + e.Message, sesion);
            }
        }

        public ResultSet getRowsTable(HttpRequestBase Request, List<string> filtros, int opc = 1)
        {
            if (Request.Params["idSiuEC"] != "" && Request.Params["idSiuEC"] != "null")
                filtros.Add("IDSIU = '" + Request.Params["idSiuEC"] + "'");

            if (Request.Params["fltrSedes"] != "" && Request.Params["fltrSedes"] != "null")
                filtros.Add("CVE_SEDE = '" + Request.Params["fltrSedes"] + "'");

            if (Request.Params["fltrPerio"] != "" && Request.Params["fltrPerio"] != "null")
                filtros.Add("PERIODO = '" + Request.Params["fltrPerio"] + "'");

            if (Request.Params["fltrEscuela"] != "" && Request.Params["fltrEscuela"] != "null")
                filtros.Add("CVE_ESCUELA = '" + Request.Params["fltrEscuela"] + "'");

            if (Request.Params["fltrTipoContr"] != "" && Request.Params["fltrTipoContr"] != "null")
                filtros.Add("CVE_TIPODEPAGO = '" + Request.Params["fltrTipoContr"] + "'");

            if (Request.Params["fltrCCost"] != "" && Request.Params["fltrCCost"] != "null")
                filtros.Add("ID_CENTRODECOSTOS = '" + Request.Params["fltrCCost"] + "'");

            if (Request.Params["fltrPagoI"] != "" && Request.Params["fltrPagoI"] != "null")
                filtros.Add("FECHAPAGO >= '" + Request.Params["fltrPagoI"] + "'");

            if (Request.Params["fltrPagoF"] != "" && Request.Params["fltrPagoF"] != "null")
                filtros.Add("FECHAPAGO <= '" + Request.Params["fltrPagoF"] + "'");

            if (Request.Params["fltrReciI"] != "" && Request.Params["fltrReciI"] != "null")
                filtros.Add("FECHARECIBO >= '" + Request.Params["fltrReciI"] + "'");

            if (Request.Params["fltrReciF"] != "" && Request.Params["fltrReciF"] != "null")
                filtros.Add("FECHARECIBO <= '" + Request.Params["fltrReciF"] + "'");

            if (Request.Params["fltrDispI"] != "" && Request.Params["fltrDispI"] != "null")
                filtros.Add("FECHADISPERSION >= '" + Request.Params["fltrDispI"] + "'");

            if (Request.Params["fltrDispF"] != "" && Request.Params["fltrDispF"] != "null")
                filtros.Add("FECHADISPERSION <= '" + Request.Params["fltrDispF"] + "'");

            if (Request.Params["fltrDepoI"] != "" && Request.Params["fltrDepoI"] != "null")
                filtros.Add("FECHADEPOSITO >= '" + Request.Params["fltrDepoI"] + "'");

            if (Request.Params["fltrDepoF"] != "" && Request.Params["fltrDepoF"] != "null")
                filtros.Add("FECHADEPOSITO <= '" + Request.Params["fltrDepoF"] + "'");

            string conditions = string.Join<string>(" AND ", filtros.ToArray());

            string union = "";
            if (conditions.Length != 0) union = " WHERE ";

            String TABLE = " " + (opc == 1 ? "VESTADO_CUENTA" : "VESTADO_CUENTA_DETALLE") + " ";
            
            String sql = "SELECT * FROM " + TABLE + " " + union + " " + conditions;

            if (TABLECONDICIONSQLD != "" && TABLECONDICIONSQLD != null)
            {
                sql += " AND " + TABLECONDICIONSQLD;
            }

            return db.getTable(sql);
        }

        private ResultSet getRowsTableH(database db, string IDCta = "")
        {
            String sql = "SELECT * FROM VESTADO_CUENTA where PADRE = " + IDCta;
            sql += " ORDER BY FECHAPAGO ";

            return db.getTable(sql);
        }
        
        private ResultSet getRowsTableP(database db, string IDCta = "")
        {
            String sql = "SELECT * FROM VESTADO_CUENTA_PENSIONADOS where ID_ESTADODECUENTA = " + IDCta;
            sql += " ORDER BY ID_CTAPENSIONADO ";

            return db.getTable(sql);
        }

        private ResultSet getRowsTableSP(database db, string Padre = "", string IDCta = "")
        {
            String sql = "SELECT * FROM VESTADO_CUENTA_PENSIONADOS where PADRE = " + Padre + " and ID_ESTADODECUENTA = " + IDCta;
            sql += " ORDER BY ID_CTAPENSIONADO ";

            return db.getTable(sql);
        }
        
        public void getRowsTableExcel(System.Data.DataTable tbl, ResultSet res)
        {
            // Here we add five DataRows.
            tbl.Rows.Add(res.Get("CVE_SEDE"), res.Get("PERIODO"), res.Get("PARTEDELPERIODODESC")
                , res.Get("ESQUEMA"), res.Get("CONCEPTO"), res.Get("MONTO")
                , res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("BANCOS")
                , res.Get("FECHAPAGO"), res.Get("FECHARECIBO"), res.Get("FECHADISPERSION")
                , res.Get("FECHADEPOSITO"), res.Get("FECHA_SOLICITADO"), res.Get("CVE_TIPODEPAGO"), res.Get("CENTRODECOSTOS")
                  , res.Get("USUARIO"), res.Get("FECHA_M"));
        }

        public void getRowsTableExcelECDetalle(System.Data.DataTable tbl, ResultSet res)
        {
            // Here we add five DataRows.
            tbl.Rows.Add("", res.Get("PERIODO"), res.Get("PARTEDELPERIODODESC")
                , res.Get("ESQUEMA"), res.Get("CONCEPTO"), res.Get("MONTO")
                , res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("BANCOS")
                , res.Get("FECHAPAGO"), res.Get("FECHARECIBO"), res.Get("FECHADISPERSION")
                , res.Get("FECHADEPOSITO"), res.Get("FECHA_SOLICITADO"), res.Get("CVE_TIPODEPAGO"), res.Get("CENTRODECOSTOS")
                  , res.Get("USUARIO"), res.Get("FECHA_M"));
        }

        [HttpPost]
        public ActionResult BuscaPersona(EstadodeCuentaModel model)
        {
            if (model.BuscaPersona())
            {
                Log.write(this, "Controller: EstadodeCuenta  - BuscaPersona", LOG.CONSULTA, "SQL:" + model.sql, sesion);
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            else
            {
                model._msg = "No se encuentra el ID ingresado";
                Log.write(this, "Controller: EstadodeCuenta - BuscaPersona", LOG.ERROR, "SQL:" + model.sql, sesion);
                return Json(new JavaScriptSerializer().Serialize(model));
            }
        }

        [HttpPost]
        public ActionResult Edit(EstadodeCuentaModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditBloqueos(EstadodeCuentaModel model)
        {
            Debug.WriteLine("controller EstadodeCuenta");
            
            if (model.EditBloqueos())
            {
                Debug.WriteLine("controller EditBloqueos");
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult Grabar(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Grabar())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Estado de cuenta guardada con exito: x") });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al GUARDAR: x") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult AgregarEdoCta(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.AgregarEdoCta())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha agregado el nuevo registro de estado de cuenta para el IDSIU: " + model.IdSIU + ".") });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Imposible agregar el nuevo registro.") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult AgregarEdoCtaModal(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.AgregarEdoCtaModal())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha agregado el nuevo registro de estado de cuenta para el IdPersona: " + model.IdPersona + ".") });
                }
                else
                {
                    if (model.mensaje_res == "NOEXISTE")
                    {
                        Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Error("Solo se pueden agregar conceptos de esquemas asignados al usuario. Sí desea agregar un nuevo esquema elija la opción: Agregar esquema.") });

                    }
                    else
                    {
                        Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Error("Imposible agregar el nuevo registro.") });

                    }


                   
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult AgregarEdoCtaModalPensionado(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.AgregarEdoCtaModalPensionado())
                {
                    if (model.Existe) {
                        Log.write(this, "Save", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Succes("Se ha agregado un Pensionado para el Profesor con IDSIU: " + model.IdSIU + ".") });
                    }
                    else
                    {
                        Log.write(this, "Save", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Warning("La Persona no tiene el concepto de Pago seleccionado ") });
                    }
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Imposible agregar el nuevo regristro.") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult AgregarEdoCtaModalExterno(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.AgregarEdoCtaModalExterno_())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha agregado el estado de cuenta para el usuario: " + model.IdSIU + ".") });
                }
                else
                {

                    if (model.mensaje_res == "EXISTE")
                    {
                        Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Error("Imposible agregar el registro. El esquema ya existe.") });

                    }
                    else
                    {

                        Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Error("Imposible agregar el registro.") });

                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult EliminarPensionado(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.EliminarPensionado())
                {
                    Log.write(this, "eliminar", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha eliminado el registro del Pensionado seleccionado.") });
                }
                else
                {
                    Log.write(this, "eliminar", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error, no se ha podido eliminar el registro del Pensionado seleccionado..") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        //Edgar															   
        public ActionResult quitarFechaContrato(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.quitarFechaContrato())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha eliminado la fecha de entrega de contrato.") });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("No se ha podido eliminar la fecha de entrega de contrato.") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult recalcular(EstadodeCuentaModel model)
        {
            if (model.recalcular())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        [HttpPost]
        public ActionResult mover(EstadodeCuentaModel model)
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.mover())
                {
                    Log.write(this, "mover", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha movido el detalle del estado de cuenta, a otro estado de cuenta.") });
                }
                else
                {
                    Log.write(this, "mover", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al mover alguno o todos los detalles de cuenta.") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult eliminar(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.eliminar())
                {
                    Log.write(this, "eliminar", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha(n) eliminado el/los registro(s) seleccionado(s) del Estado de Cuenta.") });
                }
                else
                {
                    Log.write(this, "eliminar", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error("Error, no se ha(n) podido eliminar el/los registro(s) seleccionado(s) del Estado de Cuenta.") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult getFechaConceptodePago(EstadodeCuentaModel model)
        {
            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return View();

            try
            {
                if (model.getFechaConceptodePago())
                {
                    Log.write(this, "ConceptosdePagoController - Consultar Fecha de Pago", LOG.CONSULTA, "SQL:" + model.sql, sesion);
                    return Json(new JavaScriptSerializer().Serialize(model));
                }
                else
                {
                    Log.write(this, "ConceptosdePagoController - Consultar Fecha de Pago", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return View();
                }
            }
            catch (Exception e)
            {
                Log.write(this, "ConceptosdePagoController - Consultar Fecha de Pago", LOG.ERROR, "SQL:" + e.Message, sesion);
                return View();
            }
        }
        
        //Charly
        [HttpPost]
        public ActionResult EditPagoEstadoCuenta(EstadodeCuentaModel model)
        {
		  if ((sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX)) == null)
				return Content("-1");

			if (model.EditPagoEstadoCuenta())
			{
                sesion.vdata["IDSIU"] = model.IdSIU;
				//sesion.vdata["Sede"] = model.CveSede;
				sesion.saveSession();
				model.Dispose();
				return Json(new JavaScriptSerializer().Serialize(model));
			}
			return View();
        }

        public ActionResult savePagoEdoCta(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            try
            {
                if (model.savePagoEdoCta())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("PAGO editado con exito: " + model.IdEdoCta) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al GUARDAR: " + model.IdEdoCta) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
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
        
        [HttpPost]
        public ActionResult seleccionarPadre(EstadodeCuentaModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            try
            {
                if (model.seleccionarPadre())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = model.seleccionar });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = model.seleccionar });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        //[HttpPost]
        [HttpGet]
        public string eliminarXMLoPDF(string EdoCtaID = "", string file = "")
        {

            EstadodeCuentaModel model = new EstadodeCuentaModel();
            model.IdEdoCta = EdoCtaID;

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                ;//return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.eliminarXMLoPDF(file))
                {
                    Log.write(this, "eliminar", LOG.EDICION, "SQL:" + model.sql, sesion);
                    //return Json(new { msg = Notification.Succes("Se ha(n) eliminado el/los registro(s) seleccionado(s) del Estado de Cuenta.") });
                    return "=)";
                }
                else
                {
                    Log.write(this, "eliminar", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return "=(";
                    //return Json(new { msg = Notification.Error("Error, no se ha(n) podido eliminar el/los registro(s) seleccionado(s) del Estado de Cuenta.") });
                }
            }
            catch (Exception e)
            {
                //return Json(new { msg = Notification.Error(e.Message) });
                return "=(";
            }
        }

        //[HttpPost]
        //public ActionResult getCentroCostos(EstadodeCuentaModel model)
        //{
        //    if (model.getCentroCostos())
        //    {
        //        return Json(new JavaScriptSerializer().Serialize(model.CentroCostos));
        //    }
        //    else
        //    {
        //        return Json(new { msg = Notification.Error(" Error al asiganar el esquema") });
        //    }
        //}
    }//</>
}