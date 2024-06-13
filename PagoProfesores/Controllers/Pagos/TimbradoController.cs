using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;
using System.Configuration;
using System.Text;

namespace PagoProfesores.Controllers.Pagos
{
    public class TimbradoController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public TimbradoController()
        {
            db = new database();

            string[] scripts = {
                "js/Pagos/GestiondePagos/timbrado.js",
                "js/Pagos/GestiondePagos/download.js"
            };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10124,  Element = "Controller" }, //PERMISO ACCESO 
                 new Factory.Privileges { Permiso = 10125,  Element = "formbtnconsultar" }, //PERMISO EDITAR 
                 new Factory.Privileges { Permiso = 10126,  Element = "formbtntimbrado" }, //PERMISO AGREGAR                
            };
        }

        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Pagos", "Gestión de Pagos", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "", "ASC");
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Timbrado Start", LOG.CONSULTA, "Ingresa pantalla Timbrado", sesion);

            return View(Factory.View.Access + "Pagos/GestiondePagos/Timbrado/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string fechai = "", string fechaf = "", string filter = "")//string banco = "", string tipodispersion = ""
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            DataTable table = new DataTable();
            string retorno = string.Empty;

            table.TABLE = "V_TIMBRADO_ASIMILADOS";
            table.COLUMNAS = new string[] { "", "ID Siu", "Receptor", "Mensaje de error", "Pdf", "Xml", "Fecha solicitado", "Fecha recibo", "Fecha depósito", "Fecha pago", "Concepto", "Monto" };
            table.CAMPOS = new string[] { "ID_ESTADODECUENTA", "CVE_TIPOTRANSFERENCIA", "UUID", "C", "IDSIU", "ReceptorRazonSocial", "MENSAJE_ERROR_ULTIMO_TIMBRADO", "PDF", "XML", "FECHA_SOLICITADO", "FECHARECIBO", "FECHADEPOSITO", "FILTROFECHA", "CONCEPTO", "ConceptosTOTAL" };
            table.CAMPOSSEARCH = new string[] { "IDSIU", "Timbrado" };
            string[] camposhidden = { "ID_ESTADODECUENTA", "CVE_TIPOTRANSFERENCIA", "UUID" };

            table.dictColumnFormat.Add("C", delegate (string value, ResultSet res) {
                retorno = "";

                if ((res.Get("FECHA_SOLICITADO") != null && res.Get("FECHA_SOLICITADO") != "") && (res.Get("UUID") == null || res.Get("UUID") == "") && (res.Get("FECHADEPOSITO") != null && res.Get("FECHADEPOSITO") != ""))
                    retorno = "<input type=\"checkbox\" name=\"cbxSolicitado\" id=\"" + res.Get("ID_ESTADODECUENTA").ToString() + "\" value=\"" + res.Get("ID_ESTADODECUENTA").ToString() + "\" />";
                else if ((res.Get("FECHA_SOLICITADO") != null && res.Get("FECHA_SOLICITADO") != "") && (res.Get("UUID") != null || res.Get("UUID") != ""))
                    retorno = "<span class='fa fa-check'></span>";
                else
                    retorno = "<span class='fa fa-minus'></span>";
                //retorno = "<span class='fa fa-check-square-o'></span>";

                return retorno;
            });

            table.addColumnClass("MENSAJE_ERROR_ULTIMO_TIMBRADO", "datatable_fixedColumn");

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_ESTADODECUENTA";
            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";
            table.CAMPOSHIDDEN = camposhidden;

            List<string> filtros = new List<string>();
            if (fechai != "") filtros.Add("FILTROFECHA >= '" + fechai + "'");
            if (fechaf != "") filtros.Add("FILTROFECHA <= '" + fechaf + "'");

            string union = "";
            if (filter != "" && filtros.Count > 0) { union = " AND "; }

            table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());
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
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Timbrado", typeof(string));
                tbl.Columns.Add("EmisorRazonSocial", typeof(string));
                tbl.Columns.Add("MENSAJE_ERROR_ULTIMO_TIMBRADO", typeof(string));
                tbl.Columns.Add("PDF", typeof(string));
                tbl.Columns.Add("XML", typeof(string));

                var sede = Request.Params["sedes"];
                var fechai = Request.Params["fechai"];
                var fechaf = Request.Params["fechaf"];

                ResultSet res = db.getTable("SELECT * FROM V_TIMBRADO_ASIMILADOS WHERE CVE_SEDE = '" + sede + "' AND FILTROFECHA  >='" + fechai + "' AND FILTROFECHA  <='" + fechaf + "'");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("IDSIU"), res.Get("Timbrado"), res.Get("EmisorRazonSocial")
                        , res.Get("MENSAJE_ERROR_ULTIMO_TIMBRADO"), res.Get("PDF"), res.Get("XML"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Timbrado");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:F1"].AutoFitColumns();

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:F1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Timbrado.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Timbrado", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Timbrado" + e.Message, sesion);
            }
        }

        public void base64toPDF(string b)
        {
            //  MemoryStream workStream = new MemoryStream();
            byte[] byteInfo = Convert.FromBase64String(b);
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition", "attachment; filename= " + Server.HtmlEncode("abc.pdf"));
            Response.ContentType = "APPLICATION/pdf";
            Response.BinaryWrite(byteInfo);
        }

        public string TimbradoTest(string fechai, string fechaf, string cveSede)
        {
            int ok = 0;
            int fallidos = 0;
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            string[,] sXML = RegresafacturasaTimbrarXML(fechai, fechaf);

            using (var client = new srv_mySuiteTest.FactWSFrontSoapClient("FactWSFrontSoap1"))
            {
                string server = ConfigurationManager.AppSettings["serverMySuite"];

                TimbradoMySuiteModel objMySuite = new TimbradoMySuiteModel();
                objMySuite.camp_code = cveSede;
                objMySuite.server = server;

                for (int i = 0; i <= sXML.Length - 1; i++)
                {
                    try
                    { // debido a que el índice se sale de la matriz
                        if (sXML[i, 0] != null && sXML[i, 1] != null)
                        {
                            if (objMySuite.getDatosTimbradoMySuite())
                            {
                                //srv_mySuiteTest.TransactionTag tag = client.RequestTransaction("cb6b2b32-2d5c-4c82-adc0-e1375c95a1a2",
                                //    "CONVERT_NATIVE_XML",
                                //    "MX",
                                //    "AAA010101AAA",
                                //    "cb6b2b32-2d5c-4c82-adc0-e1375c95a1a2",
                                //    "ISCOM",
                                //    sXML[i, 1], "PDF XML", "");
                                srv_mySuiteTest.TransactionTag tag = client.RequestTransaction(objMySuite.requestor,
                                    objMySuite.xtransaction,
                                    objMySuite.country,
                                    objMySuite.rfcentity,
                                    objMySuite.user_r,
                                    objMySuite.username,
                                    sXML[i, 1], "PDF XML", "");

                                if (tag.Response.Result)
                                {
                                    var _x = guarda_regreso_timbrado(sXML[i, 0], tag.ResponseData.ResponseData1, tag.ResponseData.ResponseData3, "OK", tag.Response.Identifier.DocumentGUID);
                                    ok = ok + _x;
                                }
                                else
                                {
                                    fallidos++;
                                    guarda_regreso_timbrado_error(sXML[i, 0], tag.Response.Data);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            /*string date_inicio = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            for (int i = 0; i <= s.Length -1; i++)
            {
                try
                { // debido a que el índice se sale de la matriz
                    if (s[i, 0] != null && s[i, 1] != null)
                    {
                        string respuesta = LlamadaPost(s[i, 1]);
                        dynamic dynObj = JsonConvert.DeserializeObject(respuesta);

                        if (dynObj.result.Value)
                        {
                            //ok++;
                            var _x = guarda_regreso_timbrado(s[i, 0], dynObj.xml.Value, dynObj.pdf.Value, respuesta);
                            ok = ok + _x;
                        }
                        else
                        {
                            fallidos++;
                            guarda_regreso_timbrado_error(s[i, 0], dynObj.errormsg.Value);
                        }
                    }
                }
                catch { }
            }*/

            return "Timbrados correctamente " + ok.ToString() + " Timbrados con error " + fallidos.ToString();

            //string date_fin = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //Console.WriteLine("s");
        }

        public string Timbrar(string fechai, string fechaf, string cveSede, string IDs = "")
        {
            int ok = 0;
            int fallidos = 0;
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            string[,] sXML = RegresafacturasaTimbrarXML(fechai, fechaf, IDs);

            using (var client = new srv_mySuiteTest.FactWSFrontSoapClient("FactWSFrontSoap1"))
            {
                string server = ConfigurationManager.AppSettings["serverMySuite"];

                TimbradoMySuiteModel objMySuite = new TimbradoMySuiteModel();
                objMySuite.camp_code = cveSede;
                objMySuite.server = server;

                for (int i = 0; i <= sXML.Length - 1; i++)
                {
                    try
                    { // debido a que el índice se sale de la matriz
                        if (sXML[i, 0] != null && sXML[i, 1] != null)
                        {
                            if (objMySuite.getDatosTimbradoMySuite())
                            {
                                srv_mySuiteTest.TransactionTag tag = client.RequestTransaction(objMySuite.requestor,
                                    objMySuite.xtransaction,
                                    objMySuite.country,
                                 objMySuite.rfcentity,
                                    objMySuite.user_r,
                                   objMySuite.username,
                                    sXML[i, 1], "PDF XML", "");

                                if (tag.Response.Result)
                                {
                                    var _x = guarda_regreso_timbrado(sXML[i, 0], tag.ResponseData.ResponseData1, tag.ResponseData.ResponseData3, "OK", tag.Response.Identifier.DocumentGUID);
                                    ok = ok + _x;
                                }
                                else
                                {
                                    fallidos++;
                                    guarda_regreso_timbrado_error(sXML[i, 0], tag.Response.Data);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            return "Timbrados correctamente " + ok.ToString() + " Timbrados con error " + fallidos.ToString();
        }

        public void guarda_regreso_timbrado_error(string id_estadocuenta, string s)
        {
            string rs = s.Replace("'", " ");
            string rs1 = rs.Replace("\"", " ");
            string rs2 = rs1.Replace("\\", " ");

            string sql = "INSERT INTO dbo.BITACORA_TIMBRADO (ID_ESTADOCUENTA,MENSAJE,FECHA,USUARIO) values ('" + id_estadocuenta + "','" + rs2 + "',SYSDATETIME(),'" + sesion.pkUser.ToString() + "')";
            db.execute(sql);
        }

        public int guarda_regreso_timbrado(string id_estadocuenta, string xml, string pdf, string s, string uuid = null)
        {
            try
            {
                if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

                string sql = "UPDATE ESTADODECUENTA SET XML = '" + xml + "', PDF = '" + pdf + "', UUID = '" + uuid + "', FECHARECIBO = SYSDATETIME() , FECHAEMISION = SYSDATETIME() WHERE ID_ESTADODECUENTA = " + id_estadocuenta;
                db.execute(sql);

                sql = "UPDATE BITACORA_TIMBRADO SET MENSAJE = '', FECHA_M = GETDATE(), USUARIO = '" + sesion.pkUser.ToString() + "' WHERE ID_ESTADOCUENTA = " + id_estadocuenta;
                db.execute(sql);

                return 1;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public string ObtieneMsgErrorBitacora(string id_estadocuenta)
        {
            string msj = "ND";
            string sql = "SELECT ISNULL(MENSAJE,'ND') MSJ FROM BITACORA_TIMBRADO where WHERE ID_ESTADOCUENTA=" + id_estadocuenta + "AND FECHA = (SELECT MAX(FECHA) FROM BITACORA_TIMBRADO WHERE ID_ESTADOCUENTA= " + id_estadocuenta + ");";
            ResultSet res = db.getTable(sql);
            int i = 0;
            res.ReStart();
            while (res.Next())
            {
                msj = res.Get("MSJ");
            }
            return msj;
        }

        public string[,] RegresafacturasaTimbrarXML(string fechai, string fechaf, string IDs = "")
        {
            string paEnviarCorreo = ConfigurationManager.AppSettings["enviarCorreo"];
            string correoReceptor = ConfigurationManager.AppSettings["correoReceptor"];
            string server = ConfigurationManager.AppSettings["serverMySuite"];//TEST O PROD

            //string sql = "SELECT * from  dbo.V_TIMBRADO_ASIMILADOS where FILTROFECHA >= '" + fechai + "' AND FILTROFECHA <= '" + fechaf + "' AND (FECHA_SOLICITADO IS NOT NULL AND FECHA_SOLICITADO != '') AND (FECHARECIBO IS NULL OR FECHARECIBO = '')";
            string sql = "SELECT * from  dbo.V_TIMBRADO_ASIMILADOS where FILTROFECHA >= '" + fechai + "' AND FILTROFECHA <= '" + fechaf + "' AND (FECHA_SOLICITADO IS NOT NULL AND FECHA_SOLICITADO != '') AND (UUID IS NULL OR UUID = '') ";
            if (IDs != "" && IDs != null)
            {
                sql += " and id_estadodecuenta in (" + IDs + ")";
            }
            sql += " order by id_estadodecuenta";
            ResultSet res = db.getTable(sql);

            string[,] recibos = new string[res.Count, 2];
            int i = 0;

            res.ReStart();
            while (res.Next())
            {
                recibos[i, 0] = res.Get("ID_ESTADODECUENTA");
                recibos[i, 1] = ArmaCadenaXmlTimbrar(
                   ((server == "PROD") ? res.Get("EmisorRFC").ToUpper() : "JES900109Q90")
                   , ((server == "PROD") ? quitaAcentos(res.Get("EmisorRazonSocial").ToUpper()) : quitaAcentos("jímenez estrada SALAS A A").ToUpper())
                    , sesion.pkUser.ToString()
                    , res.Get("ID_ESTADODECUENTA")
                    , ((server == "PROD") ? res.Get("DomicilioDeEmisionCodigoPostal").ToUpper() : "01030")
                    , ((paEnviarCorreo == "1") ? ((server == "PROD") ? res.Get("EmailReceptor") : correoReceptor) : "")
                    , ((server == "PROD") ? res.Get("ReceptorRFC").ToUpper() : "CACX7605101P8")
                    , ((server == "PROD") ? quitaAcentos(res.Get("ReceptorRazonSocial").ToUpper()) : quitaAcentos("xochilt CASAS CHAVEZ").ToUpper())
                    , res.Get("ConceptosSubtotal")
                    , res.Get("ConceptosTOTAL")
                    , res.Get("ConceptosTOTAL") // totalEnLetra
                    , res.Get("FILTROFECHA") // <- ok, es fecha_pago
                                             //, res.Get("FECHADEPOSITO")
                    , res.Get("CURP") //CURP
                    , res.Get("SEGUROSOCIAL") //SS
                    , res.Get("BANCO") // banco
                    , (res.Get("NOCUENTA") == "" || res.Get("NOCUENTA") == null ? res.Get("CUENTACLABE") : res.Get("NOCUENTA")) //cuentaBancaria
                    , res.Get("CVE_ENTFED") //xClaveEntFed
                    , res.Get("MONTO_ISRRET") //totalDescuentos
                    , res.Get("IDSIU") //NumEpleado
                    , res.Get("ConceptosMetodoPago") // método de pago: transferencia o cheque
                    , res.Get("CVE_TIPOTRANSFERENCIA") // tipo de transferencia: transferencia, cheque o transferenica bancaria
                    , res.Get("DomicilioDeRecepcionCodigoPostal") // CÓDIGO POSTAL FISCAL
                    );
                i++;
            }
            return recibos;
        }

        public string quitaAcentos(string nombre)
        {
            nombre = nombre.Replace("Á", "A");
            nombre = nombre.Replace("É", "E");
            nombre = nombre.Replace("Í", "I");
            nombre = nombre.Replace("Ó", "O");
            nombre = nombre.Replace("Ú", "U");
            nombre = nombre.Replace("á", "a");
            nombre = nombre.Replace("é", "e");
            nombre = nombre.Replace("í", "i");
            nombre = nombre.Replace("ó", "o");
            nombre = nombre.Replace("ú", "u");


            return nombre;
        }

        public string ArmaCadenaXmlTimbrar(
              string xRFCEmisor
            , string xRazonSocialEmisor
            , string xUsuario
            , string xNumeroInterno
            , string xLugarExpedicion
            , string xEmailReceptor
            , string xRFCReceptor
            , string xNombreReceptor
            , string xSubTotal
            , string xTotal
            , string xTotalEnLetra
            , string xFechaPago
            , string xCURP
            , string xNumeroSS
            , string xBanco
            , string xCuentaBancaria
            , string xClaveEntFed
            , string xTotalImpuestosRetenidos
            , string xNumEmpleado
            , string xMetodoPago
            , string xTipoTransferencia
            , string xCP
            )
        {
            //aquí vamos a obtener la fecha de pago inicial y la fecha de pago final, así como la cantidad de días pagados
            //xNumeroInterno -> IdEdoCta

            TimbradoModel tModel = new TimbradoModel();
            System.Data.DataTable dtDP = new System.Data.DataTable();
            string vFechaInicalPago = string.Empty;
            string vFechaFinalPago = string.Empty;
            string vFechaPago = string.Empty;

            dtDP = tModel.getDiasPagados(xNumeroInterno);
            vFechaPago = dtDP.Rows[0][0].ToString();
            vFechaInicalPago = dtDP.Rows[0][1].ToString();
            vFechaFinalPago = dtDP.Rows[0][2].ToString();

            string server = ConfigurationManager.AppSettings["serverMySuite"];//TEST O PROD
            xCP = ((server == "PROD") ? xCP : "01030");  //DESCOMENTAR
                                                         // xCP = ((server == "PROD") ? xCP : xCP);  //QUITAR

            StringBuilder stringXML = new StringBuilder()
                //facturación version4.0
                .Append("<fx:FactDocMX xmlns:fx=\"http://www.fact.com.mx/schema/fx\" ")
                .Append("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ")
                  .Append("xsi:schemaLocation=\"http://www.fact.com.mx/schema/fx http://www.mysuitemex.com/fact/schema/fx_2010_g.xsd\">  ")
                .Append("<fx:Version>8</fx:Version>")

                .Append("<fx:Identificacion>")
                .Append("<fx:CdgPaisEmisor>MX</fx:CdgPaisEmisor>")
                .Append("<fx:TipoDeComprobante>RECIBO_DE_NOMINA</fx:TipoDeComprobante>")
                   .Append("<fx:RFCEmisor>" + xRFCEmisor.ToUpper() + "</fx:RFCEmisor>")//   .Append("<fx:RFCEmisor>" + ((server == "PROD") ? xRFCEmisor : "JES900109Q90")  + "</fx:RFCEmisor>")
                   .Append("<fx:RazonSocialEmisor>" + xRazonSocialEmisor.ToUpper() + "</fx:RazonSocialEmisor>")//.Append("<fx:RazonSocialEmisor>JIMENEZ ESTRADA SALAS A A</fx:RazonSocialEmisor>")
                .Append("<fx:Usuario>" + xUsuario.ToUpper() + "</fx:Usuario>")
                .Append("<fx:NumeroInterno>" + xNumeroInterno + "</fx:NumeroInterno>")
                   .Append("<fx:Exportacion>01</fx:Exportacion>")
                   .Append("<fx:LugarExpedicion>" + xLugarExpedicion + "</fx:LugarExpedicion>")//.Append("<fx:LugarExpedicion>03300</fx:LugarExpedicion>")
                .Append("</fx:Identificacion>")

                .Append("<fx:Procesamiento>")
                .Append("<fx:Dictionary name=\"email\">")
                .Append("<fx:Entry k=\"from\" v=\"ACCOUNT_OWNER\"/>")
                //.Append("<fx:Entry k=\"to\" v=\"adrian.gutierrez@anahuac.mx\"/>")  //" + xEmailReceptor + "
                //.Append("<fx:Entry k=\"bcc\" v=\"jose.ruiz@anahuac.mx\"/>")
                .Append("<fx:Entry k=\"to\" v=\"" + xEmailReceptor + "\"/>")  //" + xEmailReceptor + "
                .Append("</fx:Dictionary>")
                .Append("</fx:Procesamiento>")

                .Append("<fx:Emisor>")
                .Append("<fx:RegimenFiscal>")
                .Append("<fx:Regimen>603</fx:Regimen>")
                .Append("</fx:RegimenFiscal>")
                .Append("</fx:Emisor>")

                .Append("<fx:Receptor>")
                .Append("<fx:CdgPaisReceptor>MX</fx:CdgPaisReceptor>")
                   .Append("<fx:RFCReceptor>" + xRFCReceptor.ToUpper() + "</fx:RFCReceptor>")  //.Append("<fx:RFCReceptor>" +  ((server == "PROD") ? xRFCReceptor : "FUNK671228PH6")  + "</fx:RFCReceptor>") ///xRFCReceptor CAMBIAR PARA PRODUCCIÓN -- AAAA010101AAA para test   " + xRFCReceptor + 
                   .Append("<fx:NombreReceptor>" + quitaAcentos(xNombreReceptor.ToUpper()) + "</fx:NombreReceptor>")//.Append("<fx:NombreReceptor>XOCHILT CASAS CHAVEZ</fx:NombreReceptor>")
                   .Append("<fx:DomicilioFiscalReceptor>" + xCP + "</fx:DomicilioFiscalReceptor>")//   .Append("<fx:DomicilioFiscalReceptor>01030</fx:DomicilioFiscalReceptor>")
                .Append("<fx:RegimenFiscalReceptor>605</fx:RegimenFiscalReceptor>")

                .Append("<fx:UsoCFDI>CN01</fx:UsoCFDI>")
                .Append("</fx:Receptor>")

                .Append("<fx:Conceptos>")
                .Append("<fx:Concepto>")
                .Append("<fx:Cantidad>1</fx:Cantidad>")
                .Append("<fx:ClaveUnidad>ACT</fx:ClaveUnidad>")
                .Append("<fx:ClaveProdServ>84111505</fx:ClaveProdServ>")
                .Append("<fx:Descripcion>Pago de nómina</fx:Descripcion>")
                .Append("<fx:ValorUnitario>" + xSubTotal + "</fx:ValorUnitario>")
                .Append("<fx:Importe>" + xSubTotal + "</fx:Importe>")
                .Append("<fx:Descuento>" + xTotalImpuestosRetenidos + "</fx:Descuento>")
                .Append("<fx:ObjetoImp>01</fx:ObjetoImp>")
                .Append("</fx:Concepto>")
                .Append("</fx:Conceptos>")
                .Append("<fx:Totales>")
                .Append("<fx:Moneda>MXN</fx:Moneda>")
                .Append("<fx:SubTotalBruto>" + xSubTotal + "</fx:SubTotalBruto>")
                .Append("<fx:SubTotal>" + xSubTotal + "</fx:SubTotal>")
                .Append("<fx:Descuento>" + xTotalImpuestosRetenidos + "</fx:Descuento>")
                .Append("<fx:ResumenDeDescuentosYRecargos>")
                .Append("<fx:TotalDescuentos>" + xTotalImpuestosRetenidos + "</fx:TotalDescuentos>")
                .Append("<fx:TotalRecargos>0</fx:TotalRecargos>")
                .Append("</fx:ResumenDeDescuentosYRecargos>")
                .Append("<fx:ResumenDeImpuestos>")
                .Append("<fx:TotalTrasladosFederales>0</fx:TotalTrasladosFederales>")
                .Append("<fx:TotalIVATrasladado>0</fx:TotalIVATrasladado>")
                .Append("<fx:TotalIEPSTrasladado>0</fx:TotalIEPSTrasladado>")
                .Append("<fx:TotalRetencionesFederales>0</fx:TotalRetencionesFederales>")
                .Append("<fx:TotalISRRetenido>0</fx:TotalISRRetenido>")
                .Append("<fx:TotalIVARetenido>0</fx:TotalIVARetenido>")
                .Append("<fx:TotalTrasladosLocales>0</fx:TotalTrasladosLocales>")
                .Append("<fx:TotalRetencionesLocales>0</fx:TotalRetencionesLocales>")
                .Append("</fx:ResumenDeImpuestos>")
                .Append("<fx:Total>" + xTotal + "</fx:Total>")
                .Append("<fx:TotalEnLetra>" + Utils.convertNumberToLetter(xTotal) + " M.N.</fx:TotalEnLetra>")
                //.Append("<fx:FormaDePago>99</fx:FormaDePago>")
                .Append("</fx:Totales>")

                .Append("<fx:Complementos>")
                //.Append("<fx:Nomina12 Version=\"1.2\" TipoNomina=\"O\" FechaPago=\"" + xFechaPago + "\" FechaInicialPago=\"" + vFechaInical + "\" FechaFinalPago=\"" + vFechaFinal + "\" NumDiasPagados=\"" + vDiasPagados  + "\" TotalPercepciones=\"" + xSubTotal + "\" TotalDeducciones=\"" + xTotalImpuestosRetenidos + "\" TotalOtrosPagos=\"0\">")
                //.Append("<fx:Nomina12 Version=\"1.2\" TipoNomina=\"O\" FechaPago=\"" + vFechaFinal + "\" FechaInicialPago=\"" + vFechaInical + "\" FechaFinalPago=\"" + vFechaFinal + "\" NumDiasPagados=\"" + vDiasPagados + "\" TotalPercepciones=\"" + xSubTotal + "\" TotalDeducciones=\"" + xTotalImpuestosRetenidos + "\" TotalOtrosPagos=\"0\">")
                .Append("<fx:Nomina12 Version=\"1.2\" TipoNomina=\"E\" FechaPago=\"" + vFechaPago + "\" FechaInicialPago=\"" + vFechaInicalPago + "\" FechaFinalPago=\"" + vFechaFinalPago + "\" NumDiasPagados=\"1\" TotalPercepciones=\"" + xSubTotal + "\" TotalDeducciones=\"" + xTotalImpuestosRetenidos + "\" TotalOtrosPagos=\"0\">")
                //.Append("<fx:Receptor Curp=\"" + xCURP + "\" TipoContrato=\"09\" TipoRegimen=\"09\" NumEmpleado=\"" + xNumEmpleado + "\" PeriodicidadPago=\"04\" " + ((xTipoTransferencia == "C") ? "" : (" Banco=\"" + xBanco + "\" CuentaBancaria=\"" + xCuentaBancaria + "\" ")) + " ClaveEntFed=\"" + xClaveEntFed + "\"></fx:Receptor>")
                //.Append("<fx:Receptor Curp=\"" + xCURP + "\" TipoContrato=\"09\" TipoRegimen=\"09\" NumEmpleado=\"" + xNumEmpleado + "\" PeriodicidadPago=\"05\" " + ((xTipoTransferencia == "C") ? "" :  ((xCuentaBancaria.Length > 11) ? (" CuentaBancaria=\"" + xCuentaBancaria + "\" ") : (" Banco=\"" + xBanco + "\" CuentaBancaria=\"" + xCuentaBancaria + "\" "))) + " ClaveEntFed=\"" + xClaveEntFed + "\"></fx:Receptor>")
                .Append("<fx:Receptor Curp=\"" + xCURP + "\" TipoContrato=\"09\" TipoRegimen=\"09\" NumEmpleado=\"" + xNumEmpleado + "\" PeriodicidadPago=\"99\" " + ((xTipoTransferencia == "C") ? "" : ((xCuentaBancaria.Length > 11) ? (" CuentaBancaria=\"" + xCuentaBancaria + "\" ") : (" Banco=\"" + xBanco + "\" CuentaBancaria=\"" + xCuentaBancaria + "\" "))) + " ClaveEntFed=\"" + xClaveEntFed + "\"></fx:Receptor>")

                .Append("<fx:Percepciones TotalSueldos=\"" + xSubTotal + "\" TotalExento=\"0.00\" TotalGravado=\"" + xSubTotal + "\">")
                 //.Append("<fx:Percepcion Clave=\"046\" Concepto=\"Asimilable a sueldos y salarios\" ImporteExento=\"0.00\" ImporteGravado=\"" + xSubTotal + "\" TipoPercepcion=\"046\"/> ")
                 .Append("<fx:Percepcion Clave=\"51100150\" Concepto=\"Honorarios asimilables a sueldos\" ImporteExento=\"0.00\" ImporteGravado=\"" + xSubTotal + "\" TipoPercepcion=\"046\"/> ")

              .Append("</fx:Percepciones>")
                .Append("<fx:Deducciones TotalImpuestosRetenidos=\"" + xTotalImpuestosRetenidos + "\">")
                 //.Append("<fx:Deduccion Clave=\"002\" Concepto=\"ISR retenido asimilados\" Importe=\"" + xTotalImpuestosRetenidos + "\" TipoDeduccion=\"002\"/>")
                 .Append("<fx:Deduccion Clave=\"24200207\" Concepto=\"ISR retenido asimilados\" Importe=\"" + xTotalImpuestosRetenidos + "\" TipoDeduccion=\"002\"/>")

                .Append("</fx:Deducciones>")
                .Append("</fx:Nomina12>")
                .Append("</fx:Complementos>")

                .Append("<fx:ComprobanteEx>")
                .Append("<fx:DatosDeNegocio>")
                .Append("<fx:Sucursal>ANAHUAC</fx:Sucursal>")
                .Append("</fx:DatosDeNegocio>")
                .Append("<fx:TerminosDePago>")
                .Append("<fx:MetodoDePago>PUE</fx:MetodoDePago>")
                .Append("</fx:TerminosDePago>")
                .Append("</fx:ComprobanteEx>")
                .Append("</fx:FactDocMX>");

            //facturación version 3.2

            /*.Append("<fx:FactDocMX xmlns:fx=\"http://www.fact.com.mx/schema/fx\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.fact.com.mx/schema/fx http://www.mysuitemex.com/fact/schema/fx_2010_d.xsd\">")
            .Append("<fx:Version>5</fx:Version>")
            .Append("<fx:Identificacion><fx:CdgPaisEmisor>MX</fx:CdgPaisEmisor><fx:TipoDeComprobante>RECIBO_DE_NOMINA</fx:TipoDeComprobante>")
            .Append("<fx:RFCEmisor>" + xRFCEmisor + "</fx:RFCEmisor>") //AAA010101AAA
            .Append("<fx:RazonSocialEmisor>" + xRazonSocialEmisor + "</fx:RazonSocialEmisor>")
            .Append("<fx:Usuario>" + xUsuario + "</fx:Usuario>")
            .Append("<fx:NumeroInterno>" + xNumeroInterno + "</fx:NumeroInterno>")
            .Append("<fx:LugarExpedicion>" + xLugarExpedicion + "</fx:LugarExpedicion>")
            .Append("</fx:Identificacion>")
            .Append("<fx:Procesamiento>")
            .Append("<fx:Dictionary name=\"email\">")
            .Append("<fx:Entry k=\"from\" v=\"ACCOUNT_OWNER\"/>")
            .Append("<fx:Entry k=\"to\" v=\"" + xEmailReceptor + "\"/>")
            .Append("</fx:Dictionary>")
            .Append("</fx:Procesamiento>")
            .Append("<fx:Emisor>")
            .Append("<fx:RegimenFiscal>")
            .Append("<fx:Regimen>603</fx:Regimen>")
            .Append("</fx:RegimenFiscal>")
            .Append("</fx:Emisor>")
            .Append("<fx:Receptor>")
            .Append("<fx:CdgPaisReceptor>MX</fx:CdgPaisReceptor>")
            .Append("<fx:RFCReceptor>" + xRFCReceptor + "</fx:RFCReceptor>") // xRFCReceptor CAMBIAR PARA PRODUCCIÓN -- AAAA010101AAA para test
            .Append("<fx:NombreReceptor>" + xNombreReceptor + "</fx:NombreReceptor>")
            .Append("</fx:Receptor>")
            .Append("<fx:Conceptos>")
            .Append("<fx:Concepto>")
            .Append("<fx:Cantidad>1</fx:Cantidad>")
            .Append("<fx:UnidadDeMedida>ACT</fx:UnidadDeMedida>")
            .Append("<fx:Descripcion>Pago de nómina</fx:Descripcion>")
            .Append("<fx:ValorUnitario>" + xSubTotal + "</fx:ValorUnitario>")
            .Append("<fx:Importe>" + xSubTotal + "</fx:Importe>")
            .Append("</fx:Concepto>")
            .Append("</fx:Conceptos>")
            .Append("<fx:Totales>")
            .Append("<fx:Moneda>MXN</fx:Moneda>")
            .Append("<fx:TipoDeCambioVenta>1</fx:TipoDeCambioVenta>")
            .Append("<fx:SubTotalBruto>" + xSubTotal + "</fx:SubTotalBruto>")
            .Append("<fx:SubTotal>" + xSubTotal + "</fx:SubTotal>")
            .Append("<fx:ResumenDeDescuentosYRecargos>")
            .Append("<fx:TotalDescuentos>" + xTotalImpuestosRetenidos + "</fx:TotalDescuentos>")
            .Append("<fx:TotalRecargos>0</fx:TotalRecargos>")
            .Append("</fx:ResumenDeDescuentosYRecargos>")
            .Append("<fx:ResumenDeImpuestos>")
            .Append("<fx:TotalTrasladosFederales>0</fx:TotalTrasladosFederales>")
            .Append("<fx:TotalIVATrasladado>0</fx:TotalIVATrasladado>")
            .Append("<fx:TotalIEPSTrasladado>0</fx:TotalIEPSTrasladado>")
            .Append("<fx:TotalRetencionesFederales>0</fx:TotalRetencionesFederales>")
            .Append("<fx:TotalISRRetenido>0</fx:TotalISRRetenido>")
            .Append("<fx:TotalIVARetenido>0</fx:TotalIVARetenido>")
            .Append("<fx:TotalTrasladosLocales>0</fx:TotalTrasladosLocales>")
            .Append("<fx:TotalRetencionesLocales>0</fx:TotalRetencionesLocales>")
            .Append("</fx:ResumenDeImpuestos>")
            .Append("<fx:Total>" + xTotal + "</fx:Total>")
            .Append("<fx:TotalEnLetra>" + Utils.convertNumberToLetter(xTotal) + " M.N.</fx:TotalEnLetra>")
            .Append("<fx:FormaDePago>PAGO EN UNA SOLA EXHIBICION</fx:FormaDePago>")
            .Append("</fx:Totales>")
            .Append("<fx:Complementos>")
            .Append("<fx:Nomina12 Version=\"1.2\" TipoNomina=\"O\" FechaPago=\"" + xFechaPago + "\" FechaInicialPago=\"" + xFechaPago + "\" FechaFinalPago=\"" + xFechaPago + "\" NumDiasPagados=\"1\" TotalPercepciones=\"" + xSubTotal + "\" TotalDeducciones=\"" + xTotalImpuestosRetenidos + "\">")
            .Append("<fx:Receptor Curp=\"" + xCURP + "\" TipoContrato=\"09\" TipoRegimen=\"09\" NumEmpleado=\"" + xNumEmpleado + "\" PeriodicidadPago=\"04\" " + ((xTipoTransferencia == "C") ? "" : (" Banco=\"" + xBanco + "\" CuentaBancaria=\"" + xCuentaBancaria + "\" ")) + " ClaveEntFed=\"" + xClaveEntFed + "\">")
            .Append("</fx:Receptor>")
            .Append("<fx:Percepciones TotalSueldos=\"" + xSubTotal + "\" TotalExento=\"0.00\" TotalGravado=\"" + xSubTotal + "\">")
            .Append("<fx:Percepcion Clave=\"046\" Concepto=\"Asimilable a sueldos y salarios\" ImporteExento=\"0.00\" ImporteGravado=\"" + xSubTotal + "\" TipoPercepcion=\"046\"/>")
            .Append("</fx:Percepciones>")
            .Append("<fx:Deducciones TotalImpuestosRetenidos=\"" + xTotalImpuestosRetenidos + "\">")
            .Append("<fx:Deduccion Clave=\"002\" Concepto=\"ISR retenido asimilados\" Importe=\"" + xTotalImpuestosRetenidos + "\" TipoDeduccion=\"002\"/>")
            .Append("</fx:Deducciones>")
            .Append("</fx:Nomina12>")
            .Append("</fx:Complementos>")
            .Append("<fx:ComprobanteEx>")
            .Append("<fx:DatosDeNegocio>")
            .Append("<fx:Sucursal>ANAHUAC</fx:Sucursal>")
            .Append("</fx:DatosDeNegocio>")
            .Append("<fx:TerminosDePago>")
            .Append("<fx:MetodoDePago>NA</fx:MetodoDePago>")
            .Append("</fx:TerminosDePago>")
            .Append("</fx:ComprobanteEx>")
            .Append("</fx:FactDocMX>");*/

            return stringXML.ToString();
        }
    }
}
