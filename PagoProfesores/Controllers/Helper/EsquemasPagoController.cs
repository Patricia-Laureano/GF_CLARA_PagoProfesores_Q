using PagoProfesores.Models.Helper;
using Session;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Helper
{
    public class EsquemasPagoController : Controller
    {
        // GET: EsquemasPago
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string ConsultarEsquema(string Periodo = "", string CampusVPDI = "", /*string Nivel = "",*/ string IdEsquema = "")
        {
            EsquemaModel model = new EsquemaModel();

            model.Periodo = Periodo;
            model.CampusVPDI = CampusVPDI;

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("<option></option>");

            if (Periodo == "" && CampusVPDI == "")
                foreach (KeyValuePair<string, string> pair in model.getEsquemaAll())
                    sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            else
            {
                string selected = "";
                foreach (KeyValuePair<string, string> pair in model.getEsquema())
                {
                    selected = (IdEsquema == pair.Key) ? "selected" : "";
                    sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                    selected = "";
                }
            }
            return sb.ToString();
        }
    }
}