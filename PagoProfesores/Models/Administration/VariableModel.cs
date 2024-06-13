using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ConnectDB;
namespace PagoProfesores.Models.Administration
{
    public class VariableModel : SuperModel
    {


        public string Variable { get; set; }
        public float Valor { get; set; }
        public string Descripcion { get; set; }

        public string sql { get; set; }

      
        public VariableModel()
        {

        }


        public bool Edit()
        {
            try
            {
                sql = "SELECT * FROM VARIABLES WHERE VARIABLE = '" + Variable + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Variable = res.Get("VARIABLE");
                    Valor = res.GetFloat("VALOR");
                    Descripcion = res.Get("DESCRIPCION");                   
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
                sql = "UPDATE VARIABLES SET ";
              //  sql += "VARIABLE = " + Variable;
                sql += "VALOR = " + Valor;
                sql += ",DESCRIPCION = '" + Descripcion + "'";
             //   sql += ",Porcentajeexcedente = " + PorcentajeExcedente;
                sql += ",Usuario = '" + this.sesion.nickName + "'";
                sql += ",Fecha_m = " + "GETDATE()";
                sql += " WHERE VARIABLE = '" + Variable + "'";
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
                sql = "DELETE FROM VARIABLES WHERE VARIABLE = " + Variable + "";
                if (db.execute(sql)) { return true; } else { return false; }
            }
            catch
            {
                return false;
            }
        }





    }
}