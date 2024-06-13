using ConnectDB;
using PagoProfesores.Models.Authentication;
using Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Authentication
{
    public class SSOffice365Controller : Controller
    {
        public SessionDB sesion;

        // GET: SSOffice365
        public ActionResult Start()
        {
            database db = new database();
            sesion = SessionDB.start(Request, Response, true, db);

            if (sesion != null)
            {
                if (sesion.tipouser == 'U')
                {

                    return RedirectToAction("Start", "Dashboard");
                }
                else
                {

                    return RedirectToAction("Home", "EstadodeCuentaWeb");
                }

            }
            else
            {
                return View();
            }
        }

        // GET: SSOffice365/Details/5
        public ActionResult VerifyToken()
        {
            database db = new database();
            sesion = SessionDB.start(Request, Response, true, db);

           string id_token = Request.QueryString["id_token"];
           var tokenString = "Bearer " + id_token;
           var jwtEncodedString = tokenString.Substring(7);

            try
            {
                // Si ya existe una sesion se redirecciona al Dashboard ...
                if (sesion != null)
                {
                    if (sesion.tipouser == 'U')
                    {

                        return RedirectToAction("Start", "Dashboard");
                    }
                    else
                    {
                        return RedirectToAction("Home", "EstadodeCuentaWeb");
                    }
                }
                else
                {
                    var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                    var correo = token.Claims.Where(c => c.Type == "unique_name");

                    var nuevo = correo.ToList();
                    var valor = nuevo[0].Value;
                    string client_user = valor;

                    db.Close();

                    LoginModel model = new LoginModel();

                    if (model.existeUsuario(client_user))
                    {
                        SessionDB.afterLogIn(client_user, db, Request, Response);
                        sesion = SessionDB.start(Request, Response, false, db);
                        //Log.write(this, "Validate", LOG.CONSULTA, "Detalle x", sesion);
                        return RedirectToAction("Start", "Dashboard");

                    }
                    else
                    {
                        //No cuenta con acceso como administrador al Sistema de Pago a Profesores
                        //Revisar si existe como profesor

                        if (model.existeProfesor(client_user))
                        {
                            string strm_Tag = ConfigurationManager.ConnectionStrings["GetToken"].ConnectionString;
                            string[] array_tag = strm_Tag.Split(';');
                            string Key = array_tag[0].Substring(4);
                            string URL = array_tag[1].Substring(5);

                            char tipouser = 'P';
                            string idsiu = "";

                            sesion =  SessionDB.afterLogIn(client_user, db, Request, Response, tipouser);
                            sesion = SessionDB.start(Request, Response, false, db);

                            string sql = "SELECT IDSIU FROM PERSONAS WHERE ID_PERSONA = '" + sesion.pkUser + "'";
                            ResultSet res = db.getTable(sql);
                            if (res.Next())
                            {
                                idsiu = res.Get("IDSIU");
                            }

                            string data = "Periodo=&Nivel=&IDSIU=" + idsiu + "&NombreCompleto=" + sesion.completeName + "&Campus=UAN";

                             Response.Redirect(URL + "Token/GenerarToken?" + data);

                        }
                        else
                        {

                            //No cuenta con acceso  al Sistema de Pago a Profesores

                             return RedirectToAction("AccesoDenegado", "SSOffice365");
                        }
                    }

                }
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
                // Response.Redirect("https://login.microsoftonline.com/common/oauth2/authorize?response_type=id_token&client_id=09fa3fa2-106e-41b2-91f4-75026bdaa9ee&redirect_uri=http%3A%2F%2Flocalhost%3A58402%2F&state=00c2faab-e87f-47be-8a4a-f61448a10ea4&client-request-id=cad66ef3-92fc-4316-8bcb-5af6b1154683&x-client-SKU=Js&x-client-Ver=1.0.12&nonce=1a39aacd-f7fc-43ab-97f1-f0286ecea418");

            }

            return null;
        }

        // GET: SSOffice365/Create
        public ActionResult AccesoDenegado()
        {
            return View();
        }

        
        
    }
}
