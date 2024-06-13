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
    public class ContratosWebModel : SuperModel
    {
        public String Clave { get; set; }
        public String Contrato { get; set; }
        public String Descripcion { get; set; }
        public string Formato { get; set; } 
        public String sql { get; set; }

        public ContratosWebModel() { }


        public bool Add()
        {

            try
            {
                sql = "INSERT INTO FORMATOCONTRATOS(Cve_Contrato,Contrato,Contrato_Descripcion,Formato,Usuario) VALUES('" + Clave + "','" + Contrato + "','" + Descripcion + "','" + Formato + "','" + this.sesion.nickName + "')";
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
                sql = "SELECT * FROM FORMATOCONTRATOS WHERE Cve_Contrato='" + Clave + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Clave = res.Get("CVE_CONTRATO");
                    Contrato = res.Get("CONTRATO");
                    Descripcion = res.Get("CONTRATO_DESCRIPCION");
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
                sql = "UPDATE FORMATOCONTRATOS SET Contrato = '" + Contrato + "', Contrato_Descripcion='" + Descripcion + "', Formato='" + Formato + "' ,USUARIO = '" + this.sesion.nickName + "',FECHA_M = GETDATE() WHERE Cve_Contrato='" + Clave + "'";
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
                sql = "DELETE FROM FORMATOCONTRATOS WHERE Cve_Contrato='" + Clave + "'";
                if (db.execute(sql)) { return true; } else { return false; }

            }
            catch
            {
                return false;
            }
        }
    }
}