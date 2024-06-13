using PagoProfesores.Models.Helper;
using Session;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Helper
{
    public class TiposTransferenciaController : Controller
    {
        // GET: TiposTransferencia
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string getTransferencia(string Transferencia = "")
        {
            TiposTransferenciaModel model = new TiposTransferenciaModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("<option></option>");
            foreach (KeyValuePair<string, string> pair in model.getTiposTransferencia())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }
    }
}