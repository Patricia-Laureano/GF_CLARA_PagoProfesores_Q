using ConnectDB;
using System;
using System.Collections.Generic;
using System.Data;

namespace PagoProfesores.Models.Pagos
{
    public class NominaXCDCModel : SuperModel
    {
        public string Periodo { get; set; }
        public string CampusVPDI { get; set; }
        public string Campus { get; set; }
        public string TipoPago { get; set; }
        public string Escuela { get; set; }
        public string PartePeriodo { get; set; }

        public string PersonaID { get; set; }
        public string CentroCostosID { get; set; }
        public string EsquemaID { get; set; }
        public string MontoPagar { get; set; }
        public string TipoPagoCVE { get; set; }
        public string nominaID { get; set; }
        public string partePeriodoX { get; set; }
        public string cveEscuela { get; set; }
        public string tipoDocente { get; set; }
        
        public string sql { get; set; }

        public bool insertaEntregaContratosXEsquemaPago()
        {

            bool exito = false;

            List<Parametros> lParamS   = new List<Parametros>();
            Parametros paramPeriodo    = new Parametros();
            Parametros paramCampusVPDI = new Parametros();
            Parametros paramTipoPago   = new Parametros();

            try
            {
                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramCampusVPDI.nombreParametro = "@campusVPDI";
                paramCampusVPDI.longitudParametro = 3;
                paramCampusVPDI.tipoParametro = SqlDbType.NVarChar;
                paramCampusVPDI.direccion = ParameterDirection.Input;
                paramCampusVPDI.value = CampusVPDI;
                lParamS.Add(paramCampusVPDI);

                paramTipoPago.nombreParametro = "@tipoPagoCVE";
                paramTipoPago.longitudParametro = 5;
                paramTipoPago.tipoParametro = SqlDbType.NVarChar;
                paramTipoPago.direccion = ParameterDirection.Input;
                paramTipoPago.value = TipoPago;
                lParamS.Add(paramTipoPago);

                exito = db.ExecuteStoreProcedure("sp_inserta_entregadecontratosXEsquemaPago", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool calculaEstadocuentaXEsquemaPago()
        {

            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramPeriodo = new Parametros();
            Parametros paramCampusVPDI = new Parametros();
            Parametros paramTipoPago = new Parametros();

            try
            {
                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramCampusVPDI.nombreParametro = "@campusVPDI";
                paramCampusVPDI.longitudParametro = 3;
                paramCampusVPDI.tipoParametro = SqlDbType.NVarChar;
                paramCampusVPDI.direccion = ParameterDirection.Input;
                paramCampusVPDI.value = CampusVPDI;
                lParamS.Add(paramCampusVPDI);

                paramTipoPago.nombreParametro = "@tipoPagoCVE";
                paramTipoPago.longitudParametro = 5;
                paramTipoPago.tipoParametro = SqlDbType.NVarChar;
                paramTipoPago.direccion = ParameterDirection.Input;
                paramTipoPago.value = TipoPago;
                lParamS.Add(paramTipoPago);

                exito = db.ExecuteStoreProcedure("sp_calcula_estadocuentaXEsquemaPago", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool calculaEstadocuentaXRegistroNomina(string usuario, string indicador = "1")
        {
            List<Parametros> lParamS;
            Parametros paramPeriodo;
            Parametros paramCampusVPDI;
            Parametros paramCampus;
            Parametros paramEscuela;
            Parametros paramTipoPagoCVE;
            Parametros paramEsquemaID;
            Parametros paramMontoPagar;
            Parametros paramPersonaID;
            Parametros paramCentroCostoID;
            Parametros paramNominaID;
            Parametros paramUsuario;
            Parametros paramPartePeriodo;
            Parametros paramTipoDocente;

            bool exito = false;

            try
            {
                
                sql = " select ID_NOMINA, ID_PERSONA, ID_CENTRODECOSTOS, ID_ESQUEMA, MONTOAPAGAR, CVE_TIPODEPAGO, CVE_ESCUELA, PARTEDELPERIODO, CVE_TIPODEDOCENTE "
                    + "   from NOMINA "
                    + "  where INDICADOR        = " + indicador
                    + "    and PERIODO          = '" + Periodo + "'"
                    + "    and CVE_SEDE         = '" + CampusVPDI + "' "
                    + "    and CAMPUS_INB       = '" + Campus + "'"
                    + "    and (CVE_ESCUELA     = " + ((Escuela      == null || Escuela      == "") ? "null" : "'" + Escuela      + "'") + " or " + ((Escuela      == null || Escuela      == "") ? "null" : "'" + Escuela      + "'") + " is null) "
                    + "    and (PARTEDELPERIODO = " + ((PartePeriodo == null || PartePeriodo == "") ? "null" : "'" + PartePeriodo + "'") + " or " + ((PartePeriodo == null || PartePeriodo == "") ? "null" : "'" + PartePeriodo + "'") + " is null) ";

                if (TipoPago != "" && TipoPago != null) sql += " and CVE_TIPODEPAGO = '" + TipoPago + "' ";

                ResultSet res = db.getTable(sql);

                while(res.Next())
                {

                    sql = "";
                    sql = "SELECT * FROM PERSONAS WHERE ID_PERSONA=" + res.Get("ID_PERSONA");
                    ResultSet res2= db.getTable(sql);
                    if (res2.Next())
                    {
                        PersonaID = res.Get("ID_PERSONA");
                        CentroCostosID = res.Get("ID_CENTRODECOSTOS");
                        EsquemaID = res.Get("ID_ESQUEMA");
                        MontoPagar = res.Get("MONTOAPAGAR");
                        TipoPagoCVE = res.Get("CVE_TIPODEPAGO");
                        nominaID = res.Get("ID_NOMINA");
                        partePeriodoX = res.Get("PARTEDELPERIODO");
                        cveEscuela = res.Get("CVE_ESCUELA");
                        tipoDocente = res.Get("CVE_TIPODEDOCENTE");

                        lParamS = new List<Parametros>();

                        paramPeriodo = new Parametros();
                        paramPeriodo.nombreParametro = "@periodo";
                        paramPeriodo.longitudParametro = 10;
                        paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                        paramPeriodo.direccion = ParameterDirection.Input;
                        paramPeriodo.value = Periodo;
                        lParamS.Add(paramPeriodo);

                        paramPartePeriodo = new Parametros();
                        paramPartePeriodo.nombreParametro = "@partePeriodo";
                        paramPartePeriodo.longitudParametro = 3;
                        paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                        paramPartePeriodo.direccion = ParameterDirection.Input;
                        paramPartePeriodo.value = partePeriodoX;
                        lParamS.Add(paramPartePeriodo);

                        paramCampusVPDI = new Parametros();
                        paramCampusVPDI.nombreParametro = "@campusVPDI";
                        paramCampusVPDI.longitudParametro = 3;
                        paramCampusVPDI.tipoParametro = SqlDbType.NVarChar;
                        paramCampusVPDI.direccion = ParameterDirection.Input;
                        paramCampusVPDI.value = CampusVPDI;
                        lParamS.Add(paramCampusVPDI);

                        paramCampus = new Parametros();
                        paramCampus.nombreParametro = "@campus";
                        paramCampus.longitudParametro = 3;
                        paramCampus.tipoParametro = SqlDbType.NVarChar;
                        paramCampus.direccion = ParameterDirection.Input;
                        paramCampus.value = Campus;
                        lParamS.Add(paramCampus);

                        paramEscuela = new Parametros();
                        paramEscuela.nombreParametro = "@escuela";
                        paramEscuela.longitudParametro = 3;
                        paramEscuela.tipoParametro = SqlDbType.NVarChar;
                        paramEscuela.direccion = ParameterDirection.Input;
                        paramEscuela.value = cveEscuela;
                        lParamS.Add(paramEscuela);

                        paramTipoPagoCVE = new Parametros();
                        paramTipoPagoCVE.nombreParametro = "@tipoPagoCVE";
                        paramTipoPagoCVE.longitudParametro = 5;
                        paramTipoPagoCVE.tipoParametro = SqlDbType.NVarChar;
                        paramTipoPagoCVE.direccion = ParameterDirection.Input;
                        paramTipoPagoCVE.value = TipoPagoCVE;
                        lParamS.Add(paramTipoPagoCVE);

                        paramEsquemaID = new Parametros();
                        paramEsquemaID.nombreParametro = "@esquemaID";
                        paramEsquemaID.tipoParametro = SqlDbType.BigInt;
                        paramEsquemaID.direccion = ParameterDirection.Input;
                        paramEsquemaID.value = EsquemaID;
                        lParamS.Add(paramEsquemaID);

                        paramMontoPagar = new Parametros();
                        paramMontoPagar.nombreParametro = "@montoapagar";
                        paramMontoPagar.tipoParametro = SqlDbType.Real;
                        paramMontoPagar.direccion = ParameterDirection.Input;
                        paramMontoPagar.value = MontoPagar;
                        lParamS.Add(paramMontoPagar);

                        paramPersonaID = new Parametros();
                        paramPersonaID.nombreParametro = "@personaID";
                        paramPersonaID.tipoParametro = SqlDbType.BigInt;
                        paramPersonaID.direccion = ParameterDirection.Input;
                        paramPersonaID.value = PersonaID;
                        lParamS.Add(paramPersonaID);

                        paramCentroCostoID = new Parametros();
                        paramCentroCostoID.nombreParametro = "@centroCostosID";
                        paramCentroCostoID.tipoParametro = SqlDbType.BigInt;
                        paramCentroCostoID.direccion = ParameterDirection.Input;
                        paramCentroCostoID.value = CentroCostosID;
                        lParamS.Add(paramCentroCostoID);

                        paramNominaID = new Parametros();
                        paramNominaID.nombreParametro = "@nominaID";
                        paramNominaID.tipoParametro = SqlDbType.Int;
                        paramNominaID.direccion = ParameterDirection.Input;
                        paramNominaID.value = nominaID;
                        lParamS.Add(paramNominaID);

                        paramTipoDocente = new Parametros();
                        paramTipoDocente.nombreParametro = "@tipoDocente";
                        paramTipoDocente.longitudParametro = 4;
                        paramTipoDocente.tipoParametro = SqlDbType.NVarChar;
                        paramTipoDocente.direccion = ParameterDirection.Input;
                        paramTipoDocente.value = tipoDocente;
                        lParamS.Add(paramTipoDocente);

                        paramUsuario = new Parametros();
                        paramUsuario.nombreParametro = "@usuario";
                        paramUsuario.longitudParametro = 180;
                        paramUsuario.tipoParametro = SqlDbType.NVarChar;
                        paramUsuario.direccion = ParameterDirection.Input;
                        paramUsuario.value = usuario;
                        lParamS.Add(paramUsuario);

                        exito = db.ExecuteStoreProcedure("sp_calcula_estadocuentaXRegistroNomina", lParamS);
                        if (exito == false)
                            return false;
                    }

                    
                }
                return true;
            }
            catch (Exception E)
            {
                return false;
            }
        }
    }
}