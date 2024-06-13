using PagoProfesores.Models.Helper;
using Session;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Helper
{
    public class ConceptosdePagoController : Controller
    {
        // GET: ConceptosdePago
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string getConceptosdePago(string CampusVPDI = "", string EsquemaID = "", string PersonaID = "", string Periodo = "", string Nivel = "")
        {
            ConceptosdePagoModel model = new ConceptosdePagoModel();

            model.CampusVPDI = CampusVPDI;
            model.EsquemaID = EsquemaID;
            model.PersonaID = PersonaID;
            model.Periodo = Periodo;

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            model.Usuario = sesion.pkUser.ToString();

            StringBuilder sb = new StringBuilder();
          //  string selected = "";
            sb.Append("<option value=\"\"></option>\n");
            foreach (KeyValuePair<string, string> pair in model.getConceptosdePago())
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");   

            return sb.ToString();
        }

        [HttpGet]
        public string getConceptosdePagoEsquema(string EsquemaID = "")
        {
            ConceptosdePagoModel model = new ConceptosdePagoModel();
            model.EsquemaID = EsquemaID;

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=\"\"></option>\n");
            foreach (KeyValuePair<string, string> pair in model.getConceptosdePago())
            {
                sb.Append("<option value=\"").Append(pair.Key).Append("\">").Append(pair.Value).Append("</option>\n");
            }
            return sb.ToString();
        }

        [HttpGet]
        public string getFechasdePagoEsquema(string EsquemaID = "", string FechaPago = "", string PersonaID = "")
        {
            ConceptosdePagoModel model = new ConceptosdePagoModel();
            model.EsquemaID = EsquemaID;
            model.PersonaID = PersonaID;

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            model.Usuario = sesion.pkUser.ToString();

            StringBuilder sb = new StringBuilder();
            string selected = "";
            sb.Append("<option value=\"\"></option>\n");
            foreach (KeyValuePair<string, string> pair in model.getFechasPago())
            {
                selected = (FechaPago == pair.Value) ? "selected" : "";
                sb.Append("<option value=\"").Append(pair.Key).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        [HttpGet]
        public string getFechaConceptoPago(string EsquemaID = "", string ConceptoPagoPk = "")
        {
            ConceptosdePagoModel model = new ConceptosdePagoModel();
            model.EsquemaID = EsquemaID;
            model.ConceptoPagoPk = ConceptoPagoPk;

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            model.Usuario = sesion.pkUser.ToString();

            string selected = "";
            selected = (model.getFechaConceptoPago() != "" ? model.getFechaConceptoPago() : selected);

            return selected;
        }
    }
}