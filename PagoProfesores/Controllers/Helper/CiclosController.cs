using PagoProfesores.Models.Helper;
using Session;
using System.Text;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Helper
{
    public class CiclosController : Controller
    {
        // GET: Ciclos
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public string getCiclos(string Ciclo = "")
        {
            CiclosModel model = new CiclosModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";
 
            StringBuilder sb = new StringBuilder();
            sb.Append("<option></option>");
            foreach (string str in model.getCiclos())
            {
                sb.Append("<option value=\"").Append(str).Append("\">").Append(str).Append("</option>\n");
            }
            return sb.ToString();
        }
    }
}