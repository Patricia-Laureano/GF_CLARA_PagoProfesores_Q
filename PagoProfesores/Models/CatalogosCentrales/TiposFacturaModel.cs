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
    public class TiposFacturaModel : SuperModel
    {
        [Required(ErrorMessage = "Clave es requerido")]
        public string Cve_TipoFactura { get; set; }
        public string TipoFactura { get; set; }
        public string TipoFacturaDescripcion { get; set; }

        public string sql { get; set; } //update

        public TiposFacturaModel()
        {

        }
        public bool Add()
        {
            try
            {
                sql = "INSERT INTO TIPOSFACTURA(";
                sql += "CVE_TIPOFACTURA";
                sql += ",TIPOFACTURA";
                sql += ",TIPOFACTURADESCRIPCION";
                sql += ",USUARIO";

                sql += ") VALUES(";
                sql += "'"+ Cve_TipoFactura + "'";
                sql += ",'" + TipoFactura + "'";
                sql += ",'" + TipoFacturaDescripcion + "'";
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
                sql = "SELECT * FROM TIPOSFACTURA WHERE CVE_TIPOFACTURA = '" + Cve_TipoFactura + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Cve_TipoFactura =                   res.Get("CVE_TIPOFACTURA");
                    TipoFactura =                       res.Get("TIPOFACTURA");
                    TipoFacturaDescripcion =            res.Get("TIPOFACTURADESCRIPCION");
                    
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
                sql = "UPDATE TIPOSFACTURA SET ";

                sql += "TIPOFACTURA = '" + TipoFactura + "'";
                sql += ",TIPOFACTURADESCRIPCION = '" + TipoFacturaDescripcion + "'";
                sql += ",USUARIO = '" + this.sesion.nickName + "'";
                sql += ",FECHA_M = " + "GETDATE()" + "";

                sql += " WHERE CVE_TIPOFACTURA = '" + Cve_TipoFactura + "'";
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
                sql = "DELETE FROM TIPOSFACTURA WHERE CVE_TIPOFACTURA = '" + Cve_TipoFactura + "'";
                if (db.execute(sql)) { return true; } else { return false; }
            }
            catch
            {
                return false;
            }
        }

    }
}