using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Configuration;

namespace ConnectUrlToken
{
    public class ConnectUrlToken
    {
        //private HttpClient client;


        //public string URL = "https://servicecloudappp.lcred.net:9099/wsProfesores/";
        public string URL = "https://uft-integ-prod.ec.lcred.net/wsProfesores/";
        public string User = "Banner";
		public string Secret = "8cTQKtn2UaqQLI9HhM28LowXlqI4ODbbjGSclCOGPvDYb9W/+zaFk0S4iY97SPHj";
		public string Format = "Banner:{0}";

		public Token token = null;
		public HttpStatusCode StatusCode;
		public string Content = "";

		public ConnectUrlToken() { }

		public ConnectUrlToken(string URL, string User, string Secret, string Format = "Banner:{0}")
		{
			this.URL = URL;
			this.User = User;
			this.Secret = Secret;
			this.Format = Format;
		}

		public void connect(string service, string string_json = "")
		{
			if ((token = getToken()) != null)
				connect(token, service, string_json);
		}


		public void connect(Token token, string service, string string_json = "")
		{
			try
			{
				if (token == null)
					return;
				HttpClient client = new HttpClient();
				client.DefaultRequestHeaders.Clear();

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
				HttpContent content = new StringContent(string_json, UnicodeEncoding.UTF8, "application/json");
				var authorizedResponse = client.PostAsync(URL + "api/" + service, content).Result;

				this.Content = authorizedResponse.Content.ReadAsStringAsync().Result;
				this.StatusCode = authorizedResponse.StatusCode;
			}
			catch (Exception) { }
		}

		public Type connectX<Type>(Token token, string service, string string_json = "")
		{
			if (token == null)
				throw new Exception("Token = null");
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Clear();

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
			HttpContent content = new StringContent(string_json, UnicodeEncoding.UTF8, "application/json");
			var authorizedResponse = client.PostAsync(URL + "api/" + service, content).Result;

			return authorizedResponse.Content.ReadAsAsync<Type>(new[] { new JsonMediaTypeFormatter() }).Result;
			//this.Content = authorizedResponse.Content.ReadAsStringAsync().Result;
			//this.StatusCode = authorizedResponse.StatusCode;
		}


		public Token getToken()
		{
			try
			{
				HttpClient client = new HttpClient();
				client.DefaultRequestHeaders.Clear();
				string authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(Format, Secret)));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
				Dictionary<string, string> form = new Dictionary<string, string>
				{
					{ "grant_type", "password"},
					{ "username", User}
				};

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                var tokenResponse = client.PostAsync(URL + "o/Server", new FormUrlEncodedContent(form)).Result;
				
				return tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;
			}
			catch (Exception ex) { Debug.WriteLine(ex.Message); }
			return null;
		}

	}

	public class Token
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }
	}

}
