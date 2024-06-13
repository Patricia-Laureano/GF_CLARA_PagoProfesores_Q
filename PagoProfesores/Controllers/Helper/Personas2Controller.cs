using PagoProfesores.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Helper
{
    public class Personas2Controller : Controller
    {
        // GET: Personas2
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getPersonas(PersonasModel model)
        {

            //SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            //if (sesion == null)
            //    return "";
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = model.ConsularPersonas();

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }


            return Json(new JavaScriptSerializer().Serialize(rows));
        }
    }
}