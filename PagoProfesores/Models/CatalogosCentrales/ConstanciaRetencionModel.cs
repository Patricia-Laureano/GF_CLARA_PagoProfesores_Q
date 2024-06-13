using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.CatalogosporSede
{
    public class ConstanciaRetencionModel : SuperModel
    {
        public String Clave { get; set; }
        public String Contrato { get; set; }
        public string Formato { get; set; }
        public String sql { get; set; }


        public bool Add()
        {

            try
            {
                sql = "INSERT INTO FORMATORETENCIONES(CVE_RETENCION,RETENCION,Formato,Usuario) VALUES('" + Clave + "','" + Contrato + "','" + Formato + "','" + this.sesion.nickName + "')";
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
                sql = "SELECT * FROM FORMATORETENCIONES WHERE CVE_RETENCION='" + Clave + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Clave = res.Get("CVE_RETENCION");
                    Contrato = res.Get("RETENCION");
                    Formato = res.Get("FORMATO");
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
                sql = "UPDATE FORMATORETENCIONES SET RETENCION = '" + Contrato + "', Formato='" + Formato + "' ,USUARIO = '" + this.sesion.nickName + "',FECHA_M = GETDATE() WHERE CVE_RETENCION='" + Clave + "'";
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
                sql = "DELETE FROM FORMATORETENCIONES WHERE CVE_RETENCION='" + Clave + "'";
                if (db.execute(sql)) { return true; } else { return false; }

            }
            catch
            {
                return false;
            }
        }

    }
}