using ConnectDB;
using PagoProfesores.Models;
using PagoProfesores.Models.Helper;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Models.Helper
{
    public class EscuelasModel : SuperModel
    {

        public Dictionary<string, string> getEscuelas()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_ESCUELA,ESCUELA FROM ESCUELAS ORDER BY CVE_ESCUELA";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_ESCUELA"), res.Get("ESCUELA"));

            return dict;
        }






    }
}