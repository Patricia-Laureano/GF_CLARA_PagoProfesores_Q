using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Session;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ConnectDB;
using System.Diagnostics;

namespace PagoProfesores.Models.Personas
{
	public class DatosPersonasModel : SuperModel
	{
        public string Id_Persona                { get; set; }
        public string Idsiu                     { get; set; }
        public string Profesor                  { get; set; }
        public string Noi                       { get; set; }
        public string Apellidos                 { get; set; }
        public string Nombres                   { get; set; }
        public string Sexo                      { get; set; }
        public string Fechanacimiento           { get; set; }
        public string Nacionalidad              { get; set; }
        public string Telefono                  { get; set; }
        public string Correo                    { get; set; }
        public string CorreoOffice              { get; set; }
        public string Cve_Tipodepago            { get; set; }
        public string Mysuite                   { get; set; }
        public string Cve_Origen                { get; set; }
        public string Datos_Fiscales            { get; set; }
        public string Rfc                       { get; set; }
        public string Curp                      { get; set; }
        public string Direccion_Pais            { get; set; }
        public string Direccion_Estado          { get; set; }
        public string Direccion_Ciudad          { get; set; }
        public string Direccion_Entidad         { get; set; }
        public string Direccion_Colonia         { get; set; }
        public string Direccion_Calle           { get; set; }
        public string Direccion_Numero          { get; set; }
        public string Direccion_Cp              { get; set; }
        public string Cve_Banco                 { get; set; }
        public string Cuentaclabe               { get; set; }
        public string Nocuenta                  { get; set; }
        public string Razonsocial               { get; set; }
        public string Rz_Rfc                    { get; set; }
        public string Rz_Curp                   { get; set; }
        public string Rz_Represnetante_Legal_P { get; set; }
        public string Rz_Direccion_Pais         { get; set; }
        public string Rz_Direccion_Estado       { get; set; }
        public string Rz_Direccion_Ciudad       { get; set; }
        public string Rz_Direccion_Entidad      { get; set; }
        public string Rz_Direccion_Colonia      { get; set; }
        public string Rz_Direccion_Calle        { get; set; }
        public string Rz_Direccion_Numero       { get; set; }
        public string Rz_Direccion_Cp           { get; set; }
        public string cveSede                   { get; set; }
		public string TituloProfesional         { get; set; }
		public string Profesion                 { get; set; }
		public string CedulaProfesional         { get; set; }
		public string FechaCedula               { get; set; }
		public string SeguroSocial              { get; set; }
		public string TipoPension               { get; set; }
        public bool   PersActivo                { get; set; }
        public string Fecha_R                   { get; set; }
        public string Fecha_M                   { get; set; }
        public string Ip                        { get; set; }
        public string Usuario                   { get; set; }
        public string Eliminado                 { get; set; }
        public string msg                       { get; set; }
        public string sql                       { get; set; } //update
        public bool   existe                    { get; set; } //update

        public string notarial_no { get; set; }
        public string notarial_volumen { get; set; }
        public string notaria_lugar { get; set; }
        public string notarial_notario_no { get; set; }
        public string notarial_lic { get; set; }

        public string Fi_Direccion_Pais { get; set; }
        public string Fi_Direccion_Estado { get; set; }
        public string Fi_Direccion_Ciudad { get; set; }
        public string Fi_Direccion_Entidad { get; set; }
        public string Fi_Direccion_Colonia { get; set; }
        public string Fi_Direccion_Calle { get; set; }
        public string Fi_Direccion_Numero { get; set; }
        public string Fi_Direccion_Cp { get; set; }

        public DatosPersonasModel() { }

		public string ComboSql(string Sql, string cve, string valor, string Inicial,bool opcionone)
		{
			string MySql = Sql;
			string Combo = "\r\n";
			string Clave = "";
			string Valor = "";
			string s = "";

			ResultSet reader = db.getTable(Sql);
			try
			{

                if (opcionone)
                {
                    Combo = "<option value=\"\"></ option >";
                }

				while (reader.Next())
				{
					Clave = reader.Get(cve);
					Valor = reader.Get(valor);
					if (Clave == Inicial || Valor == Inicial)
					{
						s = "Selected";
					}
					else
					{
						s = "";
					}


					Combo = Combo + "<option value =\"" + Clave + "\" " + s + ">";
					Combo += Valor + " </ option >\r\n";
				}
				return Combo;
			}
			catch
			{
				return "Error en consulta combo";
			}
		}
        
        private Dictionary<string, string> prepareData(bool add)
		{
			Usuario = sesion.nickName;
			Fecha_R = Fecha_M = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
			try { Fechanacimiento = DateTime.Parse(Fechanacimiento).ToString("yyyy-MM-dd"); }
			catch (Exception) { Fechanacimiento = ""; }
			try { FechaCedula = DateTime.Parse(FechaCedula).ToString("yyyy-MM-dd"); }
			catch (Exception) { FechaCedula = ""; }

			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("IDSIU", Idsiu);
			dic.Add("PROFESOR", Profesor);
			dic.Add("NOI", Noi);
			dic.Add("APELLIDOS", Apellidos);
			dic.Add("NOMBRES", Nombres);
			dic.Add("SEXO", Sexo);		
			dic.Add("NACIONALIDAD", Nacionalidad);
			dic.Add("TELEFONO", Telefono);
			dic.Add("CORREO", Correo);
            dic.Add("CORREO365", CorreoOffice);
			dic.Add("MYSUITE", (Mysuite == "1") ? "1" : "0");
			dic.Add("CVE_ORIGEN", Cve_Origen);
			dic.Add("DATOSFISCALES", Datos_Fiscales);
			dic.Add("RFC", Rfc);
			dic.Add("CURP", Curp);
			dic.Add("DIRECCION_PAIS", Direccion_Pais);
			dic.Add("DIRECCION_ESTADO", Direccion_Estado);
			dic.Add("DIRECCION_CIUDAD", Direccion_Ciudad);
			dic.Add("DIRECCION_ENTIDAD", Direccion_Entidad);
			dic.Add("DIRECCION_COLONIA", Direccion_Colonia);
			dic.Add("DIRECCION_CALLE", Direccion_Calle);
			dic.Add("DIRECCION_NUMERO", Direccion_Numero);
			dic.Add("DIRECCION_CP", Direccion_Cp);
			dic.Add("CVE_BANCO", Cve_Banco);
			dic.Add("CUENTACLABE", Cuentaclabe);
			dic.Add("NOCUENTA", Nocuenta);
			dic.Add("RAZONSOCIAL", Razonsocial);
			dic.Add("RZ_RFC", Rz_Rfc);
			dic.Add("RZ_CURP", Rz_Curp);

            dic.Add("RZ_DIRECCION_PAIS", Rz_Direccion_Pais);
			dic.Add("RZ_DIRECCION_ESTADO", Rz_Direccion_Estado);
			dic.Add("RZ_DIRECCION_CIUDAD", Rz_Direccion_Ciudad);
			dic.Add("RZ_DIRECCION_ENTIDAD", Rz_Direccion_Entidad);
			dic.Add("RZ_DIRECCION_COLONIA", Rz_Direccion_Colonia);
			dic.Add("RZ_DIRECCION_CALLE", Rz_Direccion_Calle);
			dic.Add("RZ_DIRECCION_NUMERO", Rz_Direccion_Numero);
			dic.Add("RZ_DIRECCION_CP", Rz_Direccion_Cp);
			dic.Add("TITULOPROFESIONAL", TituloProfesional);
			dic.Add("PROFESION", Profesion);
			dic.Add("CEDULAPROFESIONAL", CedulaProfesional);

            dic.Add("NOTARIAL_NO", notarial_no);
            dic.Add("NOTARIAL_VOLUMEN", notarial_volumen);
            dic.Add("NOTARIAL_NOTARIO_NO", notarial_notario_no);
            dic.Add("NOTARIAL_LUGAR", notaria_lugar);
            dic.Add("NOTARIAL_LIC", notarial_lic);

            dic.Add("REPRESENTANTE_LEGAL_P", Rz_Represnetante_Legal_P);

            dic.Add("FI_DIRECCION_PAIS", Fi_Direccion_Pais);
            dic.Add("FI_DIRECCION_ESTADO", Fi_Direccion_Estado);
            dic.Add("FI_DIRECCION_CIUDAD", Fi_Direccion_Ciudad);
            dic.Add("FI_DIRECCION_ENTIDAD", Fi_Direccion_Entidad);
            dic.Add("FI_DIRECCION_COLONIA", Fi_Direccion_Colonia);
            dic.Add("FI_DIRECCION_CALLE", Fi_Direccion_Calle);
            dic.Add("FI_DIRECCION_NUMERO", Fi_Direccion_Numero);
            dic.Add("FI_DIRECCION_CP", Fi_Direccion_Cp);


            if (!string.IsNullOrWhiteSpace(FechaCedula)) { dic.Add("FECHACEDULA", FechaCedula); }
            if (!string.IsNullOrWhiteSpace(Fechanacimiento)) { dic.Add("FECHANACIMIENTO", Fechanacimiento); }
            dic.Add("SEGUROSOCIAL", SeguroSocial);
			dic.Add("TIPOPENSION", TipoPension);
            dic.Add("ACTIVO", PersActivo ? "1" : "0");
            if (add) dic.Add("FECHA_R", Fecha_R);
			else dic.Add("FECHA_M", Fecha_M);
			dic.Add("IP", Ip);
			dic.Add("USUARIO", Usuario);

			return dic;
		}

		public bool Add()
		{
			try
			{
				Dictionary<string, string> dic = prepareData(true);

                if (!Existe())
                {
                    existe = true;
                    sql = "INSERT INTO"
                    + " PERSONAS (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
                    + " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";

                    Id_Persona = db.executeId(sql);

                    if (Id_Persona != null && Id_Persona != string.Empty)
                    {
                        sql = "INSERT INTO PERSONAS_SEDES (ID_PERSONA, CVE_SEDE, CVE_TIPODEPAGO, FECHA_R, USUARIO) " +
                              "                    VALUES (" + Id_Persona + ", '" + cveSede + "', '" + Cve_Tipodepago + "', GETDATE(), '" + sesion.pkUser.ToString() + "')";

                        Log.write(this, "PERSONAS_SEDES", LOG.REGISTRO, ("sql: ") + sql, this.sesion);

                        return db.execute(sql);
                    }
                    else
                    {
                        return false;
                    }
                }
                else {
                    existe = false;
                    return true;
                }
			}
			catch { return false; }
		}

        public bool Existe()
        {
            try
            {
                sql = "SELECT * FROM QPersonas WHERE IDSIU = '" + Idsiu + "' AND CVE_SEDE = '" + cveSede + "'";
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

		public bool Save()
		{
			try
			{
				string cuenta = string.Empty;
				string clabe = string.Empty;

				List<string> values = new List<string>();
				List<string> valuesPS = new List<string>();
				foreach (KeyValuePair<string, string> item in prepareData(false))
				{
					values.Add(item.Key + "='" + item.Value + "' ");
					if (item.Key == "CUENTACLABE")
					{
						clabe = item.Value;
					}
					if (item.Key == "NOCUENTA")
					{
						cuenta = item.Value;
					}
				}

				string sqlcuentas = "SELECT * FROM PERSONAS WHERE ID_PERSONA = " + Id_Persona;
				ResultSet rescs = db.getTable(sqlcuentas);

				if (rescs.Next())
				{
					if (cuenta != rescs.Get("NOCUENTA").ToString())
					{
						values.Add("ID_USUARIO_MODIFICACUENTA ='" + sesion.pkUser + "' ");
						values.Add("FECHA_MODIFICACUENTA =GETDATE()");
					}
					if (clabe != rescs.Get("CUENTACLABE").ToString())
					{
						values.Add("ID_USUARIO_MODIFICACLABE ='" + sesion.pkUser + "' ");
						values.Add("FECHA_MODIFICACLABE =GETDATE()");
					}
				}

				sql = "UPDATE PERSONAS SET " + string.Join<string>(",", values.ToArray<string>())
					+ " WHERE ID_PERSONA = " + Id_Persona;

				if (db.execute(sql))
				{
					sql = " UPDATE PERSONAS_SEDES "
						+ "    SET CVE_TIPODEPAGO = '" + Cve_Tipodepago + "'"
						+ "  WHERE ID_PERSONA = " + Id_Persona
						+ "    AND CVE_SEDE   = '" + cveSede + "'";

					return db.execute(sql);
				}
				else
				{
					return false;
				}
			}
			catch { return false; }
		}

		public bool SaveTP()
        {
            try
            {
                sql = "UPDATE PA SET CVE_OPCIONDEPAGO = '" + Cve_Tipodepago + "', "
                    + " OPCIONDEPAGO = (case when '" + Cve_Tipodepago + "' = 'HDI' then (select TIPOFACTURA from TIPOSFACTURA where CVE_TIPOFACTURA = 'H')"
                    + "                      when '" + Cve_Tipodepago + "' = 'ADI' then (select TIPOFACTURA from TIPOSFACTURA where CVE_TIPOFACTURA = 'A')"
                    + "                      when '" + Cve_Tipodepago + "' = 'FDI' then (select TIPOFACTURA from TIPOSFACTURA where CVE_TIPOFACTURA = 'F')"
                    + "                      else (select TIPOFACTURA from TIPOSFACTURA where CVE_TIPOFACTURA = 'E') end) "
                    + " WHERE ID_PERSONA = " + Id_Persona
                    + "   and indicador = 0";
                    //+ "   and CVE_SEDE = '" + + "'"
                    //+ "   and PERIODO = '" + + "'";
                return db.execute(sql);
            }
            catch { return false; }
        }

        public bool Edit()
		{
			try
			{
				sql = "SELECT *, CONVERT(varchar(10), FECHACEDULA, 103 ) AS 'F_CEDULA' FROM QPersonas WHERE ID_PERSONA = " + Id_Persona + " AND CVE_SEDE = '" + cveSede + "'";
				
				ResultSet res = db.getTable(sql);

				if (res.Next())
				{
					Id_Persona = res.Get("ID_PERSONA");
					Idsiu = res.Get("IDSIU");
					Profesor = res.Get("PROFESOR");
					Noi = res.Get("NOI");
					Apellidos = res.Get("APELLIDOS");
					Nombres = res.Get("NOMBRES");
					Sexo = res.Get("SEXO");
					Fechanacimiento = res.Get("FECHANACIMIENTO");
                    Nacionalidad = res.Get("NACIONALIDAD").ToUpper();
					Telefono = res.Get("TELEFONO");
					Correo = res.Get("CORREO");
                    CorreoOffice = res.Get("CORREO365");
                    Cve_Tipodepago = res.Get("CVE_TIPODEPAGO");
					Mysuite = res.Get("MYSUITE");
					Cve_Origen = res.Get("CVE_ORIGEN");
					Datos_Fiscales = res.Get("DATOSFISCALES");
					Rfc = res.Get("RFC");
					Curp = res.Get("CURP");
					Direccion_Pais = res.Get("DIRECCION_PAIS");
					Direccion_Estado = res.Get("DIRECCION_ESTADO");
					Direccion_Ciudad = res.Get("DIRECCION_CIUDAD");
					Direccion_Entidad = res.Get("DIRECCION_ENTIDAD");
					Direccion_Colonia = res.Get("DIRECCION_COLONIA");
					Direccion_Calle = res.Get("DIRECCION_CALLE");
					Direccion_Numero = res.Get("DIRECCION_NUMERO");
					Direccion_Cp = res.Get("DIRECCION_CP");
					Cve_Banco = res.Get("CVE_BANCO");
					Cuentaclabe = res.Get("CUENTACLABE");
					Nocuenta = res.Get("NOCUENTA");
					Razonsocial = res.Get("RAZONSOCIAL");
					Rz_Rfc = res.Get("RZ_RFC");
					Rz_Curp = res.Get("RZ_CURP");
                    Rz_Represnetante_Legal_P = res.Get("REPRESENTANTE_LEGAL_P");
                    Rz_Direccion_Pais = res.Get("RZ_DIRECCION_PAIS");
					Rz_Direccion_Estado = res.Get("RZ_DIRECCION_ESTADO");
					Rz_Direccion_Ciudad = res.Get("RZ_DIRECCION_CIUDAD");
					Rz_Direccion_Entidad = res.Get("RZ_DIRECCION_ENTIDAD");
					Rz_Direccion_Colonia = res.Get("RZ_DIRECCION_COLONIA");
					Rz_Direccion_Calle = res.Get("RZ_DIRECCION_CALLE");
					Rz_Direccion_Numero = res.Get("RZ_DIRECCION_NUMERO");
					Rz_Direccion_Cp = res.Get("RZ_DIRECCION_CP");
					TituloProfesional = res.Get("TITULOPROFESIONAL");
					Profesion = res.Get("PROFESION");
					CedulaProfesional = res.Get("CEDULAPROFESIONAL");
					FechaCedula = res.Get("F_CEDULA"); //FECHACEDULA
					SeguroSocial = res.Get("SEGUROSOCIAL");
					TipoPension = res.Get("TIPOPENSION");
                    PersActivo = res.GetBool("ACTIVO");

                    notarial_no = res.Get("NOTARIAL_NO");
                    notarial_volumen = res.Get("NOTARIAL_VOLUMEN"); //FECHACEDULA
                    notaria_lugar = res.Get("NOTARIAL_LUGAR");
                    notarial_notario_no = res.Get("NOTARIAL_NOTARIO_NO");
                    notarial_lic = res.Get("NOTARIAL_LIC");

                    Fi_Direccion_Pais = res.Get("FI_DIRECCION_PAIS");
                    Fi_Direccion_Estado = res.Get("FI_DIRECCION_ESTADO");
                    Fi_Direccion_Ciudad = res.Get("FI_DIRECCION_CIUDAD");
                    Fi_Direccion_Entidad = res.Get("FI_DIRECCION_ENTIDAD");
                    Fi_Direccion_Colonia = res.Get("FI_DIRECCION_COLONIA");
                    Fi_Direccion_Calle = res.Get("FI_DIRECCION_CALLE");
                    Fi_Direccion_Cp = res.Get("FI_DIRECCION_CP");
                    //Fi_Direccion_Numero = res.Get("FI_DIRECCION_NUMERO");

                    return true;
				}
				else
					return false;
			}
			catch { return false; }
		}
        
        public bool getIdPersona()
        {
            try
            {
                sql = "SELECT ID_PERSONA FROM QPersonas WHERE IDSIU = '" + Idsiu + "' AND CVE_SEDE = '" + cveSede + "'";

                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    Id_Persona = res.Get("ID_PERSONA");
                    msg = "Éxito";

                    return true;
                }
                else
                {
                    Id_Persona = "-1";
                    msg = "Sin éxito";
                    return false;
                }
            }
            catch { return false; }
        }
        
        internal void Obtener_Direccion(Dictionary<int, string> dict)
		{
			throw new NotImplementedException();
		}

		public bool Delete()
		{
			try
			{
                //primero el usuario debe eliminar todos sus pagos(desde edocuenta*)

                sql = "DELETE FROM ENTREGADECONTRATOS WHERE ID_PERSONA = " + Id_Persona + "";
                db.execute(sql);


                sql = "DELETE FROM PERSONAS WHERE ID_PERSONA = " + Id_Persona + "";
				if (db.execute(sql)) { return true; } else { return false; }
			}
			catch
			{
				return false;
			}
		}
        
		public bool Obtener_Direccion()
		{
			string sql = "SELECT D_ESTADO FROM SEPOMEX WHERE (d_codigo LIKE N'%" + Direccion_Cp + "%') GROUP BY D_ESTADO";
			Debug.WriteLine("Obtener_Direccion sql: " + sql);
			ResultSet rs = db.getTable(sql);
			String estado = "";

			Direccion_Pais = Obtener_Pais();
			Direccion_Ciudad = Obtener_Ciudad();
			Direccion_Entidad = Obtener_Municiodel();
			Direccion_Colonia = Obtener_Colonia();

			try
			{
				if (rs != null)
				{
					while (rs.Next())
					{
						estado += "<option>" + rs.Get("D_ESTADO") + "</option>";
					}

					Direccion_Estado = estado;
					return true;
				}
				else
				{ return false; }
			}
			catch
			{ return false; }
		}


		private string Obtener_Colonia()
		{
			string sql = "SELECT D_ASENTA FROM SEPOMEX WHERE (d_codigo LIKE N'%" + Direccion_Cp + "%') GROUP BY D_ASENTA";
			ResultSet rs = db.getTable(sql);
			String colonia = "";

			try
			{
				if (rs != null)
				{
					while (rs.Next())
						colonia += "<option>" + rs.Get("D_ASENTA") + "</option>";
				}

				return colonia;
			}
			catch
			{ return colonia; }
		}

		private string Obtener_Pais()
		{
			string sql = "SELECT PAIS FROM SEPOMEX WHERE (d_codigo LIKE N'%" + Direccion_Cp + "%') GROUP BY PAIS";
			ResultSet rs = db.getTable(sql);
			String pais = "";

			try
			{
				if (rs != null)
				{
					while (rs.Next())
						pais += "<option>" + rs.Get("PAIS") + "</option>";
				}

				return pais;
			}
			catch
			{ return pais; }
		}

		private string Obtener_Municiodel()
		{
			string sql = "SELECT D_MNPIO FROM SEPOMEX WHERE (d_codigo LIKE N'%" + Direccion_Cp + "%') GROUP BY D_MNPIO";
			ResultSet rs = db.getTable(sql);
			String mundel = "";

			try
			{
				if (rs != null)
				{
					while (rs.Next())
						mundel += "<option>" + rs.Get("D_MNPIO") + "</option>";
				}

				return mundel;
			}
			catch
			{ return mundel; }
		}

		private string Obtener_Ciudad()
		{
			string sql = "SELECT D_CIUDAD FROM SEPOMEX WHERE (d_codigo LIKE N'%" + Direccion_Cp + "%') GROUP BY D_CIUDAD";
			ResultSet rs = db.getTable(sql);
			String ciudad = "";

			try
			{
				if (rs != null)
				{
					while (rs.Next())
						ciudad += "<option>" + rs.Get("D_CIUDAD") + "</option>";
				}

				return ciudad;
			}
			catch
			{ return ciudad; }
		}
	}
}