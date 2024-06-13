using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Jose;

namespace JasonWebToken.Controllers
{
    public class TokenController : Controller
    {
        static byte[] Base64UrlDecode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default:
                    throw new System.Exception(
             "Illegal base64url string!");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        static long ToUnixTime(DateTime dateTime)
        {
            return (int)(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public void GenerarToken(string Periodo = "", string Nivel = "", string IDSIU = "", string NombreCompleto = "", string Campus = "")//index
        {
            string strm_Tag = ConfigurationManager.ConnectionStrings["GetToken"].ConnectionString;
            string[] array_tag = strm_Tag.Split(';');
            string Key = array_tag[0].Substring(4);
            string URL = array_tag[1].Substring(5);

            byte[] secretKey = Base64UrlDecode(Key);//93874938327481928349
            DateTime issued = DateTime.Now;
            DateTime expire = DateTime.Now.AddHours(10);

            var payload = new Dictionary<string, object>()
            {
                {"iss", "https://banner.mx/"},
                {"aud", "283932719"},
                {"sub", "pagoprofesores"},
                {"iat", ToUnixTime(issued).ToString()},
                {"exp", ToUnixTime(expire).ToString()},
                {"IDSIU", IDSIU},
                {"PROFESOR", NombreCompleto},
                {"CAMPUS", Campus},
                {"PERIODO", Periodo},
                {"NIVEL",Nivel},
            };

            string token = Jose.JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);

            Response.Redirect(URL +"EstadodeCuentaWeb/?token=" + token);
            //  Response.Redirect("https://40.84.224.118/EstadodeCuentaWeb/?token=" + token);
        }
    }
}