using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ConnectDB;
using System.Diagnostics;
namespace PagoProfesores.Models.Pagos

{
    public class DesbloqueosModel : SuperModel
    {
        public string[] arrBloqueos { get; set; }
        public string IDSIU { get; set; }
        public string sql { get; set; }
        public string IdEdoCtaD { get; set; }
        public string IdEsquema { get; set; }
        public string NumPago { get; set; }





        public bool bloquear(string data, string bloqueos)
        {


            bool ok = true;
            string[] arrChecked = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] array = bloqueos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
              
                ResultSet res = null;
                ResultSet result_bloqueos = null;          


                if (bloqueos != "")
                {
                    foreach (string ID_ESTADODECUENTA in arrChecked)
                    {

                        sql = "SELECT * " +
                         "  FROM VESTADO_CUENTA_DETALLE " +
                         " WHERE ID_ESTADODECUENTA = " + ID_ESTADODECUENTA;

                         res = db.getTable(sql);

                        if (res.Count > 0)
                        {
                            while (res.Next())
                            {

                                IdEdoCtaD = res.Get("ID_EDOCTADETALLE");
                                IdEsquema = res.Get("ID_ESQUEMA");
                                NumPago = res.Get("PKCONCEPTOPAGO");
                             
                                foreach (string item in array)
                                {

                                    sql = "SELECT ID_EDOCTABLOQDETALLE " +
                                     "  FROM ESTADODECUENTA_DETALLE_BLOQUEOS " +
                                     " WHERE ID_EDOCTADETALLE = " + IdEdoCtaD + " AND CVE_BLOQUEO = '"+ item + "'";

                                    result_bloqueos = db.getTable(sql);

                                    if (result_bloqueos.Count == 0)                                        
                                    {
                                        sql =
                                       "INSERT INTO ESTADODECUENTA_DETALLE_BLOQUEOS (ID_EDOCTADETALLE, ID_ESQUEMA, PKCONCEPTOPAGO, CVE_BLOQUEO)" +
                                       " VALUES(" + IdEdoCtaD + "," + IdEsquema + "," + NumPago + ", '" + item + "')";
                                        ok = ok && db.execute(sql);

                                    }
                                   
                                }

                            }

                        }


                    }
                }


            }
            catch { return ok; }
            

            return ok;
        }




        public bool desBloquear(string data, string bloqueos)
        {


            bool ok = true;
            string[] arrChecked = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] array = bloqueos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                ResultSet res = null;      


                if (bloqueos != "")
                {
                    foreach (string ID_ESTADODECUENTA in arrChecked)
                    {

                        sql = "SELECT * " +
                         "  FROM VESTADO_CUENTA_DETALLE " +
                         " WHERE ID_ESTADODECUENTA = " + ID_ESTADODECUENTA;

                        res = db.getTable(sql);

                        if (res.Count > 0)
                        {
                            while (res.Next())
                            {

                                IdEdoCtaD = res.Get("ID_EDOCTADETALLE");
                                IdEsquema = res.Get("ID_ESQUEMA");
                                NumPago = res.Get("PKCONCEPTOPAGO");
                               
                                foreach (string item in array)
                                {
                                    sql = "DELETE FROM ESTADODECUENTA_DETALLE_BLOQUEOS WHERE ID_EDOCTADETALLE = " + IdEdoCtaD 
                                    + "AND ID_ESQUEMA = "+ IdEsquema + " AND PKCONCEPTOPAGO = " + NumPago + " AND CVE_BLOQUEO = '" + item + "'";
                                    ok = ok && db.execute(sql);                                

                                }

                            }

                        }


                    }
                }


            }
            catch { return ok; }


            return ok;
        }



        public void eliminarBloqueos(string[] arrChecked)
        {


            foreach (string ID_ESTADODECUENTA in arrChecked)
            {

                sql = "SELECT * " +
                 "  FROM VESTADO_CUENTA_DETALLE " +
                 " WHERE ID_ESTADODECUENTA = " + ID_ESTADODECUENTA;

                ResultSet res = db.getTable(sql);

                if (res.Count > 0)
                {
                    while (res.Next())
                    {
                        IdEdoCtaD = res.Get("ID_EDOCTADETALLE");

                        sql = "DELETE FROM ESTADODECUENTA_DETALLE_BLOQUEOS WHERE ID_EDOCTADETALLE = " + IdEdoCtaD;
                        db.execute(sql);

                    }

                }


            }


        }


        /*  public bool EditBloqueos()
          {
              try
              {
                  sql = "select * " +
                        "  from ESTADODECUENTA_DETALLE_BLOQUEOS " +
                        " where ID_EDOCTADETALLE = " + IdEdoCtaD;

                  ResultSet res = db.getTable(sql);
                  List<string> listBloqueos = new List<string>();
                  while (res.Next())
                      listBloqueos.Add(res.Get("CVE_BLOQUEO"));

                  arrBloqueos = listBloqueos.ToArray<string>();

                  return true;
              }
              catch {; }
              return false;
          }*/




    }
}