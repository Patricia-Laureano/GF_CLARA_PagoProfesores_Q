using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class CentroCostosModel : SuperModel
    {
        public string CampusVPDI { get; set; }
        public string EscuelaCVE { get; set; }
        public string TipoPagoCVE { get; set; }
        public string CVE_Programa { get; set; }

        public Dictionary<string, string> getCentrosdeCostos()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT ID_CENTRODECOSTOS, CAST(CENTRODECOSTOS AS nvarchar(50)) + ' ' + ISNULL(TIPODEPAGO,'') + ' (' + CUENTA + ')' AS CENTRODECOSTOS FROM QCentrodeCostos01 ";

            if (CampusVPDI != "" && CampusVPDI != null && CampusVPDI != "null")
                sql += " WHERE CVE_SEDE = '" + CampusVPDI + "' ";

            if (EscuelaCVE != "" && EscuelaCVE != null && EscuelaCVE != "null")
                sql += " AND CVE_ESCUELA = '" + EscuelaCVE + "' ";

            if (TipoPagoCVE != "" && TipoPagoCVE != null && TipoPagoCVE != "null")
                sql += " AND CVE_TIPODEPAGO = '" + TipoPagoCVE + "' ";

            if (CVE_Programa != "" && CVE_Programa != null && CVE_Programa != "null")
                sql += " AND CVE_PROGRAMA = '" + CVE_Programa + "' ";



            sql += " ORDER BY ID_CENTRODECOSTOS ";

            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("ID_CENTRODECOSTOS"), res.Get("CENTRODECOSTOS"));

            return dict;
        }

        public Dictionary<string, string> getCentrosdeCostosAll()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT ID_CENTRODECOSTOS, CAST(CENTRODECOSTOS AS nvarchar(50)) + ' ' + ISNULL(TIPODEPAGO,'') + ' (' + CUENTA + ')' AS CENTRODECOSTOS FROM QCentrodeCostos01 ";

            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("ID_CENTRODECOSTOS"), res.Get("CENTRODECOSTOS"));

            return dict;
        }
    }
}