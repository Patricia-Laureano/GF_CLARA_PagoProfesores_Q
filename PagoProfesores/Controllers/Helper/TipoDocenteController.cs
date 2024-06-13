using PagoProfesores.Models.Helper;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Helper
{
    public class TipoDocenteController : Controller
    {
        // GET: TipoDocente
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string getTipoDocente()
        {
            TipoDocenteModel model = new TipoDocenteModel();

            //SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            //if (sesion == null)
            //    return "";

            //if (sesion == null) { sesion = SessionDB.start(Request, Response, false, model.db); }

            StringBuilder sb = new StringBuilder();
            var variables = "";
            
            foreach (KeyValuePair<string, string> pair in model.getTipoDocente())
            {
                variables += pair.Value + ",";
            }

            if (variables.Length > 0)
                variables = variables.Substring(0, variables.Length - 1);
            else variables = "";
            sb.Append(variables);

            return sb.ToString();
        }
    }
}