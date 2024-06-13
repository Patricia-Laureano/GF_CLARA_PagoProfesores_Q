using System;
using System.Linq;
using System.Web.Mvc;
using Session;
using ConnectDB;
using PagoProfesores.Models.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace PagoProfesores.Controllers.Authentication
{
    public class LoginController : Controller
    {
        //passow  75nW7naeKubWjJnkcbT1fiH
        
        // GET: /Login/Start
        public ActionResult Start()
		{
            database db = new database();
            SessionDB sesion = SessionDB.start(Request, Response, true, db);
            db.Close();
            ViewBag.TOKEN = Guid.NewGuid().ToString();
            // Si ya existe una sesion se redirecciona al Dashboard ...
            if (sesion != null)
            {
                if (sesion.tipouser == 'U')
                    return RedirectToAction("Start", "Dashboard");
                else
                    return RedirectToAction("Home", "EstadodeCuentaWeb");
            }

            return View();
        }

		// GET: /Login/Close/
		public ActionResult Close()
        {
			database db = new database();
			SessionDB sesion = SessionDB.start(Request, Response, false, db);

            if (sesion != null)
                sesion.close();
            
            return View();
        }
        // POST: /Login/Validate
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Validate(FormCollection collection)
        {
            ViewBag.TOKEN = Guid.NewGuid().ToString();
            LoginModel model = new LoginModel();
			ViewBag.Mensaje = "";
			ViewBag.Usuario = "";
			try
			{
                string client_user = JavaScriptEscape(Request.Params["usuario"]);
                string client_password = JavaScriptEscape(Request.Params["password"]);
				if (client_user != "")
				{
					ViewBag.Usuario = client_user;
					if (model.existeUsuario(client_user))
					{
                        if(Validate_Usuario_Admin(client_user, client_password,model))
                           return RedirectToAction("Index", "Dashboard");

                        if (Validate_Usuario_Ext(client_user, client_password, model))
                            return RedirectToAction("Home", "EstadodeCuentaWeb");
                    }
					else
						ViewBag.Mensaje = "El usuario no se ha encontrado";
				}
				else
					ViewBag.Mensaje = "Por favor, ingrese un usuario";
			}
			catch (Exception e)
			{
				ViewBag.Mensaje = "Por favor, revise el usuario y password";
			}
			//return View("Start");
			return View();
		}
        
        public bool Validate_Usuario_Admin(string client_user, string client_password, LoginModel model)
        {
            bool _valido = false;

            if (client_password != "")
            {

                string server_password = model.consultaPassword(client_user);
                string password = EncryptX.Decode(server_password);

                if (EncryptX.check(server_password, client_password))
                {
                    database db = new database();
                    SessionDB.afterLogIn(client_user, db, Request, Response);
                    SessionDB sesion = SessionDB.start(Request, Response, false, db);

                    //Log.write(this, "Validate", LOG.CONSULTA, "Detalle x", sesion);

                    _valido = true;
                }
                else
                    ViewBag.Mensaje = "Su password es incorrecto";
            }
            else
                ViewBag.Mensaje = "Ups, falta escribir su password";

            return _valido;

        }


        public bool Validate_Usuario_Ext(string client_user, string client_password, LoginModel model)
        {
            bool _valido = false;
            char tipouser = 'P';

            if (client_password != "")
            {

                if (model.consulta_Externo(client_user))
                {
                   tipouser = model.CVE_ORIGEN;

                      if(model.DATOS_FISCALES == "F")
                    {
                        if(model.RFC == client_password) {
                            database db = new database();
                            SessionDB.afterLogIn(client_user, db, Request, Response, tipouser);
                            SessionDB sesion = SessionDB.start(Request, Response, false, db);
                            _valido = true;
                        } else { ViewBag.Mensaje = "Su password es incorrecto"; }

                    }
                    else
                    {
                        if (model.RZ_RFC == client_password) {

                            database db = new database();
                            SessionDB.afterLogIn(client_user, db, Request, Response, tipouser);
                            SessionDB sesion = SessionDB.start(Request, Response, false, db);
                            _valido = true; }else { ViewBag.Mensaje = "Su password es incorrecto"; }
                    }
                }
            }
            else
                ViewBag.Mensaje = "Ups, falta escribir su password";

            return _valido;
        }

        // POST:   /Login/KeepSession/
        [HttpPost]
		public ActionResult KeepSession()
		{
			try
			{
				string session_val = EncryptX.Decode(Request.Params["session_val"]);
				if (SessionDB.KeepSession(session_val, new database(), Request, Response))
					return Content("ok");
			}
			catch { }
			return Content("-1");
		}

        string JavaScriptEscape(string text)
        {
            return text
                .Replace("'", @"''")
                .Replace("\\", @"\u005c")
                .Replace("\"", @"\u0022")
                .Replace("'", @"\u0027")
                .Replace("&", @"\u0026")
                .Replace("<", @"\u003c")
                .Replace(">", @"\u003e");
        }
    }// </>
}
