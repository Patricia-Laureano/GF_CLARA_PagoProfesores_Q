using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.CatalogosporSede
{
    public class CentroCostosModel :SuperModel
    {
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public string Sede { get; set; }
        public string TipoFactura { get; set; }
        public string TipoPago { get; set; }
        public string Cuenta { get; set; }
        public string CuentaIVA { get; set; }
        public string CuentaRETIVA { get; set; }
        public string CuentaRETISR { get; set; }
        public string Escuela { get; set; }
        public string Programa { get; set; }
        public int Activa { get; set; }
        public bool asignado { get; set; }
        public string sql { get; set; } //update

        public bool Add()
        {
            try
            {
                sql = "INSERT INTO CENTRODECOSTOS(CVE_CENTRODECOSTOS,CENTRODECOSTOS,CVE_SEDE,CVE_TIPODEPAGO,CUENTA,CUENTA_IVA,CUENTA_RETIVA,CUENTA_RETISR,CVE_ESCUELA,CVE_PROGRAMA,ACTIVA,USUARIO) VALUES('" + Clave + "','" + Descripcion + "','" + Sede + "','" + TipoPago + "','" + Cuenta + "','" + CuentaIVA + "','" + CuentaRETIVA + "','" + CuentaRETISR + "','" + Escuela + "','" + Programa + "','" + Activa + "','" + this.sesion.nickName + "')"; //Update quitar string
                if (db.execute(sql)) return true;   
                else return false;
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
                sql = "SELECT * FROM VCentrodeCostos WHERE ID_CENTRODECOSTOS = " + Id + ""; 
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Clave = res.Get("CVE_CENTRODECOSTOS");
                    Descripcion = res.Get("CENTRODECOSTOS");
                    Sede = res.Get("CVE_SEDE");
                    TipoFactura = res.Get("CVE_TIPOFACTURA");
                    TipoPago = res.Get("CVE_TIPODEPAGO");
                    Cuenta = res.Get("CUENTA");
                    CuentaIVA = res.Get("CUENTA_IVA");
                    CuentaRETIVA = res.Get("CUENTA_RETIVA");
                    CuentaRETISR = res.Get("CUENTA_RETISR");
                    Escuela = res.Get("CVE_ESCUELA");
                    Programa = res.Get("CVE_PROGRAMA");
                    Activa = Convert.ToInt32(res.GetBool("ACTIVA"));
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
                sql = "UPDATE CENTRODECOSTOS SET ";
                sql += "CENTRODECOSTOS = '" + Descripcion + "',";
                sql += "CVE_TIPODEPAGO = '" + TipoPago + "',";
                sql += "CUENTA = '" + Cuenta + "',";
                sql += "CUENTA_IVA = '" + CuentaIVA + "',";
                sql += "CUENTA_RETIVA = '" + CuentaRETIVA + "',";
                sql += "CUENTA_RETISR = '" + CuentaRETISR + "',";
                sql += "CVE_ESCUELA = '" + Escuela + "',";
                sql += "CVE_PROGRAMA = '" + Programa + "',";
                sql += "ACTIVA = " + Activa + "";
                sql += ",USUARIO = '" + this.sesion.nickName + "'";
                sql += ",FECHA_M = GETDATE()";
                sql += " WHERE ID_CENTRODECOSTOS= '" + Id + "'";
                if (db.execute(sql)) return true; else return false;
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
                sql = "DELETE FROM CENTRODECOSTOS WHERE ID_CENTRODECOSTOS ='" + Id + "'";    //update quitar string
            
                if (db.execute(sql))
                {
                    asignado = false;
                    return true;
                }
                else {
                    asignado = true;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}