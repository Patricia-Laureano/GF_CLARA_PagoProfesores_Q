using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using ConnectDB;

namespace PagoProfesores.Models.Authentication
{
	public class LoginModel : SuperModel
	{

        public string ID_SIU { get; set; }
        public char CVE_ORIGEN { get; set; }
        public string DATOS_FISCALES { get; set; }
        public string RFC { get; set; }
        public string RZ_RFC { get; set; }
        


        public LoginModel()
		{

		}

		public bool existeUsuario(string user)
		{
            //string sql = "SELECT COUNT(*) AS 'MAX' FROM USUARIOS WHERE USUARIO = '" + user + "'";

            string sql = "SELECT ((SELECT COUNT(*) FROM USUARIOS WHERE USUARIO = '" + user + "'"+ ") + (SELECT COUNT(*) FROM PERSONAS WHERE IDSIU = '" + user + "'"+")) as MAX";
			int max = db.Count(sql);

			return max > 0;
		}


        public bool existeProfesor(string user)
        {
            string sql = "SELECT COUNT(*) AS 'MAX' FROM PERSONAS P INNER JOIN PERSONAS_SEDES PS ON PS.ID_PERSONA = P.ID_PERSONA WHERE CORREO365 = '" + user + "' AND ACTIVO = 1";
            int max = db.Count(sql);

            return max > 0;
        }


        public bool consulta_Externo(string user)
        {
            string sql = "select P.ID_PERSONA,CORREO365,IDSIU,CVE_ORIGEN,DATOSFISCALES,RFC,RZ_RFC,ACTIVO from personas P INNER JOIN PERSONAS_SEDES PS ON PS.ID_PERSONA = P.ID_PERSONA WHERE IDSIU = '" + user + "' AND ACTIVO = 1";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                ID_SIU = res.Get("IDSIU");
                CVE_ORIGEN = res.GetChar("CVE_ORIGEN");
                DATOS_FISCALES = res.Get("DATOSFISCALES");
                RFC = res.Get("RFC");
                RZ_RFC = res.Get("RZ_RFC");

                return true;

            }
            else { return false; }

        }

        


        public string consultaPassword(string user)
		{
			string sql = "SELECT TOP 1 * FROM USUARIOS WHERE USUARIO = '" + user + "'";
			ResultSet res = db.getTable(sql);
			if (res.Next())
				return res.Get("PASSWORD");
			return "NONE";
		}

	}
}