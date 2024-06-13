using System;
using ConnectDB;
using Newtonsoft.Json;
using Session;
using System.Collections.Generic;
using System.Data;

namespace PagoProfesores.Models.Personas
{
    public class ImportarDatosSIUModel : SuperModel
    {
        [JsonProperty("CAMPUS")]
        public string CAMPUS { get; set; }
        [JsonProperty("PERIODO")]
        public string PERIODO { get; set; }
        [JsonProperty("IDSIU")]
        public string IDSIU { get; set; }
        [JsonProperty("LOGINADMIN")]
        public string LOGINADMIN { get; set; }
        [JsonProperty("APELLIDOS")]
        public string APELLIDOS { get; set; }
        [JsonProperty("NOMBRES")]
        public string NOMBRES { get; set; }
        [JsonProperty("FECHANAC")]
        public string FECHANACIMIENTO { get; set; }
        [JsonProperty("SEXO")]
        public string SEXO { get; set; }
        [JsonProperty("RFC")]
        public string RFC { get; set; }
        [JsonProperty("CURP")]
        public string CURP { get; set; }
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
        public string CORREO { get; set; }
        [JsonProperty("NACIONALIDAD")]
        public string NACIONALIDAD { get; set; }
        [JsonProperty("TIPODEEMPLEADO")]
        public string TIPODEEMPLEADO { get; set; }
        [JsonProperty("TIPODEPAGO")]
        public string CVE_TIPODEPAGO { get; set; }
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
        [JsonProperty("PROFECIONCODE")]
        public string PROFESIONCODE { get; set; }
        [JsonProperty("PROFESION")]
        public string PROFESION { get; set; }
        [JsonProperty("CORREOANAHUAC")]
        public string CORREOANAHUAC { get; set; }
        [JsonProperty("SEGUROSOCIAL")]
        public string SEGUROSOCIAL { get; set; }
        [JsonProperty("CORREOO365")]
        public string CORREO365 { get; set; }
        [JsonProperty("TITULO_LICENCIATURA")]
        public string TITULO_LICENCIATURA { get; set; }
        [JsonProperty("TITULO_MAESTRIA")]
        public string TITULO_MAESTRIA { get; set; }
        [JsonProperty("TITULO_DOCTORADO")]
        public string TITULO_DOCTORADO { get; set; }

        public string REGISTRADO { get; set; }
        public string IMPORTADO { get; set; }
        public string CVESEDE { get; set; }
        public string _msg { get; set; }

        public string ID_PERSONA;

        [JsonProperty("FI_DIRECCION_CP")]
        public string FI_DIRECCION_CP { get; set; }
        [JsonProperty("FI_DIRECCION_PAIS")]
        public string FI_DIRECCION_PAIS { get; set; }
        [JsonProperty("FI_DIRECCION_ESTADO")]
        public string FI_DIRECCION_ESTADO { get; set; }
        [JsonProperty("FI_DIRECCION_CIUDAD")]
        public string FI_DIRECCION_CIUDAD { get; set; }
        [JsonProperty("FI_DIRECCION_ENTIDAD")]
        public string FI_DIRECCION_ENTIDAD { get; set; }
        [JsonProperty("FI_DIRECCION_COLONIA")]
        public string FI_DIRECCION_COLONIA { get; set; }
        [JsonProperty("FI_DIRECCION_CALLE")]
        public string FI_DIRECCION_CALLE { get; set; }

        public string sql { get; set; }

        public bool TMP = false;
        public string TABLE = "PERSONAS";

        public bool Clean()
        {
            string sql = "DELETE FROM PERSONAS_TMP WHERE USUARIO = '" + sesion.pkUser + "'";
            return db.execute(sql);
        }

        public bool exist()
        {
            TABLE = TMP ? "QPersonasTMP" : "PERSONAS";

            string sql = "SELECT TOP 1 P.ID_PERSONA    " +
                         "  FROM " + TABLE + " P INNER JOIN " +
                         "       PERSONAS_SEDES S ON S.ID_PERSONA = P.ID_PERSONA " +
                         " WHERE IDSIU            = '" + IDSIU + "' " +
                         "   AND S.CVE_SEDE       = '" + CVESEDE + "' " + (TMP ? " and U.USUARIO = '" + sesion.pkUser + "' " : "");

            ResultSet res = db.getTable(sql);

            if (res.Next())
            {
                ID_PERSONA = res.Get("ID_PERSONA");
                return true;
            }
            else
            {
                ID_PERSONA = "-1";
                return false;
            }
        }

        public string __validDateTime(string str)
        {
            DateTime dt;
            if (str != null)
            {
                try
                {
                    string[] array = str.Split(new char[] { '/', ' ' });
                    dt = new DateTime(int.Parse(array[2]), int.Parse(array[1]), int.Parse(array[0]), 0, 0, 0, 0);
                }
                catch (Exception ex) { return null;/*dt = new DateTime(2000, 1, 1, 0, 0, 0, 0);*/ }
                return dt.ToString("yyyy-MM-dd");
            }
            else
            {
                return str;
            }

        }

        public string valid(string str)
        {
            if (str == null)
                return "";
            return str.Replace("'", "''");
        }

        public bool addTmp()
        {

            bool result = false;

            string[] VALUES = {
                ID_PERSONA
                ,IDSIU
                ,valid(APELLIDOS)
                ,valid(NOMBRES)
                ,SEXO
				//,__validDateTime(FECHANACIMIENTO)
				,NACIONALIDAD
                ,CORREO
                ,TELEFONO
                ,CVE_TIPODEPAGO
                ,RFC
                ,CURP
                ,valid(DIRECCION_PAIS)
                ,valid(DIRECCION_ESTADO)
                ,valid(DIRECCION_CIUDAD)
                ,valid(DIRECCION_ENTIDAD)
                ,valid(DIRECCION_COLONIA).Replace(",","")
                ,valid(DIRECCION_CALLE).Replace(",","")
                ,DIRECCION_CP
                ,TMP?sesion.pkUser.ToString():sesion.nickName
                ,REGISTRADO
                ,IMPORTADO
                ,TITULOPROFESIONALCODE
                ,valid(TITULOPROFESIONAL)
                ,PROFESIONCODE
                ,valid(PROFESION)
                ,CEDULA
                //,__validDateTime(FECCEDULA)
                ,SEGUROSOCIAL
                ,CORREO365
                ,TITULO_LICENCIATURA
                ,TITULO_MAESTRIA
                ,AREAASIGNACION
                ,TITULO_DOCTORADO
                ,valid(FI_DIRECCION_PAIS)
                ,valid(FI_DIRECCION_ESTADO)
                ,valid(FI_DIRECCION_CIUDAD)
                ,valid(FI_DIRECCION_ENTIDAD)
                ,valid(FI_DIRECCION_COLONIA).Replace(",","")
                ,valid(FI_DIRECCION_CALLE).Replace(",","")
                ,FI_DIRECCION_CP
            };

            string sql = "INSERT INTO PERSONAS_TMP" +
                           " (ID_PERSONA,IDSIU,APELLIDOS,NOMBRES,SEXO,NACIONALIDAD,CORREO,TELEFONO,CVE_TIPODEPAGO,RFC,CURP,DIRECCION_PAIS,DIRECCION_ESTADO,DIRECCION_CIUDAD,DIRECCION_ENTIDAD,DIRECCION_COLONIA,DIRECCION_CALLE,DIRECCION_CP,USUARIO,REGISTRADO,IMPORTADO,CVE_TITULOPROFESIONAL,TITULOPROFESIONAL,CVE_PROFESION,PROFESION,CEDULAPROFESIONAL,SEGUROSOCIAL,CORREO365,TITULO_LICENCIATURA,TITULO_MAESTRIA,AREAASIGNACION,TITULO_DOCTORADO" +
                           ",FI_DIRECCION_PAIS,FI_DIRECCION_ESTADO,FI_DIRECCION_CIUDAD,FI_DIRECCION_ENTIDAD,FI_DIRECCION_COLONIA,FI_DIRECCION_CALLE,FI_DIRECCION_CP" +
                           ",FECHACEDULA,FECHANACIMIENTO)" +
                           " VALUES ('" + string.Join("','", VALUES) + "'," + ((__validDateTime(FECCEDULA) == null || __validDateTime(FECCEDULA) == "") ? "null" : ("'" + __validDateTime(FECCEDULA)) + "'") + "," + ((__validDateTime(FECHANACIMIENTO) == null || __validDateTime(FECHANACIMIENTO) == "") ? "null" : ("'" + __validDateTime(FECHANACIMIENTO)) + "'") + ")";



            result = db.execute(sql);

            if (result == false)
            {
                Log.write(this, "addTmp", LOG.REGISTRO, (result ? "OK, " : "Error, ") + sql, this.sesion);
                InsertaDetallesConflicto("PERSONAS_TMP", IDSIU, NOMBRES + " " + APELLIDOS, sql, "", sesion.pkUser.ToString(), sesion.nickName);
                Console.WriteLine("Este IDSIU es el del problema: " + IDSIU + " (revisar)");
            }
            else
                Log.write(this, "addTmp", LOG.REGISTRO, (result ? "OK, " : "--, ") + sql, this.sesion);



            return result;
        }

        public bool add()
        {
            if (CVESEDE != "" || CVESEDE != null)
            {
                string[] VALUES = {
                IDSIU
                ,valid(APELLIDOS)
                ,valid(NOMBRES)
                ,SEXO
				//,__validDateTime(FECHANACIMIENTO)
				,NACIONALIDAD
                ,CORREO
                ,TELEFONO
                ,CVE_TIPODEPAGO
                ,RFC
                ,CURP
                ,valid(DIRECCION_PAIS)
                ,valid(DIRECCION_ESTADO)
                ,valid(DIRECCION_CIUDAD)
                ,valid(DIRECCION_ENTIDAD)
                ,valid(DIRECCION_COLONIA)
                ,valid(DIRECCION_CALLE)
                ,DIRECCION_CP
                ,"B"
                ,TMP?sesion.pkUser.ToString():sesion.nickName
                ,TITULOPROFESIONALCODE
                ,valid(TITULOPROFESIONAL)
                ,PROFESIONCODE
                ,valid(PROFESION)
                ,CEDULA
                //,__validDateTime(FECCEDULA)
                ,SEGUROSOCIAL
                ,CORREO365
                ,TITULO_LICENCIATURA
                ,TITULO_MAESTRIA
                ,AREAASIGNACION
                ,TITULO_DOCTORADO
                 ,valid(FI_DIRECCION_PAIS)
                ,valid(FI_DIRECCION_ESTADO)
                ,valid(FI_DIRECCION_CIUDAD)
                ,valid(FI_DIRECCION_ENTIDAD)
                ,valid(FI_DIRECCION_COLONIA).Replace(",","")
                ,valid(FI_DIRECCION_CALLE).Replace(",","")
                ,FI_DIRECCION_CP
                };

                string sql = "INSERT INTO PERSONAS" +
                    " (IDSIU,APELLIDOS,NOMBRES,SEXO,NACIONALIDAD,CORREO,TELEFONO,CVE_TIPODEPAGO,RFC,CURP,DIRECCION_PAIS,DIRECCION_ESTADO,DIRECCION_CIUDAD,DIRECCION_ENTIDAD,DIRECCION_COLONIA,DIRECCION_CALLE,DIRECCION_CP,CVE_ORIGEN,USUARIO,CVE_TITULOPROFESIONAL,TITULOPROFESIONAL,CVE_PROFESION,PROFESION,CEDULAPROFESIONAL,SEGUROSOCIAL,CORREO365,TITULO_LICENCIATURA,TITULO_MAESTRIA,AREAASIGNACION,TITULO_DOCTORADO" +
                " ,FI_DIRECCION_PAIS,FI_DIRECCION_ESTADO,FI_DIRECCION_CIUDAD,FI_DIRECCION_ENTIDAD,FI_DIRECCION_COLONIA,FI_DIRECCION_CALLE,FI_DIRECCION_CP" +
                 ",FECHA_R,FECHACEDULA,FECHANACIMIENTO)" +
                    " VALUES ('" + string.Join("','", VALUES) + "', GETDATE(), " + (__validDateTime(FECCEDULA) == null || __validDateTime(FECCEDULA) == "" ? "null" : __validDateTime(FECCEDULA)) + "," + (__validDateTime(FECHANACIMIENTO) == null || __validDateTime(FECHANACIMIENTO) == "" ? "null" : __validDateTime(FECHANACIMIENTO)) + ")";

                Log.write(this, "PERSONAS", LOG.REGISTRO, ("sql: ") + sql, this.sesion);

                ID_PERSONA = db.executeId(sql);

                if (ID_PERSONA != null && ID_PERSONA != string.Empty)
                {
                    sql = "INSERT INTO PERSONAS_SEDES (ID_PERSONA, CVE_SEDE, CVE_TIPODEPAGO, FECHA_R, USUARIO) " +
                          "                    VALUES (" + ID_PERSONA + ", '" + CVESEDE + "', '" + CVE_TIPODEPAGO + "', GETDATE(), '" + sesion.pkUser.ToString() + "')";

                    Log.write(this, "PERSONAS_SEDES", LOG.REGISTRO, ("sql: ") + sql, this.sesion);
                    db.execute(sql);
                    try
                    {
                        sql = "SELECT ID_PERSONA FROM PERSONAS_SEDES WHERE CVE_SEDE=''";
                        ResultSet resq = db.getTable(sql);
                        while (resq.Next())
                        {
                            depurarDocentesSinSede(resq.GetLong("ID_PERSONA"));
                        }
                        db.execute(sql);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {

                return false;
            }

        }

        public bool edit()
        {
            TABLE = TMP ? "QPersonasTMP" : "PERSONAS";
            string sql = "SELECT TOP 1 * FROM " + TABLE + " WHERE IDSIU = '" + IDSIU + "'";
            if (TMP == true)
            {
                sql += " and USUARIO = '" + sesion.pkUser.ToString() + "'";
            }
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                APELLIDOS = res.Get("APELLIDOS");
                NOMBRES = res.Get("NOMBRES");
                SEXO = res.Get("SEXO");
                FECHANACIMIENTO = res.Get("FECHANACIMIENTO");
                NACIONALIDAD = res.Get("NACIONALIDAD");
                CORREO = res.Get("CORREO");
                TELEFONO = res.Get("TELEFONO");
                CVE_TIPODEPAGO = res.Get("CVE_TIPODEPAGO");
                RFC = res.Get("RFC");
                CURP = res.Get("CURP");
                DIRECCION_PAIS = res.Get("DIRECCION_PAIS");
                DIRECCION_ESTADO = res.Get("DIRECCION_ESTADO");
                DIRECCION_CIUDAD = res.Get("DIRECCION_CIUDAD");
                DIRECCION_ENTIDAD = res.Get("DIRECCION_ENTIDAD");
                DIRECCION_COLONIA = res.Get("DIRECCION_COLONIA");
                DIRECCION_CALLE = res.Get("DIRECCION_CALLE");
                DIRECCION_CP = res.Get("DIRECCION_CP");
                TITULOPROFESIONALCODE = res.Get("CVE_TITULOPROFESIONAL");
                TITULOPROFESIONAL = res.Get("TITULOPROFESIONAL");
                PROFESIONCODE = res.Get("CVE_PROFESION");
                PROFESION = res.Get("PROFESION");
                CEDULA = res.Get("CEDULAPROFESIONAL");
                FECCEDULA = res.Get("FECHACEDULA");
                SEGUROSOCIAL = res.Get("SEGUROSOCIAL");
                CORREO365 = res.Get("CORREO365");
                TITULO_LICENCIATURA = res.Get("TITULO_LICENCIATURA");
                TITULO_MAESTRIA = res.Get("TITULO_MAESTRIA");
                AREAASIGNACION = res.Get("AREAASIGNACION");
                TITULO_DOCTORADO = res.Get("TITULO_DOCTORADO");

                FI_DIRECCION_PAIS = res.Get("FI_DIRECCION_PAIS");
                FI_DIRECCION_ESTADO = res.Get("FI_DIRECCION_ESTADO");
                FI_DIRECCION_CIUDAD = res.Get("FI_DIRECCION_CIUDAD");
                FI_DIRECCION_ENTIDAD = res.Get("FI_DIRECCION_ENTIDAD");
                FI_DIRECCION_COLONIA = res.Get("FI_DIRECCION_COLONIA");
                FI_DIRECCION_CALLE = res.Get("FI_DIRECCION_CALLE");
                FI_DIRECCION_CP = res.Get("FI_DIRECCION_CP");
                return true;
            }
            return false;
        }

        public bool save()
        {
            ResultSet res;
            string personaID = string.Empty;

            try
            {

                if (CVESEDE != "" || CVESEDE != null)
                {
                    TABLE = TMP ? "PERSONAS_TMP" : "PERSONAS";
                    string sql = "UPDATE " + TABLE + " SET " +
                        " APELLIDOS             = '" + valid(APELLIDOS) + "'" +
                        ",NOMBRES               = '" + valid(NOMBRES) + "'" +
                        ",SEXO                  = '" + SEXO + "'" +
                        ",FECHANACIMIENTO       = " + (__validDateTime(FECHANACIMIENTO) == null || __validDateTime(FECHANACIMIENTO) == "" ? "null" : ("'" + __validDateTime(FECHANACIMIENTO)) + "'") +
                        ",NACIONALIDAD          = '" + NACIONALIDAD + "'" +
                        ",CORREO                = '" + CORREO + "'" +
                        ",TELEFONO              = '" + TELEFONO + "'" +
                        ",RFC                   = '" + RFC + "'" +
                        ",CURP                  = '" + CURP + "'" +
                        ",DIRECCION_PAIS        = '" + valid(DIRECCION_PAIS) + "'" +
                        ",DIRECCION_ESTADO      = '" + valid(DIRECCION_ESTADO) + "'" +
                        ",DIRECCION_CIUDAD      = '" + valid(DIRECCION_CIUDAD) + "'" +
                        ",DIRECCION_ENTIDAD     = '" + valid(DIRECCION_ENTIDAD) + "'" +
                        ",DIRECCION_COLONIA     = '" + valid(DIRECCION_COLONIA) + "'" +
                        ",DIRECCION_CALLE       = '" + valid(DIRECCION_CALLE) + "'" +
                        ",DIRECCION_CP          = '" + DIRECCION_CP + "'" +
                        ",CVE_TITULOPROFESIONAL = '" + TITULOPROFESIONALCODE + "'" +
                        ",TITULOPROFESIONAL     = '" + valid(TITULOPROFESIONAL) + "'" +
                        ",CVE_PROFESION         = '" + PROFESIONCODE + "'" +
                        ",PROFESION             = '" + valid(PROFESION) + "'" +
                        ",CEDULAPROFESIONAL     = '" + CEDULA + "'" +
                        ",FECHACEDULA           = " + (__validDateTime(FECCEDULA) == null || __validDateTime(FECCEDULA) == "" ? "null" : ("'" + __validDateTime(FECCEDULA)) + "'") +
                        ",SEGUROSOCIAL          = '" + SEGUROSOCIAL + "'" +
                        ",CORREO365             = '" + CORREO365 + "'" +
                        ",TITULO_LICENCIATURA   = '" + TITULO_LICENCIATURA + "'" +
                        ",TITULO_MAESTRIA       = '" + TITULO_MAESTRIA + "'" +
                        ",TITULO_DOCTORADO      = '" + TITULO_DOCTORADO + "'" +
                        ",AREAASIGNACION        = '" + AREAASIGNACION + "'" +
                        ",FECHA_M               = GETDATE() " +
                        ",FI_DIRECCION_PAIS        = '" + valid(FI_DIRECCION_PAIS) + "'" +
                        ",FI_DIRECCION_ESTADO      = '" + valid(FI_DIRECCION_ESTADO) + "'" +
                        ",FI_DIRECCION_CIUDAD      = '" + valid(FI_DIRECCION_CIUDAD) + "'" +
                        ",FI_DIRECCION_ENTIDAD     = '" + valid(FI_DIRECCION_ENTIDAD) + "'" +
                        ",FI_DIRECCION_COLONIA     = '" + valid(FI_DIRECCION_COLONIA) + "'" +
                        ",FI_DIRECCION_CALLE       = '" + valid(FI_DIRECCION_CALLE) + "'" +
                        ",FI_DIRECCION_CP          = '" + FI_DIRECCION_CP + "'" +
                        " WHERE IDSIU           = '" + IDSIU + "'";

                    if (TMP == false)
                    {
                        if (db.execute(sql))
                        {
                            sql = "SELECT P.ID_PERSONA    " +
                                  "  FROM PERSONAS P INNER JOIN " +
                                  "       PERSONAS_SEDES S ON S.ID_PERSONA = P.ID_PERSONA " +
                                  " WHERE IDSIU            = '" + IDSIU + "' " +
                                  "   AND S.CVE_SEDE       = '" + CVESEDE + "' ";

                            res = db.getTable(sql);

                            if (res.Next())
                            {
                                ID_PERSONA = res.Get("ID_PERSONA");
                            }

                            if (ID_PERSONA != string.Empty && ID_PERSONA != null)
                            {
                                sql = "UPDATE PERSONAS_SEDES SET CVE_TIPODEPAGO ='" + CVE_TIPODEPAGO + "' , FECHA_R=GETDATE() ,FECHA_M=GETDATE() , USUARIO=" + sesion.pkUser.ToString() + "  WHERE ID_PERSONA = " + ID_PERSONA + " AND CVE_SEDE = '" + CVESEDE + "'";
                                db.execute(sql);

                                try
                                {
                                    sql = "SELECT ID_PERSONA FROM PERSONAS_SEDES WHERE CVE_SEDE=''";
                                    ResultSet resq = db.getTable(sql);
                                    while (resq.Next())
                                    {
                                        depurarDocentesSinSede(resq.GetLong("ID_PERSONA"));
                                    }

                                }
                                catch (Exception ex)
                                {
                                    return true;
                                }
                                return true;
                            }
                            else return false;
                        }
                        else return false;
                    }
                    else return db.execute(sql);
                }
                else
                {

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool mark()
        {
            string FECHA_M = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE PERSONAS_TMP SET REGISTRADO = 1, IMPORTADO = 1, FECHA_M = '" + FECHA_M + "' WHERE IDSIU = '" + IDSIU + "' and usuario = '" + sesion.pkUser + "'";
            return db.execute(sql);
        }

        public bool Importar(string data)
        {
            bool all_result = true;
            string[] arrChecked = data.Split(new char[] { ',' });

            foreach (string itemChecked in arrChecked)
            {
                this.IDSIU = itemChecked;

                // 1.- Consultamos los datos de la tabla temporal
                this.TMP = true;
                //elimaimportadosDuplicados();
                edit();

                // 2.- Checamos si existe en la tabla normal
                this.TMP = false;
                bool result;
                if (exist())
                    result = save();
                else
                    result = add();
                if (result)
                    mark();

                all_result = all_result & result;
            }
            return all_result;
        }

        public override string ToString()
        {
            return "IDSIU:" + IDSIU;
        }

        private static bool InsertaDetallesConflicto(string pOrigen, string pIdsiu, string pNombre, string pQuery, string pErrmsg, string pPkUser, string pUsuario)
        {
            bool exito = false;
            database db2 = new database();

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramOrigen = new Parametros();
            Parametros paramIdSiu = new Parametros();
            Parametros paramNombre = new Parametros();
            Parametros paramQuery = new Parametros();
            Parametros paramErrMsg = new Parametros();
            Parametros paramPkUser = new Parametros();
            Parametros paramUsuario = new Parametros();

            try
            {
                paramOrigen.nombreParametro = "@origen";
                paramOrigen.longitudParametro = 12;
                paramOrigen.tipoParametro = SqlDbType.NVarChar;
                paramOrigen.direccion = ParameterDirection.Input;
                paramOrigen.value = pOrigen;
                lParamS.Add(paramOrigen);

                paramIdSiu.nombreParametro = "@idsiu";
                paramIdSiu.longitudParametro = 10;
                paramIdSiu.tipoParametro = SqlDbType.NVarChar;
                paramIdSiu.direccion = ParameterDirection.Input;
                paramIdSiu.value = pIdsiu;
                lParamS.Add(paramIdSiu);

                paramNombre.nombreParametro = "@nombre";
                paramNombre.longitudParametro = 100;
                paramNombre.tipoParametro = SqlDbType.NVarChar;
                paramNombre.direccion = ParameterDirection.Input;
                paramNombre.value = pNombre;
                lParamS.Add(paramNombre);

                paramQuery.nombreParametro = "@query";
                paramQuery.longitudParametro = int.MaxValue;
                paramQuery.tipoParametro = SqlDbType.NVarChar;
                paramQuery.direccion = ParameterDirection.Input;
                paramQuery.value = pQuery;
                lParamS.Add(paramQuery);

                paramErrMsg.nombreParametro = "@errmsg";
                paramErrMsg.longitudParametro = int.MaxValue;
                paramErrMsg.tipoParametro = SqlDbType.NVarChar;
                paramErrMsg.direccion = ParameterDirection.Input;
                paramErrMsg.value = pErrmsg;
                lParamS.Add(paramErrMsg);

                paramPkUser.nombreParametro = "@pkuser";
                paramPkUser.longitudParametro = 50;
                paramPkUser.tipoParametro = SqlDbType.NVarChar;
                paramPkUser.direccion = ParameterDirection.Input;
                paramPkUser.value = pPkUser;
                lParamS.Add(paramPkUser);

                paramUsuario.nombreParametro = "@usuario";
                paramUsuario.longitudParametro = 180;
                paramUsuario.tipoParametro = SqlDbType.NVarChar;
                paramUsuario.direccion = ParameterDirection.Input;
                paramUsuario.value = pUsuario;
                lParamS.Add(paramUsuario);

                exito = db2.ExecuteStoreProcedure("sp_detallesConflictoProfesor_inserta", lParamS);
                if (exito == false)
                    return false;

                return true;
            }
            catch (Exception E)
            {
                return false;
            }
        }

        public static bool EliminaDetallesConflicto(string pPkUser)
        {
            database db2 = new database();
            string sql = "delete detalles_conflictos_profs where pkuser = '" + pPkUser + "'";
            try { return db2.execute(sql); } catch (Exception ex) { return false; }
        }

        public DataTable ConsultaDetallesConflicto(string pPkUser)
        {
            DataTable dt = new DataTable();
            string sql = "select * from detalles_conflictos_profs where pkuser = '" + pPkUser + "'";
            try
            {
                dt = db.SelectDataTable(sql);
            }
            catch (Exception ex)
            {
                dt = null;
            }

            return dt;
        }

        public bool BuscaPersona()
        {
            try
            {
                sql = "SELECT distinct * FROM QPersonas WHERE ";
                sql += "CVE_SEDE = '" + CVESEDE + "'";
                if (IDSIU != "" && IDSIU != null)
                    sql += " and IDSIU = '" + IDSIU + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    ID_PERSONA = res.Get("ID_PERSONA");
                    //Profesor = res.Get("PROFESOR");
                    //NOI = res.Get("NOI");
                    APELLIDOS = res.Get("APELLIDOS");
                    NOMBRES = res.Get("NOMBRES");
                    //cveFactura = res.Get("CVE_TIPOFACTURA");
                    //factura = res.Get("TIPOFACTURA");
                    //cveOrigen = res.Get("CVE_ORIGEN");
                    //origen = res.Get("ORIGEN");
                    IDSIU = res.Get("IDSIU");
                    //maxperiodo = res.Get("MAXPERIODO");
                    //maxciclo = res.Get("MAXCICLO");
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }

        }

        public DataTable BuscaPersonaSCP(string periodo)
        {
            DataTable dt = new DataTable();
            try
            {

                string periodos = periodo.Split('|')[0];
                int idadmin = int.Parse(periodo.Split('|')[1]);
                string campus = periodo.Split('|')[2];
                string idsius = periodo.Split('|')[3];

                if (periodos == "")
                {
                    sql = "SELECT IDSIU, NOMBRE AS DOCENTE FROM PERSONAS_TMP_SIN_CP ";
                    sql += " WHERE ID_ADMIN=" + idadmin + " AND IDSIU ='" + idsius + "' AND CVE_SEDE='" + campus + "';";
                }
                else
                {
                    sql = "SELECT DISTINCT(IDSIU), NOMBRE AS DOCENTE FROM PERSONAS_TMP_SIN_CP";
                    sql += " WHERE ID_ADMIN=" + idadmin + " AND PERIODO ='" + periodos + "' AND CVE_SEDE='" + campus + "';";
                }

                dt = db.SelectDataTable(sql);


            }
            catch (Exception ex)
            {
                dt = null;
            }
            return dt;

        }

        public int depurarDocentesSinSede(long idpersona)
        {
            sql = "";
            sql = "DELETE FROM PERSONAS_SEDES WHERE ID_PERSONA=" + idpersona;
            db.execute(sql);

            sql = "";
            sql = "DELETE FROM PERSONAS WHERE ID_PERSONA=" + idpersona;
            db.execute(sql);

            sql = "";
            sql = "DELETE FROM NOMINA WHERE ID_PERSONA=" + idpersona;
            db.execute(sql);

            return 1;
        }

        public int sinCP(string idsius, string periodo, int eliminaSinCP, string nombre)
        {
            if (eliminaSinCP == 0)
            {

                if (periodo == "")
                {
                    sql = "";
                    sql = "DELETE PERSONAS_TMP_SIN_CP WHERE  ID_ADMIN= " + sesion.pkUser + " AND CVE_SEDE='" + CVESEDE + "' AND IDSIU='" + idsius + "';";

                }
                else
                {
                    sql = "";
                    sql = "DELETE PERSONAS_TMP_SIN_CP WHERE  ID_ADMIN= " + sesion.pkUser + " AND CVE_SEDE='" + CVESEDE + "' AND PERIODO='" + periodo + "';";

                }
                db.execute(sql);
            }

            sql = "";
            sql = "INSERT INTO PERSONAS_TMP_SIN_CP (IDSIU,ID_ADMIN,CVE_SEDE,PERIODO,NOMBRE)VALUES('" + idsius + "'," + sesion.pkUser + ",'" + CVESEDE + "','" + periodo + "','" + nombre + "');";
            db.execute(sql);

            return 1;
        }

        public DataTable elimaimportadosDuplicados()
        {
            DataTable dt = new DataTable();
            string sql = "DELETE FROM PERSONAS WHERE IDSIU= '" + IDSIU + "'";
            try
            {
                db.execute(sql);
            }
            catch (Exception ex)
            {
                dt = null;
            }

            return dt;
        }
    }
}