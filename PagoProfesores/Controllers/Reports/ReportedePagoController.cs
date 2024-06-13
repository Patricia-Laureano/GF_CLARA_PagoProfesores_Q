using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using PagoProfesores.Controllers.Herramientas;

namespace PagoProfesores.Controllers.Reports
{
    public class ReportedePagoController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public string TABLECONDICIONSQLD = null;

        public ReportedePagoController()
        {
            db = new database();

            string[] scripts = { "js/Reports/ReportedePago/reportedepago.js" };
            Scripts.SCRIPTS = scripts;
            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10027,  Element = "Controller" }, //PERMISO ACCESO DetallePagos
                 new Factory.Privileges { Permiso = 10027,  Element = "formbtnconsultar" }, //PERMISO AGREGAR DetallePagos
            };
        }

        // GET: DetallePagos
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Reportes", "Reporte de Pago", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Detalle de Pagos Start", LOG.CONSULTA, "Reporte de Pagos", sesion);

            ViewBag.sede = view.createLevels(sesion, "sede");

            return View(Factory.View.Access + "Reports/ReportedePago/Start.cshtml");
        }




        //#EXPORT EXCEL
        ResultSet res2;
        ResultSet res3;

        public void ExportExcel()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return; }



            ProgressBarCalc progressbar = new ProgressBarCalc(sesion, "ReportedePago");
            progressbar.prepare();



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
                tbl.Columns.Add("ID", typeof(string));
                tbl.Columns.Add("Docente", typeof(string));
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

                int count = res.Count;

                progressbar.init(count + 1);//99

                int row = 2;
                string fil_ED = Request.Params["filtro_"];

                while (res.Next())
                {


                    if (res.Get("PADRE") == "0")
                    {

                        if (fil_ED != "PD")
                        {
                            getRowsTableExcel(tbl, res);
                            lPadre.Add("A" + row + ":U" + row);
                        }


                        var bloqueoContrato = res.GetBool("BLOQUEOCONTRATO");//BLOQUEOCONTRATO es 1(bloqueado)
                        if (((bloqueoContrato && string.IsNullOrWhiteSpace(res.Get("FECHADEENTREGA"))) || res.GetInt("BLOQUEOS") > 0) && fil_ED != "PD")
                            lPadreBloq.Add("G" + row + ":G" + row);

                        if (fil_ED != "PD")
                            row++;

                        TABLECONDICIONSQLD = " ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");
                        res2 = getRowsTable(Request, filtros, 2);

                        //Detalle Estado de Cuenta  
                        if (fil_ED != "PE")
                            while (res2.Next())
                            {
                                getRowsTableExcelECDetalle(tbl, res2);


                                bloqueoContrato = res2.GetBool("BLOQUEOCONTRATO");//BLOQUEOCONTRATO es 1(bloqueado)
                                if ((bloqueoContrato && string.IsNullOrWhiteSpace(res2.Get("FECHADEENTREGA"))) || res2.GetInt("BLOQUEOS") > 0)
                                    lHijoBloq.Add("G" + row + ":G" + row);

                                row++;
                            }
                        // termina el listado de PADRE 0  y sus detalles


                        /** -  LA BÚSQUEDA DE PENSIONADOS - **/

                        ResultSet resP = this.getRowsTableP(sesion.db, res.Get("ID_ESTADODECUENTA"));
                        if (fil_ED != "PE")
                            while (resP.Next())
                            {
                                getRowsTableExcelECDetalle(tbl, resP);
                                lPensionados.Add("B" + row + ":U" + row);
                                row++;
                            }


                        /** - LA BÚSQUEDA DE SUBPADRES - **/
                        ResultSet resH = this.getRowsTableH(sesion.db, res.Get("ID_ESTADODECUENTA"));

                        if (fil_ED != "PE")
                            while (resH.Next())
                            {

                                getRowsTableExcel(tbl, resH);
                                lSubPadre.Add("A" + row + ":U" + row);

                                bloqueoContrato = resH.GetBool("BLOQUEOCONTRATO");//BLOQUEOCONTRATO es 1(bloqueado)
                                if ((bloqueoContrato && string.IsNullOrWhiteSpace(resH.Get("FECHADEENTREGA"))) || resH.GetInt("BLOQUEOS") > 0)
                                    lSubPadreBloq.Add("G" + row + ":G" + row);

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
                                        lSubHijoBloq.Add("G" + row + ":G" + row);

                                    row++;
                                }

                                /** - COMIENZA LA BÚSQUEDA DE SUB-PENSIONADOS - **/
                                ResultSet resP2 = this.getRowsTableSP(sesion.db, res.Get("ID_ESTADODECUENTA"), resH.Get("ID_ESTADODECUENTA"));
                                while (resP2.Next())
                                {
                                    getRowsTableExcelECDetalle(tbl, resP2);
                                    lSubPensionados.Add("B" + row + ":U" + row);
                                    row++;
                                }
                            }
                    }

                    progressbar.progress();

                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Estado de Cuenta");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:U1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:U1"])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    foreach (var x in lPadre)
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


            progressbar.complete();

        }

        public ResultSet getRowsTable(HttpRequestBase Request, List<string> filtros, int opc = 1)
        {


            if (Request.Params["sedes"] != "" && Request.Params["sedes"] != "null")
                filtros.Add("CVE_SEDE = '" + Request.Params["sedes"] + "'");


            if (Request.Params["fechai"] != "" && Request.Params["fechai"] != "null")
                filtros.Add("FECHAPAGO >= '" + Request.Params["fechai"] + "'");

            if (Request.Params["fechaf"] != "" && Request.Params["fechaf"] != "null")
                filtros.Add("FECHAPAGO <= '" + Request.Params["fechaf"] + "'");

            if (Request.Params["idesquema"] != "" && Request.Params["idesquema"] != "null" && Request.Params["idesquema"] != "<>''")
                filtros.Add("ID_ESQUEMA = '" + Request.Params["idesquema"] + "'");

            //


            if ((Request.Params["tipodocente"] != "" && Request.Params["tipodocente"] != "null") && (Request.Params["tipodocente"] != "T"))
                filtros.Add("CVE_TIPOFACTURA = '" + Request.Params["tipodocente"] + "'");


            if (Request.Params["pagosdeposito"] != "" && Request.Params["pagosdeposito"] != "null")
            {
                if (Request.Params["pagosdeposito"] == "PD")//PD pagos depositados (con fecha de deposito)
                    filtros.Add("(FECHADEPOSITO IS NOT NULL AND FECHADEPOSITO <> '' )");
                else if (Request.Params["pagosdeposito"] == "PP")//PP pagos pendientes o no depositados (sin fecha de deposito)
                    filtros.Add("(FECHADEPOSITO IS NULL OR FECHADEPOSITO = '' )");
                else { }//T todos 
            }



            string conditions = string.Join<string>(" AND ", filtros.ToArray());

            string union = "";
            if (conditions.Length != 0) union = " WHERE ";

            String TABLE = " " + (opc == 1 ? "VESTADO_CUENTA" : "VESTADO_CUENTA_DETALLE") + " ";

            String sql = "SELECT * FROM " + TABLE + " " + union + " " + conditions;

            if (TABLECONDICIONSQLD != "" && TABLECONDICIONSQLD != null)
            {
                sql += " AND " + TABLECONDICIONSQLD;
            }


            filtros.Clear();

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
            tbl.Rows.Add(res.Get("CVE_SEDE"), res.Get("PERIODO"), res.Get("IDSIU"), res.Get("NOMBRES") + " " + res.Get("APELLIDOS"), res.Get("PARTEDELPERIODODESC")
                , res.Get("ESQUEMA"), res.Get("CONCEPTO"), res.Get("MONTO")
                , res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("BANCOS")
                , res.Get("FECHAPAGO"), res.Get("FECHARECIBO"), res.Get("FECHADISPERSION")
                , res.Get("FECHADEPOSITO"), res.Get("FECHA_SOLICITADO"), res.Get("CVE_TIPODEPAGO"), res.Get("CENTRODECOSTOS")
                  , res.Get("USUARIO"), res.Get("FECHA_M"));
        }

        public void getRowsTableExcelECDetalle(System.Data.DataTable tbl, ResultSet res)
        {
            // Here we add five DataRows.
            tbl.Rows.Add("", res.Get("PERIODO"), res.Get("IDSIU"), res.Get("NOMBRES") + " " + res.Get("APELLIDOS"), res.Get("PARTEDELPERIODODESC")
                , res.Get("ESQUEMA"), res.Get("CONCEPTO"), res.Get("MONTO")
                , res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("BANCOS")
                , res.Get("FECHAPAGO"), res.Get("FECHARECIBO"), res.Get("FECHADISPERSION")
                , res.Get("FECHADEPOSITO"), res.Get("FECHA_SOLICITADO"), res.Get("CVE_TIPODEPAGO"), res.Get("CENTRODECOSTOS")
                  , res.Get("USUARIO"), res.Get("FECHA_M"));
        }







    }
}