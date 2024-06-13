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
    public class PublicacionConstanciasModel : SuperModel
    {

        

        public string idPConsMens { get; set; }
        public string Anio { get; set; }
        public string Mes { get; set; }     
        public DateTime Fecha_Publicacion { get; set; }
        public string Sede { get; set; }

        public string idPConsAnual { get; set; }

        


        public string Strm_Fecha_Publicacion { get; set; }

        public bool existe { get; set; } //update


        public string sql { get; set; }


        public PublicacionConstanciasModel()
        {
        }


        public bool Add()
        {

            try
            {

                if (!Existe())
                {

                    existe = true;


                    sql = "INSERT INTO PUBLICACION_RETENCIONES_MENSUALES(CVE_SEDE,CVE_CICLO,MES,FECHA_PUBLICACION) VALUES( '" + Sede + "','" + Anio + "','" + Mes + "','" + Fecha_Publicacion.ToString("yyyy-MM-dd") + "')";
                    if (db.execute(sql))                    
                           return true;                    
                    else                   
                        return false;   


                }else
                {

                    //existe = false;

                    if (Save())
                        return true;
                    else
                        return true;                
                   

                }

            }
            catch
            {
                return false;
            }
        }




        public bool Existe()
        {
            try
            {

             //   string Fecha = Fecha_Publicacion == SuperModel.minDateTime ? "NULL" : ("'" + Fecha_Publicacion.ToString("yyyy-MM-dd") + "'");


               /* string FiltroFecha = "";
                if (!string.IsNullOrWhiteSpace(Fecha_Publicacion.ToString("yyyy-MM-dd")))
                    FiltroFecha = " AND FECHA_PUBLICACION = '" + Fecha_Publicacion.ToString("yyyy-MM-dd") + "'";*/


                sql = "SELECT * FROM PUBLICACION_RETENCIONES_MENSUALES WHERE CVE_SEDE= '" + Sede + "' AND CVE_CICLO =  '" + Anio + "' AND MES = '" + Mes + "'";
                    ResultSet publica = db.getTable(sql);


              /*  idPConsMens = "102";
                return true;*/

             

               if (publica.Next()){
                    idPConsMens = publica.Get("PK1");
                    return true;
                }              

                 else { return false; }



            }
            catch { return false; }
        }





        public bool Edit()
        {
            try
            {

                sql = "SELECT * FROM PUBLICACION_RETENCIONES_MENSUALES WHERE CVE_SEDE= '" + Sede + "' AND CVE_CICLO =  '" +Anio + "' AND MES = '" + Mes + "' ";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    idPConsMens = res.Get("PK1");
                   // Fecha_Publicacion = res.GetDateTime("FECHA_PUBLICACION");
                     Strm_Fecha_Publicacion = res.GetDateTime("FECHA_PUBLICACION").ToString("yyyy-MM-dd");

                    return true;
                }
                else {
                    Strm_Fecha_Publicacion = "";
                    idPConsMens = "";

                    return true;
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

                sql = "UPDATE PUBLICACION_RETENCIONES_MENSUALES SET  CVE_SEDE = '" + Sede + "', CVE_CICLO = '" + Anio + "', FECHA_PUBLICACION = '" + Fecha_Publicacion.ToString("yyyy-MM-dd") + "' ,MES = '" + Mes + "' WHERE PK1='" + idPConsMens + "'";
                if (db.execute(sql)) { return true; } else { return false; }

            }
            catch
            {
                return false;
            }
        }


        /***********************************************************************Retenciones Anuales***************************************************/



        public bool Add_A()
        {

            try
            {

                if (!Existe_A())
                {

                   // existe = true;

                    sql = "INSERT INTO PUBLICACION_RETENCIONES_ANUALES(CVE_SEDE,CVE_CICLO,FECHA_PUBLICACION) VALUES( '" + Sede + "','" + Anio + "','" + Fecha_Publicacion.ToString("yyyy-MM-dd") + "')";
                    if (db.execute(sql))
                        return true;
                    else
                        return false;


                }
                else
                {

                  //  existe = false;

                    if (Save_A())
                        return true;
                    else
                        return true;


                }

            }
            catch
            {
                return false;
            }
        }




        public bool Existe_A()
        {
            try
            {

                //   string Fecha = Fecha_Publicacion == SuperModel.minDateTime ? "NULL" : ("'" + Fecha_Publicacion.ToString("yyyy-MM-dd") + "'");


               /* string FiltroFecha = "";
                if (!string.IsNullOrWhiteSpace(Fecha_Publicacion.ToString("yyyy-MM-dd")))
                    FiltroFecha = " AND FECHA_PUBLICACION = '" + Fecha_Publicacion.ToString("yyyy-MM-dd") + "'";*/


                sql = "SELECT * FROM PUBLICACION_RETENCIONES_ANUALES WHERE CVE_SEDE= '" + Sede + "' AND CVE_CICLO = '" + Anio + "'";
                ResultSet publica = db.getTable(sql);


                /*  idPConsMens = "102";
                  return true;*/



                if (publica.Next())
                {
                    idPConsAnual = publica.Get("PK1");
                    return true;
                }

                else { return false; }



            }
            catch { return false; }
        }





        public bool Edit_A()
        {
            try
            {

                sql = "SELECT * FROM PUBLICACION_RETENCIONES_ANUALES WHERE CVE_SEDE= '" + Sede + "' AND CVE_CICLO =  '" + Anio + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    idPConsMens = res.Get("PK1");
                    // Fecha_Publicacion = res.GetDateTime("FECHA_PUBLICACION");
                    Strm_Fecha_Publicacion = res.GetDateTime("FECHA_PUBLICACION").ToString("yyyy-MM-dd");

                    return true;
                }
                else
                {
                    Strm_Fecha_Publicacion = "";
                    idPConsAnual = "";

                    return true;
                }

            }
            catch
            {
                return false;
            }
        }

        public bool Save_A()
        {
            try
            {

                sql = "UPDATE PUBLICACION_RETENCIONES_ANUALES SET  CVE_SEDE = '" + Sede + "', CVE_CICLO = '" + Anio + "', FECHA_PUBLICACION = '" + Fecha_Publicacion.ToString("yyyy-MM-dd") + "' WHERE PK1='" + idPConsAnual + "'";
                if (db.execute(sql)) { return true; } else { return false; }

            }
            catch
            {
                return false;
            }
        }


        


    }
}