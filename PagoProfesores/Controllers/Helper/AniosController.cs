using PagoProfesores.Models.Helper;
using Session;
using System.Text;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Helper
{
    public class AniosController : Controller
    {
        // GET: Ciclos
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string getAnios(string Ciclo = "")
        {
            AniosNominaModel model = new AniosNominaModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (string str in model.getAnios())
                sb.Append("<option value=\"").Append(str).Append("\">").Append(str).Append("</option>\n");

            return sb.ToString();
        }
    }
}