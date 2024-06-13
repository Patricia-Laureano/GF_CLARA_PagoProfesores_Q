using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{
	public class Retencion
	{
		public string CVE_SEDE { get; set; }
		public int NUMPAGOS { get; set; }
		public double MONTO { get; set; }
		public double MONTO_IVA { get; set; }
		public double MONTO_IVARET { get; set; }
		public double MONTO_ISRRET { get; set; }
		public string TIPOTRANSFERENCIA { get; set; }
		public string TIPODEPAGO { get; set; }
		public int MESDEPOSITO { get; set; }
		public int ANIODEPOSITO { get; set; }
		public long ID_ESTADODECUENTA { get; set; }
		public long PADRE { get; set; }

		public int MESDEPOSITO_INI { get; set; }
		public int MESDEPOSITO_FIN { get; set; }
		public string PERIODO { get; set; }
		public string CVE_NIVEL { get; set; }

		public Retencion(ResultSet res)
		{
			CVE_SEDE = res.Get("CVE_SEDE");
			NUMPAGOS = res.GetInt("NUMPAGOS");
			MONTO = res.GetDouble("MONTO");
			MONTO_IVA = res.GetDouble("MONTO_IVA");
			MONTO_IVARET = res.GetDouble("MONTO_IVARET");
			MONTO_ISRRET = res.GetDouble("MONTO_ISRRET");
			TIPOTRANSFERENCIA = res.Get("TIPOTRANSFERENCIA");
			TIPODEPAGO = res.Get("TIPODEPAGO");
			MESDEPOSITO_INI = MESDEPOSITO_FIN = MESDEPOSITO = res.GetInt("MESDEPOSITO");
			ANIODEPOSITO = res.GetInt("ANIODEPOSITO");
			ID_ESTADODECUENTA = res.GetInt("ID_ESTADODECUENTA");
			PADRE = res.GetLong("PADRE");
		}

		public void AcumulaMontoDe(Retencion other)
		{
			NUMPAGOS += other.NUMPAGOS;
			MONTO += other.MONTO;
			MONTO_IVA += other.MONTO_IVA;
			MONTO_IVARET += other.MONTO_IVARET;
			MONTO_ISRRET += other.MONTO_ISRRET;

			MESDEPOSITO_INI = Math.Min(MESDEPOSITO_INI, other.MESDEPOSITO_INI);
			MESDEPOSITO_FIN = Math.Max(MESDEPOSITO_FIN, other.MESDEPOSITO_FIN);
		}

		public override string ToString()
		{
			return "id:" + ID_ESTADODECUENTA + ", padre:" + PADRE + ", mes:" + MESDEPOSITO + ", monto:" + MONTO;
		}

		public static List<Retencion> calcRetencionesMensuales(string IDSIU, string SEDE, string PERIODO, string CVE_NIVEL, int ANIO)
		{
			database db = new database();
			string sql = "SELECT CVE_SEDE, NUMPAGOS, MONTO, MONTO_IVA, MONTO_IVARET, MONTO_ISRRET, TIPOTRANSFERENCIA, TIPODEPAGO, MESDEPOSITO, ANIODEPOSITO, ID_ESTADODECUENTA, PADRE"
				+ " FROM VESTADO_CUENTA"
				+ " WHERE IDSIU='" + IDSIU + "' AND CVE_SEDE='" + SEDE + "' AND ANIODEPOSITO=" + ANIO + " AND (FECHADEPOSITO IS NOT NULL)";
			if (string.IsNullOrWhiteSpace(PERIODO) == false)
				sql += " AND PERIODO='" + PERIODO + "'";
			if (string.IsNullOrWhiteSpace(CVE_NIVEL) == false)
				sql += " AND CVE_NIVEL='" + CVE_NIVEL + "'";
			sql += " ORDER BY MESDEPOSITO";
			ResultSet res = db.getTable(sql);

			// Paso 1. Se leen todas las retenciones.
			List<Retencion> todas = new List<Retencion>();
			while (res.Next())
				todas.Add(new Retencion(res));

			// Paso 2. Extrae los elementos 'padre' y ordenalos por mes
			List<Retencion> listPadres = todas.FindAll(x => x.PADRE == 0).OrderBy(x => x.MESDEPOSITO).ToList();

			// Paso 4. Estrae los elementos 'hijo'
			List<Retencion> listHijos = todas.FindAll(x => x.PADRE != 0);

			foreach (Retencion padre in listPadres)
			{
				long YO = padre.ID_ESTADODECUENTA;
				List<Retencion> misHijos = listHijos.FindAll(x => x.PADRE == YO);

				foreach (Retencion miHijo in misHijos)
				{
					padre.AcumulaMontoDe(miHijo);
				}
			}

			List<int> meses = new List<int>();
			foreach (Retencion padre in listPadres)
				if (meses.Contains(padre.MESDEPOSITO) == false)
					meses.Add(padre.MESDEPOSITO);

			meses = meses.OrderBy(x => x).ToList();

			List<Retencion> listRetenciones = new List<Retencion>();
			foreach (int mes in meses)
			{
				// Busca todas las retenciones de ese mes.
				List<Retencion> rets_x_mes = listPadres.FindAll(x => x.MESDEPOSITO == mes);
				// Extrae la primera retencion
				Retencion acumulado = rets_x_mes[0];
				// Acumula las subsecuentes retenciones;
				rets_x_mes.RemoveAt(0);
				foreach (Retencion item in rets_x_mes)
					acumulado.AcumulaMontoDe(item);

				listRetenciones.Add(acumulado);
			}

			return listRetenciones;
		}

		public static List<Retencion> calcRetencionesAnuales(string IDSIU, string SEDE, string PERIODO, string CVE_NIVEL)
		{
			database db = new database();
			string sql = "SELECT CVE_SEDE, NUMPAGOS, MONTO, MONTO_IVA, MONTO_IVARET, MONTO_ISRRET, TIPOTRANSFERENCIA, TIPODEPAGO, MESDEPOSITO, ANIODEPOSITO, ID_ESTADODECUENTA, PADRE"
				+ " FROM VESTADO_CUENTA"
				+ " WHERE IDSIU='" + IDSIU + "' AND CVE_SEDE='" + SEDE + "' AND (FECHADEPOSITO IS NOT NULL)";
			if (string.IsNullOrWhiteSpace(PERIODO) == false)
				sql += " AND PERIODO='" + PERIODO + "'";
			if (string.IsNullOrWhiteSpace(CVE_NIVEL) == false)
				sql += " AND CVE_NIVEL='" + CVE_NIVEL + "'";
			sql += " ORDER BY MESDEPOSITO";
			ResultSet res = db.getTable(sql);

			// Paso 1. Se leen todas las retenciones.
			List<Retencion> todas = new List<Retencion>();
			while (res.Next())
				todas.Add(new Retencion(res));

			// Paso 2. Extrae los elementos 'padre' y ordenalos por AÑO
			List<Retencion> listPadres = todas.FindAll(x => x.PADRE == 0).OrderBy(x => x.ANIODEPOSITO).ToList();

			// Paso 4. Estrae los elementos 'hijo'
			List<Retencion> listHijos = todas.FindAll(x => x.PADRE != 0);

			foreach (Retencion padre in listPadres)
			{
				long YO = padre.ID_ESTADODECUENTA;
				List<Retencion> misHijos = listHijos.FindAll(x => x.PADRE == YO);

				foreach (Retencion miHijo in misHijos)
				{
					padre.AcumulaMontoDe(miHijo);
					//listHijos.Remove(miHijo);
				}
			}

			List<int> listYears = new List<int>();
			foreach (Retencion padre in listPadres)
				if (listYears.Contains(padre.ANIODEPOSITO) == false)
					listYears.Add(padre.ANIODEPOSITO);

			listYears = listYears.OrderBy(x => x).ToList();

			List<Retencion> listRetenciones = new List<Retencion>();
			foreach (int year in listYears)
			{
				// Busca todas las retenciones de ese año.
				List<Retencion> rets_x_anio = listPadres.FindAll(x => x.ANIODEPOSITO == year);
				// Extrae la primera retencion
				Retencion acumulado = rets_x_anio[0];
				// Acumula las subsecuentes retenciones;
				rets_x_anio.RemoveAt(0);
				foreach (Retencion item in rets_x_anio)
					acumulado.AcumulaMontoDe(item);

				listRetenciones.Add(acumulado);
			}

			return listRetenciones;
		}
	}
}