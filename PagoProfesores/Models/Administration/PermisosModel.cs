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
    public class PermisosModel : SuperModel
    {

        public int idPermiso { get; set; }
        public string Permiso { get; set; }
        public string Description { get; set; }
        public string Tipo { get; set; }
        public int idrole { get; set; }
        public string role { get; set; }
        public string permisos { get; set; }


        public bool permitirPermiso() {

            string sql = "";
            try
            {                
                char[] separator = { '-' };
                string[] permiso = permisos.Split(separator);

                for (int i=0; i< permiso.Length; i++)
                {
                    this.eliminarPermiso(idrole, permiso[i]);

                    sql = "INSERT INTO ROLES_PERMISOS (PK_ROL,PK_PERMISO) VALUES(" + idrole + "," + permiso[i] + ")";
                    Debug.WriteLine("sql insert: " + sql);
                    
                    if (db.execute(sql))                    
                        Log.write(this, "permitirPermiso", LOG.REGISTRO, "SQL: " + sql, sesion);          
                    else
                        return false;
                }              
                
            }
            catch
            { return false;}

            return true;

        }


        public bool restringirPermiso()
        {
            string sql = "";
            try
            {
                char[] separator = { '-' };
                string[] permiso = permisos.Split(separator);

                for (int i = 0; i < permiso.Length; i++)
                {
                    sql = "DELETE FROM ROLES_PERMISOS WHERE PK_ROL = " + idrole + " AND PK_PERMISO = " + permiso[i];
                    Debug.WriteLine("sql insert: " + sql);

                    if (db.execute(sql))
                        Log.write(this, "restringirPermiso", LOG.REGISTRO, "SQL: " + sql, sesion);
                    else
                        return false;
                }

            }
            catch
            { return false; }

            return true;

        }



        private void eliminarPermiso(int idrole, String permiso)
        {
            string sql = "DELETE FROM ROLES_PERMISOS WHERE PK_ROL = " + idrole + " AND PK_PERMISO = " + permiso;
            bool res = db.execute(sql);

            if (res == true)
               Log.write(this, "eliminarPermiso", LOG.REGISTRO, "SQL: " + sql, sesion);           

        }





        public bool Add()
        {

            try
            {
                
                string sql = "INSERT INTO PERMISOS(PERMISO,DESCRIPCION) VALUES('" + Permiso + "','" + Description + "')";
                Debug.WriteLine("sql insert: "+sql);


                if (db.execute(sql))
                {
                    Log.write(this, "Start", LOG.REGISTRO, "SQL:" + sql, sesion);
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
            try
            {
                string sql = "SELECT * FROM PERMISOS WHERE PK1=" + idPermiso + "";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                   
                    Permiso = res.Get("PERMISO");
                    Description = res.Get("DESCRIPCION");
                   
                    return true;
                }
                else { return false; }

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
                //string sql = "UPDATE PERMISOS SET Permiso ='" + Permiso + "',DESCRIPCION= '" + Description + "',TIPO= '" +  Tipo + "' WHERE PK1=" + idPermiso;
                string sql = "UPDATE PERMISOS SET Permiso ='" + Permiso + "',DESCRIPCION= '" + Description + "'  WHERE PK1=" + idPermiso;
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
                string sql = "DELETE FROM PERMISOS WHERE PK1=" + idPermiso;
                if (db.execute(sql)) { return true; } else { return false; }

            }
            catch
            {
                return false;
            }
        }



        public bool Consulta_Titulo_Rol()
        {
            try
            {
                string sql = "SELECT ROLE FROM ROLES WHERE PK1=" + idrole + "";

                Debug.WriteLine("sql tit rol: " + sql);
                ResultSet res = db.getTable(sql);

                

                if (res.Next())
                {                    
                    role = res.Get("ROLE");                    
                 
                    return true;
                }
                else { return false; }

            }
            catch
            {
                return false;
            }
        }


        public bool existePermiso(String rol, String permiso)
        {

            int max = 0;
            try
            {

                String sql = "SELECT COUNT(PK1) AS 'MAX' FROM ROLES_PERMISOS WHERE PK_ROL = '" + rol + "' AND PK_PERMISO = '" + permiso + "' ";

                Debug.WriteLine("sql roles_permisos: " + sql);
                ResultSet res = db.getTable(sql);

                if (res.Next())
                    max = int.Parse(res.Get("MAX"));

            }
            catch (Exception ex) { Log.write(this, "existePermiso", LOG.REGISTRO, "SQL:" + ex, sesion); }

            if (max <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }







    }//end class

}//end namespace