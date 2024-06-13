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
    public class PensionadosHelperController : Controller
    {
        // GET: Pensionados
        public ActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public string getPensionados(string IdPersona = "")
        {
            PensionadosHelperModel model = new PensionadosHelperModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("<option></option>");
            foreach (KeyValuePair<string, string> pair in model.getPensionados(IdPersona))
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }
    }
}