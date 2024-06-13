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
    public class BloqueosModel : SuperModel
    {

        public string Cve_Bloqueo { get; set; }
        public string Bloqueo { get; set; }
        public string BloqueoDescripcion { get; set; }
		public string EstadoCuenta { get; set; }
		public string Factura { get; set; }
		public string Pagos { get; set; }

		public string sql { get; set; }

		public bool Add()
		{
			try
			{
				/*
				sql = "INSERT INTO BLOQUEOS(";
				sql += "CVE_BLOQUEO";
				sql += ",Bloqueo";
				sql += ",BloqueoDescripcion";
				sql += ",Usuario";

				sql += ") VALUES(";
				sql += "'" + Cve_Bloqueo + "'";
				sql += ",'" + Bloqueo + "'";
				sql += ",'" + BloqueoDescripcion + "'";
				sql += ",'" + this.sesion.nickName + "'";

				sql += ")";

				;
				*/
				sql = makeSqlInsert(prepareData(true, null), "BLOQUEOS", null);

				return db.execute(sql);
			}
			catch { }
			return false;
		}

		protected virtual Dictionary<string, string> prepareData(bool add, object obj)
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
			if (add)
				dict.Add("CVE_BLOQUEO", Cve_Bloqueo);
			dict.Add("BLOQUEO", Bloqueo);
			dict.Add("BLOQUEODESCRIPCION", BloqueoDescripcion);
			dict.Add("ESTADOCUENTA", EstadoCuenta);
			dict.Add("FACTURA", Factura);
			dict.Add("PAGOS", Pagos);

			//-----------
			if (add)
				dict.Add("FECHA_R", FECHA);
			else
				dict.Add("FECHA_M", FECHA);
			dict.Add("IP", "0");
			dict.Add("USUARIO", sesion.nickName);
			dict.Add("ELIMINADO", "0");
			return dict;
		}

		public bool Edit()
		{
			try
			{
				sql = "SELECT * FROM BLOQUEOS WHERE Cve_Bloqueo = '" + Cve_Bloqueo + "'";
				ResultSet res = db.getTable(sql);

				if (res.Next())
				{
					Cve_Bloqueo = res.Get("CVE_BLOQUEO");
					Bloqueo = res.Get("BLOQUEO");
					BloqueoDescripcion = res.Get("BLOQUEODESCRIPCION");
					EstadoCuenta = res.Get("ESTADOCUENTA");
					Factura = res.Get("FACTURA");
					Pagos = res.Get("PAGOS");

					return true;
				}
			}
			catch { }
			return false;
		}

		public bool Save()
		{
			try
			{
				/*
                sql = "UPDATE BLOQUEOS SET ";
                sql += "Bloqueo = '" + Bloqueo + "'";
                sql += ",BloqueoDescripcion = '" + BloqueoDescripcion + "'";
                sql += ",Usuario = '" + this.sesion.nickName + "'";
                sql += ",Fecha_M = " + "GETDATE()" + "";

                sql += " WHERE Cve_Bloqueo = '" + Cve_Bloqueo + "'";
                if (db.execute(sql)) { return true; } else { return false; }
				*/
				sql = makeSqlUpdate(prepareData(false, null), "BLOQUEOS", "CVE_BLOQUEO='" + Cve_Bloqueo + "'", null);

				return db.execute(sql);
			}
			catch { }
			return false;
		}

		public bool Delete()
		{
			try
			{
				sql = "DELETE FROM BLOQUEOS WHERE Cve_Bloqueo = '" + Cve_Bloqueo + "'";
				return db.execute(sql);
			}
			catch { }
			return false;
		}
    }
}