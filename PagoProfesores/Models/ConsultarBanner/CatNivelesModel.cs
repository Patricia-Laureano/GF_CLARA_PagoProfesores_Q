using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.ConsultarBanner
{
    public class CatNivelesModel : SuperModel
    {
        public string NIVEL { get; set; }
        public string DESCRIPCION { get; set; }

        public string CVE_NIVEL { get; set; }
        public string REGISTRADO { get; set; }
        public string IMPORTADO { get; set; }
        public string FECHA_R { get; set; }
        public string FECHA_M { get; set; }
        public string IP { get; set; }

        public string TABLE = "NIVELES";
        public bool TMP = false;

        public bool addTmp()
        {
            string[] VALUES = {
                CVE_NIVEL
                ,NIVEL
                ,__validDateTime(FECHA_R)
                ,__validDateTime(FECHA_M)
                ,IP
                ,TMP?sesion.pkUser.ToString():sesion.nickName
                ,REGISTRADO
                ,IMPORTADO
            };

            string sql = "INSERT INTO NIVELES_TMP" +
                " (CVE_NIVEL,NIVEL,FECHA_R,FECHA_M,IP,USUARIO,REGISTRADO,IMPORTADO)" +
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
            catch (Exception ex) { ex = null;  dt = new DateTime(2000, 1, 1, 0, 0, 0, 0); }
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public bool Clean()
        {
            string sql = "DELETE FROM NIVELES_TMP WHERE USUARIO = " + sesion.pkUser;
            return db.execute(sql);
        }

        public bool exist()
        {
            TABLE = TMP ? "NIVELES_TMP" : "NIVELES";
            string sql = "SELECT COUNT(*) AS 'MAX' FROM " + TABLE + " WHERE CVE_NIVEL='" + CVE_NIVEL + "'";
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
                this.CVE_NIVEL = itemChecked;

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
            TABLE = TMP ? "NIVELES_TMP" : "NIVELES";
            string sql = "SELECT TOP 1 * FROM " + TABLE + " WHERE CVE_NIVEL = '" + CVE_NIVEL + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                CVE_NIVEL = res.Get("CVE_NIVEL");
                NIVEL = res.Get("NIVEL");
                return true;
            }
            return false;
        }

        public bool save()
        {
            TABLE = TMP ? "NIVELES_TMP" : "NIVELES";
            string sql = "UPDATE " + TABLE + " SET " +
                " NIVEL = '" + NIVEL + "'" +
                ",FECHA_M = GETDATE()" +
                " WHERE CVE_NIVEL = '" + CVE_NIVEL + "'";

            return db.execute(sql);
        }

        public bool add()
        {
            string[] VALUES = {
                CVE_NIVEL
                ,NIVEL
                ,TMP?sesion.pkUser.ToString():sesion.nickName
            };

            string sql = "INSERT INTO NIVELES" +
                " (CVE_NIVEL,NIVEL,USUARIO,FECHA_R)" +
                "VALUES ('" + string.Join("','", VALUES) + "', GETDATE())";

            return db.execute(sql);
        }

        public bool mark()
        {
            string FECHA_M = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE NIVELES_TMP SET REGISTRADO=1, IMPORTADO=1, FECHA_M='" + FECHA_M + "' WHERE CVE_NIVEL='" + CVE_NIVEL + "'";
            return db.execute(sql);
        }
    }
}