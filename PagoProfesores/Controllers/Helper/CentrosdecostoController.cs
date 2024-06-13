using PagoProfesores.Models.Helper;
using Session;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Helper
{
    public class CentrosdecostoController : Controller
    {
        // GET: Centrosdecosto
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string getCentrodecostos(string CampusVPDI = "", string EscuelaCVE = "", string TipoFactura = "", string TipoPagoCVE = "", string CVE_Programa = "")
        {
            CentroCostosModel model = new CentroCostosModel();

            model.CampusVPDI = CampusVPDI;
            model.EscuelaCVE = EscuelaCVE;
            model.CVE_Programa = CVE_Programa;
            
            if (TipoPagoCVE != "" && TipoPagoCVE != null)
                model.TipoPagoCVE = TipoPagoCVE;
            else
            {
                switch (TipoFactura)
                {
                    case "A":
                        model.TipoPagoCVE = "ADI";
                        break;
                    case "H":
                        model.TipoPagoCVE = "HDI";
                        break;
                    default:
                        model.TipoPagoCVE = "";
                        break;
                }
            }

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();

            sb.Append("<option></option>");
            foreach (KeyValuePair<string, string> pair in model.getCentrosdeCostos())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }

        [HttpGet]
        public string getCentrodecostosAll()
        {
            CentroCostosModel model = new CentroCostosModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("<option></option>");
            foreach (KeyValuePair<string, string> pair in model.getCentrosdeCostosAll())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }

    }
}