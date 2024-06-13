using ConnectDB;
using Factory;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Pagos
{
	public class EstadodeCuentaWebController : Controller
	{
		private SessionDB sesion;
		private database db;
		private List<Factory.Privileges> Privileges;

		public EstadodeCuentaWebController()
		{
			db = new database();
            
			Privileges = new List<Factory.Privileges> {
				 new Factory.Privileges { Permiso = 10005,  Element = "Controller" },
                 new Factory.Privileges { Permiso = 10006,  Element = "frm-estadodecuentaweb" },
            };
		}
        
        public void setDatosProfesor(SessionDB session, EstadodeCuentaWebModel model)
        {
            if (sesion.tipouser == 'P' || sesion.tipouser == 'E')
            {
                //sesion.vdata["ID_PERSONA"] = sesion.pkUser.ToString();
                if (sesion.vdata["HmeAclzn"] == "No")
                    sesion.vdata["Sede"] = model.GetSede(sesion.vdata["IDSIU"]);
                sesion.vdata["ID_PERSONA"] = model.GetIdPersona(sesion.vdata["IDSIU"], sesion.vdata["Sede"]).ToString();
                model.ID_PERSONA = sesion.vdata["ID_PERSONA"];
                //string sql = "SELECT IDSIU FROM PERSONAS WHERE ID_PERSONA = '" + model.ID_PERSONA + "'";

                //ResultSet res = db.getTable(sql);
                //if (res.Next())
                //    sesion.vdata["IDSIU"] = res.Get("IDSIU");

                //sql = "SELECT TOP 1 CVE_SEDE  FROM ESTADODECUENTA WHERE ID_PERSONA = " + sesion.pkUser + " ORDER BY FECHA_R";
                //sql = "SELECT TOP 1 CVE_SEDE  FROM ESTADODECUENTA WHERE ID_PERSONA = " + model.ID_PERSONA + " ORDER BY FECHA_R";
                //res = db.getTable(sql);
                //if (res.Next())
                //    sesion.vdata["Sede"] = res.Get("CVE_SEDE");
                //else
                //    sesion.vdata["Sede"] = "UAN";
            }

            model.IDSIU = sesion.vdata["IDSIU"];
            model.Sede = sesion.vdata["Sede"];
            model.ID_PERSONA = sesion.vdata["ID_PERSONA"];

            model.GetDatos();
        }

		public ActionResult Start()
		{
			try
			{
				string Token = this.Request.QueryString["token"];
				string json = Decode(Token);

				var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);
				string IDSIU = dict["IDSIU"].ToString();
				string CAMPUS = dict["CAMPUS"].ToString();
				// string[] result = dict.Select(kv => kv.Value.ToString()).ToArray();
				EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();

                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }
               
                sesion.vdata["IDSIU"] = IDSIU;
				sesion.vdata["Sede"] = CAMPUS;
                sesion.vdata["ID_PERSONA"] = model.GetIdPersona(sesion.vdata["IDSIU"], sesion.vdata["Sede"]).ToString();
                sesion.saveSession();

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                Response.Redirect("/EstadodeCuentaWeb/Home");

				return Content(""); //View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/Home.cshtml");
			}
			catch (Exception)
			{
                //return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/Start.cshtml");
                return RedirectToAction("Home", "EstadodeCuentaWeb");
            }
		}

		public string Decode(string token)
		{
			byte[] secretKey = Base64UrlDecode("93874938327481928349");
			string json = Jose.JWT.Decode(token, secretKey);

			return json;
		}

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
       
        public ActionResult PagosPendientes(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            try
            {
                Scripts.SCRIPTS = new string[]
                {
                    "js/Pagos/EstadodeCuentaWeb/PagosPendientes.js"
                };

                Main view = new Main();
                ViewBag.MainUser = this.CreateMenuInfoUser(sesion);
                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);


                this.setDatosProfesor(sesion, model);

                string filter_Sede = Request.Params["filter_Sede"];
                if (filter_Sede != "" && filter_Sede != null)
                {
                    sesion.vdata["Sede"] = Request.Params["filter_Sede"];
                    model.Sede = sesion.vdata["Sede"];
                }

                ViewBag.Profesor = model.Profesor;


                // Datos fiscales
             //   EstadodeCuentaWebModel model2 = new EstadodeCuentaWebModel();

                model.GetDatosFiscales();
                if (model.Email_Sociedad == "")
                    ViewBag.Email_Sociedad = "sana@anahuac.mx";
               else
                    ViewBag.Email_Sociedad = model.Email_Sociedad;


                /* if (sesion.tipouser == 'U')
                    ViewBag.SEDES = view.createSelectSedes("Sedes", sesion);                
                else  */
                ViewBag.SEDES = view.createSelectSedesWeb("Sedes", sesion, model.ID_PERSONA);
                ViewBag.IDSIU = model.IDSIU;

                sesion.vdata["CVE_TIPOFACTURA"] = model.CveTipoFactura;
                sesion.saveSession();

               


                //IMPRIME DATATABLE
                if (filter_Sede != "" && filter_Sede != null) { }
                else
                    this.getPagosPendientes(model);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Estado de cuenta Web" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/PagosPendientes.cshtml");
        }

        public void getPagosPendientes(EstadodeCuentaWebModel model)
        {
            ViewBag.Table_PagosPendientes = model.GetPagosPendientes();
        }

        public ActionResult PagosPendientes2(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
            {
                return Content("-1");
            }

            Main view = new Main();
            ViewBag.MainUser = this.CreateMenuInfoUser(sesion);
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            this.setDatosProfesor(sesion, model);

            sesion.vdata["Sede"] = model.Sede;
            sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
            ViewBag.IDSIU = model.IDSIU;

            //model.Sede = sesion.vdata["Sede"];
            //ViewBag.IDSIU = model.IDSIU;
            sesion.saveSession();

            return Json(new
            {
                pagosPendientes = model.GetPagosPendientes(),
            });
        }

        public ActionResult PagosDepositados()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            try
            {
                Main view = new Main();
                ViewBag.MainUser = this.CreateMenuInfoUser(sesion);

                Scripts.SCRIPTS = new string[]
                {
                    "js/Pagos/EstadodeCuentaWeb/PagosDepositados.js"
                };

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();

                this.setDatosProfesor(sesion, model);

                // Datos fiscales
                model.GetDatosFiscales();
                if (model.Email_Sociedad == "")
                    ViewBag.Email_Sociedad = "sana@anahuac.mx";
                else
                    ViewBag.Email_Sociedad = model.Email_Sociedad;

                // Datos persona
                // model.getDatos();
                ViewBag.Profesor =  model.Profesor;

                /*if (sesion.tipouser == 'U')
                    ViewBag.SEDES = view.createSelectSedes("Sedes", sesion);
                else*/
                ViewBag.SEDES = view.createSelectSedesWeb("Sedes", sesion, model.ID_PERSONA);

                sesion.saveSession();

                ViewBag.Table_PagosDepositados = model.GetPagosDepositados(Request);

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Estado de cuenta Web", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Estado de cuenta Web" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/PagosDepositados.cshtml");
        }
        
        public ActionResult _PagosDepositados(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
            {
                return Content("-1");
            }

            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            this.setDatosProfesor(sesion, model);
            sesion.vdata["Sede"] = model.Sede;
            sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
            ViewBag.IDSIU = model.IDSIU;
            //model.Sede = sesion.vdata["Sede"];
            //ViewBag.IDSIU = model.IDSIU;
            sesion.saveSession();
            
            return Json(new
            {
                pagosDepositados = model.GetPagosDepositados(Request),
            });
        }

        public ActionResult Contratos()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            try
            {
                Main view = new Main();
                ViewBag.MainUser = this.CreateMenuInfoUser(sesion);

                Scripts.SCRIPTS = new string[]
                {
                    "js/Pagos/EstadodeCuentaWeb/Contratos.js",
                };

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();
                this.setDatosProfesor(sesion, model);

                // Datos persona
                model.GetDatos();
                ViewBag.Profesor = model.Profesor;

                // Datos fiscales
                model.GetDatosFiscales();

                if (model.Email_Sociedad == "")
                    ViewBag.Email_Sociedad = "sana@anahuac.mx";
                else
                    ViewBag.Email_Sociedad = model.Email_Sociedad;

                /*if (sesion.tipouser == 'U')
                    ViewBag.SEDES = view.createSelectSedes("Sedes", sesion);
                else*/
                ViewBag.SEDES = view.createSelectSedesWeb("Sedes", sesion, model.ID_PERSONA);

                sesion.saveSession();

                ViewBag.Table_Contratos = model.GetContratos(Request);

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Estado de cuenta Web", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Estado de cuenta Web" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/Contratos.cshtml");
        }
        
        public ActionResult _Contratos(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
            {
                return Content("-1");
            }

            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            this.setDatosProfesor(sesion, model);
            //model.Sede = sesion.vdata["Sede"];
            //ViewBag.IDSIU = model.IDSIU;

            sesion.vdata["Sede"] = model.Sede;
            sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
            ViewBag.IDSIU = model.IDSIU;

            sesion.saveSession();

            return Json(new
            {
                Contratos = model.GetContratos(Request),
            });
        }

        public ActionResult ConstanciasRetencion()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            try
            {
                Main view = new Main();
                ViewBag.MainUser = this.CreateMenuInfoUser(sesion);
                Scripts.SCRIPTS = new string[]
             {
                 "js/Pagos/EstadodeCuentaWeb/ECW_RetencionesMensuales.js"
             };

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();

                this.setDatosProfesor(sesion, model);

                // Datos persona
                model.GetDatos();
                ViewBag.Profesor = model.Profesor;

                /* if (sesion.tipouser == 'U')
                     ViewBag.SEDES = view.createSelectSedes("Sedes", sesion);
                 else*/
                ViewBag.SEDES = view.createSelectSedesWeb("Sedes", sesion, model.ID_PERSONA);

                sesion.vdata["CVE_TIPOFACTURA"] = model.CveTipoFactura;
                sesion.saveSession();

                ViewBag.ConstanciasRetencionMensual = model.ConstanciasRetencionMensual(Request);

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Estado de cuenta Web", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Estado de cuenta Web" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/ConstanciasRetencion.cshtml");
        }

        public ActionResult _ConstanciasRetencionMensual(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
                return Content("-1");

            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            this.setDatosProfesor(sesion, model);

            sesion.vdata["Sede"] = model.Sede;
            sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
            ViewBag.IDSIU = model.IDSIU;
            sesion.saveSession();

            return Json(new
            {
                ConstanciasRetencionMensual = model.ConstanciasRetencionMensual(Request),
            });
        }
        
        public ActionResult ConstanciasRetencionAnual()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            try
            {
                Main view = new Main();
                ViewBag.MainUser = this.CreateMenuInfoUser(sesion);
                Scripts.SCRIPTS = new string[]
                {
                    "js/Pagos/EstadodeCuentaWeb/ECW_RetencionesAnuales.js"
                };

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();

                this.setDatosProfesor(sesion, model);

                // Datos persona
                model.GetDatos();
                ViewBag.Profesor = model.Profesor;
                ViewBag.SEDES = view.createSelectSedesWeb("Sedes", sesion, model.ID_PERSONA);

                sesion.vdata["CVE_TIPOFACTURA"] = model.CveTipoFactura;
                sesion.saveSession();
                
                ViewBag.ConstanciasRetencionAnual = model.ConstanciasRetencionAnual(Request);
                
                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Estado de cuenta Web", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Estado de cuenta Web" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/ConstanciasRetencionAnual.cshtml");
        }
        
        public ActionResult _ConstanciasRetencionAnual(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
                return Content("-1");

            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            this.setDatosProfesor(sesion, model);
            //model.Sede = sesion.vdata["Sede"];
            //ViewBag.IDSIU = model.IDSIU;
            sesion.vdata["Sede"] = model.Sede;
            sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
            ViewBag.IDSIU = model.IDSIU;
            sesion.saveSession();

            return Json(new
            {
                ConstanciasRetencionAnual = model.ConstanciasRetencionAnual(Request),
            });
        }
        
        private string CreateMenuInfoUser(SessionDB sesion)
        {
            string menu = new StringBuilder()
            .Append("<li class=\"dropdown navbar-ser\"><a href=\"javascript:; \" class=\"dropdown-toggle\" data-toggle=\"dropdown\"> ")
            .Append("<img src=\"/Content/images/user.png\" width=\"\" height=\"30px\"  /> <span class=\"hidden-xs\">")
            .Append("" + sesion.completeName)
            .Append("</span> <b class=\"caret\"></b></a>")
            .Append(" <ul class=\"dropdown-menu animated fadeInLeft\"> ")
            
            .Append("<li><a href=\"/Login/Close \">Cerrar Sesión</a></li> ")
            .Append("</ul></li> ")
            .Append("<input type=\"hidden\" id=\"session_val\" value=\"")
            .Append(EncryptX.Encode("" + sesion.pkUser + "," + sesion.idSesion)).Append("\"> ")
            .ToString();

            return menu;
        }

        public ActionResult CambiarSede(EstadodeCuentaWebModel model)
		{
			model.GetDatosFiscales();
			model.Dispose();

			return Json(new JavaScriptSerializer().Serialize(model));
		}

        public ActionResult Home()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }
            try
            {
                Main view = new Main();
                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();

                Scripts.SCRIPTS = new string[]
                {
                    "js/Pagos/EstadodeCuentaWeb/ECW_Contratos.js"
                };

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                sesion.vdata["HmeAclzn"] = "No";
                this.setDatosProfesor(sesion, model);

                // Datos persona
                model.GetDatos();
                sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
                ViewBag.Profesor = model.Profesor;
                ViewBag.IDSIU = model.IDSIU;
                ViewBag.NoCuenta = model.NoCuenta;
                ViewBag.CuentaClabe = model.CuentaClabe;
                ViewBag.Banco = model.Banco;
                ViewBag.RFC = model.RFC;

                // Datos fiscales
                model.GetDatosFiscales();
                ViewBag.Fis_Sede = model.Sede;
                ViewBag.Fis_RecibiDe = model.Fis_Recibide;
                ViewBag.Fis_RFC = model.Fis_RFC;
                ViewBag.Fis_Domicilio = model.Fis_Domicilio;
                ViewBag.Fis_Concepto = model.Fis_Concepto;

                ViewBag.Email_Sociedad = model.Email_Sociedad;

                ViewBag.SEDES = view.createSelectSedesWeb("Sedes", sesion, model.ID_PERSONA); 

                sesion.vdata["CVE_TIPOFACTURA"] = model.CveTipoFactura;
                sesion.saveSession();

                //Intercom
                ViewBag.User = sesion.nickName.ToString();
                ViewBag.Email = sesion.nickName.ToString();
                ViewBag.FechaReg = DateTime.Today;

                //cuenta
                if (!string.IsNullOrWhiteSpace(model.NoCuenta))
                {
                    if (model.VerificarCuenta())
                    {
                        ViewBag.casilla_valida = "<input type=\"checkbox\" id=\"valida_check\" checked disabled/>";
                        ViewBag.boton_validacuenta = "<button type = 'button' id = \"btn_valida\" class='btn btn-sm btn-success' onclick=\"validaCuenta();\" disabled>Verificado</button>";
                    }
                    else
                    {
                        ViewBag.casilla_valida = "<input type=\"checkbox\" id=\"valida_check\"/>";
                        ViewBag.boton_validacuenta = "<button type = 'button' id = \"btn_valida\" class='btn btn-sm btn-success' onclick=\"validaCuenta();\">Verificar</button>";
                    }
                }
                else
                {
                    ViewBag.casilla_valida = "<input type=\"checkbox\" id=\"valida_check\" disabled/>";
                    ViewBag.boton_validacuenta = "<button type = 'button' id = \"btn_valida\" class='btn btn-sm btn-success' onclick=\"validaCuenta();\" disabled>Verificar</button>";

                    // limpiar validacuenta** y poner en 0
                    model.validaCuenta_2();
                }

                this.getPagosDashboardPagos(model);

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Estado de cuenta Web", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Estado de cuenta Web" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/Home.cshtml");
        }
        
        public ActionResult Home_Actualitation(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
            {
                return Content("-1");
            }

            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            sesion.vdata["HmeAclzn"] = "Sí";
            this.setDatosProfesor(sesion, model);
            //model.Sede = sesion.vdata["Sede"];
            //model.ID_PERSONA = sesion.vdata["ID_PERSONA"];

            sesion.vdata["Sede"] = model.Sede;
            sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
            ViewBag.IDSIU = model.IDSIU;
            sesion.saveSession();

            //cuenta
            string casilla_valida = "";
            string boton_validacuenta = "";
            if (!string.IsNullOrWhiteSpace(model.NoCuenta))
            {

                if (model.VerificarCuenta())
                {
                    casilla_valida = "<input type=\"checkbox\" id=\"valida_check\" checked disabled/>";
                    boton_validacuenta = "<button type = 'button' id = \"btn_valida\" class='btn btn-sm btn-success' onclick=\"validaCuenta();\" disabled>Verificado</button>";
                }
                else
                {
                    casilla_valida = "<input type=\"checkbox\" id=\"valida_check\"/>";
                    boton_validacuenta = "<button type = 'button' id = \"btn_valida\" class='btn btn-sm btn-success' onclick=\"validaCuenta();\">Verificar</button>";
                }
            }
            else
            {
                casilla_valida = "<input type=\"checkbox\" id=\"valida_check\" disabled/>";
                boton_validacuenta = "<button type = 'button' id = \"btn_valida\" class='btn btn-sm btn-success' onclick=\"validaCuenta();\" disabled>Verificar</button>";
                // limpiar validacuenta** y poner en 0
                model.validaCuenta_2();
            }

            return Json(new
            {
                PagosPendientes = model.GetPagosDashboardPendientes(),
                PagosDepositados = model.GetPagosDashboardDepositados(),
                Contratos = model.GetDashboardContratos(),
                PagosxDepositar = model.GetPagosxDepositar(),
                Bloqueos = model.GetBloqueosDashboard(),
                Comunicados = model.GetComunicados(),
                jcasilla_valida = casilla_valida,
                jboton_validacuenta = boton_validacuenta
            });
        }

        public ActionResult Perfil()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            try
            {
                Main view = new Main();
                Scripts.SCRIPTS = new string[]
                {
                    "js/Pagos/EstadodeCuentaWeb/perfil.js"
                };

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);
                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();
                this.setDatosProfesor(sesion, model);

                ViewBag.Profesor = model.Profesor;
                ViewBag.NoCuenta = model.NoCuenta;
                ViewBag.CuentaClabe1 = model.CuentaClabe;
                ViewBag.Banco = model.Banco;
                ViewBag.RFC = model.RFC;
                ViewBag.Direccion = model.Direccion;

                // Datos fiscales
                model.GetDatosFiscales();
                ViewBag.Fis_Sede = model.Sede;
                ViewBag.Fis_RecibiDe = model.Fis_Recibide;
                ViewBag.Fis_RFC = model.Fis_RFC;
                ViewBag.Fis_Domicilio = model.Fis_Domicilio;
                ViewBag.Fis_Concepto = model.Fis_Concepto;

                if (model.Email_Sociedad == "")
                    ViewBag.Email_Sociedad = "sana@anahuac.mx";
                else
                    ViewBag.Email_Sociedad = model.Email_Sociedad;
               

                ViewBag.IDSIU = model.IDSIU;
                ViewBag.SEDES = view.createSelectSedesWeb("Sedes", sesion, model.ID_PERSONA);
                sesion.vdata["CVE_TIPOFACTURA"] = model.CveTipoFactura;
                sesion.saveSession();

                ViewBag.PENSIONES = model.GetPensiones();
                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Estado de cuenta Web", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Estado de cuenta Web" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/Perfil.cshtml");
        }

        public ActionResult Perfil2_(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
                return Content("-1");

            this.setDatosProfesor(sesion, model);

            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            model.Sede = sesion.vdata["Sede"];

            model.GetDatos();
            model.GetDatosFiscales();

            sesion.vdata["CVE_TIPOFACTURA"] = model.CveTipoFactura;
                sesion.saveSession();
            
            return Json(new
            {
                Profesor = model.Profesor,              
                NoCuenta = model.NoCuenta,
                Banco = model.Banco,
                RFC = model.RFC,
                Direccion = model.Direccion,
                Fis_Sede = model.Sede,
                Fis_RecibiDe = model.Fis_Recibide,
                Fis_RFC = model.Fis_RFC,
                Fis_Domicilio = model.Fis_Domicilio,
                Fis_Concepto = model.Fis_Concepto,
                IDSIU = model.IDSIU,
                pensiones = model.GetPensiones(),
                CuentaClabe = model.CuentaClabe,
                Email_Sociedad = model.Email_Sociedad,
            });
        }

        public void getPagosDashboardPagos(EstadodeCuentaWebModel model)
        {
            ViewBag.PagosPendientes =  model.GetPagosDashboardPendientes();
            ViewBag.PagosDepositados = model.GetPagosDashboardDepositados();
            ViewBag.Contratos = model.GetDashboardContratos();
            ViewBag.PagosxDepositar = model.GetPagosxDepositar();
            ViewBag.Bloqueos = model.GetBloqueosDashboard();
            ViewBag.Comunicados = model.GetComunicados();
        }

        public ActionResult DetallePago()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) return Content("-1");

            try
            {
                Main view = new Main();
				
				Scripts.SCRIPTS = new string[]
                {
                    "plugins/Angular/jquery.ui.widget.js"
                    , "plugins/Angular/jquery.iframe-transport.js"
                    , "plugins/Angular/jquery.fileupload.js"
                    , "js/Pagos/EstadodeCuenta/EstadodeCuenta.js"
                    , "js/Pagos/EstadodeCuentaWeb/ECW_EstadoCuenta.js"
                    , "js/Pagos/EstadodeCuentaWeb/ECW_RetencionesMensuales.js"
                    , "js/Pagos/EstadodeCuentaWeb/ECW_RetencionesAnuales.js"
                    , "js/Pagos/EstadodeCuentaWeb/EstadodeCuentaWeb.js"
                };
				
                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();
				model.sesion = sesion;		  

                this.setDatosProfesor(sesion, model);
                
                ViewBag.Profesor = model.Profesor;
                ViewBag.NoCuenta = model.NoCuenta;
                ViewBag.CuentaClabe = model.CuentaClabe;
                ViewBag.Banco = model.Banco;
                ViewBag.RFC = model.RFC;
                ViewBag.Direccion = model.Direccion;

                // Datos fiscales
                model.GetDatosFiscales();
                ViewBag.Fis_Sede = model.Sede;
                ViewBag.Fis_RecibiDe = model.Fis_Recibide;
                ViewBag.Fis_RFC = model.Fis_RFC;
                ViewBag.Fis_Domicilio = model.Fis_Domicilio;
                ViewBag.Fis_Concepto = model.Fis_Concepto;

                if (model.Email_Sociedad == "")
                    ViewBag.Email_Sociedad = "sana@anahuac.mx";
                else
                    ViewBag.Email_Sociedad = model.Email_Sociedad;


                ViewBag.SEDES = view.createSelectSedesWeb("Sedes", sesion, model.ID_PERSONA, true);
                ViewBag.IDSIU = model.IDSIU;

                sesion.vdata["CVE_TIPOFACTURA"] = model.CveTipoFactura;
                sesion.saveSession();

                model.ID_ESTADODECUENTA = Convert.ToInt32(Request.QueryString["_idestadocuenta"]);

                ViewBag.ESTADO = model.GetEstadoPago();
                ViewBag.ESTADODECUENTA = model.GetEstadodeCuenta();
                ViewBag.BLOQUEOS = model.GetBloqueos();
                ViewBag.PENSIONES = model.GetPensiones();

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Estado de cuenta Web", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Detalle Pago" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/DetallePago.cshtml");
        }

        public ActionResult DetallePago_(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
                return Content("-1");

            //model.sesion = sesion;
            //this.setDatosProfesor(sesion, model);
            //sesion.saveSession();
            //model.ID_ESTADODECUENTA = Convert.ToInt32(Request.QueryString["_idestadocuenta"]);
            //sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            //model.Sede = sesion.vdata["Sede"];
            //ViewBag.IDSIU = model.IDSIU;
            //sesion.saveSession();

            model.sesion = sesion;
            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            this.setDatosProfesor(sesion, model);

            sesion.vdata["Sede"] = model.Sede;
            sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
            ViewBag.IDSIU = model.IDSIU;

            //sesion.saveSession();
            model.ID_ESTADODECUENTA = Convert.ToInt32(Request.QueryString["_idestadocuenta"]);
            sesion.saveSession();

            return Json(new
            {
                estadodeCuenta = model.GetEstadodeCuenta(),
            });
        }

        public ActionResult DetallePagoDepositado()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) return Content("-1");

            try
            {
                Main view = new Main();
                ViewBag.MainUser = this.CreateMenuInfoUser(sesion);

                Scripts.SCRIPTS = new string[]
                {
                    "plugins/Angular/jquery.ui.widget.js"
                    , "plugins/Angular/jquery.iframe-transport.js"
                    , "plugins/Angular/jquery.fileupload.js"
                    , "js/Pagos/EstadodeCuenta/EstadodeCuenta.js"
                    , "js/Pagos/EstadodeCuentaWeb/ECW_EstadoCuenta.js"
                    , "js/Pagos/EstadodeCuentaWeb/ECW_RetencionesMensuales.js"
                    , "js/Pagos/EstadodeCuentaWeb/ECW_RetencionesAnuales.js"
                    , "js/Pagos/EstadodeCuentaWeb/EstadodeCuentaWeb.js"
                };

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();
                model.sesion = sesion;

                this.setDatosProfesor(sesion, model);

                ViewBag.Profesor = model.Profesor;
                ViewBag.NoCuenta = model.NoCuenta;
                ViewBag.CuentaClabe = model.CuentaClabe;
                ViewBag.Banco = model.Banco;
                ViewBag.RFC = model.RFC;
                ViewBag.Direccion = model.Direccion;

                // Datos fiscales
                model.GetDatosFiscales();
                ViewBag.Fis_Sede = model.Sede;
                ViewBag.Fis_RecibiDe = model.Fis_Recibide;
                ViewBag.Fis_RFC = model.Fis_RFC;
                ViewBag.Fis_Domicilio = model.Fis_Domicilio;
                ViewBag.Fis_Concepto = model.Fis_Concepto;

             
                if (model.Email_Sociedad == "")
                    ViewBag.Email_Sociedad = "sana@anahuac.mx";
                else
                    ViewBag.Email_Sociedad = model.Email_Sociedad;

                ViewBag.SEDES = view.createSelectSedesWeb("Sedes", sesion, model.ID_PERSONA, true);

                ViewBag.IDSIU = model.IDSIU;

                sesion.vdata["CVE_TIPOFACTURA"] = model.CveTipoFactura;
                sesion.saveSession();

                model.ID_ESTADODECUENTA = Convert.ToInt32(Request.QueryString["_idestadocuenta"]);

                ViewBag.ESTADODECUENTA = model.GetEstadodeCuentaDepositados();

                ViewBag.PENSIONES = model.GetPensiones();

                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Estado de cuenta Web", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Detalle Pago Deposito" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/DetallePagoDeposito.cshtml");
        }

        public ActionResult _DetallePagoDepositado(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
                return Content("-1");

            //model.sesion = sesion;
            //this.setDatosProfesor(sesion, model);
            //sesion.saveSession();
            //model.ID_ESTADODECUENTA = Convert.ToInt32(Request.QueryString["_idestadocuenta"]);

            //sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            //model.Sede = sesion.vdata["Sede"];
            //ViewBag.IDSIU = model.IDSIU;

            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            model.sesion = sesion;
            this.setDatosProfesor(sesion, model);
            sesion.vdata["Sede"] = model.Sede;
            sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
            ViewBag.IDSIU = model.IDSIU;
            model.ID_ESTADODECUENTA = Convert.ToInt32(Request.QueryString["_idestadocuenta"]);
            sesion.saveSession();
            //model.Sede = sesion.vdata["Sede"];
            //ViewBag.IDSIU = model.IDSIU;

            //sesion.saveSession();

            return Json(new
            {
                estadodeCuenta = model.GetEstadodeCuenta(),
            });
        }

        public ActionResult Tutoriales()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) return Content("-1");

            try
            {
                Main view = new Main();
                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Estado de cuenta Web - Tutoriales", sesion);

                EstadodeCuentaWebModel model = new EstadodeCuentaWebModel();
                // Datos fiscales
                this.setDatosProfesor(sesion, model);
                model.GetDatosFiscales();

                if (model.Email_Sociedad == "")
                    ViewBag.Email_Sociedad = "sana@anahuac.mx";
                else
                    ViewBag.Email_Sociedad = model.Email_Sociedad;
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Estado de cuenta Web - Tutoriales" + e.Message, sesion);  //MODIFICAR LA REFERENCIA DE LA PAGINA A INGRESAR
            }
            return View(Factory.View.Access + "Pagos/EstadodeCuentaWeb/Tutoriales.cshtml");
        }

        [HttpPost]
        public ActionResult ValidaCuenta(EstadodeCuentaWebModel model)
        {
            if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null)
                return Content("-1");



            sesion.vdata["Sede"] = Request.Params["filter_Sede"];
            model.sesion = sesion;
            this.setDatosProfesor(sesion, model);
            sesion.vdata["Sede"] = model.Sede;
            sesion.vdata["ID_PERSONA"] = model.ID_PERSONA;
            ViewBag.IDSIU = model.IDSIU;
            // model.ID_ESTADODECUENTA = Convert.ToInt32(Request.QueryString["_idestadocuenta"]);
            sesion.saveSession();



            model.validaCuenta();




            return Json(new
            {
                estadodeCuenta = model.GetEstadodeCuenta(),
            });
        }
    }
}
