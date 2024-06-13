using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ConnectDB;
using System.Diagnostics;

namespace PagoProfesores.Models.Helper
{
    public class BancosModel : SuperModel
    {

        public Dictionary<string, string> getBanco()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_BANCO, BANCO FROM BANCOS ORDER BY CVE_BANCO";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_BANCO"), res.Get("BANCO"));

            return dict;
        }



    }
}