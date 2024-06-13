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
    public class TiposContratosModel : SuperModel
    {

        public Dictionary<string, string> getContratos()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT TIPOCONTRATO_INB,TIPOCONTRATODESCRIPCION FROM TIPOSCONTRATO ORDER BY TIPOCONTRATO_INB ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("TIPOCONTRATO_INB"), res.Get("TIPOCONTRATODESCRIPCION"));

            return dict;
        }

    }
}