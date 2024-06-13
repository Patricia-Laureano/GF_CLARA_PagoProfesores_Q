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
    public class TiposPagosController : Controller
    {

        // GET: TiposPagos
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string getTiposPago()
        {
            TiposdePagosModel model = new TiposdePagosModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("<option></option>");
            foreach (KeyValuePair<string, string> pair in model.getTiposPago())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }

        [HttpGet]
        public string getTiposPagoV()
        {
            TiposdePagosModel model = new TiposdePagosModel();

            StringBuilder sb = new StringBuilder();
            var variables = "";
            
            foreach (KeyValuePair<string, string> pair in model.getTiposPagoV())
                variables += pair.Value + ",";

            if (variables.Length > 0)
                variables = variables.Substring(0, variables.Length - 1);
            else variables = "";

            sb.Append(variables);

            return sb.ToString();
        }

    }
}