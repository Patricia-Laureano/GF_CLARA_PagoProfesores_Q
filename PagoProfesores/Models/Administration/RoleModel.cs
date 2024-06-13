using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectDB;
using System.Data.SqlClient;
using Session;

namespace PagoProfesores.Models
{
	public class RoleModel : SuperModel
	{
		public int idRole { get; set; }
		public string Role { get; set; }
		public string Description { get; set; }



		public bool Add()
		{

			try
			{
				string sql = "INSERT INTO ROLES(ROLE,DESCRIPCION) VALUES('" + Role + "','" + Description + "')";
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
				string sql = "SELECT * FROM ROLES WHERE PK1=" + idRole + "";
				ResultSet res = db.getTable(sql);

				if (res.Next())
				{
					Role = res.Get("ROLE");
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
				string sql = "UPDATE ROLES SET ROLE ='" + Role + "',DESCRIPCION= '" + Description + "' WHERE PK1=" + idRole;
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
				string sql = "DELETE FROM ROLES WHERE PK1=" + idRole;
				if (db.execute(sql)) { return true; } else { return false; }

			}
			catch
			{
				return false;
			}
		}

	}


}