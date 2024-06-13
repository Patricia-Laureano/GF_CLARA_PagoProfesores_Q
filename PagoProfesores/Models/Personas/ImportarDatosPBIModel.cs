using System;
using ConnectDB;
using Newtonsoft.Json;
using Session;
using System.Collections.Generic;
using System.Data;

namespace PagoProfesores.Models.Personas
{
    public class ImportarDatosPBIModel : SuperModel
    {

        [JsonProperty("ID_PBI")]
        public string ID_PBI { get; set; }
        [JsonProperty("PERIODO")]
        public string PERIODO { get; set; }
        [JsonProperty("CAMPUSPA")]
        public string CAMPUSPA { get; set; }
        [JsonProperty("ESCUELA")]
        public string ESCUELA { get; set; }
        [JsonProperty("ESCUELACODE")]
        public string ESCUELACODE { get; set; }
        [JsonProperty("NRC")]
        public string NRC { get; set; }
        [JsonProperty("SECCION")]
        public string SECCION { get; set; }
        [JsonProperty("MATERIA")]
        public string MATERIA { get; set; }
        [JsonProperty("CURSO")]
        public string CURSO { get; set; }
        [JsonProperty("NOMBRE_MATERIA")]
        public string NOMBRE_MATERIA { get; set; }
        [JsonProperty("TIPO_CURSO")]
        public string TIPO_CURSO { get; set; }
        [JsonProperty("TIPO_HORARIO")]
        public string TIPO_HORARIO { get; set; }
        [JsonProperty("METODO_INSTRUCCION")]
        public string METODO_INSTRUCCION { get; set; }
        [JsonProperty("STATUS")]
        public string STATUS { get; set; }
        [JsonProperty("PARTE_PERIODO")]
        public string PARTE_PERIODO { get; set; }
        [JsonProperty("FECHAS_PARTE_PERIODO")]
        public string FECHAS_PARTE_PERIODO { get; set; }
        [JsonProperty("HORAS_PROGRAMDAS")]
        public string HORAS_PROGRAMDAS { get; set; }
        [JsonProperty("LISTA_CRUZADA")]
        public string LISTA_CRUZADA { get; set; }
        [JsonProperty("ID_BANNER")]
        public string ID_BANNER { get; set; }
        [JsonProperty("RFC")]
        public string RFC { get; set; }
        [JsonProperty("CURP")]
        public string CURP { get; set; }
        [JsonProperty("DOCENTE")]
        public string DOCENTE { get; set; }
        [JsonProperty("MAXIMO_GRADO_ACADEMICO")]
        public string MAXIMO_GRADO_ACADEMICO { get; set; }
        [JsonProperty("TIPO_DOCENTE")]
        public string TIPO_DOCENTE { get; set; }
        [JsonProperty("TITULAR")]
        public string TITULAR { get; set; }
        [JsonProperty("NUM_INSCRITOS")]
        public string NUM_INSCRITOS { get; set; }
        [JsonProperty("PORCENTAGE_RESPONSABILIDAD")]
        public string PORCENTAGE_RESPONSABILIDAD { get; set; }
        [JsonProperty("FECHA_INICIO")]
        public string FECHA_INICIO { get; set; }
        [JsonProperty("FECHA_FIN")]
        public string FECHA_FIN { get; set; }
        [JsonProperty("TABULADOR_LC")]
        public string TABULADOR_LC { get; set; }
        [JsonProperty("TABULADOR_POS")]
        public string TABULADOR_POS { get; set; }
        [JsonProperty("TABULADOR_DI")]
        public string TABULADOR_DI { get; set; }
        [JsonProperty("ACTIVIDADES_NO_DOCENCIA")]
        public string ACTIVIDADES_NO_DOCENCIA { get; set; }
        [JsonProperty("USUARIO_IMPORTA")]
        public string USUARIO_IMPORTA { get; set; }
        [JsonProperty("IMPORTADO")]
        public string IMPORTADO { get; set; }
        [JsonProperty("FECHA_IMPORTADO")]
        public string FECHA_IMPORTADO { get; set; }
        [JsonProperty("IMPORTADO_DESDE")]
        public string IMPORTADO_DESDE { get; set; }

        public string _msg { get; set; }
        public string sql { get; set; }
 
        
        public bool TMP = false;
        public string TABLE = "POWER_BI";
        public string fechainii { get; set; }
        public string fechainff { get; set; }

        public bool Clean()
        {
            sql = "DELETE FROM POWER_BI_TMP WHERE USUARIO_IMPORTA = '" + sesion.pkUser + "'";
            db.execute(sql);

            sql = "";
            sql = "DELETE POWERBI_TMP_CONFLICTO WHERE  ID_ADMIN= " + sesion.pkUser + ";";

            return db.execute(sql);

        }

        public bool exist()
        {
            //TABLE = TMP ? "QPowerBI_TMP" : "POWER_BI";

            string sql = "SELECT TOP 1 ID_PBI " +
                         "  FROM POWER_BI WHERE IDBANNER = '" + ID_BANNER + "' AND NRC=" + NRC + " AND CURSO='" + CURSO + "'";


            ResultSet res = db.getTable(sql);

            if (res.Next())
            {
                return true;
            }
            else
            {
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
            string sql = string.Empty;
            //if ((DateTime.Parse(fechainii.ToString()) >= DateTime.Parse(FECHA_INICIO) && DateTime.Parse(fechainii.ToString()) <= DateTime.Parse(FECHA_FIN)) && (DateTime.Parse(fechainff.ToString()) >= DateTime.Parse(FECHA_INICIO) && DateTime.Parse(fechainff.ToString()) <= DateTime.Parse(FECHA_FIN)))
            //{
                string[] VALUES = {
                PERIODO
                  ,CAMPUSPA
                  ,ESCUELACODE
                  ,ESCUELA
                  ,NRC
                  ,SECCION
                  ,MATERIA
                  ,CURSO
                  ,NOMBRE_MATERIA
                  ,TIPO_CURSO
                  ,TIPO_HORARIO
                  ,METODO_INSTRUCCION
                  ,STATUS
                  ,PARTE_PERIODO
                  ,FECHAS_PARTE_PERIODO
                  ,HORAS_PROGRAMDAS
                  ,LISTA_CRUZADA
                  ,ID_BANNER
                  ,RFC
                  ,CURP
                  ,valid(DOCENTE)
                  ,MAXIMO_GRADO_ACADEMICO
                  ,TIPO_DOCENTE
                  ,TITULAR
                  ,NUM_INSCRITOS
                  ,PORCENTAGE_RESPONSABILIDAD
                  ,__validDateTime(FECHA_INICIO)
                  ,__validDateTime(FECHA_FIN)
                  ,TABULADOR_LC
                  ,TABULADOR_POS
                  ,TABULADOR_DI
                  ,ACTIVIDADES_NO_DOCENCIA
                  ,IMPORTADO
                  ,USUARIO_IMPORTA
                  ,IMPORTADO_DESDE
            };
                try
                {
                    sql = "INSERT INTO POWER_BI_TMP" +
                              " (PERIODO,CAMPUS,ESCUELA,DESCRIPCION_ESCUELA,NRC,SECCION,MATERIA,CURSO,NOMBRE_MATERIA,TIPO_CURSO ,TIPO_HORARIO,METODO_INSTRUCCION" +
                               ",ESTATUS,PARTE_PERIODO,FECHA_PARTE_PERIODO,HORAS_PROGRAMADAS,LISTA_CRUZADA ,IDBANNER,RFC,CURP ,DOCENTE,MAXIMO_GRADO_ACADEMICO" +
                               ",TIPO_DOCENTE,PROFESOR_TITULAR,NUMERO_INSCRITOS,PORCENTAJE_RESPONSABILIDAD,FECHA_INICIO,FECHA_FIN,TABULADOR_LC,TABULADOR_POS" +
                               ",TABULADOR_DI,ACTIVIDADES_NO_DOCENCIA,IMPORTADO,USUARIO_IMPORTA,IMPORTADO_DESDE)" +
                              " VALUES ('" + string.Join("','", VALUES) + "');";

                    result = db.execute(sql);
                }
                catch (Exception ex)
                {
                    result = false;
                }


                if (result == false)
                {
                    Log.write(this, "addTmpPOWER_BI", LOG.REGISTRO, ("Error, ") + sql, this.sesion);
                }
            //}
      
         
           
            
            return result;
        }

        public bool add()
        {
            try
            {
                string[] VALUES = {
                    PERIODO
                  ,CAMPUSPA
                  ,ESCUELACODE
                  ,ESCUELA
                  ,NRC
                  ,SECCION
                  ,MATERIA
                  ,CURSO
                  ,NOMBRE_MATERIA
                  ,TIPO_CURSO
                  ,TIPO_HORARIO
                  ,METODO_INSTRUCCION
                  ,STATUS
                  ,PARTE_PERIODO
                  ,FECHAS_PARTE_PERIODO
                  ,HORAS_PROGRAMDAS
                  ,LISTA_CRUZADA
                  ,ID_BANNER
                  ,RFC
                  ,CURP
                  ,valid(DOCENTE)
                  ,MAXIMO_GRADO_ACADEMICO
                  ,TIPO_DOCENTE
                  ,TITULAR
                  ,NUM_INSCRITOS
                  ,PORCENTAGE_RESPONSABILIDAD
                  ,__validDateTime(FECHA_INICIO)
                  ,__validDateTime(FECHA_FIN)
                  ,TABULADOR_LC
                  ,TABULADOR_POS
                  ,TABULADOR_DI
                  ,ACTIVIDADES_NO_DOCENCIA
                  ,IMPORTADO
                  ,USUARIO_IMPORTA
                  ,IMPORTADO_DESDE
                };

                string sql = "INSERT INTO POWER_BI" +
                         " (PERIODO,CAMPUS,ESCUELA,DESCRIPCION_ESCUELA,NRC,SECCION,MATERIA,CURSO,NOMBRE_MATERIA,TIPO_CURSO ,TIPO_HORARIO,METODO_INSTRUCCION" +
                          ",ESTATUS,PARTE_PERIODO,FECHA_PARTE_PERIODO,HORAS_PROGRAMADAS,LISTA_CRUZADA ,IDBANNER,RFC,CURP ,DOCENTE,MAXIMO_GRADO_ACADEMICO" +
                          ",TIPO_DOCENTE,PROFESOR_TITULAR,NUMERO_INSCRITOS,PORCENTAJE_RESPONSABILIDAD,FECHA_INICIO,FECHA_FIN,TABULADOR_LC,TABULADOR_POS" +
                          ",TABULADOR_DI,ACTIVIDADES_NO_DOCENCIA,IMPORTADO ,USUARIO_IMPORTA,IMPORTADO_DESDE,FECHA_IMPORTADO)" +
                         " VALUES ('" + string.Join("','", VALUES) + "',GETDATE());";
                //if (logGeneral != false)
                //{
                //    Log.write(this, "POWER_BI", LOG.REGISTRO, ("sql: ") + sql, this.sesion);
                //    logGeneral = true;

                //}
                return db.execute(sql);
            }
            catch( Exception ex)
            {
                Log.write(this, "POWER_BI ERROR", LOG.REGISTRO, ("sql: ") + sql, this.sesion);
                return false;
            }
               
           

        }

        public bool edit()
        {
           string sql = "";
           sql = "SELECT TOP 1 * FROM QPowerBI_TMP WHERE ID_PBI_TMP = '" + ID_PBI + "'  AND USUARIO_IMPORTA = '" + sesion.pkUser.ToString() + "'";

            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                ID_PBI = res.Get("ID_PBI_TMP");
                PERIODO = res.Get("PERIODO");
                CAMPUSPA = res.Get("CAMPUS");
                ESCUELA = res.Get("DESCRIPCION_ESCUELA");
                ESCUELACODE = res.Get("ESCUELA");
                NRC = res.Get("NRC");
                SECCION = res.Get("SECCION");
                MATERIA = res.Get("MATERIA");
                CURSO = res.Get("CURSO");
                NOMBRE_MATERIA = res.Get("NOMBRE_MATERIA");
                TIPO_CURSO = res.Get("TIPO_CURSO");
                TIPO_HORARIO = res.Get("TIPO_HORARIO");
                METODO_INSTRUCCION = res.Get("METODO_INSTRUCCION");
                STATUS = res.Get("ESTATUS");
                PARTE_PERIODO = res.Get("PARTE_PERIODO");
                FECHAS_PARTE_PERIODO = res.Get("FECHA_PARTE_PERIODO");
                HORAS_PROGRAMDAS = res.Get("HORAS_PROGRAMADAS");
                LISTA_CRUZADA = res.Get("LISTA_CRUZADA");
                ID_BANNER = res.Get("IDBANNER");
                RFC = res.Get("RFC");
                CURP = res.Get("CURP");
                DOCENTE = res.Get("DOCENTE");
                MAXIMO_GRADO_ACADEMICO = res.Get("MAXIMO_GRADO_ACADEMICO");
                TIPO_DOCENTE = res.Get("TIPO_DOCENTE");
                TITULAR = res.Get("PROFESOR_TITULAR");
                NUM_INSCRITOS = res.Get("NUMERO_INSCRITOS");
                PORCENTAGE_RESPONSABILIDAD = res.Get("PORCENTAJE_RESPONSABILIDAD");
                FECHA_INICIO = res.Get("FECHA_INICIO");
                FECHA_FIN = res.Get("FECHA_FIN");
                TABULADOR_LC = res.Get("TABULADOR_LC");
                TABULADOR_POS = res.Get("TABULADOR_POS");
                TABULADOR_DI = res.Get("TABULADOR_DI");
                ACTIVIDADES_NO_DOCENCIA = res.Get("ACTIVIDADES_NO_DOCENCIA");
                IMPORTADO = res.Get("IMPORTADO");
                FECHA_IMPORTADO = res.Get("FECHA_IMPORTADO");
                USUARIO_IMPORTA = res.Get("USUARIO_IMPORTA");

                return true;
            }
            return false;
        }

        public bool save()
        {
            ResultSet res;
   
            try
            {

                if (ID_BANNER != "" || ID_BANNER != null)
                {
                    //TABLE = TMP ? "POWER_BI_TMP" : "POWER_BI";
                    string sql = "UPDATE POWER_BI SET " +
                        " PERIODO             ='" + valid(PERIODO) + "'" +
                        ",CAMPUS               ='" + valid(CAMPUSPA) + "'" +
                        ",ESCUELA              ='" + ESCUELACODE + "'" +
                        ",DESCRIPCION_ESCUELA  ='" + ESCUELA + "'" +
                        ",NRC                  ='" + NRC + "'" +
                        ",SECCION              ='" + SECCION + "'" +
                        ",MATERIA              ='" + MATERIA + "'" +
                        ",CURSO                ='" + CURSO + "'" +
                        ",NOMBRE_MATERIA       ='" + NOMBRE_MATERIA + "'" +
                        ",TIPO_CURSO           ='" + TIPO_CURSO + "'" +
                        ",TIPO_HORARIO         ='" + TIPO_HORARIO + "'" +
                        ",METODO_INSTRUCCION   ='" + METODO_INSTRUCCION + "'" +
                        ",ESTATUS              ='" + STATUS + "'" +
                        ",PARTE_PERIODO        ='" + PARTE_PERIODO + "'" +
                        ",FECHA_PARTE_PERIODO  ='" + FECHAS_PARTE_PERIODO + "'" +
                        ",HORAS_PROGRAMADAS    ='" + HORAS_PROGRAMDAS + "'" +
                        ",LISTA_CRUZADA        ='" + LISTA_CRUZADA + "'" +
                        ",IDBANNER             ='" + ID_BANNER + "'" +
                        ",RFC                  ='" + RFC + "'" +
                        ",CURP                 ='" + CURP + "'" +
                        ",DOCENTE              ='" + DOCENTE + "'" +
                        ",MAXIMO_GRADO_ACADEMICO  ='" + MAXIMO_GRADO_ACADEMICO + "'" +
                        ",TIPO_DOCENTE          ='" + TIPO_DOCENTE + "'" +
                        ",PROFESOR_TITULAR      ='" + TITULAR + "'" +
                         ",NUMERO_INSCRITOS       ='" + NUM_INSCRITOS + "'" +
                        ",PORCENTAJE_RESPONSABILIDAD   ='" + PORCENTAGE_RESPONSABILIDAD + "'" +
                        ",FECHA_INICIO           ='" + __validDateTime(FECHA_INICIO)+ "'" +
                        ",FECHA_FIN              ='" + __validDateTime(FECHA_FIN) + "'" +
                        ",TABULADOR_LC           ='" + TABULADOR_LC + "'" +
                        ",TABULADOR_POS          ='" + TABULADOR_POS + "'" +
                        ",TABULADOR_DI      ='" + TABULADOR_DI + "'" +
                        ",ACTIVIDADES_NO_DOCENCIA     ='" + ACTIVIDADES_NO_DOCENCIA + "'" +
                        ",IMPORTADO     ='" + IMPORTADO + "'" +
                        ",FECHA_ACTUALIZADO       = GETDATE()" +
                        ",USUARIO_ACTUALIZA          ='" + sesion.pkUser + "'" +
                        ",IMPORTADO_DESDE     ='WS'" +
                        " WHERE IDBANNER           ='" + ID_BANNER + "' AND NRC=" + NRC + " AND CURSO='" + CURSO + "';";

                    return db.execute(sql);
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
            string sql = "UPDATE POWER_BI_TMP SET IMPORTADO = 1, FECHA_IMPORTADO = '" + FECHA_M + "' WHERE IDBANNER = '" + ID_BANNER + "' and USUARIO_IMPORTA = '" + sesion.pkUser + "'";
            return db.execute(sql);
        }

        public bool Importar(string data)
        {
            bool all_result = true;
            string[] arrChecked = data.Split(new char[] { ',' });
            bool logPowerBi = false;
            bool logPowerBiInsert = false;
            foreach (string itemChecked in arrChecked)
            {
                this.ID_PBI = itemChecked;

                // 1.- Consultamos los datos de la tabla temporal
                this.TMP = true;
               
                edit();

                // 2.- Checamos si existe en la tabla normal
                this.TMP = false;
                bool result;
                if (exist())
                {
                    result = save();
                    if (logPowerBi != false)
                    {
                        Log.write(this, "POWER_BI", LOG.EDICION, ("sql: ") + sql, this.sesion);
                        logPowerBi = true;

                    }
                }
                else
                {
                    result = add();
                    if (logPowerBiInsert != false)
                    {
                        Log.write(this, "POWER_BI", LOG.REGISTRO, ("sql: ") + sql, this.sesion);
                        logPowerBiInsert = true;

                    }
                }

                if (result)
                    mark();

                all_result = all_result & result;
            }
            return all_result;
        }

        public override string ToString()
        {
            return "ID_BANNER:" + ID_BANNER;
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

        public string mostrarSedes()
        {
            try
            {
                sql = "SELECT * FROM SEDES ORDER BY CVE_SEDE";
                ResultSet resQ = db.getTable(sql);
                while (resQ.Next())
                {
                    if (CAMPUSPA == null)
                    {
                        CAMPUSPA += resQ.Get("CVE_SEDE") + "|" + resQ.Get("SEDE");
                    }
                    else
                    {
                        CAMPUSPA += "/" + resQ.Get("CVE_SEDE") + "|" + resQ.Get("SEDE");
                    }
                }
                return CAMPUSPA;
            }
            catch { return CAMPUSPA; }
        }

        public int conConflicto(string idsiu, string nombreDocente, string campusDocente,string nrc, string curso)
        {
            sql = "";
            sql = "INSERT INTO POWERBI_TMP_CONFLICTO (IDSIU,ID_ADMIN,CVE_SEDE,NOMBRE,NRC, CURSO)VALUES('" + idsiu + "'," + sesion.pkUser + ",'" + campusDocente + "','" + nombreDocente + "','"+ nrc +"','"+curso +"'); ";
            db.execute(sql);

           return 1;
        }

        public DataTable BuscaPOWERBI_Conf(string datoss)
        {
            DataTable dt = new DataTable();
            try
            {
                if (datoss != "" || datoss != null)
                {
                    sql = "SELECT IDSIU, NOMBRE AS DOCENTE, CVE_SEDE AS CAMPUS, NRC, CURSO FROM POWERBI_TMP_CONFLICTO ";
                    sql += " WHERE ID_ADMIN=" + datoss;

                    dt = db.SelectDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                dt = null;
            }
            return dt;

        }
    }
}