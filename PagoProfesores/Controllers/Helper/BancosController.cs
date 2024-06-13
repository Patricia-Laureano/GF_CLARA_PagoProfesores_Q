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
    public class BancosController : Controller
    {
        // GET: Bancos
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public string getBanco(string idBanco = "")
        {
            BancosModel model = new BancosModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "";
            //  sb.Append("<option></option>");
            foreach (KeyValuePair<string, string> pair in model.getBanco())
            {
                selected = (idBanco == pair.Value) ? "selected" : "";
              //  sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }




    }
}