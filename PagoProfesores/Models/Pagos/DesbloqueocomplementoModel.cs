using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{
    public class DesbloqueocomplementoModel : SuperModel
    {


        public string IdEdoCta { get; set; }
        public int Publicar { get; set; }
        public string fechai { get; set; }
        public string fechaf { get; set; }
        public string sede { get; set; }
        public string sql { get; set; } //update
      
        public string search { get; set; }     
        public string Fechadepago { get; set; }
        public string Concepto { get; set; }
        public string IDSIU { get; set; }
        public string periodo { get; set; }
        public string esquema { get; set; }



        public bool desbloquearComplemento_all()
        {
            try
            {
                List<string> filtros = new List<string>();
                List<string> Searchfiltros = new List<string>();

                if (sede != "" && sede != null) filtros.Add("CVE_SEDE = '" + sede + "'");

                if (fechai != "" && fechai != null) filtros.Add("CAST(FECHADEPAGO AS DATE) >= '" + fechai + "'");

                if (fechaf != "" && fechaf != null) filtros.Add("CAST(FECHADEPAGO AS DATE) <= '" + fechaf + "'");             


                if (search != "" && search != null)
                {
                    Searchfiltros.Add("(PERIODO LIKE '%" + search + "%')");
                    Searchfiltros.Add("(ESQUEMA LIKE '%" + search + "%')");
                    Searchfiltros.Add("(CONCEPTO LIKE '%" + search + "%')");
                    Searchfiltros.Add("(IDSIU LIKE '%" + search + "%')");

                }

                string union = "";
                if (filtros.Count > 0) { union = " WHERE " + string.Join<string>(" AND ", filtros.ToArray()); }

                string filtrounion = "";
                if (Searchfiltros.Count > 0) { filtrounion = " AND (" + string.Join<string>(" OR ", Searchfiltros.ToArray()) + ")";  }   

                

                sql = " SELECT *  FROM VDESBLOQUEO_COMPLEMENTO  " + union + filtrounion + " ORDER BY IDSIU";//FECHAPAGO,PKCONCEPTOPAGO
                ResultSet res = db.getTable(sql);


                if (res.Count > 0)
                {
                    while (res.Next())
                    {
                        if (res.Get("PADRE") == "0")//***checar
                        {
                            //Detalle Estado de Cuenta
                            sql = "SELECT * FROM VESTADO_CUENTA_DETALLE WHERE ID_PERSONA = " + res.Get("ID_PERSONA") + " AND ID_ESTADODECUENTA = " + res.GetLong("ID_ESTADODECUENTA");
                            ResultSet detalle = db.getTable(sql);

                            while (detalle.Next())
                            {
                                string sql = "DELETE FROM ESTADODECUENTA_DETALLE_BLOQUEOS" +
                                      " WHERE ID_EDOCTADETALLE = " + detalle.Get("ID_EDOCTADETALLE") +
                                      " AND ID_ESQUEMA = " + detalle.Get("ID_ESQUEMA") +
                                      " AND PKCONCEPTOPAGO = " + detalle.Get("PKCONCEPTOPAGO") +
                                      " AND CVE_BLOQUEO = 'COM' ";

                                db.execute(sql);

                            }



                        }


                    }


                }


            }
            catch
            {
                return false;
            }

            return true;

        }



        }
}