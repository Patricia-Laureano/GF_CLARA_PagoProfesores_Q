using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectDB;
using System.Data.SqlClient;
using Session;
using System.Diagnostics;

namespace PagoProfesores.Models.Administration
{
    public class ProfileModel : SuperModel
    {
        public long idusuario { get; set; }
        public string usuario { get; set; }   
        public string nombre { get; set; }
        public string apellidop { get; set; }
        public string apellidom { get; set; }
        public string rfc { get; set; }
        public string edad { get; set; }

        public string telefono{ get; set; }
   
        public string cp { get; set; }
        public string pais { get; set; }
        public string estado { get; set; }
        public string mundel { get; set; }
        public string colonia { get; set; }
        public string calle { get; set; }
        public string numero { get; set; }
     

        public bool getDatosUser()
        {
            try
            {
                string sql = "SELECT * FROM USUARIOS WHERE PK1= '" + idusuario + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    nombre = res.Get("NOMBRE");
                    apellidop = res.Get("APATERNO");
                    apellidom = res.Get("AMATERNO");
                    rfc = res.Get("RFC");
                    edad = res.Get("EDAD");
                    return true;
                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }


        public bool getTelefono(char tipo)
        {
            try
            {
                string sql = "SELECT * FROM USUARIOS_TELEFONOS WHERE USUARIO = '" + usuario + "' AND TIPO = '"+tipo+"'";
                Debug.WriteLine("sql tel: "+ sql);
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    telefono = res.Get("TELEFONO");                    
                    return true;
                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }


        public bool Obtener_Direccion()
        {
            try
            {
                string sql = "SELECT * FROM USUARIOS_DIRECCION WHERE USUARIO = '" + usuario + "'";
                Debug.WriteLine("sql direccion: " + sql);
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    cp = res.Get("CP");
                    pais = res.Get("PAIS");
                    estado = res.Get("ESTADO");
                    mundel = res.Get("MUNDEL");
                    colonia = res.Get("COLONIA");
                    calle = res.Get("CALLE");
                    numero = res.Get("NUMEXT");
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