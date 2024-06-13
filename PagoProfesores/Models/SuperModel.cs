using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectDB;
using Session;

namespace PagoProfesores.Models
{
    public class SuperModel
    {

        public database db;
        public SessionDB sesion;
		public string ErrorMessage = "";
		public static DateTime minDateTime = new DateTime(2000, 1, 1, 0, 0, 0, 0);

		public SuperModel()
        {
            db = new database();
        }

		public Dictionary<long, string> ConsultaCiclos()
		{
			Dictionary<long, string> dict = new Dictionary<long, string>();

			string sql = "SELECT TOP 10 CVE_CICLO, CICLO FROM CICLOS ORDER BY CVE_CICLO DESC";
			ResultSet res = db.getTable(sql);
			while (res.Next())
				dict.Add(res.GetLong("CVE_CICLO"), res.Get("CICLO"));

			return dict;
		}

		public List<string> ConsultaPeriodos(string ClaveCiclo)
		{
			List<string> list = new List<string>();

			string sql = "SELECT PERIODO FROM PERIODOS WHERE CVE_CICLO=" + ClaveCiclo + " ORDER BY PERIODO ";
			ResultSet res = db.getTable(sql);
			while (res.Next())
				list.Add(res.Get("PERIODO"));

			return list;
		}

		public Dictionary<string, string> ConsultaNiveles()
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();

			string sql = "SELECT CVE_NIVEL,NIVEL FROM NIVELES ORDER BY NIVEL ";
			ResultSet res = db.getTable(sql);
			dict.Add("", "");
			while (res.Next())
				dict.Add(res.Get("CVE_NIVEL"), res.Get("NIVEL"));

			return dict;
		}

		

		public Dictionary<string, string> ConsultaTipoFacturas()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_TIPOFACTURA, TIPOFACTURA FROM TIPOSFACTURA ORDER BY TIPOFACTURA ASC";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_TIPOFACTURA"), res.Get("TIPOFACTURA"));

            return dict;
        }


        public Dictionary<string, string> ConsultaTiposdePago(string cveFactura)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_TIPODEPAGO,TIPODEPAGO FROM TIPOSDEPAGO WHERE CVE_TIPOFACTURA = '" + cveFactura + "' ORDER BY TIPODEPAGO ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_TIPODEPAGO"), res.Get("TIPODEPAGO"));

            return dict;
        }

        public Dictionary<string, string> ConsultaEscuelas()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_ESCUELA, ESCUELA FROM ESCUELAS ORDER BY ESCUELA ASC";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_ESCUELA"), res.Get("ESCUELA"));

            return dict;
        }


        public Dictionary<string, string> ConsultaProgramas(string cveEscuela)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_PROGRAMA,PROGRAMA FROM PROGRAMAS WHERE CVE_ESCUELA = '" + cveEscuela + "' ORDER BY PROGRAMA ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_PROGRAMA"), res.Get("PROGRAMA"));

            return dict;
        }

		public DateTime ParseDateTime(string str)
		{
			try
			{

				if (str != null)
                {
					string[] array = str.Split(new char[] { '/', '-', ' ', 'T' });

					if (str.IndexOf('/') > 0)
					{
						if (array.Length >= 6)
							return new DateTime(int.Parse(array[2]), int.Parse(array[1]), int.Parse(array[0]), int.Parse(array[3]), int.Parse(array[4]), int.Parse(array[5]), 0);
						else if (array.Length >= 3)
							return new DateTime(int.Parse(array[2]), int.Parse(array[1]), int.Parse(array[0]), 0, 0, 0, 0);
					}
					else if (str.IndexOf('-') > 0)
					{
						if (array.Length >= 6)
							return new DateTime(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]), int.Parse(array[4]), int.Parse(array[5]), 0);
						else if (array.Length >= 3)
							return new DateTime(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), 0, 0, 0, 0);
					}
				}
				
				return DateTime.Parse(str);
			}
			catch (Exception ex) { }

			return minDateTime;
		}

		public string ValidDateTime(string str)
		{
			DateTime dt = ParseDateTime(str);
			return dt.ToString("yyyy-MM-dd hh:mm:ss");
		}

		public string ValidDate(string str)
		{
			DateTime dt = ParseDateTime(str);
			return dt.ToString("yyyy-MM-dd");
		}

		public string valid(string str)
		{
			if (str == null)
				return "";
			return str.Replace("'", "''");
		}

		public string onlyDate(string str)
		{
			if (str != null)
			{
				if (str.IndexOf(" ") > 0)
					return str.Substring(0, str.IndexOf(" "));
				else if (str.IndexOf("T") > 0)
					return str.Substring(0, str.IndexOf("T"));
			}
			return str;
		}
		/*
		protected virtual Dictionary<string, string> prepareData(bool add, object obj)
		{
			return new Dictionary<string, string>();
		}
		//*/
		public string makeSqlInsert(Dictionary<string, string> dic, string table, object obj)
		{
			return "INSERT INTO " +
				" " + table + " (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")" +
				" VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";
		}

		public string makeSqlUpdate(Dictionary<string, string> dic, string table, string condition, object obj)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> pair in dic)
				list.Add(pair.Key + " = '" + pair.Value + "'");

			return "UPDATE " + table +
				" SET " + string.Join<string>(",", list.ToArray<string>()) +
				" WHERE " + condition;
		}

		public void Dispose()
		{
			db.Close();
			db = null;
			sesion = null;
		}

    }
}