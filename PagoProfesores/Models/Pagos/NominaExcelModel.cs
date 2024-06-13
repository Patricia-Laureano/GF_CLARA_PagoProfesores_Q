using ConnectDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{
	public class NominaExcelModel : SuperModel
	{
		public string CVE_SEDE;
        public string PARTEPERIODO;
        public string CVE_ESCUELA;
        public string CAMPUS_INB;
        public string PERIODO;
        public string ID_CENTRODECOSTOS;
        public string IDSIU;
		public long ID_PERSONA;
		public string TIPODEDOCENTE;
		public int NOCURSOS;
		public double HORASAPAGAR;
		public string OPCIONDEPAGO;
		public string CVE_TIPODEPAGO;
		public string TABULADOR;
        public string ID_ESQUEMA;
        public double MONTOAPAGAR;
        public string MATERIA;
        public string CURSO;
        public string NOMBREMATERIA;
		char[] ERRORES;
		// ______________________________ Listas utilizadas para validar ______________________________
		private List<string> sedes;
		private List<string> periodos;
        private List<string> centrosDeCosto;
        private List<string> opcionesDePago; // tipos de factura
		private List<string> tiposPago;
		private Dictionary<string, Dictionary<string, Dictionary<string, double>>> tabuladores;
        private Dictionary<string, Esquema> esquemas;

        public Dictionary<NOMINA, bool> validCells;

		public bool saved;
		public string sql;

		public void clean()
		{
			string sql = "DELETE FROM NOMINA_TMP WHERE USUARIO=" + sesion.pkUser;
			db.execute(sql);
		}

		public void cargaListas()
		{
			ResultSet res;
			//
			// SEDES
			//
			res = db.getTable("SELECT CVE_SEDE FROM SEDES");
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
            // CENTRODECOSTOS
            //
            //res = db.getTable("SELECT ID_CENTRODECOSTOS FROM CENTRODECOSTOS");
            res = db.getTable("SELECT distinct CVE_CENTRODECOSTOS as ID_CENTRODECOSTOS FROM CENTRODECOSTOS");
            centrosDeCosto = new List<string>();
            while (res.Next())
                centrosDeCosto.Add(res.Get("ID_CENTRODECOSTOS"));
            //
            // OPCIONES DE PAGO - [TIPOSFACTURA]
            //
            res = db.getTable("SELECT TIPOFACTURA FROM TIPOSFACTURA");
			opcionesDePago = new List<string>();
			while (res.Next())
				opcionesDePago.Add(res.Get("TIPOFACTURA").ToUpper());
			//
			// TIPOS DE DEPAGO - [TIPOSDEPAGO]
			//
			res = db.getTable("SELECT CVE_TIPODEPAGO FROM TIPOSDEPAGO");
			tiposPago = new List<string>();
			while (res.Next())
				tiposPago.Add(res.Get("CVE_TIPODEPAGO").ToUpper()); // Se guarda en mayusculas
			//
			// TABULADOR
			//
			res = db.getTable("SELECT CVE_SEDE,CVE_NIVEL,TABULADOR,Monto FROM TABULADOR");
			tabuladores = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
			while (res.Next())
			{
				string _cve_sede = res.Get("CVE_SEDE");
				string _cve_tab = res.Get("TABULADOR");
				double _monto = res.GetDouble("Monto");

				if (tabuladores.ContainsKey(_cve_sede) == false)
					tabuladores.Add(_cve_sede, new Dictionary<string, Dictionary<string, double>>());
			}
			//
			// ESQUEMAS
			//
			/*res = db.getTable("SELECT ESQUEMADEPAGO, NOSEMANAS, NOPAGOS, FECHAINICIO, FECHAFIN FROM ESQUEMASDEPAGO");
            esquemas = new Dictionary<string, Esquema>();
            while (res.Next())
				esquemas.Add(
                    res.Get("ESQUEMADEPAGO"),
                    new Esquema()
					{
                        Id = res.Get("ESQUEMADEPAGO"),
                        NumSemanas = res.GetDouble("NOSEMANAS"),
						NumPagos = res.GetInt("NOPAGOS"),
						FechaInicio = res.GetDateTime("FECHAINICIO"),
						FechaFin = res.GetDateTime("FECHAFIN"),
					}
				);*/
		}


        public void cargarEsquema(string sede, string periodo, string esquema)
        {
            ResultSet res;
            res = db.getTable("SELECT ESQUEMADEPAGO, NOSEMANAS, NOPAGOS, FECHAINICIO, FECHAFIN FROM ESQUEMASDEPAGO WHERE CVE_SEDE = '"+ sede + "' AND PERIODO = '" + periodo + "' AND ESQUEMADEPAGO = '" + esquema + "'");

            esquemas = new Dictionary<string, Esquema>();

            if (res.Count == 1)
            {
               res.Next();

                esquemas.Add(
                    res.Get("ESQUEMADEPAGO"),
                    new Esquema()
                    {
                        Id = res.Get("ESQUEMADEPAGO"),
                        NumSemanas = res.GetDouble("NOSEMANAS"),
                        NumPagos = res.GetInt("NOPAGOS"),
                        FechaInicio = res.GetDateTime("FECHAINICIO"),
                        FechaFin = res.GetDateTime("FECHAFIN"),
                    }
                  );

            }


        }

        

             public void cargarEsquemaID( string idesquema)
        {
            ResultSet res;
            res = db.getTable("SELECT   ID_ESQUEMA,ESQUEMADEPAGO, NOSEMANAS, NOPAGOS, FECHAINICIO, FECHAFIN FROM ESQUEMASDEPAGO WHERE ID_ESQUEMA = '" + idesquema + "'");

            esquemas = new Dictionary<string, Esquema>();

            if (res.Count == 1)
            {
                res.Next();

                esquemas.Add(
                    res.Get("ID_ESQUEMA"),
                    new Esquema()
                    {
                        Id = res.Get("ESQUEMADEPAGO"),
                        NumSemanas = res.GetDouble("NOSEMANAS"),
                        NumPagos = res.GetInt("NOPAGOS"),
                        FechaInicio = res.GetDateTime("FECHAINICIO"),
                        FechaFin = res.GetDateTime("FECHAFIN"),
                    }
                  );

            }


        }






        public void copiaListasDesde(NominaExcelModel other)
		{
			sedes = other.sedes;
			periodos = other.periodos;
			centrosDeCosto = other.centrosDeCosto;
			opcionesDePago = other.opcionesDePago;
			tiposPago = other.tiposPago;
			tabuladores = other.tabuladores;
			esquemas = other.esquemas;
		}

		// ____________________________  VALIDAR ____________________________
		public enum NOMINA
		{
			CVE_SEDE, PARTEPERIODO, PERIODO, ID_CENTRODECOSTOS, IDSIU, ID_PERSONA, TIPODEDOCENTE, NOCURSOS, HORASAPAGAR, OPCIONDEPAGO, CVE_TIPODEPAGO, TABULADOR, ID_ESQUEMA, MONTOAPAGAR, CVE_ESCUELA, CAMPUS_INB, MATERIA, CURSO, NOMBREMATERIA//, TABULADOR_MONTO, NOSEMANAS, NOPAGOS, FECHAINICIO, FECHAFIN, MONTOAPAGAR, ORIGEN
		};

		public bool validate()
		{
			// De inicio todo es valido.
			validCells = new Dictionary<NOMINA, bool>();
			foreach (NOMINA item in Enum.GetValues(typeof(NOMINA)))
				validCells[item] = true;

			validCells[NOMINA.CVE_SEDE] = (CVE_SEDE.Length <= 5) && sedes.Contains(CVE_SEDE);

            validCells[NOMINA.PARTEPERIODO] = (PARTEPERIODO.Length <= 3);//
            validCells[NOMINA.CVE_ESCUELA] = (CVE_ESCUELA.Length <= 5);//
            validCells[NOMINA.CAMPUS_INB] = (CAMPUS_INB.Length <= 5);//

            validCells[NOMINA.PERIODO] = (PERIODO.Length <= 10) && periodos.Contains(PERIODO);
			validCells[NOMINA.ID_CENTRODECOSTOS] = centrosDeCosto.Contains(ID_CENTRODECOSTOS);
			// La opcion de pago se busca en mayusculas.
			validCells[NOMINA.OPCIONDEPAGO] = (OPCIONDEPAGO.Length <= 15) && opcionesDePago.Contains(OPCIONDEPAGO.ToUpper());
			validCells[NOMINA.CVE_TIPODEPAGO] = (CVE_TIPODEPAGO.Length <= 15) && tiposPago.Contains(CVE_TIPODEPAGO);
            validCells[NOMINA.TABULADOR] = (TABULADOR.Length <= 5) && tabuladores.ContainsKey(CVE_SEDE);
			validCells[NOMINA.ID_ESQUEMA] = esquemas.ContainsKey(ID_ESQUEMA);

			// **************** NOTA: Es ncesario calcular el monto a pagar *************************
			validCells[NOMINA.MONTOAPAGAR] = true;
			// ****************************************************************************************

			validCells[NOMINA.IDSIU] = IDSIU.Length <= 10;
			if (validCells[NOMINA.IDSIU])
			{
				ResultSet res = db.getTable("SELECT TOP 1 ID_PERSONA FROM PERSONAS WHERE IDSIU='" + IDSIU + "'"); // aquí se tiene que hacer una modificación, agregarle el filtro de sede
				if (res.Next())
					ID_PERSONA = res.GetLong("ID_PERSONA");
				else
					validCells[NOMINA.IDSIU] = validCells[NOMINA.ID_PERSONA] = false;
			}
			else
				validCells[NOMINA.ID_PERSONA] = false;

			// Se busca algun error.
			foreach (NOMINA item in Enum.GetValues(typeof(NOMINA)))
				if (validCells[item] == false)
					return false;
			return true;
		}

		public object[] getArrayObject(Dictionary<NOMINA, object> dict)
		{
			dict[NOMINA.CVE_SEDE] = CVE_SEDE;
            dict[NOMINA.PARTEPERIODO] = PARTEPERIODO;
            dict[NOMINA.PERIODO] = PERIODO;
            dict[NOMINA.ID_CENTRODECOSTOS] = ID_CENTRODECOSTOS;
            dict[NOMINA.IDSIU] = IDSIU.ToString();
			dict[NOMINA.ID_PERSONA] = ID_PERSONA.ToString();
			dict[NOMINA.TIPODEDOCENTE] = TIPODEDOCENTE;
			dict[NOMINA.NOCURSOS] = NOCURSOS;
			dict[NOMINA.HORASAPAGAR] = HORASAPAGAR.ToString();
			dict[NOMINA.OPCIONDEPAGO] = OPCIONDEPAGO;
			dict[NOMINA.CVE_TIPODEPAGO] = CVE_TIPODEPAGO;
			dict[NOMINA.TABULADOR] = TABULADOR;
			dict[NOMINA.ID_ESQUEMA] = ID_ESQUEMA;
			dict[NOMINA.MONTOAPAGAR] = MONTOAPAGAR;
            dict[NOMINA.CVE_ESCUELA] = CVE_ESCUELA;
            dict[NOMINA.CAMPUS_INB] = CAMPUS_INB;
            dict[NOMINA.MATERIA] = MATERIA;
            dict[NOMINA.CURSO] = CURSO;
            dict[NOMINA.NOMBREMATERIA] = NOMBREMATERIA;

            return dict.Values.ToArray();
		}

		public char[] getArrayChars()
		{
			Dictionary<NOMINA, char> dict = new Dictionary<NOMINA, char>();
			dict[NOMINA.CVE_SEDE] = validCells[NOMINA.CVE_SEDE] ? '1' : '0';
            dict[NOMINA.PARTEPERIODO] = validCells[NOMINA.PARTEPERIODO] ? '1' : '0';
            dict[NOMINA.PERIODO] = validCells[NOMINA.PERIODO] ? '1' : '0';
			dict[NOMINA.ID_CENTRODECOSTOS] = validCells[NOMINA.ID_CENTRODECOSTOS] ? '1' : '0';
			dict[NOMINA.IDSIU] = validCells[NOMINA.IDSIU] ? '1' : '0';
			dict[NOMINA.ID_PERSONA] = validCells[NOMINA.ID_PERSONA] ? '1' : '0';

			dict[NOMINA.TIPODEDOCENTE] = validCells[NOMINA.TIPODEDOCENTE] ? '1' : '0';
			dict[NOMINA.NOCURSOS] = validCells[NOMINA.NOCURSOS] ? '1' : '0';
			dict[NOMINA.HORASAPAGAR] = validCells[NOMINA.HORASAPAGAR] ? '1' : '0';

			dict[NOMINA.OPCIONDEPAGO] = validCells[NOMINA.OPCIONDEPAGO] ? '1' : '0';
			dict[NOMINA.CVE_TIPODEPAGO] = validCells[NOMINA.CVE_TIPODEPAGO] ? '1' : '0';
			dict[NOMINA.TABULADOR] = validCells[NOMINA.TABULADOR] ? '1' : '0';
			dict[NOMINA.ID_ESQUEMA] = validCells[NOMINA.ID_ESQUEMA] ? '1' : '0';
			dict[NOMINA.MONTOAPAGAR] = validCells[NOMINA.MONTOAPAGAR] ? '1' : '0';

            dict[NOMINA.CVE_ESCUELA] = validCells[NOMINA.CVE_ESCUELA] ? '1' : '0';
            dict[NOMINA.CAMPUS_INB] = validCells[NOMINA.CAMPUS_INB] ? '1' : '0';

            dict[NOMINA.MATERIA] = validCells[NOMINA.MATERIA] ? '1' : '0';
            dict[NOMINA.CURSO] = validCells[NOMINA.CURSO] ? '1' : '0';
            dict[NOMINA.NOMBREMATERIA] = validCells[NOMINA.NOMBREMATERIA] ? '1' : '0';

            return dict.Values.ToArray();
		}

		public void FindFirstError(out int total, out int maxErrors, out string idsError)
		{
			maxErrors = 0;
			idsError = "";
			string sql;

			// Contar registros auditados
			sql = "SELECT COUNT(*) AS 'MAX' FROM NOMINA_TMP WHERE USUARIO = " + sesion.pkUser;
			total = db.Count(sql);

			// Contar errores
			string correcto = "";
			for (int i = 0; i < Enum.GetValues(typeof(NOMINA)).Length; i++)
				correcto += '1';
			sql = "SELECT COUNT(*) AS 'MAX' FROM NOMINA_TMP WHERE USUARIO = " + sesion.pkUser + " AND ERRORES <> '" + correcto + "'";
			maxErrors = db.Count(sql);
	
			// Consultar primeros 5 errores.
			sql = "SELECT TOP 5 * FROM NOMINA_TMP WHERE USUARIO = " + sesion.pkUser + " AND ERRORES <> '" + correcto + "'";
			idsError = "";
			ResultSet res = db.getTable(sql);
			List<string> list = new List<string>();
			while (res.Next())
				list.Add(res.Get("IDSIU"));
			idsError = string.Join(",", list);
		}

		public bool Add_TMP()
		{
			try
			{
				ERRORES = getArrayChars();
				Dictionary<string, string> dic = prepareData(true, true);
				sql = "INSERT INTO"
					+ " NOMINA_TMP (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
					+ " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";
				return db.execute(sql);
			}
			catch { return false; }
		}

		private Dictionary<string, string> prepareData(bool add, bool tmp)
		{
			string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
			Dictionary<string, string> dic = new Dictionary<string, string>();

			if (add)
			{
				dic.Add("IDSIU", IDSIU);
				dic.Add("ID_PERSONA", ID_PERSONA.ToString());
				dic.Add("CVE_SEDE", CVE_SEDE);
                dic.Add("PARTEDELPERIODO", PARTEPERIODO);
                dic.Add("PERIODO", PERIODO);
                if (tmp) dic.Add("ID_CENTRODECOSTOS", obtieneCentroCostosID());
                else dic.Add("ID_CENTRODECOSTOS", ID_CENTRODECOSTOS);
            }
			dic.Add("TIPODEDOCENTE", TIPODEDOCENTE);
			dic.Add("NOCURSOS", NOCURSOS.ToString());
			dic.Add("HORASAPAGAR", HORASAPAGAR.ToString("F"));
			dic.Add("OPCIONDEPAGO", OPCIONDEPAGO);
			dic.Add("CVE_TIPODEPAGO", CVE_TIPODEPAGO);
			dic.Add("TABULADOR", TABULADOR);
            if (tmp) dic.Add("ID_ESQUEMA", obtieneEsquemaPagoID());
            else dic.Add("ID_ESQUEMA", ID_ESQUEMA);
            if (MONTOAPAGAR == 0)
                MONTOAPAGAR = calculaMontoAPagar();
            dic.Add("MONTOAPAGAR", MONTOAPAGAR.ToString("F"));
            dic.Add("CVE_ESCUELA", CVE_ESCUELA);
            dic.Add("CAMPUS_INB", CAMPUS_INB);
            if (tmp)
			{
                dic.Add("MATERIA", MATERIA);
                dic.Add("CURSO", CURSO);
                dic.Add("NOMBREMATERIA", NOMBREMATERIA);
                dic.Add("USUARIO", sesion.pkUser.ToString());
				dic.Add("ERRORES", string.Join("", ERRORES));
			}
			else
			{
				if (esquemas.ContainsKey(ID_ESQUEMA))
				{
					dic.Add("FECHAINICIO", esquemas[ID_ESQUEMA].FechaInicio.ToString("yyyy-MM-dd"));
					dic.Add("FECHAFIN", esquemas[ID_ESQUEMA].FechaFin.ToString("yyyy-MM-dd"));
				}
				else
				{
					dic.Add("FECHAINICIO", "2000-01-01");
					dic.Add("FECHAFIN", "2000-01-01");
				}

				dic.Add("ORIGEN", "EXCEL");
                dic.Add("INDICADOR", "11");
                if (add) dic.Add("FECHA_R", FECHA);
				else dic.Add("FECHA_M", FECHA);
				dic.Add("IP", "0");
				dic.Add("USUARIO", sesion.nickName);
				dic.Add("ELIMINADO", "0");
			}
			return dic;
		}

		public bool Add()
		{
            bool resNom = false;
            string idNomina = "0";
            ResultSet res = null;
			try
			{
				Dictionary<string, string> dic = prepareData(true, false);
				sql = "INSERT INTO"
					+ " NOMINA (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
					+ " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";
                idNomina  = db.executeId(sql);

                if (Int32.Parse(idNomina) > 0)
                {
                    //sql = "select SCOPE_IDENTITY() as 'ID_NOMINA'";
                    //res = db.getTable(sql);
                    //if (res.Count > 0)
                    //    if (res.Next())
                    //    {
                            sql = "insert into NOMINA_MATERIACURSO (id_nomina, curso, materia, nombremateria, fecha_r, usuario) "
    + "                     values (" + idNomina + ", '" + CURSO + "', '" + MATERIA + "', '" + NOMBREMATERIA + "', GETDATE(), '" + sesion.nickName + "')";

                            resNom = db.execute(sql);
                        //}
                }

                return resNom;
            }
			catch { return false; }
		}

		public bool Save()
		{
			try
			{
				List<string> values = new List<string>();
				foreach (KeyValuePair<string, string> item in prepareData(false, false))
					values.Add(item.Key + "='" + item.Value + "'");

				sql = "UPDATE NOMINA SET " + string.Join<string>(", ", values.ToArray<string>())
					+ " WHERE IDSIU = " + IDSIU + "";
				return db.execute(sql);
			}
			catch { return false; }
		}

        public bool existCaso1()
        {
            sql = "SELECT COUNT(*) AS 'MAX'     "
                + "  FROM NOMINA                "
                + " WHERE IDSIU              = '" + IDSIU    + "'"
                + "   AND ID_PERSONA         =  " + ID_PERSONA
                + "   AND CVE_SEDE           = '" + CVE_SEDE + "'"
                + "   AND PERIODO           != '" + PERIODO  + "'";
            return db.Count(sql) > 0;
        }

        public bool existCaso2()
        {
            sql = "SELECT COUNT(*) AS 'MAX'     "
                + "  FROM NOMINA                "
                + " WHERE IDSIU              = '" + IDSIU    + "'"
                + "   AND ID_PERSONA         =  " + ID_PERSONA
                + "   AND CVE_SEDE           = '" + CVE_SEDE + "'"
                + "   AND PERIODO            = '" + PERIODO  + "'"
                + "   AND ID_CENTRODECOSTOS  =  " + ID_CENTRODECOSTOS
                + "   AND ID_ESQUEMA        !=  " + ID_ESQUEMA;
            return db.Count(sql) > 0;
        }

        public bool existCaso3()
        {
            sql = "SELECT COUNT(*) AS 'MAX'     "
                + "  FROM NOMINA                "
                + " WHERE IDSIU              = '" + IDSIU    + "'"
                + "   AND ID_PERSONA         =  " + ID_PERSONA
                + "   AND CVE_SEDE           = '" + CVE_SEDE + "'"
                + "   AND PERIODO            = '" + PERIODO  + "'"
                + "   AND ID_ESQUEMA         =  " + ID_ESQUEMA
                + "   AND ID_CENTRODECOSTOS !=  " + ID_CENTRODECOSTOS;
            return db.Count(sql) > 0;
        }

        public bool existCaso4()
		{
            sql = "SELECT COUNT(*) AS 'MAX'    "
                + "  FROM NOMINA               "
                + " WHERE IDSIU             = '" + IDSIU             + "'"
                + "   AND CVE_SEDE          = '" + CVE_SEDE          + "'"
                + "   AND PERIODO           = '" + PERIODO           + "'"
                + "   AND ID_CENTRODECOSTOS =  " + ID_CENTRODECOSTOS
                + "   AND ID_PERSONA        =  " + ID_PERSONA
                + "   AND ID_ESQUEMA        =  " + ID_ESQUEMA;
                //+ "   AND PARTEDELPERIODO   = '" + PARTEPERIODO      + "'"
                //+ "   AND CVE_ESCUELA       = '" + CVE_ESCUELA       + "'"
                //+ "   AND CVE_TIPODEPAGO    = '" + CVE_TIPODEPAGO    + "'"
                //+ "   AND CAMPUS_INB        = '" + CAMPUS_INB        + "'";
			return db.Count(sql) > 0;
		}

        public bool exist()
        {
            sql = "SELECT COUNT(*) AS 'MAX'    "
                + "  FROM NOMINA               "
                + " WHERE IDSIU             = '" + IDSIU + "'"
                + "   AND CVE_SEDE          = '" + CVE_SEDE + "'"
                + "   AND PERIODO           = '" + PERIODO + "'"
                + "   AND ID_CENTRODECOSTOS =  " + ID_CENTRODECOSTOS
                + "   AND ID_PERSONA        =  " + ID_PERSONA
                + "   AND ID_ESQUEMA        =  " + ID_ESQUEMA
            +"   AND PARTEDELPERIODO   = '" + PARTEPERIODO + "'"
            + "   AND CVE_ESCUELA       = '" + CVE_ESCUELA + "'"
            + "   AND CVE_TIPODEPAGO    = '" + CVE_TIPODEPAGO + "'"
            + "   AND CAMPUS_INB        = '" + CAMPUS_INB + "'";
            return db.Count(sql) > 0;
        }

        public bool edit(ResultSet res)
		{
            try
            {
                CVE_SEDE = res.Get("CVE_SEDE");
                PARTEPERIODO = res.Get("PARTEDELPERIODO");
                CVE_ESCUELA = res.Get("CVE_ESCUELA");
                CAMPUS_INB = res.Get("CAMPUS_INB");
                PERIODO = res.Get("PERIODO");
                ID_CENTRODECOSTOS = res.Get("ID_CENTRODECOSTOS");
                IDSIU = res.Get("IDSIU");
                ID_PERSONA = res.GetLong("ID_PERSONA");
                TIPODEDOCENTE = res.Get("TIPODEDOCENTE");
                NOCURSOS = res.GetInt("NOCURSOS");
                HORASAPAGAR = res.GetDouble("HORASAPAGAR");
                OPCIONDEPAGO = res.Get("OPCIONDEPAGO");
                CVE_TIPODEPAGO = res.Get("CVE_TIPODEPAGO");
                TABULADOR = res.Get("TABULADOR");
                ID_ESQUEMA = res.Get("ID_ESQUEMA");
                MONTOAPAGAR = res.GetDouble("MONTOAPAGAR");
                MATERIA = res.Get("MATERIA");
                CURSO = res.Get("CURSO");
                NOMBREMATERIA = res.Get("NOMBREMATERIA");
            }
            catch { return false; }
            return true;
        }

		public bool generar(out int total, out int errores)
		{
			cargaListas();

            sql = "SELECT * FROM V_NOMINA_TMPG WHERE USUARIO = " + sesion.pkUser;
            ResultSet res = db.getTable(sql);
			bool ok = false;
			total = 0;
			errores = 0;
			while (res.Next())
			{

                //nuevo
                cargarEsquemaID(res.Get("ID_ESQUEMA"));




                if (edit(res) == false)
                {
                    ok = false;
                    break;
                }
                //if (exist())
                //    ok = Save();
                if (existCaso4())
                    ok = UpdateNominaXExcel();
                //if (existCaso3())
                //    ok = Add();
                //if (existCaso2())
                //    ok = Add();
                //if (existCaso1())
                //    ok = Add();
                else
                    ok = Add();
                if (!ok)
					errores++;
				total++;
			}

            return ok;
		}

        public bool generarEdoCta()
        {
            bool ok = false;

            try
            {
                var sql1 = "   select CVE_SEDE, PERIODO " +
                           "     from NOMINA_TMP " +
                           "    where USUARIO = '" + sesion.pkUser + "'" +
                           " group by CVE_SEDE, PERIODO ";

                ResultSet res1 = db.getTable(sql1);

                while (res1.Next())
                {
                    sql = "   select PERIODO, CVE_SEDE, CVE_TIPODEPAGO, CAMPUS_INB, CVE_ESCUELA, PARTEDELPERIODO " +
                          "     from NOMINA " +
                          "    where PERIODO = '" + res1.Get("PERIODO") + "'" +
                          "      and CVE_SEDE = '" + res1.Get("CVE_SEDE") + "'" +
                          "      and INDICADOR = 11" +
                          " group by PERIODO, CVE_SEDE, CVE_TIPODEPAGO, CAMPUS_INB, CVE_ESCUELA, PARTEDELPERIODO ";

                    ResultSet res = db.getTable(sql);

                    NominaXCDCModel modelXCDC;

                    while (res.Next())
                    {
                        modelXCDC = new NominaXCDCModel();
                        modelXCDC.Periodo = res.Get("PERIODO");
                        modelXCDC.CampusVPDI = res.Get("CVE_SEDE");
                        modelXCDC.Campus = res.Get("CAMPUS_INB");
                        modelXCDC.Escuela = res.Get("CVE_ESCUELA");
                        modelXCDC.PartePeriodo = res.Get("PARTEDELPERIODO");
                        modelXCDC.TipoPago = res.Get("CVE_TIPODEPAGO");

                        modelXCDC.insertaEntregaContratosXEsquemaPago();
                        modelXCDC.calculaEstadocuentaXRegistroNomina(sesion.pkUser.ToString(), "11");

                        //modelXCDC.calculaEstadocuentaXEsquemaPago();
                    }
                }

                ok = true;
            }
            catch (Exception e) { return false;  }

            return ok;
        }

        public double calculaMontoAPagar()
        {
            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramEsquemaID = new Parametros();
            Parametros paramHorasPagar = new Parametros();
            Parametros paramTabulador = new Parametros();
            Parametros paramSede = new Parametros();

            DataTable dtMontoPagar = new DataTable();

            double resMontoPagar = 0;

            try
            {
                paramHorasPagar.nombreParametro = "@pHorasPagar";
                paramHorasPagar.tipoParametro = SqlDbType.Real;
                paramHorasPagar.direccion = ParameterDirection.Input;
                paramHorasPagar.value = HORASAPAGAR.ToString();
                lParamS.Add(paramHorasPagar);

                paramEsquemaID.nombreParametro = "@pEsquemaPagoID";
                paramEsquemaID.tipoParametro = SqlDbType.BigInt;
                paramEsquemaID.direccion = ParameterDirection.Input;
                paramEsquemaID.value = obtieneEsquemaPagoID();
                lParamS.Add(paramEsquemaID);

                paramTabulador.nombreParametro = "@pTabulador";
                paramTabulador.tipoParametro = SqlDbType.NVarChar;
                paramTabulador.longitudParametro = 15;
                paramTabulador.direccion = ParameterDirection.Input;
                paramTabulador.value = TABULADOR;
                lParamS.Add(paramTabulador);

                paramSede.nombreParametro = "@pCveSede";
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.longitudParametro = 5;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = CVE_SEDE;
                lParamS.Add(paramSede);

                dtMontoPagar = db.SelectDataTableFromStoreProcedure("sp_calculaMontoAPagar", lParamS);
                if (dtMontoPagar.Rows.Count == 1)
                    resMontoPagar = double.Parse(dtMontoPagar.Rows[0][0].ToString());
            }
            catch
            {
                return resMontoPagar;
            }
            return resMontoPagar;
        }

        public string obtieneCentroCostosID()
        {
            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramCC = new Parametros();
            Parametros paramSede = new Parametros();
            Parametros paramTipoPago = new Parametros();
            
            DataTable dtCentroCostosID = new DataTable();

            string resCentroCostosID = "0";

            try
            {
                paramCC.nombreParametro = "@pCc";
                paramCC.tipoParametro = SqlDbType.NVarChar;
                paramCC.longitudParametro = 10;
                paramCC.direccion = ParameterDirection.Input;
                paramCC.value = ID_CENTRODECOSTOS;
                lParamS.Add(paramCC);

                paramSede.nombreParametro = "@pSede";
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.longitudParametro = 5;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = CVE_SEDE;
                lParamS.Add(paramSede);

                paramTipoPago.nombreParametro = "@pTipoPago";
                paramTipoPago.tipoParametro = SqlDbType.NVarChar;
                paramTipoPago.longitudParametro = 5;
                paramTipoPago.direccion = ParameterDirection.Input;
                paramTipoPago.value = CVE_TIPODEPAGO;
                lParamS.Add(paramTipoPago);

                dtCentroCostosID = db.SelectDataTableFromStoreProcedure("sp_obtieneCentroCostosID", lParamS);
                if (dtCentroCostosID.Rows.Count == 1)
                    resCentroCostosID = dtCentroCostosID.Rows[0][0].ToString();
            }
            catch
            {
                return resCentroCostosID;
            }
            return resCentroCostosID;
        }

        public string obtieneEsquemaPagoID()
        {
            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramEsquemaPago = new Parametros();
            Parametros paramPeriodo = new Parametros();
            Parametros paramSede = new Parametros();

            DataTable dtEsquemaPagoID = new DataTable();

            string resEsquemaPagoID = "0";

            try
            {
                paramEsquemaPago.nombreParametro = "@pEsquemaPago";
                paramEsquemaPago.tipoParametro = SqlDbType.NVarChar;
                paramEsquemaPago.longitudParametro = 15;
                paramEsquemaPago.direccion = ParameterDirection.Input;
                paramEsquemaPago.value = ID_ESQUEMA;
                lParamS.Add(paramEsquemaPago);

                paramPeriodo.nombreParametro = "@pPeriodo";
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = PERIODO;
                lParamS.Add(paramPeriodo);

                paramSede.nombreParametro = "@pSede";
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.longitudParametro = 5;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = CVE_SEDE;
                lParamS.Add(paramSede);

                dtEsquemaPagoID = db.SelectDataTableFromStoreProcedure("sp_obtieneEsquemaPagoID", lParamS);
                if (dtEsquemaPagoID.Rows.Count == 1)
                    resEsquemaPagoID = dtEsquemaPagoID.Rows[0][0].ToString();
            }
            catch
            {
                return resEsquemaPagoID;
            }
            return resEsquemaPagoID;
        }

        public bool UpdateNominaXExcel()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramSede = new Parametros();
            Parametros paramPeriodo = new Parametros();
            Parametros paramCCID = new Parametros();
            Parametros paramEsquemaID = new Parametros();
            Parametros paramPersonaID = new Parametros();
            Parametros paramMontoPagar = new Parametros();
            Parametros paramMateria = new Parametros();
            Parametros paramCurso = new Parametros();
            Parametros paramNombreMateria = new Parametros();
            Parametros paramUsuario = new Parametros();

            try
            {
                paramSede = new Parametros();
                paramSede.nombreParametro = "@pCveSede";
                paramSede.longitudParametro = 5;
                paramSede.tipoParametro = SqlDbType.NVarChar;
                paramSede.direccion = ParameterDirection.Input;
                paramSede.value = CVE_SEDE;
                lParamS.Add(paramSede);

                paramPeriodo = new Parametros();
                paramPeriodo.nombreParametro = "@pPeriodo";
                paramPeriodo.longitudParametro = 10;
                paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                paramPeriodo.direccion = ParameterDirection.Input;
                paramPeriodo.value = PERIODO;
                lParamS.Add(paramPeriodo);

                paramCCID = new Parametros();
                paramCCID.nombreParametro = "@pCCId";
                paramCCID.tipoParametro = SqlDbType.BigInt;
                paramCCID.direccion = ParameterDirection.Input;
                paramCCID.value = ID_CENTRODECOSTOS;
                lParamS.Add(paramCCID);

                paramEsquemaID = new Parametros();
                paramEsquemaID.nombreParametro = "@pEsquemaID";
                paramEsquemaID.tipoParametro = SqlDbType.BigInt;
                paramEsquemaID.direccion = ParameterDirection.Input;
                paramEsquemaID.value = ID_ESQUEMA.ToString();
                lParamS.Add(paramEsquemaID);

                paramPersonaID = new Parametros();
                paramPersonaID.nombreParametro = "@pPersonaID";
                paramPersonaID.tipoParametro = SqlDbType.BigInt;
                paramPersonaID.direccion = ParameterDirection.Input;
                paramPersonaID.value = ID_PERSONA.ToString();
                lParamS.Add(paramPersonaID);

                paramMontoPagar = new Parametros();
                paramMontoPagar.nombreParametro = "@pMontoPagar";
                paramMontoPagar.tipoParametro = SqlDbType.Real;
                paramMontoPagar.direccion = ParameterDirection.Input;
                paramMontoPagar.value = MONTOAPAGAR.ToString();
                lParamS.Add(paramMontoPagar);

                paramMateria = new Parametros();
                paramMateria.nombreParametro = "@pMateria";
                paramMateria.longitudParametro = 15;
                paramMateria.tipoParametro = SqlDbType.NVarChar;
                paramMateria.direccion = ParameterDirection.Input;
                paramMateria.value = MATERIA;
                lParamS.Add(paramMateria);

                paramCurso = new Parametros();
                paramCurso.nombreParametro = "@pCurso";
                paramCurso.longitudParametro = 10;
                paramCurso.tipoParametro = SqlDbType.NVarChar;
                paramCurso.direccion = ParameterDirection.Input;
                paramCurso.value = CURSO;
                lParamS.Add(paramCurso);

                paramNombreMateria = new Parametros();
                paramNombreMateria.nombreParametro = "@pNombreMateria";
                paramNombreMateria.longitudParametro = 150;
                paramNombreMateria.tipoParametro = SqlDbType.NVarChar;
                paramNombreMateria.direccion = ParameterDirection.Input;
                paramNombreMateria.value = NOMBREMATERIA;
                lParamS.Add(paramNombreMateria);

                paramUsuario = new Parametros();
                paramUsuario.nombreParametro = "@pUsuario";
                paramUsuario.longitudParametro = 180;
                paramUsuario.tipoParametro = SqlDbType.NVarChar;
                paramUsuario.direccion = ParameterDirection.Input;
                paramUsuario.value = sesion.nickName;
                lParamS.Add(paramUsuario);
                
                exito = db.ExecuteStoreProcedure("sp_actualizaNominaXExcel", lParamS);
                if (exito == false)
                    return false;

                return true;
            }
            catch 
            {
                return false;
            }
        }
    }

	public class Esquema
	{
        public string Id { get; set; }
        public double NumSemanas { get; set; }
		public int NumPagos { get; set; }
		public DateTime FechaInicio { get; set; }
		public DateTime FechaFin { get; set; }
	}
}