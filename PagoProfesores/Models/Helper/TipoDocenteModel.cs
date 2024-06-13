using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class TipoDocenteModel : SuperModel
    {
        public Dictionary<string, string> getTipoDocente()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //string sql = "  select case when VARIABLE = 'HONORARIOS' then 'HON'                    " + Prinmera versión
            //             "              when VARIABLE = 'ASMILIADOS' then 'HAS'                    " +
            //             "              when VARIABLE = 'FACTURAS'   then 'HFM' end CODE, VARIABLE " +
            //             "    from VARIABLES                                                       " +
            //             "   where VARIABLE in ('ASMILIADOS','HONORARIOS','FACTURAS')              " +
            //             "     and VALOR = 1                                                       ";
            //string sql = "  select distinct convert(nvarchar(max), VALOR_TEXT) AS CODE          " + Segunda versión
            //             "    from VARIABLES                                                    " +
            //             "   where VARIABLE in ('Honorarios',                                   " + 
            //             "                      'Asimilados Honorarios', 'Asimilados Directos', " +
            //             "                      'Facturas Honorarios', 'Facturas Directos')     " +
            //             "     and VALOR = '1'                                                  ";
            string sql = "  select distinct convert(nvarchar(max), VALOR_TEXT) AS CODE " +
                         "    from VARIABLES                                           " +
                         "   where IP    = 'TipoDocente'                               " +
                         "     and VALOR = '1'                                         ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CODE"), res.Get("CODE"));

            return dict;
        }
    }
}