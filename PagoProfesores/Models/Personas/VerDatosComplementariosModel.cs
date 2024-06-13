using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ConnectDB;
using System.Diagnostics;

namespace PagoProfesores.Models.Personas
{
    public class VerDatosComplementariosModel : SuperModel
    {
        public string IDADMIN { get; set; }
        public string ID_PBI { get; set; }
        public string IDBANNER { get; set; }
        public string DOCENTE { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string MAXIMO_GRADO_ACADEMICO { get; set; }
        public string TIPO_DOCENTE { get; set; }
        public int NRC { get; set; }
        public string HORAS_PROGRAMADAS { get; set; }
        public string TABULADOR_DI { get; set; }
        public string ESCUELA { get; set; }
        public string CAMPUS { get; set; }
        public string MATERIA { get; set; }
        public string CURSO { get; set; }
        public string NOMBRE_MATERIA { get; set; }
        public string PERIODO { get; set; }
        public string PARTE_PERIODO { get; set; }
        public string FECHA_INICIO { get; set; }
        public string FECHA_FIN { get; set; }
        public int NUMERO_INSCRITOS { get; set; }
        public string USUARIO_IMPORTA { get; set; }
        public string USUARIO_ACTUALIZA { get; set; }
        public string ID_PBI_TMP { get; set; }
        public string FECHA_IMPORTADO { get; set; }
        public string DESCRIPCION_ESCUELA { get; set; }
        public string SECCION { get; set; }
        public string TIPO_CURSO { get; set; }
        public string TIPO_HORARIO { get; set; }
        public string METODO_INSTRUCCION { get; set; }
        public string ESTATUS { get; set; }
        public string FECHA_PARTE_PERIODO { get; set; }
        public string LISTA_CRUZADA { get; set; }
        public string PROFESOR_TITULAR { get; set; }
        public string PORCENTAJE_RESPONSABILIDAD { get; set; }
        public string TABULADOR_LC { get; set; }
        public string TABULADOR_POS { get; set; }
        public string ACTIVIDADES_NO_DOCENCIA { get; set; }



        public string sql;
        public string msg;
         public bool BuscaPersona()
        {
            try
            {
                sql = "SELECT * FROM QPowerBI WHERE IDBANNER = '" + IDBANNER + "' AND PERIODO = '" + PERIODO + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {

                    PERIODO = res.Get("PERIODO");
                    CAMPUS = res.Get("CAMPUS");
                    ESCUELA = res.Get("ESCUELA");
                    DESCRIPCION_ESCUELA = res.Get("DESCRIPCION_ESCUELA");
                    NRC = int.Parse(res.Get("NRC"));
                    SECCION = res.Get("SECCION");
                    MATERIA = res.Get("MATERIA");
                    CURSO = res.Get("CURSO");
                    NOMBRE_MATERIA = res.Get("NOMBRE_MATERIA");
                    TIPO_CURSO = res.Get("TIPO_CURSO");
                    TIPO_HORARIO = res.Get("TIPO_HORARIO");
                    METODO_INSTRUCCION = res.Get("METODO_INSTRUCCION");
                    ESTATUS = res.Get("ESTATUS");
                    PARTE_PERIODO = res.Get("PARTE_PERIODO");
                    FECHA_PARTE_PERIODO = res.Get("FECHA_PARTE_PERIODO");
                    LISTA_CRUZADA = res.Get("LISTA_CRUZADA");
                    IDBANNER = res.Get("IDBANNER");
                    DOCENTE = res.Get("DOCENTE");
                    RFC = res.Get("RFC");
                    CURP = res.Get("CURP");
                    MAXIMO_GRADO_ACADEMICO = res.Get("MAXIMO_GRADO_ACADEMICO");
                    TIPO_DOCENTE = res.Get("TIPO_DOCENTE");
                    PROFESOR_TITULAR = res.Get("PROFESOR_TITULAR");
                    HORAS_PROGRAMADAS = res.Get("HORAS_PROGRAMADAS").ToString();
                    NUMERO_INSCRITOS = res.GetInt("NUMERO_INSCRITOS");
                    PORCENTAJE_RESPONSABILIDAD = res.Get("PORCENTAJE_RESPONSABILIDAD").ToString();
                    FECHA_INICIO = res.Get("FECHA_INICIO");
                    FECHA_FIN = res.Get("FECHA_FIN");
                    TABULADOR_LC = res.Get("TABULADOR_LC").ToString();
                    TABULADOR_POS = res.Get("TABULADOR_LC").ToString();
                    TABULADOR_DI = res.Get("TABULADOR_DI");
                    ACTIVIDADES_NO_DOCENCIA = res.Get("TABULADOR_LC").ToString();
                    USUARIO_IMPORTA = res.Get("ADMINIMPORT");
                    USUARIO_ACTUALIZA = res.Get("ADMINUPDATE").ToString();

                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public bool getIdPersona()
        {
            try
            {
                sql = "SELECT ID_PERSONA FROM QPowerBI WHERE IDSIU = '" + IDBANNER + "' AND PERIODO = '" + PERIODO + "'";

                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    ID_PBI = res.Get("ID_PBI");
                    msg = "Éxito";

                    return true;
                }
                else
                {
                    ID_PBI = "-1";
                    msg = "Sin éxito";
                    return false;
                }
            }
            catch { return false; }
        }

        public bool Edit()
        {
            try
            {
                sql = "SELECT * FROM QPowerBI WHERE IDBANNER = '" + IDBANNER + "' AND PERIODO = '" + PERIODO + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {

                    PERIODO = res.Get("PERIODO");
                    CAMPUS = res.Get("CAMPUS");
                    ESCUELA = res.Get("ESCUELA");
                    DESCRIPCION_ESCUELA = res.Get("DESCRIPCION_ESCUELA");
                    NRC = int.Parse(res.Get("NRC"));
                    SECCION = res.Get("SECCION");
                    MATERIA = res.Get("MATERIA");
                    CURSO = res.Get("CURSO");
                    NOMBRE_MATERIA = res.Get("NOMBRE_MATERIA");
                    TIPO_CURSO = res.Get("TIPO_CURSO");
                    TIPO_HORARIO = res.Get("TIPO_HORARIO");
                    METODO_INSTRUCCION = res.Get("METODO_INSTRUCCION");
                    ESTATUS = res.Get("ESTATUS");
                    PARTE_PERIODO = res.Get("PARTE_PERIODO");
                    FECHA_PARTE_PERIODO = res.Get("FECHA_PARTE_PERIODO");
                    LISTA_CRUZADA = res.Get("LISTA_CRUZADA");
                    IDBANNER = res.Get("IDBANNER");
                    DOCENTE = res.Get("DOCENTE");
                    RFC = res.Get("RFC");
                    CURP = res.Get("CURP");
                    MAXIMO_GRADO_ACADEMICO = res.Get("MAXIMO_GRADO_ACADEMICO");
                    TIPO_DOCENTE = res.Get("TIPO_DOCENTE");
                    PROFESOR_TITULAR = res.Get("PROFESOR_TITULAR");
                    HORAS_PROGRAMADAS = res.Get("HORAS_PROGRAMADAS").ToString();
                    NUMERO_INSCRITOS = res.GetInt("NUMERO_INSCRITOS");
                    PORCENTAJE_RESPONSABILIDAD = res.Get("PORCENTAJE_RESPONSABILIDAD").ToString();
                    FECHA_INICIO = res.Get("FECHA_INICIO");
                    FECHA_FIN = res.Get("FECHA_FIN");
                    TABULADOR_LC = res.Get("TABULADOR_LC").ToString();
                    TABULADOR_POS = res.Get("TABULADOR_LC").ToString();
                    TABULADOR_DI = res.Get("TABULADOR_DI");
                    ACTIVIDADES_NO_DOCENCIA = res.Get("TABULADOR_LC").ToString();
                    USUARIO_IMPORTA = res.Get("ADMINIMPORT");
                    USUARIO_ACTUALIZA = res.Get("ADMINUPDATE").ToString();

                    return true;
                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }

    }
}