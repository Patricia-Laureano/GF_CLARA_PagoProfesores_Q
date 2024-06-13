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
    public class TiposPagoModel : SuperModel
    {
        [Required(ErrorMessage = "Clave es requerido")]

        public string Cve_TipoFactura { get; set; }
        public string Cve_TipodePago { get; set; }
        public string TipodePago { get; set; }
        public string TipodePagoDescripcion { get; set; }

        public string sql { get; set; } 

        public TiposPagoModel()
        {

        }



        public string ComboSql(string Sql, string cve, string valor)
        {
            string MySql = Sql;
            string Combo = "\r\n";

            ResultSet reader = db.getTable(MySql);
            try
            {
                while (reader.Next())
                {
                    Combo = Combo + "<option value =\"" + reader.Get(cve) + "\" >";
                    Combo += reader.Get(valor) + " </ option >\r\n";
                }
                return Combo;
            }
            catch
            {
                return "Error en consulta combo";
            }
        }

        public bool Add()
        {
            try
            {
                sql = "INSERT INTO TIPOSDEPAGO(";

                sql += "Cve_TipoFactura";
                sql += ",Cve_TipodePago";
                sql += ",TipodePago";
                sql += ",TipodePagoDescripcion";
                sql += ",Usuario";

                sql += ") VALUES(";
                sql += "'" + Cve_TipoFactura + "'";
                sql += ",'"+ Cve_TipodePago + "'";
                sql += ",'" + TipodePago + "'";
                sql += ",'" + TipodePagoDescripcion + "'";
                sql += ",'" + this.sesion.nickName + "'";

                sql += ")";
                if (db.execute(sql))
                {
                    Log.write(this, "Add", LOG.REGISTRO, "SQL:" + sql, sesion);
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
                sql = "SELECT * FROM TIPOSDEPAGO WHERE Cve_TipodePago = '" + Cve_TipodePago + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Cve_TipoFactura =                   res.Get("CVE_TIPOFACTURA");
                    Cve_TipodePago =                    res.Get("CVE_TIPODEPAGO");
                    TipodePago =                        res.Get("TIPODEPAGO");
                    TipodePagoDescripcion =             res.Get("TIPODEPAGODESCRIPCION");
                    
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

        public bool Save()
        {
            try
            {
                sql = "UPDATE TIPOSDEPAGO SET ";

                sql += "Cve_TipoFactura = '" + Cve_TipoFactura + "'";
                sql += ",TipodePago = '" + TipodePago + "'";
                sql += ",TipodePagoDescripcion = '" + TipodePagoDescripcion + "'";
                sql += ",Usuario = '" + this.sesion.nickName + "'";
                sql += ",Fecha_M = " + "GETDATE()" + "";

                sql += " WHERE Cve_TipodePago = '" + Cve_TipodePago + "'";
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
                sql = "DELETE FROM TIPOSDEPAGO WHERE Cve_TipodePago = '" + Cve_TipodePago + "'";
                if (db.execute(sql)) { return true; } else { return false; }
            }
            catch
            {
                return false;
            }
        }

    }
}