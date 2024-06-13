using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ConnectDB;
using System.Diagnostics;
using System.Data;

namespace PagoProfesores.Models.EsquemasdePago
{

	public class EsquemasModel : SuperModel
	{
		public string Clave { get; set; }
		public string sedes { get; set; }
		public string Banco { get; set; }
		public string sql { get; set; }
		public string anios { get; set; }//ciclos
		public string claveanio { get; set; }//clave ciclos
		public string periodos { get; set; }
		public string tipocontrato { get; set; }
		public string esquema { get; set; }
		public DateTime fechai { get; set; }
		public string Strmfechai { get; set; }
		public DateTime fechaf { get; set; }
		public string Strmfechaf { get; set; }

		public double nsemanas { get; set; }//cambio de int a float

		public int npagos { get; set; }
		public int bloqueo { get; set; }
		public bool bbloqueo { get; set; }
		public string idPagosF { get; set; }
		public string conceptoPago { get; set; }
		public string fechaPago { get; set; }
		public string fechaRecibo { get; set; }
		//public string StrmfechaPago { get; set; }
		//public string StrmfechaRecibo { get; set; }
		public string tipoBloqueo { get; set; }
		public string bloqueos { get; set; }
		public string[] arrBloqueos { get; set; }
        //cambio
        public string ESPECIAL { get; set; }
        public string msg { get; set; }
        public int npagosBD { get; set; }
        public bool existe { get; set; } //update
        public int personaID { get; set; }

        public string errorMsg = string.Empty;

		public string sociedades { get; set; }

		public EsquemasModel()
		{
			init();
		}

		public void init()
		{
			anios = "";
			periodos = "";
		}
        
		public bool Add() {

			string sql = "";
           	try
			{
                if (!Existe())
                {
                    existe = true;

                    sql = "INSERT INTO ESQUEMASDEPAGO(";
                    sql += "CVE_SEDE";
                    sql += ",PERIODO";
                    sql += ",ESQUEMADEPAGO";
                    sql += ",CVE_CONTRATO";
                    sql += ",FECHAINICIO";
                    sql += ",FECHAFIN";
                    sql += ",NOSEMANAS";
                    sql += ",NOPAGOS";
                    sql += ",BLOQUEOCONTRATO";
					sql += ",CVE_SOCIEDAD";
					sql += ") VALUES(";
                    sql += "'" + sedes + "'";
                    sql += ",'" + periodos + "'";
                    sql += ",'" + esquema + "'";
                    sql += ",'" + tipocontrato + "'";
                    sql += ",'" + fechai.ToString("yyyy-MM-dd") + "'";
                    sql += ",'" + fechaf.ToString("yyyy-MM-dd") + "'";
                    sql += ",'" + nsemanas + "'";
                    sql += ",'" + npagos + "'";
                    sql += ",'" + bloqueo + "'";
					sql += ",'" + sociedades + "'";
					sql += ")";

                    Debug.WriteLine("model agregar ESQUEMASDEPAGO : " + sql);
                    Clave = db.executeId(sql);

                    if (Clave != null)
                        return true;
                    else { errorMsg = db.errorMsg; return false; }
                }
                else
                {
                    existe = false;
                    return true;
                }
            }
			catch
			{
				return false;
			}
		}
        
        public bool Existe()
        {
            try
            {
                sql = "SELECT * FROM ESQUEMASDEPAGO WHERE ESQUEMADEPAGO = '" + esquema + "' AND PERIODO  = '" + periodos + "' AND CVE_SEDE = '"+ sedes + "'";
                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    return true;
                }
                else
                    return false;
            }
            catch { return false; }
        }
        
        public bool Edit()
		{
			try
			{
				sql = "SELECT * FROM ESQUEMASDEPAGO WHERE ID_ESQUEMA=" + Clave + "";
				Debug.WriteLine("model edit ESQUEMASDEPAGO" + sql);
				ResultSet res = db.getTable(sql);
                
				if (res.Next())
				{
					sedes = res.Get("CVE_SEDE");
					periodos = res.Get("PERIODO");
					Obtener_Ciclos_Periodos();
					esquema = res.Get("ESQUEMADEPAGO");
					tipocontrato = res.Get("CVE_CONTRATO");
					Strmfechai = res.GetDateTime("FECHAINICIO").ToString("yyyy-MM-dd");
					Strmfechaf = res.GetDateTime("FECHAFIN").ToString("yyyy-MM-dd");
					nsemanas = res.GetDouble("NOSEMANAS");
					npagos = res.GetInt("NOPAGOS");
					bbloqueo = res.GetBool("BLOQUEOCONTRATO");
					sociedades = res.Get("CVE_SOCIEDAD");
					return true;

				} else { return false; }
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
                sql = "UPDATE ESQUEMASDEPAGO SET ";
				sql += "CVE_SEDE = '" + sedes + "'";
				sql += ",PERIODO = '" + periodos + "'";
				sql += ",ESQUEMADEPAGO = '" + esquema + "'";
				sql += ",CVE_CONTRATO = '" + tipocontrato + "'";
				sql += ",FECHAINICIO = '" + fechai.ToString("yyyy-MM-dd") + "'";
				sql += ",FECHAFIN = '" + fechaf.ToString("yyyy-MM-dd") + "'";
				sql += ",NOSEMANAS = '" + nsemanas + "'";
				sql += ",NOPAGOS = '" + npagos + "'";
				sql += ",BLOQUEOCONTRATO = '" + bloqueo + "'";
				sql += ",CVE_SOCIEDAD = '" + sociedades + "'";
				sql += " WHERE ID_ESQUEMA = " + Clave + "";
				Debug.WriteLine("model edit ESQUEMASDEPAGO : " + sql);

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
				sql = "DELETE FROM ESQUEMASDEPAGO WHERE ID_ESQUEMA=" + Clave;
				db.execute(sql);

                return true;
			}
			catch
			{
				return false;
			}
		}
        
		public bool Obtener_Anios()
		{
			String sql = "SELECT CVE_CICLO FROM CICLOS ORDER BY CVE_CICLO DESC";
			Debug.WriteLine("CVE_CICLO sql: " + sql);
			ResultSet rs = db.getTable(sql);
			String anio = "";
			string selected = "selected";
			try
			{
				if (rs != null)
				{
					while (rs.Next())
					{
						anio += "<option  value =\"" + rs.Get("CVE_CICLO") + "\" " + selected + " > " + rs.Get("CVE_CICLO") + "</option>";
						selected = "";
					}
					anios = anio;
					return true;
				}
				else
				{ return false; }
			}
			catch
			{ return false; }
		}
        
		public bool Obtener_Periodos()
		{
			String sql = "SELECT PERIODO FROM PERIODOS WHERE CVE_CICLO = " + claveanio;
			Debug.WriteLine("PERIODO sql: " + sql);
			ResultSet rs = db.getTable(sql);
			String periodo = "";

			try
			{
				if (rs != null)
				{
					while (rs.Next())
						periodo += "<option  value =\"" + rs.Get("PERIODO") + "\"> " + rs.Get("PERIODO") + "</option>";

					periodos = periodo;
					return true;
				}
				else
				{ return false; }
			}
			catch
			{ return false; }
		}

        public bool Obtener_Ciclos_Periodos()
		{
			String sql = "SELECT CVE_CICLO FROM PERIODOS WHERE PERIODO = " + periodos;
			Debug.WriteLine("CVE_CICLO sql: " + sql);
			ResultSet rs = db.getTable(sql);

            try
			{
				if (rs != null && rs.Next()) {
					anios = rs.Get("CVE_CICLO");
					return true;
				}

				else
				{ return false; }
			}
			catch
			{ return false; }
		}

		public bool Obtener_ConceptoPago()
		{
			String sql = "SELECT CONCEPTO FROM CONCEPTOSDEPAGO ORDER BY CONCEPTO";
			Debug.WriteLine("CONCEPTO sql: " + sql);
			ResultSet rs = db.getTable(sql);
			String cpago = "";

			try
			{
				if (rs != null)
				{
					while (rs.Next())
						cpago += "<option  value =\"" + rs.Get("CONCEPTO") + "\"> " + rs.Get("CONCEPTO") + "</option>";

					conceptoPago = cpago;
					return true;
				}
				else
				{ return false; }
			}
			catch
			{ return false; }
		}
        
		public Dictionary<string, string> Obtener_TipoBloqueo()
		{
			string sql = "SELECT CVE_BLOQUEO,BLOQUEO FROM BLOQUEOS ORDER BY CVE_BLOQUEO";
			ResultSet rs = db.getTable(sql);
			Dictionary<string, string> dict = new Dictionary<string, string>();
			while (rs.Next())
				dict.Add(rs.Get("CVE_BLOQUEO"), rs.Get("BLOQUEO"));
			return dict;
			/*
			  string sbloqueo = "";
			int contador;
			for (contador = 0; rs.Next(); contador++)
				//sbloqueo += "<option value =\"" + rs.Get("CVE_BLOQUEO") + "\"> " + rs.Get("BLOQUEO") + "</option>";
				sbloqueo += "<label class='radio-inline'>\n<input id='TipoBloqueo_" + contador + "' type='checkbox' value='" + rs.Get("CVE_BLOQUEO") + "'> " + rs.Get("BLOQUEO") + "\n</label>";

			sbloqueo += "<input type='hidden' id='TipoBloqueo_length' value='" + contador + "'>";
			tipoBloqueo = sbloqueo;
			return true;
			*/
		}
        
		public bool Obtener_TipoContrato()
		{
			String sql = "SELECT CVE_CONTRATO,CONTRATO FROM FORMATOCONTRATOS ORDER BY CONTRATO";
			Debug.WriteLine("CVE_CONTRATO sql: " + sql);
			ResultSet rs = db.getTable(sql);
			String tipcontrato = "";

			try
			{
				if (rs != null)
				{
					while (rs.Next())
						tipcontrato += "<option value =\"" + rs.Get("CVE_CONTRATO") + "\"> " + rs.Get("CONTRATO") + "</option>";

					tipocontrato = tipcontrato;
					return true;
				}
				else
				{ return false; }
			}
			catch
			{ return false; }
		}

        //*******************************************
        public bool ExisteEsquema_FechaPago()
		{
			try
			{
				sql = "SELECT PK1 FROM ESQUEMASDEPAGOFECHAS WHERE ID_ESQUEMA =" + Clave + " AND FECHADEPAGO = '" + fechaPago + "'";
				Debug.WriteLine("model CalendarPagos ESQUEMASDEPAGOFECHAS" + sql);
				ResultSet fesquema = db.getTable(sql);

				if (fesquema.Next())
					return true;

				else { return false; }
			}
			catch
			{
				return false;
			}
		}

		public bool ExisteEsquema_FechaPago_Edit()
		{
			try
			{
				sql = "SELECT PK1 FROM ESQUEMASDEPAGOFECHAS WHERE ID_ESQUEMA =" + Clave
					+ "AND FECHADEPAGO = '" + fechaPago + "' AND PK1 <> '" + idPagosF + "' ";
				Debug.WriteLine("MODEL ExisteEsquema_FechaPago_Edit ESQUEMASDEPAGOFECHAS" + sql);
				ResultSet fesquema = db.getTable(sql);

				if (fesquema.Next())
					return true;

				else { return false; }

			}
			catch
			{
				return false;
			}
		}
        
        public bool AddCalendarPagos()
        {
            string sql = "";
            try
            {
                // La fecha de recibo no es obligatoria.
                DateTime dt = ParseDateTime(fechaRecibo);
                string frecibo = dt == SuperModel.minDateTime ? "NULL" : ("'" + dt.ToString("yyyy-MM-dd") + "'");

                string campo = "";
                string valorcampo = "";
                if (ESPECIAL != "" && ESPECIAL != null /*&& ESPECIAL != 0*/)
                {
                    campo = ",ESPECIAL";
                    valorcampo = ",1";
                    // valorcampo = ",'" + ESPECIAL + "'";
                }
                else
                {
                    campo = ",CVE_BLOQUEO";
                    valorcampo = ",'" + tipoBloqueo + "'";
                }
                //  sql += ",CVE_BLOQUEO"; 

                msg = "";

                if (numPagosCalendario()) {
                    sql = "INSERT INTO ESQUEMASDEPAGOFECHAS(";
                    sql += "ID_ESQUEMA";
                    sql += ",CONCEPTO";
                    sql += ",FECHADEPAGO";
                    sql += ",FECHA_RECIBO";
                    sql += campo;
                    sql += ") VALUES(";
                    sql += "'" + Clave + "'";
                    sql += ",'" + conceptoPago + "'";
                    sql += ",'" + fechaPago + "'";
                    sql += "," + frecibo + "";
                    sql += valorcampo;
                    sql += ")";
                    Debug.WriteLine("model agregar ESQUEMASDEPAGOFECHAS : " + sql);

                    this.idPagosF = db.executeId(sql);

                    Boolean res;
                    if (ESPECIAL != "" && ESPECIAL != null) { res = true; }
                    else
                        res = SaveBloqueos();

                    return (idPagosF != null) && res;
                } else {
                    msg = "EXCEDIDO";
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool AddCalendarPagosEC()
        {
            string sql = "";
            try
            {
                // La fecha de recibo no es obligatoria.
                DateTime dt = ParseDateTime(fechaRecibo);
                string frecibo = dt == SuperModel.minDateTime ? "NULL" : ("'" + dt.ToString("yyyy-MM-dd") + "'");

                string campo = "";
                string valorcampo = "";
                if (ESPECIAL != "" && ESPECIAL != null /*&& ESPECIAL != 0*/)
                {
                    campo = ",ESPECIAL";
                    valorcampo = ",1";
                    // valorcampo = ",'" + ESPECIAL + "'";
                }
                else
                {
                    campo = ",CVE_BLOQUEO";
                    valorcampo = ",'" + tipoBloqueo + "'";

                }
                //  sql += ",CVE_BLOQUEO"; 

                msg = "";

                sql = "INSERT INTO ESQUEMASDEPAGOFECHAS(";
                sql += "ID_ESQUEMA";
                sql += ",CONCEPTO";
                sql += ",FECHADEPAGO";
                sql += ",FECHA_RECIBO";
                sql += campo;
                sql += ") VALUES(";
                sql += "'" + Clave + "'";
                sql += ",'" + conceptoPago + "'";
                sql += ",'" + fechaPago + "'";
                sql += "," + frecibo + "";
                sql += valorcampo;
                sql += ")";
                Debug.WriteLine("model agregar ESQUEMASDEPAGOFECHAS : " + sql);

                this.idPagosF = db.executeId(sql);

                Boolean res;
                if (ESPECIAL != "" && ESPECIAL != null) {

                    sql = "INSERT INTO ESQUEMASDEPAGOFECHAS_ESPECIALES ("
                          + "PK_ESQUEMADEPAGOFECHA"
                          + ",ID_PERSONA"
                          + ",ID_ESQUEMA"
                          + ",USUARIO"
                          + ",FECHA_R"
                          + ") VALUES ("
                          + this.idPagosF
                          + "," + personaID
                          + "," + Clave
                          + ",'" + sesion.pkUser.ToString() + "'"
                          + ",GETDATE()"
                          + ")";
                    Debug.WriteLine("model agregar ESQUEMASDEPAGOFECHAS_ESPECIALES : " + sql);

                    db.execute(sql);

                    res = true;
                }
                else
                    res = SaveBloqueos();

                return (idPagosF != null) && res;
            }
            catch { }
            return false;
        }
        
        public bool numPagosCalendario()
        {
            sql = "SELECT COUNT(PK1) AS MAX FROM ESQUEMASDEPAGOFECHAS WHERE ID_ESQUEMA = " + Clave;
            ResultSet res = db.getTable(sql);

            if (res.Next())
            {
                npagosBD = res.GetInt("MAX");

                if (res.GetInt("MAX") < npagos)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        public bool EditCalendarPagos()
		{
			try
			{
				sql = "SELECT * FROM ESQUEMASDEPAGOFECHASBLOQUEOS WHERE PK_ESQUEMADEPAGO=" + Clave + " AND PK_ESQUEMADEPAGOFECHA=" + idPagosF;
				ResultSet res = db.getTable(sql);
				List<string> listBloqueos = new List<string>();
				while (res.Next())
					listBloqueos.Add(res.Get("CVE_BLOQUEO"));


				sql = "SELECT * FROM ESQUEMASDEPAGOFECHAS WHERE PK1 =" + idPagosF + "";
				Debug.WriteLine("controller CalendarPagos ESQUEMASDEPAGOFECHAS" + sql);
				res = db.getTable(sql);

				if (res.Next())
				{
					DateTime dt = res.GetDateTime("FECHA_RECIBO");




					string fPago = dt == DateTime.MinValue ? "" : dt.ToString("yyyy-MM-dd");

					conceptoPago = res.Get("CONCEPTO");
					fechaPago = res.GetDateTime("FECHADEPAGO").ToString("yyyy-MM-dd");
					fechaRecibo = fPago;
					tipoBloqueo = res.Get("CVE_BLOQUEO");
					arrBloqueos = listBloqueos.ToArray<string>();

					return true;
				}
			}
			catch { }
			return false;
		}
        
		public bool SaveCalendarPagos()
		{
			try
			{
				// La fecha de recibo no es obligatoria.
				DateTime dt = ParseDateTime(fechaPago);
				string fpago = dt.ToString("yyyy-MM-dd");
				dt = ParseDateTime(fechaRecibo);
                string frecibo = dt == SuperModel.minDateTime ? "NULL" : ("'" + dt.ToString("yyyy-MM-dd") + "'");

                sql = "UPDATE ESQUEMASDEPAGOFECHAS SET"
					+ " CONCEPTO = '" + conceptoPago + "'"
					+ ",FECHADEPAGO = '" + fpago + "'"
					+ ",FECHA_RECIBO = " + frecibo + ""
					+ ",CVE_BLOQUEO = '" + tipoBloqueo + "'"
					+ " WHERE PK1 = " + idPagosF;

				return db.execute(sql) && SaveBloqueos();
			}
			catch { }
			return false;
		}

        public bool UpdateEdoCtaCalendarPagos()
        {

            try
            {
                bool exito = false;
                List<Parametros> lParamS = new List<Parametros>();
                Parametros paramEsquemaID = new Parametros();
                Parametros paramConceptoPagoID = new Parametros();
                Parametros paramFechaPagoID = new Parametros();

                paramEsquemaID.nombreParametro = "@esquemaID";
                paramEsquemaID.tipoParametro = SqlDbType.BigInt;
                paramEsquemaID.direccion = ParameterDirection.Input;
                paramEsquemaID.value = Clave;
                lParamS.Add(paramEsquemaID);

                paramConceptoPagoID.nombreParametro = "@conceptoPagoID";
                paramConceptoPagoID.tipoParametro = SqlDbType.BigInt;
                paramConceptoPagoID.direccion = ParameterDirection.Input;
                paramConceptoPagoID.value = idPagosF;
                lParamS.Add(paramConceptoPagoID);

                paramFechaPagoID.nombreParametro = "@fechaPago";
                paramFechaPagoID.tipoParametro = SqlDbType.DateTime;
                paramFechaPagoID.direccion = ParameterDirection.Input;
                paramFechaPagoID.value = fechaPago;
                lParamS.Add(paramFechaPagoID);
                
                exito = db.ExecuteStoreProcedure("sp_modificaEdoCtaCalendarPagos", lParamS);

                return exito;
            }
            catch (Exception ef)
            {
                return false;
            }
        }

        public bool SaveBloqueos()
		{
			string sql = "DELETE FROM ESQUEMASDEPAGOFECHASBLOQUEOS WHERE PK_ESQUEMADEPAGOFECHA=" + idPagosF;
			bool ok = true;
			if (db.execute(sql) && bloqueos != null)
			{
				string[] array = bloqueos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string item in array)
				{
					sql =
						"INSERT INTO ESQUEMASDEPAGOFECHASBLOQUEOS (PK_ESQUEMADEPAGO, PK_ESQUEMADEPAGOFECHA,CVE_BLOQUEO,  FECHA_R,IP,USUARIO,ELIMINADO)" +
						" VALUES('" + Clave + "','" + idPagosF + "','" + item + "',  GETDATE(),'0','" + sesion.nickName + "','0')";
					ok = ok && db.execute(sql);
				}
			}
			return ok;
		}

		public bool DeleteCalendarPagos()
		{
			sql = "DELETE FROM ESQUEMASDEPAGOFECHAS WHERE PK1=" + idPagosF;
			Debug.WriteLine("model delete ESQUEMASDEPAGOFECHAS : " + sql);

			return db.execute(sql);
		}

		public string buscaSociedades()
		{
			try
			{
				//sql = "SELECT * FROM SOCIEDADES WHERE FECHAINICIO >='" + fechai.ToString("yyyy/MM/dd") + "' AND FECHAFIN <='" + fechaf.ToString("yyyy-MM-dd") + "';";
				sql = "SELECT * FROM SEDES where CVE_SEDE='" + sedes + "'";

				ResultSet res = db.getTable(sql);

				while (res.Next())
				{

					string[] sociedad = res.Get("CVE_SOCIEDAD").Split(',');

					for (var i = 0; i < sociedad.Length; i++)
					{
						string sql1 = "SELECT * FROM SOCIEDADES WHERE CVE_SOCIEDAD='" + sociedad[i] + "';";
						ResultSet ress = db.getTable(sql1);
						while (ress.Next())
						{
							if (sociedades == null)
							{
								sociedades = ress.Get("CVE_SOCIEDAD") + "," + ress.Get("SOCIEDAD");
							}
							else
							{
								sociedades = sociedades + "/" + ress.Get("CVE_SOCIEDAD") + "," + ress.Get("SOCIEDAD");
							}

						}

					}
					
				}

				return sociedades;

			}
			catch { return ""; }
		}

	}// </>
}