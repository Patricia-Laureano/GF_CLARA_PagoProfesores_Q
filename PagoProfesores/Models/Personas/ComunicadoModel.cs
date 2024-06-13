using ConnectDB;
using System;
using System.Collections.Generic;
using System.Data;

namespace PagoProfesores.Models.Personas
{
    public class ComunicadoModel : SuperModel
    {
        public string PK1 { get; set; }
        public string CveSede { get; set; }
        public string Ciclo { get; set; }
        public string Periodo { get; set; }
        public string PersonaId { get; set; }
        public string IdSIU { get; set; }
        public string Profesor { get; set; }
        public string EsquemaId { get; set; }
        public string ConceptoPagoPk { get; set; }
        public string FechaPago { get; set; }
        public string Mensaje { get; set; }
        public string Usuario { get; set; }

        public bool AgregarComunicado()
        {
            try
            {
                // En un archivo más actual todo el bloque siguiente de código se comentó.
                bool exito = false;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramCveSede = new Parametros();
                Parametros paramPeriodo = new Parametros();
                Parametros paramPersonaId = new Parametros();
                Parametros paramEsquemaId = new Parametros();
                Parametros paramConceptoPagoPk = new Parametros();
                Parametros paramFechaPago = new Parametros();
                Parametros paramMensaje = new Parametros();
                Parametros paramUsuario = new Parametros();

                paramCveSede.nombreParametro = "@cveSede";
                paramCveSede.tipoParametro = SqlDbType.NVarChar;
                paramCveSede.longitudParametro = 5;
                paramCveSede.direccion = ParameterDirection.Input;
                paramCveSede.value = CveSede;
                lParamS.Add(paramCveSede);

                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramPersonaId.nombreParametro = "@idPersona";
                paramPersonaId.tipoParametro = SqlDbType.BigInt;
                paramPersonaId.direccion = ParameterDirection.Input;
                paramPersonaId.value = (PersonaId == null || PersonaId == "" ? "0" : PersonaId);
                lParamS.Add(paramPersonaId);

                paramEsquemaId.nombreParametro = "@idEsquema";
                paramEsquemaId.tipoParametro = SqlDbType.BigInt;
                paramEsquemaId.direccion = ParameterDirection.Input;
                paramEsquemaId.value = EsquemaId;
                lParamS.Add(paramEsquemaId);

                paramConceptoPagoPk.nombreParametro = "@pkConceptoPago";
                paramConceptoPagoPk.tipoParametro = SqlDbType.Int;
                paramConceptoPagoPk.direccion = ParameterDirection.Input;
                paramConceptoPagoPk.value = ConceptoPagoPk;
                lParamS.Add(paramConceptoPagoPk);

                //paramFechaPago.nombreParametro = "@fechaPago";
                //paramFechaPago.tipoParametro = SqlDbType.DateTime;
                //paramFechaPago.direccion = ParameterDirection.Input;
                //paramFechaPago.value = FechaPago;
                //lParamS.Add(paramFechaPago);

                paramMensaje.nombreParametro = "@mensaje";
                paramMensaje.tipoParametro = SqlDbType.NVarChar;
                paramMensaje.longitudParametro = 4000;
                paramMensaje.direccion = ParameterDirection.Input;
                paramMensaje.value = Mensaje;
                lParamS.Add(paramMensaje);

                paramUsuario.nombreParametro = "@usuario";
                paramUsuario.tipoParametro = SqlDbType.NVarChar;
                paramUsuario.longitudParametro = 180;
                paramUsuario.direccion = ParameterDirection.Input;
                paramUsuario.value = sesion.pkUser.ToString();
                lParamS.Add(paramUsuario);

                exito = db.ExecuteStoreProcedure("sp_comunicado_inserta", lParamS);

                return exito;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool ActualizarComunicado()
        {
            try
            {
                // En un archivo más actual todo el bloque siguiente de código se comentó.
                bool exito = false;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramPk1 = new Parametros();
                Parametros paramCveSede = new Parametros();
                Parametros paramPeriodo = new Parametros();
                Parametros paramPersonaId = new Parametros();
                Parametros paramEsquemaId = new Parametros();
                Parametros paramConceptoPagoPk = new Parametros();
                Parametros paramFechaPago = new Parametros();
                Parametros paramMensaje = new Parametros();
                Parametros paramUsuario = new Parametros();

                paramPk1.nombreParametro = "@pk1";
                paramPk1.tipoParametro = SqlDbType.BigInt;
                paramPk1.direccion = ParameterDirection.Input;
                paramPk1.value = PK1;
                lParamS.Add(paramPk1);

                paramCveSede.nombreParametro = "@cveSede";
                paramCveSede.tipoParametro = SqlDbType.NVarChar;
                paramCveSede.longitudParametro = 5;
                paramCveSede.direccion = ParameterDirection.Input;
                paramCveSede.value = CveSede;
                lParamS.Add(paramCveSede);

                paramPeriodo.nombreParametro = "@periodo";
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = Periodo;
                lParamS.Add(paramPeriodo);

                paramPersonaId.nombreParametro = "@idPersona";
                paramPersonaId.tipoParametro = SqlDbType.BigInt;
                paramPersonaId.direccion = ParameterDirection.Input;
                paramPersonaId.value = PersonaId;
                lParamS.Add(paramPersonaId);

                paramEsquemaId.nombreParametro = "@idEsquema";
                paramEsquemaId.tipoParametro = SqlDbType.BigInt;
                paramEsquemaId.direccion = ParameterDirection.Input;
                paramEsquemaId.value = EsquemaId;
                lParamS.Add(paramEsquemaId);

                paramConceptoPagoPk.nombreParametro = "@pkConceptoPago";
                paramConceptoPagoPk.tipoParametro = SqlDbType.Int;
                paramConceptoPagoPk.direccion = ParameterDirection.Input;
                paramConceptoPagoPk.value = ConceptoPagoPk;
                lParamS.Add(paramConceptoPagoPk);

                //paramFechaPago.nombreParametro = "@fechaPago";
                //paramFechaPago.tipoParametro = SqlDbType.DateTime;
                //paramFechaPago.direccion = ParameterDirection.Input;
                //paramFechaPago.value = FechaPago;
                //lParamS.Add(paramFechaPago);

                paramMensaje.nombreParametro = "@mensaje";
                paramMensaje.tipoParametro = SqlDbType.NVarChar;
                paramMensaje.longitudParametro = 4000;
                paramMensaje.direccion = ParameterDirection.Input;
                paramMensaje.value = Mensaje;
                lParamS.Add(paramMensaje);

                paramUsuario.nombreParametro = "@usuario";
                paramUsuario.tipoParametro = SqlDbType.NVarChar;
                paramUsuario.longitudParametro = 180;
                paramUsuario.direccion = ParameterDirection.Input;
                paramUsuario.value = sesion.pkUser.ToString();
                lParamS.Add(paramUsuario);

                exito = db.ExecuteStoreProcedure("sp_comunicado_actualiza", lParamS);

                return exito;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public bool EliminarComunicado()
        {
            try
            {
                bool exito = false;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramPk1 = new Parametros();

                paramPk1.nombreParametro = "@pk1";
                paramPk1.tipoParametro = SqlDbType.BigInt;
                paramPk1.direccion = ParameterDirection.Input;
                paramPk1.value = PK1;
                lParamS.Add(paramPk1);

                exito = db.ExecuteStoreProcedure("sp_comunicado_elimina", lParamS);

                return exito;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        public DataTable ConsultaComunicados()
        {
            DataTable dtComunicados = new DataTable();

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramCveSede = new Parametros();
            Parametros paramPersonaId = new Parametros();

            try
            {
                paramCveSede.nombreParametro = "@cveSede";
                paramCveSede.tipoParametro = SqlDbType.NVarChar;
                paramCveSede.longitudParametro = 5;
                paramCveSede.direccion = ParameterDirection.Input;
                paramCveSede.value = CveSede;
                lParamS.Add(paramCveSede);

                paramPersonaId.nombreParametro = "@idPersona";
                paramPersonaId.tipoParametro = SqlDbType.BigInt;
                paramPersonaId.direccion = ParameterDirection.Input;
                paramPersonaId.value = PersonaId;
                lParamS.Add(paramPersonaId);

                dtComunicados = db.SelectDataTableFromStoreProcedure("sp_comunicados_consulta", lParamS);

            }
            catch
            {
                return null;
            }

            return dtComunicados;
        }

        public bool ConsultaComunicado()
        {
            DataTable dtComunicados = new DataTable();

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramPK1 = new Parametros();

            try
            {
                paramPK1.nombreParametro = "@pk1";
                paramPK1.tipoParametro = SqlDbType.BigInt;
                paramPK1.direccion = ParameterDirection.Input;
                paramPK1.value = PK1;
                lParamS.Add(paramPK1);

                dtComunicados = db.SelectDataTableFromStoreProcedure("sp_comunicado_consulta", lParamS);

                if (dtComunicados.Rows.Count > 0)
                    foreach(DataRow dr in dtComunicados.Rows)
                    {
                        CveSede = dr["CVE_SEDE"].ToString();
                        Ciclo = dr["CVE_CICLO"].ToString();
                        Periodo = dr["PERIODO"].ToString();
                        PersonaId = dr["ID_PERSONA"].ToString();
                        IdSIU = dr["IDSIU"].ToString();
                        Profesor = dr["PROFESOR"].ToString();
                        EsquemaId = dr["ID_ESQUEMA"].ToString();
                        ConceptoPagoPk = dr["PKCONCEPTOPAGO"].ToString();
                        FechaPago = dr["FECHADEPAGO"].ToString();
                        Mensaje = dr["MENSAJE"].ToString();
                    }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}