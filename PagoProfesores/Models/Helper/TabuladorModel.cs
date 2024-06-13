using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class TabuladorModel : SuperModel
    {
        public string CampusVPDI { get; set; }

        public Dictionary<string, string> getTabulador()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "select TABULADOR " +
                         "  from TABULADOR " +
                         " where CVE_SEDE = '" + CampusVPDI + "' ";

            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("TABULADOR"), res.Get("TABULADOR"));
            return dict;
        }
    }
}