using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.ConsultarBanner
{
    public class CatTiposContratoModel : SuperModel
    {
        public string CONTRATO { get; set; }
        public string DESCRIPCION { get; set; }

        public string TIPOCONTRATO_INB { get; set; }
        public string TIPOCONTRATODESCRIPCION { get; set; }
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
                TIPOCONTRATO_INB
                ,TIPOCONTRATODESCRIPCION
                ,__validDateTime(FECHA_R)
                ,__validDateTime(FECHA_M)
                ,IP
                ,TMP?sesion.pkUser.ToString():sesion.nickName
                ,REGISTRADO
                ,IMPORTADO
            };

            string sql = "INSERT INTO TIPOSCONTRATO_TMP" +
                " (TIPOCONTRATO_INB,TIPOCONTRATODESCRIPCION,FECHA_R,FECHA_M,IP,USUARIO,REGISTRADO,IMPORTADO)" +
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
            string sql = "DELETE FROM TIPOSCONTRATO_TMP WHERE USUARIO = " + sesion.pkUser;
            return db.execute(sql);
        }

        public bool exist()
        {
            TABLE = TMP ? "TIPOSCONTRATO_TMP" : "TIPOSCONTRATO";
            string sql = "SELECT COUNT(*) AS 'MAX' FROM " + TABLE + " WHERE TIPOCONTRATO_INB='" + TIPOCONTRATO_INB + "'";
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
                this.TIPOCONTRATO_INB = itemChecked;

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
            TABLE = TMP ? "TIPOSCONTRATO_TMP" : "TIPOSCONTRATO";
            string sql = "SELECT TOP 1 * FROM " + TABLE + " WHERE TIPOCONTRATO_INB = '" + TIPOCONTRATO_INB + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                TIPOCONTRATO_INB = res.Get("TIPOCONTRATO_INB");
                TIPOCONTRATODESCRIPCION = res.Get("TIPOCONTRATODESCRIPCION");
                return true;
            }
            return false;
        }

        public bool save()
        {
            TABLE = TMP ? "TIPOSCONTRATO_TMP" : "TIPOSCONTRATO";
            string sql = "UPDATE " + TABLE + " SET " +
                " TIPOCONTRATODESCRIPCION = '" + TIPOCONTRATODESCRIPCION + "'" +
                ",FECHA_M = GETDATE()" +
                " WHERE TIPOCONTRATO_INB = '" + TIPOCONTRATO_INB + "'";

            return db.execute(sql);
        }

        public bool add()
        {
            string[] VALUES = {
                TIPOCONTRATO_INB
                ,TIPOCONTRATODESCRIPCION
                ,TMP?sesion.pkUser.ToString():sesion.nickName
            };

            string sql = "INSERT INTO TIPOSCONTRATO" +
                " (TIPOCONTRATO_INB,TIPOCONTRATODESCRIPCION,USUARIO,FECHA_R)" +
                "VALUES ('" + string.Join("','", VALUES) + "', GETDATE())";

            return db.execute(sql);
        }

        public bool mark()
        {
            string FECHA_M = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE TIPOSCONTRATO_TMP SET REGISTRADO=1, IMPORTADO=1, FECHA_M='" + FECHA_M + "' WHERE TIPOCONTRATO_INB='" + TIPOCONTRATO_INB + "'";
            return db.execute(sql);
        }
    }
}