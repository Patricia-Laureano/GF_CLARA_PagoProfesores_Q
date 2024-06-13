using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{
    public class GestiondePagosModel : SuperModel
    {
        public string IdEdoCta { get; set; }
        public int Publicar { get; set; }
        public string fechai { get; set; }
        public string fechaf { get; set; }
        public string sede { get; set; }
        public string sql { get; set; } //update
        public string sql2 { get; set; } //update
        public string sql3{ get; set; } //update

        public string ids { get; set; }
        public string Concepto { get; set; }
        public string Fechadepago { get; set; }
        public string IDSIU { get; set; }
        public string nombres { get; set; }



        public void init()
        {
            IdEdoCta = "";

        }


         public bool publicar()
        {
            try
            {
                List<string> filtros = new List<string>();
                string strm_fecha_pu = "";
                /*
                if (Publicar == 1)                
                    strm_fecha_pu = ",FECHA_PUBLICACION = GETDATE()";
                else
                    strm_fecha_pu = ",FECHA_PUBLICACION = NULL";
              */

                if (sede != "" && sede != null) filtros.Add("CVE_SEDE = '" + sede + "'");

                if (fechai != "" && fechai != null) filtros.Add("CAST(FECHADEPAGO AS DATE) >= '" + fechai + "'");

                if (fechaf != "" && fechaf != null) filtros.Add("CAST(FECHADEPAGO AS DATE) <= '" + fechaf + "'");

                if (IdEdoCta != "" && IdEdoCta != null) filtros.Add("ID_ESTADODECUENTA = '" + IdEdoCta + "'");

                string union = "";
                if ( filtros.Count > 0) { union = " WHERE " + string.Join<string>(" AND ", filtros.ToArray()); }

                sql2 = "UPDATE ESTADODECUENTA SET PUBLICADO = '" + Publicar + "' WHERE ID_ESTADODECUENTA IN ";
                sql = "(SELECT ID_ESTADODECUENTA FROM ESTADODECUENTA_DETALLE ";
                //sql += "PUBLICADO = '" + Publicar + "'";             
                sql += strm_fecha_pu;
                sql += " "+ union +") ";
                sql3 = sql2 + sql;

                if (db.execute(sql3)) {                    
                    return true;
                } else {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }



        public bool PublicarDespublicar_Seleccionados()
        {
            try
            {
                List<string> filtros = new List<string>();
                string strm_fecha_pu = "";

                if (sede != "" && sede != null) filtros.Add("CVE_SEDE = '" + sede + "'");

                //  if (fechai != "" && fechai != null) filtros.Add("CAST(FECHADEPAGO AS DATE) >= '" + fechai + "'");

                //   if (fechaf != "" && fechaf != null) filtros.Add("CAST(FECHADEPAGO AS DATE) <= '" + fechaf + "'");

                //  if (IdEdoCta != "" && IdEdoCta != null) filtros.Add("ID_ESTADODECUENTA = '" + IdEdoCta_ + "'");



                string[] arrChecked = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


                string union = "";
                if (filtros.Count > 0) { union = " WHERE " + string.Join<string>(" AND ", filtros.ToArray()); }

                

                foreach (string IdEdoCta_ in arrChecked)
                {


                    sql2 = "UPDATE ESTADODECUENTA SET PUBLICADO = '" + Publicar + "' WHERE ID_ESTADODECUENTA IN ";
                        sql = "(SELECT ID_ESTADODECUENTA FROM ESTADODECUENTA_DETALLE ";                            
                        sql += strm_fecha_pu;
                        sql += " " + union + " AND ID_ESTADODECUENTA = '"+ IdEdoCta_ + "') ";
                        sql3 = sql2 + sql;

                        db.execute(sql3);                   
                     
                 }                

              return true;

         }
            catch
            {
                return false;
            }

        }




        public bool DatosPersona()
        {
            try
            {
                List<string> filtros = new List<string>();

                if (sede != "" && sede != null) filtros.Add("CVE_SEDE = '" + sede + "'");

                if (fechai != "" && fechai != null) filtros.Add("CAST(FECHAPAGO AS DATE) >= '" + fechai + "'");

                if (fechaf != "" && fechaf != null) filtros.Add("CAST(FECHAPAGO AS DATE) <= '" + fechaf + "'");

                if (IdEdoCta != "" && IdEdoCta != null) filtros.Add("ID_ESTADODECUENTA = '" + IdEdoCta + "'");

                string union = "";
                if (filtros.Count > 0) { union = " WHERE " + string.Join<string>(" AND ", filtros.ToArray()); }



                sql = "SELECT * FROM QGestionPagosPublicado  " + union + "";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Concepto = res.Get("CONCEPTO");
                    Fechadepago = res.Get("FECHAPAGO");
                    IDSIU = res.Get("IDSIU");
                    nombres = res.Get("NOMBRES") + " " + res.Get("APELLIDOS");
                    return true;
                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }


    }
}