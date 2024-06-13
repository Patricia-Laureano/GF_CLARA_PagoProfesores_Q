using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PagoProfesores.Models.Helper
{
    public class CiclosModel : SuperModel
    {

        public List<string> getCiclos()
        {
            List<string> list = new List<string>();

            string sql = "SELECT CVE_CICLO FROM CICLOS WHERE DISPONIBLE = 1 ORDER BY CVE_CICLO DESC";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                list.Add(res.Get("CVE_CICLO"));

            return list;
        }


    }





}