using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectDB;
using System.Data.SqlClient;

namespace Session
{
	public class Privileges
	{
		public enum RoleDB
		{
			Administrador = 2
		};

		private long pkUser;
		private bool isAdministrator;
		private List<long> list_pkPermissions;
		private database db;

		public Privileges(long pkUsuario, database db)
		{
			this.db = db;
			this.isAdministrator = false;
			this.setPkUser(pkUsuario);
		}

		public long getPkUser()
		{
			return pkUser;
		}

		public void setPkUser(long pkUsuario)
		{
			this.pkUser = pkUsuario;
		}

		public void loadPermissions()
		{
			string sql = "SELECT RP.PK_PERMISO AS 'PERMISO' FROM ROLES_USUARIO RU, ROLES_PERMISOS RP WHERE RU.PK_USUARIO = " + getPkUser() + " AND RP.PK_ROL = RU.PK_ROLE";
			ResultSet res = db.getTable(sql);

			this.list_pkPermissions = new List<long>();
			try
			{
				while (res.Next())
					list_pkPermissions.Add(res.GetLong("PERMISO"));
			}
			catch (Exception) { }

			this.isAdministrator = checkIsAdministrador();
		}

		public bool havePermission(long pkPermiso)
		{
			if (this.isAdministrator)
				return true;

			foreach (long item in list_pkPermissions)
				if (item == pkPermiso)
					return true;

			return false;
		}// <end>

		private bool checkIsAdministrador()
		{
			String sql = "SELECT COUNT(PK1) AS 'MAX' FROM ROLES_USUARIO WHERE PK_USUARIO = '" + this.getPkUser() + "' AND PK_ROLE = '" + (int)RoleDB.Administrador + "' ";
			int numero = db.Count(sql);

			return (numero > 0);
		}//<end>
	}
}
