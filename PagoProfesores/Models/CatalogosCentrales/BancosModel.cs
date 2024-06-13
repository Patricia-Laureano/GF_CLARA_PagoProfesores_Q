using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ConnectDB;

namespace PagoProfesores.Models.CatalogosporSede
{
    public class BancosModel : SuperModel
    {
       
        public string Clave { get; set; }
        public string Banco { get; set; }
        public string Transferencias { get; set; }

        public string sql { get; set; } //update

        public BancosModel()
        {

        }

        public bool Add()
        {
            try
            {
                sql = "INSERT INTO BANCOS(CVE_BANCO,BANCO,TRANSFERENCIAS,USUARIO) VALUES('" + Clave + "','" + Banco + "','" + Transferencias + "','" + this.sesion.nickName + "')";
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
                sql = "SELECT * FROM BANCOS WHERE CVE_BANCO ='" + Clave + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Clave = res.Get("CVE_BANCO");
                    Banco = res.Get("BANCO");
                    Transferencias = res.Get("TRANSFERENCIAS");
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
                sql = "UPDATE BANCOS SET ";
                sql += "BANCO = '" + Banco + "'";
                sql += ",TRANSFERENCIAS = '" + Transferencias + "'";
                sql += ",USUARIO = '" + this.sesion.nickName + "'";
                sql += ",FECHA_M = GETDATE()";
                sql += " WHERE CVE_BANCO=" + Clave; 
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
                sql = "DELETE FROM BANCOS WHERE CVE_BANCO = '" + Clave + "'";    //update quitar string
                if (db.execute(sql)) { return true; } else { return false; }
            }
            catch
            {
                return false;
            }
        }
    }
}