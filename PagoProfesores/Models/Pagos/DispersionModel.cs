using System;
using System.Collections.Generic;
using ConnectDB;
using PagoProfesores.Models.Reports;
using System.Data;

namespace PagoProfesores.Models.Pagos
{
    public class DispersionModel : SuperModel
    {	
        public string fechai { get; set; }
        public string fechaf { get; set; }
        public string FechaDispersion { get; set; }
        public string IdTransferencia { get; set; }//tipodispersion
        public string IdSede { get; set; }
        public string banco { get; set; }
        public Boolean Existe { get; set; }
        public string cve_tipodepago { get; set; }
        public string sql { get; set; }
        
        /**********/
        //Reportes

        public string IDSIU { get; set; }
        public string esquema { get; set; }
        public string concepto { get; set; }

        /**********/
        public List<string> getPDFs()
        {
            List<string> list = new List<string>();
            List<string> filtros = new List<string>();

            if (IdSede != "") filtros.Add("CVE_SEDE = '" + IdSede + "'");

            if (fechai != "") filtros.Add("CAST(FECHAPAGO AS DATE) >= '" + fechai + "'");

            if (fechaf != "") filtros.Add("CAST(FECHAPAGO AS DATE) <= '" + fechaf + "'");

            if (cve_tipodepago != "") filtros.Add("CVE_TIPOFACTURA = '" + cve_tipodepago + "'");

            if (cve_tipodepago == "A") filtros.Add(("(FECHAEMISION <> '' AND FECHAEMISION IS NOT NULL)") );
            
            string CONDICIONSQL = string.Join<string>(" AND ", filtros.ToArray());

            sql = "SELECT PDF FROM VESTADO_CUENTA WHERE (FECHARECIBO <> '' AND FECHARECIBO IS NOT NULL)  AND (PDF IS NOT NULL AND CAST(PDF AS nvarchar(MAX) ) <> '') AND " + CONDICIONSQL + " ORDER BY IDSIU ASC, ID_ESTADODECUENTA DESC";
            ResultSet res = db.getTable(sql);

            while (res.Next())            
                list.Add(res.Get("PDF"));           
            
            return list;
        }
        
        public Dictionary<string, FacturasModel> getPDFs_Honorarios()
        {
            List<string> list = new List<string>();
            List<string> filtros = new List<string>();

            if (IdSede != "") filtros.Add("CVE_SEDE = '" + IdSede + "'");

            if (fechai != "") filtros.Add("CAST(FECHAPAGO AS DATE) >= '" + fechai + "'");

            if (fechaf != "") filtros.Add("CAST(FECHAPAGO AS DATE) <= '" + fechaf + "'");

            if (cve_tipodepago != "") filtros.Add("CVE_TIPOFACTURA = '" + cve_tipodepago + "'");

            if (cve_tipodepago == "A") filtros.Add(("(FECHAEMISION <> '' AND FECHAEMISION IS NOT NULL)"));
            
            string CONDICIONSQL = string.Join<string>(" AND ", filtros.ToArray());

            sql = "SELECT ID_ESTADODECUENTA,PDF,IDSIU,ESQUEMA,CONCEPTO,PERIODO FROM VESTADO_CUENTA WHERE (FECHARECIBO <> '' AND FECHARECIBO IS NOT NULL)  AND (PDF IS NOT NULL AND CAST(PDF AS nvarchar(MAX) ) <> '') AND " + CONDICIONSQL + " ORDER BY IDSIU ASC, ID_ESTADODECUENTA DESC";
            ResultSet res = db.getTable(sql);
            
            IDSIU = "";
            esquema = "";
            concepto = "";

            Dictionary<string, FacturasModel> dict = new Dictionary<string, FacturasModel>();

            while (res.Next())
                dict.Add(res.Get("ID_ESTADODECUENTA"), new FacturasModel(res.Get("ID_ESTADODECUENTA"), res.Get("PDF"), res.Get("IDSIU"), res.Get("ESQUEMA"),res.Get("CONCEPTO"), res.Get("PERIODO")));
            
            return dict;
        }

        public bool Save()
        {

            string sql2 = "";

            try
            {
                sql2 = "SELECT * FROM VDISPERSION_TMP WHERE USUARIO = " + sesion.pkUser;
                ResultSet  rs = db.getTable(sql2);

                if (rs != null)
                {
                    while (rs.Next())
                    {
                        sql = "UPDATE ESTADODECUENTA SET" +
                            " FECHADISPERSION = GETDATE()" +
                             "WHERE ID_ESTADODECUENTA = '" + rs.Get("ID_ESTADODECUENTA") + "'";

                        db.execute(sql);

                    }
                    return true;
                }
            } catch(Exception e) { return false; }

            return false;
        }
        
        public bool DeleteFechaD()
        {
            try
            {
                string sql = "";
                Existe = true;

                sql = "SELECT * FROM VDISPERSION_TMP WHERE USUARIO = " + sesion.pkUser;
                ResultSet rs = db.getTable(sql);

                if (rs != null && rs.Count > 0)
                {
                    while (rs.Next())
                    {
                        sql = "UPDATE ESTADODECUENTA SET" +
                            " FECHADISPERSION = NULL" +
                            " WHERE ID_ESTADODECUENTA = '" + rs.Get("ID_ESTADODECUENTA") + "'";
                        db.execute(sql);
                    }
                }
                else
                {
                    Existe = false;
                    return true;
                }
                return true;
            }
            catch { return false; }
        }
        
        public bool DeleteTablaTemporal()
        {
            string sql1 = "";
            try
            {               
                sql1 = "DELETE FROM DISPERSION_TMP WHERE USUARIO=" + sesion.pkUser;
                if (db.execute(sql1)) { return true; } else { return false; }               
            }
            catch (Exception e) { return false; }
        }
        
        public bool TablaTemporalVacia(long usuario)
        {
            string sq = "";
            try
            {     
                sq = "SELECT * FROM DISPERSION_TMP WHERE USUARIO=" + usuario;
                ResultSet rs = db.getTable(sq);

                if (rs != null && rs.Count > 0)                   
                {
                    DeleteTablaTemporal();
                    return false;
                }
                  
                else
                    return true;
            }

            catch (Exception e) { return false; }
        }
        
        public ResultSet getRowsCoincidencias()
        {
            ResultSet res = null;

            try
            {       
                sql = "select IDSIU, CONCEPTO, BANCOS, count(IDSIU)AS NUM_CONCIDENCIAS"; 
                sql += " from VDISPERSION_TMP WHERE USUARIO  =" + sesion.pkUser;
                sql += " group by IDSIU, CONCEPTO, BANCOS having count(IDSIU) > 1 AND count(CONCEPTO) >1 AND count(BANCOS) >1";
                res = db.getTable(sql);

                return res;

            }
            catch (Exception e)
            {
                return res;
            }
        }
        
        public ResultSet getDispersion(string IDSIU, string concepto, string banco)
        {
            ResultSet res = null;

            try
            {       
                sql = "select * from VDISPERSION_TMP";
                sql += " WHERE USUARIO  =" + sesion.pkUser + " AND IDSIU = '"+ IDSIU + "'";
                sql += " AND CONCEPTO  = '" + concepto + "' AND BANCOS = '" + banco + "'";
                res = db.getTable(sql);

                return res;
            }
            catch (Exception e)
            {
                return res;
            }
        }

        public Dictionary<string, string> getBanco()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_BANCO, BANCO FROM BANCOS ORDER BY CVE_BANCO";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_BANCO"), res.Get("BANCO"));

            return dict;
        }

        public Dictionary<string, string> getTipoDispersion()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_TIPOTRANSFERENCIA, TIPOTRANSFERENCIA FROM TIPOSTRANSFERENCIA ORDER BY CVE_TIPOTRANSFERENCIA";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_TIPOTRANSFERENCIA"), res.Get("TIPOTRANSFERENCIA"));

            return dict;
        }

        public bool RecalculoAsimiladosDispersion(string sede, string fechaIni, string fechaFin, bool SinNumCta)
        {
            bool exito = false;

            List<Parametros> lParamS;
            Parametros paramSede;
            Parametros paramFpagoIni;
            Parametros paramFpagoFin;
            Parametros paramSinNoCta;

            try
            {
                lParamS = new List<Parametros>();
                paramSede = new Parametros();
                paramFpagoIni= new Parametros();
                paramFpagoFin = new Parametros();
                paramSinNoCta = new Parametros();

                paramSede.nombreParametro = "@sede";
                paramSede.longitudParametro = 3;
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = sede;
                lParamS.Add(paramSede);

                paramFpagoIni.nombreParametro = "@fechaPagoIni";
                paramFpagoIni.tipoParametro = SqlDbType.Date;
                paramFpagoIni.direccion = ParameterDirection.Input;
                paramFpagoIni.value = fechaIni;
                lParamS.Add(paramFpagoIni);

                paramFpagoFin.nombreParametro = "@fechaPagoFin";
                paramFpagoFin.tipoParametro = SqlDbType.Date;
                paramFpagoFin.direccion = ParameterDirection.Input;
                paramFpagoFin.value = fechaFin;
                lParamS.Add(paramFpagoFin);

                paramSinNoCta.nombreParametro = "@SinNumCuenta";
                paramSinNoCta.tipoParametro = SqlDbType.Bit;
                paramSinNoCta.direccion = ParameterDirection.Input;
                paramSinNoCta.value = SinNumCta.ToString();
                lParamS.Add(paramSinNoCta);

                exito = db.ExecuteStoreProcedure("sp_bandera_recalculo_dispersion", lParamS);
                if (exito == false)
                    return false;
                return true;
            }
            catch (Exception E)
            {
                return false;
            }
        }
    }
}