using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.ConsultarBanner
{
    public class CatCiclosModel : SuperModel
    {
        public string ANNYO { get; set; }
        public string DESCRIPCION { get; set; }
        
        public string CVE_CICLO { get; set; }
        public string CICLO { get; set; }
        public string REGISTRADO { get; set; }
        public string IMPORTADO { get; set; }
        public string FECHA_R { get; set; }
        public string FECHA_M { get; set; }
        public string IP { get; set; }

        public string DISPONIBLE { get; set; }

        public string sql { get; set; }

        public string TABLE = "CICLOS";
        public bool TMP = false;

        public bool addTmp()
        {
            string[] VALUES = {
                CVE_CICLO
                ,CICLO
                ,__validDateTime(FECHA_R)
                ,__validDateTime(FECHA_M)
                ,IP
                ,TMP?sesion.pkUser.ToString():sesion.nickName
                ,REGISTRADO
                ,IMPORTADO
                ,DISPONIBLE
            };

            string sql = "INSERT INTO CICLOS_TMP" +
                " (CVE_CICLO,CICLO,FECHA_R,FECHA_M,IP,USUARIO,REGISTRADO,IMPORTADO,DISPONIBLE)" +
                "VALUES ('" + string.Join("','", VALUES) + "')";

            return db.execute(sql);
        }

        public void isDisponible()
        {
            //CVE_CICLO es de banner
            TABLE = "CICLOS";
            string sql = "SELECT DISPONIBLE FROM " + TABLE + " WHERE CVE_CICLO='" + CVE_CICLO + "'";
       
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
            
                sql = "UPDATE CICLOS SET DISPONIBLE = '" + DISPONIBLE + "' WHERE CVE_CICLO='" + CVE_CICLO + "'";      
                     

                if (db.execute(sql))
                {
                    sql = "UPDATE CICLOS_TMP SET DISPONIBLE = '" + DISPONIBLE + "' WHERE CVE_CICLO='" + CVE_CICLO + "'";


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
            string sql = "DELETE FROM CICLOS_TMP WHERE USUARIO = " + sesion.pkUser;
            return db.execute(sql);
        }
        
        public bool exist()
        {
            TABLE = TMP ? "CICLOS_TMP" : "CICLOS";
            string sql = "SELECT COUNT(*) AS 'MAX' FROM " + TABLE + " WHERE CVE_CICLO='" + CVE_CICLO + "'";
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
                this.CVE_CICLO = itemChecked;

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
            TABLE = TMP ? "CICLOS_TMP" : "CICLOS";
            string sql = "SELECT TOP 1 * FROM " + TABLE + " WHERE CVE_CICLO = '" + CVE_CICLO + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                CVE_CICLO = res.Get("CVE_CICLO");
                CICLO = res.Get("CICLO");
                return true;
            }
            return false;
        }

        public bool save()
        {
            TABLE = TMP ? "CICLOS_TMP" : "CICLOS";
            string sql = "UPDATE " + TABLE + " SET " +
                " CICLO = '" + CICLO + "'" +
                ",FECHA_M = GETDATE()" +
                " WHERE CVE_CICLO = '" + CVE_CICLO + "'";

            return db.execute(sql);
        }

        public bool add()
        {
            string[] VALUES = {
                CVE_CICLO
                ,CICLO
                ,TMP?sesion.pkUser.ToString():sesion.nickName
            };

            string sql = "INSERT INTO CICLOS" +
                " (CVE_CICLO,CICLO,USUARIO,FECHA_R)" +
                "VALUES ('" + string.Join("','", VALUES) + "', GETDATE())";

            return db.execute(sql);
        }

        public bool mark()
        {
            string FECHA_M = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE CICLOS_TMP SET REGISTRADO=1, IMPORTADO=1, FECHA_M='" + FECHA_M + "' WHERE CVE_CICLO='" + CVE_CICLO + "'";
            return db.execute(sql);
        }
    }
}