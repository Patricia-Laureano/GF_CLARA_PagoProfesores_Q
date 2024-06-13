using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.Pagos;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Pagos
{
    public class DispersionController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        private DateTime minDateTime;

        public DispersionController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/GestiondePagos/dispersion.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                new Factory.Privileges { Permiso = 10127,  Element = "Controller" }, //PERMISO ACCESO REGISTRO DE CONTRATOS
                new Factory.Privileges { Permiso = 10128,  Element = "formbtnconsultar" }, //PERMISO EDITAR REGISTRO DE CONTRATOS
                new Factory.Privileges { Permiso = 10129,  Element = "formbtndispersion" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
                new Factory.Privileges { Permiso = 10130,  Element = "formbtnborarfechas" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
            };
        }

        [HttpPost]
        public ActionResult MergePDFS(DispersionModel bdrecibos)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            bdrecibos.sesion = sesion;

            try
            {
                string fileName = "";
                string directoryName = "";

                PdfDocument outputDocument = new PdfDocument();
                List<string> list = new List<string>();

                list = bdrecibos.getPDFs();

                if (list.Count > 0)
                {
                    foreach (string rutaPDF in list)
                    {
                        directoryName = Path.GetDirectoryName(rutaPDF);
                        fileName = Path.GetFileName(rutaPDF);

                        string path = Request.MapPath("~/" + directoryName + "") + @"\" + fileName;
                        PdfDocument inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                        CopyPages(inputDocument, outputDocument);
                    }

                    string pathsave = Request.MapPath("~/Upload") + @"\" + "filesave__.pdf";
                    outputDocument.Save(pathsave);
                    Process.Start(pathsave);
                    return Json(new { msg = Notification.Warning("se realizo con exito pdf´s") });
                }
                else
                {
                    return Json(new { msg = Factory.Notification.Warning("NO EXISTEN REGISTROS CON LA FECHA DE RECIBO") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }

        public void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }

        // GET: Recibos
        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Pagos", "Gestión de Pagos", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "FECHA_R", "ASC", sesion);
            ViewBag.TIPODISPERSION = this.getTipoDispersion();

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Gestión de Pagos Start", LOG.CONSULTA, "Ingresa Pantalla Recibos", sesion);

            return View(Factory.View.Access + "Pagos/GestiondePagos/Dispersion/Start.cshtml");
        }

        private Dictionary<string, string> prepareData(ResultSet res, bool add, bool bancaria, long usuario)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (add)
            {
                dic.Add("CVE_BANCO", res.Get("CVE_BANCO"));
                dic.Add("NOMBRES", res.Get("NOMBRES"));
                dic.Add("APELLIDOS", res.Get("APELLIDOS"));
                dic.Add("CONCEPTO", res.Get("CONCEPTO"));
                if (bancaria)
                    dic.Add("NOCUENTA", res.Get("NOCUENTA"));
                else
                    dic.Add("CUENTACLABE", res.Get("CUENTACLABE"));
                if (!string.IsNullOrWhiteSpace(res.Get("FECHADISPERSION")))
                    dic.Add("FECHADISPERSION", res.Get("FECHADISPERSION"));
            }
            else
            {
                dic.Add("TIPODEPAGO", res.Get("TIPODEPAGOPENSIONADO"));//T O C
                dic.Add("CVE_BANCO", res.Get("CVE_BANCO_PENSIONADO"));
                dic.Add("NOMBRES", res.Get("BENEFICIARIO_PENSIONADO"));
                dic.Add("APELLIDOS", res.Get("BENEFICIARIOP") + ' ' + res.Get("BENEFICIARIOM"));
                if (bancaria)
                    dic.Add("NOCUENTA", res.Get("CUENTA_PENSIONADO"));
                else
                    dic.Add("CUENTACLABE", res.Get("CLABE_PENSIONADO"));
                dic.Add("CONCEPTO", "Pensión");
            }
            dic.Add("ID_ESQUEMA", res.Get("ID_ESQUEMA"));
            dic.Add("CVE_SEDE", res.Get("CVE_SEDE"));
            dic.Add("TIPOTRANSFERENCIA", res.Get("TIPOTRANSFERENCIA"));
            if (!string.IsNullOrWhiteSpace(res.Get("FECHADEENTREGA")))
                dic.Add("FECHADEENTREGA", res.Get("FECHADEENTREGA"));
            dic.Add("IDSIU", res.Get("IDSIU"));
            dic.Add("ID_ESTADODECUENTA", res.Get("ID_ESTADODECUENTA"));
            dic.Add("PERIODO", res.Get("PERIODO"));
            dic.Add("MONTO", res.Get("MONTO"));
            dic.Add("MONTO_IVA", res.Get("MONTO_IVA"));
            dic.Add("MONTO_IVARET", res.Get("MONTO_IVARET"));
            dic.Add("MONTO_ISRRET", res.Get("MONTO_ISRRET"));
            dic.Add("BANCOS", res.Get("BANCOS"));
            if (!string.IsNullOrWhiteSpace(res.Get("FECHAPAGO")))
                dic.Add("FECHAPAGO", res.Get("FECHAPAGO"));
            if (!string.IsNullOrWhiteSpace(res.Get("FECHARECIBO")))
                dic.Add("FECHARECIBO", res.Get("FECHARECIBO"));
            if (!string.IsNullOrWhiteSpace(res.Get("FECHA_SOLICITADO")))
                dic.Add("FECHA_SOLICITADO", res.Get("FECHA_SOLICITADO"));
            if (!string.IsNullOrWhiteSpace(res.Get("FECHADEPOSITO")))
                dic.Add("FECHADEPOSITO", res.Get("FECHADEPOSITO"));
            dic.Add("CVE_ORIGENPAGO", res.Get("CVE_ORIGENPAGO"));
            dic.Add("ESQUEMA", res.Get("ESQUEMA"));
            dic.Add("PUBLICADO", (res.GetBool("PUBLICADO")) ? "1" : "0");
            dic.Add("BLOQUEADO", (res.GetBool("BLOQUEADO")) ? "1" : "0");
            dic.Add("USUARIO", usuario.ToString());
            dic.Add("PKCONCEPTOPAGO", res.Get("PKCONCEPTOPAGO"));

            return dic;
        }

        [HttpPost]
        public ActionResult DeleteTablaTemporal(DispersionModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            try
            {
                if (model.DeleteTablaTemporal())
                {

                    Log.write(this, "DeleteTemp", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se ha realizado la eliminación de la fecha de dispersión con exito!") });
                }
                else
                {
                    Log.write(this, "DeleteTemp", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Warning("No existen registros con esos filtros") });
                }
            }
            catch (Exception e)
            {
                Log.write(this, "DeleteTemp", LOG.ERROR, "SQL:" + model.sql + "e: " + e, sesion);
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }

        public void getTabla_TMP(string fechai, string fechaf, string banco, string tipodispersion, bool sinNoCuenta, string sede)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            DispersionModel model = new DispersionModel();
            //RecalculoAsimilados recalculo = new RecalculoAsimilados();

            model.sesion = sesion;
            long usuario = sesion.pkUser;

            try
            {
                string sql = "";
                ResultSet res = null;

                //recalculo

                model.RecalculoAsimiladosDispersion(sede, fechai, fechaf, sinNoCuenta);

                sql = "DELETE FROM DISPERSION_TMP WHERE USUARIO=" + usuario;
                res = db.getTable(sql);

                List<string> filtros = new List<string>();

                if (fechai != "") filtros.Add("FECHAPAGO >= '" + fechai + "'");
                if (fechaf != "") filtros.Add("FECHAPAGO <= '" + fechaf + "'");

                bool instertar = false;
                bool bancariaProf = false;

                string union = "";
                if (sede != "" && filtros.Count > 0) { union = " AND "; }
                string CONDITIONS = "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());

                sql = "SELECT *"
                    + "  FROM VESTADO_CUENTA_DISPERSION"//
                    + " WHERE CVE_SEDE           = '" + sede + "'"
                    + "   AND (FECHARECIBO      <> '' AND FECHARECIBO      IS NOT NULL)"
                    + "   AND (FECHA_SOLICITADO <> '' AND FECHA_SOLICITADO IS NOT NULL)"
                    + "   AND (FECHADEPOSITO is null or FECHADEPOSITO = '')"//NUEVO
                    + "   AND PUBLICADO          = 1"
                    + "   AND BLOQUEADO          = 0"
                    + "   AND CVE_TIPODEPAGO    IN ('HDI','HIN','FDI','RDI','RIN') " + CONDITIONS
                    + " UNION ALL "
                    + "SELECT *"
                    + "  FROM VESTADO_CUENTA_DISPERSION"//
                    + " WHERE CVE_SEDE           = '" + sede + "'"
                    + "   AND (FECHA_SOLICITADO <> '' AND FECHA_SOLICITADO IS NOT NULL)"
                    + "   AND (FECHADEPOSITO is null or FECHADEPOSITO = '')"//NUEVO
                    + "   AND PUBLICADO          = 1"
                    + "   AND BLOQUEADO          = 0"
                    + "   AND CVE_TIPODEPAGO    IN ('ADI','AIN') " + CONDITIONS;


                /* + ((sinNoCuenta) ? " UNION ALL "
                 + " SELECT *"
                 + "  FROM VESTADO_CUENTA_DISPERSION"//
                 + " WHERE CVE_SEDE           = '" + sede + "'"
                 + "   AND (FECHARECIBO      <> '' AND FECHARECIBO      IS NOT NULL)"
                 + "   AND (FECHA_SOLICITADO <> '' AND FECHA_SOLICITADO IS NOT NULL)"
                  + "  AND (FECHADEPOSITO is null or FECHADEPOSITO = '')"//NUEVO
                 + "   AND PUBLICADO          = 1"
                 + "   AND BLOQUEADO          = 0"
                 + "   AND CVE_TIPODEPAGO    IN ('HDI','HIN','FDI') " + CONDITIONS
                 + "   AND (CUENTACLABE IS NULL OR CUENTACLABE = '') AND (NOCUENTA IS NULL OR NOCUENTA = '')"
                 + " UNION ALL "
                 + "SELECT *"
                 + "  FROM VESTADO_CUENTA_DISPERSION"//
                 + " WHERE CVE_SEDE           = '" + sede + "'"
                 + "   AND (FECHA_SOLICITADO <> '' AND FECHA_SOLICITADO IS NOT NULL)"
                 + "   AND (FECHADEPOSITO is null or FECHADEPOSITO = '')"//NUEVO
                 + "   AND PUBLICADO          = 1"
                 + "   AND BLOQUEADO          = 0"
                 + "   AND CVE_TIPODEPAGO    IN ('ADI','AIN') " + CONDITIONS
                 + "   AND (CUENTACLABE IS NULL OR CUENTACLABE = '') AND (NOCUENTA IS NULL OR NOCUENTA    = '')" : ""
                 ); */

                res = db.getTable(sql);
                int count = 1;

                while (res.Next())
                {
                    if (!sinNoCuenta)
                    {
                        if (tipodispersion == "B")//transferencia Bancaria (banco)
                        {
                            bancariaProf = true;

                            if (res.Get("CVE_BANCO") == banco && !string.IsNullOrWhiteSpace(res.Get("NOCUENTA")))
                                instertar = true;
                            else
                                instertar = false;
                        }
                        else//transferencia interbancaria
                        {
                            bancariaProf = false;
                            if (res.Get("CVE_BANCO") != banco && !string.IsNullOrWhiteSpace(res.Get("CVE_BANCO")) && !string.IsNullOrWhiteSpace(res.Get("CUENTACLABE")))
                                instertar = true;
                            else
                                instertar = false;
                        }
                    }
                    else instertar = true;

                    if (count == 1)
                        model.TablaTemporalVacia(usuario);

                    if (instertar)
                    {
                        Dictionary<string, string> dic = prepareData(res, true, bancariaProf, usuario);

                        sql = "";
                        sql = "INSERT INTO"
                            + " DISPERSION_TMP (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ",FECHA_R)"
                            + " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "',GETDATE())";

                        db.execute(sql);
                        dic.Clear();
                    }

                    count++;

                }
            }
            catch (Exception e) { }
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string fechai = "", string fechaf = "", string banco = "", string tipodispersion = "", bool sinNoCuenta = false, string filter = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            //getTabla_TMP(fechai, fechaf, banco, tipodispersion, sinNoCuenta, filter, sesion.pkUser);
            if (filter == "")
            {
                string sql = "DELETE FROM DISPERSION_TMP WHERE USUARIO=" + sesion.pkUser;
                ResultSet res = db.getTable(sql);
            }

            DataTable table = new DataTable();

            table.TABLE = "VDISPERSION_TMP";
            table.COLUMNAS = new string[] { "IDSIU", "Nombre", "Apellidos", "Sede", "Fecha Dispersion", "Periodo", "Concepto", "Monto", "IVA", "IVA Ret", "ISR Ret", "Bancos", "Fecha Pago", "Fecha Recibo", "Tipo Transferecia", "Tipo de Pago" };
            table.CAMPOS = new string[] { "IDSIU", "NOMBRES", "APELLIDOS", "CVE_SEDE", "FECHADISPERSION", "PERIODO", "CONCEPTO", "MONTO", "MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "BANCOS", "FECHAPAGO", "FECHARECIBO", "TIPOTRANSFERENCIA", "TIPOPAGODESC" };
            table.CAMPOSSEARCH = new string[] { "IDSIU", "NOMBRES", "APELLIDOS", "CVE_SEDE", "CVE_ORIGENPAGO", "PERIODO", "CONCEPTO", "MONTO", "BANCOS", "FECHADISPERSION", "TIPOPAGODESC" };
            table.addColumnClass("IDSIU", "datatable_fixedColumn");

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "IDSIU";
            table.enabledButtonControls = false;
            table.TABLECONDICIONSQL = "USUARIO =" + sesion.pkUser + " AND (FECHADEPOSITO is null or FECHADEPOSITO = '') ";

            return table.CreateDataTable(sesion);
        }

        public string getPersonasCoincidencias(DispersionModel model)
        {

            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }
            model.sesion = sesion;
            string table = "";

            // DispersionModel model = new DispersionModel();


            ResultSet res = model.getRowsCoincidencias();

            if (res != null && res.Count > 0)
            {


                table += "<table class=\"table table-striped\">";
                table += "<thead><tr>";
                table += " <td><strong>IDSIU</strong></td><td><strong>Periodo</strong></td><td><strong>Concepto</strong></td><td><strong>Bancos</strong></td><td><strong>Fecha Pago</strong></td></tr></thead>";
                table += "<tbody>";

                //{ "IDSIU", "CVE_SEDE", "FECHADISPERSION", "PERIODO", "CONCEPTO", "MONTO", "MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "BANCOS", "FECHAPAGO", "FECHARECIBO", "TIPOTRANSFERENCIA", "TIPOPAGODESC" };

                int count = 0;
                while (res.Next())
                {

                    ResultSet res2 = model.getDispersion(res.Get("IDSIU"), res.Get("CONCEPTO"), res.Get("BANCOS"));
                    while (res2.Next())
                    {

                        table += "<tr>";
                        table += "<td>" + res2.Get("IDSIU") + "</td>";
                        table += "<td>" + res2.Get("PERIODO") + "</td>";
                        table += "<td>" + res2.Get("CONCEPTO") + "</td>";
                        table += "<td>" + res2.Get("BANCOS") + "</td>";
                        table += "<td>" + res2.Get("FECHAPAGO") + "</td>";
                        //table += "<td>" + res.Get("NUM_CONCIDENCIAS") + "</td>";
                        table += "<td>";
                        table += " </tr>";


                    }



                }

                table += "</tbody ></table>";



            }



            return table.ToString();

        }

        [HttpPost]
        public ActionResult Delete(DispersionModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            try
            {
                if (model.DeleteFechaD())
                {
                    if (model.Existe)
                    {
                        Log.write(this, "Delete", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Succes("Se ha realizado la eliminación de la fecha de dispersión con exito!") });
                    }
                    else
                    {
                        Log.write(this, "Delete", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Warning("No existen registros con esos filtros") });
                    }
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Delete") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Nombre", typeof(string));
                tbl.Columns.Add("Apellidos", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Fecha Dispersión", typeof(string));
                tbl.Columns.Add("Origen", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Concepto", typeof(string));
                tbl.Columns.Add("Monto", typeof(string));
                tbl.Columns.Add("IVA", typeof(string));
                tbl.Columns.Add("IVA Ret", typeof(string));
                tbl.Columns.Add("ISR Ret", typeof(string));
                tbl.Columns.Add("Bancos", typeof(string));
                tbl.Columns.Add("Fecha Pago", typeof(string));
                tbl.Columns.Add("Fecha Recibo", typeof(string));
                tbl.Columns.Add("Tipo Transferecia", typeof(string));
                tbl.Columns.Add("Tipo de Pago", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM VDISPERSION_TMP WHERE USUARIO =" + sesion.pkUser);

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("IDSIU"), res.Get("NOMBRES"), res.Get("APELLIDOS"), res.Get("CVE_SEDE"), res.Get("FECHADISPERSION"), res.Get("CVE_ORIGENPAGO")
                        , res.Get("PERIODO"), res.Get("CONCEPTO"), res.Get("MONTO"), res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("BANCOS"),
                        res.Get("FECHAPAGO"), res.Get("FECHARECIBO"), res.Get("TIPOTRANSFERENCIA"), res.Get("TIPOPAGODESC")/*, res.Get("CUENTA_CC"), res.Get("UUID"), res.Get("XML")*/);
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Dispersion");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:O1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:O1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Dispersion.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Dispersion", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Dispersion" + e.Message, sesion);
            }
        }

        //#EXPORT EXCEL
        public string ExportExcelDispersion()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return null; }
            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                System.Data.DataTable tbl2 = new System.Data.DataTable();
                ResultSet res = db.getTable("SELECT * FROM VDISPERSION_TMP WHERE USUARIO =" + sesion.pkUser);


                long usuario = sesion.pkUser;
                string nombrefile = "Bancos_traspaso" + usuario;
                string path = Server.MapPath("~/Upload/Dispersion") + @"\" + nombrefile;
                int i = 0;
                while (System.IO.File.Exists(path + i + ".xlsx"))
                    i++;



                FileInfo file = new FileInfo(Path.GetFileName(path + i + ".xlsx"));



                if (res.Count > 0)
                {

                    /* _____________________________ bandera recalculo ___________________________ */

                    //Marca con una bandera los conceptos con recalculo del ISR Ret
                    // bandera_recalculo(res.Get("IDSIU"), res.Get("CVE_SEDES"));

                    /* ________________________________________________________ */



                    if (Request.Params["banco"] == "044")
                    {//SCOTIABANK 
                        tbl.Columns.Add("C1", typeof(string));
                        tbl.Columns.Add("Tipo", typeof(string));
                        tbl.Columns.Add("Banco", typeof(string));
                        tbl.Columns.Add("CLABE", typeof(string));
                        tbl.Columns.Add("CuentaBanco", typeof(string));
                        tbl.Columns.Add("Neto_a_Pagar", typeof(string));
                        tbl.Columns.Add("neto_a_pagar(con_formato)", typeof(string));
                        tbl.Columns.Add("IDSIU", typeof(string));
                        tbl.Columns.Add("Nombre_del_profesor", typeof(string));
                        tbl.Columns.Add("ref_pago", typeof(string));
                        tbl.Columns.Add("Concepto", typeof(string));
                        tbl.Columns.Add("dias_vig", typeof(string));
                        tbl.Columns.Add("FechaP", typeof(string));
                        tbl.Columns.Add("FechaRecibo", typeof(string));
                    }
                    else
                    {//SANTANDER
                        tbl.Columns.Add("Cve Banco", typeof(string));
                        tbl.Columns.Add("CLABE", typeof(string));//CuentaBanco
                        tbl.Columns.Add("Santander_Profesor", typeof(string));
                        tbl.Columns.Add("Santander_Apellidos", typeof(string));
                        //tbl.Columns.Add("Santander_ApellidoP", typeof(string));
                        //tbl.Columns.Add("Santander_ApellidoM", typeof(string));                     
                        tbl.Columns.Add("Santander_Nombre", typeof(string));
                        tbl.Columns.Add("Santander_RFC_Física", typeof(string));
                        tbl.Columns.Add("Santander_RFC_Moral", typeof(string));
                        tbl.Columns.Add("Santander_Cuenta", typeof(string));
                        tbl.Columns.Add("Santander_Monto", typeof(string));
                        tbl.Columns.Add("Santander_Concepto", typeof(string));
                        tbl.Columns.Add("FechaContrato", typeof(string)); tbl.Columns.Add("FechaP", typeof(string));
                        tbl.Columns.Add("FechaRecibo", typeof(string));

                        //pestaña 2(UAP)
                        tbl2.Columns.Add("NUMERO", typeof(string));
                        tbl2.Columns.Add("APELLIDO PATERNO", typeof(string));
                        tbl2.Columns.Add("APELLIDO MATERNO ", typeof(string));
                        tbl2.Columns.Add("NOMBRE", typeof(string));
                        tbl2.Columns.Add("CUENTA", typeof(string));
                        tbl2.Columns.Add("IMPORTE", typeof(string));
                        tbl2.Columns.Add("CONCEPTO", typeof(string));
                        tbl2.Columns.Add("MONTO", typeof(string));
                        tbl2.Columns.Add("MONTO_IVA", typeof(string));
                        tbl2.Columns.Add("MONTO_IVARET", typeof(string));
                        tbl2.Columns.Add("MONTO_ISRRET", typeof(string));
                        tbl2.Columns.Add("MONTO_NETO", typeof(string));
                        tbl2.Columns.Add("FOLIO_CHEQUE", typeof(string));
                        tbl2.Columns.Add("RFC", typeof(string));
                        tbl2.Columns.Add("FOLIO_FISCAL", typeof(string));
                    }



                    DispersionModel bdrecibos = new DispersionModel();
                    bdrecibos.sesion = sesion;

                    bdrecibos.IdSede = Request.Params["sedes"];
                    bdrecibos.fechai = Request.Params["fechai"];
                    bdrecibos.fechaf = Request.Params["fechaf"];
                    bdrecibos.IdTransferencia = Request.Params["tipodispersion"];
                    bdrecibos.banco = Request.Params["banco"];

                    string nombres_profesor = "";
                    string c1 = "04";
                    int count = 0;
                    string tipoT = "12";
                    if (bdrecibos.IdTransferencia == "I")
                    {
                        tipoT = "09";
                    }
                    else if (bdrecibos.IdTransferencia == "B")
                    {
                        tipoT = "01";
                    }

                    int x;

                    while (res.Next())
                    {
                        string CuentaBancaria = "";
                        CuentaBancaria = res.Get("NOCUENTA");

                        if (Request.Params["banco"] == "044")
                        {//SCOTIBANK
                            count++;
                            nombres_profesor = (res.Get("APELLIDOS") + ' ' + res.Get("NOMBRES"));//  res.Get("BENEFICIARIO_PENSIONADO_P") + ' ' +res.Get("BENEFICIARIO_PENSIONADO_M") + ' ' + res.Get("NOMBRES")

                            tbl.Rows.Add(c1, tipoT
                           , res.Get("CVE_BANCO"), res.Get("CUENTACLABE"), CuentaBancaria//
                           , res.Get("BANCOS"), res.Get("BANCOSX100"), res.Get("IDSIU")//PROFESOR MONTO POR BANCOS
                           , nombres_profesor.ToUpper()
                           , count, res.Get("ID_ESQUEMA") + '/' + res.Get("ESQUEMA") + '/' + res.Get("CONCEPTO") + '/' + res.Get("PKCONCEPTOPAGO")// igual que deposito  REF_PAGO
                           , "60"
                           , res.Get("FECHAPAGO") == null || res.Get("FECHAPAGO") == "" ? "" : res.GetDateTime("FECHAPAGO").ToString("yyyyMMdd")
                           , res.Get("FECHARECIBO") == null || res.Get("FECHARECIBO") == "" ? "" : res.GetDateTime("FECHARECIBO").ToString("yyyy/MM/dd")
                           );
                        }
                        else
                        {   //SANTANDER
                            tbl.Rows.Add(res.Get("CVE_BANCO"), res.Get("CUENTACLABE")
                            , res.Get("IDSIU"), res.Get("APELLIDOS").ToUpper(), res.Get("NOMBRES").ToUpper()
                            , res.Get("RFCFISICA"), res.Get("RFCMORAL"), CuentaBancaria
                            , res.Get("BANCOS"), res.Get("ID_ESQUEMA") + '/' + res.Get("ESQUEMA") + '/' + res.Get("CONCEPTO") + '/' + res.Get("PKCONCEPTOPAGO")// igual a deposito MONTO POR BANCOS
                            , res.Get("FECHADEENTREGA") == null || res.Get("FECHADEENTREGA") == "" ? "" : res.GetDateTime("FECHADEENTREGA").ToString("yyyy/MM/dd")   //fechaContrato
                            , res.Get("FECHAPAGO") == null || res.Get("FECHAPAGO") == "" ? "" : res.GetDateTime("FECHAPAGO").ToString("yyyyMMdd")//igual q deposito
                            , res.Get("FECHARECIBO") == null || res.Get("FECHARECIBO") == "" ? "" : res.GetDateTime("FECHARECIBO").ToString("yyyy/MM/dd")
                            );


                            /**********************************/


                            //TAB2 (UAP)
                            string[] ap_separados;
                            ap_separados = res.Get("APELLIDOS").ToUpper().Split(' ');
                            string apellido1_ = "";
                            string apellido2_ = "";
                            x = 1;
                            if (ap_separados.Length > 0)
                            {
                                apellido1_ = ap_separados[0];

                                if (ap_separados.Length > 1)
                                    while (x < ap_separados.Length)
                                        apellido2_ += ap_separados[x++] + " ";
                            }



                            if (apellido2_.Length > 0)
                                apellido2_ = apellido2_.Remove(apellido2_.Length - 1);





                            tbl2.Rows.Add(res.Get("IDSIU"), apellido1_
                            , apellido2_
                            , res.Get("NOMBRES").ToUpper()
                            , CuentaBancaria
                            , res.Get("BANCOS"), res.Get("ID_ESQUEMA") + '/' + res.Get("ESQUEMA") + '/' + res.Get("CONCEPTO") + '/' + res.Get("PKCONCEPTOPAGO")// igual a deposito MONTO POR BANCOS                          
                            , res.Get("MONTO"), res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("BANCOS"), res.Get("FOLIO_CHEQUE"), res.Get("RFC"), res.Get("FOLIO_FISCAL"));
                            /**********************************/
                        }


                    }




                    using (ExcelPackage pck = new ExcelPackage())
                    {


                        //Create the worksheet
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("todos");
                        string tipoTransfernecia = "";
                        if (bdrecibos.IdTransferencia == "B") { tipoTransfernecia = "Transferencia Bancaria"; }//Transferencia Bancaria (Banco)
                        else { tipoTransfernecia = "Transferencia interbancaria"; }//Transferencia interbancaria 

                        ws.Cells["A8"].LoadFromDataTable(tbl, true);

                        if (Request.Params["banco"] == "044")
                        {//SCOTIABANK   
                            ws.Cells["A7"].Value = "DATOS REQUERIDOS PARA DISPERSAR CON SCOTIBANK  (" + tipoTransfernecia + ")";
                            ws.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            ws.Cells["A7:L7"].Merge = true;

                            using (ExcelRange rng = ws.Cells["A7:L7"])
                            {
                                //  rng.Style.Font.VerticalAlign
                                rng.Style.Font.Bold = true;
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 240, 61));  //Set color to dark blue
                                rng.Style.Font.Color.SetColor(Color.Black);
                            }

                            ws.Cells["A8:M8"].AutoFitColumns();//AF1

                            //Format the header for column 1-3
                            using (ExcelRange rng = ws.Cells["A8:L8"])
                            {
                                rng.Style.Font.Bold = true;
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                                rng.Style.Font.Color.SetColor(Color.White);

                            }

                            using (ExcelRange rng = ws.Cells["M8:N8"])//AF1
                            {
                                rng.Style.Font.Bold = true;
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 14, 14));  //Set color to dark blue
                                rng.Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        else
                        {//SANTANDER
                            ws.Cells["A7"].Value = "DATOS REQUERIDOS PARA DISPERSAR CON SANTANDER  (" + tipoTransfernecia + ")";
                            //ws.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            ws.Cells["A7:K7"].Merge = true;

                            using (ExcelRange rng = ws.Cells["A7:K7"])
                            {
                                rng.Style.Font.Bold = true;
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 240, 61));  //Set color to dark blue
                                rng.Style.Font.Color.SetColor(Color.Black);
                                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            ws.Cells["A8:M8"].AutoFitColumns();//AG1//A:8

                            //Format the header for column 1-3
                            using (ExcelRange rng = ws.Cells["A8:K8"])
                            {
                                rng.Style.Font.Bold = true;
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 133, 32));  //Set color to dark blue
                                rng.Style.Font.Color.SetColor(Color.White);
                            }

                            using (ExcelRange rng = ws.Cells["L8:M8"])//AG1
                            {
                                rng.Style.Font.Bold = true;
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 14, 14));  //Set color to dark blue
                                rng.Style.Font.Color.SetColor(Color.White);
                            }


                            if (bdrecibos.IdTransferencia == "B")
                            {

                                /***********************************************************************/
                                //TAB2 (UAP)
                                //checar validaciones (si se va a ver en scotiabank en interbancaria... )
                                ExcelWorksheet ws2 = pck.Workbook.Worksheets.Add("UAP");
                                ws2.Cells["A8"].LoadFromDataTable(tbl2, true);


                                ws2.Cells["A7"].Value = "DATOS REQUERIDOS PARA DISPERSAR CON SANTANDER  (" + tipoTransfernecia + ")";
                                //ws.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                                ws2.Cells["A7:O7"].Merge = true;

                                using (ExcelRange rng = ws2.Cells["A7:O7"])
                                {
                                    rng.Style.Font.Bold = true;
                                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 133, 32)); // 240, 240, 61
                                    rng.Style.Font.Color.SetColor(Color.White);
                                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }


                                ws2.Cells["A8:O8"].AutoFitColumns();
                                ws2.Cells["E9:G9"].AutoFitColumns();

                                using (ExcelRange rng = ws2.Cells["A8:O8"])
                                {
                                    rng.Style.Font.Bold = true;
                                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(130, 131, 133)); // 240, 240, 61
                                    rng.Style.Font.Color.SetColor(Color.White);
                                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }



                                /***********************************************************************/



                            }




                        }

                        //Example how to Format Column 1 as numeric 
                        using (ExcelRange col = ws.Cells[2, 1, 2 + tbl.Rows.Count, 1])
                        {
                            col.Style.Numberformat.Format = "#,##0.00";
                            col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }


                        byte[] data = pck.GetAsByteArray();

                        //Write it back to the client
                        /* Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                         Response.AddHeader("content-disposition", "attachment;  filename=Bancos_traspaso.xlsx");
                         Response.BinaryWrite(pck.GetAsByteArray());*/


                        bdrecibos.Save();//GUARDA LA FECHA DE DISPERSIÓN

                        // pck.SaveAs(file);



                        System.IO.File.WriteAllBytes(path + i + ".xlsx", data);

                        return nombrefile + i + ".xlsx";


                    }
                    Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Recibos (Bancos_traspaso.xlsx)", sesion);
                }
                else
                {// si no tiene datos

                    tbl.Columns.Add("NO EXISTEN REGISTROS CON FECHA DE PAGO Y/O ALGUN FILTRO!", typeof(string));
                    tbl.Rows.Add("");
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("todos");
                        ws.Cells["F1"].LoadFromDataTable(tbl, true);
                        ws.Cells["F1:F1"].AutoFitColumns();

                        using (ExcelRange rng = ws.Cells["F1:F1"])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 133, 32));//255, 229, 32 yellow
                            rng.Style.Font.Color.SetColor(Color.White);
                        }

                        using (ExcelRange col = ws.Cells[2, 1, 2 + tbl.Rows.Count, 1])
                        {
                            col.Style.Numberformat.Format = "#,##0.00";
                            col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }

                        /*    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;  filename=Bancos_traspaso.xlsx");
                            Response.BinaryWrite(pck.GetAsByteArray());*/


                        // pck.SaveAs(file);

                        byte[] data = pck.GetAsByteArray();

                        System.IO.File.WriteAllBytes(path + i + "_vacio.xlsx", data);

                        return nombrefile + i + "_vacio.xlsx";

                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Recibos (Bancos_traspaso.xlsx)" + e.Message, sesion);
            }


            return null;

        }

        public void getResponseExcel(string nombrefile = "")
        {


            string path_name = Server.MapPath("~/Upload/Dispersion") + @"\" + nombrefile;


            using (ExcelPackage pck = new ExcelPackage())
            {
                /* Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                 Response.AddHeader("content-disposition", "attachment;  filename=Bancos_traspaso.xlsx");
                 Response.BinaryWrite(System.Convert.FromBase64String(path_name));*/


                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AppendHeader("content-disposition", "attachment;  filename=Bancos_traspaso.xlsx");
                Response.TransmitFile(path_name); Response.End();
                System.IO.File.Delete(path_name);


                // WebClient webClient = new WebClient();
                // webClient.DownloadFile



            }




        }

        [HttpGet]
        public string getBanco(string Banco = "")
        {
            DispersionModel model = new DispersionModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in model.getBanco())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }

        [HttpGet]
        public string getTipoDispersion(string tdispersion = "")
        {
            DispersionModel model = new DispersionModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in model.getTipoDispersion())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }
    }
}