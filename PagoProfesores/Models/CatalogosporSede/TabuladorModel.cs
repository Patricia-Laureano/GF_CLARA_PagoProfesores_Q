using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ConnectDB;

namespace PagoProfesores.Models.CatalogosPorSede
{
    public class TabuladorModel : SuperModel
    {
        public int idTabulador { get; set; }
        public string ClaveNivel { get; set; }
        public string ClaveSede { get; set; }
        public string Tabulador { get; set; }
        public string MontoHora { get; set; }
        public string Sede { get; set; }
        public string sql { get; set; }
       


        public TabuladorModel()
        {
        }


        public string Niveles(string cve)
        {
            string MySql = "SELECT * FROM NIVELES ORDER BY NIVEL";
            string Combo = "\r\n";

            ResultSet reader = db.getTable(MySql);
            try
            {
                while (reader.Next())
                {
                    Combo = Combo + "<option value =\"" + reader.Get("CVE_NIVEL") + "\" >";
                    Combo += reader.Get("NIVEL") + " </ option >\r\n";
                }
                return Combo;
            }
            catch
            {
                return "Error en consulta";
            }
        }

        public bool Add()
        {

            try
            {
                sql = "INSERT INTO TABULADOR(CVE_SEDE,CVE_NIVEL,TABULADOR,Monto,USUARIO) VALUES('"+Sede+"','" + ClaveNivel + "','" + Tabulador + "','" + MontoHora + "','"+this.sesion.nickName+"')";
                if (db.execute(sql))
                {
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



        public bool Edit()
        {
            try
            {
                sql = "SELECT * FROM TABULADOR WHERE PK1=" + idTabulador + "";
                ResultSet  res = db.getTable(sql);

                if (res.Next())
                {
                  ClaveNivel = res.Get("CVE_NIVEL");
                  ClaveSede = res.Get("CVE_SEDE");
                  Tabulador = res.Get("TABULADOR");
                  MontoHora = res.Get("MONTO");

                    
                    return true;
                }
                else { return false; }

            }
            catch
            {
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                
                sql = "UPDATE TABULADOR SET  CVE_NIVEL = '" + ClaveNivel + "', TABULADOR = '"  + Tabulador + "', Monto = '" + MontoHora + "' ,USUARIO = '"+this.sesion.nickName+"',FECHA_M = GETDATE() WHERE PK1='" + idTabulador + "'";
                if (db.execute(sql)) { return true; } else { return false; }

            }
            catch
            {
                return false;
            }
        }

        public bool Delete()
        {
            try
            {
                sql = "DELETE FROM   TABULADOR WHERE PK1=" + idTabulador;
                if (db.execute(sql)) { return true; } else { return false; }
               
            }
            catch
            {
                return false;
            }
        }

    }

}

