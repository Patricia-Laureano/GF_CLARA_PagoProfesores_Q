using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectDB;
using System.Data.SqlClient;
using Session;
using System.Diagnostics;
using System.Collections;
using PagoProfesores.Models.Helper;
using System.Configuration;

namespace PagoProfesores.Models.CatalogosCentrales
{
    public class MySuiteModel : SuperModel
    {

        public string requestor { get; set; }
        public string transaction { get; set; }
        public string country { get; set; }
        public string entity { get; set; }
        public string user { get; set; }
        public string username { get; set; }
        public string campusCode { get; set; }
        public string sucursal { get; set; }


        public bool Add()
        {
            
            try
            {
                string server = ConfigurationManager.AppSettings["serverMySuite"];//TEST O PROD
                string FECHA_R = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sql = "INSERT INTO TIMBRADO_MYSUITE(";
                sql += " SERVER ";
                sql += ", REQUESTOR ";
                sql += ", XTRANSACTION ";
                sql += ", COUNTRY ";
                sql += ", RFCENTITY ";
                sql += ", USER_R ";
                sql += ", USERNAME ";
                sql += ", CAMP_CODE ";
                sql += ", SUCURSAL ";
                sql += ", FECHA_R ";
                sql += ") VALUES(";
                sql += "'" + server + "'";
                sql += ",'" + requestor + "'";
                sql += ",'" + transaction + "'";
                sql += ",'" + country + "'";
                sql += ",'" + entity + "'";
                sql += ",'" + user + "'";
                sql += ",'" + username + "'";
                sql += ",'" + campusCode + "'";
                sql += ",'" + sucursal + "'";
                sql += ",'" + FECHA_R + "'";
                sql += ")";   

                //Debug.WriteLine("sql add MYSUITE: " + sql);
                if (db.execute(sql))
                {
                    Log.write(this, "MYSUITE add", LOG.REGISTRO, "SQL:" + sql, sesion);     
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

            string server = ConfigurationManager.AppSettings["serverMySuite"];//TEST O PROD

            try
            {
                string sql = "SELECT * FROM TIMBRADO_MYSUITE WHERE CAMP_CODE='" + campusCode + "' AND SERVER='" + server +"';";  //ID DEL PRIMERO REG
            //    Debug.WriteLine("sql edit MYSUITE: " + sql);
                ResultSet res = db.getTable(sql);
                if (res.Next())
                {                 

                    requestor = res.Get("REQUESTOR");
                    transaction = res.Get("XTRANSACTION");
                    country = res.Get("COUNTRY");
                    entity = res.Get("RFCENTITY");
                    user = res.Get("USER_R");
                    username = res.Get("USERNAME");
                    campusCode = res.Get("CAMP_CODE");
                    sucursal = res.Get("SUCURSAL");

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

            string server = ConfigurationManager.AppSettings["serverMySuite"];//TEST O PROD
            try
            {
                string sql = "UPDATE TIMBRADO_MYSUITE SET ";              
                sql += "REQUESTOR= '" + requestor + "'";
                sql += ",XTRANSACTION= '" + transaction + "'";
                sql += ",COUNTRY= '" + country + "'";
                sql += ",RFCENTITY= '" + entity + "'";
                sql += ",USER_R= '" + user + "'";
                sql += ",USERNAME= '" + username + "'";
                sql += ",CAMP_CODE= '" + campusCode + "'";
                sql += ",SUCURSAL= '" + sucursal + "'";

                sql += " WHERE CAMP_CODE='" + campusCode + "' AND SERVER='" + server + "';" ;  //EDITAR
                if (db.execute(sql))
                {
                    Log.write(this, "MySuite Save", LOG.REGISTRO, "SQL:" + sql, sesion);      //EDITAR
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




    }
}