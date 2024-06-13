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

namespace PagoProfesores.Models
{
    public class SociedadesModel : SuperModel
    {
        public string Cve_Sociedad { get; set; }
        public string Sociedad { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string Direccion_CP { get; set; }
        public string Direccion_Pais { get; set; }
        public string Direccion_Estado { get; set; }
        public string Direccion_Ciudad { get; set; }
        public string Direccion_Entidad { get; set; }
        public string Direccion_Colonia { get; set; }
        public string Direccion_Calle { get; set; }
        public string Direccion_Numero { get; set; }
        public string Email_Sociedad { get; set; }

        public string RepresentanteLegal { get; set; }
        public string RL_RFC { get; set; }
        public string RL_CURP { get; set; }
        public string RL_Direccion_Pais { get; set; }
        public string RL_Direccion_Estado { get; set; }
        public string RL_Direccion_Ciudad { get; set; }
        public string RL_Direccion_Entidad { get; set; }
        public string RL_Direccion_Colonia { get; set; }
        public string RL_Direccion_Calle { get; set; }
        public string RL_Direccion_Numero { get; set; }
        public string RL_Direccion_CP { get; set; }

        public string Firma { get; set; }
        public string Firma_Img { get; set; }
        public string Sello { get; set; }
        public string Sello_Img { get; set; }

        public string numinstnotarial { get; set; }
        public string volumen { get; set; }
        public string ciudadnotarial { get; set; }
        public string numeronotariopub { get; set; }
        public string nombrenotariopublico { get; set; }
        public string representantelegap { get; set; }


        public string requestor { get; set; }
        public string transaction { get; set; }
        public string country { get; set; }
        public string entity { get; set; }
        public string user { get; set; }
        public string username { get; set; }
        public string data1 { get; set; }
        public string data2 { get; set; }
        public string data3 { get; set; }
        public string mensaje { get; set; }
        public string file1 { get; set; }
        public string file2 { get; set; }
        public string success { get; set; }
        public string archivo { get; set; }
        public string uuid { get; set; }

        public string sepomex_codigo { get; set; }
        public string sepomex_estado { get; set; }
        public string sepomex_municipio { get; set; }
        public string sepomex_ciudad { get; set; }
        public string sepomex_dir { get; set; }

        public DateTime fechai { get; set; }
        public string Strmfechai { get; set; }
        public DateTime fechaf { get; set; }
        public string Strmfechaf { get; set; }
        public string Cve_Entidad { get; set; }
        public string sql { get; set; }
        public string CPValida { get; set; }
        public string RazonValida { get; set; }
        public string RFCValida { get; set; }
        public string Cve_LlaveMySuite { get; set; }
        public int Cve_LlaveMySuite_ID { get; set; }

        public List<string> direcciones = new List<string>();

        public bool Add()
        {
            try
            {
                string sql = "INSERT INTO Sociedades(";
                sql += "Cve_Sociedad";
                sql += ",Sociedad";
                sql += ",RFC";
                sql += ", CURP ";
                sql += ", Direccion_CP ";
                sql += ", Direccion_Pais ";
                sql += ", Direccion_Estado ";
                sql += ", Direccion_Ciudad ";
                sql += ", Direccion_Entidad ";
                sql += ", Direccion_Colonia ";
                sql += ", Direccion_Calle ";
                sql += ", Direccion_Numero ";
                sql += ", Email_Soc ";
                sql += ", RepresentanteLegal ";
                sql += ", RL_RFC ";
                sql += ", RL_CURP ";
                sql += ", RL_Direccion_Pais ";
                sql += ", RL_Direccion_Estado ";
                sql += ", RL_Direccion_Ciudad ";
                sql += ", RL_Direccion_Entidad ";
                sql += ", RL_Direccion_Colonia ";
                sql += ", RL_Direccion_Calle ";
                sql += ", RL_Direccion_Numero ";
                sql += ", RL_Direccion_CP ";
                sql += ", Firma ";
                sql += ", Firma_Img ";
                sql += ", Sello ";
                sql += ", Sello_Img ";
                sql += ", NOTARIAL_NO ";
                sql += ", NOTARIAL_VOLUMEN ";
                sql += ", NOTARIAL_LUGAR ";
                sql += ", NOTARIAL_NOTARIO_NO ";
                sql += ", NOTARIAL_LIC ";
                sql += ", CVE_ENTFED ";
                sql += ", CP_VALIDA ";
                sql += ", LLAVESMYSUITE ";
                sql += ") VALUES (";
                sql += "'" + Cve_Sociedad + "'";
                sql += ",'" + Sociedad + "'";
                sql += ",'" + RFC + "'";
                sql += ",'" + CURP + "'";
                sql += ",'" + Direccion_CP + "'";
                sql += ",'" + Direccion_Pais + "'";
                sql += ",'" + Direccion_Estado + "'";
                sql += ",'" + Direccion_Ciudad + "'";
                sql += ",'" + Direccion_Entidad + "'";
                sql += ",'" + Direccion_Colonia + "'";
                sql += ",'" + Direccion_Calle + "'";
                sql += ",'" + Direccion_Numero + "'";
                sql += ",'" + Email_Sociedad + "'";

                sql += ",'" + RepresentanteLegal + "'";
                sql += ",'" + RL_RFC + "'";
                sql += ",'" + RL_CURP + "'";
                sql += ",'" + RL_Direccion_Pais + "'";
                sql += ",'" + RL_Direccion_Estado + "'";
                sql += ",'" + RL_Direccion_Ciudad + "'";
                sql += ",'" + RL_Direccion_Entidad + "'";
                sql += ",'" + RL_Direccion_Colonia + "'";
                sql += ",'" + RL_Direccion_Calle + "'";
                sql += ",'" + RL_Direccion_Numero + "'";
                sql += ",'" + RL_Direccion_CP + "'";

                sql += ",'" + Firma + "'";
                sql += ",'" + Firma_Img + "'";
                sql += ",'" + Sello + "'";
                sql += ",'" + Sello_Img + "'";
                sql += ",'" + numinstnotarial + "'";
                sql += ",'" + volumen + "'";
                sql += ",'" + ciudadnotarial + "'";
                sql += ",'" + numeronotariopub + "'";
                sql += ",'" + nombrenotariopublico + "'";
                sql += ",'" + Cve_Entidad + "'";

                sql += ",'" + CPValida + "'";
                sql += "," + Cve_LlaveMySuite_ID;
                sql += ")";

                Debug.WriteLine("sql add sociedad: " + sql);
                if (db.execute(sql))
                {
                    Log.write(this, "SOCIEDADES add", LOG.REGISTRO, "SQL:" + sql, sesion);      //EDITAR
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Edit()
        {
            try
            {
                string sql = "SELECT * FROM Sociedades WHERE Cve_Sociedad='" + Cve_Sociedad + "'";
                Debug.WriteLine("sql edit sociedad: " + sql);
                ResultSet res = db.getTable(sql);
                if (res.Next())
                {
                    Cve_Sociedad = res.Get("CVE_SOCIEDAD");
                    Sociedad = res.Get("SOCIEDAD");
                    RFC = res.Get("RFC");
                    CURP = res.Get("CURP");
                    Direccion_CP = res.Get("DIRECCION_CP");
                    Direccion_Pais = res.Get("DIRECCION_PAIS");
                    Direccion_Estado = res.Get("DIRECCION_ESTADO");
                    Direccion_Ciudad = res.Get("DIRECCION_CIUDAD");
                    Direccion_Entidad = res.Get("DIRECCION_ENTIDAD");
                    Direccion_Colonia = res.Get("DIRECCION_COLONIA");
                    Direccion_Calle = res.Get("DIRECCION_CALLE");
                    Direccion_Numero = res.Get("DIRECCION_NUMERO");
                    Email_Sociedad = res.Get("EMAIL_SOC");
                    Cve_Entidad = res.Get("CVE_ENTFED");

                    RepresentanteLegal = res.Get("REPRESENTANTELEGAL");
                    RL_RFC = res.Get("RL_RFC");
                    RL_CURP = res.Get("RL_CURP");
                    RL_Direccion_Pais = res.Get("RL_DIRECCION_PAIS");
                    RL_Direccion_Estado = res.Get("RL_DIRECCION_ESTADO");
                    RL_Direccion_Ciudad = res.Get("RL_DIRECCION_CIUDAD");
                    RL_Direccion_Entidad = res.Get("RL_DIRECCION_ENTIDAD");
                    RL_Direccion_Colonia = res.Get("RL_DIRECCION_COLONIA");
                    RL_Direccion_Calle = res.Get("RL_DIRECCION_CALLE");
                    RL_Direccion_Numero = res.Get("RL_DIRECCION_NUMERO");
                    RL_Direccion_CP = res.Get("RL_DIRECCION_CP");

                    Firma = res.Get("FIRMA");
                    Firma_Img = res.Get("FIRMA_IMG");
                    Sello = res.Get("SELLO");
                    Sello_Img = res.Get("SELLO_IMG");

                    numinstnotarial = res.Get("NOTARIAL_NO");
                    volumen = res.Get("NOTARIAL_VOLUMEN");
                    ciudadnotarial = res.Get("NOTARIAL_LUGAR");
                    numeronotariopub = res.Get("NOTARIAL_NOTARIO_NO");
                    nombrenotariopublico = res.Get("NOTARIAL_LIC");

                    try
                    {
                        Strmfechai = res.GetDateTime("FECHAINICIO").ToString("yyyy-MM-dd");
                    }
                    catch
                    {
                        Strmfechai = null;
                    }

                    try
                    {
                        Strmfechaf = res.GetDateTime("FECHAFIN").ToString("yyyy-MM-dd");
                    }
                    catch
                    {
                        Strmfechaf = null;
                    }

                    CPValida = res.Get("CP_VALIDA");

                    Cve_LlaveMySuite_ID = int.Parse(res.Get("LLAVESMYSUITE"));

                    return true;
                }
                else return false;
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
                string sql = "UPDATE Sociedades SET ";
                sql += "Sociedad            = '" + Sociedad + "'";
                sql += ",RFC                = '" + RFC + "'";
                sql += ",CURP               = '" + CURP + "'";
                sql += ",Direccion_CP       = '" + Direccion_CP + "'";
                sql += ",Direccion_Pais     = '" + Direccion_Pais + "'";
                sql += ",Direccion_Estado   = '" + Direccion_Estado + "'";
                sql += ",Direccion_Ciudad   = '" + Direccion_Ciudad + "'";
                sql += ",Direccion_Entidad  = '" + Direccion_Entidad + "'";
                sql += ",Direccion_Colonia  = '" + Direccion_Colonia + "'";
                sql += ",Direccion_Calle    = '" + Direccion_Calle + "'";
                sql += ",Direccion_Numero   = '" + Direccion_Numero + "'";
                sql += ",Email_Soc          = '" + Email_Sociedad + "'";

                sql += ",RepresentanteLegal   = '" + RepresentanteLegal + "'";
                sql += ",RL_RFC               = '" + RL_RFC + "'";
                sql += ",RL_CURP              = '" + RL_CURP + "'";
                sql += ",RL_Direccion_Pais    = '" + RL_Direccion_Pais + "'";
                sql += ",RL_Direccion_Estado  = '" + RL_Direccion_Estado + "'";
                sql += ",RL_Direccion_Ciudad  = '" + RL_Direccion_Ciudad + "'";
                sql += ",RL_Direccion_Entidad = '" + RL_Direccion_Entidad + "'";
                sql += ",RL_Direccion_Colonia = '" + RL_Direccion_Colonia + "'";
                sql += ",RL_Direccion_Calle   = '" + RL_Direccion_Calle + "'";
                sql += ",RL_Direccion_Numero  = '" + RL_Direccion_Numero + "'";
                sql += ",RL_Direccion_CP      = '" + RL_Direccion_CP + "'";

                sql += ",Firma                = '" + Firma + "'";
                sql += ",Firma_Img            = '" + Firma_Img + "'";
                sql += ",Sello                = '" + Sello + "'";
                sql += ",Sello_Img            = '" + Sello_Img + "'";

                sql += ",NOTARIAL_NO         = '" + numinstnotarial + "'";
                sql += ",NOTARIAL_VOLUMEN    = '" + volumen + "'";
                sql += ",NOTARIAL_NOTARIO_NO = '" + numeronotariopub + "'";
                sql += ",NOTARIAL_LUGAR      = '" + ciudadnotarial + "'";
                sql += ",NOTARIAL_LIC        = '" + nombrenotariopublico + "'";

                sql += ",FECHAINICIO = '" + fechai.ToString("yyyy-MM-dd") + "'";
                sql += ",FECHAFIN = '" + fechaf.ToString("yyyy-MM-dd") + "'";

                sql += ",CVE_ENTFED        = '" + Cve_Entidad + "'";

                sql += ",CP_VALIDA        = '" + CPValida + "'";

                sql += ",LLAVESMYSUITE        = '" + Cve_LlaveMySuite_ID + "'";

                sql += " WHERE Cve_Sociedad  = '" + Cve_Sociedad + "'";
                if (db.execute(sql))
                {
                    Log.write(this, "SOCIEDADES Save", LOG.REGISTRO, "SQL:" + sql, sesion);
                    return true;
                }
                else return false;
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
                string sql = "DELETE FROM Sociedades WHERE Cve_Sociedad ='" + Cve_Sociedad + "'";
                if (db.execute(sql))
                {
                    Log.write(this, "SOCIEDADES Delete", LOG.REGISTRO, "SQL:" + sql, sesion);
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public string ListadoEntidadFederativa()
        {
            try
            {
                sql = "Select * from ENTIDADES_FEDERATIVAS order by CVE_ESTADO";
                ResultSet resQ = db.getTable(sql);
                while (resQ.Next())
                {
                    if (Cve_Entidad == null)
                    {
                        Cve_Entidad += resQ.Get("CVE_ESTADO") + "|" + resQ.Get("NOMBRE_ESTADO");
                    }
                    else
                    {
                        Cve_Entidad += "/" + resQ.Get("CVE_ESTADO") + "|" + resQ.Get("NOMBRE_ESTADO");
                    }
                }
                return Cve_Entidad;
            }
            catch { return Cve_Entidad; }
        }

        public string ListadoLlavesMySuite(string server)
        {
            try
            {

                sql = "Select * from TIMBRADO_MYSUITE WHERE SERVER='" + server + "' ORDER BY CAMP_CODE";
                ResultSet resQ = db.getTable(sql);
                while (resQ.Next())
                {
                    if (Cve_LlaveMySuite == null)
                    {
                        Cve_LlaveMySuite += resQ.Get("PK") + "|" + resQ.Get("CAMP_CODE") + "|" + resQ.Get("RFCENTITY");
                    }
                    else
                    {
                        Cve_LlaveMySuite += "/" + resQ.Get("PK") + "|" + resQ.Get("CAMP_CODE") + "|" + resQ.Get("RFCENTITY");
                    }
                }
                return Cve_LlaveMySuite;
            }
            catch { return Cve_LlaveMySuite; }
        }
    }
}