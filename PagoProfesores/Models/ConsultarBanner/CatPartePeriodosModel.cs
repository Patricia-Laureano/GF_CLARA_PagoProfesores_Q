using ConnectDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.ConsultarBanner
{
    public class CatPartePeriodosModel : SuperModel
    {


      
        [JsonProperty("STVPTRM_CODE")]
        public string PERIODO { get; set; }

        [JsonProperty("STVPTRM_DESC")]
        public string DESCRIPCION { get; set; }

        public string FECHA_R { get; set; }
        public string FECHA_M { get; set; }
        public string IP { get; set; }
        public string REGISTRADO { get; set; }
        public string IMPORTADO { get; set; }

        public string TABLE = "PERIODOS";
        public bool TMP = false;

        public bool addTmp()
        {
            string[] VALUES = {               
                PERIODO
                ,DESCRIPCION
                ,__validDateTime(FECHA_R)
                ,__validDateTime(FECHA_M)              
                ,TMP?sesion.pkUser.ToString():sesion.nickName
                ,REGISTRADO
                ,IMPORTADO
            };

            string sql = "INSERT INTO PARTE_PERIODOS_TMP" +
                " (PERIODO, DESCRIPCION, FECHA_R, FECHA_M, USUARIO, REGISTRADO, IMPORTADO)" +
                "VALUES ('" + string.Join("','", VALUES) + "')";

            return db.execute(sql);
        }

        public string __validDateTime(string str)
        {
            DateTime dt;
            try
            {
                string[] array = str.Split(new char[] { '/' });
                dt = new DateTime(int.Parse(array[2]), int.Parse(array[1]), int.Parse(array[0]), 0, 0, 0, 0);
            }
            catch (Exception ex) { dt = new DateTime(2000, 1, 1, 0, 0, 0, 0); }
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public bool Clean()
        {
            string sql = "DELETE FROM PARTE_PERIODOS_TMP WHERE USUARIO = " + sesion.pkUser;
            return db.execute(sql);
        }

        public bool exist()
        {
            TABLE = TMP ? "PARTE_PERIODOS_TMP" : "PARTE_PERIODOS";
            string sql = "SELECT COUNT(*) AS 'MAX' FROM " + TABLE + " WHERE PERIODO = '" + PERIODO + "'";
            int MAX = db.Count(sql);
            return MAX > 0;
        }

        public bool Importar(string data)
        {
            bool all_result = true;
            string[] arrChecked = data.Split(new char[] { ',' });

            if (arrChecked.Length == 1)
                if (arrChecked[0] == "")
                    return false;

            foreach (string itemChecked in arrChecked)
            {
                this.PERIODO = itemChecked;
               

                // 1.- Consultamos los datos de la tabla temporal
                this.TMP = true;
                edit();

                // 2.- Checamos si existe en la tabla normal
                this.TMP = false;
                bool result;
                if (exist())
                    result = save();
                else
                    result = add();
                if (result)
                    mark();

                all_result = all_result & result;
            }
            return all_result;
        }

        public bool edit()
        {
            TABLE = TMP ? "PARTE_PERIODOS_TMP" : "PARTE_PERIODOS";
            string sql = "SELECT TOP 1 * FROM " + TABLE + " WHERE PERIODO = '" + PERIODO + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {              
                PERIODO = res.Get("PERIODO");
                DESCRIPCION = res.Get("DESCRIPCION");
                return true;
            }
            return false;
        }

        public bool save()
        {
            TABLE = TMP ? "PARTE_PERIODOS_TMP" : "PARTE_PERIODOS";
            string sql = "UPDATE " + TABLE + " SET " +
                " DESCRIPCION     = '" + DESCRIPCION + "'" +
                ",FECHA_M         = GETDATE()" +
                " WHERE  PERIODO  = '" + PERIODO + "'";

            return db.execute(sql);
        }

        public bool add()
        {
            string[] VALUES = {             
                 PERIODO
                ,DESCRIPCION
                ,TMP?sesion.pkUser.ToString():sesion.nickName
            };

            string sql = "INSERT INTO PARTE_PERIODOS" +
                " (PERIODO, DESCRIPCION, USUARIO, FECHA_R)" +
                "VALUES ('" + string.Join("','", VALUES) + "', GETDATE())";

            return db.execute(sql);
        }

        public bool mark()
        {
            string FECHA_M = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE PARTE_PERIODOS_TMP SET REGISTRADO=1, IMPORTADO=1, FECHA_M='" + FECHA_M + "' WHERE PERIODO = '" + PERIODO + "'";
            return db.execute(sql);
        }

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
