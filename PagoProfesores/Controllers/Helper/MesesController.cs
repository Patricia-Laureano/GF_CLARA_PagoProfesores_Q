using ConnectDB;
using PagoProfesores.Models;
using PagoProfesores.Models.Helper;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Helper
{
    public class MesesController : Controller
    {

       
        // GET: Ciclos
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public string getMeses(string Anio = "")
        {
            MesesNominaModel model = new MesesNominaModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

         
            StringBuilder sb = new StringBuilder();
           // foreach (string str in model.getMeses())
                foreach (KeyValuePair<string, string> me in model.getMeses(Anio))
                {
                sb.Append("<option value=\"").Append(me.Key).Append("\">").Append(me.Value).Append("</option>\n");
            }
            sb.Append("<option value=\"\"></option>\n");
            return sb.ToString();
        }






    }
}