using ConnectDB;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{
    public class DepositoModel : SuperModel
    {
		public string TIPO_INSTRUCCION { get; set; }
		public string CODIGO_CLAVE { get; set; }
		public string STATUS_PAGO { get; set; }
		public string FECHA_APLICACION_PAGO { get; set; }// ************
		public string PLAZA_PAGO { get; set; }
		public string SUCUSAL_PAGO { get; set; }
		public string FORMA_PAGO { get; set; }
		public string TIPO_CUENTA { get; set; }
		public string BANCO_RECEPTOR { get; set; }
		public string CUENTA_ABONO { get; set; }// ************
		public string IMPORTE_PAGO { get; set; }
		public string CLAVE_BENEFICIARIO { get; set; }// ************
		public string NOMBRE_BENEFICIARIO { get; set; }
		public string REFERENCIA_PAGO { get; set; }
		public string CONCEPTO_PAGO { get; set; }// ************
		public string DIAS_VIGENCIA { get; set; }
		public string INFO { get; set; }

		public string ERRORES { get; set; }
		public long ID_ESTADODECUENTA { get; set; }
		public string FECHADEPOSITO { get; set; }
		public string USUARIO { get; set; }

		public string Clabe { get; set; }
		public string IDSIU { get; set; }
        
        public string Id_Esquema { get; set; }
        public string Esquema { get; set; }
		public string Concepto { get; set; }
        public string Id_Concepto { get; set; }
		public string FechaRegistro { get; set; }
        public string Sedes { get; set; }

        public string sql;

        public string[] arr;


        public enum DEPOSITO
		{
			TIPO_INSTRUCCION, CODIGO_CLAVE, STATUS_PAGO, FECHA_APLICACION_PAGO, PLAZA_PAGO, SUCUSAL_PAGO, FORMA_PAGO, TIPO_CUENTA, BANCO_RECEPTOR, CUENTA_ABONO, IMPORTE_PAGO, CLAVE_BENEFICIARIO, NOMBRE_BENEFICIARIO, REFERENCIA_PAGO, CONCEPTO_PAGO, DIAS_VIGENCIA, INFO, ID_ESTADODECUENTA
		};

		Dictionary<string, long> dict_IDSIU;
		Dictionary<string, long> dict_CLABE;
		Dictionary<long, string> dict_Esquemas;
		List<string> list_Conceptos;

		public void cargaListas(string sedes)
		{
			//
			// IDSIU
			//
			sql = "SELECT IDSIU, ID_PERSONA, CUENTACLABE, NOCUENTA FROM Qpersonas WHERE CVE_SEDE = '"+ sedes + "' ";
			ResultSet res = db.getTable(sql);
			dict_IDSIU = new Dictionary<string, long>();
			dict_CLABE = new Dictionary<string, long>();
			while (res.Next())
			{
				if (dict_IDSIU.Keys.Contains(res.Get("IDSIU")) == false)
					dict_IDSIU.Add(res.Get("IDSIU"), res.GetLong("ID_PERSONA"));
				if (dict_CLABE.Keys.Contains(res.Get("CUENTACLABE")) == false &&  !string.IsNullOrWhiteSpace(res.Get("CUENTACLABE")) )
					dict_CLABE.Add(res.Get("CUENTACLABE"), res.GetLong("ID_PERSONA"));
                if (dict_CLABE.Keys.Contains(res.Get("NOCUENTA")) == false && !string.IsNullOrWhiteSpace(res.Get("NOCUENTA")) )
                    dict_CLABE.Add(res.Get("NOCUENTA"), res.GetLong("ID_PERSONA"));
            }

			//
			// ESQUEMAS
			// 
			sql = "SELECT ESQUEMADEPAGO, ID_ESQUEMA FROM ESQUEMASDEPAGO";
			res = db.getTable(sql);
			dict_Esquemas = new Dictionary<long,string>();
			while (res.Next())
			{
                dict_Esquemas.Add(res.GetLong("ID_ESQUEMA"), res.Get("ESQUEMADEPAGO"));
            }

            //
            // CONCEPTOS
            // 
            sql = "SELECT DISTINCT(CONCEPTO) FROM CONCEPTOSDEPAGO";
			res = db.getTable(sql);
			list_Conceptos = new List<string>();
			while (res.Next())
			{
				list_Conceptos.Add(res.Get("CONCEPTO"));
			}
        }

        public void CopiaListasDesde(DepositoModel other)
		{
			dict_CLABE = other.dict_CLABE;
			dict_IDSIU = other.dict_IDSIU;
			dict_Esquemas = other.dict_Esquemas;
			list_Conceptos = other.list_Conceptos;
		}

		public string CutStr(int length, string str)
		{
			if (str != null && str.Length > length)
				return str.Substring(0, length);
			return str;
		}

		public void ReadWorkSheetRow(ExcelWorksheet worksheet, int row, Dictionary<string, int> col)
		{
			TIPO_INSTRUCCION = CutStr(20, worksheet.Cells[row, col["TIPO_INSTRUCCION"]].Text);
			CODIGO_CLAVE = CutStr(20, worksheet.Cells[row, col["CODIGO_CLAVE"]].Text);
			STATUS_PAGO = CutStr(20, worksheet.Cells[row, col["STATUS_PAGO"]].Text);
			FECHA_APLICACION_PAGO = CutStr(20, worksheet.Cells[row, col["FECHA_APLICACION__PAGO"]].Text);
			PLAZA_PAGO = CutStr(20, worksheet.Cells[row, col["PLAZA_PAGO"]].Text);
			SUCUSAL_PAGO = CutStr(20, worksheet.Cells[row, col["SUCUSAL_PAGO"]].Text);
			FORMA_PAGO = CutStr(20, worksheet.Cells[row, col["FORMA_PAGO"]].Text);
			TIPO_CUENTA = CutStr(20, worksheet.Cells[row, col["TIPO_CUENTA"]].Text);
			BANCO_RECEPTOR = CutStr(20, worksheet.Cells[row, col["BANCO_RECEPTOR"]].Text);
			CUENTA_ABONO = CutStr(50, worksheet.Cells[row, col["CUENTA_ABONO"]].Text.Trim());
			IMPORTE_PAGO = CutStr(20, worksheet.Cells[row, col["IMPORTE_PAGO"]].Text);
			CLAVE_BENEFICIARIO = CutStr(20, worksheet.Cells[row, col["CLAVE_BENEFICIARIO"]].Text.Trim());
			NOMBRE_BENEFICIARIO = CutStr(100, worksheet.Cells[row, col["NOMBRE_BENEFICIARIO"]].Text);
			REFERENCIA_PAGO = CutStr(50, worksheet.Cells[row, col["REFERENCIA_PAGO"]].Text);
			CONCEPTO_PAGO = CutStr(50, worksheet.Cells[row, col["CONCEPTO_PAGO"]].Text);
			DIAS_VIGENCIA = CutStr(22, worksheet.Cells[row, col["DIAS_VIGENCIA"]].Text);
			INFO = CutStr(50, worksheet.Cells[row, col["CAMPO_PARA_ORDENAR_INFORMACION"]].Text);
		}

		public char[] getArrayChars()
		{
			Dictionary<DEPOSITO, char> dict = new Dictionary<DEPOSITO, char>();
			foreach (DEPOSITO item in Enum.GetValues(typeof(DEPOSITO)))
				dict[item] = validCells[item] ? '1' : '0';
			return dict.Values.ToArray();
		}

		public bool Add_TMP()
		{
			try
			{
				ERRORES = string.Join<char>("", getArrayChars());
				Dictionary<string, string> dic = prepareData();
				sql = "INSERT INTO"
					+ " DEPOSITOS_TMP (" + string.Join<string>(",", dic.Keys.ToArray<string>()) + ")"
					+ " VALUES ('" + string.Join<string>("','", dic.Values.ToArray<string>()) + "')";

				return db.execute(sql);
			}
			catch (Exception) { }
			return false;
		}

		private Dictionary<string, string> prepareData()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("TIPO_INSTRUCCION", TIPO_INSTRUCCION);
			dic.Add("CODIGO_CLAVE", CODIGO_CLAVE);
			dic.Add("STATUS_PAGO", STATUS_PAGO);
			dic.Add("FECHA_APLICACION_PAGO", FECHA_APLICACION_PAGO);
			dic.Add("PLAZA_PAGO", PLAZA_PAGO);
			dic.Add("SUCUSAL_PAGO", SUCUSAL_PAGO);
			dic.Add("FORMA_PAGO", FORMA_PAGO);
			dic.Add("TIPO_CUENTA", TIPO_CUENTA);
			dic.Add("BANCO_RECEPTOR", BANCO_RECEPTOR);
			dic.Add("CUENTA_ABONO", CUENTA_ABONO);
			dic.Add("IMPORTE_PAGO", IMPORTE_PAGO);
			dic.Add("CLAVE_BENEFICIARIO", CLAVE_BENEFICIARIO.Trim());
			dic.Add("NOMBRE_BENEFICIARIO", NOMBRE_BENEFICIARIO);
			dic.Add("REFERENCIA_PAGO", REFERENCIA_PAGO);
			dic.Add("CONCEPTO_PAGO", CONCEPTO_PAGO);
			dic.Add("DIAS_VIGENCIA", DIAS_VIGENCIA);
			dic.Add("INFO", INFO);
			dic.Add("ERRORES", ERRORES);
			dic.Add("ID_ESTADODECUENTA", ID_ESTADODECUENTA.ToString());
			dic.Add("FECHADEPOSITO", FECHADEPOSITO);
			dic.Add("USUARIO", USUARIO);

			return dic;
		}

		public Dictionary<DEPOSITO, bool> validCells;

		public bool Validate(string sedes)
		{
			ResultSet res;

			validCells = new Dictionary<DEPOSITO, bool>();
			foreach (DEPOSITO item in Enum.GetValues(typeof(DEPOSITO)))
				validCells[item] = true;

			FECHADEPOSITO = null;
			IDSIU = null;
			Clabe = null;
			Esquema = null;
			Concepto = null;
            Id_Concepto = null;

			// Validar FECHA
			try
			{
				this.FECHADEPOSITO = new DateTime(
					int.Parse(FECHA_APLICACION_PAGO.Substring(0, 4)),
					int.Parse(FECHA_APLICACION_PAGO.Substring(4, 2)),
					int.Parse(FECHA_APLICACION_PAGO.Substring(6, 2))
					).ToString("yyyy-MM-dd");
				validCells[DEPOSITO.FECHA_APLICACION_PAGO] = true;
			}
			catch (Exception) { validCells[DEPOSITO.FECHA_APLICACION_PAGO] = false; }

            // Validar IDSIU
            validCells[DEPOSITO.CLAVE_BENEFICIARIO] = string.IsNullOrWhiteSpace(CLAVE_BENEFICIARIO) == false
                 && dict_IDSIU.Keys.Contains(CLAVE_BENEFICIARIO);

            // ESQUEMA y CONCEPTO
            validCells[DEPOSITO.CONCEPTO_PAGO] = false;
            validCells[DEPOSITO.ID_ESTADODECUENTA] = false;

            Id_Esquema = "";
            Esquema = "";
            Concepto = "";
            Id_Concepto = "";

            if (string.IsNullOrWhiteSpace(CONCEPTO_PAGO) == false)
            {
                arr = CONCEPTO_PAGO.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 4)
                {
                    Id_Esquema =  arr[0];
                    Esquema = arr[1];
                    Concepto = arr[2];
                    Id_Concepto = arr[3];
                }

                if (Concepto == "Pensión")
                    validCells[DEPOSITO.CUENTA_ABONO] = string.IsNullOrWhiteSpace(CUENTA_ABONO) == false;
                else
                    validCells[DEPOSITO.CUENTA_ABONO] =
                    string.IsNullOrWhiteSpace(CUENTA_ABONO) == false && dict_CLABE.Keys.Contains(CUENTA_ABONO);
            }
            else
                validCells[DEPOSITO.CUENTA_ABONO] =
                   string.IsNullOrWhiteSpace(CUENTA_ABONO) == false && dict_CLABE.Keys.Contains(CUENTA_ABONO);

            Id_Esquema = "";
            Esquema = "";
            Concepto = "";
            Id_Concepto = "";

            if (string.IsNullOrWhiteSpace(CONCEPTO_PAGO) == false)
			{
                if (arr.Length == 4)
				{
                    Id_Esquema = arr[0];
                    Esquema = arr[1];
                    Concepto = arr[2];
                    Id_Concepto = arr[3]; 

                    if (Concepto == "Pensión") {
                        validCells[DEPOSITO.CONCEPTO_PAGO] = true;
                        validCells[DEPOSITO.ID_ESTADODECUENTA] = true;
                        return true;
                    }
                    else
                    {
                        long ID_ESQUEMA = 0;
                        string ESQUEMA = null;
                        long ID_CONCEPTO = 0;

                        try
                        {                          
                            ID_ESQUEMA = Convert.ToInt64(Id_Esquema);
                            ESQUEMA = dict_Esquemas[ID_ESQUEMA];
                            ID_CONCEPTO = Convert.ToInt64(Id_Concepto);
                        }
                        catch (Exception) { ID_ESQUEMA = 0; ESQUEMA = "";  ID_CONCEPTO = 0; }
                        
                        // Si existe el esquema se obtiene su ID, 
                        if ( (dict_Esquemas.ContainsKey(ID_ESQUEMA) && ESQUEMA == Esquema) && list_Conceptos.Contains(Concepto))//se cambio Esquema  por Id_Esquema //dict_Esquemas.Keys.Contains(Esquema)
                        {
                            validCells[DEPOSITO.CONCEPTO_PAGO] = true;
                            
                            // Si existe el IDSIU se obteien el ID de persona.
                            if (validCells[DEPOSITO.CLAVE_BENEFICIARIO])
                            {
                                IDSIU = CLAVE_BENEFICIARIO;
                                long ID_PERSONA = dict_IDSIU[IDSIU];

                                // Consultamos el ID de concepto de pago en la relacion de esquemas-fechas
                                sql = "SELECT PK1 FROM ESQUEMASDEPAGOFECHAS WHERE ID_ESQUEMA=" + ID_ESQUEMA + " AND CONCEPTO='" + Concepto + "' AND PK1=" + ID_CONCEPTO;// ID_PERSONA=" + ID_PERSONA + " AND
                                                                                                                                                 //sql = "SELECT * FROM ESQUEMASDEPAGOFECHAS WHERE  ID_ESQUEMA=9 AND CONCEPTO='P01 Enero'";
                                res = db.getTable(sql);
                                if (res.Next())
                                {
                                    long PKCONCEPTOPAGO = res.GetLong("PK1");

                                    sql = "SELECT * FROM ESTADODECUENTA WHERE ID_PERSONA='" + ID_PERSONA + "' AND PKCONCEPTOPAGO='" + PKCONCEPTOPAGO + "' AND CVE_SEDE = '" + sedes + "'";
                                    res = db.getTable(sql);
                                    if (res.Next())
                                    {
                                        ID_ESTADODECUENTA = res.GetLong("ID_ESTADODECUENTA");

                                        validCells[DEPOSITO.ID_ESTADODECUENTA] = true;


                                        if (!(validCells[DEPOSITO.CUENTA_ABONO] && validCells[DEPOSITO.FECHA_APLICACION_PAGO]))//checar
                                            return false;

                                        return true;
                                    }
                                   
                                }
                            }
                        }
                    }
                }
			}

            return false;
		}

		public bool Generar(out int total, out int cuantos)
		{
			total = 0;
			cuantos = 0;
			bool ok = false;
			string NOT_ERROR = "";
			foreach (DEPOSITO item in Enum.GetValues(typeof(DEPOSITO)))
				NOT_ERROR += "1";

			sql = "SELECT * FROM DEPOSITOS_TMP WHERE USUARIO=" + sesion.pkUser + " AND ID_ESTADODECUENTA<>0 AND FECHADEPOSITO IS NOT NULL AND FECHADEPOSITO<>'' AND ERRORES='" + NOT_ERROR + "'";
           // sql = "SELECT * FROM DEPOSITOS_TMP WHERE USUARIO=" + sesion.pkUser + " AND ID_ESTADODECUENTA<>0 AND ERRORES='" + NOT_ERROR + "'";
            ResultSet res = db.getTable(sql);


			if (res.HasRows)
			{
				FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
				ok = true;
				total = res.Count;
				while (res.Next())
				{
					this.ID_ESTADODECUENTA = res.GetLong("ID_ESTADODECUENTA");
					this.FECHADEPOSITO = res.Get("FECHADEPOSITO");
					bool saved = Save();
					if (saved)
						cuantos++;
					ok = ok & saved;


					long ID_DEPOSITO = res.GetLong("ID_DEPOSITO");
					sql = "UPDATE DEPOSITOS_TMP SET INFO='Pago aplicado' WHERE ID_DEPOSITO=" + ID_DEPOSITO;
					db.execute(sql);
				}
            }
         
			return ok;
		}

		public bool Save()
		{
			sql = "UPDATE ESTADODECUENTA SET FECHADEPOSITO='" + FECHADEPOSITO + "', FECHA_R='" + FechaRegistro + "' WHERE ID_ESTADODECUENTA=" + ID_ESTADODECUENTA + "";

			return db.execute(sql);
		}

		public void clean()
		{
			string sql = "DELETE FROM DEPOSITOS_TMP WHERE USUARIO=" + sesion.pkUser + "";
			db.execute(sql);
		}

		public bool edit(ResultSet res)
		{
			TIPO_INSTRUCCION= res.Get("TIPO_INSTRUCCION");
			CODIGO_CLAVE = res.Get("CODIGO_CLAVE");
			STATUS_PAGO = res.Get("STATUS_PAGO");
			FECHA_APLICACION_PAGO = res.Get("FECHA_APLICACION_PAGO");
			PLAZA_PAGO = res.Get("PLAZA_PAGO");
			SUCUSAL_PAGO = res.Get("SUCUSAL_PAGO");
			FORMA_PAGO = res.Get("FORMA_PAGO");
			TIPO_CUENTA = res.Get("TIPO_CUENTA");
			BANCO_RECEPTOR = res.Get("BANCO_RECEPTOR");
			CUENTA_ABONO = res.Get("CUENTA_ABONO");
			IMPORTE_PAGO = res.Get("IMPORTE_PAGO");
			CLAVE_BENEFICIARIO = res.Get("CLAVE_BENEFICIARIO");
			NOMBRE_BENEFICIARIO = res.Get("NOMBRE_BENEFICIARIO");
			REFERENCIA_PAGO = res.Get("REFERENCIA_PAGO");
			CONCEPTO_PAGO = res.Get("CONCEPTO_PAGO");
			DIAS_VIGENCIA = res.Get("DIAS_VIGENCIA");
			INFO = res.Get("INFO");
			ERRORES = res.Get("ERRORES");
			return true;
		}

		public ResultSet getRowsTable()
		{
			sql = "SELECT * FROM DEPOSITOS_TMP WHERE USUARIO=" + sesion.pkUser + "";
			return db.getTable(sql);
		}

		public override string ToString()
		{
			return "IDSIU:" + CLAVE_BENEFICIARIO + ", E/C:" + CONCEPTO_PAGO + ", ID_EC:" + ID_ESTADODECUENTA + ", F:" + FECHADEPOSITO;
		}

	} // </>
}