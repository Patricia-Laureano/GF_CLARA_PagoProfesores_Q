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
    public class TiposdePagosModel : SuperModel
    {

        public Dictionary<string, string> getTiposPago()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //  string sql = "SELECT CVE_TIPODEPAGO,TIPODEPAGO FROM TIPOSDEPAGO ORDER BY CVE_TIPODEPAGO";
            string sql = "SELECT CVE_TIPODEPAGO,TIPODEPAGO FROM TIPOSDEPAGO WHERE ACTIVO = 1 ORDER BY CVE_TIPODEPAGO";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_TIPODEPAGO"), res.Get("TIPODEPAGO"));

            return dict;
        }

        public Dictionary<string, string> getTiposPagoV()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //string sql = "  select distinct VALOR_OP AS CODE                                    " +
            //             "    from VARIABLES                                                    " +
            //             "   where VARIABLE in ('Honorarios',                                   " +
            //             "                      'Asimilados Honorarios', 'Asimilados Directos', " +
            //             "                      'Facturas Honorarios', 'Facturas Directos')     " +
            //             "     and VALOR = 1                                                    ";
            string sql = "  select distinct VALOR_OP AS CODE " +
                         "    from VARIABLES                 " +
                         "   where IP    = 'TipoDocente'     " +
                         "     and VALOR = '1'               ";

            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CODE"), res.Get("CODE"));

            return dict;
        }
    }
}