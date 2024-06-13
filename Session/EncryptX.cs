using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session
{
	public class EncryptX
	{
		private static readonly string Obfuscation = "C0%7r4$3%y4-P@$$w0rd-C74B3-3%crYp7";
		private static readonly int LimitOfus = 5;

		private static string InsertObfuscation(string str)
		{
			int start = CalcStart(str);
			str = str + PostObfuscationWord(start);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < str.Length; i++)
			{
				sb.Append(Obfuscation[(start + i) % Obfuscation.Length]);
				sb.Append(str[i]);
			}
			return sb.ToString();
		}

		private static int CalcStart(string str)
		{
			int acum = 0;
			for (int i = 0; i < str.Length; i++)
				acum += (int)str[i];
			return acum;
		}

		private static string RemoveObfuscation(string str)
		{


            StringBuilder sb = new StringBuilder();
			for (int i = 1; i < str.Length; i += 2)
				sb.Append(str[i]);
			sb.Remove(sb.Length - LimitOfus, LimitOfus);
			return sb.ToString();
		}

		private static string PostObfuscationWord(int start)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < LimitOfus; i++)
				sb.Append(Obfuscation[(i + start) % Obfuscation.Length]);
			return sb.ToString();
		}

		public static string Encode(string plainText)
		{
			plainText = InsertObfuscation(plainText);
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static string Decode(string base64EncodedData)
		{
            if (base64EncodedData == "NONE")
                return base64EncodedData;


            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			string str = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
			return RemoveObfuscation(str);
		}

		public static bool check(string code, string compare)
		{
			string decode = EncryptX.Decode(code);
			return decode.CompareTo(compare) == 0;
		}

	}//<end class>

}
