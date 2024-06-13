using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PagoProfesores.Models.Helper
{
    public class MesesNominaModel : SuperModel
    {
        
        public string anio { get; set; }
        public Dictionary<string, string> getMeses(String anio)
        {
            Dictionary<string, string> dict= new Dictionary<string, string>();
            string sql;
            //string sql = "set language spanish ";
            //sql= sql + "select distinct month(FECHA_PA) mes, datename(month, fecha_pa) nombremes from QNomina order by mes asc";
            if (anio.Equals(""))
            {
                sql = "set language spanish ;select distinct mes, datename(month, fecha_de_pago) nombremes from QNominaMesAnio order by mes asc";
            }
            else
            {
                sql = "set language spanish ;select distinct mes, datename(month, fecha_de_pago) nombremes from QNominaMesAnio where  year(fecha_de_pago) ='" + anio+"' order by mes asc";
            }
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("mes"),res.Get("nombremes"));

            return dict;
        }


    }





}