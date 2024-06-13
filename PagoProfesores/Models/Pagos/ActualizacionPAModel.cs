using ConnectDB;
using Newtonsoft.Json;
using Session;
using System;
using System.Collections.Generic;
using System.Data;

namespace PagoProfesores.Models.Pagos
{
    public partial class ActualizacionPAModel : SuperModel
    {
        [JsonProperty("CAMPUSVPDI")]
        public string CVE_SEDE { get; set; }

        [JsonProperty("CAMPUSPA")]
        public string CAMPUS_INB { get; set; }

        [JsonProperty("TIPOCONTRATO")]
        public string TIPOCONTRATO { get; set; }

        [JsonProperty("CONTRATO")]
        public string CONTRATO { get; set; }

        [JsonProperty("ESCUELACODE")]
        public string ESCUELACODE { get; set; }

        [JsonProperty("ESCUELA")]
        public string ESCUELA { get; set; }

        [JsonProperty("NRC")]
        public string NRC { get; set; }

        [JsonProperty("MATERIA")]
        public string MATERIA { get; set; }

        [JsonProperty("CURSO")]
        public string CURSO { get; set; }

        [JsonProperty("NOMBREMATERIA")]
        public string NOMBREMATERIA { get; set; }

        [JsonProperty("FECHAINICIO")]
        public string FECHAINICIO { get; set; }

        [JsonProperty("FECHAFIN")]
        public string FECHAFIN { get; set; }

        [JsonProperty("TIPOCURSO")]
        public string TIPOCURSO { get; set; }

        [JsonProperty("METODOINSTRUCCION")]
        public string METODOINSTRUCCION { get; set; }

        [JsonProperty("STATUS")]
        public string STATUS { get; set; }

        [JsonProperty("INSCRITOS")]
        public string INSCRITOS { get; set; }

        [JsonProperty("PARTEPERIODO")]
        public string PARTEPERIODO { get; set; }

        [JsonProperty("PARTEPERIODODESC")]
        public string PARTEPERIODODESC { get; set; }

        [JsonProperty("APELLIDOS")]
        public string APELLIDOS { get; set; }

        [JsonProperty("NOMBRES")]
        public string NOMBRES { get; set; }

        [JsonProperty("RFC")]
        public string RFC { get; set; }

        [JsonProperty("CURP")]
        public string CURP { get; set; }

        [JsonProperty("TIPODOCENTECODE")]
        public string TIPODOCENTECODE { get; set; }

        [JsonProperty("TIPODOCENTE")]
        public string TIPODOCENTE { get; set; }

        [JsonProperty("MAXIMOGRADOACAD")]
        public string MAXIMOGRADOACAD { get; set; }

        [JsonProperty("HORASSEMANALES")]
        public string HORASSEMANALES { get; set; }

        [JsonProperty("HORASPROGRAMADAS")]
        public string HORASPROGRAMADAS { get; set; }

        [JsonProperty("PORCENTAJERESPON")]
        public string PORCENTAJERESPON { get; set; }

        [JsonProperty("HORASPAGAR")]
        public string HORASPAGAR { get; set; }

        [JsonProperty("LOGINADMIN")]
        public string LOGINADMIN { get; set; }

        [JsonProperty("TABULADOR")]
        public string TABULADOR { get; set; }

        [JsonProperty("INDICADORSESION")]
        public string INDICADORSESION { get; set; }

        [JsonProperty("PERIODO")]
        public string PERIODO { get; set; }

        [JsonProperty("ID")]
        public string IDSIU { get; set; }

        [JsonProperty("FECHANAC")]
        public string FECHANACIMIENTO { get; set; }

        [JsonProperty("SEXO")]
        public string SEXO { get; set; }

        [JsonProperty("CALLE")]
        public string DIRECCION_CALLE { get; set; }

        [JsonProperty("COLONIA")]
        public string DIRECCION_COLONIA { get; set; }

        [JsonProperty("MUNICIPIO")]
        public string DIRECCION_ENTIDAD { get; set; }

        [JsonProperty("ESTADO")]
        public string DIRECCION_ESTADO { get; set; }

        [JsonProperty("CIUDAD")]
        public string DIRECCION_CIUDAD { get; set; }

        [JsonProperty("PAIS")]
        public string DIRECCION_PAIS { get; set; }

        [JsonProperty("CP")]
        public string DIRECCION_CP { get; set; }

        [JsonProperty("TELEFONO")]
        public string TELEFONO { get; set; }

        [JsonProperty("EMAIL")]
        public string EMAIL { get; set; }

        [JsonProperty("NACIONALIDAD")]
        public string NACIONALIDAD { get; set; }

        [JsonProperty("TIPODEEMPLEADO")]
        public string TIPODEEMPLEADO { get; set; }

        [JsonProperty("TIPODEPAGO")]
        public string TIPODEPAGO { get; set; }

        [JsonProperty("TIPODEPAGOCODE")]
        public string TIPODEPAGOCODE { get; set; }

        [JsonProperty("AREAASIGNACION")]
        public string AREAASIGNACION { get; set; }

        [JsonProperty("NIVELESTUDIOS")]
        public string NIVELESTUDIOS { get; set; }

        [JsonProperty("CEDULA")]
        public string CEDULA { get; set; }

        [JsonProperty("FECCEDULA")]
        public string FECCEDULA { get; set; }

        [JsonProperty("ACTIVO")]
        public string ACTIVO { get; set; }

        [JsonProperty("TITULOPROFESIONALCODE")]
        public string TITULOPROFESIONALCODE { get; set; }

        [JsonProperty("TITULOPROFESIONAL")]
        public string TITULOPROFESIONAL { get; set; }

        [JsonProperty("PROFESIONCODE")]
        public string PROFESIONCODE { get; set; }

        [JsonProperty("PROFESION")]
        public string PROFESION { get; set; }

        [JsonProperty("CORREOANAHUAC")]
        public string CORREOANAHUAC { get; set; }

        [JsonProperty("SEGUROSOCIAL")]
        public string SEGUROSOCIAL { get; set; }

        public string IdPersona { get; set; }

        public string PeriodoX { get; set; }
        public string PartePeriodoX { get; set; }
        public string SedeX { get; set; }
        public string CampusX { get; set; }
        public string EscuelaX { get; set; }

        public string TipoPago { get; set; }
        public string CentroCostosID { get; set; }
        public string EsquemaID { get; set; }
        public string MontoPagar { get; set; }
        public string TipoPagoCVE { get; set; }
        public string nominaID { get; set; }

        public string IdSiuX { get; set; }
        public string NrcX { get; set; }
        public string MateriaX { get; set; }
        public string CursoX { get; set; }
        public string EstatusX { get; set; }
        public string TableX { get; set; }
        public string TipoActX { get; set; }
        public string CamposMX { get; set; }
        public string UsuarioX { get; set; }

        [JsonProperty("ATRIBUTO")]
        public string ATRIBUTO { get; set; }


        [JsonProperty("ATRIBUTOSPROGRAMAGRADO")]
        public string ATRIBUTOSPROGRAMAGRADO { get; set; }

        public string sql { get; set; }

        public bool existPAyPersona()
        {
            string sql = "SELECT COUNT(*) AS 'MAX' FROM PA_TMP PA, PERSONAS P WHERE PA.ID_PA = '" + PK_PA + "' AND  P.IDSIU=PA.IDSIU";
            int MAX = db.Count(sql);
            return MAX > 0;
        }

        public static int Consultar(ActualizacionPAModel[] models, SessionDB sesion)
        {
            int agregados = 0;
            List<string> listIDSIU = new List<string>();
            if (models.Length > 0)
            {
                bool TMP = true;
                foreach (ActualizacionPAModel model in models)
                {
                    if (model.IDSIU == "00260600")
                        model.IDSIU = model.IDSIU;
                    model.sesion = sesion;

                    IPersona persona = model as IPersona;
                    persona.findPersona();
                    model.PERSONA_REGISTRADA = model.PK_PERSONA != -1 ? "1" : "0";

                    if (listIDSIU.Contains(model.IDSIU) || persona.addPersona(TMP))
                    {
                        IPA pa = model as IPA;
                        pa.findPA();
                        model.PA_REGISTRADA = model.PK_PA != -1 ? "1" : "0";

                        if (pa.addPA(TMP))
                        {
                            agregados++;
                            listIDSIU.Add(model.IDSIU);
                            continue;
                        }
                    }
                    break;
                }
            }
            return agregados;
        }

        public bool Generar(string data, out int totalPA, out int registradosPA, out int registradosPersonas)
        {
            string[] arrChecked = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            totalPA = arrChecked.Length;
            registradosPA = 0;
            registradosPersonas = 0;
            bool all_result = true;

            bool TMP = true;
            int NOT_FOUND = -1;

            List<string> listIDSIU = new List<string>();
            foreach (string itemChecked in arrChecked)
            {
                long.TryParse(itemChecked, out PK_PA);

                bool ok;
                bool personaActualizada;

                IPersona persona = this as IPersona;
                IPA PA = this as IPA;

                if (PA.editPA(TMP))
                {
                    personaActualizada = listIDSIU.Contains(IDSIU);

                    if (personaActualizada || persona.editPersona(TMP))
                    {
                        if (personaActualizada)
                            ok = true;
                        else
                        {
                            if (persona.findPersona() == NOT_FOUND)
                                ok = persona.addPersona();
                            else
                                ok = persona.savePersona();
                            registradosPersonas++;
                        }

                        if (ok)
                        {
                            if (PA.findPA() == NOT_FOUND)
                                ok = PA.addPA();
                            else
                                ok = PA.savePA();

                            if (ok)
                            {
                                markPersona();
                                markPA();
                                registradosPA++;

                                if (personaActualizada == false)
                                    listIDSIU.Add(IDSIU);
                                continue;
                            }
                        }
                    }
                }
                all_result = false;
                break;
            }

            return all_result;
        }

        public bool GenerarTodo(string usuario = "")
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramUsuario = new Parametros();

            try
            {
                paramUsuario.nombreParametro = "@usuario";
                paramUsuario.longitudParametro = 25;
                paramUsuario.tipoParametro = SqlDbType.NVarChar;
                paramUsuario.direccion = ParameterDirection.Input;
                paramUsuario.value = usuario;
                lParamS.Add(paramUsuario);

                exito = db.ExecuteStoreProcedure("sp_actualizaPA_all", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ActualizaNomina()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramPeriodo = new Parametros();
            Parametros paramPartePeriodo = new Parametros();
            Parametros paramSede = new Parametros();
            Parametros paramCampus = new Parametros();

            try
            {
                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = PeriodoX;
                lParamS.Add(paramPeriodo);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.longitudParametro = 3;
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = PartePeriodoX;
                lParamS.Add(paramPartePeriodo);

                paramSede.nombreParametro = "@campusVPDI";
                paramSede.longitudParametro = 3;
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = SedeX;
                lParamS.Add(paramSede);

                paramCampus.nombreParametro = "@campusINB";
                paramCampus.longitudParametro = 3;
                paramCampus.tipoParametro = SqlDbType.NVarChar;
                paramCampus.direccion = ParameterDirection.Input;
                paramCampus.value = CampusX;
                lParamS.Add(paramCampus);

                exito = db.ExecuteStoreProcedure("sp_recalcula_nomina_xconciliacion", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EliminaPA(string usuario = "")
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramPeriodo = new Parametros();
            Parametros paramEscuela = new Parametros();
            Parametros paramSede = new Parametros();
            Parametros paramCampus = new Parametros();
            Parametros paramTipPago = new Parametros();
            Parametros paramPartePeriodo = new Parametros();
            Parametros paramUsusario = new Parametros();

            try
            {
                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = PeriodoX;
                lParamS.Add(paramPeriodo);

                paramEscuela.nombreParametro = "@escuela";
                paramEscuela.longitudParametro = 5;
                paramEscuela.tipoParametro = SqlDbType.NVarChar;
                paramEscuela.direccion = ParameterDirection.Input;
                paramEscuela.value = (EscuelaX == "" || EscuelaX == null) ? null : EscuelaX;
                lParamS.Add(paramEscuela);

                paramSede.nombreParametro = "@sede";
                paramSede.longitudParametro = 3;
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = SedeX;
                lParamS.Add(paramSede);

                paramCampus.nombreParametro = "@campus";
                paramCampus.longitudParametro = 3;
                paramCampus.tipoParametro = SqlDbType.NVarChar;
                paramCampus.direccion = ParameterDirection.Input;
                paramCampus.value = CampusX;
                lParamS.Add(paramCampus);

                paramTipPago.nombreParametro = "@opcionPago";
                paramTipPago.longitudParametro = 5;
                paramTipPago.tipoParametro = SqlDbType.NVarChar;
                paramTipPago.direccion = ParameterDirection.Input;
                paramTipPago.value = (TipoPago == "" || TipoPago == null) ? null : TipoPago;
                lParamS.Add(paramTipPago);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.longitudParametro = 5;
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = (PartePeriodoX == "" || PartePeriodoX == null) ? null : PartePeriodoX;
                lParamS.Add(paramPartePeriodo);

                paramUsusario.nombreParametro = "@usuario";
                paramUsusario.longitudParametro = 5;
                paramUsusario.tipoParametro = SqlDbType.NVarChar;
                paramUsusario.direccion = ParameterDirection.Input;
                paramUsusario.value = usuario;
                lParamS.Add(paramUsusario);

                exito = db.ExecuteStoreProcedure("sp_eliminaPAXConciliacion", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ReInsertaNomina()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramPeriodo = new Parametros();
            Parametros paramPartePeriodo = new Parametros();
            Parametros paramEscuela = new Parametros();
            Parametros paramTipPago = new Parametros();
            Parametros paramSede = new Parametros();
            Parametros paramCampus = new Parametros();

            try
            {
                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = PeriodoX;
                lParamS.Add(paramPeriodo);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.longitudParametro = 3;
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = (PartePeriodoX == "" || PartePeriodoX == null) ? null : PartePeriodoX;
                lParamS.Add(paramPartePeriodo);

                paramEscuela.nombreParametro = "@escuela";
                paramEscuela.longitudParametro = 5;
                paramEscuela.tipoParametro = SqlDbType.NVarChar;
                paramEscuela.direccion = ParameterDirection.Input;
                paramEscuela.value = (EscuelaX == "" || EscuelaX == null) ? null : EscuelaX;
                lParamS.Add(paramEscuela);

                paramTipPago.nombreParametro = "@opcionPago";
                paramTipPago.longitudParametro = 5;
                paramTipPago.tipoParametro = SqlDbType.NVarChar;
                paramTipPago.direccion = ParameterDirection.Input;
                paramTipPago.value = (TipoPago == "" || TipoPago == null) ? null : TipoPago;
                lParamS.Add(paramTipPago);
                
                paramSede.nombreParametro = "@sede";
                paramSede.longitudParametro = 3;
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = SedeX;
                lParamS.Add(paramSede);

                paramCampus.nombreParametro = "@campus";
                paramCampus.longitudParametro = 3;
                paramCampus.tipoParametro = SqlDbType.NVarChar;
                paramCampus.direccion = ParameterDirection.Input;
                paramCampus.value = CampusX;
                lParamS.Add(paramCampus);

                exito = db.ExecuteStoreProcedure("sp_reinserta_nomina", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ReInsertaEstadoCuenta()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramPeriodo = new Parametros();
            Parametros paramPartePeriodo = new Parametros();
            Parametros paramTipPago = new Parametros();
            Parametros paramSede = new Parametros();

            try
            {
                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = PeriodoX;
                lParamS.Add(paramPeriodo);

                paramPartePeriodo.nombreParametro = "@partePeriodo";
                paramPartePeriodo.longitudParametro = 3;
                paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPartePeriodo.direccion = ParameterDirection.Input;
                paramPartePeriodo.value = (PartePeriodoX == "" || PartePeriodoX == null) ? null : PartePeriodoX;
                lParamS.Add(paramPartePeriodo);

                paramTipPago.nombreParametro = "@tipoPagoCVE";
                paramTipPago.longitudParametro = 5;
                paramTipPago.tipoParametro = SqlDbType.NVarChar;
                paramTipPago.direccion = ParameterDirection.Input;
                paramTipPago.value = (TipoPago == "" || TipoPago == null) ? null : TipoPago;
                lParamS.Add(paramTipPago);

                paramSede.nombreParametro = "@campus";
                paramSede.longitudParametro = 3;
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = SedeX;
                lParamS.Add(paramSede);

                exito = db.ExecuteStoreProcedure("sp_reinserta_estadocuenta", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool calculaEstadocuentaXRegistroNomina(string usuario)
        {
            List<Parametros> lParamS;
            Parametros paramPeriodo;
            Parametros paramCampusVPDI;
            Parametros paramTipoPagoCVE;
            Parametros paramEsquemaID;
            Parametros paramMontoPagar;
            Parametros paramPersonaID;
            Parametros paramCentroCostoID;
            Parametros paramNominaID;
            Parametros paramUsuario;

            bool exito = false;

            try
            {
                sql = " select ID_NOMINA, ID_PERSONA, ID_CENTRODECOSTOS, ID_ESQUEMA, MONTOAPAGAR, CVE_TIPODEPAGO "
                    + "   from NOMINA                                                                            "
                    + "  where ACTUALIZADO     = 1                                                               "
                    + "    and PERIODO         = '" + PeriodoX + "'                                              "
                    + "    and CVE_SEDE        = '" + SedeX    + "'                                              ";

                if (TipoPago != "" && TipoPago != null) sql += " and CVE_TIPODEPAGO = '" + TipoPago + "' ";

                ResultSet res = db.getTable(sql);

                while (res.Next())
                {
                    IdPersona = res.Get("ID_PERSONA");
                    CentroCostosID = res.Get("ID_CENTRODECOSTOS");
                    EsquemaID = res.Get("ID_ESQUEMA");
                    MontoPagar = res.Get("MONTOAPAGAR");
                    TipoPagoCVE = res.Get("CVE_TIPODEPAGO");
                    nominaID = res.Get("ID_NOMINA");

                    lParamS = new List<Parametros>();

                    paramPeriodo = new Parametros();
                    paramPeriodo.nombreParametro = "@periodo";
                    paramPeriodo.longitudParametro = 10;
                    paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                    paramPeriodo.direccion = ParameterDirection.Input;
                    paramPeriodo.value = PeriodoX;
                    lParamS.Add(paramPeriodo);

                    paramCampusVPDI = new Parametros();
                    paramCampusVPDI.nombreParametro = "@campusVPDI";
                    paramCampusVPDI.longitudParametro = 3;
                    paramCampusVPDI.tipoParametro = SqlDbType.NVarChar;
                    paramCampusVPDI.direccion = ParameterDirection.Input;
                    paramCampusVPDI.value = SedeX;
                    lParamS.Add(paramCampusVPDI);

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
                    paramPersonaID.value = IdPersona;
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

                    paramUsuario = new Parametros();
                    paramUsuario.nombreParametro = "@usuario";
                    paramUsuario.longitudParametro = 180;
                    paramUsuario.tipoParametro = SqlDbType.NVarChar;
                    paramUsuario.direccion = ParameterDirection.Input;
                    paramUsuario.value = usuario;
                    lParamS.Add(paramUsuario);

                    exito = db.ExecuteStoreProcedure("sp_recalcula_estadocuentaXConciliacion", lParamS);
                    if (exito == false)
                        return false;
                }
                return true;
            }
            catch (Exception E)
            {
                return false;
            }
        }

        public bool insertaPAExcelTMP()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramIdSIU = new Parametros();
            Parametros paramNrc = new Parametros();
            Parametros paramMateria = new Parametros();
            Parametros paramCurso = new Parametros();
            Parametros paramEstatus = new Parametros();
            Parametros paramTable = new Parametros();
            Parametros paramTipoAct = new Parametros();
            Parametros paramCamposM = new Parametros();
            Parametros paramUsuario = new Parametros();

            try
            {
                paramIdSIU.nombreParametro = "@IdSIU";
                paramIdSIU.longitudParametro = 10;
                paramIdSIU.tipoParametro = SqlDbType.NVarChar;
                paramIdSIU.direccion = ParameterDirection.Input;
                paramIdSIU.value = IdSiuX;
                lParamS.Add(paramIdSIU);

                paramNrc.nombreParametro = "@nrc";
                paramNrc.tipoParametro = SqlDbType.Int;
                paramNrc.direccion = ParameterDirection.Input;
                paramNrc.value = NrcX;
                lParamS.Add(paramNrc);

                paramMateria.nombreParametro = "@materia";
                paramMateria.longitudParametro = 15;
                paramMateria.tipoParametro = SqlDbType.NVarChar;
                paramMateria.direccion = ParameterDirection.Input;
                paramMateria.value = MateriaX;
                lParamS.Add(paramMateria);

                paramCurso.nombreParametro = "@curso";
                paramCurso.longitudParametro = 10;
                paramCurso.tipoParametro = SqlDbType.NVarChar;
                paramCurso.direccion = ParameterDirection.Input;
                paramCurso.value = CursoX;
                lParamS.Add(paramCurso);

                paramEstatus.nombreParametro = "@estatus";
                paramEstatus.longitudParametro = 50;
                paramEstatus.tipoParametro = SqlDbType.NVarChar;
                paramEstatus.direccion = ParameterDirection.Input;
                paramEstatus.value = EstatusX;
                lParamS.Add(paramEstatus);

                paramTable.nombreParametro = "@table";
                paramTable.longitudParametro = 6;
                paramTable.tipoParametro = SqlDbType.NVarChar;
                paramTable.direccion = ParameterDirection.Input;
                paramTable.value = TableX;
                lParamS.Add(paramTable);

                paramTipoAct.nombreParametro = "@tipoAct";
                paramTipoAct.tipoParametro = SqlDbType.Int;
                paramTipoAct.direccion = ParameterDirection.Input;
                paramTipoAct.value = TipoActX;
                lParamS.Add(paramTipoAct);

                paramCamposM.nombreParametro = "@camposM";
                paramCamposM.longitudParametro = 50;
                paramCamposM.tipoParametro = SqlDbType.NVarChar;
                paramCamposM.direccion = ParameterDirection.Input;
                paramCamposM.value = CamposMX;
                lParamS.Add(paramCamposM);

                paramUsuario.nombreParametro = "@usuario";
                paramUsuario.longitudParametro = 180;
                paramUsuario.tipoParametro = SqlDbType.NVarChar;
                paramUsuario.direccion = ParameterDirection.Input;
                paramUsuario.value = UsuarioX;
                lParamS.Add(paramUsuario);

                exito = db.ExecuteStoreProcedure("sp_paExcelTmp_inserta", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool cleanPAExcelTMP(SessionDB sesion)
        {
            sql = "delete from PAEXCEL_TMP where USUARIO = '" + sesion.pkUser.ToString() + "'";
            return db.execute(sql);
        }

        public string string_real(string str)
        {
            if (str == null)
                return "";
            return str.Replace(",", ".");
        }

        public Dictionary<string, string> getCampusPA()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_SEDE,SEDE FROM QSedesPorUsuario01 ORDER BY CVE_SEDE ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_SEDE"), res.Get("SEDE"));

            return dict;
        }

        public bool existExcel(bool TMP, string param_nrc, string param_periodo, string param_idsiu, string param_campusPA)
        {
            string TABLE = TMP ? "PA_TMP" : "PA";
            string sql = "SELECT COUNT(*) AS 'MAX' FROM " + TABLE + " WHERE NRC = '" + param_nrc + "' AND PERIODO = '" + param_periodo + "' AND IDSIU = '" + param_idsiu + "' AND  CAMPUS_INB = '" + param_campusPA + "'";
            int MAX = db.Count(sql);
            return MAX > 0;
        }

        public override string ToString()
        {
            return "(PAModel) IDSIU:" + IDSIU + ", Nombre:" + NOMBRES;
        }
    }
}
