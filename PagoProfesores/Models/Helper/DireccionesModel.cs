using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class DireccionesModel : SuperModel
    {
        public string codigo { get; set; }
        public string pais { get; set; }
        public string estado { get; set; }
        public string ciudad { get; set; }
        public string municipio { get; set; }
        public string colonia { get; set; }
       
        

        public Dictionary<string, string> getPaises(string pais = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CODIGO,PAIS FROM PAISES";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CODIGO"), res.Get("PAIS"));

            return dict;
        }


        public Dictionary<string, string> getEstados(string pais = "", string estado = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT d_estado as d , d_estado FROM QSEPOMEX_ESTADOS where PAIS='" + pais + "' order by d_estado";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("d"), res.Get("d_estado"));

            return dict;
        }


        public Dictionary<string, string> getCiudades(string pais = "", string estado = "", string ciudad = "", string municipio = "")
            {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT d_ciudad as d , d_ciudad FROM QSEPOMEX_CIUDADES WHERE d_estado = '" + estado + "'";

            if (municipio != "" && municipio != null)
            {
                sql += " and d_ciudad in (select d_ciudad "
                     + "                    from QSEPOMEX_DELEGACIONES "
                     + "                   where D_mnpio = '" + municipio + "') ";
            }

            sql += " Order by d_ciudad ";

            ResultSet res = db.getTable(sql);

            while (res.Next())
                dict.Add(res.Get("d"), res.Get("d_ciudad"));

            return dict;
        }


        public Dictionary<string, string> getMunicipios(string pais = "", string estado = "", string ciudad = "", string municipio = "")
            {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT distinct D_mnpio AS d, D_mnpio FROM QSEPOMEX_DELEGACIONES WHERE  d_estado = '" + estado + "'";
            if (ciudad != "" && ciudad != null)
            {
                sql += " AND d_ciudad = '" + ciudad + "' ";
            }

            sql += " Order by D_mnpio";

            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("d"), res.Get("D_mnpio"));

            return dict;
        }
        
        public List<string> getColonias(string pais = "", string estado = "", string ciudad = "", string municipio = "", string cp="")
        {
            List<string> list = new List<string>();

            string sql = "SELECT d_codigo,d_asenta as d, d_asenta FROM QSEPOMEX_COLONIAS WHERE d_estado = '" + estado + "'  and D_mnpio = '" + municipio + "' ";
            sql += (cp.Equals("")) ? "" : "AND d_codigo='" + cp + "' ";
            sql += "order by d_asenta";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                list.Add(res.Get("d_asenta"));

            return list;
        }

        public bool getPaisEstadoCiudadMunicipio()
        {
            string sql = "SELECT PAIS,d_estado,d_ciudad,D_mnpio FROM SEPOMEX WHERE d_codigo = '" + codigo + "' group by PAIS,d_estado,d_ciudad,D_mnpio";
            ResultSet res = db.getTable(sql);

            if (res.Next())
            {
                pais = res.Get("PAIS");
                estado = res.Get("d_estado");
                ciudad = res.Get("d_ciudad");
                municipio = res.Get("D_mnpio");
                return true;
            }
            else { return false; }
            
        }


        public bool getCP()
        {
            string sql = "SELECT d_codigo FROM SEPOMEX WHERE d_estado = '" + estado + "' AND d_ciudad ='" + ciudad + "' AND D_mnpio ='" + municipio + "' AND d_asenta ='" + colonia + "' ";
            ResultSet res = db.getTable(sql);

            if (res.Next())
            {
                codigo = res.Get("d_codigo");
             
                return true;
            }
            else { return false; }

        }

    

}

}