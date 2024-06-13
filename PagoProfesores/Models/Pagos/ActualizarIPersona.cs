using ConnectDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{

    interface ActualizarIPersona
    {
        Dictionary<string, string> prepareDataPersona(bool add, object obj);
        long findPersona(bool TMP = false);
        bool addPersona(bool TMP = false);
        bool savePersona(bool TMP = false);
        bool editPersona(bool TMP = false);
        bool markPersona();
        bool CleanPersona();
    }

    public partial class ActualizaciondePAModel : SuperModel, ActualizarIPersona
    {
        public long PK_PERSONA;
        public string PERSONA_REGISTRADA = "0";
        //public string PERSONA_REGISTRADA;
        public string xQuery { get; set; }
        public string xErrMsg { get; set; }

        public Dictionary<string, string> prepareDataPersona(bool add, object obj)
        {
            bool TMP = (bool)obj;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            if (add)
            {
                dict.Add("IDSIU", IDSIU);
                if (TMP)
                    dict.Add("ID_PERSONA", PK_PERSONA.ToString());
            }

            dict.Add("APELLIDOS", valid(APELLIDOS));
            dict.Add("NOMBRES", valid(NOMBRES));
            dict.Add("SEXO", SEXO);
            dict.Add("FECHANACIMIENTO", ValidDate(FECHANACIMIENTO));
            dict.Add("NACIONALIDAD", NACIONALIDAD);
            dict.Add("CORREO", EMAIL);
            dict.Add("TELEFONO", TELEFONO);
            if (TMP)
                dict.Add("CVE_TIPODEPAGO", TIPODEPAGOCODE);
            dict.Add("RFC", RFC);
            dict.Add("CURP", CURP);
            dict.Add("DIRECCION_PAIS", DIRECCION_PAIS);
            dict.Add("DIRECCION_ESTADO", valid(DIRECCION_ESTADO));
            dict.Add("DIRECCION_CIUDAD", valid(DIRECCION_CIUDAD));
            dict.Add("DIRECCION_ENTIDAD", valid(DIRECCION_ENTIDAD));
            dict.Add("DIRECCION_COLONIA", valid(DIRECCION_COLONIA));
            dict.Add("DIRECCION_CALLE", valid(DIRECCION_CALLE));
            dict.Add("DIRECCION_CP", DIRECCION_CP);
            dict.Add("CVE_ORIGEN", "B");
            dict.Add("CVE_TITULOPROFESIONAL", TITULOPROFESIONALCODE);
            dict.Add("TITULOPROFESIONAL", valid(TITULOPROFESIONAL));
            dict.Add("CVE_PROFESION", PROFESIONCODE);
            dict.Add("PROFESION", valid(PROFESION));
            dict.Add("CEDULAPROFESIONAL", CEDULA);
            dict.Add("FECHACEDULA", ValidDate(FECCEDULA));
            dict.Add("SEGUROSOCIAL", SEGUROSOCIAL);
            dict.Add("CORREO365", CORREO365);
            if (TMP)
            {
                dict.Add("REGISTRADO", PERSONA_REGISTRADA);
                dict.Add("IMPORTADO", "0");
            }
            // -----------------------------
            if (add)
                dict.Add("FECHA_R", FECHA);
            else
                dict.Add("FECHA_M", FECHA);
            dict.Add("IP", "0");
            dict.Add("USUARIO", TMP ? sesion.pkUser.ToString() : sesion.nickName);
            dict.Add("ELIMINADO", "0");

            return dict;
        }

        public long findPersona(bool TMP = false)
        {
            //string sql = "SELECT ID_PERSONA FROM " + (TMP ? "QPersonasTMP" : "PERSONAS") + " WHERE IDSIU='" + this.IDSIU + "'";
            string sql = "SELECT TOP 1 P.ID_PERSONA    " +
             "  FROM " + (TMP ? "QPersonasTMP" : "PERSONAS") + " P INNER JOIN " +
             "       PERSONAS_SEDES S ON S.ID_PERSONA = P.ID_PERSONA " +
             " WHERE IDSIU            = '" + IDSIU + "' " +
             "   AND S.CVE_SEDE       = '" + CVE_SEDE + "' ";
            //"   AND S.CVE_TIPODEPAGO = '" + CVE_TIPODEPAGO  + "'";

            ResultSet res = db.getTable(sql);
            if (res.Next())
                PK_PERSONA = res.GetLong("ID_PERSONA");
            else
                PK_PERSONA = -1;
            return PK_PERSONA;
        }

        public bool editPersona(bool TMP = false)
        {
            string TABLE = TMP ? "QPersonasTMP" : "PERSONAS";
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
                EMAIL = res.Get("CORREO");
                TELEFONO = res.Get("TELEFONO");
                TIPODEPAGOCODE = res.Get("CVE_TIPODEPAGO");
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

                return true;
            }
            return false;
        }

        public bool addPersona(bool TMP = false)
        {
            Dictionary<string, string> values = prepareDataPersona(true, TMP);
            string sql = base.makeSqlInsert(values, TMP ? "PERSONAS_TMP" : "PERSONAS", TMP);
            try
            {
                PK_PERSONA = long.Parse(db.executeId(sql));

                if (TMP == false)
                {
                    sql = "INSERT INTO PERSONAS_SEDES (ID_PERSONA, CVE_SEDE, CVE_TIPODEPAGO, FECHA_R) " +
                      "                    VALUES (" + PK_PERSONA + ", '" + CVE_SEDE + "', '" + TIPODEPAGOCODE + "', GETDATE())";

                    db.execute(sql);
                }
            }
            catch (Exception ex)
            {
                PK_PERSONA = -1;
                xQuery = sql;
                xErrMsg = ex.Message;
            }
            return PK_PERSONA != -1;
        }

        public bool savePersona(bool TMP = false)
        {
            Dictionary<string, string> values = prepareDataPersona(false, TMP);
            string sql = base.makeSqlUpdate(values, TMP ? "PERSONAS_TMP" : "PERSONAS", "IDSIU = '" + IDSIU + "'", TMP);

            if (db.execute(sql))
            {
                if (TMP == false)
                {
                    sql = "DELETE FROM PERSONAS_SEDES WHERE ID_PERSONA = " + PK_PERSONA + " AND CVE_SEDE = '" + CVE_SEDE + "'";
                    db.execute(sql);

                    sql = "INSERT INTO PERSONAS_SEDES (ID_PERSONA, CVE_SEDE, CVE_TIPODEPAGO, FECHA_R, FECHA_M, USUARIO) " +
                      "                    VALUES (" + PK_PERSONA + ", '" + CVE_SEDE + "', '" + TIPODEPAGOCODE + "', GETDATE(), GETDATE(), '" + sesion.pkUser.ToString() + "')";

                    db.execute(sql);
                }
                return true;
            }
            else
                return false;
        }

        public bool markPersona()
        {
            string FECHA_M = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE PERSONAS_TMP SET REGISTRADO=1, IMPORTADO=1, FECHA_M='" + FECHA_M + "' WHERE IDSIU='" + IDSIU + "'";
            return db.execute(sql);
        }

        public bool CleanPersona()
        {
            string sql = "DELETE FROM PERSONAS_TMP WHERE USUARIO=" + sesion.pkUser;
            return db.execute(sql);
        }

    }// </>

    /*public partial class ActualizacionPAModel : SuperModel, ActualizarIPersona
    {
        public long PK_PERSONA;
        public string PERSONA_REGISTRADA = "0";
        //public string PERSONA_REGISTRADA;

        public Dictionary<string, string> prepareDataPersona(bool add, object obj)
        {
            bool TMP = (bool)obj;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            if (add)
            {
                dict.Add("IDSIU", IDSIU);
                if (TMP)
                    dict.Add("ID_PERSONA", PK_PERSONA.ToString());
            }

            dict.Add("APELLIDOS", valid(APELLIDOS));
            dict.Add("NOMBRES", valid(NOMBRES));
            dict.Add("SEXO", SEXO);
            dict.Add("FECHANACIMIENTO", ValidDate(FECHANACIMIENTO));
            dict.Add("NACIONALIDAD", NACIONALIDAD);
            dict.Add("CORREO", EMAIL);
            dict.Add("TELEFONO", TELEFONO);
            dict.Add("CVE_TIPODEPAGO", TIPODEPAGOCODE);
            dict.Add("RFC", RFC);
            dict.Add("CURP", CURP);
            dict.Add("DIRECCION_PAIS", DIRECCION_PAIS);
            dict.Add("DIRECCION_ESTADO", valid(DIRECCION_ESTADO));
            dict.Add("DIRECCION_CIUDAD", valid(DIRECCION_CIUDAD));
            dict.Add("DIRECCION_ENTIDAD", valid(DIRECCION_ENTIDAD));
            dict.Add("DIRECCION_COLONIA", valid(DIRECCION_COLONIA));
            dict.Add("DIRECCION_CALLE", valid(DIRECCION_CALLE));
            dict.Add("DIRECCION_CP", DIRECCION_CP);
            dict.Add("CVE_ORIGEN", "B");
            dict.Add("CVE_TITULOPROFESIONAL", TITULOPROFESIONALCODE);
            dict.Add("TITULOPROFESIONAL", valid(TITULOPROFESIONAL));
            dict.Add("CVE_PROFESION", PROFESIONCODE);
            dict.Add("PROFESION", valid(PROFESION));
            dict.Add("CEDULAPROFESIONAL", CEDULA);
            dict.Add("FECHACEDULA", ValidDate(FECCEDULA));
            dict.Add("SEGUROSOCIAL", SEGUROSOCIAL);
            if (TMP)
            {
                dict.Add("REGISTRADO", PERSONA_REGISTRADA);
                dict.Add("IMPORTADO", "0");
            }
            // -----------------------------
            if (add)
                dict.Add("FECHA_R", FECHA);
            else
                dict.Add("FECHA_M", FECHA);
            dict.Add("IP", "0");
            dict.Add("USUARIO", TMP ? sesion.pkUser.ToString() : sesion.nickName);
            dict.Add("ELIMINADO", "0");

            return dict;
        }

        public long findPersona(bool TMP = false)
        {
            string sql = "SELECT ID_PERSONA FROM " + (TMP ? "QPersonasTMP" : "PERSONAS") + " WHERE IDSIU='" + this.IDSIU + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
                PK_PERSONA = res.GetLong("ID_PERSONA");
            else
                PK_PERSONA = -1;
            return PK_PERSONA;
        }

        public bool editPersona(bool TMP = false)
        {
            string TABLE = TMP ? "QPersonasTMP" : "PERSONAS";
            string sql = "SELECT TOP 1 * FROM " + TABLE + " WHERE IDSIU = '" + IDSIU + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                APELLIDOS = res.Get("APELLIDOS");
                NOMBRES = res.Get("NOMBRES");
                SEXO = res.Get("SEXO");
                FECHANACIMIENTO = res.Get("FECHANACIMIENTO");
                NACIONALIDAD = res.Get("NACIONALIDAD");
                EMAIL = res.Get("CORREO");
                TELEFONO = res.Get("TELEFONO");
                TIPODEPAGOCODE = res.Get("CVE_TIPODEPAGO");
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

                return true;
            }
            return false;
        }

        public bool addPersona(bool TMP = false)
        {
            Dictionary<string, string> values = prepareDataPersona(true, TMP);
            string sql = base.makeSqlInsert(values, TMP ? "PERSONAS_TMP" : "PERSONAS", TMP);
            try
            {
                PK_PERSONA = long.Parse(db.executeId(sql));
            }
            catch (Exception) { PK_PERSONA = -1; }
            return PK_PERSONA != -1;
        }

        public bool savePersona(bool TMP = false)
        {
            Dictionary<string, string> values = prepareDataPersona(false, TMP);
            string sql = base.makeSqlUpdate(values, TMP ? "PERSONAS_TMP" : "PERSONAS", "IDSIU = '" + IDSIU + "'", TMP);
            return db.execute(sql);
        }

        public bool markPersona()
        {
            string FECHA_M = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE PERSONAS_TMP SET REGISTRADO=1, IMPORTADO=1, FECHA_M='" + FECHA_M + "' WHERE IDSIU='" + IDSIU + "'";
            return db.execute(sql);
        }

        public bool CleanPersona()
        {
            string sql = "DELETE FROM PERSONAS_TMP WHERE USUARIO=" + sesion.pkUser;
            return db.execute(sql);
        }

    }*/

}