using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectDB;
using System.Data.SqlClient;
using System.Web;

namespace Session
{
	public enum SESSION_BEHAVIOR
	{
		URL,
		AJAX
	};

	public class SessionDB
	{
		public long PK1;
		public readonly long idSesion;
		public readonly long pkUser;
		public string nickName;
		public string completeName;
        public char tipouser;
		//public string role;
		private DateTime accessTime;
		public Dictionary<string, string> vdata; // Usado para guardar datos varios
		public bool active;
		public int idMenu;
		public int idSubMenu;
		public string servlet;
		public Privileges permisos;
		public HttpRequestBase request;
		public HttpResponseBase response;

		public static readonly int limit_day = 1;
		public static readonly int limit_hour = 0;
		public static readonly int limit_minute = 1;

		public database db;

		// TODO  Constructor auxiliar
		public SessionDB(long pkUser, database db, HttpRequestBase request, HttpResponseBase response,char tipouser)
		{
			this.PK1 = 0L;
			this.idSesion = SessionDB.buildSessionId();
			this.pkUser = pkUser;
            this.tipouser = tipouser;

			// Datos de entorno
			this.idMenu = 1; // Por default : el Dashboard
			this.idSubMenu = 0;
			this.active = false;
			this.db = db;
			this.request = request;
			this.response = response;
			this.vdata = new Dictionary<string, string>();
		}//<end>
		/*
		// TODO  Constructor por consulta.
		public SessionDB(SqlDataReader res, database db, HttpRequestBase request, HttpResponseBase response)
		{
			this.PK1 = res.GetInt64(res.GetOrdinal("PK1"));
			this.idSesion = res.GetInt64(res.GetOrdinal("ID_SESION"));
			this.pkUser = res.GetInt64(res.GetOrdinal("PK_USUARIO"));
			this.accessTime = res.GetDateTime(res.GetOrdinal("FECHA_R"));
			this.idMenu = res.GetInt32(res.GetOrdinal("ID_MENU"));
			this.idSubMenu = res.GetInt32(res.GetOrdinal("ID_SUBMENU"));
			this.active = true;

			char[] separator = { '~' };
			this.vdata = res.GetString(res.GetOrdinal("VDATA")).Split(separator);
			// _______________________
			this.nickName = res.GetString(res.GetOrdinal("NICKNAME"));
			this.completeName = res.GetString(res.GetOrdinal("NOMBRE_COMPLETO"));
			//this.role = res.GetString(res.GetOrdinal("ROLE"));
			this.db = db;
			this.request = request;
			this.response = response;
			res.Close();
		}//<end>
		//*/
		// TODO  Constructor por consulta.
		public SessionDB(ResultSet res, database db, HttpRequestBase request, HttpResponseBase response)
		{
			this.PK1 = res.GetLong("PK1");
			this.idSesion = res.GetLong("ID_SESION");
			this.pkUser = res.GetLong("PK_USUARIO");
			this.accessTime = res.GetDateTime("FECHA_R");
			this.idMenu = res.GetInt("ID_MENU");
			this.idSubMenu = res.GetInt("ID_SUBMENU");
            this.tipouser = res.GetChar("TIPOUSUARIO");

			this.active = true;

			this.vdata = new Dictionary<string, string>();

			string[] array = res.Get("VDATA").Split(new string[] { "];[" }, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] dat = array[i].Split(new string[] { "[", ", ", "]" }, StringSplitOptions.RemoveEmptyEntries);
				if (2 <= dat.Length)
					vdata[dat[0]] = dat[1];
			}
			// _______________________
			this.nickName = res.Get("NICKNAME");
			this.completeName = res.Get("NOMBRE_COMPLETO");
			this.db = db;
			this.request = request;
			this.response = response;
		}//<end>

		public void set(SessionDB sesion)
		{
			this.PK1 = sesion.PK1;
			this.accessTime = sesion.accessTime;
			this.idMenu = sesion.idMenu;
			this.idSubMenu = sesion.idSubMenu;
			this.servlet = sesion.servlet;
			this.nickName = sesion.nickName;
			this.completeName = sesion.completeName;
            this.tipouser = sesion.tipouser;
			//this.role = sesion.role;
			this.active = sesion.active;

			this.vdata = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> pair in sesion.vdata)
				vdata.Add(pair.Key, pair.Value);

			this.db = (this.db != null) ? this.db : sesion.db;
			this.request = sesion.request;
			this.response = sesion.response;
		}//<end>

		// TODO  guardaSesion
		public void saveSession()
		{
			// Primero se consulta si ya hay una sesion previa
			SessionDB otraSesion = SessionDB.getPreviusSession(pkUser, idSesion, db, this.request, this.response);
			if (otraSesion == null)
			{
				// Si NO hay sesion se inserta una nueva en la BD.
				this.insertNewSession();
				otraSesion = SessionDB.getPreviusSession(pkUser, idSesion, db, this.request, this.response);
				if (otraSesion != null)
					// Se reasignan los datos de sesion debido a que hay valores automaticos en la BD.
					this.set(otraSesion);
			}
			else
			{
				this.updateSession();
			}
		}//<end>

		//
		// TODO  insertaNuevaSesion
		//
		private int insertNewSession()
		{
            string sql = new StringBuilder()
                .Append("INSERT INTO SESION")
                .Append(" (ID_SESION,PK_USUARIO,ID_MENU, ID_SUBMENU, VDATA,TIPOUSUARIO)")
                .Append(" VALUES (").Append(idSesion)
                .Append(", ").Append(pkUser)
                .Append(", ").Append(idMenu)
                .Append(", ").Append(idSubMenu)
                .Append(", '").Append(string.Join<KeyValuePair<string, string>>(";", vdata)).Append("'")
                .Append(", '").Append(tipouser).Append("'")
                .Append(")").ToString();
			return db.execute(sql) ? 1 : 0;
		}//<end>

		// TODO  actualizaSesion
		public void updateSession()
		{
			string sql = new StringBuilder()
				.Append("UPDATE SESION SET")
				.Append("  ID_MENU=").Append(this.idMenu)
				.Append(", ID_SUBMENU=").Append(this.idSubMenu)
                //.Append(", VDATA='").Append(string.Join<KeyValuePair<string, string>>(";", vdata)).Append("'")
                .Append(", VDATA='").Append(string.Join<KeyValuePair<string, string>>(";", vdata).Replace("'", "''")).Append("'")
                .Append(", FECHA_M=GETDATE()")

				.Append(" WHERE (PK1=").Append(this.PK1)
				.Append(" AND PK_USUARIO=").Append(this.pkUser)
				.Append(" AND ID_SESION=").Append(this.idSesion)
				.Append(")")
				.ToString();

			db.execute(sql);
		}//<end>

		public bool isFinished()
		{
			DateTime now = DateTime.Now;

			TimeSpan transcurrido = now - this.accessTime;
			TimeSpan limite = new TimeSpan(limit_day, limit_hour, limit_minute, 0);

			// Si el tiempo transcurrido es mayor que el limite 
			return (transcurrido > limite);
		}//<end>
		 /*
		 public bool checkParameter(string args_extra)
		 {
			 if (args_extra != null)
			 {
				 char[] separator = { '|' };
				 string[] array_args = args_extra.Split(separator);
				 if (3 <= array_args.Length)
					 try
					 {
						 this.servlet = array_args[0];
						 this.idMenu = int.Parse(array_args[1]);
						 this.idSubMenu = int.Parse(array_args[2]);
						 return true;
					 }
					 catch (Exception) { }
			 }
			 return false;
		 }
		 */
		public void close()
		{

        
            if (request.Cookies["sesiondata"] != null)
            {
                HttpCookie myCookie = new HttpCookie("sesiondata");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                response.Cookies.Add(myCookie);


               // response.Cookies.Remove("sesiondata");
            }

            SessionDB.clearSessions(pkUser, db);
		}

		public override string ToString()
		{
			return "sesion{act=" + active + ", usr=" + pkUser + ", data:\"" + string.Join<KeyValuePair<string, string>>(";", vdata) + "\""
					+ ", menu=[" + this.idMenu + "," + this.idSubMenu + "]}";
		}



		// _______________________________________________________________
		//
		//                       Funciones_estaticas
		// _______________________________________________________________

		public static SessionDB afterLogIn(string nickname, database db, HttpRequestBase request, HttpResponseBase response,char tipouser='U')
		{
			// Se consulta el id de usuario apartir del nombre.

			long pkUser = getpkUser(nickname, db,tipouser);

           // pkUser = 510;



            if (pkUser != -1)
			{

                 if (tipouser == 'B')tipouser = 'P';

                SessionDB sesion = SessionDB.createNewSession(pkUser, db, request, response, tipouser);

				createCookie(sesion, response);

				return sesion;
			}
			return null;
		}//<end>

		private static void createCookie(SessionDB sesion, HttpResponseBase response)
		{
			HttpCookie myCookie = new HttpCookie("sesiondata", "" + sesion.PK1 + "," + sesion.idSesion + "," + sesion.pkUser);
			//DateTime now = DateTime.Now;

            // Set the cookie expiration date.
            /*myCookie.Expires = now
				.AddDays(limit_day)
				.AddHours(limit_hour)
				.AddMinutes(limit_minute);*/

            myCookie.Expires = DateTime.Now.AddMinutes(720);

            // Add the cookie.
            response.Cookies.Add(myCookie);
		}

		public static bool KeepSession(string session_val, database db, HttpRequestBase request, HttpResponseBase response)
		{
			try
			{
				HttpCookie cookie = request.Cookies["sesiondata"];

				string[] array = session_val.Split(new char[] { ',' });
				long PK_USUARIO = long.Parse(array[0]);
				long ID_SESION = long.Parse(array[1]);

				SessionDB sesion = SessionDB.getPreviusSession(PK_USUARIO, ID_SESION, db, request, response);

				// Si existe la sesion en la BD, pero no la cookie
				if (sesion != null && cookie == null)
				{
					createCookie(sesion, response);

					string sql = "UPDATE SESION SET FECHA_R=GETDATE(), FECHA_M=GETDATE() WHERE PK1=" + sesion.PK1;
					return db.execute(sql);
				}
			}
			catch (Exception) { }
			return false;
		}

		/*
		public static void validateLogin()
		{
			// Se busca la cookie de inicio de sesion.
			HttpCookie cookie = request.Cookies["sesiondata"];

			if (cookie != null)
			{
				string datosdesesion = cookie.Value;
				char[] separator = { ',' };
				string[] array = datosdesesion.Split(separator);
				if (array != null && 3 <= array.Length)
				{
					// Hasta aqui ya se obtubo la coookie con los datos de sesion
					long pkUser = long.Parse(array[2]);

				}
			}
		}
		//*/

		// start
		public static SessionDB start(HttpRequestBase request, HttpResponseBase response, bool login, database db, SESSION_BEHAVIOR behavior = SESSION_BEHAVIOR.URL)
		{
			// Se busca la cookie de inicio de sesion.
			HttpCookie cookie = request.Cookies["sesiondata"];
            

            if (cookie != null)
			{
				string datosdesesion = cookie.Value;
				char[] separator = { ',' };
				string[] array = datosdesesion.Split(separator);
				if (array != null && 3 <= array.Length)
				{
					// Hasta aqui ya se obtubo la coookie con los datos de sesion
					long pkUser = long.Parse(array[2]);
					long idSesion = long.Parse(array[1]);

                    SessionDB sesion = SessionDB.getPreviusSession(pkUser, idSesion, db, request, response);

					// Si ya habia una sesion previa
					if (sesion != null)
					{
						// Se checa si la sesion NO ha caducado
						if (sesion.isFinished() == false)
						{
							sesion.permisos = new Privileges(sesion.pkUser, sesion.db);
							sesion.permisos.loadPermissions();
							return sesion;
						}
					}
				}
			}

			if (login == false)
			{
				if (behavior == SESSION_BEHAVIOR.AJAX)
				{
					response.Write("<session_error>");
					response.Flush();
					response.End();
					return null;
				}
                // En este punto no se encontro ningun dato para rescatar la sesion
                
                // Se solicita el servlet con los parametros
                string servletToInvoke = "~/";// Session.getServletToInvoke(request, "Login.do", model.db);
                if (cookie == null)
                {
                    response.Redirect(servletToInvoke);
                }
             
			}
			return null;
		}//<end>

		// TODO  creaNuevaSesion
		public static SessionDB createNewSession(long pkUser, database db, HttpRequestBase request, HttpResponseBase response,char tipouser)
		{
			//SessionDB.clearSessions(pkUser, db);

			SessionDB sesion = new SessionDB(pkUser, db, request, response,tipouser);

			// Al insertar nueva sesion se crea automaticamente el id.
			sesion.insertNewSession();

			return SessionDB.getPreviusSession(pkUser, sesion.idSesion, db, request, response);
		}//<end>

		public static long getpkUser(string nickname, database db,char tipouser)
		{
			long pkUser = -1;
            string sql = "";
            if (tipouser == 'U')
            {
                sql = "SELECT PK1,CONCAT(NOMBRE,' ',APATERNO,' ',AMATERNO) AS 'NOMBRE_COMPLETO' FROM USUARIOS WHERE USUARIO='" + nickname + "'";
            }
            else if(tipouser == 'P')
            {                
                sql = "SELECT P.ID_PERSONA AS PK1,CONCAT(NOMBRES,' ',APELLIDOS) AS 'NOMBRE_COMPLETO' FROM PERSONAS P INNER JOIN PERSONAS_SEDES PS ON PS.ID_PERSONA = P.ID_PERSONA WHERE CORREO365='" + nickname + "'";

            }
            else if (tipouser == 'E' || tipouser == 'B')
            {
                sql = "SELECT P.ID_PERSONA AS PK1,CONCAT(NOMBRES,' ',APELLIDOS) AS 'NOMBRE_COMPLETO' FROM PERSONAS P INNER JOIN PERSONAS_SEDES PS ON PS.ID_PERSONA = P.ID_PERSONA WHERE IDSIU = '" + nickname + "'";

            }

         /*  else if (tipouser == 'E')
            {
                sql = "SELECT ID_PERSONA AS PK1,CONCAT(NOMBRES,' ',APELLIDOS) AS 'NOMBRE_COMPLETO' FROM PERSONAS WHERE IDSIU = '" + nickname + "'";
            }*/



            ResultSet res = db.getTable(sql);
			if (res.Next())
				pkUser = res.GetLong("PK1");
			return pkUser;
		}//<end>
		 /*
		 public static string getCurrentServletName(HttpRequestBase request)
		 {
			 string servletInvoked = request.getServletPath();
			 int indexDiagonal = servletInvoked.lastIndexOf("/");
			 if (0 <= indexDiagonal)
				 servletInvoked = servletInvoked.substring(indexDiagonal + 1, servletInvoked.Length());
			 return servletInvoked;
		 }//<end>

		 private static ArrayList<string> consultarIdServlet(HttpRequestBase request, database db)
		 {
			 string servletInvoked = getCurrentServletName(request);

			 try
			 {
				 string sql = "SELECT [URL] AS 'SERVLET',[PADRE] AS 'MENU', [PK1] AS 'SUBMENU' from MENU WHERE URL='" + servletInvoked + "'";

				 SqlDataReader res = db.getDatos(sql);
				 if (res != null && res.next())
				 {
					 ArrayList<string> list = new ArrayList<string>();
					 list.add(res.getstring("SERVLET"));
					 list.add(res.getstring("MENU"));
					 list.add(res.getstring("SUBMENU"));
					 return list;
				 }
			 }
			 catch (SQLException ex) { }

			 return null;
		 }//<end>

		 public static string getServletToInvoke(HttpRequestBase request, string otherServlet, database db)
		 {
			 ArrayList<string> list = consultarIdServlet(request, db);
			 if (list != null && 3 <= list.size())
				 if (otherServlet == null)
					 return list.get(0) + "?args=" + list.get(0) + "," + list.get(1) + "," + list.get(2);
				 else
					 return otherServlet + "?args=" + list.get(0) + "," + list.get(1) + "," + list.get(2);
			 return null;
		 }//<end>
		 //*/
		public static SessionDB getPreviusSession(long pkUser, long idSesion, database db, HttpRequestBase request, HttpResponseBase response)
		{
            // SUPER CONSULTA

            char tipouser = 'U';

            string sqltypeuser = "SELECT TIPOUSUARIO FROM SESION WHERE PK_USUARIO = " + pkUser + " AND ID_SESION = " + idSesion;
            ResultSet res = db.getTable(sqltypeuser);
            if (res.Next())
            {
                tipouser = res.GetChar("TIPOUSUARIO");
            }


            StringBuilder sql = new StringBuilder();

            if (tipouser == 'U')
            {
                sql.Append("SELECT PK1,ID_SESION,PK_USUARIO,ID_MENU,ID_SUBMENU,FECHA_R,VDATA,TIPOUSUARIO")
                  .Append(",(SELECT TOP 1 CONCAT(NOMBRE,' ',APATERNO,' ',AMATERNO) FROM USUARIOS WHERE PK1=").Append(pkUser).Append(") AS 'NOMBRE_COMPLETO'")
                  .Append(",(SELECT USUARIO FROM USUARIOS WHERE PK1=").Append(pkUser).Append(") AS 'NICKNAME'")
                  //.Append(",(SELECT TOP 1 ROLE FROM ROLES_USUARIO RU, ROLES R WHERE RU.PK_ROLE = R.PK1 AND RU.PK_USUARIO=").Append(pkUser).Append(") AS 'ROLE'")
                  .Append(" FROM SESION WHERE PK_USUARIO=").Append(pkUser).Append(" AND ID_SESION=").Append(idSesion)
                  .ToString();
            }
            else {

                sql.Append("SELECT PK1,ID_SESION,PK_USUARIO,ID_MENU,ID_SUBMENU,FECHA_R,VDATA,TIPOUSUARIO")
                 .Append(",(SELECT TOP 1 CONCAT(NOMBRES,' ',APELLIDOS) FROM PERSONAS WHERE ID_PERSONA=").Append(pkUser).Append(") AS 'NOMBRE_COMPLETO'")
                 .Append(",(SELECT CORREO365 FROM PERSONAS WHERE ID_PERSONA=").Append(pkUser).Append(") AS 'NICKNAME'")
                 //.Append(",(SELECT TOP 1 ROLE FROM ROLES_USUARIO RU, ROLES R WHERE RU.PK_ROLE = R.PK1 AND RU.PK_USUARIO=").Append(pkUser).Append(") AS 'ROLE'")
                 .Append(" FROM SESION WHERE PK_USUARIO=").Append(pkUser).Append(" AND ID_SESION=").Append(idSesion)
                 .ToString();

            }

             res = db.getTable(sql.ToString());
			try
			{
				if (res.Next())
					return new SessionDB(res, db, request, response);
			}
			catch (Exception) { }
			return null;
		}//<end>

		public static int clearSessions(long pkUser, database db)
		{
			string sql = "DELETE FROM SESION WHERE PK_USUARIO = " + pkUser;
			return db.execute(sql) ? 1 : 0;
		}//<end>

		public static long buildSessionId()
		{
			return DateTime.Now.Ticks;
		}//<end>
		/*
		public static int limiteDeTiempo()
		{
			// convertimos a segundos
			return ((SessionDB.limit_day * 24 + SessionDB.limit_hour) * 60 + SessionDB.limit_minute) * 60;
		}//<end>
		//*/
	}//<end class>

}
