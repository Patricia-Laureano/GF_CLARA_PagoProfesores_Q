using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class TiposBloqueoModel : SuperModel
    {
        public Dictionary<string, string> getBloqueos()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_BLOQUEO, BLOQUEO FROM BLOQUEOS ORDER BY CVE_BLOQUEO";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_BLOQUEO"), res.Get("BLOQUEO"));

            return dict;
        }
    }
}