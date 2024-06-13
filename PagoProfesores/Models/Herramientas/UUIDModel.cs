using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Herramientas
{
	public class UUIDModel : SuperModel
	{
		public ResultSet validar()
		{
			string sql = "SELECT ID_ESTADODECUENTA,UUID,XML FROM ESTADODECUENTA WHERE (UUID IS NULL OR LTRIM(UUID)='') AND (XML IS NOT NULL)";
			return db.getTable(sql);
		}


        public void save_UUID(long ID_ESTADODECUENTA, string UUID)
        {
            string sql = "";

          //  string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            sql = "UPDATE ESTADODECUENTA SET "             
                + "UUID='" + UUID + "'"               
               // + ",USUARIO='" + sesion.nickName + "'"
                + " WHERE ID_ESTADODECUENTA=" + ID_ESTADODECUENTA;

              db.execute(sql);
        }



    }
}