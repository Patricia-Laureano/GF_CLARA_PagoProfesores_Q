using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class PersonasModel : SuperModel
    {
        public string cveSede { get; set; }
        public string nombre { get; set; }
        public string sql { get; set; }

        public DataTable ConsularPersonas()
        {
            DataTable dt = new DataTable();
            try
            {
                //sql = "select distinct IDSIU, ID_PERSONA, (NOMBRES + ' ' + APELLIDOS) AS PERSONA, ID_ESQUEMA from QPersonasListado where CVE_SEDE = '" + cveSede + "'";
                //sql = "select distinct IDSIU, ID_PERSONA, (NOMBRES + ' ' + APELLIDOS) AS PERSONA from QPersonasListado where CVE_SEDE = '" + cveSede + "'";


                sql = "select distinct IDSIU, ID_PERSONA, (NOMBRES + ' ' + APELLIDOS) AS PERSONA " +
                      "  from QPersonasListado                                                   " +
                      " where CVE_SEDE = '" + cveSede + "'                                       " +
                      "union                                                                     " +
                      "select distinct IDSIU, ID_PERSONA, (NOMBRES + ' ' + APELLIDOS) AS PERSONA " +
                      "  from QPersonasListado                                                   " +
                      " where IDSIU like 'E%'";

                //sql = "select distinct IDSIU, (NOMBRES + ' ' + APELLIDOS) AS PERSONA from QPersonasListado where CVE_SEDE = '" + cveSede + "' and (NOMBRES + ' ' + APELLIDOS) like '%" + nombre + "%'";
                dt = db.SelectDataTable(sql);

                return dt;
            }
            catch
            {
                return null;
            }
        }
    }
}