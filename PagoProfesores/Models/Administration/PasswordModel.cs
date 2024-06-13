using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectDB;
using System.Data.SqlClient;
using Session;
using System.Diagnostics;

namespace PagoProfesores.Models
{
    public class PasswordModel : SuperModel
    {
        public string passwordActual { get; set; }
        public string usuario { get; set; }       
        public string password { get; set; }


             

        public bool ValidaPassword(string passwordencript)
        {
            try
            {
                string sql = "SELECT PK1 FROM USUARIOS WHERE USUARIO = '" + usuario + "' AND PASSWORD = '" + passwordencript + "'";
                Debug.WriteLine("valida sql: "+sql);
                ResultSet res = db.getTable(sql);

                if (res.Next())               
                    return true;
               
                else
                    return false; 
            }
            catch
            {
                return false;
            }
        }

        public bool ActualizaPassword(string passwordencript, string passworactualdencript)
         {
             try
             {
                 string sql = "UPDATE USUARIOS SET PASSWORD ='" + passwordencript + "' WHERE USUARIO = '" + usuario + "' AND PASSWORD = '" + passworactualdencript + "'";
                Debug.WriteLine("actualiza sql: " + sql);
                if (db.execute(sql)) { return true; } else { return false; }

             }
             catch
             {
                 return false;
             }
         }




    }


}