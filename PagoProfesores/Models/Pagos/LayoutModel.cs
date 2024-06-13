using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{
    public class LayoutModel : SuperModel
    {
        public string ComboSql(string Sql, string cve, string valor, string Inicial)
        {
            string MySql = Sql;
            string Combo = "\r\n";
            string Clave = "";
            string Valor = "";
            string s = "";

            ResultSet reader = db.getTable(Sql);
            try
            {
                while (reader.Next())
                {
                    Clave = reader.Get(cve);
                    Valor = reader.Get(valor);
                    if (Clave == Inicial || Valor == Inicial)
                    {
                        s = "Selected";
                    }
                    else
                    {
                        s = "";
                    }
                    Combo = Combo + "<option value =\"" + Clave + "\" " + s + ">";
                    Combo += Valor + " </ option >\r\n";
                }
                return Combo;
            }
            catch
            {
                return "Error en consulta combo";
            }
        }
    }
}