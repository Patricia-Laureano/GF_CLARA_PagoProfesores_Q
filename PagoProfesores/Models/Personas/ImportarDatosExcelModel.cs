using ConnectDB;
using Session;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Personas
{
    public class ImportarDatosExcelModel : SuperModel
    {
        public string IDSIU { get; set; }
        public string ID_PERSONA { get; set; }
        public string CVESEDE { get; set; }
        public string NOMBRES {get; set; }
        public string APELLIDOS { get; set; }
        public string SEXO { get; set; }
        public string NACIONALIDAD { get; set; }
        public string CORREO { get; set; }
        public string TELEFONO { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string DIRECCION_PAIS { get; set; }
        public string DIRECCION_ESTADO { get; set; }
        public string DIRECCION_CIUDAD { get; set; }
        public string DIRECCION_ENTIDAD { get; set; }
        public string DIRECCION_COLONIA { get; set; }
        public string DIRECCION_CALLE { get; set; }
        public string DIRECCION_CP { get; set; }
        public string CVE_ORIGEN { get; set; }
        public string USUARIO { get; set; }
        public string CVE_TIPODEPAGO { get; set; }
        public string CVE_TITULOPROFESIONAL { get; set; }
        public string TITULOPROFESIONAL { get; set; }
        public string CVE_PROFESION { get; set; }
        public string PROFESION { get; set; }
        public string CEDULAPROFESIONAL { get; set; }
        public string SEGUROSOCIAL { get; set; }
        public string CORREO365 { get; set; }
        public string TITULO_LICENCIATURA { get; set; }
        public string TITULO_MAESTRIA { get; set; }
        public string AREAASIGNACION { get; set; }
        public string TITULO_DOCTORADO { get; set; }
        public string FI_DIRECCION_PAIS { get; set; }
        public string FI_DIRECCION_ESTADO { get; set; }
        public string FI_DIRECCION_CIUDAD { get; set; }
        public string FI_DIRECCION_ENTIDAD { get; set; }
        public string FI_DIRECCION_COLONIA { get; set; }
        public string FI_DIRECCION_CALLE { get; set; }
        public string FI_DIRECCION_CP { get; set; }
        public string FECHACEDULA { get; set; }
        public string FECHANACIMIENTO { get; set; }
        public string REGISTRADO { get; set; }
        public string IMPORTADO { get; set; }


        public string FECHA_R { get; set; }
        char[] ERRORES;

        public Dictionary<CARGARDATOSEXCEL, bool> validCells;

        public bool saved;
        public string sql;
        public string sql1;
        public bool TMP = false;
        public string TABLE = "PERSONAS";

        public bool clean()
        {
            string sql = "DELETE FROM PERSONAS_TMP WHERE USUARIO = '" + sesion.pkUser + "'";
            return db.execute(sql);
        }
        public enum CARGARDATOSEXCEL
        {
            IDSIU,
            ID_PERSONA,
            CVESEDE,
            NOMBRES,
            APELLIDOS,
            SEXO,
            NACIONALIDAD,
            CORREO,
            TELEFONO,
            RFC,
            CURP,
            DIRECCION_PAIS,
            DIRECCION_ESTADO,
            DIRECCION_CIUDAD,
            DIRECCION_ENTIDAD,
            DIRECCION_COLONIA,
            DIRECCION_CALLE,
            DIRECCION_CP,
            CVE_TITULOPROFESIONAL,
            TITULOPROFESIONAL,
            CVE_PROFESION,
            PROFESION,
            CEDULAPROFESIONAL,
            SEGUROSOCIAL,
            AREAASIGNACION,
            CORREO365,
            TITULO_LICENCIATURA,
            TITULO_MAESTRIA,
            TITULO_DOCTORADO,
            FI_DIRECCION_PAIS,
            FI_DIRECCION_ESTADO,
            FI_DIRECCION_CIUDAD,
            FI_DIRECCION_ENTIDAD,
            FI_DIRECCION_COLONIA,
            FI_DIRECCION_CALLE,
            FI_DIRECCION_CP,
            FECHACEDULA,
            FECHANACIMIENTO,
            CVE_TIPODEPAGO,
            USUARIO,
            CVE_ORIGEN,
            REGISTRADO,
            IMPORTADO,
            FECHA_R
            

        };

        public bool validate()
        {
            validCells = new Dictionary<CARGARDATOSEXCEL, bool>();
            foreach (CARGARDATOSEXCEL item in Enum.GetValues(typeof(CARGARDATOSEXCEL)))
                validCells[item] = true;

            validCells[CARGARDATOSEXCEL.IDSIU] = (IDSIU.Length == 10);
            validCells[CARGARDATOSEXCEL.CVESEDE] = (CVESEDE.Length == 5);
            validCells[CARGARDATOSEXCEL.NOMBRES] = (NOMBRES.Length == 50);
            validCells[CARGARDATOSEXCEL.APELLIDOS] = (APELLIDOS.Length == 50);
            validCells[CARGARDATOSEXCEL.SEXO] = (SEXO.Length == 1);
            validCells[CARGARDATOSEXCEL.NACIONALIDAD] = (NACIONALIDAD.Length == 30);
            validCells[CARGARDATOSEXCEL.CORREO] = (CORREO.Length == 50);
            validCells[CARGARDATOSEXCEL.TELEFONO] = (TELEFONO.Length == 30);
            validCells[CARGARDATOSEXCEL.RFC] = (RFC.Length == 20);
            validCells[CARGARDATOSEXCEL.CURP] = (CURP.Length == 30);
            validCells[CARGARDATOSEXCEL.DIRECCION_PAIS] = (DIRECCION_PAIS.Length == 50);
            validCells[CARGARDATOSEXCEL.DIRECCION_ESTADO] = (DIRECCION_ESTADO.Length == 50);
            validCells[CARGARDATOSEXCEL.DIRECCION_CIUDAD] = (DIRECCION_CIUDAD.Length == 50);
            validCells[CARGARDATOSEXCEL.DIRECCION_ENTIDAD] = (DIRECCION_ENTIDAD.Length == 50);
            validCells[CARGARDATOSEXCEL.DIRECCION_COLONIA] = (DIRECCION_COLONIA.Length == 50);
            validCells[CARGARDATOSEXCEL.DIRECCION_CALLE] = (DIRECCION_CALLE.Length == 250);
            validCells[CARGARDATOSEXCEL.DIRECCION_CP] = (DIRECCION_CP.Length == 10);
            validCells[CARGARDATOSEXCEL.CVE_TITULOPROFESIONAL] = (CVE_TITULOPROFESIONAL.Length == 50);
            validCells[CARGARDATOSEXCEL.TITULOPROFESIONAL] = (TITULOPROFESIONAL.Length == 150);
            validCells[CARGARDATOSEXCEL.CVE_PROFESION] = (CVE_PROFESION.Length == 50);
            validCells[CARGARDATOSEXCEL.PROFESION] = (PROFESION.Length == 150);
            validCells[CARGARDATOSEXCEL.CEDULAPROFESIONAL] = (CEDULAPROFESIONAL.Length == 100);
            validCells[CARGARDATOSEXCEL.SEGUROSOCIAL] = (SEGUROSOCIAL.Length == 11);
            validCells[CARGARDATOSEXCEL.AREAASIGNACION] = (AREAASIGNACION.Length == 30);
            validCells[CARGARDATOSEXCEL.CORREO365] = (CORREO365.Length == 60);
            validCells[CARGARDATOSEXCEL.TITULO_LICENCIATURA] = (TITULO_LICENCIATURA.Length == 30);
            validCells[CARGARDATOSEXCEL.TITULO_MAESTRIA] = (TITULO_MAESTRIA.Length == 30);
            validCells[CARGARDATOSEXCEL.TITULO_DOCTORADO] = (TITULO_DOCTORADO.Length == 30);
            validCells[CARGARDATOSEXCEL.FI_DIRECCION_PAIS] = (FI_DIRECCION_PAIS.Length == 50);
            validCells[CARGARDATOSEXCEL.FI_DIRECCION_ESTADO] = (FI_DIRECCION_ESTADO.Length == 0);
            validCells[CARGARDATOSEXCEL.FI_DIRECCION_CIUDAD] = (FI_DIRECCION_CIUDAD.Length == 50);
            validCells[CARGARDATOSEXCEL.FI_DIRECCION_ENTIDAD] = (FI_DIRECCION_ENTIDAD.Length == 50);
            validCells[CARGARDATOSEXCEL.FI_DIRECCION_COLONIA] = (FI_DIRECCION_COLONIA.Length == 50);
            validCells[CARGARDATOSEXCEL.FI_DIRECCION_CALLE] = (FI_DIRECCION_CALLE.Length == 250);
            validCells[CARGARDATOSEXCEL.FI_DIRECCION_CP] = (FI_DIRECCION_CP.Length == 10);
            validCells[CARGARDATOSEXCEL.FECHACEDULA] = true;
            validCells[CARGARDATOSEXCEL.FECHANACIMIENTO] = true;
            validCells[CARGARDATOSEXCEL.CVE_TIPODEPAGO] = true;
            foreach (CARGARDATOSEXCEL item in Enum.GetValues(typeof(CARGARDATOSEXCEL)))
                if (validCells[item] == false)
                    return false;
            return true;
        }

        public object[] getArrayObject(Dictionary<CARGARDATOSEXCEL, object> dict)
        {
            dict[CARGARDATOSEXCEL.IDSIU] = IDSIU;
            dict[CARGARDATOSEXCEL.CVESEDE] = CVESEDE;
            dict[CARGARDATOSEXCEL.NOMBRES] = NOMBRES;
            dict[CARGARDATOSEXCEL.APELLIDOS] = APELLIDOS;
            dict[CARGARDATOSEXCEL.SEXO] = SEXO;
            dict[CARGARDATOSEXCEL.NACIONALIDAD] = NACIONALIDAD;
            dict[CARGARDATOSEXCEL.CORREO] = CORREO;
            dict[CARGARDATOSEXCEL.TELEFONO] = TELEFONO;
            dict[CARGARDATOSEXCEL.RFC] = RFC;
            dict[CARGARDATOSEXCEL.CURP] = CURP;
            dict[CARGARDATOSEXCEL.DIRECCION_PAIS] = DIRECCION_PAIS;
            dict[CARGARDATOSEXCEL.DIRECCION_ESTADO] = DIRECCION_ESTADO;
            dict[CARGARDATOSEXCEL.DIRECCION_CIUDAD] = DIRECCION_CIUDAD;
            dict[CARGARDATOSEXCEL.DIRECCION_ENTIDAD] = DIRECCION_ENTIDAD;
            dict[CARGARDATOSEXCEL.DIRECCION_COLONIA] = DIRECCION_COLONIA;
            dict[CARGARDATOSEXCEL.DIRECCION_CALLE] = DIRECCION_CALLE;
            dict[CARGARDATOSEXCEL.DIRECCION_CP] = DIRECCION_CP;
            dict[CARGARDATOSEXCEL.CVE_TITULOPROFESIONAL] = CVE_TITULOPROFESIONAL;
            dict[CARGARDATOSEXCEL.TITULOPROFESIONAL] = TITULOPROFESIONAL;
            dict[CARGARDATOSEXCEL.CVE_PROFESION] = CVE_PROFESION;
            dict[CARGARDATOSEXCEL.PROFESION] = PROFESION;
            dict[CARGARDATOSEXCEL.CEDULAPROFESIONAL] = CEDULAPROFESIONAL;
            dict[CARGARDATOSEXCEL.SEGUROSOCIAL] = SEGUROSOCIAL;
            dict[CARGARDATOSEXCEL.AREAASIGNACION] = AREAASIGNACION;
            dict[CARGARDATOSEXCEL.CORREO365] = CORREO365;
            dict[CARGARDATOSEXCEL.TITULO_LICENCIATURA] = TITULO_LICENCIATURA;
            dict[CARGARDATOSEXCEL.TITULO_MAESTRIA] = TITULO_MAESTRIA;
            dict[CARGARDATOSEXCEL.TITULO_DOCTORADO] = TITULO_DOCTORADO;
            dict[CARGARDATOSEXCEL.FI_DIRECCION_PAIS] = FI_DIRECCION_PAIS;
            dict[CARGARDATOSEXCEL.FI_DIRECCION_ESTADO] = FI_DIRECCION_ESTADO;
            dict[CARGARDATOSEXCEL.FI_DIRECCION_CIUDAD] = FI_DIRECCION_CIUDAD;
            dict[CARGARDATOSEXCEL.FI_DIRECCION_ENTIDAD] = FI_DIRECCION_ENTIDAD;
            dict[CARGARDATOSEXCEL.FI_DIRECCION_COLONIA] = FI_DIRECCION_COLONIA;
            dict[CARGARDATOSEXCEL.FI_DIRECCION_CALLE] = FI_DIRECCION_CALLE;
            dict[CARGARDATOSEXCEL.FI_DIRECCION_CP] = FI_DIRECCION_CP;
            dict[CARGARDATOSEXCEL.FECHACEDULA] = __validDateTime(FECHACEDULA);
            dict[CARGARDATOSEXCEL.FECHANACIMIENTO] = __validDateTime(FECHANACIMIENTO);
            dict[CARGARDATOSEXCEL.CVE_TIPODEPAGO] = CVE_TIPODEPAGO;
            dict[CARGARDATOSEXCEL.FECHA_R] = __validDateTime(FECHA_R);            
            dict[CARGARDATOSEXCEL.USUARIO] = sesion.nickName;
            dict[CARGARDATOSEXCEL.ID_PERSONA] = -1;
            dict[CARGARDATOSEXCEL.REGISTRADO] = 0;
            dict[CARGARDATOSEXCEL.IMPORTADO] = 0;
            dict[CARGARDATOSEXCEL.CVE_ORIGEN] = "X";
  
            return dict.Values.ToArray();
        }

        public char[] getArrayChars()
        {
            Dictionary<CARGARDATOSEXCEL, char> dict = new Dictionary<CARGARDATOSEXCEL, char>();
            dict[CARGARDATOSEXCEL.IDSIU] = validCells[CARGARDATOSEXCEL.IDSIU] ? '1' : '0';
            dict[CARGARDATOSEXCEL.CVESEDE] = validCells[CARGARDATOSEXCEL.CVESEDE] ? '1' : '0'; 
            dict[CARGARDATOSEXCEL.NOMBRES] = validCells[CARGARDATOSEXCEL.NOMBRES] ? '1' : '0';
            dict[CARGARDATOSEXCEL.APELLIDOS] = validCells[CARGARDATOSEXCEL.APELLIDOS] ? '1' : '0';
            dict[CARGARDATOSEXCEL.SEXO] = validCells[CARGARDATOSEXCEL.SEXO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.NACIONALIDAD] = validCells[CARGARDATOSEXCEL.NACIONALIDAD] ? '1' : '0';
            dict[CARGARDATOSEXCEL.CORREO] = validCells[CARGARDATOSEXCEL.CORREO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.TELEFONO] = validCells[CARGARDATOSEXCEL.TELEFONO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.RFC] = validCells[CARGARDATOSEXCEL.RFC] ? '1' : '0';
            dict[CARGARDATOSEXCEL.CURP] = validCells[CARGARDATOSEXCEL.CURP] ? '1' : '0';
            dict[CARGARDATOSEXCEL.DIRECCION_PAIS] = validCells[CARGARDATOSEXCEL.DIRECCION_PAIS] ? '1' : '0';
            dict[CARGARDATOSEXCEL.DIRECCION_ESTADO] = validCells[CARGARDATOSEXCEL.DIRECCION_ESTADO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.DIRECCION_CIUDAD] = validCells[CARGARDATOSEXCEL.DIRECCION_CIUDAD] ? '1' : '0';
            dict[CARGARDATOSEXCEL.DIRECCION_ENTIDAD] = validCells[CARGARDATOSEXCEL.DIRECCION_ENTIDAD] ? '1' : '0';
            dict[CARGARDATOSEXCEL.DIRECCION_COLONIA] = validCells[CARGARDATOSEXCEL.DIRECCION_COLONIA] ? '1' : '0';
            dict[CARGARDATOSEXCEL.DIRECCION_CALLE] = validCells[CARGARDATOSEXCEL.DIRECCION_CALLE] ? '1' : '0';
            dict[CARGARDATOSEXCEL.DIRECCION_CP] = validCells[CARGARDATOSEXCEL.DIRECCION_CP] ? '1' : '0';
            dict[CARGARDATOSEXCEL.CVE_TITULOPROFESIONAL] = validCells[CARGARDATOSEXCEL.CVE_TITULOPROFESIONAL] ? '1' : '0';
            dict[CARGARDATOSEXCEL.TITULOPROFESIONAL] = validCells[CARGARDATOSEXCEL.TITULOPROFESIONAL] ? '1' : '0';
            dict[CARGARDATOSEXCEL.CVE_PROFESION] = validCells[CARGARDATOSEXCEL.CVE_PROFESION] ? '1' : '0';
            dict[CARGARDATOSEXCEL.PROFESION] = validCells[CARGARDATOSEXCEL.PROFESION] ? '1' : '0';
            dict[CARGARDATOSEXCEL.CEDULAPROFESIONAL] = validCells[CARGARDATOSEXCEL.CEDULAPROFESIONAL] ? '1' : '0';
            dict[CARGARDATOSEXCEL.SEGUROSOCIAL] = validCells[CARGARDATOSEXCEL.SEGUROSOCIAL] ? '1' : '0';
            dict[CARGARDATOSEXCEL.AREAASIGNACION] = validCells[CARGARDATOSEXCEL.AREAASIGNACION] ? '1' : '0';
            dict[CARGARDATOSEXCEL.CORREO365] = validCells[CARGARDATOSEXCEL.CORREO365] ? '1' : '0';
            dict[CARGARDATOSEXCEL.TITULO_LICENCIATURA] = validCells[CARGARDATOSEXCEL.TITULO_LICENCIATURA] ? '1' : '0';
            dict[CARGARDATOSEXCEL.TITULO_MAESTRIA] = validCells[CARGARDATOSEXCEL.TITULO_MAESTRIA] ? '1' : '0';
            dict[CARGARDATOSEXCEL.TITULO_DOCTORADO] = validCells[CARGARDATOSEXCEL.TITULO_DOCTORADO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.FI_DIRECCION_PAIS] = validCells[CARGARDATOSEXCEL.FI_DIRECCION_PAIS] ? '1' : '0';
            dict[CARGARDATOSEXCEL.FI_DIRECCION_ESTADO] = validCells[CARGARDATOSEXCEL.FI_DIRECCION_ESTADO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.FI_DIRECCION_CIUDAD] = validCells[CARGARDATOSEXCEL.FI_DIRECCION_CIUDAD] ? '1' : '0';
            dict[CARGARDATOSEXCEL.FI_DIRECCION_ENTIDAD] = validCells[CARGARDATOSEXCEL.FI_DIRECCION_ENTIDAD] ? '1' : '0';
            dict[CARGARDATOSEXCEL.FI_DIRECCION_COLONIA] = validCells[CARGARDATOSEXCEL.FI_DIRECCION_COLONIA] ? '1' : '0';
            dict[CARGARDATOSEXCEL.FI_DIRECCION_CALLE] = validCells[CARGARDATOSEXCEL.FI_DIRECCION_CALLE] ? '1' : '0';
            dict[CARGARDATOSEXCEL.FI_DIRECCION_CP] = validCells[CARGARDATOSEXCEL.FI_DIRECCION_CP] ? '1' : '0';
            dict[CARGARDATOSEXCEL.FECHACEDULA] = validCells[CARGARDATOSEXCEL.FECHACEDULA] ? '1' : '0';
            dict[CARGARDATOSEXCEL.FECHANACIMIENTO] = validCells[CARGARDATOSEXCEL.FECHANACIMIENTO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.CVE_TIPODEPAGO] = validCells[CARGARDATOSEXCEL.CVE_TIPODEPAGO] ? '1' : '0'; 
            dict[CARGARDATOSEXCEL.FECHA_R] = validCells[CARGARDATOSEXCEL.FECHA_R] ? '1' : '0';            
            dict[CARGARDATOSEXCEL.USUARIO] = validCells[CARGARDATOSEXCEL.USUARIO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.ID_PERSONA] = validCells[CARGARDATOSEXCEL.ID_PERSONA] ? '1' : '0';
            dict[CARGARDATOSEXCEL.REGISTRADO] = validCells[CARGARDATOSEXCEL.REGISTRADO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.IMPORTADO] = validCells[CARGARDATOSEXCEL.IMPORTADO] ? '1' : '0';
            dict[CARGARDATOSEXCEL.CVE_ORIGEN] = validCells[CARGARDATOSEXCEL.CVE_ORIGEN] ? '1' : '0';
            return dict.Values.ToArray();
        }

        public void FindFirstError(out int total, out int maxErrors, out string idsError)
        {
            maxErrors = 0;
            idsError = "";
            string sql;

            // Contar registros auditados
            sql = "SELECT COUNT(*) AS 'MAX' FROM PERSONAS_TMP WHERE USUARIO = " + sesion.pkUser;
            total = db.Count(sql);

            // Contar errores
            string correcto = "";
            for (int i = 0; i < Enum.GetValues(typeof(CARGARDATOSEXCEL)).Length; i++)
                correcto += '1';
            sql = "SELECT COUNT(*) AS 'MAX' FROM PERSONAS_TMP WHERE USUARIO = " + sesion.pkUser + " AND ERRORES <> '" + correcto + "'";
            maxErrors = db.Count(sql);

            // Consultar primeros 5 errores.
            sql = "SELECT TOP 5 * FROM PERSONAS_TMP WHERE USUARIO = " + sesion.pkUser + " AND ERRORES <> '" + correcto + "'";
            idsError = "";
            ResultSet res = db.getTable(sql);
            List<string> list = new List<string>();
            while (res.Next())
                list.Add(res.Get("IDSIU"));
            idsError = string.Join(",", list);
        }

        public bool Add_TMP()
        {
            try
            {
                ERRORES = getArrayChars();
                Dictionary<string, string> dic = prepareData(true, true);
                sql = "INSERT INTO"
                    + " PERSONAS_TMP (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
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
                dic.Add("IDSIU", IDSIU);
                dic.Add("CVESEDE", CVESEDE);
                dic.Add("NOMBRES", NOMBRES);
                dic.Add("APELLIDOS", APELLIDOS);
                dic.Add("SEXO", SEXO);
                dic.Add("NACIONALIDAD", NACIONALIDAD);
                dic.Add("CORREO", CORREO);
                dic.Add("TELEFONO", TELEFONO);
                dic.Add("RFC", RFC);
                dic.Add("CURP", CURP);
                dic.Add("DIRECCION_PAIS", DIRECCION_PAIS);
                dic.Add("DIRECCION_ESTADO", DIRECCION_ESTADO);
                dic.Add("DIRECCION_CIUDAD", DIRECCION_CIUDAD);
                dic.Add("DIRECCION_ENTIDAD", DIRECCION_ENTIDAD);
                dic.Add("DIRECCION_COLONIA", DIRECCION_COLONIA);
                dic.Add("DIRECCION_CALLE", DIRECCION_CALLE);
                dic.Add("DIRECCION_CP", DIRECCION_CP);
                dic.Add("CVE_TITULOPROFESIONAL", CVE_TITULOPROFESIONAL);
                dic.Add("TITULOPROFESIONAL", TITULOPROFESIONAL);
                dic.Add("CVE_PROFESION", CVE_PROFESION);
                dic.Add("PROFESION", PROFESION);
                dic.Add("CEDULAPROFESIONAL", CEDULAPROFESIONAL);
                dic.Add("SEGUROSOCIAL", SEGUROSOCIAL);
                dic.Add("AREAASIGNACION", AREAASIGNACION);
                dic.Add("CORREO365", CORREO365);
                dic.Add("TITULO_LICENCIATURA", TITULO_LICENCIATURA);
                dic.Add("TITULO_MAESTRIA", TITULO_MAESTRIA);
                dic.Add("TITULO_DOCTORADO", TITULO_DOCTORADO);
                dic.Add("FI_DIRECCION_PAIS", FI_DIRECCION_PAIS);
                dic.Add("FI_DIRECCION_ESTADO", FI_DIRECCION_ESTADO);
                dic.Add("FI_DIRECCION_CIUDAD", FI_DIRECCION_CIUDAD);
                dic.Add("FI_DIRECCION_ENTIDAD", FI_DIRECCION_ENTIDAD);
                dic.Add("FI_DIRECCION_COLONIA", FI_DIRECCION_COLONIA);
                dic.Add("FI_DIRECCION_CALLE", FI_DIRECCION_CALLE);
                dic.Add("FI_DIRECCION_CP", FI_DIRECCION_CP);
                dic.Add("FECHACEDULA", __validDateTime(FECHACEDULA));
                dic.Add("FECHANACIMIENTO", __validDateTime(FECHANACIMIENTO)); 
                dic.Add("CVE_TIPODEPAGO", CVE_TIPODEPAGO);
                dic.Add("FECHA_R", FECHA);
                dic.Add("USUARIO", sesion.pkUser.ToString());
                dic.Add("ID_PERSONA", ID_PERSONA);
                dic.Add("REGISTRADO", "0");
                dic.Add("IMPORTADO", "0");
                dic.Add("CVE_ORIGEN", "X");
                
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

        public bool exist_TMP()
        {
            bool resultado = false;
            this.TMP = false;
            try
            {
                string sql = "SELECT TOP 1 P.ID_PERSONA    " +
                             "  FROM QPersonasTMP P INNER JOIN " +
                             "       PERSONAS_SEDES S ON S.ID_PERSONA = P.ID_PERSONA " +
                             " WHERE IDSIU            = '" + IDSIU + "' " +
                             "   AND S.CVE_SEDE       = '" + CVESEDE + "';";

                ResultSet res = db.getTable(sql);
                if (!res.Next())
                {
                    ID_PERSONA = "-1";
                }
                else
                {
                    ID_PERSONA = res.Get("IDSIU");
                }
                resultado = true;
            }
            catch (Exception ex)
            {

            }
            return resultado;
        }
        public bool exist()
        {
            bool resultado = false;
            this.TMP = false;
            try
            {

                CVESEDE = sesion.vdata["CVESEDE"].ToString();
                IDSIU = sesion.vdata["IDSIU"].ToString();

                TABLE = TMP ? "QPersonasTMP" : "PERSONAS";

                string sql = "SELECT TOP 1 P.ID_PERSONA    " +
                             "  FROM " + TABLE + " P INNER JOIN " +
                             "       PERSONAS_SEDES S ON S.ID_PERSONA = P.ID_PERSONA " +
                             " WHERE IDSIU            = '" + IDSIU + "' " +
                             "   AND S.CVE_SEDE       = '" + CVESEDE + "' " + (TMP ? " and U.USUARIO = '" + sesion.pkUser + "' " : "");

                ResultSet res = db.getTable(sql); 
                if (res.Next())
                {
                    resultado = UpdateImportarDatosExcel();
                }
                else
                {
                    resultado= add();
                }

                return resultado;
            }
            catch(Exception ex)
            {
                return resultado;
            }
         
        }

        public bool UpdateImportarDatosExcel()
        {

            bool resultado = false;
            try
            {

                CVESEDE = sesion.vdata["CVESEDE"].ToString();
                IDSIU = sesion.vdata["IDSIU"].ToString();

                string sql = "SELECT *  FROM PERSONAS_TMP WHERE IDSIU = '" + IDSIU + "' " +
                            "   AND CVESEDE = '" + CVESEDE + "' AND USUARIO = '" + sesion.pkUser + "'";

                ResultSet res = db.getTable(sql);
                if (res.Next())
                {

                    CVE_TIPODEPAGO = res.Get("CVE_TIPODEPAGO");
                    

                    TABLE = TMP ? "PERSONAS_TMP" : "PERSONAS";
                     sql = "UPDATE " + TABLE + " SET " +
                        " APELLIDOS             = '" + res.Get("APELLIDOS") + "'" +
                        ",NOMBRES               = '" + res.Get("NOMBRES") + "'" +
                        ",SEXO                  = '" + res.Get("SEXO") + "'" +
                        ",FECHANACIMIENTO       = '" + __validDateTime(res.Get("FECHANACIMIENTO")) + "'" +
                        ",NACIONALIDAD          = '" + res.Get("NACIONALIDAD") + "'" +
                        ",CORREO                = '" + res.Get("CORREO") + "'" +
                        ",TELEFONO              = '" + res.Get("TELEFONO") + "'" +
                        ",RFC                   = '" + res.Get("RFC") + "'" +
                        ",CURP                  = '" + res.Get("CURP") + "'" +
                        ",DIRECCION_PAIS        = '" + res.Get("DIRECCION_PAIS") + "'" +
                        ",DIRECCION_ESTADO      = '" + res.Get("DIRECCION_ESTADO") + "'" +
                        ",DIRECCION_CIUDAD      = '" + res.Get("DIRECCION_CIUDAD") + "'" +
                        ",DIRECCION_ENTIDAD     = '" + res.Get("DIRECCION_ENTIDAD") + "'" +
                        ",DIRECCION_COLONIA     = '" + res.Get("DIRECCION_COLONIA") + "'" +
                        ",DIRECCION_CALLE       = '" + res.Get("DIRECCION_CALLE") + "'" +
                        ",DIRECCION_CP          = '" + res.Get("DIRECCION_CP") + "'" +
                        ",CVE_TITULOPROFESIONAL = '" + res.Get("CVE_TITULOPROFESIONAL") + "'" +
                        ",TITULOPROFESIONAL     = '" + res.Get("TITULOPROFESIONAL") + "'" +
                        ",CVE_PROFESION         = '" + res.Get("CVE_PROFESION") + "'" +
                        ",PROFESION             = '" + res.Get("PROFESION") + "'" +
                        ",CEDULAPROFESIONAL     = '" + res.Get("CEDULAPROFESIONAL") + "'" +
                        ",FECHACEDULA           = '" + __validDateTime(res.Get("FECHACEDULA")) + "'" +
                        ",SEGUROSOCIAL          = '" + res.Get("SEGUROSOCIAL") + "'" +
                        ",CORREO365             = '" + res.Get("CORREO365") + "'" +
                        ",TITULO_LICENCIATURA   = '" + res.Get("TITULO_LICENCIATURA") + "'" +
                        ",TITULO_MAESTRIA       = '" + res.Get("TITULO_MAESTRIA") + "'" +
                        ",TITULO_DOCTORADO      = '" + res.Get("TITULO_DOCTORADO") + "'" +
                        ",AREAASIGNACION        = '" + res.Get("AREAASIGNACION") + "'" +
                        ",FECHA_M               = GETDATE() " +
                        ",FI_DIRECCION_PAIS        = '" + res.Get("FI_DIRECCION_PAIS") + "'" +
                        ",FI_DIRECCION_ESTADO      = '" + res.Get("FI_DIRECCION_ESTADO") + "'" +
                        ",FI_DIRECCION_CIUDAD      = '" + res.Get("FI_DIRECCION_CIUDAD") + "'" +
                        ",FI_DIRECCION_ENTIDAD     = '" + res.Get("FI_DIRECCION_ENTIDAD") + "'" +
                        ",FI_DIRECCION_COLONIA     = '" + res.Get("FI_DIRECCION_COLONIA") + "'" +
                        ",FI_DIRECCION_CALLE       = '" + res.Get("FI_DIRECCION_CALLE") + "'" +
                        ",FI_DIRECCION_CP          = '" + res.Get("FI_DIRECCION_CP") + "'" +
                        ",USUARIO                 =  '" + sesion.pkUser + "'" +
                        ",CVE_TIPODEPAGO          =  '" + res.Get("CVE_TIPODEPAGO") + "'" +

                        " WHERE IDSIU           = '" + IDSIU + "'";

                    Log.write(this, "PERSONAS", LOG.REGISTRO, ("sql: ") + sql, this.sesion);
                }
                if (db.execute(sql))
                {
                    sql = "SELECT P.ID_PERSONA FROM PERSONAS_SEDES ps INNER JOIN PERSONAS p ON ps.ID_PERSONA=p.ID_PERSONA WHERE IDSIU='" + IDSIU + "' AND CVE_SEDE='"+ CVESEDE + "'";

                    ResultSet res2 = db.getTable(sql);
                    if (res2.Next())
                    {
                        ID_PERSONA = res2.Get("ID_PERSONA");

                    }

                    if (ID_PERSONA != null)
                    {
                        sql = "";
                        sql = "UPDATE PERSONAS_SEDES SET  CVE_TIPODEPAGO='" + CVE_TIPODEPAGO + "', FECHA_R=GETDATE(), USUARIO='" + sesion.pkUser.ToString() + "' WHERE ID_PERSONA=" + ID_PERSONA + ";";
                        Log.write(this, "PERSONAS_SEDES", LOG.REGISTRO, ("sql: ") + sql, this.sesion);
                        resultado = db.execute(sql);

                        sql = "";
                        sql = "UPDATE PERSONAS_TMP SET REGISTRADO=1 WHERE IDSIU='" + IDSIU + "';";
                        Log.write(this, "PERSONAS_TMP", LOG.REGISTRO, ("sql: ") + sql, this.sesion);
                        resultado = db.execute(sql);
                    }
                   
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return resultado;
        }
        public bool add()
        {
            bool resultado = false;

            try
            {

                CVESEDE = sesion.vdata["CVESEDE"].ToString();

                string sql = "SELECT *  FROM PERSONAS_TMP WHERE IDSIU            = '" + sesion.vdata["IDSIU"].ToString() + "' " +
                            "   AND CVESEDE       = '" + CVESEDE + "' AND USUARIO = '" + sesion.pkUser + "'";

                ResultSet res = db.getTable(sql);
                if (res.Next())
                {

                    FECHACEDULA = res.Get("FECHACEDULA");
                    FECHANACIMIENTO =res.Get("FECHANACIMIENTO");
                    CVE_TIPODEPAGO = res.Get("CVE_TIPODEPAGO");

                    string[] VALUES = {
                        res.Get("IDSIU")
                        ,valid(res.Get("APELLIDOS"))
                        ,valid(res.Get("NOMBRES"))
                        ,res.Get("SEXO")                      
                        ,res.Get("NACIONALIDAD")
                        ,res.Get("CORREO")
                        ,res.Get("TELEFONO")
                        ,res.Get("RFC")
                        ,res.Get("CURP")
                        ,valid(res.Get("DIRECCION_PAIS"))
                        ,valid(res.Get("DIRECCION_ESTADO"))
                        ,valid(res.Get("DIRECCION_CIUDAD"))
                        ,valid(res.Get("DIRECCION_ENTIDAD"))
                        ,valid(res.Get("DIRECCION_COLONIA"))
                        ,valid(res.Get("DIRECCION_CALLE"))
                        ,res.Get("DIRECCION_CP")
                        ,"X"
                        ,res.Get("USUARIO")
                        ,res.Get("CVE_TITULOPROFESIONAL")
                        ,valid(res.Get("TITULOPROFESIONAL"))
                        ,res.Get("CVE_PROFESION")
                        ,valid(res.Get("PROFESION"))
                        ,res.Get("CEDULAPROFESIONAL")
                        ,res.Get("SEGUROSOCIAL")
                        ,res.Get("CORREO365")
                        ,res.Get("TITULO_LICENCIATURA")
                        ,res.Get("TITULO_MAESTRIA")
                        ,res.Get("AREAASIGNACION")
                        ,res.Get("TITULO_DOCTORADO")
                        ,valid(res.Get("FI_DIRECCION_PAIS"))
                        ,valid(res.Get("FI_DIRECCION_ESTADO"))
                        ,valid(res.Get("FI_DIRECCION_CIUDAD"))
                        ,valid(res.Get("FI_DIRECCION_ENTIDAD"))
                        ,valid(res.Get("FI_DIRECCION_COLONIA"))
                        ,valid(res.Get("FI_DIRECCION_CALLE"))
                        ,res.Get("FI_DIRECCION_CP")
                        ,CVE_TIPODEPAGO
                    };
            
                    sql = "";
                     sql = "INSERT INTO PERSONAS" +
                        " (IDSIU,APELLIDOS,NOMBRES,SEXO,NACIONALIDAD,CORREO,TELEFONO,RFC,CURP,DIRECCION_PAIS,DIRECCION_ESTADO,DIRECCION_CIUDAD,DIRECCION_ENTIDAD,DIRECCION_COLONIA,DIRECCION_CALLE,DIRECCION_CP,CVE_ORIGEN,USUARIO,CVE_TITULOPROFESIONAL,TITULOPROFESIONAL,CVE_PROFESION,PROFESION,CEDULAPROFESIONAL,SEGUROSOCIAL,CORREO365,TITULO_LICENCIATURA,TITULO_MAESTRIA,AREAASIGNACION,TITULO_DOCTORADO" +
                    " ,FI_DIRECCION_PAIS,FI_DIRECCION_ESTADO,FI_DIRECCION_CIUDAD,FI_DIRECCION_ENTIDAD,FI_DIRECCION_COLONIA,FI_DIRECCION_CALLE,FI_DIRECCION_CP,CVE_TIPODEPAGO" +
                     ",FECHA_R,FECHACEDULA,FECHANACIMIENTO)" +
                        " VALUES ('" + string.Join("','", VALUES) + "', GETDATE(), '" + __validDateTime(FECHACEDULA) + "','" + __validDateTime(FECHANACIMIENTO) + "')";

                    Log.write(this, "PERSONAS", LOG.REGISTRO, ("sql: ") + sql, this.sesion);

                    ID_PERSONA = db.executeId(sql);
                }
                if (ID_PERSONA != null && ID_PERSONA != string.Empty)
                {
                    sql = "INSERT INTO PERSONAS_SEDES (ID_PERSONA, CVE_SEDE, CVE_TIPODEPAGO, FECHA_R, USUARIO) " +
                          "                    VALUES (" + ID_PERSONA + ", '" + CVESEDE + "', '" + CVE_TIPODEPAGO + "', GETDATE(), '" + sesion.pkUser.ToString() + "')";

                    Log.write(this, "PERSONAS_SEDES", LOG.REGISTRO, ("sql: ") + sql, this.sesion);

                    resultado = db.execute(sql);

                    sql = "";
                    sql = "UPDATE PERSONAS_TMP SET REGISTRADO=1 WHERE IDSIU='" + IDSIU + "';";
                    Log.write(this, "PERSONAS_TMP", LOG.REGISTRO, ("sql: ") + sql, this.sesion);
                    resultado = db.execute(sql);

                    
                }
            }
            catch(Exception ex)
            {
                resultado = true;
            }

            return resultado;
                 
            
        }

    }
}