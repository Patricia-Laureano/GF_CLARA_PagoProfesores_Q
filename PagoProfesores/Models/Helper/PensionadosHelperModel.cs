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
    public class PensionadosHelperModel : SuperModel
    {

        public Dictionary<string, string> getPensionados(string IdPersona)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT PK1,BENEFICIARION FROM VPENSIONADOS WHERE ID_PERSONA = '" + IdPersona + "' ORDER BY PK1";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("PK1"), res.Get("BENEFICIARION"));

            return dict;
        }







    }
}