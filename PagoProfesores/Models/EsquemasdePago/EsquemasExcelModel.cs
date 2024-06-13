using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.EsquemasdePago
{
	public class EsquemasExcelModel : SuperModel
	{
		public string Id_esquema { get; set; }
		public string Id_concepto { get; set; }
		public string EsquemaDePago { get; set; }
		public string Sede { get; set; }
		public string Periodo { get; set; }
		public string EsquemaDePagoDes { get; set; }
		public string ClaveContrato { get; set; }
		public string FechaInicio { get; set; }
		public string FechaFin { get; set; }
		public string NumSemanas { get; set; }
		public string NumPagos { get; set; }
		public string BloqueoContrato { get; set; }
		public string Concepto { get; set; }
		public string FechaPago { get; set; }
		public string FechaRecibo { get; set; }

		public bool DUPLICADO;
		public bool CONCEPTO_DUPLICADO;
		public string FileName { get; set; }

        public string SedeX { get; set; }

        char[] ERRORES;

		private List<string> sedes;
		private List<string> periodos;
		private List<string> contratos;
		private Dictionary<string, string> conceptos;

		public Dictionary<ESQUEMA, bool> validCells;
		//                sede               periodo            nivel              esquema            
		public Dictionary<string, Dictionary<string, Dictionary<string, Esquema>>> __esquemas;

		public string sql;

		public enum ESQUEMA
		{
			ESQUEMADEPAGO, CVE_SEDE, PERIODO, ESQUEMADEPAGODES, CVE_CONTRATO, FECHAINICIO, FECHAFIN, NOSEMANAS, NOPAGOS, BLOQUEOCONTRATO, CONCEPTO, FECHAPAGO, FECHARECIBO
        }

        public class Esquema
		{
			public string ESQUEMADEPAGO { get; set; }
			public Dictionary<string, string> conceptos;
			public Esquema(string ESQUEMADEPAGO)
			{
				this.ESQUEMADEPAGO = ESQUEMADEPAGO;
				conceptos = new Dictionary<string, string>();
			}
		}

		public void clean()
		{
			string sql = "DELETE FROM ESQUEMASDEPAGO_TMP WHERE USUARIO = '" + sesion.nickName + "'";
			db.execute(sql);
		}

		public void cargaListas()
		{
			ResultSet res;
            //
            // SEDES
            //
            string sqlSedes = "SELECT CVE_SEDE FROM SEDES";

            if (!(sqlSedes == string.Empty && sqlSedes == null)) sqlSedes += " where cve_sede = '" + SedeX + "'";

            res = db.getTable(sqlSedes);
			sedes = new List<string>();
			while (res.Next())
				sedes.Add(res.Get("CVE_SEDE"));
			sedes.Sort(delegate (string str1, string str2)
			{
				return str1.CompareTo(str2);
			});

			//
			// PERIODOS
			//
			res = db.getTable("SELECT PERIODO FROM PERIODOS");
			periodos = new List<string>();
			while (res.Next())
				periodos.Add(res.Get("PERIODO"));

			//
			// CONCEPTOS
			//
			res = db.getTable("SELECT DISTINCT(CONCEPTO) FROM CONCEPTOSDEPAGO ORDER BY CONCEPTO");
			conceptos = new Dictionary<string, string>();
			while (res.Next())
				conceptos.Add(res.Get("CONCEPTO").ToUpper(), res.Get("CONCEPTO"));
            
			//res = db.getTable("SELECT ID_ESQUEMA, CVE_SEDE, PERIODO, CVE_NIVEL, ESQUEMADEPAGO FROM ESQUEMASDEPAGO ORDER BY CVE_SEDE ASC, PERIODO ASC, CVE_NIVEL ASC, ESQUEMADEPAGO");
			res = db.getTable(@"SELECT E.ID_ESQUEMA, E.CVE_SEDE, E.PERIODO, E.ESQUEMADEPAGO, R.CONCEPTO, R.FECHA_RECIBO, R.FECHADEPAGO FROM ESQUEMASDEPAGO E, ESQUEMASDEPAGOFECHAS R WHERE R.ID_ESQUEMA = E.ID_ESQUEMA ");

            __esquemas = new Dictionary<string, Dictionary<string, Dictionary<string, Esquema>>>();
			while (res.Next())
			{
				Sede = res.Get("CVE_SEDE");
				Periodo = res.Get("PERIODO");
				EsquemaDePago = res.Get("ESQUEMADEPAGO");
				Concepto = res.Get("CONCEPTO");

				if (__esquemas.Keys.Contains(Sede) == false)
					__esquemas.Add(Sede, new Dictionary<string, Dictionary<string, Esquema>>());

				if (__esquemas[Sede].Keys.Contains(Periodo) == false)
					__esquemas[Sede].Add(Periodo, new Dictionary<string, Esquema>());

				if (__esquemas[Sede][Periodo].Keys.Contains(EsquemaDePago.ToUpper()) == false)
					__esquemas[Sede][Periodo].Add(EsquemaDePago.ToUpper(), new Esquema(EsquemaDePago));

				if (__esquemas[Sede][Periodo][EsquemaDePago.ToUpper()].conceptos.Keys.Contains(Concepto.ToUpper()) == false)
					__esquemas[Sede][Periodo][EsquemaDePago.ToUpper()].conceptos.Add(Concepto.ToUpper(), Concepto);
			}
            //
            // CONTRATOS
            //
            //res = db.getTable("SELECT CVE_CONTRATO FROM FORMATOCONTRATOS");
            //contratos = new List<string>();
            //while (res.Next())
            //	contratos.Add(res.Get("CVE_CONTRATO"));
            res = db.getTable("SELECT CONTRATO FROM FORMATOCONTRATOS");
            contratos = new List<string>();
            while (res.Next())
                contratos.Add(res.Get("CONTRATO"));
        }

		public void copiaListasDesde(EsquemasExcelModel other)
		{
			sedes = other.sedes;
			periodos = other.periodos;
			contratos = other.contratos;
			__esquemas = other.__esquemas;
			conceptos = other.conceptos;
		}

		public bool Validate()
		{
			validCells = new Dictionary<ESQUEMA, bool>();
			foreach (ESQUEMA item in Enum.GetValues(typeof(ESQUEMA)))
				validCells[item] = true;

			DateTime aux;
			int int_aux;
			double double_aux;

			validCells[ESQUEMA.CVE_SEDE] = (Sede.Length <= 5) && sedes.Contains(Sede);
			validCells[ESQUEMA.PERIODO] = (Periodo.Length <= 10) && periodos.Contains(Periodo);
			validCells[ESQUEMA.ESQUEMADEPAGODES] = true;
            //validCells[ESQUEMA.CVE_CONTRATO] = (ClaveContrato.Length <= 5) && contratos.Contains(ClaveContrato); // anteriormente se valida la longitud, pero ahora que sólo se considera la descripción, ya no más LENGTH
            validCells[ESQUEMA.CVE_CONTRATO] = contratos.Contains(ClaveContrato);
            validCells[ESQUEMA.FECHAINICIO] = DateTime.TryParse(FechaInicio, out aux);
			validCells[ESQUEMA.FECHAFIN] = DateTime.TryParse(FechaFin, out aux);
			validCells[ESQUEMA.NOSEMANAS] = double.TryParse(NumSemanas, out double_aux);
			validCells[ESQUEMA.NOPAGOS] = int.TryParse(NumPagos, out int_aux);
			validCells[ESQUEMA.BLOQUEOCONTRATO] = (BloqueoContrato == "0" || BloqueoContrato == "1");

			// Conceptos
			validCells[ESQUEMA.CONCEPTO] = (Concepto.Length <= 25) && conceptos.Keys.Contains(Concepto.ToUpper());
			if (validCells[ESQUEMA.CONCEPTO])
				Concepto = conceptos[Concepto.ToUpper()];// Se busca el concepto con MAYUSCULAS, y se reasigna con MAYUSCULAS y minusculas.
			validCells[ESQUEMA.FECHAPAGO] = DateTime.TryParse(FechaPago, out aux);
			validCells[ESQUEMA.FECHARECIBO] = DateTime.TryParse(FechaRecibo, out aux);

			// Esquemas
			DUPLICADO = false;
			CONCEPTO_DUPLICADO = false;
			if (EsquemaDePago.Length <= 15 && validCells[ESQUEMA.CVE_SEDE] && validCells[ESQUEMA.PERIODO]/* && validCells[ESQUEMA.CVE_NIVEL]*/)
			{
				validCells[ESQUEMA.ESQUEMADEPAGO] = true;
				try
				{
					string nombre_esquema = EsquemaDePago.ToUpper();

					if (__esquemas.Keys.Contains(Sede) &&
						__esquemas[Sede].Keys.Contains(Periodo) &&
						__esquemas[Sede][Periodo].Keys.Contains(nombre_esquema))
					{
						EsquemaDePago = __esquemas[Sede][Periodo][nombre_esquema].ESQUEMADEPAGO;
						DUPLICADO = true;

						if (__esquemas[Sede][Periodo][nombre_esquema].conceptos.Keys.Contains(Concepto.ToUpper()))
							CONCEPTO_DUPLICADO = true;
					}
				}
				catch (Exception) { }
			}
			else
				validCells[ESQUEMA.ESQUEMADEPAGO] = false;

			// Se busca algun error.
			foreach (ESQUEMA item in Enum.GetValues(typeof(ESQUEMA)))
				if (validCells[item] == false)
					return false;
			return true;
		}

		//
		public char[] getArrayChars()
		{
			Dictionary<ESQUEMA, char> dict = new Dictionary<ESQUEMA, char>();
			foreach (ESQUEMA item in Enum.GetValues(typeof(ESQUEMA)))
				dict[item] = validCells[item] ? '1' : '0';
			return dict.Values.ToArray();
		}

		public bool Add_TMP()
		{
			try
			{
				ERRORES = getArrayChars();
				Dictionary<string, string> dic = prepareData(true);
				sql = "INSERT INTO"
					+ " ESQUEMASDEPAGO_TMP (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
					+ " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";
				return db.execute(sql);
			}
			catch { return false; }
		}

		public bool Add()
		{
            try
            {
                Dictionary<string, string> dic = prepareData(false);
                sql = "INSERT INTO"
                    + " ESQUEMASDEPAGO (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
                    + " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";
                Id_esquema = db.executeId(sql);
                if (Id_esquema == null)
                    return false;

				return true;
			}
			catch { return false; }
		}

		private Dictionary<string, string> prepareData(bool tmp)
		{
			string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

			Dictionary<string, string> dic = new Dictionary<string, string>();

			dic.Add("CVE_SEDE", Sede);
			dic.Add("PERIODO", Periodo);
		//	dic.Add("CVE_NIVEL", Nivel);
			dic.Add("ESQUEMADEPAGO", EsquemaDePago);
			dic.Add("ESQUEMADEPAGODES", EsquemaDePagoDes);
            if(tmp)
                dic.Add("CVE_CONTRATO", getClaveContrato(ClaveContrato));
            else dic.Add("CVE_CONTRATO", ClaveContrato);
            dic.Add("FECHAINICIO", FechaInicio);
			dic.Add("FECHAFIN", FechaFin);
			dic.Add("NOSEMANAS", NumSemanas);
			dic.Add("NOPAGOS", NumPagos);
			dic.Add("BLOQUEOCONTRATO", BloqueoContrato);
            
			dic.Add("FECHA_R", FECHA);
			dic.Add("USUARIO", sesion.nickName);

			if (tmp)
			{
				dic.Add("CONCEPTO", Concepto);
				dic.Add("FECHAPAGO", FechaPago);
				dic.Add("FECHARECIBO", FechaRecibo);

				dic.Add("ERRORES", string.Join("", ERRORES));
				dic.Add("E_DUPLICADO", DUPLICADO ? "1" : "0");
				dic.Add("R_DUPLICADO", CONCEPTO_DUPLICADO ? "1" : "0");
			}
			else
			{
				dic.Add("IP", "0");
				dic.Add("ELIMINADO", "0");
			}
			return dic;
		}

		public bool markEsquema()
		{
			sql = "UPDATE ESQUEMASDEPAGO_TMP SET REGISTRADO=1 WHERE ESQUEMADEPAGO='" + EsquemaDePago + "'"
				+ " AND CVE_SEDE='" + Sede + "' AND PERIODO='" + Periodo + "' ";//AND CVE_NIVEL='" + Nivel + "'
            return db.execute(sql);
		}

		public void FindFirstError(out int total, out int maxErrors, out string idsError)
		{
			maxErrors = 0;
			idsError = "";
			string sql;

			// Contar registros auditados
			sql = "SELECT COUNT(*) AS 'MAX' FROM ESQUEMASDEPAGO_TMP WHERE USUARIO='" + sesion.nickName + "'";
			total = db.Count(sql);

			// Contar errores
			string correcto = "";
			for (int i = 0; i < Enum.GetValues(typeof(ESQUEMA)).Length; i++)
				correcto += '1';
			sql = "SELECT COUNT(*) AS 'MAX' FROM ESQUEMASDEPAGO_TMP WHERE USUARIO='" + sesion.nickName + "' AND ERRORES<>'" + correcto + "'";
			maxErrors = db.Count(sql);
            
			// Consultar primeros 5 errores.
			sql = "SELECT TOP 5 * FROM ESQUEMASDEPAGO_TMP WHERE USUARIO='" + sesion.nickName + "' AND ERRORES<>'" + correcto + "'";
			idsError = "";
			ResultSet res = db.getTable(sql);
			List<string> list = new List<string>();
			while (res.Next())
				list.Add(res.Get("ESQUEMADEPAGO"));
			idsError = string.Join(", ", list);
		}

		public void generar(string str_bloqueos, out int total, out int errores)
		{
			cargaListas();

			sql = "SELECT * FROM ESQUEMASDEPAGO_TMP WHERE USUARIO='" + sesion.nickName + "'";
			ResultSet res = db.getTable(sql);
			bool ok;
			total = 0;
			errores = 0;
			while (res.Next())
			{
				edit(res);
				// Si no existe ...
				if (exist() == false)
					// Si no se puede agregar ...
					if (Add() == false)
						errores++;

				// Si existe es que ya quedó registrado.
				if (exist())
					markEsquema();

				// si no se puede agregar el concepto ...
				if (Id_esquema != null && AddConcepto() == false)
					errores++;

				AddBloqueos(str_bloqueos);

				total++;
			}
		}

		public void edit(ResultSet res)
		{
			EsquemaDePago = res.Get("ESQUEMADEPAGO");
			Sede = res.Get("CVE_SEDE");
			Periodo = res.Get("PERIODO");
		//	Nivel = res.Get("CVE_NIVEL");
			EsquemaDePagoDes = res.Get("ESQUEMADEPAGODES");
			ClaveContrato = res.Get("CVE_CONTRATO");
			FechaInicio = res.Get("FECHAINICIO");
			FechaFin = res.Get("FECHAFIN");
			NumSemanas = res.Get("NOSEMANAS");
			NumPagos = res.Get("NOPAGOS");
			BloqueoContrato = res.Get("BLOQUEOCONTRATO");
			Concepto = res.Get("CONCEPTO");
			FechaPago = res.Get("FECHAPAGO");
			FechaRecibo = res.Get("FECHARECIBO");
		}

		public bool exist()
		{
			sql = "SELECT ID_ESQUEMA FROM ESQUEMASDEPAGO"
				+ " WHERE CVE_SEDE='" + Sede + "'"
			//	+ " AND CVE_NIVEL='" + Nivel + "'"
				+ " AND PERIODO='" + Periodo + "'"
				+ " AND ESQUEMADEPAGO='" + EsquemaDePago + "'";
			ResultSet res = db.getTable(sql);
			if (res.Next())
			{
				Id_esquema = res.Get("ID_ESQUEMA");
				return true;
			}
			Id_esquema = null;
			return false;
		}

		public bool AddConcepto()
		{
			sql = "SELECT PK1 FROM ESQUEMASDEPAGOFECHAS WHERE ID_ESQUEMA='" + Id_esquema + "' AND CONCEPTO='" + Concepto + "'";

			ResultSet res = db.getTable(sql);

			// Si ya existe el concepto se obtiene el ID.
			if (res.Next())
			{
				Id_concepto = res.Get("PK1");
				return true;
			}
			else
			{
				// Si No existe la relacion: se registra
				DateTime dt;

				DateTime.TryParse(FechaPago, out dt);
				FechaPago = "'" + dt.ToString("yyyy-MM-dd") + "'";

				DateTime.TryParse(FechaRecibo, out dt);
				FechaRecibo = "'" + dt.ToString("yyyy-MM-dd") + "'";

				sql = @"
					INSERT INTO ESQUEMASDEPAGOFECHAS (ID_ESQUEMA, PAGO, CONCEPTO, FECHADEPAGO, FECHA_RECIBO, FECHA_R, USUARIO)
					VALUES ('" + Id_esquema + "','0','" + Concepto + "'," + FechaPago + "," + FechaRecibo + ",GETDATE(),'" + sesion.nickName + "')";
				Id_concepto = db.executeId(sql);
				return (Id_concepto != null);
			}
		}

		public bool AddBloqueos(string str_bloqueos)
		{
			EsquemasModel model = new EsquemasModel();
			model.Clave = Id_esquema;
			model.idPagosF = Id_concepto;
			model.bloqueos = str_bloqueos;
			model.sesion = this.sesion;
			return model.SaveBloqueos();
		}

        public string getClaveContrato(string contrato)
        {
            sql = "SELECT top 1 CVE_CONTRATO FROM FORMATOCONTRATOS WHERE CONTRATO = '" + contrato  + "'";
            ResultSet res = db.getTable(sql);

            if (res.Next())
                return res.Get("CVE_CONTRATO");
            else return "";
        }
	}
}