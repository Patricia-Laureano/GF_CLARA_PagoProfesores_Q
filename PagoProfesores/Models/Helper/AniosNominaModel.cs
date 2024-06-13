using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PagoProfesores.Models.Helper
{
    public class AniosNominaModel : SuperModel
    {

        public List<string> getAnios()
        {
            List<string> list = new List<string>();

            int anio = int.Parse(DateTime.Now.ToString("yyyy")) + 1;
            string sql = "select distinct anio from QNominaMesAnio";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                list.Add(res.Get("anio"));
            
            return list;
        }


    }





}