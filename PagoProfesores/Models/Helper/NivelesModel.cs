using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class NivelesModel : SuperModel
    {

        public Dictionary<string, string> getNiveles()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_NIVEL,NIVEL FROM NIVELES ORDER BY NIVEL ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_NIVEL"), res.Get("NIVEL"));

            return dict;
        }







    }
}