using ConnectDB;
using PagoProfesores.Models.Helper;
using Session;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Helper
{
    public class DireccionesController : Controller
    {
        private database db;
       
        public DireccionesController()
        {
            db = new database();
        }
        
        // GET: Direcciones
        public ActionResult Index()
        {
            return View();
        }

        // GET: Direcciones/Paises
        public string Paises(string Pais = "")
        {
            DireccionesModel model = new DireccionesModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "";
            sb.Append("<option value=\"\">Seleccionar País..</option>");

            foreach (KeyValuePair<string, string> pair in model.getPaises())
            {
                selected = (pair.Value == Pais) ? "selected" : "";
                sb.Append("<option value=\"").Append(pair.Value).Append("\" ").Append(selected).Append(" >").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        // GET: Direcciones/Estados
        public string Estados(string Pais, string Estado)
        {
            DireccionesModel model = new DireccionesModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "";
            sb.Append("<option value=\"\">Seleccionar Estado..</option>");
            foreach (KeyValuePair<string, string> pair in model.getEstados(Pais))
            {
                selected = (pair.Value == Estado) ? "selected" : "";
                sb.Append("<option value=\"").Append(pair.Value).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }


        // GET: Direcciones/Ciudades
        public string Ciudades(string Pais, string Estado, string Ciudad, string Municipio)
        {
            DireccionesModel model = new DireccionesModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "";
            sb.Append("<option value=\"\">Seleccionar Ciudad..</option>");
            foreach (KeyValuePair<string, string> pair in model.getCiudades(Pais, Estado, Municipio))
            {
                selected = (pair.Value == Ciudad) ? "selected" : "";
                sb.Append("<option value=\"").Append(pair.Value).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }


        // GET: Direcciones/Municipios
        public string Municipios(string Pais, string Estado, string Ciudad, string Municipio)
        {
            DireccionesModel model = new DireccionesModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "";
            sb.Append("<option value=\"\">Seleccionar Municipio..</option>");
            foreach (KeyValuePair<string, string> pair in model.getMunicipios(Pais, Estado,Ciudad))
            {
                selected = (pair.Value == Municipio) ? "selected" : "";
                sb.Append("<option value=\"").Append(pair.Value).Append("\" ").Append(selected).Append(">").Append(pair.Value).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        // GET: Direcciones/Colonias
        public string Colonias(string Pais, string Estado, string Ciudad,string Municipio,string Colonia, string CP)
        {
            DireccionesModel model = new DireccionesModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "";
            sb.Append("<option value=\"\">Seleccionar Colonia..</option>");
            foreach (string coloniar in model.getColonias(Pais, Estado, Ciudad, Municipio, CP))
            {
                selected = (coloniar == Colonia) ? "selected" : "";
                sb.Append("<option value=\"").Append(coloniar).Append("\" ").Append(selected).Append(">").Append(coloniar).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }

        [HttpPost]
        public JsonResult getPaisEstadoCiudadMunicipio(DireccionesModel model)
        {
            if (model.getPaisEstadoCiudadMunicipio())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }

            return Json(new { data = "prueba" });
        }

        [HttpPost]
        public JsonResult getCP(DireccionesModel model)
        {
            if (model.getCP())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }

            return Json(new { data = "prueba" });
        }
    }
}
