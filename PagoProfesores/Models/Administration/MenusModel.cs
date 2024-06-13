using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagoProfesores.Models;
using Session;
using System.Data.SqlClient;
using System.Data;
using ConnectDB;

namespace PagoProfesores.Models
{
	public class MenusModel : SuperModel
	{
		public int Id { get; set; }
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public string Url { get; set; }
		public int Padre { get; set; }
		public int Orden { get; set; }
		public long PkPermiso { get; set; }
		public string Icono { get; set; }

		public bool Add()
		{
			try
			{
				string sql =
					"INSERT INTO" +
					" MENU (NOMBRE,DESCRIPCION,URL,PADRE,ORDEN,PK_PERMISO,PK_USUARIO,ICONO,DISPONIBLE)" +
					" VALUES('" + Nombre + "','" + Descripcion + "','" + Url +
					"','" + Padre + "','" + Orden + "','" + PkPermiso + "','" + sesion.pkUser + "','" + Icono + "',1)";
				if (db.execute(sql))
				{
					Log.write(this, "Add", LOG.REGISTRO, "SQL:" + sql, sesion);
					return true;
				}
			}
			catch (Exception) { }
			return false;
		}

		public bool Edit()
		{
			try
			{
				string sql = "SELECT * FROM MENU WHERE PK1=" + Id;

				ConnectDB.ResultSet res = db.getTable(sql);

				while (res.Next())
				{
					Nombre = res.Get("NOMBRE");
					Descripcion = res.Get("DESCRIPCION");
					Url = res.Get("URL");

					Padre = int.Parse(res.Get("PADRE"));
					Orden = int.Parse(res.Get("ORDEN"));
					PkPermiso = long.Parse(res.Get("PK_PERMISO"));
					Icono = res.Get("ICONO");
				}
				return true;
			}
			catch { }
			return false;
		}

		public bool Save()
		{
			try
			{
				string sql = "UPDATE MENU SET" +
					"  NOMBRE = '" + Nombre + "'" +
					", DESCRIPCION = '" + Descripcion + "'" +
					", URL = '" + Url + "'" +
					", PADRE = '" + Padre + "'" +
					", ORDEN = '" + Orden + "'" +
					", PK_PERMISO = '" + PkPermiso + "'" +
					", FECHA_M = GETDATE()" +
					", PK_USUARIO = '" + sesion.pkUser + "'" +
					", ICONO = '<i class=\"" + Icono + "\"></i>'" +
					", DISPONIBLE = '1'" +
					" WHERE PK1 = " + Id;
				return db.execute(sql);
			}
			catch { }
			return false;
		}

		public bool Delete()
		{
			try
			{
				string sql = "DELETE FROM MENU WHERE PK1 = " + Id;
				return db.execute(sql);
			}
			catch { }
			return false;
		}

		public void ConsultaIdsMenu(Dictionary<int, string> dict)
		{
			string sql = "SELECT * FROM MENU WHERE PADRE = 0 ";
			ResultSet res = db.getTable(sql);
			while (res.Next())
				dict.Add(res.GetInt("PK1"), res.Get("NOMBRE"));
		}

		public void ConsultaIdsPermisos(Dictionary<long, string> dict)
		{
			string sql = "SELECT * FROM PERMISOS ORDER BY PK1 ASC";
			ResultSet res = db.getTable(sql);
			while (res.Next())
				dict.Add(res.GetLong("PK1"), res.Get("PERMISO"));
		}

	}//<end class>
}
