using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class PeriodosModel : SuperModel
    {

        public string id_ciclo { get; set; }
        public string periodo { get; set; }


        public List<string> getPeriodos(string id_ciclo = "")
        {
            List<string> list = new List<string>();

            string sql = "SELECT CVE_CICLO, PERIODO FROM PERIODOS WHERE  DISPONIBLE = 1 ";
            sql += (id_ciclo.Equals("")) ? "" : " AND CVE_CICLO = " + id_ciclo;
            ResultSet res = db.getTable(sql);
            while (res.Next())
                list.Add(res.Get("PERIODO"));
            return list;
        }

    }
}