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
    public class OrigenController : Controller
    {
        // GET: Origen
        public ActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public string getOrigen(string Origen = "")
        {
            OrigenModel model = new OrigenModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in model.getOrigen())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }



    }
}