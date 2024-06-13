using PagoProfesores.Models.Helper;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Helper
{
    public class SedeSelectorController : Controller
    {
		//private SessionDB sesion;
		[HttpPost]
		public ActionResult Edit(SedeSelectorModel model)
		{
			if ((model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX)) == null) { return Content("-1"); }
			if (model.Edit())
			{
				return Json(new JavaScriptSerializer().Serialize(model));
			}
			return Content("-1");
		}
        
		// POST: Menus/Save
		[HttpPost]
		public ActionResult Save(SedeSelectorModel model)
		{
			try
			{
				if ((model.sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX)) == null) { return Content("-1"); }
				model.Save();
			}
			catch (Exception e) { }
			return Content("0");
		}
	}
}