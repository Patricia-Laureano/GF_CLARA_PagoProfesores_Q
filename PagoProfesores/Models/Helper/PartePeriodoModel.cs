using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class PartePeriodoModel : SuperModel
    {
        public Dictionary<string, string> getPartePeriodo()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "select PERIODO, DESCRIPCION from PARTE_PERIODOS ORDER BY PERIODO";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("PERIODO"), res.Get("PERIODO") + " - " + res.Get("DESCRIPCION"));

            return dict;
        }
    }
}