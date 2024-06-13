using ConnectDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{
    public class EstadodeCuentaModel : SuperModel
    {
        public string IdPersona { get; set; }
        public string IdSIU { get; set; }
        public string IdEdoCta { get; set; } // =)
        public string IdEdoCtaD { get; set; }
        public string IdCtaPensionado { get; set; }
        public string Profesor { get; set; }
        public string Noi { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string CveSede { get; set; }
        public string Periodo { get; set; }
        public string PartePeriodo { get; set; }
        public string Concepto { get; set; }
        public string NumPago { get; set; }
        public string Monto { get; set; }
        public string TotalMonto { get; set; }
        public string TotalIva { get; set; }
        public string TotalIvaRet { get; set; }
        public string TotalIsrRet { get; set; }
        public string TotalBancos { get; set; }
        public string FechaPago { get; set; }
        public string NR { get; set; }
        public string IdEsquema { get; set; }
        public string Esquema { get; set; }
        public string PagoA { get; set; }
        public string PagoM { get; set; }
        public string ciclo { get; set; }
        public string cveFactura { get; set; }
        public string factura { get; set; }
        public string cveOrigen { get; set; }
        public string origen { get; set; }
        public string FechaRecibo { get; set; }
        public string FechaDispersion { get; set; }
        public string FechaDeposito { get; set; }
        public string FechaSolicitado { get; set; }
        public string Folio { get; set; }
        public string Beneficiario { get; set; }
        public string IdBeneficiario { get; set; }
        public string Solicitado { get; set; }
        public string Uuid { get; set; }
        public string Xml { get; set; }
        public string ConceptoM { get; set; }
        public string Bloqueado { get; set; }
        public string idCentroCosto { get; set; }
        public string tipoPago { get; set; }
        public string idTransferencia { get; set; }
        public string folioCheque { get; set; }
        public string idBloqueo { get; set; }
        public string ConceptoPagoID { get; set; }
        public string IDs { get; set; }
        public string _msg { get; set; }
        public string sql { get; set; } //update
        public string file_xml_name { get; set; }
        public string file_pdf_name { get; set; }
        public string estado { get; set; }
        public string Motivos { get; set; }
        public string FPago { get; set; }
        public string CampusINB { get; set; }
        public string Escuela { get; set; }
        public string CentroCostos { get; set; }
        //Impuestos
        public string iva { get; set; }
        public string ivaRet { get; set; }
        public string isrRet { get; set; }
        public string limInferior { get; set; }
        public string cuotaFija { get; set; }
        public string porcExcedente { get; set; }
        public Boolean Existe { get; set; }
        public string bloqueos { get; set; }
        public string[] arrBloqueos { get; set; }
        //Máximos de Periodo y Ciclo para la búsqueda de una persona
        public string maxperiodo { get; set; }
        public string maxciclo { get; set; }
        public bool seleccionar { get; set; }
        public string importeMarginal;

        public string mensaje_res;

        public bool BuscaPersona()
        {
            try
            {
                sql = "SELECT distinct * FROM QPersonas WHERE ";
                sql += "CVE_SEDE = '" + CveSede + "'";
                if (IdSIU != "" && IdSIU != null)
                    sql += " and IDSIU = '" + IdSIU + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    IdPersona = res.Get("ID_PERSONA");
                    Profesor = res.Get("PROFESOR");
                    Noi = res.Get("NOI");
                    Apellidos = res.Get("APELLIDOS");
                    Nombres = res.Get("NOMBRES");
                    cveFactura = res.Get("CVE_TIPOFACTURA");
                    factura = res.Get("TIPOFACTURA");
                    cveOrigen = res.Get("CVE_ORIGEN");
                    origen = res.Get("ORIGEN");
                    IdSIU = res.Get("IDSIU");
                    maxperiodo = res.Get("MAXPERIODO");
                    maxciclo = res.Get("MAXCICLO");
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public bool Edit()
        {
            try
            {
                sql = "SELECT * FROM VESTADO_CUENTA_DETALLE WHERE ID_EDOCTADETALLE = " + IdEdoCtaD; //update quitar string
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    IdSIU = res.Get("IDSIU");
                    Nombres = res.Get("NOMBRES");
                    Apellidos = res.Get("APELLIDOS");
                    CveSede = res.Get("CVE_SEDE");
                    Periodo = res.Get("PERIODO");
                    PartePeriodo = res.Get("PARTEDELPERIODO");
                    Concepto = res.Get("CONCEPTO");
                    NumPago = res.Get("PKCONCEPTOPAGO");
                    Monto = res.Get("MONTO");
                    TotalIva = res.Get("MONTO_IVA");
                    TotalIvaRet = res.Get("MONTO_IVARET");
                    TotalIsrRet = res.Get("MONTO_ISRRET");
                    TotalBancos = res.Get("BANCOS");
                    FechaPago = res.Get("FECHAPAGO");
                    FechaRecibo = res.Get("FECHARECIBO");
                    FechaDispersion = res.Get("FECHADISPERSION");
                    FechaDeposito = res.Get("FECHADEPOSITO");
                    IdEsquema = res.Get("ID_ESQUEMA");
                    IdPersona = res.Get("ID_PERSONA");
                    ciclo = res.Get("CVE_CICLO");
                    IdEdoCta = res.Get("ID_ESTADODECUENTA");
                    IdEdoCtaD = res.Get("ID_EDOCTADETALLE");
                    idCentroCosto = res.Get("ID_CENTRODECOSTOS");
                    tipoPago = res.Get("CVE_TIPODEPAGO");
                    idTransferencia = res.Get("CVE_TIPOTRANSFERENCIA");
                    idBloqueo = res.Get("CVE_BLOQUEO");
                    Bloqueado = res.Get("BLOQUEADO");
                    return true;
                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }

        public bool EditBloqueos()
        {
            try
            {
                sql = "select * " +
                      "  from ESTADODECUENTA_DETALLE_BLOQUEOS " +
                      " where ID_EDOCTADETALLE = " + IdEdoCtaD;

                ResultSet res = db.getTable(sql);
                List<string> listBloqueos = new List<string>();
                while (res.Next())
                    listBloqueos.Add(res.Get("CVE_BLOQUEO"));

                arrBloqueos = listBloqueos.ToArray<string>();

                return true;
            }
            catch {; }
            return false;
        }

        public bool Grabar()
        {
            try
            {
                sql = "UPDATE ESTADODECUENTA_DETALLE";
                sql += " set     PKCONCEPTOPAGO        = " + NumPago + ",";
                sql += "      MOTIVO       = '" + Motivos + "',";
                sql += "      USUARIO       = '" + sesion.nickName + "',";
                if (FechaPago != null && FechaPago != "")
                    sql += "  FECHADEPAGO           = '" + FechaPago + "',";
                if (FechaRecibo != null && FechaRecibo != "")
                    sql += "  FECHARECIBO           = '" + FechaRecibo + "',";
                if (FechaDispersion != null && FechaDispersion != "")
                    sql += "  FECHADISPERSION       = '" + FechaDispersion + "',";
                if (FechaDeposito != null && FechaDeposito != "")
                    sql += "  FECHADEPOSITO         = '" + FechaDeposito + "',";
                sql += "      BANCOS                = " + TotalBancos + ",";
                sql += "      MONTO_IVA             = " + TotalIva + ",";
                sql += "      MONTO_IVARET          = " + TotalIvaRet + ",";
                sql += "      MONTO_ISRRET          = " + TotalIsrRet + ",";
                sql += "      MONTO                 = " + Monto + ",";
                if (idCentroCosto != null && idCentroCosto != "")
                    sql += "      ID_CENTRODECOSTOS     = " + idCentroCosto + ",";
                sql += "      CVE_TIPOTRANSFERENCIA = '" + idTransferencia + "',";
                sql += "      BLOQUEADO             = " + (Bloqueado == "on" ? "1" : "0");
                sql += " WHERE ID_EDOCTADETALLE = " + IdEdoCtaD;

                db.execute(sql);
                return SaveBloqueos();
            }
            catch
            {
                return false;
            }
        }

        public bool SaveBloqueos()
        {
            string sql = "DELETE FROM ESTADODECUENTA_DETALLE_BLOQUEOS WHERE ID_EDOCTADETALLE = " + IdEdoCtaD;
            bool ok = true;
            if (db.execute(sql) && bloqueos != null)
            {
                string[] array = bloqueos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in array)
                {
                    sql =
                        "INSERT INTO ESTADODECUENTA_DETALLE_BLOQUEOS (ID_EDOCTADETALLE, ID_ESQUEMA, PKCONCEPTOPAGO, CVE_BLOQUEO)" +
                        "                                      VALUES(" + IdEdoCtaD + "," + IdEsquema + "," + NumPago + ", '" + item + "')";
                    ok = ok && db.execute(sql);
                }
            }
            return ok;
        }

        public bool AgregarEdoCta()
        {
            try
            {
                // En un archivo más actual todo el bloque siguiente de código se comentó.
                bool exito = false;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramEdoCtaID = new Parametros();
                Parametros paramPkpago = new Parametros();
                Parametros paramConcepto = new Parametros();
                Parametros paramTipoTransf = new Parametros();
                Parametros paramFolioCheque = new Parametros();
                Parametros paramCentroCosto = new Parametros();
                Parametros paramFechaPa = new Parametros();
                Parametros paramFechaRe = new Parametros();
                Parametros paramFechaDi = new Parametros();
                Parametros paramFechaDe = new Parametros();
                Parametros paramMontoTotal = new Parametros();
                Parametros paramIva = new Parametros();
                Parametros paramIvaRet = new Parametros();
                Parametros paramIsrRet = new Parametros();
                Parametros paramMonto = new Parametros();
                Parametros paramFolio = new Parametros();
                Parametros paramBeneficiado = new Parametros();
                Parametros paramUuid = new Parametros();
                Parametros paramXml = new Parametros();
                Parametros paramBloqueado = new Parametros();
                Parametros paramBloqueos = new Parametros();

                paramEdoCtaID.nombreParametro = "@estadoCuentaId";
                paramEdoCtaID.tipoParametro = SqlDbType.BigInt;
                paramEdoCtaID.direccion = ParameterDirection.Input;
                paramEdoCtaID.value = IdEdoCtaD;
                lParamS.Add(paramEdoCtaID);

                paramPkpago.nombreParametro = "@pkpago";
                paramPkpago.tipoParametro = SqlDbType.Int;
                paramPkpago.direccion = ParameterDirection.Input;
                paramPkpago.value = NumPago;
                lParamS.Add(paramPkpago);

                paramTipoTransf.nombreParametro = "@tipoTransf";
                paramTipoTransf.tipoParametro = SqlDbType.NVarChar;
                paramTipoTransf.longitudParametro = 1;
                paramTipoTransf.direccion = ParameterDirection.Input;
                paramTipoTransf.value = idTransferencia;
                lParamS.Add(paramTipoTransf);

                paramCentroCosto.nombreParametro = "@centroCosto";
                paramCentroCosto.tipoParametro = SqlDbType.BigInt;
                paramCentroCosto.direccion = ParameterDirection.Input;
                paramCentroCosto.value = idCentroCosto;
                lParamS.Add(paramCentroCosto);

                paramFechaPa.nombreParametro = "@fechaPa";
                paramFechaPa.tipoParametro = SqlDbType.DateTime;
                paramFechaPa.direccion = ParameterDirection.Input;
                paramFechaPa.value = FechaPago;
                lParamS.Add(paramFechaPa);

                paramFechaRe.nombreParametro = "@fechaRe";
                paramFechaRe.tipoParametro = SqlDbType.DateTime;
                paramFechaRe.direccion = ParameterDirection.Input;
                paramFechaRe.value = FechaRecibo;
                lParamS.Add(paramFechaRe);

                paramFechaDi.nombreParametro = "@fechaDi";
                paramFechaDi.tipoParametro = SqlDbType.DateTime;
                paramFechaDi.direccion = ParameterDirection.Input;
                paramFechaDi.value = FechaDispersion;
                lParamS.Add(paramFechaDi);

                paramFechaDe.nombreParametro = "@fechaDe";
                paramFechaDe.tipoParametro = SqlDbType.DateTime;
                paramFechaDe.direccion = ParameterDirection.Input;
                paramFechaDe.value = FechaDeposito;
                lParamS.Add(paramFechaDe);

                paramMontoTotal.nombreParametro = "@bancos";
                paramMontoTotal.tipoParametro = SqlDbType.Real;
                paramMontoTotal.direccion = ParameterDirection.Input;
                paramMontoTotal.value = TotalBancos;
                lParamS.Add(paramMontoTotal);

                paramIva.nombreParametro = "@iva";
                paramIva.tipoParametro = SqlDbType.Real;
                paramIva.direccion = ParameterDirection.Input;
                paramIva.value = TotalIva;
                lParamS.Add(paramIva);

                paramIvaRet.nombreParametro = "@ivaRet";
                paramIvaRet.tipoParametro = SqlDbType.Real;
                paramIvaRet.direccion = ParameterDirection.Input;
                paramIvaRet.value = TotalIvaRet;
                lParamS.Add(paramIvaRet);

                paramIsrRet.nombreParametro = "@isrRet";
                paramIsrRet.tipoParametro = SqlDbType.Real;
                paramIsrRet.direccion = ParameterDirection.Input;
                paramIsrRet.value = TotalIsrRet;
                lParamS.Add(paramIsrRet);

                paramMonto.nombreParametro = "@monto";
                paramMonto.tipoParametro = SqlDbType.Real;
                paramMonto.direccion = ParameterDirection.Input;
                paramMonto.value = Monto;
                lParamS.Add(paramMonto);

                paramBloqueado.nombreParametro = "@bloqueado";
                paramBloqueado.tipoParametro = SqlDbType.Bit;
                paramBloqueado.direccion = ParameterDirection.Input;
                paramBloqueado.value = (Bloqueado == "on" ? "true" : "false");
                lParamS.Add(paramBloqueado);

                paramBloqueos.nombreParametro = "@bloqueos";
                paramBloqueos.tipoParametro = SqlDbType.NVarChar;
                paramBloqueos.direccion = ParameterDirection.Input;
                paramBloqueos.value = bloqueos;
                lParamS.Add(paramBloqueos);

                exito = db.ExecuteStoreProcedure("sp_inserta_estadodecuentaXEdoCtaId", lParamS);

                return exito;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool AgregarEdoCtaModalExterno_()
        {
            mensaje_res = "";
            bool exito = false;
            ResultSet rs = null;

            sql = "SELECT top 1 * FROM ESTADODECUENTA";
            sql += " WHERE CVE_SEDE = '" + CveSede + "'";
            sql += " AND ID_PERSONA = '" + IdPersona + "'";
            sql += " AND ID_ESQUEMA = '" + IdEsquema + "'";
            sql += " AND PERIODO = '" + Periodo + "'";

            rs = db.getTable(sql);

            try
            {
                if (rs != null)
                {
                    if (rs.Count == 0)
                    {
                        exito = AgregarEdoCtaModalExterno();
                        return exito;
                    }
                    else { mensaje_res = "EXISTE"; return false; }   //poner mensaje 

                }

                else  return false;
            }
            catch (Exception er)
            {
                return false;
            }

        }

        public bool AgregarEdoCtaModalExterno()
        {
            try
            {
                string sql2 = "SELECT ps.CVE_SEDE,ps.ID_PERSONA FROM PERSONAS_SEDES ps INNER JOIN PERSONAS p ON ps.ID_PERSONA=p.ID_PERSONA WHERE P.IDSIU='" + IdSIU + "' AND  ps.CVE_SEDE='" + sesion.vdata["Sede"] + "'";
                ResultSet res2 = db.getTable(sql2);
                 if (res2.Next())
                {
                    IdPersona = res2.Get("ID_PERSONA");
                    CveSede = res2.Get("CVE_SEDE");
                }
            }
            catch (Exception ex)
            {

            }
           
            try
            {
                bool exito = false;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramSede = new Parametros();
                Parametros paramPersonaId = new Parametros();
                Parametros paramPeriodo = new Parametros();
                Parametros paramEsquemaId = new Parametros();
                Parametros paramPartePeriodo = new Parametros();
                Parametros paramUsuario = new Parametros();
                Parametros paramCampus = new Parametros();
                Parametros paramEscuela = new Parametros();

                paramSede.nombreParametro = "@sede";
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.longitudParametro = 5;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = CveSede;
                lParamS.Add(paramSede);

                paramPersonaId.nombreParametro = "@personaId";
                paramPersonaId.tipoParametro = SqlDbType.Int;
                paramPersonaId.direccion = ParameterDirection.Input;
                paramPersonaId.value = IdPersona;
                lParamS.Add(paramPersonaId);

                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramEsquemaId.nombreParametro = "@esquemaId";
                paramEsquemaId.tipoParametro = SqlDbType.BigInt;
                paramEsquemaId.direccion = ParameterDirection.Input;
                paramEsquemaId.value = IdEsquema;
                lParamS.Add(paramEsquemaId);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.longitudParametro = 3;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = PartePeriodo;
                lParamS.Add(paramPartePeriodo);

                paramUsuario.nombreParametro = "@usuario";
                paramUsuario.tipoParametro = SqlDbType.NVarChar;
                paramUsuario.longitudParametro = 180;
                paramUsuario.direccion = ParameterDirection.Input;
                paramUsuario.value = sesion.nickName;
                lParamS.Add(paramUsuario);

                paramCampus.nombreParametro = "@campus";
                paramCampus.tipoParametro = SqlDbType.NVarChar;
                paramCampus.longitudParametro = 5;
                paramCampus.direccion = ParameterDirection.Input;
                paramCampus.value = CampusINB;
                lParamS.Add(paramCampus);

                paramEscuela.nombreParametro = "@escuela";
                paramEscuela.tipoParametro = SqlDbType.NVarChar;
                paramEscuela.longitudParametro = 5;
                paramEscuela.direccion = ParameterDirection.Input;
                paramEscuela.value = Escuela;
                lParamS.Add(paramEscuela);

                exito = db.ExecuteStoreProcedure("sp_inserta_estadodecuentaXIdExterno", lParamS);

                return exito;
            }
            catch (Exception er)
            {
                return false;
            }
        }
        public bool quitarFechaContrato()
        {

            try
            {
                string sql2 = "SELECT ps.CVE_SEDE,ps.ID_PERSONA FROM PERSONAS_SEDES ps INNER JOIN PERSONAS p ON ps.ID_PERSONA=p.ID_PERSONA WHERE P.IDSIU='" + IdSIU + "' AND  ps.CVE_SEDE='" + sesion.vdata["Sede"] + "'";
                ResultSet res2 = db.getTable(sql2);
                if (res2.Next())
                {
                    IdPersona = res2.Get("ID_PERSONA");
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                bool exito = false;
                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramPersonaID = new Parametros();
                Parametros paramEsquemaID = new Parametros();

                paramPersonaID.nombreParametro = "@personaID";
                paramPersonaID.tipoParametro = SqlDbType.BigInt;
                paramPersonaID.direccion = ParameterDirection.Input;
                paramPersonaID.value = IdPersona;
                lParamS.Add(paramPersonaID);

                paramEsquemaID.nombreParametro = "@esquemaID";
                paramEsquemaID.tipoParametro = SqlDbType.BigInt;
                paramEsquemaID.direccion = ParameterDirection.Input;
                paramEsquemaID.value = IdEsquema;
                lParamS.Add(paramEsquemaID);

                exito = db.ExecuteStoreProcedure("sp_actualiza_fechaEntregaContrato", lParamS);

                return exito;
            }
            catch (Exception ef)
            {
                return false;
            }
        }

        public bool recalcular()
        {
            try
            {
                if (tipoPago == "HDI" || tipoPago == "HDIC" || tipoPago == "HIN")
                {
                    sql = "select TIVA.IVA, TIVARet.IVARET, TISRRET.ISRRET" +
                          "  from (select VALOR as IVA " +
                          "          from VARIABLES " +
                          "         where VARIABLE = 'IVA') as TIVA, " +
                          "       (select VALOR as IVARET " +
                          "          from VARIABLES " +
                          "         where VARIABLE = 'IVA Ret') as TIVARet, " +
                          "       (select VALOR as ISRRET " +
                          "          from VARIABLES " +
                          "         where VARIABLE = 'ISR Ret') as TISRRet";

                    ResultSet res = db.getTable(sql);

                    if (res.Next())
                    {
                        iva = res.Get("IVA");
                        ivaRet = res.Get("IVARET");
                        isrRet = res.Get("ISRRET");

                        TotalMonto = Monto;
                        return true;
                    }
                    else { return false; }

                }

                if (tipoPago == "FDI" || tipoPago == "FDIC" || tipoPago == "FIN")
                {
                    sql = "select TIVA.IVA, TIVARet.IVARET, TISRRET.ISRRET" +
                          "  from (select VALOR as IVA " +
                          "          from VARIABLES " +
                          "         where VARIABLE = 'IVA') as TIVA, " +
                          "       (select VALOR as IVARET " +
                          "          from VARIABLES " +
                          "         where VARIABLE = 'IVA Ret') as TIVARet, " +
                          "       (select VALOR as ISRRET " +
                          "          from VARIABLES " +
                          "         where VARIABLE = 'ISR Ret') as TISRRet";

                    ResultSet res = db.getTable(sql);

                    if (res.Next())
                    {
                        iva = res.Get("IVA");
                        ivaRet = res.Get("IVARET");
                        isrRet = res.Get("ISRRET");

                        TotalMonto = Monto;
                        return true;
                    }
                    else { return false; }
                }

                if (tipoPago == "RDI" || tipoPago == "RIN" )
                {
                    sql = "select TIVA.IVA, TIVARet.IVARET, TISRRET.ISRRET" +
                          "  from (select VALOR as IVA " +
                          "          from VARIABLES " +
                          "         where VARIABLE = 'IVA') as TIVA, " +
                          "       (select VALOR as IVARET " +
                          "          from VARIABLES " +
                          "         where VARIABLE = 'IVA Ret') as TIVARet, " +
                          "       (select VALOR as ISRRET " +
                          "          from VARIABLES " +
                          "         where VARIABLE = 'ISR Ret RESICO') as TISRRet";

                    ResultSet res = db.getTable(sql);

                    if (res.Next())
                    {
                        iva = res.Get("IVA");
                        ivaRet = res.Get("IVARET");
                        isrRet = res.Get("ISRRET");

                        TotalMonto = Monto;
                        return true;
                    }
                    else { return false; }
                }

                if (tipoPago == "ADI" || tipoPago == "ADIC" || tipoPago == "AIN")
                {
                    sql = "select LIMITEINFERIOR, CUOTAFIJA, (PORCENTAJEEXCEDENTE / 100) as PORCENTAJEEXCEDENTE " +
                          "  from IMPUESTOSASIMILADOS " +
                          " where (" + Monto + ") >= LIMITEINFERIOR " +
                          "   and (" + Monto + ") <= LIMITESUPERIOR";

                    ResultSet res = db.getTable(sql);

                    if (res.Next())
                    {
                        limInferior = res.Get("LIMITEINFERIOR");
                        cuotaFija = res.Get("CUOTAFIJA");
                        porcExcedente = res.Get("PORCENTAJEEXCEDENTE");

                        TotalMonto = Monto;

                        return true;
                    }
                    else { return false; }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool mover()
        {
            try
            {
                // En un archivo más actual todo el bloque siguiente de código se comentó.
                bool exito = false;
                string xId;
                string tipoRegEC;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramCuentaDetID;
                Parametros paramConceptoPagoID;
                Parametros paramTipoRegistroEC;

                string[] _IDs = IDs.Split(',');

                foreach (var id in _IDs)
                {
                    paramCuentaDetID = new Parametros();
                    paramConceptoPagoID = new Parametros();
                    paramTipoRegistroEC = new Parametros();
                    lParamS = new List<Parametros>();

                    xId = id.Substring(1);
                    tipoRegEC = id.Substring(0, 1);

                    paramTipoRegistroEC.nombreParametro = "@tipoRecEC";
                    paramTipoRegistroEC.tipoParametro = SqlDbType.NVarChar;
                    paramTipoRegistroEC.direccion = ParameterDirection.Input;
                    paramTipoRegistroEC.value = tipoRegEC;
                    lParamS.Add(paramTipoRegistroEC);

                    paramCuentaDetID.nombreParametro = "@cuentaDetID";
                    paramCuentaDetID.tipoParametro = SqlDbType.BigInt;
                    paramCuentaDetID.direccion = ParameterDirection.Input;
                    paramCuentaDetID.value = xId;
                    lParamS.Add(paramCuentaDetID);

                    paramConceptoPagoID.nombreParametro = "@conceptoPagoID";
                    paramConceptoPagoID.tipoParametro = SqlDbType.BigInt;
                    paramConceptoPagoID.direccion = ParameterDirection.Input;
                    paramConceptoPagoID.value = ConceptoPagoID;
                    lParamS.Add(paramConceptoPagoID);

                    exito = db.ExecuteStoreProcedure("sp_estadodecuentaDetalle_moverConcepto", lParamS);
                }

                return exito;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool eliminar()
        {
            try
            {
                // En un archivo más actual todo el bloque siguiente de código se comentó.
                bool exito = false;
                string xId;
                string tipoRegEC;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramCuentaDetID;
                Parametros paramTipoRegistroEC;
                Parametros paramMotivo;
                Parametros paramUsuario;

                string[] _IDs = IDs.Split(',');

                foreach (var id in _IDs)
                {
                    paramCuentaDetID = new Parametros();
                    paramTipoRegistroEC = new Parametros();
                    lParamS = new List<Parametros>();
                    paramMotivo = new Parametros();
                    paramUsuario = new Parametros();

                    xId = id.Substring(1);
                    tipoRegEC = id.Substring(0, 1);

                    paramTipoRegistroEC.nombreParametro = "@tipoRecEC";
                    paramTipoRegistroEC.tipoParametro = SqlDbType.NVarChar;
                    paramTipoRegistroEC.direccion = ParameterDirection.Input;
                    paramTipoRegistroEC.value = tipoRegEC;
                    lParamS.Add(paramTipoRegistroEC);

                    paramCuentaDetID.nombreParametro = "@cuentaDetID";
                    paramCuentaDetID.tipoParametro = SqlDbType.BigInt;
                    paramCuentaDetID.direccion = ParameterDirection.Input;
                    paramCuentaDetID.value = xId;
                    lParamS.Add(paramCuentaDetID);
                    
                    paramMotivo.nombreParametro = "@motivo";
                    paramMotivo.tipoParametro = SqlDbType.NVarChar;
                    paramMotivo.direccion = ParameterDirection.Input;
                    paramMotivo.value = Motivos;//
                    lParamS.Add(paramMotivo);
                    
                    paramUsuario.nombreParametro = "@usuario";
                    paramUsuario.tipoParametro = SqlDbType.NVarChar;
                    paramUsuario.direccion = ParameterDirection.Input;
                    paramUsuario.value = sesion.nickName;//sesion.pkUser.ToString()
                    lParamS.Add(paramUsuario);
                    
                    exito = db.ExecuteStoreProcedure("sp_estadodecuenta_eliminar", lParamS);
                    sql = "Stored: sp_estadodecuenta_eliminarl, Parametros: " + lParamS;
                }

                return exito;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool eliminarXMLoPDF(string file = "")
        {
            try
            {
                // En un archivo más actual todo el bloque siguiente de código se comentó.
                bool exito = false;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramCuentaDetID = new Parametros();
                Parametros paramFileType = new Parametros();
                Parametros paramUsuario = new Parametros();

                paramCuentaDetID.nombreParametro = "@pEdoCtaID";
                paramCuentaDetID.tipoParametro = SqlDbType.BigInt;
                paramCuentaDetID.direccion = ParameterDirection.Input;
                paramCuentaDetID.value = IdEdoCta;
                lParamS.Add(paramCuentaDetID);

                paramFileType.nombreParametro = "@pFile";
                paramFileType.tipoParametro = SqlDbType.Char;
                paramFileType.direccion = ParameterDirection.Input;
                paramFileType.value = file;
                lParamS.Add(paramFileType);

                paramUsuario.nombreParametro = "@pUsr";
                paramUsuario.tipoParametro = SqlDbType.NVarChar;
                paramUsuario.direccion = ParameterDirection.Input;
                paramUsuario.value = sesion.nickName;//sesion.pkUser.ToString()
                lParamS.Add(paramUsuario);

                exito = db.ExecuteStoreProcedure("sp_estadodecuenta_eliminarPDFoXML", lParamS);

                return exito;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool getFechaConceptodePago()
        {
            try
            {
                bool exito = false;

                sql = " select ISNULL(CAST(FORMAT(FECHADEPAGO, 'yyyy-MM-dd', 'en-US') AS NVARCHAR), N'') AS FECHADEPAGO " +
                      "   from ESQUEMASDEPAGOFECHAS " +
                      "  where PK1 = " + ConceptoPagoID;
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    FechaPago = res.Get("FECHADEPAGO");
                    exito = true;
                }

                return exito;
            }
            catch
            {
                return false;
            }
        }

        public string formatFechaWeb(string str)
        {
            if (str != null)
            {
                DateTime dt;
                if (DateTime.TryParse(str, out dt))
                    return dt.ToString("yyyy-MM-dd");
            }
            return "";

        }

        public bool EditPagoEstadoCuenta()
        {
            try
            {
                sql = "SELECT * FROM VESTADO_CUENTA WHERE ID_ESTADODECUENTA ='" + IdEdoCtaD + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    IdEdoCtaD = res.Get("ID_ESTADODECUENTA");
                    CveSede = res.Get("CVE_SEDE");
                    Periodo = res.Get("PERIODO");
                    PartePeriodo = res.Get("PARTEDELPERIODODESC");
                    IdEsquema = res.Get("ID_ESQUEMA");
                    Esquema = res.Get("ESQUEMA");
                    Concepto = res.Get("CONCEPTO");
                    FechaPago = res.Get("FECHAPAGO");
                    TotalMonto = res.Get("MONTO");
                    TotalIva = res.Get("MONTO_IVA");
                    TotalIvaRet = res.Get("MONTO_IVARET");
                    TotalIsrRet = res.Get("MONTO_ISRRET");
                    TotalBancos = res.Get("BANCOS");
                    FechaRecibo = formatFechaWeb(res.Get("FECHARECIBO"));
                    FechaDispersion = formatFechaWeb(res.Get("FECHADISPERSION"));
                    FechaDeposito = formatFechaWeb(res.Get("FECHADEPOSITO"));
                    FechaSolicitado = formatFechaWeb(res.Get("FECHA_SOLICITADO"));
                    tipoPago = res.Get("CVE_TIPODEPAGO");
                    idTransferencia = res.Get("CVE_TIPOTRANSFERENCIA");
                    IdSIU = res.Get("IDSIU");

                    if (res.GetInt("BLOQUEOS") > 0)
                        estado = "Bloqueado";
                    else
                    {
                        if (string.IsNullOrWhiteSpace(res.Get("FECHARECIBO")) ||
                            string.IsNullOrWhiteSpace(res.Get("FECHA_SOLICITADO")))
                            estado = "Pendiente";
                        else if (!string.IsNullOrWhiteSpace(res.Get("FECHARECIBO")) &&
                            !string.IsNullOrWhiteSpace(res.Get("FECHA_SOLICITADO")))
                            estado = "Entregado";
                        else if (!string.IsNullOrWhiteSpace(res.Get("FECHADEPOSITO")))
                            estado = "Depositado";
                    }

                    file_xml_name = res.Get("XML").Trim(); //file_xml_name = file_xml_name.Substring(file_xml_name.LastIndexOf("/") + 1);
                    file_pdf_name = res.Get("PDF").Trim(); //file_pdf_name = file_xml_name.Substring(file_pdf_name.LastIndexOf("/") + 1);																							 
                    folioCheque = res.Get("FOLIOCHEQUE");
                    return true;
                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }

        private string str_fecha(string fecha)
        {
            if (fecha == null || fecha.Trim() == "")
                return "NULL";
            return "'" + fecha.Trim() + "'";
        }

        public bool savePagoEdoCta()
        {

            if (tipoPago == "")
            {
                tipoPago = null;
            }
                 sql = "UPDATE ESTADODECUENTA SET"
                + " FECHA_SOLICITADO      = " + str_fecha(FechaSolicitado)
                + ",FECHADISPERSION       = " + str_fecha(FechaDispersion)
                + ",FECHADEPOSITO         = " + str_fecha(FechaDeposito)
                + ",FECHARECIBO           = " + str_fecha(FechaRecibo)
                + ",CVE_TIPODEPAGO        = '" + tipoPago + "'"
                + ",CVE_TIPOTRANSFERENCIA = '" + idTransferencia + "'"
                + ",USUARIO               = '" + this.sesion.nickName + "'"
                + ",FECHA_M               = GETDATE()"
                + ",FOLIOCHEQUE           = '" + folioCheque + "'"
                + " WHERE ID_ESTADODECUENTA = " + IdEdoCtaD;
            
            db.execute(sql);

            savePagoEdoCtaDetalle();
            
            return true;
        }

        public bool savePagoEdoCtaDetalle()
        {
            try
            {
                sql = "UPDATE ESTADODECUENTA_DETALLE SET ";
                sql += "FECHARECIBO = " + str_fecha(FechaRecibo);
                sql += ",FECHADISPERSION = " + str_fecha(FechaDispersion);
                sql += ",FECHADEPOSITO = " + str_fecha(FechaDeposito);
                if (FechaPago != "" && FechaPago != null && FechaPago != "")
                    sql += ",FECHADEPAGO = " + str_fecha(FechaPago) ;//JABB 16122019
                sql += ",CVE_TIPODEPAGO = '" + tipoPago + "'";
                sql += ",CVE_TIPOTRANSFERENCIA = '" + idTransferencia + "'";
                sql += ",USUARIO = '" + this.sesion.nickName + "'";
                sql += ",FECHA_M = GETDATE()";
                sql += " WHERE ID_ESTADODECUENTA =" + IdEdoCtaD;

                if (db.execute(sql))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public bool AgregarEdoCtaModal()
        {
            string sql = "";
            ResultSet rs = null;
            string CVE_TIPODEPAGO_up = "";
            mensaje_res = "";


        sql = "SELECT top 1 * FROM ESTADODECUENTA";
            sql += " WHERE CVE_SEDE = '" + CveSede + "'";
            sql += " AND ID_PERSONA = '" + IdPersona + "'";
            sql += " AND ID_ESQUEMA = '" + IdEsquema + "'";
            sql += " AND PERIODO = '" + Periodo + "'";

            rs = db.getTable(sql);

            try
            {
                if (rs != null)
                {
                    if (rs.Count > 0)
                    {
                        while (rs.Next())
                        {
                            if (rs.Get("CVE_TIPODEPAGO") == "")
                            {
                                CVE_TIPODEPAGO_up = null;
                            }
                            else
                            {
                                CVE_TIPODEPAGO_up = rs.Get("CVE_TIPODEPAGO");
                            }


                            ////sql = "INSERT INTO ESTADODECUENTA(";
                            ////sql += "CVE_SEDE";
                            ////sql += ",PERIODO";
                            ////sql += ",ID_ESQUEMA";
                            ////sql += ",PKCONCEPTOPAGO";
                            ////sql += ",ID_PERSONA";
                            ////sql += ",CVE_TIPODEPAGO";
                            ////sql += ",CAMPUS_INB";
                            ////sql += ",USUARIO";
                            ////sql += ") VALUES(";
                            ////sql += "'" + CveSede + "'";
                            ////sql += ",'" + Periodo + "'";
                            ////sql += ",'" + IdEsquema + "'";
                            ////sql += ",'" + ConceptoPagoID + "'";
                            ////sql += ",'" + IdPersona + "'";
                            ////sql += ",'" + rs.Get("CVE_TIPODEPAGO") + "'";
                            ////sql += ",'" + rs.Get("CAMPUS_INB") + "'";
                            ////sql += ",'" + sesion.nickName + "'";
                            ////sql += ")";

                            sql = "INSERT INTO ESTADODECUENTA(";
                            sql += "CVE_SEDE";
                            sql += ",PERIODO";
                            sql += ",ID_ESQUEMA";
                            sql += ",PKCONCEPTOPAGO";
                            sql += ",ID_PERSONA";
                            sql += ",CVE_TIPODEPAGO";
                            sql += ",CAMPUS_INB";
                            sql += ",USUARIO";
                            sql += ") VALUES(";
                            sql += "'" + CveSede + "'";
                            sql += ",'" + Periodo + "'";
                            sql += ",'" + IdEsquema + "'";
                            sql += ",'" + ConceptoPagoID + "'";
                            sql += ",'" + IdPersona + "'";
                            sql += ",'" + CVE_TIPODEPAGO_up + "'";
                            sql += ",'" + rs.Get("CAMPUS_INB") + "'";
                            sql += ",'" + sesion.nickName + "'";
                            sql += ")";

                            Debug.WriteLine("model agregarModal ESTADODECUENTA : " + sql);
                            IdEdoCta = db.executeId(sql);

                            if (AgregarEdoCtaDetalleModal())
                                return true;
                            else
                                return false;
                        }
                        return true;
                    }
                    else { mensaje_res = "NOEXISTE"; return false; }
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public bool AgregarEdoCtaDetalleModal()
        {
            string sql = "";
            ResultSet rs = null;
   

            sql = "SELECT top 1 * FROM ESTADODECUENTA_DETALLE";
            sql += " WHERE CVE_SEDE = '" + CveSede + "'";
            sql += " AND ID_PERSONA = '" + IdPersona + "'";
            sql += " AND ID_ESQUEMA = '" + IdEsquema + "'";
            sql += " AND PERIODO = '" + Periodo + "'";

            rs = db.getTable(sql);
            
            try
            {
                if (rs != null)
                {
                    if (rs.Count > 0)
                    {
                        while (rs.Next())          
                        {
                            sql = "INSERT INTO ESTADODECUENTA_DETALLE(";
                            sql += "ID_ESTADODECUENTA";
                            sql += ",CVE_SEDE";
                            sql += ",PERIODO";
                            sql += ",PARTEDELPERIODO";
                            sql += ",ID_ESQUEMA";
                            sql += ",PKCONCEPTOPAGO";
                            sql += ",ID_PERSONA";
                            sql += ",CVE_TIPODEPAGO";
                            sql += ",CAMPUS_INB";
                            sql += ",USUARIO";
                            sql += ",ID_NOMINA";
                            sql += ",CVE_ESCUELA";
                            sql += ",ID_CENTRODECOSTOS";
                            sql += ",INDICADOR";
                            sql += ",FECHADEPAGO";
                            sql += ") VALUES(";
                            sql += "'" + IdEdoCta + "'";
                            sql += ",'" + CveSede + "'";
                            sql += ",'" + Periodo + "'";
                            sql += ",'" + PartePeriodo + "'";
                            sql += ",'" + IdEsquema + "'";
                            sql += ",'" + ConceptoPagoID + "'";
                            sql += ",'" + IdPersona + "'";
                            sql += ",'" + rs.Get("CVE_TIPODEPAGO") + "'";
                            sql += ",'" + rs.Get("CAMPUS_INB") + "'";
                            sql += ",'" + sesion.nickName + "'";
                            sql += ",'" + rs.Get("ID_NOMINA") + "'";
                            sql += ",'" + rs.Get("CVE_ESCUELA") + "'";
                            sql += ",'" + rs.Get("ID_CENTRODECOSTOS") + "'";
                            sql += ",'" + rs.Get("INDICADOR") + "'";
                            sql += ",'" + FPago + "'";
                            sql += ")";

                            Debug.WriteLine("model agregarModal ESTADODECUENTA_DETALLE : " + sql);
                            db.execute(sql);

                            return true;
                        }
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public bool AgregarEdoCtaModalPensionado()
        {
            string sql = "";
            ResultSet rs = null;
            Existe = true;

            string condicion = "";
            if (ConceptoPagoID != "" && ConceptoPagoID != null)
                condicion = " AND PKCONCEPTOPAGO = '" + ConceptoPagoID + "'";

            sql = "SELECT * FROM ESTADODECUENTA";
            sql += " WHERE CVE_SEDE = '" + CveSede + "'";
            sql += " AND ID_PERSONA = '" + IdPersona + "'";
            sql += " AND ID_ESQUEMA = '" + IdEsquema + "'";
            sql += condicion;

            rs = db.getTable(sql);

            try
            {
                if (rs != null)
                {
                    if (rs.Count > 0)
                    {
                        while (rs.Next())
                        {
                            sql = "INSERT INTO ESTADODECUENTA_PENSIONADOS(";
                            sql += "ID_PENSIONADO";
                            sql += ",ID_ESTADODECUENTA";
                            sql += ",ID_PERSONA";
                            sql += ",PKCONCEPTOPAGO";
                            sql += ",PARTEDELPERIODO";
                            sql += ",USUARIO";
                            sql += ") VALUES(";
                            sql += "'" + IdBeneficiario + "'";
                            sql += ",'" + rs.Get("ID_ESTADODECUENTA") + "'";
                            sql += ",'" + IdPersona + "'";
                            sql += ",'" + rs.Get("PKCONCEPTOPAGO") + "'";
                            sql += ",'" + PartePeriodo + "'";
                            sql += ",'" + sesion.nickName + "'";
                            sql += ")";

                            db.execute(sql);
                        }
                    }
                    else
                    {
                        Existe = false;
                        return true;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }

        public bool EliminarPensionado()
        {
            try
            {
                sql = "DELETE FROM ESTADODECUENTA_PENSIONADOS WHERE PK1=" + IdCtaPensionado;
                db.execute(sql);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Dictionary<string, string> Obtener_TipoBloqueo()
        {
            string sql = "SELECT CVE_BLOQUEO,BLOQUEO FROM BLOQUEOS ORDER BY CVE_BLOQUEO";
            ResultSet rs = db.getTable(sql);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            while (rs.Next())
                dict.Add(rs.Get("CVE_BLOQUEO"), rs.Get("BLOQUEO"));
            return dict;
        }

        public bool seleccionarPadre()
        {
            try
            {
                seleccionar = false;

                sql = "SELECT * FROM VESTADO_CUENTA_DETALLE WHERE  IDSIU =  '" + IdSIU + "' AND CVE_SEDE = '" + CveSede + "' AND ID_ESTADODECUENTA = " + IdEdoCta; 
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    if (res.Count == 1)
                    {
                        seleccionar = true;
                        return seleccionar;
                    }
                    else
                    {
                        seleccionar = false;
                        return seleccionar;
                    }
                }

                return seleccionar;
            }
            catch
            {              
                return seleccionar;
            }
        }

        //public bool getCentroCostos()
        //{
        //    string centrocostos = "";
        //    try
        //    {
        //        string buscarEscuela;
        //        if (Escuela == "" || Escuela == null) { buscarEscuela = ""; }
        //        else { buscarEscuela = " AND CVE_ESCUELA = '" + Escuela + "'"; }

        //        string sql = "SELECT ID_CENTRODECOSTOS,CENTRODECOSTOS FROM CENTRODECOSTOS WHERE CVE_SEDE = '" + CveSede + "' " + buscarEscuela + " ORDER BY ID_CENTRODECOSTOS ";
        //        ResultSet res = db.getTable(sql);

        //        if (res != null)
        //        {
        //            while (res.Next())
        //            {
        //                centrocostos += "<option value='" + res.Get("ID_CENTRODECOSTOS") + "'>" + res.Get("CENTRODECOSTOS") + "</option>";
        //            }

        //            CentroCostos = centrocostos;
        //            return true;
        //        }
        //        else
        //        { return false; }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}