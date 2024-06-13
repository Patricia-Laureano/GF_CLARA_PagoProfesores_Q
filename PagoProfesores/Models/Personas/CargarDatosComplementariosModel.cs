using ConnectDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Personas
{
    public class CargarDatosComplementariosModel : SuperModel
    {
        public string IDADMIN { get; set; }
        public string IDBANNER { get; set; }
        public string DOCENTE { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string MAXIMO_GRADO_ACADEMICO { get; set; }
        public string TIPO_DOCENTE { get; set; }
        public string NRC { get; set; }
        public string HORAS_PROGRAMADAS { get; set; }
        public string TABULADOR_DI { get; set; }
        public string ESCUELA { get; set; }
        public string DESCRIPCION_ESCUELA { get; set; }
        public string CAMPUS { get; set; }
        public string MATERIA { get; set; }
        public string CURSO { get; set; }
        public string NOMBRE_MATERIA { get; set; }
        public string PERIODO { get; set; }
        public string PARTE_PERIODO { get; set; }
        public string FECHA_INICIO { get; set; }
        public string FECHA_FIN { get; set; }
        public string NUMERO_INSCRITOS { get; set; }
        public string USUARIO_IMPORTA { get; set; }
        public string USUARIO_ACTUALIZA{ get; set; }
        public string ID_PBI_TMP { get; set; }
        public string FECHA_IMPORTADO { get; set; }
        public string SECCION{ get; set; }
        public string TIPO_CURSO { get; set; }
        public string TIPO_HORARIO { get; set; }
        public string METODO_INSTRUCCION { get; set; }
        public string ESTATUS { get; set; }
        public string FECHA_PARTE_PERIODO { get; set; }
        public string LISTA_CRUZADA { get; set; }
        public string PROFESOR_TITULAR { get; set; }
        public string PORCENTAJE_RESPONSABILIDAD { get; set; }
        public string ACTIVIDADES_NO_DOCENCIA { get; set; }
        public string IMPORTADO { get; set; }
        public string IMPORTADO_DESDE { get; set; }

        char[] ERRORES;

        public Dictionary<CARGARDATOS, bool> validCells;

        public bool saved;
        public string sql;
        public string sql1;

        public void clean()
        {
            string sql = "DELETE FROM POWER_BI_TMP WHERE USUARIO_IMPORTA=" + sesion.pkUser;
            db.execute(sql);
        }

        // ____________________________  VALIDAR ____________________________
        public enum CARGARDATOS
        {
            IDBANNER, DOCENTE, RFC, CURP, MAXIMO_GRADO_ACADEMICO, TIPO_DOCENTE, NRC, HORAS_PROGRAMADAS, TABULADOR_DI, ESCUELA, CAMPUS, MATERIA, CURSO, NOMBRE_MATERIA, PERIODO, PARTE_PERIODO, FECHA_INICIO, FECHA_FIN, NUMERO_INSCRITOS, FECHA_IMPORTADO, USUARIO_IMPORTA
        };

        public bool validate()
        {
            validCells = new Dictionary<CARGARDATOS, bool>();
            foreach (CARGARDATOS item in Enum.GetValues(typeof(CARGARDATOS)))
                validCells[item] = true;

            validCells[CARGARDATOS.IDBANNER] = (IDBANNER.Length == 20);
            validCells[CARGARDATOS.DOCENTE] = (DOCENTE.Length == 100);
            validCells[CARGARDATOS.RFC] = (RFC.Length == 13);
            validCells[CARGARDATOS.CURP] = (CURP.Length == 18);
            validCells[CARGARDATOS.MAXIMO_GRADO_ACADEMICO] = (MAXIMO_GRADO_ACADEMICO.Length == 50);
            validCells[CARGARDATOS.TIPO_DOCENTE] = (MAXIMO_GRADO_ACADEMICO.Length == 20);
            validCells[CARGARDATOS.NRC] = true;
            validCells[CARGARDATOS.HORAS_PROGRAMADAS] = (HORAS_PROGRAMADAS.Length == 10);
            validCells[CARGARDATOS.TABULADOR_DI] = (TABULADOR_DI.Length == 7);
            validCells[CARGARDATOS.ESCUELA] = (ESCUELA.Length == 5);
            validCells[CARGARDATOS.CAMPUS] = (CAMPUS.Length == 5);
            validCells[CARGARDATOS.MATERIA] = (MATERIA.Length == 5);
            validCells[CARGARDATOS.CURSO] = (CURSO.Length == 5);
            validCells[CARGARDATOS.NOMBRE_MATERIA] = (NOMBRE_MATERIA.Length == 50);
            validCells[CARGARDATOS.PERIODO] = (PERIODO.Length == 20);
            validCells[CARGARDATOS.PARTE_PERIODO] = true;
            validCells[CARGARDATOS.FECHA_INICIO] = true;
            validCells[CARGARDATOS.FECHA_FIN] = true;
            validCells[CARGARDATOS.FECHA_IMPORTADO] = true;

            validCells[CARGARDATOS.IDBANNER] = IDBANNER.Length <= 10;
            validCells[CARGARDATOS.USUARIO_IMPORTA] = true;
            // Se busca algun error.
            foreach (CARGARDATOS item in Enum.GetValues(typeof(CARGARDATOS)))
                if (validCells[item] == false)
                    return false;
            return true;
        }

        public object[] getArrayObject(Dictionary<CARGARDATOS, object> dict)
        {
            dict[CARGARDATOS.IDBANNER] = IDBANNER;
            dict[CARGARDATOS.DOCENTE] = DOCENTE;
            dict[CARGARDATOS.RFC] = RFC;
            dict[CARGARDATOS.CURP] = CURP;
            dict[CARGARDATOS.MAXIMO_GRADO_ACADEMICO] = MAXIMO_GRADO_ACADEMICO;
            dict[CARGARDATOS.TIPO_DOCENTE] = TIPO_DOCENTE;
            dict[CARGARDATOS.NRC] = NRC;
            dict[CARGARDATOS.HORAS_PROGRAMADAS] = HORAS_PROGRAMADAS;
            dict[CARGARDATOS.TABULADOR_DI] = TABULADOR_DI;
            dict[CARGARDATOS.ESCUELA] = ESCUELA;
            dict[CARGARDATOS.CAMPUS] = CAMPUS;
            dict[CARGARDATOS.MATERIA] = MATERIA;
            dict[CARGARDATOS.CURSO] = CURSO;
            dict[CARGARDATOS.NOMBRE_MATERIA] = NOMBRE_MATERIA;
            dict[CARGARDATOS.PERIODO] = PERIODO;
            dict[CARGARDATOS.PARTE_PERIODO] = PARTE_PERIODO;
            dict[CARGARDATOS.FECHA_INICIO] = __validDateTime(FECHA_INICIO);
            dict[CARGARDATOS.FECHA_FIN] = __validDateTime(FECHA_FIN);
            dict[CARGARDATOS.NUMERO_INSCRITOS] = NUMERO_INSCRITOS;
            dict[CARGARDATOS.USUARIO_IMPORTA] = sesion.pkUser;

            return dict.Values.ToArray();
        }

        public char[] getArrayChars()
        {
            Dictionary<CARGARDATOS, char> dict = new Dictionary<CARGARDATOS, char>();
            dict[CARGARDATOS.IDBANNER] = validCells[CARGARDATOS.IDBANNER] ? '1' : '0';
            dict[CARGARDATOS.DOCENTE] = validCells[CARGARDATOS.DOCENTE] ? '1' : '0';
            dict[CARGARDATOS.RFC] = validCells[CARGARDATOS.RFC] ? '1' : '0';
            dict[CARGARDATOS.CURP] = validCells[CARGARDATOS.CURP] ? '1' : '0';
            dict[CARGARDATOS.MAXIMO_GRADO_ACADEMICO] = validCells[CARGARDATOS.MAXIMO_GRADO_ACADEMICO] ? '1' : '0';
            dict[CARGARDATOS.TIPO_DOCENTE] = validCells[CARGARDATOS.TIPO_DOCENTE] ? '1' : '0';

            dict[CARGARDATOS.NRC] = validCells[CARGARDATOS.NRC] ? '1' : '0';
            dict[CARGARDATOS.HORAS_PROGRAMADAS] = validCells[CARGARDATOS.HORAS_PROGRAMADAS] ? '1' : '0';
            dict[CARGARDATOS.TABULADOR_DI] = validCells[CARGARDATOS.TABULADOR_DI] ? '1' : '0';

            dict[CARGARDATOS.ESCUELA] = validCells[CARGARDATOS.ESCUELA] ? '1' : '0';
            dict[CARGARDATOS.CAMPUS] = validCells[CARGARDATOS.CAMPUS] ? '1' : '0';
            dict[CARGARDATOS.MATERIA] = validCells[CARGARDATOS.MATERIA] ? '1' : '0';
            dict[CARGARDATOS.CURSO] = validCells[CARGARDATOS.CURSO] ? '1' : '0';
            dict[CARGARDATOS.NOMBRE_MATERIA] = validCells[CARGARDATOS.NOMBRE_MATERIA] ? '1' : '0';

            dict[CARGARDATOS.PERIODO] = validCells[CARGARDATOS.PERIODO] ? '1' : '0';
            dict[CARGARDATOS.PARTE_PERIODO] = validCells[CARGARDATOS.PARTE_PERIODO] ? '1' : '0';

            dict[CARGARDATOS.FECHA_INICIO] = validCells[CARGARDATOS.FECHA_INICIO] ? '1' : '0';
            dict[CARGARDATOS.FECHA_FIN] = validCells[CARGARDATOS.FECHA_FIN] ? '1' : '0';
            dict[CARGARDATOS.NUMERO_INSCRITOS] = validCells[CARGARDATOS.NUMERO_INSCRITOS] ? '1' : '0';
            dict[CARGARDATOS.USUARIO_IMPORTA] =  validCells[CARGARDATOS.USUARIO_IMPORTA] ? '1' : '0';

            return dict.Values.ToArray();
        }

        public void FindFirstError(out int total, out int maxErrors, out string idsError)
        {
            maxErrors = 0;
            idsError = "";
            string sql;

            // Contar registros auditados
            sql = "SELECT COUNT(*) AS 'MAX' FROM POWER_BI_TMP WHERE USUARIO_IMPORTA = " + sesion.pkUser;
            total = db.Count(sql);

            // Contar errores
            string correcto = "";
            for (int i = 0; i < Enum.GetValues(typeof(CARGARDATOS)).Length; i++)
                correcto += '1';
            sql = "SELECT COUNT(*) AS 'MAX' FROM POWER_BI_TMP WHERE USUARIO_IMPORTA = " + sesion.pkUser + " AND ERRORES <> '" + correcto + "'";
            maxErrors = db.Count(sql);

            // Consultar primeros 5 errores.
            sql = "SELECT TOP 5 * FROM POWER_BI_TMP WHERE USUARIO_IMPORTA = " + sesion.pkUser + " AND ERRORES <> '" + correcto + "'";
            idsError = "";
            ResultSet res = db.getTable(sql);
            List<string> list = new List<string>();
            while (res.Next())
                list.Add(res.Get("IDBANNER"));
            idsError = string.Join(",", list);
        }

        public bool Add_TMP()
        {
            try
            {
                ERRORES = getArrayChars();
                Dictionary<string, string> dic = prepareData(true, true);
                sql = "INSERT INTO"
                    + " POWER_BI_TMP (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
                    + " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";
                return db.execute(sql);
            }
            catch { return false; }
        }

        private Dictionary<string, string> prepareData(bool add, bool tmp)
        {
            string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (add)
            {
                dic.Add("IDBANNER", IDBANNER);
                dic.Add("DOCENTE", DOCENTE.ToString());
                dic.Add("RFC", RFC);
                dic.Add("CURP", CURP);
                dic.Add("MAXIMO_GRADO_ACADEMICO", MAXIMO_GRADO_ACADEMICO);
                dic.Add("TIPO_DOCENTE", TIPO_DOCENTE);
                dic.Add("NRC", NRC.ToString());
                dic.Add("HORAS_PROGRAMADAS", HORAS_PROGRAMADAS.ToString());
                dic.Add("TABULADOR_DI", TABULADOR_DI);
                dic.Add("ESCUELA", ESCUELA);
                dic.Add("CAMPUS", CAMPUS);
                dic.Add("MATERIA", MATERIA);
                dic.Add("CURSO", CURSO);
                dic.Add("NOMBRE_MATERIA", NOMBRE_MATERIA);
                dic.Add("PERIODO", PERIODO.ToString());
                dic.Add("PARTE_PERIODO", PARTE_PERIODO);
                dic.Add("FECHA_INICIO", FECHA_INICIO);
                dic.Add("FECHA_FIN", FECHA_FIN);
                dic.Add("NUMERO_INSCRITOS", NUMERO_INSCRITOS.ToString());
                dic.Add("USUARIO_IMPORTA", sesion.pkUser.ToString());
            }
            return dic;
        }


        public bool Importar(string data)
        {
               return exist();
               
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

        public bool UpdateCargarDatosComplementariosXExcel(string IDBANNER, string NRC, string CURSO)
        {
            try
            {
                bool exito = false;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramUSUARIO_IMPORTA = new Parametros();
                Parametros paramIDBANNER = new Parametros();
                Parametros paramNRC = new Parametros();
                Parametros paramCURSO = new Parametros();

                try
                {

                    paramIDBANNER = new Parametros();
                    paramIDBANNER.nombreParametro = "@idbanner";
                    paramIDBANNER.tipoParametro = SqlDbType.NVarChar;
                    paramIDBANNER.direccion = ParameterDirection.Input;
                    paramIDBANNER.value = IDBANNER;
                    lParamS.Add(paramIDBANNER);

                    paramNRC = new Parametros();
                    paramNRC.nombreParametro = "@nrc";
                    paramNRC.tipoParametro = SqlDbType.Int;
                    paramNRC.direccion = ParameterDirection.Input;
                    paramNRC.value = NRC;
                    lParamS.Add(paramNRC);

                    paramCURSO = new Parametros();
                    paramCURSO.nombreParametro = "@curso";
                    paramCURSO.tipoParametro = SqlDbType.NVarChar;
                    paramCURSO.direccion = ParameterDirection.Input;
                    paramCURSO.value = CURSO;
                    lParamS.Add(paramCURSO);

                    paramUSUARIO_IMPORTA = new Parametros();
                    paramUSUARIO_IMPORTA.nombreParametro = "@usuarioImporta";
                    paramUSUARIO_IMPORTA.tipoParametro = SqlDbType.Int;
                    paramUSUARIO_IMPORTA.direccion = ParameterDirection.Input;
                    paramUSUARIO_IMPORTA.value = sesion.pkUser.ToString();
                    lParamS.Add(paramUSUARIO_IMPORTA);

                    exito = db.ExecuteStoreProcedure("sp_actualiza_PBI", lParamS);
                    
                    return exito;
                }
                catch
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool exist()
        {
            //TABLE = TMP ? "QPowerBI_TMP" : "POWER_BI";

            string sql = "SELECT * FROM QPowerBI_TMP WHERE USUARIO_IMPORTA = '" + sesion.pkUser.ToString() + "'"; ;
            ResultSet res = db.getTable(sql);
            bool resultado = false;
            if (res != null)
            {
                if (res.Count > 0)
                {
                    while (res.Next())
                    {
                        sql1 = "SELECT *  FROM POWER_BI WHERE IDBANNER = '" + res.Get("IDBANNER") + "' AND NRC=" + res.Get("NRC") + " AND CURSO='" + res.Get("CURSO") + "' AND  USUARIO_IMPORTA = '" + sesion.pkUser + "'";

                        ResultSet res1 = db.getTable(sql1);

                        if (res1.Next())
                        {
                            resultado= UpdateCargarDatosComplementariosXExcel(res.Get("IDBANNER"), res.Get("NRC"), res.Get("CURSO"));
                        }
                        else
                        {
                            resultado= add(res.Get("IDBANNER"), res.Get("NRC"),res.Get("CURSO"));
                        }
                    
                    }
                }
            }
            return resultado;
        }

        public bool add(string IDBANNER,string NRC,string CURSO)
        {
            try
            {
                bool exito = false;

                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramUSUARIO_IMPORTA = new Parametros();
                Parametros paramIDBANNER = new Parametros();
                Parametros paramNRC = new Parametros();
                Parametros paramCURSO = new Parametros();

                try
                {
                    paramUSUARIO_IMPORTA = new Parametros();
                    paramUSUARIO_IMPORTA.nombreParametro = "@usuarioImporta";
                    paramUSUARIO_IMPORTA.tipoParametro = SqlDbType.BigInt;
                    paramUSUARIO_IMPORTA.direccion = ParameterDirection.Input;
                    paramUSUARIO_IMPORTA.value = sesion.pkUser.ToString();
                    lParamS.Add(paramUSUARIO_IMPORTA);

                    paramIDBANNER = new Parametros();
                    paramIDBANNER.nombreParametro = "@idbanner";
                    paramIDBANNER.tipoParametro = SqlDbType.NVarChar;
                    paramIDBANNER.direccion = ParameterDirection.Input;
                    paramIDBANNER.value = IDBANNER;
                    lParamS.Add(paramIDBANNER);

                    paramNRC = new Parametros();
                    paramNRC.nombreParametro = "@nrc";
                    paramNRC.tipoParametro = SqlDbType.Int;
                    paramNRC.direccion = ParameterDirection.Input;
                    paramNRC.value = NRC;
                    lParamS.Add(paramNRC);

                    paramCURSO = new Parametros();
                    paramCURSO.nombreParametro = "@curso";
                    paramCURSO.tipoParametro = SqlDbType.NVarChar;
                    paramCURSO.direccion = ParameterDirection.Input;
                    paramCURSO.value = CURSO;
                    lParamS.Add(paramCURSO);

                   

                    exito = db.ExecuteStoreProcedure("sp_inserta_PBI", lParamS);
                    
                    return exito;
                }
                catch
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }


}