using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using ConnectDB;
using System.Diagnostics;
using Factory;

namespace PagoProfesores.Models.Administration
{
	public class UsersModel : SuperModel
	{
		public string Id { get; set; }
		public string Usuario { get; set; }
		public string Password { get; set; }
		public string Nombre { get; set; }
		public string APaterno { get; set; }
		public string AMaterno { get; set; }
		public string RFC { get; set; }
		public string Edad { get; set; }
		public string Activo { get; set; }
		public string SedePrincipal { get; set; }
		public string[] IdsSedesAcceso { get; set; }

		public string[] Roles { get; set; }

		private Dictionary<string, string> prepareData(bool add)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("USUARIO", Usuario);
			dic.Add("PASSWORD", EncryptX.Encode(Password));
			dic.Add("NOMBRE", Nombre);
			dic.Add("APATERNO", APaterno);
			dic.Add("AMATERNO", AMaterno);
			dic.Add("RFC", RFC);
			dic.Add("EDAD", Edad);
			dic.Add("ACTIVO", Activo);
			dic.Add("CVE_SEDE", SedePrincipal);
			if(add)
				dic.Add("FECHA_R", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
			else
				dic.Add("FECHA_M", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
			dic.Add("PK_USUARIO", sesion.pkUser.ToString());
			return dic;
		}
		public bool Add()
		{
			try
			{
				string sql = "SELECT COUNT(PK1) AS 'MAX' FROM USUARIOS WHERE USUARIO='" + Usuario + "'";
				if (db.Count(sql) > 0)
					throw new Exception("El usuario ya existe.");
				/*
				sql =
					"INSERT INTO" +
					" USUARIOS (USUARIO,PASSWORD,NOMBRE,APATERNO,AMATERNO,RFC,EDAD,CVE_SEDE,PK_USUARIO)" +
					" VALUES('" + Usuario +
					"','" + EncryptX.Encode(Password) +
					"','" + Nombre +
					"','" + APaterno +
					"','" + AMaterno +
					"','" + RFC +
					"','" + Edad +
					"','" + SedePrincipal +
					"'," + sesion.pkUser + ")";
				//*/
				Dictionary<string, string> dic = prepareData(true);
				sql = "INSERT INTO"
					+ " USUARIOS (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
					+ " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";

				string PK_USUARIO = db.executeId(sql);
				if (PK_USUARIO != null)
				{
					if (AddSedes(PK_USUARIO) == false)
						throw new Exception("No se han podido agregar las sedes de acceso.");
					if (AddRoles(PK_USUARIO) == false)
						throw new Exception("No se han podido agregar las sedes de acceso.");
					return true;
				}
			}
			catch (Exception ex) { ErrorMessage = ex.Message; }
			return false;
		}

		public bool AddSedes(string PK_USUARIO)
		{
			string sql = "DELETE FROM USUARIOS_SEDES WHERE PK_USUARIO='" + PK_USUARIO + "'";
			bool ok = db.execute(sql);
			if (ok && IdsSedesAcceso != null)
				foreach (string idSede in IdsSedesAcceso)
				{
					sql =
						"INSERT INTO USUARIOS_SEDES (PK_USUARIO, CVE_SEDE)" +
						" VALUES('" + PK_USUARIO + "','" + idSede + "')";
					ok = ok && db.execute(sql);
				}
			return ok;
		}

		public bool AddRoles(string PK_USUARIO)
		{
			string sql = "DELETE FROM ROLES_USUARIO WHERE PK_USUARIO='" + PK_USUARIO + "'";
			bool ok = db.execute(sql);
			if (ok && Roles != null)
				foreach (string idRole in Roles)
				{
					sql =
						"INSERT INTO ROLES_USUARIO (PK_USUARIO, PK_ROLE)" +
						" VALUES('" + PK_USUARIO + "','" + idRole + "')";
					ok = ok && db.execute(sql);
				}
			return ok;
		}

		public string[] EditSedes(string PK_USUARIO)
		{
			string sql = "SELECT * FROM USUARIOS_SEDES WHERE PK_USUARIO='" + PK_USUARIO + "' ";
			List<string> list = new List<string>();
			ResultSet res = db.getTable(sql);
			while (res.Next())
			{
				list.Add(res.Get("CVE_SEDE"));
			}
			return list.ToArray<string>();
		}

		public string[] EditRoles(string PK_USUARIO)
		{
			string sql = "SELECT * FROM ROLES_USUARIO WHERE PK_USUARIO='" + PK_USUARIO + "' ";
			List<string> list = new List<string>();
			ResultSet res = db.getTable(sql);
			while (res.Next())
			{
				list.Add(res.Get("PK_ROLE"));
			}
			return list.ToArray<string>();
		}

		public bool Edit()
		{
			try
			{
				string sql = "SELECT TOP 1 * FROM USUARIOS WHERE PK1=" + Id;
				ResultSet res = db.getTable(sql);

				if (res != null && res.Next())
				{
					Usuario = res.Get("USUARIO");
					Nombre = res.Get("NOMBRE");
					APaterno = res.Get("APATERNO");
					AMaterno = res.Get("AMATERNO");
					RFC = res.Get("RFC");
					Edad = res.Get("EDAD");
					try
					{
						Password = EncryptX.Decode(res.Get("PASSWORD"));
					}
					catch { Password = ""; }

					Activo = res.Get("ACTIVO") == "True" ? "1" : "0";
					SedePrincipal = res.Get("CVE_SEDE");
					SedePrincipal = SedePrincipal == "" ? "0" : SedePrincipal;
					IdsSedesAcceso = EditSedes(res.Get("PK1"));

					Roles = EditRoles(res.Get("PK1"));

					return true;
				}
			}
			catch (Exception) { }
			return false;
		}

		public bool Save()
		{
			try
			{
				/*
				string sql = "UPDATE USUARIOS SET" +
					"  USUARIO = '" + Usuario + "'" +
					", PASSWORD = '" + EncryptX.Encode(Password) + "'" +
					", NOMBRE = '" + Nombre + "'" +
					", APATERNO = '" + APaterno + "'" +
					", AMATERNO = '" + AMaterno + "'" +
					", RFC = '" + RFC + "'" +
					", EDAD = '" + Edad + "'" +
					", FECHA_M = GETDATE()" +
					", PK_USUARIO = '" + sesion.pkUser + "'" +
					" WHERE PK1 = " + Id;
				
				*/
				List<string> values = new List<string>();
				foreach (KeyValuePair<string, string> item in prepareData(false))
					values.Add(item.Key + "='" + item.Value + "'");

				string sql = "UPDATE USUARIOS SET " + string.Join<string>(", ", values.ToArray<string>())
					+ " WHERE PK1 = " + Id + "";

				bool res = db.execute(sql);
				AddSedes(Id);
				AddRoles(Id);
				return res;
			}
			catch { }
			return false;
		}

		public bool Delete()
		{
			try
			{
				string sql = "DELETE FROM USUARIOS WHERE PK1 = " + Id;
				return db.execute(sql);
			}
			catch { }
			return false;
		}

	}
}
