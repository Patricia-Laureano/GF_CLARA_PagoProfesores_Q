using ConnectDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace PagoProfesores.Models.Pagos
{
    public class NominaModel : SuperModel
    {
        public string IdSiu { get; set; }
        public string IdPA { get; set; }
        public string IdEsquema { get; set; }
        public string Periodo { get; set; }
        public string PartePeriodo { get; set; }
        public string StrmFechaI { get; set; }
        public string StrmFechaF { get; set; }
        public DateTime FechaI { get; set; }
        public DateTime FechaF { get; set; }
        public string NoSemanas { get; set; }
        public string NoPagos { get; set; }
        public string Nivel { get; set; }
        public string Escuela { get; set; }
        public string CampusVPDI { get; set; }
        public string CampusINB { get; set; }
        public string NRC { get; set; }
        public string NombreMateria { get; set; }
        public string Profesor { get; set; }
        public string CentroCostosID { get; set; }
        public string CentroCostos { get; set; }
        public string OpcionPago { get; set; }
        public string StrmIdsPA { get; set; }
        public string Tabulador { get; set; }
        
        public string sql { get; set; }

        public Dictionary<string, string> getCampusPA()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_SEDE,SEDE FROM QSedesPorUsuario01 ORDER BY CVE_SEDE ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_SEDE"), res.Get("SEDE"));

            return dict;
        }

        public Dictionary<string, string> getEsquemaPago()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT ID_ESQUEMA,ESQUEMADEPAGODES FROM ESQUEMASDEPAGO WHERE PERIODO = '"+ Periodo +"' ORDER BY ID_ESQUEMA ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("ID_ESQUEMA"), res.Get("ESQUEMADEPAGODES"));

            return dict;
        }

        public bool getDetalleEsquemaPago()
        {
            try
            {
                string sql = "SELECT NOSEMANAS,NOPAGOS,FECHAINICIO,FECHAFIN FROM ESQUEMASDEPAGO WHERE ID_ESQUEMA = '" + IdEsquema + "' AND CVE_SEDE = '" + CampusVPDI + "'  ORDER BY ID_ESQUEMA ";
               // string sql = "SELECT NOSEMANAS,NOPAGOS,FECHAINICIO,FECHAFIN FROM ESQUEMASDEPAGO WHERE ID_ESQUEMA = '" + IdEsquema + "' ORDER BY ID_ESQUEMA ";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    NoSemanas = res.Get("NOSEMANAS");
                    NoPagos = res.Get("NOPAGOS");
                    StrmFechaI = res.GetDateTime("FECHAINICIO").ToString("yyyy-MM-dd");
                    StrmFechaF = res.GetDateTime("FECHAFIN").ToString("yyyy-MM-dd");

                    return true;

                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }

        public bool getCentroCostos()
        {
            string centrocostos = "";
            try
            {
                string buscarEscuela;
                if (Escuela == "" || Escuela == null ) {   buscarEscuela = "";  }
                else { buscarEscuela = " AND CVE_ESCUELA = '" + Escuela + "'";  }

                string sql = "SELECT ID_CENTRODECOSTOS,CENTRODECOSTOS FROM CENTRODECOSTOS WHERE CVE_SEDE = '" + CampusVPDI + "' " + buscarEscuela + " ORDER BY ID_CENTRODECOSTOS ";
                ResultSet res = db.getTable(sql);
                
                if (res != null)
                {
                    while (res.Next())
                    {
                        centrocostos += "<option value='" + res.Get("ID_CENTRODECOSTOS") + "'>" + res.Get("CENTRODECOSTOS") + "</option>";
                    }

                    CentroCostos = centrocostos;
                    return true;
                }
                else
                { return false; }
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
                sql = "SELECT IDSIU, ID_PA, NRC, NOMBREMATERIA, NOMBRE + ' ' + APELLIDOS AS PROFESOR, ID_CENTRODECOSTOS, CVE_SEDE, case when CVE_OPCIONDEPAGO = 2 then 'H' when CVE_OPCIONDEPAGO = 3 THEN 'A' else 'X' end CVE_OPCIONDEPAGO, OPCIONDEPAGO  FROM PA where ID_PA = " + IdPA; //update quitar string
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    IdSiu = res.Get("IDSIU");
                    IdPA = res.Get("ID_PA");
                    NRC = res.Get("NRC");
                    NombreMateria = res.Get("NOMBREMATERIA");
                    Profesor = res.Get("PROFESOR");
                    CentroCostosID = res.Get("ID_CENTRODECOSTOS");
                    CampusVPDI = res.Get("CVE_SEDE");
                    OpcionPago = res.Get("CVE_OPCIONDEPAGO");

                    return true;
                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }

        public bool asginaEsquemadePago()
        {
            List<Parametros> lParamS;
            Parametros paramEsquema;
            Parametros paramPAID;
            bool exito = false;

            string data = this.StrmIdsPA;

            string[] arrChecked = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (string itemChecked in arrChecked)
                {
                    this.IdPA = itemChecked;

                    lParamS = new List<Parametros>();
                    paramEsquema = new Parametros();
                    paramPAID = new Parametros();

                    paramEsquema.nombreParametro = "@esquema";
                    paramEsquema.tipoParametro = SqlDbType.BigInt;
                    paramEsquema.direccion = ParameterDirection.Input;
                    paramEsquema.value = IdEsquema;
                    lParamS.Add(paramEsquema);

                    paramPAID.nombreParametro = "@paid";
                    paramPAID.tipoParametro = SqlDbType.BigInt;
                    paramPAID.direccion = ParameterDirection.Input;
                    paramPAID.value = IdPA;
                    lParamS.Add(paramPAID);

                    exito = db.ExecuteStoreProcedure("sp_asigna_esquema_a_pa", lParamS);
               }

            }
            catch
            {
                return false;
            }
            return true;
        }
        
        public bool asginaCentrodeCostos()
        {
            string data = this.StrmIdsPA;

            string[] arrChecked = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (string itemChecked in arrChecked)
                {
                    this.IdPA = itemChecked;

                    sql = "UPDATE PA SET ";

                    sql += "ID_CENTRODECOSTOS = '" + CentroCostosID + "'";
                    sql += " WHERE INDICADOR = 0 and ID_PA = " + IdPA + "";
                    Debug.WriteLine("model centro costos  PA : " + sql);

                    db.execute(sql);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        public bool asginaEsquemaPago() // INSERTA A PA SEGÚN LOS FILTROS SELECCIONADOS Y NO POR ID DE PERSONA
        {

            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramEsquema = new Parametros();
            Parametros paramPeriodo = new Parametros();
            Parametros paramPartePeriodo = new Parametros();
            Parametros paramCampusVPDI = new Parametros();
            Parametros paramCampusINB = new Parametros();
            Parametros paramEscuela = new Parametros();
            Parametros paramOpcionP = new Parametros();

            try
            {
                paramEsquema.nombreParametro = "@esquema";
                paramEsquema.tipoParametro = SqlDbType.BigInt;
                paramEsquema.direccion = ParameterDirection.Input;
                paramEsquema.value = IdEsquema;
                lParamS.Add(paramEsquema);

                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.longitudParametro = 3;
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = PartePeriodo;
                lParamS.Add(paramPartePeriodo);

                paramCampusVPDI.nombreParametro = "@campusVPDI";
                paramCampusVPDI.longitudParametro = 3;
                paramCampusVPDI.tipoParametro = SqlDbType.NVarChar;
                paramCampusVPDI.direccion = ParameterDirection.Input;
                paramCampusVPDI.value = CampusVPDI;
                lParamS.Add(paramCampusVPDI);

                paramCampusINB.nombreParametro = "@campusINB";
                paramCampusINB.longitudParametro = 3;
                paramCampusINB.tipoParametro = SqlDbType.NVarChar;
                paramCampusINB.direccion = ParameterDirection.Input;
                paramCampusINB.value = CampusINB;
                lParamS.Add(paramCampusINB);

                paramEscuela.nombreParametro = "@escuela";
                paramEscuela.longitudParametro = 5;
                paramEscuela.tipoParametro = SqlDbType.NVarChar;
                paramEscuela.direccion = ParameterDirection.Input;
                paramEscuela.value = Escuela;
                lParamS.Add(paramEscuela);

                paramOpcionP.nombreParametro = "@opcionPago";
                paramOpcionP.longitudParametro = 5;
                paramOpcionP.tipoParametro = SqlDbType.NVarChar;
                paramOpcionP.direccion = ParameterDirection.Input;
                paramOpcionP.value = OpcionPago;
                lParamS.Add(paramOpcionP);
                
                exito = db.ExecuteStoreProcedure("sp_asigna_esquema_a_pa_all", lParamS);

                if (exito == false)
                    return false;

                return true;
            } catch
            {
                return false;
            }
        }

        public bool asginaCentroCostos()
        {

            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramPeriodo = new Parametros();
            Parametros paramPartePeriodo = new Parametros();
            Parametros paramCampusVPDI = new Parametros();
            Parametros paramCampusINB = new Parametros();
            Parametros paramNivel = new Parametros();
            Parametros paramEscuela = new Parametros();

            try
            {
                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.longitudParametro = 3;
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = PartePeriodo;
                lParamS.Add(paramPartePeriodo);

                paramCampusVPDI.nombreParametro = "@campusVPDI";
                paramCampusVPDI.longitudParametro = 3;
                paramCampusVPDI.tipoParametro = SqlDbType.NVarChar;
                paramCampusVPDI.direccion = ParameterDirection.Input;
                paramCampusVPDI.value = CampusVPDI;
                lParamS.Add(paramCampusVPDI);

                paramCampusINB.nombreParametro = "@campusINB";
                paramCampusINB.longitudParametro = 3;
                paramCampusINB.tipoParametro = SqlDbType.NVarChar;
                paramCampusINB.direccion = ParameterDirection.Input;
                paramCampusINB.value = CampusINB;
                lParamS.Add(paramCampusINB);

                //paramNivel.nombreParametro = "@nivel";
                //paramNivel.longitudParametro = 5;
                //paramNivel.tipoParametro = SqlDbType.NVarChar;
                //paramNivel.direccion = ParameterDirection.Input;
                //paramNivel.value = Nivel;
                //lParamS.Add(paramNivel);

                paramEscuela.nombreParametro = "@escuela";
                paramEscuela.longitudParametro = 5;
                paramEscuela.tipoParametro = SqlDbType.NVarChar;
                paramEscuela.direccion = ParameterDirection.Input;
                paramEscuela.value = Escuela;
                lParamS.Add(paramEscuela);

                exito = db.ExecuteStoreProcedure("sp_asigna_centrocosto_a_pa_all", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool generaCentroCostos()
        {

            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramPeriodo = new Parametros();
            Parametros paramPartePeriodo = new Parametros();
            Parametros paramCampusVPDI = new Parametros();
            Parametros paramCampusINB = new Parametros();
            Parametros paramEscuela = new Parametros();
            Parametros paramOpcionPago = new Parametros();

            try
            {
                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.longitudParametro = 3;
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = PartePeriodo;
                lParamS.Add(paramPartePeriodo);

                paramCampusVPDI.nombreParametro = "@campusVPDI";
                paramCampusVPDI.longitudParametro = 3;
                paramCampusVPDI.tipoParametro = SqlDbType.NVarChar;
                paramCampusVPDI.direccion = ParameterDirection.Input;
                paramCampusVPDI.value = CampusVPDI;
                lParamS.Add(paramCampusVPDI);

                paramCampusINB.nombreParametro = "@campusINB";
                paramCampusINB.longitudParametro = 3;
                paramCampusINB.tipoParametro = SqlDbType.NVarChar;
                paramCampusINB.direccion = ParameterDirection.Input;
                paramCampusINB.value = CampusINB;
                lParamS.Add(paramCampusINB);

                paramEscuela.nombreParametro = "@escuela";
                paramEscuela.longitudParametro = 5;
                paramEscuela.tipoParametro = SqlDbType.NVarChar;
                paramEscuela.direccion = ParameterDirection.Input;
                paramEscuela.value = Escuela;
                lParamS.Add(paramEscuela);

                paramOpcionPago.nombreParametro = "@opcionP";
                paramOpcionPago.longitudParametro = 1;
                paramOpcionPago.tipoParametro = SqlDbType.NVarChar;
                paramOpcionPago.direccion = ParameterDirection.Input;
                paramOpcionPago.value = OpcionPago;
                lParamS.Add(paramOpcionPago);

                exito = db.ExecuteStoreProcedure("sp_asigna_centrocosto_a_pa_all", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool grabaCentroCosto()
        {

            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramIDPA = new Parametros();
            Parametros paramIDCentroCosto = new Parametros();

            try
            {
                paramIDPA.nombreParametro = "@id_pa";
                paramIDPA.tipoParametro = SqlDbType.BigInt;
                paramIDPA.direccion = ParameterDirection.Input;
                paramIDPA.value = IdPA;
                lParamS.Add(paramIDPA);

                paramIDCentroCosto.nombreParametro = "@id_centrocosto";
                paramIDCentroCosto.tipoParametro = SqlDbType.BigInt;
                paramIDCentroCosto.direccion = ParameterDirection.Input;
                paramIDCentroCosto.value = CentroCostosID;
                lParamS.Add(paramIDCentroCosto);

                exito = db.ExecuteStoreProcedure("sp_asigna_centrocosto_a_pa", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool insertaNominaXCentrodeCosto()
        {

            bool exito = false;
            DataTable dtRes = new DataTable();

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramPeriodo = new Parametros();
            Parametros paramPartePeriodo = new Parametros();
            Parametros paramEscuela = new Parametros();
            Parametros paramOpcionPago = new Parametros();
            Parametros paramCampusVPDI = new Parametros();
            Parametros paramCampusINB = new Parametros();

            try
            {
                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.longitudParametro = 3;
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = PartePeriodo;
                lParamS.Add(paramPartePeriodo);

                paramEscuela.nombreParametro = "@escuela";
                paramEscuela.longitudParametro = 5;
                paramEscuela.tipoParametro = SqlDbType.NVarChar;
                paramEscuela.direccion = ParameterDirection.Input;
                paramEscuela.value = Escuela;
                lParamS.Add(paramEscuela);

                paramOpcionPago.nombreParametro = "@cveOpcionPago";
                paramOpcionPago.longitudParametro = 5;
                paramOpcionPago.tipoParametro = SqlDbType.NVarChar;
                paramOpcionPago.direccion = ParameterDirection.Input;
                paramOpcionPago.value = OpcionPago;
                lParamS.Add(paramOpcionPago);

                paramCampusVPDI.nombreParametro = "@campusVPDI";
                paramCampusVPDI.longitudParametro = 3;
                paramCampusVPDI.tipoParametro = SqlDbType.NVarChar;
                paramCampusVPDI.direccion = ParameterDirection.Input;
                paramCampusVPDI.value = CampusVPDI;
                lParamS.Add(paramCampusVPDI);

                paramCampusINB.nombreParametro = "@campusINB";
                paramCampusINB.longitudParametro = 3;
                paramCampusINB.tipoParametro = SqlDbType.NVarChar;
                paramCampusINB.direccion = ParameterDirection.Input;
                paramCampusINB.value = CampusINB;
                lParamS.Add(paramCampusINB);

                //exito = db.ExecuteStoreProcedure("sp_inserta_nomina_xcentrodecosto", lParamS);
                dtRes = db.SelectDataTableFromStoreProcedure("sp_inserta_nomina_xcentrodecosto", lParamS);

                if (dtRes.Rows.Count >= 1)
                    exito = true;

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool cambiaTabulador()
        {
            List<Parametros> lParamS;
            Parametros paramTabulador;
            Parametros paramSede;
            Parametros paramPAID;
            bool exito = false;

            string data = this.StrmIdsPA;

            string[] arrChecked = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (string itemChecked in arrChecked)
                {
                    this.IdPA = itemChecked;

                    lParamS = new List<Parametros>();
                    paramTabulador = new Parametros();
                    paramSede = new Parametros();
                    paramPAID = new Parametros();

                    paramTabulador.nombreParametro = "@tabulador";
                    paramTabulador.tipoParametro = SqlDbType.NVarChar;
                    paramTabulador.longitudParametro = 15;
                    paramTabulador.direccion = ParameterDirection.Input;
                    paramTabulador.value = Tabulador;
                    lParamS.Add(paramTabulador);

                    paramSede.nombreParametro = "@sede";
                    paramSede.tipoParametro = SqlDbType.NVarChar;
                    paramSede.longitudParametro = 5;
                    paramSede.direccion = ParameterDirection.Input;
                    paramSede.value = CampusVPDI;
                    lParamS.Add(paramSede);

                    paramPAID.nombreParametro = "@paid";
                    paramPAID.tipoParametro = SqlDbType.BigInt;
                    paramPAID.direccion = ParameterDirection.Input;
                    paramPAID.value = IdPA;
                    lParamS.Add(paramPAID);

                    exito = db.ExecuteStoreProcedure("sp_cambia_tabulador_a_pa", lParamS);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool cambiaTabuladorTodo()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramTabulador = new Parametros();
            Parametros paramPeriodo = new Parametros();
            Parametros paramPartePeriodo = new Parametros();
            Parametros paramCampusVPDI = new Parametros();
            Parametros paramCampusINB = new Parametros();
            Parametros paramEscuela = new Parametros();
            Parametros paramOpcionP = new Parametros();

            try
            {
                paramTabulador.nombreParametro = "@tabulador";
                paramTabulador.tipoParametro = SqlDbType.NVarChar;
                paramTabulador.longitudParametro = 15;
                paramTabulador.direccion = ParameterDirection.Input;
                paramTabulador.value = Tabulador;
                lParamS.Add(paramTabulador);

                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.longitudParametro = 3;
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = (PartePeriodo == "" ? null : PartePeriodo);
                lParamS.Add(paramPartePeriodo);

                paramCampusVPDI.nombreParametro = "@campusVPDI";
                paramCampusVPDI.longitudParametro = 3;
                paramCampusVPDI.tipoParametro = SqlDbType.NVarChar;
                paramCampusVPDI.direccion = ParameterDirection.Input;
                paramCampusVPDI.value = CampusVPDI;
                lParamS.Add(paramCampusVPDI);

                paramCampusINB.nombreParametro = "@campusINB";
                paramCampusINB.longitudParametro = 3;
                paramCampusINB.tipoParametro = SqlDbType.NVarChar;
                paramCampusINB.direccion = ParameterDirection.Input;
                paramCampusINB.value = CampusINB;
                lParamS.Add(paramCampusINB);

                paramEscuela.nombreParametro = "@escuela";
                paramEscuela.longitudParametro = 5;
                paramEscuela.tipoParametro = SqlDbType.NVarChar;
                paramEscuela.direccion = ParameterDirection.Input;
                paramEscuela.value = (Escuela == "" ? null : Escuela);
                lParamS.Add(paramEscuela);

                paramOpcionP.nombreParametro = "@opcionPago";
                paramOpcionP.longitudParametro = 5;
                paramOpcionP.tipoParametro = SqlDbType.NVarChar;
                paramOpcionP.direccion = ParameterDirection.Input;
                paramOpcionP.value = (OpcionPago == "" ? null : OpcionPago);
                lParamS.Add(paramOpcionP);

                exito = db.ExecuteStoreProcedure("sp_cambia_tabulador_a_pa_all", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<PersonaCC> validaAsignacion()
        {

            string data = this.StrmIdsPA;

            string[] arrChecked = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //Dictionary<string, PersonaCC> dictCC = new Dictionary<string, PersonaCC>();
            PersonaCC pcc;
            List<PersonaCC> lPersonasCC = new List<PersonaCC>();

            try
            {
                ResultSet res;
                foreach (string itemChecked in arrChecked)
                {
                    this.IdPA = itemChecked;

                    sql = "select * from PA where ID_PA = " + IdPA;
                    //Debug.WriteLine("model centro costos  PA : " + sql);
                    res = db.getTable(sql);

                    if (res.Next())
                    {
                        sql = " select count(1) as MAX                                          " +
                              "   from CENTRODECOSTOS                                           " +
                              "  where ID_CENTRODECOSTOS = " + CentroCostosID                     +
                              "    and CVE_TIPODEPAGO    = '" + res.Get("CVE_OPCIONDEPAGO") + "'" + 
                              "    and CVE_ESCUELA       = '" + res.Get("CVE_ESCUELA")      + "'" + 
                              "    and CVE_SEDE          = '" + res.Get("CVE_SEDE")         + "'" +
                              "    and ACTIVA            = 1";

                        pcc = new PersonaCC(res.Get("ID_PA").ToString(), res.Get("IDSIU").ToString(), res.Get("NOMBRE").ToString() + " " + res.Get("APELLIDOS").ToString(), res.Get("CVE_OPCIONDEPAGO").ToString(), res.Get("NRC").ToString());

                        if (db.Count(sql) == 0)
                        {
                            lPersonasCC.Add(pcc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return lPersonasCC;
        }
    }
}