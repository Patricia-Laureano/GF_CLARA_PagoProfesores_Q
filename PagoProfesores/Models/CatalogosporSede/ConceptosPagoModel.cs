using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.CatalogosporSede
{
    public class ConceptosPagoModel : SuperModel
    {
        public string Concepto { get; set; }
        public string Descripcion { get; set; }
        public string Sede { get; set; }

        public string sql { get; set; } //update


        public bool Add()
        {
            try
            {
                sql = "INSERT INTO CONCEPTOSDEPAGO(CVE_SEDE,CONCEPTO,CONCEPTO_DES,USUARIO) VALUES('"+ Sede + "','" + Concepto + "','" + Descripcion + "','" + this.sesion.nickName + "')"; //Update quitar string
                if (db.execute(sql))
                {
                    return true;    //Update quitar log
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
                sql = "SELECT * FROM CONCEPTOSDEPAGO WHERE CONCEPTO = '" + Concepto + "'"; //update quitar string
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Concepto = res.Get("CONCEPTO");
                    Descripcion = res.Get("CONCEPTO_DES");
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
                sql = "UPDATE CONCEPTOSDEPAGO SET ";
                sql += "CONCEPTO_DES = '" + Descripcion + "'";
                sql += ",USUARIO = '" + this.sesion.nickName + "'";
                sql += ",FECHA_M = GETDATE()";
                sql += " WHERE CONCEPTO= '" + Concepto+"'";
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
                sql = "DELETE FROM CONCEPTOSDEPAGO WHERE CONCEPTO ='" + Concepto+"'";    //update quitar string
                if (db.execute(sql)) { return true; } else { return false; }
            }
            catch
            {
                return false;
            }
        }


    }
}