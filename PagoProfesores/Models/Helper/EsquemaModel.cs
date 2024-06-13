using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class EsquemaModel : SuperModel
    {
        public string Periodo { get; set; }
        public string CampusVPDI { get; set; }
        //public string Nivel { get; set; }
        
        public Dictionary<string, string> getEsquema()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //TODO OLD string sql = "SELECT ID_ESQUEMA, ESQUEMADEPAGO FROM ESQUEMASDEPAGO WHERE CVE_SEDE = '" + CampusVPDI + "' ";
            //sql += (Periodo.Equals("")) ? "" : " AND PERIODO = '" + Periodo + "'";
            //sql += (Nivel.Equals("")) ? "" : " AND CVE_NIVEL = '" + Nivel + "'";
            string sql = "SELECT ES.ID_ESQUEMA, ES.ESQUEMADEPAGO FROM ESQUEMASDEPAGO ES INNER JOIN ESQUEMASDEPAGOFECHAS E ON(E.ID_ESQUEMA=ES.ID_ESQUEMA) WHERE CVE_SEDE = '" + CampusVPDI + "' ";
            sql += (Periodo.Equals("")) ? "" : " AND ES.PERIODO = '" + Periodo + "'";
            sql += " GROUP BY ES.ID_ESQUEMA, ES.ESQUEMADEPAGO";

            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("ID_ESQUEMA"), res.Get("ESQUEMADEPAGO"));
            return dict;
        }

        public Dictionary<string, string> getEsquemaAll()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //TODO OLD string sql = "SELECT ID_ESQUEMA, ESQUEMADEPAGO FROM ESQUEMASDEPAGO";
            string sql = "SELECT ES.ID_ESQUEMA, ES.ESQUEMADEPAGO FROM ESQUEMASDEPAGO ES INNER JOIN ESQUEMASDEPAGOFECHAS E ON(E.ID_ESQUEMA=ES.ID_ESQUEMA) GROUP BY ES.ID_ESQUEMA, ES.ESQUEMADEPAGO";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("ID_ESQUEMA"), res.Get("ESQUEMADEPAGO"));
            return dict;
        }
    }
}