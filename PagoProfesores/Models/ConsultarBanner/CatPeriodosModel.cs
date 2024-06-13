using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.ConsultarBanner
{
    public class CatPeriodosModel : SuperModel
    {
        public string CVE_CICLO { get; set; }
        public string PERIODO { get; set; }
        public string DESCRIPCION { get; set; }
        public string FECHA_INICIAL { get; set; }
        public string FECHA_FINAL { get; set; }
        public string TIPOPERIODO { get; set; }
        public string FECHA_R { get; set; }
        public string FECHA_M { get; set; }
        public string IP { get; set; }
        public string REGISTRADO { get; set; }
        public string IMPORTADO { get; set; }

        public string DISPONIBLE { get; set; }

        public string sql { get; set; }


        public string TABLE = "PERIODOS";
        public bool TMP = false;

        public bool addTmp()
        {
            string[] VALUES = {
                CVE_CICLO
                ,PERIODO
                ,DESCRIPCION
                ,FECHA_INICIAL
                ,FECHA_FINAL
                ,TIPOPERIODO
                ,__validDateTime(FECHA_R)
                ,__validDateTime(FECHA_M)
                ,IP
                ,TMP?sesion.pkUser.ToString():sesion.nickName
                ,REGISTRADO
                ,IMPORTADO
                ,DISPONIBLE
            };

            string sql = "INSERT INTO PERIODOS_TMP" +
                " (CVE_CICLO, PERIODO, DESCRIPCION, FECHA_INICIAL, FECHA_FINAL, TIPOPERIODO, FECHA_R, FECHA_M, IP, USUARIO, REGISTRADO, IMPORTADO,DISPONIBLE)" +
                "VALUES ('" + string.Join("','", VALUES) + "')";

            return db.execute(sql);
        }

        public void isDisponible()
        {
            //CVE_CICLO es de banner
            TABLE = "PERIODOS";
            string sql = "SELECT DISPONIBLE FROM " + TABLE + " WHERE CVE_CICLO='" + CVE_CICLO + "' AND PERIODO = '" + PERIODO + "'";

            ResultSet res = db.getTable(sql);
            if (res.Next())
            {

                string disponible = res.Get("DISPONIBLE");

                if (disponible == "True")
                    DISPONIBLE = "1";
                else
                    DISPONIBLE = "0";

            }
            else
                DISPONIBLE = "0";


        }


        public bool setDisponible()
        {
            try
            {

                sql = "UPDATE PERIODOS SET DISPONIBLE = '" + DISPONIBLE + "' WHERE CVE_CICLO='" + CVE_CICLO + "' AND PERIODO = '" + PERIODO + "' ";


                if (db.execute(sql))
                {
                    sql = "UPDATE PERIODOS_TMP SET DISPONIBLE = '" + DISPONIBLE + "' WHERE CVE_CICLO='" + CVE_CICLO + "' AND PERIODO = '" + PERIODO + "'";


                    if (db.execute(sql))
                        return true;
                    else
                        return true;

                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
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
            string sql = "DELETE FROM PERIODOS_TMP WHERE USUARIO = " + sesion.pkUser;
            return db.execute(sql);
        }

        public bool exist()
        {
            TABLE = TMP ? "PERIODOS_TMP" : "PERIODOS";
            string sql = "SELECT COUNT(*) AS 'MAX' FROM " + TABLE + " WHERE CVE_CICLO='" + CVE_CICLO + "' AND PERIODO = '" + PERIODO + "'";
            int MAX = db.Count(sql);
            return MAX > 0;
        }

        public bool Importar(string data, string data0)
        {
            bool all_result = true;
            string[] arrChecked = data.Split(new char[] { ',' });

            if (arrChecked.Length == 1)
                if (arrChecked[0] == "")
                    return false;

            foreach (string itemChecked in arrChecked)
            {
                this.PERIODO = itemChecked;
                this.CVE_CICLO = data0;

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
            TABLE = TMP ? "QPeriodosT" : "QPeriodos";
            string sql = "SELECT TOP 1 * FROM " + TABLE + " WHERE CVE_CICLO = '" + CVE_CICLO + "' AND PERIODO = '" + PERIODO + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                CVE_CICLO     = res.Get("CVE_CICLO");
                PERIODO       = res.Get("PERIODO");
                DESCRIPCION   = res.Get("DESCRIPCION");
                FECHA_INICIAL = res.Get("FECHA_INICIAL");
                FECHA_FINAL   = res.Get("FECHA_FINAL");
                TIPOPERIODO   = res.Get("TIPOPERIODO");
                return true;
            }
            return false;
        }

        public bool save()
        {
            TABLE = TMP ? "PERIODOS_TMP" : "PERIODOS";
            string sql = "UPDATE " + TABLE + " SET " +
                " DESCRIPCION     = '" + DESCRIPCION + "'" +
                ",FECHA_INICIAL   = CONVERT(datetime,'" + FECHA_INICIAL + "', 111)" +
                ",FECHA_FINAL     = CONVERT(datetime,'" + FECHA_FINAL + "', 111)" +
                ",TIPOPERIODO     = '" + TIPOPERIODO + "'" +
                ",FECHA_M         = GETDATE()" +
                " WHERE CVE_CICLO = '" + CVE_CICLO + "'" +
                " AND PERIODO     = '" + PERIODO + "'";

            return db.execute(sql);
        }

        public bool add()
        {
            string[] VALUES = {
                CVE_CICLO
                ,PERIODO
                ,DESCRIPCION
                ,FECHA_INICIAL
                ,FECHA_FINAL
                ,TIPOPERIODO
                ,TMP?sesion.pkUser.ToString():sesion.nickName
            };

            string sql = "INSERT INTO PERIODOS" +
                " (CVE_CICLO, PERIODO, DESCRIPCION, FECHA_INICIAL, FECHA_FINAL, TIPOPERIODO, USUARIO, FECHA_R)" +
                "VALUES ('" + string.Join("','", VALUES) + "', GETDATE())";

            return db.execute(sql);
        }

        public bool mark()
        {
            string FECHA_M = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE PERIODOS_TMP SET REGISTRADO=1, IMPORTADO=1, FECHA_M='" + FECHA_M + "' WHERE CVE_CICLO='" + CVE_CICLO + "' AND PERIODO = '" + PERIODO + "'";
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