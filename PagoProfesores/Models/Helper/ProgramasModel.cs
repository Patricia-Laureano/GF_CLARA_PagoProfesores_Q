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
    public class ProgramasModel : SuperModel
    {

        public Dictionary<string, string> getProgramas(string escuela = "", string programa = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();


            string stmescuela = "";
            if (escuela != "" && escuela != null)
                stmescuela = " WHERE CVE_ESCUELA = '" + escuela + "'";


            string sql = "SELECT CVE_PROGRAMA,PROGRAMA FROM PROGRAMAS " + stmescuela + "  ORDER BY CVE_PROGRAMA ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_PROGRAMA"), res.Get("PROGRAMA"));

            return dict;
        }



        public Dictionary<string, string> getProgramas2(string escuela = "", string programa = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();


            string stmescuela = "";
            if (escuela != "" && escuela != null)
                stmescuela = " WHERE CVE_ESCUELA = '" + escuela + "'";


            string sql = "SELECT CVE_PROGRAMA,PROGRAMA FROM PROGRAMAS  "+stmescuela+"  ORDER BY CVE_PROGRAMA ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_PROGRAMA"), res.Get("PROGRAMA"));

            return dict;
        }






    }
}