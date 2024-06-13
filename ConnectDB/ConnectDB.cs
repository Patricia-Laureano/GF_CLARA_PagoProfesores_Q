using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;
using System.Diagnostics;


namespace ConnectDB
{
    public class database
    {
        private string connection = string.Empty;
        private SqlConnection connect;
        private SqlCommand command;
        private SqlDataAdapter da;
        private DataTable dt;
        private DataSet ds;
        public string errorMsg = string.Empty;

        public database()
        {
            connect = new SqlConnection();
            try
            {
                connection = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            }
            catch
            {
                connection = ConfigurationManager.AppSettings.Get("connection");
            }
        }

        private SqlConnection connecttodb()
        {
            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.ConnectionString = connection;
                    connect.Open();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error de conexion:\n" + ex.Message);
                }
            }
            return connect;
        }

        private void closeconnection()
        {
            try
            {
                if (connect.State != ConnectionState.Closed)
                    connect.Close();
            }
            catch { }
        }

        public void Close()
        {
            closeconnection();
        }

        public string selectstring(string query)
        {
            string cadena = string.Empty;
            try
            {
                connecttodb();
                command = new SqlCommand(query, connect);
                cadena = command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                cadena = string.Empty;
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                closeconnection();
            }
            return cadena;
        }

        public bool execute(string query)
        {
            bool exito = false;
            try
            {
                connecttodb();
                command = new SqlCommand(query, connect);
                command.ExecuteNonQuery();
                exito = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error execute , query:" + query + ex.ToString());
            }
            finally
            {
                closeconnection();
            }
            return exito;
        }

        public string executeId(string query)
        {
            string LastId = null;
            try
            {
                connecttodb();
                command = new SqlCommand(query, connect);
                command.ExecuteNonQuery();

                command = new SqlCommand("SELECT scope_identity() AS NEWSAMPLEID", connect);
                SqlDataReader reader = command.ExecuteReader();
                if (reader != null && reader.HasRows && reader.Read())
                {
                    LastId = this.getValueField(reader, "NEWSAMPLEID");
                }
                reader.Close();
            }
            catch (SqlException sqlex)
            {
                Debug.WriteLine("Error execute , query:" + query + sqlex.ToString());
                errorMsg = sqlex.Number.ToString();
            }
            finally
            {
                closeconnection();
            }
            return LastId;
        }

        public int Count(string query)
        {
            int max = 0;
            try
            {
                connecttodb();
                SqlCommand cmd = new SqlCommand(query, connect);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader != null && reader.HasRows && reader.Read())
                {
                    max = reader.GetInt32(reader.GetOrdinal("MAX"));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error executeCount , query:" + query + ex.ToString());
            }
            finally
            {
                closeconnection();
            }
            return max;
        }

        /*  *
		public SqlDataReader getRows(string sql)
		{
			try
			{
				connecttodb();
				SqlCommand cmd = new SqlCommand(sql, connect);
				SqlDataReader sdr = cmd.ExecuteReader();

				return sdr;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}

			return null;
		}
		//*/

        public bool ExecuteStoreProcedure(string namestoreprocedure)
        {
            try
            {
                connecttodb();
                command = new SqlCommand(namestoreprocedure, connect);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error ExecuteStoreProcedure:" + namestoreprocedure + ex.ToString());
                return false;
            }
            finally
            {
                closeconnection();
            }
        }

        public bool ExecuteStoreProcedure(string namestoreprocedure, List<Parametros> parametros)
        {
            try
            {
                connecttodb();
                command = new SqlCommand(namestoreprocedure, connect);
                command.CommandType = CommandType.StoredProcedure;
                foreach (Parametros param in parametros)
                {
                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = param.nombreParametro;
                    parameter.SqlDbType = param.tipoParametro;
                    parameter.Direction = param.direccion;
                    parameter.Value = param.value;

                    command.Parameters.Add(parameter);
                }
                    

                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error ExecuteStoreProcedure:" + namestoreprocedure + ex.ToString());
                return false;
            }
            finally
            {
                closeconnection();
            }
        }

        public DataTable SelectDataTableFromStoreProcedure(string namestoreprocedure)
        {
            dt = new DataTable();
            try
            {
                connecttodb();
                command = new SqlCommand(namestoreprocedure, connect);//
                command.CommandType = CommandType.StoredProcedure;
                dt = new DataTable();
                da = new SqlDataAdapter();
                da.SelectCommand = command;
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error SelectDataTableFromStoreProcedure:" + namestoreprocedure + ex.ToString());
            }
            finally
            {
                closeconnection();
            }
            return dt;
        }

        public DataTable SelectDataTableFromStoreProcedure(string namestoreprocedure, List<Parametros> parametros)
        {
            dt = new DataTable();
            try
            {
                connecttodb();
                command = new SqlCommand(namestoreprocedure, connect);//
                command.CommandType = CommandType.StoredProcedure;
                foreach (Parametros param in parametros)
                {
                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = param.nombreParametro;
                    parameter.SqlDbType = param.tipoParametro;
                    parameter.Direction = param.direccion;
                    parameter.Value = param.value;

                    command.Parameters.Add(parameter);
                }
                dt = new DataTable();
                da = new SqlDataAdapter();
                da.SelectCommand = command;
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error SelectDataTableFromStoreProcedure:" + namestoreprocedure + ex.ToString());
            }
            finally
            {
                closeconnection();
            }
            return dt;
        }

        public DataTable SelectDataTable(string query)
        {
            dt = new DataTable();
            try
            {
                connecttodb();
                da = new SqlDataAdapter(query, connect);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error SelectDataTable:" + query + ex.ToString());
            }
            finally
            {
                connecttodb();
            }
            return dt;
        }

        public DataSet SelectDataSet(string query, string table)
        {
            ds = new DataSet();
            try
            {
                connecttodb();
                da = new SqlDataAdapter(query, connect);
                da.Fill(ds, table);
            }
            catch (Exception ex)
            {
                // ds = null;
                Debug.WriteLine("Error SelectDataSet:" + query + ",Table:" + table + ex.ToString());
            }
            finally
            {
                closeconnection();
            }
            return ds;
        }

        public ResultSet getTable(string sql)
        {
            ResultSet set;
            try
            {
                connecttodb();
                SqlCommand cmd = new SqlCommand(sql, connect);
                cmd.CommandTimeout = 40;
                SqlDataReader reader = cmd.ExecuteReader();
                
                set = new ResultSet(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                set = new ResultSet();
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                closeconnection();
            }
            return set;
        }

        public ResultSetX getTableX(string sql)
        {
            ResultSetX set;
            try
            {
                connecttodb();
                SqlCommand cmd = new SqlCommand(sql, connect);
                cmd.CommandTimeout = 40;
                SqlDataReader reader = cmd.ExecuteReader();

                set = new ResultSetX(reader);
            }
            catch (Exception ex)
            {
                set = new ResultSetX();
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                closeconnection();
            }
            return set;
        }

        public string getValueField(SqlDataReader res, string campo)
        {
            int ordinal = res.GetOrdinal(campo);
            return getValueField(res, ordinal);
        }

        public static string getValueField(SqlDataReader res, int ordinal)
        {
            string value = "";
            Type type = res.GetFieldType(ordinal);

            if (!res.IsDBNull(ordinal))
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        value = res.GetBoolean(ordinal).ToString();
                        break;

                    case TypeCode.Byte:
                        value = res.GetByte(ordinal).ToString();
                        break;

                    case TypeCode.Int16:
                        value = res.GetInt16(ordinal).ToString();
                        break;

                    case TypeCode.Int32:
                        value = res.GetInt32(ordinal).ToString();
                        break;

                    case TypeCode.Int64:
                        value = res.GetInt64(ordinal).ToString();
                        break;

                    case TypeCode.DateTime:
                        value = res.GetDateTime(ordinal).ToString();
                        break;

                    case TypeCode.Single:
                        value = res.GetFloat(ordinal).ToString();
                        break;

                    case TypeCode.Decimal:
                        value = res.GetDecimal(ordinal).ToString();
                        break;

                    case TypeCode.Double:
                        value = res.GetDouble(ordinal).ToString();
                        break;

                    case TypeCode.Char:
                        value = res.GetChar(ordinal).ToString();
                        break;

                    case TypeCode.String:
                        value = res.GetString(ordinal).Trim();
                        break;
                }
            else
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean: value = "False"; break;
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        value = "0";
                        break;
                    case TypeCode.Single:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                        value = "0.0";
                        break;
                    case TypeCode.DateTime:
                        //value = DateTime.MinValue.ToString();
                        value = "";
                        break;
                    default: value = ""; break;
                }
            return value;
        }

    }//</>

    public class ResultSet
    {
        private Dictionary<string, int> headers;
        private List<List<string>> table;
        private int current;
		public bool HasRows { get { return table != null && table.Count > 0; } }
		public int Count { get { return table == null ? 0 : table.Count; } }

		public ResultSet()
        {
            headers = new Dictionary<string, int>();
            table = new List<List<string>>();
        }

        public ResultSet(SqlDataReader reader)
        {
            if (reader != null)
            {
                // Se crean las cabeceras.
                headers = new Dictionary<string, int>();
                for (int i = 0; i < reader.FieldCount; i++)
                    headers.Add(reader.GetName(i), i);

                table = new List<List<string>>();
                if (reader.HasRows)
                {
                    // Se guardan los datos en una matriz.
                    while (reader.Read())
                    {
                        List<string> list = new List<string>();
                        for (int ordinal = 0; ordinal < reader.FieldCount; ordinal++)
                            list.Add(database.getValueField(reader, ordinal));
                        table.Add(list);
                    }
                    ReStart();
                }
            }
        }

        public void ReStart()
        {
            current = -1;
        }

        public bool Next()
        {
            return (++current) < table.Count;
        }

        // ___________ o ___________

        public string Get(string columnName)
        {
            return table[current][headers[columnName]];
        }

        public bool GetBool(string columnName)
        {
            return bool.Parse(table[current][headers[columnName]]);
        }

        public byte GetByte(string columnName)
        {
            return byte.Parse(table[current][headers[columnName]]);
        }

        public short GetShort(string columnName)
        {
            return short.Parse(table[current][headers[columnName]]);
        }

        public int GetInt(string columnName)
        {
            return int.Parse(table[current][headers[columnName]]);
        }

        public long GetLong(string columnName)
        {
            return long.Parse(table[current][headers[columnName]]);
        }

        public decimal GetDecimal(string columnName)
        {
            return decimal.Parse(table[current][headers[columnName]]);
        }

        public double GetDouble(string columnName)
        {
            return double.Parse(table[current][headers[columnName]]);
        }

        public Single GetSingle(string columnName)
        {
            return Single.Parse(table[current][headers[columnName]]);
        }

        public float GetFloat(string columnName)
        {
            return float.Parse(table[current][headers[columnName]]);
        }

        public char GetChar(string columnName)
        {
            if (table[current][headers[columnName]].Length > 0)
                return table[current][headers[columnName]][0];
            else return '\x0';
        }

        public DateTime GetDateTime(string columnName)
        {
            return DateTime.Parse(table[current][headers[columnName]]);
        }

    }//</>

    /* */
    public class ResultSetX
    {
        private Dictionary<string, int> headers;
        private List<List<object>> table;
        private int current;

        public ResultSetX()
        {
            headers = new Dictionary<string, int>();
            table = new List<List<object>>();
        }

        public ResultSetX(SqlDataReader reader)
        {
            if (reader != null && reader.HasRows)
            {
                // Se crean las cabeceras.
                headers = new Dictionary<string, int>();
                for (int i = 0; i < reader.FieldCount; i++)
                    headers.Add(reader.GetName(i), i);

                // Se guardan los datos en una matriz.
                table = new List<List<object>>();
                while (reader.Read())
                {
                    List<object> list = new List<object>();
                    for (int ordinal = 0; ordinal < reader.FieldCount; ordinal++)
                    {
                        list.Add(ResultSetX.getValueFieldX(reader, ordinal));
                    }
                    table.Add(list);
                }
                ReStart();
            }
        }

        public static object getValueFieldX(SqlDataReader res, int ordinal)
        {
            object value = "";
            Type type = res.GetFieldType(ordinal);
            if (!res.IsDBNull(ordinal))
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        value = res.GetBoolean(ordinal);
                        break;

                    case TypeCode.Byte:
                        value = res.GetByte(ordinal);
                        break;

                    case TypeCode.Int16:
                        value = res.GetInt16(ordinal);
                        break;

                    case TypeCode.Int32:
                        value = res.GetInt32(ordinal);
                        break;

                    case TypeCode.Int64:
                        value = res.GetInt64(ordinal);
                        break;

                    case TypeCode.Decimal:
                        value = res.GetDecimal(ordinal);
                        break;

                    case TypeCode.Double:
                        value = res.GetDouble(ordinal);
                        break;

                    case TypeCode.Single:
                        value = (float)res.GetFloat(ordinal);
                        break;

                    case TypeCode.Char:
                        value = res.GetChar(ordinal);
                        break;

                    case TypeCode.String:
                        value = res.GetString(ordinal);
                        break;

                    case TypeCode.DateTime:
                        value = res.GetDateTime(ordinal);
                        break;
                }
            else
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean: value = false; break;
                    case TypeCode.Byte: value = (byte)0; break;
                    case TypeCode.Int16: value = (short)0; break;
                    case TypeCode.Int32: value = 0; break;
                    case TypeCode.Int64: value = 0L; break;
                    case TypeCode.Decimal: value = (decimal)0.0f; break;
                    case TypeCode.Double: value = 0.0; break;
                    case TypeCode.Single: value = (Single)0.0; break; // con 7 digitos de presicion
                    case TypeCode.Char: value = '\x0'; break;
                    case TypeCode.String: value = ""; break;
                    case TypeCode.DateTime: value = DateTime.MinValue; break;
                }
            return value;
        }

        public void ReStart()
        {
            current = -1;
        }

        public bool Next()
        {
            return (++current) < table.Count;
        }

        //
        /* *
		public void Get(string columnName, ref object value)
		{
			value = (bool)(table[current][headers[columnName]]);
		}
		/* */
        public void Get(string columnName, ref bool value)
        {
            value = (bool)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref byte value)
        {
            value = (byte)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref short value)
        {
            value = (Int16)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref int value)
        {
            value = (Int32)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref long value)
        {
            value = (Int64)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref decimal value)
        {
            value = (decimal)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref double value)
        {
            value = (double)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref Single value) // float
        {
            value = (Single)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref char value)
        {
            value = (char)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref string value)
        {
            value = (string)(table[current][headers[columnName]]);
        }

        public void Get(string columnName, ref DateTime value)
        {
            value = (DateTime)(table[current][headers[columnName]]);
        }
        //*/
    }//</>

    public class Parametros {
        public string nombreParametro { get; set; }
        public SqlDbType tipoParametro { get; set; }
        public int longitudParametro { get; set; }
        public ParameterDirection direccion { get; set; }
        public string value { get; set; }
    }
}
