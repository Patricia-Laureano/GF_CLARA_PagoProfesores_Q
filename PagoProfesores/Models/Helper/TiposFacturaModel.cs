using ConnectDB;
using System.Collections.Generic;

namespace PagoProfesores.Models.Helper
{
    public class TiposFacturaModel : SuperModel
    {
        public Dictionary<string, string> getTiposFactura()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_TIPOFACTURA, TIPOFACTURA FROM TIPOSFACTURA WHERE CVE_TIPOFACTURA <> 'E' ORDER BY CVE_TIPOFACTURA";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_TIPOFACTURA"), res.Get("TIPOFACTURA"));

            return dict;
        }
    }
}