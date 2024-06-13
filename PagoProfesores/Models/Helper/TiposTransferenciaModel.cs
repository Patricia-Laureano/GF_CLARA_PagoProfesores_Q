using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class TiposTransferenciaModel : SuperModel
    {
        public Dictionary<string, string> getTiposTransferencia()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_TIPOTRANSFERENCIA, TIPOTRANSFERENCIA FROM TIPOSTRANSFERENCIA ORDER BY CVE_TIPOTRANSFERENCIA";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_TIPOTRANSFERENCIA"), res.Get("TIPOTRANSFERENCIA"));

            return dict;
        }
    }
}