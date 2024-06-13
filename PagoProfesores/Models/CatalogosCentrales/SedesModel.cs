using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ConnectDB;
using System.Data;
using System.Reflection;

namespace PagoProfesores.Models.CatalogosporSede
{
    public class SedesModel : SuperModel
    {
        [Required(ErrorMessage = "Clave es requerido")]

        public string Cve_Sociedad { get; set; }
        public string Cve_Sede { get; set; }
        public string Sede { get; set; }
        public string Campus_Inb { get; set; }
        public string TipoContrato_Inb { get; set; }

        public string Direccion_Pais { get; set; }
        public string Direccion_Estado { get; set; }
        public string Direccion_Ciudad { get; set; }
        public string Direccion_Entidad { get; set; }
        public string Direccion_Colonia { get; set; }
        public string Direccion_Calle { get; set; }
        public string Direccion_CP { get; set; }
        public string Direccion_Numero { get; set; }
        public string Nombre_Responsable { get; set; }
        public string Correo_Responsable { get; set; }
        public string Telefono_Responsable { get; set; }
        public string Sociedad_A_Mostrar { get; set; }

        public string Nombre_sociedad { get; set; }

        public string sql { get; set; } //update


        public SedesModel()
        {

        }
        public string ComboSql(string Sql, string cve, string valor)
        {
            string MySql = Sql;
            string Combo = "\r\n";

            ResultSet reader = db.getTable(MySql);
            try
            {
                while (reader.Next())
                {
                    Combo = Combo + "<option value =\"" + reader.Get(cve) + "\" >";
                    Combo += reader.Get(valor) + " </ option >\r\n";
                }
                return Combo;
            }
            catch
            {
                return "Error en consulta combo";
            }
        }

        public bool Add()
        {
            try
            {
                sql = "INSERT INTO SEDES(";

                sql += "Cve_Sociedad";
                sql += ",Cve_Sede";
                sql += ",Sede";
                sql += ",Campus_Inb";
                sql += ",TipoContrato_Inb";
                sql += ",Usuario";
                sql += ",DIRECCION_PAIS";
                sql += ",DIRECCION_ESTADO";
                sql += ",DIRECCION_CIUDAD";
                sql += ",DIRECCION_ENTIDAD";
                sql += ",DIRECCION_COLONIA";
                sql += ",DIRECCION_CALLE";
                sql += ",DIRECCION_NUMERO";
                sql += ",DIRECCION_CP";
                sql += ",CORREO_RESPONSABLE";
                sql += ",NOMBRE_RESPONSABLE";
                sql += ",TELEFONO_RESPONSABLE";
                sql += ",SOCIEDAD_A_MOSTRAR"; 
                 sql += ") VALUES(";
                sql += "'" + Cve_Sociedad + "'";
                sql += ",'" + Cve_Sede + "'";
                sql += ",'" + Sede + "'";
                sql += ",'" + Campus_Inb + "'";
                sql += ",'" + TipoContrato_Inb + "'";
                sql += ",'" + this.sesion.nickName + "'";
                sql += ",'" + Direccion_Pais + "'";
                sql += ",'" + Direccion_Estado + "'";
                sql += ",'" + Direccion_Ciudad + "'";
                sql += ",'" + Direccion_Entidad + "'";
                sql += ",'" + Direccion_Colonia + "'";
                sql += ",'" + Direccion_Calle + "'";
                sql += ",'" + Direccion_Numero + "'";
                sql += ",'" + Direccion_CP + "'";
                sql += ",'" + Correo_Responsable + "'";
                sql += ",'" + Nombre_Responsable + "'";
                sql += ",'" + Telefono_Responsable + "'";
                sql += ",'" + Sociedad_A_Mostrar + "'";
                sql += ")";
                if (db.execute(sql))
                {
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
                sql = "SELECT * FROM SEDES WHERE Cve_Sede = '" + Cve_Sede + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Cve_Sociedad = res.Get("CVE_SOCIEDAD");
                    Cve_Sede = res.Get("CVE_SEDE");
                    Sede = res.Get("SEDE");
                    Campus_Inb = res.Get("CAMPUS_INB");
                    TipoContrato_Inb = res.Get("TIPOCONTRATO_INB");
                    Direccion_CP = res.Get("DIRECCION_CP");
                    Direccion_Pais = res.Get("DIRECCION_PAIS");
                    Direccion_Estado = res.Get("DIRECCION_ESTADO");
                    Direccion_Ciudad = res.Get("DIRECCION_CIUDAD");
                    Direccion_Entidad = res.Get("DIRECCION_ENTIDAD");
                    Direccion_Colonia = res.Get("DIRECCION_COLONIA");
                    Direccion_Calle = res.Get("DIRECCION_CALLE");
                    Direccion_Numero = res.Get("DIRECCION_NUMERO");
                    Nombre_Responsable = res.Get("NOMBRE_RESPONSABLE");
                    Correo_Responsable = res.Get("CORREO_RESPONSABLE");
                    Telefono_Responsable = res.Get("TELEFONO_RESPONSABLE");
                    Nombre_sociedad = res.Get("NOMBRE_SOCIEDADES");
                    Sociedad_A_Mostrar = res.Get("SOCIEDAD_A_MOSTRAR");

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
            string nombres = string.Empty;
        
            try
            {
                var Cve_SociedadArr = Cve_Sociedad.Split(',');
              

                foreach (var clave in Cve_SociedadArr)
                {
                    sql = "SELECT * FROM SOCIEDADES WHERE CVE_SOCIEDAD='" + clave + "'";
                    ResultSet res = db.getTable(sql);
                    if (res.Next())
                    {
                        if (nombres == string.Empty)
                        {
                            nombres = res.Get("SOCIEDAD");
                        }
                        else
                        {
                            nombres = nombres + " | " + res.Get("SOCIEDAD");
                        }
                    }
                   

                }
            }
            catch
            {

            }

            try
            {
                sql = "UPDATE SEDES SET ";

                sql += "Cve_Sociedad = '" + Cve_Sociedad + "'";
                sql += ",Sede = '" + Sede + "'";
                sql += ",Campus_Inb = '" + Campus_Inb + "'";
                sql += ",TipoContrato_Inb = '" + TipoContrato_Inb + "'";
                sql += ",Usuario = '" + this.sesion.nickName + "'";
                sql += ",Fecha_M = " + "GETDATE()" + "";
                sql += ",Direccion_CP = '" + Direccion_CP + "'";
                sql += ",Direccion_Pais = '" + Direccion_Pais + "'";
                sql += ",Direccion_Estado = '" + Direccion_Estado + "'";
                sql += ",Direccion_Ciudad = '" + Direccion_Ciudad + "'";
                sql += ",Direccion_Entidad = '" + Direccion_Entidad + "'";
                sql += ",Direccion_Colonia = '" + Direccion_Colonia + "'";
                sql += ",Direccion_Calle = '" + Direccion_Calle + "'";
                sql += ",Direccion_Numero = '" + Direccion_Numero + "'";
                sql += ",Nombre_Responsable = '" + Nombre_Responsable + "'";
                sql += ",Correo_Responsable = '" + Correo_Responsable + "'";
                sql += ",Telefono_Responsable = '" + Telefono_Responsable + "'";
                sql += ",NOMBRE_SOCIEDADES = '" + nombres + "'";
                sql += ",SOCIEDAD_A_MOSTRAR = '" + Sociedad_A_Mostrar + "'";
                sql += " WHERE Cve_Sede = '" + Cve_Sede + "';";
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
                sql = "DELETE FROM SEDES WHERE Cve_Sede = '" + Cve_Sede + "'";
                if (db.execute(sql)) { return true; } else { return false; }
            }
            catch
            {
                return false;
            }
        }

        public string ListadoSociedades()
        {
            try
            {
                sql = "Select * from SOCIEDADES order by SOCIEDAD";
                ResultSet res = db.getTable(sql);

                while (res.Next())
                {
                    if (Nombre_sociedad == null)
                    {
                        Nombre_sociedad += res.Get("CVE_SOCIEDAD") + "|" + res.Get("SOCIEDAD");
                    }
                    else
                    {
                        Nombre_sociedad +="/" + res.Get("CVE_SOCIEDAD") + "|" + res.Get("SOCIEDAD");
                    }            
                }
                 return Nombre_sociedad;
            }
            catch { return Nombre_sociedad; }
        }

        public string SociedadesFechas()
        {
            try
            {
                sql = "Select * from SOCIEDADES order by SOCIEDAD";
                ResultSet res = db.getTable(sql);

                while (res.Next())
                {
                    if (Nombre_sociedad == null)
                    {
                        Nombre_sociedad += res.Get("CVE_SOCIEDAD") + "|" + res.Get("SOCIEDAD");
                    }
                    else
                    {
                        Nombre_sociedad += "/" + res.Get("CVE_SOCIEDAD") + "|" + res.Get("SOCIEDAD");
                    }
                }
                return Nombre_sociedad;
            }
            catch { return Nombre_sociedad; }
        }

        public string MostrarSociedades(string Cve_Sede)
        {
            try
            {
                sql = "Select * FROM SOCIEDADES ORDER BY SOCIEDAD";

                ResultSet res = db.getTable(sql);

                while (res.Next())
                {
                    if (Nombre_sociedad == null)
                    {
                        Nombre_sociedad += res.Get("CVE_SOCIEDAD") + "|" + res.Get("SOCIEDAD");
                    }
                    else
                    {
                        Nombre_sociedad += "/" + res.Get("CVE_SOCIEDAD") + "|" + res.Get("SOCIEDAD");
                    }
                }
                return Nombre_sociedad;
            }
            catch { return Nombre_sociedad; }
        }
    }
}