using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Personas
{
	public class PensionadosModel : SuperModel
	{
		public long PK1 { get; set; }
		public long IdPersona { get; set; }
		public long IdPensionado { get; set; }
		public string Nombre { get; set; }
        public string ApellidoP { get; set; }
        public string ApellidoM { get; set; }
        public double Porcentaje { get; set; }
		public bool Activo { get; set; }
        public string TipoPago { get; set; }
        public string Cuenta { get; set; }
        public string Clabe { get; set; }
        public string Banco { get; set; }
        public double MontoFijo { get; set; }
        public string Comentarios { get; set; }
        public string TipoPension { get; set; }

        public string IDSIU { get; set; }
		public bool found;

		public string sql { get; set; } //update

		private Dictionary<string, string> prepareData(bool add)
		{
			string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

			Dictionary<string, string> dic = new Dictionary<string, string>();

			if (add)
			{
				dic.Add("ID_PERSONA", IdPersona.ToString());
				//dic.Add("ID_PENSIONADO", IdPensionado.ToString());
			}
			
            dic.Add("CUENTA", Cuenta);
            dic.Add("CLABE", Clabe);


         //   dic.Add("CUENTA", Cuenta.ToString());
           // dic.Add("CLABE", Clabe.ToString());



            dic.Add("BENEFICIARIO",Nombre);
            dic.Add("BENEFICIARIOP", ApellidoP);
            dic.Add("BENEFICIARIOM", ApellidoM);
            dic.Add("CVE_BANCO", Banco);//CVE_BANCO
            dic.Add("TIPODEPAGO", TipoPago);
            dic.Add("TIPOPENSION",TipoPension );
            dic.Add("PORCENTAJE", Porcentaje.ToString("F"));
            dic.Add("CUOTA", MontoFijo.ToString("F"));
            dic.Add("COMENTARIOS", Comentarios);

        
			/*if (add)
				dic.Add("FECHA_R", FECHA);
			else
				dic.Add("FECHA_M", FECHA);*/
			dic.Add("IP", "0");
			dic.Add("USUARIO", sesion.nickName);
            dic.Add("ACTIVO", Activo ? "1" : "0");
            return dic;
		}

		public bool Add()
		{
			try
			{
				double suma = ConsultaPorcentaje(0);
				if (Porcentaje + suma > 99.9)
					Porcentaje = 99.9 - suma;
				if (Porcentaje < 0)
					Porcentaje = 0;

				Dictionary<string, string> dic = prepareData(true);
				sql = "INSERT INTO"
					+ " PENSIONADOS (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
					+ " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";
				return db.execute(sql);
			}
			catch { return false; }
		}

		public bool Save()
		{
			try
			{
				double suma = ConsultaPorcentaje(PK1);
				if (Porcentaje + suma > 99.9)
					Porcentaje = 99.9 - suma;
				if (Porcentaje < 0)
					Porcentaje = 0;

				List<string> values = new List<string>();
				foreach (KeyValuePair<string, string> item in prepareData(false))
					values.Add(item.Key + "='" + item.Value + "'");

				sql = "UPDATE PENSIONADOS SET " + string.Join<string>(", ", values.ToArray<string>())
					+ " WHERE PK1 = " + PK1 + "";
				return db.execute(sql);
			}
			catch { return false; }
		}

		public double ConsultaPorcentaje(long PK1_excluido)
		{
			try
			{
				sql = "SELECT SUM(PORCENTAJE) AS 'SUM' FROM VPENSIONADOS WHERE ID_PERSONA = " + IdPersona;
				if (PK1_excluido != 0)
					sql += " AND PK1<>" + PK1_excluido;
				ResultSet res = db.getTable(sql);
				if (res.Next())
				{
					return res.GetDouble("SUM");
				}
			}
			catch { return 99.9; }
			return 0.0;
		}

		public bool Edit()
		{
			try
			{
				sql = "SELECT * FROM VPENSIONADOS WHERE PK1 = " + PK1 + "";
				ResultSet res = db.getTable(sql);
                
				if (res.Next())
				{
                    PK1 = res.GetLong("PK1");
					IdPersona = res.GetLong("ID_PERSONA");
					Cuenta = res.Get("CUENTA");
                    Clabe = res.Get("CLABE");
                    Nombre = res.Get("BENEFICIARIO");
                    ApellidoP = res.Get("BENEFICIARIOP");
                    ApellidoM = res.Get("BENEFICIARIOM");
                    Banco = res.Get("CVE_BANCO");//
                    TipoPago = res.Get("TIPODEPAGO");
                    TipoPension = res.Get("TIPOPENSION");                   
                    Porcentaje = res.GetDouble("PORCENTAJE");
                    MontoFijo = res.GetDouble("CUOTA");
                    Comentarios = res.Get("COMENTARIOS");
                    IDSIU = res.Get("IDSIU");
                    Activo = res.GetBool("ACTIVO");

                    found = true;
					return true;
				}
			}
			catch { }
			return false;
		}
        
		public bool Delete()
		{
			try
			{
				sql = "DELETE FROM PENSIONADOS WHERE PK1 = " + PK1 + "";
				return db.execute(sql);
			}
			catch { }
			return false;
		}
        
		public bool BuscaPersona()
		{
			sql = "SELECT TOP 1 * FROM PERSONAS WHERE IDSIU = '" + IDSIU + "'";
			ResultSet res = db.getTable(sql);

			if (res.Next())
			{
				IdPensionado = res.GetLong("ID_PERSONA"); // Persona a ser beneficiario.
				Nombre = res.Get("NOMBRES") + ", " + res.Get("APELLIDOS");
				PK1 = 0;
				Porcentaje = 0.0;
				Activo = false;
				found = false;

				sql = "SELECT TOP 1 * FROM PENSIONADOS WHERE ID_PERSONA=" + IdPersona+" AND ID_PENSIONADO="+IdPensionado;
				res = db.getTable(sql);
				if (res.Next())
				{
					PK1 = res.GetLong("PK1");
					Porcentaje = res.GetDouble("PORCENTAJE");
					Activo = res.GetBool("ACTIVO");
					found = true;
				}
				
				return true;
			}
			return false;
		}
	}
}