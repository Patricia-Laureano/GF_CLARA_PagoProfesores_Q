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
    public class EscuelasCCModel : SuperModel
    {

        public string sql { get; set; }
        public string idEscuela { get; set; }
        public string Sede { get; set; }
        public string tipo { get; set; }

        public EscuelasCCModel()
        {
        }


        public bool Save()
        {
            try
            {
                  sql = "UPDATE CENTRODECOSTOS_ESCUELAS SET ";
                //  sql += "CVE_SEDE = '" + Sede + "'";
                     //  sql += ",CVE_ESCUELA = '" + periodos + "'";
                  sql += "TIPO = '" + tipo + "'";
                  sql += " WHERE CVE_ESCUELA = '" + idEscuela + "' AND CVE_SEDE = '"+ Sede + "'";                   

                  if (db.execute(sql)) { return true; } else { return false; }
             
            }
            catch
            {
                return false;
            }
        }



    }
}