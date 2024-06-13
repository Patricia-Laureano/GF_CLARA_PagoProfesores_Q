using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{
	public class RegistroContratosModel : SuperModel
	{
		public string IdSIU { get; set; }
		public string IdPersona { get; set; }
        public string IdContratos { get; set; }
        public int idEsquemas { get; set; }
        public string periodos { get; set; }
        public string FechaEntrega { get; set; }
		public string Nombre { get; set; }
		public string sql { get; set; }

		public bool Edit()
		{
			try
			{               

                sql = "SELECT * FROM VENTREGA_CONTRATOS WHERE ID_CONTRATO = '" + IdContratos + "'";
				ResultSet res = db.getTable(sql);

				if (res.Next())
				{

                    IdContratos = res.Get("ID_CONTRATO");
                    IdSIU = res.Get("IDSIU");                  
                    IdPersona = res.Get("ID_PERSONA");
                    //FechaEntrega = res.Get("FECHADEENTREGA");
                    Nombre = res.Get("NOMBRES");
                    idEsquemas = res.GetInt("ID_ESQUEMA");
                    periodos = res.Get("PERIODO");


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
				if (FechaEntrega == null || FechaEntrega.Trim() == "")
					sql = "UPDATE ENTREGADECONTRATOS SET FECHADEENTREGA = NULL" +
                        " WHERE PK1 = " + IdContratos;
                else
					sql = "UPDATE ENTREGADECONTRATOS SET" +
						" FECHADEENTREGA = CONVERT(datetime,'" + FechaEntrega + "', 103)" +
                        " WHERE  PK1 = " + IdContratos;
                return db.execute(sql);
			}
			catch { }
			return false;
		}

		public bool Delete()
		{
			try
			{
				sql = "UPDATE ENTREGADECONTRATOS SET"
					+ " FECHADEENTREGA = NULL"
					+ " WHERE  ID_CONTRATO = '" + IdContratos + "'";
                return db.execute(sql);
			}
			catch { }
			return false;
		}
	}
}
